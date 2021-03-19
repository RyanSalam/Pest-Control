using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBomb : MonoBehaviour
{
    [SerializeField] LayerMask playerMask;
    [SerializeField] int damage = 50;
    [SerializeField] float radiusToPlayer = 5f;
    [SerializeField] GameObject explosionVFX;
  
    //on collisions - if we hit a trap increase their current uses (damaging them) - if player hurt them
    //else destroy it - so it can be killed by projectiles , need to look into how a raycast can affect it, may need to check on hitscan script
    private void OnCollisionEnter(Collision c)
    {
        Collider[] actorColliders = Physics.OverlapSphere(transform.position, radiusToPlayer, playerMask);

        //so we can hurt player if our bomb hits them
        if (actorColliders.Length > 0)
        {
            foreach (Collider col in actorColliders)
            {
                Actor damageBody = col.gameObject.GetComponent<Actor>();

                if (damageBody != null)
                    damageBody.TakeDamage(damage);
            }
        }
       
        //spawn VFX + destroy existing objects on all collisions
        spawnVFX();   
    }
    private void OnTriggerEnter(Collider other)
    {
        //traps use triggers so need seperate trigger function 
        if (other.gameObject.tag == "Trap")
        {
            Trap trap = other.gameObject.GetComponent<Trap>();
            trap.TakeDamage(damage);
        }
    }

    void spawnVFX()
    {
        //no vfx return else - spawn
        if (explosionVFX != null)
        {
            GameObject temp = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            temp.transform.localScale *= 5f;
            Destroy(temp, 0.15f);
            
        }
        //destroying entire object here because this will always be the last line read in this script. so destroy last, after we handle everything else
        Destroy(gameObject, 0.15f);
    }
}
