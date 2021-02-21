using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_DroneSetTarget : StateMachineBehaviour
{

    Enemy_DroneScript enemyDrone;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyDrone = animator.GetComponentInParent<Enemy_DroneScript>();

        //if we dont find any traps target player
        if (!enemyDrone.SearchForTraps())
        {
            Debug.Log("no traps found making player target");
            //enemyDrone.SwitchTarget(enemyDrone.Player.transform);
            enemyDrone.SwitchTarget(LevelManager.Instance.Player.transform);
            animator.SetBool("HasTarget", false);
        }
        //if we find traps get the trap at the top of the queue
        else
        {
            Debug.Log(" traps found making trap target");
            enemyDrone.SwitchTarget(enemyDrone.targetQueue.Dequeue().transform);
        }

        
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
