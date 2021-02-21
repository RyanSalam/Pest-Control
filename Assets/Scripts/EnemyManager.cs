using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoSingleton<EnemyManager>
{
    #region Variables
    // Lists to keep track of how many enemies are assigned to each target
    List<Actor_Enemy> enemiesOnCore = new List<Actor_Enemy>();
    List<Actor_Enemy> enemiesOnPlayer = new List<Actor_Enemy>();

    // Timer to track when to update the grunt targeting
    Timer _intervalTimer;

    // How long between targeting refreshes
    public float targetRefreshDelay;
    #endregion

    private void Start()
    {
        _intervalTimer = new Timer(targetRefreshDelay, true);
        _intervalTimer.OnTimerEnd += () => ReassessGrunts(null);
    }

    private void Update()
    {
        _intervalTimer.Tick(Time.deltaTime);
    }


    #region Grunt Management
    // Assign the enemy to it's relevant list
    public void RegisterGrunt(Actor_Enemy enemyA)
    {
        if (enemyA.CurrentTarget == LevelManager.Instance.Core)
            enemiesOnCore.Add(enemyA);
        else
            enemiesOnPlayer.Add(enemyA);
    }

    // Reevaluate enemies' target based upon ratios & removes passed enemies from their lists
    public void ReassessGrunts(Actor_Enemy enemyA)
    {
        // If fed an enemy reference will remove it from it's relevant list
        if (enemyA)
        {
            if (enemiesOnCore.Contains(enemyA))
                enemiesOnCore.Remove(enemyA);
            else if (enemiesOnPlayer.Contains(enemyA))
                enemiesOnPlayer.Remove(enemyA);
        }

        if (enemiesOnCore.Count > 1)
        {
            //// Rearragnes enemy priorities to meet the minimum thresholds
            //if (enemiesOnCore.Count / WaveManager.Instance.currentWave.TotalEnemies() < WaveManager.Instance.currentWave.minCoreThresh * 100)
            //{
            //    enemiesOnPlayer[0].SwitchTarget(LevelManager.Instance.Core.transform);
            //    RegisterGrunt(enemiesOnPlayer[0]);
            //    return;
            //}
            //else if (enemiesOnPlayer.Count / WaveManager.Instance.currentWave.TotalEnemies() < WaveManager.Instance.currentWave.minPlayerThresh * 100 && enemiesOnCore.Count / WaveManager.Instance.currentWave.TotalEnemies() < WaveManager.Instance.currentWave.minCoreThresh * 100)
            //{
            //    enemiesOnCore[0].SwitchTarget(LevelManager.Instance.Player.transform);
            //    RegisterGrunt(enemiesOnCore[0]);
            //}
        }

        _intervalTimer.PlayFromStart();
    }
    #endregion

    #region Drone Management
    // Return a random target for the drone to follow
    public Trap GetDroneTarget()
    {
        Trap target;

        int randIndex = Random.Range(0, LevelManager.Instance.activeTraps.Count);

        target = LevelManager.Instance.activeTraps[randIndex];

        return target;
    }
    #endregion
}
