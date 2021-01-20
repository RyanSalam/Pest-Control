using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] protected AnimationCurve animCurve;
    [SerializeField] protected float timeFiring = 0f;

    //range variable for our raycast
    [SerializeField] protected float range = 80.0f;

    //this is our hitscan script. The pistol and SMG will use this script
    private void Awake()
    {
         
    }

    public override void PrimaryFire()
    {
        //our base shoot function is what oversees our weapon heating and cooldown (reload) functionality
        //it increments shots -> overheats -> cooldowns
        base.PrimaryFire();

        //muzzle flash creation
        if (muzzleFlashParticle != null)
            muzzleFlashParticle.Play();

        //Ray mouseRay = playerCam.ScreenPointToRay(Input.mousePosition);
        //Vector2 mousePosition = new Vector2(Random.Range(-bloomX, bloomX)
        
        //Random spread along axis
        float spreadX = Random.Range(-bloomX, bloomX);
        float spreadY = Random.Range(-bloomY, bloomY);

        //converting vector2 to vector3 so it can be used with mouseposition
        Vector3 spreadVector = new Vector2(spreadX, spreadY);

        //adding bloom to the mouse position
        Vector3 mousePosition = playerCam.ScreenToWorldPoint(Input.mousePosition + spreadVector);

        Ray ray = new Ray(mousePosition, playerCam.transform.forward + recoil);
        RaycastHit hit;
        Debug.DrawRay(mousePosition, playerCam.transform.forward + recoil, Color.red);

        

        if (Physics.Raycast(ray, out hit, range))
        {

            //check if we hit enemy
            if (hit.transform.tag == "Enemy")
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

    }

    public override void Release()
    {
        base.Release();

        //reseting our recoil
       
    }

    protected override void Start()
    {
        base.Start();
        //this will be removed later on, for testing purposes.
        player = GameObject.FindObjectOfType<Actor_Player>();
        player.EquipWeapon(this);

        playerCam = Camera.main;

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
