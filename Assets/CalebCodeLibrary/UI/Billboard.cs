using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Billboard : MonoBehaviour
{
    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = mainCamera.transform.forward;
    }
}
