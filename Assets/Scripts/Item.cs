using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Item : ScriptableObject
{
    // variables for item 
    public string itemName; 
    public int itemCost;
    public Sprite itemIcon;
    public string itemDescription;
    //weapon 
    public AudioClip equipSound; 
    public GameObject EquipableToSpawn;
    //trap
    private int _refundCost;
    public bool isWeapon;
    public AltFireAttachment altFireAttachment;


    public void Use() //equips the item
    {
        LevelManager.Instance.UseItem(this); // equiping weapon 
    }
    public virtual void ItemPurchased() //purchases an Item
    {
        //gets inventory and adds item
        LevelManager.Instance.InventoryAdd(this); 
    }
}
