using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RyderDroneScript : Actor
{
    int shotsRemaining = 25;
    float attackCooldown = 0.33f;
    float damage = 20f;
    float currentAttackCooldown = 0f;
    float kamikazeSearchRange = 25f;
    float kamiSelfDestruct = 5f;

    float targetDistance = 10f;
    public float attackRange = 7f;

    float explosionDamage = 200f;
    float explosionRadiusMin = 5f;
    float explosionRadiusMax = 15f;

    public Actor_Enemy currentTarget = null;

    public NavMeshAgent agent;
    public GameObject pA;

    Collider[] enemyInRange;
    Collider[] enemyInExplosionRange;
    [SerializeField] LayerMask enemies;
    [SerializeField] GameObject GroundFire;

    public Ability_Drone master;

    protected override void Start()
    {
        base.Start();
        pA = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        shotsRemaining = master.shots;
        attackCooldown = (float)60 / master.roundPerMinute;
        damage = master.damage;
        kamiSelfDestruct = master.kamikazeSelfDestructTime;
        kamikazeSearchRange = master.kamikazeSearchRange;
        targetDistance = master.targetDistance;
        attackRange = master.attackRange;
        agent.stoppingDistance = master.playerStoppingDistance;
        agent.speed = master.movementSpeed;
        explosionDamage = master.explosionDamage;
        explosionRadiusMin = master.explosionRadiusMin;
        explosionRadiusMax = master.explosionRadiusMax;
    }

    private void Update()
    {
        if (currentAttackCooldown < attackCooldown) currentAttackCooldown += Time.deltaTime;
    }

    public void Attack(Actor_Enemy target)
    {
        currentAttackCooldown = 0f;
        target.TakeDamage(damage);
        shotsRemaining--;
        if (shotsRemaining == 0) Explode();
        // Play sound, yada yada do whatever the fuck else

    }

    public Actor_Enemy FindPrimeTarget()
    {
        Collider[] kamikazeSearch = Physics.OverlapSphere(transform.position, kamikazeSearchRange, enemies);
        Actor_Enemy primeTarget = null;
        int affectedEnemies = 0;
        if(kamikazeSearch.Length == 0) return null;
        else
        {
            for(int i = 0; i < kamikazeSearch.Length; i++)
            {
                Collider[] temp = Physics.OverlapSphere(kamikazeSearch[i].transform.position, explosionRadiusMax, enemies);
                if (temp.Length > affectedEnemies)
                {
                    primeTarget = kamikazeSearch[i].GetComponent<Actor_Enemy>();
                    affectedEnemies = temp.Length;
                }
            }
        }
        return primeTarget;
    }

    public void Explode()
    {
        // damage - Distance from min * step factor (damage / (max - min))
        enemyInExplosionRange = Physics.OverlapSphere(transform.position, explosionRadiusMax, enemies);
        if (enemyInExplosionRange.Length > 0)
        {
            // Step factor - Damage reduction per unit of distance from target
            float stepFactor = explosionDamage / (explosionRadiusMax - explosionRadiusMin);
            //Debug.Log("Step: " + stepFactor);
            for(int i = 0; i < enemyInExplosionRange.Length; i++)
            {
                // Get distance
                float dist = Vector3.Distance(transform.position, enemyInExplosionRange[i].transform.position);
                // Min - distance * step + damage
                float relativeDist = explosionRadiusMin - dist;
                float distancePenalty = relativeDist * stepFactor;
                float finalDamage = distancePenalty + explosionDamage;
                // Cap damage if target is closer than minimum range
                if (finalDamage > explosionDamage) finalDamage = explosionDamage;

                enemyInExplosionRange[i].GetComponent<Actor_Enemy>().TakeDamage(finalDamage);
            }
        }
        // Used to spawn fire on the ground
        RaycastHit findGround;
        Physics.Raycast(transform.position, Vector3.down, out findGround, 10f);
        Instantiate(GroundFire, findGround.transform.position, transform.rotation);
        master.DroneFinished();
    }

    public Actor_Enemy CheckForTargets()
    {
        enemyInRange = Physics.OverlapSphere(transform.position, targetDistance, enemies);
        if (enemyInRange.Length > 0) return enemyInRange[0].GetComponent<Actor_Enemy>();
        else return null;
    }

    public bool ReadyToFire()
    {
        if (currentAttackCooldown >= attackCooldown && shotsRemaining > 0) return true;
        else return false;
    }
}

