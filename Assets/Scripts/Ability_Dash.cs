using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Dash", menuName = "Abilities/Dash Ability")]
public class Ability_Dash : Ability
{

    private Actor_Player pA;
    private int dashLimit = 3;
    private int dashCounter = 0;
    private float dashDistance = 5.0f;

    public override void Execute()
    {
        base.Execute();
        // Do the dash here
        // Increment the dashCounter
        Vector3 destination = (pA.transform.forward * dashDistance);
        pA.Controller.Move(destination);

        dashCounter++;
            
    }

    public override bool CanExecute()
    {
        return (dashCounter < dashLimit);
    }

    public override void Initialize(GameObject abilitySource)
    {
        base.Initialize(abilitySource);
        pA = abilitySource.GetComponent<Actor_Player>();
        dashCounter = 0;

    }

    public override void OnCooldownEnd()
    {
        dashCounter--;
        if (dashCounter != 0)
        {
            cooldownTimer.PlayFromStart();
        }


        //throw new System.NotImplementedException();
    }

    public override void OnLifetimeEnd()
    {
       
    }
}
