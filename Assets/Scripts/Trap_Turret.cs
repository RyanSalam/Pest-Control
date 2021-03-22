using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Turret fires in a loop in between rotations - it should be rotating as it fires

public class Trap_Turret : Trap
{
    [Header("Detection Attributes")]
    [SerializeField] float maxRange = 5.0f;
    [Range(0.0f, 360.0f)]
    [SerializeField] private float detectionAngle = 270;
    //[SerializeField] private float detectionDelay = 1.2f;

    [SerializeField] Transform bulletSpawn;
    [SerializeField] Transform turretRotHinge; // rotating 
    [SerializeField] Transform turretVerHinge; // rotating on the x-axis
    [SerializeField] Rigidbody projectile;

    private Actor_Enemy enemyTarget;
    Scanner<Actor_Enemy> enemyScanner;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] float damage = 10f;
    [SerializeField] int RoundsPerMinute = 60;
    float rateOfFire = 0.1f;
    [SerializeField] float bulletSpeed = 10.0f;
    [SerializeField] int bulletCount = 10;

    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;

    [SerializeField] GameObject center;

    Quaternion startingRot;
    Quaternion startingRotVert;

    Vector3 temp;

    float fireTimer = 0f;

    [SerializeField] AudioCueSO attackSound;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        enemyScanner = new Scanner<Actor_Enemy>(transform);
        enemyScanner.targetMask = enemyLayer;
        enemyScanner.detectionRadius = maxRange;
        enemyScanner.detectionAngle = detectionAngle;
        startingRot = turretRotHinge.rotation;
        startingRotVert = turretVerHinge.rotation;
        rateOfFire = (float)60 / RoundsPerMinute;
        ObjectPooler.Instance.InitializePool(hitEffect, 3);
        //Anim.SetBool("isIdle", true);
        temp = transform.position;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(fireTimer <= rateOfFire) fireTimer += Time.deltaTime;
        base.Update();
        if (enemyTarget == null)
        {
            //Anim.SetBool("isIdle", true);
            //Anim.SetBool("isAttacking", false);

            //transform.position = temp;

            // Reset turret rotation to default if it isn't already
            if (turretRotHinge.rotation != startingRot || turretVerHinge.rotation != startingRotVert) ResetRotation();
            FindClosestEnemy();
        }
        else
        {
            //Anim.SetBool("isIdle", false);
            //Anim.SetBool("isAttacking", true);

            //Vector3 adjust = new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z);

            //transform.position = adjust;

            if (OutOfRange()) enemyTarget = null;
            else if (!enemyTarget.isActiveAndEnabled) enemyTarget = null;
            else
            {
                AimAtTarget();
                if (fireTimer >= rateOfFire)
                {
                    Fire();
                }
                
            }
        }

        Debug.DrawRay(bulletSpawn.position, Vector3.forward * 10, Color.green);
    }
    public override void Activate()
    {
        if (!isTrapBuilt) //checks if the trap is not built 
        {
            return;
        }
        base.Activate();
        
        //Anim.SetBool("isIdle", false);
        //Anim.SetBool("isAttacking", true);
    }
    private void FindClosestEnemy()
    {
        // find enemy through sight cone 
        //set enemy target to the closet enemy found on sight cone
        enemyTarget = TurretDetection();
        if (enemyTarget != null)
        {
            Activate();
        }   
    }
    void ResetRotation()
    {
        turretRotHinge.rotation = Quaternion.Slerp(turretRotHinge.rotation, startingRot, Time.deltaTime * 5);
        turretVerHinge.rotation = Quaternion.Slerp(turretVerHinge.rotation, startingRotVert, Time.deltaTime * 5);
    }
    void AimAtTarget()
    {
        Vector3 turretLookPos = enemyTarget.transform.position - transform.position;
        Vector3 rot = Quaternion.LookRotation(turretLookPos).eulerAngles;

        Vector3 lookHor = new Vector3(0f, rot.y, -90f);
        Quaternion rotHor = Quaternion.Euler(lookHor);
        turretRotHinge.rotation = Quaternion.Slerp(turretRotHinge.rotation, rotHor, Time.deltaTime * 5);

        Vector3 lookVert = new Vector3(0f, 0f, rot.z + 90);

        Quaternion rotVert = Quaternion.Euler(lookVert);
        turretVerHinge.rotation = Quaternion.Slerp(turretVerHinge.rotation, Quaternion.LookRotation(turretLookPos), Time.deltaTime * 5);


        //turretRotHinge.transform.rotation = Quaternion.LookRotation(turretLookPos, Vector3.up);

    }
    void Fire()
    {
        audioPlayer.PlayAudioCue(attackSound);
        fireTimer = 0f;
        if (hitEffect) ObjectPooler.Instance.GetFromPool(hitEffect, enemyTarget.transform.position, enemyTarget.transform.rotation);
        enemyTarget.GetComponent<Actor_Enemy>().TakeDamage(damage);
        muzzleFlash.Play();
        ImpactSystem.Instance.DamageIndication(damage, trapColor, enemyTarget.transform.position, Quaternion.LookRotation(enemyTarget.transform.position - LevelManager.Instance.Player.transform.position));
    }
    Actor_Enemy TurretDetection()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, maxRange, enemyLayer);

        foreach (Collider col in cols)
        {
            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(detectionAngle, transform.up) * forward;

            Vector3 pos = col.transform.position - transform.position;
            pos -= transform.up * Vector3.Dot(transform.up, pos);
            
            if (enemyTarget != null)
            {
                enemyTarget.TakeDamage(damage);
                ImpactSystem.Instance.DamageIndication(damage, trapColor, enemyTarget.transform.position,
                   Quaternion.LookRotation(enemyTarget.transform.position - LevelManager.Instance.Player.transform.position));
            }
            //Rigidbody proj = Instantiate(projectile, bulletSpawn.position, bulletSpawn.rotation);
            //proj.AddForce(bulletSpawn.forward * bulletSpeed, ForceMode.Impulse);
            //Instantiate(hitEffect, enemyTarget.transform.position, enemyTarget.transform.rotation);
            
            //enemyTarget.GetComponent<Actor_Enemy>();

            if (Vector3.Angle(forward, pos) > detectionAngle / 2)
            {
                // Assign the collider to a temp variable
                Actor_Enemy temp = col.GetComponent<Actor_Enemy>();

                if (temp != null && temp.isActiveAndEnabled)
                {
                    return temp;
                }
            }
        }
        return null; 
    }

    bool OutOfRange()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, maxRange, enemyLayer);

        foreach (Collider col in cols)
        {
            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(detectionAngle, transform.up) * forward;

            Vector3 pos = col.transform.position - transform.position;
            pos -= transform.up * Vector3.Dot(transform.up, pos);

            if (Vector3.Angle(forward, pos) > detectionAngle / 2)
            {
                // Assign the collider to a temp variable
                Actor_Enemy temp = col.GetComponent<Actor_Enemy>();

                for(int i = 0; i < cols.Length; i++)
                {
                    if (temp == enemyTarget) return false;
                }
            }
        }
        return true;
    }

#if UNITY_EDITOR
    //private void OnDrawGizmos()
    //{
    //    if(enemyScanner != null)
    //    {
    //       enemyScanner.EditorGizmo(transform);
    //    }   
    //}
#endif
}