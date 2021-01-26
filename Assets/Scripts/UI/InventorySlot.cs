using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Image itemIcon;

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
}
