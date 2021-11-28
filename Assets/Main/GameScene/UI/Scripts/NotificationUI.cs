using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class NotificationUI : MonoBehaviour
{
    public Text titleText;

    public Text messageText;

    public InterfaceAnimManager animManager;

    [HideInInspector]
    public int ranking;

    [HideInInspector]
    public float activeDurationLeft;

    private void Awake()
    {
        animManager = GetComponent<InterfaceAnimManager>();
    }

    private void Update()
    {
        if (activeDurationLeft > 0f)
        {
            activeDurationLeft -= Time.deltaTime;
        }
        else
        {
            if (animManager.currentState == CSFHIAnimableState.appeared)
                animManager.startDisappear();
            else if (animManager.currentState == CSFHIAnimableState.disappeared)
                OnAnimEnd();
        }
    }

    private void OnAnimEnd()
    {
        NotificationManager.instance.IncreaseUIPosition();
        gameObject.SetActive(false);
    }
}
