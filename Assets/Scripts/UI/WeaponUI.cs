using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] public Image heatBar;
    [SerializeField] public Image crosshair;
    [SerializeField] Image hitmarkerImage;
    [SerializeField] float hitmarkerTime;

    [SerializeField] Image equippedWeaponIcon;

    [SerializeField] InventorySlot[] inventorySlots;

    private void Start()
    {
        UpdateInventoryUI();
    }

    // Update heat bar - called when heat level changes
    /// <summary>
    /// Update the heat bar. Call when heat level changes. Takes in current heat level and max heat level
    /// </summary>
    public void UpdateHeatBar(float heatLevel, float maxHeatLevel)
    {
        heatBar.fillAmount = heatLevel / maxHeatLevel;
    }

    // Crosshair
    //public void CrosshairSpread(Vector2 spread)
    //{
    //    crosshair.transform.position = spread;
    //}

    // Toggle canvas
    /// <summary>
    /// True - enable, False - disable
    /// </summary>
    public void ToggleCanvas(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    // Hit marker
    public void DrawHitmarker()
    {
        StopCoroutine("showhitmarker");
        hitmarkerImage.enabled = true;
        StartCoroutine("showhitmarker");
    }
    public IEnumerator showhitmarker()
    {
        yield return new WaitForSeconds(hitmarkerTime);
        hitmarkerImage.enabled = false;
    }

    public void UpdateEquippedWeapon(Item itemToUse)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            // Find currently equipped weapon
            if (itemToUse.itemIcon == slot.itemIcon.sprite)
                slot.SetSelectedItem(true);
            else
                slot.SetSelectedItem(false);
        }
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < LevelManager.Instance.InventoryList.Count)
            {
                //Debug.Log(LevelManager.Instance.InventoryList[i].ToString());
                //slots[i].AddItem(LevelManager.Instance.InventoryList[i]);
                inventorySlots[i].AddItem(LevelManager.Instance.InventoryList[i]);
            }
            else
            {
                //slots[i].ClearSlot();
                inventorySlots[i].ClearInventorySlot();
            }
        }
    }

}
