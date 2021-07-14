using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamsData", menuName = "Team/TeamsData", order = 2)]
public class TeamsData : ScriptableObject
{
    public List<TeamObject> teams;
}