using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_EnemyAttacking : StateMachineBehaviour
{

    Enemy_Grunt self;

    [Range(0, 1)]
    public float lookAtSmoothness = 0.55f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        self = animator.GetComponentInParent<Enemy_Grunt>();
        self.audioPlayer.PlayAudioCue(self.enemyAttack);
        self.Agent.isStopped = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // During the attack we want to adjust our rotation to look at the target.
        // Will need to make sure this doesn't happen throughout the animation but only at the first few frames.
        
        Vector3 dist = self.transform.position - self.CurrentTarget.position;
        dist.y = 0;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        self.Agent.isStopped = false;
        self.SetDestinationAroundTarget(self.CurrentTarget.position, self.AttackRange);
    }
}
