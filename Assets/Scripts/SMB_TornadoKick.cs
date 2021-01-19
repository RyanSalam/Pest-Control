using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_TornadoKick : StateMachineBehaviour
{
    private Actor_Player pA;

    [SerializeField] private float tornadoStrikeForwardMomentum = 10f;
    [SerializeField] private float tornadoStrikeAoERange = 6f;
    [SerializeField] private float tornadoStrikeDamagePerSecond;

    [SerializeField] private LayerMask whatisEnemy;
    private Collider[] enemiesHit;

    private float timeElapsed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pA = animator.GetComponentInParent<Actor_Player>();
        pA.controlsEnabled = false;
        tornadoStrikeDamagePerSecond = 2f;
        tornadoStrikeForwardMomentum = 5f;
        timeElapsed = 0.0f;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Move forward
        pA.Controller.Move(pA.transform.forward * tornadoStrikeForwardMomentum * Time.deltaTime);
        // Detect enemies within the range
        enemiesHit = Physics.OverlapSphere(pA.transform.position, tornadoStrikeAoERange, whatisEnemy);

        if (enemiesHit.Length > 0)
        {
            // Increment timeElapsed
            timeElapsed += Time.deltaTime;
            // Execute AoEDamageSequence here 
            AoEDamageSequence();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pA.controlsEnabled = true;
        timeElapsed = 0.0f;
    }

    public void AoEDamageSequence()
    {
        var data = new DamageData
        {
            damageAmount = Mathf.RoundToInt(tornadoStrikeDamagePerSecond * timeElapsed),
            direction = pA.transform.forward,
            damageSource = pA.transform.position,
            damager = pA,
        };

        // For testing purposes
        //Debug.Log("Damage Amount: " + data.damageAmount);

        foreach (Collider enemy in enemiesHit)
        {
            enemy.gameObject.GetComponentInParent<Actor_Enemy>().TakeDamage(data);
            // Following line is for testing purposes
            //Debug.Log(enemy.name + ": " + enemy.gameObject.GetComponentInParent<Actor_Enemy>().CurrentHealth + " health");
        }

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
