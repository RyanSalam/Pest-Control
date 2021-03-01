using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoobToobAttachment : MonoBehaviour
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

    public GameObject explosionVFX;
    AudioCue ac;

    LayerMask enemyLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        projectile = GetComponent<Rigidbody>();
        bounceCount = 0;

        maxBounceCount = 2;

        enemyLayerMask = LayerMask.GetMask("Enemy");
        ac = GetComponent<AudioCue>();
        projectile.AddForce(transform.forward * projectileForce, ForceMode.Impulse);
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Vector3 newDirection = Vector3.Reflect(transform.position, collision.GetContact(0).normal);


        projectile.AddForce(collision.GetContact(0).normal * projectileForce, ForceMode.Impulse);

        bounceCount += 1;

        if (bounceCount >= maxBounceCount)
        {
            if (explosionVFX != null)
            {
                GameObject tempVFX = Instantiate(explosionVFX, transform.position, transform.rotation);
                tempVFX.transform.localScale *= 5f;
                Destroy(tempVFX, 0.15f);
            }
            damageEnemies();

        }

        ac.PlayAudioCue();
    }

    void damageEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemyLayerMask);

        if (colliders.Length > 0)
        {
            foreach (Collider c in colliders)
            {
                //temporary actor enemy so we can null check
                Actor_Enemy temp = c.gameObject.GetComponent<Actor_Enemy>();

                //if the enemyActor component exists damage it
                if (temp != null)
                {
                    //Debug.Log("damaging enemies");
                    temp.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject, 0.25f);
    }
}
