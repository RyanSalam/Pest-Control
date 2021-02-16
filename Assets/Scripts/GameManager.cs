using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public static Character selectedPlayer;
    public static string selectedLevel;

    public void SetMissionParam()
    {
        selectedPlayer = FindObjectOfType<MenuHandler>().GetCharacter();
        selectedLevel = FindObjectOfType<MenuHandler>().GetLevel();
        LoadScene();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(selectedLevel);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }    

    public void OnQuitButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
