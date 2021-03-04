using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_GroundFire : MonoBehaviour
{
    [SerializeField] float damagePerTick = 5f;
    [SerializeField] float delay = 1f;
    [SerializeField] ParticleSystem ps_Fire;

    bool shouldDealDamage;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Begin", delay);
    }

    void Begin()
    {
        ps_Fire.Play();
        shouldDealDamage = true;
        Destroy(gameObject, 12f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ps_Fire.isPlaying)
        {
            // Stop dealing damage when fire effect finishes playing
            shouldDealDamage = false;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if(shouldDealDamage && other.gameObject.tag == "Enemy")
        {
            other.GetComponent<Actor_Enemy>().TakeDamage(damagePerTick);
        }
    }
}
