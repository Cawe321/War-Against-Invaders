using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamCamera : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 200f;
    [SerializeField]
    float rotationSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Transform Movement
        if (Input.GetButton("Forward"))
        {
            transform.position += transform.forward * Time.deltaTime * moveSpeed;
        }
        if (Input.GetButton("Backward"))
        {
            transform.position -= transform.forward * Time.deltaTime * moveSpeed;
        }
        if (Input.GetButton("Leftward"))
        {
            transform.position -= transform.right * Time.deltaTime * moveSpeed;
        }
        if (Input.GetButton("Rightward"))
        {
            transform.position += transform.right * Time.deltaTime * moveSpeed;
        }
        if (Input.GetButton("Upward"))
        {
            transform.position += Vector3.up * Time.deltaTime * moveSpeed;
        }
        if (Input.GetButton("Downward"))
        {
            transform.position -= Vector3.up * Time.deltaTime * moveSpeed;
        }
        
        // Transform Rotation
        if (Input.GetMouseButton(2))
        {
            // Center the mouse
            MouseManager.instance.mouseLockState = CursorLockMode.Locked;
            float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * rotationSpeed;
            float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * rotationSpeed;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }
        else
        {
            MouseManager.instance.mouseLockState = CursorLockMode.None;
        }
    }
}
