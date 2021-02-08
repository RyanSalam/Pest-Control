using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Image healthBarImage;

    private void Start()
    {
        LevelManager.Instance.Player.OnHealthChanged += UpdateHealth;
    }

    // Update current health. Caused when taking damage or gaining health
    /// <summary>
    /// Update the health bar to the set value. Takes in floats for current health and maximum health.
    /// </summary>
    public void UpdateHealth(float maxHealth, float health)
    {
        // Set health to current health as a decimal
        float healthAsDecimal = health / maxHealth;
        healthBarImage.fillAmount = healthAsDecimal;
    }
}
