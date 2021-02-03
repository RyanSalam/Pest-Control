using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] Image heatBar;
    [SerializeField] Image crosshair;
    [SerializeField] Image hitmarkerImage;
    [SerializeField] float hitmarkerTime;

    [SerializeField] Image equippedWeaponIcon;

    // Update heat bar - called when heat level changes
    /// <summary>
    /// Update the heat bar. Call when heat level changes. Takes in current heat level and max heat level
    /// </summary>
    public void UpdateHeatBar(float heatLevel, float maxHeatLevel)
    {
        heatBar.fillAmount = heatLevel / maxHeatLevel;
    }

    // Crosshair
    public void CrosshairSpread(Vector2 spread)
    {
        crosshair.transform.position = spread;
    }

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

    public void UpdateEquippedWeapon()
    {
        equippedWeaponIcon.sprite = LevelManager.Instance.CurrentlyEquipped.itemIcon;
    }

}
