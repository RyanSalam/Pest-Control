using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Laser : Trap
{
    // Start is called before the first frame update
    [SerializeField] float TrapDamage;
    private Actor_Enemy enemyTarget;
    Animator animator;

    protected override void Start()
    {
        base.Start();
    }
    public override void Activate()
    {
        /*if (!isTrapBuilt)
        {
            return;
        }*/
        animator.SetTrigger("Lasers");
        enemyTarget.TakeDamage(TrapDamage);
        enemyTarget = null;
        base.Activate(); 
    }
    private void OnTriggerEnter(Collider trigger)
    {
        enemyTarget = GetComponent<Actor_Enemy>();
        if (enemyTarget != null) //if the enemy actor collides with trap
        {
            Activate();
        }
    }
}

