using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamCamera : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 200f;
    [SerializeField]
    float rotationSpeed = 10f;
    [SerializeField]
    float maxHeightAngle = 89f;


    [SerializeField]
    float minHeight = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.gameplayPhase != GameplayManager.GAMEPLAY_PHASE.GAME)
        {
            // Override camera rotation for Intro Event
            transform.LookAt(GameplayManager.instance.GetSpaceshipEntity().transform);
        }
        else
        {
            // No special events that require camera's attention.
            // Transform Rotation
            if (Input.GetMouseButton(1))
            {
                // Center the mouse
                MouseManager.instance.mouseLockState = CursorLockMode.Locked;
                float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * rotationSpeed;
                float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * rotationSpeed;
                if (newRotationY > 180f)
                    newRotationY -= 360f;
                transform.localEulerAngles = new Vector3(Mathf.Clamp(newRotationY, -maxHeightAngle, maxHeightAngle), newRotationX);
            }
            else
            {
                MouseManager.instance.mouseLockState = CursorLockMode.None;
            }
        }

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
        
        if (transform.position.y < minHeight)
            transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);
    }
}
