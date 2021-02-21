using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private ShopUI shop;

    [SerializeField] private Item item;
    [SerializeField] public Image itemIcon;

    [SerializeField] private Image borderImage;
    [SerializeField] private Sprite inventoryOccupied;
    [SerializeField] private Sprite inventoryEmpty;

    // NEVER ATTACH A SPRITE TO THIS FOR SHOP INVENTORY SLOTS
    [SerializeField] public Sprite inventorySelected;
    bool selected = false;

    private void Awake()
    {
        shop = GetComponentInParent<ShopUI>();
    }

    private void Update()
    {
        // For inventory slots in the shop UI
        if (inventorySelected == null)
            UpdateFrame();
        // For inventory slots in in-game UI
        else
            UpdateFrameInGame();
    }

    // For shop UI inventory slots
    private void UpdateFrame()
    {
        if (item != null)
            borderImage.sprite = inventoryOccupied;
        else
            borderImage.sprite = inventoryEmpty;
    }

    // For in game inventory slots
    public void UpdateFrameInGame()
    {
        if (itemIcon.sprite == null)
            borderImage.sprite = inventoryEmpty;
        else if (itemIcon.sprite != null && selected)
            borderImage.sprite = inventorySelected;
        else if (itemIcon.sprite != null && !selected)
            borderImage.sprite = inventoryOccupied;

        if (itemIcon.sprite != null)
            itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 255f);
        else
            itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 0.01f);
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        itemIcon.sprite = item.itemIcon;
        itemIcon.enabled = true;
    }

    // For shop UI inventory
    public void ClearSlot()
    {
        item = null;
        itemIcon.enabled = false;
    }

    // For in game UI inventory
    public void ClearInventorySlot()
    {
        item = null;
        itemIcon.sprite = null;
    }

    // TODO: Implement refunding
    public void RefundItem()
    {
        if (item != null && LevelManager.Instance.InventoryList.Count > 1)
        {
            int refundingAmount = item.itemCost / 2;
            LevelManager.Instance.CurrentEnergy += refundingAmount;
            LevelManager.Instance.InventoryRemove(item);

            //Debug.Log("50% of item cost refunded: " + refundingAmount + " Energy");

            ClearSlot();
            shop.UpdateItemUI();
            shop.RefreshEnergyText();
        }
        else
        {
            Debug.Log("This slot is empty! Please select a valid inventory slot.");
        }
    }
    
    public void SetSelectedItem(bool isSelected)
    {
        selected = isSelected;

    }
}
