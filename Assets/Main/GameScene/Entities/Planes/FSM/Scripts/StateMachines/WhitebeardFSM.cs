using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhitebeardFSM : StateMachine
{
    PlaneEntity planeEntity;
    BaseEntity enemyEntity;

    bool isLastFramePlayerControlled = false;

    private void Awake()
    {
        isLastFramePlayerControlled = false;
        states = new List<BaseState>();

        // ALL STATES
        states.Add(new PlaneTravelState(this));   // Initial State
        states.Add(new PlaneDefenderBombingState(this));
        states.Add(new PlaneRunwayState(this));
        states.Add(new PlaneDogfightState(this));
    }


    protected override void Start()
    {
        planeEntity = GetComponent<PlaneEntity>();

        //base.Start();

        StartCoroutine(WaitForFrameBeforeStartState(2));
    }

    IEnumerator WaitForFrameBeforeStartState(int numberOfFrames)
    {
        enemyEntity = GameplayManager.instance.GetSpaceshipEntity();

        for (int i = 0; i < numberOfFrames; ++i)
            yield return new WaitForEndOfFrame();
        planeEntity.StartFlyingInstantly(); // For demo purposes only
        ChangeStateByName(GetInitialState().stateName);
    }

    protected override void Update()
    {
        // Return back to AI
        if (isLastFramePlayerControlled == true && planeEntity.baseEntity.isAnyPlayerControlling == false)
        {
            ChangeStateByName(GetInitialState().stateName);
            return;
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
                    case "PlaneDogfightState":
                        {
                            ChangeState(state, planeEntity);
                            break;
                        }
                    case "PlaneTravelState":
                        {
                            ChangeState(state, planeEntity, enemyEntity);
                            break;
                        }
                    case "PlaneBombingState":
                        {
                            ChangeState(state, planeEntity, GameplayManager.instance.GetRandomSpaceshipPart());
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
