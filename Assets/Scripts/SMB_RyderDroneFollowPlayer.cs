using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_RyderDroneFollowPlayer : StateMachineBehaviour
{
    RyderDroneScript drone;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        drone = animator.GetComponent<RyderDroneScript>();
        drone.agent.stoppingDistance = drone.master.playerStoppingDistance;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        drone.agent.SetDestination(drone.pA.transform.position);

        Actor_Enemy temp = drone.CheckForTargets();
        if(temp != null)
        {
            drone.currentTarget = temp;
            animator.SetTrigger("TriggerEnemy");
        }
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
