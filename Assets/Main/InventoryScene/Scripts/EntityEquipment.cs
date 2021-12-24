using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityEquipment
{
    public int equipmentID = -1;

    public TEAM_TYPE teamType;

    public int level;

    public enum EQUIPMENT_TYPE
    {
        WING = 0,
        LIGHT_WEAPON,
        HEAVY_WEAPON,
    }
    public EQUIPMENT_TYPE equipmentType;

    public enum EQUIPMENT_RARITY
    {
        COMMON = 0,
        RARE,
        LEGENDARY,
    }
    public EQUIPMENT_RARITY equipmentRarity;

    public STAT mainStat;

    public STAT[] subStats;
}

[System.Serializable]
public struct STAT
{
    public enum STAT_TYPE
    {
        DMG_BOOST = 0,
        DMG_REDUCTION,
        FLIGHT_SPEED,
        LOWER_FUEL_CONSUMPTION,
    }
    public STAT_TYPE statType;
    public int value;
}
