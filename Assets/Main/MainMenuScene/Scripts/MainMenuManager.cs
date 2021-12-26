using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : SingletonObject<MainMenuManager>
{
    [Header("References")]
    [SerializeField]
    MainMenuVFXManager mainMenuVFXManager;

    [SerializeField]
    TextMeshProUGUI playerNameText;
    [SerializeField]
    TextMeshProUGUI playerCommonCurrencyText;
    [SerializeField]
    TextMeshProUGUI playerPremiumCurrencyText;

    [SerializeField]
    RectTransform battleMenu;

    bool openSettings = false;

    BattleModeSelection_Class.BATTLEMODE_SELECTION selectedBattlemode = BattleModeSelection_Class.BATTLEMODE_SELECTION.NONE;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("MainMenuSkipVFX") && PlayerPrefs.GetInt("MainMenuSkipVFX") != 0)
        {
            // Skip VFX
            mainMenuVFXManager.ForceEndVFX();
            // Reset value to false
            PlayerPrefs.SetInt("MainMenuSkipVFX", 0);
        }
        UpdatePlayerDataDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainMenuVFXManager.vfxStatus != MainMenuVFXManager.VFX_STATUS.ENDED && Input.GetMouseButtonDown(0)) // Skip Intro VFX when player LMB down
        {
            mainMenuVFXManager.ForceEndVFX();
        }

        // CODE HERE to remove once testing of cloudscript works
        if (Input.GetKeyDown(KeyCode.Home))
        {
            SceneManager.LoadScene("TestCloudScript");
        }
    }

    public void UpdatePlayerDataDisplay()
    {
        playerNameText.text = DataManager.instance.playerName;
        playerCommonCurrencyText.text = "Spare Parts:\n" + DataManager.instance.commonCurrency;
        playerPremiumCurrencyText.text = "Techno Cubes:\n" + DataManager.instance.premiumCurrency;
    }

    public void SwitchTeamMainMenu(bool toDefenders)
    {
        if (openSettings)
            return;
        if (toDefenders)
        {
            DataManager.instance.chosenGameTeam = TEAM_TYPE.DEFENDERS;
            DataManager.instance.SetLastTeam(DataManager.instance.chosenGameTeam);
            SceneTransitionManager.instance.SwitchScene("MainMenu_Defenders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
        }
        else
        {
            DataManager.instance.chosenGameTeam = TEAM_TYPE.INVADERS;
            DataManager.instance.SetLastTeam(DataManager.instance.chosenGameTeam);
            SceneTransitionManager.instance.SwitchScene("MainMenu_Invaders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
        }
        AudioManager.instance.StopBGM();
    }

    public void GoToInventory()
    {
        if (openSettings)
            return;
        DataManager.instance.SetLastTeam(DataManager.instance.chosenGameTeam);
        SceneTransitionManager.instance.SwitchScene("InventoryScene", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
    }

    public void ToggleBattleMenu(bool toOpen)
    {
        if (!openSettings)
            battleMenu.gameObject.SetActive(toOpen);
    }

    public void BattleSelection(BattleModeSelection_Class selectionClass)
    {
        selectedBattlemode = selectionClass.selection;
        AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._buttonClickSFX);
    }

    public void GoButton()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._uiCloseSound);
        AudioManager.instance.StopBGM();
        switch (selectedBattlemode)
        {
            case BattleModeSelection_Class.BATTLEMODE_SELECTION.SINGLEPLAYER:
                {
                    SceneTransitionManager.instance.SwitchScene("GameScene", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
                    return;
                }
        }
       
    }

    public void ToggleSettingsMenu()
    {
        openSettings = !openSettings;
        if (openSettings)
        {
            SceneManager.LoadSceneAsync("SettingScene", LoadSceneMode.Additive);
            AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._uiOpenSound);
        }
        else
        {
            SceneManager.UnloadSceneAsync("SettingScene");
            AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._uiCloseSound);
        }
    }
}

