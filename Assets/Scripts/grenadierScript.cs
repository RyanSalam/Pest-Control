using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadierScript : MonoBehaviour
{
    Rigidbody projectile;
    public Rigidbody Projectile { get { return projectile; } }

    //counter to track our bounces
    int bounceCount;

    //max counts for our grenade
    int maxBounceCount;

    //scalar value for our addForce();
    int projectileForce = 10;

    //explosion radius
    int radius = 10;

    //damage on explosion
    int damage = 10;

    //enemy LayerMask
    LayerMask enemyMask;

    //our array of enemies we need to damage on explosion
    Actor_Enemy[] enemiesToDamage;

    // Start is called before the first frame update
    void Start()
    {
        projectile = GetComponent<Rigidbody>();
        
        bounceCount = 0;

        maxBounceCount = 2;

        enemyMask = LayerMask.GetMask("Enemy");

        projectile.AddForce(transform.forward * projectileForce, ForceMode.Impulse);

        enemiesToDamage = new Actor_Enemy[20];
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        //Vector3 newDirection = Vector3.Reflect(transform.position, collision.GetContact(0).normal);
       

        projectile.AddForce(collision.GetContact(0).normal * projectileForce, ForceMode.Impulse);

        bounceCount += 1;

        if (bounceCount >= maxBounceCount)
        {
            //here is where we would start the explosion VFX (or in function)

            DamageEnemies();

            Destroy(gameObject);
        }
    }

    private void DamageEnemies()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemyMask);
      

        int enemyCount = 0;

        if (colliders.Length > 0)
        {
            foreach (Collider c in colliders)
            {
                enemiesToDamage[enemyCount] = colliders[enemyCount].GetComponentInParent<Actor_Enemy>();

                enemiesToDamage[enemyCount].TakeDamage(damage);

                enemyCount++;

            }
        }
    }
}
