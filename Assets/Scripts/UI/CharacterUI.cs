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

    // Start is called before the first frame update
    void Start()
    {
        player = LevelManager.Instance.Player;
        ability1_Icon.sprite = player.AbilityOne.abilitySprite;
        ability2_Icon.sprite = player.AbilityTwo.abilitySprite;

        player.AbilityOne.abilityTimer.OnTimerEnd += () => ability1_Clock.fillAmount = 0;
        player.AbilityTwo.abilityTimer.OnTimerEnd += () => ability2_Clock.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Update current energy on UI
        playerEnergy.text = LevelManager.Instance.CurrentEnergy.ToString();
        //Tick the player ability timers
        player.AbilityOne.abilityTimer.Tick(Time.deltaTime);
        player.AbilityTwo.abilityTimer.Tick(Time.deltaTime);

        // Display cooldown clock for abilities
        if (player.AbilityOne.abilityTimer.isPlaying)
        {
            ability1_Clock.fillAmount = 1 - player.AbilityOne.abilityTimer.GetProgress();
        }
        if (player.AbilityTwo.abilityTimer.isPlaying)
        {
 
            ability2_Clock.fillAmount = 1 - player.AbilityTwo.abilityTimer.GetProgress();
        }

    }
}
