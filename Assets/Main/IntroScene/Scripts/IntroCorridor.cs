using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script that identifies a corridor for IntroScene.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class IntroCorridor : MonoBehaviour
{

    [HideInInspector]
    public IntroCorridorManager corridorManager;

    Collider triggerCollider;

    [Tooltip("The empty gameobject that is positioned at the start of the corridor")]
    public Transform startPosition;

    [Tooltip("The empty gameobject that is positioned at the end of the corridor")]
    public Transform endPosition;

    [HideInInspector]
    public Rigidbody rb;
    
    Camera mainCamera;

    float length;

    private void Awake()
    {
        mainCamera = Camera.main;
        triggerCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        if (triggerCollider == null)
            Debug.Log("IntroCorridor: Corridor has no box collider to trigger next animation!");
        else
            triggerCollider.isTrigger = false;

        length = endPosition.position.z - startPosition.position.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        Camera camera = other.gameObject.GetComponent<Camera>();
        if (camera && mainCamera == camera)
        {
            corridorManager.PlayNextLoop();
        }
    }

    private void Update()
    {
        if (mainCamera.transform.position.z - transform.position.z > length)
        {
            gameObject.SetActive(false);
        }
    }
}
