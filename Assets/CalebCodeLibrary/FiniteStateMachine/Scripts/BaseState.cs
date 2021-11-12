using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <b>SELF NOTE TO MYSELF</b>:
/// It is recommended to list the values that the state needs in the XML summary comments fur easy reference.
/// </summary>
public abstract class BaseState
{
    /// <summary>
    /// Will be used to identify the state.
    /// </summary>
    public string stateName { get; protected set; }
    protected StateMachine stateMachine;

    /// <summary>
    /// Constructor that helps initialise the BaseState.
    /// </summary>
    /// <param name="stateName">Name of the state.</param>
    /// <param name="stateMachine">The state machine that this BaseState belongs to.</param>
    public BaseState(string stateName, StateMachine stateMachine)
    {
        this.stateName = stateName;
        this.stateMachine = stateMachine;
    }

    /// <summary>
    /// Calls the start of the BaseStart. Parameters required are determined by the respective base states.
    /// </summary>
    /// <param name="inputs">The parameters that this BaseState will need.</param>
    public abstract void Enter(params object[] inputs);
    public abstract void UpdateLogic(); // UpdateLogic for its name so as to not confuse with Unity's Update()
    public abstract void Exit();

    public abstract void ForceFinishState();
}