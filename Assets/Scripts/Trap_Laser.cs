using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Laser : Trap
{

    private Actor_Enemy enemyTarget;

    protected override void Start()
    {
        base.Start();
        Anim.SetBool("isIdle", true); 
    }

    public override void Activate()
    {
        if (!isTrapBuilt)
        {
            return;
        }
        base.Activate();
        enemyTarget.TakeDamage(trapDamage); //damage enemy 
        Anim.SetBool("isIdle", false);
        Anim.SetBool("isAttacking", true);
        enemyTarget = null; //reseting enemy back to null
    }
    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.tag == "Enemy") //if the enemy actor collides with trap
        {

            enemyTarget = trigger.gameObject.GetComponent<Actor_Enemy>();
            if (enemyTarget != null)
            {
                Activate(); //when triggered activate
            }
            if (enemyTarget == null)
            {
                Anim.SetBool("isIdle", true);
                Anim.SetBool("isAttacking", false);
            }
        }

    }
}

