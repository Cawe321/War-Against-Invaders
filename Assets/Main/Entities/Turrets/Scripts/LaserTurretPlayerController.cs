using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserTurretPlayerController : TurretPlayerController
{
    List<EntityLaserBeam> entityLaserBeams;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        entityLaserBeams = new List<EntityLaserBeam>(GetComponentsInChildren<EntityLaserBeam>());
    }

    // Update is called once per frame
    protected override void Update()
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
                foreach (EntityLaserBeam entityLaserBeam in entityLaserBeams)
                {
                    entityLaserBeam.DisableLaser();
                }
            }

            // Only rotate when LMB is pressed down
            if (Input.GetMouseButton(0))
            {
                turretEntity.UpdateRotation(-MouseManager.instance.mousePosChanges.y, MouseManager.instance.mousePosChanges.x);
                //planeEntity.UpdateLeftRightRotation(MouseManager.instance.mousePosAwayFromCenter.x / Screen.width);
                //planeEntity.UpdateUpDownRotation(MouseManager.instance.mousePosAwayFromCenter.y / Screen.height);
            }

        }
        else
        {
            if (wasPlayerControlled)
            {
                // player just lost control
                DisabledPlayerControl();
            }
        }
    }
}
