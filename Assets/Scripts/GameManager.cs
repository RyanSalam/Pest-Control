using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public static Actor_Player selectedPlayer;
    public static string SelectedLevel;

    public void Restart()
    {
        string scene = SceneManager.GetActiveScene().name;
        LoadScene(scene);
    }

    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void SetLevel(string name)
    {
        SelectedLevel = name;
    }

    public void OnQuitButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
