﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_DroneV2 : Actor_Enemy
{
    //queue of our traps to attack
    public Queue<Transform> trapQueue;

    [SerializeField] GameObject droneBombProjectile;
    [SerializeField] Transform bombSpawnPoint;
    public Transform distanceChecker;

    //collision variables
    [SerializeField] Transform collisionChecker;
    [SerializeField] LayerMask collisionLayer;
    bool isColliding;

    float timeAtCollision;

    //reference to our agent / variables well need for our base offset
    NavMeshAgent agent;
    float baseOffsetValue;

    // Start is called before the first frame update
    protected override void Start()
    {
        //reset our queue on start
        trapQueue = new Queue<Transform>();

        agent = gameObject.GetComponent<NavMeshAgent>();

        if (!agent)
            Debug.Log("Agent not found");

        baseOffsetValue = agent.baseOffset;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {


        //we need to check for obstacle detection - 
        //ex. if we run into a doorframe/roof we need to adjust our agent-offset so the droneModel can go through it
        //without going through the wall
        if (Physics.SphereCast(collisionChecker.position, 2.5f, transform.forward, out RaycastHit hit, 1.0f, collisionLayer))
        {
            //checking if our hitpoint was above or below our y position. so we know if we should move below or above the obstacle in our way
            if (hit.point.y > gameObject.transform.position.y)//go down
            {
                agent.baseOffset -= 0.4f;
            }
            else if (hit.point.y < gameObject.transform.position.y) //go up
            {
                agent.baseOffset += 0.4f;
            }

            //using a bool and timer so we re-adjust our position when we are done colliding for x seconds - helps with smoothing and makes it less glitchy 
            isColliding = true;

            timeAtCollision = Time.time;
        }
        else
        {
            //not colliding so toggle our bool
            isColliding = false;
        }

        //needed to add a timer till it tries to re adjust its position back to normal.
        //or it will instantly click back through objects while the sphere collider the sphere collider returns false
        if (Time.time > timeAtCollision + 1.0f)
        {
            ////only go back down if our sphere cast isnt hitting something and our offset is above 5 - so we are always at yPos = 5 units 
            if (!isColliding && agent.baseOffset > 5) //shrink back down to 5 offset
            {
                agent.baseOffset -= 0.1f;
            }
            if (!isColliding && agent.baseOffset < 5)//grow back up to 5 offset
            {
                agent.baseOffset += 0.1f;
            }
        }

        base.Update();
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(collisionChecker.position, 2.5f);
    //}

    public Transform searchForTarget()
    {
        //SEARCH FOR TRAPS
        //IF FOUND SET TRAP T-FORM TO TARGET
        //ELSE  SET PLAYER T-FORM AS TARGET 
        //GameObject[] trapsInScene = GameObject.FindGameObjectsWithTag("Trap");
        //here we can add a shuffle funtion ? -shuffle the array, then add to queue


        //if (trapsInScene.Length > 0) //we have found traps
        //{
        //    foreach (GameObject go in trapsInScene) //add all our gameobjects to our key
        //    {
        //        trapQueue.Enqueue(go.transform);
        //    }
        //     return trapQueue.Dequeue(); //return/remove the trap item at top of queue 
        //}
        //else //we didnt find any traps so we target player
        //{
        //    return LevelManager.Instance.Player.transform;
        //}

        return EnemyManager.Instance.GetDroneTarget().transform;
    }

    public override void OnPathCompleted()
    {
        throw new System.NotImplementedException();
    }

    public void droneAttack()
    {
        if (droneBombProjectile != null)
            Instantiate(droneBombProjectile, bombSpawnPoint.position, bombSpawnPoint.rotation);
    }

    protected override void Death()
    {
        base.Death();
        gameObject.SetActive(false);
    }


}
