using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryAnimManager : MonoBehaviour
{
    public UnityEvent finishAnim;
    
    public enum ANIM_PHASE
    {
        OPENING_DOOR,
        MOVEMENT,
        VIEWING,
    };
    [HideInInspector]
    public ANIM_PHASE animPhase;

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
        animPhase = ANIM_PHASE.OPENING_DOOR;
        cameraDirection = cameraDestination.position - mainCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (animPhase != ANIM_PHASE.VIEWING) // If anim is playing
        {
            if (Input.GetMouseButtonDown(0)) // If left mouse button clicked, skip anim
                SkipAnim();
        }
        else return;
            
        switch (animPhase)
        {
            case ANIM_PHASE.OPENING_DOOR:
                {
                    currDoorDistance += doorDistance * (Time.deltaTime / doorDuration);
                    leftDoor.transform.localPosition = new Vector3(-currDoorDistance, leftDoor.transform.localPosition.y, leftDoor.transform.localPosition.z);
                    rightDoor.transform.localPosition = new Vector3(currDoorDistance, rightDoor.transform.localPosition.y, rightDoor.transform.localPosition.z);

                    if (currDoorDistance > doorDistance)
                    {
                        animPhase = ANIM_PHASE.MOVEMENT;
                    }

                    break;
                }
            case ANIM_PHASE.MOVEMENT:
                {
                    mainCamera.transform.localPosition += cameraDirection * (Time.deltaTime / movementDuration);
                    if ((mainCamera.transform.position - cameraDestination.transform.position).sqrMagnitude < 1f)
                    {
                        SkipAnim();
                    }
                    break;
                }
        }
    }


    void AnimationDone()
    {
        animPhase = ANIM_PHASE.VIEWING;
        finishAnim.Invoke();
    }

    void SkipAnim()
    {
        mainCamera.transform.position = cameraDestination.transform.position;
        AnimationDone();
    }
}
