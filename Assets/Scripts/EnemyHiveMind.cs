using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnemyHiveMind : MonoSingleton<EnemyHiveMind>
{
    #region Variables
    private List<Enemy_Grunt> gruntsOnPlayer = new List<Enemy_Grunt>();
    private List<Enemy_Grunt> gruntsOnCore = new List<Enemy_Grunt>();

    private int totalGrunts;
    private int MinPlayer => WaveManager.Instance.currentWave.minPlayerThresh;
    #endregion

    // Function to default the grunt to player
    public void RegisterGrunt(Enemy_Grunt grunt)
    {
        if (gruntsOnPlayer.Count >= MinPlayer)
        {
            grunt.SwitchTarget(LevelManager.Instance.Core.transform);
            gruntsOnCore.Add(grunt);
        }
        else
        {
            grunt.SwitchTarget(LevelManager.Instance.Player.transform);
            gruntsOnPlayer.Add(grunt);
            Debug.Log("Player");
        }

        grunt.OnDeath += () => DeRegisterGrunt(grunt);

        totalGrunts++;
    }

    private void DeRegisterGrunt(Enemy_Grunt grunt)
    {
        if (gruntsOnCore.Contains(grunt))
            gruntsOnCore.Remove(grunt);
        else if (gruntsOnPlayer.Contains(grunt))
            gruntsOnPlayer.Remove(grunt);

        totalGrunts--;
        ReassessGrunts();
    }

    // Function to be called when a grunt is created/dies in order to keep the majority of enemies on the core
    public void ReassessGrunts()
    {
        // If enough grunts for both player and core targeting
        if (totalGrunts > MinPlayer)
        {
            if (gruntsOnPlayer.Count > MinPlayer)
            { 
                while (gruntsOnPlayer.Count > MinPlayer)
                {
                    int ranIndex = Random.Range(0, gruntsOnPlayer.Count);

                    gruntsOnPlayer[ranIndex].SwitchTarget(LevelManager.Instance.Core.transform);
                    gruntsOnCore.Add(gruntsOnPlayer[ranIndex]);
                    gruntsOnPlayer.RemoveAt(ranIndex);
                }
            }
        }
        else
        {
            foreach(Enemy_Grunt grunt in gruntsOnPlayer)
            {
                grunt.SwitchTarget(LevelManager.Instance.Core.transform);
            }
        }
    }


    // Function to update drone to a random trap in the game
    public Transform UpdateDrone(Enemy_DroneV2 drone)
    {
        int randIndex = Random.Range(0, LevelManager.Instance.activeTraps.Count);

        return LevelManager.Instance.activeTraps[randIndex].transform;
    }
}
