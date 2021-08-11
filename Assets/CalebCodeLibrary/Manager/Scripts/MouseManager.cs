using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton object that helps to control the mouse in game.
/// </summary>
public class MouseManager : SingletonObject<MouseManager>
{
    #region ACCESSIBLE_VALUES
    /// <summary>
    /// Gets/Sets the lockstate of the cursor.
    /// </summary>
    public CursorLockMode mouseLockState
    {
        get
        {
            return Cursor.lockState;
        }
        set
        {
            Cursor.lockState = value;
        }
    }

    /// <summary>
    /// Get the position of the mouse away from center.
    /// </summary>
    public Vector2 mousePosAwayFromCenter
    {
        get
        {
            return new Vector2(Input.mousePosition.x - (Screen.width * 0.5f), Input.mousePosition.y - (Screen.height * 0.5f));
        }
    }
    #endregion;

    public override void Awake()
    {
        // Singleton awake
        base.Awake();
    }


}
