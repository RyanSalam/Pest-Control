using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTrapProjectile : MonoBehaviour
{
    Actor_Enemy enemyTarget; 
    [SerializeField] GameObject hitEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            other.GetComponent<Actor_Enemy>().TakeDamage(30f); 
        }
        if (other.gameObject.tag != "Trap")
        {
            Instantiate(hitEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        
    }
}
