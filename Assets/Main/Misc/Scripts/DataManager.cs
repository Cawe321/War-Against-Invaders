using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : SingletonObject<DataManager>
{
    #region SERVER
    // Start of Server Values
    public string playerName { get; private set; } = "Nameless";
    public int commonCurrency { get; private set; }
    public int premiumCurrency { get; private set; }
    public TEAM_TYPE lastTeam { get; private set; } // used to keep track of which team the last player selected

    public UnityEvent inventorySaved;

    public UnityEvent inventoryLoaded;

    public UnityEvent currencyLoaded;

    /// <summary>
    /// Access this value only after inventoryLoaded has been invoked!
    /// </summary>
    [HideInInspector] 
    public EntityEquipment[] loadedPlayerEquipment;
    /// <summary>
    /// Access this value only after inventoryLoaded has been invoked!
    /// </summary>
    [HideInInspector]
    public PlaneEquipmentEntity[] loadedPlayerPlaneEquipment;

    public void AddCommonCurrency(int newCurrency)
    {
        var request = new AddUserVirtualCurrencyRequest()
        {
            VirtualCurrency = "SP",
            Amount = newCurrency,
        };
        PlayFabClientAPI.AddUserVirtualCurrency(request, AddCommonCurrencySuccess, OnError);
        
    }

    private void AddCommonCurrencySuccess(ModifyUserVirtualCurrencyResult obj)
    {
        commonCurrency = obj.Balance;
    }

    public void AddPremiumCurrency(int newCurrency)
    {

        var request = new AddUserVirtualCurrencyRequest()
        {
            VirtualCurrency = "TC",
            Amount = newCurrency,
        };
        PlayFabClientAPI.AddUserVirtualCurrency(request, AddPremiumCurrencySuccess, OnError);
    }

    private void AddPremiumCurrencySuccess(ModifyUserVirtualCurrencyResult obj)
    {
        premiumCurrency = obj.Balance;
    }

    public void SetLastTeam(TEAM_TYPE team)
    {
        lastTeam = team;
        SetUserData();
    }

    // End of server values;
    #endregion

    public string userPlayFabID;

    public TEAM_TYPE chosenGameTeam;
    
    public bool isLoggedIn { get { return profileData && userData && currencyData; } }
    bool profileData = false;
    bool userData = false;
    bool currencyData = false;

    public override void Awake()
    {
        base.Awake();
        //LoadPlayerData();

        
    }

    public void LoadPlayerData()
    {
        profileData = userData = currencyData = false;
        var inventoryRequest = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(inventoryRequest, LoadInventorySuccess, OnError);
        var profileRequest = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(profileRequest, LoadProfileSuccess, OnError);

        GetUserData(userPlayFabID);
        LoadPlayerInventory();
        /*EntityEquipment entityEquipment = new EntityEquipment();
        entityEquipment.equipmentID = 0;
        entityEquipment.equipmentRarity = EntityEquipment.EQUIPMENT_RARITY.RARE;
        entityEquipment.equipmentType = EntityEquipment.EQUIPMENT_TYPE.WING;
        entityEquipment.level = 0;
        entityEquipment.mainStat = new STAT { statType = STAT.STAT_TYPE.DMG_BOOST, value = 1 };
        entityEquipment.subStats = new STAT[0];
        entityEquipment.teamType = TEAM_TYPE.DEFENDERS;
        SavePlayerInventory(new EntityEquipment[] { entityEquipment });*/
    }

    public void LoadCurrencyData()
    {
        currencyData = false;
        var inventoryRequest = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(inventoryRequest, LoadInventorySuccess, OnError);
    }

    public void SavePlayerInventory(EntityEquipment[] entityEquipments, PlaneEquipmentEntity[] planeEquipmentEntities)
    {
        // CODE HERE to save player inventory
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>()
            {
                
               {"Equipment", JsonConvert.SerializeObject(entityEquipments) },
               {"PlaneSlots", JsonConvert.SerializeObject(planeEquipmentEntities) }
            },
            Permission = UserDataPermission.Private
        }, OnSuccessStoreList => { inventorySaved.Invoke(); }, OnError => { Debug.LogError("Save Inventory Error: " + OnError.ErrorMessage); });
    }

    public void LoadPlayerInventory()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = userPlayFabID,
            Keys = new List<string>() { "Equipment", "PlaneSlots" },

        }, result =>
        {
            Debug.Log("Got user inventory");
            if (result.Data != null && result.Data.ContainsKey("Equipment"))
            {
                if (result.Data.ContainsKey("Equipment"))
                    loadedPlayerEquipment = JsonConvert.DeserializeObject<EntityEquipment[]>(result.Data["Equipment"].Value);
                else
                    loadedPlayerEquipment = new EntityEquipment[0];

                if (result.Data.ContainsKey("PlaneSlots"))
                    loadedPlayerPlaneEquipment = JsonConvert.DeserializeObject<PlaneEquipmentEntity[]>(result.Data["PlaneSlots"].Value);
                else
                    loadedPlayerPlaneEquipment = new PlaneEquipmentEntity[0];

                inventoryLoaded.Invoke();
            }
            
        }, (error) =>
        {
            Debug.Log("Load Inventory Error: " + error.ErrorMessage);
            Debug.Log(error.GenerateErrorReport());
        });
    }

    void GetUserData(string myPlayFabId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myPlayFabId,
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data != null) // ERROR
            {
                if (result.Data.ContainsKey("LastTeam"))
                {
                    lastTeam = (TEAM_TYPE)System.Enum.Parse(typeof(TEAM_TYPE), result.Data["LastTeam"].Value);
                    chosenGameTeam = lastTeam;
                }
                else
                {
                    lastTeam = TEAM_TYPE.DEFENDERS;
                    chosenGameTeam = lastTeam;
                    SetUserData();
                }
            }
        }, (error) => 
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
        userData = true;
    }

    void SetUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"LastTeam", lastTeam.ToString()},
        }
        },
        result => Debug.Log("Successfully updated user data"),
        error => 
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private void LoadProfileSuccess(GetAccountInfoResult obj)
    {
        playerName = obj.AccountInfo.Username;
        profileData = true;
    }

    private void LoadInventorySuccess(GetUserInventoryResult obj)
    {
        commonCurrency = obj.VirtualCurrency["SP"];
        premiumCurrency = obj.VirtualCurrency["TC"];
        currencyData = true;
        currencyLoaded.Invoke();
    }

    private void OnError(PlayFabError obj)
    {
        Debug.LogError("DataManager: Playfab:" + obj.ErrorMessage);
    }

  

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


   
}


