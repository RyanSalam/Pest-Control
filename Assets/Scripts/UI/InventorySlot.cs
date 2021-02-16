using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private ShopUI shop;

    [SerializeField] private Item item;
    [SerializeField] private Image itemIcon;

    [SerializeField] private Image borderImage;
    [SerializeField] private Sprite inventoryOccupied;
    [SerializeField] private Sprite inventoryEmpty;

    private void Awake()
    {
        shop = GetComponentInParent<ShopUI>();
    }

    private void Update()
    {
        UpdateFrame();
    }

    private void UpdateFrame()
    {
        if (item != null)
            borderImage.sprite = inventoryOccupied;
        else
            borderImage.sprite = inventoryEmpty;

    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        itemIcon.sprite = item.itemIcon;
        itemIcon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        itemIcon.enabled = false;
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
}
