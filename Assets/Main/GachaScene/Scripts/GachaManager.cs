using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GachaManager : SingletonObject<GachaManager>
{
    [Header("External References")]
    public GachaTypeContainer currentGachaList;
    public EquipmentColor equipmentColor;
    [Header("Settings")]
    public TEAM_TYPE gachaTeam;
    public Sprite wingIcon;
    public Sprite lightIcon;
    public Sprite heavyIcon;
    [Header("UI References")]
    public GameObject gachaMenu;
    public Text gachaTitle;
    public Text gachaDescription;
    public Image gachaBanner;
    [Space]
    public GameObject confirmationMenu;
    public GameObject processingMenu;
    [Space]
    public GameObject resultMenu;
    public Image resultBorder;
    public Image resultIcon;
    public ResultInfoMenu resultInfoMenu;
    [Space]
    public TextMeshProUGUI commonCurrencyText;
    public TextMeshProUGUI premiumCurrencyText;
    public Button3D gachaButton;

    public enum GACHA_PHASE
    {
        ANIM,
        VIEW,
    }
    [HideInInspector]
    public GACHA_PHASE gachaPhase;

    /*In-script Values */
    List<GachaType> allAvailableGacha;
    int gachaSelector;
    bool openSettings;
    void Start()
    {
        gachaMenu.SetActive(false);
        gachaPhase = GACHA_PHASE.ANIM;
        if (DataManager.instance != null)
            gachaTeam = DataManager.instance.chosenGameTeam;

        allAvailableGacha = currentGachaList.GetAllGachaTypeOfTeam(gachaTeam);
        gachaSelector = 0;
        openSettings = false;
        GetComponent<InventoryAnimManager>().finishAnim.AddListener(OnAnimFinish);
        UpdateGachaSelector();
        DataManager.instance.currencyLoaded.AddListener(CurrencyLoaded);
    }


    void Update()
    {
        if (DataManager.instance != null)
        {
            commonCurrencyText.text = "Spare Parts:\n" + DataManager.instance.commonCurrency;
            premiumCurrencyText.text = "Techno Cubes:\n" + DataManager.instance.premiumCurrency;
            if (DataManager.instance.premiumCurrency >= 100)
                gachaButton.enabled = false;
            else
                gachaButton.enabled = true;
        }
    }


    public void SwitchLeft()
    {
        if (gachaSelector > 0)
            --gachaSelector;
        else
            gachaSelector = allAvailableGacha.Count - 1;
        UpdateGachaSelector();
    }
    public void SwitchRight()
    {
        if (gachaSelector < allAvailableGacha.Count - 1)
            ++gachaSelector;
        else
            gachaSelector = 0;
        UpdateGachaSelector();
    }

    public void AskConfirmation()
    {
        // CODE HERE to set confirmation text
        confirmationMenu.SetActive(true);
    }

    public void ConfirmButtonSelected(bool isYes)
    {
        if (isYes)
        {
            // CODE HERE to activate gacha
            // LoadCurrency()
            // EnableResultMenu()
        }
        confirmationMenu.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        PlayerPrefs.SetInt("MainMenuSkipVFX", 1);
        if (gachaTeam == TEAM_TYPE.DEFENDERS)
            SceneTransitionManager.instance.SwitchScene("MainMenu_Defenders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
        else
            SceneTransitionManager.instance.SwitchScene("MainMenu_Invaders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
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

    public void DisableResultMenu()
    {
        resultMenu.SetActive(false);
        resultInfoMenu.gameObject.SetActive(false);
    }

    void EnableResultMenu(EntityEquipment entityEquipment)
    {
        resultBorder.color = equipmentColor.GetColorOfRarity(entityEquipment.equipmentRarity);
        switch (entityEquipment.equipmentType)
        {
            case EntityEquipment.EQUIPMENT_TYPE.WING:
                resultIcon.sprite = wingIcon;
                break;
            case EntityEquipment.EQUIPMENT_TYPE.LIGHT_WEAPON:
                resultIcon.sprite = lightIcon;
                break;
            case EntityEquipment.EQUIPMENT_TYPE.HEAVY_WEAPON:
                resultIcon.sprite = heavyIcon;
                break;
        }
        resultMenu.SetActive(true);
    }

    void LoadCurrency()
    {
        DataManager.instance.LoadCurrencyData();
    }

    void CurrencyLoaded()
    {
        processingMenu.SetActive(false);
    }

    void OnAnimFinish()
    {
        gachaPhase = GACHA_PHASE.VIEW;
        gachaMenu.SetActive(true);
    }
    void UpdateGachaSelector()
    {
        GachaType gachaType = allAvailableGacha[gachaSelector];
        gachaTitle.text = gachaType.gachaTitle;
        gachaDescription.text = gachaType.gachaDescription;
        gachaBanner.sprite = gachaType.imageBanner;
    }

}
