using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectileHit : MonoBehaviour
{
    [SerializeField] ParticleSystem effect;
    // Start is called before the first frame update
    void Start()
    {
        effect.Play();
        GameObject.Destroy(gameObject, 1.5f);
    }
}
