using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TurretGunShootingAgent : Agent
{
    public bool isTraining = true;
    public Transform target;
    public Rigidbody targetRB;
    public Transform aim;
    [HideInInspector]
    public TurretEntity turretEntity;
    [HideInInspector]
    public int hitCounter = 0;

    private void Awake()
    {
        turretEntity = GetComponent<TurretEntity>();     
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        UpdateRotation(-actions.ContinuousActions[0], actions.ContinuousActions[1]);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(transform.position); // Add its own position
        //sensor.AddObservation(target.position); // Add target's position
        sensor.AddObservation(aim.InverseTransformDirection((target.position - transform.position).normalized)); // Add direction vector
        sensor.AddObservation(Vector3.Distance(target.position, transform.position));
        //sensor.AddObservation(Vector3.Distance(transform.position, target.position)); // Add distance
        sensor.AddObservation(targetRB.velocity); // Add target's velocity
    }

    public override void OnEpisodeBegin()
    {
        hitCounter = 0;
    }


    public void UpdateRotation(float valueX, float valueY)
    {
        if (isTraining)
        {
            Debug.Log("RotateX: " + valueX);
            Debug.Log("RotateY: " + valueY);
        }
        turretEntity.UpdateRotation(valueX, valueY);
    }


}
