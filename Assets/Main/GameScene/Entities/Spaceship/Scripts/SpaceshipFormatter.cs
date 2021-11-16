using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class SpaceshipFormatter : EditorWindow
{
    GameObject spaceshipModel;

    float defaultHealth = 100f;
    float stabilityScore = 1f;

    public GameObject[] fuelTanks;
    float fuelTankHealth;
    float fuelTankExplosionDmg;
    float fuelTankStabilityScore;
    float fuelTankExplosionRadius;
    
    public GameObject[] thrusters;
    float thrusterHealth;
    float thrusterStabilityScore;

    GameObject captainRoom;
    float captainRoomHealth;

    Vector2 scrollPos = Vector2.zero;

    [MenuItem("Spaceship/Spaceship Formatter")]
    public static void ShowWindow()
    {
        GetWindow<SpaceshipFormatter>("Spaceship Formatter");
    }

    private void OnGUI()
    {
        spaceshipModel = (GameObject)EditorGUILayout.ObjectField(spaceshipModel, typeof(GameObject), true);

        scrollPos = EditorGUILayout.BeginScrollView(Vector2.zero);
        
        EditorGUILayout.LabelField("The following settings for the easy destructible objects.");
        EditorGUILayout.LabelField("Default Health:");
        defaultHealth = EditorGUILayout.FloatField(defaultHealth);
        EditorGUILayout.LabelField("Stabilty Score:");
        stabilityScore = EditorGUILayout.FloatField(stabilityScore);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("The following settings are for core objects.");
        EditorGUILayout.LabelField("Captain Room:");
        captainRoom = (GameObject)EditorGUILayout.ObjectField(captainRoom, typeof(GameObject), true);
        EditorGUILayout.LabelField("Captain Room Health:");
        captainRoomHealth = EditorGUILayout.FloatField(captainRoomHealth);

        EditorGUILayout.LabelField("List of Fuel Tanks:");
        SerializedObject so = new SerializedObject(this);
        SerializedProperty fuelTankProperty = so.FindProperty("fuelTanks");
        EditorGUILayout.PropertyField(fuelTankProperty, true);
        EditorGUILayout.LabelField("Fuel Tank Health:");
        fuelTankHealth = EditorGUILayout.FloatField(fuelTankHealth);
        EditorGUILayout.LabelField("Fuel Tank Stability Score:");
        fuelTankStabilityScore = EditorGUILayout.FloatField(fuelTankStabilityScore);
        EditorGUILayout.LabelField("Fuel Tank Explosion Dmg:");
        fuelTankExplosionDmg = EditorGUILayout.FloatField(fuelTankExplosionDmg);
        EditorGUILayout.LabelField("Fuel Tank Explosion Radius:");
        fuelTankExplosionRadius = EditorGUILayout.FloatField(fuelTankExplosionRadius);

        EditorGUILayout.LabelField("List of Thrusters:");
        SerializedProperty thrusterProperty = so.FindProperty("thrusters");
        EditorGUILayout.PropertyField(thrusterProperty, true);

        EditorGUILayout.LabelField("Thruster Health:");
        thrusterHealth = EditorGUILayout.FloatField(thrusterHealth);
        EditorGUILayout.LabelField("Thruster Stability Score:");
        thrusterStabilityScore = EditorGUILayout.FloatField(thrusterStabilityScore);

        so.ApplyModifiedProperties(); // Remember to apply modified properties

        if (GUILayout.Button("Start"))
        {
            SetupGameObject();
        }

        EditorGUILayout.EndScrollView();
    }

    public void SetupGameObject()
    {
        if (spaceshipModel.GetComponent<BaseEntity>() == null)
        {
            spaceshipModel.AddComponent<BaseEntity>();
        }

        SpaceshipEntity spaceshipEntity = spaceshipModel.GetComponent<SpaceshipEntity>();
        if (spaceshipEntity == null)
            spaceshipEntity = spaceshipModel.AddComponent<SpaceshipEntity>();
        spaceshipEntity.fuelTanks = new List<GameObject>(fuelTanks);
        spaceshipEntity.thrusters = new List<GameObject>(thrusters);
       

        Transform[] transforms = spaceshipModel.GetComponentsInChildren<Transform>(); 
        foreach (Transform transform in transforms)
        {
            Collider collider = transform.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
                EntityHealth entityHealth = transform.GetComponent<EntityHealth>();
                if (entityHealth == null)
                    entityHealth = transform.gameObject.AddComponent<EntityHealth>();

                entityHealth.maxHealth = defaultHealth;
                entityHealth.stabilityScore = stabilityScore;  
            }
        }

        // For Captain Room
        {
            EntityHealth entityHealth = captainRoom.GetComponent<EntityHealth>();
            entityHealth.isCoreComponent = true;
            entityHealth.maxHealth = captainRoomHealth;
            entityHealth.destructionType = EntityHealth.DESTRUCTION_TYPE.DETACH;
        }

        foreach (GameObject fuelTank in fuelTanks)
        {
            EntityExplosion entityExplosion = fuelTank.GetComponent<EntityExplosion>();
            if (entityExplosion == null)
                entityExplosion = fuelTank.AddComponent<EntityExplosion>();
            EntityHealth entityHealth = fuelTank.GetComponent<EntityHealth>();
            entityHealth.destructionType = EntityHealth.DESTRUCTION_TYPE.EXPLODE;
            entityHealth.stabilityScore = fuelTankStabilityScore;
            entityExplosion.damage = fuelTankExplosionDmg;
            entityExplosion.explosionRadius = fuelTankExplosionRadius;
        }

        foreach (GameObject thruster in thrusters)
        {
            EntityHealth entityHealth = thruster.GetComponent<EntityHealth>();
            entityHealth.maxHealth = thrusterHealth;
            entityHealth.stabilityScore = thrusterStabilityScore;
        }
    }
}
