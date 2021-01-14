using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Inventory : MonoSingleton<Inventory> 
{
    [SerializeField]
    private Item startingItem; // sets starting weapon 
    public List<Item> inventoryList = new List<Item>(); //makes the lists of Items from item class
    [SerializeField]
    private int inventoryLimit =  4; //defaulting inventory limit to 4

    public Action onItemChangeCallback; //delgate void for when an item is changed from inventory 


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
        if (inventoryList.Count <= inventoryLimit && !inventoryList.Contains(item)) 
        {
            inventoryList.Add(item);  //adds item to inventory 
            onItemChangeCallback?.Invoke();
        }
    }
    public void InventoryRemove(Item item) //function that removes an item from the Item class to the inventory 
    {
        //checks to see if the inventory contains and item
        if (inventoryList.Contains(item))
        {
            inventoryList.Remove(item); // removes item from inventory 
            onItemChangeCallback?.Invoke();
        }
    }

}
