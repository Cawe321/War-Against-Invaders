using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencySettings", menuName = "WarAgainstInvaders/Settings/CurrencySettings", order = 2)]
public class CurrencySettings : ScriptableObject
{
    [Header("Game Settings")]
    [SerializeField]
    int _victoryCommonCurrencyReward;
    [SerializeField]
    int _victoryPremiumCurrencyReward;
    [SerializeField]
    int _defeatCommonCurrencyReward;
    [SerializeField]
    int _defeatPremiumCurrencyReward;

    [Header("In-game Settings")]
    [SerializeField]
    int _planeDestroyedReward;
    [SerializeField]
    int _turretDestroyedReward;

    #region GAME_SETTINGS
    public int victoryCommonCurrencyReward { get { return _victoryCommonCurrencyReward; } }
    public int victoryPremiumCurrencyReward { get { return _victoryPremiumCurrencyReward; } }
    public int defeatCommonCurrencyReward { get { return _defeatCommonCurrencyReward; } }
    public int defeatPremiumCurrencyReward { get { return _defeatPremiumCurrencyReward; } }
    #endregion

    #region IN_GAME_SETTINGS
    public int planeDestroyedReward { get { return _planeDestroyedReward; } }
    public int turretDestroyedReward { get { return _turretDestroyedReward; } }
    #endregion

}
