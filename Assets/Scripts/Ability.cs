using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{   
    [Header("Ability Info")]
    public string abilityName = "Ability";
    [Multiline(5)]public string abilityDesc = "Description";
    public Sprite abilitySprite;

    public Timer lifetimeTimer;
    public Timer cooldownTimer;
    public float lifetimeDuration;
    public float cooldownDuration;
    public string AbilityButton = "Button Name";
    public bool isAbilityOnCoolDown = false;

    /// <summary>
    /// Initializes the ability to be used.
    /// Takes in Gameobject parameter that will contain variables and components the ability will need.
    /// </summary>
    /// <param name="abilitySource"></param>
    public virtual void Initialize(GameObject abilitySource)
    {
        lifetimeTimer = new Timer(lifetimeDuration, false);
        cooldownTimer = new Timer(cooldownDuration, false);
        //lifetimeTimer.OnTimerEnd += OnCooldownEnd;
        lifetimeTimer.OnTimerEnd += OnLifetimeEnd;
        lifetimeTimer.OnTimerEnd += () => cooldownTimer.PlayFromStart();
        cooldownTimer.OnTimerEnd += OnCooldownEnd;

        isAbilityOnCoolDown = false;

        Actor_Player player = abilitySource.GetComponent<Actor_Player>();
        player.playerInputs.actions[AbilityButton].performed += (context) => HandleInput();
    }
    public virtual void HandleInput() 
    {
        if (!CanExecute()) return;

        Execute();
    }

    public virtual void Execute()
    {
        lifetimeTimer.PlayFromStart();
    }
    /// <summary>
    /// Conditions that must be met for our ability to be triggered.
    /// </summary>
    /// <returns></returns>
    /// 
    public virtual bool CanExecute() => !isAbilityOnCoolDown;

    /// <summary>
    /// Gets triggered when the ability timer ends.
    /// </summary>
    /// <returns></returns>
    /// 
    public abstract void OnCooldownEnd();

    public abstract void OnLifetimeEnd();
}