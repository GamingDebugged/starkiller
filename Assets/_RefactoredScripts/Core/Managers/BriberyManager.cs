using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages bribery offers, acceptance, and consequences
    /// Extracted from GameManager for focused bribery responsibility
    /// </summary>
    public class BriberyManager : MonoBehaviour
    {
        [Header("Bribery Settings")]
        [SerializeField] private bool enableBribery = true;
        [SerializeField] private float briberyChance = 0.15f; // 15% chance per encounter
        [SerializeField] private int minBribeAmount = 10;
        [SerializeField] private int maxBribeAmount = 50;
        [SerializeField] private int moralChoiceBribeAmount = 20;
        [SerializeField] private int luxuryBribeAmount = 30;
        
        [Header("Bribery Consequences")]
        [SerializeField] private int loyaltyPenalty = -1; // Imperial loyalty decrease
        [SerializeField] private int sympathyGain = 1; // Rebellion sympathy increase
        [SerializeField] private float detectionRiskIncrease = 0.1f; // Risk increase per bribe
        [SerializeField] private int maxDetectionRisk = 80; // Maximum detection risk %
        
        [Header("Detection System")]
        [SerializeField] private bool enableDetection = true;
        [SerializeField] private float baseDetectionRisk = 5f; // Base 5% detection risk
        [SerializeField] private float detectionDecayRate = 0.02f; // Daily risk reduction
        [SerializeField] private int inspectionTriggerThreshold = 3; // Bribes before inspection
        
        [Header("Audio")]
        [SerializeField] private bool enableBribeySounds = true;
        [SerializeField] private string bribeOfferSound = "bribe_offer";
        [SerializeField] private string bribeAcceptedSound = "bribe_accepted";
        [SerializeField] private string bribeRejectedSound = "bribe_rejected";
        [SerializeField] private string detectionSound = "bribe_detected";
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableBriberyLogging = true;
        
        // Bribery state
        private BriberyOffer _currentOffer;
        private List<BriberyRecord> _briberyHistory = new List<BriberyRecord>();
        private int _totalBribesAccepted = 0;
        private int _totalBribesRejected = 0;
        private int _totalBribeCredits = 0;
        private float _currentDetectionRisk = 0f;
        
        // Detection and consequences
        private int _consecutiveBribes = 0;
        private float _lastDetectionCheck = 0f;
        private bool _isUnderSuspicion = false;
        private List<string> _briberyDecisions = new List<string>();
        
        // Dependencies
        private CreditsManager _creditsManager;
        private LoyaltyManager _loyaltyManager;
        private NotificationManager _notificationManager;
        private AudioManager _audioManager;
        private EncounterManager _encounterManager;
        private PerformanceManager _performanceManager;
        
        // Events
        public static event Action<BriberyOffer> OnBribeOffered;
        public static event Action<BriberyRecord> OnBribeAccepted;
        public static event Action<BriberyOffer> OnBribeRejected;
        public static event Action<float> OnDetectionRiskChanged;
        public static event Action OnBribeDetected;
        public static event Action<bool> OnSuspicionStatusChanged;
        
        // Public properties
        public bool HasActiveBribe => _currentOffer != null;
        public BriberyOffer CurrentOffer => _currentOffer;
        public int TotalBribesAccepted => _totalBribesAccepted;
        public int TotalBribesRejected => _totalBribesRejected;
        public int TotalBribeCredits => _totalBribeCredits;
        public float CurrentDetectionRisk => _currentDetectionRisk;
        public bool IsUnderSuspicion => _isUnderSuspicion;
        public List<BriberyRecord> BriberyHistory => new List<BriberyRecord>(_briberyHistory);
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<BriberyManager>(this);
            
            // Initialize detection risk
            _currentDetectionRisk = baseDetectionRisk;
            
            if (enableDebugLogs)
                Debug.Log("[BriberyManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _creditsManager = ServiceLocator.Get<CreditsManager>();
            _loyaltyManager = ServiceLocator.Get<LoyaltyManager>();
            _notificationManager = ServiceLocator.Get<NotificationManager>();
            _audioManager = ServiceLocator.Get<AudioManager>();
            _encounterManager = ServiceLocator.Get<EncounterManager>();
            _performanceManager = ServiceLocator.Get<PerformanceManager>();
            
            // Subscribe to events
            if (_encounterManager != null)
            {
                EncounterManager.OnEncounterGenerated += OnEncounterGenerated;
            }
            
            if (enableDebugLogs)
                Debug.Log("[BriberyManager] Bribery system ready");
        }
        
        private void Update()
        {
            // Daily detection risk decay
            if (Time.time - _lastDetectionCheck >= 86400f) // 24 hours
            {
                DecayDetectionRisk();
                _lastDetectionCheck = Time.time;
            }
        }
        
        /// <summary>
        /// Generate a bribery offer for an encounter
        /// </summary>
        public BriberyOffer GenerateBribeOffer(IEncounter encounter)
        {
            if (!enableBribery || encounter == null)
                return null;
            
            // Check if this encounter should offer a bribe
            if (UnityEngine.Random.value > briberyChance)
                return null;
            
            // Generate bribe amount based on encounter type
            int bribeAmount = CalculateBribeAmount(encounter);
            
            var offer = new BriberyOffer
            {
                Id = System.Guid.NewGuid().ToString(),
                Encounter = encounter,
                Amount = bribeAmount,
                Reason = GenerateBribeReason(encounter),
                OfferedTime = DateTime.Now,
                DetectionRisk = CalculateOfferDetectionRisk(bribeAmount),
                BribeType = DetermineBribeType(encounter)
            };
            
            _currentOffer = offer;
            
            // Play offer sound
            if (_audioManager != null && enableBribeySounds)
            {
                _audioManager.PlaySound(bribeOfferSound);
            }
            
            // Trigger events
            OnBribeOffered?.Invoke(offer);
            
            if (enableBriberyLogging)
            {
                Debug.Log($"[BriberyManager] Bribe offered: {bribeAmount} credits from {encounter.ShipType}");
            }
            
            return offer;
        }
        
        /// <summary>
        /// Accept the current bribe offer
        /// </summary>
        public void AcceptBribe()
        {
            if (_currentOffer == null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[BriberyManager] No active bribe offer to accept");
                return;
            }
            
            var offer = _currentOffer;
            
            // Create bribery record
            var record = new BriberyRecord
            {
                Id = offer.Id,
                Encounter = offer.Encounter,
                Amount = offer.Amount,
                Reason = offer.Reason,
                OfferedTime = offer.OfferedTime,
                AcceptedTime = DateTime.Now,
                WasAccepted = true,
                DetectionRisk = offer.DetectionRisk,
                BribeType = offer.BribeType
            };
            
            // Apply consequences
            ApplyBribeConsequences(record);
            
            // Update tracking
            _totalBribesAccepted++;
            _totalBribeCredits += offer.Amount;
            _consecutiveBribes++;
            _briberyHistory.Add(record);
            
            // Add to decision tracking
            string decision = $"Accepted a bribe of {offer.Amount} credits";
            _briberyDecisions.Add(decision);
            
            if (_loyaltyManager != null)
            {
                _loyaltyManager.AddKeyDecision(decision);
            }
            
            // Update detection risk
            IncreaseDetectionRisk(offer.DetectionRisk);
            
            // Check for detection
            CheckBribeDetection(record);
            
            // Play accepted sound
            if (_audioManager != null && enableBribeySounds)
            {
                _audioManager.PlaySound(bribeAcceptedSound);
            }
            
            // Show notification
            if (_notificationManager != null)
            {
                string message = $"💰 Bribe accepted: +{offer.Amount} credits";
                if (_currentDetectionRisk > 30f)
                    message += " ⚠️ Suspicion rising!";
                
                _notificationManager.ShowNotification(message, NotificationType.Warning);
            }
            
            // Trigger events
            OnBribeAccepted?.Invoke(record);
            
            // Clear current offer
            _currentOffer = null;
            
            if (enableBriberyLogging)
            {
                Debug.Log($"[BriberyManager] Bribe accepted: {offer.Amount} credits");
            }
        }
        
        /// <summary>
        /// Reject the current bribe offer
        /// </summary>
        public void RejectBribe()
        {
            if (_currentOffer == null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[BriberyManager] No active bribe offer to reject");
                return;
            }
            
            var offer = _currentOffer;
            
            // Update tracking
            _totalBribesRejected++;
            _consecutiveBribes = 0; // Reset consecutive counter
            
            // Small loyalty bonus for rejecting bribes
            if (_loyaltyManager != null)
            {
                _loyaltyManager.UpdateLoyalty(1, 0, $"Rejected bribe of {offer.Amount} credits");
            }
            
            // Play rejected sound
            if (_audioManager != null && enableBribeySounds)
            {
                _audioManager.PlaySound(bribeRejectedSound);
            }
            
            // Show notification
            if (_notificationManager != null)
            {
                _notificationManager.ShowNotification($"✅ Bribe rejected: {offer.Amount} credits", NotificationType.Success);
            }
            
            // Trigger events
            OnBribeRejected?.Invoke(offer);
            
            // Clear current offer
            _currentOffer = null;
            
            if (enableBriberyLogging)
            {
                Debug.Log($"[BriberyManager] Bribe rejected: {offer.Amount} credits");
            }
        }
        
        /// <summary>
        /// Calculate bribe amount based on encounter
        /// </summary>
        private int CalculateBribeAmount(IEncounter encounter)
        {
            int baseAmount = UnityEngine.Random.Range(minBribeAmount, maxBribeAmount + 1);
            
            // Adjust based on encounter type
            if (encounter.ShipType.Contains("Imperial") || encounter.ShipType.Contains("Officer"))
            {
                baseAmount = (int)(baseAmount * 1.5f); // Imperial officers offer more
            }
            else if (encounter.ShipType.Contains("Luxury") || encounter.ShipType.Contains("Contraband"))
            {
                baseAmount = luxuryBribeAmount;
            }
            
            return baseAmount;
        }
        
        /// <summary>
        /// Generate a reason for the bribe
        /// </summary>
        private string GenerateBribeReason(IEncounter encounter)
        {
            string[] reasons = {
                "Overlook expired documentation",
                "Expedite clearance process",
                "Ignore minor manifest discrepancies",
                "Skip additional security checks",
                "Forget about questionable cargo",
                "Approve without full inspection"
            };
            
            return reasons[UnityEngine.Random.Range(0, reasons.Length)];
        }
        
        /// <summary>
        /// Determine the type of bribe being offered
        /// </summary>
        private BribeType DetermineBribeType(IEncounter encounter)
        {
            if (encounter.ShipType.Contains("Imperial"))
                return BribeType.Authority;
            else if (encounter.ShipType.Contains("Smuggler"))
                return BribeType.Contraband;
            else if (encounter.ShipType.Contains("Medical"))
                return BribeType.Humanitarian;
            else
                return BribeType.Standard;
        }
        
        /// <summary>
        /// Calculate detection risk for this specific offer
        /// </summary>
        private float CalculateOfferDetectionRisk(int bribeAmount)
        {
            float risk = detectionRiskIncrease;
            
            // Higher amounts = higher risk
            if (bribeAmount > 30)
                risk *= 1.5f;
            else if (bribeAmount > 20)
                risk *= 1.2f;
            
            // More consecutive bribes = higher risk
            risk *= (1f + _consecutiveBribes * 0.1f);
            
            return risk;
        }
        
        /// <summary>
        /// Apply consequences of accepting a bribe
        /// </summary>
        private void ApplyBribeConsequences(BriberyRecord record)
        {
            // Add credits
            if (_creditsManager != null)
            {
                _creditsManager.AddCredits(record.Amount, $"Bribe: {record.Reason}");
            }
            
            // Update loyalty
            if (_loyaltyManager != null)
            {
                _loyaltyManager.UpdateLoyalty(loyaltyPenalty, sympathyGain, $"Accepted bribe: {record.Reason}");
            }
            
            // Record performance impact
            if (_performanceManager != null)
            {
                _performanceManager.AddKeyDecision($"Accepted bribe of {record.Amount} credits");
            }
        }
        
        /// <summary>
        /// Increase detection risk
        /// </summary>
        private void IncreaseDetectionRisk(float riskIncrease)
        {
            _currentDetectionRisk = Mathf.Min(_currentDetectionRisk + riskIncrease, maxDetectionRisk);
            
            // Check suspicion level
            bool wasSuspicious = _isUnderSuspicion;
            _isUnderSuspicion = _currentDetectionRisk > 25f;
            
            if (_isUnderSuspicion != wasSuspicious)
            {
                OnSuspicionStatusChanged?.Invoke(_isUnderSuspicion);
                
                if (_isUnderSuspicion && _notificationManager != null)
                {
                    _notificationManager.ShowNotification("⚠️ Imperial Intelligence is monitoring your activities", NotificationType.Warning);
                }
            }
            
            OnDetectionRiskChanged?.Invoke(_currentDetectionRisk);
        }
        
        /// <summary>
        /// Decay detection risk over time
        /// </summary>
        private void DecayDetectionRisk()
        {
            if (_currentDetectionRisk > baseDetectionRisk)
            {
                _currentDetectionRisk = Mathf.Max(_currentDetectionRisk - detectionDecayRate, baseDetectionRisk);
                OnDetectionRiskChanged?.Invoke(_currentDetectionRisk);
                
                if (enableDebugLogs)
                    Debug.Log($"[BriberyManager] Detection risk decayed to {_currentDetectionRisk:F1}%");
            }
        }
        
        /// <summary>
        /// Check if bribe is detected
        /// </summary>
        private void CheckBribeDetection(BriberyRecord record)
        {
            if (!enableDetection)
                return;
            
            // Check if detected
            if (UnityEngine.Random.value * 100f < _currentDetectionRisk)
            {
                TriggerBribeDetection(record);
            }
            
            // Check for inspection trigger
            if (_totalBribesAccepted >= inspectionTriggerThreshold)
            {
                TriggerSuspicionInspection();
            }
        }
        
        /// <summary>
        /// Trigger bribe detection
        /// </summary>
        private void TriggerBribeDetection(BriberyRecord record)
        {
            if (enableDebugLogs)
                Debug.Log($"[BriberyManager] Bribe detected: {record.Amount} credits");
            
            // Play detection sound
            if (_audioManager != null && enableBribeySounds)
            {
                _audioManager.PlaySound(detectionSound);
            }
            
            // Show notification
            if (_notificationManager != null)
            {
                _notificationManager.ShowNotification("🚨 BRIBERY DETECTED - Imperial Investigation Initiated!", NotificationType.Error);
            }
            
            // Apply penalties
            ApplyDetectionPenalties(record);
            
            OnBribeDetected?.Invoke();
        }
        
        /// <summary>
        /// Apply penalties for detected bribery
        /// </summary>
        private void ApplyDetectionPenalties(BriberyRecord record)
        {
            // Remove the bribe money
            if (_creditsManager != null)
            {
                _creditsManager.DeductCredits(record.Amount, "Bribery investigation penalty");
            }
            
            // Severe loyalty penalty
            if (_loyaltyManager != null)
            {
                _loyaltyManager.UpdateLoyalty(-5, -2, "Caught accepting bribes");
            }
            
            // Reset detection risk (investigation complete)
            _currentDetectionRisk = baseDetectionRisk;
            _isUnderSuspicion = false;
            OnDetectionRiskChanged?.Invoke(_currentDetectionRisk);
            OnSuspicionStatusChanged?.Invoke(false);
        }
        
        /// <summary>
        /// Trigger suspicion-based inspection
        /// </summary>
        private void TriggerSuspicionInspection()
        {
            if (_notificationManager != null)
            {
                _notificationManager.ShowNotification("🔍 Routine inspection scheduled due to recent activities", NotificationType.Warning);
            }
            
            // This would trigger the InspectionManager when it's created
            // For now, just increase detection risk
            IncreaseDetectionRisk(10f);
        }
        
        /// <summary>
        /// Get bribery statistics
        /// </summary>
        public BriberyStatistics GetStatistics()
        {
            return new BriberyStatistics
            {
                TotalBribesAccepted = _totalBribesAccepted,
                TotalBribesRejected = _totalBribesRejected,
                TotalBribeCredits = _totalBribeCredits,
                CurrentDetectionRisk = _currentDetectionRisk,
                IsUnderSuspicion = _isUnderSuspicion,
                ConsecutiveBribes = _consecutiveBribes,
                BriberyHistory = new List<BriberyRecord>(_briberyHistory)
            };
        }
        
        /// <summary>
        /// Reset bribery tracking (for new game)
        /// </summary>
        public void ResetBriberyTracking()
        {
            _currentOffer = null;
            _briberyHistory.Clear();
            _totalBribesAccepted = 0;
            _totalBribesRejected = 0;
            _totalBribeCredits = 0;
            _currentDetectionRisk = baseDetectionRisk;
            _consecutiveBribes = 0;
            _isUnderSuspicion = false;
            _briberyDecisions.Clear();
            
            if (enableDebugLogs)
                Debug.Log("[BriberyManager] Bribery tracking reset");
        }
        
        // Event handlers
        private void OnEncounterGenerated(EncounterData encounter)
        {
            // Generate bribe offer for some encounters
            if (encounter != null && enableBribery)
            {
                GenerateBribeOffer(encounter);
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_encounterManager != null)
            {
                EncounterManager.OnEncounterGenerated -= OnEncounterGenerated;
            }
            
            // Clear event subscriptions
            OnBribeOffered = null;
            OnBribeAccepted = null;
            OnBribeRejected = null;
            OnDetectionRiskChanged = null;
            OnBribeDetected = null;
            OnSuspicionStatusChanged = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Generate Bribe Offer")]
        private void TestGenerateBribeOffer()
        {
            var mockEncounter = new MockEncounter("Test Ship", "Test Captain");
            GenerateBribeOffer(mockEncounter);
        }
        
        [ContextMenu("Test: Accept Current Bribe")]
        private void TestAcceptBribe()
        {
            AcceptBribe();
        }
        
        [ContextMenu("Test: Reject Current Bribe")]
        private void TestRejectBribe()
        {
            RejectBribe();
        }
        
        [ContextMenu("Show Bribery Statistics")]
        private void ShowBriberyStatistics()
        {
            var stats = GetStatistics();
            Debug.Log("=== BRIBERY STATISTICS ===");
            Debug.Log($"Total Accepted: {stats.TotalBribesAccepted}");
            Debug.Log($"Total Rejected: {stats.TotalBribesRejected}");
            Debug.Log($"Total Credits: {stats.TotalBribeCredits}");
            Debug.Log($"Detection Risk: {stats.CurrentDetectionRisk:F1}%");
            Debug.Log($"Under Suspicion: {stats.IsUnderSuspicion}");
            Debug.Log($"Consecutive Bribes: {stats.ConsecutiveBribes}");
            Debug.Log("=== END STATISTICS ===");
        }
    }
    
    // Supporting data structures
    [System.Serializable]
    public class BriberyOffer
    {
        public string Id;
        public IEncounter Encounter;
        public int Amount;
        public string Reason;
        public DateTime OfferedTime;
        public float DetectionRisk;
        public BribeType BribeType;
    }
    
    [System.Serializable]
    public class BriberyRecord
    {
        public string Id;
        public IEncounter Encounter;
        public int Amount;
        public string Reason;
        public DateTime OfferedTime;
        public DateTime AcceptedTime;
        public bool WasAccepted;
        public float DetectionRisk;
        public BribeType BribeType;
    }
    
    [System.Serializable]
    public struct BriberyStatistics
    {
        public int TotalBribesAccepted;
        public int TotalBribesRejected;
        public int TotalBribeCredits;
        public float CurrentDetectionRisk;
        public bool IsUnderSuspicion;
        public int ConsecutiveBribes;
        public List<BriberyRecord> BriberyHistory;
    }
    
    public enum BribeType
    {
        Standard,
        Authority,
        Contraband,
        Humanitarian,
        Luxury
    }
    
    // Mock encounter for testing
    public class MockEncounter : IEncounter
    {
        public string ShipType { get; private set; }
        public string CaptainName { get; private set; }
        public string AccessCode { get; private set; }
        public bool IsValid { get; private set; }
        
        public MockEncounter(string shipType, string captainName)
        {
            ShipType = shipType;
            CaptainName = captainName;
            AccessCode = "TEST-1234";
            IsValid = true;
        }
    }
}