using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Starkiller.Core.Helpers;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages inspections, audits, and surveillance activities
    /// Extracted from GameManager for focused inspection responsibility
    /// </summary>
    public class InspectionManager : MonoBehaviour
    {
        [Header("Inspection Settings")]
        [SerializeField] private bool enableInspections = true;
        [SerializeField] private float inspectionDisplayTime = 5f;
        [SerializeField] private int suspicionThreshold = 3;
        [SerializeField] private int bribeDetectionThreshold = 2;
        [SerializeField] private bool enableRandomInspections = true;
        [SerializeField] private float randomInspectionChance = 0.1f; // 10% chance per day
        
        [Header("Inspection Triggers")]
        [SerializeField] private bool triggerOnBribery = true;
        [SerializeField] private bool triggerOnHighSympathy = true;
        [SerializeField] private bool triggerOnLowLoyalty = true;
        [SerializeField] private bool triggerOnSuspiciousActivity = true;
        
        [Header("Inspection Consequences")]
        [SerializeField] private int loyaltyPenaltyOnIrregularities = -3;
        [SerializeField] private int creditPenaltyOnIrregularities = 50;
        [SerializeField] private int strikePenaltyOnIrregularities = 1;
        [SerializeField] private bool enableInspectionPenalties = true;
        
        [Header("UI References")]
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private TMP_Text feedbackText;
        [SerializeField] private GameObject inspectionPanel;
        [SerializeField] private TMP_Text inspectionText;
        
        [Header("Enhanced Inspection UI")]
        [SerializeField] private TMP_Text inspectionTitleText;
        [SerializeField] private TMP_Text inspectionReasonText;
        [SerializeField] private TMP_Text inspectionStatusText;
        [SerializeField] private UnityEngine.Video.VideoPlayer inspectionVideoPlayer;
        [SerializeField] private UnityEngine.UI.RawImage videoDisplay;
        [SerializeField] private CanvasGroup inspectionCanvasGroup;
        
        [Header("Audio")]
        [SerializeField] private bool enableInspectionSounds = true;
        [SerializeField] private string inspectionStartSound = "inspection_start";
        [SerializeField] private string inspectionPassSound = "inspection_pass";
        [SerializeField] private string inspectionFailSound = "inspection_fail";
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableInspectionLogging = true;
        
        // Inspection state
        private bool _isInspectionActive = false;
        private bool _isProcessingInspection = false;
        private List<string> _keyDecisions = new List<string>();
        private List<InspectionRecord> _inspectionHistory = new List<InspectionRecord>();
        private int _totalInspections = 0;
        private int _passedInspections = 0;
        private int _failedInspections = 0;
        
        // Tracking variables
        private int _bribeCount = 0;
        private int _suspiciousDecisionCount = 0;
        private DateTime _lastInspectionTime = DateTime.MinValue;
        private int _consecutiveCleanInspections = 0;
        
        // Dependencies
        private LoyaltyManager _loyaltyManager;
        private CreditsManager _creditsManager;
        private PerformanceManager _performanceManager;
        private NotificationManager _notificationManager;
        private AudioManager _audioManager;
        private UICoordinator _uiCoordinator;
        private DayProgressionManager _dayManager;
        private BriberyManager _briberyManager;
        
        // Events
        public static event Action<InspectionType> OnInspectionTriggered;
        public static event Action<InspectionResult> OnInspectionCompleted;
        public static event Action<string> OnSuspiciousActivityDetected;
        public static event Action<InspectionRecord> OnInspectionRecorded;
        public static event Action<bool> OnInspectionStatusChanged;
        
        // Public properties
        public bool IsInspectionActive => _isInspectionActive;
        public int TotalInspections => _totalInspections;
        public int PassedInspections => _passedInspections;
        public int FailedInspections => _failedInspections;
        public float InspectionPassRate => _totalInspections > 0 ? (float)_passedInspections / _totalInspections : 0f;
        public List<string> KeyDecisions => new List<string>(_keyDecisions);
        public List<InspectionRecord> InspectionHistory => new List<InspectionRecord>(_inspectionHistory);
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<InspectionManager>(this);
            
            if (enableDebugLogs)
                Debug.Log("[InspectionManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _loyaltyManager = ServiceLocator.Get<LoyaltyManager>();
            _creditsManager = ServiceLocator.Get<CreditsManager>();
            _performanceManager = ServiceLocator.Get<PerformanceManager>();
            _notificationManager = ServiceLocator.Get<NotificationManager>();
            _audioManager = ServiceLocator.Get<AudioManager>();
            _uiCoordinator = ServiceLocator.Get<UICoordinator>();
            _dayManager = ServiceLocator.Get<DayProgressionManager>();
            _briberyManager = ServiceLocator.Get<BriberyManager>();
            
            // Subscribe to events
            if (_briberyManager != null)
            {
                BriberyManager.OnBribeAccepted += OnBribeAccepted;
                BriberyManager.OnBribeDetected += OnBribeDetected;
            }
            
            if (_loyaltyManager != null)
            {
                LoyaltyManager.OnLoyaltyChanged += OnLoyaltyChanged;
            }
            
            if (_dayManager != null)
            {
                DayProgressionManager.OnDayStarted += OnDayStarted;
            }
            
            // Try to find UI components if not assigned
            if (feedbackPanel == null)
                feedbackPanel = GameObject.Find("FeedbackPanel");
            
            if (feedbackText == null && feedbackPanel != null)
                feedbackText = feedbackPanel.GetComponentInChildren<TMP_Text>();
            
            // Disable inspection video auto-play to prevent background video on launch
            DisableInspectionVideoAutoPlay();
            
            if (enableDebugLogs)
                Debug.Log("[InspectionManager] Inspection system ready");
        }
        
        /// <summary>
        /// Disable inspection video auto-play to prevent background videos on launch
        /// </summary>
        private void DisableInspectionVideoAutoPlay()
        {
            if (inspectionVideoPlayer != null)
            {
                inspectionVideoPlayer.playOnAwake = false;
                inspectionVideoPlayer.Stop();
                
                if (enableDebugLogs)
                    Debug.Log("[InspectionManager] Disabled inspection video auto-play");
            }
            
            // Also ensure inspection panel is hidden on startup
            if (inspectionPanel != null && inspectionPanel.activeInHierarchy)
            {
                inspectionPanel.SetActive(false);
                
                if (enableDebugLogs)
                    Debug.Log("[InspectionManager] Hidden inspection panel on startup");
            }
        }
        
        /// <summary>
        /// Trigger an inspection with a specific reason
        /// </summary>
        public void TriggerInspection(InspectionType inspectionType, string reason = "")
        {
            TriggerInspection(inspectionType, reason, null);
        }
        
        /// <summary>
        /// Trigger an inspection with a specific scenario
        /// </summary>
        public void TriggerInspection(InspectionType inspectionType, string reason = "", ShipScenario inspectionScenario = null)
        {
            if (!enableInspections)
            {
                if (enableDebugLogs)
                    Debug.Log("[InspectionManager] Inspections disabled, skipping");
                return;
            }
            
            if (_isInspectionActive)
            {
                if (enableDebugLogs)
                    Debug.Log("[InspectionManager] Inspection already active, skipping");
                return;
            }
            
            StartCoroutine(ProcessInspection(inspectionType, reason, inspectionScenario));
        }
        
        /// <summary>
        /// Show inspection UI to the player
        /// </summary>
        public void ShowInspectionUI(string inspectionReason = "", Action<bool> onInspectionComplete = null, ShipScenario inspectionScenario = null)
        {
            if (_isInspectionActive)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[InspectionManager] Inspection already active");
                return;
            }
            
            _isInspectionActive = true;
            OnInspectionStatusChanged?.Invoke(true);
            
            // Pause game if game state manager is available
            var gameStateManager = ServiceLocator.Get<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.PauseGame();
            }
            
            // Use ScenarioMediaHelper if scenario is provided and available
            var scenarioHelper = ServiceLocator.Get<Starkiller.Core.Helpers.ScenarioMediaHelper>();
            if (scenarioHelper != null && inspectionScenario != null && inspectionScenario.IsInspectionScenario())
            {
                // Use the enhanced scenario system
                string customMessage = "⚠️ ATTENTION OFFICER: A surprise inspection of Imperium Command is underway.\n\nAll operations temporarily suspended.";
                if (!string.IsNullOrEmpty(inspectionReason))
                {
                    customMessage += $"\n\n📋 REASON: {inspectionReason}";
                }
                
                scenarioHelper.ShowScenario(inspectionScenario, customMessage, () =>
                {
                    CompleteInspection(DetermineInspectionResult(), onInspectionComplete);
                });
            }
            else
            {
                // Fall back to legacy system
                ShowLegacyInspectionUI(inspectionReason, onInspectionComplete);
            }
            
            // Play inspection sound
            if (_audioManager != null && enableInspectionSounds)
            {
                _audioManager.PlaySound(inspectionStartSound);
            }
            
            if (enableInspectionLogging)
            {
                Debug.Log($"[InspectionManager] Inspection UI displayed: {inspectionReason}");
            }
        }
        
        /// <summary>
        /// Legacy inspection UI system (fallback)
        /// </summary>
        private void ShowLegacyInspectionUI(string inspectionReason, Action<bool> onInspectionComplete)
        {
            // Use dedicated inspection panel if available, otherwise fall back to feedback panel
            GameObject panelToUse = inspectionPanel != null ? inspectionPanel : feedbackPanel;
            TMP_Text textToUse = inspectionText != null ? inspectionText : feedbackText;
            
            if (panelToUse == null)
            {
                Debug.LogError("[InspectionManager] No UI panel available for inspection display");
                CompleteInspection(false, onInspectionComplete);
                return;
            }
            
            if (textToUse == null)
            {
                Debug.LogError("[InspectionManager] No text component found for inspection display");
                CompleteInspection(false, onInspectionComplete);
                return;
            }
            
            // Show the panel
            panelToUse.SetActive(true);
            
            // Set inspection message using enhanced UI if available
            if (inspectionTitleText != null)
            {
                inspectionTitleText.text = "IMPERIAL INSPECTION";
            }
            
            string message = "⚠️ ATTENTION OFFICER: A surprise inspection of Imperium Command is underway.\n\nAll operations temporarily suspended.";
            if (!string.IsNullOrEmpty(inspectionReason))
            {
                message += $"\n\n📋 REASON: {inspectionReason}";
            }
            textToUse.text = message;
            
            // Set reason text separately if available
            if (inspectionReasonText != null)
            {
                inspectionReasonText.text = !string.IsNullOrEmpty(inspectionReason) ? inspectionReason : "Routine compliance check";
            }
            
            // Set status text if available
            if (inspectionStatusText != null)
            {
                inspectionStatusText.text = "IN PROGRESS";
            }
            
            // Auto-dismiss after delay
            StartCoroutine(DismissInspectionUI(inspectionDisplayTime, onInspectionComplete));
        }
        
        /// <summary>
        /// Process an inspection from start to finish
        /// </summary>
        private IEnumerator ProcessInspection(InspectionType inspectionType, string reason, ShipScenario inspectionScenario = null)
        {
            if (_isProcessingInspection)
                yield break;
            
            _isProcessingInspection = true;
            
            // Trigger inspection event
            OnInspectionTriggered?.Invoke(inspectionType);
            
            // Show inspection UI
            bool inspectionResult = false;
            bool inspectionCompleted = false;
            
            ShowInspectionUI(reason, (result) =>
            {
                inspectionResult = result;
                inspectionCompleted = true;
            }, inspectionScenario);
            
            // Wait for inspection to complete
            while (!inspectionCompleted)
            {
                yield return null;
            }
            
            // Process inspection result
            var record = new InspectionRecord
            {
                Type = inspectionType,
                Date = DateTime.Now,
                Reason = reason,
                IrregularitiesFound = inspectionResult,
                KeyDecisionsAtTime = new List<string>(_keyDecisions),
                LoyaltyLevel = _loyaltyManager != null ? _loyaltyManager.ImperialLoyalty : 0,
                SympathyLevel = _loyaltyManager != null ? _loyaltyManager.RebellionSympathy : 0
            };
            
            // Update statistics
            _totalInspections++;
            if (inspectionResult)
            {
                _failedInspections++;
                _consecutiveCleanInspections = 0;
            }
            else
            {
                _passedInspections++;
                _consecutiveCleanInspections++;
            }
            
            // Apply consequences
            if (inspectionResult && enableInspectionPenalties)
            {
                ApplyInspectionPenalties(record);
            }
            else if (!inspectionResult)
            {
                ApplyInspectionReward(record);
            }
            
            // Record inspection
            _inspectionHistory.Add(record);
            OnInspectionRecorded?.Invoke(record);
            
            // Update last inspection time
            _lastInspectionTime = DateTime.Now;
            
            // Complete inspection
            var result = new InspectionResult
            {
                Type = inspectionType,
                IrregularitiesFound = inspectionResult,
                Record = record
            };
            
            OnInspectionCompleted?.Invoke(result);
            
            _isProcessingInspection = false;
            
            if (enableInspectionLogging)
            {
                Debug.Log($"[InspectionManager] Inspection completed: {inspectionType} - {(inspectionResult ? "FAILED" : "PASSED")}");
            }
        }
        
        /// <summary>
        /// Dismiss inspection UI after delay
        /// </summary>
        private IEnumerator DismissInspectionUI(float delay, Action<bool> onInspectionComplete)
        {
            yield return new WaitForSeconds(delay);
            
            // Hide inspection UI
            GameObject panelToHide = inspectionPanel != null ? inspectionPanel : feedbackPanel;
            if (panelToHide != null)
            {
                panelToHide.SetActive(false);
            }
            
            // Determine inspection result
            bool irregularitiesFound = DetermineInspectionResult();
            
            // Complete inspection
            CompleteInspection(irregularitiesFound, onInspectionComplete);
        }
        
        /// <summary>
        /// Complete the inspection process
        /// </summary>
        private void CompleteInspection(bool irregularitiesFound, Action<bool> onInspectionComplete)
        {
            // Resume game
            var gameStateManager = ServiceLocator.Get<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.ResumeGame();
            }
            
            _isInspectionActive = false;
            OnInspectionStatusChanged?.Invoke(false);
            
            // Invoke completion callback
            onInspectionComplete?.Invoke(irregularitiesFound);
        }
        
        /// <summary>
        /// Determine inspection result based on player actions
        /// </summary>
        private bool DetermineInspectionResult()
        {
            // Check for direct evidence of bribery
            if (_keyDecisions.Any(d => d.Contains("Accepted a bribe")))
            {
                return true;
            }
            
            // Check rebellion sympathy level
            if (_loyaltyManager != null && _loyaltyManager.RebellionSympathy > suspicionThreshold)
            {
                return true;
            }
            
            // Check imperial loyalty level
            if (_loyaltyManager != null && _loyaltyManager.ImperialLoyalty < -suspicionThreshold)
            {
                return true;
            }
            
            // Check for suspicious decision patterns
            int suspiciousCount = 0;
            foreach (string decision in _keyDecisions)
            {
                if (decision.Contains("Bent the rules") || 
                    decision.Contains("insurgent sympathiser") || 
                    decision.Contains("personal reasons"))
                {
                    suspiciousCount++;
                }
            }
            
            if (suspiciousCount >= bribeDetectionThreshold)
            {
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Apply penalties for failed inspection
        /// </summary>
        private void ApplyInspectionPenalties(InspectionRecord record)
        {
            // Apply loyalty penalty
            if (_loyaltyManager != null)
            {
                _loyaltyManager.UpdateLoyalty(loyaltyPenaltyOnIrregularities, 0, "Failed inspection - irregularities found");
            }
            
            // Apply credit penalty
            if (_creditsManager != null)
            {
                _creditsManager.DeductCredits(creditPenaltyOnIrregularities, "Inspection penalty - irregularities found");
            }
            
            // Apply performance strike
            if (_performanceManager != null)
            {
                for (int i = 0; i < strikePenaltyOnIrregularities; i++)
                {
                    // Add strike through DecisionTracker if available
                    var decisionTracker = ServiceLocator.Get<DecisionTracker>();
                    if (decisionTracker != null)
                    {
                        decisionTracker.AddStrike("Failed inspection");
                    }
                }
            }
            
            // Play fail sound
            if (_audioManager != null && enableInspectionSounds)
            {
                _audioManager.PlaySound(inspectionFailSound);
            }
            
            // Show notification
            if (_notificationManager != null)
            {
                _notificationManager.ShowNotification(
                    $"🚨 INSPECTION FAILED - Irregularities found! -{creditPenaltyOnIrregularities} credits",
                    NotificationType.Error
                );
            }
        }
        
        /// <summary>
        /// Apply rewards for passed inspection
        /// </summary>
        private void ApplyInspectionReward(InspectionRecord record)
        {
            // Small loyalty bonus for clean inspection
            if (_loyaltyManager != null)
            {
                _loyaltyManager.UpdateLoyalty(1, 0, "Passed inspection - clean record");
            }
            
            // Bonus for consecutive clean inspections
            if (_consecutiveCleanInspections >= 3)
            {
                int bonus = _consecutiveCleanInspections * 5;
                if (_creditsManager != null)
                {
                    _creditsManager.AddCredits(bonus, "Consecutive clean inspections bonus");
                }
            }
            
            // Play pass sound
            if (_audioManager != null && enableInspectionSounds)
            {
                _audioManager.PlaySound(inspectionPassSound);
            }
            
            // Show notification
            if (_notificationManager != null)
            {
                string message = _consecutiveCleanInspections >= 3 ? 
                    $"✅ INSPECTION PASSED - Clean record! +{_consecutiveCleanInspections * 5} credits bonus" :
                    "✅ INSPECTION PASSED - Clean record maintained";
                
                _notificationManager.ShowNotification(message, NotificationType.Success);
            }
        }
        
        /// <summary>
        /// Add a key decision to the inspection tracking
        /// </summary>
        public void AddKeyDecision(string decision)
        {
            if (string.IsNullOrEmpty(decision))
                return;
            
            _keyDecisions.Add(decision);
            
            // Check if this decision should trigger an inspection
            CheckForInspectionTriggers(decision);
            
            if (enableDebugLogs)
                Debug.Log($"[InspectionManager] Key decision added: {decision}");
        }
        
        /// <summary>
        /// Check if a decision should trigger an inspection
        /// </summary>
        private void CheckForInspectionTriggers(string decision)
        {
            if (!enableInspections || _isInspectionActive)
                return;
            
            // Bribery trigger
            if (triggerOnBribery && decision.Contains("Accepted a bribe"))
            {
                _bribeCount++;
                if (_bribeCount >= bribeDetectionThreshold)
                {
                    TriggerInspection(InspectionType.Bribery, "Multiple bribery incidents detected");
                }
            }
            
            // Suspicious activity trigger
            if (triggerOnSuspiciousActivity && 
                (decision.Contains("Bent the rules") || decision.Contains("insurgent sympathiser")))
            {
                _suspiciousDecisionCount++;
                if (_suspiciousDecisionCount >= suspicionThreshold)
                {
                    TriggerInspection(InspectionType.SuspiciousActivity, "Pattern of suspicious decisions detected");
                }
            }
            
            OnSuspiciousActivityDetected?.Invoke(decision);
        }
        
        /// <summary>
        /// Trigger a random inspection
        /// </summary>
        public void TriggerRandomInspection()
        {
            if (!enableRandomInspections || _isInspectionActive)
                return;
            
            if (UnityEngine.Random.value < randomInspectionChance)
            {
                TriggerInspection(InspectionType.Random, "Routine compliance check");
            }
        }
        
        /// <summary>
        /// Get inspection statistics
        /// </summary>
        public InspectionStatistics GetStatistics()
        {
            return new InspectionStatistics
            {
                TotalInspections = _totalInspections,
                PassedInspections = _passedInspections,
                FailedInspections = _failedInspections,
                PassRate = InspectionPassRate,
                ConsecutiveCleanInspections = _consecutiveCleanInspections,
                LastInspectionTime = _lastInspectionTime,
                TotalKeyDecisions = _keyDecisions.Count,
                BribeCount = _bribeCount,
                SuspiciousDecisionCount = _suspiciousDecisionCount,
                InspectionHistory = new List<InspectionRecord>(_inspectionHistory)
            };
        }
        
        /// <summary>
        /// Reset inspection tracking (for new game)
        /// </summary>
        public void ResetInspectionTracking()
        {
            _keyDecisions.Clear();
            _inspectionHistory.Clear();
            _totalInspections = 0;
            _passedInspections = 0;
            _failedInspections = 0;
            _bribeCount = 0;
            _suspiciousDecisionCount = 0;
            _consecutiveCleanInspections = 0;
            _lastInspectionTime = DateTime.MinValue;
            _isInspectionActive = false;
            _isProcessingInspection = false;
            
            if (enableDebugLogs)
                Debug.Log("[InspectionManager] Inspection tracking reset");
        }
        
        // Event handlers
        private void OnBribeAccepted(BriberyRecord record)
        {
            AddKeyDecision($"Accepted a bribe of {record.Amount} credits");
        }
        
        private void OnBribeDetected()
        {
            TriggerInspection(InspectionType.Bribery, "Bribery activity detected by surveillance");
        }
        
        private void OnLoyaltyChanged(int imperialLoyalty, int rebellionSympathy)
        {
            // Trigger inspection if loyalty drops too low
            if (triggerOnLowLoyalty && imperialLoyalty < -suspicionThreshold && !_isInspectionActive)
            {
                TriggerInspection(InspectionType.LoyaltyCheck, "Imperial loyalty concerns raised");
            }
            
            // Trigger inspection if rebellion sympathy gets too high
            if (triggerOnHighSympathy && rebellionSympathy > suspicionThreshold && !_isInspectionActive)
            {
                TriggerInspection(InspectionType.SympathyCheck, "Rebellion sympathy concerns raised");
            }
        }
        
        private void OnDayStarted(int day)
        {
            // Reset daily counters
            _bribeCount = 0;
            _suspiciousDecisionCount = 0;
            
            // Chance for random inspection
            TriggerRandomInspection();
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_briberyManager != null)
            {
                BriberyManager.OnBribeAccepted -= OnBribeAccepted;
                BriberyManager.OnBribeDetected -= OnBribeDetected;
            }
            
            if (_loyaltyManager != null)
            {
                LoyaltyManager.OnLoyaltyChanged -= OnLoyaltyChanged;
            }
            
            if (_dayManager != null)
            {
                DayProgressionManager.OnDayStarted -= OnDayStarted;
            }
            
            // Clear event subscriptions
            OnInspectionTriggered = null;
            OnInspectionCompleted = null;
            OnSuspiciousActivityDetected = null;
            OnInspectionRecorded = null;
            OnInspectionStatusChanged = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Trigger Random Inspection")]
        private void TestTriggerRandomInspection()
        {
            TriggerInspection(InspectionType.Random, "Test inspection");
        }
        
        [ContextMenu("Test: Add Suspicious Decision")]
        private void TestAddSuspiciousDecision()
        {
            AddKeyDecision("Accepted a bribe of 50 credits");
        }
        
        [ContextMenu("Show Inspection Statistics")]
        private void ShowInspectionStatistics()
        {
            var stats = GetStatistics();
            Debug.Log("=== INSPECTION STATISTICS ===");
            Debug.Log($"Total Inspections: {stats.TotalInspections}");
            Debug.Log($"Passed: {stats.PassedInspections}");
            Debug.Log($"Failed: {stats.FailedInspections}");
            Debug.Log($"Pass Rate: {stats.PassRate:P1}");
            Debug.Log($"Consecutive Clean: {stats.ConsecutiveCleanInspections}");
            Debug.Log($"Total Key Decisions: {stats.TotalKeyDecisions}");
            Debug.Log($"Bribe Count: {stats.BribeCount}");
            Debug.Log($"Suspicious Count: {stats.SuspiciousDecisionCount}");
            Debug.Log("=== END STATISTICS ===");
        }
    }
    
    // Supporting data structures
    public enum InspectionType
    {
        Random,
        Bribery,
        SuspiciousActivity,
        LoyaltyCheck,
        SympathyCheck,
        Scheduled,
        Emergency
    }
    
    [System.Serializable]
    public class InspectionRecord
    {
        public InspectionType Type;
        public DateTime Date;
        public string Reason;
        public bool IrregularitiesFound;
        public List<string> KeyDecisionsAtTime;
        public int LoyaltyLevel;
        public int SympathyLevel;
    }
    
    [System.Serializable]
    public struct InspectionResult
    {
        public InspectionType Type;
        public bool IrregularitiesFound;
        public InspectionRecord Record;
    }
    
    [System.Serializable]
    public struct InspectionStatistics
    {
        public int TotalInspections;
        public int PassedInspections;
        public int FailedInspections;
        public float PassRate;
        public int ConsecutiveCleanInspections;
        public DateTime LastInspectionTime;
        public int TotalKeyDecisions;
        public int BribeCount;
        public int SuspiciousDecisionCount;
        public List<InspectionRecord> InspectionHistory;
    }
}