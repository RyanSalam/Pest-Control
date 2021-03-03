using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NanoDroneScript : MonoBehaviour
{
    private NavMeshAgent agent;
    public Collider target;
    Vector3 offset = new Vector3(0f, 1f, 0f);
    Vector3 finalTar = new Vector3();
    [SerializeField] float speed = 1f;
    [SerializeField] float rotationSpeed = 1f;
    public float damage = 10f;
    float distanceToExplodeAt = 1f;

    bool hasTarget = false;
    bool attackFinished = false;

    public ParticleSystem explosionEffect;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        //agent.speed = speed;
        //Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (target.GetComponent<Actor_Enemy>().isActiveAndEnabled)
        {
            // Move to target
            if (!DroneInRange())
            {
                rotationSpeed += 0.1f;
                Vector3 targetDir = target.transform.position - transform.position;
                finalTar = offset + targetDir;
                Quaternion lookRotation = Quaternion.LookRotation(finalTar);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                transform.position += transform.forward * (speed * Time.deltaTime);
            }
            // Explode
            else if (DroneInRange() && !attackFinished)
            {
                Explode();
                attackFinished = true;
            }
        }
        // This activates if the target is killed before the drone can reach it
        else if (!target.GetComponent<Actor_Enemy>().isActiveAndEnabled && hasTarget == true && !attackFinished)
        {
            Explode();
            attackFinished = true;
        }
        else if (!attackFinished)
        {
            Debug.Log(gameObject + " has no target. If you're seeing this, something went wrong.");
        }

    }

    void Explode()
    {
        if(target!= null) target.GetComponent<Actor_Enemy>().TakeDamage(damage);
        GetComponent<MeshRenderer>().enabled = false;
        explosionEffect.Play();
        Destroy(gameObject, 2f);
    }

    public void SetTarget(Collider t, float move, float rot, float dam)
    {
        target = t;
        hasTarget = true;
        speed = move;
        rotationSpeed = rot;
        damage = dam;
    }

    bool DroneInRange()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < distanceToExplodeAt) return true;
        else return false;
    }
}
