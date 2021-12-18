using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainHUDManager : SingletonObject<MainHUDManager>
{
    [Header("External References")]
    [SerializeField] RectTransform shopUI;

    [Header("References")]
    [SerializeField]
    TextMeshProUGUI coinText;
    [SerializeField]
    TextMeshProUGUI gameTimer;
    [SerializeField]
    TextMeshProUGUI spawnWaveTimer;
    [SerializeField]
    RectTransform spawnWaveTimerParent;
    [SerializeField]
    Button spectateButton;
    [SerializeField]
    TextMeshProUGUI spectateButtonText;

    bool openSettings = false;
    bool openShop = false;
    // Update is called once per frame
    void Update()
    {
        coinText.text = "| " + PlayerManager.instance.coins.ToString();

        // Game Timer
        {
            float gameTime = GameplayManager.instance.gameTimer;
            string minutes = ((int)(gameTime / 60f)).ToString();
            if (minutes.Length == 1)
                minutes = minutes.Insert(0, "0");
            string seconds = ((int)gameTime % 60).ToString();
            if (seconds.Length == 1)
                seconds = seconds.Insert(0, "0");
            gameTimer.text = minutes + " : " + seconds;
        }

        // Spawn Wave Timer
        {
            int seconds = 0;
            if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
            {
                seconds = (int)(GameplayManager.instance.defenderSpawnCooldown / GameplayManager.instance.defenderSpawnCooldownMultiplier);
            }
            else if (PlayerManager.instance.playerTeam == TEAM_TYPE.INVADERS)
            {
                seconds = (int)(GameplayManager.instance.invaderSpawnCooldown / GameplayManager.instance.invaderSpawnCooldownMultiplier);              
            }
            else
            {
                Debug.LogError("MainHUDManager: SpawnWaveTimer is not defined to handle the team of the current player!");
            }
            spawnWaveTimer.text = "Reinforcements E.T.A:\n<i>" + seconds.ToString() + "</i>";
        }

        // Spectate Button
        {
            spectateButton.gameObject.SetActive(!PlayerManager.instance.isControllingEntity);
            if (PlayerManager.instance.freeRoamCamera.isSpectate)
                spectateButtonText.text = "| ON";
            else
                spectateButtonText.text = "| OFF";
        }
    }

    public void ToggleShopMenu()
    {
        // CODE HERE to toggle shop menu\
        openShop = !openShop;
        if (openShop)
        {
            PlayerManager.instance.isFocused = false;
            shopUI.gameObject.SetActive(true);
            AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._uiOpenSound);
        }
        else
        {
            if (!openSettings)
                PlayerManager.instance.isFocused = true;
            shopUI.gameObject.SetActive(false);
            AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._uiCloseSound);
        }
    }

    public void ToggleSettingsMenu()
    {
        openSettings = !openSettings;
        if (openSettings)
        {
            SceneManager.LoadSceneAsync("SettingScene", LoadSceneMode.Additive);
            PlayerManager.instance.isFocused = false;
            AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._uiOpenSound);
        }
        else
        {
            SceneManager.UnloadSceneAsync("SettingScene");
            if (!openShop)
                PlayerManager.instance.isFocused = true;
            AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._uiCloseSound);
        }
    }

    public void ToggleSpawnWaveUI(bool visible)
    {
        spawnWaveTimerParent.gameObject.SetActive(visible);
    }

    public void ToggleSpectate()
    {
        if (PlayerManager.instance.freeRoamCamera.isSpectate)
        {
            PlayerManager.instance.freeRoamCamera.StopSpectate();
        }
        else
        {
            PlayerManager.instance.freeRoamCamera.StartSpectate();
        }
    }
}
