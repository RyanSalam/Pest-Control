﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Actor : MonoBehaviour
{
    // Useful delegates for binding to certain events. 

    public delegate void HealthChanged(float max, float current);
    public event HealthChanged OnHealthChanged;

    public delegate void DamageHandler(DamageData data);
    public event DamageHandler OnDamageTaken;

    public event DamageHandler OnDamageFailed;

    public event Action OnDeath;

    [SerializeField] protected float maxHealth = 10.0f;
    private float m_currentHealth;
    public float CurrentHealth // Getter Setter for currentHealth which clamps it for us
    {
        get { return m_currentHealth; }
        set
        {
            m_currentHealth = Mathf.Clamp(value, 0f, maxHealth);
            OnHealthChanged?.Invoke(maxHealth, m_currentHealth); //calls health delegate

            if (m_currentHealth <= 0)
                Death(); // calling Death Delegate if our health has reached 0.
        }
    }
    [SerializeField] protected float moveSpeed = 10.0f;
    protected bool m_isDead = false;
    public bool isDead { get { return m_isDead; } }


    [Header("Hit Box Properties")]
    [Tooltip("Determines The area where this Actor can take damage from")]
    [Range(0.0f, 360.0f)]
    [SerializeField] public float hitAngle = 360.0f;

    [Tooltip("Determines where your hit angle starts from.")]
    [Range(0.0f, 360.0f)]
    [SerializeField] protected float hitForwardRot = 360.0f;

    protected bool isInvulnerable;

    protected Animator m_Anim;
    public virtual Animator Anim { get { return m_Anim; } set { m_Anim = value; } }

    protected virtual void Awake() // All components should be cached in Awake.
    {
        m_Anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        m_currentHealth = maxHealth;
        m_isDead = false;
    }

    // Take Damage should have two different functions.
    // One is that'll handle damage from another Actor / weapons.
    // Other is when traps deal damage.

    /// <summary>
    /// Damage taken by Actor from traps
    /// </summary>
    /// <param name="damageAmount"></param>
    public virtual void TakeDamage(float damageAmount)
    {
        if (m_isDead) return; // We don't want to take anymore damage if we're already dead.
        if (isInvulnerable) return;

        CurrentHealth -= damageAmount;
        if (Anim != null)
            Anim.SetTrigger("Hit");
    }

    /// <summary>
    /// Damage Taken by Actor from other actors and weapons
    /// </summary>
    /// <param name="data"></param>
    public virtual void TakeDamage(DamageData data)
    {
        if (m_isDead) return;
        if (isInvulnerable) return;

        // This is mainly checking hit registration and if the attack is inside of the actor's hit angle.
        Vector3 forward = transform.forward;
        forward = Quaternion.AngleAxis(hitForwardRot, transform.up) * forward;

        Vector3 posFromDamager = data.damageSource - transform.position;
        posFromDamager -= transform.up * Vector3.Dot(transform.up, posFromDamager);

        if (Vector3.Angle(forward, posFromDamager) > hitAngle * 0.5f)
        {
            // Our damage was not within the hit angle and so we call the onDamageFailed Event 
            // and return from this function (Stopping it entirely).
            OnDamageFailed?.Invoke(data);
            return;
        }

        CurrentHealth -= data.damageAmount;
        OnDamageTaken?.Invoke(data);

        if (Anim != null)
            Anim.SetTrigger("Hit");
    }

    protected virtual void Death()
    {
        m_isDead = true;
        OnDeath?.Invoke();
        if (Anim != null)
            Anim.SetTrigger("Death");
    }

    // OnEnabled will basically be our respawn solution.
    // When an Actor is renabled, we should act it as a respawn.
    // Allows us to easily set up 
    protected virtual void OnEnable()
    {
        m_isDead = false;
        CurrentHealth = maxHealth;
    }

    protected void OnDestroy()
    {
        OnHealthChanged = null;
        OnDamageTaken = null;
        OnDamageFailed = null;
        OnDeath = null;
    }

    // Gizmos to just help us visualize the hitbox inside the engine
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 forward = transform.forward;
        forward = Quaternion.AngleAxis(hitForwardRot, transform.up) * forward;

        if (Event.current.type == EventType.Repaint)
        {
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(forward), 1.0f,
                EventType.Repaint);
        }


        UnityEditor.Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        forward = Quaternion.AngleAxis(-hitAngle * 0.5f, transform.up) * forward;
        UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, forward, hitAngle, 1.0f);
    }
#endif
}
