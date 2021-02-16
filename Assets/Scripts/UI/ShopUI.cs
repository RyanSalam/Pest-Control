using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private TMP_Text customerEnergy = null;
    //[SerializeField] private Text itemDescription = null;
    //[SerializeField] private GameObject inventoryPanel = null;
    public Actor_Player Customer;
    [SerializeField] private GameObject shopUI;
    [SerializeField] public GameObject combatHUD;
    [SerializeField] public GameObject pauseMenu;
    public GameObject[] hudElements;

    private void Awake()
    {
        if (shopUI == null)
            shopUI = this.gameObject;

    }

    private void Start()
    {
        LevelManager.Instance.onItemChangeCallback += UpdateItemUI;
        Customer = LevelManager.Instance.Player;
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

        gameObject.SetActive(false);
        combatHUD.SetActive(true);
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LevelManager.Instance.Player.controlsEnabled = true;
        LevelManager.Instance.Player.playerInputs.SwitchCurrentActionMap("Player");
    }

    // Toggles shop menu
    public void ToggleMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        foreach (GameObject hudElement in hudElements)
        {
            hudElement.SetActive(!gameObject.activeSelf);
        }

        //Time.timeScale = gameObject.activeSelf ? 0.0f : 1.0f;
        Cursor.lockState = gameObject.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
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
                LevelManager.Instance.Player._audioCue.PlayAudioCue(LevelManager.Instance.Player._cInfo.PurchaseItem, 35);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public void RefreshEnergyText()
    {
        customerEnergy.text = LevelManager.Instance.CurrentEnergy.ToString();
    }
}
