using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManagerOLD : MonoSingleton<WaveManager>
{
    // Enemy info data struct to store relevant data
    [System.Serializable]
    public struct EnemyInfo
    {
        [Header("Enemy Information")]
        [Tooltip("Game Object prefab to spawn")]
        public Actor_Enemy enemyPrefab;

        [Tooltip("Number of clones to spawn")]
        public int numberToSpawn;
    }

    // Wave info data struct to store relevant data
    [System.Serializable]
    public class Wave
    {
        [Header("Wave Information")]
        [Tooltip("List of enemies that will be spawned in this wave")]
        public List<WaveManagerOLD.EnemyInfo> enemiesToSpawn;

        [Tooltip("Minimum percentage of enemies that will prioritise the core")]
        [Range(0, 1)] public float minCoreThresh;

        [Tooltip("Minimum number of enemies that will prioritise the player, after the core's threshold has been met")]
        [Range(0, 1)] public float minPlayerThresh;

        [Tooltip("Amount of time before enemies begin spawning in a wave for reinforcing and preparations")]
        [Range(0, 60)] public int buildDuration = 5;

        [Tooltip("Amount of time between each enemy spawn")]
        [Range(0, 5)] public int spawnDelay = 5;

        [Tooltip("List of accessible spawners from the spawnpoints list, defined by the spawn point element numbers")]
        public int[] availableSpawnPoints;

        [Tooltip("Event system for any custom events occuring on wave start, audio, or otherwise")]
        public UnityEvent onWaveStarted;

        [Tooltip("Event system for any custom events occuring at the end of a wave")]
        public UnityEvent onWaveEnded;

        // Function to evalutate the total number of enemiesin a wave
        public int TotalEnemies()
        {
            int maxEnemies = 0;
            foreach (EnemyInfo enemy in enemiesToSpawn)
            {
                maxEnemies += (int)enemy.numberToSpawn;
            }

            return maxEnemies;
        }
    }

    #region Variables
    [Header("Spawn Points")]
    [Tooltip("Available spawn locations for enemies")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Waves")]
    [Tooltip("List of wave structs in order")]
    [SerializeField] private List<Wave> waveList = new List<Wave>();

    // Interger index to track which wave we're in
    private int _waveIndex = 0;
    public int WaveNumber { get { return _waveIndex + 1; } }

    // Current wave variable to track which wave we're in
    [HideInInspector] public Wave currentWave;

    // Boolean to track what phase of the wave we're in
    private bool _isBuildPhase;
    public bool IsBuildPhase { get { return _isBuildPhase; } }

    // Timer to track the amount of time passed in the build phase
    Timer buildPhaseTimer;
    public Timer BuildPhaseTimer { get { return buildPhaseTimer; } }

    // Interger to track the enemies remaining in a wave
    private int _enemiesRemaining;
    public int EnemiesRemaining { get { return _enemiesRemaining; } }
    #endregion

    private Actor_Player player;

    [SerializeField] private HUDUI hudUI;
    public HUDUI HudUI { get { return hudUI; } }

    // Creating the timer and initiating it to 0. Subscribing the Spawner Coroutine to run at the end of the build phase, then starts the first wave
    private void Start()
    {
        player = LevelManager.Instance.Player;

        buildPhaseTimer = new Timer(0, false);
        buildPhaseTimer.OnTimerEnd += () => StartCoroutine(SpawnerCoroutine());
        WaveStart();
    }

    private void Update()
    {
        // If in the build phase then tick the build phase timer
        if (IsBuildPhase)
        {
            buildPhaseTimer.Tick(Time.deltaTime);
        }
    }

    // This is when build phase starts
    private void WaveStart()
    {
        // Setting the current wave variable
        currentWave = waveList[_waveIndex];

        _isBuildPhase = true;

        // Set the duration of the build phase timer
        buildPhaseTimer.SetDuration(currentWave.buildDuration);
        buildPhaseTimer.PlayFromStart();

        // Setting the enemies remaining tracker to the total of enemies being spawned this wave
        _enemiesRemaining = currentWave.TotalEnemies();

        //player.ing
        //    _audioCue.PlayAudioCue(player._cInfo.WaveStart, 30);

        //LevelManager.Instance.Player.audio.PlayAudioCue(GameManager.selectedPlayer.WaveStart);

        // Display the build phase on UI
        StartCoroutine(hudUI.BuildPhase());
    }

    // Cleanup for each wave and beginning of next wave
    private void WaveEnd()
    {
        // Increase wave index
        _waveIndex++;

        // Game win condition
        if (_waveIndex >= waveList.Count)
        {
            LevelManager.Instance.GameOver(true);
        }
        else
        {
            //player._audioCue.PlayAudioCue(player._cInfo.BuildPhaseStart, 30);
            //LevelManager.Instance.Player.audio.PlayAudioCue(GameManager.selectedPlayer.EnemyKill);
            WaveStart();
        }
    }

    // Decrease the enemies remaining and check for wave end
    public void UpdateRemEnemies()
    {
        _enemiesRemaining--;

        if (_enemiesRemaining <= 0)
            WaveEnd();
    }

    // Spawner coroutine to handle enemy spawns
    // Build phase ends here ; moving to the defence phase
    private IEnumerator SpawnerCoroutine()
    {
        // Display the defence phase on UI
        StartCoroutine(hudUI.DefensePhase());

        int enemiesToSpawn = currentWave.TotalEnemies();
        _isBuildPhase = false;

        //Loops according to the wave's maximum amount of enemies
        for (int i = 0; i < currentWave.TotalEnemies(); i++)
        {
            //Randomly assigning an index to both randomEnemy and randomTransform, to ensure that spawnpoints are randomised and 
            //enemy types (according to the wave) are randomised
            int randEnemy = Random.Range(0, currentWave.enemiesToSpawn.Count);
            int randTransfrom = Random.Range(0, spawnPoints.Count);

            //Choosing a new enemy AI of corresponding type at corresponding point
            var enemyType = currentWave.enemiesToSpawn[randEnemy];
            Actor_Enemy toSpawn = enemyType.enemyPrefab;

            enemiesToSpawn--;

            if (enemiesToSpawn <= 0)
                currentWave.enemiesToSpawn.Remove(enemyType);

            Transform whereToSpawn = spawnPoints[randTransfrom];
            //Spawning the enemy accordingly
            toSpawn = Instantiate(toSpawn, whereToSpawn.position, whereToSpawn.rotation);

            //Adds the enemy tracker function to the enemyAI's death event
            toSpawn.OnDeath += UpdateRemEnemies;

            if (!toSpawn.GetComponent<Enemy_DroneScript>())
            {
                EnemyManager.Instance.RegisterGrunt(toSpawn);
                EnemyManager.Instance.ReassessGrunts(null);
            }

            //Waits for the set spawndelay before spawning again.
            yield return new WaitForSeconds(currentWave.spawnDelay);
        }
    }
}

