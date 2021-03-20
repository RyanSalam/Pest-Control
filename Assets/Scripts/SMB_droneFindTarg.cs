using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_droneFindTarg : StateMachineBehaviour
{
    Enemy_DroneV2 drone;

    //i want a reference to the drones model, so i can rotate it and make it do stuff during movement - going to use the navmesh agents variables to help rotation
    Transform droneBody;

    public float rotationForce = 50f;

    bool canAttack;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        drone = animator.gameObject.GetComponentInParent<Enemy_DroneV2>();
        //droneBody = drone.transform.GetChild(0); //getting our drone model to rotate

        canAttack = true; //this is a second bool so we dont enter the attack state every frame

        Transform target = drone.searchForTarget();

        if (target != null) //we caught one boys, get the camera
        {
            drone.SwitchTarget(target);
        }

        //when we enter this state, we seek our target, tilting the model here so it looks as if we are propelling ourselfs towards the target - state exit will readjust
        //droneBody.Rotate(droneBody.transform.right * 25);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //overall null check to make sure we only do things when we have a target
        if (drone.CurrentTarget != null)
        {
            drone.Agent.SetDestination(drone.CurrentTarget.position); //needed to add this so it can update position - kiting

            //here we will do a distance comparison to see if we should enter attack state
            bool inAttackRange = Vector3.Distance(drone.distanceChecker.position, drone.CurrentTarget.position) < drone.AttackRange;

            ////if we change our attack range to only evaluate our targets x and y position, our height wont matter and we will still be able to drop bombs
            //float xDistance = drone.distanceChecker.position.x - drone.CurrentTarget.position.x;
            //float zDistance = drone.distanceChecker.position.z - drone.CurrentTarget.position.z;

            ////so if our x or z distance is within -3, 3 we are in the attack range - can change these values
            //if (xDistance.IsWithin(-3, 3) || zDistance.IsWithin(-3, 3))
            //    inAttackRange = true;


            //this is being triggered way too much adding a second bool
            if (inAttackRange && canAttack)
            {

                canAttack = false; //bool gets fliped when we re-enter this state

                animator.SetTrigger("Attack"); //set attack trigger entering the attack state
            }
        }
        else //default to player
        {
            //drone.SwitchTarget(LevelManager.Instance.Player.transform);
        }

        //applying rotation on the y axis
        //droneBody.Rotate(Vector3.up * rotationForce * Time.deltaTime);
    }



    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //resseting our x axis rotation, needs to flatten out when it starts to attack
        //droneBody.Rotate(Vector3.right * 0);
    }
}
