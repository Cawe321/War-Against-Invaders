using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SingletonObject<GameplayManager>
{
    [SerializeField]
    BaseEntity dockEntity;
    
    [SerializeField]
    BaseEntity spaceshipEntity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Gets the dock entity.
    /// </summary>
    /// <returns>Returns BaseEntity of the dock.</returns>
    public BaseEntity GetDockEntity()
    {
        return dockEntity;
    }

    /// <summary>
    /// Gets the spaceship entity.
    /// </summary>
    /// <returns>Returns BaseEntity of the spaceship</returns>
    public BaseEntity GetSpaceshipEntity()
    {
        return spaceshipEntity;
    }

    public EntityHealth GetRandomSpaceshipPart()
    {
        return null;
    }

    public EntityHealth GetRandomCoreDockComponent()
    {
        return null;
    }
}
