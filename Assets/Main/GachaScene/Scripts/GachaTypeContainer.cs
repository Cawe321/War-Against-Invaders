using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GachaTypeContainer", menuName = "WarAgainstInvaders/Gacha/GachaTypeContainer", order = 0)]
public class GachaTypeContainer : ScriptableObject
{
    [SerializeField]
    public List<GachaType> gachaTypeContainer;

    public List<GachaType> GetAllGachaTypeOfTeam(TEAM_TYPE teamType)
    {
        List<GachaType> filteredList = new List<GachaType>();
        foreach (GachaType gacha in gachaTypeContainer)
        {
            if (gacha.teamType == teamType)
                filteredList.Add(gacha);
        }
        return filteredList;
    }
}

