
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Entity script for planes. Requires <see cref="BaseEntity"/>.
/// </summary>
[RequireComponent(typeof(BaseEntity))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
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
    public float flightBankingBalanceSpeed = 0.025f;
    public float flightStability = 0.3f;
    public float flightStabilitySpeed = 2f;
    public float flightEnginePower = 2f;

    /// <summary>
    /// Minimum speed for takeoff
    /// </summary>
    public float flightMinTakeOffSpeed = 100f;

    [Header("UI Settings")]
    public GameObject cmCamera;
    public Transform mirrorCameraPosition;
    public Transform radarCameraPosition;
    public EntityHealth LWing;
    public EntityHealth RWing;

    /* in-script values*/
    Rigidbody rb;

    AudioSource jetEngineAudio;

    float storedFlightSpeed;

    [HideInInspector]
    public BaseEntity baseEntity;

    [HideInInspector]
    public StateMachine stateMachine;

    //[HideInInspector]
    public float flightSpeed = 0f;

    [HideInInspector]
    public bool engineActive;


    JetEngineVFXController[] jetEngineVFXControllers;

    [HideInInspector]
    public Vector3 torque;

    /// <summary>
    /// public values
    /// </summary>
    public bool isLocalPlayerControl { get { return baseEntity.isLocalPlayerControlling; } }
    
    // Start is called before the first frame update
    void Start()
    {
        baseEntity = GetComponent<BaseEntity>();
        rb = GetComponent<Rigidbody>();
        stateMachine = GetComponent<StateMachine>();
        jetEngineAudio = GetComponent<AudioSource>();
        jetEngineAudio.clip = AudioManager.instance.audioFiles._flightEngineSound;
        jetEngineVFXControllers = GetComponentsInChildren<JetEngineVFXController>();
        rb.inertiaTensor = new Vector3(30f, 30f, 30f);
        StartCoroutine(WaitForBaseEntity(baseEntity));
    }

    void Init()
    {
        // We are using BaseEntity's variables
        baseEntity.maxFuel = maxFuel;
        baseEntity.fuelConsumptionRate = fuelConsumptionRate;
        baseEntity.ReloadAll();

        engineActive = false;
        flightSpeed = storedFlightSpeed = 0f;
    }

   
    
    IEnumerator WaitForBaseEntity(BaseEntity entity)
    {
        while (!entity.initialised)
            yield return new WaitForEndOfFrame();

        Init();
    }


    void FixedUpdate()
    {
        if (!baseEntity.initialised)
            return;

        // if plane pepsi
        if (!baseEntity.CheckHealth())
        {
            baseEntity.DisconnectLocalPlayer();
            baseEntity.playerCanControl = false;
            engineActive = false;
            if (GetComponent<DestroyAfterSeconds>() == null)
            {
                DestroyAfterSeconds destroyScript = gameObject.AddComponent<DestroyAfterSeconds>();
                destroyScript.DestroyAfterWaiting(10);
                GiveCoinsOnDestruction();
            }
        }


        //Set drag and angular drag according relative to speed
        rb.drag = Mathf.Clamp(0.1f * flightSpeed, 0f, 1f);
        rb.angularDrag = Mathf.Clamp(0.01f * flightSpeed, 0f, 1f);



        // Lock rotation 
        rb.angularVelocity = Vector3.zero;

        if (engineActive)
        {
            baseEntity.currFuel -= baseEntity.fuelConsumptionRate * Time.fixedDeltaTime;
            if (baseEntity.currFuel <= 0)
            {
                ToggleEngine();
            }

            // Calculate speed percentage
            float speedPercent = (flightSpeed / flightMinTakeOffSpeed) * 100f;
            foreach(JetEngineVFXController jet in jetEngineVFXControllers)
            {
                jet.percentage = speedPercent;
            }
            jetEngineAudio.volume = flightSpeed / flightMaxSpeed;

            if (flightSpeed < flightMinTakeOffSpeed && transform.forward.y > -0.95f)
                rb.AddRelativeTorque(Vector3.right * Mathf.Abs(1f - transform.forward.y) * flightEnginePower);

            rb.AddTorque(torque, ForceMode.Acceleration);

            // Banking
            // Store to variable eulerAngles to avoid calculation
            //Vector3 eulerAngles = rb.rotation.eulerAngles;
            //rb.MoveRotation(Quaternion.AngleAxis(-rb.rotation.eulerAngles.z, transform.forward) * rb.rotation);
            //rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0), 1f));
            //rb.MoveRotation(Quaternion.AngleAxis(-rb.rotation.eulerAngles.z, rb.velocity.normalized));
            //UpdateStability();
            FixBanking();

            // Move Forward

            if (flightSpeed > flightMinTakeOffSpeed)
                rb.AddForce((flightSpeed * transform.forward - rb.velocity) * flightEnginePower);
            else
                rb.AddForce(flightSpeed / flightMinTakeOffSpeed * transform.forward * flightEnginePower);
            //if (flightSpeed > flightMinTakeOffSpeed && rb.velocity.magnitude > flightSpeed)
                //rb.velocity = rb.velocity.normalized * flightSpeed;
            //rb.velocity = (rb.velocity + transform.forward * flightSpeed).normalized * flightSpeed;

            // Simulate no takeoff without reahcing min speed
            if (flightSpeed < flightMinTakeOffSpeed)
            {
                if (rb.velocity.y > 0f)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);                
                }
            }


            //FixBanking();


        }
        else
        {
            foreach (JetEngineVFXController jet in jetEngineVFXControllers)
            {
                jet.percentage = 0f;
                
            }
            jetEngineAudio.volume = 0f;
        }

        
    }

    /// <summary>
    /// Gives coins on destruction of this plane entity
    /// </summary>
    void GiveCoinsOnDestruction()
    {
        // Give gold to opponent team
        if (PlayerManager.instance.playerTeam != baseEntity.team)
        {
            PlayerManager.instance.AddCoins(ResourceReference.instance.currencySettings.planeDestroyedReward, "An enemy plane has been destroyed.");
        }
        else
        {
            EnemyAIBehaviour.instance.AddCoins(ResourceReference.instance.currencySettings.planeDestroyedReward);
        }
    }

    public void StartFlyingInstantly()
    {
        StartCoroutine(StartFlyingInstantlyOnLoad());
    }
    IEnumerator StartFlyingInstantlyOnLoad()
    {
        while (baseEntity == null)
        {
            yield return new WaitForFixedUpdate();
        }

        if (!engineActive)
            ToggleEngine();

        SetFlightSpeed(flightMinTakeOffSpeed);

        rb.useGravity = false;
    }

    public void Accelerate()
    {
        if (!engineActive)
            return;

        flightSpeed += flightAcceleration * Time.deltaTime;
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

        flightSpeed -= flightAcceleration * Time.deltaTime;
        if (flightSpeed < flightMinSpeed)
            flightSpeed = flightMinSpeed;

        // Enable/Disable gravity according to the plane speed compared to minimum take off speed
        if (engineActive)
            rb.useGravity = (flightSpeed < flightMinTakeOffSpeed);
    }

    public void SetFlightSpeed(float targetSpeed)
    {
        flightSpeed = targetSpeed;
        flightSpeed = Mathf.Clamp(flightSpeed, flightMinSpeed, flightMaxSpeed);

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

        /*rb.AddTorque(rb.transform.up * flightTurnSpeed * xPercent, ForceMode.Acceleration);
        rb.AddTorque(rb.transform.forward * flightTurnSpeed * -xPercent, ForceMode.Acceleration);
        rb.AddTorque(-rb.transform.right * flightTurnSpeed * yPercent, ForceMode.Acceleration);*/

        torque = rb.transform.up * flightTurnSpeed * xPercent + rb.transform.forward * flightTurnSpeed * -xPercent + -rb.transform.right * flightTurnSpeed * yPercent;
        //Debug.Log($"X: {xPercent } | Y: {yPercent}");
    }

    public void RotateToTargetPosition(Vector3 targetWorldPos)
    {
        Vector3 targetDir = (targetWorldPos - transform.position).normalized;
        RotateToTargetDirection(targetDir);
        //Debug.Log(targetDir);
    }
    
    public void RotateToTargetDirection(Vector3 targetDir)
{
        Vector3 localDir = transform.InverseTransformDirection(targetDir);
        if (localDir.x < Mathf.Epsilon && localDir.x > -Mathf.Epsilon && localDir.z < 0f) // target is directly behind
        {
            UpdateRotation(Mathf.Clamp(localDir.z, -1f, 1f), Mathf.Clamp(localDir.y, -1f, 1f));
        }
        else
        {
            if (localDir.z < 0f)
            {
                if (localDir.x < 0f)
                    UpdateRotation(Mathf.Clamp(localDir.x - Mathf.Abs(localDir.z), -1f, 1f), Mathf.Clamp(localDir.y, -1f, 1f));
                else
                    UpdateRotation(Mathf.Clamp(localDir.x + Mathf.Abs(localDir.z), -1f, 1f), Mathf.Clamp(localDir.y, -1f, 1f));
            }
            else
                UpdateRotation(Mathf.Clamp(localDir.x, -1f, 1f), Mathf.Clamp(localDir.y, -1f, 1f));
        }


    }

    public void ToggleEngine()
    {
        Debug.Log("Engine");
        if (engineActive)
        {
            // Turning off engine
            rb.useGravity = true;
            if (flightSpeed > flightMinTakeOffSpeed)
                storedFlightSpeed = flightMinTakeOffSpeed;
            else
                storedFlightSpeed = flightSpeed;
            flightSpeed = 0f;
        }
        else if (baseEntity.currFuel > 0f)
        {
            // Turning on engine
            flightSpeed = storedFlightSpeed;
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
        /*Vector3 currEulerAngles = rb.rotation.eulerAngles;
        Quaternion targetQuat = Quaternion.Euler(0, 0, -currEulerAngles.z);

        Quaternion AngleDifference = Quaternion.FromToRotation(transform.up, targetQuat * transform.up);

        float AngleToCorrect = Quaternion.Angle(targetQuat, transform.rotation);
        Vector3 Perpendicular = Vector3.Cross(transform.up, transform.forward);
        if (Vector3.Dot(targetQuat * Vector3.forward, Perpendicular) < 0)
            AngleToCorrect *= -1;
        Quaternion Correction = Quaternion.AngleAxis(AngleToCorrect, transform.up);

        Vector3 MainRotation = RectifyAngleDifference((AngleDifference).eulerAngles);
        Vector3 CorrectiveRotation = RectifyAngleDifference((Correction).eulerAngles);
        rb.AddTorque(((MainRotation - CorrectiveRotation / 2) - rb.angularVelocity) * flightBankingBalanceSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);*/

        Quaternion targetRotation = Quaternion.AngleAxis(-rb.rotation.eulerAngles.z, transform.forward) * rb.rotation;

        Quaternion diff = Quaternion.Inverse(rb.rotation) * targetRotation;
        Vector3 eulers = RectifyAngleDifference(diff.eulerAngles);
        Vector3 torque = eulers;
        //put the torque back in body space
        torque = rb.rotation * torque;

        //just zero out the current angularVelocity so it doesnt interfere
        //rigidbody.angularVelocity = Vector3.zero;

        rb.AddTorque((torque - rb.angularVelocity) * flightBankingBalanceSpeed, ForceMode.VelocityChange);
    }
}
