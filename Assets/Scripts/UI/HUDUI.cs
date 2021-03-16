using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

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
    float coreCurrentHealth;
    float coreMaxHealth;
    string currentPhase;

    [HideInInspector] public bool warningNeeded = false;
    [SerializeField] float warningMessageDuration;
    [SerializeField] GameObject warningPanel;
    Coroutine warningMessageCoroutine;
    [SerializeField] Color warningColor;


    [SerializeField] GameObject enemyInfoPanel;
    [SerializeField] GameObject phaseTimer;
    [SerializeField] Image phaseTimerClock;
    [SerializeField] GameObject waveNumberIndicator;
    [SerializeField] TMP_Text waveNumberText;
    [SerializeField] GameObject buildPhaseInfo;
    public float phaseTimerProgress;

    [SerializeField] Sprite defaultCoreHealthFill;
    [SerializeField] Sprite damagedCoreHealthFill;
    [SerializeField] Sprite damagedCoreIcon;
    Sprite defaultCoreIcon;
    [SerializeField] Image coreIcon;
    [SerializeField] Material glowingMat;
    Coroutine changeCoreColor;

    [SerializeField] public TMP_Text energyText;
    [SerializeField] TMP_Text energyChangeIndicator;
    [SerializeField] public int energyValue;

    //Coroutine energyChangeCoroutine;

    private void Start()
    {
        defaultCoreHealthFill = coreFill.sprite;
        core = LevelManager.Instance.Core;
        core.OnHealthChanged += UpdateCoreHealth;
        waveNumberIndicator.SetActive(false);
        energyChangeIndicator.text = "";
        defaultCoreIcon = coreIcon.sprite;
        warningPanel.SetActive(false);
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

        if (warningNeeded)
        {
            if (warningMessageCoroutine == null)
                warningMessageCoroutine = StartCoroutine(WarningMessage());
            else
            {
                StopCoroutine(warningMessageCoroutine);
                warningMessageCoroutine = StartCoroutine(WarningMessage());
            }
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
        //coreIcon.material = glowingMat;
        coreIcon.sprite = damagedCoreIcon;

        yield return new WaitForSeconds(1f);

        //coreIcon.material = null;
        coreIcon.sprite = defaultCoreIcon;
        coreFill.sprite = defaultCoreHealthFill;
    }

    public IEnumerator WarningMessage()
    {
        energyText.rectTransform.DOPunchAnchorPos(Vector2.one * 20f, 1f, 50, 10f);
        energyText.color = warningColor;
        warningNeeded = false;
        warningPanel.SetActive(true);
        yield return new WaitForSeconds(warningMessageDuration);
        warningPanel.SetActive(false);
        energyText.color = Color.white;
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

    //public void EnergyChange()
    //{

    //    if (energyValue == LevelManager.Instance.CurrentEnergy) return;

    //    int.TryParse(energyText.text, out energyValue);

    //    //Debug.Log("energyValue: " + energyValue);
    //    //Debug.Log("LevelManager.Instance.CurrentEnergy: " + LevelManager.Instance.CurrentEnergy.ToString());

    //    // If energy goes down
    //    if (energyValue > LevelManager.Instance.CurrentEnergy)
    //    {
    //        energyChangeAmount = energyValue - LevelManager.Instance.CurrentEnergy;
    //        if (energyChangeCoroutine == null)
    //            energyChangeCoroutine = StartCoroutine(EnergyChangeSequence(false));
    //        else
    //        {
    //            StopCoroutine(energyChangeCoroutine);
    //            energyText.text = LevelManager.Instance.CurrentEnergy.ToString();
    //            energyChangeCoroutine = StartCoroutine(EnergyChangeSequence(false));
    //        }
    //    }
    //    // If energy goes up
    //    else if (LevelManager.Instance.CurrentEnergy > energyValue)
    //    {
    //        energyChangeAmount = LevelManager.Instance.CurrentEnergy - energyValue;
    //        if (energyChangeCoroutine == null)
    //            energyChangeCoroutine = StartCoroutine(EnergyChangeSequence(true));
    //        else
    //        {
    //            StopCoroutine(energyChangeCoroutine);
    //            energyText.text = LevelManager.Instance.CurrentEnergy.ToString();
    //            energyChangeCoroutine = StartCoroutine(EnergyChangeSequence(true));
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("No Energy Change");
    //    }

    //}

    //IEnumerator EnergyChangeSequence(bool increasing)
    //{
    //    //Debug.Log("Energy Change started!");
    //    energyChanging = true;
    //    if (increasing)
    //    {
    //        energyChangeIndicator.text = "+" + energyChangeAmount.ToString();
    //    }
    //    else
    //    {
    //        energyChangeIndicator.text = "-" + energyChangeAmount.ToString();
    //    }

    //    while (energyValue != LevelManager.Instance.CurrentEnergy)
    //    {
    //        energyValue += Mathf.RoundToInt(energyChangePerSecond);
    //        energyText.text = energyValue.ToString();
    //        yield return new WaitForSeconds(1f);
    //    }

    //    energyChanging = false;
    //    energyChangeIndicator.text = "";
    //    Debug.Log("Energy change done!");
    //}

    #endregion

}
