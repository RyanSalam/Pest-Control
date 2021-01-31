using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDUI : MonoBehaviour
{
    // Wave management
    // Update wave number - called on a new wave
    /// <summary>
    /// Update the current wave. Call at start of a new wave. Takes in current wave number
    /// </summary>
    [SerializeField] TMP_Text waveNumber;

    Actor_Core core;
    [SerializeField] Image coreFill;
    [SerializeField] TMP_Text enemyCount;
    [SerializeField] float coreCurrentHealth;
    [SerializeField] float coreMaxHealth;

    private void Start()
    {
        core = LevelManager.Instance.Core;
        core.OnHealthChanged += UpdateCoreHealth;
    }

    private void Update()
    {
        UpdateEnemyCount();
        UpdateWaveNumber();
    }

    #region Updating HUD UI
    // subscribe this function to core.OnHealthChanged on start to update the core health info
    private void UpdateCoreHealth(float max, float current)
    {
        coreCurrentHealth = current;
        coreMaxHealth = max;

        // Setting the core's health bar
        coreFill.fillAmount = coreCurrentHealth / coreMaxHealth;
    }

    private void UpdateEnemyCount()
    {
        enemyCount.text = WaveManager.Instance.EnemiesRemaining.ToString();
    }

    public void UpdateWaveNumber()
    {
        waveNumber.text = WaveManager.Instance.WaveNumber.ToString();
    }
    #endregion

}
