using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePatrolDemoSM : StateMachine
{
    PlaneEntity planeEntity;

    [SerializeField] BaseEntity targetEntity;

    bool isLastFramePlayerControlled = false;

    private void Awake()
    {
        isLastFramePlayerControlled = false;
        states = new List<BaseState>();

        // ALL STATES
        states.Add(new PlanePatrolState(this)); // Initial State
        //states.Add(new PlaneRunwayState(this)); 
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
        ChangeStateByName("PlanePatrolState");
    }

    protected override void Update()
    {
        // Return back to AI
        if (isLastFramePlayerControlled == true && planeEntity.baseEntity.isAnyPlayerControlling == false)
        {
            ChangeStateByName("PlanePatrolState");
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
                    case "PlanePatrolState":
                        {
                            ChangeState(state, planeEntity, targetEntity);
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
