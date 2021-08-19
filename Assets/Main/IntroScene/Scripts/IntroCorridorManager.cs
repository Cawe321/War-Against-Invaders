using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCorridorManager : MonoBehaviour
{
    [Header("References")]
    public Transform corridorContainer;

    public IntroCorridor corridorGameObject;

    [Header("Tech Settings")]
    [Tooltip("Amount of corridor gameobjects to preload")]
    public int numToPreload = 1;

    [Tooltip("The number of corridors that will exist at once")]
    public int existingCorridors = 2;

    [Header("Anim Settings")]
    public float startHeightOffset = 5f;

    [Range(1f, 100f)]
    public float fallAnimSpeed = 1f;

    public float forwardAnimSpeed = 1f;

    public enum ANIM_STATUS
    {
        AWAITING_PLAY,  // not doing anything
        PLAYING,        // playing anium
        QUEUED_END,     // queued to finish anim, once anim finishes, return to AWAITING_PLAY
    }
    public ANIM_STATUS animStatus = ANIM_STATUS.AWAITING_PLAY;

    float distanceBetweenCorridor;

    IntroCorridor previousCorridor = null;

    bool animBusy;

    // Start is called before the first frame update
    void Start()
    {
        if (existingCorridors < 2)
        {
            Debug.LogWarning("IntroCorridorManager: Setting existingCorridors to < 2 may cause unwanted visual issues.");
        }

        animStatus = ANIM_STATUS.AWAITING_PLAY;

        // Precalculate neccessary values that will be used.
        distanceBetweenCorridor = (corridorGameObject.startPosition.position - corridorGameObject.endPosition.position).magnitude;

        // Preloading the corridors
        for (int i = 0; i < numToPreload; ++i)
        {
            // Instantiate corridor gameobject and parent under gameobject, then set gameobject to inactive
            InstantiateCorridor();
        }

        // Spawn the first set corridors
        Camera mainCamera = Camera.main;
        for (int i = 0; i < existingCorridors; ++i)
        {
            IntroCorridor newCorridor = GetCorridor();
            newCorridor.gameObject.SetActive(true);
            newCorridor.transform.position = mainCamera.transform.position + Vector3.forward * ((distanceBetweenCorridor * 0.5f) + (i * distanceBetweenCorridor)); // Set position to infront of camera or infront of the earlier corridors
            newCorridor.transform.forward = Vector3.forward; // Set rotation to be the same as camera
            previousCorridor = newCorridor;
        }

        StartCorridorAnim();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region START_AND_END_FUNCTIONS
    /// <summary>
    /// Officially start the animation. Called by an external manager script.
    /// </summary>
    public void StartCorridorAnim()
    {
        if (animStatus == ANIM_STATUS.AWAITING_PLAY)
        {
            animStatus = ANIM_STATUS.PLAYING;
        }
    }
    /// <summary>
    /// A couroutine function that queues the end of the end anim by playing the ending animation.
    /// </summary>
    /// <returns>Returns when the scripted anim has finished playing.</returns>
    IEnumerator FinishCorridorAnim()
    {
        animStatus = ANIM_STATUS.QUEUED_END;
        return null;
    }
    #endregion

    #region ANIMATION_FUNCTIONS
    /// <summary>
    /// Starts to play next animation
    /// </summary>
    public void PlayNextLoop()
    {
        if (animStatus == ANIM_STATUS.PLAYING)
        {
            StartCoroutine(QueueToPlayAnim());
        }
    }

    IEnumerator QueueToPlayAnim()
    {
        while (animBusy)
            yield return new WaitForFixedUpdate();

        if (animStatus == ANIM_STATUS.PLAYING) // check again if the manager should still play anim
        {
            IntroCorridor newCorridor = GetCorridor();
            newCorridor.gameObject.SetActive(true);
            newCorridor.transform.position = previousCorridor.transform.position + Vector3.forward * distanceBetweenCorridor + Vector3.up * startHeightOffset;
            newCorridor.transform.forward = Vector3.forward; // Set rotation to be the same as camera

            Vector3 targetPos = previousCorridor.transform.position + Vector3.forward * distanceBetweenCorridor;
            previousCorridor = newCorridor;
            StartCoroutine(PlayAnim(newCorridor, targetPos));
        }

    }

    IEnumerator PlayAnim(IntroCorridor corridor, Vector3 targetPos)
    {
        animBusy = true;
        while (Vector3.Distance(corridor.transform.position, targetPos) > 0.1f)
        {
            Debug.Log("Running");
            corridor.rb.MovePosition(Vector3.Lerp(corridor.rb.position, targetPos, 0.01f * fallAnimSpeed));
            yield return new WaitForFixedUpdate();
        }
        animBusy = false;
        yield return true;
    }

    #endregion

    #region OBJECT_POOLER_FUNCTIONS
    private IntroCorridor GetCorridor()
    {
        foreach (Transform corridor in corridorContainer)
        {
            IntroCorridor introCorridor = corridor.GetComponent<IntroCorridor>();
            if (introCorridor && !corridor.gameObject.activeInHierarchy)
            {
                return introCorridor;
            }
        }

        // If the code reaches here, it means it cannot find an inactive introCorridor;
        IntroCorridor newCorridor = InstantiateCorridor();
        return newCorridor;
    }

    private IntroCorridor InstantiateCorridor()
    {
        IntroCorridor newCorridor = Instantiate(corridorGameObject, corridorContainer);
        newCorridor.gameObject.SetActive(false);
        newCorridor.corridorManager = this;
        return newCorridor;
    }
    #endregion
}

