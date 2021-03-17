using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class MenuHandler : MonoBehaviour
{
    public Character character;
    public string levelname;
    AudioCue Cues;

    [Header("Panel GameObjects")]
    public GameObject settingsPanel;
    public GameObject characterPanel;
    public GameObject levelPanel;
    public GameObject loadingScreenPanel;

    [Header("Loading Screen")]
    public bool startButtonPressed = false;
    public GameObject levelStartButton;
    public TMP_Text loadingText;
    public TMP_Text loadPercentage;
    public VideoPlayer videoPlayer;
    public Image loadingBar;

    [Header("Character Panel")]
    [SerializeField] TMP_Text charName;
    [SerializeField] TMP_Text charDesc;
    [SerializeField] Image charProfile;
    [SerializeField] Image A1_image;
    [SerializeField] TMP_Text A1_text;
    [SerializeField] TMP_Text A1_name;
    [SerializeField] Image A2_image;
    [SerializeField] TMP_Text A2_text;
    [SerializeField] TMP_Text A2_name;

    [Header("Level Panel")]
    [SerializeField] TMP_Text maptitle;
    [SerializeField] Image map;

    [Header("ControllerInput")]
    [SerializeField] GameObject startingButton;

    private void Awake()
    {
        startButtonPressed = false;
        videoPlayer.Prepare();
    }


    // Start is called before the first frame update
    void Start()
    {
        settingsPanel.SetActive(true);
        settingsPanel.GetComponent<SettingsManager>().LoadAudioLevels();
        settingsPanel.SetActive(false);
        Cues = GetComponent<AudioCue>();

        SetSelectedButton(startingButton);

        levelStartButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PressedStartButton()
    {
        startButtonPressed = true;
    }

    public void TurnObjectOn(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void SetSelectedButton(GameObject obj)
    {
        if(Gamepad.current != null)
        EventSystem.current.SetSelectedGameObject(obj);
    }

    public void TurnObjectOff(GameObject obj)
    {    
            obj.SetActive(false);
    }

    public void Quit()
    {
        FindObjectOfType<GameManager>().OnQuitButton();
    }

    public void SelectCharacter(Character c)
    {
        character = c;
        charName.text = c.c_name;
        charDesc.text = c.c_desc;
        charProfile.sprite = c.c_profile;
        A1_image.sprite = c.A1_image;
        A1_text.text = c.ab1.abilityDesc;
        A1_name.text = c.ab1.abilityName;
        A2_image.sprite = c.A2_image;
        A2_text.text = c.ab2.abilityDesc;
        A2_name.text = c.ab2.abilityName;
        Cues.PlayAudioCue(c.CharacterChosen);
    }

    public void SelectMap(Sprite mapImage)
    {
        map.sprite = mapImage;
    }
    public void SelectLevel(string text)
    {
        maptitle.text = text;
    }
    public void SelectScene(string scene)
    {
        levelname = scene;
    }

    public Character GetCharacter()
    {
        return character;
    }
    public String GetLevel()
    {
        return levelname;
    }

    
}
