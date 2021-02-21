using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHiveMind : MonoSingleton<EnemyHiveMind>
{
    #region Variables
    private List<Enemy_Grunt> gruntsOnPlayer = new List<Enemy_Grunt>();
    private List<Enemy_Grunt> gruntsOnCore = new List<Enemy_Grunt>();
    #endregion

    // Function to default the grunt to player
    public void ActivateGrunt(Enemy_Grunt grunt)
    {
        if (gruntsOnCore.Contains(grunt))
            gruntsOnCore.Remove(grunt);

        gruntsOnPlayer.Add(grunt);
        grunt.SwitchTarget(LevelManager.Instance.Player.transform);

        UpdateGrunts();
    }

    // Function to update drone to a random trap in the game
    public Transform UpdateDrone(Enemy_DroneV2 drone)
    {
        int randIndex = Random.Range(0, LevelManager.Instance.activeTraps.Count);

        return LevelManager.Instance.activeTraps[randIndex].transform;
    }

    // Function to be called when a grunt is created/dies in order to keep the majority of enemies on the core
    public void UpdateGrunts()
    {
        if(gruntsOnPlayer.Count / WaveManager.Instance.currentWave.TotalGrunts() > WaveManager.Instance.currentWave.minPlayerThresh * 100)
        {
            if (gruntsOnPlayer.Count / WaveManager.Instance.currentWave.TotalGrunts() < WaveManager.Instance.currentWave.minPlayerThresh * 100)
                return;

            foreach (Enemy_Grunt grunt in gruntsOnPlayer)
            {
                grunt.SwitchTarget(LevelManager.Instance.Core.transform);
                gruntsOnPlayer.Remove(grunt);
                gruntsOnCore.Add(grunt);
            }
        }
    }
}
