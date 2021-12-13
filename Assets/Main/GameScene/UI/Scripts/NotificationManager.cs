using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class NotificationManager : SingletonObject<NotificationManager>
{
    [Header("References")]
    [SerializeField]
    GameObject notificationContainer;

    [Header("Settings")]
    float notificationDuration = 5f;

    List<Vector3> rankPositions;

    List<NotificationUI> notificationUIs;
    struct NotificationObject
    {
        public string message;
        public List<string> items;
    }
    Queue<NotificationObject> notificationQueue;

    int activeUIs;

    private void Start()
    {
        activeUIs = 0;
        notificationQueue = new Queue<NotificationObject>();
        rankPositions = new List<Vector3>();
        notificationUIs = new List<NotificationUI>();
        foreach (Transform transform in notificationContainer.transform)
        {
            rankPositions.Add(transform.localPosition);
            NotificationUI notifcation = transform.GetComponent<NotificationUI>();
            notificationUIs.Add(notifcation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (NotificationUI notification in notificationUIs)
        {
            if (notification.gameObject.activeInHierarchy)
            {
                if ((notification.transform.localPosition - rankPositions[notification.ranking]).sqrMagnitude > 10f)
                    notification.transform.localPosition += (rankPositions[notification.ranking] - notification.transform.localPosition).normalized * Time.deltaTime * 1000f;
                else
                    notification.transform.localPosition = rankPositions[notification.ranking];
            }
        }

        if (notificationQueue.Count > 0)
        {
            NotificationUI notification = GetAvailableNotificationUI(); // Get an available notifcation UI
            if (notification != null) // Check if there is an available notification UI
            {
                ++activeUIs;

                NotificationObject notificationObject = notificationQueue.Dequeue();
                notification.activeDurationLeft = notificationDuration;
                notification.ranking = activeUIs - 1;
                notification.transform.localPosition = rankPositions[notification.ranking];
                notification.titleText.text = notificationObject.message;
                notification.messageText.text = "";
                foreach (string message in notificationObject.items)
                    notification.messageText.text += message + "\n";
                notification.gameObject.SetActive(true);
                notification.animManager.startAppear();
                AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._gameplayNotificationOpenSound);
            }
        }
    }

    /// <summary>
    /// Adds to notification queue. Parameters(string title, string[] message) where "..." is all the strings you wish to add on as a message. Each new string will add a "\n";
    /// </summary>
    public void AddToNotification(string message, params string[] items)
    {
        List<string> itemList = new List<string>();
        for (int i = 0; i < items.Length; ++i)
            itemList.Add(items[i]);
        AddToNotification(message, itemList);
    }

    public void AddToNotification(string message, List<string> itemList)
    {
        NotificationObject notification = new NotificationObject();
        notification.message = message;
        notification.items = itemList;
        notificationQueue.Enqueue(notification);
    }

    public void IncreaseUIPosition()
    {
        --activeUIs;
        foreach (NotificationUI notification in notificationUIs)
        {
            --notification.ranking;
        }
        AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._gameplayNotificationCloseSound);
    }

    NotificationUI GetAvailableNotificationUI()
    {
        foreach (NotificationUI notification in notificationUIs)
        {
            if (!notification.gameObject.activeInHierarchy) // found an available notification UI
            {
                return notification;
            }
        }
        return null;
    }
}


