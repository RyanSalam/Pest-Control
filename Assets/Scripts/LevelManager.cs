﻿using System.Collections;
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
    [SerializeField] private TextMeshProUGUI EndGameText;

    private static LevelManager _instance;
    public static LevelManager Instance { get{ return _instance; } }

    private void Awake() //On Awake set check LevelManager's Instance and playerSpawnPoint
    {
        if (_instance) //if instance exists destroy gameObject
            Destroy(gameObject);
        else
            _instance = this; //if instance is this check if there is playerSpawnPoint
        if(playerSpawnPoint == null)
        {
            Debug.LogError("No Spawn Point Found");
            //Respawn Player if there is no Spawn Point Found
            playerSpawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform; 
        }
        //uncomment after GameManager is finnished line 41-42
        //player = Instantiate(GameManager.playerPrefab, playerSpawnPoint.transform.position, Quaternion.identity); 
        //_player.DamageBody.OnDeath += Respawn;

        if (_player == null)
        {
            Debug.LogError("There was no player selected from character selection");
        }
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
        EndGameText.text = playerWon ? "Victory!" : "Defeat!";
    }

    public void OnRestartButton()
    {
        string scene = SceneManager.GetActiveScene().name;
        //GameManager.instance.LoadScene(scene); //uncomment after GameManager is finnished
    }
}
