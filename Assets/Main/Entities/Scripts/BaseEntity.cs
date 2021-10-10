using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The base class of every entity.
/// </summary>
public class BaseEntity : MonoBehaviour
{
    [Header("Settings/Configuration")]
    public bool playerCanControl;
    
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

    virtual protected void Start()
    {
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
    /// Checks all <see cref="EntityHealth"/> in this entity.
    /// </summary>
    /// <returns>Returns true if this entity is deemed to be alive. Returns false if it should be destroyed/dead.</returns>
    public bool CheckHealth()
    {
        bool isAlive = false;
        foreach (EntityHealth entityHealth in entityHealthList)
        {
            if (entityHealth.isAlive)
            {
                isAlive = true;
            }
            else if (entityHealth.isCoreComponent) // is dead and the health entity is a core component
            {
                isAlive = false;
                break;
            }
        }
        return isAlive;
    }

    void Init()
    {
        currFuel = maxFuel;
        foreach (EntityWeapon weapon in entityWeaponList)
            weapon.owner = this;
        foreach (EntityHealth health in entityHealthList)
            health.baseEntity = this;
    }

    private void OnMouseDown()
    {
        TakeControl();
    }

    bool CheckControlAccess()
    {
        return playerCanControl && playerControlling == "";
    }

    void TakeControl()
    {
        if (CheckControlAccess())
        {
            playerControlling = PlayerManager.instance.playerName;
            Debug.Log("Player has taken over");
        }
    }
}
