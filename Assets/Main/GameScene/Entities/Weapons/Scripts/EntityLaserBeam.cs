using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLaserBeam : EntityWeapon
{
    [Header("References")]
    [SerializeField]
    LaserVFXHandler laserVFX;

    [SerializeField]
    [Tooltip("The collider of the laser beam. Collider Extender should also be attached to the collider")]
    ColliderExtender laserCollider;

    [Header("Settings")]
    [Tooltip("The strength of the laser beam. Will push any EntityHealth gameobjects if their health is destroyed")]
    public float pushForce;

    /*In-script Values*/
    float finalDamage;

    void Start()
    {
        base.Start();
        if (laserVFX == null)
            Debug.LogError("EntityLaserBeam: EntityLaserBeam has no laser VFX!");
        if (laserCollider == null)
            Debug.LogError("EntityLaserBeam: EntityLaserBeam has no collider!");
    }

    void FixedUpdate()
    {
        foreach (Collider collider in laserCollider.triggeredObjects)
        {
            // Try to find the EntityHealth component
            EntityHealth entityHealth = collider.GetComponent<EntityHealth>();
            if (entityHealth != null && entityHealth.baseEntity.team != owner.team)
            {
                entityHealth.TakeDamage(owner, finalDamage, laserCollider.transform.forward * pushForce);
            }
            else
            {
                EntityProjectile entityProjectile = collider.GetComponent<EntityProjectile>();
                if (entityProjectile != null)
                {
                    // If laser hitted a projectile, simulate the projectile hitting something. (Missiles will explode, bullets will despawn)
                    entityProjectile.OnHit(null, Vector3.zero);
                }
            }
            
        }
    }

    public void DisableLaser()
    {
        laserVFX.TriggerLaser(false);
    }

    public override void FireWeapon(BaseEntity parent)
    {
        laserVFX.TriggerLaser(true);
        finalDamage = defaultDamage * (1 + parent.dmgIncrease);
        owner = parent;
    }

    protected override GameObject GetAvailableObject()
    {
        return laserVFX.gameObject;
    }
}
