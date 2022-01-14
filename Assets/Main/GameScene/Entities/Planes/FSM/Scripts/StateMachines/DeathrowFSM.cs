using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathrowFSM : StateMachine
{
    PlaneEntity planeEntity;
    BaseEntity homeEntity;
    BaseEntity enemyEntity;

    bool isLastFramePlayerControlled = false;

    private void Awake()
    {
        isLastFramePlayerControlled = false;
        states = new List<BaseState>();

        // ALL STATES
        states.Add(new PlaneTravelState(this));   // Initial State
        states.Add(new PlaneInvaderBombingState(this));
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
        homeEntity = GameplayManager.instance.GetSpaceshipEntity();
        enemyEntity = GameplayManager.instance.GetDockEntity();

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
        foreach (BaseState state in states)
        {
            if (state.stateName == newStateName)
            {
                switch (state.stateName)
                {
                    case "PlanePatrolState":
                        {
                            ChangeState(state, planeEntity, homeEntity);
                            break;
                        }
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
                            ChangeState(state, planeEntity, GameplayManager.instance.GetRandomCoreDockComponent());
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
