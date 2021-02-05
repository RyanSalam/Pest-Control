using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Laser : Trap
{
    // Start is called before the first frame update
    //[SerializeField] float TrapDamage;
    private Actor_Enemy enemyTarget;
    [SerializeField] LayerMask enemyMask;
    //Animator animator;

    public override void Activate()
    {
        if (!isTrapBuilt)
        {
            return;
        }
        Debug.Log("Trap Active");
        //animator.SetTrigger("Lasers");
        Debug.Log("FUCK");
        enemyTarget.TakeDamage(trapDamage); //damage enemy 
        enemyTarget = null; //reseting enemy back to null
        base.Activate();
    }

    protected  override  void Update()
    {
        base.Update();

        if (Physics.BoxCast(transform.position, Vector3.one * 3, Vector3.up, Quaternion.identity, 3f, enemyMask))
        {
            Debug.Log("DOUBLEFUCK");
            Activate(); //when triggered activate
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        
        //enemyTarget = GetComponent<Actor_Enemy>();
        if (trigger.gameObject.tag == "Enemy") //if the enemy actor collides with trap
        {
            /*
            Debug.Log("Triggered");
            enemyTarget = trigger.gameObject.GetComponent<Actor_Enemy>();
            if(enemyTarget != null)
            {
                Debug.Log("DOUBLEFUCK");
                Activate(); //when triggered activate
            }
            else
            {
                Debug.Log("Enemys null");
            }
           */
        }
    }
}

