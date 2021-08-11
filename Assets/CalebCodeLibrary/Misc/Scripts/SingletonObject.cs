using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton template inherited from <see cref="MonoBehaviour"/> to make game objects follow the singleton design pattern.
/// </summary>
public class SingletonObject<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance { get { return _instance; } }
    private static T _instance;

    public virtual void Awake()
    {
        SingletonAwake();   
    }

    public void SingletonAwake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this as T;
        }
    }
}
