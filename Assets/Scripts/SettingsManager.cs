using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public AudioManager AM;
    public TMP_Dropdown resolutionDropdown;
    //public TMP_Dropdown qualityDropdown;
    //public TMP_Dropdown aaDropdown;
    //public TMP_Dropdown textureDropdown;
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;
    float currentMasterVolume;
    float currentMusicVolume;
    float currentSFXVolume;
    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        AM = FindObjectOfType<AudioManager>();
        
        //search the system for possible resolutions available and populate the drop down with that list.
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " +
                     resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width
                  && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();

        //If player preferences exist, set their settings.
        LoadSettings(currentResolutionIndex);
    }

    //Setter functions for settings values.
    public void SetMasterVolume(float volume)
    {
        AM.SetGroupVolume("Master", volume);
        currentMasterVolume = volume;
    }
    public void SetMusicVolume(float volume)
    {
        AM.SetGroupVolume("Music", volume);
        currentMusicVolume = volume;
    }
    public void SetSFXVolume(float volume)
    {
        AM.SetGroupVolume("SFX", volume);
        currentSFXVolume = volume;
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //public void SetTextureQuality(int textureIndex)
    //{
    //    QualitySettings.masterTextureLimit = textureIndex;
    //    qualityDropdown.value = 6;
    //}
    //public void SetAntiAliasing(int aaIndex)
    //{
    //    QualitySettings.antiAliasing = aaIndex;
    //    qualityDropdown.value = 6;
    //}

    //public void SetQuality(int qualityIndex)
    //{
    //    if (qualityIndex != 6) // if the user is not using 
    //                           //any of the presets
    //        QualitySettings.SetQualityLevel(qualityIndex);
    //    switch (qualityIndex)
    //    {
    //        case 0: // quality level - very low
    //            textureDropdown.value = 3;
    //            aaDropdown.value = 0;
    //            break;
    //        case 1: // quality level - low
    //            textureDropdown.value = 2;
    //            aaDropdown.value = 0;
    //            break;
    //        case 2: // quality level - medium
    //            textureDropdown.value = 1;
    //            aaDropdown.value = 0;
    //            break;
    //        case 3: // quality level - high
    //            textureDropdown.value = 0;
    //            aaDropdown.value = 0;
    //            break;
    //        case 4: // quality level - very high
    //            textureDropdown.value = 0;
    //            aaDropdown.value = 1;
    //            break;
    //        case 5: // quality level - ultra
    //            textureDropdown.value = 0;
    //            aaDropdown.value = 2;
    //            break;
    //    }

    //    qualityDropdown.value = qualityIndex;
    //}

    
    public void SaveSettings()  //Save the current setting values to the player preferences
    {
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value); Debug.Log("ResolutionPreference:" + resolutionDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("MasterVolumePreference", currentMasterVolume); Debug.Log("MasterVolumePreference:" + currentMasterVolume);
        PlayerPrefs.SetFloat("MusicVolumePreference", currentMusicVolume); Debug.Log("MusicVolumePreference:" + currentMusicVolume);
        PlayerPrefs.SetFloat("SFXVolumePreference", currentSFXVolume); Debug.Log("SFXVolumePreference:" + currentSFXVolume);

        //PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value); Debug.Log("QualitySettingPreference:" + qualityDropdown.value);
        //PlayerPrefs.SetInt("TextureQualityPreference", textureDropdown.value); Debug.Log("TextureQualityPreference:" + textureDropdown.value);
        //PlayerPrefs.SetInt("AntiAliasingPreference", aaDropdown.value); Debug.Log("AntiAliasingPreference:" + aaDropdown.value);

    }

    public void LoadSettings(int currentResolutionIndex) //Called at start based on the player preferences
    {

        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;
        
        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;

        if (PlayerPrefs.HasKey("MasterVolumePreference"))
            MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolumePreference");
        else
            MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolumePreference");

        if (PlayerPrefs.HasKey("MusicVolumePreference"))
            MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolumePreference");
        else
            MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolumePreference");

        if (PlayerPrefs.HasKey("SFXVolumePreference"))
            SFXVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolumePreference");
        else
            SFXVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolumePreference");
        
        //if (PlayerPrefs.HasKey("QualitySettingPreference"))
        //    qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
        //else
        //    qualityDropdown.value = 3;
        //if (PlayerPrefs.HasKey("TextureQualityPreference"))
        //    textureDropdown.value = PlayerPrefs.GetInt("TextureQualityPreference");
        //else
        //    textureDropdown.value = 0;
        //if (PlayerPrefs.HasKey("AntiAliasingPreference"))
        //    aaDropdown.value = PlayerPrefs.GetInt("AntiAliasingPreference");
        //else
        //    aaDropdown.value = 1;


    }


}
