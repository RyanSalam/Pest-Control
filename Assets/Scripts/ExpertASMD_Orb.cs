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

    public Color color;

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
        //if (other.gameObject.tag == "ASMDammo")
        if (other.transform.gameObject.layer != enemyMask || other.gameObject.tag == "ASMDammo") 
        {
            if (explosionEffect)
            {
                GameObject tempVFX = explosionEffect;
                //tempVFX.transform.localScale *=  2;
                GameObject tempExplosion = Instantiate(tempVFX, transform.position, transform.rotation);
                tempExplosion.transform.localScale *= 5f;
                Destroy(tempExplosion, 1.5f);
            }

            explode(transform.position);
        }
    }

    private void explode(Vector3 hitPoint)
    {
        //sphere cast for enemies
       

        Collider[] colliders = Physics.OverlapSphere(hitPoint, explosionRadius, enemyMask);

        foreach(Collider c in colliders)
        {
            Actor_Enemy tempEnemy = c.gameObject.GetComponent<Actor_Enemy>();

            if (tempEnemy)
                tempEnemy.TakeDamage(explosionDamage);

            ImpactSystem.Instance.DamageIndication(explosionDamage, color, c.gameObject.transform.position, Quaternion.LookRotation(transform.position - LevelManager.Instance.Player.transform.position));
        }

        Destroy(gameObject);
    }
}
