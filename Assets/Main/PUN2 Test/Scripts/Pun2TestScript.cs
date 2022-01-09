using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pun2TestScript : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + " server");
    }

    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.Log(debugMessage);
    }
}
