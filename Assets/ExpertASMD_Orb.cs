using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpertASMD_Orb : MonoBehaviour
{
    Rigidbody rb;
    public float force = 5;

    [SerializeField] GameObject explosionEffect;
    [SerializeField] float explosionRadius; 
    [SerializeField] float explosionDamage;
    [SerializeField] LayerMask enemyMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        if (rb)
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        //explosionss???
        if (other.gameObject.tag == "ASMDammo")
        {
            Debug.Log("explosionss???");
           
            if (explosionEffect)
            {
                GameObject tempVFX = explosionEffect;
                tempVFX.transform.localScale *= 3;
                GameObject tempExplosion = Instantiate(explosionEffect, transform.position, transform.rotation);
                Destroy(tempExplosion, 1.5f);
            }

            explode(transform.position);
        }
    }

    private void explode(Vector3 hitPoint)
    {
        Debug.Log("yes explosionss");
        //sphere cast for enemies
        Destroy(gameObject);

        Collider[] colliders = Physics.OverlapSphere(hitPoint, explosionRadius, enemyMask);

        foreach(Collider c in colliders)
        {
            Actor_Enemy tempEnemy = c.gameObject.GetComponent<Actor_Enemy>();

            if (tempEnemy)
                tempEnemy.TakeDamage(explosionDamage);
        }
    }
}
