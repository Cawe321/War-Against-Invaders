using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCameraMovement : MonoBehaviour
{

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
    }
}
