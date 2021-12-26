using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public EntityList entityList;

    public TEAM_TYPE inventoryTeam;

    public EquipmentColor equipmentColor;
    [Header("References")]
    public Transform objectContainer;
    [Space]
    public GameObject modelSelectionUI;
    public Text modelTitle;
    public Text modelDescription;
    [Space]
    public GameObject inventoryUI;
    public Text planeTitle;
    public Image planeIcon;
    public EntityEquipmentUI wingUI;
    public Image wingImage;
    public Image wingBorder;
    public EntityEquipmentUI lightUI;
    public Image lightImage;
    public Image lightBorder;
    public EntityEquipmentUI heavyUI;
    public Image heavyImage;
    public Image heavyBorder;
    [Space]
    public Sprite noneIcon;
    public Sprite wingIcon;
    public Sprite lightWeaponIcon;
    public Sprite heavyWeaponIcon;
    public RectTransform inventoryFrame;
    public RectTransform equipmentUIContainer;
    public EntityEquipmentUI sampleEquipmentUI;
    [Space]
    public TextMeshProUGUI commonCurrencyText;
    public TextMeshProUGUI premiumCurrencyText;
    [Space]
    public RectTransform equipStatsUI;
    public Image equipStatsIcon;
    public Image equipStatsBorder;
    public Text equipStatsLevel;
    public Text equipStatsMainStat;
    public Text equipStatsSubStats;
    public Text equipStatsUpgradeButtonText;
    public Text equipStatsSellButtonText;
    public Text equipStatsEquipButtonText;
    EntityEquipmentUI equipStatsEntity = null;
    [Space]
    public RectTransform confirmationMenu;
    public Text confirmationMenuText;


    enum CONFIRMATION_TYPE
    {
        UPGRADE, SELL,
    }
    CONFIRMATION_TYPE confirmationMenuType;
    EntityEquipment confirmationMenuEquipment;
    [Space]
    public GameObject processingMenu;

    enum INVENTORY_PHASE
    {
        ANIM,
        SELECTION,
        SELECTED,
    }
    INVENTORY_PHASE inventoryPhase;

    InventoryAnimManager inventoryAnimManager;


    Dictionary<EntityTypes, GameObject> objectDictionary;



    // Cache values to store player's settings
    int rotationNumber;
    bool openSettings = false;
    EquipmentTypeSelector lastEquipmentTypeSelector = null;

    void Start()
    {
        if (DataManager.instance != null)
            inventoryTeam = DataManager.instance.chosenGameTeam;

        inventoryAnimManager = GetComponent<InventoryAnimManager>();
        inventoryAnimManager.finishAnim.AddListener(OnAnimDone);
        inventoryPhase = INVENTORY_PHASE.ANIM;
        rotationNumber = 0;

        objectDictionary = new Dictionary<EntityTypes, GameObject>();
        if (inventoryTeam == TEAM_TYPE.DEFENDERS)
        {
            objectDictionary.Add(EntityTypes.StealthWing, Instantiate(entityList.GetEntityObject(EntityTypes.StealthWing).displayModel, objectContainer));
            objectDictionary.Add(EntityTypes.Whitebeard, Instantiate(entityList.GetEntityObject(EntityTypes.Whitebeard).displayModel, objectContainer));
            objectDictionary.Add(EntityTypes.F16, Instantiate(entityList.GetEntityObject(EntityTypes.F16).displayModel, objectContainer));
        }
        else if (inventoryTeam == TEAM_TYPE.INVADERS)
        {
            objectDictionary.Add(EntityTypes.Mako, Instantiate(entityList.GetEntityObject(EntityTypes.Mako).displayModel, objectContainer));
            objectDictionary.Add(EntityTypes.X_Wing, Instantiate(entityList.GetEntityObject(EntityTypes.X_Wing).displayModel, objectContainer));
            objectDictionary.Add(EntityTypes.Deathrow, Instantiate(entityList.GetEntityObject(EntityTypes.Deathrow).displayModel, objectContainer));
        }
        else
            Debug.LogError("InventoryManager: Error! A team that is not supported has been set for this InventoryScene.");

        modelSelectionUI.SetActive(false);
        inventoryUI.SetActive(false);
        confirmationMenu.gameObject.SetActive(false);
        processingMenu.SetActive(false);
        UpdateModel();

        DataManager.instance.inventoryLoaded.AddListener(InventoryLoaded);
        DataManager.instance.inventorySaved.AddListener(InventorySaved);
        DataManager.instance.currencyLoaded.AddListener(CurrencyLoaded);
        ReloadInventory();
    }

    #region WAIT_FOR_INVENTORY_SERVER_FUNCTIONS
    void ReloadInventory()
    {
        processingMenu.SetActive(true);
        DataManager.instance.LoadPlayerInventory();
    }

    void InventoryLoaded()
    {
        LoadCurrency();

        if (inventoryPhase != INVENTORY_PHASE.ANIM)
            SelectPlane();
        if (lastEquipmentTypeSelector != null)
            ShowInventoryOfType(lastEquipmentTypeSelector);
        if (equipStatsEntity != null)
            ItemSelected(equipStatsEntity);
    }

    void LoadCurrency()
    {
        DataManager.instance.LoadCurrencyData();
    }

    void CurrencyLoaded()
    {
        processingMenu.SetActive(false);
    }

    void SaveInventory()
    {
        processingMenu.SetActive(true);
        DataManager.instance.SavePlayerInventory(DataManager.instance.loadedPlayerEquipment, DataManager.instance.loadedPlayerPlaneEquipment);
    }

    void InventorySaved()
    {
        ReloadInventory();
    }
    #endregion


    void Update()
    {
        if (inventoryPhase == INVENTORY_PHASE.SELECTION)
        {
            if (Input.GetButtonDown("SpectateLeft"))
                LeftSelect();

            if (Input.GetButtonDown("SpectateRight"))
                RightSelect();
        }
        else if (inventoryPhase == INVENTORY_PHASE.SELECTED)
        {
            if (DataManager.instance != null)
            {
                commonCurrencyText.text = "Spare Parts:\n" + DataManager.instance.commonCurrency;
                premiumCurrencyText.text = "Techno Cubes:\n" + DataManager.instance.premiumCurrency;
            }
        }
    }

    #region PUBLIC_PLANE_SELECTION
    public void LeftSelect()
    {
        if (rotationNumber == 0)
            rotationNumber = objectDictionary.Count - 1;
        else --rotationNumber;
        UpdateModel();
    }

    public void RightSelect()
    {
        if (rotationNumber == objectDictionary.Count - 1)
            rotationNumber = 0;
        else ++rotationNumber;
        UpdateModel();
    }

    public void ReturnToMainMenu()
    {
        PlayerPrefs.SetInt("MainMenuSkipVFX", 1);
        if (inventoryTeam == TEAM_TYPE.DEFENDERS)
            SceneTransitionManager.instance.SwitchScene("MainMenu_Defenders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
        else
            SceneTransitionManager.instance.SwitchScene("MainMenu_Invaders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
    }
    public void SelectPlane()
    {
        // Get Info about object
        EntityTypes entityType = objectDictionary.Keys.ToArray()[rotationNumber];
        EntityObject entityObject = entityList.GetEntityObject(entityType);

        // Set & Display Info
        planeTitle.text = entityObject.entityName;
        planeIcon.sprite = entityObject.entityIcon;

        PlaneEquipmentEntity selectedPlaneEquipment = null;
        foreach (PlaneEquipmentEntity planeEquipmentEntity in DataManager.instance.loadedPlayerPlaneEquipment)
        {
            if (planeEquipmentEntity.entityType == entityType)
            {
                selectedPlaneEquipment = planeEquipmentEntity;
                break;
            }
        }
        if (selectedPlaneEquipment == null) // this value does not exist
        {
            selectedPlaneEquipment = new PlaneEquipmentEntity() { entityType = entityType, teamType = inventoryTeam, lightID = -1, wingID = -1, heavyID = -1 };
            List<PlaneEquipmentEntity> newList = new List<PlaneEquipmentEntity>(DataManager.instance.loadedPlayerPlaneEquipment);
            newList.Add(selectedPlaneEquipment);
            DataManager.instance.loadedPlayerPlaneEquipment = newList.ToArray();
            DataManager.instance.SavePlayerInventory(DataManager.instance.loadedPlayerEquipment, DataManager.instance.loadedPlayerPlaneEquipment);
        }

        EntityEquipment wingEntity = FindEquipmentEntityOfID(selectedPlaneEquipment.wingID);
        if (wingEntity != null)
        {
            wingUI.entityEquipment = wingEntity;
            wingImage.sprite = wingIcon;
            wingBorder.color = equipmentColor.GetColorOfRarity(wingEntity.equipmentRarity);
        }
        else
        {
            wingUI.entityEquipment = null;
            wingImage.sprite = noneIcon;
            wingBorder.color = Color.white;
        }

        EntityEquipment lightEntity = FindEquipmentEntityOfID(selectedPlaneEquipment.lightID);
        if (lightEntity != null)
        {
            lightUI.entityEquipment = lightEntity;
            lightImage.sprite = lightWeaponIcon;
            lightBorder.color = equipmentColor.GetColorOfRarity(lightEntity.equipmentRarity);
        }
        else
        {
            lightUI.entityEquipment = null;
            lightImage.sprite = noneIcon;
            lightBorder.color = Color.white;
        }

        EntityEquipment heavyEntity = FindEquipmentEntityOfID(selectedPlaneEquipment.heavyID);
        if (heavyEntity != null)
        {
            heavyUI.entityEquipment = heavyEntity;
            heavyImage.sprite = heavyWeaponIcon;
            heavyBorder.color = equipmentColor.GetColorOfRarity(heavyEntity.equipmentRarity);
        }
        else
        {
            heavyUI.entityEquipment = null;
            heavyImage.sprite = noneIcon;
            heavyBorder.color = Color.white;
        }

        // UI is ready to show to player
        SwitchToSelected();
    }

    EntityEquipment FindEquipmentEntityOfID(int id)
    {
        foreach (EntityEquipment entityEquipment in DataManager.instance.loadedPlayerEquipment)
        {
            if (entityEquipment.equipmentID == id)
                return entityEquipment;
        }
        return null;
    }
    #endregion

    #region PUBLIC_PLANE_SELECTED
    public void BackToSelection()
    {
        SwitchToSelection();
    }

    public void ShowInventoryOfType(EquipmentTypeSelector equipmentTypeSelector)
    {
        lastEquipmentTypeSelector = equipmentTypeSelector;

        inventoryFrame.gameObject.SetActive(true);

        EntityEquipment.EQUIPMENT_TYPE equipmentType = equipmentTypeSelector.equipmentType;

        // Clear the existing equipment UIs
        foreach (Transform transform in equipmentUIContainer)
        {
            if (transform.gameObject != sampleEquipmentUI.gameObject)
                Destroy(transform.gameObject);
        }

        List<EntityEquipment> allEntityEquipments = new List<EntityEquipment>(DataManager.instance.loadedPlayerEquipment);
        List<EntityEquipment> filteredEntityEquipments = new List<EntityEquipment>();
        foreach (EntityEquipment entityEquipment in allEntityEquipments)
        {
            //  Check whether this equipment is equipped. If true, it shouldnt be in the list
            if (entityEquipment.teamType == inventoryTeam && entityEquipment.equipmentType == equipmentType && !CheckIfEquipmentIsEquipped(entityEquipment)) // This is the equipment I am looking for
                filteredEntityEquipments.Add(entityEquipment);
        }

        // Bubble Sorting
        for (int i = 0; i < filteredEntityEquipments.Count - 1; ++i)
        {
            EntityEquipment currentEntity = filteredEntityEquipments[i];
            for (int j = i + 1; j < filteredEntityEquipments.Count; ++i)
            {
                EntityEquipment comparisonEntity = filteredEntityEquipments[j];
                if (currentEntity.equipmentRarity > comparisonEntity.equipmentRarity) // this means that comparisonEntity is rarer than currentEntity
                {
                    filteredEntityEquipments[i] = comparisonEntity;
                    filteredEntityEquipments[j] = currentEntity;
                }
                else if (currentEntity.equipmentRarity == comparisonEntity.equipmentRarity // this means that they have same rarity, however comparisonEntity has a higher level
                        && currentEntity.level < comparisonEntity.level)
                {
                    filteredEntityEquipments[i] = comparisonEntity;
                    filteredEntityEquipments[j] = currentEntity;
                }
            }
        }


        // Cache the respective icon
        Sprite icon = null;
        switch (equipmentType)
        {
            case EntityEquipment.EQUIPMENT_TYPE.WING:
                {
                    icon = wingIcon;
                    break;
                }
            case EntityEquipment.EQUIPMENT_TYPE.LIGHT_WEAPON:
                {
                    icon = lightWeaponIcon;
                    break;
                }
            case EntityEquipment.EQUIPMENT_TYPE.HEAVY_WEAPON:
                {
                    icon = heavyWeaponIcon;
                    break;
                }
        }

        // Time to display the UIs
        foreach (EntityEquipment entityEquipment in filteredEntityEquipments)
        {
            EntityEquipmentUI newUI = Instantiate(sampleEquipmentUI, equipmentUIContainer);
            newUI.entityEquipment = entityEquipment;
            newUI.level.text = $"Level { entityEquipment.level }";
            newUI.icon.sprite = icon;
            newUI.border.color = equipmentColor.GetColorOfRarity(entityEquipment.equipmentRarity);
            newUI.gameObject.SetActive(true);
        }
    }

    public void EquipButtonPressed()
    {
        EntityEquipment targetEntity = equipStatsEntity.entityEquipment;
        EntityTypes entityType = objectDictionary.Keys.ToArray()[rotationNumber];

        if (CheckIfEquipmentIsEquipped(targetEntity))
        {
            foreach (PlaneEquipmentEntity planeEquipmentEntity in DataManager.instance.loadedPlayerPlaneEquipment)
            {
                if (planeEquipmentEntity.entityType == entityType)
                {
                    switch (targetEntity.equipmentType)
                    {
                        case EntityEquipment.EQUIPMENT_TYPE.WING:
                            {
                                planeEquipmentEntity.wingID = -1;
                                break;
                            }
                        case EntityEquipment.EQUIPMENT_TYPE.LIGHT_WEAPON:
                            {
                                planeEquipmentEntity.lightID = -1;
                                break;
                            }
                        case EntityEquipment.EQUIPMENT_TYPE.HEAVY_WEAPON:
                            {
                                planeEquipmentEntity.heavyID = -1;
                                break;
                            }
                    }

                }
            }
        }
        else
        {
            foreach (PlaneEquipmentEntity planeEquipmentEntity in DataManager.instance.loadedPlayerPlaneEquipment)
            {
                if (planeEquipmentEntity.entityType == entityType)
                {
                    switch (targetEntity.equipmentType)
                    {
                        case EntityEquipment.EQUIPMENT_TYPE.WING:
                            {
                                planeEquipmentEntity.wingID = targetEntity.equipmentID;
                                break;
                            }
                        case EntityEquipment.EQUIPMENT_TYPE.LIGHT_WEAPON:
                            {
                                planeEquipmentEntity.lightID = targetEntity.equipmentID;
                                break;
                            }
                        case EntityEquipment.EQUIPMENT_TYPE.HEAVY_WEAPON:
                            {
                                planeEquipmentEntity.heavyID = targetEntity.equipmentID;
                                break;
                            }
                    }

                }
            }
        }
        

        SaveInventory();
    }

    public void SellButtonPressed()
    {
        EntityEquipment targetEntity = equipStatsEntity.entityEquipment;
        ActivateConfirmationMenu(CONFIRMATION_TYPE.SELL, targetEntity);
    }

    public void UpgradeButtonPressed()
    {
        EntityEquipment targetEntity = equipStatsEntity.entityEquipment;
        ActivateConfirmationMenu(CONFIRMATION_TYPE.UPGRADE, targetEntity);
    }

    public void ItemSelected(EntityEquipmentUI selectedUI)
    {
        if (selectedUI != null && selectedUI.entityEquipment != null && selectedUI.entityEquipment.equipmentID >= 0)
        {
            equipStatsUI.gameObject.SetActive(true);

            // Store the ID
            equipStatsEntity = selectedUI;

            // Set the icon
            {
                Sprite icon = null;
                switch (selectedUI.entityEquipment.equipmentType)
                {
                    case EntityEquipment.EQUIPMENT_TYPE.WING:
                        {
                            icon = wingIcon;
                            break;
                        }
                    case EntityEquipment.EQUIPMENT_TYPE.LIGHT_WEAPON:
                        {
                            icon = lightWeaponIcon;
                            break;
                        }
                    case EntityEquipment.EQUIPMENT_TYPE.HEAVY_WEAPON:
                        {
                            icon = heavyWeaponIcon;
                            break;
                        }
                }
                equipStatsIcon.sprite = icon;
            }

            // Set the border
            {
                equipStatsBorder.color = equipmentColor.GetColorOfRarity(selectedUI.entityEquipment.equipmentRarity);
            }

            // Set the level
            equipStatsLevel.text = "Level " + selectedUI.entityEquipment.level;

            // Set the main stat
            equipStatsMainStat.text = EquipmentStatsToString(selectedUI.entityEquipment.mainStat.statType, selectedUI.entityEquipment.mainStat.value);

            // Set the sub stats
            equipStatsSubStats.text = "";
            foreach (STAT statEntity in selectedUI.entityEquipment.subStats)
            {
                equipStatsSubStats.text += EquipmentStatsToString(statEntity.statType, statEntity.value);
            }

            // CODE HERE to check max level
            if (true)
            {
                // If not max level yet
                // CODE HERE to set upgrade cost
                equipStatsUpgradeButtonText.text = $"Upgrade ({1000} SP)";
                equipStatsUpgradeButtonText.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                // If max level
                equipStatsUpgradeButtonText.transform.parent.gameObject.SetActive(false);
            }

            // CODE HERE to set sell cost
            equipStatsSellButtonText.text = $"Sell for {1000} SP";

            if (CheckIfEquipmentIsEquipped(selectedUI.entityEquipment))
            {
                // If equipped
                equipStatsEquipButtonText.text = "Unequip";
            }
            else
            {
                // If not equipped
                equipStatsEquipButtonText.text = "Equip";
            }

            
        }
        else equipStatsUI.gameObject.SetActive(false);

    }

    bool CheckIfEquipmentIsEquipped(EntityEquipment entityEquipment)
    {
        foreach (PlaneEquipmentEntity planeEquipmentEntity in DataManager.instance.loadedPlayerPlaneEquipment)
        {
            switch (entityEquipment.equipmentType)
            {
                case EntityEquipment.EQUIPMENT_TYPE.WING:
                    return entityEquipment.equipmentID == planeEquipmentEntity.wingID;
                case EntityEquipment.EQUIPMENT_TYPE.LIGHT_WEAPON:
                    return entityEquipment.equipmentID == planeEquipmentEntity.lightID;
                case EntityEquipment.EQUIPMENT_TYPE.HEAVY_WEAPON:
                    return entityEquipment.equipmentID == planeEquipmentEntity.heavyID;
            }
        }
        return false;
    }

    public void ConfirmMenu(bool isYes)
    {
        if (isYes)
        {
            if (confirmationMenuType == CONFIRMATION_TYPE.SELL)
            {
                // CODE HERE to insert selling cost
                DataManager.instance.AddCommonCurrency(1000);

                List<EntityEquipment> newList = new List<EntityEquipment>(DataManager.instance.loadedPlayerEquipment);
                foreach (EntityEquipment entityEquipment in newList)
                {
                    if (entityEquipment.equipmentID == confirmationMenuEquipment.equipmentID)
                    {
                        newList.Remove(entityEquipment);
                        break;
                    }
                }    
                
                DataManager.instance.loadedPlayerEquipment = newList.ToArray();
                Debug.Log("Size:" + DataManager.instance.loadedPlayerEquipment.Length);
                // Unequip if equipped
                {
                    foreach (PlaneEquipmentEntity planeEquipmentEntity in DataManager.instance.loadedPlayerPlaneEquipment)
                    {
                        switch (confirmationMenuEquipment.equipmentType)
                        {
                            case EntityEquipment.EQUIPMENT_TYPE.WING:
                                if (confirmationMenuEquipment.equipmentID == planeEquipmentEntity.wingID)
                                    planeEquipmentEntity.wingID = -1;
                                break;
                            case EntityEquipment.EQUIPMENT_TYPE.LIGHT_WEAPON:
                                if (confirmationMenuEquipment.equipmentID == planeEquipmentEntity.lightID)
                                    planeEquipmentEntity.wingID = -1;
                                break;
                            case EntityEquipment.EQUIPMENT_TYPE.HEAVY_WEAPON:
                                if (confirmationMenuEquipment.equipmentID == planeEquipmentEntity.heavyID)
                                    planeEquipmentEntity.wingID = -1;
                                break;
                        }
                    }
                }
                SaveInventory();
            }
            else if (confirmationMenuType == CONFIRMATION_TYPE.UPGRADE)
            {
                // CODE HERE to handle upgrading of equipment
            }
        }

        confirmationMenu.gameObject.SetActive(false);
    }

    void ActivateConfirmationMenu(CONFIRMATION_TYPE confirmationType, EntityEquipment equipment)
    {
        confirmationMenuType = confirmationType;
        confirmationMenuEquipment = equipment;

        switch (confirmationMenuType)
        {
            case CONFIRMATION_TYPE.UPGRADE:
                {
                    // CODE HERE to insert upgrade costs
                    confirmationMenuText.text = $"Are you sure you want to upgrade for {1000} SP?";
                    break;
                }
            case CONFIRMATION_TYPE.SELL:
                {
                    // CODE HERE to insert sell sell costs
                    confirmationMenuText.text = $"Are you sure you want to sell for {1000} SP?";
                    break;
                }
        }

        confirmationMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// Helper function to convert Equipment Stats To String for UI display
    /// </summary>
    /// <returns>String that was converted from the stat provided.</returns>
    string EquipmentStatsToString(STAT.STAT_TYPE statType, int value)
    {
        string finalString = "";
        switch (statType)
        {
            case STAT.STAT_TYPE.DMG_BOOST:
                finalString = "DAMAGE BOOST: ";
                break;
            case STAT.STAT_TYPE.DMG_REDUCTION:
                finalString = "DAMAGE BLOCK: ";
                break;
            case STAT.STAT_TYPE.FLIGHT_SPEED:
                finalString = "SPEED BOOST: ";
                break;
            case STAT.STAT_TYPE.LOWER_FUEL_CONSUMPTION:
                finalString = "FUEL REDUCED: ";
                break;        
        }
        finalString += value.ToString() + "%";
        return finalString;
    }


    #endregion

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


    void UpdateModel()
    {
        EntityTypes entityType = objectDictionary.Keys.ToArray()[rotationNumber];
        foreach (Transform displayObject in objectContainer)
        {
            displayObject.gameObject.SetActive(false);
        }

        EntityObject entityObject = entityList.GetEntityObject(entityType);
        modelTitle.text = entityObject.entityName;
        modelDescription.text = entityObject.entityDescription;
        objectDictionary[entityType].gameObject.SetActive(true);
    }


    // Animation has completed.
    void OnAnimDone()
    {
        inventoryPhase = INVENTORY_PHASE.SELECTION;
        SwitchToSelection();
    }

    void SwitchToSelection()
    {
        inventoryPhase = INVENTORY_PHASE.SELECTION;
        modelSelectionUI.SetActive(true);
        inventoryUI.SetActive(false);
    }

    void SwitchToSelected()
    {
        inventoryPhase = INVENTORY_PHASE.SELECTED;
        inventoryUI.SetActive(true);
        equipStatsUI.gameObject.SetActive(false);
        inventoryFrame.gameObject.SetActive(false);
        modelSelectionUI.SetActive(false);
    }
    
}
