using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Hitscan : Weapon
{
    //recoil variables
    [Header("REference Points:")]
    public Transform recoilPosition;
    public Transform rotationPoint;
    [Space(10)]

    [Header("Speed Settings:")]
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 8f;
    [Space(10)]

    public float positionalReturnSpeed = 18f;
    public float rotationalReturnSpeed = 38f;
    [Space(10)]

    [Header("Amount Settings:")]
    public Vector3 RecoilRotation = new Vector3(10, 5, 7);
    public Vector3 RecoilKickBack = new Vector3(0.015f, 0f, -0.2f);
    [Space(10)]
    public Vector3 RecoilRotationAim = new Vector3(10, 4, 6);
    public Vector3 RecoilKickBackAim = new Vector3(0.015f, 0f, -0.2f);
    [Space(10)]

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 Rot;

    //range variable for our raycast
    public float range = 80.0f;

    //this is our hitscan script. The pistol and SMG will use this script
    private void Awake()
    {
           
        
    }

    public override void Shoot()
    {
        //our base shoot function is what oversees our weapon heating and cooldown (reload) functionality
        //it increments shots -> overheats -> cooldowns
        base.Shoot();

        //muzzle flash creation
        if (muzzleFlashParticle != null)
            muzzleFlashParticle.Play();

        //Ray mouseRay = playerCam.ScreenPointToRay(Input.mousePosition);
        Vector3 mousePosition = playerCam.ScreenToWorldPoint(Input.mousePosition);
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
            //weapon recoil script

            rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
            positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

            recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);
            Rot = Vector3.Slerp(Rot, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
            rotationPoint.localRotation = Quaternion.Euler(Rot);

            //on shoot, not sure if this is the best location

            rotationalRecoil += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
            positionalRecoil += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);
        


            //instantiating our impact particles for now - hope for an object pool down the line
            if (ImpactParticle != null)
                Instantiate(ImpactParticle, hit.point, Quaternion.LookRotation(hit.normal));

            
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
