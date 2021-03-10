using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camsTeslaTrap : MonoBehaviour
{
    //when an enemy walks in our range we attack
    //then from that enemys position search for new enemies and attack them

    public LayerMask enemyMask;

    float maxChainCount = 5;
    float chainRadius = 10f;
    float timeAtAttack = 0;
    float attackDelay = 3f;
    float damageAmount = 35f;

    public GameObject trapVFX;

    Collider[] hitColliders;
    Actor_Enemy[] enemies;
    public Queue<Actor_Enemy> enemyQueue = new Queue<Actor_Enemy>();
    // Start is called before the first frame update
    void Start()
    {
        enemyQueue = new Queue<Actor_Enemy>();
        timeAtAttack = 0;

        ObjectPooler.Instance.InitializePool(trapVFX, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeAtAttack + attackDelay)
        {
            hitColliders = Physics.OverlapSphere(transform.position, chainRadius, enemyMask);
            if (hitColliders.Length > 0)
                attack();           
        }
    }

    void attack()
    {
        Debug.Log("attacking enemy");
        timeAtAttack = Time.time;

        
        foreach(Collider c in hitColliders)
        {
            if (!enemyQueue.Contains(c.gameObject.GetComponent<Actor_Enemy>()))
                   enemyQueue.Enqueue(c.gameObject.GetComponent<Actor_Enemy>());
        }

        enemies = enemyQueue.ToArray();

        Debug.DrawLine(transform.position, enemies[0].transform.position, Color.blue,10f);

        for (int i = 1; i < enemies.Length; i++)
        {
            Debug.DrawLine(enemies[i - 1].transform.position, enemies[i].transform.position, Color.red, 10f);
            //Debug.DrawLine(enemyQueue.Peek().transform.position, transform.position, Color.red,10f);
            ObjectPooler.Instance.GetFromPool(trapVFX, enemies[i].transform.position, Quaternion.identity);
            
            Debug.Log("Chaining from enemy: " + enemies[i - 1] + " TO: " + enemies[i]);
            enemies[i].TakeDamage(damageAmount);

            Vector3 newPos = enemies[i].transform.position;
            newPos.y += 1.5f;
            ImpactSystem.Instance.DamageIndication(damageAmount, Color.cyan, newPos,
                Quaternion.LookRotation(enemies[i].transform.position - LevelManager.Instance.Player.transform.position));
        }
    }
}
