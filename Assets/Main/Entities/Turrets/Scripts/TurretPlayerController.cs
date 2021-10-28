using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player script that will help control the turret.
/// </summary>
[RequireComponent(typeof(TurretEntity))]
public class TurretPlayerController : MonoBehaviour
{
    TurretEntity turretEntity;



    // Start is called before the first frame update
    void Start()
    {
        turretEntity = GetComponent<TurretEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (turretEntity.isLocalPlayerControl)
        {
            // Temp camera script
            Camera.main.transform.position = turretEntity.xTransform.transform.position - turretEntity.xTransform.transform.forward * 20 + transform.up * 10;
            Camera.main.transform.LookAt(turretEntity.xTransform);

            if (Input.GetButton("PrimaryFire"))
            {
                turretEntity.FireAllWeapons(EntityWeapon.WEAPON_TYPE.PRIMARY);
            }

            // Only rotate when LMB is pressed down
            if (Input.GetMouseButton(0))
            {
                turretEntity.UpdateRotation(-MouseManager.instance.mousePosAwayFromCenter.y / Screen.height, MouseManager.instance.mousePosAwayFromCenter.x / Screen.width);
                //planeEntity.UpdateLeftRightRotation(MouseManager.instance.mousePosAwayFromCenter.x / Screen.width);
                //planeEntity.UpdateUpDownRotation(MouseManager.instance.mousePosAwayFromCenter.y / Screen.height);
            }

        }
    }
}
