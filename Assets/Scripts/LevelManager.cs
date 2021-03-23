using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random; 
using TMPro;
using System;
using DG.Tweening;
using UnityEngine.InputSystem;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private Actor_Core _core = null;
    public Actor_Core Core { get { return _core; } }

    [SerializeField] private Actor_Player _player;
    public Actor_Player Player
    {
        get { return _player; }
    }

    public Character Char_SO;
    private AudioCue Cues;

    Coroutine energyChangeCoroutine;
    float timeForEnergyUpdate = 2f;

    private int _currentEnergy;
    public int CurrentEnergy
    {
        get { return _currentEnergy; }
        set
        {
            if (energyChangeCoroutine == null)
                energyChangeCoroutine = StartCoroutine(EnergyChange(_currentEnergy, value));
            else
            {
                StopCoroutine(energyChangeCoroutine);
                hudUI.energyText.text = _currentEnergy.ToString();
                energyChangeCoroutine = StartCoroutine(EnergyChange(_currentEnergy, value));
            }
            _currentEnergy = value;
            // Call UI Update Here
            //shopUI.RefreshEnergyText();
        }
    }
    public int waveEnergyReward;
    //Respawn/SpawnPoints VAR
    private Transform playerSpawnPoint;
    public Transform[] playerSpawns; 
    [SerializeField] private float respawnTimer = 2.0f;
    private bool isRespawning = false;
    public LayerMask enemyLayer; 

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

    [HideInInspector] public List<Trap> activeTraps = new List<Trap>();

    // Dictionary to store all the item classes and bind them to a gameobject that gets spawned 
    // when an item is added to the inventory.
    private Dictionary<Item, IEquippable> m_equipables;
    public Dictionary<Item, IEquippable> Equipables => m_equipables;

    [SerializeField] public HUDUI hudUI;
    [SerializeField] ShopUI shopUI;
    [SerializeField] WeaponUI weaponUI;
    public WeaponUI WeaponUI { get { return weaponUI; } }
    [SerializeField] CharacterUI characterUI;
    public CharacterUI CharacterUI { get { return characterUI; } }

    private Item _currentlyEquipped;
    public Item CurrentlyEquipped { get { return _currentlyEquipped; } }

    [SerializeField] GameObject ShopStartingButton;
    [SerializeField] GameObject PauseStartingButton;
    [SerializeField] GameObject GameOverStartingButton;

    protected bool gameOver = false;

    [SerializeField] Material armMaterial;
    [SerializeField] PostProcessHandler playerHealthVolume;

    protected override void Awake() //On Awake set check LevelManager's Instance and playerSpawnPoint
    {
        base.Awake();

        if (_player == null)
        {
            _player = FindObjectOfType<Actor_Player>();
        }

        //Copy reference to Char Info and AudioManager
        Char_SO = GameManager.Instance.GetCharacter();

        if (Char_SO != null)
        {
            
            var temp = Instantiate(Char_SO.player, playerSpawns[0].position, playerSpawns[0].rotation);
            //_player.gameObject.SetActive(false);
            temp.AbilityOne = Char_SO.ab1;
            temp.AbilityTwo = Char_SO.ab2;
            temp.AbilityOne.Initialize(temp.gameObject);
            temp.AbilityTwo.Initialize(temp.gameObject);
            _player = temp;

            if (armMaterial != null)
            {
                armMaterial.SetFloat("_EmissionIntensity", Char_SO.emissionIntensity);
                armMaterial.SetColor("_GlovesColour", Char_SO.glovesColour);
                armMaterial.SetColor("_EmissionColour", Char_SO.emissionColour);
                armMaterial.SetFloat("_SkinTone", Char_SO.skinTone);
            }

        }
        Cues = FindObjectOfType<AudioCue>();

        m_equipables = new Dictionary<Item, IEquippable>();

        if (StartingItem != null)
        {
            InventoryAdd(StartingItem);
            StartingItem.Use();
        }

        Player.OnDeath += Respawn; // adding the respawn function to character after death 
        Player.OnDeath += characterUI.ResetHealthOnRespawn; //Reset health UI on respawn
        Player.OnHealthChanged += UpdateVolume;

        Player.playerInputs.ActivateInput();

        Player.controlsEnabled = true;
        shopUI.pauseMenu.SetActive(false);
        gameOver = false;

        // Set the starting energy value here -> 250 
        hudUI.energyText.text = "250";
        CurrentEnergy = 250;
        // Set the initial waveEnergyReward -> 50
        waveEnergyReward = 50;

    }

    private void Start()
    {
        //Core.OnDeath += () => GameOver(false);
        
        Time.timeScale = 1;

        Player.playerInputs.onActionTriggered += HandleInput;

        SubscribeToAudioEvents();
        if (Char_SO != null)
            Cues.PlayAudioCue(Char_SO.MissionStart);
    }

    public void SetSelectedButton(GameObject newSelectedObj)
    {
        if (Gamepad.current != null)
        {
            EventSystem.current.SetSelectedGameObject(newSelectedObj);
        }
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Pause":
                // Couldn't find a simple hold for now. Handling automatic firing in update.
                if (context.phase == InputActionPhase.Performed && !shopUI.gameObject.activeSelf && Core.CurrentHealth > 0 && !gameOver)
                    TogglePause();
                break;

            case "Shop":
                if (context.phase == InputActionPhase.Performed && !shopUI.pauseMenu.activeSelf && !gameOver)
                    ToggleShop();
                break;

            case "SkipBuildPhase":
                if (context.phase == InputActionPhase.Performed && WaveManager.Instance.isBuildPhase)
                    // Skip the build phase timer
                    WaveManager.Instance.buildPhaseTimer.Tick(1000f);
                    break;

        }
    }

    public void Update()
    {
        if (shopUI.gameObject.activeSelf && !WaveManager.Instance.isBuildPhase)
            shopUI.CloseShop();
    }

    private IEnumerator EnergyChange(int previousEnergy, int updatedEnergy)
    {
        float elapsed = 0f;

        hudUI.energyText.rectTransform.DOPunchAnchorPos(Vector2.one * 5f, 0.75f, 10, 5f)
            .OnComplete(() => hudUI.energyText.rectTransform.DORewind());

        while (elapsed < timeForEnergyUpdate)
        {
            elapsed += Time.deltaTime;
            float ratio = elapsed / timeForEnergyUpdate;
            previousEnergy = (int)Mathf.Lerp(previousEnergy, updatedEnergy, ratio + 0.1f);
            hudUI.energyText.text = previousEnergy.ToString();
            yield return null;
        }
    }

    public void TogglePause()
    {
        shopUI.pauseMenu.SetActive(!shopUI.pauseMenu.activeSelf);
        shopUI.combatHUD.SetActive(!shopUI.combatHUD.activeSelf);

        shopUI.settingsPanel.SetActive(false);
        shopUI.pausePanel.SetActive(true);

        Time.timeScale = shopUI.pauseMenu.gameObject.activeSelf ? 0.0f : 1.0f;
        Cursor.lockState = shopUI.pauseMenu.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = shopUI.pauseMenu.activeSelf;

        Player.controlsEnabled = !shopUI.pauseMenu.activeSelf;

        if (shopUI.pauseMenu.activeSelf)
        {
            Player.playerInputs.SwitchCurrentActionMap("UI");
            SetSelectedButton(PauseStartingButton);
        }
            

        else
            Player.playerInputs.SwitchCurrentActionMap("Player");
    }

    public void TurnObjectOn(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void TurnObjectOff(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void UpdateVolume(float max, float current)
    {
        float ratio = current / max;
        ratio -= 1;
        playerHealthVolume.SetVolumeWeight(Mathf.Abs(ratio));
    }

    public void ToggleShop()
    {
        if (WaveManager.Instance.isBuildPhase)
        {
            shopUI.ToggleMenu();
            shopUI.UpdateItemUI();
            shopUI.RefreshEnergyText();
            Player.EquipWeapon(Equipables[InventoryList[0]]);
            weaponUI.UpdateEquippedWeapon(CurrentlyEquipped);
        }

        if (shopUI.gameObject.activeSelf)
        {
            Player.playerInputs.SwitchCurrentActionMap("UI");
            SetSelectedButton(ShopStartingButton);
        }
        else
            Player.playerInputs.SwitchCurrentActionMap("Player");
    }

    // Takes passed trap and checks to see if it is already in the active traps list. If it is then remove it, if not add it.
    public void AssessTraps(Trap trap)
    {
        if (activeTraps.Contains(trap))
            activeTraps.Remove(trap);
        else
            activeTraps.Add(trap);
    }

    #region GameLoop

    private void GetPlayerSpawnPoint()
    {
        Collider[] detectionCollider;
        foreach (Transform spawnPoint in playerSpawns)
        {
            detectionCollider = Physics.OverlapSphere(spawnPoint.position, 3f, enemyLayer);
            if (detectionCollider.Length <= 0)
            {
                playerSpawnPoint = spawnPoint;
                break;
            }
        }
        if (playerSpawnPoint == null)
        {
            playerSpawnPoint = playerSpawns[Random.Range(0, playerSpawns.Length)];
        }
    }

    private void Respawn() // respawning character and Setting Spectator Camera
    {
  
        if (isRespawning) return;
        //activate Spectator Camera and set Player's gameObject to false
        spectatorCamera.gameObject.SetActive(true);
        Player.gameObject.SetActive(false);
        GetPlayerSpawnPoint();
        //start the player respawn
        StartCoroutine(RespawnTimer());
    }

    private IEnumerator RespawnTimer()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnTimer);

        _player.transform.position = playerSpawnPoint.position;
        _player.transform.rotation = playerSpawnPoint.rotation;

        playerSpawnPoint = null; 

        spectatorCamera.gameObject.SetActive(false);
        Player.controlsEnabled = true; 
        Player.gameObject.SetActive(true);
        isRespawning = false;
    }

    public void GameOver(bool playerWon)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Player.controlsEnabled = false;
        gameOver = true;

        EndGameMenu.SetActive(true);
        EndGameMenu.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero).OnComplete(() => Time.timeScale = 0.0f);
        EndGameText.text = playerWon ? "Victory!" : "Defeat!";
        if(playerWon)
        {
            Cues.PlayAudioCue(Char_SO.MissionWin);
        }
        else if(!playerWon)
        {
            Cues.PlayAudioCue(Char_SO.MissionLoss);
        }

        Player.playerInputs.SwitchCurrentActionMap("UI");
        SetSelectedButton(GameOverStartingButton);
    }

    public void OnRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        Player.playerInputs.SwitchCurrentActionMap("Player");
        gameOver = false;
    }

    public void OnQuitButton()
    {
        Time.timeScale = 1;
        GameManager.Instance.ReturnToMenu();
    }

    public void OnGameQuit()
    {
        Time.timeScale = 1;
        GameManager.Instance.OnQuitButton();
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

            if (Char_SO != null)
                Cues.PlayAudioCue(Char_SO.PurchaseItem, 15);
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
        weaponUI.UpdateEquippedWeapon(itemToUse);
    }

    #endregion

    private void SubscribeToAudioEvents()
    {
        //char chosen - Handled in MenuHandler.cs

        if (Char_SO == null) return;

        WaveManager.Instance.OnWaveEnded += () => Cues.PlayAudioCue(Char_SO.BuildPhaseStart, 30);
        WaveManager.Instance.OnWaveStarted += () => Cues.PlayAudioCue(Char_SO.WaveStart, 30);

        _core.OnDamageTaken += (DamageData) => Cues.PlayAudioCue(Char_SO.CoreDamaged, 3);
        //Player.OnAbilityOneTriggered
        //Player.OnAbilityTwoTriggered

        foreach (Actor_Enemy enemy in FindObjectsOfType<Actor_Enemy>())
        {
            enemy.OnDeath += () => Cues.PlayAudioCue(Char_SO.EnemyKill, 5);
        }

        Player.OnDamageTaken += (DamageData) => Cues.PlayAudioCue(Char_SO.PlayerHit, 10);
        Player.OnDeath += () => Cues.PlayAudioCue(Char_SO.PlayerRespawn);
        
        //OpenShop?
                //purchaseItem - handled in InventoryAdd()
                //Trap Build - Handled in TrapPlacement.cs
                //Trap Destroyed - Handled in Trap.cs
            
                //MissionStart - Handled in Level Manager Start()
                //MissionWin/MissionLoss - Handled in GameOver Method of LevelManager

    }

    private void OnDrawGizmos()
    {
        foreach (Transform spawn in playerSpawns)
        {
            Gizmos.color = Color.green; 
            Gizmos.DrawWireSphere(spawn.position, 3f); 
        }
    }
}
