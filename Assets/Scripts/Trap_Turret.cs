using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] float rateOfFire = 0.1f;
    [SerializeField] float bulletSpeed = 10.0f;
    [SerializeField] int bulletCount = 10;
   
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        enemyScanner = new Scanner<Actor_Enemy>(transform);
        enemyScanner.targetMask = enemyLayer;
        enemyScanner.detectionRadius = maxRange;
        enemyScanner.detectionAngle = detectionAngle;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (enemyTarget == null)
        {
            Debug.Log("Finding Enemy");
            FindClosestEnemy();
        }
    }
    public override void Activate()
    {
        if (!isTrapBuilt) //checks if the trap is not built 
        {
            return;
        }
        base.Activate();
        StartCoroutine(Fire()); //starts the couroutine to fire when trap is activated
    }
    private void FindClosestEnemy()
    {
        // find enemy through sight cone 
        //set enemy target to the closet enemy found on sight cone
        enemyTarget = TurretDetection();
        if (enemyTarget != null)
        {
            Debug.Log("Shoot the target!");
            AimAtTarget();
            Activate();
        }
    }
    void AimAtTarget()
    {
        //roatating turret hinge 
        Vector3 turretLookPos = enemyTarget.transform.position - transform.position;
        turretLookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(turretLookPos, Vector3.up);
        turretRotHinge.rotation = Quaternion.Slerp(turretRotHinge.rotation, rotation, Time.deltaTime * 2);
    }
    IEnumerator Fire()
    { 
        for (int i = 0; i < bulletCount; i++)
        {
            Rigidbody proj = Instantiate(projectile, bulletSpawn.position, bulletSpawn.rotation);
            proj.AddForce(bulletSpawn.forward * bulletSpeed, ForceMode.Impulse);

            yield return new WaitForSeconds(rateOfFire);
        }
        enemyTarget = null;
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

            if (Vector3.Angle(forward, pos) > detectionAngle / 2)
            {
                // Assign the collider to a temp variable
                Actor_Enemy temp = col.GetComponent<Actor_Enemy>();

                if (temp != null)
                {
                    return temp;
                }
            }
        }
        return null; 
    }

    private void OnDrawGizmos()
    {
        if(enemyScanner != null)
        {
           enemyScanner.EditorGizmo(transform);
        }   
    }

}
