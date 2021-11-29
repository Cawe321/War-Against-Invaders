using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencySettings", menuName = "WarAgainstInvaders/Settings/CurrencySettings", order = 2)]
public class CurrencySettings : ScriptableObject
{
    [Header("***Game Settings***")]
    [SerializeField]
    int _victoryCommonCurrencyReward;
    [SerializeField]
    int _victoryPremiumCurrencyReward;
    [SerializeField]
    int _defeatCommonCurrencyReward;
    [SerializeField]
    int _defeatPremiumCurrencyReward;

    [Header("***In-game Settings***")]
    [Header("Entity Destruction Rewards")]
    [SerializeField]
    int _planeDestroyedReward;
    [SerializeField]
    int _turretDestroyedReward;
    [Header("Plane Purchase Cost")]
    [SerializeField]
    int _scoutPurchaseCost;
    [SerializeField]
    int _balancedPurchaseCost;
    [SerializeField]
    int _assaultPurchaseCost;
    [Header("Turret Full Repair Costs")]
    [SerializeField]
    int _lightTurretRepairCost;
    [SerializeField]
    int _heavyTurretRepairCost;
    [Header("Other Full Repair Costs")]
    [SerializeField]
    int _warehouseRepairCost;
    [SerializeField]
    int _thrusterRepairCost;
    [SerializeField]
    int _fuelTankRepairCost;


    [Header("Care Package")]
    [SerializeField]
    int _carePackageAmount;
    [SerializeField]
    int _carePackageCooldown;

    

    #region GAME_SETTINGS
    public int victoryCommonCurrencyReward { get { return _victoryCommonCurrencyReward; } }
    public int victoryPremiumCurrencyReward { get { return _victoryPremiumCurrencyReward; } }
    public int defeatCommonCurrencyReward { get { return _defeatCommonCurrencyReward; } }
    public int defeatPremiumCurrencyReward { get { return _defeatPremiumCurrencyReward; } }
    #endregion

    #region IN_GAME_SETTINGS
    public int planeDestroyedReward { get { return _planeDestroyedReward; } }
    public int turretDestroyedReward { get { return _turretDestroyedReward; } }
    public int scoutPurchaseCost { get { return _scoutPurchaseCost; } }
    public int balancedPurchaseCost { get { return _balancedPurchaseCost; } }
    public int assaultPurchaseCost { get { return _assaultPurchaseCost; } }
    public int lightTurretRepairCost { get { return _lightTurretRepairCost; } }
    public int heavyTurretRepairCost { get { return _heavyTurretRepairCost; } }

    public int warehouseRepairCost { get { return _warehouseRepairCost; } }
    public int thrusterRepairCost { get { return _thrusterRepairCost; } }
    public int fuelTankRepairCost { get { return _fuelTankRepairCost; } }

    public int carePackageAmount { get { return _carePackageAmount; } }
    public int carePackageCooldown { get { return _carePackageCooldown; } }
    #endregion

}
