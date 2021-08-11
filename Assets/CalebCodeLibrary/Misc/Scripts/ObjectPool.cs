using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script that helps to identify an object pool(The parent of all objects that are in the pool).
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectPool<T> : MonoBehaviour
{
    #region SETTING_VARIABLES
    [Header("Settings")]
    [Tooltip("Used to help identify the type of object pool this is.")]
    public string objectType;

    [Tooltip("The object that the script will create and add into the pool whenever requested.")]
    [SerializeField] 
    GameObject originalObject;

    [Tooltip("The number of copies of the original object the script will create at start.")]
    [SerializeField]
    int defaultNumberOfObjects;
    #endregion

}
