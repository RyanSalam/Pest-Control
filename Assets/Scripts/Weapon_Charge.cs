using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Charge : Weapon
{
    [SerializeField] protected float chargeRate = 2f;
    [SerializeField] protected float maxCharge = 10f;
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


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isCharging && currentCharge < maxCharge)
        {
            Charge();
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
            // Play charging vFX
        }
    }

    public override void Release()
    {
        if (tempProjectile == null) return;

        base.Release();

        // Stop charging animations
        // Play release animation

        tempProjectile.transform.SetParent(null);

        // Play audio

        Vector3 mousePos = playerCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.y = 0f;
        mousePos.z = 0f;

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
        isCharging = false;
        currentCharge = 0;
        chargeTime = 0.45f;


    }

    protected void Charge()
    {
        isCharging = true;
        currentCharge += (chargeRate * chargeTime);
        tempProjectile.transform.localScale = Vector3.one * currentCharge;
        chargeTime += chargeRate;

    }

}
