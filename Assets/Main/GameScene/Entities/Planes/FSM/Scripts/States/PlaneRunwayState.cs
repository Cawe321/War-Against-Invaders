using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parameters needed:
/// <para>(0)-(PlaneEntity)</para>
/// </summary>
public class PlaneRunwayState : BaseState
{
    public PlaneRunwayState(StateMachine stateMachine) : base("PlaneRunwayState", stateMachine) { }

    /*In-script Values */
    PlaneEntity planeEntity;
    RunwayManager runwayManager;
    RunwayObject assignedRunway;
    const float circlingRadius = 75f;
    float circlingAngle = 1f;
    float cooldown = 1f;
    enum PHASE
    {
        LOOK_FOR_RUNWAY,
        ENTER_RUNWAY,
        LANDING_1,
        LANDING_2,
        WAIT_TO_RELOAD,
        TAKE_OFF,
    }
    PHASE currentPhase;

    public override void Enter(params object[] inputs)
    {
        cooldown = 1f;
        circlingAngle = 1f;
        currentPhase = PHASE.LOOK_FOR_RUNWAY;
        assignedRunway = null;

        // Handle All Inputs
        {
            planeEntity = inputs[0] as PlaneEntity;
            if (planeEntity == null)
                Debug.LogError("StateMachine's PlaneRunwayState has been given wrong inputs.");
        } 

        runwayManager = RunwayManager.GetInstanceOfTeam(planeEntity.baseEntity.team);
    }

    public override void UpdateLogic()
    {
        //Debug.Log(currentPhase);
        switch (currentPhase)
        {
            case PHASE.LOOK_FOR_RUNWAY:
                {
                    assignedRunway = runwayManager.FindAndAssignRunway(planeEntity.baseEntity);
                    if (assignedRunway != null)
                    {
                        currentPhase = PHASE.ENTER_RUNWAY;
                        return;
                    }
                    else
                    {
                        circlingAngle += Time.deltaTime;
                        if (circlingAngle >= 360)
                            circlingAngle -= 360;
                        // Continue flying around the runway at an altitude
                        if (planeEntity.flightSpeed > planeEntity.flightMinTakeOffSpeed + 10f) // Slowly reduce speed
                        {
                            for (int i = 0; i < stateMachine.updateFrameCooldown; ++i)
                                planeEntity.Decelerate();
                        }

                        Vector3 targetPosition = new Vector3(Mathf.Cos(circlingAngle) * circlingRadius + runwayManager.transform.position.x,
                                                                runwayManager.transform.position.y + 50f,
                                                                Mathf.Sin(circlingAngle) * circlingRadius + runwayManager.transform.position.z);
                        planeEntity.RotateToTargetPosition(targetPosition);
                    }
                    break;
                }
            case PHASE.ENTER_RUNWAY:
                {
                    Vector3 offset;
                    offset = new Vector3(0, 100, -planeEntity.flightSpeed * 5);
                    offset = assignedRunway.transform.rotation * offset;
                    Vector3 targetPos = assignedRunway.entrancePosition.position + offset;
                    if ((planeEntity.transform.position - targetPos).sqrMagnitude < 25f) // If plane entity is near the target position (<5 metres)
                    {
                        currentPhase = PHASE.LANDING_1;
                    }
                    else
                    {
                        // Continue landing
                        // Aim the plane to the target position
                       

                        planeEntity.RotateToTargetPosition(targetPos);
                    }
                    break;
                }
            case PHASE.LANDING_1:
                {
                    if ((planeEntity.transform.position - assignedRunway.landingTargetPosition.position).sqrMagnitude < 25f) // If plane entity is near the target position (<5 metres)
                    {
                        currentPhase = PHASE.LANDING_2;
                        cooldown = 1f;
                        return;
                    }
                    else if (planeEntity.flightSpeed < 0) // Plane has stopped.
                    {
                        planeEntity.ToggleEngine();
                        currentPhase = PHASE.WAIT_TO_RELOAD;
                        cooldown = 1f;
                        return;
                    }
                    else
                    {
                        // Continue landing
                        // Reduce the plane speed to its minimum flight speed
                        {
                            if (planeEntity.flightSpeed > planeEntity.flightMinTakeOffSpeed)
                            {
                                for (int i = 0; i < stateMachine.updateFrameCooldown; ++i)
                                    planeEntity.Decelerate();
                            }
                            if (planeEntity.flightSpeed < planeEntity.flightMinTakeOffSpeed)
                                planeEntity.SetFlightSpeed(planeEntity.flightMinTakeOffSpeed);
                        }


                        planeEntity.RotateToTargetPosition(assignedRunway.landingTargetPosition.position);
                    }
                    break;
                }
            case PHASE.LANDING_2:
                {
                    if (planeEntity.flightSpeed < 1f) // Plane has stopped
                    {
                        planeEntity.ToggleEngine();
                        currentPhase = PHASE.WAIT_TO_RELOAD;
                        cooldown = 1f;
                        return;
                    }
                    else
                    {
                        // Continue landing
                        // Reduce the plane speed to 0f
                        {
                            if (planeEntity.flightSpeed > Mathf.Epsilon)
                            {
                                for (int i = 0; i < stateMachine.updateFrameCooldown; ++i)
                                    planeEntity.Decelerate();
                            }
                            if (planeEntity.flightSpeed < Mathf.Epsilon)
                                planeEntity.SetFlightSpeed(0f);
                        }

                        planeEntity.RotateToTargetDirection((assignedRunway.landingTargetDirectionPosition.position - assignedRunway.landingTargetPosition.position).normalized);
                    }
                    break;
                }
            case PHASE.WAIT_TO_RELOAD:
                {
                    if (cooldown <= Mathf.Epsilon)
                    {
                        planeEntity.ToggleEngine(); // Turn the engine on
                        currentPhase = PHASE.TAKE_OFF;
                        return;
                    }
                    else
                        cooldown -= Time.deltaTime * stateMachine.updateFrameCooldown;
                    break;
                }
            case PHASE.TAKE_OFF:
                {

                    if ((planeEntity.transform.position - assignedRunway.exitPosition.position).sqrMagnitude < 25f) // If plane entity is near the target position (<5 metres) flight
                    {
                        // CHANGE TO NEXT STATE
                        stateMachine.ChangeStateByName("PlaneTravelState");
                        //Debug.Log("Finished Takeoff");
                        return;
                    }
                    else
                    {
                        // Increase the plane speed
                        for (int i = 0; i < stateMachine.updateFrameCooldown; ++i)
                            planeEntity.Accelerate();

                        // Continue Taking Off
                        planeEntity.RotateToTargetDirection((assignedRunway.exitPosition.position - assignedRunway.landingTargetPosition.position).normalized);
                        planeEntity.RotateToTargetPosition(assignedRunway.exitPosition.position);
                    }

                    break;
                }
        }
    }

    public override void Exit()
    {
        // Nothing to do currently.
    }

    public override void ForceFinishState()
    {
        // CHANGE TO NEXT STATE
        stateMachine.ChangeStateByName("PlaneTravelState");
    }


}
