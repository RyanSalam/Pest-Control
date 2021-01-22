using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image BorderImage = null;
    [SerializeField] private Image ItemIcon = null;
    [SerializeField] private TMP_Text itemName = null;
    [SerializeField] private TMP_Text itemCost = null;

    [SerializeField] private Item item;

    [SerializeField] private Sprite selectedSprite = null;
    [SerializeField] private Sprite unselectedSprite = null;

    private bool purchased = false;

    private void Start()
    {
        itemName.text = item.itemName;
        ItemIcon.sprite = item.itemIcon;

        itemCost.text = item.itemCost.ToString();
    }

    public void BuyItem()
    {
        if (LevelManager.Instance.InventoryList.Contains(item)) return;

        LevelManager.Instance.InventoryAdd(item);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one * 1.25f, 0.25f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.25f);
    }
}
