using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script should only be activated when the player is alone in the match. It will simulate a player in the opponent team
/// </summary>
public class EnemyAIBehaviour : SingletonObject<EnemyAIBehaviour>
{
    public bool isActive = true;

    /* In Script Values*/
    [HideInInspector]
    public int coinBank = 0;

    [HideInInspector]
    public int numberOfSimulatedPlayers;

    int decisionID;

    public void AddCoins(int coins)
    {
        if (isActive) // only need to add to coin back if AI is active
            coinBank += coinBank * numberOfSimulatedPlayers;
    }



    // Start is called before the first frame update
    void Start()
    {
        coinBank = 0;   
    }

    // Update is called once per frame
    void Update()
    {
        // CODE HERE - Update only if Master Client only

        if (!isActive)
            return;
        switch (decisionID)
        {
            case 0:
                {
                    if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
                    {
                        // AI is INVADERS
                        int cost = UpgradeManager.instance.GetPlanePurchaseCost(EntityTypes.Mako);
                        if (coinBank >= cost)
                        {
                            coinBank -= cost;
                            GameplayManager.instance.AddToSpawnWave(TEAM_TYPE.INVADERS, EntityTypes.Mako, 1);
                            MakeANewDecision();
                        }
                    }
                    else
                    {
                        // AI is DEFENDERS
                        int cost = UpgradeManager.instance.GetPlanePurchaseCost(EntityTypes.StealthWing);
                        if (coinBank >= cost)
                        {
                            coinBank -= cost;
                            GameplayManager.instance.AddToSpawnWave(TEAM_TYPE.DEFENDERS, EntityTypes.StealthWing, 1);
                            MakeANewDecision();
                        }
                    }
                    break;
                }
            case 1:
                {
                    if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
                    {
                        // AI is INVADERS
                        int cost = UpgradeManager.instance.GetPlanePurchaseCost(EntityTypes.X_Wing);
                        if (coinBank >= cost)
                        {
                            coinBank -= cost;
                            GameplayManager.instance.AddToSpawnWave(TEAM_TYPE.INVADERS, EntityTypes.X_Wing, 1);
                            MakeANewDecision();
                        }
                    }
                    else
                    {
                        // AI is DEFENDERS
                        int cost = UpgradeManager.instance.GetPlanePurchaseCost(EntityTypes.Whitebeard);
                        if (coinBank >= cost)
                        {
                            coinBank -= cost;
                            GameplayManager.instance.AddToSpawnWave(TEAM_TYPE.DEFENDERS, EntityTypes.Whitebeard, 1);
                            MakeANewDecision();
                        }
                    }
                    break;
                }
            case 2:
                {

                    if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
                    {
                        // AI is INVADERS
                        int cost = UpgradeManager.instance.GetPlanePurchaseCost(EntityTypes.Deathrow);
                        if (coinBank >= cost)
                        {
                            coinBank -= cost;
                            GameplayManager.instance.AddToSpawnWave(TEAM_TYPE.INVADERS, EntityTypes.Deathrow, 1);
                            MakeANewDecision();
                        }
                    }
                    else
                    {
                        // AI is DEFENDERS
                        int cost = UpgradeManager.instance.GetPlanePurchaseCost(EntityTypes.F16);
                        if (coinBank >= cost)
                        {
                            coinBank -= cost;
                            GameplayManager.instance.AddToSpawnWave(TEAM_TYPE.DEFENDERS, EntityTypes.F16, 1);
                            MakeANewDecision();
                        }
                    }
                    break;
                }
        }
    }

    void MakeANewDecision()
    {
        decisionID = Random.Range(0, 3); //0 - 2
    }
}
