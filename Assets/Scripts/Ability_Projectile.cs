using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Projectile Ability")]
public class Ability_Projectile : Ability
{
    public GameObject prefabToSpawn;
    public GameObject spawnedProjectile;
    private Actor_Player pA;

    [SerializeField] protected float lifeTime = 5f;

    // Start is called before the first frame update
    public override void Execute()
    {
        base.Execute();
        spawnedProjectile = Instantiate(prefabToSpawn, pA.AbilitySpawnPoint.position, pA.AbilitySpawnPoint.rotation);

    }

    public override void Initialize(GameObject abilitySource)
    {
        base.Initialize(abilitySource);
        pA = abilitySource.GetComponent<Actor_Player>();
        
    }

    public override bool CanExecute()
    {
        return spawnedProjectile == null;
    }

    public override void OnCooldownEnd()
    {
        Destroy(spawnedProjectile);
    }
}
