using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoSingleton<WaveManager>
{
    // Enemy info data struct to store relevant data
    [System.Serializable]
    public struct EnemyInfo
    {
        [Header("Enemy Information")]
        [Tooltip("Game Object prefab to spawn")]
        public enemyType enemyPrefab;

        [Tooltip("Number of clones to spawn")]
        public int numberToSpawn;

        public enum enemyType
        {
            Grunt,
            Drone
        }
    }
    //[Tooltip("Event system for any custom events occuring on wave start, audio, or otherwise")]
    public event System.Action OnWaveStarted;

    //[Tooltip("Event system for any custom events occuring at the end of a wave")]
    public event System.Action OnWaveEnded;

    // Wave info data struct to store relevant data
    [System.Serializable]
    public class Wave
    {
        [Header("Wave Information")]
        [Tooltip("List of enemies that will be spawned in this wave")]
        public List<WaveManager.EnemyInfo> enemiesToSpawn;

        [Tooltip("Minimum number of enemies that will prioritise the player, after the core's threshold has been met")]
        public int minPlayerThresh;

        [Tooltip("Amount of time before enemies begin spawning in a wave for reinforcing and preparations")]
        [Range(0, 60)] public int buildDuration = 5;

        [Tooltip("Amount of time between each enemy spawn")]
        [Range(0, 5)] public int spawnDelay = 5;

        [Tooltip("List of accessible spawners from the spawnpoints list, defined by the spawn point element numbers")]
        public Transform[] availableSpawnPoints;



        // Functions to evalutate the total number of enemies in a wave
        public int TotalEnemies()
        {
            return TotalDrones() + TotalGrunts();
        }

        public int TotalGrunts()
        {
            int _maxGrunts = 0;
            foreach (EnemyInfo enemy in enemiesToSpawn)
            {
                if (enemy.enemyPrefab == EnemyInfo.enemyType.Grunt)
                    _maxGrunts += enemy.numberToSpawn;
            }

            return _maxGrunts;
        }

        public int TotalDrones()
        {
            int _maxDrones = 0;
            foreach (EnemyInfo enemy in enemiesToSpawn)
            {
                if (enemy.enemyPrefab == EnemyInfo.enemyType.Drone)
                    _maxDrones += enemy.numberToSpawn;
            }

            return _maxDrones;
        }
    }

    #region Variables
    // Private pool lists to track all enemies spawned
    List<GameObject> _gruntPool = new List<GameObject>();
    List<GameObject> _dronePool = new List<GameObject>();

    [Tooltip("Enemy prefabs available to be spawned across any wave")]
    [SerializeField] GameObject gruntPrefab;
    [SerializeField] GameObject dronePrefab;

    [Tooltip("Gameobject to act as a parent to the enemies in order to tidy the hierarchy")]
    [SerializeField] Transform enemyPoolParent;

    [Tooltip("List of all waves in the level")]
    [SerializeField] List<Wave> levelWaves = new List<Wave>();

    // Interger and current wave to track which wave we are on
    [HideInInspector] public int waveIndex = 0;
    [HideInInspector] public Wave currentWave { get { return levelWaves[waveIndex]; } }

    // Boolean to track whether or not we are in build phase
    bool _isBuildPhase = true;
    public bool isBuildPhase { get { return _isBuildPhase; } }

    // Timer to track how long build phase lasts
    public Timer buildPhaseTimer;

    // Interger to track how many enemies are left in the wave
    int _enemiesRemaining;
    public int enemiesRemaining { get { return _enemiesRemaining; } }

    int _gruntsRemaining;
    public int gruntsRemaining { get { return _gruntsRemaining; } }

    int _dronesRemaining;
    public int dronesRemaining { get { return _dronesRemaining; } }

    // Hud reference for hud management
    [SerializeField] private HUDUI hudUI;
    public HUDUI HudUI { get { return hudUI; } }
    Coroutine waveInfoCoroutine;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // If we have the relevant prefabs then generate the relevant pools
        if (gruntPrefab)
            GenerateGruntPool();

        if (dronePrefab)
            GenerateDronePool();
        // Properly set up the build phase timer and subscribe the activation coroutine to it's end
        buildPhaseTimer = new Timer(0, false);
        buildPhaseTimer.OnTimerEnd += () => StartCoroutine(SpawnerCoroutine());

        WaveStart();
    }

    // Decrease the enemies remaining and check for wave end
    public void UpdateRemEnemies()
    {
        _enemiesRemaining--;

        if (_enemiesRemaining <= 0)
            WaveEnd();
    }

    // Update is called once per frame
    void Update()
    {
        // If in the build phase then tick the build phase timer
        if (isBuildPhase)
        {
            buildPhaseTimer.Tick(Time.deltaTime);
        }
    }

    // This is when build phase starts
    private void WaveStart()
    {
        _isBuildPhase = true;

        // Set the duration of the build phase timer
        buildPhaseTimer.SetDuration(currentWave.buildDuration);
        buildPhaseTimer.PlayFromStart();

        // Setting the enemies remaining tracker to the total of enemies being spawned this wave
        _enemiesRemaining = currentWave.TotalEnemies();
        //LevelManager.Instance.Player.audio.PlayAudioCue(GameManager.selectedPlayer.WaveStart);
        _gruntsRemaining = currentWave.TotalGrunts();
        _dronesRemaining = currentWave.TotalDrones();
        // Display the build phase on UI
        waveInfoCoroutine = StartCoroutine(hudUI.BuildPhase());
    }

    // Cleanup for each wave and beginning of next wave
    private void WaveEnd()
    {
        // Increase wave index
        waveIndex++;

        // Game win condition
        if (waveIndex >= levelWaves.Count)
        {
            LevelManager.Instance.GameOver(true);
        }
        else
        {
            WaveStart();
            OnWaveEnded?.Invoke();
        }
    }

    // Spawner coroutine to activate enemies accordingly
    IEnumerator SpawnerCoroutine()
    {
        _isBuildPhase = false;

        OnWaveStarted?.Invoke();
        // Display the defence phase on UI
        // Making sure each message would last the usual duration instead of cutting out from the previous one
        if (waveInfoCoroutine == null)
            StartCoroutine(hudUI.DefensePhase());
        else
        {
            StopCoroutine(waveInfoCoroutine);
            waveInfoCoroutine = StartCoroutine(hudUI.DefensePhase());
        }

        // Calculate how many enemies we spawn this wave and start a loop that will only activate that amount
        int _enemiesToSpawn = _enemiesRemaining;
        for (int i = 0; i < _enemiesToSpawn; i++)
        {
            // If we have both grunts and drones in the wave then randomly spawn from them
            if (_gruntsRemaining > 0 && _dronesRemaining > 0)
            {
                int index = Random.Range(0, 1);
                if (index == 0 && _gruntsRemaining > 0)
                {
                    foreach (GameObject grunt in _gruntPool)
                    {
                        if (!grunt.activeSelf)
                        {
                            grunt.transform.position = currentWave.availableSpawnPoints[Random.Range(0, currentWave.availableSpawnPoints.Length)].position;
                            grunt.SetActive(true);
                            _gruntsRemaining--;
                            break;
                        }
                    }
                }
                else if (index == 1 && _dronesRemaining > 0)
                {
                    foreach (GameObject drone in _dronePool)
                    {
                        if (!drone.activeSelf)
                        {
                            int droneI = Random.Range(0, currentWave.availableSpawnPoints.Length);
                            drone.transform.position = currentWave.availableSpawnPoints[droneI].position;
                            drone.transform.rotation = currentWave.availableSpawnPoints[droneI].rotation;
                            drone.SetActive(true);
                            _dronesRemaining--;
                            break;
                        }
                    }
                }
            }
            else if (_gruntsRemaining > 0)
            {
                foreach (GameObject grunt in _gruntPool)
                {
                    if (!grunt.activeSelf)
                    {
                        grunt.transform.position = currentWave.availableSpawnPoints[Random.Range(0, currentWave.availableSpawnPoints.Length)].position;
                        grunt.SetActive(true);
                        break;
                    }
                }
            }
            else
            {
                foreach (GameObject drone in _dronePool)
                {
                    if (!drone.activeSelf)
                    {
                        drone.transform.position = currentWave.availableSpawnPoints[Random.Range(0, currentWave.availableSpawnPoints.Length)].position;
                        drone.SetActive(true);
                        break;
                    }
                }
            }
        }

        yield return new WaitForSeconds(currentWave.spawnDelay);
    }

    #region Object Pooling
    void GenerateGruntPool()
    {
        int _gruntsToSpawn = 0;
        // Determine the most amount of enemies we need to spawn in any wave
        foreach (Wave wave in levelWaves)
        {
            if (_gruntsToSpawn < wave.TotalGrunts())
                _gruntsToSpawn = wave.TotalGrunts();
        }

        // Instantiate the relevant number of grunts
        for (int i = 0; i < _gruntsToSpawn; i++)
        {
            GameObject grunt = Instantiate(gruntPrefab, enemyPoolParent.transform);
            _gruntPool.Add(grunt);
            grunt.name = ("Grunt: " + i);
            grunt.GetComponent<Actor_Enemy>().OnDeath += UpdateRemEnemies;
            grunt.SetActive(false);
        }
    }

    void GenerateDronePool()
    {
        int _dronesToSpawn = 0;
        // Determine the most amount of enemies we need to spawn in any wave
        foreach (Wave wave in levelWaves)
        {
            if (_dronesToSpawn < wave.TotalDrones())
                _dronesToSpawn = wave.TotalDrones();
        }

        // Instantiate the relevant number of grunts
        for (int i = 0; i < _dronesToSpawn; i++)
        {
            GameObject drone = Instantiate(dronePrefab, enemyPoolParent.transform);
            _dronePool.Add(drone);
            drone.name = ("Drone: " + i);
            drone.GetComponent<Actor_Enemy>().OnDeath += UpdateRemEnemies;
            drone.SetActive(false);
        }
    }
    #endregion
}
