using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloudScript : MonoBehaviour
{
    //variables for our posion damage
    float damageAmount = 1.0f;
    float timeSinceLastDamage = 0f;
    float damageDelay = 2f;

    //our max lifetime
    float lifetime = 60.0f;
    float timer = 0;

    //Overlap-sphere variables
    float radius = 300f;
    LayerMask enemyLayerMask;

    //bool to see if we have hit something -> time to deploy poison
    bool isDeployingPoison = false;

    //enemies to damage array
    Collider[] colliders;
    [SerializeField] Actor_Enemy[] enemiesToDamage;

    //Rigidbody reference
    Rigidbody projectile;
    int projectileForce = 7;

    // Start is called before the first frame update
    void Start()
    {
        enemyLayerMask = LayerMask.GetMask("Enemy");

        timer = 0;

        projectile = GetComponent<Rigidbody>();

        projectile.AddForce(transform.forward * projectileForce, ForceMode.Impulse);

        enemiesToDamage = new Actor_Enemy[20];
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
                
                //Debug.Log("Enemy taking damage");
                
                if (enemiesToDamage.Length > 0)
                {                  
                    //damage enemy
                    foreach (Actor_Enemy enemy in enemiesToDamage)
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
        colliders = Physics.OverlapSphere(transform.position, radius, enemyLayerMask);

        int enemyCount = 0;

        //load all our colliders into our enemies to damage array by grabbing the component
        if (colliders.Length > 0)
        {
            foreach (Collider c in colliders)
            {
                enemiesToDamage[enemyCount] = colliders[enemyCount].GetComponentInParent<Actor_Enemy>();
                
                enemyCount++;
            }
            
        }
        
    }
}
