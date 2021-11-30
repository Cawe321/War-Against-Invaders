using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Makes object rotate and float
/// </summary>
public class SpaceFloatingObject : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 10f;

    [SerializeField]
    float upDownRange = 100f;

    [SerializeField]
    float upDownSpeed = 0.5f;

    Vector3 originalPosition;

    float offset;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
        offset = Random.Range(0f, 180f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = originalPosition + (Vector3.up * Mathf.Cos((offset + Time.time) * upDownSpeed) * upDownRange);
        transform.Rotate(transform.up, rotationSpeed * Time.deltaTime);
    }
}
