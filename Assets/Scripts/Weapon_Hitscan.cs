
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Weapon_Hitscan : Weapon
{
    //recoil variables
    [Header("REference Points:")]
    [SerializeField] protected Transform recoilPosition;
    [SerializeField] protected Transform rotationPoint;
    [Space(10)]

    [Header("Speed Settings:")]
    [SerializeField] protected float positionalRecoilSpeed = 8f;
    [SerializeField] protected float rotationalRecoilSpeed = 8f;
    [Space(10)]

    [SerializeField] protected float positionalReturnSpeed = 18f;
    [SerializeField] protected float rotationalReturnSpeed = 38f;
    [Space(10)]

    [Header("Amount Settings:")]
    [SerializeField] protected Vector3 RecoilRotation = new Vector3(10, 5, 7);
    [SerializeField] protected Vector3 RecoilKickBack = new Vector3(0.015f, 0f, -0.2f);

    //[Space(10)]
    //public Vector3 RecoilRotationAim = new Vector3(10, 4, 6);
    //public Vector3 RecoilKickBackAim = new Vector3(0.015f, 0f, -0.2f);
    [Space(10)]

    [SerializeField] protected float bloomX = 1f;
    [SerializeField] protected float bloomY = 1f;

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 Rot;

    Vector2 weaponRecoil;

    float timeTillMaxSpread = 4;
    float maxSpreadAngle = 2;


    [SerializeField] protected AnimationCurve spreadCurve;
    [SerializeField] protected float timeFiring = 0f;


    //Audio Settings
    AudioCue ACue;

    //range variable for our raycast
    [SerializeField] protected float range = 80.0f;

    //this is our hitscan script. The pistol and SMG will use this script
    protected override void Awake()
    {
        base.Awake();
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

        ACue.PlayAudioCue();

        timeFiring += Time.deltaTime;

        //float percent = timeFiring / timeTillMaxSpread;
        float spread = spreadCurve.Evaluate(timeFiring);
        float currentSpreadAngle = spread * maxSpreadAngle;

        Vector3 mousePosition = playerCam.ScreenToWorldPoint(Input.mousePosition);
        
        float randAnglePitch = Random.Range(-currentSpreadAngle, currentSpreadAngle);
        float randAngleYaw = Random.Range(-currentSpreadAngle, currentSpreadAngle);

        Quaternion spreadAxis = Quaternion.AngleAxis(randAnglePitch, Vector3.right) * Quaternion.AngleAxis(randAngleYaw, Vector3.up);

        Ray ray = new Ray(mousePosition, spreadAxis * playerCam.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(FirePoint.position, spreadAxis * playerCam.transform.forward, Color.green);

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log("We are shooting at: " + hit.transform.name);
            //check if we hit enemy
            if (hit.transform.CompareTag("Enemy"))
            {
                Actor enemyHit = hit.transform.GetComponent<Actor>();

                //create our damageData struct, things we need to hurt enemies
                DamageData damageData = new DamageData
                {
                    damager = player,
                    damageAmount = Damage,
                    direction = transform.forward,
                    damageSource = transform.position,
                    damagedActor = enemyHit,
                };
                //apply damage to our enemy
                enemyHit.TakeDamage(damageData);

            }

            //instantiating our impact particles for now - hope for an object pool down the line
            if (ImpactParticle != null)
                Instantiate(ImpactParticle, hit.point, Quaternion.LookRotation(hit.normal));
        }

        if (!auto) // Ryan Was Here
        {
            Release();
            //playerCam.transform.DORotate(Vector3.right * -3.5f, 0.25f);
            playerCam.transform.DOPunchRotation(Vector3.right * -2.5f, 0.25f);
            //playerCam.transform.DOShakeRotation(0.25f, transform.right * 5.0f, 10, 1);
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

    protected override void Start()
    {
        base.Start();
        ACue = GetComponent<AudioCue>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //when we are firing simulate recoil
        if (isFiring)
        {
            //weapon recoil script
            rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
            positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

            recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);
            Rot = Vector3.Slerp(Rot, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
            rotationPoint.localRotation = Quaternion.Euler(Rot);

            //on shoot, not sure if this is the best location

            rotationalRecoil += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
            positionalRecoil += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);

            timeFiring += Time.deltaTime;
            

        }
        else //if we are not firing we need to go back to normal
        {
            //weapon recoil script
            rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
            positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);
            
        }

        transform.localPosition = positionalRecoil;
        //transform.rotation = Quaternion.Euler(rotationalRecoil);

    }

}
