using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;


public class MatchmakingRoomEntry : MonoBehaviour
{
    public Text RoomNameText;
    public Text RoomPlayersText;
    public Button JoinRoomButton;

    private string roomName;

    public void Start()
    {
        JoinRoomButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(roomName);
        });
    }

    public void Initialize(string name, byte currentPlayers, byte defenderPlayers, byte invaderPlayers)
    {
        roomName = name;

        RoomNameText.text = name;
        RoomPlayersText.text = "Defenders: " + defenderPlayers + " | Invaders: " + invaderPlayers + " | Max Players: 6";
    }
}
