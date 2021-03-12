using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
public class NanoDroneScript : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] float rotationSpeed = 1f;

    

    // Start is called before the first frame update

    private Rigidbody rb;

    [Header("Projectile Attributes")]
    [SerializeField] float damage = 25.0f;
    [SerializeField] float speed = 10.0f;
    [SerializeField] float rotationAdjustmentTime = 0.35f;
    [SerializeField] AnimationCurve rotationCurve;
    [SerializeField] ParticleSystem explosionEffect;

    private float timeElapsed = 0.0f;
    private Quaternion initRot;
    private GameObject target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Start()
    {
        ObjectPooler.Instance.InitializePool(explosionEffect.gameObject, 5);
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        timeElapsed += Time.fixedDeltaTime;
        float lerp = rotationCurve.Evaluate(timeElapsed / rotationAdjustmentTime);
        Vector3 toTarget = target.transform.position.AddY(0.5f) - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(toTarget);

        rb.rotation = Quaternion.Lerp(initRot, lookRotation, lerp);
        rb.velocity = transform.forward * speed;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    private void OnEnable()
    {
        initRot = rb.rotation;
        timeElapsed = 0.0f;

        transform.DOScale(Vector3.one, 0.25f).From(Vector3.zero).SetEase(Ease.InOutBounce);
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Actor_Enemy>().TakeDamage(damage);
        }

        target = null;
        ObjectPooler.Instance.GetFromPool(explosionEffect.gameObject, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (target.GetComponent<Actor_Enemy>().isActiveAndEnabled)
    //    {
    //        // Move to target
    //        if (!DroneInRange())
    //        {
    //            rotationSpeed += 0.1f;
    //            Vector3 targetDir = target.transform.position - transform.position;
    //            finalTar = offset + targetDir;
    //            Quaternion lookRotation = Quaternion.LookRotation(finalTar);
    //            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    //            transform.position += transform.forward * (speed * Time.deltaTime);
    //        }
    //        // Explode
    //        else if (DroneInRange() && !attackFinished)
    //        {
    //            Explode();
    //            attackFinished = true;
    //        }
    //    }
    //    // This activates if the target is killed before the drone can reach it
    //    else if (!target.GetComponent<Actor_Enemy>().isActiveAndEnabled && hasTarget == true && !attackFinished)
    //    {
    //        Explode();
    //        attackFinished = true;
    //    }
    //    else if (!attackFinished)
    //    {
    //        Debug.Log(gameObject + " has no target. If you're seeing this, something went wrong.");
    //    }

    //}

    //void Explode()
    //{
    //    if(target!= null) target.GetComponent<Actor_Enemy>().TakeDamage(damage);
    //    GetComponent<MeshRenderer>().enabled = false;
    //    explosionEffect.Play();
    //    Destroy(gameObject, 2f);
    //}

    //public void SetTarget(Collider t, float move, float rot, float dam)
    //{
    //    target = t;
    //    hasTarget = true;
    //    speed = move;
    //    rotationSpeed = rot;
    //    damage = dam;
    //}

    //bool DroneInRange()
    //{
    //    if (Vector3.Distance(transform.position, target.transform.position) < distanceToExplodeAt) return true;
    //    else return false;
    //}
}
