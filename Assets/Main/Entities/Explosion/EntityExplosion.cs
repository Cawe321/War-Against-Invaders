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
    public float damage;


    GameObject explosionVFX;

    public void Ignite(Vector3 position)
    {
        explosionVFX = Instantiate(explosionVFXPrefab);
        explosionVFX.transform.position = position;
        explosionVFX.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
        explosionVFX.SetActive(true);
        DestroyAfterSeconds destroyScript = explosionVFX.AddComponent<DestroyAfterSeconds>();
        destroyScript.DestroyAfterWaiting(5);
        AreaDamage();
    }

    public void AreaDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in hitColliders)
        {
            // Make an attempt to find EntityHealth component
            EntityHealth entityHealth = collider.transform.GetComponent<EntityHealth>();
            if (entityHealth != null)
            {
                Vector3 dist = entityHealth.transform.position - transform.position;
                // Calculate explosion force
                Vector3 explosionForce = (explosionRadius - dist.magnitude) * dist * explosionRadius *1000f;
                entityHealth.TakeDamage(damage, explosionForce);
            }
        }
    }
}
