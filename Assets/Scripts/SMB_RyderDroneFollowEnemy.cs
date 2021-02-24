using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_RyderDroneFollowEnemy : StateMachineBehaviour
{
    RyderDroneScript drone;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        drone = animator.GetComponent<RyderDroneScript>();
        drone.agent.stoppingDistance = drone.master.enemyStoppingDistance;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If target is alive
        if (drone.currentTarget != null)
        {
            drone.agent.SetDestination(drone.currentTarget.transform.position);
            // Check if in attack range
            if (Vector3.Distance(animator.transform.position, drone.currentTarget.transform.position) < drone.attackRange)
            {
                Vector3 lookTarget = new Vector3(drone.currentTarget.transform.position.z, animator.transform.position.y, drone.currentTarget.transform.position.z);
                animator.transform.LookAt(lookTarget);
                if(drone.ReadyToFire()) drone.Attack(drone.currentTarget);
            }
        }
        // If target is killed
        else animator.SetTrigger("EnemyKilled");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
