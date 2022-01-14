using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerManager : SingletonObject<PlayerManager>
{
    [Header("References")]
    public FreeRoamCamera freeRoamCamera;
    public PlaneUIManager planeUI;

    [Header("Settings")]
    public string playerName = "";
    public TEAM_TYPE playerTeam;

    public bool isFocused = true;

    public bool hasLoaded = false;

    #region BUFF_STATS_STORAGE
    public float maxFlightSpeedIncrease = 0;
    public float flightAccelerationIncrease = 0;
    #endregion
    public override void Awake()
    {
        base.Awake();
        playerTeam = DataManager.instance.chosenGameTeam;
        hasLoaded = false;
        DataManager.instance.inventoryLoaded.AddListener(InventoryLoaded);
        DataManager.instance.LoadPlayerInventory();
    }

    void InventoryLoaded()
    {
        hasLoaded = true;
    }

    #region CURRENCY_MANAGEMENT
    public int coins { get; private set; }

  
    public void AddCoins(int amountToAdd, string reason)
    {
        coins += amountToAdd;

        // INVOKE NOTIFICATION MANAGER
        NotificationManager.instance.AddToNotification(reason, "+ " + amountToAdd + " coins", "You now have " + coins + " coins.");
    }

    public void RemoveCoins(int amountToRemove)
    {
        coins -= amountToRemove;
    }
    #endregion

    #region ENTITY_CONTROL;
    public BaseEntity controllingEntity { get; private set; } = null;

    public bool isControllingEntity { get { return controllingEntity != null; } }

    public bool TakeOverEntity(BaseEntity baseEntity)
    {
        if (baseEntity.CheckControlAccess())
        {
            if (freeRoamCamera.isSpectate)
            {
                freeRoamCamera.StopSpectate();
            }

            controllingEntity = baseEntity;
            PhotonView photonView = baseEntity.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            photonView.RPC("UpdatePlayerControlling", RpcTarget.All, playerName);
            //baseEntity.playerControlling = playerName;
            Debug.Log("Local Player has taken over");

            PlaneEntity planeEntity = baseEntity.GetComponent<PlaneEntity>();
            if (planeEntity != null)
            {
                planeEntity.cmCamera.SetActive(true);
                planeUI.EnableUI(planeEntity);

                foreach (PlaneEquipmentEntity planeEquipmentEntity in DataManager.instance.loadedPlayerPlaneEquipment)
                {
                    if (planeEquipmentEntity.entityType == controllingEntity.entityType)
                    {
                        LoadEquipmentStatsToEntity(controllingEntity, planeEquipmentEntity);
                        break;
                    }
                }
                
            }

            TurretEntity turretEntity = baseEntity.GetComponent<TurretEntity>();
            if (turretEntity != null)
            {
                turretEntity.cmCamera.SetActive(true);
            }
            freeRoamCamera.enabled = false;

            

            return true;
        }

        return false;
    }

    public void DisconnectFromEntity()
    {
        
        controllingEntity.playerControlling = "";

        PlaneEntity planeEntity = controllingEntity.GetComponent<PlaneEntity>();
        if (planeEntity != null)
        {
            planeEntity.cmCamera.SetActive(false);
            planeUI.DisableUI();
            freeRoamCamera.transform.position = planeEntity.cmCamera.transform.position;
            freeRoamCamera.transform.rotation = planeEntity.cmCamera.transform.rotation;
            UnloadEquipmentStatsFromEntity(controllingEntity);
        }

        TurretEntity turretEntity = controllingEntity.GetComponent<TurretEntity>();
        if (turretEntity != null)
        {
            turretEntity.cmCamera.SetActive(false);
            freeRoamCamera.transform.position = turretEntity.cmCamera.transform.position;
            freeRoamCamera.transform.rotation = turretEntity.cmCamera.transform.rotation;
        }
        freeRoamCamera.enabled = true;

        PhotonView photonView = controllingEntity.GetComponent<PhotonView>();
        photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

        controllingEntity = null;
    }

    void LoadEquipmentStatsToEntity(BaseEntity baseEntity, PlaneEquipmentEntity equipmentEntity)
    {
        if (baseEntity == null || equipmentEntity == null)
            return;

        // Find all relevant equipments first
        List<EntityEquipment> targetEntityEquipments = new List<EntityEquipment>(); // The equipment we are looking for
        foreach (EntityEquipment equipment in DataManager.instance.loadedPlayerEquipment)
        {
            if (equipment.equipmentID == equipmentEntity.wingID || equipment.equipmentID == equipmentEntity.lightID || equipment.equipmentID == equipmentEntity.heavyID) // this equipment belongs to this entity
                targetEntityEquipments.Add(equipment);
        }

        // Loop through all equipments and apply stats (NOTE: This loop can be combined with the first "foreach loop", but for debugging purposes, they have been split)
        float flightSpeedIncreasePercent = 0;
        foreach (EntityEquipment entityEquipment in targetEntityEquipments)
        {
            List<STAT> equipmentStats = new List<STAT>(entityEquipment.subStats);
            equipmentStats.Add(entityEquipment.mainStat); // add the main stat to the calculation

            foreach (STAT stat in equipmentStats)
            {
                switch (stat.statType)
                {
                    case STAT.STAT_TYPE.DMG_BOOST:
                        baseEntity.dmgIncrease += stat.value * 0.01f; // * 0.01f is to convert whole number percent to float (eg 100% == 1f)
                        break;
                    case STAT.STAT_TYPE.DMG_REDUCTION:
                        baseEntity.dmgReduction += stat.value * 0.01f; // * 0.01f is to convert whole number percent to float (eg 100% == 1f)
                        break;
                    case STAT.STAT_TYPE.FLIGHT_SPEED:
                        flightSpeedIncreasePercent += stat.value * 0.01f; // * 0.01f is to convert whole number percent to float (eg 100% == 1f)
                        break;
                    case STAT.STAT_TYPE.LOWER_FUEL_CONSUMPTION:
                        baseEntity.fuelReduction += stat.value * 0.01f; // * 0.01f is to convert whole number percent to float (eg 100% == 1f)
                        break;
                }
            }
        }

        // Check & Handle Equipment Buff Data
        if (baseEntity.dmgReduction > 0.75f) // clamp dmg reduction at 75%
            baseEntity.dmgReduction = 0.75f;

        if (baseEntity.fuelReduction > 0.75f) // clamp fuel reduction at 75%
            baseEntity.fuelReduction = 0.75f;

        // Apply flight speed increase to PlaneEntity and store the increased value
        PlaneEntity basePlaneEntity = baseEntity.GetComponent<PlaneEntity>();
        maxFlightSpeedIncrease = basePlaneEntity.flightMaxSpeed * (flightSpeedIncreasePercent);
        flightAccelerationIncrease = basePlaneEntity.flightAcceleration * (flightSpeedIncreasePercent);
        basePlaneEntity.flightMaxSpeed += maxFlightSpeedIncrease;
        basePlaneEntity.flightAcceleration += flightAccelerationIncrease;
    }

    void UnloadEquipmentStatsFromEntity(BaseEntity baseEntity)
    {
        // Reset baseEntity buff values
        baseEntity.dmgIncrease = 0;
        baseEntity.dmgReduction = 0;
        baseEntity.fuelReduction = 0;

        // Disable PlaneEntity speed buffs
        PlaneEntity basePlaneEntity = baseEntity.GetComponent<PlaneEntity>();
        basePlaneEntity.flightMaxSpeed -= maxFlightSpeedIncrease;
        basePlaneEntity.flightAcceleration -= flightAccelerationIncrease;
        maxFlightSpeedIncrease = 0;
        flightAccelerationIncrease = 0;
    }
    #endregion
}
