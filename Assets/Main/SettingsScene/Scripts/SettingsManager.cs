using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsManager : SingletonObject<SettingsManager>
{
    [Header("References - Graphics")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullscreenToggle;

    [Header("References - Volume")]
    [SerializeField] Slider masterVolumeSlider;

    Resolution[] resolutions;

    public static void LoadSettingsAndApply()
    {
        Resolution[] allResolutions = Screen.resolutions;
        int resolutionIndex = 0;
        if (PlayerPrefs.HasKey("Resolution"))
            resolutionIndex = PlayerPrefs.GetInt("Resolution");
        else
        {
            for (int i = 0; i < allResolutions.Length; ++i)
                if (Screen.currentResolution.width == allResolutions[i].width && Screen.currentResolution.height == allResolutions[i].height && Screen.currentResolution.refreshRate == allResolutions[i].refreshRate)
                    resolutionIndex = i;

        }

        bool fullScreen = true;
        if (PlayerPrefs.HasKey("Fullscreen"))
            fullScreen = PlayerPrefs.GetInt("Fullscreen") != 0;
        else
            fullScreen = Screen.fullScreen;

        Screen.SetResolution(allResolutions[resolutionIndex].width, allResolutions[resolutionIndex].height, fullScreen);

        if (PlayerPrefs.HasKey("MasterVolume"))
            AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");
        else
            AudioListener.volume = 0.5f;
    }

    void Start()
    {
        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            // I only want this refresh rate
            if (Screen.currentResolution.refreshRate == resolutions[i].refreshRate)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
            }
            
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        List<Resolution> filteredResolution = new List<Resolution>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            // I only want this refresh rate
            if (Screen.currentResolution.refreshRate == resolutions[i].refreshRate)
            {
                filteredResolution.Add(resolutions[i]);
            }
        }
        Resolution resolution = filteredResolution[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume * 0.01f;
    }

    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value * 0.01f);
        PlayerPrefs.Save();
    }

    public void LoadSettings(int currResolutionIndex)
    {
        if (PlayerPrefs.HasKey("Resolution"))
            resolutionDropdown.value = PlayerPrefs.GetInt("Resolution");
        else
            resolutionDropdown.value = currResolutionIndex;

        if (PlayerPrefs.HasKey("Fullscreen"))
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") != 0;
        else
            fullscreenToggle.isOn = Screen.fullScreen;

        if (PlayerPrefs.HasKey("MasterVolume"))
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume") * 100f;
        else
            masterVolumeSlider.value = 50f;
    }

    public void OnDisable()
    {
        SaveSettings();
    }
}
