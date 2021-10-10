using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entity script for planes. Requires <see cref="BaseEntity"/>.
/// </summary>
[RequireComponent(typeof(BaseEntity))]
[RequireComponent(typeof(Rigidbody))]
public class PlaneEntity : MonoBehaviour
{
    [Header("Base Settings")]
    public float maxFuel = 100f;
    public float fuelConsumptionRate = 0.1f;

    [Header("Flight Settings")]
    public float flightTurnSpeed = 1f;
    public float flightAcceleration = 1f;
    public float flightMaxSpeed = 200f;
    public float flightMinSpeed = -20f;
    //public float flightBankingMaxAngle = 60f;
    //public float flightBankingMultiplier = 0.5f;
    public float flightBankingBalanceSpeed = 0.5f;
    public float flightStability = 0.3f;
    public float flightStabilitySpeed = 2f;
    public float flightEnginePower = 2f;

    /// <summary>
    /// Minimum speed for takeoff
    /// </summary>
    public float flightMinTakeOffSpeed = 100f;

    Rigidbody rb;

    BaseEntity baseEntity;

    //[HideInInspector]
    public float flightSpeed = 0f;

    /* in-script values*/
    bool engineActive;

    /// <summary>
    /// public values
    /// </summary>
    public bool isLocalPlayerControl { get { return baseEntity.isLocalPlayerControlling; } }
    
    // Start is called before the first frame update
    void Start()
    {
        baseEntity = GetComponent<BaseEntity>();
        rb = GetComponent<Rigidbody>();
        Init();
    }

    void Init()
    {
        // We are using BaseEntity's variables
        baseEntity.maxFuel = maxFuel;
        baseEntity.fuelConsumptionRate = fuelConsumptionRate;

        engineActive = false;
        flightSpeed = 0;
    }


    void FixedUpdate()
    {
        // if plane pepsi
        if (!baseEntity.CheckHealth())
        {
            if (isLocalPlayerControl)
            {
                // Disable baseEntity to be controllable by players.
                baseEntity.playerCanControl = false;
                baseEntity.playerControlling = "";
                PlayerManager.instance.freeRoam = true;
            }
            engineActive = false;
            if (GetComponent<DestroyAfterSeconds>() == null)
            {
                DestroyAfterSeconds destroyScript = gameObject.AddComponent<DestroyAfterSeconds>();
                destroyScript.DestroyAfterWaiting(10);
            }
        }

        if (engineActive)
        {
            // Banking
            // Store to variable eulerAngles to avoid calculation
            Vector3 eulerAngles = rb.rotation.eulerAngles;
            rb.MoveRotation(Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0));
            //FixBanking();

            // Move Forward
            rb.AddForce(rb.transform.forward * (flightSpeed - rb.velocity.magnitude) * flightEnginePower);
            if (rb.velocity.magnitude > flightSpeed)
                rb.velocity = rb.velocity.normalized * flightSpeed;
            //rb.velocity = (rb.velocity + transform.forward * flightSpeed).normalized * flightSpeed;

            // Lock rotation 
            rb.angularVelocity = Vector3.zero;
            //FixBanking();

            //Set drag and angular drag according relative to speed
            rb.drag = 0.1f * rb.velocity.magnitude;
            rb.angularDrag = 0.01f * rb.velocity.magnitude;
        }

        
    }

    public void Accelerate()
    {
        if (!engineActive)
            return;

        flightSpeed += flightAcceleration * Time.fixedDeltaTime;
        if (flightSpeed > flightMaxSpeed)
            flightSpeed = flightMaxSpeed;

        // Enable/Disable gravity according to the plane speed compared to minimum take off speed
        if (engineActive)
            rb.useGravity = (flightSpeed < flightMinTakeOffSpeed);
    }

    public void Decelerate()
    {
        if (!engineActive)
            return;

        flightSpeed -= flightAcceleration * Time.fixedDeltaTime;
        if (flightSpeed < flightMinSpeed)
            flightSpeed = flightMinSpeed;

        // Enable/Disable gravity according to the plane speed compared to minimum take off speed
        if (engineActive)
            rb.useGravity = (flightSpeed < flightMinTakeOffSpeed);
    }

    /// <summary>
    /// Updates rotation
    /// </summary>
    /// <param name="percent">In decimal places. For eg, 10% should be 0.01.</param>
    public void UpdateRotation(float xPercent, float yPercent)
    {
        if (!engineActive)
            return;

        if (flightSpeed < 1f)
            return;

        //rb.AddRelativeTorque(new Vector3(-yPercent, 0f, -xPercent) * flightTurnSpeed, ForceMode.Acceleration);
        rb.AddTorque(rb.transform.up * flightTurnSpeed * xPercent, ForceMode.Acceleration);
        rb.AddTorque(-rb.transform.right * flightTurnSpeed * yPercent, ForceMode.Acceleration);
        //rb.MoveRotation(Quaternion.AngleAxis(flightTurnSpeed * percent, transform.up) * rb.rotation);

        /*float currBankingAngle = flightBankingMultiplier * -percent * flightBankingMaxAngle;
        currBankingAngle = Mathf.Clamp(currBankingAngle, -flightBankingMaxAngle, flightBankingMaxAngle);
        if (rb.velocity.y < 0)
            rb.AddTorque(rb.transform.forward * (-currBankingAngle), ForceMode.Acceleration);
        else
            rb.AddTorque(rb.transform.forward * (currBankingAngle), ForceMode.Acceleration);*/
    }


    public void ToggleEngine()
    {
        Debug.Log("Engine");
        if (engineActive)
        {
            // Turning off engine
            rb.useGravity = true;
        }
        else
        {
            // Turning on engine
            rb.useGravity = (flightSpeed < flightMinTakeOffSpeed);
        }

        engineActive = !engineActive;

    }

    public void FireAllWeapons(EntityWeapon.WEAPON_TYPE weaponType)
    {
        baseEntity.FireAllWeapons(weaponType);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.IsChildOf(transform))
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
    }

    void UpdateStability()
    {
        Vector3 predictedUp = Quaternion.AngleAxis(
         rb.angularVelocity.magnitude * Mathf.Rad2Deg * flightStability / flightStabilitySpeed,
         rb.angularVelocity
        ) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        rb.AddTorque(torqueVector * flightStabilitySpeed * flightStabilitySpeed);
    }

    private Vector3 RectifyAngleDifference(Vector3 angleDiff)
    {
        if (angleDiff.x > 180) 
            angleDiff.x -= 360;
        if (angleDiff.y > 180) 
            angleDiff.y -= 360;
        if (angleDiff.z > 180) 
            angleDiff.z -= 360;
        return angleDiff;
    }

    void FixBanking()
    {
        Vector3 currEulerAngles = rb.rotation.eulerAngles;
        Quaternion targetQuat = Quaternion.Euler(currEulerAngles.x, currEulerAngles.y, 0);

        Quaternion AngleDifference = Quaternion.FromToRotation(transform.up, targetQuat * Vector3.up);

        float AngleToCorrect = Quaternion.Angle(targetQuat, transform.rotation);
        Vector3 Perpendicular = Vector3.Cross(transform.up, transform.forward);
        if (Vector3.Dot(targetQuat * Vector3.forward, Perpendicular) < 0)
            AngleToCorrect *= -1;
        Quaternion Correction = Quaternion.AngleAxis(AngleToCorrect, transform.up);

        Vector3 MainRotation = RectifyAngleDifference((AngleDifference).eulerAngles);
        Vector3 CorrectiveRotation = RectifyAngleDifference((Correction).eulerAngles);
        rb.AddTorque(((MainRotation - CorrectiveRotation / 2) - rb.angularVelocity) * flightBankingBalanceSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}
