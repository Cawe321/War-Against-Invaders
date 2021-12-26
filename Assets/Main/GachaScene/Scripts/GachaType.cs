using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GachaType", menuName = "WarAgainstInvaders/Gacha/GachaType", order = 0)]
public class GachaType : ScriptableObject
{
    [Header("Frontend Settings")]
    public string gachaTitle;
    [TextArea]
    public string gachaDescription;
    public Sprite imageBanner;

    [Header("Backend Settings")]
    public TEAM_TYPE teamType;
    public string cloudscriptFunctionName;

}
