using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{   
    [Header("Ability Info")]
    public string abilityName = "Ability";
    [Multiline(5)]public string abilityDesc = "Description";
    public Sprite abilitySprite;

    public float coolDownDuration = 5.0f;
    public string AbilityButton = "Button Name";
    public bool isAbilityOnCoolDown = false;

    /// <summary>
    /// Initializes the ability to be used.
    /// Takes in Gameobject parameter that will contain variables and components the ability will need.
    /// </summary>
    /// <param name="abilitySource"></param>
    public abstract void Initialize(GameObject abilitySource);
    public abstract void Execute();
    /// <summary>
    /// Conditions that must be met for our ability to be triggered.
    /// </summary>
    /// <returns></returns>
    public virtual bool CanExecute() => isAbilityOnCoolDown;
}