using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Projectile : MonoBehaviour
{
    [SerializeField] protected float projSpeed = 3f;
    [SerializeField] protected float damage;
    [SerializeField] protected float launchForce;
    [SerializeField] protected float lifeTime = 5f;
    protected Vector3 direction;
    private float timeElapsed;
    protected Rigidbody rb;
    public Rigidbody RB
    {
        get { return rb; }
    }
    protected Weapon source;
    protected DamageData Data;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > lifeTime)
            DeactivateProj();
    }

    public virtual void Initialize(float Weapondamage, Weapon source = null)
    {
        damage = Weapondamage;
        this.source = source;
    }

    public virtual void Initialize(DamageData Newdata)
    {
        Data = Newdata;
        Destroy(gameObject, lifeTime);
    }

    protected virtual void DeactivateProj()
    {
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<Actor_Enemy>())
        {
            Actor_Enemy enemy = other.gameObject.GetComponentInParent<Actor_Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(Data);
                //if (Data.weaponUsed != null)
                //    Data.weaponUsed.DamageDealt(Data);
            }
        }
    }
}
