using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DroneV2 : Actor_Enemy
{
    //queue of our traps to attack
    public Queue<Transform> trapQueue;

    

    // Start is called before the first frame update
    protected override void Start()
    {
        //reset our queue on start
        trapQueue = new Queue<Transform>();

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public Transform searchForTarget()
    {
        //SEARCH FOR TRAPS
        //IF FOUND SET TRAP T-FORM TO TARGET
        //ELSE  SET PLAYER T-FORM AS TARGET 
        GameObject[] trapsInScene = GameObject.FindGameObjectsWithTag("Trap");

        if (trapsInScene.Length > 0) //we have found traps
        {
            foreach (GameObject go in trapsInScene) //add all our gameobjects to our key
            {
                trapQueue.Enqueue(go.transform);
            }
            return trapQueue.Dequeue(); //return/remove the trap item at top of queue 
        }
        else //we didnt find any traps so we target player
        {
            return LevelManager.Instance.Player.transform;
        }

    }

    public override void OnPathCompleted()
    {
        throw new System.NotImplementedException();
    }

    public void droneAttack()
    {
        Debug.Log("release da fookin hounds");
    }
}
