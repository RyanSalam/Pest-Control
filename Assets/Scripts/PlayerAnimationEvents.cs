using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    Animator anim;
    Timer abilityExecution;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (abilityExecution != null)
            abilityExecution.Tick(Time.deltaTime);
    }

    public void ExecuteAbility(UnityEngine.Object ability)
    {
        Ability abilityToCast = (Ability)ability;
        abilityToCast.Execute();
    }

    public void DoCryostasisExplosion(UnityEngine.Object ability)
    {
        Ability_Animation abilityAnim = (Ability_Animation)ability;
        abilityAnim.DoCryoExplosion();
    }

    public void TurnControllerOff(UnityEngine.Object abilityObj)
    {
        Ability_Animation abilityAnim = (Ability_Animation)abilityObj;
        abilityAnim.SwitchController(false);
    }
    public void TurnControllerOn(UnityEngine.Object abilityObj)
    {
        Ability_Animation abilityAnim = (Ability_Animation)abilityObj;
        abilityAnim.SwitchController(true);
    }

    private void UnfreezeAnimation()
    {
        anim.SetTrigger("Ability");
    }
}
