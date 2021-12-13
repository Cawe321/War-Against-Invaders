using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parameters needed:
/// <para>(0)-(PlaneEntity)</para>
/// <para>(1)-(BaseEntity) The target entity</para>
/// </summary>
public class PlanePatrolState : BaseState
{
    public PlanePatrolState(StateMachine stateMachine) : base("PlanePatrolState", stateMachine) { }

    /* Setting Values */
    float radiusOfPatrol = 1000f;
    float circlingSpeed = 2f;

    /* Script Values */
    PlaneEntity planeEntity;
    BaseEntity patrolEntity;
    float circlingAngle = 0f;

    float yOffset = 500f;

    public override void Enter(params object[] inputs)
    {
        // Init from the inputs
        planeEntity = inputs[0] as PlaneEntity;
        patrolEntity = inputs[1] as BaseEntity;

        // Init values
        circlingAngle = Random.Range(0f, circlingAngle);
    }
    public override void UpdateLogic()
    {
        if (planeEntity.baseEntity.getFuelPercentage < 25f) // Check if plane has less than 25% fuel
        {
            // PlaneEntity is low on fuel
            // Change to next state
            stateMachine.ChangeStateByName("PlaneRunwayState");
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(planeEntity.transform.position, 750f); // Check if enemy plane is nearby
        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                EntityHealth entityHealth = collider.GetComponent<EntityHealth>();
                if (entityHealth != null && entityHealth.baseEntity.team != planeEntity.baseEntity.team && entityHealth.baseEntity.GetComponent<PlaneEntity>() != null)
                {
                    // Enemy plane has been detected
                    // Change to next state
                    stateMachine.ChangeStateByName("PlaneDogfightState");
                    return;
                }
            }
        }

        circlingAngle += Time.deltaTime * circlingSpeed;
        if (circlingAngle >= 360)
            circlingAngle -= 360;

        // keep on accelerating because why not?
        for (int i = 0; i < stateMachine.updateFrameCooldown; ++i)
            planeEntity.Accelerate();

        // Check if any object is near, fly away if that's the case
        Collider[] allColliders = Physics.OverlapSphere(planeEntity.transform.position, 100f);
        bool toAvoid = false;
        Vector3 averageDirection = Vector3.zero;
        foreach (Collider collider in allColliders)
        {
            if (collider != planeEntity.GetComponent<Collider>() && !collider.transform.IsChildOf(planeEntity.transform))
            {
                toAvoid = true;
                averageDirection += (planeEntity.transform.position - collider.ClosestPoint(planeEntity.transform.position)).normalized;
            }
        }
        if (toAvoid) // Fly away to avoid collision
        {
            planeEntity.RotateToTargetDirection(averageDirection.normalized);
        }
        else
        {
            //Travel in a circle == Patrol
            Vector3 targetPosition = new Vector3(Mathf.Cos(circlingAngle) * radiusOfPatrol + patrolEntity.transform.position.x,
                                                  patrolEntity.transform.position.y + yOffset,
                                                  Mathf.Sin(circlingAngle) * radiusOfPatrol + patrolEntity.transform.position.z);
            planeEntity.RotateToTargetPosition(targetPosition);
        }
    }

    public override void Exit()
    {
    }

    public override void ForceFinishState()
    {
    }

}
