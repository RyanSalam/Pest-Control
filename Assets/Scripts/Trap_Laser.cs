using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Laser : Trap
{
    private Actor_Enemy enemyTarget;
    [SerializeField] private GameObject particleEffectObjects; 

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
        particleEffectObjects.SetActive(true); 
        //laserAttack.Play();
        enemyTarget.TakeDamage(trapDamage); //damage enemy 
        ImpactSystem.Instance.DamageIndication(trapDamage, trapColor, enemyTarget.transform.position, 
            Quaternion.LookRotation(LevelManager.Instance.Player.transform.position - enemyTarget.transform.position));
        Anim.SetBool("isIdle", false); 
        Anim.SetBool("isAttacking", true);
        StartCoroutine(trapAnimationSet()); 
        enemyTarget = null; //reseting enemy back to null
    }

    IEnumerator trapAnimationSet ()
    {
        yield return new WaitForSeconds(0.5f);
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isAttacking", false);
        particleEffectObjects.SetActive(false);
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
        }
    }
}

