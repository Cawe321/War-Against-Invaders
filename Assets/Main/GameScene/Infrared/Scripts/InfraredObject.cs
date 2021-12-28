using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfraredObject : MonoBehaviour
{
    InfraredManager infraredManager;

    BaseEntity baseEntity;

    MeshRenderer meshRenderer;

    [HideInInspector]
    public bool active = false;
    public void ActivateInfrared(InfraredManager manager, BaseEntity baseEntity)
    {
        infraredManager = manager;
        this.baseEntity = baseEntity;

        if (baseEntity.team == PlayerManager.instance.playerTeam)
            meshRenderer.material = infraredManager.allyIR;
        else
            meshRenderer.material = infraredManager.enemyIR;
        active = true;
    }

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseEntity == null || !baseEntity.CheckHealth()) // entity that it is tracking is gone
        {
            active = false;
            gameObject.SetActive(false);
        }
        else
        {
            if (infraredManager.infraActive)
            {
                if ((baseEntity.transform.position - infraredManager.infraredCamera.transform.position).sqrMagnitude >= infraredManager.minActiveDistance * infraredManager.minActiveDistance)
                {
                    meshRenderer.enabled = true;
                    //Vector3 screenPosition = infraredManager.infraredCamera.WorldToScreenPoint(baseEntity.transform.position);
                    Vector3 screenPosition = baseEntity.transform.position;
                    transform.position = screenPosition;
                }
                else
                    meshRenderer.enabled = false;
            }
            else
                meshRenderer.enabled = false;
            
        }
    }
}
