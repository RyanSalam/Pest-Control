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
    [SerializeField] GameObject rewardPanel;
    [SerializeField] TMP_Text rewardAmount;

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
    [SerializeField] GameObject waveNumberIndicator;
    [SerializeField] TMP_Text waveNumberText;
    [SerializeField] GameObject buildPhaseInfo;
    public float phaseTimerProgress;

    [SerializeField] Sprite defaultCoreHealthFill;
    [SerializeField] Sprite damagedCoreHealthFill;
    [SerializeField] Color defaultCoreIconColor;
    [SerializeField] Color damagedCoreIconColor;
    [SerializeField] GameObject coreBase;
    [SerializeField] Image coreIcon;
    Coroutine changeCoreColor;

    private void Start()
    {
        defaultCoreHealthFill = coreFill.sprite;
        core = LevelManager.Instance.Core;
        core.OnHealthChanged += UpdateCoreHealth;
        waveNumberIndicator.SetActive(false);
    }

    private void Update()
    {
        UpdateEnemyCount();

        if (phaseTimer.activeSelf)
        {
            phaseTimerProgress = WaveManager.Instance.buildPhaseTimer.GetProgress();
            //phaseTimerCount.text = Mathf.RoundToInt(WaveManager.Instance.buildPhaseTimer.GetRemaining()).ToString();
            phaseTimerClock.fillAmount = 1 - phaseTimerProgress;
        }
    }

    #region Updating HUD UI
    // subscribe this function to core.OnHealthChanged on start to update the core health info
    private void UpdateCoreHealth(float max, float current)
    {
        coreCurrentHealth = current;
        coreMaxHealth = max;

        // Setting the core's health bar
        coreFill.fillAmount = coreCurrentHealth / coreMaxHealth;

        CoreHealthFlash();
    }

    public void CoreHealthFlash()
    {
        if (changeCoreColor == null)
            changeCoreColor = StartCoroutine(CoreHealthChange());
        else
        {
            StopCoroutine(changeCoreColor);
            changeCoreColor = StartCoroutine(CoreHealthChange());
        }
    }

    public IEnumerator CoreHealthChange()
    {
        coreFill.sprite = damagedCoreHealthFill;
        coreIcon.color = damagedCoreIconColor;

        yield return new WaitForSeconds(1.2f);

        coreIcon.color = defaultCoreIconColor;
        coreFill.sprite = defaultCoreHealthFill;
    }

    public IEnumerator BuildPhase()
    {
        // Make sure to disable waveNumberIndicator during build phase
        waveNumberIndicator.SetActive(false);

        // Display wave info panel
        WaveInfoPanel.SetActive(true);

        // If we passed a wave: display reward panel & set reward amount
        if (WaveManager.Instance.waveIndex + 1 > 1)
        {
            rewardPanel.SetActive(true);
            rewardAmount.text = LevelManager.Instance.waveEnergyReward.ToString();
        }
        else
            rewardPanel.SetActive(false);

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
        // Display Build Phase text
        buildPhaseInfo.SetActive(true);

        yield return new WaitForSeconds(5f);

        // Hide wave info panel;
        WaveInfoPanel.SetActive(false);
    }

    public IEnumerator DefensePhase()
    {
        // Hide timer for the buildphase
        phaseTimer.SetActive(false);
        // Display wave info panel
        // Display defense phase
        currentPhase = "Enemies Incoming!";
        buildPhase.text = currentPhase;
        // Disable reward panel regardless
        rewardPanel.SetActive(false);
        WaveInfoPanel.SetActive(true);
        waveNumber.text = "Wave " + (WaveManager.Instance.waveIndex + 1).ToString();
        //Start displaying the enemyInfo Panel
        enemyInfoPanel.SetActive(true);
        // Start displaying coreInfo Panel
        coreInfoPanel.SetActive(true);
        // Display waveNumberIndicator during the wave
        waveNumberIndicator.SetActive(true);
        // Update wave number text
        waveNumberText.text = (WaveManager.Instance.waveIndex + 1).ToString();
        // Hide build phase text
        buildPhaseInfo.SetActive(false);

        yield return new WaitForSeconds(5f);

        // Hide wave info panel
        WaveInfoPanel.SetActive(false);
    }

    private void UpdateEnemyCount()
    {
        enemyCount.text = WaveManager.Instance.enemiesRemaining.ToString();
    }

    #endregion

}
