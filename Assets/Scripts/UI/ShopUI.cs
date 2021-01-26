using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    //public static ShopUI instance;

    [SerializeField] private TextMeshProUGUI customerEnergy = null;
    [SerializeField] private Text itemDescription = null;

    [SerializeField] private GameObject inventoryPanel = null;
    public Actor_Player Customer;
    [SerializeField] private GameObject shopUI;

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

    public void ToggleMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        Time.timeScale = gameObject.activeSelf ? 0.0f : 1.0f;
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
                Debug.Log(LevelManager.Instance.InventoryList[i].ToString());
                slots[i].AddItem(LevelManager.Instance.InventoryList[i]);
            }

            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public void RefreshEnergyText()
    {
        customerEnergy.text = "ENERGY: " + LevelManager.Instance.CurrentEnergy.ToString();
    }
}
