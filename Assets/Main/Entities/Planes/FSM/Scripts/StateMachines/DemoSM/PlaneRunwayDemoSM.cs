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
        planeEntity.StartFlyingInstantly(); // For demo purposes only
                                            //base.Start();

        StartCoroutine(WaitOneFrameBeforeStartState());
    }

    IEnumerator WaitOneFrameBeforeStartState()
    {
        yield return new WaitForEndOfFrame();
        ChangeStateByName("PlaneRunwayState", planeEntity);
    }

    protected override void Update()
    {
        // Return back to AI
        if (isLastFramePlayerControlled == true && planeEntity.baseEntity.isAnyPlayerControlling == false)
        {
            ChangeStateByName("PlaneRunwayState", planeEntity);
        }
        base.Update();
        isLastFramePlayerControlled = planeEntity.baseEntity.isAnyPlayerControlling;
    }
}
