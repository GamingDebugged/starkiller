using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages game notifications, alerts, and messaging system
    /// Extracted from GameManager for focused notification responsibility
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {
        [Header("Notification Settings")]
        [SerializeField] private int maxNotifications = 10;
        [SerializeField] private float defaultDisplayTime = 3f;
        [SerializeField] private float priorityDisplayTime = 5f;
        [SerializeField] private bool enableNotificationQueue = true;
        
        [Header("Notification Types")]
        [SerializeField] private bool showGameplayNotifications = true;
        [SerializeField] private bool showSystemNotifications = true;
        [SerializeField] private bool showWarningNotifications = true;
        [SerializeField] private bool showErrorNotifications = true;
        
        [Header("Auto-Notifications")]
        [SerializeField] private bool autoNotifyDecisions = true;
        [SerializeField] private bool autoNotifyDayProgress = true;
        [SerializeField] private bool autoNotifyCredits = true;
        [SerializeField] private bool autoNotifyStrikes = true;
        
        [Header("Notification Sounds")]
        [SerializeField] private bool enableNotificationSounds = true;
        [SerializeField] private string infoSoundName = "notification";
        [SerializeField] private string warningSoundName = "warning";
        [SerializeField] private string errorSoundName = "error";
        [SerializeField] private string successSoundName = "success";
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool logAllNotifications = false;
        
        // Notification system state
        private Queue<NotificationData> _notificationQueue = new Queue<NotificationData>();
        private List<NotificationData> _activeNotifications = new List<NotificationData>();
        private List<NotificationData> _notificationHistory = new List<NotificationData>();
        private bool _isProcessingQueue = false;
        
        // Dependencies
        private AudioManager _audioManager;
        private UICoordinator _uiCoordinator;
        
        // Statistics
        private int _totalNotificationsShown = 0;
        private Dictionary<NotificationType, int> _notificationsByType = new Dictionary<NotificationType, int>();
        
        // Events
        public static event Action<NotificationData> OnNotificationShown;
        public static event Action<NotificationData> OnNotificationDismissed;
        public static event Action<int> OnQueueSizeChanged;
        public static event Action OnNotificationHistoryUpdated;
        
        // Public properties
        public int QueueSize => _notificationQueue.Count;
        public int ActiveNotifications => _activeNotifications.Count;
        public int TotalNotificationsShown => _totalNotificationsShown;
        public List<NotificationData> NotificationHistory => new List<NotificationData>(_notificationHistory);
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<NotificationManager>(this);
            
            // Initialize notification type counter
            foreach (NotificationType type in Enum.GetValues(typeof(NotificationType)))
            {
                _notificationsByType[type] = 0;
            }
            
            if (enableDebugLogs)
                Debug.Log("[NotificationManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _audioManager = ServiceLocator.Get<AudioManager>();
            _uiCoordinator = ServiceLocator.Get<UICoordinator>();
            
            // Subscribe to automatic notification events
            SubscribeToAutoNotificationEvents();
            
            // Subscribe to UI notification events
            GameEvents.OnUINotification += ShowUINotification;
            GameEvents.OnUITimedNotification += ShowTimedUINotification;
            
            if (enableDebugLogs)
                Debug.Log("[NotificationManager] Notification system ready");
        }
        
        /// <summary>
        /// Subscribe to events for automatic notifications
        /// </summary>
        private void SubscribeToAutoNotificationEvents()
        {
            if (autoNotifyCredits)
            {
                CreditsManager.OnCreditsChanged += OnCreditsChanged;
            }
            
            if (autoNotifyStrikes)
            {
                DecisionTracker.OnStrikesChanged += OnStrikesChanged;
            }
            
            if (autoNotifyDayProgress)
            {
                DayProgressionManager.OnDayStarted += OnDayStarted;
                DayProgressionManager.OnQuotaReached += OnQuotaReached;
                DayProgressionManager.OnTimeWarning += OnTimeWarning;
            }
            
            if (autoNotifyDecisions)
            {
                DecisionTracker.OnDecisionRecorded += OnDecisionRecorded;
            }
        }
        
        /// <summary>
        /// Show a notification with specified parameters
        /// </summary>
        public void ShowNotification(string message, NotificationType type = NotificationType.Info, 
                                   float displayTime = 0f, bool playSound = true)
        {
            if (string.IsNullOrEmpty(message)) return;
            
            // Check if this notification type is enabled
            if (!IsNotificationTypeEnabled(type)) return;
            
            var notification = new NotificationData
            {
                Id = System.Guid.NewGuid().ToString(),
                Message = message,
                Type = type,
                DisplayTime = displayTime > 0 ? displayTime : GetDefaultDisplayTime(type),
                PlaySound = playSound && enableNotificationSounds,
                Timestamp = DateTime.Now,
                Priority = GetNotificationPriority(type)
            };
            
            if (enableNotificationQueue)
            {
                QueueNotification(notification);
            }
            else
            {
                DisplayNotificationImmediate(notification);
            }
        }
        
        /// <summary>
        /// Show a notification with custom display time
        /// </summary>
        public void ShowTimedNotification(string message, float displayTime, NotificationType type = NotificationType.Info)
        {
            ShowNotification(message, type, displayTime);
        }
        
        /// <summary>
        /// Show a notification from UI events
        /// </summary>
        private void ShowUINotification(string message)
        {
            ShowNotification(message, NotificationType.Info);
        }
        
        /// <summary>
        /// Show a timed notification from UI events
        /// </summary>
        private void ShowTimedUINotification(string message, float displayTime)
        {
            ShowNotification(message, NotificationType.Info, displayTime);
        }
        
        /// <summary>
        /// Queue a notification for display
        /// </summary>
        private void QueueNotification(NotificationData notification)
        {
            // Check queue size limit
            if (_notificationQueue.Count >= maxNotifications)
            {
                // Remove oldest non-priority notification
                var queueList = _notificationQueue.ToList();
                var oldestNonPriority = queueList.FirstOrDefault(n => n.Priority < NotificationPriority.High);
                
                if (oldestNonPriority != null)
                {
                    var tempQueue = new Queue<NotificationData>();
                    foreach (var n in queueList)
                    {
                        if (n != oldestNonPriority)
                            tempQueue.Enqueue(n);
                    }
                    _notificationQueue = tempQueue;
                }
                else
                {
                    // If all are high priority, remove oldest
                    _notificationQueue.Dequeue();
                }
            }
            
            _notificationQueue.Enqueue(notification);
            OnQueueSizeChanged?.Invoke(_notificationQueue.Count);
            
            if (enableDebugLogs && logAllNotifications)
                Debug.Log($"[NotificationManager] Queued notification: {notification.Message}");
            
            // Start processing if not already running
            if (!_isProcessingQueue)
            {
                StartCoroutine(ProcessNotificationQueue());
            }
        }
        
        /// <summary>
        /// Process the notification queue
        /// </summary>
        private IEnumerator ProcessNotificationQueue()
        {
            _isProcessingQueue = true;
            
            while (_notificationQueue.Count > 0)
            {
                var notification = _notificationQueue.Dequeue();
                OnQueueSizeChanged?.Invoke(_notificationQueue.Count);
                
                yield return DisplayNotificationCoroutine(notification);
                
                // Brief pause between notifications
                yield return new WaitForSeconds(0.2f);
            }
            
            _isProcessingQueue = false;
        }
        
        /// <summary>
        /// Display notification immediately
        /// </summary>
        private void DisplayNotificationImmediate(NotificationData notification)
        {
            StartCoroutine(DisplayNotificationCoroutine(notification));
        }
        
        /// <summary>
        /// Display notification coroutine
        /// </summary>
        private IEnumerator DisplayNotificationCoroutine(NotificationData notification)
        {
            // Add to active notifications
            _activeNotifications.Add(notification);
            _totalNotificationsShown++;
            _notificationsByType[notification.Type]++;
            
            // Add to history
            _notificationHistory.Add(notification);
            if (_notificationHistory.Count > 100) // Keep last 100 notifications
            {
                _notificationHistory.RemoveAt(0);
            }
            
            if (enableDebugLogs)
                Debug.Log($"[NotificationManager] Displaying {notification.Type}: {notification.Message}");
            
            // Play sound if enabled
            if (notification.PlaySound && _audioManager != null)
            {
                string soundName = GetSoundForNotificationType(notification.Type);
                if (!string.IsNullOrEmpty(soundName))
                {
                    _audioManager.PlaySound(soundName);
                }
            }
            
            // Show in UI
            if (_uiCoordinator != null)
            {
                _uiCoordinator.ShowNotification(notification.Message);
            }
            
            // Trigger events
            OnNotificationShown?.Invoke(notification);
            OnNotificationHistoryUpdated?.Invoke();
            
            // Wait for display time
            yield return new WaitForSeconds(notification.DisplayTime);
            
            // Remove from active notifications
            _activeNotifications.Remove(notification);
            OnNotificationDismissed?.Invoke(notification);
        }
        
        /// <summary>
        /// Dismiss a specific notification
        /// </summary>
        public void DismissNotification(string notificationId)
        {
            var notification = _activeNotifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null)
            {
                _activeNotifications.Remove(notification);
                OnNotificationDismissed?.Invoke(notification);
                
                if (enableDebugLogs)
                    Debug.Log($"[NotificationManager] Dismissed notification: {notification.Message}");
            }
        }
        
        /// <summary>
        /// Clear all notifications
        /// </summary>
        public void ClearAllNotifications()
        {
            var activeNotifications = new List<NotificationData>(_activeNotifications);
            _activeNotifications.Clear();
            _notificationQueue.Clear();
            
            foreach (var notification in activeNotifications)
            {
                OnNotificationDismissed?.Invoke(notification);
            }
            
            OnQueueSizeChanged?.Invoke(0);
            
            if (enableDebugLogs)
                Debug.Log("[NotificationManager] All notifications cleared");
        }
        
        /// <summary>
        /// Get default display time for notification type
        /// </summary>
        private float GetDefaultDisplayTime(NotificationType type)
        {
            return type switch
            {
                NotificationType.Error => priorityDisplayTime,
                NotificationType.Warning => priorityDisplayTime,
                NotificationType.Success => defaultDisplayTime,
                NotificationType.Info => defaultDisplayTime,
                NotificationType.System => defaultDisplayTime,
                _ => defaultDisplayTime
            };
        }
        
        /// <summary>
        /// Get priority for notification type
        /// </summary>
        private NotificationPriority GetNotificationPriority(NotificationType type)
        {
            return type switch
            {
                NotificationType.Error => NotificationPriority.Critical,
                NotificationType.Warning => NotificationPriority.High,
                NotificationType.Success => NotificationPriority.Medium,
                NotificationType.Info => NotificationPriority.Low,
                NotificationType.System => NotificationPriority.Medium,
                _ => NotificationPriority.Low
            };
        }
        
        /// <summary>
        /// Get sound name for notification type
        /// </summary>
        private string GetSoundForNotificationType(NotificationType type)
        {
            return type switch
            {
                NotificationType.Error => errorSoundName,
                NotificationType.Warning => warningSoundName,
                NotificationType.Success => successSoundName,
                NotificationType.Info => infoSoundName,
                NotificationType.System => infoSoundName,
                _ => infoSoundName
            };
        }
        
        /// <summary>
        /// Check if notification type is enabled
        /// </summary>
        private bool IsNotificationTypeEnabled(NotificationType type)
        {
            return type switch
            {
                NotificationType.Error => showErrorNotifications,
                NotificationType.Warning => showWarningNotifications,
                NotificationType.Success => showGameplayNotifications,
                NotificationType.Info => showGameplayNotifications,
                NotificationType.System => showSystemNotifications,
                _ => true
            };
        }
        
        /// <summary>
        /// Get notification statistics
        /// </summary>
        public NotificationStatistics GetStatistics()
        {
            return new NotificationStatistics
            {
                TotalShown = _totalNotificationsShown,
                CurrentlyActive = _activeNotifications.Count,
                InQueue = _notificationQueue.Count,
                HistorySize = _notificationHistory.Count,
                NotificationsByType = new Dictionary<NotificationType, int>(_notificationsByType)
            };
        }
        
        /// <summary>
        /// Set notification type enabled/disabled
        /// </summary>
        public void SetNotificationTypeEnabled(NotificationType type, bool enabled)
        {
            switch (type)
            {
                case NotificationType.Error:
                    showErrorNotifications = enabled;
                    break;
                case NotificationType.Warning:
                    showWarningNotifications = enabled;
                    break;
                case NotificationType.Success:
                case NotificationType.Info:
                    showGameplayNotifications = enabled;
                    break;
                case NotificationType.System:
                    showSystemNotifications = enabled;
                    break;
            }
            
            if (enableDebugLogs)
                Debug.Log($"[NotificationManager] {type} notifications {(enabled ? "enabled" : "disabled")}");
        }
        
        // Auto-notification event handlers
        private void OnCreditsChanged(int newAmount, int change)
        {
            if (change != 0)
            {
                string message = change > 0 ? 
                    $"Credits gained: +{change} (Total: {newAmount})" : 
                    $"Credits spent: {change} (Total: {newAmount})";
                
                NotificationType type = change > 0 ? NotificationType.Success : NotificationType.Info;
                ShowNotification(message, type);
            }
        }
        
        private void OnStrikesChanged(int currentStrikes, int maxStrikes)
        {
            if (currentStrikes > 0)
            {
                string message = $"Strike received! ({currentStrikes}/{maxStrikes})";
                NotificationType type = currentStrikes >= maxStrikes - 1 ? 
                    NotificationType.Error : NotificationType.Warning;
                
                ShowNotification(message, type);
            }
        }
        
        private void OnDayStarted(int day)
        {
            ShowNotification($"Day {day} has begun. Good luck, operator!", NotificationType.System);
        }
        
        private void OnQuotaReached()
        {
            ShowNotification("Daily quota reached! Excellent work!", NotificationType.Success);
        }
        
        private void OnTimeWarning(string warning)
        {
            ShowNotification(warning, NotificationType.Warning);
        }
        
        private void OnDecisionRecorded(bool wasCorrect, string reason)
        {
            if (!wasCorrect && !string.IsNullOrEmpty(reason))
            {
                ShowNotification($"Incorrect decision: {reason}", NotificationType.Warning);
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (autoNotifyCredits)
            {
                CreditsManager.OnCreditsChanged -= OnCreditsChanged;
            }
            
            if (autoNotifyStrikes)
            {
                DecisionTracker.OnStrikesChanged -= OnStrikesChanged;
            }
            
            if (autoNotifyDayProgress)
            {
                DayProgressionManager.OnDayStarted -= OnDayStarted;
                DayProgressionManager.OnQuotaReached -= OnQuotaReached;
                DayProgressionManager.OnTimeWarning -= OnTimeWarning;
            }
            
            if (autoNotifyDecisions)
            {
                DecisionTracker.OnDecisionRecorded -= OnDecisionRecorded;
            }
            
            GameEvents.OnUINotification -= ShowUINotification;
            GameEvents.OnUITimedNotification -= ShowTimedUINotification;
            
            // Clear event subscriptions
            OnNotificationShown = null;
            OnNotificationDismissed = null;
            OnQueueSizeChanged = null;
            OnNotificationHistoryUpdated = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Info Notification")]
        private void TestInfoNotification() => ShowNotification("This is a test info notification", NotificationType.Info);
        
        [ContextMenu("Test: Warning Notification")]
        private void TestWarningNotification() => ShowNotification("This is a test warning notification", NotificationType.Warning);
        
        [ContextMenu("Test: Error Notification")]
        private void TestErrorNotification() => ShowNotification("This is a test error notification", NotificationType.Error);
        
        [ContextMenu("Test: Success Notification")]
        private void TestSuccessNotification() => ShowNotification("This is a test success notification", NotificationType.Success);
        
        [ContextMenu("Test: Multiple Notifications")]
        private void TestMultipleNotifications()
        {
            ShowNotification("First notification", NotificationType.Info);
            ShowNotification("Second notification", NotificationType.Warning);
            ShowNotification("Third notification", NotificationType.Success);
        }
        
        [ContextMenu("Clear All Notifications")]
        private void TestClearAll() => ClearAllNotifications();
        
        [ContextMenu("Show Notification Statistics")]
        private void ShowNotificationStatistics()
        {
            var stats = GetStatistics();
            Debug.Log("=== NOTIFICATION STATISTICS ===");
            Debug.Log($"Total Shown: {stats.TotalShown}");
            Debug.Log($"Currently Active: {stats.CurrentlyActive}");
            Debug.Log($"In Queue: {stats.InQueue}");
            Debug.Log($"History Size: {stats.HistorySize}");
            
            foreach (var kvp in stats.NotificationsByType)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value}");
            }
            Debug.Log("=== END STATISTICS ===");
        }
    }
    
    // Supporting data structures
    [System.Serializable]
    public class NotificationData
    {
        public string Id;
        public string Message;
        public NotificationType Type;
        public float DisplayTime;
        public bool PlaySound;
        public DateTime Timestamp;
        public NotificationPriority Priority;
    }
    
    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        Success,
        System
    }
    
    public enum NotificationPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }
    
    [System.Serializable]
    public struct NotificationStatistics
    {
        public int TotalShown;
        public int CurrentlyActive;
        public int InQueue;
        public int HistorySize;
        public Dictionary<NotificationType, int> NotificationsByType;
    }
}