using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script that helps to identify an object pool(The parent of all objects that are in the pool).
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ObjectPool<T> : MonoBehaviour
{
    #region SETTING_VARIABLES
    [Header("Settings")]
    [SerializeField]
    [Tooltip("Used to help identify the type of object pool this is.")]
    protected Transform objectContainer;

    [Tooltip("The object that the script will create and add into the pool whenever requested.")]
    [SerializeField] 
    protected GameObject originalObject;

    [Tooltip("The number of copies of the original object the script will create at start.")]
    [SerializeField]
    int defaultNumberOfObjects;
    #endregion

    protected void Start()
    {
        for (int i = 0; i < defaultNumberOfObjects; ++i)
            Instantiate(originalObject, objectContainer).SetActive(false);
    }

    protected abstract GameObject GetAvailableObject();
}
