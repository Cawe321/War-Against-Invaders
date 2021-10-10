using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityBullet : EntityProjectile
{
    [SerializeField]
    float outputForce;

    Collider collider;
    Rigidbody rb = null;
    public override void ActivateProjectile(EntityWeapon parent)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (collider == null)
            collider = GetComponent<Collider>();

        // A hard false
        collider.isTrigger = false;
        rb.transform.forward = parent.transform.forward;
        rb.velocity = parent.owner.GetComponent<Rigidbody>().velocity;
        rb.AddRelativeForce(Vector3.forward * outputForce, ForceMode.Acceleration);
        //rb.AddForce(Vector3.zero, ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision collision)
    {
        // The projectile has hitted itself. Ignore collision.
        if (collision.transform.IsChildOf(owner.transform))
            Physics.IgnoreCollision(collider, collision.collider);
        else
        {
            // This projectile has hitted something.
            EntityHealth oppositionHealth = collision.gameObject.GetComponent<EntityHealth>();
            if (oppositionHealth != null)
            {
                if (oppositionHealth.baseEntity.team != owner.team)
                {
                    OnHit(oppositionHealth);
                    return;
                }
            }
            else
            {
                // Projectile must have hitted an object without health. Do accordingly
                EntityProjectile entityProjectile = collision.gameObject.GetComponent<EntityProjectile>();
                if (entityProjectile != null)
                    entityProjectile.OnHit(null);
                
                OnHit(null);
            }
        }

    }

    public override void OnHit(EntityHealth targetHealth)
    {
        if (targetHealth != null)
            targetHealth.TakeDamage(finalDamage);

        gameObject.SetActive(false);
    }
}
