using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField]
    Transform transformToFollow;

    void FixedUpdate()
    {
        transform.position = transformToFollow.position;
        transform.rotation = transformToFollow.rotation;
    }
}
