using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

[RequireComponent(typeof(TurretGunShootingAgent))]
public class TurretAI : MonoBehaviour
{
    TurretGunShootingAgent agent;

    BaseEntity targetEntity = null;

    public float enemyCheckerCooldown = 1f;
    float enemyCheckerCooldownDebounce;

    private void Awake()
    {
        agent = GetComponent<TurretGunShootingAgent>();
        enemyCheckerCooldownDebounce = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (agent.target != null)
            targetEntity = agent.target.GetComponent<BaseEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.turretEntity.baseEntity.isAnyPlayerControlling)
        {
            return; // dont update since player is controlling the turret
        }

        if (!PhotonNetwork.IsMasterClient) // Dont update if not the master client
            return;

        if (agent.enabled && agent.target != null && (targetEntity != null && targetEntity.CheckHealth()))
        {
            if ((transform.position - agent.target.position).sqrMagnitude < 500 * 500)
                agent.turretEntity.FireAllWeapons(EntityWeapon.WEAPON_TYPE.PRIMARY);
            agent.RequestDecision();
            agent.RequestAction();
            //Debug.Log("Step:" + agent.StepCount);
        }
        else
        {
            if (enemyCheckerCooldownDebounce < 0)
            {
                enemyCheckerCooldownDebounce = enemyCheckerCooldown;
                Transform entityContainer = null;
                if (GameplayManager.instance != null) // Gameplay mode, not training
                {
                    if (agent.turretEntity.baseEntity.team == TEAM_TYPE.DEFENDERS)
                        entityContainer = GameplayManager.instance.invaderPlaneContainer.transform; // check for enemy planes
                    else if (agent.turretEntity.baseEntity.team == TEAM_TYPE.INVADERS)
                        entityContainer = GameplayManager.instance.defenderPlaneContainer.transform; // check for enemy planes

                    foreach (Transform enemyPlane in entityContainer)
                    {
                        if ((enemyPlane.position - transform.position).sqrMagnitude < 500 * 500)
                        {
                            targetEntity = enemyPlane.GetComponent<BaseEntity>();
                            agent.target = targetEntity.transform;
                            agent.targetRB = agent.target.GetComponent<Rigidbody>();
                            agent.enabled = true;
                            return;
                        }
                    }
                }
                else
                {
                    Collider[] colliders = Physics.OverlapSphere(transform.position, 500f); // Check if enemy plane is nearby
                    if (colliders.Length > 0)
                    {
                        foreach (Collider collider in colliders)
                        {
                            EntityHealth entityHealth = collider.GetComponent<EntityHealth>();
                            if (entityHealth != null && entityHealth.baseEntity.GetComponent<PlaneEntity>() != null)
                            {
                                // Enemy plane has been detected
                                targetEntity = entityHealth.baseEntity;
                                agent.target = targetEntity.transform;
                                agent.targetRB = agent.target.GetComponent<Rigidbody>();
                                agent.enabled = true;
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                enemyCheckerCooldownDebounce -= Time.deltaTime;
            }

          
        }


    }
}
