using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class that will handle the IntroScene.
/// </summary>
public class IntroSceneManager : MonoBehaviour
{
    [SerializeField]
    IntroCorridorManager corridorManager = null;

    [SerializeField]
    string SceneToSwitchTo;

    [SerializeField]
    float sceneSwitchCooldown = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (corridorManager == null)
        {
            Debug.LogWarning("IntroSceneManager: IntroCorridorManager has not been assigned. GetComponent will be used.");
            corridorManager = GetComponent<IntroCorridorManager>();
        }
    }

    public void EndIntroScene()
    {
        print("pressed");
        corridorManager.EndCorridorAnim();
        StartCoroutine(WaitBeforeChangingScenes());
    }

    #region COROUTINE_FUNCTIONS
    IEnumerator WaitBeforeChangingScenes()
    {
        // Wait for corridor animation to end
        while (corridorManager.animStatus != IntroCorridorManager.ANIM_STATUS.END)
            yield return new WaitForFixedUpdate();

        // Wait for specified cooldown
        yield return new WaitForSeconds(sceneSwitchCooldown);

        // Switch scenes
        SceneTransitionManager.instance.SwitchScene(SceneToSwitchTo, SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
    }
    #endregion
}