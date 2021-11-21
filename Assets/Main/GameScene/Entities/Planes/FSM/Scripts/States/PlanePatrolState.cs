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
    float circlingSpeed = 0.1f;

    /* Script Values */
    PlaneEntity planeEntity;
    BaseEntity patrolEntity;
    float circlingAngle = 0f;

    public override void Enter(params object[] inputs)
    {
        // Init from the inputs
        planeEntity = inputs[0] as PlaneEntity;
        patrolEntity = inputs[1] as BaseEntity;

        // Init values
        circlingAngle = 0f;
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
        planeEntity.Accelerate();

        // Check if any object is near, fly away if that's the case
        Collider[] allColliders = Physics.OverlapSphere(planeEntity.transform.position, 200f);
        Collider closestCollider = null;
        float closestDistance = 0f;
        foreach (Collider collider in allColliders)
        {
            if (collider != planeEntity.GetComponent<Collider>() && !collider.transform.IsChildOf(planeEntity.transform) && (closestCollider == null || closestDistance > (collider.transform.position - planeEntity.transform.position).sqrMagnitude))
            {
                closestCollider = collider;
                closestDistance = (collider.transform.position - planeEntity.transform.position).sqrMagnitude;
            }
        }
        if (closestCollider != null) // Fly away to avoid collision
            planeEntity.RotateToTargetDirection(planeEntity.transform.position - closestCollider.ClosestPoint(planeEntity.transform.position));
        else
        {
            //Travel in a circle == Patrol
            Vector3 targetPosition = new Vector3(Mathf.Cos(circlingAngle) * radiusOfPatrol + patrolEntity.transform.position.x,
                                                  patrolEntity.transform.position.y + 50f,
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
