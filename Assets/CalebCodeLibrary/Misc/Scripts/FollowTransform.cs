using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField]
    Transform transformToFollow;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();    
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            transform.position = transformToFollow.position;
            transform.rotation = transformToFollow.rotation;
        }
        else
        {
            rb.MovePosition(transformToFollow.position);
            rb.MoveRotation(transformToFollow.rotation);
        }
    }
}
