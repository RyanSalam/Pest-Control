using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DroneAbility", menuName = "Abilities/Drone Ability")]
public class Ability_Drone : Ability
{
    [Header("Attack Attributes")]
    [SerializeField] public int shots = 25;
    [SerializeField] public int roundPerMinute = 180;
    [SerializeField] public float damage = 20f;
    [SerializeField] public float kamikazeSelfDestructTime = 5f;
    [SerializeField] public float kamikazeSearchRange = 25f;

    [Header("Detection Attributes")]
    [SerializeField] public float targetDistance = 10f;
    [SerializeField] public float attackRange = 7f;

    [Header("Movement Attributes")]
    [SerializeField] public float playerStoppingDistance = 5f;
    [SerializeField] public float enemyStoppingDistance = 3f;
    [SerializeField] public float movementSpeed = 5f;
    [SerializeField] public float kamikazeMovementSpeed = 15f;

    [Header("Explosion Attributes")]
    [SerializeField] public float explosionDamage = 200f;
    [SerializeField] public float explosionRadiusMin = 5f;
    [SerializeField] public float explosionRadiusMax = 15f;


    private Actor_Player pA;
    public GameObject Drone;

    GameObject thisDrone;

    public override void Initialize(GameObject abilitySource)
    {
        base.Initialize(abilitySource);
        pA = abilitySource.GetComponent<Actor_Player>();
        Debug.Log("Here");
    }

    public override void Execute()
    {
        base.Execute();
        Debug.Log("aggg");
        // If no drone exists, spawn a new one
        if(thisDrone == null) thisDrone = Instantiate(Drone, pA.AbilitySpawnPoint.position, pA.AbilitySpawnPoint.rotation);
        // If drone exist, begin kamikaze
        else
        {
            thisDrone.GetComponent<Animator>().SetTrigger("Explode");
        }
    }

    public override void OnCooldownEnd()
    {

    }
    public override void OnLifetimeEnd()
    {
        //throw new System.NotImplementedException();
    }
}
