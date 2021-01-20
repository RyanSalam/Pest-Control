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

    // Start is called before the first frame update
    void Start()
    {
        projectile = GetComponent<Rigidbody>();
        bounceCount = 0;

        maxBounceCount = 3;

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
            Debug.Log("Kaboom?");
            Destroy(gameObject);
        }
    }
}
