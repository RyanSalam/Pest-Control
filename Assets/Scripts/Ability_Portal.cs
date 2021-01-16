using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Portal Ability")]
public class Ability_Portal : Ability
{
    public GameObject prefabToSpawn;
    private Actor_Player pA;
    public override void Execute()
    {
        Instantiate(prefabToSpawn, pA.AbilitySpawnPoint.position, pA.AbilitySpawnPoint.rotation);

    }

    public override void Initialize(GameObject abilitySource)
    {
       pA = abilitySource.GetComponent<Actor_Player>();

    }
}
