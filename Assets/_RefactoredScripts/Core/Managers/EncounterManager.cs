using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages ship encounters, generation, and queuing
    /// Extracted from GameManager for focused encounter responsibility
    /// </summary>
    public class EncounterManager : MonoBehaviour
    {
        [Header("Encounter Settings")]
        [SerializeField] private float baseEncounterInterval = 15f; // Base time between encounters
        [SerializeField] private float encounterVariation = 5f; // Random variation (+/- seconds)
        [SerializeField] private int maxQueueSize = 5; // Maximum encounters in queue
        [SerializeField] private bool autoStartEncounters = true;
        
        [Header("Encounter Types")]
        [SerializeField] private float normalShipChance = 70f; // %
        [SerializeField] private float suspiciousShipChance = 20f; // %
        [SerializeField] private float specialEventChance = 10f; // %
        
        [Header("Difficulty Scaling")]
        [SerializeField] private bool enableDifficultyScaling = true;
        [SerializeField] private float difficultyIncreasePerDay = 0.1f;
        [SerializeField] private float maxDifficultyMultiplier = 2.0f;
        
        [Header("Performance")]
        [SerializeField] private int maxEncountersPerDay = 50;
        [SerializeField] private float encounterTimeout = 300f; // 5 minutes timeout
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableEncounterPreview = false;
        
        // Core state
        private Queue<EncounterData> _encounterQueue = new Queue<EncounterData>();
        private EncounterData _currentEncounter;
        private bool _isEncounterActive = false;
        private bool _isGenerating = false;
        private float _nextEncounterTime;
        private int _encountersGeneratedToday = 0;
        private float _currentDifficultyMultiplier = 1.0f;
        
        // Dependencies
        private DayProgressionManager _dayManager;
        private GameStateManager _gameStateManager;
        
        // Statistics
        private int _totalEncountersGenerated = 0;
        private int _totalEncountersCompleted = 0;
        private float _averageEncounterTime = 0f;
        private List<float> _encounterTimes = new List<float>();
        
        // Events
        public static event Action<EncounterData> OnEncounterGenerated;
        public static event Action<EncounterData> OnEncounterStarted;
        public static event Action<EncounterData> OnEncounterCompleted;
        public static event Action<int> OnQueueSizeChanged;
        public static event Action<float> OnDifficultyChanged;
        
        // Public properties
        public int QueueSize => _encounterQueue.Count;
        public bool IsEncounterActive => _isEncounterActive;
        public EncounterData CurrentEncounter => _currentEncounter;
        public int EncountersGeneratedToday => _encountersGeneratedToday;
        public float CurrentDifficultyMultiplier => _currentDifficultyMultiplier;
        public int TotalEncountersGenerated => _totalEncountersGenerated;
        public int TotalEncountersCompleted => _totalEncountersCompleted;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<EncounterManager>(this);
            
            if (enableDebugLogs)
                Debug.Log("[EncounterManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _dayManager = ServiceLocator.Get<DayProgressionManager>();
            _gameStateManager = ServiceLocator.Get<GameStateManager>();
            
            // Subscribe to events
            if (_dayManager != null)
            {
                DayProgressionManager.OnDayStarted += OnDayStarted;
                DayProgressionManager.OnShiftStarted += OnShiftStarted;
                DayProgressionManager.OnShiftEnded += OnShiftEnded;
                Debug.Log("[EncounterManager] Successfully subscribed to DayProgressionManager events");
            }
            else
            {
                Debug.LogError("[EncounterManager] DayProgressionManager not found - cannot subscribe to shift events!");
            }
            
            if (_gameStateManager != null)
            {
                GameEvents.OnGameStateChanged += OnGameStateChanged;
            }
            
            GameEvents.OnDecisionMade += OnDecisionMade;
            
            // Initialize encounter timing
            ScheduleNextEncounter();
            
            if (enableDebugLogs)
                Debug.Log("[EncounterManager] Encounter system ready");
        }
        
        private void Update()
        {
            // Check if it's time for next encounter
            if (autoStartEncounters && CanGenerateEncounter() && Time.time >= _nextEncounterTime)
            {
                StartCoroutine(GenerateEncounterCoroutine());
            }
            
            // Debug logging every 10 seconds to reduce spam
            if (Time.frameCount % 600 == 0 && enableDebugLogs) // Log every ~10 seconds at 60fps
            {
                bool canGenerate = CanGenerateEncounter();
                bool timeReady = Time.time >= _nextEncounterTime;
                string reason = canGenerate ? "CAN generate" : "CANNOT generate";
                Debug.Log($"[EncounterManager] Status - Day {_dayManager?.CurrentDay ?? -1}: {reason}, " +
                    $"timeReady={timeReady}, queue={_encounterQueue.Count}/{maxQueueSize}");
            }
        }
        
        /// <summary>
        /// Generate a new encounter
        /// </summary>
        public void GenerateEncounter()
        {
            if (!CanGenerateEncounter())
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[EncounterManager] Cannot generate encounter at this time");
                return;
            }
            
            StartCoroutine(GenerateEncounterCoroutine());
        }
        
        /// <summary>
        /// Force reset the encounter timer (for debugging)
        /// </summary>
        [ContextMenu("Force Reset Encounter Timer")]
        public void ForceResetEncounterTimer()
        {
            _nextEncounterTime = Time.time + UnityEngine.Random.Range(2f, 5f); // Quick encounter for testing
            Debug.Log($"[EncounterManager] FORCED timer reset - Next encounter at {_nextEncounterTime:F1}s (in {_nextEncounterTime - Time.time:F1}s)");
        }
        
        /// <summary>
        /// Coroutine to generate encounter with proper timing
        /// </summary>
        private IEnumerator GenerateEncounterCoroutine()
        {
            if (_isGenerating) yield break;
            
            _isGenerating = true;
            
            // Generate encounter data
            EncounterData encounter = CreateEncounterData();
            
            // Add to queue
            _encounterQueue.Enqueue(encounter);
            _encountersGeneratedToday++;
            _totalEncountersGenerated++;
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterManager] Generated encounter: {encounter.ShipType} - {encounter.CaptainName}");
            
            // Trigger events
            OnEncounterGenerated?.Invoke(encounter);
            OnQueueSizeChanged?.Invoke(_encounterQueue.Count);
            GameEvents.TriggerEncounterGenerated(encounter);
            
            // Schedule next encounter
            ScheduleNextEncounter();
            
            // If no encounter is active, start the next one
            if (!_isEncounterActive && _encounterQueue.Count > 0)
            {
                yield return new WaitForSeconds(0.5f); // Brief delay
                StartNextEncounter();
            }
            
            _isGenerating = false;
        }
        
        /// <summary>
        /// Create encounter data based on current game state
        /// </summary>
        private EncounterData CreateEncounterData()
        {
            var encounter = new EncounterData();
            
            // Determine encounter type based on probabilities
            float roll = UnityEngine.Random.Range(0f, 100f);
            if (roll < specialEventChance * _currentDifficultyMultiplier)
            {
                encounter.Type = EncounterType.SpecialEvent;
                encounter = GenerateSpecialEventEncounter();
            }
            else if (roll < (specialEventChance + suspiciousShipChance) * _currentDifficultyMultiplier)
            {
                encounter.Type = EncounterType.Suspicious;
                encounter = GenerateSuspiciousEncounter();
            }
            else
            {
                encounter.Type = EncounterType.Normal;
                encounter = GenerateNormalEncounter();
            }
            
            // Add common data
            encounter.Id = System.Guid.NewGuid().ToString();
            encounter.GeneratedTime = Time.time;
            encounter.Day = _dayManager != null ? _dayManager.CurrentDay : 1;
            encounter.DifficultyMultiplier = _currentDifficultyMultiplier;
            
            return encounter;
        }
        
        /// <summary>
        /// Generate normal encounter
        /// </summary>
        private EncounterData GenerateNormalEncounter()
        {
            var encounter = new EncounterData
            {
                Type = EncounterType.Normal,
                ShipType = GetRandomShipType(false),
                CaptainName = GetRandomCaptainName(),
                AccessCode = GenerateValidAccessCode(),
                IsValid = true,
                ThreatLevel = UnityEngine.Random.Range(1, 4), // 1-3 for normal
                Description = "Standard ship requesting docking clearance"
            };
            
            return encounter;
        }
        
        /// <summary>
        /// Generate suspicious encounter
        /// </summary>
        private EncounterData GenerateSuspiciousEncounter()
        {
            var encounter = new EncounterData
            {
                Type = EncounterType.Suspicious,
                ShipType = GetRandomShipType(true),
                CaptainName = GetRandomCaptainName(),
                ThreatLevel = UnityEngine.Random.Range(3, 6), // 3-5 for suspicious
                Description = "Ship with questionable credentials"
            };
            
            // 50% chance of invalid access code for suspicious ships
            if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
            {
                encounter.AccessCode = GenerateInvalidAccessCode();
                encounter.IsValid = false;
            }
            else
            {
                encounter.AccessCode = GenerateValidAccessCode();
                encounter.IsValid = true;
            }
            
            return encounter;
        }
        
        /// <summary>
        /// Generate special event encounter
        /// </summary>
        private EncounterData GenerateSpecialEventEncounter()
        {
            var encounter = new EncounterData
            {
                Type = EncounterType.SpecialEvent,
                ShipType = GetRandomSpecialShipType(),
                CaptainName = GetRandomSpecialCaptainName(),
                AccessCode = GenerateSpecialAccessCode(),
                IsValid = true, // Special events are usually valid but have special conditions
                ThreatLevel = UnityEngine.Random.Range(1, 8), // Can be any threat level
                Description = "Special diplomatic or military vessel"
            };
            
            return encounter;
        }
        

        /// <summary>
        /// Expose the queue of upcoming encounters
        /// </summary>
        public List<EncounterData> GetUpcomingEncounters(int maxCount = 5)
        {
            return _encounterQueue.Take(maxCount).ToList();
        }

        public int GetQueueSize()
        {
            return _encounterQueue.Count;
        }

        /// <summary>
        /// Start the next encounter from the queue
        /// </summary>
        public void StartNextEncounter()
        {
            if (_isEncounterActive || _encounterQueue.Count == 0)
            {
                if (enableDebugLogs && _encounterQueue.Count == 0)
                    Debug.LogWarning("[EncounterManager] No encounters in queue");
                return;
            }
            
            _currentEncounter = _encounterQueue.Dequeue();
            _isEncounterActive = true;
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterManager] Starting encounter: {_currentEncounter.ShipType}");
            
            // Trigger events
            OnEncounterStarted?.Invoke(_currentEncounter);
            OnQueueSizeChanged?.Invoke(_encounterQueue.Count);
            GameEvents.TriggerEncounterDisplayed(_currentEncounter);
        }
        
        /// <summary>
        /// Complete the current encounter
        /// </summary>
        public void CompleteCurrentEncounter()
        {
            if (!_isEncounterActive || _currentEncounter == null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[EncounterManager] No active encounter to complete");
                return;
            }
            
            // Calculate encounter time
            float encounterTime = Time.time - _currentEncounter.GeneratedTime;
            _encounterTimes.Add(encounterTime);
            _averageEncounterTime = _encounterTimes.Average();
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterManager] Completed encounter: {_currentEncounter.ShipType} in {encounterTime:F1}s");
            
            // Trigger events
            OnEncounterCompleted?.Invoke(_currentEncounter);
            GameEvents.TriggerEncounterCompleted(_currentEncounter);
            
            _totalEncountersCompleted++;
            _isEncounterActive = false;
            _currentEncounter = null;
            
            // Start next encounter if available
            if (_encounterQueue.Count > 0)
            {
                StartCoroutine(DelayedNextEncounter());
            }
        }
        
        /// <summary>
        /// Start next encounter with a small delay
        /// </summary>
        private IEnumerator DelayedNextEncounter()
        {
            yield return new WaitForSeconds(1f); // Brief cooldown between encounters
            StartNextEncounter();
        }
        
        /// <summary>
        /// Schedule the next encounter
        /// </summary>
        private void ScheduleNextEncounter()
        {
            float interval = baseEncounterInterval;
            
            // Apply difficulty scaling
            if (enableDifficultyScaling)
            {
                interval /= _currentDifficultyMultiplier;
            }
            
            // Add random variation
            float variation = UnityEngine.Random.Range(-encounterVariation, encounterVariation);
            interval += variation;
            
            // Ensure minimum interval
            interval = Mathf.Max(interval, 5f);
            
            _nextEncounterTime = Time.time + interval;
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterManager] Next encounter scheduled in {interval:F1}s");
        }
        
        /// <summary>
        /// Check if we can generate a new encounter
        /// </summary>
        private bool CanGenerateEncounter()
        {
            if (_isGenerating) 
            {
                // Don't spam logs for this common condition
                return false;
            }
            if (_encounterQueue.Count >= maxQueueSize) 
            {
                if (enableDebugLogs) Debug.Log($"[EncounterManager] Cannot generate: Queue full ({_encounterQueue.Count}/{maxQueueSize})");
                return false;
            }
            if (_encountersGeneratedToday >= maxEncountersPerDay) 
            {
                if (enableDebugLogs) Debug.Log($"[EncounterManager] Cannot generate: Daily limit reached ({_encountersGeneratedToday}/{maxEncountersPerDay})");
                return false;
            }
            if (_dayManager != null && !_dayManager.CanProcessMoreShips()) 
            {
                // Only log occasionally to prevent spam
                if (enableDebugLogs && Time.frameCount % 600 == 0) 
                    Debug.Log($"[EncounterManager] Cannot generate: DayManager says cannot process more ships (Day {_dayManager.CurrentDay}, Shift: {_dayManager.IsShiftActive}, Remaining: {_dayManager.RemainingTime:F1}s)");
                return false;
            }
            if (_gameStateManager != null && _gameStateManager.CurrentState != GameState.Gameplay) 
            {
                if (enableDebugLogs) Debug.Log($"[EncounterManager] Cannot generate: Game state is {_gameStateManager.CurrentState}, need Gameplay");
                return false;
            }
            
            if (enableDebugLogs) Debug.Log($"[EncounterManager] CAN generate encounter - Day {_dayManager?.CurrentDay ?? -1}");
            return true;
        }
        
        /// <summary>
        /// Update difficulty based on current day
        /// </summary>
        private void UpdateDifficulty()
        {
            if (!enableDifficultyScaling || _dayManager == null) return;
            
            float newDifficulty = 1.0f + ((_dayManager.CurrentDay - 1) * difficultyIncreasePerDay);
            _currentDifficultyMultiplier = Mathf.Min(newDifficulty, maxDifficultyMultiplier);
            
            OnDifficultyChanged?.Invoke(_currentDifficultyMultiplier);
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterManager] Difficulty updated to {_currentDifficultyMultiplier:F2}");
        }
        
        /// <summary>
        /// Clear all encounters (for game reset)
        /// </summary>
        public void ClearAllEncounters()
        {
            _encounterQueue.Clear();
            _currentEncounter = null;
            _isEncounterActive = false;
            _isGenerating = false;
            
            OnQueueSizeChanged?.Invoke(0);
            
            if (enableDebugLogs)
                Debug.Log("[EncounterManager] All encounters cleared");
        }
        
        /// <summary>
        /// Get encounter statistics
        /// </summary>
        public EncounterStatistics GetStatistics()
        {
            return new EncounterStatistics
            {
                TotalGenerated = _totalEncountersGenerated,
                TotalCompleted = _totalEncountersCompleted,
                GeneratedToday = _encountersGeneratedToday,
                CurrentQueueSize = _encounterQueue.Count,
                AverageEncounterTime = _averageEncounterTime,
                CurrentDifficulty = _currentDifficultyMultiplier
            };
        }
        
        // Event handlers
        private void OnDayStarted(int day)
        {
            _encountersGeneratedToday = 0;
            UpdateDifficulty();
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterManager] Day {day} started - encounter tracking reset");
        }
        
        private void OnShiftStarted()
        {
            if (enableDebugLogs)
                Debug.Log("[EncounterManager] Shift started - encounters enabled");
            
            // Reset encounter timing for new shift
            _nextEncounterTime = Time.time + UnityEngine.Random.Range(5f, 10f); // First encounter 5-10s after shift starts
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterManager] Next encounter scheduled at {_nextEncounterTime:F1}s (in {_nextEncounterTime - Time.time:F1}s from now)");
        }
        
        private void OnShiftEnded()
        {
            if (enableDebugLogs)
                Debug.Log("[EncounterManager] Shift ended - encounters paused");
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Gameplay:
                    // Resume encounter generation
                    break;
                    
                case GameState.Paused:
                case GameState.DayReport:
                case GameState.GameOver:
                    // Pause encounter generation
                    break;
            }
        }
        
        private void OnDecisionMade(DecisionType decision, IEncounter encounter)
        {
            // Complete current encounter when a decision is made
            CompleteCurrentEncounter();
        }
        
        // Helper methods for generating encounter data
        private string GetRandomShipType(bool suspicious)
        {
            string[] normalShips = { "Imperium Scout Ship", "Battleship", "Frigate", "Transport Vessel", "Cargo Ship" };
            string[] suspiciousShips = { "Unknown Vessel", "Modified Transport", "Ravager", "Unmarked Ship", "Derelict" };
            
            string[] ships = suspicious ? suspiciousShips : normalShips;
            return ships[UnityEngine.Random.Range(0, ships.Length)];
        }
        
        private string GetRandomSpecialShipType()
        {
            string[] specialShips = { "Imperial Dreadnought", "Diplomatic Vessel", "Research Ship", "Command Carrier", "Supply Convoy" };
            return specialShips[UnityEngine.Random.Range(0, specialShips.Length)];
        }
        
        private string GetRandomCaptainName()
        {
            string[] firstNames = { "Marcus", "Aria", "Zane", "Nova", "Rex", "Luna", "Vex", "Echo" };
            string[] lastNames = { "Steele", "Voss", "Kane", "Cross", "Storm", "Drake", "Fox", "Wolf" };
            
            return $"{firstNames[UnityEngine.Random.Range(0, firstNames.Length)]} {lastNames[UnityEngine.Random.Range(0, lastNames.Length)]}";
        }
        
        private string GetRandomSpecialCaptainName()
        {
            string[] specialNames = { "Admiral Thrawn", "Commander Vex", "Director Krennic", "Captain Phasma", "General Hux" };
            return specialNames[UnityEngine.Random.Range(0, specialNames.Length)];
        }
        
        private string GenerateValidAccessCode()
        {
            string[] prefixes = { "STD", "MIL", "DIP", "TRD", "SPL" };
            string prefix = prefixes[UnityEngine.Random.Range(0, prefixes.Length)];
            int number = UnityEngine.Random.Range(1000, 9999);
            return $"{prefix}-{number}";
        }
        
        private string GenerateInvalidAccessCode()
        {
            string[] invalidPrefixes = { "OLD", "EXP", "ERR", "INV", "BAD" };
            string prefix = invalidPrefixes[UnityEngine.Random.Range(0, invalidPrefixes.Length)];
            int number = UnityEngine.Random.Range(1000, 9999);
            return $"{prefix}-{number}";
        }
        
        private string GenerateSpecialAccessCode()
        {
            string[] specialPrefixes = { "VIP", "EMR", "CMD", "ADM", "SEC" };
            string prefix = specialPrefixes[UnityEngine.Random.Range(0, specialPrefixes.Length)];
            int number = UnityEngine.Random.Range(7000, 9999);
            return $"{prefix}-{number}";
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_dayManager != null)
            {
                DayProgressionManager.OnDayStarted -= OnDayStarted;
                DayProgressionManager.OnShiftStarted -= OnShiftStarted;
                DayProgressionManager.OnShiftEnded -= OnShiftEnded;
            }
            
            GameEvents.OnGameStateChanged -= OnGameStateChanged;
            GameEvents.OnDecisionMade -= OnDecisionMade;
            
            // Clear event subscriptions
            OnEncounterGenerated = null;
            OnEncounterStarted = null;
            OnEncounterCompleted = null;
            OnQueueSizeChanged = null;
            OnDifficultyChanged = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Generate Encounter")]
        private void TestGenerateEncounter() => GenerateEncounter();
        
        [ContextMenu("Test: Start Next Encounter")]
        private void TestStartNextEncounter() => StartNextEncounter();
        
        [ContextMenu("Test: Complete Current Encounter")]
        private void TestCompleteCurrentEncounter() => CompleteCurrentEncounter();
        
        [ContextMenu("Test: Clear All Encounters")]
        private void TestClearAllEncounters() => ClearAllEncounters();
        
        [ContextMenu("Show Encounter Statistics")]
        private void ShowEncounterStatistics()
        {
            var stats = GetStatistics();
            Debug.Log($"=== ENCOUNTER STATISTICS ===");
            Debug.Log($"Generated: {stats.TotalGenerated} total, {stats.GeneratedToday} today");
            Debug.Log($"Completed: {stats.TotalCompleted}");
            Debug.Log($"Queue Size: {stats.CurrentQueueSize}");
            Debug.Log($"Average Time: {stats.AverageEncounterTime:F1}s");
            Debug.Log($"Difficulty: {stats.CurrentDifficulty:F2}x");
            Debug.Log($"=== END STATISTICS ===");
        }
    }
    
    // Supporting data structures
    [System.Serializable]
    public class EncounterData : IEncounter
    {
        public string Id;
        public EncounterType Type;
        public string ShipType { get; set; }
        public string CaptainName { get; set; }
        public string AccessCode { get; set; }
        public bool IsValid { get; set; }
        public int ThreatLevel;
        public string Description;
        public float GeneratedTime;
        public int Day;
        public float DifficultyMultiplier;
    }
    
    public enum EncounterType
    {
        Normal,
        Suspicious,
        SpecialEvent
    }
    
    [System.Serializable]
    public struct EncounterStatistics
    {
        public int TotalGenerated;
        public int TotalCompleted;
        public int GeneratedToday;
        public int CurrentQueueSize;
        public float AverageEncounterTime;
        public float CurrentDifficulty;
    }
}