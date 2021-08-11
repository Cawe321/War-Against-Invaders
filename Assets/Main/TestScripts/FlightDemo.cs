using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class FlightDemo : MonoBehaviour
{
    public float flightSpeed = 0;

    public float flightTurnSpeed = 1f;

    public float flightAcceleration = 1f;

    /// <summary>
    /// Minimum speed for takeoff
    /// </summary>
    public float flightMinTakeOffSpeed = 100f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        flightSpeed = 0;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Camera.main.transform.position = transform.position - transform.forward * 20 + transform.up * 10;
        Camera.main.transform.LookAt(transform);

        #region CONTROLS
        // Get direction
        //transform.Rotate(Vector3.right * turnSpeed * Input.GetAxis("Mouse X"));    //Uses the left/right mouse movement to rotate the object.
        if (Input.GetMouseButton(0))
        {
            // Left right
            rb.MoveRotation(Quaternion.AngleAxis(flightTurnSpeed * (MouseManager.instance.mousePosAwayFromCenter.x / Screen.width), transform.up) * rb.rotation);

            // Up down
            rb.MoveRotation(Quaternion.AngleAxis(flightTurnSpeed * (MouseManager.instance.mousePosAwayFromCenter.y / Screen.height), -transform.right) * rb.rotation);


        }
        // Banking
        // Store to variable eulerAngles to avoid calculation
        Vector3 eulerAngles = rb.rotation.eulerAngles;
        rb.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0);


        // Get acceleration
        if (Input.GetKey(KeyCode.W))
        {
            flightSpeed += flightAcceleration * Time.fixedDeltaTime;
        }

        // Get deceleration
        if (Input.GetKey(KeyCode.S))
        {
            flightSpeed -= flightAcceleration * Time.fixedDeltaTime;
        }
        #endregion

        #region FLIGHT_UPDATE

        // Enable/Disable gravity according to the plane speed compared to minimum take off speed
        rb.useGravity = (flightSpeed < flightMinTakeOffSpeed);
          


        rb.MovePosition(rb.position + transform.forward * flightSpeed * Time.fixedDeltaTime);
        //rb.velocity = transform.forward * flightSpeed;

        //Set drag and angular drag according relative to speed
        rb.drag = 0.001f * rb.velocity.magnitude;
        rb.angularDrag = 0.01f * rb.velocity.magnitude;
        #endregion
    }
}
