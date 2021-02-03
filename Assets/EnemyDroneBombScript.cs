using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDroneBombScript : MonoBehaviour
{
    float damageToPlayer = 0f;
    float playerDamageRadius = 0f;
    float distanceFromPlayer = 0f;

    public LayerMask trapLayer;
    GameObject player;

    public float speed = 5f;
    public GameObject target;

    [SerializeField] ParticleSystem explosionVFX;

    bool firstStrike = false;

    public Collider[] trapsInRange;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void SetTarget(GameObject t)
    {
        target = t;
    }
    public void FirstStrike(bool first, float damage, float radius)
    {
        firstStrike = first;
        damageToPlayer = damage;
        playerDamageRadius = radius;
    }
    private void Update()
    {
        if (target != null) transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        else
        {
            Debug.Log("Drone bomb has no target. This shouldn't be happening.");
        }

    }

    void Explode()
    {
        // Play particle effects here


        //If it's the first bomb...
        if (firstStrike)
        {
            // Check for traps
            trapsInRange = Physics.OverlapSphere(transform.position, 3f, trapLayer);
            // If traps found in radius
            if (trapsInRange.Length > 0)
            {
                // Damage the traps
                for(int i = 0; i < trapsInRange.Length; i++)
                {
                    DamageTrap(trapsInRange[i]);
                }
            }
            
        }
        // If player in radius damage them
        if (PlayerInRange()) DamagePlayer();

        // Destroy after particle effect ends
        Destroy(gameObject);

    }

    void DamageTrap(Collider trap)
    {
        Debug.Log("Damage trap: " + trap);
    }

    void DamagePlayer()
    {
        // Damage player (damage/distance - cap damage at bomb damage)
        float damage = damageToPlayer / distanceFromPlayer;
        // If damage is more than what it should be (distance < 1) cap damage
        if (damage > damageToPlayer) damage = damageToPlayer;
        //Debug.Log("Damage: " + damage);
        player.GetComponent<Actor_Player>().TakeDamage(damage);
    }

    bool PlayerInRange()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer <= playerDamageRadius) return true;
        else return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
