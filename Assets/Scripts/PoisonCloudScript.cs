using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloudScript : MonoBehaviour
{
    //variables for our posion damage
    float damageAmount = 1f;
    float timeSinceLastDamage = 0f;
    float damageDelay = 2f;
    float projectileForce = 10.0f;

    //our max lifetime
    float lifetime = 60.0f;
    float timer = 0;

    //Overlap-sphere variables
    float radius = 30f;
    LayerMask layerMask;

    //bool to see if we have hit something -> time to deploy poison
    bool isDeployingPoison = false;

    //enemies to damage array
    Collider[] colliders;
    Actor_Enemy[] enemiesToDamage;
    List<Actor_Enemy> enemyList = new List<Actor_Enemy>();

    Rigidbody projectile;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Enemy");

        isDeployingPoison = false;

        timer = 0;

        projectile = GetComponent<Rigidbody>();

        projectile.AddForce(transform.forward * projectileForce, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDeployingPoison)
        {
           
            //increment our timer
            timer += Time.deltaTime;

            if (Time.time > timeSinceLastDamage + damageDelay)
            {
                //recheck our list
                EnemyListCheck();

                //reset our clock
                timeSinceLastDamage = Time.time;

                if (enemyList != null)
                {
                    //damage enemy
                    foreach (Actor_Enemy enemy in enemyList)
                    {
                        Debug.Log("Enemy taking damage");
                        enemy.TakeDamage(damageAmount);
                    }
                }
            }

            //lifetime check
            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        //toggle our bool so our posion logic in update kicks in
        isDeployingPoison = true;

        projectile.constraints = RigidbodyConstraints.FreezeAll;
    }


    void EnemyListCheck()
    {
        //search for enemies in our physics.overlap
        colliders = Physics.OverlapSphere(transform.position, radius, layerMask);

        int enemyCount = 0;

        //clear existing enemies - re assigning
        enemyList.Clear();

        //load all our colliders into our enemies to damage array by grabbing the component
        foreach (Collider c in colliders)
        {
            enemyList.Add(c.gameObject.GetComponent<Actor_Enemy>());
            
            enemyCount++;

            Debug.Log("Enemy found, enemyList = " + enemyList.Count);
        }

    }
}
