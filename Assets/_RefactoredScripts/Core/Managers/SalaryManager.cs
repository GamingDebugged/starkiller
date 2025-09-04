using UnityEngine;
using System;
using System.Collections.Generic;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages salary calculations, bonuses, penalties, and daily earnings
    /// Extracted from GameManager for focused salary responsibility
    /// </summary>
    public class SalaryManager : MonoBehaviour
    {
        [Header("Salary Settings")]
        [SerializeField] private int baseSalary = 30;
        [SerializeField] private int bonusPerExtraShip = 5;
        [SerializeField] private int penaltyPerMistake = 5;
        [SerializeField] private int requiredShipsPerDay = 8;
        [SerializeField] private int perfectDayBonus = 20;
        [SerializeField] private int loyaltyBonus = 10;
        
        [Header("Bonus Multipliers")]
        [SerializeField] private float efficiencyMultiplier = 1.2f; // 20% bonus for high efficiency
        [SerializeField] private float accuracyMultiplier = 1.15f; // 15% bonus for high accuracy
        [SerializeField] private float speedMultiplier = 1.1f; // 10% bonus for fast processing
        
        [Header("Salary Caps")]
        [SerializeField] private int maxDailySalary = 200;
        [SerializeField] private int minDailySalary = 0;
        [SerializeField] private bool enableSalaryCaps = true;
        
        [Header("Performance Thresholds")]
        [SerializeField] private float highEfficiencyThreshold = 1.5f; // 150% of quota
        [SerializeField] private float highAccuracyThreshold = 0.9f; // 90% accuracy
        [SerializeField] private float fastProcessingThreshold = 120f; // Under 2 minutes per ship average
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableSalaryBreakdown = true;
        
        // Salary state
        private int _currentSalary = 0;
        private int _totalEarnedToday = 0;
        private int _totalEarnedThisSession = 0;
        private SalaryBreakdown _currentBreakdown = new SalaryBreakdown();
        
        // Performance tracking for bonuses
        private bool _isPerfectDay = false;
        private bool _isHighEfficiency = false;
        private bool _isHighAccuracy = false;
        private bool _isFastProcessing = false;
        
        // Salary history
        private List<DailySalaryRecord> _salaryHistory = new List<DailySalaryRecord>();
        private int _highestDailySalary = 0;
        private int _lowestDailySalary = int.MaxValue;
        private float _averageDailySalary = 0f;
        
        // Dependencies
        private PerformanceManager _performanceManager;
        private DayProgressionManager _dayManager;
        private LoyaltyManager _loyaltyManager;
        private CreditsManager _creditsManager;
        private NotificationManager _notificationManager;
        
        // Events
        public static event Action<int, SalaryBreakdown> OnSalaryCalculated;
        public static event Action<int> OnSalaryPaid;
        public static event Action<SalaryBonus> OnBonusEarned;
        public static event Action<int> OnPenaltyApplied;
        public static event Action<DailySalaryRecord> OnDailySalaryRecorded;
        
        // Public properties
        public int CurrentSalary => _currentSalary;
        public int TotalEarnedToday => _totalEarnedToday;
        public int TotalEarnedThisSession => _totalEarnedThisSession;
        public SalaryBreakdown CurrentBreakdown => _currentBreakdown;
        public int HighestDailySalary => _highestDailySalary;
        public int LowestDailySalary => _lowestDailySalary == int.MaxValue ? 0 : _lowestDailySalary;
        public float AverageDailySalary => _averageDailySalary;
        public List<DailySalaryRecord> SalaryHistory => new List<DailySalaryRecord>(_salaryHistory);
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<SalaryManager>(this);
            
            if (enableDebugLogs)
                Debug.Log("[SalaryManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _performanceManager = ServiceLocator.Get<PerformanceManager>();
            _dayManager = ServiceLocator.Get<DayProgressionManager>();
            _loyaltyManager = ServiceLocator.Get<LoyaltyManager>();
            _creditsManager = ServiceLocator.Get<CreditsManager>();
            _notificationManager = ServiceLocator.Get<NotificationManager>();
            
            // Subscribe to events
            if (_dayManager != null)
            {
                DayProgressionManager.OnShiftEnded += OnShiftEnded;
                DayProgressionManager.OnDayStarted += OnDayStarted;
            }
            
            if (enableDebugLogs)
                Debug.Log("[SalaryManager] Salary system ready");
        }
        
        /// <summary>
        /// Calculate daily salary based on performance
        /// </summary>
        public int CalculateSalary()
        {
            if (_performanceManager == null)
            {
                Debug.LogWarning("[SalaryManager] PerformanceManager not available for salary calculation");
                return baseSalary;
            }
            
            // Reset breakdown
            _currentBreakdown = new SalaryBreakdown();
            
            // Base salary
            int salary = baseSalary;
            _currentBreakdown.BaseSalary = baseSalary;
            
            // Get performance data
            int shipsProcessed = _performanceManager.ShipsProcessed;
            int currentStrikes = _performanceManager.CurrentStrikes;
            float accuracy = _performanceManager.AccuracyRate;
            float avgDecisionTime = _performanceManager.AverageDecisionTime;
            
            // Calculate efficiency
            float efficiency = (float)shipsProcessed / requiredShipsPerDay;
            
            // Extra ship bonus
            if (shipsProcessed > requiredShipsPerDay)
            {
                int extraShips = shipsProcessed - requiredShipsPerDay;
                int extraShipBonus = extraShips * bonusPerExtraShip;
                salary += extraShipBonus;
                _currentBreakdown.ExtraShipBonus = extraShipBonus;
                _currentBreakdown.ExtraShipsProcessed = extraShips;
            }
            
            // Strike penalties
            if (currentStrikes > 0)
            {
                int strikePenalty = currentStrikes * penaltyPerMistake;
                salary -= strikePenalty;
                _currentBreakdown.StrikePenalty = strikePenalty;
                _currentBreakdown.StrikesIncurred = currentStrikes;
            }
            
            // Performance bonuses
            CalculatePerformanceBonuses(ref salary, efficiency, accuracy, avgDecisionTime);
            
            // Perfect day bonus
            if (_performanceManager.IsPerfectDay)
            {
                salary += perfectDayBonus;
                _currentBreakdown.PerfectDayBonus = perfectDayBonus;
                _isPerfectDay = true;
                TriggerBonus(SalaryBonus.PerfectDay, perfectDayBonus);
            }
            
            // Loyalty bonus
            if (_loyaltyManager != null && _loyaltyManager.IsImperialLoyal)
            {
                salary += loyaltyBonus;
                _currentBreakdown.LoyaltyBonus = loyaltyBonus;
                TriggerBonus(SalaryBonus.ImperialLoyalty, loyaltyBonus);
            }
            
            // Apply caps
            if (enableSalaryCaps)
            {
                salary = Mathf.Clamp(salary, minDailySalary, maxDailySalary);
            }
            else
            {
                salary = Mathf.Max(salary, minDailySalary);
            }
            
            _currentSalary = salary;
            _currentBreakdown.TotalSalary = salary;
            _currentBreakdown.ShipsProcessed = shipsProcessed;
            _currentBreakdown.RequiredShips = requiredShipsPerDay;
            _currentBreakdown.Accuracy = accuracy;
            _currentBreakdown.Efficiency = efficiency;
            
            // Trigger events
            OnSalaryCalculated?.Invoke(salary, _currentBreakdown);
            
            if (enableDebugLogs && enableSalaryBreakdown)
            {
                LogSalaryBreakdown();
            }
            
            return salary;
        }
        
        /// <summary>
        /// Calculate performance-based bonuses
        /// </summary>
        private void CalculatePerformanceBonuses(ref int salary, float efficiency, float accuracy, float avgDecisionTime)
        {
            int performanceBonus = 0;
            
            // High efficiency bonus
            if (efficiency >= highEfficiencyThreshold)
            {
                int efficiencyBonus = Mathf.RoundToInt(baseSalary * (efficiencyMultiplier - 1f));
                performanceBonus += efficiencyBonus;
                _currentBreakdown.EfficiencyBonus = efficiencyBonus;
                _isHighEfficiency = true;
                TriggerBonus(SalaryBonus.HighEfficiency, efficiencyBonus);
            }
            
            // High accuracy bonus
            if (accuracy >= highAccuracyThreshold)
            {
                int accuracyBonus = Mathf.RoundToInt(baseSalary * (accuracyMultiplier - 1f));
                performanceBonus += accuracyBonus;
                _currentBreakdown.AccuracyBonus = accuracyBonus;
                _isHighAccuracy = true;
                TriggerBonus(SalaryBonus.HighAccuracy, accuracyBonus);
            }
            
            // Fast processing bonus
            if (avgDecisionTime > 0 && avgDecisionTime <= fastProcessingThreshold)
            {
                int speedBonus = Mathf.RoundToInt(baseSalary * (speedMultiplier - 1f));
                performanceBonus += speedBonus;
                _currentBreakdown.SpeedBonus = speedBonus;
                _isFastProcessing = true;
                TriggerBonus(SalaryBonus.FastProcessing, speedBonus);
            }
            
            salary += performanceBonus;
            _currentBreakdown.PerformanceBonus = performanceBonus;
        }
        
        /// <summary>
        /// Pay the calculated salary to the player
        /// </summary>
        public void PaySalary()
        {
            if (_currentSalary <= 0)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[SalaryManager] No salary to pay or salary is zero");
                return;
            }
            
            // Add credits to player account
            if (_creditsManager != null)
            {
                _creditsManager.AddCredits(_currentSalary, "Daily salary payment");
            }
            
            // Update tracking
            _totalEarnedToday = _currentSalary;
            _totalEarnedThisSession += _currentSalary;
            
            // Show notification
            if (_notificationManager != null)
            {
                string message = _isPerfectDay ? 
                    $"💰 Salary paid: {_currentSalary} credits (Perfect Day!)" :
                    $"💰 Salary paid: {_currentSalary} credits";
                
                _notificationManager.ShowNotification(message, NotificationType.Success);
            }
            
            // Trigger events
            OnSalaryPaid?.Invoke(_currentSalary);
            
            if (enableDebugLogs)
                Debug.Log($"[SalaryManager] Salary paid: {_currentSalary} credits");
        }
        
        /// <summary>
        /// Record daily salary in history
        /// </summary>
        private void RecordDailySalary()
        {
            var record = new DailySalaryRecord
            {
                Day = _dayManager != null ? _dayManager.CurrentDay : 1,
                Salary = _currentSalary,
                Breakdown = _currentBreakdown,
                Date = DateTime.Now,
                WasPerfectDay = _isPerfectDay,
                PerformanceBonuses = GetEarnedBonuses()
            };
            
            _salaryHistory.Add(record);
            
            // Update statistics
            if (_currentSalary > _highestDailySalary)
                _highestDailySalary = _currentSalary;
            
            if (_currentSalary < _lowestDailySalary)
                _lowestDailySalary = _currentSalary;
            
            _averageDailySalary = _salaryHistory.Count > 0 ? 
                (float)_salaryHistory.Sum(r => r.Salary) / _salaryHistory.Count : 0f;
            
            OnDailySalaryRecorded?.Invoke(record);
            
            if (enableDebugLogs)
                Debug.Log($"[SalaryManager] Daily salary recorded: {_currentSalary} credits");
        }
        
        /// <summary>
        /// Get list of bonuses earned today
        /// </summary>
        private List<SalaryBonus> GetEarnedBonuses()
        {
            var bonuses = new List<SalaryBonus>();
            
            if (_isPerfectDay) bonuses.Add(SalaryBonus.PerfectDay);
            if (_isHighEfficiency) bonuses.Add(SalaryBonus.HighEfficiency);
            if (_isHighAccuracy) bonuses.Add(SalaryBonus.HighAccuracy);
            if (_isFastProcessing) bonuses.Add(SalaryBonus.FastProcessing);
            if (_currentBreakdown.LoyaltyBonus > 0) bonuses.Add(SalaryBonus.ImperialLoyalty);
            if (_currentBreakdown.ExtraShipBonus > 0) bonuses.Add(SalaryBonus.ExtraShips);
            
            return bonuses;
        }
        
        /// <summary>
        /// Trigger bonus notification
        /// </summary>
        private void TriggerBonus(SalaryBonus bonusType, int amount)
        {
            OnBonusEarned?.Invoke(bonusType);
            
            if (_notificationManager != null)
            {
                string message = bonusType switch
                {
                    SalaryBonus.PerfectDay => $"🏆 Perfect Day Bonus: +{amount} credits!",
                    SalaryBonus.HighEfficiency => $"⚡ Efficiency Bonus: +{amount} credits!",
                    SalaryBonus.HighAccuracy => $"🎯 Accuracy Bonus: +{amount} credits!",
                    SalaryBonus.FastProcessing => $"⏱️ Speed Bonus: +{amount} credits!",
                    SalaryBonus.ImperialLoyalty => $"🏛️ Loyalty Bonus: +{amount} credits!",
                    SalaryBonus.ExtraShips => $"📦 Extra Ships Bonus: +{amount} credits!",
                    _ => $"💎 Bonus earned: +{amount} credits!"
                };
                
                _notificationManager.ShowNotification(message, NotificationType.Success);
            }
        }
        
        /// <summary>
        /// Log detailed salary breakdown
        /// </summary>
        private void LogSalaryBreakdown()
        {
            Debug.Log("=== SALARY BREAKDOWN ===");
            Debug.Log($"Base Salary: {_currentBreakdown.BaseSalary} credits");
            
            if (_currentBreakdown.ExtraShipBonus > 0)
                Debug.Log($"Extra Ships ({_currentBreakdown.ExtraShipsProcessed}): +{_currentBreakdown.ExtraShipBonus} credits");
            
            if (_currentBreakdown.StrikePenalty > 0)
                Debug.Log($"Strike Penalty ({_currentBreakdown.StrikesIncurred}): -{_currentBreakdown.StrikePenalty} credits");
            
            if (_currentBreakdown.PerformanceBonus > 0)
                Debug.Log($"Performance Bonus: +{_currentBreakdown.PerformanceBonus} credits");
            
            if (_currentBreakdown.PerfectDayBonus > 0)
                Debug.Log($"Perfect Day Bonus: +{_currentBreakdown.PerfectDayBonus} credits");
            
            if (_currentBreakdown.LoyaltyBonus > 0)
                Debug.Log($"Loyalty Bonus: +{_currentBreakdown.LoyaltyBonus} credits");
            
            Debug.Log($"TOTAL SALARY: {_currentBreakdown.TotalSalary} credits");
            Debug.Log($"Ships: {_currentBreakdown.ShipsProcessed}/{_currentBreakdown.RequiredShips}");
            Debug.Log($"Accuracy: {_currentBreakdown.Accuracy:P1}, Efficiency: {_currentBreakdown.Efficiency:P1}");
            Debug.Log("=== END BREAKDOWN ===");
        }
        
        /// <summary>
        /// Get salary statistics
        /// </summary>
        public SalaryStatistics GetStatistics()
        {
            return new SalaryStatistics
            {
                CurrentSalary = _currentSalary,
                TotalEarnedToday = _totalEarnedToday,
                TotalEarnedThisSession = _totalEarnedThisSession,
                HighestDailySalary = _highestDailySalary,
                LowestDailySalary = LowestDailySalary,
                AverageDailySalary = _averageDailySalary,
                TotalDaysWorked = _salaryHistory.Count,
                PerfectDays = _salaryHistory.Count(r => r.WasPerfectDay),
                CurrentBreakdown = _currentBreakdown
            };
        }
        
        /// <summary>
        /// Reset daily salary tracking
        /// </summary>
        private void ResetDailyTracking()
        {
            _currentSalary = 0;
            _totalEarnedToday = 0;
            _currentBreakdown = new SalaryBreakdown();
            _isPerfectDay = false;
            _isHighEfficiency = false;
            _isHighAccuracy = false;
            _isFastProcessing = false;
            
            if (enableDebugLogs)
                Debug.Log("[SalaryManager] Daily tracking reset for new day");
        }
        
        /// <summary>
        /// Set salary values (for loading saved games)
        /// </summary>
        public void SetSalaryValues(int currentSalary, int totalEarnedToday, int totalEarnedSession)
        {
            _currentSalary = currentSalary;
            _totalEarnedToday = totalEarnedToday;
            _totalEarnedThisSession = totalEarnedSession;
            
            if (enableDebugLogs)
                Debug.Log($"[SalaryManager] Salary values loaded: Current={currentSalary}, Today={totalEarnedToday}");
        }
        
        // Event handlers
        private void OnShiftEnded()
        {
            // Calculate and pay salary at end of shift
            CalculateSalary();
            PaySalary();
            RecordDailySalary();
            
            if (enableDebugLogs)
                Debug.Log("[SalaryManager] Shift ended - salary processed");
        }
        
        private void OnDayStarted(int day)
        {
            ResetDailyTracking();
            
            if (enableDebugLogs)
                Debug.Log($"[SalaryManager] Day {day} started - salary tracking reset");
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_dayManager != null)
            {
                DayProgressionManager.OnShiftEnded -= OnShiftEnded;
                DayProgressionManager.OnDayStarted -= OnDayStarted;
            }
            
            // Clear event subscriptions
            OnSalaryCalculated = null;
            OnSalaryPaid = null;
            OnBonusEarned = null;
            OnPenaltyApplied = null;
            OnDailySalaryRecorded = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Calculate Salary")]
        private void TestCalculateSalary()
        {
            int salary = CalculateSalary();
            Debug.Log($"Test salary calculation result: {salary} credits");
        }
        
        [ContextMenu("Test: Pay Salary")]
        private void TestPaySalary()
        {
            CalculateSalary();
            PaySalary();
        }
        
        [ContextMenu("Show Salary Statistics")]
        private void ShowSalaryStatistics()
        {
            var stats = GetStatistics();
            Debug.Log("=== SALARY STATISTICS ===");
            Debug.Log($"Current Salary: {stats.CurrentSalary}");
            Debug.Log($"Today's Earnings: {stats.TotalEarnedToday}");
            Debug.Log($"Session Earnings: {stats.TotalEarnedThisSession}");
            Debug.Log($"Highest Daily: {stats.HighestDailySalary}");
            Debug.Log($"Lowest Daily: {stats.LowestDailySalary}");
            Debug.Log($"Average Daily: {stats.AverageDailySalary:F1}");
            Debug.Log($"Days Worked: {stats.TotalDaysWorked}");
            Debug.Log($"Perfect Days: {stats.PerfectDays}");
            Debug.Log("=== END STATISTICS ===");
        }
    }
    
    // Supporting data structures
    [System.Serializable]
    public class SalaryBreakdown
    {
        public int BaseSalary;
        public int ExtraShipBonus;
        public int ExtraShipsProcessed;
        public int StrikePenalty;
        public int StrikesIncurred;
        public int PerformanceBonus;
        public int EfficiencyBonus;
        public int AccuracyBonus;
        public int SpeedBonus;
        public int PerfectDayBonus;
        public int LoyaltyBonus;
        public int TotalSalary;
        public int ShipsProcessed;
        public int RequiredShips;
        public float Accuracy;
        public float Efficiency;
    }
    
    [System.Serializable]
    public class DailySalaryRecord
    {
        public int Day;
        public int Salary;
        public SalaryBreakdown Breakdown;
        public DateTime Date;
        public bool WasPerfectDay;
        public List<SalaryBonus> PerformanceBonuses;
    }
    
    [System.Serializable]
    public struct SalaryStatistics
    {
        public int CurrentSalary;
        public int TotalEarnedToday;
        public int TotalEarnedThisSession;
        public int HighestDailySalary;
        public int LowestDailySalary;
        public float AverageDailySalary;
        public int TotalDaysWorked;
        public int PerfectDays;
        public SalaryBreakdown CurrentBreakdown;
    }
    
    public enum SalaryBonus
    {
        PerfectDay,
        HighEfficiency,
        HighAccuracy,
        FastProcessing,
        ImperialLoyalty,
        ExtraShips
    }
    
    // Extension method for LINQ Sum
    public static class SalaryExtensions
    {
        public static int Sum<T>(this List<T> source, Func<T, int> selector)
        {
            int sum = 0;
            foreach (var item in source)
            {
                sum += selector(item);
            }
            return sum;
        }
        
        public static int Count<T>(this List<T> source, Func<T, bool> predicate)
        {
            int count = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                    count++;
            }
            return count;
        }
    }
}