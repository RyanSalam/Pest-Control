using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_RyderDroneKamikaze : StateMachineBehaviour
{
    RyderDroneScript drone;
    Actor_Enemy target = null;
    float timeInKamikaze = 0f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        drone = animator.GetComponent<RyderDroneScript>();
        drone.agent.speed = drone.master.kamikazeMovementSpeed;
        target = drone.FindPrimeTarget();
        if(target == null)
        {
            drone.Explode();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (target != null)
        {
            // Automatically explode after a set amount of time; just in case it gets stuck
            if (timeInKamikaze < drone.master.kamikazeSelfDestructTime) timeInKamikaze += Time.deltaTime;
            else
            {
                drone.Explode();
            }

            drone.agent.SetDestination(target.transform.position);
            if (Vector3.Distance(animator.transform.position, target.transform.position) <= drone.attackRange)
            {
                drone.Explode();
            }
        }
        else drone.Explode();
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
