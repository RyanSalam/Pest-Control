using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] Image healthBarImage;
    // Update current health. Caused when taking damage or gaining health
    /// <summary>
    /// Update the health bar to the set value. Takes in floats for current health and maximum health.
    /// </summary>
    public void UpdateHealth(float health, float maxHealth)
    {
        // Set health to current health as a decimal
        float healthAsDecimal = health / maxHealth;
        healthBar.value = healthAsDecimal;
        healthBarImage.fillAmount = healthAsDecimal;
    }
}
