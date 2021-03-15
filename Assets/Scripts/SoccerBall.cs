using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public LayerMask groundLayer;
    Rigidbody rb;
    public float projectileForce = 5;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb)
            Debug.Log("Rigidbody not found on soccerball");
    }
    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.layer != groundLayer)
        {
            rb.AddForce(c.GetContact(0).normal * (projectileForce * 0.5f), ForceMode.Impulse);
        }
    }
}
