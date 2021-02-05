using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_DroneSetTarget : StateMachineBehaviour
{

    Enemy_DroneScript enemyDrone;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyDrone = animator.GetComponentInParent<Enemy_DroneScript>();

        if (!enemyDrone.SearchForTraps())
        {
            enemyDrone.SwitchTarget(enemyDrone.Player.transform);
        }

        else
        {
            enemyDrone.SwitchTarget(enemyDrone.targetQueue.Dequeue().transform);
        }

        //Debug.Log("Finding target...");
        //enemyDrone = animator.GetComponentInParent<Enemy_DroneScript>();
        //if (!enemyDrone.SearchForTraps())
        //{
        //    //Debug.Log("Trap not found");
        //    enemyDrone.SwitchTarget(enemyDrone.player.transform);
        //    enemyDrone.tempTarget = enemyDrone.player;
        //    animator.SetBool("HasTarget", false);
        //}
        //else
        //{
        //    //Debug.Log("Trap found");
        //    enemyDrone.SwitchTarget(enemyDrone.targetQueue.Peek().transform);
        //    enemyDrone.tempTarget = enemyDrone.targetQueue.Peek();
        //    animator.SetBool("HasTarget", true);
        //}
        
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
