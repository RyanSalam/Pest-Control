﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Charge : Weapon
{
    [SerializeField] protected float chargeRate = 2f;
    [SerializeField] protected float maxChargeDuration = 10f;
    [SerializeField] protected float chargeModifier = 1.5f;
    [SerializeField] protected float projForce = 10f;
    [SerializeField] protected float chargeTime = 0.45f;

    [SerializeField] GameObject projectilePrefab;
    protected GameObject tempProjectile;

    [SerializeField] private ParticleSystem chargeHold;
    [SerializeField] private ParticleSystem charging;
    [SerializeField] private ParticleSystem release;

    protected float currentCharge = 0f;
    protected bool isCharging = false;

    [SerializeField] AnimationCurve bulletScaleCurve;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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
            // Play charging vFX

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
        // Play release animation

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

        tempProjectile.GetComponent<Rigidbody>().isKinematic = false;
        // HERE: Assign data to temp object
        tempProjectile.GetComponent<Collider>().enabled = true;
        tempProjectile.GetComponent<Rigidbody>().AddForce(data.direction * (projForce + 10), ForceMode.Impulse);

        lastFired = Time.time;

        currentCharge = 0;
        chargeTime = 0.45f;
    }

    private void LaunchProjectile() { }
}
