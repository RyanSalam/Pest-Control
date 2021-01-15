using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Projectile Ability")]
public class Ability_Projectile : Ability
{
    public GameObject prefabToSpawn;
    private Actor_Player pA;

    // Start is called before the first frame update
    public override void Execute()
    {
        Instantiate(prefabToSpawn, pA.AbilitySpawnPoint.position, pA.AbilitySpawnPoint.rotation);

    }

    public override void Initialize(GameObject abilitySource)
    {
        pA = abilitySource.GetComponent<Actor_Player>();
        
    }
}
