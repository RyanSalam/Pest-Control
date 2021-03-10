using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Trap_Tesla : Trap
{
    private bool isChaining = false;
    private int currentChainAmount = 0; // default chain value 
    [SerializeField] private int enemyChainAmount = 5; // setting how much enemy can be inflicted by lighting chain

    public float chainRadius = 5.0f; //radius of the chain
    [SerializeField] private LayerMask whatIsEnemy; //check enemy layer 
    private Actor_Enemy enemyTarget;
    public bool canAttack = true;
    private float timeAttack = 0.0f;
    private float attackDelay = 0.3f;
    private List<Actor_Enemy> enemies = new List<Actor_Enemy>();
    private LineRenderer lineRenderer;

    [SerializeField] private Transform chainOrigin;
    [SerializeField] private GameObject particleVFX;

    [SerializeField] private DamageIndicator damageIndicator;
    protected override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.positionCount = enemyChainAmount;
        lineRenderer.enabled = false;
        //Anim.SetBool("isIdle", true); //uncomment these when tesla trap animations is set on animator and applied

        //initialize our pool
        ObjectPooler.Instance.InitializePool(particleVFX, 10);
    }

    protected override void Update()
    {
        if (Physics.SphereCast(chainOrigin.position, chainRadius, chainOrigin.forward, out RaycastHit hit, 1f, whatIsEnemy) && canAttack) //searching every frame using sphere cast for an enemy
        {
            enemyTarget = hit.collider.gameObject.GetComponent<Actor_Enemy>(); //enemy goes collides with sphere cast 

            if (enemyTarget != null) // if there is an enemy, add it to the list, attack!
            {
                enemies.Add(enemyTarget);
                Debug.Log("enemy target not null");
                Debug.DrawLine(enemyTarget.transform.position, hit.point);
                Activate(); //activate the trap when there is an enemy in the collider 
                timeAttack = Time.time;
                canAttack = false;
            }
        }

        base.Update(); //base trap update
    }


    public override void Activate()
    {
        EnemySphereCast(enemyTarget); //activate chain sphere cast on attacked enemy

        lineRenderer.enabled = true; //adjusting linerenders position based off the chain origin
        lineRenderer.SetPosition(0, chainOrigin.position);
        lineRenderer.positionCount = currentChainAmount + 1;
        for (int i = 1; i < currentChainAmount + 1; i++) // adding enemies to the list and adding current chain amount 
        {
            
            Vector3 position = enemies[i - 1].transform.position;
            position.y += 1.4f;
            ObjectPooler.Instance.GetFromPool(particleVFX, position, enemies[i - 1].transform.rotation);
            //Instantiate(particleVFX, position, enemies[i - 1].transform.rotation);
            lineRenderer.SetPosition(i, position);
        }

        foreach (Actor_Enemy enemy in enemies)
        {
            if (enemy != null)
            {
               

                enemy.TakeDamage(trapDamage);
                ImpactSystem.Instance.DamageIndication(trapDamage, trapColor, enemy.transform.position, Quaternion.LookRotation(LevelManager.Instance.Player.transform.position - enemy.transform.position));
                 // when an enemy is hit (above line) spawn particle. need to spawn particle above feet...

                //enemies.Remove(enemy);
            }
        }
        base.Activate();
        StartCoroutine(Cooldown());
        //Anim.SetBool("isAttacking", true); 
    }

    private IEnumerator Cooldown() //cooldown for attacks
    {
        yield return new WaitForSeconds(attackDelay);

        lineRenderer.enabled = false;
        //enemies = new List<Actor_Enemy>();
        lineRenderer.positionCount = 1;
        canAttack = true;

        currentChainAmount = 0;
        enemyTarget = null;

        base.Activate();
    }

    void EnemySphereCast(Actor_Enemy currentEnemy) //the base of the chain, creating sphere cast on another enemy so it can chain another enemy
    {
        if (currentEnemy == null) { return; }

        currentChainAmount++;

        // currentEnemy.TakeDamage(trapDamage);

        RaycastHit[] hits;

        hits = Physics.SphereCastAll(currentEnemy.transform.position, chainRadius, currentEnemy.transform.forward, 1, whatIsEnemy); //when hits, spherecast on the current enemy to search for new enemies to chain

        Actor_Enemy newEnemy = currentEnemy;

        foreach (RaycastHit enemyHit in hits) //hit new enemies you have not previously hit
        {
            // newEnemy = enemyHit.transform.GetComponent<Actor_Enemy>();
            if (!enemies.Contains(newEnemy))
            {
                newEnemy = enemyHit.transform.GetComponent<Actor_Enemy>();
                enemies.Add(newEnemy);
                break;
            }
        }
        if (currentChainAmount <= enemyChainAmount && newEnemy != currentEnemy)
        {
            EnemySphereCast(newEnemy);
        }
        if (currentChainAmount == enemyChainAmount) //reset chain amount
        {
            currentChainAmount = 0;
        }
    }
}

        
    

