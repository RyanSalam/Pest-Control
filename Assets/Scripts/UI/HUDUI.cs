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

    private void Start()
    {
        core = LevelManager.Instance.Core;
    }

    private void Update()
    {
        UpdateCoreHealth();
        UpdateEnemyCount();
        UpdateWaveNumber();
    }

    #region Updating HUD UI
    private void UpdateCoreHealth()
    {
        coreFill.fillAmount = core.CurrentHealth / core.MaxHealth;
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
