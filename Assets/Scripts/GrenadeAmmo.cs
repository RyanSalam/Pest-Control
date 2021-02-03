using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAmmo : MonoBehaviour
{
    [SerializeField] LayerMask enemyMask;

    public float radius;
    public float damage;

    [SerializeField] GameObject explosionVFX;

    private void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy");
    }

    private void OnCollisionEnter(Collision c)
    {
        //here we would add VFX 
        GameObject tempVFX = Instantiate(explosionVFX, transform.position, transform.rotation);
        tempVFX.transform.localScale *= 5f;
        Destroy(tempVFX, 0.15f);
        //pass the point the grenade hit something
        damageEnemies(this.transform.position);
 
    }

    void damageEnemies(Vector3 hitPoint)
    {
        //overlap sphere to try and find enemies
        Collider[] colliders = Physics.OverlapSphere(hitPoint, radius, enemyMask);

        foreach (Collider c in colliders)
        {
            Actor_Enemy temp = c.gameObject.GetComponent<Actor_Enemy>();

            if (temp != null)
            {
                Debug.Log("Enemy taking damage");
                temp.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
