using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGame
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        [SerializeField] private NotificationUI notificationPrefab;
        [SerializeField] private Transform notificationContainer;
        [SerializeField] private int maxConcurrentNotifications = 3;
        [SerializeField] private float notificationDuration = 3f;

        private Queue<NotificationData> notificationQueue = new Queue<NotificationData>();
        private List<NotificationUI> activeNotifications = new List<NotificationUI>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            // Obtenir le GameObject racine et le marquer
            Transform rootTransform = transform.root;
            DontDestroyOnLoad(rootTransform.gameObject);
            
            Debug.Log("[NotificationManager] Initialisé avec succès.");
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        public void ShowNotification(string message, NotificationType type = NotificationType.Info, float duration = 0)
        {
            if (duration <= 0)
                duration = notificationDuration;

            var notification = new NotificationData
            {
                Message = message,
                Type = type,
                Duration = duration
            };

            notificationQueue.Enqueue(notification);
            ProcessQueue();
        }

        private void ProcessQueue()
        {
            while (notificationQueue.Count > 0 && activeNotifications.Count < maxConcurrentNotifications)
            {
                var notification = notificationQueue.Dequeue();
                DisplayNotification(notification);
            }
        }

        private void DisplayNotification(NotificationData data)
        {
            if (notificationPrefab == null || notificationContainer == null)
            {
                Debug.LogError("[NotificationManager] Prefab ou Container manquant!");
                return;
            }

            var notificationUI = Instantiate(notificationPrefab, notificationContainer);
            notificationUI.Initialize(data, () => RemoveNotification(notificationUI));
            activeNotifications.Add(notificationUI);

            Debug.Log($"[NotificationManager] Notification affichée: {data.Message} ({data.Type})");
        }

        private void RemoveNotification(NotificationUI notificationUI)
        {
            activeNotifications.Remove(notificationUI);
            Destroy(notificationUI.gameObject);
            ProcessQueue();
        }

        private void SubscribeToEvents()
        {
            if (UpgradeManager.Instance != null)
                UpgradeManager.Instance.OnUpgradeUnlocked += OnUpgradeUnlocked;

            if (ExperienceManager.Instance != null)
                ExperienceManager.Instance.OnLevelUp += OnLevelUp;
        }

        private void UnsubscribeFromEvents()
        {
            if (UpgradeManager.Instance != null)
                UpgradeManager.Instance.OnUpgradeUnlocked -= OnUpgradeUnlocked;

            if (ExperienceManager.Instance != null)
                ExperienceManager.Instance.OnLevelUp -= OnLevelUp;
        }

        private void OnUpgradeUnlocked(UpgradeData upgrade)
        {
            ShowNotification($"?? Upgrade débloqué: {upgrade.DisplayName}", NotificationType.Success);
        }

        private void OnLevelUp(string cropName, int newLevel)
        {
            ShowNotification($"? {cropName} - Niveau {newLevel} atteint!", NotificationType.Success);
        }
    }

    [System.Serializable]
    public class NotificationData
    {
        public string Message;
        public NotificationType Type;
        public float Duration;
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
}
