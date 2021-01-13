using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Hitscan : Weapon
{
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
            //here we would add our recoil - currently we are waiting for more weapon progress


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
