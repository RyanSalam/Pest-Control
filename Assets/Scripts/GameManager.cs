using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public static Character selectedPlayer;
    public static string selectedLevel;
    private MenuHandler menuHandler;

    public void SetMissionParam()
    {
        menuHandler = FindObjectOfType<MenuHandler>();
        selectedPlayer = menuHandler.GetCharacter();
        selectedLevel = menuHandler.GetLevel();
        if (selectedLevel != "" && selectedPlayer != null)
            LoadScene();
    }

    public void LoadScene()
    {
        StartCoroutine(LoadAsynchronously(selectedLevel));
    }

    IEnumerator LoadAsynchronously(string level)
    {
        yield return null;
        menuHandler.characterPanel.SetActive(false);
        menuHandler.levelPanel.SetActive(false);
        menuHandler.settingsPanel.SetActive(false);

        menuHandler.loadingScreenPanel.SetActive(true);
        menuHandler.videoPlayer.Prepare();
        menuHandler.videoPlayer.Play();

        AsyncOperation operation = SceneManager.LoadSceneAsync(level);
        operation.allowSceneActivation = false;
     
        menuHandler.loadingText.text = "LOADING...";

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            //Debug.Log("Progress: " + operation.progress);

            menuHandler.loadingBar.fillAmount = progress;

            if (operation.progress >= 0.9f)
            {
                menuHandler.loadPercentage.text = "100%";
                menuHandler.loadingText.text = "LOADED";
                menuHandler.levelStartButton.SetActive(true);
                menuHandler.SetSelectedButton(menuHandler.levelStartButton);

                if (menuHandler.startButtonPressed)
                {
                    operation.allowSceneActivation = true;
                }
            }
            else
            {
                // Update current loading progress 
                menuHandler.loadPercentage.text = (Mathf.RoundToInt(operation.progress * 100 * 1.1f)) + "%";
            }
            yield return null;
        }

    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }    

    public Character GetCharacter()
    {
        return selectedPlayer;
    }

    public void playClick()
    {
        GetComponent<AudioCue>().PlayAudioCue();
    }

    public void OnQuitButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
