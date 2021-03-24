using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Weapon_Charge : Weapon
{
    [SerializeField] protected float maxChargeDuration;
    [SerializeField] protected float chargeModifier = 1.5f;
    [SerializeField] protected float projForce;
    protected float forceModifier = 1.0f;
    protected float currentCharge = 0f;
    protected float chargeTimer = 0.0f;
    [SerializeField] protected float chargeThreshold;
    protected bool reachedThreshold = false;
    protected bool isCharging = false;

    [SerializeField] Weapon_Projectile projectilePrefab;
    protected Weapon_Projectile tempProjectile;

    [SerializeField] private ParticleSystem chargeHold;
    [SerializeField] private ParticleSystem charging;
    [SerializeField] private ParticleSystem release;
    [SerializeField] AnimationCurve bulletScaleCurve;
    [SerializeField] bool cooldownActive = false;

    DamageData data;

    //private void OnEnable()
    //{
    //    //need to switch our tiling offset for the weapon icon
    //    SkinnedMeshRenderer skinnedMesh = spriteScreen.GetComponent<SkinnedMeshRenderer>();
    //    if (skinnedMesh != null)
    //        MaterialHandler.Vector2Changer(skinnedMesh, "_TilingOffset", new Vector2(0.8f, 0f));
    //}

    // Update is called once per frame
    protected override void Update()
    {
        if (isCharging)
        {
            if (Input.GetButtonUp("Fire1") || isCanceled)
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
            chargeTimer += Time.deltaTime;
            if (chargeTimer >= 4.5f)
                Release();
        }

        if (!reachedThreshold)
        {
            ChargeUntilThreshold();
        }

        // Only tick if you're not firing/charging
        if (!isFiring)
            cooldownDelayTimer.Tick(Time.deltaTime);

        // Call PrimaryFire() here since we're not calling base.Update()
        if (auto && isFiring && canFire && !cooldownActive)
            PrimaryFire();

        // Protects the heat bar from changing if we try to shoot during cooldown
        if (cooldownActive)
        {
            if (currentShots == 0)
                cooldownActive = false;
            else
            {
                canFire = false;
                isFiring = false;
            }
        }

    }

    // Checks if the charging bullet has reached the threshold
    public bool PassedThreshold()
    {
        return currentCharge >= chargeThreshold;
    }

    // Makes sure the charge reaches the threshold before release
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

        Recoil();
        if (canFire == false) return;

        if (isCharging == false && !cooldownActive)
        {
            tempProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation, firePoint);
            tempProjectile.GetComponent<Collider>().enabled = false;
            isCharging = true;

            // Play chargeHold vFX
            chargeHold.Play();
            // Play charging vFX
            charging.Play();

            currentChargeCoroutine = StartCoroutine(Charge());
        }
    }

    private Coroutine currentChargeCoroutine;

    private IEnumerator Charge()
    {
        isCharging = true;
        currentCharge = 0.0f;

        //setting animator parameters
        animator.SetTrigger("fire");
        animator.SetBool("isFiring", isFiring);

        if (currentCooldown != null)
        {
            StopCoroutine(currentCooldown);
            currentCooldown = null;
        }
            

        while (currentCharge < maxChargeDuration)
        {
            currentCharge += Time.fixedDeltaTime;
            forceModifier -= Time.fixedDeltaTime / 2.5f;
            float scaleFactor = bulletScaleCurve.Evaluate(currentCharge / maxChargeDuration);
            tempProjectile.transform.localScale += Vector3.one * scaleFactor;            

            yield return null;
        }
    }

    public override void Release()
    {
        base.Release();
        
        if (tempProjectile == null) return;

        isCanceled = false;

        // Stop charging animations
        chargeHold.Stop();
        charging.Stop();
        // Play release animation
        if (isCharging)
            release.Play();

        if (currentChargeCoroutine != null)
            StopCoroutine(currentChargeCoroutine);

        currentChargeCoroutine = null;
        tempProjectile.transform.SetParent(null);

        if (currentCooldown != null && !cooldownActive)
            StopCoroutine(currentCooldown);

        isCharging = false;
        //currentRatio = GetHeatRatio();
        isFiring = false;

        //Debug.Log("Current shots += " + currentCharge * 5);

        LevelManager.Instance.WeaponUI.UpdateHeatBar((float)currentShots, (float)maxShots);
        if (Mathf.RoundToInt(currentCharge * 5) < 1)
            currentShots += 3; //increment our current shots by 3 (minimum)
        else
            currentShots += Mathf.RoundToInt(currentCharge * 5); //increment our current shots as usual
        LevelManager.Instance.WeaponUI.UpdateHeatBar((float)currentShots, (float)maxShots);

        currentRatio = GetHeatRatio();
        if (currentShots >= maxShots)
        {
            isFiring = false;
            canFire = false; //we cannot fire now
            //temporary coroutine until we get smarter - coroutine toggles our weapon variables
            currentCooldown = StartCoroutine(WeaponCooldown(currentRatio));
            cooldownActive = true;
            animator.SetBool("chargeOverheat", true);
        }

        // If we did a max charge, apply bonus damage
        if (currentCharge >= maxChargeDuration)
        {
            data = new DamageData
            {
                damager = player,
                damageAmount = Mathf.RoundToInt(Damage + (chargeModifier * currentCharge) * 4),
                direction = (firePoint.forward).normalized,
                damageSource = firePoint.transform.position
            };
        }
        // Apply normal charge damage
        else
        {
            data = new DamageData
            {
                damager = player,
                damageAmount = Mathf.RoundToInt(Damage + (chargeModifier * currentCharge)),
                direction = (firePoint.forward).normalized,
                damageSource = firePoint.transform.position
            };
        }

        tempProjectile.RB.isKinematic = false;
        tempProjectile.Initialize(data);
        tempProjectile.GetComponent<Collider>().enabled = true;
        tempProjectile.RB.AddForce(data.direction * projForce * forceModifier, ForceMode.Impulse);
        LevelManager.Instance.Player.PlayerCam.DOShakePosition((3 *currentCharge)/5, new Vector3(0.5f * currentCharge, 0, -0.8f * currentCharge), (int)currentCharge);
        
        // Reset variables
        lastFired = Time.time;
        currentCharge = 0;
        chargeTimer = 0.0f;
        forceModifier = 1.0f;
    }
}
