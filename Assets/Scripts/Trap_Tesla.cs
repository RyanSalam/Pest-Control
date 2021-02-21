using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Tesla : Trap
{
    private bool isChaining = false;
    private int currentChainAmount = 0; // deafult chain value 
    [SerializeField] private int enemyChainAmount = 4; // setting how much enemy can be infliceted by lighting chain
    public float chainRadius = 5.0f; //raidus of the chain
    [SerializeField] private LayerMask whatIsEnemy; //check enemy layer 
    private Actor_Enemy enemyTarget;
    public bool canAttack = true;
    private float timeAttack = 0.0f;
    private float attackDelay = 2.0f;

    
    protected override void Update()
    {
        if (Physics.SphereCast(transform.position, chainRadius, transform.forward, out RaycastHit hit, whatIsEnemy) && canAttack)
        {
            enemyTarget = hit.collider.gameObject.GetComponent<Actor_Enemy>();
            Activate();
            timeAttack = Time.time;
            canAttack = false;
        }
        if (Time.time > timeAttack + attackDelay)
        {
            canAttack = true;
           
        }
        base.Update();
    }
    
        
    
    public override void Activate()
    {
        EnemySphereCast(enemyTarget);
        base.Activate();

    }
    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.tag == "Enemy") //if the enemy actor collides with trap
        {
            
            enemyTarget = trigger.gameObject.GetComponent<Actor_Enemy>();
            if (enemyTarget != null)
            {
                Activate(); //when triggered activate
                //Physics.SphereCast()
                enemyTarget.TakeDamage(trapDamage);
            }
        }
    }

    void EnemySphereCast(Actor_Enemy currentEnemy)
    {
        
        currentChainAmount++;
        currentEnemy.TakeDamage(trapDamage);
        
        if (Physics.SphereCast(currentEnemy.transform.position, chainRadius, currentEnemy.transform.forward, out RaycastHit hit))
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                enemyTarget = hit.collider.gameObject.GetComponent<Actor_Enemy>();
                Debug.Log("Chaining Sphere");
                
                if (currentChainAmount <= enemyChainAmount)
                {
                    EnemySphereCast(enemyTarget);
                   
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemyTarget.transform.position, chainRadius);

    }
}
