using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaneUIManager : SingletonObject<PlaneUIManager>
{
    [Header("References")]
    [SerializeField]
    GameObject UIContainer;

    [SerializeField]
    Camera mirrorCamera;

    [SerializeField]
    Camera radarCamera;

    [SerializeField]
    RectTransform LWingHealthBar;

    [SerializeField]
    RectTransform RWingHealthBar;

    [SerializeField]
    RectTransform BodyHealthBar;

    [SerializeField]
    RectTransform fuelBar;

    [SerializeField]
    TextMeshProUGUI fuelText;

    [SerializeField]
    RectTransform primaryWeaponBar;

    [SerializeField]
    TextMeshProUGUI primaryWeaponText;

    [SerializeField]
    RectTransform secondaryWeaponBar;

    [SerializeField]
    TextMeshProUGUI secondaryWeaponText;

    [SerializeField]
    Image engineBar;

    [SerializeField]
    TextMeshProUGUI engineText;

    [SerializeField]
    RectTransform speedBar;

    [SerializeField]
    TextMeshProUGUI takeOffSpeedText;

    [SerializeField]
    TextMeshProUGUI speedText;

    [HideInInspector]
    public PlaneEntity planeEntity;

    public void EnableUI(PlaneEntity planeEntity)
    {
        UIContainer.SetActive(true);
        this.planeEntity = planeEntity;
        mirrorCamera.gameObject.SetActive(true);
        MainHUDManager.instance.ToggleSpawnWaveUI(false);
    }

    public void DisableUI()
    {
        UIContainer.SetActive(false);
        planeEntity = null;
        MainHUDManager.instance.ToggleSpawnWaveUI(true);
    }


    // Update is called once per frame
    void Update()
    {
        if (planeEntity != null)
        {
            // Mirror View
            mirrorCamera.transform.position = planeEntity.mirrorCameraPosition.transform.position;
            mirrorCamera.transform.rotation = planeEntity.mirrorCameraPosition.transform.rotation;

            // Radar View
            radarCamera.transform.position = planeEntity.radarCameraPosition.transform.position;
            radarCamera.transform.rotation = planeEntity.radarCameraPosition.transform.rotation;
            radarCamera.transform.LookAt(planeEntity.transform, planeEntity.transform.up);
            //radarCamera.transform.rotation = planeEntity.radarCameraPosition.transform.rotation;

            // Health Bars
            LWingHealthBar.localScale = new Vector3(planeEntity.LWing.currHealth/planeEntity.LWing.maxHealth, LWingHealthBar.localScale.y, LWingHealthBar.localScale.z);
            RWingHealthBar.localScale = new Vector3(planeEntity.RWing.currHealth/planeEntity.RWing.maxHealth, RWingHealthBar.localScale.y, RWingHealthBar.localScale.z);
            EntityHealth bodyHealth = planeEntity.GetComponent<EntityHealth>();
            BodyHealthBar.localScale = new Vector3(bodyHealth.currHealth/bodyHealth.maxHealth, BodyHealthBar.localScale.y, BodyHealthBar.localScale.z);

            // Fuel Bar
            fuelBar.localScale = new Vector3(planeEntity.baseEntity.currFuel / planeEntity.baseEntity.maxFuel, fuelBar.localScale.y, fuelBar.localScale.z);
            fuelText.text = "Fuel: " + (int)planeEntity.baseEntity.currFuel + "/" + (int)planeEntity.baseEntity.maxFuel;

            // Ammo Bar
            EntityWeapon primaryWeapon = planeEntity.baseEntity.GetFirstWeaponOfType(EntityWeapon.WEAPON_TYPE.PRIMARY);
            EntityWeapon secondaryWeapon = planeEntity.baseEntity.GetFirstWeaponOfType(EntityWeapon.WEAPON_TYPE.SECONDARY);
            primaryWeaponBar.localScale = new Vector3(primaryWeapon.currAmmunition / primaryWeapon.maxAmmunition, primaryWeaponBar.localScale.y, primaryWeaponBar.localScale.z);
            primaryWeaponText.text = primaryWeapon.currAmmunition + "/" + primaryWeapon.maxAmmunition;
            if (secondaryWeapon != null)
            {
                secondaryWeaponBar.localScale = new Vector3(secondaryWeapon.currAmmunition / secondaryWeapon.maxAmmunition, secondaryWeaponBar.localScale.y, secondaryWeaponBar.localScale.z);
                secondaryWeaponText.text = secondaryWeapon.currAmmunition + "/" + secondaryWeapon.maxAmmunition;
            }
            else
            {
                secondaryWeaponBar.localScale = new Vector3(1f, secondaryWeaponBar.localScale.y, secondaryWeaponBar.localScale.z);
                secondaryWeaponText.text = "0/0";
            }

            // Engine
            if (planeEntity.engineActive)
            {
                engineText.text = "Engine: ON";
                engineBar.color = Color.green;
            }
            else
            {
                engineText.text = "Engine: OFF";
                engineBar.color = Color.red;
            }

            // Speed Meter
            speedBar.localScale = new Vector3(speedBar.localScale.x, planeEntity.flightSpeed / planeEntity.flightMaxSpeed, speedBar.localScale.z);
            speedText.text = "Speed: " + (int)planeEntity.flightSpeed + "/" + (int)planeEntity.flightMaxSpeed;
            takeOffSpeedText.text = "Take Off Speed: " + (int)planeEntity.flightMinTakeOffSpeed;
        }

    }
}
