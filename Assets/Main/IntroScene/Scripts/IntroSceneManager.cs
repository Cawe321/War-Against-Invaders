using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class that will handle the IntroScene.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class IntroSceneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    IntroCorridorManager corridorManager = null;

    [SerializeField]
    InterfaceAnimManager UIAnimManager = null;

    [Header("Settings")]
    [SerializeField]
    string SceneToSwitchTo;

    [SerializeField]
    float sceneSwitchCooldown = 1f;

    [SerializeField]
    AudioClip zoomSFX;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (corridorManager == null)
        {
            Debug.LogWarning("IntroSceneManager: IntroCorridorManager has not been assigned. GetComponent will be used.");
            corridorManager = GetComponent<IntroCorridorManager>();
        }
        if (UIAnimManager == null)
            Debug.LogError("IntroSceneManager: InterfaceAnimManager not attached.");

        corridorManager.StartCorridorAnim();
        UIAnimManager.startAppear();
        audioSource.clip = UIAnimManager.openSound;
        audioSource.Play();
    }

    public void EndIntroScene()
    {
        print("pressed");
        UIAnimManager.startDisappear();
        //StartCoroutine(WaitBeforeChangingScenes());
        UIAnimManager.OnEndDisappear += DisableCorridorAnim;

        audioSource.clip = UIAnimManager.closeSound;
        audioSource.Play();
    }
    
    void DisableCorridorAnim(InterfaceAnimManager _IAM)
    {
        StartCoroutine(WaitBeforeChangingScenes());
    }

    #region COROUTINE_FUNCTIONS
    IEnumerator WaitBeforeChangingScenes()
    {

        corridorManager.EndCorridorAnim();

        // Wait for corridor animation to end
        while (corridorManager.animStatus != IntroCorridorManager.ANIM_STATUS.END)
            yield return new WaitForFixedUpdate();

        audioSource.clip = zoomSFX;
        audioSource.volume = 1f; // quick fix to low zoom sfx volume
        audioSource.Play();

        // Wait for specified cooldown
        yield return new WaitForSeconds(sceneSwitchCooldown);

        // Switch scenes
        SceneTransitionManager.instance.SwitchScene(SceneToSwitchTo, SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
    }
    #endregion
}