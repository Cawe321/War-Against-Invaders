using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityBullet : EntityProjectile
{
    [SerializeField]
    float outputForce;

    Collider collider;
    Rigidbody rb = null;

    private Vector3 prevPos;

    private void FixedUpdate()
    {
        if (prevPos != Vector3.zero)
        {
            RaycastHit hit;
            if (Physics.Linecast(prevPos, transform.position)) // if hitted something
            {
                Physics.Raycast(prevPos, transform.position - prevPos, out hit, (transform.position - prevPos).magnitude);
                
                if (!hit.collider.transform.IsChildOf(owner.transform))
                {
                    // Didnt hit its owner
                    // This projectile has hitted something.
                    EntityHealth oppositionHealth = hit.collider.transform.GetComponent<EntityHealth>();
                    if (oppositionHealth != null)
                    {
                        
                        if (oppositionHealth.baseEntity.team != owner.team)
                        {
                            OnHit(oppositionHealth, rb.velocity);
                            return;
                        }
                    }
                    else
                    {
                        

                        // Projectile must have hitted an object without health. Do accordingly
                        EntityProjectile entityProjectile = hit.collider.transform.GetComponent<EntityProjectile>();
                        // Since other projectile cannot detect this raycast detected collision, initiate it for them
                        if (entityProjectile != null)
                        {
                            entityProjectile.OnHit(null, Vector3.zero);
                        }

                        OnHit(null, Vector3.zero);
                    }

                }
            }
    
        }
        prevPos = transform.position;
    }

    public override void ActivateProjectile(EntityWeapon parent)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (collider == null)
            collider = GetComponent<Collider>();

        // A hard false
        owner = parent.owner;
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
                    OnHit(oppositionHealth, collision.impulse);
                    return;
                }
            }
            else
            {
                // Projectile must have hitted an object without health. Do accordingly
                EntityProjectile entityProjectile = collision.gameObject.GetComponent<EntityProjectile>();

                
                OnHit(null, collision.impulse);
            }
        }

    }

    public override void OnHit(EntityHealth targetHealth, Vector3 impulse)
    {
        if (targetHealth != null)
            targetHealth.TakeDamage(finalDamage, impulse);

        gameObject.SetActive(false);
    }
}
