using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAmmo : MonoBehaviour
{
    public LayerMask layermask;

    public float radius;
    public float damage;

    [SerializeField] ParticleSystem explosionVFX;

    private void Start()
    {
        layermask = LayerMask.GetMask("Enemy");
    }
    private void OnCollisionEnter(Collision c)
    {
        //here we would add VFX 
        //also call a damage enemy function to see results
        
        //pass the point the grenade hit something
        damageEnemies(c.GetContact(0).normal);
        
        Destroy(gameObject, 0.15f);

    }

    void damageEnemies(Vector3 hitPoint)
    {
        Instantiate(explosionVFX, hitPoint, Quaternion.identity);
        //overlap sphere to try and find enemies
        Collider[] colliders = Physics.OverlapSphere(hitPoint, radius, layermask);

        foreach (Collider c in colliders)
        {
            Actor_Enemy temp = c.gameObject.GetComponent<Actor_Enemy>();

            if (temp != null)
            {
                Debug.Log("Enemy taking damage");
                temp.TakeDamage(damage);
            }
        }
    }
}
