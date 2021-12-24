using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaneEquipmentEntity
{
    public EntityTypes entityType;
    public TEAM_TYPE teamType;

    public int wingID = -1;
    public int lightID = -1;
    public int heavyID = -1;
}
