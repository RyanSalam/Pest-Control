using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public void ExecuteAbility(object ability)
    {
        Ability abilityToCast = (Ability)ability;
        abilityToCast.Execute();
    }
}
