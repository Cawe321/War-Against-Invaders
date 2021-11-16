using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlaneEntity))]
public class PlanePlayerController : MonoBehaviour
{
    PlaneEntity planeEntity;



    private void Start()
    {
        planeEntity = GetComponent<PlaneEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (planeEntity.isLocalPlayerControl)
        {
            if (Input.GetButtonDown("Engine"))
            {
                planeEntity.ToggleEngine();
            }

            // Temp camera script
            Camera.main.transform.position = transform.position - transform.forward * 20 + transform.up * 10;
            Camera.main.transform.LookAt(transform);

            if (Input.GetButton("Forward"))
            {
                planeEntity.Accelerate();
            }
            if (Input.GetButton("Backward"))
            {
                planeEntity.Decelerate();
            }

            if (Input.GetButton("PrimaryFire"))
            {
                planeEntity.FireAllWeapons(EntityWeapon.WEAPON_TYPE.PRIMARY);
            }

            if (Input.GetButton("SecondaryFire"))
            {
                planeEntity.FireAllWeapons(EntityWeapon.WEAPON_TYPE.SECONDARY);
            }

            if (Input.GetButton("LeaveControl"))
            {
                planeEntity.baseEntity.DisconnectLocalPlayer();
            }

            // Only rotate when LMB is pressed down
            if (Input.GetMouseButton(0))
            {
                planeEntity.UpdateRotation(MouseManager.instance.mousePosAwayFromCenter.x / Screen.width, MouseManager.instance.mousePosAwayFromCenter.y / Screen.height);
                //planeEntity.UpdateLeftRightRotation(MouseManager.instance.mousePosAwayFromCenter.x / Screen.width);
                //planeEntity.UpdateUpDownRotation(MouseManager.instance.mousePosAwayFromCenter.y / Screen.height);
            }
            else
            {
                planeEntity.UpdateRotation(0f, 0f);
            }

        }
        
    }
}
