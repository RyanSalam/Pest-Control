using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_DroneAttackV2 : StateMachineBehaviour
{
    Enemy_DroneV2 drone;

    Transform droneBody; //once again we will be adjusting the model in this script
    public float rotationForce = 50f;
    
    float attackDelay = 3f;
    float timeAtAttack;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        drone = animator.gameObject.GetComponentInParent<Enemy_DroneV2>();
        droneBody = drone.transform.GetChild(0); //getting our drone model to rotate

        if (drone != null)
        {
            //call attack function
            drone.droneAttack();

            timeAtAttack = Time.time;  
        }
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //i want them to stop for x seconds while they are dropping bombs
        if (Time.time > timeAtAttack + attackDelay)
        { 
            animator.SetTrigger("reset");
        }

        //still spinning our drone, just slower than the movement phase, as we are hovering in this phase
        droneBody.Rotate(Vector3.up * rotationForce * Time.deltaTime);
    }
   
}
