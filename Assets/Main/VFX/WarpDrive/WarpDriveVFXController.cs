using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WarpDriveVFXController : MonoBehaviour
{
    [Header("References")]
    public VisualEffect warpDriveVFX;
    [Header("Settings")]
    float deactivationRate = 0.02f;

    bool warpActive = false;


    // Start is called before the first frame update
    void Start()
    {
        warpDriveVFX.Stop();
    }

    /// <summary>
    /// Activates/Deactivates WarpVFX.
    /// </summary>
    /// <param name="toActivate">Whether to activate the warp drive</param>
    public void ActivateWarpDrive(bool toActivate)
    {
        warpActive = toActivate;

        if (toActivate)
        {
            warpDriveVFX.SetFloat("WarpAmount", 1f);
            warpDriveVFX.Play();
        }
        else
        {
            StartCoroutine(StopWarpDrive());
        }
    }

    IEnumerator StopWarpDrive()
    {
        float amount = warpDriveVFX.GetFloat("WarpAmount");
        while (amount > Mathf.Epsilon)
        {
            amount -= deactivationRate * Time.deltaTime;
            warpDriveVFX.SetFloat("WarpAmount", amount);
            yield return new WaitForEndOfFrame();
        }
        warpDriveVFX.Stop();
    }
}
