using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Coordinates UI updates across all game systems
    /// Extracted from GameManager for focused UI responsibility
    /// </summary>
    public class UICoordinator : MonoBehaviour
    {
        [Header("Core UI References")]
        [SerializeField] private TextMeshProUGUI creditsText;
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI strikesText;
        [SerializeField] private TextMeshProUGUI shipsProcessedText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        
        [Header("Progress Bars")]
        [SerializeField] private Slider shiftProgressBar;
        [SerializeField] private Slider quotaProgressBar;
        [SerializeField] private Image timeProgressFill;
        
        [Header("Status Indicators")]
        [SerializeField] private Image gameStateIndicator;
        [SerializeField] private Image quotaStatusIndicator;
        [SerializeField] private GameObject pauseOverlay;
        [SerializeField] private GameObject gameOverOverlay;
        
        [Header("Notification System")]
        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private float notificationDuration = 3f;
        [SerializeField] private AnimationCurve notificationFade = AnimationCurve.EaseInOut(0, 1, 1, 0);
        
        [Header("Color Themes")]
        [SerializeField] private Color normalTimeColor = Color.white;
        [SerializeField] private Color warningTimeColor = Color.yellow;
        [SerializeField] private Color criticalTimeColor = Color.red;
        [SerializeField] private Color quotaMetColor = Color.green;
        [SerializeField] private Color quotaPendingColor = Color.yellow;
        
        [Header("Performance Indicators")]
        [SerializeField] private TextMeshProUGUI performanceText;
        [SerializeField] private Image performanceBar;
        [SerializeField] private Color excellentPerformanceColor = Color.green;
        [SerializeField] private Color goodPerformanceColor = Color.yellow;
        [SerializeField] private Color poorPerformanceColor = Color.red;
        
        [Header("Panel Management")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private GameObject dailyReportPanel;
        
        [Header("Shift Control")]
        [SerializeField] private Button startShiftButton;
        [SerializeField] private GameObject startShiftPanel;
        
        [Header("Audio")]
        [SerializeField] private AudioSource buttonClickSound;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private TextMeshProUGUI debugInfoText;
        
        // Manager references
        private CreditsManager creditsManager;
        private DecisionTracker decisionTracker;
        private DayProgressionManager dayManager;
        private GameStateManager gameStateManager;
        
        // UI state tracking
        private Coroutine notificationCoroutine;
        private Queue<string> notificationQueue = new Queue<string>();
        private bool isShowingNotification = false;
        
        // Events
        public static event Action<string> OnNotificationShown;
        public static event Action OnUIRefreshed;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<UICoordinator>(this);
            
            if (enableDebugLogs)
                Debug.Log("[UICoordinator] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager references
            creditsManager = ServiceLocator.Get<CreditsManager>();
            decisionTracker = ServiceLocator.Get<DecisionTracker>();
            dayManager = ServiceLocator.Get<DayProgressionManager>();
            gameStateManager = ServiceLocator.Get<GameStateManager>();
            
            // Log connection status for debugging
            if (enableDebugLogs)
            {
                Debug.Log($"[UICoordinator] Manager connections:");
                Debug.Log($"  - Credits Manager: {(creditsManager != null ? "Connected" : "Missing")}");
                Debug.Log($"  - Decision Tracker: {(decisionTracker != null ? "Connected" : "Missing")}");
                Debug.Log($"  - Day Manager: {(dayManager != null ? "Connected" : "Missing")}");
                Debug.Log($"  - Game State Manager: {(gameStateManager != null ? "Connected" : "Missing")}");
            }
            
            // Subscribe to manager events
            SubscribeToEvents();
            
            // Initial UI update
            RefreshAllUI();
            
            if (enableDebugLogs)
                Debug.Log("[UICoordinator] UI system ready");
            
            // If day manager is missing, try to reconnect after a delay
            if (dayManager == null)
            {
                StartCoroutine(RetryDayManagerConnection());
            }
        }
        
        /// <summary>
        /// Retry connecting to DayProgressionManager if it wasn't available at start
        /// </summary>
        private IEnumerator RetryDayManagerConnection()
        {
            float retryDelay = 1f;
            int maxRetries = 5;
            int retryCount = 0;
            
            while (dayManager == null && retryCount < maxRetries)
            {
                yield return new WaitForSeconds(retryDelay);
                
                dayManager = ServiceLocator.Get<DayProgressionManager>();
                if (dayManager == null)
                {
                    dayManager = FindFirstObjectByType<DayProgressionManager>();
                }
                
                retryCount++;
                
                if (enableDebugLogs)
                    Debug.Log($"[UICoordinator] Retry {retryCount}/{maxRetries} - Day Manager: {(dayManager != null ? "Found" : "Still Missing")}");
                
                if (dayManager != null)
                {
                    // Subscribe to day manager events
                    DayProgressionManager.OnDayChanged += OnDayChanged;
                    DayProgressionManager.OnTimeUpdated += OnTimeUpdated;
                    DayProgressionManager.OnShipProcessed += OnShipProcessed;
                    DayProgressionManager.OnQuotaReached += OnQuotaReached;
                    DayProgressionManager.OnTimeWarning += OnTimeWarning;
                    
                    // Update the UI now that we have the day manager
                    RefreshAllUI();
                    
                    if (enableDebugLogs)
                        Debug.Log("[UICoordinator] Successfully connected to DayProgressionManager and refreshed UI");
                    
                    break;
                }
            }
            
            if (dayManager == null)
            {
                Debug.LogError("[UICoordinator] Failed to connect to DayProgressionManager after all retries!");
            }
        }
        
        /// <summary>
        /// Subscribe to all relevant manager events
        /// </summary>
        private void SubscribeToEvents()
        {
            // Credits events
            if (creditsManager != null)
                CreditsManager.OnCreditsChanged += OnCreditsChanged;
            
            // Decision events
            if (decisionTracker != null)
            {
                DecisionTracker.OnStrikesChanged += OnStrikesChanged;
                DecisionTracker.OnAccuracyChanged += OnAccuracyChanged;
            }
            
            // Day progression events
            if (dayManager != null)
            {
                DayProgressionManager.OnDayChanged += OnDayChanged;
                DayProgressionManager.OnTimeUpdated += OnTimeUpdated;
                DayProgressionManager.OnShipProcessed += OnShipProcessed;
                DayProgressionManager.OnQuotaReached += OnQuotaReached;
                DayProgressionManager.OnTimeWarning += OnTimeWarning;
            }
            
            // Game state events
            if (gameStateManager != null)
                GameEvents.OnGameStateChanged += OnGameStateChanged;
            
            // Global UI events
            GameEvents.OnUINotification += ShowNotification;
            GameEvents.OnUIRefreshRequested += RefreshAllUI;
        }
        
        /// <summary>
        /// Update the entire UI
        /// </summary>
        public void RefreshAllUI()
        {
            UpdateCreditsDisplay();
            UpdateDayDisplay();
            UpdateTimeDisplay();
            UpdateStrikesDisplay();
            UpdateShipsDisplay();
            UpdateAccuracyDisplay();
            UpdateProgressBars();
            UpdateStatusIndicators();
            UpdatePerformanceDisplay();
            UpdateDebugInfo();
            
            OnUIRefreshed?.Invoke();
            
            if (enableDebugLogs && showDebugInfo)
                Debug.Log("[UICoordinator] Full UI refresh completed");
        }
        
        /// <summary>
        /// Update credits display
        /// </summary>
        private void UpdateCreditsDisplay()
        {
            if (creditsText != null && creditsManager != null)
            {
                creditsText.text = $"Credits: {creditsManager.CurrentCredits:N0}";
            }
        }
        
        /// <summary>
        /// Update day display
        /// </summary>
        private void UpdateDayDisplay()
        {
            if (enableDebugLogs)
                Debug.Log($"[UICoordinator] UpdateDayDisplay called - dayText: {(dayText != null ? "Available" : "Null")}, dayManager: {(dayManager != null ? "Available" : "Null")}");
            
            if (dayText != null && dayManager != null)
            {
                int currentDay = dayManager.CurrentDay;
                string oldText = dayText.text;
                string newText = $"Day {currentDay}";
                dayText.text = newText;
                
                if (enableDebugLogs)
                    Debug.Log($"[UICoordinator] Day text updated: '{oldText}' -> '{newText}' (Manager Current Day: {currentDay})");
            }
            else
            {
                if (enableDebugLogs)
                {
                    if (dayText == null)
                        Debug.LogWarning("[UICoordinator] dayText is null! Day display cannot be updated.");
                    if (dayManager == null)
                        Debug.LogWarning("[UICoordinator] dayManager is null! Day display cannot be updated.");
                }
                
                // Try to reconnect to day manager if it's missing
                if (dayManager == null)
                {
                    dayManager = ServiceLocator.Get<DayProgressionManager>();
                    if (dayManager == null)
                    {
                        dayManager = FindFirstObjectByType<DayProgressionManager>();
                    }
                    
                    if (enableDebugLogs)
                        Debug.Log($"[UICoordinator] Reconnection attempt in UpdateDayDisplay - Day Manager: {(dayManager != null ? "Found" : "Still Missing")}");
                    
                    // Try again if we found the manager
                    if (dayText != null && dayManager != null)
                    {
                        string newText = $"Day {dayManager.CurrentDay}";
                        dayText.text = newText;
                        
                        if (enableDebugLogs)
                            Debug.Log($"[UICoordinator] Day text updated after reconnection: '{newText}' (Current Day: {dayManager.CurrentDay})");
                    }
                }
            }
        }
        
        /// <summary>
        /// Update time display with color coding
        /// </summary>
        private void UpdateTimeDisplay()
        {
            if (timeText != null && dayManager != null)
            {
                string formattedTime = dayManager.GetFormattedTime();
                timeText.text = formattedTime;
                
                // Color code based on remaining time
                float remainingTime = dayManager.RemainingTime;
                if (remainingTime <= 10f)
                {
                    timeText.color = criticalTimeColor;
                }
                else if (remainingTime <= 30f)
                {
                    timeText.color = warningTimeColor;
                }
                else
                {
                    timeText.color = normalTimeColor;
                }
            }
        }
        
        /// <summary>
        /// Update strikes display
        /// </summary>
        private void UpdateStrikesDisplay()
        {
            if (strikesText != null && decisionTracker != null)
            {
                int current = decisionTracker.CurrentStrikes;
                int max = decisionTracker.MaxStrikes;
                strikesText.text = $"Strikes: {current}/{max}";
                
                // Color code based on strike count
                if (current >= max - 1)
                {
                    strikesText.color = criticalTimeColor;
                }
                else if (current >= max / 2)
                {
                    strikesText.color = warningTimeColor;
                }
                else
                {
                    strikesText.color = normalTimeColor;
                }
            }
        }
        
        /// <summary>
        /// Update ships processed display
        /// </summary>
        private void UpdateShipsDisplay()
        {
            if (shipsProcessedText != null && dayManager != null)
            {
                int processed = dayManager.ShipsProcessedToday;
                int quota = dayManager.GetCurrentDayQuota(); // Use direct quota method instead of calculation
                shipsProcessedText.text = $"Ships: {processed}/{quota}";
                
                if (enableDebugLogs)
                    Debug.Log($"[UICoordinator] UpdateShipsDisplay - Day {dayManager.CurrentDay}: {processed}/{quota} (Until quota: {dayManager.ShipsUntilQuota})");
            }
            else if (enableDebugLogs)
            {
                Debug.LogWarning($"[UICoordinator] UpdateShipsDisplay failed - shipsProcessedText: {(shipsProcessedText != null ? "OK" : "NULL")}, dayManager: {(dayManager != null ? "OK" : "NULL")}");
            }
        }
        
        /// <summary>
        /// Update accuracy display
        /// </summary>
        private void UpdateAccuracyDisplay()
        {
            if (accuracyText != null && decisionTracker != null)
            {
                float accuracy = decisionTracker.AccuracyPercentage;
                accuracyText.text = $"Accuracy: {accuracy:F1}%";
                
                // Color code based on accuracy
                if (accuracy >= 90f)
                {
                    accuracyText.color = excellentPerformanceColor;
                }
                else if (accuracy >= 70f)
                {
                    accuracyText.color = goodPerformanceColor;
                }
                else
                {
                    accuracyText.color = poorPerformanceColor;
                }
            }
        }
        
        /// <summary>
        /// Update progress bars
        /// </summary>
        private void UpdateProgressBars()
        {
            if (dayManager != null)
            {
                // Shift progress bar
                if (shiftProgressBar != null)
                {
                    shiftProgressBar.value = dayManager.ShiftProgress;
                }
                
                // Quota progress bar
                if (quotaProgressBar != null)
                {
                    int processed = dayManager.ShipsProcessedToday;
                    int quota = dayManager.ShipsUntilQuota + processed;
                    quotaProgressBar.value = quota > 0 ? (float)processed / quota : 0f;
                }
                
                // Time progress fill
                if (timeProgressFill != null)
                {
                    timeProgressFill.fillAmount = 1f - dayManager.ShiftProgress;
                    
                    // Change color based on time remaining
                    float remainingTime = dayManager.RemainingTime;
                    if (remainingTime <= 10f)
                    {
                        timeProgressFill.color = criticalTimeColor;
                    }
                    else if (remainingTime <= 30f)
                    {
                        timeProgressFill.color = warningTimeColor;
                    }
                    else
                    {
                        timeProgressFill.color = normalTimeColor;
                    }
                }
            }
        }
        
        /// <summary>
        /// Update status indicators
        /// </summary>
        private void UpdateStatusIndicators()
        {
            // Game state indicator
            if (gameStateIndicator != null && gameStateManager != null)
            {
                switch (gameStateManager.CurrentState)
                {
                    case Starkiller.Core.GameState.Gameplay:
                        gameStateIndicator.color = Color.green;
                        break;
                    case Starkiller.Core.GameState.Paused:
                        gameStateIndicator.color = Color.yellow;
                        break;
                    case Starkiller.Core.GameState.GameOver:
                        gameStateIndicator.color = Color.red;
                        break;
                    default:
                        gameStateIndicator.color = Color.gray;
                        break;
                }
            }
            
            // Quota status indicator
            if (quotaStatusIndicator != null && dayManager != null)
            {
                if (dayManager.QuotaMet)
                {
                    quotaStatusIndicator.color = quotaMetColor;
                }
                else
                {
                    quotaStatusIndicator.color = quotaPendingColor;
                }
            }
            
            // Pause overlay
            if (pauseOverlay != null && gameStateManager != null)
            {
                pauseOverlay.SetActive(gameStateManager.CurrentState == Starkiller.Core.GameState.Paused);
            }
            
            // Game over overlay
            if (gameOverOverlay != null && gameStateManager != null)
            {
                gameOverOverlay.SetActive(gameStateManager.CurrentState == Starkiller.Core.GameState.GameOver);
            }
        }
        
        /// <summary>
        /// Update performance display
        /// </summary>
        private void UpdatePerformanceDisplay()
        {
            if (performanceText != null && decisionTracker != null)
            {
                float accuracy = decisionTracker.AccuracyPercentage;
                string performanceLevel;
                Color performanceColor;
                
                if (accuracy >= 90f)
                {
                    performanceLevel = "Excellent";
                    performanceColor = excellentPerformanceColor;
                }
                else if (accuracy >= 70f)
                {
                    performanceLevel = "Good";
                    performanceColor = goodPerformanceColor;
                }
                else
                {
                    performanceLevel = "Needs Improvement";
                    performanceColor = poorPerformanceColor;
                }
                
                performanceText.text = $"Performance: {performanceLevel}";
                performanceText.color = performanceColor;
                
                if (performanceBar != null)
                {
                    performanceBar.fillAmount = accuracy / 100f;
                    performanceBar.color = performanceColor;
                }
            }
        }
        
        /// <summary>
        /// Update debug information
        /// </summary>
        private void UpdateDebugInfo()
        {
            if (debugInfoText != null && showDebugInfo)
            {
                string debugInfo = "=== DEBUG INFO ===\n";
                
                if (creditsManager != null)
                    debugInfo += $"Credits: {creditsManager.CurrentCredits}\n";
                
                if (decisionTracker != null)
                    debugInfo += $"Decisions: {decisionTracker.CorrectDecisions}C/{decisionTracker.WrongDecisions}W\n";
                
                if (dayManager != null)
                    debugInfo += $"Day {dayManager.CurrentDay}, Ships: {dayManager.ShipsProcessedToday}\n";
                
                if (gameStateManager != null)
                    debugInfo += $"State: {gameStateManager.CurrentState}\n";
                
                debugInfoText.text = debugInfo;
                debugInfoText.gameObject.SetActive(true);
            }
            else if (debugInfoText != null)
            {
                debugInfoText.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Show a notification message
        /// </summary>
        public void ShowNotification(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            
            // Queue the notification if one is already showing
            if (isShowingNotification)
            {
                notificationQueue.Enqueue(message);
                return;
            }
            
            if (notificationCoroutine != null)
            {
                StopCoroutine(notificationCoroutine);
            }
            
            notificationCoroutine = StartCoroutine(DisplayNotification(message));
        }
        
        /// <summary>
        /// Display notification with fade animation
        /// </summary>
        private IEnumerator DisplayNotification(string message)
        {
            isShowingNotification = true;
            
            if (notificationPanel != null && notificationText != null)
            {
                notificationText.text = message;
                notificationPanel.SetActive(true);
                
                CanvasGroup canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = notificationPanel.AddComponent<CanvasGroup>();
                
                // Fade in
                for (float t = 0; t < 0.5f; t += Time.deltaTime)
                {
                    canvasGroup.alpha = Mathf.Lerp(0, 1, t / 0.5f);
                    yield return null;
                }
                canvasGroup.alpha = 1f;
                
                // Wait for display duration
                yield return new WaitForSeconds(notificationDuration);
                
                // Fade out
                for (float t = 0; t < 0.5f; t += Time.deltaTime)
                {
                    canvasGroup.alpha = Mathf.Lerp(1, 0, t / 0.5f);
                    yield return null;
                }
                canvasGroup.alpha = 0f;
                
                notificationPanel.SetActive(false);
            }
            
            OnNotificationShown?.Invoke(message);
            
            if (enableDebugLogs)
                Debug.Log($"[UICoordinator] Notification shown: {message}");
            
            isShowingNotification = false;
            
            // Check for queued notifications
            if (notificationQueue.Count > 0)
            {
                string nextMessage = notificationQueue.Dequeue();
                yield return new WaitForSeconds(0.2f); // Brief pause between notifications
                notificationCoroutine = StartCoroutine(DisplayNotification(nextMessage));
            }
        }
        
        /// <summary>
        /// Event handlers
        /// </summary>
        private void OnCreditsChanged(int newAmount, int change)
        {
            UpdateCreditsDisplay();
        }
        
        private void OnStrikesChanged(int currentStrikes, int maxStrikes)
        {
            UpdateStrikesDisplay();
            UpdateStatusIndicators();
        }
        
        private void OnAccuracyChanged(float accuracy)
        {
            UpdateAccuracyDisplay();
            UpdatePerformanceDisplay();
        }
        
        private void OnDayChanged(int newDay)
        {
            if (enableDebugLogs)
                Debug.Log($"[UICoordinator] OnDayChanged event received - New Day: {newDay}");
            
            // Use coroutine to ensure day manager state is fully updated
            StartCoroutine(RefreshUIAfterDayChange(newDay));
        }
        
        /// <summary>
        /// Refresh UI after day change with a slight delay to ensure state consistency
        /// </summary>
        private IEnumerator RefreshUIAfterDayChange(int expectedDay)
        {
            // Wait one frame to ensure day manager state is fully updated
            yield return null;
            
            Debug.Log($"[UICoordinator] *** RefreshUIAfterDayChange START *** Expected day: {expectedDay}");
            
            // Reconnect to day manager in case it was recreated
            if (dayManager == null)
            {
                Debug.Log("[UICoordinator] Day manager is null, attempting to reconnect...");
                dayManager = ServiceLocator.Get<DayProgressionManager>();
                if (dayManager == null)
                {
                    dayManager = FindFirstObjectByType<DayProgressionManager>();
                }
                
                if (dayManager != null)
                {
                    Debug.Log("[UICoordinator] Successfully reconnected to DayProgressionManager");
                    // Re-subscribe to events if we had to reconnect
                    DayProgressionManager.OnDayChanged += OnDayChanged;
                    DayProgressionManager.OnTimeUpdated += OnTimeUpdated;
                    DayProgressionManager.OnShipProcessed += OnShipProcessed;
                    DayProgressionManager.OnQuotaReached += OnQuotaReached;
                    DayProgressionManager.OnTimeWarning += OnTimeWarning;
                }
                else
                {
                    Debug.LogError("[UICoordinator] Failed to reconnect to DayProgressionManager!");
                }
            }
            
            if (enableDebugLogs)
                Debug.Log($"[UICoordinator] Refreshing UI after day change - Expected: {expectedDay}, Manager reports: {(dayManager != null ? dayManager.CurrentDay.ToString() : "NULL")}");
            
            // Force refresh all UI to ensure consistency
            RefreshAllUI();
            
            if (enableDebugLogs)
            {
                Debug.Log($"[UICoordinator] Day display updated to: {(dayText != null ? dayText.text : "dayText is null")}");
                if (dayManager != null)
                {
                    Debug.Log($"[UICoordinator] Current day from manager: {dayManager.CurrentDay}");
                    Debug.Log($"[UICoordinator] Ships display reset - Processed: {dayManager.ShipsProcessedToday}, Quota: {dayManager.GetCurrentDayQuota()}");
                    Debug.Log($"[UICoordinator] Ships text updated to: {(shipsProcessedText != null ? shipsProcessedText.text : "shipsProcessedText is null")}");
                }
            }
            
            Debug.Log($"[UICoordinator] *** RefreshUIAfterDayChange COMPLETED *** Day manager connected: {dayManager != null}");
        }
        
        private void OnTimeUpdated(float remainingTime)
        {
            UpdateTimeDisplay();
            UpdateProgressBars();
        }
        
        private void OnShipProcessed(int totalShips)
        {
            UpdateShipsDisplay();
            UpdateProgressBars();
        }
        
        private void OnQuotaReached()
        {
            UpdateStatusIndicators();
            ShowNotification("Daily quota reached! Excellent work!");
        }
        
        private void OnTimeWarning(string warning)
        {
            ShowNotification(warning);
        }
        
        private void OnGameStateChanged(Starkiller.Core.GameState newState)
        {
            UpdateStatusIndicators();
            
            // Handle specific state changes
            switch (newState)
            {
                case Starkiller.Core.GameState.GameOver:
                    ShowNotification("Game Over! Performance review in progress...");
                    break;
                case Starkiller.Core.GameState.DayReport:
                    ShowNotification("Shift complete! Generating daily report...");
                    break;
            }
        }
        
        /// <summary>
        /// Public utility methods
        /// </summary>
        public void ToggleDebugInfo()
        {
            showDebugInfo = !showDebugInfo;
            UpdateDebugInfo();
        }
        
        public void SetDebugInfoVisible(bool visible)
        {
            showDebugInfo = visible;
            UpdateDebugInfo();
        }
        
        public void ClearNotificationQueue()
        {
            notificationQueue.Clear();
        }
        
        /// <summary>
        /// Force update the day display - useful for debugging or manual refresh
        /// </summary>
        public void ForceUpdateDayDisplay()
        {
            if (enableDebugLogs)
                Debug.Log("[UICoordinator] ForceUpdateDayDisplay called");
            
            // Try to reconnect to day manager if it's missing
            if (dayManager == null)
            {
                dayManager = ServiceLocator.Get<DayProgressionManager>();
                if (dayManager == null)
                {
                    dayManager = FindFirstObjectByType<DayProgressionManager>();
                }
                
                if (enableDebugLogs)
                    Debug.Log($"[UICoordinator] Reconnection attempt - Day Manager: {(dayManager != null ? "Found" : "Still Missing")}");
            }
            
            UpdateDayDisplay();
        }
        
        /// <summary>
        /// Force update ships display - useful for debugging
        /// </summary>
        public void ForceUpdateShipsDisplay()
        {
            if (enableDebugLogs)
                Debug.Log("[UICoordinator] ForceUpdateShipsDisplay called");
            
            // Try to reconnect to day manager if it's missing
            if (dayManager == null)
            {
                dayManager = ServiceLocator.Get<DayProgressionManager>();
                if (dayManager == null)
                {
                    dayManager = FindFirstObjectByType<DayProgressionManager>();
                }
                
                if (enableDebugLogs)
                    Debug.Log($"[UICoordinator] Reconnection attempt for ships display - Day Manager: {(dayManager != null ? "Found" : "Still Missing")}");
            }
            
            UpdateShipsDisplay();
            
            if (enableDebugLogs && shipsProcessedText != null)
                Debug.Log($"[UICoordinator] Force ships display result: {shipsProcessedText.text}");
        }
        
        // === Panel Management Methods (from UIManager) ===
        
        /// <summary>
        /// Show main menu panel
        /// </summary>
        public void ShowMainMenu()
        {
            PlayButtonSound();
            ShowPanel(mainMenuPanel);
            HidePanel(gameplayPanel);
            HidePanel(gameOverPanel);
            HidePanel(creditsPanel);
            HidePanel(tutorialPanel);
            HidePanel(dailyReportPanel);
        }
        
        /// <summary>
        /// Show gameplay panel and start game
        /// </summary>
        public void StartGameUI()
        {
            PlayButtonSound();
            ShowPanel(gameplayPanel);
            HidePanel(mainMenuPanel);
            HidePanel(gameOverPanel);
            HidePanel(creditsPanel);
            HidePanel(tutorialPanel);
            HidePanel(dailyReportPanel);
            
            // Trigger game start through game state manager
            if (gameStateManager != null)
                gameStateManager.StartGame();
        }
        
        /// <summary>
        /// Show tutorial panel
        /// </summary>
        public void ShowTutorialPanel()
        {
            PlayButtonSound();
            ShowPanel(tutorialPanel);
            HidePanel(mainMenuPanel);
            HidePanel(gameplayPanel);
            HidePanel(gameOverPanel);
            HidePanel(creditsPanel);
            HidePanel(dailyReportPanel);
        }
        
        /// <summary>
        /// Show credits panel
        /// </summary>
        public void ShowCreditsPanel()
        {
            PlayButtonSound();
            ShowPanel(creditsPanel);
            HidePanel(mainMenuPanel);
            HidePanel(gameplayPanel);
            HidePanel(gameOverPanel);
            HidePanel(tutorialPanel);
            HidePanel(dailyReportPanel);
        }
        
        /// <summary>
        /// Show daily report panel
        /// </summary>
        public void ShowDailyReportPanel()
        {
            PlayButtonSound();
            ShowPanel(dailyReportPanel);
            HidePanel(mainMenuPanel);
            HidePanel(gameplayPanel);
            HidePanel(gameOverPanel);
            HidePanel(creditsPanel);
            HidePanel(tutorialPanel);
        }
        
        /// <summary>
        /// Show game over panel
        /// </summary>
        public void ShowGameOverPanel()
        {
            ShowPanel(gameOverPanel);
            HidePanel(gameplayPanel);
        }
        
        /// <summary>
        /// Quit the game
        /// </summary>
        public void QuitGame()
        {
            PlayButtonSound();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        private void ShowPanel(GameObject panel)
        {
            if (panel != null)
                panel.SetActive(true);
        }
        
        private void HidePanel(GameObject panel)
        {
            if (panel != null)
                panel.SetActive(false);
        }
        
        private void PlayButtonSound()
        {
            if (buttonClickSound != null)
                buttonClickSound.Play();
        }
        
        // === UI Manipulation Methods (from UIHelper) ===
        
        /// <summary>
        /// Change the color of a UI element by name
        /// </summary>
        public bool ChangeUIColor(string objectName, Color color)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
            {
                Debug.LogError($"[UICoordinator] Cannot find object '{objectName}'");
                return false;
            }
            
            Image image = obj.GetComponent<Image>();
            if (image != null)
            {
                image.color = color;
                if (enableDebugLogs)
                    Debug.Log($"[UICoordinator] Changed color of '{objectName}' to {color}");
                return true;
            }
            
            Debug.LogError($"[UICoordinator] Object '{objectName}' does not have an Image component");
            return false;
        }
        
        /// <summary>
        /// Change the text of a UI element by name
        /// </summary>
        public bool ChangeUIText(string objectName, string newText)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
            {
                Debug.LogError($"[UICoordinator] Cannot find object '{objectName}'");
                return false;
            }
            
            // Try TextMeshPro first
            TMP_Text tmpText = obj.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = newText;
                if (enableDebugLogs)
                    Debug.Log($"[UICoordinator] Changed TMP_Text of '{objectName}' to '{newText}'");
                return true;
            }
            
            // Try legacy Text
            Text legacyText = obj.GetComponent<Text>();
            if (legacyText != null)
            {
                legacyText.text = newText;
                if (enableDebugLogs)
                    Debug.Log($"[UICoordinator] Changed legacy Text of '{objectName}' to '{newText}'");
                return true;
            }
            
            Debug.LogError($"[UICoordinator] Object '{objectName}' has no text component");
            return false;
        }
        
        /// <summary>
        /// Toggle UI element visibility
        /// </summary>
        public bool ToggleUIElement(string objectName, bool? state = null)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
            {
                Debug.LogError($"[UICoordinator] Cannot find object '{objectName}'");
                return false;
            }
            
            if (state.HasValue)
            {
                obj.SetActive(state.Value);
            }
            else
            {
                obj.SetActive(!obj.activeSelf);
            }
            
            if (enableDebugLogs)
                Debug.Log($"[UICoordinator] Set active state of '{objectName}' to {obj.activeSelf}");
            
            return true;
        }
        
        /// <summary>
        /// Set button interactable state
        /// </summary>
        public bool SetButtonInteractable(string objectName, bool interactable)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
            {
                Debug.LogError($"[UICoordinator] Cannot find object '{objectName}'");
                return false;
            }
            
            Button button = obj.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = interactable;
                if (enableDebugLogs)
                    Debug.Log($"[UICoordinator] Set interactable state of button '{objectName}' to {interactable}");
                return true;
            }
            
            Debug.LogError($"[UICoordinator] Object '{objectName}' does not have a Button component");
            return false;
        }
        
        // === Static Convenience Methods ===
        
        private static UICoordinator instance;
        
        public static UICoordinator Instance
        {
            get
            {
                if (instance == null)
                    instance = ServiceLocator.Get<UICoordinator>();
                return instance;
            }
        }
        
        public static bool SetColor(string objectName, Color color)
        {
            return Instance?.ChangeUIColor(objectName, color) ?? false;
        }
        
        public static bool SetText(string objectName, string text)
        {
            return Instance?.ChangeUIText(objectName, text) ?? false;
        }
        
        public static bool Toggle(string objectName, bool? state = null)
        {
            return Instance?.ToggleUIElement(objectName, state) ?? false;
        }
        
        public static bool SetInteractable(string objectName, bool interactable)
        {
            return Instance?.SetButtonInteractable(objectName, interactable) ?? false;
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (creditsManager != null)
                CreditsManager.OnCreditsChanged -= OnCreditsChanged;
            
            if (decisionTracker != null)
            {
                DecisionTracker.OnStrikesChanged -= OnStrikesChanged;
                DecisionTracker.OnAccuracyChanged -= OnAccuracyChanged;
            }
            
            if (dayManager != null)
            {
                DayProgressionManager.OnDayChanged -= OnDayChanged;
                DayProgressionManager.OnTimeUpdated -= OnTimeUpdated;
                DayProgressionManager.OnShipProcessed -= OnShipProcessed;
                DayProgressionManager.OnQuotaReached -= OnQuotaReached;
                DayProgressionManager.OnTimeWarning -= OnTimeWarning;
            }
            
            GameEvents.OnGameStateChanged -= OnGameStateChanged;
            GameEvents.OnUINotification -= ShowNotification;
            GameEvents.OnUIRefreshRequested -= RefreshAllUI;
            
            // Clear event subscriptions
            OnNotificationShown = null;
            OnUIRefreshed = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Show Test Notification")]
        private void TestNotification() => ShowNotification("This is a test notification!");
        
        [ContextMenu("Test: Refresh All UI")]
        private void TestRefreshUI() => RefreshAllUI();
        
        [ContextMenu("Test: Toggle Debug Info")]
        private void TestToggleDebug() => ToggleDebugInfo();
        
        [ContextMenu("Test: Force Day Display Update")]
        private void TestDayDisplayUpdate()
        {
            Debug.Log("[UICoordinator] Manual day display update triggered");
            UpdateDayDisplay();
        }
        
        [ContextMenu("Test: Force Ships Display Update")]
        private void TestShipsDisplayUpdate()
        {
            Debug.Log("[UICoordinator] Manual ships display update triggered");
            ForceUpdateShipsDisplay();
        }
        
        [ContextMenu("Test: Simulate Day Change")]
        private void TestDayChange()
        {
            if (dayManager != null)
            {
                Debug.Log($"[UICoordinator] Current day before test: {dayManager.CurrentDay}");
                OnDayChanged(dayManager.CurrentDay + 1);
            }
            else
            {
                Debug.LogWarning("[UICoordinator] Cannot test day change - dayManager is null");
            }
        }
        
        [ContextMenu("Show UI Status")]
        private void ShowUIStatus()
        {
            Debug.Log("=== UI COORDINATOR STATUS ===");
            Debug.Log($"Credits Manager: {(creditsManager != null ? "Connected" : "Missing")}");
            Debug.Log($"Decision Tracker: {(decisionTracker != null ? "Connected" : "Missing")}");
            Debug.Log($"Day Manager: {(dayManager != null ? "Connected" : "Missing")}");
            Debug.Log($"Game State Manager: {(gameStateManager != null ? "Connected" : "Missing")}");
            Debug.Log($"Notifications Queued: {notificationQueue.Count}");
            Debug.Log($"Debug Info Visible: {showDebugInfo}");
            Debug.Log("=== END STATUS ===");
        }
    }
}