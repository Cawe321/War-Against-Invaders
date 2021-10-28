using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entity script for planes. Requires <see cref="BaseEntity"/>.
/// </summary>
[RequireComponent(typeof(BaseEntity))]
public class TurretEntity : MonoBehaviour
{
    [Header("References")]
    public Transform xTransform = null;
    public Transform yTransform = null;

    [Header("Turret Settings")]
    [SerializeField]
    float turnSpeed = 5f;
    [SerializeField]
    [Range(1f, 80f)]
    float maxHeightAngle = 80f;


    /*In-script Values*/
    BaseEntity baseEntity;

    public bool isLocalPlayerControl { get { return baseEntity.isLocalPlayerControlling; } }

    // Start is called before the first frame update
    void Start()
    {
        baseEntity = GetComponent<BaseEntity>();
        if (xTransform == null)
            Debug.LogError("TurretEntity: xTransform is null!");
        if (yTransform == null)
            Debug.LogError("TurretEntity: yTransform is null!");
    }

    // Update is called once per frame
    void Update()
    {
        
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
        newX = Mathf.Clamp(newX, -maxHeightAngle, maxHeightAngle);
        xTransform.localEulerAngles = new Vector3(newX, xTransform.localEulerAngles.y, xTransform.localEulerAngles.z);

        yTransform.localEulerAngles = new Vector3(yTransform.localEulerAngles.x, yTransform.localEulerAngles.y + (yEuler * turnSpeed), yTransform.localEulerAngles.z);
    }
}
