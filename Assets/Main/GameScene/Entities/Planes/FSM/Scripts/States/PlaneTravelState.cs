using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parameters needed:
/// <para>(0)-(PlaneEntity)</para>
/// <para>(1)-(BaseEntity) The enemy entity</para>
/// </summary>
public class PlaneTravelState : BaseState
{
    public PlaneTravelState(StateMachine stateMachine) : base("PlaneTravelState", stateMachine) { }

    /* Setting Values */
    Vector3 maxTargetPosOffset;

    /* Script Values */
    PlaneEntity planeEntity;
    BaseEntity targetEntity;
    Vector3 targetPosOffset;

    public override void Enter(params object[] inputs)
    {
        // Init the setting values first
        maxTargetPosOffset = new Vector3(50f, 50f, 50f);

        // Init from the inputs
        planeEntity = inputs[0] as PlaneEntity;
        targetEntity = inputs[1] as BaseEntity;           
    }
    public override void UpdateLogic()
    {
        targetPosOffset = targetEntity.transform.position + new Vector3(Random.Range(-maxTargetPosOffset.x, maxTargetPosOffset.x), Random.Range(-maxTargetPosOffset.y, maxTargetPosOffset.y), Random.Range(-maxTargetPosOffset.z, maxTargetPosOffset.z));

        if (planeEntity.baseEntity.getFuelPercentage < 25f || (planeEntity.baseEntity.HasSecondaryWeapon() && !planeEntity.baseEntity.HasAmmo(EntityWeapon.WEAPON_TYPE.SECONDARY))) // Check if plane has less than 25% fuel
        {
            // PlaneEntity is low on fuel
            // Change to next state
            stateMachine.ChangeStateByName("PlaneRunwayState");
            return;
        }
        else if ((targetPosOffset - planeEntity.transform.position).sqrMagnitude < 1000f * 1000f) // Check if plane has reached destination
        {
            // PlaneEntity has reached it's target
            // Change to next state
            stateMachine.ChangeStateByName("PlaneBombingState");
            return;
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(planeEntity.transform.position, 500f);
            if (colliders.Length > 0)
            {
                foreach (Collider collider in colliders)
                {
                    EntityHealth entityHealth = collider.GetComponent<EntityHealth>();
                    if (entityHealth != null && entityHealth.baseEntity != null && entityHealth.baseEntity.team != planeEntity.baseEntity.team && entityHealth.baseEntity.GetComponent<PlaneEntity>() != null)
                    {
                        // Enemy plane has been detected
                        // Change to next state
                        stateMachine.ChangeStateByName("PlaneDogfightState");
                        return;
                    }
                }

                /*Collider closestCollider = null;
                float closestDistance = 0f;
                foreach (Collider collider in colliders)
                {
                    if (closestCollider == null || closestDistance > (collider.transform.position - planeEntity.transform.position).sqrMagnitude)
                    {
                        closestCollider = collider;
                        closestDistance = (collider.transform.position - planeEntity.transform.position).sqrMagnitude;
                    }
                }*/
            }

            // Check if any object is near, fly away if that's the case
            // Check if any object is near, fly away if that's the case
            Collider[] allColliders = Physics.OverlapSphere(planeEntity.transform.position, 100f);
            bool toAvoid = false;
            Vector3 averageDirection = Vector3.zero;
            foreach (Collider collider in allColliders)
            {
                if (collider != planeEntity.GetComponent<Collider>() && !collider.transform.IsChildOf(planeEntity.transform) && collider.gameObject.layer == 0)
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
                // if still remaining on this state, do what this state does
                // moves towards target position & accelerate
                planeEntity.RotateToTargetPosition(targetPosOffset);
                for (int i = 0; i < stateMachine.updateFrameCooldown; ++i)
                    planeEntity.Accelerate(); // No need to check for max flight speed since it has already been handled in this function
            }
        }

       

    }

    public override void Exit()
    {
        // Do nothing
    }

    public override void ForceFinishState()
    {
        // Do nothing
    }

}
