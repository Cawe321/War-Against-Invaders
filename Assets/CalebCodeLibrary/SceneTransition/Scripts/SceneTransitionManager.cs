/*
 * IMPORTANT!
 * Singleton
 * DontDestroyOnLoad  
*/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Undestructible object that has functions that you may call before you LoadScene() another scene.
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    #region SINGLETON_VARIABLES
    static public SceneTransitionManager instance { get { return _instance; } private set { _instance = value; } }

    static private SceneTransitionManager _instance;
    #endregion

    [Header("References")]
    [Tooltip("{REQUIRED] The menu that will appear when transitioning.")]
    [SerializeField] Canvas splashScreen;
    [Tooltip("[OPTIONAL] The text that will show the percentage of loading progress.")]
    [SerializeField] TextMeshProUGUI textMesh;
    [Tooltip("{OPTIONAL] The progress bar that will show the percentage of loading progress. Will modify X scale on transform.")]
    [SerializeField] Transform progressBar;

    [SerializeField] List<InterfaceAnimManager> allAnims;


    [Space(0.5f)]
    [Header("Fade Anim Settings")]
    [Tooltip("Time to fade in (seconds).")]
    public float fadeInDuration = 1f;
    [Tooltip("Time to fade out (seconds).")]
    public float fadeOutDuration = 1f;
    
    /// <summary>
    /// The type of splash screen entrance.
    /// </summary>
    public enum ENTRANCE_TYPE
    {
        FADE_IN,
    }

    /// <summary>
    /// The type of splash screen exit.
    /// </summary>
    public enum EXIT_TYPE
    {
        FADE_OUT,
    }

    /// <summary>
    /// Value to check from other scripts whether the splashScreen is active. 
    /// </summary>
    [HideInInspector]
    public bool splashActive {  get 
                                { 
                                    return _splashActive; 
                                } 
                                private set 
                                { 
                                    _splashActive = value;  
                                    splashScreen.gameObject.SetActive(value);
                                }  
                             }
    private bool _splashActive;

    // Internal variables
    Coroutine currCoroutine;
    CanvasGroup canvasGroup;

    #region INITIALIZATION
    // On Awake()
    private void Awake()
    {
        #region SINGLETON_HANDLING
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
            _instance = this;
        #endregion

        DontDestroyOnLoad(this);
        splashActive = false;
    }

    // Used to check for neccessary components and add if neccessary
    private void Start()
    {
        if (splashScreen != null)
        {
            // Check for CanvasGroup component. If not found, automatically add one
            canvasGroup = splashScreen.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = splashScreen.gameObject.AddComponent<CanvasGroup>();
            }

            // Hardcode settings
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        else
            Debug.LogError("SceneTransitionManager: No splash screen selected.");
        allAnims = new List<InterfaceAnimManager>(canvasGroup.GetComponentsInChildren<InterfaceAnimManager>());
    }
    #endregion

    #region PUBLIC_FUNCTIONS
    public void SwitchScene(string sceneName, ENTRANCE_TYPE entranceType, EXIT_TYPE exitType)
    {
        // if not active, means it's ok to switch scenes
        if (currCoroutine == null)
        {
            currCoroutine = StartCoroutine(StartSwitchingScenes(sceneName, entranceType, exitType));
        }
    }
    #endregion

    #region PRIVATE_HELPER_FUNCTIONS
    IEnumerator StartSwitchingScenes(string sceneName, ENTRANCE_TYPE entranceType, EXIT_TYPE exitType)
    {
        if (textMesh)
            textMesh.text = "0%";
        if (progressBar)
            progressBar.localScale = new Vector3(0f, progressBar.localScale.y, progressBar.localScale.z);

        // Handle entrance of loading screen
        switch (entranceType)
        {
            case ENTRANCE_TYPE.FADE_IN:
                {
                    yield return StartCoroutine(StartFadeIn());
                    break;
                }
        }
        foreach (InterfaceAnimManager anim in allAnims)
            anim.startAppear();

        // Load scene async
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            if (textMesh)
                textMesh.text = $"{(int)(asyncLoad.progress * 100f)}%";
            if (progressBar)
                progressBar.localScale = new Vector3(asyncLoad.progress, progressBar.localScale.y, progressBar.localScale.z);
            yield return new WaitForFixedUpdate();
        }

        if (textMesh)
            textMesh.text = "100%";
        if (progressBar)
            progressBar.localScale = new Vector3(1f, progressBar.localScale.y, progressBar.localScale.z);

        foreach (InterfaceAnimManager anim in allAnims)
            anim.startDisappear();

        // Handle exit of loading screen
        switch (exitType)
        {
            case EXIT_TYPE.FADE_OUT:
                {
                    yield return StartCoroutine(StartFadeOut());
                    break;
                }
        }

        // Switch scenes complete!
        currCoroutine = null;
    }
    #endregion

    #region TRANSITION_FUNCTIONS
    IEnumerator StartFadeIn()
    {
        splashActive = true;
        // Ensure that canvas is invisible first
        canvasGroup.alpha = 0f;

        float speed = 1f / fadeInDuration;
        // Keep reducing alpha every frame until alpha is 0
        while (canvasGroup.alpha < 1f)
        {
            yield return new WaitForFixedUpdate();
            canvasGroup.alpha += speed * Time.fixedDeltaTime;
        }
        canvasGroup.alpha = 1f;
        
    }

    IEnumerator StartFadeOut()
    {
        // Ensure that canvas is visible first
        canvasGroup.alpha = 1f;

        float speed = 1f / fadeOutDuration;
        while (canvasGroup.alpha > 0f)
        {
            yield return new WaitForFixedUpdate();
            canvasGroup.alpha -= speed * Time.fixedDeltaTime;
        }
        canvasGroup.alpha = 0f;
        splashActive = false;
    }
    #endregion
}
