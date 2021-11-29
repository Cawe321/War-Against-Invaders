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
            controllingEntity = baseEntity;
            baseEntity.playerControlling = playerName;
            Debug.Log("Player has taken over");

            PlaneEntity planeEntity = baseEntity.GetComponent<PlaneEntity>();
            if (planeEntity != null)
            {
                planeEntity.cmCamera.SetActive(true);
                planeUI.EnableUI(planeEntity);
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
        }

        TurretEntity turretEntity = controllingEntity.GetComponent<TurretEntity>();
        if (turretEntity != null)
        {
            turretEntity.cmCamera.SetActive(false);
            freeRoamCamera.transform.position = turretEntity.cmCamera.transform.position;
            freeRoamCamera.transform.rotation = turretEntity.cmCamera.transform.rotation;
        }
        freeRoamCamera.enabled = true;

        controllingEntity = null;
    }
    #endregion
}
