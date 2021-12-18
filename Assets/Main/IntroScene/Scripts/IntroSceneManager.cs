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
    RectTransform WAITING_UI;

    [SerializeField]
    RectTransform LOGIN_UI;

    [SerializeField]
    InterfaceAnimManager[] UIAnimManagers;

    [Header("Settings")]
    [SerializeField]
    float sceneSwitchCooldown = 1f;

    [SerializeField]
    AudioClip zoomSFX;

    [SerializeField]
    AudioClip openSound;

    [SerializeField]
    AudioClip closeSound;

    AudioSource audioSource;

    public enum PHASE
    {
        LOGIN,
        WAIT_TO_START,
    }
    public PHASE introPhase;

    // Start is called before the first frame update
    void Start()
    {
        introPhase = PHASE.LOGIN;
        LOGIN_UI.gameObject.SetActive(true);
        WAITING_UI.gameObject.SetActive(false);

        //SettingsManager.LoadSettingsAndApply();
        audioSource = GetComponent<AudioSource>();

        if (corridorManager == null)
        {
            Debug.LogWarning("IntroSceneManager: IntroCorridorManager has not been assigned. GetComponent will be used.");
            corridorManager = GetComponent<IntroCorridorManager>();
        }

        corridorManager.StartCorridorAnim();
    }

    public void LoginSuccessful()
    {
        DataManager.instance.LoadPlayerData();
        StartCoroutine(WaitForLoginSuccessful());
    }

    IEnumerator WaitForLoginSuccessful()
    {
        while (!DataManager.instance.isLoggedIn)
            yield return new WaitForEndOfFrame();

        introPhase = PHASE.WAIT_TO_START;
        LOGIN_UI.gameObject.SetActive(false);
        WAITING_UI.gameObject.SetActive(true);

        foreach (InterfaceAnimManager UIAnimManager in UIAnimManagers)
        {
            UIAnimManager.startAppear();
        }
        audioSource.clip = openSound;
        audioSource.Play();
    }

    public void EndIntroScene()
    {
        print("pressed");
        foreach (InterfaceAnimManager UIAnimManager in UIAnimManagers)
        {
            UIAnimManager.startDisappear();
        }
        //StartCoroutine(WaitBeforeChangingScenes());
        UIAnimManagers[0].OnEndDisappear += DisableCorridorAnim;

        audioSource.clip = closeSound;
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
        if (DataManager.instance.lastTeam == TEAM_TYPE.DEFENDERS)
            SceneTransitionManager.instance.SwitchScene("MainMenu_Defenders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
        else
            SceneTransitionManager.instance.SwitchScene("MainMenu_Invaders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);

    }
    #endregion
}