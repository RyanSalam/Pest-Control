﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmSpawn : MonoBehaviour
{
    //[SerializeField] bool ShotgunStyle;

    float swarmCurrentCharge = 0f;
    float swarmMaxcharge = 2f;
    int maxNanoDrones = 10;
    float nanoDroneDamage = 10f;
    float nanoDroneAttackRadius = 10f;
    float nanoDroneLaunchDelay = 0.1f;
    float droneMoveSpeed = 1f;
    float droneRotSpeed = 1f;

    public Collider[] enemiesInExplosionRange;

    // Used for controlling the spawning of the swarm
    [SerializeField] GameObject[] nanoDrones;

    // The individual drone prefab to spawn.
    [SerializeField] GameObject nanoDronePrefab;

    // Where the drones will spawn
    [SerializeField] GameObject nanoDroneSpawn;

    [SerializeField] LayerMask enemies;

    [SerializeField] Ability_Swarm master;

    [SerializeField] GameObject aP;

    // Start is called before the first frame update
    void Start()
    {
        aP = GameObject.FindGameObjectWithTag("Player");
        swarmCurrentCharge = 0f;
        //enemies = LayerMask.NameToLayer("Enemy");
        swarmMaxcharge = master.swarmMaxcharge;
        maxNanoDrones = master.maxNanoDrones;
        nanoDroneDamage = master.nanoDroneDamage;
        nanoDroneAttackRadius = master.nanoDroneAttackRadius;
        nanoDroneLaunchDelay = master.nanoDroneLaunchDelay;
        droneMoveSpeed = master.nanoDroneMovementSpeed;
        droneRotSpeed = master.nanoDroneRotationSpeed;
        nanoDrones = new GameObject[maxNanoDrones];
        StartCoroutine("SwarmCharge");
    }

    private void Update()
    {
        Vector3 offset = transform.forward * -1f;
        transform.position = aP.transform.position + offset;
    }

    IEnumerator SwarmCharge()
    {
        yield return new WaitForSeconds(swarmMaxcharge);
        StartCoroutine("SwarmFire");
    }

    IEnumerator SwarmFire()
    {
        enemiesInExplosionRange = Physics.OverlapSphere(transform.position, nanoDroneAttackRadius, enemies);
        int enemyIndex = 0;
        //int dronesToSpawnInt = maxNanoDrones;
        if (enemiesInExplosionRange.Length > 0)
        {
            for (int i = 0; i < maxNanoDrones; i++)
            {
                // Only fire when there are enemies left
                if (enemiesInExplosionRange.Length > 0)
                {

                    Quaternion up = Quaternion.LookRotation(Vector3.up);
                    up.y = aP.transform.rotation.y;
                    // Create nano drone
                    nanoDrones[i] = Instantiate(nanoDronePrefab, nanoDroneSpawn.transform.position, up);
                    // Set drone target to enemy index
                    nanoDrones[i].GetComponent<NanoDroneScript>().SetTarget(enemiesInExplosionRange[enemyIndex], droneMoveSpeed, droneRotSpeed, nanoDroneDamage);
                    enemyIndex++;
                    // If more drones are fired than there are enemies, loop back around and hit the first enemy again
                    if (maxNanoDrones > enemiesInExplosionRange.Length && enemyIndex == enemiesInExplosionRange.Length)
                    {
                        enemyIndex = 0;
                    }

                    // If all enemies in range are killed, end attack

                    yield return new WaitForSeconds(nanoDroneLaunchDelay);
                }
            }
        }
        FinishAttack();
        yield return null;
    }

    void FinishAttack()
    {
        Destroy(gameObject);
    }
}