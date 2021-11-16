using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected List<BaseState> states;

    protected BaseState currentState;

    protected virtual void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    protected virtual void Update()
    {
        if (currentState != null)
            currentState.UpdateLogic();
    }

    protected void ChangeState(BaseState newState, params object[] inputs)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter(inputs);
    }

    /// <summary>
    /// Public helper function that helps to reload/restart the current state.
    /// </summary>
    public void ReloadState(params object[] inputs)
    {
        currentState.Exit();
        currentState.Enter(inputs);
    }

    /// <summary>
    /// Public function that changes the state of the statemachine by the state's name. Should be used in BaseStates.UpdateLogic() when changing states.
    /// </summary>
    /// <param name="newStateName">(string)Name of the new state</param>
    /// <param name="inputs">A list of inputs the new BaseState will need.</param>
    /// <returns>Whether the changing of state was successful.</returns>
    public virtual bool ChangeStateByName(string newStateName)
    {
        Debug.LogError("StateMachine is using default ChangeStateByName! This function needs to be overriden");
        return false;
    }

    /// <summary>
    /// Returns the initial state of the Statemachine.
    /// </summary>
    /// <returns>The initial state of the StateMachine</returns>
    protected virtual BaseState GetInitialState()
    {
        return states[0];
    }

    // Used to show the current name of the state.
    private void OnGUI()
    {
        string content = currentState != null ? currentState.stateName : "(no current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
}