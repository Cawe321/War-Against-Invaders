using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for weapons
/// </summary>
public abstract class EntityWeapon : ObjectPool<EntityWeapon>
{
    public float defaultDamage;

    public int maxAmmunition;
    public int currAmmunition;

    [HideInInspector]
    public BaseEntity owner;

    [SerializeField]
    private float _weaponCooldown;

    public enum WEAPON_TYPE
    {
        PRIMARY,
        SECONDARY,
    }

    public WEAPON_TYPE weaponType;

    public float weaponCooldown { get { return _weaponCooldown; } }

    /// <summary>
    /// Used to keep track of current cooldown
    /// </summary>
    protected float currWeaponCooldown;

    protected void Start()
    {
        base.Start();
    }

    public abstract void FireWeapon(BaseEntity parent);

    public void ReloadAmmo()
    {
        currAmmunition = maxAmmunition;
    }
    
}
