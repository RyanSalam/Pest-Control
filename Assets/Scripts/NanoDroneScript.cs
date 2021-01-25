using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NanoDroneScript : MonoBehaviour
{
    private NavMeshAgent agent;
    public Collider target;

    float speed = 50f;
    public float damage = 10f;
    float distanceToExplodeAt = 1f;

    bool hasTarget = false;
    bool attackFinished = false;

    // Prevents drone from being destroyed until explosion effect is finished playing
    bool keepAlive = true;

    public ParticleSystem explosionEffect;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //damage = GameObject.Find("Ryder").GetComponent<RyderController>().nanoDroneDamage;
        //agent.speed = speed;
        //Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!explosionEffect.isPlaying && !keepAlive) Destroy(gameObject);

        if (target != null)
        {
            // Move to target
            if (!DroneInRange())
            {
                //agent.SetDestination(target.transform.position);
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.fixedDeltaTime);
            }
            // Explode
            else if (DroneInRange() && !attackFinished)
            {
                Explode();
                attackFinished = true;
            }
        }
        // This activates if the target is killed before the drone can reach it
        else if (target == null && hasTarget == true)
        {
            Explode();
        }
        else
        {
            Debug.Log(gameObject + " has no target. If you're seeing this, something went wrong.");
        }

    }

    void Explode()
    {
        //if(target!= null) target.GetComponent<Actor_Enemy>().TakeDamage(damage);
        GetComponent<MeshRenderer>().enabled = false;
        explosionEffect.Play();
        keepAlive = false;
    }

    public void SetTarget(Collider t)
    {
        target = t;
        hasTarget = true;
    }

    bool DroneInRange()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < distanceToExplodeAt) return true;
        else return false;
    }
}
