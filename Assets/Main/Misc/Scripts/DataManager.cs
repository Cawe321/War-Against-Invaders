using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonObject<DataManager>
{
    [Serializable]
    public struct PlayerOnlineData
    {
        public string playerName;
        public int commonCurrency;
        public int premiumCurrency;
        public TEAM_TYPE lastTeam; // used to keep track of which team the last player selected
    }
    public PlayerOnlineData playerData;

    public TEAM_TYPE chosenGameTeam;

    public override void Awake()
    {
        base.Awake();
        LoadPlayerData();
    }

    public void LoadPlayerData()
    {
        // CODE HERE to load player data from server


        chosenGameTeam = playerData.lastTeam;
    }

    public void SavePlayerData()
    {
        // CODE HERE to save player data to server
    }


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


