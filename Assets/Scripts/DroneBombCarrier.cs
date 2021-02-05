using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBombCarrier : MonoBehaviour
{

    [SerializeField] GameObject droneBomb;
    Enemy_DroneScript parentDrone;
    GameObject target;
    int bombs;
    float dropDelay;

    float damageToPlayer;
    float playerDamageRadius;

    // Start is called before the first frame update
    void Start()
    {
        parentDrone = GetComponentInParent<Enemy_DroneScript>();
        bombs = parentDrone.bombsToDrop;
        dropDelay = parentDrone.bombDropDelay;
        damageToPlayer = parentDrone.bombDamageToPlayer;
        playerDamageRadius = parentDrone.bombDamageToPlayerRadius;
    }

    public void BeginAttack(GameObject t)
    {
        target = t;
        StartCoroutine("Attack");
    }

    IEnumerator Attack()
    {
        for(int i = 0; i < bombs; i++)
        {
            if (i == 0) CarpetBomb(true);
            else CarpetBomb(false);
            yield return new WaitForSeconds(dropDelay);
        }

        yield return null;
    }
    public void CarpetBomb(bool firstStrike)
    {
        GameObject temp = Instantiate(droneBomb, transform.position, transform.rotation);
        temp.GetComponent<EnemyDroneBombScript>().SetTarget(target);
        temp.GetComponent<EnemyDroneBombScript>().FirstStrike(firstStrike, damageToPlayer, playerDamageRadius);

    }
}
