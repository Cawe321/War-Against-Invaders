using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parameters needed:
/// <para>(0)-(PlaneEntity) This plane entity</para>
/// <para>(1)-(BaseEntity)</para>
/// </summary>
public class PlaneInvaderBombingState : BaseState
{
    public PlaneInvaderBombingState(StateMachine stateMachine) : base("PlaneBombingState", stateMachine) { }
    /* Setting Values */
    float maxShootAngle = 1f;

    /* Script Values */
    PlaneEntity planeEntity;

    BaseEntity targetEntity;
    EntityHealth targetHealth;
    public override void Enter(params object[] inputs)
    {
        // Init from inputs
        planeEntity = inputs[0] as PlaneEntity;
        targetEntity = inputs[1] as BaseEntity;
        targetHealth = targetEntity.GetComponent<EntityHealth>();

        if (targetEntity == null)
        {
            // Assuming that cannot find any targets
            stateMachine.ChangeStateByName("PlanePatrolState");
        }
    }

    public override void UpdateLogic()
    {
        if (planeEntity.baseEntity.getFuelPercentage < 25f || !planeEntity.baseEntity.HasAmmo(EntityWeapon.WEAPON_TYPE.SECONDARY)) // Check if plane has less than 25% fuel or is out of secondary ammo
        {
            // Change to next state
            stateMachine.ChangeStateByName("PlaneRunwayState");
            return;
        }

        if (targetEntity == null || !targetHealth.isAlive)
        {
            stateMachine.ChangeStateByName("PlaneTravelState");
        }

        // moves towards predicted target position & accelerate
        planeEntity.RotateToTargetPosition(targetEntity.transform.position);
        planeEntity.Accelerate(); // No need to check for max flight speed since it has already been handled in this function

        if (Vector3.Angle(targetEntity.transform.position - planeEntity.transform.position, planeEntity.transform.forward) <= maxShootAngle)
        {
            // Target is within shooting range, fire all secondary weapons
            planeEntity.FireAllWeapons(EntityWeapon.WEAPON_TYPE.SECONDARY);
        }
    }

    public override void Exit()
    {
    }

    public override void ForceFinishState()
    {
    }


}
