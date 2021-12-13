using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;

public class MainMenuVFXManager : MonoBehaviour
{
    [SerializeField] bool isDefender;
    [Header("References")]
    [SerializeField] Camera mainCamera;
    [SerializeField] WarpDriveVFXController warpDrive;
    [SerializeField] GameObject earth;
    [SerializeField] Volume bloomVolume;
    [SerializeField] Transform mapContainer;
    [Tooltip("Skybox used for VFX")]
    [SerializeField] Material vfxSkybox;
    [Tooltip("Skybox used for default map")]
    [SerializeField] Material defaultSkybox;
    [SerializeField] VolumeProfile vfxVolumeProfile;
    [SerializeField] VolumeProfile defaultVolumeProfile;
    
    [Header("Settings")]
    [SerializeField] float warpDuration = 3f;
    [SerializeField] float warpDeactivationDuration = 0.5f;
    [SerializeField] float enlargeDuration = 5f;
    [SerializeField] float earthScaleSpeed = 10f;
    [SerializeField] float cameraVFXOffset = 100f;

    /*Public References from this Script*/
    [Header("Events")]
    public UnityEvent onVFXEnd;

    /* Script Values*/
    Vector3 originalCameraPos;
    public bool vfxEnded { get { return vfxStatus == VFX_STATUS.ENDED; } }

    /*In-script Values*/
    public enum VFX_STATUS
    {
        WAITING_TO_START,
        WARP_DRIVE,
        DEACTIVATING_WARP,
        APPEAR_MAP,
        BLOOM_DECREASE,
        ENDED,
    }
    [HideInInspector]
    public VFX_STATUS vfxStatus = VFX_STATUS.WAITING_TO_START;

    float cooldownVariable;
    Coroutine vfxCoroutine;


    void Start()
    {
        StartVFX();
    }

    /// <summary>
    /// Starts VFX effects
    /// </summary>
    public void StartVFX()
    {
        if (vfxStatus == VFX_STATUS.WAITING_TO_START)
        {
            cooldownVariable = warpDuration;
            bloomVolume.profile = vfxVolumeProfile;
            RenderSettings.skybox = vfxSkybox;
            warpDrive.ActivateWarpDrive(true);
            warpDrive.deactivationRate = 1f / warpDeactivationDuration;
            vfxStatus = VFX_STATUS.WARP_DRIVE;
            earth.SetActive(false);
            mapContainer.gameObject.SetActive(false);
            originalCameraPos = mainCamera.transform.localPosition;
            mainCamera.transform.localPosition += new Vector3(0f, cameraVFXOffset, cameraVFXOffset);
            vfxCoroutine = StartCoroutine(RunVFX());
        }
    }

    /// <summary>
    /// Force ends vfx
    /// </summary>
    public void ForceEndVFX()
    {
        StopCoroutine(vfxCoroutine);
        earth.SetActive(false);
        warpDrive.ActivateWarpDrive(false);
        warpDrive.gameObject.SetActive(false);
        mapContainer.gameObject.SetActive(true);
        bloomVolume.profile = defaultVolumeProfile;
        RenderSettings.skybox = defaultSkybox;
        mainCamera.transform.localPosition = originalCameraPos;
        vfxStatus = VFX_STATUS.ENDED;
        onVFXEnd.Invoke();
        if (isDefender)
            AudioManager.instance.PlayBGM(AudioManager.instance.audioFiles._defenderMainMenuBGM);
        else
            AudioManager.instance.PlayBGM(AudioManager.instance.audioFiles._invaderMainMenuBGM);
    }

    IEnumerator RunVFX()
    {
        
        while (!vfxEnded)
        {
            print("Running" + vfxStatus);
            switch (vfxStatus)
            {
                case VFX_STATUS.WARP_DRIVE:
                    {
                        cooldownVariable -= Time.fixedDeltaTime;
                        if (cooldownVariable < 0f)
                        {
                            vfxStatus = VFX_STATUS.DEACTIVATING_WARP;
                            cooldownVariable = earth.transform.localScale.x;
                            warpDrive.ActivateWarpDrive(false);
                        }
                        break;
                    }
                case VFX_STATUS.DEACTIVATING_WARP:
                    {
                        if (!warpDrive.isWarpActive)
                        {
                            earth.SetActive(true);
                            cooldownVariable += Time.fixedDeltaTime * earthScaleSpeed * enlargeDuration;
                            earth.transform.localScale = new Vector3(cooldownVariable, cooldownVariable, cooldownVariable);

                            if (warpDrive.GetParticleCount() == 0)
                            {
                                vfxStatus = VFX_STATUS.APPEAR_MAP;
                                mapContainer.gameObject.SetActive(true);
                                bloomVolume.profile = defaultVolumeProfile;
                                cooldownVariable = 0f;
                                if (isDefender)
                                    AudioManager.instance.PlayBGM(AudioManager.instance.audioFiles._defenderMainMenuBGM);
                                else
                                    AudioManager.instance.PlayBGM(AudioManager.instance.audioFiles._invaderMainMenuBGM);

                            }
                           
                        }
                        break;
                    }
                case VFX_STATUS.APPEAR_MAP:
                    {
                        earth.SetActive(false);
                        RenderSettings.skybox = defaultSkybox;
                        cooldownVariable += Time.fixedDeltaTime * (1f / enlargeDuration);
                        mainCamera.transform.localPosition -= new Vector3(0f, cameraVFXOffset * Time.fixedDeltaTime / enlargeDuration, cameraVFXOffset * Time.fixedDeltaTime / enlargeDuration);
                        if (cooldownVariable > 1f)
                        {
                            mainCamera.transform.localPosition = originalCameraPos;
                            cooldownVariable = 1f;
                            vfxStatus = VFX_STATUS.ENDED;
                            onVFXEnd.Invoke(); 
                        }
                      
                        break;
                    }
            }
            yield return new WaitForFixedUpdate(); // FixedUpdate()
        }

    }

}
