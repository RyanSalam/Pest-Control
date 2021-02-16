using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackEvent : MonoBehaviour
{
    Actor_Enemy thisEnemy;
    int damage;
    private void Start()
    {
        // Assign the enemy reference accordingly.
        thisEnemy = GetComponentInParent<Actor_Enemy>();
        damage = (int)thisEnemy.Damage;
        enabled = false;
    }

    public void StartAttack()
    {
        // Enable the attack box;
        enabled = true;

        // Initialise damage data according to the enemy's properties.

    }

    private void Update()
    {
        //// Make an artifical trigger box that will damage any players within it.
        Collider[] targets = Physics.OverlapBox(transform.position, Vector3.one * thisEnemy.AttackRange, thisEnemy.transform.rotation);

        foreach ( Collider col in targets)
        {
            if (!col.CompareTag("Enemy"))
            {
                Actor actor = col.GetComponent<Actor>();

                if (actor != null)
                {
                    DamageData data = new DamageData()
                    {
                        damageAmount = damage,
                        damager = thisEnemy,
                        damagedActor = actor,
                        direction = transform.forward,
                        damageSource = transform.position
                    };

                    actor.TakeDamage(data);
                    EndAttack(); // May Remove Later
                    break;
                }
            }
        }
    }

    public void EndAttack()
    {
        // Disable the attack box.
        enabled = false;
    }
}
