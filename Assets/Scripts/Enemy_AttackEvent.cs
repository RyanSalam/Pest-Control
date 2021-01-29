using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackEvent : MonoBehaviour
{
    #region Variables
    Actor_Enemy thisEnemy;
    DamageData data;
    #endregion

    private void Start()
    {
        // Assign the enemy reference accordingly.
        thisEnemy = GetComponentInParent<Actor_Enemy>();
        enabled = false;
    }

    public void StartAttack()
    {
        // Enable the attack box;
        enabled = true;

        // Initialise damage data according to the enemy's properties.
        data = new DamageData();
        data.damageAmount = (int)thisEnemy.Damage;
        data.damager = thisEnemy;
        data.direction = thisEnemy.transform.forward;
        data.damageSource = transform.position;
    }

    private void Update()
    {
        //// Make an artifical trigger box that will damage any players within it.
        //Collider[] targets = Physics.OverlapBox(thisEnemy.attackBox.transform.position, Vector3.one * thisEnemy.AttackRange, thisEnemy.transform.rotation, thisEnemy.PlayerLayer);

        //if (targets.Length > 0)
        //{
        //    targets[0].GetComponentInParent<Actor_Player>().TakeDamage(data);
        //    enabled = false;
        //}
    }

    public void EndAttack()
    {
        // Disable the attack box.
        enabled = false;
    }
}
