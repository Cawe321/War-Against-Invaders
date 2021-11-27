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
    float maxDistance = 500f;

    BaseEntity baseEntity;

    MeshFilter meshFilter;
    Renderer meshRenderer;

    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        baseEntity = GetComponentInParent<BaseEntity>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (baseEntity.isLocalPlayerControlling)
        {
            meshFilter.mesh = arrowMesh;
            meshRenderer.material = selfMaterial;
            sprite.enabled = true;
        }
        else if (baseEntity.team == PlayerManager.instance.playerTeam)
        {
            meshFilter.mesh = sphereMesh;
            meshRenderer.material = allyMaterial;
            sprite.enabled = false;
        }
        else
        {
            meshFilter.mesh = sphereMesh;
            meshRenderer.material = enemyMaterial;
            sprite.enabled = false;
        }

        if (PlaneUIManager.instance.planeEntity != null && (PlaneUIManager.instance.planeEntity.transform.position - transform.position).sqrMagnitude < maxDistance * maxDistance)
            meshRenderer.enabled = true;
        else
            meshRenderer.enabled = false;
    }
}
