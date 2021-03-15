using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VarType
{
    Death,
    Attack,
    Stun
}

public class SMB_EnemyVar : StateMachineBehaviour
{
    public int variations;
    public VarType type;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (type)
        {
            case VarType.Death:
                animator.SetFloat("DeathVar", (float)Mathf.RoundToInt(Random.Range(0, variations)));
                return;

            case VarType.Attack:
                animator.SetFloat("AttackVar", (float)Mathf.RoundToInt(Random.Range(0, variations)));
                return;

            case VarType.Stun:
                animator.SetFloat("StunVar", (float)Mathf.RoundToInt(Random.Range(0, variations)));
                return;
        }
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
