using UnityEngine;
using System;
using System.Collections;
using Starkiller.Core.Configuration;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages day/time progression, shift timing, and daily cycles
    /// Extracted from GameManager for focused time management
    /// </summary>
    public class DayProgressionManager : MonoBehaviour
    {
        [Header("Time Settings")]
        [SerializeField] private int requiredShipsPerDay = 8; // Fallback if no difficulty profile
        [SerializeField] private float timeBetweenShips = 2f;
        [SerializeField] private bool pauseTimeDuringDecisions = true;
        
        [Header("Difficulty Settings")]
        [SerializeField] private DifficultyProfile difficultyProfile;
        [SerializeField] private bool useDifficultyProfile = true;
        
        [Header("Day Settings")]
        [SerializeField] private int startingDay = 1;
        [SerializeField] private int maxDays = 30; // For campaign mode
        [SerializeField] private bool enableDayLimit = false;
        
        [Header("Shift Control")]
        [SerializeField] private bool autoStartShiftOnGameplay = true; // Set to false when using StartShift button
        [Tooltip("When false, shifts must be manually started via button or code")]
        [SerializeField] private bool requireManualShiftStart = false;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showTimeInUI = true;
        
        // Core state
        private int _currentDay;
        private float _remainingTime;
        private int _shipsProcessedToday;
        private int _totalShipsProcessed; // Accumulates across all days for final score
        private bool _isShiftActive = false;
        private bool _isDayTransitioning = false;
        private bool _timePaused = false;
        
        // Time tracking
        private float _totalTimeToday = 0f;
        private float _timeSpentInDecisions = 0f;
        private float _lastShipTime = 0f;
        
        // Public properties
        public int CurrentDay 
        { 
            get 
            { 
                if (_currentDay < 1)
                {
                    Debug.LogError($"[DayProgressionManager] CurrentDay is {_currentDay} which is less than 1! Returning 1 as fallback.");
                    return 1;
                }
                return _currentDay; 
            }
        }
        public float RemainingTime => _remainingTime;
        public int ShipsProcessedToday => _shipsProcessedToday; // Daily count for UI/quota tracking
        public int TotalShipsProcessed => _totalShipsProcessed; // Total across all days for final score
        public bool IsShiftActive => _isShiftActive;
        public bool IsDayTransitioning => _isDayTransitioning;
        public float ShiftProgress
        {
            get
            {
                // Get actual time limit from ShiftTimerManager
                var timerManager = ServiceLocator.Get<ShiftTimerManager>();
                if (timerManager != null && timerManager.TotalShiftTime > 0)
                {
                    return timerManager.TimeProgress;
                }
                // Fallback calculation (use default 30s)
                const float fallbackTimeLimit = 30f;
                return fallbackTimeLimit > 0 ? (fallbackTimeLimit - _remainingTime) / fallbackTimeLimit : 0f;
            }
        }
        public bool QuotaMet => _shipsProcessedToday >= GetCurrentDayQuota();
        public int ShipsUntilQuota => Mathf.Max(0, GetCurrentDayQuota() - _shipsProcessedToday);
        
        /// <summary>
        /// Get ship quota for current day from difficulty profile or fallback
        /// </summary>
        public int GetCurrentDayQuota()
        {
            if (useDifficultyProfile && difficultyProfile != null)
            {
                var daySettings = difficultyProfile.GetDaySettings(_currentDay);
                return daySettings.shipQuota;
            }
            return requiredShipsPerDay; // Fallback to hardcoded value
        }
        
        // Events
        public static event Action<int> OnDayChanged;
        public static event Action<int> OnDayStarted;
        public static event Action<int> OnDayEnded;
        public static event Action OnShiftStarted;
        public static event Action OnShiftEnded;
        public static event Action<float> OnTimeUpdated;
        public static event Action<int> OnShipProcessed;
        public static event Action OnQuotaReached;
        public static event Action<string> OnTimeWarning; // For low time warnings
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<DayProgressionManager>(this);
            
            if (enableDebugLogs)
                Debug.Log("[DayProgressionManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Initialize day
            _currentDay = startingDay;
            
            Debug.Log($"[DayProgressionManager] *** INITIALIZATION *** Starting at day {_currentDay} (startingDay = {startingDay})");
            
            // Subscribe to relevant events
            GameEvents.OnDecisionMade += OnDecisionCompleted;
            GameEvents.OnGameStateChanged += OnGameStateChanged;
            
            // Subscribe to daily report manager to advance days
            RefactoredDailyReportManager.OnNextDayRequested += OnNextDayRequestedFromReport;
            
            if (enableDebugLogs)
                Debug.Log($"[DayProgressionManager] Started on day {_currentDay}");
        }
        
        private void Update()
        {
            // Check if ShiftTimerManager is handling the timer
            var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
            if (shiftTimerManager != null && shiftTimerManager.IsTimerActive)
            {
                // Let ShiftTimerManager handle the timer completely
                // Just sync our remaining time for compatibility
                _remainingTime = shiftTimerManager.RemainingTime;
                
                // Check if ShiftTimerManager says the shift should end
                if (shiftTimerManager.IsShiftEnded && _isShiftActive)
                {
                    if (enableDebugLogs)
                        Debug.Log("[DayProgressionManager] ShiftTimerManager ended shift - ending our shift too");
                    EndShift();
                }
                return;
            }
            
            // Debug: Check for orphaned shift state
            if (shiftTimerManager != null && !shiftTimerManager.IsTimerActive && _isShiftActive)
            {
                Debug.LogWarning($"[DayProgressionManager] ⚠️ SYNC ISSUE: Shift active but timer inactive! Day {_currentDay}, IsShiftEnded: {shiftTimerManager.IsShiftEnded}");
                
                // If shift ended but we're still active, sync up
                if (shiftTimerManager.IsShiftEnded)
                {
                    Debug.Log("[DayProgressionManager] Auto-syncing: ending shift because ShiftTimerManager says it ended");
                    EndShift();
                    return;
                }
            }
            
            // Only update time if shift is active and not paused (legacy fallback)
            if (_isShiftActive && !_timePaused && !_isDayTransitioning)
            {
                UpdateShiftTimer();
            }
        }
        
        /// <summary>
        /// Start a new day
        /// </summary>
        public void StartNewDay()
        {
            Debug.Log($"[DayProgressionManager] *** StartNewDay() CALLED *** Current day: {_currentDay}, Stack trace: {System.Environment.StackTrace}");
            
            // Check if game is over before starting new day
            var gameOverManager = ServiceLocator.Get<GameOverManager>();
            if (gameOverManager != null && gameOverManager.IsGameOver)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[DayProgressionManager] Cannot start new day - game is over");
                return;
            }
            
            if (_isDayTransitioning)
            {
                Debug.LogWarning($"[DayProgressionManager] *** BLOCKED: Already transitioning days *** Current day: {_currentDay}, Transition flag: {_isDayTransitioning}");
                Debug.LogWarning($"[DayProgressionManager] This call will be ignored. Stack trace: {System.Environment.StackTrace}");
                return;
            }
            
            int previousDay = _currentDay;
            _isDayTransitioning = true; // Set flag BEFORE incrementing
            Debug.Log($"[DayProgressionManager] *** TRANSITION FLAG SET *** Starting day increment process");
            
            _currentDay++;
            ResetDailyTracking();
            
            Debug.Log($"[DayProgressionManager] *** DAY INCREMENT *** Previous: {previousDay}, New: {_currentDay}");
            
            if (enableDebugLogs)
                Debug.Log($"[DayProgressionManager] Starting day {_currentDay}");
            
            // Trigger events with detailed logging
            Debug.Log($"[DayProgressionManager] Triggering OnDayChanged event for day {_currentDay}");
            OnDayChanged?.Invoke(_currentDay);
            Debug.Log($"[DayProgressionManager] Triggering OnDayStarted event for day {_currentDay}");
            OnDayStarted?.Invoke(_currentDay);
            Debug.Log($"[DayProgressionManager] Triggering GameEvents.TriggerDayChanged for day {_currentDay}");
            GameEvents.TriggerDayChanged(_currentDay);
            
            // Notify ConsequenceManager about the new day
            var consequenceManager = ConsequenceManager.Instance;
            if (consequenceManager != null)
            {
                Debug.Log($"[DayProgressionManager] Notifying ConsequenceManager of day {_currentDay}");
                consequenceManager.StartNewDay(_currentDay);
            }
            else
            {
                Debug.LogWarning($"[DayProgressionManager] ConsequenceManager not found to notify of day {_currentDay}");
            }
            
            // Notify MasterShipGenerator about the new day
            var masterShipGenerator = MasterShipGenerator.Instance;
            if (masterShipGenerator != null)
            {
                Debug.Log($"[DayProgressionManager] Notifying MasterShipGenerator of day {_currentDay}");
                masterShipGenerator.StartNewDay(_currentDay);
            }
            else
            {
                Debug.LogWarning($"[DayProgressionManager] MasterShipGenerator not found to notify of day {_currentDay}");
            }
            
            // Check if we've reached day limit
            if (enableDayLimit && _currentDay > maxDays)
            {
                if (enableDebugLogs)
                    Debug.Log("[DayProgressionManager] Max days reached - triggering game end");
                
                GameEvents.TriggerGameEnded();
            }
            
            Debug.Log($"[DayProgressionManager] *** StartNewDay() COMPLETED *** Final day: {_currentDay}");
        }
        
        /// <summary>
        /// Initialize the first day without incrementing the day counter
        /// </summary>
        public void InitializeFirstDay()
        {
            if (_isDayTransitioning)
            {
                Debug.LogWarning("[DayProgressionManager] Already transitioning days");
                return;
            }
            
            // Don't increment _currentDay for the first day
            ResetDailyTracking();
            
            if (enableDebugLogs)
                Debug.Log($"[DayProgressionManager] Initializing day {_currentDay}");
            
            // Trigger events
            OnDayChanged?.Invoke(_currentDay);
            OnDayStarted?.Invoke(_currentDay);
            GameEvents.TriggerDayChanged(_currentDay);
        }
        
        /// <summary>
        /// Set the current day (for synchronization with GameManager)
        /// </summary>
        public void SetCurrentDay(int day)
        {
            int previousDay = _currentDay;
            _currentDay = day;
            Debug.LogWarning($"[DayProgressionManager] *** DAY SET DIRECTLY *** Changed from {previousDay} to {_currentDay}");
            Debug.LogWarning($"[DayProgressionManager] Stack trace: {System.Environment.StackTrace}");
            if (enableDebugLogs)
                Debug.Log($"[DayProgressionManager] Current day set to {_currentDay}");
        }
        
        /// <summary>
        /// Start the work shift
        /// </summary>
        public void StartShift()
        {
            if (_isShiftActive)
            {
                Debug.LogWarning($"[DayProgressionManager] Shift already active (Day {_currentDay})");
                return;
            }
            
            Debug.Log($"[DayProgressionManager] *** STARTING SHIFT FOR DAY {_currentDay} ***");
            Debug.Log($"[DayProgressionManager] BEFORE StartShift: _isShiftActive={_isShiftActive}, _isDayTransitioning={_isDayTransitioning}, _remainingTime={_remainingTime}");
            _isShiftActive = true;
            Debug.Log($"[DayProgressionManager] AFTER StartShift: _isShiftActive={_isShiftActive}");
            
            // Check if ShiftTimerManager is available to handle the timer
            var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
            if (shiftTimerManager != null)
            {
                // Let ShiftTimerManager handle the timer initialization
                if (enableDebugLogs)
                    Debug.Log($"[DayProgressionManager] Shift started - ShiftTimerManager will handle timer");
            }
            else
            {
                // Fallback to legacy timer system (DEPRECATED - ShiftTimerManager should always be available)
                const float fallbackTimeLimit = 30f;
                _remainingTime = fallbackTimeLimit;
                if (enableDebugLogs)
                    Debug.Log($"[DayProgressionManager] Shift started - {_remainingTime:F0} seconds available (legacy/deprecated)");
            }
            
            _totalTimeToday = 0f;
            _timeSpentInDecisions = 0f;
            _lastShipTime = Time.time;
            
            OnShiftStarted?.Invoke();
            GameEvents.TriggerGameStarted(); // Compatibility with old system
        }
        
        /// <summary>
        /// End the current shift
        /// </summary>
        public void EndShift()
        {
            if (!_isShiftActive)
            {
                Debug.LogWarning("[DayProgressionManager] No active shift to end - but forcing end anyway for timer expiration");
                // Don't return - allow the shift end to proceed to trigger daily report
            }
            
            _isShiftActive = false;
            // Note: _isDayTransitioning flag is managed by StartNewDay(), not EndShift()
            
            if (enableDebugLogs)
            {
                Debug.Log($"[DayProgressionManager] Shift ended - Ships processed: {_shipsProcessedToday}/{requiredShipsPerDay}");
                Debug.Log($"[DayProgressionManager] Time spent: {_totalTimeToday:F1}s total, {_timeSpentInDecisions:F1}s in decisions");
            }
            
            // Calculate performance
            bool quotaMet = _shipsProcessedToday >= requiredShipsPerDay;
            
            // Trigger events
            OnShiftEnded?.Invoke();
            OnDayEnded?.Invoke(_currentDay);
            
            // Let other systems handle the end of day report
            GameEvents.TriggerDayChanged(_currentDay);
        }
        
        /// <summary>
        /// Update the shift timer
        /// </summary>
        private void UpdateShiftTimer()
        {
            float deltaTime = Time.deltaTime;
            _remainingTime -= deltaTime;
            _totalTimeToday += deltaTime;
            
            // Trigger periodic time updates
            OnTimeUpdated?.Invoke(_remainingTime);
            
            // Check for time warnings
            CheckTimeWarnings();
            
            // Check if shift time is up
            if (_remainingTime <= 0f)
            {
                _remainingTime = 0f;
                
                if (enableDebugLogs)
                    Debug.Log("[DayProgressionManager] Shift time expired!");
                
                EndShift();
            }
        }
        
        /// <summary>
        /// Check and trigger time warnings
        /// </summary>
        private void CheckTimeWarnings()
        {
            // 30 second warning
            if (_remainingTime <= 30f && _remainingTime > 29f)
            {
                OnTimeWarning?.Invoke("30 seconds remaining!");
                GameEvents.TriggerUINotification("WARNING: 30 seconds left in shift!");
            }
            // 10 second warning
            else if (_remainingTime <= 10f && _remainingTime > 9f)
            {
                OnTimeWarning?.Invoke("10 seconds remaining!");
                GameEvents.TriggerUINotification("CRITICAL: 10 seconds left!");
            }
        }
        
        /// <summary>
        /// Record that a ship was processed
        /// </summary>
        public void RecordShipProcessed()
        {
            // Check if game is over before processing
            var gameOverManager = ServiceLocator.Get<GameOverManager>();
            if (gameOverManager != null && gameOverManager.IsGameOver)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[DayProgressionManager] Cannot process ship - game is over");
                return;
            }
            
            _shipsProcessedToday++;
            _totalShipsProcessed++; // Track total across all days
            _lastShipTime = Time.time;
            
            if (enableDebugLogs)
                Debug.Log($"[DayProgressionManager] Ship processed - Daily: ({_shipsProcessedToday}/{requiredShipsPerDay}), Total: {_totalShipsProcessed}");
            
            OnShipProcessed?.Invoke(_shipsProcessedToday);
            
            // Check if quota was just reached
            if (_shipsProcessedToday == requiredShipsPerDay)
            {
                if (enableDebugLogs)
                    Debug.Log("[DayProgressionManager] Daily quota reached!");
                
                OnQuotaReached?.Invoke();
                GameEvents.TriggerUINotification("Daily quota reached! Bonus pay for extra ships.");
            }
        }
        
        /// <summary>
        /// Pause or unpause the timer
        /// </summary>
        public void SetTimePaused(bool paused)
        {
            _timePaused = paused;
            
            if (enableDebugLogs)
                Debug.Log($"[DayProgressionManager] Time {(paused ? "paused" : "resumed")}");
        }
        
        /// <summary>
        /// Add bonus time (for special events)
        /// </summary>
        public void AddBonusTime(float seconds, string reason = "")
        {
            if (!_isShiftActive) return;
            
            // Note: This is legacy code - ShiftTimerManager handles bonus time
            const float fallbackTimeLimit = 30f;
            _remainingTime = Mathf.Min(_remainingTime + seconds, fallbackTimeLimit);
            
            if (enableDebugLogs)
                Debug.Log($"[DayProgressionManager] Added {seconds}s bonus time. Reason: {reason}. New time: {_remainingTime:F0}s");
            
            OnTimeUpdated?.Invoke(_remainingTime);
            GameEvents.TriggerUINotification($"Bonus time added: +{seconds}s! {reason}");
        }
        
        /// <summary>
        /// Get formatted time string for UI
        /// </summary>
        public string GetFormattedTime()
        {
            int minutes = Mathf.FloorToInt(_remainingTime / 60f);
            int seconds = Mathf.FloorToInt(_remainingTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
        
        /// <summary>
        /// Get daily summary for reporting
        /// </summary>
        public DailySummary GetDailySummary()
        {
            return new DailySummary
            {
                day = _currentDay,
                shipsProcessed = _shipsProcessedToday,
                quotaMet = QuotaMet,
                totalTimeUsed = _totalTimeToday,
                timeInDecisions = _timeSpentInDecisions,
                efficiency = _shipsProcessedToday > 0 ? _totalTimeToday / _shipsProcessedToday : 0f
            };
        }
        
        /// <summary>
        /// Reset daily tracking variables (but keep total across days)
        /// </summary>
        private void ResetDailyTracking()
        {
            int previousShipsToday = _shipsProcessedToday;
            _shipsProcessedToday = 0; // Reset daily count for new day
            // _totalShipsProcessed stays the same - accumulates across days
            _totalTimeToday = 0f;
            _timeSpentInDecisions = 0f;
            
            // Ensure shift state is clean for new day
            _isShiftActive = false;
            _timePaused = false;
            
            // Reset timer manager state to ensure clean start
            var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
            if (shiftTimerManager != null)
            {
                if (shiftTimerManager.IsTimerActive)
                {
                    Debug.Log($"[DayProgressionManager] Stopping previous day's timer for clean day {_currentDay} start");
                    shiftTimerManager.StopTimer();
                }
            }
            
            Debug.Log($"[DayProgressionManager] *** DAILY RESET *** Ships today: {previousShipsToday} → 0, Total ships preserved: {_totalShipsProcessed}, Shift state reset for Day {_currentDay}");
            
            if (enableDebugLogs)
                Debug.Log($"[DayProgressionManager] Daily tracking reset - Starting fresh day {_currentDay} with 0 ships (Total processed: {_totalShipsProcessed})");
            
            // Clear the transitioning flag AFTER all events are fired to ensure proper sequencing
            StartCoroutine(ClearTransitioningFlag());
        }
        
        /// <summary>
        /// Clear the transitioning flag after one frame to ensure all events are processed
        /// </summary>
        private IEnumerator ClearTransitioningFlag()
        {
            yield return null; // Wait one frame for all events to be processed
            
            // Add safety delay and force clear the flag
            yield return new WaitForSeconds(0.5f);
            
            _isDayTransitioning = false;
            Debug.Log($"[DayProgressionManager] *** TRANSITION FLAG CLEARED *** Day {_currentDay} transition completed");
            
            // Emergency backup - set a delayed clear just in case
            Invoke(nameof(ForceClearTransitionFlag), 2f);
        }
        
        /// <summary>
        /// Emergency method to force clear transition flag if it gets stuck
        /// </summary>
        [ContextMenu("Force Clear Transition Flag")]
        public void ForceClearTransitionFlag()
        {
            _isDayTransitioning = false;
            Debug.LogWarning($"[DayProgressionManager] *** EMERGENCY TRANSITION FLAG CLEAR *** Day {_currentDay}");
        }
        
        /// <summary>
        /// Handle decision completion events
        /// </summary>
        private void OnDecisionCompleted(Starkiller.Core.DecisionType decision, Starkiller.Core.IEncounter encounter)
        {
            // Record ship as processed
            RecordShipProcessed();
            
            // Track time spent in decision if we're pausing during decisions
            if (pauseTimeDuringDecisions)
            {
                float decisionTime = Time.time - _lastShipTime;
                _timeSpentInDecisions += decisionTime;
            }
        }
        
        /// <summary>
        /// Handle next day request from daily report manager
        /// </summary>
        private void OnNextDayRequestedFromReport(int requestedDay)
        {
            Debug.Log($"[DayProgressionManager] *** NEXT DAY REQUESTED FROM REPORT *** Report requesting day {requestedDay}, current DayProgressionManager day: {_currentDay}");
            
            // The RefactoredDailyReportManager has already incremented its own day counter
            // We need to sync our day counter and trigger the proper day start sequence
            if (requestedDay != _currentDay + 1)
            {
                Debug.LogWarning($"[DayProgressionManager] Day sync issue! Report wants day {requestedDay}, but we would expect {_currentDay + 1}");
            }
            
            // Start the new day properly through our system
            StartNewDay();
        }
        
        /// <summary>
        /// Handle game state changes
        /// </summary>
        private void OnGameStateChanged(Starkiller.Core.GameState newState)
        {
            Debug.Log($"[DayProgressionManager] *** GAME STATE CHANGED *** Day {_currentDay}: {newState}, IsShiftActive: {_isShiftActive}");
            
            switch (newState)
            {
                case Starkiller.Core.GameState.Gameplay:
                    Debug.Log($"[DayProgressionManager] Gameplay state - Day {_currentDay}, ShiftActive: {_isShiftActive}");
                    if (!_isShiftActive && !requireManualShiftStart && autoStartShiftOnGameplay)
                    {
                        Debug.Log($"[DayProgressionManager] Auto-starting shift for gameplay on Day {_currentDay}");
                        StartShift();
                    }
                    else if (!_isShiftActive && requireManualShiftStart)
                    {
                        Debug.Log($"[DayProgressionManager] Manual shift start required - waiting for button click");
                    }
                    break;
                    
                case Starkiller.Core.GameState.Paused:
                    SetTimePaused(true);
                    break;
                    
                case Starkiller.Core.GameState.DayReport:
                    if (_isShiftActive)
                        EndShift();
                    break;
                    
                case Starkiller.Core.GameState.GameOver:
                    // Stop everything when game over
                    _isShiftActive = false;
                    _timePaused = true;
                    _isDayTransitioning = false;
                    
                    if (enableDebugLogs)
                        Debug.Log("[DayProgressionManager] Game over state - stopping all progression");
                    break;
            }
        }
        
        /// <summary>
        /// Set day values (for loading saves)
        /// </summary>
        public void SetDayValues(int day, int shipsProcessed, float remainingTime, int totalShips = -1)
        {
            _currentDay = Mathf.Max(1, day);
            _shipsProcessedToday = Mathf.Max(0, shipsProcessed);
            // Note: This is legacy code - actual time is managed by ShiftTimerManager
            const float fallbackTimeLimit = 30f;
            _remainingTime = Mathf.Clamp(remainingTime, 0f, fallbackTimeLimit);
            
            // Set total ships if provided, otherwise keep current total
            if (totalShips >= 0)
                _totalShipsProcessed = totalShips;
            
            if (enableDebugLogs)
                Debug.Log($"[DayProgressionManager] Day values loaded: Day {_currentDay}, {_shipsProcessedToday} ships today, {_totalShipsProcessed} total, {_remainingTime:F0}s remaining");
            
            OnDayChanged?.Invoke(_currentDay);
            OnShipProcessed?.Invoke(_shipsProcessedToday);
            OnTimeUpdated?.Invoke(_remainingTime);
        }
        
        /// <summary>
        /// Check if we can process more ships today
        /// </summary>
        public bool CanProcessMoreShips()
        {
            // Check if game is over
            var gameOverManager = ServiceLocator.Get<GameOverManager>();
            if (gameOverManager != null && gameOverManager.IsGameOver)
            {
                Debug.Log($"[DayProgressionManager] CanProcessMoreShips: NO - Game is over");
                return false;
            }
            
            bool canProcess = _isShiftActive && _remainingTime > 0f && !_isDayTransitioning;
            
            if (!canProcess && enableDebugLogs)
            {
                Debug.Log($"[DayProgressionManager] CanProcessMoreShips: NO - Day {_currentDay}, ShiftActive: {_isShiftActive}, RemainingTime: {_remainingTime:F1}s, DayTransitioning: {_isDayTransitioning}");
            }
            
            return canProcess;
        }
        
        /// <summary>
        /// Get time since last ship
        /// </summary>
        public float GetTimeSinceLastShip()
        {
            return Time.time - _lastShipTime;
        }
        
        /// <summary>
        /// Stop all day progression (for game over)
        /// </summary>
        public void StopDayProgression()
        {
            _isShiftActive = false;
            _timePaused = true;
            _isDayTransitioning = false;
            
            if (enableDebugLogs)
                Debug.Log("[DayProgressionManager] All day progression stopped");
        }
        
        /// <summary>
        /// Reset for a new game - single source of truth for day management
        /// </summary>
        public void ResetForNewGame()
        {
            Debug.LogWarning($"[DayProgressionManager] *** RESET FOR NEW GAME CALLED *** Previous day was {_currentDay}, resetting to {startingDay}");
            Debug.LogWarning($"[DayProgressionManager] Stack trace: {System.Environment.StackTrace}");
            
            _currentDay = startingDay; // Usually 1
            _shipsProcessedToday = 0;
            _totalShipsProcessed = 0;
            _remainingTime = 0;
            _isShiftActive = false;
            _isDayTransitioning = false;
            _timePaused = false;
            _totalTimeToday = 0f;
            _timeSpentInDecisions = 0f;
            
            if (enableDebugLogs)
                Debug.Log($"[DayProgressionManager] Reset for new game - Starting at day {_currentDay}");
            
            // Fire day changed event to update UI
            OnDayChanged?.Invoke(_currentDay);
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            GameEvents.OnDecisionMade -= OnDecisionCompleted;
            GameEvents.OnGameStateChanged -= OnGameStateChanged;
            RefactoredDailyReportManager.OnNextDayRequested -= OnNextDayRequestedFromReport;
            
            // Clear event subscriptions
            OnDayChanged = null;
            OnDayStarted = null;
            OnDayEnded = null;
            OnShiftStarted = null;
            OnShiftEnded = null;
            OnTimeUpdated = null;
            OnShipProcessed = null;
            OnQuotaReached = null;
            OnTimeWarning = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Start New Day")]
        private void TestStartNewDay() => StartNewDay();
        
        [ContextMenu("Test: Start Shift")]
        private void TestStartShift() => StartShift();
        
        [ContextMenu("Test: End Shift")]
        private void TestEndShift() => EndShift();
        
        [ContextMenu("Test: Process Ship")]
        private void TestProcessShip() => RecordShipProcessed();
        
        [ContextMenu("Test: Add 30s Bonus Time")]
        private void TestAddBonusTime() => AddBonusTime(30f, "Debug Test");
        
        [ContextMenu("Show Daily Summary")]
        private void ShowDailySummary()
        {
            var summary = GetDailySummary();
            Debug.Log($"Day {summary.day}: {summary.shipsProcessed} ships, Quota: {(summary.quotaMet ? "MET" : "NOT MET")}, " +
                     $"Time: {summary.totalTimeUsed:F1}s, Efficiency: {summary.efficiency:F1}s/ship");
        }
    }
    
    /// <summary>
    /// Daily summary data structure
    /// </summary>
    [System.Serializable]
    public struct DailySummary
    {
        public int day;
        public int shipsProcessed;
        public bool quotaMet;
        public float totalTimeUsed;
        public float timeInDecisions;
        public float efficiency;
    }
}