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

    [HideInInspector]
    public BaseEntity owner;

    [Header("Settings")]
    public float explosionRadius;
    public float damage;


    GameObject explosionVFX;

    [HideInInspector]
    public bool isClientMine = false;

    public void Ignite(Vector3 position)
    {
        explosionVFX = Instantiate(explosionVFXPrefab);
        explosionVFX.transform.position = position;
        explosionVFX.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
        explosionVFX.SetActive(true);
        explosionVFX.GetComponent<AudioSource>().Play();
        DestroyAfterSeconds destroyScript = explosionVFX.AddComponent<DestroyAfterSeconds>();
        destroyScript.DestroyAfterWaiting(5);
        AreaDamage();
    }

    public void AreaDamage()
    {
        if (isClientMine) // only handle stuff when the client owns this explosion
        {
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider collider in hitColliders)
            {
                Debug.Log("hitting " + collider.name);
                // Make an attempt to find EntityHealth component
                EntityHealth entityHealth = collider.transform.GetComponent<EntityHealth>();
                if (entityHealth != null)
                {
                    
                    Vector3 dist = collider.ClosestPoint(transform.position) - transform.position;

                    // Calculate explosion force
                    Vector3 explosionForce = (explosionRadius - dist.magnitude) * dist * explosionRadius;
                    entityHealth.TakeDamage(owner, damage, explosionForce);
                }
            }
        }
        
    }
}
