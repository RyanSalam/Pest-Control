using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Trap_Tesla : Trap
{
    //private bool isChaining = false;
    private int currentChainAmount = 0; // default chain value 
    [SerializeField] private int enemyChainAmount = 5; // setting how much enemy can be inflicted by lighting chain

    public float chainRadius = 5.0f; //radius of the chain
    [SerializeField] private LayerMask whatIsEnemy; //check enemy layer 
    private Actor_Enemy enemyTarget;
    public bool canAttack = true;
    private float timeAttack = 0.0f;
    private float attackDelay = 2f;
    
    public List<Actor_Enemy> enemies = new List<Actor_Enemy>();
    private LineRenderer lineRenderer;

    [SerializeField] private Transform chainOrigin;
    [SerializeField] private GameObject particleVFX;
    [SerializeField] private GameObject particleVFXAttacking;
    protected AudioCue ACue;

    [SerializeField] private DamageIndicator damageIndicator;
    protected override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.positionCount = enemyChainAmount;
        lineRenderer.enabled = false;
        Anim.SetBool("isIdle", true); 

        //initialize our pool
        ObjectPooler.Instance.InitializePool(particleVFX, 10);
        //Audio Settings
        ACue = GetComponent<AudioCue>();
    }

    private void FixedUpdate()
    {
        if (canAttack)
        {
            Collider[] colls = Physics.OverlapSphere(chainOrigin.position, chainRadius, whatIsEnemy);

            if (colls.Length <= 0) return;

            enemyTarget = colls[0].gameObject.GetComponent<Actor_Enemy>(); //enemy goes collides with sphere cast 

            if (enemyTarget != null)
            {
                canAttack = false;
                enemies.Add(enemyTarget);
                Activate();
            }
        }

        else
        {
            Debug.Log("Cannot attack so we call this");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(chainOrigin.position, chainRadius);
    }

    public override void Activate()
    {
        Debug.Log("Activate Tesla");
        canAttack = false;
        EnemySphereCast(enemyTarget); //activate chain sphere cast on attacked enemy
        ACue.PlayAudioCue();
        spawnVFX(); //spawning attack VFX 

        lineRenderer.enabled = true; //adjusting linerenders position based off the chain origin
        lineRenderer.SetPosition(0, chainOrigin.position);
        lineRenderer.positionCount = currentChainAmount + 1;
        for (int i = 1; i <= enemies.Count; i++) // adding enemies to the list and adding current chain amount 
        {
            enemyTarget = enemies[i - 1];            
            Vector3 position = enemies[i - 1].transform.position;
            position.y += 1.4f;
            ObjectPooler.Instance.GetFromPool(particleVFX, position, enemies[i - 1].transform.rotation);
            lineRenderer.SetPosition(i, position);
            Debug.Log("Enemy Position for Line Renderer is: " + position);
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
        Anim.SetBool("isAttacking", true);
        Anim.SetBool("isIdle", false);

        StartCoroutine(Cooldown());
        base.Activate();
        
    }

    private IEnumerator Cooldown() //cooldown for attacks
    {
        Debug.Log("Deactivate Tesla");
        yield return new WaitForSeconds(0.45f);

        lineRenderer.enabled = false;
        lineRenderer.positionCount = 1;

        yield return new WaitForSeconds(attackDelay);

        Debug.Log("Wait For Delay");


        enemies.Clear();
        canAttack = true;
        currentChainAmount = 0;
        enemyTarget = null;
        Anim.SetBool("isAttacking", false);
        Anim.SetBool("isIdle", true);
    }

    void EnemySphereCast(Actor_Enemy currentEnemy) //the base of the chain, creating sphere cast on another enemy so it can chain another enemy
    {
        if (currentEnemy == null) { return; }

        currentChainAmount++;

        Collider[] enemiesFound = Physics.OverlapSphere(currentEnemy.transform.position, chainRadius, whatIsEnemy);
        Actor_Enemy newEnemy = currentEnemy;

        foreach (Collider col in enemiesFound)
        {
            newEnemy = col.transform.GetComponent<Actor_Enemy>();
            if (!enemies.Contains(newEnemy))
            {
                newEnemy = col.transform.GetComponent<Actor_Enemy>();
                enemies.Add(newEnemy);
                break;
            }
        }        

        if (currentChainAmount <= enemyChainAmount && newEnemy != currentEnemy)
        {
            EnemySphereCast(newEnemy);
        }
    }

    void spawnVFX()
    {
        if (particleVFXAttacking != null)
        {
            GameObject temp = Instantiate(particleVFXAttacking, transform.position, Quaternion.identity);
            Destroy(temp, 2f); 
        }
    }
}




