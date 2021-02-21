using System.Collections;
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

    

    // Start is called before the first frame update
    protected override void Start()
    {
        //reset our queue on start
        trapQueue = new Queue<Transform>();

        agent = gameObject.GetComponent<NavMeshAgent>();

        if (!agent)
            Debug.Log("Agent not found");

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {


        //we need to check for obstacle detection - 
        //ex. if we run into a doorframe/roof we need to adjust our agent-offset so the droneModel can go through it
        //without going through the wall
        if (Physics.SphereCast(collisionChecker.position, 1.0f, transform.forward , out RaycastHit hit, 1.5f, collisionLayer))
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
            //put a raycast here to make sure we can only toggle our bool- in turn, adjusting our position - when there is nothing above us.
            //Lets us continue at new height more smoothly - basically we spawn this new ray as a double check when the sphere cast is false
            if (Physics.Raycast(collisionChecker.position, collisionChecker.up, 1.5f, collisionLayer)) //looking up
            {
                isColliding = true;
            }
            else if (Physics.Raycast(collisionChecker.position, -collisionChecker.up, 1.0f, collisionLayer)) //looking down - a bit shorter due to models shape
            {
                isColliding = true;
            }
            else //finally if we dont hit anything on up or down, we are free to readjust our agent offset - representing our yPos
            {
                isColliding = false;
            }
           
        }

        //here is where we finally change our agents offset(y position) based of our semi-speghetti collision detection system
        //needed to add a timer till it tries to re adjust its position back to normal. Or it will glitch around and look very bad
        if (Time.time > timeAtCollision + 0.5f)
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


        //here, if our agent is above a certain velocity we will rotate the model on the x axis so it appears its tilted/propelling towards its target - its max mag-Velocity is agents speed parameter
        if (agent.velocity.magnitude > 3.2f)
        {
            transform.Rotate(Vector3.right * 20); //rotating x axis by 15 degrees - should slowly increment this up too but quaternions are wild
        }
        else //slowing down
        {
            float xVal = transform.rotation.x;
            transform.rotation *= Quaternion.Euler(xVal -= 0.5f, 0, 0); //trying to slowly bring the x rotation back to 0
        }

        base.Update();
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(collisionChecker.position, 1.0f);
        
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

        return EnemyHiveMind.Instance.UpdateDrone(this);
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
