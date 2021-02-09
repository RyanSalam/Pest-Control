using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_DroneAttackV2 : StateMachineBehaviour
{
    Enemy_DroneV2 drone;
    float attackDelay = 2f;
    float timeAtAttack;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        drone = animator.gameObject.GetComponentInParent<Enemy_DroneV2>();
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
           animator.SetTrigger("reset");
        
    }
   
}
