using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Add this script to a collider gameobject so that another gameobject can access
/// </summary>
[RequireComponent(typeof(Collider))]
public class ColliderExtender : MonoBehaviour
{
    public Dictionary<GameObject, Collision> collidedObjects;
    public List<Collider> triggeredObjects;

    public OnNewCollision onNewCollision;

    public OnEndCollision onEndCollision;
    
    public OnNewTrigger onNewTrigger;

    public OnEndTrigger onEndTrigger;

    private void Awake()
    {
        onNewCollision = new OnNewCollision();
        onEndCollision = new OnEndCollision();

        onNewTrigger = new OnNewTrigger();
        onEndTrigger = new OnEndTrigger();

        collidedObjects = new Dictionary<GameObject, Collision>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        onNewCollision.Invoke(collision);
        if (!collidedObjects.ContainsKey(collision.gameObject))
            collidedObjects.Add(collision.gameObject, collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        onEndCollision.Invoke(collision);
        if (collidedObjects.ContainsKey(collision.gameObject))
            collidedObjects.Remove(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        onNewTrigger.Invoke(other);
        if (!triggeredObjects.Contains(other))
            triggeredObjects.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onEndTrigger.Invoke(other);
        if (triggeredObjects.Contains(other))
            triggeredObjects.Remove(other);
    }
}


public class OnNewCollision : UnityEvent<Collision> { }
public class OnEndCollision : UnityEvent<Collision> { }
public class OnNewTrigger : UnityEvent<Collider> { }
public class OnEndTrigger : UnityEvent<Collider> { }



