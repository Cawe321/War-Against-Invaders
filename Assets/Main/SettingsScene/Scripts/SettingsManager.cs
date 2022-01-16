using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : SingletonObject<SettingsManager>
{
    [Header("References - Graphics")]
    [SerializeField] Toggle fullscreenToggle;

    [Header("References - Volume")]
    [SerializeField] Slider masterVolumeSlider;

    [Header("References - Quit Items")]
    [SerializeField] GameObject confirmQuitMenu;
    [SerializeField] Text confirmQuitText;
    [SerializeField] Button quitToMainMenuButton;
    
    enum QUIT_TYPE
    {
        MAIN_MENU,
        DESKTOP,
    }
    QUIT_TYPE chosenQuitType;


    public static void LoadSettingsAndApply()
    {
      
        bool fullScreen = true;
        if (PlayerPrefs.HasKey("Fullscreen"))
            fullScreen = PlayerPrefs.GetInt("Fullscreen") != 0;
        else
            fullScreen = Screen.fullScreen;

        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullScreen);

        if (PlayerPrefs.HasKey("MasterVolume"))
            AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");
        else
            AudioListener.volume = 0.5f;
    }

    void Start()
    {
       
        if (SceneManager.GetActiveScene().name == "GameScene")
            quitToMainMenuButton.gameObject.SetActive(true);
        else 
            quitToMainMenuButton.gameObject.SetActive(false);


        LoadSettings();
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
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value * 0.01f);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Fullscreen"))
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") != 0;
        else
            fullscreenToggle.isOn = Screen.fullScreen;

        if (PlayerPrefs.HasKey("MasterVolume"))
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume") * 100f;
        else
            masterVolumeSlider.value = 50f;
    }

    public void OnQuitToMainMenuClicked()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.Disconnect();
        chosenQuitType = QUIT_TYPE.MAIN_MENU;
        confirmQuitText.text = "Are you sure you want to quit to main menu?";
        confirmQuitMenu.SetActive(true);
    }

    public void OnQuitToDesktopClicked()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.Disconnect();
        chosenQuitType = QUIT_TYPE.DESKTOP;
        confirmQuitText.text = "ARE you sure you want to quit to desktop?";
        confirmQuitMenu.SetActive(true);
    }

    public void DisableQuitConfirmMenu(bool isYes)
    {
        if (isYes)
        {
            switch (chosenQuitType)
            {
                case QUIT_TYPE.MAIN_MENU:
                    {
                        if (DataManager.instance.chosenGameTeam == TEAM_TYPE.DEFENDERS)
                            SceneTransitionManager.instance.SwitchScene("MainMenu_Defenders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
                        else
                            SceneTransitionManager.instance.SwitchScene("MainMenu_Invaders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
                        SceneManager.UnloadSceneAsync("SettingScene");
                        break;
                    }
                case QUIT_TYPE.DESKTOP:
                    {
                        Application.Quit(0);
                        break;
                    }
            }
        }
        confirmQuitMenu.gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        SaveSettings();
    }
}
