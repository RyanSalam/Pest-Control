using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Text customerEnergy = null;
    [SerializeField] private Text itemDescription = null;

    [SerializeField] private GameObject inventoryPanel = null;

    private void Start()
    {
        LevelManager.Instance.onItemChangeCallback += UpdateItemUI;
    }

    public void ToggleMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        Time.timeScale = gameObject.activeSelf ? 0.0f : 1.0f;
        Cursor.lockState = gameObject.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = gameObject.activeSelf;

        LevelManager.Instance.Player.controlsEnabled = !gameObject.activeSelf;
    }

    private void UpdateItemUI()
    {
        
    }
}
