using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_DroneMove : StateMachineBehaviour
{
    Enemy_DroneScript enemyDrone;
    bool hasTraps;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyDrone = animator.GetComponentInParent<Enemy_DroneScript>();


        hasTraps = enemyDrone.SearchForTraps();
        if (hasTraps)
        {
            enemyDrone.tempTarget = enemyDrone.targetQueue.Dequeue();
            enemyDrone.targetQueue.Enqueue(enemyDrone.tempTarget);
            enemyDrone.SetDestinationAroundTarget(enemyDrone.tempTarget.transform.position, 1f);
            //Debug.Log("Moving to target");
        }
        else
        {
            enemyDrone.tempTarget = enemyDrone.Player.gameObject;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!hasTraps) enemyDrone.SetDestinationAroundTarget(enemyDrone.tempTarget.transform.position, enemyDrone.attackRange2);
        //Debug.Log(Vector3.Distance(animator.gameObject.transform.position, enemyDrone.tempTarget.transform.position));
        if (Vector3.Distance(animator.gameObject.transform.position, enemyDrone.tempTarget.transform.position) < enemyDrone.attackRange2)
        {
            //Debug.Log("At target");
            animator.SetTrigger("HasArrived");
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
