using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be added to parts that can explode
/// </summary>
public class EntityExplosion : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    GameObject explosionVFXPrefab;

    [Header("Settings")]
    [SerializeField]
    float explosionRadius;

    GameObject explosionVFX;

    public void Ignite(Vector3 position)
    {
        explosionVFX = Instantiate(explosionVFXPrefab);
        explosionVFX.transform.position = position;
        explosionVFX.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
        explosionVFX.SetActive(true);
        DestroyAfterSeconds destroyScript = explosionVFX.AddComponent<DestroyAfterSeconds>();
        destroyScript.DestroyAfterWaiting(5);
    }


}
