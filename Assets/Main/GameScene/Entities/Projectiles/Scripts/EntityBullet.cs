using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityBullet : EntityProjectile
{
    [SerializeField]
    float outputForce;

    Collider collider;
    Rigidbody rb = null;
    TrailRenderer trailRenderer;

    private Vector3 prevPos;

    [SerializeField]
    private float lifespan = 10f;
    private float currLifespan;

    private void Awake()
    {
        currLifespan = lifespan;
    }

    private void FixedUpdate()
    {
        lifespan -= Time.fixedDeltaTime;
        if (lifespan < 0f)
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            gameObject.SetActive(false);
            Debug.Log("GONE");

        }
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
                        
                        if (!oppositionHealth.immortalObject && oppositionHealth.baseEntity.team != owner.team)
                        {
                            OnHit(oppositionHealth, rb.velocity);
                            Debug.Log("hitted something");
                            return;
                        }
                        else
                        {
                            OnHit(null, Vector3.zero);
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
        lifespan = currLifespan;
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (collider == null)
            collider = GetComponent<Collider>();
        if (trailRenderer == null)
            trailRenderer = GetComponent<TrailRenderer>();

        trailRenderer.Clear();

        rb.angularVelocity = Vector3.zero;

        // A hard false
        owner = parent.owner;
        //transform.forward = parent.transform.forward;
        collider.isTrigger = false;
        Rigidbody ownerRB = parent.owner.GetComponent<Rigidbody>();
        if (ownerRB != null)
            rb.velocity = ownerRB.velocity;
        rb.AddForce(transform.forward * outputForce, ForceMode.Acceleration);
        //rb.AddForce(Vector3.zero, ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision collision)
    {
        // The projectile has hitted itself. Ignore collision.
        if (collision.transform.IsChildOf(owner.transform))
            Physics.IgnoreCollision(collider, collision.collider);
        else
        {

            // Didnt hit its owner
            // This projectile has hitted something.
            EntityHealth oppositionHealth = collision.transform.GetComponent<EntityHealth>();
            if (oppositionHealth != null)
            {

                if (!oppositionHealth.immortalObject && oppositionHealth.baseEntity.team != owner.team)
                {
                    OnHit(oppositionHealth, rb.velocity);
                    return;
                }
                else
                {
                    OnHit(null, Vector3.zero);
                }
            }
            else
            {


                // Projectile must have hitted an object without health. Do accordingly
                EntityProjectile entityProjectile = collision.transform.GetComponent<EntityProjectile>();
                // Since other projectile cannot detect this raycast detected collision, initiate it for them
                if (entityProjectile != null)
                {
                    entityProjectile.OnHit(null, Vector3.zero);
                }

                OnHit(null, Vector3.zero);
            }
        }

    }

    public override void OnHit(EntityHealth targetHealth, Vector3 impulse)
    {
        if (targetHealth != null)
            targetHealth.TakeDamage(owner, finalDamage, impulse * 0.1f);

        gameObject.SetActive(false);

    }
}
