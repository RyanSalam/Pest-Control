using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwarmSpawn : MonoBehaviour
{
    [Header("Essential")]
    [SerializeField] Ability_Swarm ability;
    [SerializeField] NanoDroneScript missilePrefab;
    [SerializeField] Transform spawnPoint;
    Actor_Player player;


    [Header("Post Process Settings")]
    [SerializeField] float transitionDuration;
    [SerializeField][Range(0.1f, 1f)] float timeScale = 0.75f;
    PostProcessHandler lockOnVolume;

    [Header("Lock On Attributes")]
    [SerializeField] GameObject lockOnUI;
    [SerializeField] public int maxMissiles = 5;
    [SerializeField] float lockOnDuration = 0.45f;
    [SerializeField] float missileFireRate = 0.4f;
    private Timer lockOnTimer;
    public int currentMissiles;

    [Header("Enemy Detection")]
    [SerializeField] List<GameObject> enemiesTargetted;
    [SerializeField] float detectionRadius = 10.0f;
    [SerializeField] LayerMask enemyLayer;
    private int currentIndex = 0;

    private List<GameObject> lockOnUIList;

    // Start is called before the first frame update
    private void Awake()
    {
        // Just have a different script to manipulate post processes 
        lockOnVolume = GetComponent<PostProcessHandler>(); 
        lockOnTimer = new Timer(lockOnDuration, false);
        lockOnTimer.OnTimerEnd += LookForTargets; // Allows the timer to loop
        lockOnUIList = new List<GameObject>();

        currentMissiles = maxMissiles;

        ObjectPooler.Instance.InitializePool(missilePrefab.gameObject, maxMissiles);
        ObjectPooler.Instance.InitializePool(lockOnUI.gameObject, maxMissiles);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        lockOnTimer.Tick(Time.deltaTime);
    }

    public void ExitState()
    {
        player.playerInputs.actions["Move"].Enable();
        Time.timeScale = 1.0f;
        FireMissiles();
    }

    public void OnEnable()
    {
        if (player == null)
            player = LevelManager.Instance.Player;

        enemiesTargetted.Clear();
        enemiesTargetted.TrimExcess();
        currentIndex = 0;

        lockOnVolume.SetVolumeWeight(0, 1, transitionDuration);
        player.playerInputs.actions["Move"].Disable();
        Time.timeScale = timeScale;
        lockOnTimer.PlayFromStart();
    }
    
    private void LookForTargets()
    {
        if (currentIndex >= maxMissiles) return;

        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            if (!enemiesTargetted.Contains(enemy.gameObject))
            {
                Debug.Log("Enemy Targetted: " + enemy.name);
                enemiesTargetted.Add(enemy.gameObject);
                GameObject lockOn = ObjectPooler.Instance.GetFromPool
                    (lockOnUI.gameObject,
                    enemy.transform.position.AddY(0.5f),
                    Quaternion.LookRotation(enemy.transform.position - transform.position, Vector3.up));

                lockOn.transform.SetParent(enemy.transform);
                lockOnUIList.Add(lockOn);

                currentIndex++;
                break;
            }
        }

        if (currentIndex < maxMissiles) // Looping our timer to Look for more enemies if we didn't reach max enemies.
        {
            lockOnTimer.PlayFromStart();
        }
    }

    private void FireMissiles()
    {
        Debug.Log("firing missiles");
        foreach (GameObject ui in lockOnUIList) // Wanted to get rid of the UI after we fire.
        {
            ui.transform.SetParent(null);
            ui.SetActive(false);            
        }

        StartCoroutine(MissilesWithDelay());
    }

    private IEnumerator MissilesWithDelay()
    {
        var tween = lockOnVolume.SetVolumeWeight(1, 0, missileFireRate * 5);
        tween.onComplete += () => gameObject.SetActive(false);

        if (enemiesTargetted.Count > 0)
        {
            for (int i = 0; i < maxMissiles; i++)
            {

                // Allows us to still shoot 5 missiles but it goes back to the previous target in case.
                // This way if we locked onto only 2 enemies we still shoot 5 missiles. 3 locking onto the first and 2 on the second.
                int index = i % enemiesTargetted.Count;
                GameObject enemy = enemiesTargetted[index];

                GameObject missile = ObjectPooler.Instance.GetFromPool(missilePrefab.gameObject, spawnPoint.position, spawnPoint.rotation);
                missile.GetComponent<NanoDroneScript>().SetTarget(enemy);
                Debug.Log("Missile is chasing enemy: " + enemy.name);
                // Add a slight delay between each missile to make it a bit smoother
                yield return new WaitForSeconds(missileFireRate);
            }
        }

        
        lockOnUIList = new List<GameObject>();
    }

    //IEnumerator SwarmCharge()
    //{
    //    yield return new WaitForSeconds(swarmMaxcharge);
    //    StartCoroutine("SwarmFire");
    //}

    //IEnumerator SwarmFire()
    //{
    //    enemiesInExplosionRange = Physics.OverlapSphere(transform.position, nanoDroneAttackRadius, enemies);
    //    float[] virtualEnemyHealth = new float[enemiesInExplosionRange.Length];
    //    int enemyIndex = 0;

    //    int enemiesSkipped = 0;

    //    // Encourage saving drones
    //    for(int i = 0; i < virtualEnemyHealth.Length; i++)
    //    {
    //        virtualEnemyHealth[i] = 100f;
    //    }

    //    if (enemiesInExplosionRange.Length > 0)
    //    {
    //        for (int i = 0; i < maxNanoDrones; i++)
    //        {
    //            // Only fire when there are enemies left
    //            if (enemiesInExplosionRange.Length > 0)
    //            {
    //                if (enemiesSkipped == enemiesInExplosionRange.Length)
    //                {
    //                    Debug.Log("All enemies skipped. Ending");
    //                    break;
    //                }
    //                if (virtualEnemyHealth[enemyIndex] <= 0)
    //                {
    //                    Debug.Log("Not firing at enemy that should be dead");
    //                    i--;
    //                    enemyIndex++;
    //                    enemiesSkipped++;
    //                }
    //                else
    //                {
    //                    enemiesSkipped = 0;
    //                    Quaternion up = Quaternion.LookRotation(Vector3.up);
    //                    up.y = aP.transform.rotation.y;
    //                    // Create nano drone
    //                    nanoDrones[i] = Instantiate(nanoDronePrefab, nanoDroneSpawn.transform.position, up);

    //                    virtualEnemyHealth[enemyIndex] -= nanoDroneDamage;

    //                    master.UpdateCurrentDrones();
    //                    // Set drone target to enemy index
    //                    nanoDrones[i].GetComponent<NanoDroneScript>().SetTarget(enemiesInExplosionRange[enemyIndex], droneMoveSpeed, droneRotSpeed, nanoDroneDamage);
    //                    enemyIndex++;
    //                    // If more drones are fired than there are enemies, loop back around and hit the first enemy again
    //                    if (maxNanoDrones > enemiesInExplosionRange.Length && enemyIndex == enemiesInExplosionRange.Length)
    //                    {
    //                        enemyIndex = 0;
    //                    }
    //                    yield return new WaitForSeconds(nanoDroneLaunchDelay);
    //                }
    //            }
    //            else break;
    //        }
    //    }
    //    FinishAttack();
    //    yield return null;
    //}

    //void FinishAttack()
    //{
    //    master.OnSwarmEnd();
    //    //Destroy(gameObject);
    //}
}
