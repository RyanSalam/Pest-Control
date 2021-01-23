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

    protected override void Update()
    {
        base.Update();
        if (isChaining == false) //if chaining is false put back the 
        {
            // ressetting chaing lighting position
            ChainLighting(transform.position);
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
                //reseting chaing and current chain ammount
                isChaining = false;
                currentChainAmmount = 0;
                return;
            }

            Actor_Enemy enemy = objects[0].GetComponent<Actor_Enemy>(); //setting enemy as objects
            // if the enemy is not empty
            if (enemy != null)
            {
                //make enemy take damage from the trap
                enemy.TakeDamage(trapDamage);
                currentChainAmmount++;          //add the current chain ammount when it is an enemy
                ChainLighting(enemy.transform.position); //set the lighting of the trap to the enemys position
            }
            else
            {
                // resetting chain back to normal
                isChaining = false;
                currentChainAmmount = 0;
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawSphere(transform.position, chainRadius); //shows the traps ChainRadius on scene 
    }

    private void OnTriggerEnter(Collider other)
    {
        //checking traps uses till it destroys 
        if (maxUses > 0) 
        {
            //checkig if collider is an enemy and checks if the trap chain is off
            if (other.GetComponent<Actor_Enemy>() && isChaining == false) 
            {
                //set the chain to true and get the lighting to that enemy
                isChaining = true;
                ChainLighting(other.transform.position);
            }
            maxUses--; //decrease the max uses
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
