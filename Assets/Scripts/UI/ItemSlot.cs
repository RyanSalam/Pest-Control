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
    //[SerializeField] public bool hasTooltip = false;
    //[SerializeField] public TMP_Text tooltipNameText;
    //[SerializeField] public TMP_Text tooltipDescriptionText;

    //[SerializeField] GameObject tooltipPanel;

    [SerializeField] private Image BorderImage = null;
    [SerializeField] private Image ItemIcon = null;
    [SerializeField] private TMP_Text itemName = null;
    [SerializeField] private TMP_Text itemCost = null;

    [SerializeField] private Item item;

    [SerializeField] private Sprite selectedSprite = null;
    [SerializeField] private Sprite unselectedSprite = null;
    [SerializeField] private Sprite ownedSprite = null;

    [SerializeField] private TMP_Text ownedText;

    [SerializeField] private Image altFire;
    [SerializeField] public bool storingWeapon = true;

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
        
        UpdateAltFire();
    }

    private void FixedUpdate()
    {
        if (LevelManager.Instance.InventoryList.Contains(item))
        {
            ownedText.text = "Owned";
            BorderImage.sprite = ownedSprite;
        }
        else
        {
            ownedText.text = "Buy";
            BorderImage.sprite = unselectedSprite;
        }

    }

    public void UpdateAltFire()
    {
        if (storingWeapon)
        {
            if (altFire != null && item.isWeapon && item.altFireAttachment != null)
            {
                altFire.enabled = true;
                altFire.sprite = item.altFireAttachment.altFireIcon;
            }
            else
                altFire.enabled = false;
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

            BorderImage.sprite = ownedSprite;
            //Debug.Log("Bought item: " + itemName.text + " for " + item.itemCost.ToString() + " Energy.");
        }
        else
        {
            // Player does not have enough energy
            Debug.Log("You don't have enough Energy to purchase this item!");
            shop.warningNeeded = true;
        }
        shop.RefreshEnergyText();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.05f;
        BorderImage.sprite = selectedSprite;

        Fluffy.Instance.textToOverride = item.itemDescription.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        if (LevelManager.Instance.InventoryList.Contains(item))
            BorderImage.sprite = ownedSprite;
        else
            BorderImage.sprite = unselectedSprite;

        Fluffy.Instance.textToOverride = "";
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
