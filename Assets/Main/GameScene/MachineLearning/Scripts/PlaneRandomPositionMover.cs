using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for training of turret AI
/// </summary>
[RequireComponent(typeof(PlaneEntity))]
public class PlaneRandomPositionMover : MonoBehaviour
{
    public Transform originPosition;
    public float flightRadius = 250;

    public float maxRandomSpeed = 100;
    public float minRandomSpeed = 20;

    PlaneEntity planeEntity;


    public Vector3 targetPos;

    EntityHealth[] allEntityHealth;

    bool loaded = false;
    // Start is called before the first frame update
    void Start()
    {
        planeEntity = GetComponent<PlaneEntity>();
        StartCoroutine(WaitForTwoFrames());
        loaded = false;
    }

    IEnumerator WaitForTwoFrames()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        planeEntity.StartFlyingInstantly();
        allEntityHealth = planeEntity.GetComponents<EntityHealth>();
        ChangeNewTarget();
        loaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!loaded)
            return;
        if (Vector3.Distance(transform.position, targetPos) < 10)
        {
            ChangeNewTarget();
        }
        planeEntity.baseEntity.ReloadFuel();

        foreach (EntityHealth entityHealth in allEntityHealth)
            entityHealth.currHealth = entityHealth.maxHealth;

        planeEntity.RotateToTargetPosition(targetPos);
    }

    void ChangeNewTarget()
    {
        // Find new position to fly to
        targetPos = new Vector3(originPosition.position.x + Random.Range(-flightRadius, flightRadius + 1), originPosition.position.y + Random.Range(100, flightRadius + 1), originPosition.position.z + Random.Range(-flightRadius, flightRadius + 1));
        planeEntity.flightSpeed = Random.Range(minRandomSpeed, maxRandomSpeed+1);
    }
}
