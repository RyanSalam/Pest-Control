using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Swarm : MonoBehaviour
{
    private float swarmCurrentCharge = 0f;
    [SerializeField] private float swarmMaxcharge = 2f;
    [SerializeField] int maxNanoDrones = 10;
    [SerializeField] public float nanoDroneDamage = 10f;
    [SerializeField] public float nanoDroneAttackRadius = 10f;
    [SerializeField] private float nanoDroneLaunchDelay = 0.1f;

    // Used for controller the spawning of the swarm
    [SerializeField] GameObject[] nanoDrones;

    // The individual drone prefab to spawn.
    [SerializeField] GameObject nanoDronePrefab;

    // Where the drones will spawn
    [SerializeField] GameObject nanoDroneSpawn;

    [SerializeField] LayerMask enemies;

    public void Start()
    {
        
    }

    public void Update()
    {
        /*
        // Charge on hold
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwarmCharge();
        }
        // Fire on release
        if (Input.GetKeyUp(KeyCode.E) && swarmCurrentCharge > 0f)
        {
            StartCoroutine("SwarmFire");
        }
        */
    }

    public void Fire()
    {
        StartCoroutine("SwarmAltCharge");
    }

    IEnumerator SwarmAltCharge()
    {
        yield return new WaitForSeconds(swarmMaxcharge);
        StartCoroutine("SwarmFire");
    }

    void SwarmCharge()
    {
        if(swarmCurrentCharge < swarmMaxcharge) swarmCurrentCharge += Time.deltaTime;
        if (swarmCurrentCharge > swarmMaxcharge) swarmCurrentCharge = swarmMaxcharge;
    }

    IEnumerator SwarmFire()
    {
        var enemiesInExplosionRange = Physics.OverlapSphere(transform.position, nanoDroneAttackRadius, enemies);
        int enemyIndex = 0;

        // Used for the old charging method
        //float chargePercentage = (swarmCurrentCharge / swarmMaxcharge);
        //float dronesToSpawn = maxNanoDrones * chargePercentage;
        //int dronesToSpawnInt = (int)dronesToSpawn;

        int dronesToSpawnInt = maxNanoDrones;

        swarmCurrentCharge = 0f;
        if (enemiesInExplosionRange.Length > 0)
        {
            for (int i = 0; i < dronesToSpawnInt; i++)
            {
                // Create nano drone
                nanoDrones[i] = Instantiate(nanoDronePrefab, nanoDroneSpawn.transform.position, transform.rotation);
                // Set drone target to enemy index
                nanoDrones[i].GetComponent<NanoDroneScript>().SetTarget(enemiesInExplosionRange[enemyIndex]);
                enemyIndex++;

                // If more drones are fired than there are enemies, loop back around and hit the first enemy again
                if (maxNanoDrones > enemiesInExplosionRange.Length && enemyIndex == enemiesInExplosionRange.Length) enemyIndex = 0;

                yield return new WaitForSeconds(nanoDroneLaunchDelay);
            }
        }
        yield return null;
    }
}
