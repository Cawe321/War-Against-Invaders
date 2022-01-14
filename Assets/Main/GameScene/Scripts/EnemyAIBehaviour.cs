using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script should only be activated when the player is alone in the match. It will simulate a player in the opponent team
/// </summary>
public class EnemyAIBehaviour : SingletonObject<EnemyAIBehaviour>
{
    public bool isActive = false;

    /* In Script Values*/
    [HideInInspector]
    public int coinBank = 0;

    [HideInInspector]
    public int numberOfSimulatedPlayers;

    int decisionID;

    TEAM_TYPE team;
    
    public void AddCoins(int coins)
    {
        if (isActive) // only need to add to coin back if AI is active
            coinBank += coinBank * numberOfSimulatedPlayers;
    }



    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        coinBank = 0;   
    }

    // Update is called once per frame
    void Update()
    {
        // CODE HERE - Update only if Master Client only
        // missing code...

        if (!isActive) // no need to check whether to activate once it's activated
        {
            Player[] playerList = PhotonNetwork.PlayerList;
            bool isInvaderEmpty = true;
            bool isDefenderEmpty = true;
            foreach (Player p in playerList)
            {
                object playerTeamObject;
                if (p.CustomProperties.TryGetValue(MatchmakingKeyIDs.PLAYER_TEAM, out playerTeamObject))
                {
                    TEAM_TYPE playerTeam = (TEAM_TYPE)playerTeamObject;
                    if (playerTeam == TEAM_TYPE.DEFENDERS)
                    {
                        isDefenderEmpty = false;
                    }
                    else if (playerTeam == TEAM_TYPE.INVADERS)
                    {
                        isInvaderEmpty = false;
                    }
                    if (!isDefenderEmpty && !isInvaderEmpty) // no need to continue checking, both teams are not empty
                    {
                        isActive = false;
                        break;
                    }
                }
            }
            if (isDefenderEmpty)
            {
                isActive = true;
                team = TEAM_TYPE.DEFENDERS;
            }
            else if (isInvaderEmpty)
            {
                isActive = true;
                team = TEAM_TYPE.INVADERS;
            }
            if (isActive)
                Debug.Log("Enemy AI Behaviour activated for" + team.ToString()); // This is for debugging purposes only ^_^
        }

       

        if (!isActive)
            return;
        switch (decisionID)
        {
            case 0:
                {
                    if (team == TEAM_TYPE.INVADERS)
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
                    if (team == TEAM_TYPE.INVADERS)
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

                    if (team == TEAM_TYPE.INVADERS)
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
