using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_EnemyAttacking : StateMachineBehaviour
{
    #region Variables
    // Reference to the enemy script on this enemy.
    Actor_Enemy thisEnemy;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Assign the enemy reference accordingly.
        thisEnemy = animator.GetComponentInParent<Actor_Enemy>();

        // Prevent the enemy agent from moving.
        thisEnemy.Agent.isStopped = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
