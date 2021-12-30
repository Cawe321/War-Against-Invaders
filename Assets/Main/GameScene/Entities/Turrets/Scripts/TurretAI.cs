using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurretGunShootingAgent))]
public class TurretAI : MonoBehaviour
{
    TurretGunShootingAgent agent;
    private void Awake()
    {
        agent = GetComponent<TurretGunShootingAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - agent.target.position).sqrMagnitude < 500 * 500)
            agent.turretEntity.FireAllWeapons(EntityWeapon.WEAPON_TYPE.PRIMARY);
        agent.RequestDecision();
        agent.RequestAction();
        //Debug.Log("Step:" + agent.StepCount);
    }
}
