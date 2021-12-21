using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryAnimManager : MonoBehaviour
{
    public UnityEvent finishAnim;
    
    public enum INVENTORY_PHASE
    {
        OPENING_DOOR,
        MOVEMENT,
        VIEWING,
    };
    [HideInInspector]
    public INVENTORY_PHASE inventoryPhase;

    Camera mainCamera;

    [Header("OPENING_DOOR Animation")]
    public Transform leftDoor;
    public Transform rightDoor;
    public float doorDuration;
    public float doorDistance;
    float currDoorDistance;


    [Header("MOVEMENT Animation")]
    public Transform cameraDestination;
    public float movementDuration;
    Vector3 cameraDirection;


    // Start is called before the first frame update
    void Start()
    {
        // Cache the camera
        mainCamera = Camera.main;
        inventoryPhase = INVENTORY_PHASE.OPENING_DOOR;
        cameraDirection = cameraDestination.position - mainCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (inventoryPhase)
        {
            case INVENTORY_PHASE.OPENING_DOOR:
                {
                    currDoorDistance += doorDistance * (Time.deltaTime / doorDuration);
                    leftDoor.transform.localPosition = new Vector3(-currDoorDistance, leftDoor.transform.localPosition.y, leftDoor.transform.localPosition.z);
                    rightDoor.transform.localPosition = new Vector3(currDoorDistance, rightDoor.transform.localPosition.y, rightDoor.transform.localPosition.z);

                    if (currDoorDistance > doorDistance)
                    {
                        inventoryPhase = INVENTORY_PHASE.MOVEMENT;
                    }

                    break;
                }
            case INVENTORY_PHASE.MOVEMENT:
                {
                    mainCamera.transform.localPosition += cameraDirection * (Time.deltaTime / movementDuration);
                    if ((mainCamera.transform.position - cameraDestination.transform.position).sqrMagnitude < 1f)
                    {
                        mainCamera.transform.position = cameraDestination.transform.position;
                        AnimationDone();
                    }
                    break;
                }
        }
    }


    void AnimationDone()
    {
        inventoryPhase = INVENTORY_PHASE.VIEWING;
        finishAnim.Invoke();
    }
}
