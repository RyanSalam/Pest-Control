using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    private ShopUI shop;
    [SerializeField] public bool hasTooltip = false;
    [SerializeField] public TMP_Text tooltipNameText;
    [SerializeField] public TMP_Text tooltipDescriptionText;

    [SerializeField] GameObject tooltipPanel;

    [SerializeField] private Image BorderImage = null;
    [SerializeField] private Image ItemIcon = null;
    [SerializeField] private TMP_Text itemName = null;
    [SerializeField] private TMP_Text itemCost = null;

    [SerializeField] private Item item;

    [SerializeField] private Sprite selectedSprite = null;
    [SerializeField] private Sprite unselectedSprite = null;

    private bool purchased = false;

    private void Awake()
    {
        shop = GetComponentInParent<ShopUI>();
    }

    private void Start()
    {
        itemName.text = item.itemName;
        ItemIcon.sprite = item.itemIcon;

        itemCost.text = item.itemCost.ToString();

        if (hasTooltip)
        {
            tooltipPanel.SetActive(false);
            tooltipNameText.text = item.itemName.ToString();
            tooltipDescriptionText.text = item.itemDescription.ToString();
        }
    }

    // Buying a shop item
    public void BuyItem()
    {
        // To prevent duplicates
        if (LevelManager.Instance.InventoryList.Contains(item)) return;

        // Checking if player has enough energy to make the purchase
        if (LevelManager.Instance.CurrentEnergy >= item.itemCost)
        {
            LevelManager.Instance.InventoryAdd(item);
            purchased = true;
            item.ItemPurchased();

            //Debug.Log("Bought item: " + itemName.text + " for " + item.itemCost.ToString() + " Energy.");
        }
        else
        {
            // Player does not have enough energy
            Debug.Log("You don't have enough Energy to purchase this item!");
        }
        shop.RefreshEnergyText();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.25f;
        BorderImage.sprite = selectedSprite;

        if (hasTooltip)
        {
            tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        BorderImage.sprite = unselectedSprite;

        if (hasTooltip)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void OnMove(AxisEventData eventData)
    {
        //if (eventData.selectedObject == gameObject)
        //{
        //    transform.localScale = Vector3.one * 1.25f;
        //    BorderImage.sprite = selectedSprite;
        //}

        //else
        //{
        //    transform.localScale = Vector3.one;
        //    BorderImage.sprite = unselectedSprite;
        //}
    }

    public void OnSelect(BaseEventData eventData)
    {
        //if (eventData.selectedObject == gameObject)
        //{
        //    transform.localScale = Vector3.one * 1.25f;
        //    BorderImage.sprite = selectedSprite;
        //}

        //else
        //{
        //    transform.localScale = Vector3.one;
        //    BorderImage.sprite = unselectedSprite;
        //}
    }
}
