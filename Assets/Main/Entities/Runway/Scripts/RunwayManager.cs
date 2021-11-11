using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunwayManager : MonoBehaviour
{
    public TEAM_TYPE teamType;

    /* In Script Values */
    List<RunwayObject> runwayObjects;

    private void Start()
    {
        runwayObjects = new List<RunwayObject>(GetComponentsInChildren<RunwayObject>());
        foreach (RunwayObject runwayObject in runwayObjects)
        {
            runwayObject.owner = this;
        }
    }

    /// <summary>
    /// Static function that finds the instance of RunwayManager associated with the given team.
    /// </summary>
    /// <param name="teamType">The team that the Runway Mananager is associated with</param>
    /// <returns>Instance of the RunwayManager associated with the given team type. Returns null if not found.</returns>
    public static RunwayManager GetInstanceOfTeam(TEAM_TYPE teamType)
    {
        RunwayManager[]  runwayManagers = FindObjectsOfType<RunwayManager>();
        foreach(RunwayManager runwayManager in runwayManagers)
        {
            if (runwayManager.teamType == teamType)
                return runwayManager;
        }
        return null;
    }

    /// <summary>
    /// Public function that finds an available runway and assign it to the basee entity.
    /// </summary>
    /// <returns>A RunwayObject that the entity is assigned to. Returns null if no runways are available.</returns>
    public RunwayObject FindAndAssignRunway(BaseEntity occupant)
    {
        if (occupant.GetComponent<PlaneEntity>() != null && occupant.team == teamType)
        {
            RunwayObject availableRunway = FindEmptyRunway();
            if (availableRunway != null)
            {
                availableRunway.occupant = occupant;
                Debug.Log("Runway assigned to:" + occupant.name);
                return availableRunway;
            }

        }
        return null;
    }

    /// <summary>
    /// Private function that finds an available runway.
    /// </summary>
    /// <returns>A RunwayObject that is empty. Returns null if none.</returns>
    RunwayObject FindEmptyRunway()
    {
        foreach (RunwayObject runwayObject in runwayObjects)
        {
            if (!runwayObject.isOccupied)
                return runwayObject;
        }
        return null;
    }
}
