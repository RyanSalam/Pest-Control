using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private TMP_Text customerEnergy = null;
    //[SerializeField] private int energyValue;
    //[SerializeField] private TMP_Text energyChangeIndicator;
    //[SerializeField] private Text itemDescription = null;
    //[SerializeField] private GameObject inventoryPanel = null;
    public Actor_Player Customer;
    [SerializeField] private GameObject shopUI;
    [SerializeField] public GameObject combatHUD;
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject settingsPanel;
    [SerializeField] public GameObject pausePanel;
    public GameObject[] hudElements;
    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] WeaponUI weaponUI;
    [SerializeField] GameObject warningPanel;
    [HideInInspector] public bool warningNeeded = false;
    Coroutine warningMessageCoroutine;
    [SerializeField] Color warningColor;
    [SerializeField] float warningMessageDuration;

    public Image[] in_Game_Inventory;

    [SerializeField] GameObject phaseTimer;
    [SerializeField] Image phaseTimerClock;
    public float phaseTimerProgress;

    private void Awake()
    {
        if (shopUI == null)
            shopUI = this.gameObject;

    }

    private void Start()
    {
        LevelManager.Instance.onItemChangeCallback += UpdateItemUI;
        Customer = LevelManager.Instance.Player;
        warningPanel.SetActive(false);
    }

    private void Update()
    {
        if (warningNeeded)
        {
            if (warningMessageCoroutine == null)
                warningMessageCoroutine = StartCoroutine(WarningMessage());
            else
            {
                StopCoroutine(warningMessageCoroutine);
                warningMessageCoroutine = StartCoroutine(WarningMessage());
            }
        }

        if (phaseTimer.activeSelf)
        {
            phaseTimerProgress = WaveManager.Instance.buildPhaseTimer.GetProgress();
            phaseTimerClock.fillAmount = 1 - phaseTimerProgress;
        }
    }

    public IEnumerator WarningMessage()
    {
        customerEnergy.rectTransform.DOPunchAnchorPos(Vector2.one * 20f, 1f, 50, 10f).OnComplete(() => customerEnergy.rectTransform.DORewind());
        customerEnergy.color = warningColor;
        warningNeeded = false;
        warningPanel.SetActive(true);
        warningPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        yield return new WaitForSeconds(warningMessageDuration);
        warningPanel.transform.DOScale(Vector3.zero, 0.15f).From(Vector3.one).OnComplete(() => warningPanel.SetActive(false));
        customerEnergy.color = Color.white;
    }

    public void ToggleShop()
    {
        LevelManager.Instance.ToggleShop();
    }

    // Forcing player to close shop when the build phase ends
    public void CloseShop()
    {
        foreach (GameObject hudElement in hudElements)
        {
            hudElement.SetActive(true);
        }
        foreach (Image img in in_Game_Inventory)
        {
            img.enabled = true;
        }

        LevelManager.Instance.hudUI.buildPhaseInfo.SetActive(false);

        gameObject.SetActive(false);
        combatHUD.SetActive(true);
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LevelManager.Instance.Player.controlsEnabled = true;
        LevelManager.Instance.Player.playerInputs.SwitchCurrentActionMap("Player");
        weaponUI.UpdateEquippedWeapon(LevelManager.Instance.CurrentlyEquipped);
    }

    // Toggles shop menu
    public void ToggleMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        foreach (GameObject hudElement in hudElements)
        {
            hudElement.SetActive(!gameObject.activeSelf);
        }
        foreach (Image img in in_Game_Inventory)
        {
            img.enabled = !gameObject.activeSelf;
        }
        LevelManager.Instance.hudUI.buildPhaseInfo.SetActive(!gameObject.activeSelf);

        //Time.timeScale = gameObject.activeSelf ? 0.0f : 1.0f;
        Cursor.lockState = gameObject.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = gameObject.activeSelf;

        LevelManager.Instance.Player.controlsEnabled = !gameObject.activeSelf;
    }

    public void UpdateItemUI()
    {
        InventorySlot[] slots = shopUI.GetComponentsInChildren<InventorySlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < LevelManager.Instance.InventoryList.Count)
            {
                //Debug.Log(LevelManager.Instance.InventoryList[i].ToString());
                slots[i].AddItem(LevelManager.Instance.InventoryList[i]);
                inventorySlots[i].AddItem(LevelManager.Instance.InventoryList[i]);
            }
            else
            {
                slots[i].ClearSlot();
                inventorySlots[i].ClearInventorySlot();
            }
        }
    }

    public void RefreshEnergyText()
    {
        // If shop open
        if (gameObject.activeSelf)
        {
            customerEnergy.text = LevelManager.Instance.CurrentEnergy.ToString();
            combatHUD.GetComponent<HUDUI>().energyText.text = LevelManager.Instance.CurrentEnergy.ToString();
        }
    }

    private void OnDisable()
    {
        if (warningMessageCoroutine != null)
        {
            StopCoroutine(warningMessageCoroutine);
            warningMessageCoroutine = null;
        }
        warningPanel.SetActive(false);
    }
}
