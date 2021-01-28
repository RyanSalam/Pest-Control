using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Charge : Weapon
{
    [SerializeField] protected float maxChargeDuration;
    [SerializeField] protected float chargeModifier = 1.5f;
    [SerializeField] protected float projForce;
    protected float currentCharge = 0f;
    [SerializeField] protected float chargeThreshold;
    protected bool reachedThreshold = false;
    protected bool isCharging = false;

    [SerializeField] Weapon_Projectile projectilePrefab;
    protected Weapon_Projectile tempProjectile;

    [SerializeField] private ParticleSystem chargeHold;
    [SerializeField] private ParticleSystem charging;
    [SerializeField] private ParticleSystem release;
    [SerializeField] AnimationCurve bulletScaleCurve;

    // Update is called once per frame
    protected override void Update()
    {
        if (isCharging)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                // Release the bullet if it has passed the threshold
                if (PassedThreshold())
                {
                    Release();
                }
                else
                {
                    reachedThreshold = false;
                }
            }
        }

        if (!reachedThreshold)
        {
            ChargeUntilThreshold();
        }
    }

    // Checks if the charging bullet has reached the threshold
    public bool PassedThreshold()
    {
        return currentCharge >= chargeThreshold;
    }

    public void ChargeUntilThreshold()
    {
        if (!PassedThreshold())
        {
            Charge();
        }
        else
        {
            Release();
            reachedThreshold = true;
        }
    }

    public override void PrimaryFire()
    {
        base.PrimaryFire();

        if (isCharging == false)
        {
            tempProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation, firePoint);
            tempProjectile.GetComponent<Collider>().enabled = false;
            isCharging = true;

            // Play chargeHold vFX
            chargeHold.Play();
            // Play charging vFX
            charging.Play();

            currentChargeCoroutine = StartCoroutine(Charge());

            if (tempProjectile != null)
            {

            }
        }
    }

    private Coroutine currentChargeCoroutine;

    private IEnumerator Charge()
    {
        isCharging = true;
        currentCharge = 0.0f;

        while (currentCharge < maxChargeDuration)
        {
            currentCharge += Time.deltaTime;
            float scaleFactor = bulletScaleCurve.Evaluate(currentCharge / maxChargeDuration);
            tempProjectile.transform.localScale += Vector3.one * scaleFactor;            

            yield return null;
        }
    }

    public override void Release()
    {
        base.Release();

        if (tempProjectile == null) return;

        // Stop charging animations
        chargeHold.Stop();
        charging.Stop();
        // Play release animation
        release.Play();

        isCharging = false;

        StopCoroutine(currentChargeCoroutine);
        currentChargeCoroutine = null;

        tempProjectile.transform.SetParent(null);

        // Play audio

        DamageData data = new DamageData
        {
            damager = player,
            damageAmount = Mathf.RoundToInt(Damage + (chargeModifier * currentCharge)),
            direction = (firePoint.forward).normalized,
            damageSource = firePoint.transform.position
        };

        tempProjectile.RB.isKinematic = false;
        tempProjectile.Initialize(data);
        tempProjectile.GetComponent<Collider>().enabled = true;
        tempProjectile.RB.AddForce(data.direction * projForce, ForceMode.Impulse);

        lastFired = Time.time;
        currentCharge = 0;
    }
}
