using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EntityList", menuName = "EntityList", order = 1)]
public class EntityList : ScriptableObject
{
    [SerializeField]
    List<EntityObject> entityObjects;

    public List<EntityObject> GetAllEntities()
    {
        return entityObjects;
    }
}

[System.Serializable]
public class EntityObject
{
    [Header("Settings")]
    [Tooltip("Name Of Entity")]
    public string entityName = "NamelessEntity";

    [Tooltip("What entity this is.")]
    public EntityTypes entityType;

    [Tooltip("Description of Entity")]
    [TextArea]
    public string entityDescription = "Nothing to say here.";

    [Tooltip("Team of Entity")]
    public TEAM_TYPE entityTeam;

    [Tooltip("Icon of Entity")]
    public Image entityIcon;

    [Header("References")]
    [Tooltip("The prefab to be used in game.")]
    public GameObject gameModel;

    [Tooltip("The prefab model to be used for display.")]
    public GameObject displayModel;
}
