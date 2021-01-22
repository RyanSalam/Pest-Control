using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    [SerializeField] protected float launchForwardForce = 5f;
    [SerializeField] protected float launchUpWardForce = 5f;
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField] protected float timeToSuck = 2.0f;
    [SerializeField] protected float areaOfEffect = 8.0f;
    [SerializeField] protected float pullStrength = 5.0f;

    [Tooltip("Maximum Distance we want to pull our enemies from")]
    [Range(1.0f, 7.0f)] public float TargetRadius;

    private float timeInAir;
    //public bool blackholeReady = true;
    protected Vector3 direction;

    protected Rigidbody rb;
    public Rigidbody RB
    {
        get { return rb; }
    }

    [Header("VFX Properties")]
    //[SerializeField] Transform plane;
   // [SerializeField] Transform swirl;
    [SerializeField] private float expandTime = 0.8f;
   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //Applies force to the projectile in an arc
        rb.AddForce(transform.forward * launchForwardForce + Vector3.up * launchUpWardForce, ForceMode.Impulse);
    }

   
    void Update()
    {

        if (timeInAir < timeToSuck)
        {
            timeInAir += Time.deltaTime;

            if (timeInAir >= timeToSuck)
            {
                /*
                swirl.gameObject.SetActive(true);
                swirl.DOScale(Vector3.one, expandTime).From(Vector3.zero).SetEase(Ease.InFlash)
                    .OnComplete(() => plane.gameObject.SetActive(true));
                    */
            }
        }


        if (timeInAir >= timeToSuck)
        {
            //Detection of enemies
            DetectEnemies();
            rb.isKinematic = true;
            //Pulling enemies in     
            
        // wait for Timer script to destroy the ball after
        }
    }

     void DeactivateProj()
    {
        /*
        swirl.DOScale(Vector3.zero, expandTime).OnComplete(()
            => transform.DOScale(Vector3.zero, expandTime / 2)).OnComplete(() => base.DeactivateProj());
            */
    }

    void DetectEnemies()
    {
        Debug.Log("Looking for alien scum");
        Collider[] allEnemies = Physics.OverlapSphere(transform.position, TargetRadius);
        foreach (Collider enemy in allEnemies)

        {
            var enemyActor = enemy.GetComponentInParent<Actor_Enemy>();
            if (enemyActor != null && Vector3.Distance(enemyActor.transform.position, transform.position) > 1.0f)
            {
                Vector3 direction = (transform.position - enemyActor.transform.position).normalized;
                enemyActor.transform.position += direction * pullStrength * Time.deltaTime;
            }
        }
    }

   
}
