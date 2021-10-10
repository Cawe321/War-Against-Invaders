using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that handles the laser VFX.
/// </summary>
public class LaserVFXHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The line renderer that acts as the laser.")]
    GameObject laser;

    [Header("Settings")]
    [SerializeField]
    [Tooltip("Whether the laser is enabled. Toggle this to decide whether to enable/disable the laser at start of runtime.")]
    bool laserEnabled = false;

    [Tooltip("Max range of laser")]
    public float maxRange = 20f;
    

    [Tooltip("Time taken to activate laser.")]
    public float activationTime = 1f;

    Coroutine lastCO;

    // Start is called before the first frame update
    void Start()
    {
        if (laser == null)
            Debug.LogError("LaserVFXHandler is used, but no laser gameobject is attached!");

        if (laserEnabled)
            laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, maxRange);
        else
            laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, 0f);

    }

    /*// For testing purposes
    void Update()
    {
        if (Input.GetMouseButton(0))
            TriggerLaser(true);
        else
            TriggerLaser(false);

    }*/

    public void TriggerLaser(bool toActivate)
    {
        if (laserEnabled != toActivate)
        {
            laserEnabled = toActivate;
            float speedOfActivation = maxRange / activationTime;
            if (lastCO != null)
                StopCoroutine(lastCO);
            if (toActivate)
                lastCO = StartCoroutine(ActivateLaser(speedOfActivation));
            else
                lastCO = StartCoroutine(DeactivateLaser(speedOfActivation));
        }
    }

    IEnumerator ActivateLaser(float speed)
    {
        while (laser.transform.localScale.z < maxRange)
        {
            yield return new WaitForFixedUpdate();
            laser.transform.localScale += new Vector3(0, 0, speed * Time.fixedDeltaTime);
        }
        laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, maxRange);
    }

    IEnumerator DeactivateLaser(float speed)
    {
        while (laser.transform.localScale.z > 0)
        {
            yield return new WaitForFixedUpdate();
            laser.transform.localScale -= new Vector3(0, 0, speed * Time.fixedDeltaTime);
        }
        laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, 0f);
    }
}
