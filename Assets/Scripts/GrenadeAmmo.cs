using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAmmo : MonoBehaviour
{
    Rigidbody rigidbody;

    float projectileForce = 20f;

    private void Start()
    {
       // rigidbody.AddForce(transform.forward * projectileForce, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);

        //here we would add VFX 
        //also call a damage enemy function to see results
    }
}
