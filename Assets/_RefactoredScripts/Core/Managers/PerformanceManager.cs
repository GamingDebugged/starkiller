using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages player performance tracking, scoring, and decision analysis
    /// Extracted from GameManager for focused performance responsibility
    /// </summary>
    public class PerformanceManager : MonoBehaviour
    {
        [Header("Performance Settings")]
        [SerializeField] private int maxMistakes = 3;
        [SerializeField] private int requiredShipsPerDay = 8;
        [SerializeField] private bool enablePerformanceAnalytics = true;
        [SerializeField] private bool trackDecisionHistory = true;
        [SerializeField] private int maxDecisionHistory = 100;
        
        [Header("Scoring System")]
        [SerializeField] private int baseScore = 0;
        [SerializeField] private int correctDecisionPoints = 10;
        [SerializeField] private int wrongDecisionPenalty = -5;
        [SerializeField] private int quotaBonusPoints = 50;
        [SerializeField] private int perfectDayBonusPoints = 100;
        
        [Header("Salary Calculation")]
        [SerializeField] private int baseSalary = 30;
        [SerializeField] private int bonusPerExtraShip = 5;
        [SerializeField] private int penaltyPerMistake = 5;
        [SerializeField] private int perfectDayBonus = 20;
        
        [Header("Performance Thresholds")]
        [SerializeField] private float excellentPerformanceThreshold = 0.95f; // 95% accuracy
        [SerializeField] private float goodPerformanceThreshold = 0.80f; // 80% accuracy
        [SerializeField] private float poorPerformanceThreshold = 0.60f; // 60% accuracy
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enablePerformanceLogging = true;
        
        // Performance state
        private int _currentScore = 0;
        private int _currentStrikes = 0;
        private int _correctDecisions = 0;
        private int _wrongDecisions = 0;
        private int _shipsProcessed = 0;
        private int _totalDecisions = 0;
        
        // Session statistics
        private float _sessionStartTime;
        private float _totalSessionTime = 0f;
        private float _averageDecisionTime = 0f;
        private List<float> _decisionTimes = new List<float>();
        private float _lastDecisionTime;
        
        // Decision tracking
        private List<PerformanceDecisionRecord> _decisionHistory = new List<PerformanceDecisionRecord>();
        private List<string> _keyDecisions = new List<string>();
        private Dictionary<string, int> _decisionTypeCount = new Dictionary<string, int>();
        
        // Performance analytics
        private PerformanceMetrics _dailyMetrics = new PerformanceMetrics();
        private PerformanceMetrics _sessionMetrics = new PerformanceMetrics();
        private List<PerformanceMetrics> _dailyHistory = new List<PerformanceMetrics>();
        
        // Streaks and achievements
        private int _currentCorrectStreak = 0;
        private int _longestCorrectStreak = 0;
        private int _currentWrongStreak = 0;
        private int _perfectDays = 0;
        
        // Dependencies
        private CreditsManager _creditsManager;
        private DayProgressionManager _dayManager;
        private NotificationManager _notificationManager;
        private AudioManager _audioManager;
        
        // Events
        public static event Action<int> OnScoreChanged;
        public static event Action<int, int> OnStrikesChanged; // (current, max)
        public static event Action<int, int> OnDecisionCountChanged; // (correct, wrong)
        public static event Action<int> OnShipsProcessedChanged;
        public static event Action<PerformanceRating> OnPerformanceRatingChanged;
        public static event Action<PerformanceDecisionRecord> OnDecisionRecorded;
        public static event Action<PerformanceMetrics> OnDailyMetricsUpdated;
        public static event Action<int> OnCorrectStreakChanged;
        public static event Action OnPerfectDayAchieved;
        public static event Action<string> OnPerformanceAchievement;
        
        // Public properties
        public int CurrentScore => _currentScore;
        public int CurrentStrikes => _currentStrikes;
        public int MaxMistakes => maxMistakes;
        public int CorrectDecisions => _correctDecisions;
        public int WrongDecisions => _wrongDecisions;
        public int ShipsProcessed => _shipsProcessed;
        public int TotalDecisions => _totalDecisions;
        public float AccuracyRate => _totalDecisions > 0 ? (float)_correctDecisions / _totalDecisions : 0f;
        public PerformanceRating CurrentRating => CalculatePerformanceRating();
        public int CurrentCorrectStreak => _currentCorrectStreak;
        public int LongestCorrectStreak => _longestCorrectStreak;
        public bool IsGameOver => _currentStrikes >= maxMistakes;
        public bool HasMetQuota => _shipsProcessed >= requiredShipsPerDay;
        public bool IsPerfectDay => _wrongDecisions == 0 && _shipsProcessed >= requiredShipsPerDay;
        public float AverageDecisionTime => _averageDecisionTime;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<PerformanceManager>(this);
            
            // Initialize session start time
            _sessionStartTime = Time.time;
            
            if (enableDebugLogs)
                Debug.Log("[PerformanceManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _creditsManager = ServiceLocator.Get<CreditsManager>();
            _dayManager = ServiceLocator.Get<DayProgressionManager>();
            _notificationManager = ServiceLocator.Get<NotificationManager>();
            _audioManager = ServiceLocator.Get<AudioManager>();
            
            // Subscribe to events
            if (_dayManager != null)
            {
                DayProgressionManager.OnDayStarted += OnDayStarted;
                DayProgressionManager.OnShiftEnded += OnShiftEnded;
            }
            
            GameEvents.OnDecisionMade += OnDecisionMade;
            
            // Initialize metrics
            ResetDailyMetrics();
            
            if (enableDebugLogs)
                Debug.Log("[PerformanceManager] Performance tracking system ready");
        }
        
        /// <summary>
        /// Record a decision made by the player
        /// </summary>
        public void RecordDecision(bool approved, bool correctDecision, IEncounter encounter, string reason = "")
        {
            _totalDecisions++;
            _shipsProcessed++;
            
            // Calculate decision time
            float decisionTime = 0f;
            if (_lastDecisionTime > 0)
            {
                decisionTime = Time.time - _lastDecisionTime;
                _decisionTimes.Add(decisionTime);
                _averageDecisionTime = _decisionTimes.Average();
            }
            _lastDecisionTime = Time.time;
            
            // Create decision record
            var record = new PerformanceDecisionRecord
            {
                Id = System.Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now,
                ShipType = encounter?.ShipType ?? "Unknown",
                CaptainName = encounter?.CaptainName ?? "Unknown",
                WasApproved = approved,
                WasCorrect = correctDecision,
                Reason = reason,
                DecisionTime = decisionTime,
                SequenceNumber = _totalDecisions
            };
            
            // Process decision
            ProcessDecision(correctDecision, record);
            
            // Record in history
            if (trackDecisionHistory)
            {
                _decisionHistory.Add(record);
                if (_decisionHistory.Count > maxDecisionHistory)
                {
                    _decisionHistory.RemoveAt(0);
                }
            }
            
            // Update metrics
            UpdateMetrics(correctDecision, decisionTime);
            
            // Trigger events
            OnDecisionRecorded?.Invoke(record);
            OnShipsProcessedChanged?.Invoke(_shipsProcessed);
            
            if (enablePerformanceLogging)
            {
                Debug.Log($"[PerformanceManager] Decision recorded: {(correctDecision ? "✓" : "✗")} " +
                         $"({encounter?.ShipType}) - Accuracy: {AccuracyRate:P1}");
            }
        }
        
        /// <summary>
        /// Process a decision and update scores/strikes
        /// </summary>
        private void ProcessDecision(bool correctDecision, PerformanceDecisionRecord record)
        {
            if (correctDecision)
            {
                // Correct decision
                _correctDecisions++;
                _currentScore += correctDecisionPoints;
                _currentCorrectStreak++;
                _currentWrongStreak = 0;
                
                // Check for streak achievements
                if (_currentCorrectStreak > _longestCorrectStreak)
                {
                    _longestCorrectStreak = _currentCorrectStreak;
                    CheckStreakAchievements();
                }
                
                OnCorrectStreakChanged?.Invoke(_currentCorrectStreak);
            }
            else
            {
                // Wrong decision
                _wrongDecisions++;
                _currentScore += wrongDecisionPenalty;
                _currentStrikes++;
                _currentCorrectStreak = 0;
                _currentWrongStreak++;
                
                // Apply credit penalties if applicable
                ApplyDecisionPenalties(record);
                
                // Check for game over
                if (_currentStrikes >= maxMistakes)
                {
                    TriggerGameOver();
                }
                
                OnStrikesChanged?.Invoke(_currentStrikes, maxMistakes);
                
                // Show strike notification
                if (_notificationManager != null)
                {
                    string message = _currentStrikes >= maxMistakes ? 
                        "❌ GAME OVER - Maximum strikes reached!" : 
                        $"❌ Strike {_currentStrikes}/{maxMistakes} - {record.Reason}";
                    
                    NotificationType type = _currentStrikes >= maxMistakes ? 
                        NotificationType.Error : NotificationType.Warning;
                    
                    _notificationManager.ShowNotification(message, type);
                }
            }
            
            // Update UI events
            OnScoreChanged?.Invoke(_currentScore);
            OnDecisionCountChanged?.Invoke(_correctDecisions, _wrongDecisions);
            OnPerformanceRatingChanged?.Invoke(CalculatePerformanceRating());
        }
        
        /// <summary>
        /// Apply credit penalties for wrong decisions
        /// </summary>
        private void ApplyDecisionPenalties(PerformanceDecisionRecord record)
        {
            // This would integrate with the original GameManager's penalty system
            // For now, we'll use a basic penalty system
            
            int creditPenalty = 5; // Default penalty
            
            if (_creditsManager != null)
            {
                _creditsManager.DeductCredits(creditPenalty, $"Wrong decision: {record.ShipType}");
            }
        }
        
        /// <summary>
        /// Calculate current performance rating
        /// </summary>
        private PerformanceRating CalculatePerformanceRating()
        {
            float accuracy = AccuracyRate;
            
            if (accuracy >= excellentPerformanceThreshold)
                return PerformanceRating.Excellent;
            else if (accuracy >= goodPerformanceThreshold)
                return PerformanceRating.Good;
            else if (accuracy >= poorPerformanceThreshold)
                return PerformanceRating.Average;
            else
                return PerformanceRating.Poor;
        }
        
        /// <summary>
        /// Calculate daily salary based on performance
        /// </summary>
        public int CalculateSalary()
        {
            int salary = baseSalary;
            
            // Bonus for exceeding quota
            if (_shipsProcessed > requiredShipsPerDay)
            {
                int extraShips = _shipsProcessed - requiredShipsPerDay;
                salary += extraShips * bonusPerExtraShip;
            }
            
            // Penalty for mistakes
            salary -= _currentStrikes * penaltyPerMistake;
            
            // Perfect day bonus
            if (IsPerfectDay)
            {
                salary += perfectDayBonus;
            }
            
            // Ensure salary doesn't go negative
            salary = Mathf.Max(0, salary);
            
            return salary;
        }
        
        /// <summary>
        /// Add a key decision to the record
        /// </summary>
        public void AddKeyDecision(string decision)
        {
            _keyDecisions.Add(decision);
            
            if (enableDebugLogs)
                Debug.Log($"[PerformanceManager] Key decision recorded: {decision}");
        }
        
        /// <summary>
        /// Update performance metrics
        /// </summary>
        private void UpdateMetrics(bool correctDecision, float decisionTime)
        {
            // Update daily metrics
            _dailyMetrics.TotalDecisions = _totalDecisions;
            _dailyMetrics.CorrectDecisions = _correctDecisions;
            _dailyMetrics.WrongDecisions = _wrongDecisions;
            _dailyMetrics.AccuracyRate = AccuracyRate;
            _dailyMetrics.AverageDecisionTime = _averageDecisionTime;
            _dailyMetrics.ShipsProcessed = _shipsProcessed;
            _dailyMetrics.CurrentStrikes = _currentStrikes;
            _dailyMetrics.CurrentScore = _currentScore;
            _dailyMetrics.LongestCorrectStreak = _longestCorrectStreak;
            
            // Update session metrics
            _sessionMetrics = _dailyMetrics; // Session = current day for now
            
            OnDailyMetricsUpdated?.Invoke(_dailyMetrics);
        }
        
        /// <summary>
        /// Check for streak achievements
        /// </summary>
        private void CheckStreakAchievements()
        {
            string achievement = _currentCorrectStreak switch
            {
                5 => "🎯 Five in a Row!",
                10 => "🔥 Hot Streak!",
                15 => "⭐ Accuracy Master!",
                20 => "🏆 Perfect Operator!",
                _ => null
            };
            
            if (achievement != null)
            {
                OnPerformanceAchievement?.Invoke(achievement);
                
                if (_notificationManager != null)
                {
                    _notificationManager.ShowNotification(achievement, NotificationType.Success);
                }
            }
        }
        
        /// <summary>
        /// Trigger game over
        /// </summary>
        private void TriggerGameOver()
        {
            if (enableDebugLogs)
                Debug.Log("[PerformanceManager] Game Over - Maximum strikes reached");
            
            // This would trigger the GameOverManager when it's created
            GameEvents.TriggerGameStateChanged(GameState.GameOver);
        }
        
        /// <summary>
        /// Reset daily performance metrics
        /// </summary>
        private void ResetDailyMetrics()
        {
            _correctDecisions = 0;
            _wrongDecisions = 0;
            _shipsProcessed = 0;
            _totalDecisions = 0;
            _currentStrikes = 0;
            _currentCorrectStreak = 0;
            _currentWrongStreak = 0;
            _decisionTimes.Clear();
            _averageDecisionTime = 0f;
            _keyDecisions.Clear();
            
            _dailyMetrics = new PerformanceMetrics();
            
            if (enableDebugLogs)
                Debug.Log("[PerformanceManager] Daily metrics reset");
        }
        
        /// <summary>
        /// Get performance statistics
        /// </summary>
        public PerformanceStatistics GetStatistics()
        {
            return new PerformanceStatistics
            {
                CurrentScore = _currentScore,
                TotalDecisions = _totalDecisions,
                CorrectDecisions = _correctDecisions,
                WrongDecisions = _wrongDecisions,
                AccuracyRate = AccuracyRate,
                CurrentStrikes = _currentStrikes,
                ShipsProcessed = _shipsProcessed,
                AverageDecisionTime = _averageDecisionTime,
                LongestCorrectStreak = _longestCorrectStreak,
                CurrentRating = CurrentRating,
                PerfectDays = _perfectDays,
                KeyDecisions = new List<string>(_keyDecisions)
            };
        }
        
        /// <summary>
        /// Get recent decision history
        /// </summary>
        public List<PerformanceDecisionRecord> GetRecentDecisions(int count = 10)
        {
            return _decisionHistory.TakeLast(count).ToList();
        }
        
        /// <summary>
        /// Set performance values (for loading saved games)
        /// </summary>
        public void SetPerformanceValues(int score, int strikes, int correct, int wrong, int processed)
        {
            _currentScore = score;
            _currentStrikes = strikes;
            _correctDecisions = correct;
            _wrongDecisions = wrong;
            _shipsProcessed = processed;
            _totalDecisions = correct + wrong;
            
            // Update metrics
            UpdateMetrics(true, 0f); // Dummy call to update metrics
            
            if (enableDebugLogs)
                Debug.Log($"[PerformanceManager] Performance values loaded: Score={score}, Strikes={strikes}");
        }
        
        // Event handlers
        private void OnDayStarted(int day)
        {
            // Save previous day metrics
            if (_totalDecisions > 0)
            {
                _dailyHistory.Add(_dailyMetrics);
                
                if (IsPerfectDay)
                {
                    _perfectDays++;
                    OnPerfectDayAchieved?.Invoke();
                    
                    if (_notificationManager != null)
                    {
                        _notificationManager.ShowNotification("🏆 Perfect Day! No mistakes!", NotificationType.Success);
                    }
                }
            }
            
            // Reset for new day
            ResetDailyMetrics();
            
            if (enableDebugLogs)
                Debug.Log($"[PerformanceManager] Day {day} started - performance tracking reset");
        }
        
        private void OnShiftEnded()
        {
            // Calculate final metrics for the day
            UpdateMetrics(true, 0f); // Dummy call to finalize metrics
            
            if (enableDebugLogs)
            {
                Debug.Log($"[PerformanceManager] Shift ended - Final accuracy: {AccuracyRate:P1}");
                Debug.Log($"[PerformanceManager] Ships processed: {_shipsProcessed}/{requiredShipsPerDay}");
            }
        }
        
        private void OnDecisionMade(DecisionType decision, IEncounter encounter)
        {
            // This will be connected to the actual decision validation logic
            // For now, we'll assume all decisions are correct (placeholder)
            RecordDecision(decision == DecisionType.Approve, true, encounter, "Auto-recorded decision");
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_dayManager != null)
            {
                DayProgressionManager.OnDayStarted -= OnDayStarted;
                DayProgressionManager.OnShiftEnded -= OnShiftEnded;
            }
            
            GameEvents.OnDecisionMade -= OnDecisionMade;
            
            // Clear event subscriptions
            OnScoreChanged = null;
            OnStrikesChanged = null;
            OnDecisionCountChanged = null;
            OnShipsProcessedChanged = null;
            OnPerformanceRatingChanged = null;
            OnDecisionRecorded = null;
            OnDailyMetricsUpdated = null;
            OnCorrectStreakChanged = null;
            OnPerfectDayAchieved = null;
            OnPerformanceAchievement = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Record Correct Decision")]
        private void TestCorrectDecision()
        {
            var mockEncounter = new MockEncounter("Test Ship", "Test Captain");
            RecordDecision(true, true, mockEncounter, "Test correct decision");
        }
        
        [ContextMenu("Test: Record Wrong Decision")]
        private void TestWrongDecision()
        {
            var mockEncounter = new MockEncounter("Test Ship", "Test Captain");
            RecordDecision(false, false, mockEncounter, "Test wrong decision");
        }
        
        [ContextMenu("Show Performance Statistics")]
        private void ShowPerformanceStatistics()
        {
            var stats = GetStatistics();
            Debug.Log("=== PERFORMANCE STATISTICS ===");
            Debug.Log($"Score: {stats.CurrentScore}");
            Debug.Log($"Accuracy: {stats.AccuracyRate:P1} ({stats.CorrectDecisions}✓/{stats.WrongDecisions}✗)");
            Debug.Log($"Strikes: {stats.CurrentStrikes}/{maxMistakes}");
            Debug.Log($"Ships: {stats.ShipsProcessed}/{requiredShipsPerDay}");
            Debug.Log($"Rating: {stats.CurrentRating}");
            Debug.Log($"Longest Streak: {stats.LongestCorrectStreak}");
            Debug.Log($"Perfect Days: {stats.PerfectDays}");
            Debug.Log("=== END STATISTICS ===");
        }
    }
    
    // Supporting structures and enums
    [System.Serializable]
    public class PerformanceDecisionRecord
    {
        public string Id;
        public DateTime Timestamp;
        public string ShipType;
        public string CaptainName;
        public bool WasApproved;
        public bool WasCorrect;
        public string Reason;
        public float DecisionTime;
        public int SequenceNumber;
    }
    
    [System.Serializable]
    public struct PerformanceMetrics
    {
        public int TotalDecisions;
        public int CorrectDecisions;
        public int WrongDecisions;
        public float AccuracyRate;
        public float AverageDecisionTime;
        public int ShipsProcessed;
        public int CurrentStrikes;
        public int CurrentScore;
        public int LongestCorrectStreak;
    }
    
    [System.Serializable]
    public struct PerformanceStatistics
    {
        public int CurrentScore;
        public int TotalDecisions;
        public int CorrectDecisions;
        public int WrongDecisions;
        public float AccuracyRate;
        public int CurrentStrikes;
        public int ShipsProcessed;
        public float AverageDecisionTime;
        public int LongestCorrectStreak;
        public PerformanceRating CurrentRating;
        public int PerfectDays;
        public List<string> KeyDecisions;
    }
    
    public enum PerformanceRating
    {
        Poor,
        Average,
        Good,
        Excellent
    }
    
}