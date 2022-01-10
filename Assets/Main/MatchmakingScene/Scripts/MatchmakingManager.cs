using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    [Header("Misc References")]
    public GameObject ProcessingMenu;

    [Header("Selection Panel")]
    public GameObject SelectionPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomPanel;

    public InputField RoomNameInputField;

    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomPanel;

    [Header("Room List Panel")]
    public GameObject RoomListPanel;

    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;

    public Button StartGameButton;
    public GameObject PlayerListEntryPrefab;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;


    bool openSettings;
    #region UNITY

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        openSettings = false;
    }

    public void Start()
    {
        //if (DataManager.instance != null)
        OnLoginButtonClicked();
    }

    #endregion

    #region PUN CALLBACKS

    public override void OnConnectedToMaster()
    {
        Hashtable teamProperty = new Hashtable { { MatchmakingKeyIDs.PLAYER_TEAM, DataManager.instance.chosenGameTeam } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(teamProperty);

        this.SetActivePanel(SelectionPanel.name);
        ProcessingMenu.SetActive(false);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby()
    {
        // whenever this joins a new lobby, clear any previous room lists
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed: " + returnCode + " " + message);
        SetActivePanel(SelectionPanel.name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed: " + returnCode + " " + message);
        SetActivePanel(SelectionPanel.name);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + Random.Range(1000, 10000);
        Hashtable customRoomProperties = new Hashtable { { TEAM_TYPE.DEFENDERS.ToString(), 0 }, { TEAM_TYPE.INVADERS.ToString(), 0 } };

        RoomOptions options = new RoomOptions { MaxPlayers = 6, CustomRoomProperties = customRoomProperties, CustomRoomPropertiesForLobby = new string[] { TEAM_TYPE.DEFENDERS.ToString(), TEAM_TYPE.INVADERS.ToString() } };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public override void OnJoinedRoom()
    {
        // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
        cachedRoomList.Clear();

        object count = null;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(DataManager.instance.chosenGameTeam.ToString(), out count);
        if (count != null)
        {
            Hashtable newProperty = new Hashtable { { DataManager.instance.chosenGameTeam.ToString(), (int)count + 1 } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProperty);
        }
        else
            Debug.LogError("The room has no count for the chosen team");

        SetActivePanel(InsideRoomPanel.name);

        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerTeam = null;
            TEAM_TYPE playerTeamType = TEAM_TYPE.NONE;
            if (p.CustomProperties.TryGetValue(MatchmakingKeyIDs.PLAYER_TEAM, out playerTeam))
            {
                playerTeamType = (TEAM_TYPE)((int)playerTeam);

            }
            else
            {
                Debug.LogError("Player Team not found");
            }

            GameObject entry = Instantiate(PlayerListEntryPrefab, InsideRoomPanel.transform);
            //entry.transform.SetParent(InsideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<MatchmakingPlayerListEntry>().Initialize(p.ActorNumber, p.NickName, playerTeamType.ToString());

            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(MatchmakingKeyIDs.IS_PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<MatchmakingPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }

            playerListEntries.Add(p.ActorNumber, entry);
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());

        Hashtable props = new Hashtable
            {
                {MatchmakingKeyIDs.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnLeftRoom()
    {
    

        SetActivePanel(SelectionPanel.name);


        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(PlayerListEntryPrefab, InsideRoomPanel.transform);
        //entry.transform.SetParent(InsideRoomPanel.transform);
        entry.transform.localScale = Vector3.one;
        object playerTeam = null;
        TEAM_TYPE playerTeamType = TEAM_TYPE.NONE;
        if (newPlayer.CustomProperties.TryGetValue(MatchmakingKeyIDs.PLAYER_TEAM, out playerTeam))
        {
            playerTeamType = (TEAM_TYPE)((int)playerTeam);
        }
        else
        {
            Debug.LogError("Player Team not found");
        }
        entry.GetComponent<MatchmakingPlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName, (string)playerTeamType.ToString());

        playerListEntries.Add(newPlayer.ActorNumber, entry);

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(MatchmakingKeyIDs.IS_PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<MatchmakingPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion

    #region UI CALLBACKS

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(SelectionPanel.name);
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = RoomNameInputField.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

        //RoomOptions options = new RoomOptions { MaxPlayers = (byte)6,  };
        Hashtable customRoomProperties = new Hashtable { { TEAM_TYPE.DEFENDERS.ToString(), 0 }, { TEAM_TYPE.INVADERS.ToString(), 0 } };

        RoomOptions options = new RoomOptions { MaxPlayers = 6, CustomRoomProperties = customRoomProperties, CustomRoomPropertiesForLobby = new string[] { TEAM_TYPE.DEFENDERS.ToString(), TEAM_TYPE.INVADERS.ToString() }, PlayerTtl = 10000 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        SetActivePanel(JoinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLeaveGameButtonClicked()
    {
        object count;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(DataManager.instance.chosenGameTeam.ToString(), out count);
        if (count != null)
        {
            Hashtable newProperty = new Hashtable { { DataManager.instance.chosenGameTeam.ToString(), (int)count - 1 } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProperty);
            Debug.Log(DataManager.instance.chosenGameTeam.ToString() + ":"+ ((int)count));
        }
        else
            Debug.LogError("The room has no count for the chosen team");
        ProcessingMenu.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    public void OnLoginButtonClicked()
    {
        ProcessingMenu.SetActive(true);
        string playerName = "Nameless";
        if (DataManager.instance != null)
            playerName = DataManager.instance.playerName;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }

    public void OnRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(RoomListPanel.name);
    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("DemoAsteroids-GameScene");
    }

    #endregion

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(MatchmakingKeyIDs.IS_PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    public void LocalPlayerPropertiesUpdated()
    {
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public void SetActivePanel(string activePanel)
    {
        SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
        CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
        JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
        RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
        InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(RoomListEntryPrefab, RoomListContent.transform);
            //entry.transform.SetParent(RoomListContent.transform);
            entry.transform.localScale = Vector3.one;

            object defenderCount, invaderCount;
            info.CustomProperties.TryGetValue(TEAM_TYPE.DEFENDERS.ToString(), out defenderCount);
            info.CustomProperties.TryGetValue(TEAM_TYPE.INVADERS.ToString(), out invaderCount);
            entry.GetComponent<MatchmakingRoomEntry>().Initialize(info.Name, (byte)info.PlayerCount, (byte)(int)defenderCount, (byte)(int)invaderCount);

            roomListEntries.Add(info.Name, entry);
        }
    }

    public void ReturnToMainMenu()
    {
        PhotonNetwork.Disconnect();
        PlayerPrefs.SetInt("MainMenuSkipVFX", 1);
        if (DataManager.instance.lastTeam == TEAM_TYPE.DEFENDERS)
            SceneTransitionManager.instance.SwitchScene("MainMenu_Defenders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
        else
            SceneTransitionManager.instance.SwitchScene("MainMenu_Invaders", SceneTransitionManager.ENTRANCE_TYPE.FADE_IN, SceneTransitionManager.EXIT_TYPE.FADE_OUT);
    }

    public void ToggleSettings()
    {
        openSettings = !openSettings;
        if (openSettings)
        {
            SceneManager.LoadSceneAsync("SettingScene", LoadSceneMode.Additive);
            AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._uiOpenSound);
        }
        else
        {
            SceneManager.UnloadSceneAsync("SettingScene");
            AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._uiCloseSound);
        }

    }
}
