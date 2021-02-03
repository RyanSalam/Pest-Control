using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DroneScript : Actor_Enemy
{
    public float attackRange2 = 5f;
    public float fleeRange = 5f;

    public GameObject droneBombSpawnpoint;
    public GameObject droneBomb;
    public GameObject tempTarget;

    public GameObject[] traps;
    public DroneBombCarrier dbc;

    public Queue<GameObject> targetQueue;

    public GameObject player;

    public int bombsToDrop = 5;
    public float bombDropDelay = 0.1f;
    public float bombDamageToPlayer = 0.25f;
    public float bombDamageToPlayerRadius = 5f;

    // Start is called before the first frame update
    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        targetQueue = new Queue<GameObject>();
        SearchForTraps();
        dbc = GetComponentInChildren<DroneBombCarrier>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public void Attack()
    {
        //Instantiate(droneBomb, droneBombSpawnpoint.transform.position, droneBombSpawnpoint.transform.rotation);
        dbc.BeginAttack(tempTarget);
    }

    public int SearchForTrapss()
    {
        traps = GameObject.FindGameObjectsWithTag("Trap");
        Debug.Log(traps.Length);
        if (traps.Length > 0)
        {
            foreach (GameObject go in traps)
            {
                targetQueue.Enqueue(go);
            }
        }
        return traps.Length;
    }

    public bool SearchForTraps()
    {
        traps = GameObject.FindGameObjectsWithTag("Trap");
        if (traps.Length > 0)
        {
            foreach (GameObject go in traps)
            {
                targetQueue.Enqueue(go);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Return true if only 1 game object is in queue
    /// </summary>
    /// <returns></returns>
    public bool CheckQueue(GameObject temp)
    {
        if (temp == targetQueue.Peek()) return true;
        else return false;
    }

    public override void OnPathCompleted()
    {
        throw new System.NotImplementedException();
    }
}
