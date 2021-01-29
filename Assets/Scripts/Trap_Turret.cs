using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Turret : Trap
{
    [SerializeField] Transform bulletSpawn;
    [SerializeField] Transform turretRotHinge; 
    [SerializeField] Transform turretVerHinge;
    [SerializeField] Rigidbody projectile;
    private Actor_Enemy enemyTarget;

    [SerializeField] float rateOfFire = 0.1f;
    [SerializeField] float bulletSpeed = 10.0f;
    [SerializeField] int bulletCount = 10;
    int currentBullet = 0;

    [Header("Detection Attributes")]
    [SerializeField] float maxRange = 5.0f;
    [Range(0.0f, 360.0f)]
    [SerializeField] private float detectionAngle = 270;
    [SerializeField] private float heightOffset = 1;
    [SerializeField] private float detectionDelay = 1.2f;
    [SerializeField] private LayerMask enemyLayer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (enemyTarget == null)
        {
            FindClosestEnemy();
        }
    }

    void AimAtTarget()
    {
        //roatating turret hinge 
        /*
        var lookPos = target.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos, Vector3.up);
        turretRotHinge.rotation = Quaternion.Slerp(turretRotHinge.rotation, rotation, Time.deltaTime * 2);
        */
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

    private void FindClosestEnemy()
    {
        // find enemy through sight cone 

        //set enemy target to the closet enemy found on sight cone

        if(enemyTarget != null)
        {
            Activate();
        }
    }

    public override void Activate()
    {
        if (!isTrapBuilt) //checks if the trap is not built 
        {
            return;
        }

        StartCoroutine(Fire()); //starts the couroutine to fire when 

        base.Activate();
    }

    //private void EditorGizmo(Transform transform)
    //{
    //    Color c = new Color(0, 0, 0.7f, 0.4f);

    //    UnityEditor.Handles.color = c;
    //    Vector3 rotatedForward = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * transform.forward;
    //}
}
