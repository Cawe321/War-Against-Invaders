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
    public float pointsIncrease;
    [HideInInspector]
    public float ammunitionIncrease;
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

    [HideInInspector]
    public bool initialised = false;

    public bool autoGetWeapons = true;

    /* In-script Values */
    float maxStability;

    virtual protected void Start()
    {
        initialised = false;
        entityHealthList = new List<EntityHealth>(GetComponentsInChildren<EntityHealth>());
        entityWeaponList = new List<EntityWeapon>(GetComponentsInChildren<EntityWeapon>());
        
        Init();
    }

    /// <summary>
    /// Commands all associated weapon types to fire.
    /// </summary>
    public void FireAllWeapons(EntityWeapon.WEAPON_TYPE weaponType)
    {
        foreach (EntityWeapon entityWeapon in entityWeaponList)
        {
            if (entityWeapon.weaponType == weaponType)
                entityWeapon.FireWeapon(this);
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

    private void OnMouseDown()
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

}
