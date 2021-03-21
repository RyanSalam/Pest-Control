using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SMB_Cryostasis : StateMachineBehaviour
{
    private Actor_Player pA;
    public string AbilityButton = "Button Name";
    [SerializeField] float abilityLifetime = 5.0f;
    [SerializeField] float healPerSecond;
    private float healingTimer;
    private Collider[] enemiesHit;
    [SerializeField] float AoERange;
    [SerializeField] float explosionDamage;
    [SerializeField] LayerMask whatIsEnemy;
    [SerializeField] GameObject explosionVFX;
    private bool isCanceled = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pA = LevelManager.Instance.Player;
        pA.OnDamageFailed += Charge;
        // Set player's hit angle to 0
        pA.hitAngle = 0.0f;

        // Set the default explosion damage HERE
        explosionDamage = 40f;

        //pA.controlsEnabled = false;
        pA.playerInputs.currentActionMap.Disable();
        healingTimer = 0.0f;
        enemiesHit = null;

        pA.playerInputs.onActionTriggered += HandleInput;
        isCanceled = false;
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        if (context.action.name == AbilityButton)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                isCanceled = true;
                CancelAbility();
            }
                
        }
    }

    private void CancelAbility()
    {
        // TODO - Finish animation early & skip to the transition
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LevelManager.Instance.CharacterUI.FadeAbilityEffect(abilityLifetime);
        LevelManager.Instance.CharacterUI.abilityCancelText.SetActive(!isCanceled);

        healingTimer += Time.deltaTime;
        if (healingTimer >= 1.0f)
        {
            pA.CurrentHealth += healPerSecond;
            healingTimer = 0.0f;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CryostasisExplosion();
        // Set player's hit angle to 360
        pA.hitAngle = 360.0f;
        //pA.controlsEnabled = true;
        pA.playerInputs.currentActionMap.Enable();
        LevelManager.Instance.CharacterUI.abilityCancelText.SetActive(false);
        LevelManager.Instance.CharacterUI.ResetAlphaValue();
        Instantiate(explosionVFX, pA.gameObject.transform.position, pA.gameObject.transform.rotation);
    }

    // Increment the explosion damage every time the player receives damage during the cryo phase
    public void Charge(DamageData data)
    {
        explosionDamage += data.damageAmount;
    }

    private void CryostasisExplosion()
    {
        enemiesHit = Physics.OverlapSphere(pA.gameObject.transform.position, AoERange, whatIsEnemy);

        foreach (Collider enemy in enemiesHit)
        {
            Actor_Enemy e = enemy.GetComponent<Actor_Enemy>();
            if (e != null)
                e.TakeDamage(explosionDamage);
        }
    }

}
