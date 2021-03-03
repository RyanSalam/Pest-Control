using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpinKick", menuName = "Abilities/SpinKick Ability")]

public class Ability_SpinKick : Ability

{
    

    [Header("Invulnerability")]
    [SerializeField] public float playerDamage = 0f;
    [SerializeField] public float playerHealth = 0f;





    [Header("Kick Force")]
    [SerializeField] public float kickForce = 10f;
    [SerializeField] public float playerSpeed = 0f;
    [SerializeField] public float enemyKickbackDistance = 20f;

    [Header("Spin Speed")]
    [SerializeField] public float speed = 30f;
    [SerializeField] public float rotation = 360f;
    


    [Header("Attack Radius")]
    [SerializeField] public float attackRadius = 15f;
    [SerializeField] public float pushEnemy = 40f;
    

    [Header("Kick Damage")]
    [SerializeField] public float enemyDamage = 10f;
    [SerializeField] LayerMask enemies;
    [SerializeField] Collider[] enemiesHit;


    private readonly enemyLocation enemyKickbackDistanc;
    private Actor_Player pA;
    private GameObject spinkKick;


    
    public override void Initialize(GameObject abilitySource)
    {
        base.Initialize(abilitySource);
        pA = abilitySource.GetComponent<Actor_Player>();
        Debug.Log("Here");
    }


    public override void Execute()
{
    base.Execute();
    
    if (spinkKick == null)  Instantiate(spinkKick, pA.AbilitySpawnPoint.position, pA.AbilitySpawnPoint.rotation);
    
   
}
    private void Update()
    {
        LookForEnemies();
    }


    private void LookForEnemies()
    {
        

        if (enemiesHit.Length > 0)
        {
            foreach (Collider enemy in enemiesHit)
            {
                Debug.Log(enemy.name + " is being slowed down");
                enemy.GetComponent<Actor_Enemy>().Agent.angularSpeed *= enemyKickbackDistance;
            }
            //Invoke("ResetEnemyposition", enemyKickbackDistance);
        }
    }

    
    private void ResetEnemyPosition()
    {
        foreach (Collider enemy in enemiesHit)
        {
            enemy.GetComponent<Actor_Enemy>().Agent.speed += enemyKickbackDistance;
        }
    }

    private void Invoke(string v, float enemyKickbackDistance)
    {
        throw new NotImplementedException();
    }

    public override void OnCooldownEnd()
    {
        throw new System.NotImplementedException();
    }

    public override void OnLifetimeEnd()
    {
        throw new System.NotImplementedException();
    }
}
