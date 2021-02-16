using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliceBlizzard : MonoBehaviour
{
    [SerializeField] float movementSpeed = 30f;
    [SerializeField] float blizzardRange = 25f;

    // How much speed is being reduced.
    [SerializeField] float speedReductionMultiplier = 0.5f;
    // Time this status effect lasts for.
    [SerializeField] float blizzardEffectTime = 2f;

    [SerializeField] ParticleSystem blizzardParticles;

    [SerializeField] LayerMask enemies;
    [SerializeField] Collider[] enemiesHit;

    [SerializeField] private float lifeTimeCount;
    private Timer lifeTime;

    private void Start()
    {
        transform.parent = null;
        lifeTime = new Timer(lifeTimeCount);
        //lifeTime.OnTimerEnd += () => transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        //transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero);
        lifeTime.PlayFromStart();
    }

    private void Update()
    {
        lifeTime.Tick(Time.deltaTime);
        lookForEnemies();
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }


    void lookForEnemies()
    {
        var enemiesHit = Physics.OverlapSphere(transform.position, blizzardRange, enemies);

        if (enemiesHit.Length > 0)
        {
            foreach (Collider enemy in enemiesHit)
            {
                Debug.Log(enemy.name + " is being slowed down");
                enemy.GetComponent<Actor_Enemy>().Agent.speed *= speedReductionMultiplier;
            }
            Invoke("ResetEnemySpeed", blizzardEffectTime);
        }
    }

    void ResetEnemySpeed()
    {
        Debug.Log("Resetting speed");
        foreach (Collider enemy in enemiesHit)
        {
            enemy.GetComponent<Actor_Enemy>().Agent.speed /= speedReductionMultiplier;
        }
    }
}
