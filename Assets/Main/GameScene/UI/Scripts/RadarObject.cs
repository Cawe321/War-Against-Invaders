using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    Mesh arrowMesh;
    [SerializeField]
    Mesh sphereMesh;
    [SerializeField]
    Material allyMaterial;
    [SerializeField]
    Material enemyMaterial;
    [SerializeField]
    Material selfMaterial;
    [SerializeField]
    float maxDistance = 250f;

    BaseEntity baseEntity;

    MeshFilter meshFilter;
    Renderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        baseEntity = GetComponentInParent<BaseEntity>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (baseEntity.isLocalPlayerControlling)
        {
            meshFilter.mesh = arrowMesh;
            meshRenderer.material = selfMaterial;
        }
        if (baseEntity.team == PlayerManager.instance.playerTeam)
        {
            meshFilter.mesh = sphereMesh;
            meshRenderer.material = allyMaterial;
        }
        else
        {
            meshFilter.mesh = sphereMesh;
            meshRenderer.material = enemyMaterial;
        }
    }

    private void Update()
    {
        if (PlaneUIManager.instance.planeEntity != null && (PlaneUIManager.instance.planeEntity.transform.position - transform.position).sqrMagnitude < maxDistance * maxDistance)
            meshRenderer.enabled = true;
        else
            meshRenderer.enabled = false;
    }
}
