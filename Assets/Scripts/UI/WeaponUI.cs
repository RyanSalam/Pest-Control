using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    // Update heat bar - called when heat level changes
    [SerializeField] Slider heatBar;
    /// <summary>
    /// Update the heat bar. Call when heat level changes. Takes in current heat level and max heat level
    /// </summary>
    public void UpdateHeatBar(float heatLevel, float maxHeatLevel)
    {
        heatBar.value = heatLevel / maxHeatLevel;
    }

    // Crosshair
    [SerializeField] Image crosshair;
    public void ui_CrosshairSpread(Vector2 spread)
    {
        crosshair.transform.position = spread;
    }

    // Toggle canvas
    [SerializeField] Canvas weaponUICanvas;
    /// <summary>
    /// True - enable, False - disable
    /// </summary>
    public void ToggleCanvas(bool toggle)
    {
        weaponUICanvas.gameObject.SetActive(toggle);
    }

    // Hit marker
    [SerializeField] Image hitmarkerImage;
    [SerializeField] float hitmarkerTime;
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

}
