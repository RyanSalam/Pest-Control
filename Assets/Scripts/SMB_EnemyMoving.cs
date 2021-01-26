using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_EnemyMoving : StateMachineBehaviour
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

        // Allow the enemy agent to move.
        thisEnemy.agent.isStopped = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(thisEnemy.target)
            thisEnemy.agent.SetDestination(thisEnemy.target.transform.position);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
