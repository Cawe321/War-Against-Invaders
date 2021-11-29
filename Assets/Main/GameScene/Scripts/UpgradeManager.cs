using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : SingletonObject<UpgradeManager>
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public Dictionary<EntityTypes, int> GetEntitiesOfTeamInSpawnWave(TEAM_TYPE team)
    {
        if (team == TEAM_TYPE.DEFENDERS)
            return GameplayManager.instance.defenderSpawnWave;
        else if (team == TEAM_TYPE.INVADERS)
            return GameplayManager.instance.invaderSpawnWave;
        else return null;
    }

    #region PLANE_UPGRADES
    /// <summary>
    /// Retrieves the cost of an entity.
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public int GetPlanePurchaseCost(EntityTypes entityType)
    {
        if (entityType == EntityTypes.Mako || entityType == EntityTypes.StealthWing)
            return ResourceReference.instance.currencySettings.scoutPurchaseCost;
        else if (entityType == EntityTypes.Whitebeard || entityType == EntityTypes.X_Wing)
            return ResourceReference.instance.currencySettings.balancedPurchaseCost;
        else if (entityType == EntityTypes.F16 || entityType == EntityTypes.Deathrow)
            return ResourceReference.instance.currencySettings.assaultPurchaseCost;
        else
        {
            Debug.LogError("UpgradeManager: GetPurchaseCost() was called but cost has not been set.");
            return -1;
        }
    }

    /// <summary>
    /// Checks whether the player has enough money to buy.
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public bool CheckPlanePurchaseCost(EntityTypes entityType)
    {
        int cost = GetPlanePurchaseCost(entityType);
        if (cost < 0)
            return false;
        else
            return (PlayerManager.instance.coins >= cost);
        
    }

    /// <summary>
    /// Make an attempt to purchase an entity
    /// </summary>
    /// <param name="entityType"></param>
    public void PurchasePlaneEntity(EntityTypes entityType)
    {
        if (CheckPlanePurchaseCost(entityType))
        {
            // Can purchase
            PlayerManager.instance.RemoveCoins(GetPlanePurchaseCost(entityType));
            GameplayManager.instance.AddToSpawnWave(ResourceReference.instance.entityList.GetEntityTeam(entityType), entityType);

            // CODE HERE to announce to players a purchase has been made.
            NotificationManager.instance.AddToNotification("Your purchase has been made!", "You now have " + PlayerManager.instance.coins + " coins");
        }
    }
    #endregion

    #region TURRET_REPAIRS


    public int GetTurretDamageRepairCost(TurretEntity turret, float damagedHealth)
    {
        EntityHealth entityHealth = turret.GetComponent<EntityHealth>();
        return (int)Math.Ceiling((damagedHealth / (float)entityHealth.maxHealth) * (float)GetTurretFullRepairCost(turret));
    }

    /// <summary>
    /// Repairs turret by a percentage of the destroyed health. 0.7 = 70%
    /// </summary>
    /// <param name="turret">Turret entity to repair</param>
    /// <param name="repairPercentage">The percentage of the destroyed health it should repair.</param>
    public void RepairTurret(TurretEntity turret, float repairPercentage, float damagedHealth)
    {
        int cost = (int)Math.Ceiling(repairPercentage * GetTurretDamageRepairCost(turret, damagedHealth));
        if (PlayerManager.instance.coins >= cost)
            PlayerManager.instance.RemoveCoins(cost);

        turret.GetComponent<EntityHealth>().AddHealth(repairPercentage * damagedHealth);
    }
    int GetTurretFullRepairCost(TurretEntity turret)
    {
        if (turret.baseEntity.entityType == EntityTypes.LaserGunTurret || turret.baseEntity.entityType == EntityTypes.MachineGunTurret)
            return ResourceReference.instance.currencySettings.lightTurretRepairCost;
        else
            return ResourceReference.instance.currencySettings.heavyTurretRepairCost;
    }


    #endregion

    public void RepairEntity(EntityHealth entity, int cost, float repairedHealth)
    {
        if (PlayerManager.instance.coins >= cost)
        {
            PlayerManager.instance.RemoveCoins(cost);
            entity.AddHealth(repairedHealth);
        }
    }
}
