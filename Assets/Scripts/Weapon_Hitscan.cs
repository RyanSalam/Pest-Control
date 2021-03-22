﻿
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class Weapon_Hitscan : Weapon
{


    Vector2 weaponRecoil;

    float timeTillMaxSpread = 4;
    float maxSpreadAngle = 2;

    public ParticleSystem bulletTrail;
    //public GameObject bulletDecal;

    [SerializeField] protected AnimationCurve spreadCurve;
    [SerializeField] protected float timeFiring = 0f;
    

    //range variable for our raycast
    [SerializeField] protected float range = 80.0f;

    //our damage indicator prefabs
    //[SerializeField] GameObject damageIndicatorObj;

    //this is our hitscan script. The pistol and SMG will use this script
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        ObjectPooler.Instance.InitializePool(ImpactParticle.gameObject , 30);
        //ObjectPooler.Instance.InitializePool(damageIndicatorObj, 5);
        //ObjectPooler.Instance.InitializePool(bulletDecal, 20);
        
        base.Start();
    }
    

    Coroutine releaseCurrent;
    public override void PrimaryFire()
    {
        //if these are not true we do not do anything, so nothing below will get run
        if (!(Time.time > fireRate + lastFired && canFire == true))
            return;

        //our base shoot function is what oversees our weapon heating and cooldown (reload) functionality
        //it increments shots -> overheats -> cooldowns        

        if (releaseCurrent != null)
            StopCoroutine(releaseCurrent);

        //muzzle flash creation
        if (muzzleFlashParticle != null)
            muzzleFlashParticle.Play();

        //bullet trail
        if (bulletTrail != null)
            bulletTrail.Play();

        ACue.PlayAudioCue();

        timeFiring += Time.deltaTime;

        //float percent = timeFiring / timeTillMaxSpread;
        float spread = spreadCurve.Evaluate(timeFiring);
        float currentSpreadAngle = spread * maxSpreadAngle;

        Vector3 mousePosition = playerCam.ScreenToWorldPoint(player.playerInputs.actions["Mouse Pos"].ReadValue<Vector2>());
        
        float randAnglePitch = Random.Range(-currentSpreadAngle, currentSpreadAngle);
        float randAngleYaw = Random.Range(-currentSpreadAngle, currentSpreadAngle);

        Quaternion spreadAxis = Quaternion.AngleAxis(randAnglePitch, Vector3.right) * Quaternion.AngleAxis(randAngleYaw, Vector3.up);

        Ray ray = new Ray(mousePosition, spreadAxis * playerCam.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(FirePoint.position, spreadAxis * playerCam.transform.forward, Color.green);

        if (Physics.Raycast(ray, out hit, range))
        {
            int newDamage = CalculateDamageFalloff(firePoint.position, hit.point);

            //Debug.Log("We are shooting at: " + hit.transform.name);
            //check if we hit enemy
            if (hit.transform.CompareTag("Enemy"))
            {
                Actor enemyHit = hit.transform.GetComponent<Actor>();

                //create our damageData struct, things we need to hurt enemies
                DamageData damageData = new DamageData
                {
                    damager = player,
                    damageAmount = newDamage,
                    direction = transform.forward,
                    damageSource = hit.point,
                    damagedActor = enemyHit,
                    hitNormal = hit.normal,
                };
                //apply damage to our enemy
                enemyHit.TakeDamage(damageData);

                ImpactSystem.Instance.DamageIndication(damageData.damageAmount, weaponColour, damageData.damageSource, Quaternion.LookRotation(-hit.normal));

                //DamageDealt(damageData);

                //if (damageIndicatorObj != null)
                //{
                //   //GameObject temp = ObjectPooler.Instance.GetFromPool(damageIndicatorObj, hit.point, Quaternion.LookRotation(hit.normal)).gameObject;
                //    GameObject temp = ObjectPooler.Instance.GetFromPool(damageIndicatorObj, hit.point, Quaternion.LookRotation(-hit.normal)).gameObject;
                //    temp.GetComponent<DamageIndicator>().setDamageIndicator(newDamage, weaponColour);
                //    //DamageIndicator.setDamageIndicator(temp, newDamage, weaponColour);
                //}

            }
            else
            {
                //going to remove this from here when impact system is setup
                //if (bulletDecal != null)
                //{
                //    ObjectPooler.Instance.GetFromPool(bulletDecal, hit.point, Quaternion.LookRotation(hit.normal));
                //}
            }

            ImpactSystem.Instance.HandleImpact(hit.transform.gameObject, hit.point, Quaternion.LookRotation(hit.normal), weaponColour);

            //instantiating our impact particles for now - hope for an object pool down the line
            if (ImpactParticle != null)
            {
                //var temp = Instantiate(ImpactParticle, hit.point, Quaternion.LookRotation(hit.normal));
                //Vector3 newRotation = new Vector3( , hit.normal.z, 0);
                ObjectPooler.Instance.GetFromPool(ImpactParticle.gameObject, hit.point, Quaternion.LookRotation(hit.normal));
                //Destroy(temp.gameObject, 1f); // replaced with object pool
            }
                
        }

        if (!auto) // Ryan Was Here
        {
            Release();
            playerCam.transform.DOPunchRotation(Vector3.right * -2.5f, 0.25f);
        }
            

        base.PrimaryFire();
    }

    
    public override void Release()
    {
        base.Release();
        
        releaseCurrent = StartCoroutine(ReleaseDelay());
    }

    IEnumerator ReleaseDelay()
    {
        yield return new WaitForSeconds(fireRate + 0.3f);
        timeFiring = 0f;
        isFiring = false;
        muzzleFlashParticle.Stop();
    }

    int CalculateDamageFalloff(Vector3 firePosition, Vector3 hitPosition)
    {
        //going to change our damage value based on how far away our target it
        Vector3 shotDistance = firePosition - hitPosition;

        if (shotDistance.magnitude < 5)
            return Damage;
       
        float damageFalloff = shotDistance.magnitude / 100; //get a percentage
        damageFalloff *= Damage; //apply the percentage to our damage
       
        //now if we subtract the distance penalty from damage we have our new damage value
        //Debug.Log("newDamage = " +  (Damage - damageFalloff));

        return (int)(Damage - damageFalloff);
    }

    
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

}
