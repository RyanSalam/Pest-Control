using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuHandler : MonoBehaviour
{
    public Character character;
    public string levelname;
    AudioCue Cues;

    public GameObject settingsPanel;
    public GameObject characterPanel;
    public GameObject levelPanel;
    

    //Character Panel
    [SerializeField] TMP_Text charName;
    [SerializeField] TMP_Text charDesc;
    [SerializeField] Image charProfile;
    [SerializeField] Image A1_image;
    [SerializeField] TMP_Text A1_text;
    [SerializeField] Image A2_image;
    [SerializeField] TMP_Text A2_text;

    //Level Panel
    [SerializeField] TMP_Text maptitle;
    [SerializeField] Image map;

    private void Awake()
    {
       
    }


    // Start is called before the first frame update
    void Start()
    {
        settingsPanel.SetActive(true);
        settingsPanel.GetComponent<SettingsManager>().LoadAudioLevels();
        settingsPanel.SetActive(false);
        Cues = GetComponent<AudioCue>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TurnObjectOn(GameObject obj)
    {
        obj.SetActive(true);
        Debug.Log("clicked");
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
        A1_text.text = c.A1_text;
        A2_image.sprite = c.A2_image;
        A2_text.text = c.A2_text;
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
