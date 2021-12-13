using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Unity.Mathematics;
using UnityEngine;

public class GameplayManager : SingletonObject<GameplayManager>
{
    [Header("UI References")]
    [SerializeField]
    MatchSummaryManager matchSummaryManager;

    [Header("References")]

    public DockEntity dockEntity;
    
    public SpaceshipEntity spaceshipEntity;

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
    [SerializeField]
    float spawnDistanceOffset = 50f;

    /* In-script Values */
    EntityList entityList;

    [HideInInspector]
    public float defenderSpawnCooldown;
    [HideInInspector]
    public float invaderSpawnCooldown;

    /// <summary>
    /// List of entities to spawn for defenders
    /// </summary>
    public Dictionary<EntityTypes, int> defenderSpawnWave;
    [HideInInspector]
    public float defenderSpawnCooldownMultiplier = 1f;

    /// <summary>
    /// List of entities to spawn for invaders
    /// </summary>
    public Dictionary<EntityTypes, int> invaderSpawnWave;
    [HideInInspector]
    public float invaderSpawnCooldownMultiplier = 1f;

    float carePackageCooldownCounter;
    int carePackageAmount = 500;

    public float gameTimer = 0f;
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

        entityList = ResourceReference.instance.entityList;

        carePackageAmount = ResourceReference.instance.currencySettings.carePackageAmount;

        carePackageCooldownCounter = ResourceReference.instance.currencySettings.carePackageCooldown;

        waitForPlayersCanvas.gameObject.SetActive(true);

        gameplayPhase = GAMEPLAY_PHASE.WAIT;

        defenderSpawnWave = new Dictionary<EntityTypes, int>();
        AddToSpawnWave(TEAM_TYPE.DEFENDERS, EntityTypes.F16, 3);
        AddToSpawnWave(TEAM_TYPE.DEFENDERS, EntityTypes.StealthWing, 2);
        //defenderSpawnWave.Add(EntityTypes.F16, 3);
        //defenderSpawnWave.Add(EntityTypes.StealthWing, 2);

        invaderSpawnWave = new Dictionary<EntityTypes, int>();
        AddToSpawnWave(TEAM_TYPE.INVADERS, EntityTypes.Deathrow, 3);
        AddToSpawnWave(TEAM_TYPE.INVADERS, EntityTypes.Mako, 2);
        //invaderSpawnWave.Add(EntityTypes.Deathrow, 3);
        //invaderSpawnWave.Add(EntityTypes.Mako, 2);

        gameTimer = 0f;
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

    void FixedUpdate()
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
                        NotificationManager.instance.AddToNotification("Welcome To War!", "Good luck out there!");

                        gameplayPhase = GAMEPLAY_PHASE.GAME;
                        SpawnTheEntityWave(TEAM_TYPE.DEFENDERS);
                        SpawnTheEntityWave(TEAM_TYPE.INVADERS);
                        defenderSpawnCooldown = originalSpawnCooldown;
                        invaderSpawnCooldown = originalSpawnCooldown;

                        AudioManager.instance.PlayBGM(AudioManager.instance.audioFiles._gameSceneBGM);
                    }
                    break;
                }
            case GAMEPLAY_PHASE.GAME:
                {
                    // Timer
                    gameTimer += Time.deltaTime;

                    // Spawn Wave Logic
                    // Each warehouse destroyed will result in a 20% increase of the original cooldown time
                    // Base 40% cooldown for defender cooldown multiplier
                    defenderSpawnCooldownMultiplier = 0.4f + dockEntity.GetPercentageOfExistingWarehouses() * 0.6f;

                    defenderSpawnCooldown -= Time.fixedDeltaTime * defenderSpawnCooldownMultiplier;
                    invaderSpawnCooldown -= Time.fixedDeltaTime * invaderSpawnCooldownMultiplier;
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

                    // Care Package Logic
                    carePackageCooldownCounter -= Time.fixedDeltaTime;
                    if (carePackageCooldownCounter < 0f)
                    {
                        carePackageCooldownCounter = ResourceReference.instance.currencySettings.carePackageCooldown;
                        PlayerManager.instance.AddCoins(carePackageAmount, "Care Package has just arrived!");
                        EnemyAIBehaviour.instance.AddCoins(carePackageAmount);
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
        List<string> entitiesString = new List<string>();
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
                entitiesString.Add("+ " + defenderSpawnWave[entity] + " " + entity.ToString());
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
                entitiesString.Add("> " + invaderSpawnWave[entity] + " " + entity.ToString());
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

        
        AnnounceToPlayerSpawnWave(team , entitiesString);
    }

    void AnnounceToPlayerSpawnWave(TEAM_TYPE team, List<string>entitiesString)
    {
        if (PlayerManager.instance.playerTeam == team)
            NotificationManager.instance.AddToNotification("Reinforcements have arrived!", entitiesString);
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
        if (gameplayPhase != GAMEPLAY_PHASE.GAME)
            return;
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

        if (PlayerManager.instance.playerTeam == winningTeam)
        {
            // Win
            DataManager.instance.AddCommonCurrency(ResourceReference.instance.currencySettings.victoryCommonCurrencyReward);
            DataManager.instance.AddPremiumCurrency(ResourceReference.instance.currencySettings.victoryPremiumCurrencyReward);
        }
        else
        {
            // Lose
            DataManager.instance.AddCommonCurrency(ResourceReference.instance.currencySettings.defeatCommonCurrencyReward);
            DataManager.instance.AddPremiumCurrency(ResourceReference.instance.currencySettings.defeatPremiumCurrencyReward);
        }

        // Calls for UI to appear
        matchSummaryManager.gameObject.SetActive(true);
        matchSummaryManager.ActivateUI(PlayerManager.instance.playerTeam == winningTeam);
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
