using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The gameobject that the object is attached to has health.
/// </summary>
[RequireComponent(typeof(Collider))]
//[RequireComponent(typeof(Rigidbody))]
public class EntityHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 100;
    [Tooltip("If this object is a core component, when its health reaches 0, the entity this health object belongs to will automatically be destroyed, regardless of all other health objects.")]
    public bool isCoreComponent = false;


    public enum DESTRUCTION_TYPE
    {
        DETACH,
        EXPLODE
    }
    public DESTRUCTION_TYPE destructionType;


    public bool isAlive { get { return currHealth >= 0f; } }

    //[HideInInspector]
    public float currHealth;

    /// <summary>
    /// The entity that this health belongs to.
    /// </summary>
    [HideInInspector]
    public BaseEntity baseEntity;


    bool zeroHealthHandled = false;

    private void Start()
    {
        Init();
        if (destructionType == DESTRUCTION_TYPE.EXPLODE && GetComponent<EntityExplosion>() == null)
            Debug.Log("[EntityHealth] ERROR: DESTRUCTION_TYPE.EXPLODE has been selected but cannot find any EntityExplosion components.");
    }

    public void Init()
    {
        currHealth = maxHealth;
        zeroHealthHandled = false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Do nothing currently. All damaging objects colliding with this Entity will be handled in the colliding objects' scripts.
    }

    public void HandleZeroHealth(Vector3 hitStrength)
    {
        if (zeroHealthHandled)
            return;

        zeroHealthHandled = true;
        switch (destructionType)
        {
            case DESTRUCTION_TYPE.DETACH:
                {
                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb == null)
                        rb = gameObject.AddComponent<Rigidbody>();

                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.useGravity = true;
                    rb.AddForce(hitStrength * 0.5f);
                    break;
                }
            case DESTRUCTION_TYPE.EXPLODE:
                {
                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb == null)
                        rb = gameObject.AddComponent<Rigidbody>();

                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.useGravity = true;
                    rb.AddForce(hitStrength * 0.5f);

                    EntityExplosion entityExplosion = GetComponent<EntityExplosion>();
                    if (entityExplosion != null)
                    {
                        entityExplosion.Ignite(transform.position);
                    }
                    break;
                }
        }
    }

    #region UTILITY_FUNCTIONS
    public void TakeDamage(float damage, Vector3 hitStrength)
    {
        UpdateHealth(currHealth - (damage * (1f - baseEntity.dmgReduction))); // currently a function to handle multiplayer in the future
        if (currHealth <= 0f)
        {
            currHealth = 0f;
            HandleZeroHealth(hitStrength);
        }
    }

    public void UpdateHealth(float newHealth)
    {
        currHealth = newHealth;
    }
    #endregion


}
