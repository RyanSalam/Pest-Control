﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    // Update current health. Caused when taking damage or gaining health
    [SerializeField] Slider healthBar;
    [SerializeField] Image healthBarImage;
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

    // Core health
    [SerializeField] Slider coreHealthBar;
    [SerializeField] Image coreHealthBarImage;
    /// <summary>
    /// Update the core's health bar to the set value. Takes in floats for current health and maximum health.
    /// </summary>
    public void UpdateCoreHealth(float health, float maxHealth)
    {
        // Set health to current health as a decimal
        float healthAsDecimal = health / maxHealth;
        coreHealthBar.value = health / maxHealth;
        coreHealthBarImage.fillAmount = healthAsDecimal;
    }
}
