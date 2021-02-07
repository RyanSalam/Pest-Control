using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUI : MonoBehaviour
{
    Actor_Player player;
    [SerializeField] Image ability1_Icon;
    [SerializeField] Image ability2_Icon;
    [SerializeField] Image ability1_Clock;
    [SerializeField] Image ability2_Clock;
    [SerializeField] TMP_Text playerEnergy;

    [SerializeField] public Image abilityEffect;
    private float effectTimer = 0.0f;
    public bool abilityActive = false;
    private bool isUsed = false;

    // Start is called before the first frame update
    void Start()
    {
        player = LevelManager.Instance.Player;
        ability1_Icon.sprite = player.AbilityOne.abilitySprite;
        ability2_Icon.sprite = player.AbilityTwo.abilitySprite;

        player.AbilityOne.cooldownTimer.OnTimerEnd += () => ability1_Clock.fillAmount = 0;
        player.AbilityTwo.cooldownTimer.OnTimerEnd += () => ability2_Clock.fillAmount = 0;

        abilityEffect.color = new Color(abilityEffect.color.r, abilityEffect.color.g, abilityEffect.color.b, 0.01f);
        effectTimer = 0.0f;
        isUsed = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Update current energy on UI
        playerEnergy.text = LevelManager.Instance.CurrentEnergy.ToString();
        //Tick the player ability timers
        player.AbilityOne.lifetimeTimer.Tick(Time.deltaTime);
        player.AbilityOne.cooldownTimer.Tick(Time.deltaTime);
        player.AbilityTwo.lifetimeTimer.Tick(Time.deltaTime);
        player.AbilityTwo.cooldownTimer.Tick(Time.deltaTime);

        // Display cooldown clock for abilities
        if (player.AbilityOne.cooldownTimer.isPlaying)
        {
            ability1_Clock.fillAmount = 1 - player.AbilityOne.cooldownTimer.GetProgress();
        }
        if (player.AbilityTwo.cooldownTimer.isPlaying)
        {
 
            ability2_Clock.fillAmount = 1 - player.AbilityTwo.cooldownTimer.GetProgress();
        }
    }

    public void FadeAbilityEffect(float time)
    {
        //Debug.Log("Fading alpha");

        effectTimer += Time.deltaTime;
        if (effectTimer < time && !isUsed)
        {
            abilityEffect.CrossFadeAlpha(255f, time, true);
            abilityActive = true;
        }
        else
        {
            abilityActive = false;
            isUsed = true;
        }
    }

    public void ResetAlphaValue()
    {
        abilityEffect.CrossFadeAlpha(0.01f, 0.5f, true);
        abilityActive = false;
        isUsed = false;
        effectTimer = 0.0f;
    }


}
