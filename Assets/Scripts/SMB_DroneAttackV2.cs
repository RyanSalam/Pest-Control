using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_DroneAttackV2 : StateMachineBehaviour
{
    Enemy_DroneV2 drone;
    float currentSpeed;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        drone = animator.gameObject.GetComponentInParent<Enemy_DroneV2>();

        if (drone != null)
        {
            //caching the current speed because i want it to stop when it hits the target - so set speed to 0 
            currentSpeed = drone.movementSpeed;
            drone.movementSpeed = 0.5f;

            //call attack function
            drone.droneAttack();

            animator.SetTrigger("reset");

            animator.StopPlayback();
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //on state exit we will but the speed back to what it was
        //drone.movementSpeed = currentSpeed;
    }
}
