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

    void ChainLighting(Vector3 chainPosition)
    {
        //setting array of objects with collider to have a sphere checking chain's position, raidus, and know what enemy
        Collider[] objects = Physics.OverlapSphere(chainPosition, chainRadius, whatIsEnemy);

        if (objects[0] != null)
        {
            if (currentChainAmmount >= enemyChainAmmount)
            {
                isChaining = false;
                currentChainAmmount = 0;
                return;
            }

            Actor_Enemy enemy = objects[0].GetComponent<Actor_Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(trapDamage);
                currentChainAmmount++;
                ChainLighting(enemy.transform.position);
            }
            else
            {
                isChaining = false;
                currentChainAmmount = 0;
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isChaining == false)
        {
            ChainLighting(transform.position);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawSphere(transform.position, chainRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (maxUses > 0)
        {
            if (other.GetComponent<Actor_Enemy>() && isChaining == false)
            {
                isChaining = true;
                ChainLighting(other.transform.position);
            }
            maxUses--;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
