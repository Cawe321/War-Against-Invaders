using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamObject", menuName = "Team/TeamObject", order = 1)]
public class TeamObject : ScriptableObject
{
    /** Visual Settings **/
    [Header("Visual Settings")]

    [Tooltip("Name of the team.")]
    public string teamName;

    [Tooltip("Initials of the team, or short name.")]
    public string teamInitials;

    [Tooltip("Logo of the team.")]
    public Sprite teamLogo;

    [Tooltip("Default color of the team. Will be used for text color.")]
    public Color teamColor;

    /** Technical Settings **/
    [Tooltip("The type of team this team will be categorized into.")]
    public TEAM_TYPE startingTeamType;

    [TextArea]
    [Tooltip("Will be used whenever this team requires text for special events.")]
    public string entranceText;
}
