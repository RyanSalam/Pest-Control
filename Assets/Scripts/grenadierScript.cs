using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadierScript : MonoBehaviour
{
    Rigidbody projectile;
    public Rigidbody Projectile { get { return projectile; } }

    //counter to track our bounces
    [SerializeField] int bounceCount;

    //max counts for our grenade
    [SerializeField] int maxBounceCount;

    //scalar value for our addForce();
    [SerializeField] int projectileForce = 10;

    //explosion radius 
    [SerializeField] int radius = 15;

    [SerializeField] int damage = 10;

    LayerMask enemyLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        projectile = GetComponent<Rigidbody>();
        bounceCount = 0;

        maxBounceCount = 2;

        enemyLayerMask = LayerMask.GetMask("Enemy");

        projectile.AddForce(transform.forward * projectileForce, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Vector3 newDirection = Vector3.Reflect(transform.position, collision.GetContact(0).normal);
       

        projectile.AddForce(collision.GetContact(0).normal * projectileForce, ForceMode.Impulse);

        bounceCount += 1;

        if (bounceCount >= maxBounceCount)
        {
            damageEnemies();
            Destroy(gameObject, 0.25f);
        }
    }

    void damageEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemyLayerMask);

        if (colliders != null)
        {
            foreach (Collider c in colliders)
            {
                Debug.Log("Damaging enemies");
                c.gameObject.GetComponent<Actor_Enemy>().TakeDamage(damage);
            }
        }
    }
}
