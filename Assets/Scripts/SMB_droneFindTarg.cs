using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_droneFindTarg : StateMachineBehaviour
{
    Enemy_DroneV2 drone;

    bool canAttack;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        drone = animator.gameObject.GetComponentInParent<Enemy_DroneV2>();
       
        canAttack = true; //this is a second bool so we dont enter the attack state every frame
        
        Transform target = drone.searchForTarget();

        if (target != null) //we caught one boys, get the camera
        {
            Debug.Log("we caught one boys, get the camera" + target.gameObject.ToString());

            drone.SwitchTarget(target);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //overall null check to make sure we only do things when we have a target
        if (drone.CurrentTarget != null)
        {
            drone.Agent.SetDestination(drone.CurrentTarget.position); //needed to add this so it can update position - kiting

            //here we will do a distance comparison to see if we should enter attack state
            bool inAttackRange = Vector3.Distance(animator.gameObject.transform.position, drone.CurrentTarget.position) < drone.AttackRange;

            //this is being triggered way too much adding a second bool
            if (inAttackRange && canAttack)
            {
                canAttack = false; //bool gets fliped when we re-enter this state

                Debug.Log("in targetable range - attacking");
                animator.SetTrigger("Attack"); //set attack trigger entering the attack state

            }
        }
        else //default to player
        {
           drone.SwitchTarget(LevelManager.Instance.Player.transform);
        }
    }

    


}
