using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_GroundFire : MonoBehaviour
{
    [SerializeField] float damagePerTick = 5f;

    [SerializeField] ParticleSystem ps_Fire;

    bool shouldDealDamage;

    // Start is called before the first frame update
    void Start()
    {
        ps_Fire.Play();
        shouldDealDamage = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ps_Fire.isPlaying)
        {
            // Stop dealing damage when fire effect finishes playing
            shouldDealDamage = false;

            // Delete game object when all particles are gone
            if (ps_Fire.particleCount == 0) Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(shouldDealDamage && other.gameObject.tag == "Enemy")
        {
            other.GetComponent<Actor_Enemy>().TakeDamage(damagePerTick);
        }
    }
}
