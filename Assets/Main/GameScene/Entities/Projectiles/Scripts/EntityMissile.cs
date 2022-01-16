using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMissile : EntityProjectile
{
    [Header("References")]
    [SerializeField]
    JetEngineVFXController jetEngineVFXController;
    [SerializeField]
    EntityExplosion entityExplosion;

    [Header("Settings")]
    [SerializeField]
    float propulsionAccelerationForce = 10f;

    [SerializeField]
    float durationOfPropulsion = 10f;

    [SerializeField]
    [Tooltip("Time delay before missile starts pushing forward. (In seconds)")]
    float propulsionDelay = 1f;

    Coroutine lastCO;

    bool propulsionActive = false;
    float propulsionRemainingDuration;

    Collider collider;
    Rigidbody rb = null;

    [HideInInspector]
    public PhotonView photonView;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (jetEngineVFXController != null)
            jetEngineVFXController.percentage = 0f;
    }

    public override void ActivateProjectile(EntityWeapon parent)
    {
        parent.weaponAudio.Play();
        owner = parent.owner;

        propulsionActive = false;

        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (collider == null)
            collider = GetComponent<Collider>();

        rb.velocity = Vector3.zero;

        // A hard false
        collider.isTrigger = false;

        rb.transform.forward = parent.transform.forward;
        rb.angularVelocity = Vector3.zero;
        Rigidbody parentRB = parent.owner.GetComponent<Rigidbody>();
        if (parentRB != null)
            rb.velocity = parentRB.velocity;
        if (propulsionDelay > 0.000f)
            rb.useGravity = true;

        // CODE HERE no need to call this coroutine if missile doesnt belong to client (Multiplayer Note)
            lastCO = StartCoroutine(PropulsionDelay(propulsionDelay, parent.transform.forward));
        this.gameObject.SetActive(true);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            // The projectile has hitted itself. Ignore collision.
            if (collision.transform.IsChildOf(owner.transform) || collision.transform == owner.transform)
                Physics.IgnoreCollision(collider, collision.collider);
            else
            {
                if (photonView.IsMine)
                    OnHit(null, Vector3.zero);
            }

        }

    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (propulsionActive && rb != null)
            {
                rb.AddRelativeForce(Vector3.forward * propulsionAccelerationForce * Time.fixedDeltaTime, ForceMode.Acceleration);
                propulsionRemainingDuration -= Time.fixedDeltaTime;
                rb.angularVelocity = Vector3.zero;
                if (propulsionRemainingDuration <= 0)
                {
                    propulsionActive = false;
                    rb.useGravity = true;
                    if (jetEngineVFXController.percentage > 1f)
                        photonView.RpcSecure("DeactivateEffects", RpcTarget.All, false);
                }
            }
        }
        
    }

    public override void OnHit(EntityHealth entityHealth, Vector3 impulse)
    {
        entityExplosion.damage = finalDamage;
        entityExplosion.owner = owner;
        Explode(true);
        photonView.RpcSecure("Explode", RpcTarget.Others, false, false);
    }

    [PunRPC]
    void Explode(bool isClientMine)
    {
        entityExplosion.isClientMine = isClientMine;
        entityExplosion.Ignite(transform.position);
        gameObject.SetActive(false);
    }

    IEnumerator PropulsionDelay(float seconds, Vector3 direction)
    {
        propulsionActive = false;
        float countdown = seconds;
        while (countdown > 0)
        {
            countdown -= Time.fixedDeltaTime;
            transform.forward = direction;
            rb.angularVelocity = Vector3.zero;
            yield return new WaitForFixedUpdate();
        }
        transform.forward = direction;
        StartPropulsion();
        yield return null;
    }

    void StartPropulsion()
    {
        rb.useGravity = false;
        rb.angularVelocity = Vector3.zero;
        propulsionActive = true;
        propulsionRemainingDuration = durationOfPropulsion;
        photonView.RpcSecure("ActivateEffects", RpcTarget.All, false);
       
    }

    [PunRPC]
    void ActivateEffects()
    {
        if (jetEngineVFXController != null)
            jetEngineVFXController.percentage = 100f;
    }

    [PunRPC]
    void DeactivateEffects()
    {
        if (jetEngineVFXController != null)
            jetEngineVFXController.percentage = 0f;
    }
}
