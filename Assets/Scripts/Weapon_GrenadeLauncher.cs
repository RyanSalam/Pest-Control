using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_GrenadeLauncher : Weapon
{
    [SerializeField] protected Rigidbody projectilePrefab;
    [SerializeField] protected float projectileForce = 10f;

    GameObject currentProjectile;
    GameObject phase3Proj1;
    GameObject phase3Proj2;

    Transform objTracker;

    bool phase1 = false;
    bool phase2 = false;
    bool phase3 = false;

    //phases: 
    //1. shoot grenade x distance in shootPhase1();
    public float breakDistance = 5.0f;
    //2. at distance split grenade into two more grenades = 4 total;
    //also slow the velocity down -> add drag?
    //3. split the grenades AGAIN
    //4. finally go boom
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (phase1)
        {
            if (currentProjectile != null && firePoint != null)
            {
                if (Vector3.Distance(firePoint.position, currentProjectile.transform.position) > breakDistance)
                {
                    //toggle phase 1 bool
                    phase1 = false;
                    //start phase 2
                    shootPhase2();
                }

            }

        }

        if (phase2)
        {
            if (phase3Proj1 != null && objTracker != null)
            {
                if (Vector3.Distance(objTracker.position, phase3Proj1.transform.position) > 2)
                {
                    //Debug.Log("phase2 distance reached... Starting phase 3");
                    //toggle phase 1 bool
                    phase2 = false;
                    //start phase 3
                    shootPhase3();
                }
            }
        }
    }

    public override void PrimaryFire()
    {
        if (Time.time  > lastFired + fireRate)
        {
            shootPhase1();

            //DamageData data = new DamageData
            //{
            //    damager = wielder,
            //    weaponUsed = this,
            //    damageAmount = Attack.Value,
            //    direction = firePoint.forward,
            //    damageSource = firePoint.transform.position
            //};

            lastFired = Time.time;
        }

        base.PrimaryFire();
    }


    void shootPhase1()
    {
        phase1 = true;
        Rigidbody temp = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        temp.AddForce(firePoint.transform.forward * projectileForce, ForceMode.Impulse);
        currentProjectile = temp.gameObject;
    }


    //2. at distance split grenade into two = 2 total nades;
    void shootPhase2()
    {
        phase2 = true;
        //destroy our current proj and create two new ones at its location
        //cache location first
        Transform phase2SpawnPoint = currentProjectile.transform;
        // Destroy(currProj);
        //instantiate more grenades AT OUR ALREADY LIVE GRENADES :O
        Rigidbody temp1 = Instantiate(projectilePrefab, phase2SpawnPoint.position + phase2SpawnPoint.right * 0.75f, phase2SpawnPoint.rotation);
        Rigidbody temp2 = Instantiate(projectilePrefab, phase2SpawnPoint.position + phase2SpawnPoint.right * -0.75f, phase2SpawnPoint.rotation);

        //add force
        temp1.AddForce(temp1.transform.forward * projectileForce, ForceMode.Impulse);
        temp2.AddForce(temp2.transform.forward * projectileForce, ForceMode.Impulse);

        phase3Proj1 = temp1.gameObject;
        phase3Proj2 = temp2.gameObject;

        objTracker = phase2SpawnPoint;

    }


    //also slow the velocity down -> add drag?
    //3. split the grenades AGAIN
    void shootPhase3()
    {
        phase3 = true;
        //destroy our current proj and create two new ones at its location
        //cache location first
        Transform phase3SpawnPoint1 = phase3Proj1.transform;
        Transform phase3SpawnPoint2 = phase3Proj2.transform;

        //instantiate more grenades AT OUR ALREADY LIVE GRENADES :O
        Rigidbody temp1 = Instantiate(projectilePrefab, phase3SpawnPoint1.position + phase3SpawnPoint1.right * 0.75f, phase3SpawnPoint1.rotation);
        Rigidbody temp2 = Instantiate(projectilePrefab, phase3SpawnPoint2.position + phase3SpawnPoint2.right * -0.75f, phase3SpawnPoint2.rotation);

        //add force
        temp1.AddForce(temp1.transform.forward * (projectileForce / 2), ForceMode.Impulse);
        temp2.AddForce(temp2.transform.forward * (projectileForce / 2), ForceMode.Impulse);

    }
}
