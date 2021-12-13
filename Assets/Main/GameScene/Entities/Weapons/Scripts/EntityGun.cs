using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;

/// <summary>
/// The default weapon class that shoots projectiles.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class EntityGun : EntityWeapon
{
    [SerializeField]
    float projectileSpread = 0.1f;

    AudioSource weaponAudio;

    protected void Start()
    {
        base.Start();
        weaponAudio = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (currWeaponCooldown > 0f)
        {
            currWeaponCooldown -= Time.fixedDeltaTime;
        }
    }

    public override void FireWeapon(BaseEntity parent)
    {
        if (currWeaponCooldown <= 0f && currAmmunition > 0)
        {
            --currAmmunition;
            //owner = parent;
            currWeaponCooldown = weaponCooldown;
            GameObject newProjectile = GetAvailableObject();
            newProjectile.transform.position = transform.position;
            EntityProjectile entityProjectile = newProjectile.GetComponent<EntityProjectile>();
            entityProjectile.owner = parent;
            entityProjectile.finalDamage = defaultDamage * (1 + parent.dmgIncrease);
            entityProjectile.transform.forward = transform.forward;
            Vector3 rotationEuler = new Vector3(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), 0f);
            entityProjectile.transform.Rotate(rotationEuler);
            entityProjectile.ActivateProjectile(this);

            weaponAudio.Play();
        }
    }


    protected override GameObject GetAvailableObject()
    {
        foreach(Transform projectile in objectContainer)
        {
            if (!projectile.gameObject.activeInHierarchy)
            {
                projectile.gameObject.SetActive(true);
                return projectile.gameObject;
            }
        }

        // If code runs here, means all projectiles are inavailable. Time to instantiate a new one.
        GameObject go = Instantiate(originalObject, objectContainer);
        go.SetActive(true);
        return go;
    }
}
