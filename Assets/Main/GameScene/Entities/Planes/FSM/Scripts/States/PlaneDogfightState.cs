using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parameters needed:
/// <para>(0)-(PlaneEntity) This plane entity</para>
/// </summary>
public class PlaneDogfightState : BaseState
{
    public PlaneDogfightState(StateMachine stateMachine) : base("PlaneDogfightState",stateMachine) { }

    /* Setting Values*/
    const float maxShootAngle = 30f;

    /* In-script Values*/
    PlaneEntity planeEntity;

    PlaneEntity enemyPlaneEntity;

    public override void Enter(params object[] inputs)
    {
        // Init the values from inputs
        planeEntity = inputs[0] as PlaneEntity;

        Collider[] colliders = Physics.OverlapSphere(planeEntity.transform.position, 750f);
        if (colliders.Length > 0)
        {
            // Find the enemy plane entity
            Collider closestCollider = null;
            float closestDistance = 0f;
            foreach (Collider collider in colliders)
            {
                EntityHealth entityHealth = collider.GetComponent<EntityHealth>();
                if (entityHealth != null && entityHealth.baseEntity.team != planeEntity.baseEntity.team && entityHealth.baseEntity.GetComponent<PlaneEntity>() != null && (closestCollider == null || closestDistance > (collider.transform.position - planeEntity.transform.position).sqrMagnitude))
                {
                    closestCollider = collider;
                    closestDistance = (collider.transform.position - planeEntity.transform.position).sqrMagnitude;
                }
            }

            if (closestCollider != null)
            {
                // Found an enemy plane, it shall be this plane's target
                enemyPlaneEntity = closestCollider.GetComponent<EntityHealth>().baseEntity.GetComponent<PlaneEntity>();
            }
            else
            {
                // Couldnt find a target
                stateMachine.ChangeStateByName("PlaneTravelState");
            }
        }
            
    }
    public override void UpdateLogic()
    {
        if (enemyPlaneEntity == null || !enemyPlaneEntity.baseEntity.CheckHealth())
        {
            // Enemy is dead
            stateMachine.ChangeStateByName("PlaneTravelState");
        }
        if (!planeEntity.baseEntity.HasAmmo(EntityWeapon.WEAPON_TYPE.PRIMARY)) // Check if plane still has ammo
        {
            // Plane no longer has ammo!
            stateMachine.ChangeStateByName("PlaneRunwayState");
        }

        // Check if any object is near, fly away if that's the case
        Collider[] allColliders = Physics.OverlapSphere(planeEntity.transform.position, 100f);
        bool toAvoid = false;
        Vector3 averageDirection = Vector3.zero;
        foreach (Collider collider in allColliders)
        {
            if (collider != planeEntity.GetComponent<Collider>() && !collider.transform.IsChildOf(planeEntity.transform) && collider.gameObject.layer == 0)
            {
                toAvoid = true;
                averageDirection += (planeEntity.transform.position - collider.ClosestPoint(planeEntity.transform.position)).normalized * 0.5f;
            }
        }
        Vector3 targetAimPosition = enemyPlaneEntity.transform.position + (enemyPlaneEntity.flightSpeed * 0.5f * enemyPlaneEntity.transform.forward);

        if (toAvoid) // Fly away to avoid collision
        {
            planeEntity.RotateToTargetDirection(averageDirection.normalized);
        }
        else
        {
            // if still remaining on this state, do what this state does
            // moves towards predicted target position & accelerate
            planeEntity.RotateToTargetPosition(targetAimPosition);
            for (int i = 0; i < stateMachine.updateFrameCooldown; ++i)
                planeEntity.Accelerate(); // No need to check for max flight speed since it has already been handled in this function
        }

        if (Vector3.Angle(targetAimPosition - planeEntity.transform.position, planeEntity.transform.forward) <= maxShootAngle)
        {
            // Enemy is within shooting range, fire all primary weapons
            planeEntity.FireAllWeapons(EntityWeapon.WEAPON_TYPE.PRIMARY);
        }
    }

    public override void Exit()
    {
        //throw new System.NotImplementedException();
    }

    public override void ForceFinishState()
    {
        // Do nothing
    }


}
