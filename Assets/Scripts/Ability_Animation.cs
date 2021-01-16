﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animation Ability", menuName = "Abilities/Animation Ability")]
public class Ability_Animation : Ability
{
    private Actor_Player pA;
    private string triggerName;

    public override void Execute()
    {
        pA.Anim.SetTrigger(triggerName);
    }

    public override void Initialize(GameObject abilitySource)
    {
        pA = abilitySource.GetComponent<Actor_Player>();
    }
}