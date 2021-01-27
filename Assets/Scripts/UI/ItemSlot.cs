using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ShopUI shop;

    [SerializeField] private Image BorderImage = null;
    [SerializeField] private Image ItemIcon = null;
    [SerializeField] private TMP_Text itemName = null;
    [SerializeField] private TMP_Text itemCost = null;

    [SerializeField] private Item item;

    [SerializeField] private Sprite selectedSprite = null;
    [SerializeField] private Sprite unselectedSprite = null;

    //private bool purchased = false;

    private void Awake()
    {
        shop = GetComponentInParent<ShopUI>();
    }

    private void Start()
    {
        itemName.text = item.itemName;
        ItemIcon.sprite = item.itemIcon;

        itemCost.text = item.itemCost.ToString();
    }

    // Buying a shop item
    public void BuyItem()
    {
        // To prevent duplicates
        if (LevelManager.Instance.InventoryList.Contains(item)) return;

        Actor_Player buyer = LevelManager.Instance.Player;

        Debug.Log("Bought item: " + itemName.text);

        // TODO: Add functionality for purchasing using Energy

        LevelManager.Instance.InventoryAdd(item);

        shop.RefreshEnergyText();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.25f;
        BorderImage.sprite = selectedSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        BorderImage.sprite = unselectedSprite;
    }
}
