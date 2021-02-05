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

    public int bombsToDrop = 5;
    public float bombDropDelay = 0.1f;
    public float bombDamageToPlayer = 0.25f;
    public float bombDamageToPlayerRadius = 5f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        targetQueue = new Queue<GameObject>();
        SearchForTraps();
        dbc = GetComponentInChildren<DroneBombCarrier>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    {
        Anim.SetBool("hasArrived", Vector3.Distance(transform.position, currentTarget.position) <= _attackRange);
        Anim.SetBool("hasTarget", currentTarget != Player.transform);
    }

    public void Attack()
    {
        //Instantiate(droneBomb, droneBombSpawnpoint.transform.position, droneBombSpawnpoint.transform.rotation);
        dbc.BeginAttack(tempTarget);
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

    protected override void Death()
    {
        base.Death();
        Destroy(gameObject);
    }

    public override void OnPathCompleted()
    {
        throw new System.NotImplementedException();
    }
}
