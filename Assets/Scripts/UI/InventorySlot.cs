using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private ShopUI shop;

    [SerializeField] private Item item;
    [SerializeField] private Image itemIcon;

    private void Awake()
    {
        shop = GetComponentInParent<ShopUI>();
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
