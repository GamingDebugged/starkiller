using UnityEngine;
using System;
using System.Collections.Generic;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Tracks player decisions, strikes, and performance metrics
    /// Extracted from GameManager for focused responsibility
    /// </summary>
    public class DecisionTracker : MonoBehaviour
    {
        [Header("Decision Settings")]
        [SerializeField] private int maxMistakes = 3;
        [SerializeField] private bool enableDebugLogs = true;
        
        [Header("Decision Tracking")]
        [SerializeField] private int _correctDecisions = 0;
        [SerializeField] private int _wrongDecisions = 0;
        [SerializeField] private int _currentStrikes = 0;
        [SerializeField] private int _dailyCorrectDecisions = 0;
        [SerializeField] private int _dailyWrongDecisions = 0;
        
        // Performance tracking
        private List<DecisionRecord> _decisionHistory = new List<DecisionRecord>();
        private Dictionary<string, int> _decisionsByType = new Dictionary<string, int>();
        
        // Public properties
        public int CorrectDecisions => _correctDecisions;
        public int WrongDecisions => _wrongDecisions;
        public int CurrentStrikes => _currentStrikes;
        public int MaxStrikes => maxMistakes;
        public int DailyCorrectDecisions => _dailyCorrectDecisions;
        public int DailyWrongDecisions => _dailyWrongDecisions;
        public int TotalDecisions => _correctDecisions + _wrongDecisions;
        public float AccuracyPercentage => TotalDecisions > 0 ? (_correctDecisions / (float)TotalDecisions) * 100f : 0f;
        
        // Events
        public static event Action<int, int> OnStrikesChanged; // (currentStrikes, maxStrikes)
        public static event Action OnGameOverTriggered;
        public static event Action<bool, string> OnDecisionRecorded; // (wasCorrect, reason)
        public static event Action<float> OnAccuracyChanged;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<DecisionTracker>(this);
            
            if (enableDebugLogs)
                Debug.Log("[DecisionTracker] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Subscribe to decision events
            GameEvents.OnDecisionMade += OnDecisionMade;
            GameEvents.OnDayChanged += OnNewDay;
            
            // Trigger initial updates
            TriggerStrikesChanged();
            TriggerAccuracyChanged();
            
            if (enableDebugLogs)
                Debug.Log($"[DecisionTracker] Started tracking. Max strikes: {maxMistakes}");
        }
        
        /// <summary>
        /// Record a decision and its correctness
        /// </summary>
        public void RecordDecision(bool wasCorrect, string reason = "", Starkiller.Core.DecisionType decisionType = Starkiller.Core.DecisionType.Approve)
        {
            // Check if game is over - don't record decisions after game over
            var gameOverManager = ServiceLocator.Get<GameOverManager>();
            if (gameOverManager != null && gameOverManager.IsGameOver)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[DecisionTracker] Cannot record decision - game is over");
                return;
            }
            
            // Update counters
            if (wasCorrect)
            {
                _correctDecisions++;
                _dailyCorrectDecisions++;
            }
            else
            {
                _wrongDecisions++;
                _dailyWrongDecisions++;
                AddStrike(reason);
            }
            
            // Record decision details
            var record = new DecisionRecord
            {
                timestamp = Time.time,
                wasCorrect = wasCorrect,
                reason = reason,
                decisionType = decisionType,
                day = 1 // We'll implement DayProgressionManager in Phase 2
            };
            
            _decisionHistory.Add(record);
            
            // Track decisions by type
            string typeKey = decisionType.ToString();
            if (!_decisionsByType.ContainsKey(typeKey))
                _decisionsByType[typeKey] = 0;
            _decisionsByType[typeKey]++;
            
            // Trigger events
            OnDecisionRecorded?.Invoke(wasCorrect, reason);
            TriggerAccuracyChanged();
            
            if (enableDebugLogs)
            {
                Debug.Log($"[DecisionTracker] Decision recorded: {(wasCorrect ? "CORRECT" : "WRONG")} - {reason}");
                Debug.Log($"[DecisionTracker] Running total: {_correctDecisions} correct, {_wrongDecisions} wrong, {_currentStrikes} strikes");
            }
        }
        
        /// <summary>
        /// Add a strike for incorrect decision
        /// </summary>
        public void AddStrike(string reason = "")
        {
            _currentStrikes++;
            
            if (enableDebugLogs)
                Debug.LogWarning($"[DecisionTracker] Strike added! ({_currentStrikes}/{maxMistakes}) - {reason}");
            
            TriggerStrikesChanged();
            
            // Check for game over
            if (_currentStrikes >= maxMistakes)
            {
                TriggerGameOver();
            }
            else
            {
                // Warn player about approaching game over
                int strikesRemaining = maxMistakes - _currentStrikes;
                GameEvents.TriggerUINotification($"Strike! {strikesRemaining} mistakes remaining before termination!");
            }
        }
        
        /// <summary>
        /// Remove a strike (for special circumstances)
        /// </summary>
        public bool RemoveStrike(string reason = "")
        {
            if (_currentStrikes <= 0) return false;
            
            _currentStrikes--;
            
            if (enableDebugLogs)
                Debug.Log($"[DecisionTracker] Strike removed! ({_currentStrikes}/{maxMistakes}) - {reason}");
            
            TriggerStrikesChanged();
            GameEvents.TriggerUINotification($"Strike removed! {reason}");
            
            return true;
        }
        
        /// <summary>
        /// Reset strikes (for new day or special events)
        /// </summary>
        public void ResetStrikes(string reason = "")
        {
            int previousStrikes = _currentStrikes;
            _currentStrikes = 0;
            
            if (enableDebugLogs)
                Debug.Log($"[DecisionTracker] Strikes reset from {previousStrikes} to 0 - {reason}");
            
            TriggerStrikesChanged();
            
            if (previousStrikes > 0)
            {
                GameEvents.TriggerUINotification($"Strikes reset! {reason}");
            }
        }
        
        /// <summary>
        /// Get performance summary for the current day
        /// </summary>
        public string GetDailyPerformanceSummary()
        {
            int totalDaily = _dailyCorrectDecisions + _dailyWrongDecisions;
            float dailyAccuracy = totalDaily > 0 ? (_dailyCorrectDecisions / (float)totalDaily) * 100f : 0f;
            
            return $"Today: {_dailyCorrectDecisions} correct, {_dailyWrongDecisions} wrong ({dailyAccuracy:F1}% accuracy)\n" +
                   $"Total: {_correctDecisions} correct, {_wrongDecisions} wrong ({AccuracyPercentage:F1}% accuracy)\n" +
                   $"Strikes: {_currentStrikes}/{maxMistakes}";
        }
        
        /// <summary>
        /// Get performance summary for all time
        /// </summary>
        public string GetOverallPerformanceSummary()
        {
            return $"Overall Performance:\n" +
                   $"Correct Decisions: {_correctDecisions}\n" +
                   $"Wrong Decisions: {_wrongDecisions}\n" +
                   $"Accuracy: {AccuracyPercentage:F1}%\n" +
                   $"Current Strikes: {_currentStrikes}/{maxMistakes}\n" +
                   $"Total Decisions: {TotalDecisions}";
        }
        
        /// <summary>
        /// Get decision breakdown by type
        /// </summary>
        public Dictionary<string, int> GetDecisionBreakdown()
        {
            return new Dictionary<string, int>(_decisionsByType);
        }
        
        /// <summary>
        /// Check if player is close to game over
        /// </summary>
        public bool IsNearGameOver()
        {
            return _currentStrikes >= (maxMistakes - 1);
        }
        
        /// <summary>
        /// Reset daily tracking (called at start of new day)
        /// </summary>
        private void OnNewDay(int newDay)
        {
            _dailyCorrectDecisions = 0;
            _dailyWrongDecisions = 0;
            
            // Optionally reset strikes each day (uncomment if desired)
            // ResetStrikes("New Day");
            
            if (enableDebugLogs)
                Debug.Log($"[DecisionTracker] Reset daily tracking for day {newDay}");
        }
        
        /// <summary>
        /// Handle decision made events from the game
        /// </summary>
        private void OnDecisionMade(Starkiller.Core.DecisionType decision, Starkiller.Core.IEncounter encounter)
        {
            // This will be called when decisions are made through the event system
            // The actual correctness evaluation would need to be determined by game logic
            // For now, this is a placeholder that other systems can hook into
            
            if (enableDebugLogs)
                Debug.Log($"[DecisionTracker] Decision event received: {decision} for {encounter?.ShipType ?? "Unknown"}");
        }
        
        /// <summary>
        /// Trigger game over condition
        /// </summary>
        private void TriggerGameOver()
        {
            if (enableDebugLogs)
                Debug.LogError($"[DecisionTracker] GAME OVER! Maximum strikes reached ({_currentStrikes}/{maxMistakes})");
            
            OnGameOverTriggered?.Invoke();
            GameEvents.TriggerGameEnded();
            GameEvents.TriggerUINotification("TERMINATED! Too many mistakes made.");
        }
        
        /// <summary>
        /// Trigger strikes changed event
        /// </summary>
        private void TriggerStrikesChanged()
        {
            OnStrikesChanged?.Invoke(_currentStrikes, maxMistakes);
            GameEvents.TriggerStrikesChanged(_currentStrikes);
        }
        
        /// <summary>
        /// Trigger accuracy changed event
        /// </summary>
        private void TriggerAccuracyChanged()
        {
            OnAccuracyChanged?.Invoke(AccuracyPercentage);
        }
        
        /// <summary>
        /// Set decision counts (for loading saves)
        /// </summary>
        public void SetDecisionCounts(int correct, int wrong, int strikes)
        {
            _correctDecisions = Mathf.Max(0, correct);
            _wrongDecisions = Mathf.Max(0, wrong);
            _currentStrikes = Mathf.Clamp(strikes, 0, maxMistakes);
            
            TriggerStrikesChanged();
            TriggerAccuracyChanged();
            
            if (enableDebugLogs)
                Debug.Log($"[DecisionTracker] Decision counts loaded: {correct} correct, {wrong} wrong, {strikes} strikes");
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            GameEvents.OnDecisionMade -= OnDecisionMade;
            GameEvents.OnDayChanged -= OnNewDay;
            
            // Clear event subscriptions
            OnStrikesChanged = null;
            OnGameOverTriggered = null;
            OnDecisionRecorded = null;
            OnAccuracyChanged = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Add Correct Decision")]
        private void TestCorrectDecision() => RecordDecision(true, "Debug Test");
        
        [ContextMenu("Test: Add Wrong Decision")]
        private void TestWrongDecision() => RecordDecision(false, "Debug Test");
        
        [ContextMenu("Test: Add Strike")]
        private void TestAddStrike() => AddStrike("Debug Test");
        
        [ContextMenu("Test: Reset Strikes")]
        private void TestResetStrikes() => ResetStrikes("Debug Test");
        
        [ContextMenu("Show Performance Summary")]
        private void ShowPerformanceSummary() => Debug.Log(GetOverallPerformanceSummary());
    }
    
    /// <summary>
    /// Record of a single decision for tracking purposes
    /// </summary>
    [System.Serializable]
    public class DecisionRecord
    {
        public float timestamp;
        public bool wasCorrect;
        public string reason;
        public Starkiller.Core.DecisionType decisionType;
        public int day;
    }
}