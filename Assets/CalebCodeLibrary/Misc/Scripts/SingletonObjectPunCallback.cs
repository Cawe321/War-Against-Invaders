using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingletonObjectPunCallback<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
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