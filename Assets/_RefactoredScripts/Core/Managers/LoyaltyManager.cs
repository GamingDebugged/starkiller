using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages player loyalty to Imperial and Rebellion factions
    /// Extracted from GameManager for focused loyalty responsibility
    /// </summary>
    public class LoyaltyManager : MonoBehaviour
    {
        [Header("Loyalty Settings")]
        [SerializeField] private int minLoyaltyValue = -10;
        [SerializeField] private int maxLoyaltyValue = 10;
        [SerializeField] private int startingImperialLoyalty = 0;
        [SerializeField] private int startingRebellionSympathy = 0;
        [SerializeField] private bool enableLoyaltyFeedback = true;
        [SerializeField] private int significantChangeThreshold = 2;
        
        [Header("Loyalty Thresholds")]
        [SerializeField] private int highImperialLoyalty = 5;
        [SerializeField] private int highRebellionSympathy = 5;
        [SerializeField] private int extremeImperialLoyalty = 8;
        [SerializeField] private int extremeRebellionSympathy = 8;
        
        [Header("Decision Tracking")]
        [SerializeField] private bool trackKeyDecisions = true;
        [SerializeField] private int maxKeyDecisions = 50;
        [SerializeField] private bool enableDecisionAnalytics = true;
        
        [Header("Feedback Settings")]
        [SerializeField] private bool enableVisualFeedback = true;
        [SerializeField] private float feedbackDisplayTime = 3f;
        [SerializeField] private Color imperialColor = Color.blue;
        [SerializeField] private Color rebellionColor = Color.red;
        [SerializeField] private Color neutralColor = Color.yellow;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableLoyaltyLogging = true;
        [SerializeField] private bool enableDecisionLogging = true;
        
        // Loyalty state
        private int _imperialLoyalty = 0;
        private int _rebellionSympathy = 0;
        private LoyaltyAlignment _currentAlignment = LoyaltyAlignment.Neutral;
        
        // Decision tracking
        private List<string> _keyDecisions = new List<string>();
        private List<LoyaltyDecision> _decisionHistory = new List<LoyaltyDecision>();
        private Dictionary<string, int> _decisionCounts = new Dictionary<string, int>();
        
        // Statistics
        private int _totalLoyaltyChanges = 0;
        private int _imperialLoyaltyChanges = 0;
        private int _rebellionSympathyChanges = 0;
        private float _averageImperialChange = 0f;
        private float _averageRebellionChange = 0f;
        
        // Session tracking
        private int _sessionImperialGain = 0;
        private int _sessionImperialLoss = 0;
        private int _sessionRebellionGain = 0;
        private int _sessionRebellionLoss = 0;
        
        // Dependencies
        private NotificationManager _notificationManager;
        private UICoordinator _uiCoordinator;
        private AudioManager _audioManager;
        private GameStateManager _gameStateManager;
        
        // Events
        public static event Action<int, int> OnLoyaltyChanged; // (imperial, rebellion)
        public static event Action<LoyaltyAlignment> OnAlignmentChanged;
        public static event Action<string> OnKeyDecisionAdded;
        public static event Action<LoyaltyDecision> OnLoyaltyDecisionRecorded;
        public static event Action<LoyaltyThreshold> OnLoyaltyThresholdReached;
        public static event Action<string, Color> OnLoyaltyFeedback;
        
        // Public properties
        public int ImperialLoyalty => _imperialLoyalty;
        public int RebellionSympathy => _rebellionSympathy;
        public LoyaltyAlignment CurrentAlignment => _currentAlignment;
        public bool IsImperialLoyal => _imperialLoyalty > highImperialLoyalty && _rebellionSympathy < 0;
        public bool IsRebellionSympathizer => _rebellionSympathy > highRebellionSympathy && _imperialLoyalty < 0;
        public bool IsExtremistImperial => _imperialLoyalty >= extremeImperialLoyalty;
        public bool IsExtremistRebellion => _rebellionSympathy >= extremeRebellionSympathy;
        public bool IsNeutral => Mathf.Abs(_imperialLoyalty) <= 2 && Mathf.Abs(_rebellionSympathy) <= 2;
        public List<string> KeyDecisions => new List<string>(_keyDecisions);
        public int TotalLoyaltyChanges => _totalLoyaltyChanges;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<LoyaltyManager>(this);
            
            // Initialize loyalty values
            _imperialLoyalty = startingImperialLoyalty;
            _rebellionSympathy = startingRebellionSympathy;
            _currentAlignment = CalculateAlignment();
            
            if (enableDebugLogs)
                Debug.Log("[LoyaltyManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _notificationManager = ServiceLocator.Get<NotificationManager>();
            _uiCoordinator = ServiceLocator.Get<UICoordinator>();
            _audioManager = ServiceLocator.Get<AudioManager>();
            _gameStateManager = ServiceLocator.Get<GameStateManager>();
            
            // Subscribe to events
            GameEvents.OnDecisionMade += OnDecisionMade;
            
            if (enableDebugLogs)
                Debug.Log($"[LoyaltyManager] Loyalty system ready - Imperial: {_imperialLoyalty}, Rebellion: {_rebellionSympathy}");
        }
        
        /// <summary>
        /// Update loyalty values with bounds checking
        /// </summary>
        public void UpdateLoyalty(int imperialChange, int rebellionChange, string reason = "")
        {
            // Store previous values
            int previousImperial = _imperialLoyalty;
            int previousRebellion = _rebellionSympathy;
            LoyaltyAlignment previousAlignment = _currentAlignment;
            
            // Update loyalty values
            _imperialLoyalty += imperialChange;
            _rebellionSympathy += rebellionChange;
            
            // Clamp values to prevent extreme swings
            _imperialLoyalty = Mathf.Clamp(_imperialLoyalty, minLoyaltyValue, maxLoyaltyValue);
            _rebellionSympathy = Mathf.Clamp(_rebellionSympathy, minLoyaltyValue, maxLoyaltyValue);
            
            // Update statistics
            UpdateStatistics(imperialChange, rebellionChange);
            
            // Calculate new alignment
            _currentAlignment = CalculateAlignment();
            
            // Record decision
            if (trackKeyDecisions && !string.IsNullOrEmpty(reason))
            {
                RecordLoyaltyDecision(imperialChange, rebellionChange, reason);
            }
            
            // Check for threshold changes
            CheckLoyaltyThresholds(previousImperial, previousRebellion);
            
            // Show feedback if significant change
            if (enableLoyaltyFeedback && (Mathf.Abs(imperialChange) >= significantChangeThreshold || 
                                         Mathf.Abs(rebellionChange) >= significantChangeThreshold))
            {
                ShowLoyaltyFeedback(imperialChange, rebellionChange, reason);
            }
            
            // Trigger events
            OnLoyaltyChanged?.Invoke(_imperialLoyalty, _rebellionSympathy);
            
            if (_currentAlignment != previousAlignment)
            {
                OnAlignmentChanged?.Invoke(_currentAlignment);
            }
            
            if (enableLoyaltyLogging)
            {
                Debug.Log($"[LoyaltyManager] Loyalty updated - Imperial: {_imperialLoyalty} ({imperialChange:+0;-0;0}), " +
                         $"Rebellion: {_rebellionSympathy} ({rebellionChange:+0;-0;0}) - {reason}");
            }
        }
        
        /// <summary>
        /// Calculate current loyalty alignment
        /// </summary>
        private LoyaltyAlignment CalculateAlignment()
        {
            if (IsExtremistImperial)
                return LoyaltyAlignment.ExtremistImperial;
            else if (IsExtremistRebellion)
                return LoyaltyAlignment.ExtremistRebellion;
            else if (IsImperialLoyal)
                return LoyaltyAlignment.ImperialLoyal;
            else if (IsRebellionSympathizer)
                return LoyaltyAlignment.RebellionSympathizer;
            else if (_imperialLoyalty > _rebellionSympathy + 2)
                return LoyaltyAlignment.ImperialLeaning;
            else if (_rebellionSympathy > _imperialLoyalty + 2)
                return LoyaltyAlignment.RebellionLeaning;
            else
                return LoyaltyAlignment.Neutral;
        }
        
        /// <summary>
        /// Add a key decision to the record
        /// </summary>
        public void AddKeyDecision(string decision)
        {
            if (string.IsNullOrEmpty(decision)) return;
            
            _keyDecisions.Add(decision);
            
            // Maintain maximum size
            if (_keyDecisions.Count > maxKeyDecisions)
            {
                _keyDecisions.RemoveAt(0);
            }
            
            // Update decision counts
            if (_decisionCounts.ContainsKey(decision))
            {
                _decisionCounts[decision]++;
            }
            else
            {
                _decisionCounts[decision] = 1;
            }
            
            OnKeyDecisionAdded?.Invoke(decision);
            
            if (enableDecisionLogging)
            {
                Debug.Log($"[LoyaltyManager] Key decision added: {decision}");
            }
        }
        
        /// <summary>
        /// Record a loyalty decision
        /// </summary>
        private void RecordLoyaltyDecision(int imperialChange, int rebellionChange, string reason)
        {
            var decision = new LoyaltyDecision
            {
                Id = System.Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now,
                ImperialChange = imperialChange,
                RebellionChange = rebellionChange,
                Reason = reason,
                ResultingImperialLoyalty = _imperialLoyalty,
                ResultingRebellionSympathy = _rebellionSympathy,
                ResultingAlignment = _currentAlignment
            };
            
            _decisionHistory.Add(decision);
            OnLoyaltyDecisionRecorded?.Invoke(decision);
        }
        
        /// <summary>
        /// Update loyalty statistics
        /// </summary>
        private void UpdateStatistics(int imperialChange, int rebellionChange)
        {
            _totalLoyaltyChanges++;
            
            if (imperialChange != 0)
            {
                _imperialLoyaltyChanges++;
                _averageImperialChange = (_averageImperialChange * (_imperialLoyaltyChanges - 1) + imperialChange) / _imperialLoyaltyChanges;
                
                if (imperialChange > 0)
                    _sessionImperialGain += imperialChange;
                else
                    _sessionImperialLoss += Math.Abs(imperialChange);
            }
            
            if (rebellionChange != 0)
            {
                _rebellionSympathyChanges++;
                _averageRebellionChange = (_averageRebellionChange * (_rebellionSympathyChanges - 1) + rebellionChange) / _rebellionSympathyChanges;
                
                if (rebellionChange > 0)
                    _sessionRebellionGain += rebellionChange;
                else
                    _sessionRebellionLoss += Math.Abs(rebellionChange);
            }
        }
        
        /// <summary>
        /// Check for loyalty threshold changes
        /// </summary>
        private void CheckLoyaltyThresholds(int previousImperial, int previousRebellion)
        {
            // Check imperial thresholds
            if (previousImperial < highImperialLoyalty && _imperialLoyalty >= highImperialLoyalty)
            {
                OnLoyaltyThresholdReached?.Invoke(LoyaltyThreshold.HighImperialLoyalty);
            }
            else if (previousImperial >= highImperialLoyalty && _imperialLoyalty < highImperialLoyalty)
            {
                OnLoyaltyThresholdReached?.Invoke(LoyaltyThreshold.LostImperialLoyalty);
            }
            
            // Check rebellion thresholds
            if (previousRebellion < highRebellionSympathy && _rebellionSympathy >= highRebellionSympathy)
            {
                OnLoyaltyThresholdReached?.Invoke(LoyaltyThreshold.HighRebellionSympathy);
            }
            else if (previousRebellion >= highRebellionSympathy && _rebellionSympathy < highRebellionSympathy)
            {
                OnLoyaltyThresholdReached?.Invoke(LoyaltyThreshold.LostRebellionSympathy);
            }
            
            // Check extremist thresholds
            if (previousImperial < extremeImperialLoyalty && _imperialLoyalty >= extremeImperialLoyalty)
            {
                OnLoyaltyThresholdReached?.Invoke(LoyaltyThreshold.ExtremistImperial);
            }
            
            if (previousRebellion < extremeRebellionSympathy && _rebellionSympathy >= extremeRebellionSympathy)
            {
                OnLoyaltyThresholdReached?.Invoke(LoyaltyThreshold.ExtremistRebellion);
            }
        }
        
        /// <summary>
        /// Show loyalty feedback to player
        /// </summary>
        private void ShowLoyaltyFeedback(int imperialChange, int rebellionChange, string reason)
        {
            string message = "";
            Color feedbackColor = neutralColor;
            NotificationType notificationType = NotificationType.Info;
            
            // Imperial feedback
            if (imperialChange >= significantChangeThreshold)
            {
                message = "Your superiors are pleased with your decision.";
                feedbackColor = imperialColor;
                notificationType = NotificationType.Success;
            }
            else if (imperialChange <= -significantChangeThreshold)
            {
                message = "The Imperium is displeased with your actions.";
                feedbackColor = imperialColor;
                notificationType = NotificationType.Warning;
            }
            
            // Rebellion feedback
            if (rebellionChange >= significantChangeThreshold)
            {
                if (!string.IsNullOrEmpty(message))
                    message += " ";
                message += "The resistance will remember this.";
                feedbackColor = rebellionColor;
                notificationType = NotificationType.Info;
            }
            else if (rebellionChange <= -significantChangeThreshold)
            {
                if (!string.IsNullOrEmpty(message))
                    message += " ";
                message += "The resistance questions your loyalty.";
                feedbackColor = rebellionColor;
                notificationType = NotificationType.Warning;
            }
            
            // Show feedback
            if (!string.IsNullOrEmpty(message))
            {
                // Show notification
                if (_notificationManager != null)
                {
                    _notificationManager.ShowNotification(message, notificationType);
                }
                
                // Show additional UI notification if visual feedback is enabled
                if (_uiCoordinator != null && enableVisualFeedback)
                {
                    // UICoordinator can show notifications as well
                    _uiCoordinator.ShowNotification(message);
                }
                
                // Trigger feedback event
                OnLoyaltyFeedback?.Invoke(message, feedbackColor);
            }
        }
        
        /// <summary>
        /// Get loyalty description for UI
        /// </summary>
        public string GetLoyaltyDescription()
        {
            return _currentAlignment switch
            {
                LoyaltyAlignment.ExtremistImperial => "Fanatic Imperial Loyalist",
                LoyaltyAlignment.ImperialLoyal => "Imperial Loyalist",
                LoyaltyAlignment.ImperialLeaning => "Imperial Sympathizer",
                LoyaltyAlignment.Neutral => "Neutral Operator",
                LoyaltyAlignment.RebellionLeaning => "Rebellion Sympathizer",
                LoyaltyAlignment.RebellionSympathizer => "Rebellion Supporter",
                LoyaltyAlignment.ExtremistRebellion => "Rebellion Extremist",
                _ => "Unknown"
            };
        }
        
        /// <summary>
        /// Get loyalty color for UI
        /// </summary>
        public Color GetLoyaltyColor()
        {
            return _currentAlignment switch
            {
                LoyaltyAlignment.ExtremistImperial or LoyaltyAlignment.ImperialLoyal or LoyaltyAlignment.ImperialLeaning => imperialColor,
                LoyaltyAlignment.ExtremistRebellion or LoyaltyAlignment.RebellionSympathizer or LoyaltyAlignment.RebellionLeaning => rebellionColor,
                _ => neutralColor
            };
        }
        
        /// <summary>
        /// Get ending text based on loyalty
        /// </summary>
        public string GetEndingText()
        {
            if (IsImperialLoyal)
            {
                return "You remained loyal to the Imperium until the end.";
            }
            else if (IsRebellionSympathizer)
            {
                return "Your sympathy for the Insurgency has been noted in your permanent record.";
            }
            else if (IsExtremistImperial)
            {
                return "Your unwavering devotion to the Imperium has been recognized at the highest levels.";
            }
            else if (IsExtremistRebellion)
            {
                return "The Rebellion has marked you as a valuable asset for future operations.";
            }
            else
            {
                return "You maintained a balanced perspective throughout your service.";
            }
        }
        
        /// <summary>
        /// Reset loyalty values (for new game)
        /// </summary>
        public void ResetLoyalty()
        {
            _imperialLoyalty = startingImperialLoyalty;
            _rebellionSympathy = startingRebellionSympathy;
            _currentAlignment = CalculateAlignment();
            _keyDecisions.Clear();
            _decisionHistory.Clear();
            _decisionCounts.Clear();
            
            // Reset statistics
            _totalLoyaltyChanges = 0;
            _imperialLoyaltyChanges = 0;
            _rebellionSympathyChanges = 0;
            _averageImperialChange = 0f;
            _averageRebellionChange = 0f;
            _sessionImperialGain = 0;
            _sessionImperialLoss = 0;
            _sessionRebellionGain = 0;
            _sessionRebellionLoss = 0;
            
            OnLoyaltyChanged?.Invoke(_imperialLoyalty, _rebellionSympathy);
            OnAlignmentChanged?.Invoke(_currentAlignment);
            
            if (enableDebugLogs)
                Debug.Log("[LoyaltyManager] Loyalty system reset");
        }
        
        /// <summary>
        /// Get loyalty statistics
        /// </summary>
        public LoyaltyStatistics GetStatistics()
        {
            return new LoyaltyStatistics
            {
                ImperialLoyalty = _imperialLoyalty,
                RebellionSympathy = _rebellionSympathy,
                CurrentAlignment = _currentAlignment,
                TotalLoyaltyChanges = _totalLoyaltyChanges,
                AverageImperialChange = _averageImperialChange,
                AverageRebellionChange = _averageRebellionChange,
                SessionImperialGain = _sessionImperialGain,
                SessionImperialLoss = _sessionImperialLoss,
                SessionRebellionGain = _sessionRebellionGain,
                SessionRebellionLoss = _sessionRebellionLoss,
                TotalKeyDecisions = _keyDecisions.Count,
                MostCommonDecision = _decisionCounts.OrderByDescending(x => x.Value).FirstOrDefault().Key
            };
        }
        
        /// <summary>
        /// Get recent loyalty decisions
        /// </summary>
        public List<LoyaltyDecision> GetRecentDecisions(int count = 10)
        {
            return _decisionHistory.TakeLast(count).ToList();
        }
        
        /// <summary>
        /// Set loyalty values (for loading saved games)
        /// </summary>
        public void SetLoyaltyValues(int imperial, int rebellion)
        {
            _imperialLoyalty = Mathf.Clamp(imperial, minLoyaltyValue, maxLoyaltyValue);
            _rebellionSympathy = Mathf.Clamp(rebellion, minLoyaltyValue, maxLoyaltyValue);
            _currentAlignment = CalculateAlignment();
            
            OnLoyaltyChanged?.Invoke(_imperialLoyalty, _rebellionSympathy);
            OnAlignmentChanged?.Invoke(_currentAlignment);
            
            if (enableDebugLogs)
                Debug.Log($"[LoyaltyManager] Loyalty values loaded: Imperial={imperial}, Rebellion={rebellion}");
        }
        
        // Event handlers
        private void OnDecisionMade(DecisionType decision, IEncounter encounter)
        {
            // This would be extended to handle specific encounter types
            // For now, it's a placeholder for future integration
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            GameEvents.OnDecisionMade -= OnDecisionMade;
            
            // Clear event subscriptions
            OnLoyaltyChanged = null;
            OnAlignmentChanged = null;
            OnKeyDecisionAdded = null;
            OnLoyaltyDecisionRecorded = null;
            OnLoyaltyThresholdReached = null;
            OnLoyaltyFeedback = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Gain Imperial Loyalty")]
        private void TestGainImperialLoyalty()
        {
            UpdateLoyalty(2, -1, "Test: Followed Imperial protocol");
        }
        
        [ContextMenu("Test: Gain Rebellion Sympathy")]
        private void TestGainRebellionSympathy()
        {
            UpdateLoyalty(-1, 2, "Test: Showed sympathy for rebels");
        }
        
        [ContextMenu("Test: Add Key Decision")]
        private void TestAddKeyDecision()
        {
            AddKeyDecision("Test decision - Manual trigger");
        }
        
        [ContextMenu("Show Loyalty Statistics")]
        private void ShowLoyaltyStatistics()
        {
            var stats = GetStatistics();
            Debug.Log("=== LOYALTY STATISTICS ===");
            Debug.Log($"Imperial Loyalty: {stats.ImperialLoyalty}");
            Debug.Log($"Rebellion Sympathy: {stats.RebellionSympathy}");
            Debug.Log($"Current Alignment: {stats.CurrentAlignment}");
            Debug.Log($"Total Changes: {stats.TotalLoyaltyChanges}");
            Debug.Log($"Average Imperial Change: {stats.AverageImperialChange:F2}");
            Debug.Log($"Average Rebellion Change: {stats.AverageRebellionChange:F2}");
            Debug.Log($"Session Imperial: +{stats.SessionImperialGain}/-{stats.SessionImperialLoss}");
            Debug.Log($"Session Rebellion: +{stats.SessionRebellionGain}/-{stats.SessionRebellionLoss}");
            Debug.Log($"Key Decisions: {stats.TotalKeyDecisions}");
            Debug.Log($"Most Common Decision: {stats.MostCommonDecision}");
            Debug.Log("=== END STATISTICS ===");
        }
    }
    
    // Supporting data structures
    [System.Serializable]
    public class LoyaltyDecision
    {
        public string Id;
        public DateTime Timestamp;
        public int ImperialChange;
        public int RebellionChange;
        public string Reason;
        public int ResultingImperialLoyalty;
        public int ResultingRebellionSympathy;
        public LoyaltyAlignment ResultingAlignment;
    }
    
    [System.Serializable]
    public struct LoyaltyStatistics
    {
        public int ImperialLoyalty;
        public int RebellionSympathy;
        public LoyaltyAlignment CurrentAlignment;
        public int TotalLoyaltyChanges;
        public float AverageImperialChange;
        public float AverageRebellionChange;
        public int SessionImperialGain;
        public int SessionImperialLoss;
        public int SessionRebellionGain;
        public int SessionRebellionLoss;
        public int TotalKeyDecisions;
        public string MostCommonDecision;
    }
    
    public enum LoyaltyAlignment
    {
        ExtremistRebellion,
        RebellionSympathizer,
        RebellionLeaning,
        Neutral,
        ImperialLeaning,
        ImperialLoyal,
        ExtremistImperial
    }
    
    public enum LoyaltyThreshold
    {
        HighImperialLoyalty,
        LostImperialLoyalty,
        HighRebellionSympathy,
        LostRebellionSympathy,
        ExtremistImperial,
        ExtremistRebellion
    }
}