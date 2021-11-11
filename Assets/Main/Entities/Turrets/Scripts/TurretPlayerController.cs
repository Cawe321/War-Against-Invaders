using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player script that will help control the turret.
/// </summary>
[RequireComponent(typeof(TurretEntity))]
public class TurretPlayerController : MonoBehaviour
{
    protected TurretEntity turretEntity;

    protected bool wasPlayerControlled;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        turretEntity = GetComponent<TurretEntity>();
        wasPlayerControlled = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (turretEntity.isLocalPlayerControl)
        {
            
            if (!wasPlayerControlled)
            {
                // player just gain control
                EnabledPlayerControl();
            }


            // Temp camera script
            Camera.main.transform.position = turretEntity.xTransform.transform.position - turretEntity.xTransform.transform.forward * 20 + transform.up * 10;
            Camera.main.transform.LookAt(turretEntity.xTransform);

            if (Input.GetButton("PrimaryFire"))
            {
                turretEntity.FireAllWeapons(EntityWeapon.WEAPON_TYPE.PRIMARY);
            }
            else
            {
                
            }

            if (Input.GetButtonDown("LeaveControl"))
            {
                turretEntity.baseEntity.DisconnectLocalPlayer();
            }

            // Only rotate when LMB is pressed down
            if (Input.GetMouseButton(0))
            {
                turretEntity.UpdateRotation(-MouseManager.instance.mousePosChanges.y, MouseManager.instance.mousePosChanges.x);
                //planeEntity.UpdateLeftRightRotation(MouseManager.instance.mousePosAwayFromCenter.x / Screen.width);
                //planeEntity.UpdateUpDownRotation(MouseManager.instance.mousePosAwayFromCenter.y / Screen.height);
            }

        }

        if (wasPlayerControlled && !turretEntity.isLocalPlayerControl)
        {
            // player just lost control
            DisabledPlayerControl();
        }
        wasPlayerControlled = turretEntity.isLocalPlayerControl;
    }

    virtual protected void EnabledPlayerControl()
    {
        MouseManager.instance.mouseLockState = CursorLockMode.Locked;
    }

    virtual protected void DisabledPlayerControl()
    {
        MouseManager.instance.mouseLockState = CursorLockMode.None;
    }
}
