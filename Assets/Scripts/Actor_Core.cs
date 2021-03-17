using UnityEngine;
using DG.Tweening;
using System;

public class Actor_Core : Actor
{
    // Serializable class so it shows up in the inspector without needing to create manually 
    [System.Serializable]
    class CoreRing 
    {
        [Header("Default Set Up")]
        [SerializeField] Rigidbody ring;
        [SerializeField] Vector3 rotationDir;
        public float defaultSpeed;
        private float currentSpeed;

        [Header("Attributes For Ring Dropping")]
        [Tooltip("The health percentage required to turn this ring off. 1 is 100% (Full Health) and 0 is 0% (No Health) ")]
        [Range(0, 1)][SerializeField] float healthRequirement = 0.3f;
        [Tooltip("The rotation that this ring will interpolate back to and then drop")]
        [SerializeField] Vector3 restRotation;
        [Tooltip("The time it will take to interpolate to it's resting rotation")]
        [SerializeField] float interpolateDuration = 0.3f;

        public event System.Action CoreRingDestroyed;
        private bool bIsRotating = true;

        public void HandleRingRotation()
        {
            if (!bIsRotating) return;
            ring.transform.Rotate(rotationDir * currentSpeed * Time.deltaTime);
        }

        public void SetSpeed(float newSpeed)
        {
            currentSpeed = newSpeed;
        }

        // Delegate takes 
        public void CheckThreshold(float current, float max)
        {
            if (!bIsRotating) return;

            float percent = current / max;

            if (percent <= healthRequirement)
            {
                bIsRotating = false;
                ring.transform.DORotate(restRotation, interpolateDuration).OnComplete(() => 
                    ring.isKinematic = false);
                CoreRingDestroyed?.Invoke();
                
            }
        }
    }

    [SerializeField] CoreRing[] rings;
    [SerializeField] protected float damagedRingSpeed = 120.0f;
    protected Timer CoreSpeedTimer;
    [SerializeField] protected float coreSpeedupDuration = 1.0f;

    [SerializeField] Transform coreCentre;
    [SerializeField] GameObject CoreVFX;

    protected override void Start()
    {
        base.Start();

        m_Anim = CoreVFX.GetComponent<Animator>();
        Anim.SetFloat("HealthRatio", CurrentHealth / maxHealth);

        CoreSpeedTimer = new Timer(coreSpeedupDuration, false);
        CoreSpeedTimer.OnTimerEnd += ResetRingSpeed;
        OnDamageTaken += HandleCoreDamage;

        // Binding the core's health change delegate to each ring's check function
        foreach (CoreRing ring in rings)
        {
            ring.SetSpeed(ring.defaultSpeed);
            OnHealthChanged += ring.CheckThreshold;            
        }
    }

    private void ResetRingSpeed()
    {
        foreach (CoreRing ring in rings)
        {
            ring.SetSpeed(ring.defaultSpeed);
        }
    }

    protected virtual void Update()
    {   // We handle the rotation for each ring class
        foreach(CoreRing ring in rings)
        {
            ring.HandleRingRotation();
        }

        CoreSpeedTimer.Tick(Time.deltaTime);
    }

    protected void LateUpdate()
    {
        Anim.SetFloat("HealthRatio", CurrentHealth / maxHealth);
    }

    protected void HandleCoreDamage(DamageData data)
    {
        CoreSpeedTimer.PlayFromStart();
        Anim.SetTrigger("Hit");
        coreCentre.DOShakePosition(1f, 0.1f).OnComplete(()=> coreCentre.DORestart());

        foreach(CoreRing ring in rings)
        {
            ring.SetSpeed(damagedRingSpeed);
        }
    }
}
