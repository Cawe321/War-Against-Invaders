using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchSummaryManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    TextMeshProUGUI title;
    [SerializeField]
    TextMeshProUGUI message;
    [SerializeField]
    CanvasGroup canvasGroup;

    public void ActivateUI(bool win)
    {
        if (win)
        {
            title.text = "VICTORY!";
            message.text = "You earnt:\n+ " + ResourceReference.instance.currencySettings.victoryCommonCurrencyReward + " Spare Parts\n+ " + ResourceReference.instance.currencySettings.victoryPremiumCurrencyReward + " Techno Cubes";
        }
        else
        {
            title.text = "DEFEAT...";
            message.text = "You earnt:\n+ " + ResourceReference.instance.currencySettings.defeatCommonCurrencyReward + " Spare Parts\n+ " + ResourceReference.instance.currencySettings.defeatPremiumCurrencyReward + " Techno Cubes";
        }
        canvasGroup.alpha = 0f;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.alpha = 1f;
    }

    public void ReturnToScene()
    {
        // CODE HERE to switch between different main menus
        if (SceneTransitionManager.instance != null)
        {
            if (DataManager.instance.lastTeam == TEAM_TYPE.DEFENDERS)
                SceneTransitionManager.instance.SwitchScene("MainMenu_Defenders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
            else
                SceneTransitionManager.instance.SwitchScene("MainMenu_Invaders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
        }
        else
            SceneManager.LoadScene("MainMenu_Defenders");
    }
}
