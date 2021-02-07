using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_Cryostasis : StateMachineBehaviour
{
    private Actor_Player pA;
    [SerializeField] float abilityLifetime = 5.0f;
    [SerializeField] float healPerSecond = 1.0f;
    private float healingTimer;
    private Collider[] enemiesHit;
    [SerializeField] float AoERange;
    [SerializeField] float chargeAmount;
    [SerializeField] LayerMask whatIsEnemy;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pA = animator.GetComponentInParent<Actor_Player>();
        pA.OnDamageFailed += Charge;
        // Set player's hit angle to 0
        pA.hitAngle = 0.0f;
        pA.controlsEnabled = false;
        healingTimer = 0.0f;
        enemiesHit = null;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LevelManager.Instance.CharacterUI.FadeAbilityEffect(abilityLifetime);

        healingTimer += Time.deltaTime;
        if (healingTimer >= 1.0f)
        {
            pA.CurrentHealth += healPerSecond;
            healingTimer = 0.0f;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CryostasisExplosion();
        // Set player's hit angle to 360
        pA.hitAngle = 360.0f;
        pA.controlsEnabled = true;

        LevelManager.Instance.CharacterUI.ResetAlphaValue();
    }

    public void Charge(DamageData data)
    {
        chargeAmount += data.damageAmount;
    }

    private void CryostasisExplosion()
    {
        enemiesHit = Physics.OverlapSphere(pA.gameObject.transform.position, AoERange, whatIsEnemy);

        foreach (Collider enemy in enemiesHit)
        {
            Actor_Enemy e = enemy.GetComponent<Actor_Enemy>();
            if (e != null)
                e.TakeDamage(chargeAmount);
        }
        chargeAmount = 0.0f;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
