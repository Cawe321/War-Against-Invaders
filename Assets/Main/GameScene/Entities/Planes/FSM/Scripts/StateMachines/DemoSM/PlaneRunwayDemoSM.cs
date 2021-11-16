using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneRunwayDemoSM : StateMachine
{
    PlaneEntity planeEntity;

    bool isLastFramePlayerControlled = false;
    private void Awake()
    {
        isLastFramePlayerControlled = false;
        states = new List<BaseState>();

        // ALL STATES
        states.Add(new PlaneRunwayState(this)); // Initial State
    }

    protected override void Start()
    {
        planeEntity = GetComponent<PlaneEntity>();
         
        //base.Start();

        StartCoroutine(WaitForFrameBeforeStartState(2));
    }

    IEnumerator WaitForFrameBeforeStartState(int numberOfFrames)
    {
        for (int i = 0; i < numberOfFrames; ++i)
            yield return new WaitForEndOfFrame();
        planeEntity.StartFlyingInstantly(); // For demo purposes only
        ChangeStateByName("PlaneRunwayState");
    }

    protected override void Update()
    {
        // Return back to AI
        if (isLastFramePlayerControlled == true && planeEntity.baseEntity.isAnyPlayerControlling == false)
        {
            ChangeStateByName("PlaneRunwayState");
        }

        if (!planeEntity.baseEntity.isAnyPlayerControlling)
            base.Update();
        isLastFramePlayerControlled = planeEntity.baseEntity.isAnyPlayerControlling;
    }

    public override bool ChangeStateByName(string newStateName)
    {
        foreach (BaseState state in states)
        {
            if (state.stateName == newStateName)
            {
                switch (state.stateName)
                {
                    case "PlaneRunwayState":
                        {
                            ChangeState(state, planeEntity);
                            break;
                        }
                }
                return true;
            }
        }
        Debug.LogError("StateMachine: ChangeStateByName was called, but the state corresponding to the string given was not found!.");
        return false;
    }
}
