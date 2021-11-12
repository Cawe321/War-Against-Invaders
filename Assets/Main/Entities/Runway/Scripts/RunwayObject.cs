using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RunwayObject : MonoBehaviour
{
    [HideInInspector]
    public RunwayManager owner = null;

    public Transform entrancePosition;

    public Transform exitPosition;

    public Transform landingTargetPosition;
    public Transform landingTargetDirectionPosition;

    BaseEntity _occupant = null;
    bool _reloaded = false;

    /// <summary>
    /// 
    /// </summary>
    public BaseEntity occupant { get { return _occupant; } set { SetOccupant(value); } }

    /// <summary>
    /// Helper Get function that checks if the runway is occupied. If occupied, returns true. Else returns false.
    /// </summary>
    public bool isOccupied { get { return _occupant != null; } }

    [HideInInspector]
    public Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    /// <summary>
    /// Private function that checks if the runway is empty before setting a new occupant.
    /// </summary>
    /// <param name="newOccupant">The new occupant.</param>
    void SetOccupant(BaseEntity newOccupant)
    {
        if (newOccupant.team == owner.teamType && newOccupant.GetComponent<PlaneEntity>() != null) // Checks if current runway is occupied and the new occupant is a Plane and that they are in the same team
        {
            // No longer occupied, ready for new occupant.
            _occupant = newOccupant;
            _reloaded = false;
        }
        else
            Debug.LogError("RunwayObject: SetOccupant was called even when the occupant is not compatible with the runway.");
    }

    /// <summary>
    /// Tells the current entity to find another runway.
    /// </summary>
    void RevokeRunwayPermissions()
    {
        if (!occupant.isAnyPlayerControlling)
        {
            // Tell the AI to find a new Runway
            PlaneEntity planeEntity = occupant.GetComponent<PlaneEntity>();
            planeEntity.stateMachine.ReloadState(planeEntity);
            _occupant = null;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        BaseEntity colliderEntity = collision.gameObject.GetComponent<EntityHealth>().baseEntity;
        if (colliderEntity != null) // If another Entity which is not the assigned occupant landed on the runway
        {
            if (occupant != null && collision.gameObject != occupant.gameObject)
            {
                // Check if it's the player
                if (colliderEntity.isAnyPlayerControlling)
                {
                    // If true, revoke the current assigned occupant since the player has priority over runways
                    RevokeRunwayPermissions();
                    occupant = colliderEntity;
                }
            }

            // Do what the runway needs to do regardless of whether it's the correct occupant.
            if (colliderEntity.team == owner.teamType)
            {
                PlaneEntity planeEntity = colliderEntity.GetComponent<PlaneEntity>();
                if (planeEntity != null)
                {
                    // Can confirm this entity is a plane. Check if the plane is powered off before reloading.
                    if (!planeEntity.engineActive)
                    {
                        colliderEntity.ReloadAll();
                        _reloaded = true;
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        BaseEntity colliderEntity = collision.gameObject.GetComponent<EntityHealth>().baseEntity;
        if (colliderEntity != null && colliderEntity == occupant && _reloaded) // Entity is leaving the runway
        {
            Debug.Log("Occupant gone");
            _occupant = null;
        }
    }
}
