using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_EnemySearching : StateMachineBehaviour
{
    #region Variables
    // Reference to the enemy script on this enemy.
    Actor_Enemy thisEnemy;
    #endregion

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Assign the enemy reference accordingly.
        thisEnemy = animator.GetComponentInParent<Actor_Enemy>();

        // Ensure the Enemy is stopped as they search.
        thisEnemy.agent.isStopped = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Create an overlap sphere to search the area around the enemy for any colliders on the player layer.
        Collider[] targets = Physics.OverlapSphere(thisEnemy.transform.position, thisEnemy.detectionRadius, thisEnemy.playerLayer);

        // If any targets are found set newTarget to the first found player.
        if (targets.Length > 0)
        {
            thisEnemy.newTarget = targets[0].gameObject.GetComponent<Actor_Player>();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}
}
