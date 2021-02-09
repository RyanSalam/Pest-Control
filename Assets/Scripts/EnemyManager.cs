using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoSingleton<EnemyManager>
{
    #region Variables
    // Lists to keep track of how many enemies are assigned to each target
    List<Actor_Enemy> enemiesOnCore = new List<Actor_Enemy>();
    List<Actor_Enemy> enemiesOnPlayer = new List<Actor_Enemy>();

    // Current wave reference to keep track of current values
    WaveManager.Wave currentWave;
    #endregion

    // Assign the enemy to it's relevant list
    public void RegisterEnemy(Actor_Enemy enemyA)
    {
        if (enemyA.CurrentTarget == LevelManager.Instance.Core)
            enemiesOnCore.Add(enemyA);
        else
            enemiesOnPlayer.Add(enemyA);
    }

    // Reevaluate enemies' target based upon ratios & removes passed enemies from their lists
    public void ReassesTargets(Actor_Enemy enemyA)
    {
        // If fed an enemy reference will remove it from it's relevant list
        if (enemyA)
        {
            if (enemiesOnCore.Contains(enemyA))
                enemiesOnCore.Remove(enemyA);
            else if (enemiesOnPlayer.Contains(enemyA))
                enemiesOnPlayer.Remove(enemyA);
        }

        // Rearragnes enemy priorities to meet the minimum thresholds
        if (enemiesOnCore.Count/currentWave.TotalEnemies() < currentWave.minCoreThresh)
        {
            enemiesOnPlayer[0].SwitchTarget(LevelManager.Instance.Core.transform);
            RegisterEnemy(enemiesOnPlayer[0]);
            return;
        }
        else if(enemiesOnPlayer.Count/currentWave.TotalEnemies() < currentWave.minPlayerThresh && enemiesOnCore.Count/currentWave.TotalEnemies() < currentWave.minCoreThresh)
        {
            enemiesOnCore[0].SwitchTarget(LevelManager.Instance.Player.transform);
            RegisterEnemy(enemiesOnCore[0]);
        }
    }
}
