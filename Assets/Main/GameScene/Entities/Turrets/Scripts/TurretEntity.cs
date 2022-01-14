using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

/// <summary>
/// Entity script for planes. Requires <see cref="BaseEntity"/>.
/// </summary>
[RequireComponent(typeof(BaseEntity))]
public class TurretEntity : MonoBehaviour
{
    [Header("References")]
    public GameObject cmCamera;
    public Transform xTransform = null;
    public Transform yTransform = null;

    [Header("Turret Settings")]
    [SerializeField]
    float turnSpeed = 5f;
    [SerializeField]
    [Range(1f, 80f)]
    float maxHeightAngle = 80f;


    /*In-script Values*/
    [HideInInspector]
    public BaseEntity baseEntity;


    TurretGunShootingAgent turretAgent;
    public bool isLocalPlayerControl { get { return baseEntity.isLocalPlayerControlling; } }

    // Start is called before the first frame update
    void Start()
    {
        baseEntity = GetComponent<BaseEntity>();
        if (xTransform == null)
            Debug.LogError("TurretEntity: xTransform is null!");
        if (yTransform == null)
            Debug.LogError("TurretEntity: yTransform is null!");
        turretAgent = GetComponent<TurretGunShootingAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!baseEntity.isAnyPlayerControlling)
        {
            if (!PhotonNetwork.IsMasterClient)  // Since no player is controlling, and this client is not the master client, don't let this client update
                return;
        }
        else // A player is controlling
        {
            if (!baseEntity.isLocalPlayerControlling) // Since this is not the client that is controlling the turret, dont let this client update
            {
                return;
            }
        }

        if (!baseEntity.CheckHealth())
        {
            baseEntity.DisconnectLocalPlayer();
            baseEntity.playerCanControl = false;
            if (GetComponent<DestroyAfterSeconds>() == null)
            {
                DestroyAfterSeconds destroyScript = gameObject.AddComponent<DestroyAfterSeconds>();
                destroyScript.DestroyAfterWaiting(10);
                GiveCoinsOnDestruction();
            }
        }
        else
        {
            baseEntity.ReloadAllWeapons();
        }
    }
    /// <summary>
    /// Gives coins on destruction of this turret entity
    /// </summary>
    void GiveCoinsOnDestruction()
    {
        // Give gold to opponent team
        if (PlayerManager.instance.playerTeam != baseEntity.team)
        {
            PlayerManager.instance.AddCoins(ResourceReference.instance.currencySettings.turretDestroyedReward, "An enemy turret has been destroyed.");
        }
        else
        {
            EnemyAIBehaviour.instance.AddCoins(ResourceReference.instance.currencySettings.turretDestroyedReward);
        }
    }


    /// <summary>
    /// Commands BaseEntity to fire all weapons of the respective weapon type.
    /// </summary>
    /// <param name="weaponType">Fire Primary or Secondary weapon</param>
    public void FireAllWeapons(EntityWeapon.WEAPON_TYPE weaponType)
    {
        baseEntity.FireAllWeapons(weaponType);
    }

    /// <summary>
    /// Adds the angle to existing rotation.
    /// </summary>
    /// <param name="xEuler">Euler X angle</param>
    /// <param name="yEuler">Euler Y angle</param>
    public void UpdateRotation(float xEuler, float yEuler)
    {
        float newX = xTransform.localEulerAngles.x + (xEuler * turnSpeed);
        if (newX > 270f)
            newX = newX - 360f;

        if (newX > maxHeightAngle || newX < -maxHeightAngle)
        {
            newX = Mathf.Clamp(newX, -maxHeightAngle, maxHeightAngle);
        }
        else
            if (turretAgent != null && turretAgent.isTraining)
                turretAgent.AddReward(0.1f);

        /*if (turretAgent.isTraining && (Mathf.Abs(xEuler) > 0.5f || Mathf.Abs(yEuler) > 0.5f))
            turretAgent.AddReward(5f);

        if (turretAgent.isTraining) // Suggestions for the AI
{
            Vector3 displacementDirection = turretAgent.aim.InverseTransformDirection((turretAgent.target.position - transform.position).normalized);
            if ((displacementDirection.x < 0 && yEuler < 0) || (displacementDirection.x > 0 && yEuler > 0))
                turretAgent.AddReward(50f);
            else
                turretAgent.AddReward(-50f);

            if ((displacementDirection.y < 0 && xEuler > 0) || (displacementDirection.y > 0 && xEuler < 0))
                turretAgent.AddReward(50f);
            else
                turretAgent.AddReward(-50f);
        }*/

        xTransform.localEulerAngles = new Vector3(newX, xTransform.localEulerAngles.y, xTransform.localEulerAngles.z);

        yTransform.localEulerAngles = new Vector3(yTransform.localEulerAngles.x, yTransform.localEulerAngles.y + (yEuler * turnSpeed), yTransform.localEulerAngles.z);
    }
}
