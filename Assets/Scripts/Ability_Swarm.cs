using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwarmAbility", menuName = "Abilities/Swarm Ability")]
public class Ability_Swarm : Ability
{
    [SerializeField] public float swarmMaxcharge = 2f;
    [SerializeField] public int maxNanoDrones = 10;
    [SerializeField] public float nanoDroneDamage = 10f;
    [SerializeField] public float nanoDroneAttackRadius = 10f;
    [SerializeField] public float nanoDroneLaunchDelay = 0.1f;
    [SerializeField] public float nanoDroneMovementSpeed = 1f;
    [SerializeField] public float nanoDroneRotationSpeed = 1f;

    private Actor_Player pA;

    public GameObject swarmMaster;

    public override void Initialize(GameObject abilitySource)
    {
        base.Initialize(abilitySource);
        pA = abilitySource.GetComponent<Actor_Player>();

    }

    public override void Execute()
    {
        base.Execute();

        Instantiate(swarmMaster, pA.AbilitySpawnPoint.position, pA.AbilitySpawnPoint.rotation);
    }

    public override void OnCooldownEnd()
    {

    }
    public override void OnLifetimeEnd()
    {
        //throw new System.NotImplementedException();
    }
    
}
