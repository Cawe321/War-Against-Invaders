using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleModeSelection_Class : MonoBehaviour
{
    public enum BATTLEMODE_SELECTION
    {
        NONE,
        SINGLEPLAYER,
        MULTIPLAYER,
    }
    public BATTLEMODE_SELECTION selection;
}
