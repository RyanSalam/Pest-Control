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
    protected override void Update()
    {
        base.Update();
    }
    public override void Activate()
    {
        /*if (!isTrapBuilt) // need nayeems push to reference base trap script 
        {
            return;
        }*/
        animator.SetTrigger("Lasers");
        enemyTarget.TakeDamage(TrapDamage); //damage enemy 
        enemyTarget = null; //reseting enemy back to null
        base.Activate(); 
    }
    private void OnTriggerEnter(Collider trigger)
    {
        enemyTarget = GetComponent<Actor_Enemy>();
        if (enemyTarget != null) //if the enemy actor collides with trap
        {
            Activate(); //when triggered activate
        }
    }
}

