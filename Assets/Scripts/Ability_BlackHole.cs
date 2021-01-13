using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_BlackHole : Ability
{

    public GameObject prefabToSpawn;
    public Actor_Player pA;
    
    // Start is called before the first frame update
    public override void Execute()
    {
        Instantiate(prefabToSpawn, pA.AbilitySpawnPoint.position, pA.AbilitySpawnPoint.rotation);
        
    }

    public override void Initialize(GameObject abilitySource)
    {
        pA = abilitySource.GetComponent<Actor_Player>();
        throw new System.NotImplementedException();
    }
}
