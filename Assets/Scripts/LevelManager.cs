using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private Actor_Core _core = null;
    public Actor_Core Core { get { return _core; } }

    [SerializeField] private Actor_Player _player;
    public Actor_Player Player
    {
        get { return _player; }
    }

    private int _currentEnergy = 200;
    public int CurrentEnergy
    {
        get { return _currentEnergy; }
        set
        {
            _currentEnergy = value;
            // Call UI Update Here 
        }
    }

    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private float respawnTimer = 2.0f;
    private bool isRespawning = false;

    [SerializeField] private Camera spectatorCamera;
    [SerializeField] private GameObject EndGameMenu;
    [SerializeField] private TMP_Text EndGameText;

    [Header("Inventory System")]
    [SerializeField] private Item StartingItem;

    private List<Item> m_inventoryList = new List<Item>();
    public List<Item> InventoryList
    {
        get { return m_inventoryList; }
    }

    [SerializeField] private int inventoryLimit = 4; //defaulting inventory limit to 4

    public Action onItemChangeCallback; //delgate void for when an item is changed from inventory 

    // Dictionary to store all the item classes and bind them to a gameobject that gets spawned 
    // when an item is added to the inventory.
    private Dictionary<Item, IEquippable> m_equipables;
    public Dictionary<Item, IEquippable> Equipables => m_equipables;

    [SerializeField] ShopUI shopUI;
    [SerializeField] WeaponUI weaponUI;
    public WeaponUI WeaponUI { get { return weaponUI; } }
    [SerializeField] CharacterUI characterUI;
    public CharacterUI CharacterUI { get { return characterUI; } }

    private Item _currentlyEquipped;
    public Item CurrentlyEquipped { get { return _currentlyEquipped; } }

    protected override void Awake() //On Awake set check LevelManager's Instance and playerSpawnPoint
    {
        base.Awake();

        if (_player == null)
        {
            _player = FindObjectOfType<Actor_Player>();
        }

        m_equipables = new Dictionary<Item, IEquippable>();

        if (StartingItem != null)
        {
            InventoryAdd(StartingItem);
            StartingItem.Use();
        }



        #region PlayerSetupInitialization

        //if (_player == null)
        //    _player = FindObjectOfType<Actor_Player>();

        //if (playerSpawnPoint == null)
        //{
        //    //Respawn Player if there is no Spawn Point Found
        //    playerSpawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;

        //    if (playerSpawnPoint == null)
        //        Debug.LogError("No Spawn Point Found you cunt!");
        //}

        //if (GameManager.selectedPlayer != null)
        //{
        //    // Spawn Player from gameManager.
        //    _player = Instantiate(GameManager.selectedPlayer, playerSpawnPoint.transform.position, Quaternion.identity);            
        //}

        //else
        //{
        //    Debug.Log("No GameManager found, Searching for player here");


        //    if (_player == null)
        //        Debug.LogError("THere's no player in your scene you cunt! Are you even trying?");
        //}

        //_player.OnDeath += Respawn;

        #endregion
        Player.controlsEnabled = true;
        shopUI.pauseMenu.SetActive(false);

    }

    private void Start()
    {
        Core.OnDeath += () => GameOver(false);
        Player.OnDamageTaken += (DamageData) => Player._audioCue.PlayAudioCue(Player._cInfo.PlayerHit, 15);
        Time.timeScale = 1;
    }

    public void Update()
    {
        // Quick test will be removed in the future.
        if (Input.GetKeyDown(KeyCode.I) && !shopUI.pauseMenu.activeSelf)
        {
            ToggleShop();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !shopUI.gameObject.activeSelf && Core.CurrentHealth > 0)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        shopUI.pauseMenu.SetActive(!shopUI.pauseMenu.activeSelf);
        shopUI.combatHUD.SetActive(!shopUI.combatHUD.activeSelf);
        

        Time.timeScale = shopUI.pauseMenu.gameObject.activeSelf ? 0.0f : 1.0f;
        Cursor.lockState = shopUI.pauseMenu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = gameObject.activeSelf;

        Player.controlsEnabled = !shopUI.pauseMenu.activeSelf;
    }

    public void ToggleShop()
    {
        shopUI.ToggleMenu();
        shopUI.UpdateItemUI();
        shopUI.RefreshEnergyText();
        Player.EquipWeapon(Equipables[InventoryList[0]]);
        
        
    }

    #region GameLoop

    private void Respawn() // respawning character and Setting Spectator Camera
    {
        if (isRespawning) return;
        //activate Spectator Camera and set Player's gameObject to false
        spectatorCamera.gameObject.SetActive(true);
        Player.gameObject.SetActive(false);
        //start the player respawn
        StartCoroutine(RespawnTimer());
    }

    private IEnumerator RespawnTimer()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnTimer);

        _player.transform.position = playerSpawnPoint.position;
        _player.transform.rotation = playerSpawnPoint.rotation;

        spectatorCamera.gameObject.SetActive(false);
        Player.gameObject.SetActive(true);
        Player._audioCue.PlayAudioCue(Player._cInfo.PlayerRespawn);
        isRespawning = false;
    }

    public void GameOver(bool playerWon)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Player.controlsEnabled = false;

        EndGameMenu.SetActive(true);
        EndGameMenu.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero).OnComplete(() => Time.timeScale = 0.0f);
        EndGameText.text = playerWon ? "Victory!" : "Defeat!";
        if(playerWon)
            Player._audioCue.PlayAudioCue(Player._cInfo.MissionWin);
        if(!playerWon)
            Player._audioCue.PlayAudioCue(Player._cInfo.MissionLoss);


    }

    public void OnRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void OnQuitButton()
    {
        GameManager.Instance.ReturnToMenu();
    }

    #endregion

    #region InventoryFunc

    public void InventoryAdd(Item item) //function that adds an item from the Item class to the inventory 
    {
        //checks to see if the inventory count is less then the limit and checks if it dose not contain the item
        if (m_inventoryList.Count < inventoryLimit && !m_inventoryList.Contains(item))
        {
            m_inventoryList.Add(item);  //adds item to inventory 
            onItemChangeCallback?.Invoke(); //calling delgate function "onItemChangeCallback" "?" is !=null
            CurrentEnergy -= item.itemCost;
            // We instantiate the gameobject for this item as soon as we add it to the inventory.
            // Helps us create the object as we need it rather than creating them on start.
            // We only need to instantiate once and should not need to destroy them when we remove.
            if (!Equipables.ContainsKey(item))
            {
                GameObject temp = Instantiate(item.EquipableToSpawn) as GameObject;
                IEquippable test = temp.GetComponent<IEquippable>();

                if (test != null)
                    Equipables.Add(item, test);
            }
        }
    }

    public void InventoryRemove(Item item) //function that removes an item from the Item class to the inventory 
    {
        //checks to see if the inventory contains and item
        if (m_inventoryList.Contains(item))
        {
            if (Player.CurrentEquipped == Equipables[item])
                Player.CurrentEquipped.Unequip();

            m_inventoryList.Remove(item); // removes item from inventory 
            onItemChangeCallback?.Invoke(); //calling delgate function "onItemChangeCallback" "?" is !=null
        }
    }

    public void UseItem(Item itemToUse)
    {
        if (m_equipables.ContainsKey(itemToUse))
        {
            var newEquip = m_equipables[itemToUse];
            Player.EquipWeapon(newEquip);
        }
        _currentlyEquipped = itemToUse;
        weaponUI.UpdateEquippedWeapon();
    }

    #endregion
}
