using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ability_Cryo : MonoBehaviour
{
    private Actor_Player pA;
    private Animator _animator;
    private Timer cryoTimer;
    public AnimatorStateInfo stateInfo;
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

    private void Awake()
    {
        cryoTimer = new Timer(abilityLifetime, false);
        cryoTimer.OnTimerEnd += CryoTimerEnded;
    }

    private void CryoTimerEnded()
    {
        pA.CurrentEquipped.GetAnimator().SetTrigger("abilityCast");
        //gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the default explosion damage HERE
        explosionDamage = 40f;

        healingTimer = 0.0f;
        enemiesHit = null;
        isCanceled = false;
    }

    // Update is called once per frame
    void Update()
    {
        LevelManager.Instance.CharacterUI.FadeAbilityEffect(abilityLifetime);
        LevelManager.Instance.CharacterUI.abilityCancelText.SetActive(!isCanceled);

        healingTimer += Time.deltaTime;
        if (healingTimer >= 1.0f)
        {
            pA.CurrentHealth += healPerSecond;
            healingTimer = 0.0f;
        }

        cryoTimer.Tick(Time.deltaTime);
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        if (context.action.name == AbilityButton)
        {
            if (context.phase == InputActionPhase.Started)
            {
                isCanceled = true;
                cryoTimer.Tick(1000f);
            }
        }
    }

    // Increment the explosion damage every time the player receives damage during the cryo phase
    public void Charge(DamageData data)
    {
        explosionDamage += data.damageAmount;
    }

    public void CryostasisExplosion()
    {
        enemiesHit = Physics.OverlapSphere(pA.gameObject.transform.position, AoERange, whatIsEnemy);

        foreach (Collider enemy in enemiesHit)
        {
            Actor_Enemy e = enemy.GetComponent<Actor_Enemy>();
            if (e != null)
                e.TakeDamage(explosionDamage);
        }

        //gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        pA = LevelManager.Instance.Player;
        cryoTimer.PlayFromStart();
        if (pA != null)
        {
            pA.OnDamageFailed += Charge;
            // Set player's hit angle to 0
            pA.hitAngle = 0.0f;
            pA.playerInputs.onActionTriggered += HandleInput;
            pA.playerInputs.currentActionMap.Disable();
            pA.playerInputs.actions[AbilityButton].Enable();
        }

        // Set the default explosion damage HERE
        explosionDamage = 40f;

        healingTimer = 0.0f;
        enemiesHit = null;
        isCanceled = false;

    }

    private void OnDisable()
    {
        pA.hitAngle = 360.0f;
        //pA.controlsEnabled = true;
        pA.playerInputs.currentActionMap.Enable();
        LevelManager.Instance.CharacterUI.abilityCancelText.SetActive(false);
        LevelManager.Instance.CharacterUI.ResetAlphaValue();
        Instantiate(explosionVFX, pA.gameObject.transform.position, pA.gameObject.transform.rotation);
        pA.playerInputs.onActionTriggered -= HandleInput;
        pA.OnDamageFailed -= Charge;
    }
}
