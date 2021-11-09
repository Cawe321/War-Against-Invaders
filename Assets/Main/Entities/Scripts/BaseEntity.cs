using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [HideInInspector]
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
    public bool isAnyPlayerControlling { get { return playerControlling == ""; } }

    [HideInInspector]
    public bool initialised = false;

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
    /// Reloads both fuel and ammunition. Use <see cref="ReloadAllWeapons"/> to reload weapons only or <see cref="ReloadFuel"/> to reload fuel.
    /// </summary>
    public void ReloadAll()
    {
        ReloadFuel();
        ReloadAllWeapons();
    }

    public void ReloadFuel()
    {
        currFuel = maxFuel;
    }

    /// <summary>
    /// Checks all <see cref="EntityHealth"/> in this entity.
    /// </summary>
    /// <returns>Returns true if this entity is deemed to be alive. Returns false if it should be destroyed/dead.</returns>
    public bool CheckHealth()
    {
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
    }

    bool CheckControlAccess()
    {
        return playerCanControl && playerControlling == "" && PlayerManager.instance.freeRoam;
    }

    void TakeControl()
    {
        if (CheckControlAccess())
        {
            playerControlling = PlayerManager.instance.playerName;
            Debug.Log("Player has taken over");
        }
    }
    public void DisconnectLocalPlayer()
    {
        if (isLocalPlayerControlling)
        {
            // Disable baseEntity to be controllable by players.
            playerControlling = "";
            PlayerManager.instance.freeRoam = true;
        }
    }

}
