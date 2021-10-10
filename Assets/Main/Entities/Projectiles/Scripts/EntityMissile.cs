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
    public override void ActivateProjectile(EntityWeapon parent)
    {
        propulsionActive = false;

        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (collider == null)
            collider = GetComponent<Collider>();

        // A hard false
        collider.isTrigger = false;

        rb.transform.forward = parent.transform.forward;
        rb.velocity = parent.owner.GetComponent<Rigidbody>().velocity;
        rb.useGravity = true;

        // No need to call this coroutine if missile doesnt belong to client (Multiplayer Note)
        lastCO = StartCoroutine(PropulsionDelay(propulsionDelay, parent.transform.forward));

        if (jetEngineVFXController != null)
            jetEngineVFXController._percentage = 0f;
    }

    public void OnCollisionEnter(Collision collision)
    {
        // The projectile has hitted itself. Ignore collision.
        if (collision.transform.IsChildOf(owner.transform))
            Physics.IgnoreCollision(collider, collision.collider);
        else
        {
            OnHit(null);
        }

    }

    private void FixedUpdate()
    {
        if (propulsionActive && rb != null)
        {
            rb.AddRelativeForce(Vector3.forward * propulsionAccelerationForce * Time.fixedDeltaTime, ForceMode.Acceleration);
            propulsionRemainingDuration -= Time.fixedDeltaTime;
            if (propulsionRemainingDuration <= 0)
            {
                propulsionActive = false;
                rb.useGravity = false;
            }
        }
    }

    public override void OnHit(EntityHealth entityHealth)
    {
        // Explosion!!!
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
            yield return new WaitForFixedUpdate();
        }
        StartPropulsion();
        yield return null;
    }

    void StartPropulsion()
    {
        rb.useGravity = false;
        rb.angularVelocity = Vector3.zero;
        propulsionActive = true;
        propulsionRemainingDuration = durationOfPropulsion;
        if (jetEngineVFXController != null)
            jetEngineVFXController._percentage = 100f;
    }
}