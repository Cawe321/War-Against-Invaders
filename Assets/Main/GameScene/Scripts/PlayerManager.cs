using ExitGames.Client.Photon;
using Newtonsoft.Json;
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

    public bool takingControl = false;

    #region BUFF_STATS_STORAGE
    public float maxFlightSpeedIncrease = 0;
    public float flightAccelerationIncrease = 0;
    #endregion
    public override void Awake()
    {
        base.Awake();
        playerName = DataManager.instance.playerName;
        playerTeam = DataManager.instance.chosenGameTeam;
        hasLoaded = false;
        DataManager.instance.inventoryLoaded.AddListener(InventoryLoaded);
        DataManager.instance.LoadPlayerInventory();
        takingControl = false;
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
        if (takingControl)
            return false;
        if (baseEntity.CheckControlAccess())
        {
            takingControl = true;
            if (freeRoamCamera.isSpectate)
            {
                freeRoamCamera.StopSpectate();
            }

            controllingEntity = baseEntity;
            PhotonView photonView = baseEntity.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            PhotonView[] childViews = baseEntity.GetComponentsInChildren<PhotonView>();
            foreach (PhotonView child in childViews)
                child.TransferOwnership(PhotonNetwork.LocalPlayer);
            photonView.RPC("UpdatePlayerControlling", RpcTarget.All, playerName);
            //baseEntity.playerControlling = playerName;
            Debug.Log("Local Player has taken over");

            PlaneEntity planeEntity = baseEntity.GetComponent<PlaneEntity>();
            if (planeEntity != null)
            {
                planeEntity.cmCamera.SetActive(true);
                planeUI.EnableUI(planeEntity);


                List<EntityEquipment> targetEntityEquipments = new List<EntityEquipment>(); // The equipment we are looking for
                foreach (PlaneEquipmentEntity planeEquipmentEntity in DataManager.instance.loadedPlayerPlaneEquipment) 
                {
                    if (planeEquipmentEntity.entityType == controllingEntity.entityType)
                    {
                        foreach (EntityEquipment equipment in DataManager.instance.loadedPlayerEquipment)
                        {
                            if (equipment.equipmentID == planeEquipmentEntity.wingID || equipment.equipmentID == planeEquipmentEntity.lightID || equipment.equipmentID == planeEquipmentEntity.heavyID) // this equipment belongs to this entity
                                targetEntityEquipments.Add(equipment);
                        }
                        break; 
                    }
                }

                List<STAT> equipmentStats = new List<STAT>();
                foreach (EntityEquipment entityEquipment in targetEntityEquipments)
                {
                    equipmentStats.Add(entityEquipment.mainStat);
                    equipmentStats.AddRange(entityEquipment.subStats);
                }

                if (equipmentStats.Count > 0)
                {
                    baseEntity.photonView.RpcSecure("LoadEquipmentStatsToEntity", RpcTarget.All, false, JsonConvert.SerializeObject(equipmentStats.ToArray()));
                    Debug.Log("Pre Equipment" + JsonConvert.SerializeObject(equipmentStats.ToArray()));
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
        if (controllingEntity == null)
            return;

        takingControl = false;

        PlaneEntity planeEntity = controllingEntity.GetComponent<PlaneEntity>();
        if (planeEntity != null)
        {
            planeEntity.cmCamera.SetActive(false);
            planeUI.DisableUI();
            freeRoamCamera.transform.position = planeEntity.cmCamera.transform.position;
            freeRoamCamera.transform.rotation = planeEntity.cmCamera.transform.rotation;
            controllingEntity.photonView.RpcSecure("UnloadEquipmentStatsFromEntity", RpcTarget.All, false);
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
        photonView.RPC("UpdatePlayerControlling", RpcTarget.All, "");
        photonView.TransferOwnership(PhotonNetwork.MasterClient);
        PhotonView[] childViews = controllingEntity.GetComponentsInChildren<PhotonView>();
        foreach (PhotonView child in childViews)
            child.TransferOwnership(PhotonNetwork.MasterClient);

        controllingEntity = null;
    }
    #endregion
}
