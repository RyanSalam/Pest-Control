using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoSingleton<EnemyManager>
{
    #region Variables
    // Lists to keep track of how many enemies are assigned to each target
    List<Actor_Enemy> enemiesOnCore = new List<Actor_Enemy>();
    List<Actor_Enemy> enemiesOnPlayer = new List<Actor_Enemy>();

    WaveManager.Wave currentWave;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Assign the enemy to it's relevant list
    public void RegisterEnemy(Actor_Enemy enemyA)
    {
        if (enemyA.target == LevelManager.Instance.Core)
            enemiesOnCore.Add(enemyA);
        else
            enemiesOnPlayer.Add(enemyA);
    }

    // Reevaluate enemies' target based upon ratios & removes passed enemies from their lists
    public void ReassesTargets(Actor_Enemy enemyA)
    {
        // Rearragnes enemy priorities to meet the minimum thresholds
        if(enemiesOnCore.Count/currentWave.TotalEnemies() < currentWave.minCoreThresh)
        {
            enemiesOnPlayer[0].SwitchTarget(LevelManager.Instance.Core);
            RegisterEnemy(enemiesOnPlayer[0]);
        }
        else if(enemiesOnPlayer.Count/currentWave.TotalEnemies() < currentWave.minPlayerThresh)
        {
            enemiesOnCore[0].SwitchTarget(LevelManager.Instance.Player);
            RegisterEnemy(enemiesOnCore[0]);
        }
        
        // If fed an enemy reference will remove it from it's relevant list
        if(enemyA)
        {
            if(enemiesOnCore.Contains(enemyA))
                enemiesOnCore.Remove(enemyA);
            else if(enemiesOnPlayer.Contains(enemyA))
                enemiesOnPlayer.Remove(enemyA);
        }
    }
}
