using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using PlayFab.EconomyModels;
using ExitGames.Client.Photon;

public class NetworkManager : SingletonObjectPunCallback<NetworkManager>
{
    // Event Codes
    public const byte StartGameEventCode = 1;
    public const byte UpdateGameStats = 2;
    public const byte UpdateSpawnWave = 3;
    public const byte MakeAnnouncementSpawnWave = 4;
    public const byte GiveCarePackage = 5;

    // Start is called before the first frame update
    void Start()
    {
        Hashtable props = new Hashtable
            {
                {MatchmakingKeyIDs.PLAYER_LOADED_LEVEL, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        PlayerManager.instance.hasLoaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        if (DataManager.instance.chosenGameTeam == TEAM_TYPE.DEFENDERS)
            SceneTransitionManager.instance.SwitchScene("MainMenu_Defenders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
        else
            SceneTransitionManager.instance.SwitchScene("MainMenu_Invaders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
    }

    #region PUN CALLBACKS

    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DemoAsteroids-LobbyScene");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(MatchmakingKeyIDs.PLAYER_LOADED_LEVEL))
        {
            if (CheckAllPlayerLoadedLevel()) // All players have loaded
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(StartGameEventCode, null, raiseEventOptions, SendOptions.SendReliable);
            }
        }

    }

    #endregion

    private bool CheckAllPlayerLoadedLevel()
    {
        if (PhotonNetwork.OfflineMode)
            return true;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(MatchmakingKeyIDs.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }
}
