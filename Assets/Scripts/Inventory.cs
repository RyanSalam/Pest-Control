using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Inventory : MonoSingleton<Inventory> 
{
    [SerializeField]
    private Item startingItem; // sets starting weapon 
    public Item StartingItem => startingItem; // Lamda expression for shorter getter.

    [SerializeField] private List<Item> m_inventoryList = new List<Item>(); //makes the lists of Items from item class
    public List<Item> InventoryList
    {
        get { return m_inventoryList; }
    }


    [SerializeField] private int inventoryLimit =  4; //defaulting inventory limit to 4

    public Action onItemChangeCallback; //delgate void for when an item is changed from inventory 

    // Dictionary to store all the item classes and bind them to a gameobject that gets spawned 
    // when an item is added to the inventory.
    private Dictionary<Item, IEquippable> m_equipables;
    public Dictionary<Item, IEquippable> Equipables => m_equipables;

    protected override void Awake()
    {
        base.Awake();
        m_equipables = new Dictionary<Item, IEquippable>();
    }

    //checks to see if the starting item exists then adds it into the inventory list
    private void Start()
    {
        if(startingItem != null) 
        {
            InventoryAdd(startingItem);            
        }
    }
    public void InventoryAdd(Item item) //function that adds an item from the Item class to the inventory 
    {  
        //checks to see if the inventory count is less then the limit and checks if it dose not contain the item
        if (m_inventoryList.Count <= inventoryLimit && !m_inventoryList.Contains(item)) 
        {
            m_inventoryList.Add(item);  //adds item to inventory 
            onItemChangeCallback?.Invoke(); //calling delgate function "onItemChangeCallback" "?" is !=null

            // We instantiate the gameobject for this item as soon as we add it to the inventory.
            // Helps us create the object as we need it rather than creating them on start.
            // We only need to instantiate once and should not need to destroy them when we remove.
            if (!Equipables.ContainsKey(item)) 
            {
                GameObject temp = Instantiate(item.EquipableToSpawn) as GameObject;
                IEquippable test = temp.GetComponent<IEquippable>();

                if (test != null)
                    Equipables.Add(item, test);
            }
        }
    }
    public void InventoryRemove(Item item) //function that removes an item from the Item class to the inventory 
    {
        //checks to see if the inventory contains and item
        if (m_inventoryList.Contains(item))
        {
            m_inventoryList.Remove(item); // removes item from inventory 
            onItemChangeCallback?.Invoke(); //calling delgate function "onItemChangeCallback" "?" is !=null
        }
    }

}
