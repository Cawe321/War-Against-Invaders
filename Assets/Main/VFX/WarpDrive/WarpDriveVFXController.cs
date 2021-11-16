using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WarpDriveVFXController : MonoBehaviour
{
    [Header("References")]
    public VisualEffect warpDriveVFX;
    [Header("Settings")]
    public float deactivationRate = 0.02f;

    bool warpActive = false;

    public bool isWarpActive { get { return warpActive; } }

    /// <summary>
    /// Returns the active particle count
    /// </summary>
    /// <returns>(int)The number of active particles.</returns>
    public int GetParticleCount()
    {
        return warpDriveVFX.aliveParticleCount;
    }

    /// <summary>
    /// Activates/Deactivates WarpVFX.
    /// </summary>
    /// <param name="toActivate">Whether to activate the warp drive</param>
    public void ActivateWarpDrive(bool toActivate)
    {
        if (toActivate)
        {
            warpActive = true;
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
        warpActive = false;
    }
}
