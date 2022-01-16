using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityWeapon;

/// <summary>
/// The base class of every entity.
/// </summary>
[RequireComponent(typeof(Collider))]
public class BaseEntity : MonoBehaviour
{
    [Header("Settings/Configuration")]
    public bool playerCanControl;
    public EntityTypes entityType;

    [Header("Health Settings")]
    [Range(1f, 100f)]
    public float requiredStability = 20f;

    [Header("Base Stats")]
    public TEAM_TYPE team;
    [HideInInspector]
    public float maxFuel = 0f;
    [HideInInspector]
    public float fuelConsumptionRate = 0f;

    /* Buff Values */
    // All buff values are in decimal places. So 10% is 0.1f.
    [HideInInspector]
    public float dmgIncrease;
    [HideInInspector]
    public float dmgReduction;
    [HideInInspector]
    public float flightSpeedIncrease;
    [HideInInspector]
    public float flightAccelerationIncrease;
    /// <summary>
    /// Buff to reduce fuel consumption.
    /// </summary>
    [HideInInspector]
    public float fuelReduction;

    /* public values */
    [HideInInspector]
    public float currFuel;

    //[HideInInspector]
    public string playerControlling = "";


    [HideInInspector]
    public List<EntityHealth> entityHealthList;
    [HideInInspector]
    public List<EntityWeapon> entityWeaponList;

    /// <summary>
    /// Checks if the local player is controlling this entity.
    /// </summary>
    public bool isLocalPlayerControlling  { get { return playerControlling == PlayerManager.instance.playerName; } }
    /// <summary>
    /// Checks if any players are controlling this entity.
    /// </summary>
    public bool isAnyPlayerControlling { get { return playerControlling != ""; } }

    /// <summary>
    /// Returns percentage of fuel. 10f = 10%
    /// </summary>
    public float getFuelPercentage { get { return (currFuel / maxFuel) * 100f; } }

    [PunRPC]
    public void UpdatePlayerControlling(string newPlayerName)
    {
        playerControlling = newPlayerName;
    }

    [HideInInspector]
    public bool initialised = false;

    public bool autoGetWeapons = true;

    /* In-script Values */
    float maxStability;

    [HideInInspector]
    public PhotonView photonView;

    virtual protected void Start()
    {
        initialised = false;
        entityHealthList = new List<EntityHealth>(GetComponentsInChildren<EntityHealth>());
        entityWeaponList = new List<EntityWeapon>(GetComponentsInChildren<EntityWeapon>());
        photonView = GetComponent<PhotonView>();
        Init();
    }

    /// <summary>
    /// Commands all associated weapon types to fire.
    /// </summary>
    [PunRPC]
    public void FireAllWeapons(EntityWeapon.WEAPON_TYPE weaponType, bool isMine)
    {
        foreach (EntityWeapon entityWeapon in entityWeaponList)
        {
            if (entityWeapon.weaponType == weaponType)
            {
                entityWeapon.isMine = isMine;
                entityWeapon.FireWeapon(this);
            }
        }
    }



    /// <summary>
    /// Set all ammunition to max
    /// </summary>
    public void ReloadAllWeapons()
    {
        foreach(EntityWeapon entityWeapon in entityWeaponList)
        {
            // Reload ammo
            entityWeapon.ReloadAmmo();
        }
    }

    /// <summary>
    /// Checks if the entity still has the respective ammo.
    /// </summary>
    /// <param name="weaponType">The type of ammo to check.</param>
    /// <returns>Returns whether the entity still has ammo.</returns>
    public bool HasAmmo(EntityWeapon.WEAPON_TYPE weaponType)
    {
        foreach (EntityWeapon entityWeapon in entityWeaponList)
        {
            if (entityWeapon.weaponType == weaponType && entityWeapon.currAmmunition > 0)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the entity has secondary weapons installed.
    /// </summary>
    /// <returns>True if the entity has secondary weapons.</returns>
    public bool HasSecondaryWeapon()
    {
        foreach (EntityWeapon entityWeapon in entityWeaponList)
{
            if (entityWeapon.weaponType == EntityWeapon.WEAPON_TYPE.SECONDARY)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Reloads both fuel and ammunition. Use <see cref="ReloadAllWeapons"/> to reload weapons only or <see cref="ReloadFuel"/> to reload fuel.
    /// </summary>
    public void ReloadAll()
    {
        ReloadFuel();
        ReloadAllWeapons();
    }

    /// <summary>
    /// Reloads fuel
    /// </summary>
    public void ReloadFuel()
    {
        currFuel = maxFuel;
    }


    public EntityWeapon GetFirstWeaponOfType(WEAPON_TYPE weaponType)
    {
        foreach (EntityWeapon weapon in entityWeaponList)
        {
            if (weapon.weaponType == weaponType)
                return weapon;
        }
        return null;
    }

    /// <summary>
    /// Checks all <see cref="EntityHealth"/> in this entity.
    /// </summary>
    /// <returns>Returns true if this entity is deemed to be alive. Returns false if it should be destroyed/dead.</returns>
    public bool CheckHealth()
    {
        if (!initialised) // always return true since this entity has not intialised.
            return true;

        bool isAlive = false;
        float currStability = 0f;
        foreach (EntityHealth entityHealth in entityHealthList)
        {
            if (entityHealth.isAlive)
            {
                isAlive = true;
                currStability += entityHealth.stabilityScore;
            }
            else if (entityHealth.isCoreComponent) // is dead and the health entity is a core component
            {
                isAlive = false;
                break;
            }
        }

        // Current stability is lower than required stability to survive
        if (isAlive && (currStability / maxStability) * 100f < requiredStability)
        {
            isAlive = false;
        }
        return isAlive;
    }

    /// <summary>
    /// Returns the percentage of the current stability
    /// </summary>
    /// <returns>Percentage of current stability. 0.01f = 1%</returns>
    public float GetCurrentStability()
    {
        float currStability = 0f;
        foreach (EntityHealth entityHealth in entityHealthList)
        {
            if (entityHealth.isAlive)
            {
                currStability += entityHealth.stabilityScore;
            }
        }
        return currStability / maxStability;
    }

    void Init()
    {
        currFuel = maxFuel;
        if (autoGetWeapons)
            foreach (EntityWeapon weapon in entityWeaponList)
                weapon.owner = this;

        maxStability = 0f;
        foreach (EntityHealth health in entityHealthList)
        {
            health.baseEntity = this;
            maxStability += health.stabilityScore;
        }
        initialised = true;


    }

    public void OnMouseDown()
    {
        TakeControl();
        Debug.Log("Pressed");
    }

    public bool CheckControlAccess()
    {
        return playerCanControl && playerControlling == "" && !PlayerManager.instance.isControllingEntity && team == PlayerManager.instance.playerTeam;
    }

    void TakeControl()
    {
        PlayerManager.instance.TakeOverEntity(this);
    }

    public void DisconnectLocalPlayer()
    {
        if (isLocalPlayerControlling)
        {
            PlayerManager.instance.DisconnectFromEntity();
        }
    }

    [PunRPC]
    public void UpdateEntityParent(TEAM_TYPE team)
    {
        switch (team)
        {
            case TEAM_TYPE.DEFENDERS:
                transform.parent = GameplayManager.instance.defenderPlaneContainer.transform;
                break;
            case TEAM_TYPE.INVADERS:
                transform.parent = GameplayManager.instance.invaderPlaneContainer.transform;
                break;

        }
        GameplayManager.instance.infraredManager.AddInfrared(this);
    }


    [PunRPC]
    void LoadEquipmentStatsToEntity(string jsonStringOfStats)
    {
        Debug.Log(jsonStringOfStats);
        STAT[] allStats = JsonUtility.FromJson<STAT[]>(jsonStringOfStats);

        float flightSpeedIncreasePercent = 0;
        foreach (STAT stat in allStats)
        {
            switch (stat.statType)
            {
                case STAT.STAT_TYPE.DMG_BOOST:
                    dmgIncrease += stat.value * 0.01f; // * 0.01f is to convert whole number percent to float (eg 100% == 1f)
                    break;
                case STAT.STAT_TYPE.DMG_REDUCTION:
                    dmgReduction += stat.value * 0.01f; // * 0.01f is to convert whole number percent to float (eg 100% == 1f)
                    break;
                case STAT.STAT_TYPE.FLIGHT_SPEED:
                    flightSpeedIncreasePercent += stat.value * 0.01f; // * 0.01f is to convert whole number percent to float (eg 100% == 1f)
                    break;
                case STAT.STAT_TYPE.LOWER_FUEL_CONSUMPTION:
                   fuelReduction += stat.value * 0.01f; // * 0.01f is to convert whole number percent to float (eg 100% == 1f)
                    break;
            }
        }





        // Check & Handle Equipment Buff Data
        if (dmgReduction > 0.75f) // clamp dmg reduction at 75%
            dmgReduction = 0.75f;

        if (fuelReduction > 0.75f) // clamp fuel reduction at 75%
            fuelReduction = 0.75f;

        // Apply flight speed increase to PlaneEntity and store the increased value
        PlaneEntity basePlaneEntity = GetComponent<PlaneEntity>();
        flightSpeedIncrease = basePlaneEntity.flightMaxSpeed * (flightSpeedIncreasePercent);
        flightAccelerationIncrease = basePlaneEntity.flightAcceleration * (flightSpeedIncreasePercent);
        basePlaneEntity.flightMaxSpeed += flightSpeedIncrease;
        basePlaneEntity.flightAcceleration += flightAccelerationIncrease;
    }

    [PunRPC]
    void UnloadEquipmentStatsFromEntity()
    {
        // Reset baseEntity buff values
        dmgIncrease = 0;
        dmgReduction = 0;
        fuelReduction = 0;

        // Disable PlaneEntity speed buffs
        PlaneEntity basePlaneEntity = GetComponent<PlaneEntity>();
        basePlaneEntity.flightMaxSpeed -= flightSpeedIncrease;
        basePlaneEntity.flightAcceleration -= flightAccelerationIncrease;
        flightSpeedIncrease = 0;
        flightAccelerationIncrease = 0;
    }
}
