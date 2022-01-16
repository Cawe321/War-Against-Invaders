using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// The gameobject that the object is attached to has health.
/// </summary>
[RequireComponent(typeof(Collider))]
//[RequireComponent(typeof(Rigidbody))]
public class EntityHealth : MonoBehaviour
{
    [Header("Settings")]
    public bool immortalObject = false;
    public float maxHealth = 100;
    [Tooltip("If this object is a core component, when its health reaches 0, the entity this health object belongs to will automatically be destroyed, regardless of all other health objects.")]
    public bool isCoreComponent = false;
    [Tooltip("Even if this Entity isCoreComponent, stability score still required as it still will account for the total stability.")]
    public float stabilityScore = 1f;

    public enum DESTRUCTION_TYPE
    {
        DETACH,
        EXPLODE
    }
    public DESTRUCTION_TYPE destructionType;

    [Header("References")]
    public MeshDestroy meshDestroy;


    /// <summary>
    /// CHecks if health > 0
    /// </summary>
    public bool isAlive { get { return currHealth > 0f; } }

    //[HideInInspector]
    public float currHealth;

    /// <summary>
    /// The entity that this health belongs to.
    /// </summary>
    [HideInInspector]
    public BaseEntity baseEntity;

    Collider collider;

    bool zeroHealthHandled = false;

    PhotonView photonView;
  
    private void Start()
    {
        Init();
        photonView = GetComponent<PhotonView>();
        collider = GetComponent<Collider>();
        if (destructionType == DESTRUCTION_TYPE.EXPLODE && GetComponent<EntityExplosion>() == null)
            Debug.Log("[EntityHealth] ERROR: For" + transform.name +", DESTRUCTION_TYPE.EXPLODE has been selected but cannot find any EntityExplosion components.");
    }

    public void Init()
    {
        currHealth = maxHealth;
        zeroHealthHandled = false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Do nothing currently. All damaging objects colliding with this Entity will be handled in the colliding objects' scripts.

        // This is for spaceship components to ignore each other
        if (transform.IsChildOf(GameplayManager.instance.GetSpaceshipEntity().transform) && collision.transform.IsChildOf(GameplayManager.instance.GetSpaceshipEntity().transform))
        {
            Physics.IgnoreCollision(collider, collision.collider);
        }
        if (!photonView.IsMine)
            return;
        // This is to check collision of other entities
        EntityHealth opposition = collision.collider.GetComponent<EntityHealth>();
        if (opposition != null && !immortalObject) // Check if the other entity has health
        {
            if (opposition.transform.IsChildOf(baseEntity.transform)) // Check if the collided entityhealth is connected to the owner of this entityhealth
                Physics.IgnoreCollision(collision.collider, collider); // Set both entity's collider to ignore each other
            else if (baseEntity.GetComponent<PlaneEntity>() != null && collision.impulse.sqrMagnitude > 100f * 100f) // only deal dmg to ownself if entity is a plane
                TakeDamage(opposition.baseEntity, maxHealth, collision.impulse, true); // Deal dmg to ownself
        }
    }

    public void HandleZeroHealth(Vector3 hitStrength)
    {
        if (zeroHealthHandled)
            return;
        Debug.Log("Handling Zero Health: " + hitStrength);
        zeroHealthHandled = true;
        // Disable collision
        GetComponent<Collider>().isTrigger = true;

        switch (destructionType)
        {
            case DESTRUCTION_TYPE.DETACH:
                {
                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        rb = gameObject.AddComponent<Rigidbody>();
                        rb.mass = 500f;
                    }

                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.useGravity = true;
                    rb.AddForce(hitStrength * (1f/rb.mass), ForceMode.Impulse);
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
                    rb.AddForce(hitStrength * rb.mass, ForceMode.Impulse);

                    EntityExplosion entityExplosion = GetComponent<EntityExplosion>();
                    if (entityExplosion != null)
                    {
                        Explode(true);
                        photonView.RpcSecure("Explode", RpcTarget.Others, false, false);
                    }

                    MeshDestroy meshDestroy = GetComponent<MeshDestroy>();
                    if (meshDestroy != null)
                    {
                        meshDestroy.DestroyMesh();
                    }

                    break;
                }
        }
    }

    [PunRPC]
    void Explode(bool isClientMine)
    {
        EntityExplosion entityExplosion = GetComponent<EntityExplosion>();
        entityExplosion.owner = baseEntity;
        entityExplosion.isClientMine = isClientMine;
        entityExplosion.Ignite(transform.position);
    }


    public void AddHealth(float addHealth)
    {
        float tempHealth = currHealth + addHealth;
        if (tempHealth > maxHealth)
            tempHealth = maxHealth;
        baseEntity.photonView.RpcSecure("UpdateHealth", RpcTarget.All, false, tempHealth, Vector3.zero);
    }

    #region UTILITY_FUNCTIONS
    /// <summary>
    /// Function that makes EntityHealth take damage.
    /// </summary>
    /// <param name="dealer">(BaseEntity) Dealer of the damage</param>
    /// <param name="damage">(float) Base Damage</param>
    /// <param name="hitStrength">(Vector3) Impulse of hit</param>
    /// <param name="ignoreDmgReduction">(bool) Whether to ignore dmg reduction</param>
    public void TakeDamage(BaseEntity dealer, float damage, Vector3 hitStrength, bool ignoreDmgReduction = false)
    {
        if (immortalObject)
            return;

        if (ignoreDmgReduction)
            photonView.RpcSecure("UpdateHealth", RpcTarget.All, false, currHealth - damage, hitStrength);
        else
            photonView.RpcSecure("UpdateHealth", RpcTarget.All, false, currHealth - (damage * (1f - baseEntity.dmgReduction)), hitStrength); 
    }

    [PunRPC]
    public void UpdateHealth(float newHealth, Vector3 hitStrength)
    {
        currHealth = newHealth;
        if (currHealth <= 0f)
        {
            currHealth = 0f;
            HandleZeroHealth(hitStrength);
        }
    }
    #endregion


}
