using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private Actor_Core _core = null;
    public Actor_Core Core { get { return _core; } }  

    private Actor_Player _player;
    public Actor_Player Player { get { return _player; } }

    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private float respawnTimer = 2.0f;
    private bool isRespawning = false;

    [SerializeField] private Camera spectatorCamera;

    [SerializeField] private GameObject EndGameMenu;

    protected override void Awake() //On Awake set check LevelManager's Instance and playerSpawnPoint
    {
        base.Awake();

        if (playerSpawnPoint == null)
        {
            //Respawn Player if there is no Spawn Point Found
            playerSpawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;

            if (playerSpawnPoint == null)
                Debug.LogError("No Spawn Point Found you cunt!");
        }

        if (GameManager.selectedPlayer != null)
        {
            // Spawn Player from gameManager.
            _player = Instantiate(GameManager.selectedPlayer, playerSpawnPoint.transform.position, Quaternion.identity);
        }

        else
        {
            _player = FindObjectOfType<Actor_Player>();

            if (_player == null)
                Debug.LogError("THere's no player in your scene you cunt! Are you even trying?");
        }

        _player.OnDeath += Respawn;       
    }
    
    private void Respawn() // respawning character and Setting Spectator Camera
    {
        if (isRespawning) return; 
        //activate Spectator Camera and set Player's gameObject to false
        spectatorCamera.gameObject.SetActive(true); 
        Player.gameObject.SetActive(false);
        //start the player respawn
        StartCoroutine(RespawnTimer());
    }

    private IEnumerator RespawnTimer()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnTimer);

        _player.transform.position = playerSpawnPoint.position;
        _player.transform.rotation = playerSpawnPoint.rotation;

        spectatorCamera.gameObject.SetActive(false);
        Player.gameObject.SetActive(true);

        isRespawning = false;
    }

    public void GameOver(bool playerWon)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EndGameMenu.SetActive(true);
        //EndGameMenu DoTween here 
    }

    public void OnRestartButton()
    {
        string scene = SceneManager.GetActiveScene().name;
        GameManager.Instance.LoadScene(scene);
    }
}
