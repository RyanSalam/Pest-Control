using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDUI : MonoBehaviour
{
    [SerializeField] GameObject WaveInfoPanel;
    // Wave management
    // Update wave number - called on a new wave
    /// <summary>
    /// Update the current wave. Call at start of a new wave. Takes in current wave number
    /// </summary>
    [SerializeField] TMP_Text waveNumber;
    [SerializeField] TMP_Text buildPhase;

    Actor_Core core;
    [SerializeField] GameObject coreInfoPanel;
    [SerializeField] Image coreFill;
    [SerializeField] TMP_Text enemyCount;
    [SerializeField] float coreCurrentHealth;
    [SerializeField] float coreMaxHealth;
    [SerializeField] string currentPhase;

    [SerializeField] GameObject enemyInfoPanel;
    [SerializeField] GameObject phaseTimer;
    [SerializeField] Image phaseTimerClock;
    [SerializeField] TMP_Text phaseTimerCount;
    [SerializeField] GameObject skipBuildPhaseInfo;
    public float phaseTimerProgress;


    private void Start()
    {
        core = LevelManager.Instance.Core;
        core.OnHealthChanged += UpdateCoreHealth;
    }

    private void Update()
    {
        UpdateEnemyCount();

        if (phaseTimer.activeSelf)
        {
            phaseTimerProgress = WaveManager.Instance.buildPhaseTimer.GetProgress();
            phaseTimerCount.text = Mathf.RoundToInt(WaveManager.Instance.buildPhaseTimer.GetRemaining()).ToString();
            phaseTimerClock.fillAmount = 1 - phaseTimerProgress;
        }
    }

    //public static float Round(float value, int digits)
    //{
    //    float mult = Mathf.Pow(10.0f, (float)digits);
    //    return Mathf.Round(value * mult) / mult;
    //}

    #region Updating HUD UI
    // subscribe this function to core.OnHealthChanged on start to update the core health info
    private void UpdateCoreHealth(float max, float current)
    {
        coreCurrentHealth = current;
        coreMaxHealth = max;

        // Setting the core's health bar
        coreFill.fillAmount = coreCurrentHealth / coreMaxHealth;
    }

    public IEnumerator BuildPhase()
    {
        // Display wave info panel
        WaveInfoPanel.SetActive(true);
        // Display build phase
        currentPhase = "Build Phase";
        buildPhase.text = currentPhase;
        waveNumber.text = "Wave " + (WaveManager.Instance.waveIndex + 1).ToString();
        // Display the phase timer until the end of the buildphase
        phaseTimer.SetActive(true);
        // Hide the enemyInfo Panel
        enemyInfoPanel.SetActive(false);
        // Hide coreInfo Panel until the Defence Phase
        coreInfoPanel.SetActive(false);

        // Show text for skipping build phase
        skipBuildPhaseInfo.SetActive(true);

        yield return new WaitForSeconds(5f);

        // Hide wave info panel;
        WaveInfoPanel.SetActive(false);
        // Hide skip build phase text
        skipBuildPhaseInfo.SetActive(false);
    }

    public IEnumerator DefensePhase()
    {
        // Hide timer for the buildphase
        phaseTimer.SetActive(false);
        // Display wave info panel
        // Display defense phase
        currentPhase = "Enemies Incoming!";
        buildPhase.text = currentPhase;
        WaveInfoPanel.SetActive(true);
        waveNumber.text = "Wave " + (WaveManager.Instance.waveIndex + 1).ToString();
        //Start displaying the enemyInfo Panel
        enemyInfoPanel.SetActive(true);
        // Start displaying coreInfo Panel
        coreInfoPanel.SetActive(true);

        // Make sure to disable the skip build phase info
        skipBuildPhaseInfo.SetActive(false);

        yield return new WaitForSeconds(5f);

        // Hide wave info panel
        WaveInfoPanel.SetActive(false);
    }

    private void UpdateEnemyCount()
    {
        enemyCount.text = WaveManager.Instance.enemiesRemaining.ToString();
    }

    //public void UpdateWaveNumber()
    //{
    //    waveNumber.text = WaveManagerDepreciated.Instance.WaveNumber.ToString();
    //}
    #endregion

}
