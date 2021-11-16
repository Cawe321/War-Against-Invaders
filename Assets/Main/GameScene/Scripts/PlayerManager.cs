using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonObject<PlayerManager>
{
    public string playerName = "";
    public TEAM_TYPE playerTeam;
    public bool freeRoam = true;
}
