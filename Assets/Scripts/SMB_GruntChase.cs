using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_GruntChase : StateMachineBehaviour
{
    Actor_Enemy self;

    public float chaseSpeed = 5.0f;
    public float loseAggroDistance = 10.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        self = animator.GetComponentInParent<Actor_Enemy>();
        self.IntervalTimer.OnTimerEnd += HandleChase;

        self.Agent.isStopped = false;

        self.Agent.speed = chaseSpeed;
        self.SetDestinationAroundTarget(self.CurrentTarget.position, self.AttackRange);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        self.Agent.speed = self.movementSpeed;
        self.IntervalTimer.OnTimerEnd -= HandleChase;
    }

    private void HandleChase()
    {
        if (Vector3.Distance(self.transform.position, self.CurrentTarget.position) >= loseAggroDistance)
        {
            self.SwitchTarget(self.Core.transform);
        }

        else
        {
            Vector3 destination;
            if (self.GetRandomPointAroundTarget(self.CurrentTarget.position, self.AttackRange, out destination))
            {
                self.Agent.SetDestination(destination);
            }
        }
    }

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
