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
        ObjectPooler.Instance.InitializePool(hitEffect, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            enemyTarget = other.GetComponent<Actor_Enemy>();
            if (enemyTarget != null)
            {
                enemyTarget.TakeDamage(30f);
                //ImpactSystem.Instance.DamageIndication(30f, Color.red, enemyTarget.transform.position, Quaternion.LookRotation(transform.position - enemyTarget.transform.position));
            }
        }
        if (other.gameObject.tag != "Trap")
        {
            Instantiate(hitEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        
    }
}
