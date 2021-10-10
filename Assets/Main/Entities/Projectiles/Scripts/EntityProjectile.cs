using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all projectile types.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class EntityProjectile : MonoBehaviour
{
    public ProjectileTypes projectileType;

    [HideInInspector]
    public float finalDamage;

    [HideInInspector]
    public BaseEntity owner;

    public abstract void ActivateProjectile(EntityWeapon parent);

    public abstract void OnHit(EntityHealth entityHealth);
}
