using UnityEngine;
using System;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Handles all credit/money management operations
    /// Extracted from GameManager to provide focused responsibility
    /// </summary>
    public class CreditsManager : MonoBehaviour
    {
        [Header("Credit Settings")]
        [SerializeField] private int startingCredits = 30;
        [SerializeField] private int baseSalary = 30;
        [SerializeField] private int bonusPerExtraShip = 5;
        [SerializeField] private int penaltyPerMistake = 5;
        [SerializeField] private int requiredShipsPerDay = 8;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        
        private int _currentCredits;
        private int _creditsEarnedToday = 0;
        private int _creditsSpentToday = 0;
        
        // Public properties
        public int CurrentCredits => _currentCredits;
        public int CreditsEarnedToday => _creditsEarnedToday;
        public int CreditsSpentToday => _creditsSpentToday;
        public int BaseSalary => baseSalary;
        
        // Events for other systems to listen to
        public static event Action<int, int> OnCreditsChanged; // (newAmount, changeAmount)
        public static event Action<int> OnSalaryCalculated;
        public static event Action<int> OnCreditsSpent;
        public static event Action<int> OnCreditsEarned;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<CreditsManager>(this);
            
            if (enableDebugLogs)
                Debug.Log("[CreditsManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Initialize credits
            _currentCredits = startingCredits;
            
            // Subscribe to relevant events
            GameEvents.OnShipApproved += OnShipProcessed;
            GameEvents.OnShipDenied += OnShipProcessed;
            GameEvents.OnDayChanged += OnNewDay;
            
            // Trigger initial credit update
            TriggerCreditsChanged(0);
            
            if (enableDebugLogs)
                Debug.Log($"[CreditsManager] Started with {_currentCredits} credits");
        }
        
        /// <summary>
        /// Add credits to the player's account
        /// </summary>
        public bool AddCredits(int amount, string reason = "")
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[CreditsManager] Attempted to add non-positive amount: {amount}");
                return false;
            }
            
            _currentCredits += amount;
            _creditsEarnedToday += amount;
            
            if (enableDebugLogs)
                Debug.Log($"[CreditsManager] Added {amount} credits. Reason: {reason}. Total: {_currentCredits}");
            
            TriggerCreditsChanged(amount);
            OnCreditsEarned?.Invoke(amount);
            
            return true;
        }
        
        /// <summary>
        /// Deduct credits from the player's account
        /// </summary>
        public bool DeductCredits(int amount, string reason = "")
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[CreditsManager] Attempted to deduct non-positive amount: {amount}");
                return false;
            }
            
            if (_currentCredits < amount)
            {
                Debug.LogWarning($"[CreditsManager] Insufficient credits. Have: {_currentCredits}, Need: {amount}");
                return false;
            }
            
            _currentCredits -= amount;
            _creditsSpentToday += amount;
            
            if (enableDebugLogs)
                Debug.Log($"[CreditsManager] Deducted {amount} credits. Reason: {reason}. Total: {_currentCredits}");
            
            TriggerCreditsChanged(-amount);
            OnCreditsSpent?.Invoke(amount);
            
            return true;
        }
        
        /// <summary>
        /// Set credits to a specific amount (for loading saves, etc.)
        /// </summary>
        public void SetCredits(int amount, bool triggerEvents = true)
        {
            int previousCredits = _currentCredits;
            _currentCredits = Mathf.Max(0, amount);
            
            if (enableDebugLogs)
                Debug.Log($"[CreditsManager] Credits set to: {_currentCredits}");
            
            if (triggerEvents)
            {
                TriggerCreditsChanged(_currentCredits - previousCredits);
            }
        }
        
        /// <summary>
        /// Calculate daily salary based on ships processed and performance
        /// </summary>
        public int CalculateDailySalary(int shipsProcessed, int correctDecisions, int wrongDecisions)
        {
            int salary = 0;
            
            // Base salary for meeting minimum quota
            if (shipsProcessed >= requiredShipsPerDay)
            {
                salary += baseSalary;
                
                if (enableDebugLogs)
                    Debug.Log($"[CreditsManager] Base salary earned: {baseSalary}");
            }
            else
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[CreditsManager] Quota not met. Ships processed: {shipsProcessed}/{requiredShipsPerDay}");
            }
            
            // Bonus for extra ships
            int extraShips = Mathf.Max(0, shipsProcessed - requiredShipsPerDay);
            int bonusCredits = extraShips * bonusPerExtraShip;
            salary += bonusCredits;
            
            if (extraShips > 0 && enableDebugLogs)
                Debug.Log($"[CreditsManager] Bonus for {extraShips} extra ships: {bonusCredits}");
            
            // Penalty for mistakes
            int penaltyCredits = wrongDecisions * penaltyPerMistake;
            salary -= penaltyCredits;
            
            if (wrongDecisions > 0 && enableDebugLogs)
                Debug.Log($"[CreditsManager] Penalty for {wrongDecisions} mistakes: -{penaltyCredits}");
            
            // Ensure salary isn't negative
            salary = Mathf.Max(0, salary);
            
            if (enableDebugLogs)
                Debug.Log($"[CreditsManager] Total daily salary calculated: {salary}");
            
            OnSalaryCalculated?.Invoke(salary);
            return salary;
        }
        
        /// <summary>
        /// Process daily salary (typically called at end of day)
        /// </summary>
        public void ProcessDailySalary(int shipsProcessed, int correctDecisions, int wrongDecisions)
        {
            int salary = CalculateDailySalary(shipsProcessed, correctDecisions, wrongDecisions);
            
            if (salary > 0)
            {
                AddCredits(salary, "Daily Salary");
            }
        }
        
        /// <summary>
        /// Accept a bribe (adds credits but may have consequences)
        /// </summary>
        public bool AcceptBribe(int bribeAmount, string source = "Unknown")
        {
            if (bribeAmount <= 0) return false;
            
            AddCredits(bribeAmount, $"Bribe from {source}");
            
            // Trigger event for consequence tracking
            GameEvents.TriggerUINotification($"Accepted bribe: {bribeAmount} credits");
            
            if (enableDebugLogs)
                Debug.Log($"[CreditsManager] Accepted bribe of {bribeAmount} from {source}");
            
            return true;
        }
        
        /// <summary>
        /// Process daily expenses (family costs, rent, etc.)
        /// </summary>
        public void ProcessDailyExpenses(int totalExpenses)
        {
            if (totalExpenses <= 0) return;
            
            bool success = DeductCredits(totalExpenses, "Daily Expenses");
            
            if (!success)
            {
                // Handle insufficient funds for expenses
                Debug.LogWarning($"[CreditsManager] Unable to pay daily expenses of {totalExpenses}!");
                GameEvents.TriggerUINotification("Warning: Insufficient funds for daily expenses!");
            }
        }
        
        /// <summary>
        /// Check if player can afford a specific amount
        /// </summary>
        public bool CanAfford(int amount)
        {
            return _currentCredits >= amount;
        }
        
        /// <summary>
        /// Get credit summary for UI display
        /// </summary>
        public string GetCreditSummary()
        {
            return $"Credits: {_currentCredits}\nEarned Today: +{_creditsEarnedToday}\nSpent Today: -{_creditsSpentToday}";
        }
        
        /// <summary>
        /// Reset daily tracking (called at start of new day)
        /// </summary>
        private void OnNewDay(int newDay)
        {
            _creditsEarnedToday = 0;
            _creditsSpentToday = 0;
            
            if (enableDebugLogs)
                Debug.Log($"[CreditsManager] Reset daily tracking for day {newDay}");
        }
        
        /// <summary>
        /// Called when a ship is processed (for any bonuses/penalties)
        /// </summary>
        private void OnShipProcessed(IEncounter encounter)
        {
            // This is a placeholder for any per-ship credit operations
            // Could be extended for specific ship type bonuses, etc.
            if (enableDebugLogs)
                Debug.Log($"[CreditsManager] Ship processed: {encounter?.ShipType ?? "Unknown"}");
        }
        
        /// <summary>
        /// Trigger credit change events
        /// </summary>
        private void TriggerCreditsChanged(int changeAmount)
        {
            OnCreditsChanged?.Invoke(_currentCredits, changeAmount);
            GameEvents.TriggerCreditsChanged(_currentCredits);
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            GameEvents.OnShipApproved -= OnShipProcessed;
            GameEvents.OnShipDenied -= OnShipProcessed;
            GameEvents.OnDayChanged -= OnNewDay;
            
            // Clear event subscriptions
            OnCreditsChanged = null;
            OnSalaryCalculated = null;
            OnCreditsSpent = null;
            OnCreditsEarned = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Add 100 Credits")]
        private void TestAddCredits() => AddCredits(100, "Debug Test");
        
        [ContextMenu("Test: Deduct 50 Credits")]
        private void TestDeductCredits() => DeductCredits(50, "Debug Test");
        
        [ContextMenu("Test: Calculate Sample Salary")]
        private void TestCalculateSalary() => CalculateDailySalary(10, 8, 2);
    }
}