using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainHUDManager : SingletonObject<MainHUDManager>
{
    [Header("References")]
    [SerializeField]
    TextMeshProUGUI coinText;
    [SerializeField]
    TextMeshProUGUI gameTimer;
    [SerializeField]
    TextMeshProUGUI spawnWaveTimer;
    [SerializeField]
    RectTransform spawnWaveTimerParent;

    bool openSettings = false;
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
    }

    public void ToggleShopMenu(bool toOpen)
    {
        // CODE HERE to toggle shop menu
    }

    public void ToggleSettingsMenu()
    {
        openSettings = !openSettings;
        if (openSettings)
        {
            SceneManager.LoadSceneAsync("SettingScene", LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.UnloadSceneAsync("SettingScene");
        }
    }

    public void ToggleSpawnWaveUI(bool visible)
    {
        spawnWaveTimerParent.gameObject.SetActive(visible);
    }
}
