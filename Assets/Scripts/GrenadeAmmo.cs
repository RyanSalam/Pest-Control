using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAmmo : MonoBehaviour
{
    public LayerMask layermask;

    public float radius;
    public float damage;

    AudioCue ac;
    Weapon weapon;

    [SerializeField] GameObject explosionVFX;

    private void Start()
    {
        ac = GetComponent<AudioCue>();
        layermask = LayerMask.GetMask("Enemy");
    }
    private void OnCollisionEnter(Collision c)
    {
        //here we would add VFX 
        //also call a damage enemy function to see results
        if (explosionVFX != null)
        {
            GameObject tempVFX = Instantiate(explosionVFX, transform.position, transform.rotation);
            tempVFX.transform.localScale *= 5;
            Destroy(tempVFX, 0.15f);
        }

        ac.PlayAudioCue();


        //pass the point the grenade hit something
        damageEnemies(transform.position);
       
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
                temp.TakeDamage(damage);
                Vector3 damagePos = new Vector3(hitPoint.x, hitPoint.y + 1.25f, hitPoint.z);
                ImpactSystem.Instance.DamageIndication(damage, Color.red, damagePos, Quaternion.LookRotation(-hitPoint.normalized));
            }
        }

        Destroy(gameObject, 0.15f);
    }
}
