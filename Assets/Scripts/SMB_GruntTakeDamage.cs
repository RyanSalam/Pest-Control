﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_GruntTakeDamage : StateMachineBehaviour
{
    #region Variables
    // Reference to the enemy script on this enemy.
    Enemy_Grunt thisEnemy;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        // Assign the enemy reference accordingly.
        thisEnemy = animator.GetComponentInParent<Enemy_Grunt>();
        thisEnemy.audioPlayer.PlayAudioCue(thisEnemy.enemyHit);
        // Ensure the Enemy is stopped as they react.
        thisEnemy.Agent.isStopped = true;

        // Reset the hits recieved.
        thisEnemy.hitsRecieved = 0;
        thisEnemy.Anim.SetInteger("hitsRecieved", 0);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        thisEnemy.Agent.isStopped = false;
    }
}
