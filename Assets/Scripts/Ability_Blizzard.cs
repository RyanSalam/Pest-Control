using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ability_Blizzard : MonoBehaviour
{
    [SerializeField] float movementSpeed = 30f;
    [SerializeField] float blizzardRange = 25f;

    // How much speed is being reduced.
    [SerializeField] float speedReductionMultiplier = 0.5f;
    // Time this status effect lasts for.
    [SerializeField] float blizzardEffectTime;

    //[SerializeField] ParticleSystem blizzardParticles;

    [SerializeField] LayerMask enemies;
    [SerializeField] Collider[] enemiesHit;
    [SerializeField] HashSet<Actor_Enemy> enemiesHash;

    //[SerializeField] private float lifeTimeCount;
    //private Timer lifeTime;

    private void Start()
    {
        transform.parent = null;
        //lifeTime = new Timer(lifeTimeCount);
        //lifeTime.OnTimerEnd += () => transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => gameObject.SetActive(false));
        enemiesHash = new HashSet<Actor_Enemy>();
    }

    private void OnEnable()
    {
        transform.DOScale(Vector3.one, 0.1f).From(Vector3.zero);
        //lifeTime.PlayFromStart();
    }

    private void Update()
    {
        //lifeTime.Tick(Time.deltaTime);
        LookForEnemies();
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * movementSpeed * Time.fixedDeltaTime;
    }


    void LookForEnemies()
    {
        enemiesHit = Physics.OverlapSphere(transform.position, blizzardRange, enemies);

        if (enemiesHit.Length > 0)
        {
            foreach (Collider enemy in enemiesHit)
            {

                Debug.Log(enemy.name + " is being slowed down");
                Actor_Enemy enemy1 = enemy.GetComponent<Actor_Enemy>();

                if (enemiesHash.Add(enemy1))
                {
                    enemy1.Agent.speed *= speedReductionMultiplier;
                    StartCoroutine(EnemySlowSequence(enemy1));
                }
            }
        }
    }

    //void ResetEnemySpeed()
    //{
    //    Debug.Log("ABILITY BLIZZARD: Resetting speed");
    //    foreach (Collider enemy in enemiesHit)
    //    {
    //        enemy.GetComponent<Actor_Enemy>().Agent.speed /= speedReductionMultiplier;
    //    }
    //}

    public IEnumerator EnemySlowSequence(Actor_Enemy enemy)
    {
        
        yield return new WaitForSeconds(blizzardEffectTime);

        Debug.Log("Speed before: " + enemy.Agent.speed);
        if (enemy != null)
        {
            enemy.Agent.speed /= speedReductionMultiplier;
            Debug.Log("Speed after: " + enemy.Agent.speed);
        }
    }
}
