using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Tesla : Trap
{
    private bool isChaining = false;
    private int currentChainAmmount = 0; // deafult chain value 
    [SerializeField] private int enemyChainAmmount = 4; // setting how much enemy can be infliceted by lighting chain
    [SerializeField] private float chainRadius = 2.0f; //raidus of the chain
    [SerializeField] private LayerMask whatIsEnemy; //check enemy layer 
    private Actor_Enemy enemyTarget;


    protected override void Update()
    {
        base.Update();
        if (enemyTarget == null) // finding enemy
        {
            ChainLighting(transform.position); // ressetting chain lighting position
        }
    }

    void ChainLighting(Vector3 chainPosition)
    {
        //setting array of objects with collider to have a sphere checking chain's position, raidus, and know what enemy
        Collider[] objects = Physics.OverlapSphere(chainPosition, chainRadius, whatIsEnemy);

        if (objects[0] != null) //checks if objects is not empty 
        {
            //checking the current chain ammount is greater or equal to the enemy's chain
            if (currentChainAmmount >= enemyChainAmmount)
            {
                //reseting chain and current chain ammount
                isChaining = false;
                currentChainAmmount = 0;
                return;
            }

            enemyTarget = objects[0].GetComponent<Actor_Enemy>(); //setting enemy as objects
            // if the enemy is not empty
            if (enemyTarget != null)
            {
                //make enemy take damage from the trap
                enemyTarget.TakeDamage(trapDamage);
                //add the current chain ammount when it is an enemy
                currentChainAmmount++;         
            }
            else
            {
                // resetting chain back to normal
                isChaining = false;
                currentChainAmmount = 0;
            }
        }
    }

    public override void Activate()
    {
        if (!isTrapBuilt) //checks if the trap is not built 
        {
            return;
        }

        //set the chain to true and get the lighting to that enemy
        isChaining = true;
        ChainLighting(enemyTarget.transform.position); //set the lighting of the trap to the enemys position
        base.Activate();
    }

    private void OnTriggerEnter(Collider other)
    {
        //checkig if collider is an enemy and checks if the trap chain is off
        if (enemyTarget !=null && isChaining == false)
        {
            Activate();
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawSphere(transform.position, chainRadius); //shows the traps ChainRadius on scene 
    }
}
