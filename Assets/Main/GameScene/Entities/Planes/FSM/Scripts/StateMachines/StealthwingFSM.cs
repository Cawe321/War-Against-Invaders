using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthwingFSM : StateMachine
{
    PlaneEntity planeEntity;
    BaseEntity enemyEntity;

    bool isLastFramePlayerControlled = false;

    private void Awake()
    {
        isLastFramePlayerControlled = false;
        states = new List<BaseState>();

        // ALL STATES
        states.Add(new PlaneTravelState(this)); // Initial State
        states.Add(new PlanePatrolState(this));
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
        if (PhotonNetwork.IsMasterClient)
        {
            planeEntity.StartFlyingInstantly(); // For demo purposes only
            ChangeStateByName(GetInitialState().stateName);
        }
    }

    protected override void Update()
    {
        if (!PhotonNetwork.IsMasterClient) // Dont update AI if not master client
            return;

        // Return back to AI
        if (isLastFramePlayerControlled == true && planeEntity.baseEntity.isAnyPlayerControlling == false)
        {
            ChangeStateByName(GetInitialState().stateName);
        }

        if (!planeEntity.baseEntity.isAnyPlayerControlling)
            base.Update();
        isLastFramePlayerControlled = planeEntity.baseEntity.isAnyPlayerControlling;
    }

    public override bool ChangeStateByName(string newStateName)
    {

        switch (newStateName)
        {
            case "PlaneTravelState":
                {
                    ChangeState(FindStateByName(newStateName), planeEntity, enemyEntity);
                    break;
                }
            case "PlanePatrolState":
                {
                    ChangeState(FindStateByName(newStateName), planeEntity, enemyEntity);
                    break;
                }
            case "PlaneRunwayState":
                {
                    ChangeState(FindStateByName(newStateName), planeEntity);
                    break;
                }
            case "PlaneDogfightState":
                {
                    ChangeState(FindStateByName(newStateName), planeEntity);
                    break;
                }
            case "PlaneBombingState":
                {
                    ChangeStateByName("PlaneDogfightState");
                    break;
                }

            default:
                Debug.LogError("StateMachine: ChangeStateByName was called, but the state " + newStateName + "corresponding to the string given was not found!.");
                return false;
        }
        return true;
    }

}
