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
    [SerializeField] private int enemyChainAmount = 10; // setting how much enemy can be inflicted by lighting chain

    public float chainRadius = 5.0f; //radius of the chain
    [SerializeField] private LayerMask whatIsEnemy; //check enemy layer 
    private Actor_Enemy enemyTarget;
    public bool canAttack = true;
    private float timeAttack = 0.0f;
    private float attackDelay = 1f;
    private List<Actor_Enemy> enemies = new List<Actor_Enemy>();
    private LineRenderer lineRenderer;

    [SerializeField] private Transform chainOrigin;
    [SerializeField] private GameObject particleVFX;

    protected override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.positionCount = enemyChainAmount;
        lineRenderer.enabled = false;
    }

    protected override void Update()
    {
        if (Physics.SphereCast(chainOrigin.position, chainRadius, chainOrigin.forward, out RaycastHit hit, 1f, whatIsEnemy) && canAttack) //searching every frame using sphere cast for an enemy
        {
            enemyTarget = hit.collider.gameObject.GetComponent<Actor_Enemy>(); //enemy goes collides with sphere cast 

            if (enemyTarget != null)
            {
                enemies.Add(enemyTarget);
                Debug.Log("enemy target not null");
                Debug.DrawLine(enemyTarget.transform.position, hit.point);
                Activate(); //activate the trap when there is an enemy in the collider 
                timeAttack = Time.time;
                canAttack = false;
            }
        }

        base.Update();
    }


    public override void Activate()
    {
        EnemySphereCast(enemyTarget); //activate chain sphere cast on attacked enemy

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, chainOrigin.position);
        lineRenderer.positionCount = currentChainAmount + 1;

        for (int i = 1; i < currentChainAmount + 1; i++)
        {
            Vector3 position = enemies[i - 1].transform.position;
            //position = GetHeightOffset(position);
            lineRenderer.SetPosition(i, position);
        }

        foreach (Actor_Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(trapDamage);
                Instantiate(particleVFX, enemy.transform.position, enemy.transform.rotation);

                //enemies.Remove(enemy);
            }
        }
        base.Activate();
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
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

    void EnemySphereCast(Actor_Enemy currentEnemy)
    {
        if (currentEnemy == null) { return; }

        currentChainAmount++;

        // currentEnemy.TakeDamage(trapDamage);

        RaycastHit[] hits;

        hits = Physics.SphereCastAll(currentEnemy.transform.position, chainRadius, currentEnemy.transform.forward, 1, whatIsEnemy);

        Actor_Enemy newEnemy = currentEnemy;

        foreach (RaycastHit enemyHit in hits)
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
        if (currentChainAmount == enemyChainAmount)
        {
            currentChainAmount = 0;
        }
        }

        /*if (Physics.SphereCast(currentEnemy.transform.position, chainRadius, currentEnemy.transform.forward, out RaycastHit hit)) //&& currentEnemy != enemyTarget)
        {
            //currentEnemy.TakeDamage(trapDamage);

            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                
                enemyTarget = hit.collider.gameObject.GetComponent<Actor_Enemy>();
                Debug.Log("Chaining Sphere");
                Debug.DrawLine(currentEnemy.transform.position, hit.point);
                if(!enemies.Contains(enemyTarget))
                {
                    enemies.Add(enemyTarget);
                }
                //enemyTarget.TakeDamage(trapDamage);*/

        /*if (currentChainAmount < enemyChainAmount)
        {
            EnemySphereCast(enemyTarget);

            if (currentChainAmount == enemyChainAmount)
            {
                currentChainAmount = 0;
            }
        }*/
    
}
    
    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemyTarget.transform.position, chainRadius);

    }*/
    

