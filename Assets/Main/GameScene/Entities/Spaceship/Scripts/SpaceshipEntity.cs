using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

[RequireComponent(typeof(BaseEntity))]
[RequireComponent(typeof(Rigidbody))]
public class SpaceshipEntity : MonoBehaviour
{
    [Header("References")]
    public List<GameObject> fuelTanks;
    public List<GameObject> thrusters;
    int originalNumberOfThrusters;

    [Header("Flying Settings")]
    [SerializeField]
    [Tooltip("Starting transform of the spaceship")]
    Transform startTransform;

    [SerializeField]
    [Tooltip("The transform the spaceship is aiming for when flying. It's Y position of the transform will be the flight height position of the spaceship.")]
    Transform targetFlyInTransform;

    [SerializeField]
    [Tooltip("The speed of the spaceship flying in. Not the moving forward speed.")]
    [Range(0f, 1f)]
    float flyMovementSpeed;

    [Header("Moving Settings")]
    [SerializeField]
    [Tooltip("Destination of the spaceship. Y coord position will be ignored.")]
    Transform destination;

    [SerializeField]
    [Tooltip("Speed of the spaceship moving")]
    float moveSpeed;
    float originalMoveSpeed;
    public enum PHASE
    {
        WAITING_TO_FLY_IN,
        FLYING_IN,
        WAITING_TO_MOVE,
        MOVING_FORWARD,
        FALL,
        REACHED_DESTINATION
    }
    [HideInInspector]
    public PHASE phase;

    /* In-script Values */
    Rigidbody rb;

    Collider[] colliders;

    [HideInInspector]
    public BaseEntity baseEntity;


    private void Awake()
    {
        baseEntity = GetComponent<BaseEntity>();
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.isKinematic = true;
        originalMoveSpeed = moveSpeed;
        originalNumberOfThrusters = thrusters.Count;
        transform.forward = startTransform.forward;
        phase = PHASE.WAITING_TO_FLY_IN;
        colliders = GetComponentsInChildren<Collider>();

        // Reduce Lag
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        //StartFlyIn();
        //StartFallDown();
    }

    void FixedUpdate()
    {
        switch (phase)
        {
            case PHASE.MOVING_FORWARD:
                {
                    Vector3 targetDest = new Vector3(destination.position.x, transform.position.y, destination.position.z);

                    if (!baseEntity.CheckHealth()) // Check if Spaceship is dead
                        StartFallDown();
                    else if((transform.position - targetDest).sqrMagnitude < 1f) // Check if Spaceship has reached the destination.
                        ReachedDestination();
                    //Debug.LogWarning("Warning:" + (transform.position - targetDest).sqrMagnitude);
                    float functioningThrusters = 0;
                    foreach (GameObject thruster in thrusters)
                    {
                        if (thruster == null)
                        {
                            thrusters.Remove(thruster); // This object has despawned.
                            continue;
                        }

                        EntityHealth entityHealth = thruster.GetComponent<EntityHealth>();
                        if (entityHealth.currHealth > 0)
                            ++functioningThrusters;
                    }
                    // Slow down according to number of thrusters
                    // If all thrusters are destroyed, spaceship should run at 25% speed.
                    if (functioningThrusters == 0)
                        moveSpeed = originalMoveSpeed * 0.25f;
                    else
                        moveSpeed = originalMoveSpeed * (0.25f + (functioningThrusters / originalNumberOfThrusters) * 0.75f);

                    // Enter moving to destination code
                    rb.MovePosition(transform.position + (moveSpeed * (targetDest - transform.position).normalized));

                    

                    break;
                }
        }
    }

    public void StartFlyIn()
    {
        if (phase == PHASE.WAITING_TO_FLY_IN) // only fly in when it is waiting to fly in
        {
            phase = PHASE.FLYING_IN;
            StartCoroutine(FlyIn());
        }
    }

    IEnumerator FlyIn()
    {
        transform.forward = targetFlyInTransform.forward;
        while ((targetFlyInTransform.position - transform.position).sqrMagnitude > 0.1f)
        {
            Vector3 targetDir = targetFlyInTransform.position - transform.position;
            transform.position = Vector3.Lerp(transform.position, targetFlyInTransform.position, flyMovementSpeed);
            yield return new WaitForFixedUpdate();
        }

        transform.forward = targetFlyInTransform.forward;
        transform.position = targetFlyInTransform.position;

        // End of flying in.
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
        phase = PHASE.WAITING_TO_MOVE;
    }

    void StartFallDown()
    {
        phase = PHASE.FALL;
        StartCoroutine(FallDown());
    }

    public bool StartMoving()
    {
        if (phase == PHASE.WAITING_TO_MOVE)
        {
            phase = PHASE.MOVING_FORWARD;
            return true;
        }
        return false;
    }

    IEnumerator FallDown()
    {
        while ((Vector3.down - transform.forward).sqrMagnitude > 0.1f)
        {
            transform.forward = Vector3.Lerp(transform.forward, Vector3.down, flyMovementSpeed * 0.01f);
            transform.position += transform.forward * Time.fixedDeltaTime * moveSpeed;
            yield return new WaitForFixedUpdate();
        }

        GameplayManager.instance.EndMatch(TEAM_TYPE.DEFENDERS);
    }

    void ReachedDestination()
    {
        GameplayManager.instance.EndMatch(TEAM_TYPE.INVADERS);
        phase = PHASE.REACHED_DESTINATION;
    }
}
