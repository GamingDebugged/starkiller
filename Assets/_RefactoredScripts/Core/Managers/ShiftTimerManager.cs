using UnityEngine;
using System;
using System.Collections;
using TMPro;
using Starkiller.Core.Configuration;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages shift timer functionality including countdown, display, and time warnings
    /// Extracted from GameManager for focused timer responsibility
    /// 
    /// IMPORTANT: This is the AUTHORITATIVE source for all shift time limits in the game.
    /// GameManager and DayProgressionManager have deprecated shiftTimeLimit fields that are no longer used.
    /// 
    /// Time limit priority:
    /// 1. Explicit timeLimit parameter passed to StartTimer() (highest priority)
    /// 2. DifficultyProfile settings when useDifficultyProfile = true (recommended)
    /// 3. ShiftTimerManager's shiftTimeLimit field (fallback only)
    /// </summary>
    public class ShiftTimerManager : MonoBehaviour
    {
        [Header("Timer Settings")]
        [SerializeField] private float shiftTimeLimit = 30f; // 30 seconds default (overridden by difficulty profile)
        [SerializeField] private bool enableTimeWarnings = true;
        [SerializeField] private float[] warningTimes = { 10f, 5f }; // Warning at 10s and 5s
        [SerializeField] private bool enableBonusTime = true;
        [SerializeField] private float maxBonusTime = 60f; // Maximum bonus time allowed
        
        [Header("Difficulty Management")]
        [SerializeField] private DifficultyProfile difficultyProfile;
        [SerializeField] private bool useDifficultyProfile = true;
        [SerializeField] private bool logDifficultyChanges = true;
        
        [Header("Timer Display")]
        [SerializeField] private string timeFormat = "Shift Time: {0:00}:{1:00}";
        [SerializeField] private string warningFormat = "⚠️ {0:00}:{1:00}";
        [SerializeField] private string bonusFormat = "✨ {0:00}:{1:00}";
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private Color bonusColor = Color.green;
        
        [Header("UI References")]
        [SerializeField] private TMP_Text gameTimerText;
        [SerializeField] private GameObject timerWarningPanel;
        [SerializeField] private TMP_Text warningText;
        
        [Header("Audio")]
        [SerializeField] private bool enableTimerSounds = true;
        [SerializeField] private string tickSoundName = "timer_tick";
        [SerializeField] private string warningSoundName = "timer_warning";
        [SerializeField] private string expiredSoundName = "timer_expired";
        [SerializeField] private string bonusSoundName = "bonus_time";
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableCountdownLogs = false;
        [SerializeField] private float countdownLogInterval = 10f;
        
        // Timer state
        private float _remainingTime;
        private float _totalShiftTime;
        private float _bonusTime = 0f;
        private bool _isTimerActive = false;
        private bool _isTimerPaused = false;
        private bool _isShiftEnded = false;
        private TimerPhase _currentPhase = TimerPhase.Normal;
        
        // Current day settings
        private DifficultyProfile.DaySettings _currentDaySettings;
        
        // Warning system
        private bool[] _warningsTriggered;
        private float _lastCountdownLog;
        
        // Dependencies
        private DayProgressionManager _dayManager;
        private GameStateManager _gameStateManager;
        private AudioManager _audioManager;
        private NotificationManager _notificationManager;
        
        // Statistics
        private float _totalTimeSpent = 0f;
        private float _averageDecisionTime = 0f;
        private int _decisionCount = 0;
        private float _lastDecisionTime;
        
        // Events
        public static event Action<float> OnTimeUpdated;
        public static event Action<float> OnTimeWarning;
        public static event Action<float> OnBonusTimeAdded;
        public static event Action OnTimerExpired;
        public static event Action OnTimerPaused;
        public static event Action OnTimerResumed;
        public static event Action<TimerPhase> OnTimerPhaseChanged;
        
        // Public properties
        public float RemainingTime => _remainingTime;
        public float TotalShiftTime => _totalShiftTime;
        public float BonusTime => _bonusTime;
        public bool IsTimerActive => _isTimerActive;
        public bool IsTimerPaused => _isTimerPaused;
        public bool IsShiftEnded => _isShiftEnded;
        public TimerPhase CurrentPhase => _currentPhase;
        public float TimeProgress => _totalShiftTime > 0 ? 1f - (_remainingTime / _totalShiftTime) : 0f;
        public float AverageDecisionTime => _averageDecisionTime;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<ShiftTimerManager>(this);
            
            // Initialize warnings array
            _warningsTriggered = new bool[warningTimes.Length];
            
            if (enableDebugLogs)
                Debug.Log("[ShiftTimerManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _dayManager = ServiceLocator.Get<DayProgressionManager>();
            _gameStateManager = ServiceLocator.Get<GameStateManager>();
            _audioManager = ServiceLocator.Get<AudioManager>();
            _notificationManager = ServiceLocator.Get<NotificationManager>();
            
            // Subscribe to events
            if (_dayManager != null)
            {
                DayProgressionManager.OnShiftStarted += OnShiftStarted;
                DayProgressionManager.OnShiftEnded += OnShiftEnded;
                DayProgressionManager.OnDayStarted += OnDayStarted;
            }
            
            if (_gameStateManager != null)
            {
                GameEvents.OnGameStateChanged += OnGameStateChanged;
            }
            
            GameEvents.OnDecisionMade += OnDecisionMade;
            
            // Initialize UI
            ValidateUIReferences();
            
            if (enableDebugLogs)
                Debug.Log("[ShiftTimerManager] Timer system ready");
        }
        
        private void Update()
        {
            if (!_isTimerActive || _isTimerPaused || _isShiftEnded)
            {
                // Debug why timer isn't updating
                if (!_isTimerActive && enableDebugLogs && _dayManager != null && _dayManager.IsShiftActive)
                {
                    Debug.LogWarning($"[ShiftTimerManager] ⚠️ Timer inactive but shift active! Day {_dayManager.CurrentDay}, TimerActive: {_isTimerActive}, Paused: {_isTimerPaused}, ShiftEnded: {_isShiftEnded}");
                }
                return;
            }
                
            // Check if game is over and stop timer
            var gameOverManager = ServiceLocator.Get<GameOverManager>();
            if (gameOverManager != null && gameOverManager.IsGameOver)
            {
                _isTimerActive = false;
                _isTimerPaused = true;
                return;
            }
            
            // Update timer
            _remainingTime -= Time.deltaTime;
            _totalTimeSpent += Time.deltaTime;
            
            // Update UI
            UpdateTimerDisplay();
            
            // Check for warnings
            CheckTimeWarnings();
            
            // Check for timer expiration
            if (_remainingTime <= 0f)
            {
                _remainingTime = 0f;
                TimerExpired();
            }
            
            // Debug countdown logs - ALWAYS log every 3 seconds for Day 2+ debugging
            if (Time.time - _lastCountdownLog >= 3f)
            {
                _lastCountdownLog = Time.time;
                Debug.Log($"[ShiftTimerManager] ⏰ COUNTDOWN Day {(_dayManager != null ? _dayManager.CurrentDay : -1)}: {_remainingTime:F1}s remaining (Active: {_isTimerActive})");
            }
            
            // Trigger time update event
            OnTimeUpdated?.Invoke(_remainingTime);
        }
        
        /// <summary>
        /// Start the shift timer with configurable time limit
        /// </summary>
        /// <param name="timeLimit">Optional explicit time limit. If 0 or negative, uses difficulty profile or default.</param>
        public void StartTimer(float timeLimit = 0f)
        {
            Debug.Log($"[ShiftTimerManager] *** StartTimer CALLED *** Day: {(_dayManager != null ? _dayManager.CurrentDay : -1)}, timeLimit: {timeLimit}, current shiftTimeLimit: {shiftTimeLimit}, useDifficultyProfile: {useDifficultyProfile}");
            
            // Apply difficulty profile settings if enabled
            if (useDifficultyProfile && difficultyProfile != null && _dayManager != null && timeLimit <= 0)
            {
                if (enableDebugLogs)
                    Debug.Log($"[ShiftTimerManager] Applying difficulty settings for day {_dayManager.CurrentDay}");
                ApplyDifficultySettings(_dayManager.CurrentDay);
            }
            else if (timeLimit > 0)
            {
                shiftTimeLimit = timeLimit;
                if (enableDebugLogs)
                    Debug.Log($"[ShiftTimerManager] Using provided timeLimit: {timeLimit}s");
            }
            else
            {
                if (enableDebugLogs)
                    Debug.Log($"[ShiftTimerManager] Using default shiftTimeLimit: {shiftTimeLimit}s");
            }
            
            // Safety check to prevent zero or negative timer
            if (shiftTimeLimit <= 0)
            {
                Debug.LogError($"[ShiftTimerManager] Invalid shift time limit: {shiftTimeLimit}s! Setting to 30s fallback.");
                shiftTimeLimit = 30f;
            }
            
            _remainingTime = shiftTimeLimit + _bonusTime;
            _totalShiftTime = _remainingTime;
            _isTimerActive = true;
            _isTimerPaused = false;
            _isShiftEnded = false;
            _currentPhase = TimerPhase.Normal;
            
            // Reset warnings
            for (int i = 0; i < _warningsTriggered.Length; i++)
                _warningsTriggered[i] = false;
            
            Debug.Log($"[ShiftTimerManager] *** TIMER STARTED *** Day: {(_dayManager != null ? _dayManager.CurrentDay : -1)}, Time: {_remainingTime:F1}s (Base: {shiftTimeLimit}s, Bonus: {_bonusTime}s), Active: {_isTimerActive}");
            
            UpdateTimerDisplay();
            OnTimerPhaseChanged?.Invoke(_currentPhase);
        }
        
        /// <summary>
        /// Pause the timer
        /// </summary>
        public void PauseTimer()
        {
            if (!_isTimerActive || _isTimerPaused)
                return;
            
            _isTimerPaused = true;
            OnTimerPaused?.Invoke();
            
            if (enableDebugLogs)
                Debug.Log("[ShiftTimerManager] Timer paused");
        }
        
        /// <summary>
        /// Resume the timer
        /// </summary>
        public void ResumeTimer()
        {
            if (!_isTimerActive || !_isTimerPaused)
                return;
            
            _isTimerPaused = false;
            OnTimerResumed?.Invoke();
            
            if (enableDebugLogs)
                Debug.Log("[ShiftTimerManager] Timer resumed");
        }
        
        /// <summary>
        /// Stop the timer
        /// </summary>
        public void StopTimer()
        {
            _isTimerActive = false;
            _isTimerPaused = false;
            
            if (enableDebugLogs)
                Debug.Log("[ShiftTimerManager] Timer stopped");
        }
        
        /// <summary>
        /// Add bonus time to the timer
        /// </summary>
        public void AddBonusTime(float bonusSeconds, string reason = "")
        {
            if (!enableBonusTime || _isShiftEnded)
                return;
            
            // Apply difficulty profile bonus multiplier if available
            if (_currentDaySettings != null)
            {
                bonusSeconds *= _currentDaySettings.bonusTimeMultiplier;
            }
            
            float actualBonus = Mathf.Min(bonusSeconds, maxBonusTime - _bonusTime);
            
            if (actualBonus > 0)
            {
                _bonusTime += actualBonus;
                _remainingTime += actualBonus;
                
                if (enableDebugLogs)
                    Debug.Log($"[ShiftTimerManager] Bonus time added: +{actualBonus:F1}s ({reason})");
                
                // Play bonus sound
                if (_audioManager != null && enableTimerSounds)
                {
                    _audioManager.PlaySound(bonusSoundName);
                }
                
                // Show notification
                if (_notificationManager != null)
                {
                    _notificationManager.ShowNotification($"Bonus time: +{actualBonus:F0}s {reason}", 
                        NotificationType.Success);
                }
                
                OnBonusTimeAdded?.Invoke(actualBonus);
            }
        }
        
        /// <summary>
        /// Set timer to specific remaining time
        /// </summary>
        public void SetRemainingTime(float timeInSeconds)
        {
            _remainingTime = Mathf.Max(0, timeInSeconds);
            UpdateTimerDisplay();
            
            if (enableDebugLogs)
                Debug.Log($"[ShiftTimerManager] Remaining time set to: {_remainingTime:F1}s");
        }
        
        /// <summary>
        /// Get formatted time string
        /// </summary>
        public string GetFormattedTime(bool includePhaseIcon = true)
        {
            // Ensure we don't show negative time
            float displayTime = Mathf.Max(0f, _remainingTime);
            int minutes = Mathf.FloorToInt(displayTime / 60);
            int seconds = Mathf.FloorToInt(displayTime % 60);
            
            string format = _currentPhase switch
            {
                TimerPhase.Warning => warningFormat,
                TimerPhase.Critical => warningFormat,
                TimerPhase.Bonus => bonusFormat,
                _ => timeFormat
            };
            
            return string.Format(format, minutes, seconds);
        }
        
        /// <summary>
        /// Update timer display
        /// </summary>
        private void UpdateTimerDisplay()
        {
            if (gameTimerText == null)
                return;
            
            gameTimerText.text = GetFormattedTime();
            
            // Update color based on phase
            Color textColor = _currentPhase switch
            {
                TimerPhase.Warning => warningColor,
                TimerPhase.Critical => criticalColor,
                TimerPhase.Bonus => bonusColor,
                _ => normalColor
            };
            
            gameTimerText.color = textColor;
        }
        
        /// <summary>
        /// Check for time warnings
        /// </summary>
        private void CheckTimeWarnings()
        {
            if (!enableTimeWarnings)
                return;
            
            TimerPhase previousPhase = _currentPhase;
            
            // Check each warning time
            for (int i = 0; i < warningTimes.Length; i++)
            {
                if (!_warningsTriggered[i] && _remainingTime <= warningTimes[i])
                {
                    _warningsTriggered[i] = true;
                    TriggerTimeWarning(warningTimes[i]);
                    
                    // Update phase
                    if (i == warningTimes.Length - 1) // Last warning = critical
                        _currentPhase = TimerPhase.Critical;
                    else
                        _currentPhase = TimerPhase.Warning;
                }
            }
            
            // Check if we have bonus time
            if (_bonusTime > 0 && _remainingTime > shiftTimeLimit)
            {
                _currentPhase = TimerPhase.Bonus;
            }
            
            // Trigger phase change event
            if (_currentPhase != previousPhase)
            {
                OnTimerPhaseChanged?.Invoke(_currentPhase);
            }
        }
        
        /// <summary>
        /// Trigger time warning
        /// </summary>
        private void TriggerTimeWarning(float warningTime)
        {
            if (enableDebugLogs)
                Debug.Log($"[ShiftTimerManager] Time warning: {warningTime}s remaining");
            
            // Play warning sound
            if (_audioManager != null && enableTimerSounds)
            {
                _audioManager.PlaySound(warningSoundName);
            }
            
            // Show notification
            if (_notificationManager != null)
            {
                string message = warningTime <= 10f ? 
                    $"⚠️ URGENT: {warningTime:F0} seconds remaining!" : 
                    $"⚠️ Warning: {warningTime:F0} seconds remaining";
                
                _notificationManager.ShowNotification(message, NotificationType.Warning);
            }
            
            // Show warning panel
            if (timerWarningPanel != null)
            {
                StartCoroutine(ShowWarningPanel(warningTime));
            }
            
            OnTimeWarning?.Invoke(warningTime);
        }
        
        /// <summary>
        /// Show warning panel temporarily
        /// </summary>
        private IEnumerator ShowWarningPanel(float warningTime)
        {
            timerWarningPanel.SetActive(true);
            
            if (warningText != null)
            {
                warningText.text = $"⚠️ {warningTime:F0} SECONDS REMAINING!";
            }
            
            yield return new WaitForSeconds(2f);
            
            timerWarningPanel.SetActive(false);
        }
        
        /// <summary>
        /// Handle timer expiration
        /// </summary>
        private void TimerExpired()
        {
            _isTimerActive = false;
            _isShiftEnded = true;
            
            Debug.Log($"[ShiftTimerManager] *** TIMER EXPIRED *** Day: {(_dayManager != null ? _dayManager.CurrentDay : -1)}, RemainingTime: {_remainingTime:F1}s - FORCING shift end");
            
            // Play expired sound
            if (_audioManager != null && enableTimerSounds)
            {
                _audioManager.PlaySound(expiredSoundName);
            }
            
            // Show notification
            if (_notificationManager != null)
            {
                _notificationManager.ShowNotification("⏰ Shift time expired!", NotificationType.Error);
            }
            
            OnTimerExpired?.Invoke();
            
            // FORCE end shift even if day manager thinks it's not active
            if (_dayManager != null)
            {
                if (!_dayManager.IsShiftActive)
                {
                    Debug.LogWarning($"[ShiftTimerManager] Day manager says shift not active, but timer expired! Day {_dayManager.CurrentDay} - forcing EndShift()");
                }
                _dayManager.EndShift();
            }
            else
            {
                Debug.LogError("[ShiftTimerManager] DayManager is null when timer expired!");
            }
        }
        
        /// <summary>
        /// Track decision timing
        /// </summary>
        private void TrackDecisionTime()
        {
            if (_lastDecisionTime > 0)
            {
                float decisionTime = Time.time - _lastDecisionTime;
                _averageDecisionTime = (_averageDecisionTime * _decisionCount + decisionTime) / (_decisionCount + 1);
                _decisionCount++;
            }
            
            _lastDecisionTime = Time.time;
        }
        
        /// <summary>
        /// Get timer statistics
        /// </summary>
        public TimerStatistics GetStatistics()
        {
            return new TimerStatistics
            {
                TotalTimeSpent = _totalTimeSpent,
                AverageDecisionTime = _averageDecisionTime,
                DecisionCount = _decisionCount,
                BonusTimeEarned = _bonusTime,
                TimeProgress = TimeProgress,
                CurrentPhase = _currentPhase
            };
        }
        
        /// <summary>
        /// Reset timer statistics
        /// </summary>
        public void ResetStatistics()
        {
            _totalTimeSpent = 0f;
            _averageDecisionTime = 0f;
            _decisionCount = 0;
            _lastDecisionTime = 0f;
            _bonusTime = 0f;
            
            if (enableDebugLogs)
                Debug.Log("[ShiftTimerManager] Statistics reset");
        }
        
        /// <summary>
        /// Apply difficulty settings for a specific day
        /// </summary>
        private void ApplyDifficultySettings(int day)
        {
            if (difficultyProfile == null)
            {
                if (logDifficultyChanges)
                    Debug.LogWarning("[ShiftTimerManager] No difficulty profile assigned!");
                return;
            }
            
            _currentDaySettings = difficultyProfile.GetDaySettings(day);
            
            if (_currentDaySettings == null)
            {
                if (logDifficultyChanges)
                    Debug.LogWarning($"[ShiftTimerManager] No settings found for day {day}!");
                return;
            }
            
            // Apply timer settings
            shiftTimeLimit = _currentDaySettings.shiftTimeLimit;
            enableTimeWarnings = _currentDaySettings.enableWarnings;
            warningTimes = _currentDaySettings.warningTimes;
            maxBonusTime = _currentDaySettings.maxBonusTime;
            
            // Update warning array size
            _warningsTriggered = new bool[warningTimes.Length];
            
            if (logDifficultyChanges)
            {
                Debug.Log($"[ShiftTimerManager] Applied difficulty settings for day {day}:");
                Debug.Log($"  - Shift Time Limit: {shiftTimeLimit}s");
                Debug.Log($"  - Bonus Time Multiplier: {_currentDaySettings.bonusTimeMultiplier}x");
                Debug.Log($"  - Max Bonus Time: {maxBonusTime}s");
                Debug.Log($"  - Warnings Enabled: {enableTimeWarnings}");
                Debug.Log($"  - Ship Quota: {_currentDaySettings.shipQuota}");
                Debug.Log($"  - Credit Multiplier: {_currentDaySettings.creditMultiplier}x");
                
                if (!string.IsNullOrEmpty(_currentDaySettings.specialEvent))
                {
                    Debug.Log($"  - Special Event: {_currentDaySettings.specialEvent}");
                }
            }
        }
        
        /// <summary>
        /// Get current day settings
        /// </summary>
        public DifficultyProfile.DaySettings GetCurrentDaySettings()
        {
            return _currentDaySettings;
        }
        
        /// <summary>
        /// Get difficulty profile
        /// </summary>
        public DifficultyProfile GetDifficultyProfile()
        {
            return difficultyProfile;
        }
        
        /// <summary>
        /// Set difficulty profile
        /// </summary>
        public void SetDifficultyProfile(DifficultyProfile profile)
        {
            difficultyProfile = profile;
            
            if (logDifficultyChanges)
                Debug.Log($"[ShiftTimerManager] Difficulty profile set to: {(profile != null ? profile.profileName : "None")}");
        }
        
        /// <summary>
        /// Get time limit for a specific day (without applying it)
        /// </summary>
        public float GetTimeLimitForDay(int day)
        {
            if (!useDifficultyProfile || difficultyProfile == null)
                return shiftTimeLimit;
                
            var daySettings = difficultyProfile.GetDaySettings(day);
            return daySettings?.shiftTimeLimit ?? shiftTimeLimit;
        }
        
        /// <summary>
        /// Validate UI references
        /// </summary>
        private void ValidateUIReferences()
        {
            if (gameTimerText == null)
            {
                Debug.LogError("[ShiftTimerManager] gameTimerText is not assigned!");
            }
            
            if (timerWarningPanel == null && enableTimeWarnings)
            {
                Debug.LogWarning("[ShiftTimerManager] timerWarningPanel is not assigned but warnings are enabled");
            }
            
            // Validate difficulty profile
            if (useDifficultyProfile && difficultyProfile == null)
            {
                Debug.LogWarning("[ShiftTimerManager] Difficulty profile is enabled but no profile is assigned!");
            }
        }
        
        // Event handlers
        private void OnShiftStarted()
        {
            StartTimer();
            
            if (enableDebugLogs)
                Debug.Log("[ShiftTimerManager] Shift started - timer activated");
        }
        
        private void OnShiftEnded()
        {
            StopTimer();
            
            if (enableDebugLogs)
                Debug.Log("[ShiftTimerManager] Shift ended - timer stopped");
        }
        
        private void OnDayStarted(int day)
        {
            // Apply difficulty settings for the new day
            if (useDifficultyProfile && difficultyProfile != null)
            {
                ApplyDifficultySettings(day);
            }
            
            ResetStatistics();
            
            if (enableDebugLogs)
                Debug.Log($"[ShiftTimerManager] Day {day} started - statistics reset, waiting for shift to start");
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Gameplay:
                    if (_isTimerActive && _isTimerPaused)
                        ResumeTimer();
                    break;
                    
                case GameState.Paused:
                    if (_isTimerActive && !_isTimerPaused)
                        PauseTimer();
                    break;
                    
                case GameState.DayReport:
                case GameState.GameOver:
                    StopTimer();
                    break;
            }
        }
        
        private void OnDecisionMade(DecisionType decision, IEncounter encounter)
        {
            TrackDecisionTime();
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_dayManager != null)
            {
                DayProgressionManager.OnShiftStarted -= OnShiftStarted;
                DayProgressionManager.OnShiftEnded -= OnShiftEnded;
                DayProgressionManager.OnDayStarted -= OnDayStarted;
            }
            
            GameEvents.OnGameStateChanged -= OnGameStateChanged;
            GameEvents.OnDecisionMade -= OnDecisionMade;
            
            // Clear event subscriptions
            OnTimeUpdated = null;
            OnTimeWarning = null;
            OnBonusTimeAdded = null;
            OnTimerExpired = null;
            OnTimerPaused = null;
            OnTimerResumed = null;
            OnTimerPhaseChanged = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Add 30s Bonus Time")]
        private void TestAddBonusTime() => AddBonusTime(30f, "Debug Test");
        
        [ContextMenu("Test: Set 30s Remaining")]
        private void TestSetLowTime() => SetRemainingTime(30f);
        
        [ContextMenu("Test: Set 10s Remaining")]
        private void TestSetCriticalTime() => SetRemainingTime(10f);
        
        [ContextMenu("Test: Pause Timer")]
        private void TestPauseTimer() => PauseTimer();
        
        [ContextMenu("Test: Resume Timer")]
        private void TestResumeTimer() => ResumeTimer();
        
        [ContextMenu("Show Timer Statistics")]
        private void ShowTimerStatistics()
        {
            var stats = GetStatistics();
            Debug.Log("=== TIMER STATISTICS ===");
            Debug.Log($"Total Time Spent: {stats.TotalTimeSpent:F1}s");
            Debug.Log($"Average Decision Time: {stats.AverageDecisionTime:F1}s");
            Debug.Log($"Decision Count: {stats.DecisionCount}");
            Debug.Log($"Bonus Time Earned: {stats.BonusTimeEarned:F1}s");
            Debug.Log($"Time Progress: {stats.TimeProgress:P1}");
            Debug.Log($"Current Phase: {stats.CurrentPhase}");
            Debug.Log("=== END STATISTICS ===");
        }
        
        [ContextMenu("Show Current Day Settings")]
        private void ShowCurrentDaySettings()
        {
            if (_currentDaySettings == null)
            {
                Debug.Log("No current day settings loaded");
                return;
            }
            
            Debug.Log("=== CURRENT DAY SETTINGS ===");
            Debug.Log($"Day Number: {_currentDaySettings.dayNumber}");
            Debug.Log($"Shift Time Limit: {_currentDaySettings.shiftTimeLimit}s");
            Debug.Log($"Bonus Time Multiplier: {_currentDaySettings.bonusTimeMultiplier}x");
            Debug.Log($"Max Bonus Time: {_currentDaySettings.maxBonusTime}s");
            Debug.Log($"Warnings Enabled: {_currentDaySettings.enableWarnings}");
            Debug.Log($"Ship Quota: {_currentDaySettings.shipQuota}");
            Debug.Log($"Credit Multiplier: {_currentDaySettings.creditMultiplier}x");
            Debug.Log($"Special Event: {_currentDaySettings.specialEvent}");
            Debug.Log($"Description: {_currentDaySettings.dayDescription}");
            Debug.Log("=== END DAY SETTINGS ===");
        }
        
        [ContextMenu("Test: Apply Day 1 Settings")]
        private void TestApplyDay1() => ApplyDifficultySettings(1);
        
        [ContextMenu("Test: Apply Day 5 Settings")]
        private void TestApplyDay5() => ApplyDifficultySettings(5);
        
        [ContextMenu("Test: Apply Day 10 Settings")]
        private void TestApplyDay10() => ApplyDifficultySettings(10);
    }
    
    // Supporting enums and structures
    public enum TimerPhase
    {
        Normal,
        Warning,
        Critical,
        Bonus
    }
    
    [System.Serializable]
    public struct TimerStatistics
    {
        public float TotalTimeSpent;
        public float AverageDecisionTime;
        public int DecisionCount;
        public float BonusTimeEarned;
        public float TimeProgress;
        public TimerPhase CurrentPhase;
    }
}