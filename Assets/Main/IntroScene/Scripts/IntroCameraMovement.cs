using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCameraMovement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The speed of the camera once IntroScene has been queued to switch scenes.")]
    float endingAccelerationMultiplier = 1.1f;

    IntroCorridorManager corridorManager;

    // Start is called before the first frame update
    void Start()
    {
        corridorManager = FindObjectOfType<IntroCorridorManager>();    
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (corridorManager.animStatus == IntroCorridorManager.ANIM_STATUS.PLAYING)
        {
            transform.position += Vector3.forward * corridorManager.forwardAnimSpeed * Time.fixedDeltaTime;
        }
        else if (corridorManager.animStatus == IntroCorridorManager.ANIM_STATUS.END)
        {
            corridorManager.forwardAnimSpeed *= endingAccelerationMultiplier;
            transform.position += Vector3.forward * corridorManager.forwardAnimSpeed * Time.fixedDeltaTime;
        }
    }
}
