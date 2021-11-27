using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Unity.Mathematics;
using UnityEngine;

public class GameplayManager : SingletonObject<GameplayManager>
{
    [Header("References")]
    public EntityList entityList;

    public CurrencySettings currencySettings;

    [SerializeField]
    DockEntity dockEntity;
    
    [SerializeField]
    SpaceshipEntity spaceshipEntity;

    [SerializeField]
    CanvasGroup waitForPlayersCanvas;

    [Header("Settings")]
    [SerializeField]
    Transform defendersSpawnLocation;

    [SerializeField]
    Transform invadersSpawnLocation;

    [Header("Spawn Wave Settings")]
    [SerializeField]
    [Tooltip("Cooldown for both teams")]
    float originalSpawnCooldown = 60f;
    float defenderSpawnCooldown;
    float invaderSpawnCooldown;

    [SerializeField]
    float spawnDistanceOffset = 50f;

    /// <summary>
    /// List of entities to spawn for defenders
    /// </summary>
    Dictionary<EntityTypes, int> defenderSpawnWave;
    float defenderSpawnCooldownMultiplier = 1f;

    /// <summary>
    /// List of entities to spawn for invaders
    /// </summary>
    Dictionary<EntityTypes, int> invaderSpawnWave;
    float invaderSpawnCooldownMultiplier = 1f;



    public enum GAMEPLAY_PHASE
    {
        WAIT,
        INTRO,
        GAME,
        END_WINNER_INVADERS,
        END_WINNER_DEFENDERS,
    }
    public GAMEPLAY_PHASE gameplayPhase { get; private set; } = GAMEPLAY_PHASE.WAIT;

    bool playersLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        waitForPlayersCanvas.gameObject.SetActive(true);

        gameplayPhase = GAMEPLAY_PHASE.WAIT;

        defenderSpawnWave = new Dictionary<EntityTypes, int>();
        defenderSpawnWave.Add(EntityTypes.F16, 1);

        invaderSpawnWave = new Dictionary<EntityTypes, int>();
        invaderSpawnWave.Add(EntityTypes.Mako, 3);

        //StartIntro();
        StartCoroutine(dummyPlayerLoad());
    }

    IEnumerator dummyPlayerLoad()
    {
        yield return new WaitForSecondsRealtime(5f);
        playersLoaded = true;
    }

    IEnumerator DisableWaitForPlayersCanvas()
    {
        while (waitForPlayersCanvas.alpha > Mathf.Epsilon)
        {
            yield return new WaitForEndOfFrame();
            waitForPlayersCanvas.alpha -= Time.deltaTime;
        }
        waitForPlayersCanvas.alpha = 0f;
        waitForPlayersCanvas.gameObject.SetActive(false);
        StartIntro();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameplayPhase)
        {
            case GAMEPLAY_PHASE.WAIT:
                {
                    if (playersLoaded)
                    {
                        StartCoroutine(DisableWaitForPlayersCanvas());
                    }
                    break;
                }
            case GAMEPLAY_PHASE.INTRO:
                {
                    if (spaceshipEntity.StartMoving()) // Check if spaceship is ready to start moving. If ready, automatically starts moving
                    {
                        gameplayPhase = GAMEPLAY_PHASE.GAME;
                        SpawnTheEntityWave(TEAM_TYPE.DEFENDERS);
                        SpawnTheEntityWave(TEAM_TYPE.INVADERS);
                        defenderSpawnCooldown = originalSpawnCooldown;
                        invaderSpawnCooldown = originalSpawnCooldown;

                    }
                    break;
                }
            case GAMEPLAY_PHASE.GAME:
                {
                    // Spawn Wave Logic
                    // Each warehouse destroyed will result in a 20% increase of the original cooldown time
                    // Base 40% cooldown for defender cooldown multiplier
                    defenderSpawnCooldownMultiplier = 0.4f + dockEntity.GetPercentageOfExistingWarehouses() * 0.6f;

                    defenderSpawnCooldown -= Time.deltaTime * defenderSpawnCooldownMultiplier;
                    invaderSpawnCooldown -= Time.deltaTime * invaderSpawnCooldownMultiplier;
                    if (defenderSpawnCooldown < 0f)
                    {
                        SpawnTheEntityWave(TEAM_TYPE.DEFENDERS);
                        defenderSpawnCooldown = originalSpawnCooldown;
                    }
                    if (invaderSpawnCooldown < 0f)
                    {
                        SpawnTheEntityWave(TEAM_TYPE.INVADERS);
                        invaderSpawnCooldown = originalSpawnCooldown;
                    }

                    break;
                }
            case GAMEPLAY_PHASE.END_WINNER_INVADERS:
                {
                    Debug.Log("Invaders have won!");
                    break;
                }
            case GAMEPLAY_PHASE.END_WINNER_DEFENDERS:
                {
                    Debug.Log("Defenders have won!");
                    break;
                }
        }
    }

    /// <summary>
    /// Adds the entity to spawn wave.
    /// </summary>
    /// <param name="team">Team of entity</param>
    /// <param name="entity">The entity to add</param>
    /// <param name="count">Number of entities</param>
    public void AddToSpawnWave(TEAM_TYPE team, EntityTypes entity, int count = 1)
    {
        if (team == TEAM_TYPE.DEFENDERS)
        {
            if (entityList.GetEntityTeam(entity) == team)
            {
                if (defenderSpawnWave.ContainsKey(entity))
                    defenderSpawnWave[entity] = defenderSpawnWave[entity] + count;
                else
                    defenderSpawnWave.Add(entity, count);
            }
            else Debug.LogWarning("GameplayManager.AddToSpawnWave(): Entity added to team does not belong to that team!");
        }
        else if (team == TEAM_TYPE.INVADERS)
        {
            if (entityList.GetEntityTeam(entity) == team)
            {
                if (invaderSpawnWave.ContainsKey(entity))
                    invaderSpawnWave[entity] = invaderSpawnWave[entity] + count;
                else
                    invaderSpawnWave.Add(entity, count);
            }
            else Debug.LogWarning("GameplayManager.AddToSpawnWave(): Entity added to team does not belong to that team!");
        }
    }

    /// <summary>
    /// Spawns the wave of selected entity. Will spawn based on the camera spawn location.
    /// </summary>
    void SpawnTheEntityWave(TEAM_TYPE team)
    {
        List<EntityTypes> objectsToSpawn = new List<EntityTypes>();
        Vector3 originalSpawnPos = Vector3.zero;
        Quaternion quaternion = new Quaternion();
        if (team == TEAM_TYPE.DEFENDERS)
        {
            originalSpawnPos = defendersSpawnLocation.position;
            quaternion = dockEntity.transform.rotation;
            foreach(EntityTypes entity in defenderSpawnWave.Keys.ToArray())
            {
                for (int count = 0; count < defenderSpawnWave[entity]; ++count)
                    objectsToSpawn.Add(entity);
            }
        }
        else if (team == TEAM_TYPE.INVADERS)
        {
            originalSpawnPos = invadersSpawnLocation.position;
            quaternion = spaceshipEntity.transform.rotation;
            foreach (EntityTypes entity in invaderSpawnWave.Keys.ToArray())
            {
                for (int count = 0; count < invaderSpawnWave[entity]; ++count)
                    objectsToSpawn.Add(entity);
            }
        }

        float distance = 0;
        for (int i = 0; i < objectsToSpawn.Count; ++i)
        {
            if (i == 0)
            {
                GameObject go = Instantiate(entityList.GetCombatEntityObject(objectsToSpawn[i]), originalSpawnPos, quaternion);
                StartCoroutine(WaitForTwoFramesToStartPlane(go.GetComponent<PlaneEntity>()));
            }
            else if (i % 2 == 1)
            {
                distance += spawnDistanceOffset;
                GameObject go = Instantiate(entityList.GetCombatEntityObject(objectsToSpawn[i]), originalSpawnPos + new Vector3(distance, 0f, 0f), quaternion);
                StartCoroutine(WaitForTwoFramesToStartPlane(go.GetComponent<PlaneEntity>()));
            }
            else
            {
                GameObject go = Instantiate(entityList.GetCombatEntityObject(objectsToSpawn[i]), originalSpawnPos + new Vector3(-distance, 0f, 0f), quaternion);
                StartCoroutine(WaitForTwoFramesToStartPlane(go.GetComponent<PlaneEntity>()));
            }
        }
    }

    IEnumerator WaitForTwoFramesToStartPlane(PlaneEntity planeEntity)
    {
        for (int i = 0; i < 2; ++i)
            yield return new WaitForEndOfFrame();
        planeEntity.StartFlyingInstantly();
    }

    /// <summary>
    /// Tells the GameplayManager to end match with the winner.
    /// </summary>
    /// <param name="team">(TEAM_TYPE) The Winning Team</param>
    public void EndMatch(TEAM_TYPE winningTeam)
    {
        switch (winningTeam)
        {
            case TEAM_TYPE.DEFENDERS:
                {
                    gameplayPhase = GAMEPLAY_PHASE.END_WINNER_DEFENDERS;
                    break;
                }
            case TEAM_TYPE.INVADERS:
                {
                    gameplayPhase = GAMEPLAY_PHASE.END_WINNER_INVADERS;
                    break;
                }
            default:
                {
                    Debug.LogError("GameplayManager: EndMatch was called but the winner is an unknown team.");
                    break;
                }
        }
    }



    #region INTRO_FUNCTIONS
    /// <summary>
    /// Call this function only when all players have loaded to start the intro.
    /// </summary>
    void StartIntro()
    {
        HandleCameraSpawns();
        gameplayPhase = GAMEPLAY_PHASE.INTRO;
        spaceshipEntity.StartFlyIn();
    }

    /// <summary>
    ///  Handle movement of cameras as "Spawn Locations".
    /// </summary>
    void HandleCameraSpawns()
    {
        switch (PlayerManager.instance.playerTeam)
        {
            case TEAM_TYPE.DEFENDERS:
                {
                    PlayerManager.instance.freeRoamCamera.transform.position = defendersSpawnLocation.position;
                    break;
                }
            case TEAM_TYPE.INVADERS:
                { 
                    PlayerManager.instance.freeRoamCamera.transform.position = invadersSpawnLocation.position;
                    break;
                }
            default:
                {
                    Debug.LogError("GameplayManager.HandleCameraSpawns(): Player was assigned an invalid team type.");
                    break;
                }

        }
    }
    #endregion


    #region PUBLIC_GETTERS
    /// <summary>
    /// Gets the dock entity.
    /// </summary>
    /// <returns>Returns BaseEntity of the dock.</returns>
    public BaseEntity GetDockEntity()
    {
        return dockEntity.baseEntity;
    }


    /// <summary>
    /// Gets the spaceship entity.
    /// </summary>
    /// <returns>Returns BaseEntity of the spaceship</returns>
    public BaseEntity GetSpaceshipEntity()
    {
        return spaceshipEntity.baseEntity;
    }

    public EntityHealth GetRandomSpaceshipPart()
    {
        EntityHealth[] arrayOfDestructibles = spaceshipEntity.GetComponentsInChildren<EntityHealth>();
        int random = UnityEngine.Random.Range(0, arrayOfDestructibles.Length - 1);
        return arrayOfDestructibles[random];
    }

    public EntityHealth GetRandomCoreDockComponent()
    {
        EntityCore[] arrayOfDestructibles = dockEntity.GetComponentsInChildren<EntityCore>();
        int random = UnityEngine.Random.Range(0, arrayOfDestructibles.Length - 1);
        return arrayOfDestructibles[random].GetComponent<EntityHealth>();
    }
    #endregion
}
