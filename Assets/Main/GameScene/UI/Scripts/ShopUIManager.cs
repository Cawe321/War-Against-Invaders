using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Button statusButton;

    [SerializeField]
    RectTransform statusReportMenu;
    [SerializeField]
    DefenderStatusReport defenderStatusReport;
    [SerializeField]
    InvaderStatusReport invaderStatusReport;

    [SerializeField]
    RectTransform reinforcementMenu;

    // Start is called before the first frame update
    void OnEnable()
    {
        statusButton.Select();
        SwitchToStatusReport();

        if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
            defenderStatusReport.gameObject.SetActive(true);
        else if (PlayerManager.instance.playerTeam == TEAM_TYPE.INVADERS)
            invaderStatusReport.gameObject.SetActive(true);
        else
            Debug.LogError("ShopUIManager: Status Report doesnt support the team the player is currently on!");
    }

    public void SwitchToStatusReport()
    {
        statusReportMenu.gameObject.SetActive(true);
        reinforcementMenu.gameObject.SetActive(false);
        AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._buttonClickSFX);

    }

    public void SwitchToReinforcement()
    {
        reinforcementMenu.gameObject.SetActive(true);
        statusReportMenu.gameObject.SetActive(false);
        AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._buttonClickSFX);
    }
}
