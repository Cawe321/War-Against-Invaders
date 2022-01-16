using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// The default weapon class that shoots projectiles.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class EntityGun : EntityWeapon
{
    [SerializeField]
    float projectileSpread = 0.1f;

 

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
            GameObject newProjectile = null;
            if (originalObject.GetComponent<PhotonView>() == null)
                newProjectile = Instantiate(originalObject, transform.position, Quaternion.identity);
            else if (isMine)
                newProjectile = PhotonNetwork.Instantiate(originalObject.name, transform.position, Quaternion.identity);

            if (newProjectile == null)
                return;

            newProjectile.transform.position = transform.position;
            EntityProjectile entityProjectile = newProjectile.GetComponent<EntityProjectile>();
            entityProjectile.owner = parent;
            entityProjectile.finalDamage = defaultDamage * (1 + parent.dmgIncrease);
            entityProjectile.transform.forward = transform.forward;
            //Vector3 rotationEuler = new Vector3(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), 0f);
            //entityProjectile.transform.Rotate(rotationEuler);
            entityProjectile.ActivateProjectile(this);

            //weaponAudio.Play();
        }
    }


    protected override GameObject GetAvailableObject()
    {
        // We are not using this function
        return null;
    }
}
