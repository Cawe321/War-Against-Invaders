using System.Collections;
using System.Collections.Generic;
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

    BaseEntity baseEntity;


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

        StartFlyIn();
        //StartFallDown();
    }

    void FixedUpdate()
    {
        switch (phase)
        {
            case PHASE.MOVING_FORWARD:
                {
                    if (!baseEntity.CheckHealth()) // Check if Spaceship is dead
                        StartFallDown();
                    else if ((transform.position - destination.position).sqrMagnitude < 0.1f) // Check if Spaceship has reached the destination.
                        ReachedDestination();

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
                    moveSpeed = originalMoveSpeed * (0.25f + (functioningThrusters / originalNumberOfThrusters) * 0.75f);

                    // Enter moving to destination code

                    break;
                }
        }
    }

    void StartFlyIn()
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
            collider.enabled = false;
        }
        phase = PHASE.WAITING_TO_MOVE;
    }

    void StartFallDown()
    {
        phase = PHASE.FALL;
        StartCoroutine(FallDown());
    }

    IEnumerator FallDown()
    {
        while ((Vector3.down - transform.forward).sqrMagnitude > 0.1f)
        {
            transform.forward = Vector3.Lerp(transform.forward, Vector3.down, flyMovementSpeed * 0.01f);
            transform.position += transform.forward * Time.fixedDeltaTime * moveSpeed;
            yield return new WaitForFixedUpdate();
        }
    }

    void ReachedDestination()
    {

    }
}
