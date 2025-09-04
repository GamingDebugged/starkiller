using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Starkiller.Core.ScriptableObjects;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages game ending determination and presentation for the 30-day campaign
    /// Analyzes player journey to determine one of 10 possible endings
    /// </summary>
    public class EndGameManager : MonoBehaviour
    {
        [Header("Ending Database")]
        [SerializeField] private List<EndingDataSO> endingDatabase = new List<EndingDataSO>();
        [SerializeField] private List<ScenarioConsequenceSO> scenarioConsequences = new List<ScenarioConsequenceSO>();
        
        [Header("Ending Determination Settings")]
        [SerializeField] private float rebelAlignmentThreshold = -0.5f;
        [SerializeField] private float imperialAlignmentThreshold = 0.5f;
        [SerializeField] private float familySurvivalThreshold = 50f;
        [SerializeField] private PerformanceRating highPerformanceThreshold = PerformanceRating.Good;
        
        [Header("Achievement Collection")]
        [SerializeField] private int maxAchievementsToDisplay = 12;
        [SerializeField] private bool showAllKeyScenarios = true;
        [SerializeField] private bool prioritizeKeyAchievements = true;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showEndingFactors = false;
        
        // Manager References
        private NarrativeStateManager narrativeState;
        private FamilyPressureManager familyPressure;
        private PerformanceManager performance;
        private LoyaltyManager loyalty;
        private DecisionTracker decisionTracker;
        private UICoordinator uiCoordinator;
        
        // Current ending data
        private EndingDataSO currentEnding;
        private List<AchievementData> collectedAchievements = new List<AchievementData>();
        
        // Events
        public System.Action<EndingType> OnEndingDetermined;
        public System.Action<EndingDataSO> OnEndingPresented;
        
        private bool isInitialized = false;
        
        #region Initialization
        
        private void Start()
        {
            InitializeManager();
        }
        
        private void InitializeManager()
        {
            // Get manager references via ServiceLocator
            narrativeState = ServiceLocator.Get<NarrativeStateManager>();
            familyPressure = ServiceLocator.Get<FamilyPressureManager>();
            performance = ServiceLocator.Get<PerformanceManager>();
            loyalty = ServiceLocator.Get<LoyaltyManager>();
            decisionTracker = ServiceLocator.Get<DecisionTracker>();
            uiCoordinator = ServiceLocator.Get<UICoordinator>();
            
            if (ValidateReferences())
            {
                // Register with ServiceLocator
                ServiceLocator.Register(this);
                
                // Subscribe to game completion event
                // Note: This would need to be implemented in DayProgressionManager or GameStateManager
                // GameEvents.OnDay30Complete += HandleGameCompletion;
                
                isInitialized = true;
                if (enableDebugLogs) Debug.Log("EndGameManager initialized successfully");
            }
        }
        
        private bool ValidateReferences()
        {
            bool valid = true;
            
            if (narrativeState == null)
            {
                Debug.LogError("EndGameManager: NarrativeStateManager not found!");
                valid = false;
            }
            
            if (familyPressure == null)
            {
                Debug.LogError("EndGameManager: FamilyPressureManager not found!");
                valid = false;
            }
            
            if (performance == null)
            {
                Debug.LogError("EndGameManager: PerformanceManager not found!");
                valid = false;
            }
            
            if (loyalty == null)
            {
                Debug.LogError("EndGameManager: LoyaltyManager not found!");
                valid = false;
            }
            
            if (endingDatabase.Count == 0)
            {
                Debug.LogWarning("EndGameManager: No ending data configured!");
            }
            
            return valid;
        }
        
        #endregion
        
        #region Ending Determination
        
        /// <summary>
        /// Determine which ending the player gets based on their 30-day journey
        /// </summary>
        public EndingType DetermineEnding()
        {
            if (!isInitialized) return EndingType.GrayMan;
            
            // Primary factor: Point of No Return choice (Days 23-27)
            if (narrativeState.IsPointOfNoReturnReached())
            {
                var lockedEnding = GetLockedPathEnding();
                if (enableDebugLogs) Debug.Log($"Using locked ending path: {lockedEnding}");
                return lockedEnding;
            }
            
            // Secondary factors for non-locked paths
            var calculatedEnding = CalculateEndingFromFactors();
            if (enableDebugLogs) Debug.Log($"Calculated ending from factors: {calculatedEnding}");
            
            return calculatedEnding;
        }
        
        private EndingType GetLockedPathEnding()
        {
            var lockedPath = narrativeState.GetLockedEndingPath();
            float alignment = GetAlignmentScore(); // -1 to +1 scale
            bool familySurvived = familyPressure.GetFamilySafety() > familySurvivalThreshold;
            bool highPerformance = performance.CurrentRating >= highPerformanceThreshold;
            
            switch (lockedPath)
            {
                case EndingPath.Rebel:
                    if (familySurvived && highPerformance) return EndingType.FreedomFighter;
                    if (!familySurvived) return EndingType.Martyr;
                    if (familySurvived && !highPerformance) return EndingType.Refugee;
                    return EndingType.Underground;
                    
                case EndingPath.Imperial:
                    if (!familySurvived) return EndingType.GoodSoldier;
                    if (highPerformance && alignment > 0.8f) return EndingType.ImperialHero;
                    if (highPerformance) return EndingType.BridgeCommander;
                    return EndingType.TrueBeliever;
                    
                case EndingPath.Neutral:
                    return familySurvived ? EndingType.GrayMan : EndingType.Compromised;
                    
                case EndingPath.Corrupt:
                    return EndingType.Compromised;
                    
                default:
                    return EndingType.GrayMan;
            }
        }
        
        private EndingType CalculateEndingFromFactors()
        {
            float alignment = GetAlignmentScore(); // -1 to +1
            bool familySurvived = familyPressure.GetFamilySafety() > familySurvivalThreshold;
            bool highPerformance = performance.CurrentRating >= highPerformanceThreshold;
            bool isExtremistImperial = loyalty.IsExtremistImperial;
            
            if (showEndingFactors)
            {
                Debug.Log($"Ending factors - Alignment: {alignment}, Family: {familySurvived}, Performance: {highPerformance}");
            }
            
            if (alignment < rebelAlignmentThreshold) // Rebel Path
            {
                if (familySurvived && highPerformance) return EndingType.FreedomFighter;
                if (!familySurvived) return EndingType.Martyr;
                if (familySurvived && !highPerformance) return EndingType.Refugee;
                return EndingType.Underground;
            }
            else if (alignment > imperialAlignmentThreshold) // Imperial Path
            {
                if (!familySurvived) return EndingType.GoodSoldier;
                if (highPerformance && isExtremistImperial) return EndingType.ImperialHero;
                if (highPerformance) return EndingType.BridgeCommander;
                return EndingType.TrueBeliever;
            }
            else // Neutral Path
            {
                if (familySurvived) return EndingType.GrayMan;
                return EndingType.Compromised;
            }
        }
        
        /// <summary>
        /// Convert loyalty values to -1 to +1 alignment score
        /// </summary>
        private float GetAlignmentScore()
        {
            if (loyalty == null) return 0f;
            
            int imperial = loyalty.ImperialLoyalty;
            int rebellion = loyalty.RebellionSympathy;
            
            // Convert from -10/+10 to -1/+1 scale
            // Imperial positive = positive alignment, Rebellion positive = negative alignment
            float normalizedImperial = imperial / 10f;
            float normalizedRebellion = rebellion / 10f;
            
            // Net alignment: positive = imperial, negative = rebellion
            return normalizedImperial - normalizedRebellion;
        }
        
        #endregion
        
        #region Achievement Collection
        
        /// <summary>
        /// Collect achievements from scenario decisions and consequence history
        /// </summary>
        private void CollectAchievements()
        {
            collectedAchievements.Clear();
            
            // 1. Collect achievements from authored scenario consequences (if available)
            CollectScenarioAchievements();
            
            // 2. Collect achievements from ConsequenceManager history
            CollectConsequenceAchievements();
            
            // 3. Collect achievements from DecisionTracker performance
            CollectPerformanceAchievements();
            
            // Sort achievements by priority and key status
            SortAchievements();
        }
        
        /// <summary>
        /// Collect achievements from authored scenario ScriptableObjects
        /// </summary>
        private void CollectScenarioAchievements()
        {
            if (decisionTracker == null) return;
            
            // This would work if GetScenarioDecisions() method exists in DecisionTracker
            // For now, we'll rely on the ConsequenceManager integration
            
            foreach (var consequence in scenarioConsequences)
            {
                if (consequence != null && consequence.IsValid())
                {
                    // Check if this scenario was encountered
                    // Implementation depends on how DecisionTracker stores scenario data
                    // For now, assume all configured scenarios contribute potential achievements
                }
            }
        }
        
        /// <summary>
        /// Collect achievements from ConsequenceManager incident history
        /// </summary>
        private void CollectConsequenceAchievements()
        {
            var consequenceManager = ServiceLocator.Get<ConsequenceManager>();
            if (consequenceManager != null)
            {
                // Use the bridge extension to convert consequence history to achievements
                var consequenceAchievements = consequenceManager.GetEndGameAchievements();
                collectedAchievements.AddRange(consequenceAchievements);
                
                if (enableDebugLogs)
                    Debug.Log($"Collected {consequenceAchievements.Count} achievements from ConsequenceManager");
            }
        }
        
        /// <summary>
        /// Collect achievements based on overall performance metrics
        /// </summary>
        private void CollectPerformanceAchievements()
        {
            if (performance == null || loyalty == null) return;
            
            // Performance-based achievements
            var rating = performance.CurrentRating;
            if (rating >= PerformanceRating.Excellent)
            {
                collectedAchievements.Add(new AchievementData
                {
                    Text = "Maintained excellent performance throughout 30-day assignment",
                    Category = AchievementCategory.Positive,
                    Priority = 85,
                    IsKeyAchievement = true,
                    ScenarioName = "Excellent_Performance"
                });
            }
            else if (rating <= PerformanceRating.Poor)
            {
                collectedAchievements.Add(new AchievementData
                {
                    Text = "Struggled with checkpoint duties and accuracy",
                    Category = AchievementCategory.Negative,
                    Priority = 80,
                    IsKeyAchievement = true,
                    ScenarioName = "Poor_Performance"
                });
            }
            
            // Loyalty-based achievements
            int imperial = loyalty.ImperialLoyalty;
            int rebellion = loyalty.RebellionSympathy;
            
            if (imperial >= 8)
            {
                collectedAchievements.Add(new AchievementData
                {
                    Text = "Demonstrated unwavering loyalty to the Empire",
                    Category = AchievementCategory.Imperial,
                    Priority = 90,
                    IsKeyAchievement = true,
                    ScenarioName = "Imperial_Loyalty"
                });
            }
            else if (rebellion >= 8)
            {
                collectedAchievements.Add(new AchievementData
                {
                    Text = "Developed strong sympathy for the rebellion",
                    Category = AchievementCategory.Rebellion,
                    Priority = 90,
                    IsKeyAchievement = true,
                    ScenarioName = "Rebellion_Sympathy"
                });
            }
            else if (Mathf.Abs(imperial - rebellion) <= 2)
            {
                collectedAchievements.Add(new AchievementData
                {
                    Text = "Maintained neutrality between competing factions",
                    Category = AchievementCategory.Neutral,
                    Priority = 75,
                    IsKeyAchievement = false,
                    ScenarioName = "Political_Neutrality"
                });
            }
            
            // Decision tracking achievements
            if (decisionTracker != null)
            {
                float accuracy = decisionTracker.AccuracyPercentage;
                if (accuracy >= 95f)
                {
                    collectedAchievements.Add(new AchievementData
                    {
                        Text = $"Achieved {accuracy:F1}% decision accuracy",
                        Category = AchievementCategory.Positive,
                        Priority = 85,
                        IsKeyAchievement = true,
                        ScenarioName = "High_Accuracy"
                    });
                }
                else if (accuracy <= 60f)
                {
                    collectedAchievements.Add(new AchievementData
                    {
                        Text = $"Decision accuracy below standards ({accuracy:F1}%)",
                        Category = AchievementCategory.Negative,
                        Priority = 80,
                        IsKeyAchievement = true,
                        ScenarioName = "Low_Accuracy"
                    });
                }
                
                if (decisionTracker.CurrentStrikes >= decisionTracker.MaxStrikes)
                {
                    collectedAchievements.Add(new AchievementData
                    {
                        Text = "Accumulated maximum disciplinary strikes",
                        Category = AchievementCategory.Negative,
                        Priority = 95,
                        IsKeyAchievement = true,
                        ScenarioName = "Maximum_Strikes"
                    });
                }
            }
        }
        
        private void SortAchievements()
        {
            collectedAchievements = collectedAchievements
                .OrderByDescending(a => a.IsKeyAchievement)
                .ThenByDescending(a => a.Priority)
                .Take(maxAchievementsToDisplay)
                .ToList();
        }
        
        /// <summary>
        /// Get achievements organized by category for display
        /// </summary>
        public Dictionary<AchievementCategory, List<string>> GetOrganizedAchievements()
        {
            var organized = new Dictionary<AchievementCategory, List<string>>();
            
            foreach (var achievement in collectedAchievements)
            {
                if (!organized.ContainsKey(achievement.Category))
                {
                    organized[achievement.Category] = new List<string>();
                }
                organized[achievement.Category].Add(achievement.Text);
            }
            
            return organized;
        }
        
        #endregion
        
        #region Ending Presentation
        
        /// <summary>
        /// Present the determined ending to the player
        /// </summary>
        public void PresentEnding(EndingType endingType)
        {
            currentEnding = endingDatabase.Find(e => e.EndingType == endingType);
            
            if (currentEnding == null)
            {
                Debug.LogError($"EndGameManager: No ending data found for {endingType}");
                return;
            }
            
            // Collect achievements before presentation
            CollectAchievements();
            
            // Trigger ending presentation
            OnEndingPresented?.Invoke(currentEnding);
            
            // Interface with UI system to display ending screen
            if (uiCoordinator != null)
            {
                DisplayEndingScreen();
            }
            
            if (enableDebugLogs) Debug.Log($"Presenting ending: {endingType}");
        }
        
        private void DisplayEndingScreen()
        {
            // This would interface with your UI system
            // Example implementation:
            var endingUIData = new EndingUIData
            {
                Title = currentEnding.Title,
                Subtitle = currentEnding.Subtitle,
                NarrativeText = currentEnding.MainNarrative,
                Achievements = GetOrganizedAchievements(),
                ThemeColor = currentEnding.ThemeColor,
                BackgroundImage = currentEnding.EndingImage,
                Music = currentEnding.EndingMusic,
                Duration = currentEnding.DisplayDuration
            };
            
            // uiCoordinator.ShowEndingScreen(endingUIData);
        }
        
        /// <summary>
        /// Handle game completion (called when day 30 ends)
        /// </summary>
        public void HandleGameCompletion()
        {
            if (!isInitialized) return;
            
            var endingType = DetermineEnding();
            OnEndingDetermined?.Invoke(endingType);
            PresentEnding(endingType);
        }
        
        #endregion
        
        #region Debug Methods
        
        /// <summary>
        /// Force a specific ending for testing
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void DEBUG_ForceEnding(EndingType endingType)
        {
            if (enableDebugLogs) Debug.Log($"DEBUG: Forcing ending {endingType}");
            PresentEnding(endingType);
        }
        
        /// <summary>
        /// Skip to day 30 with preset conditions
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void DEBUG_SkipToEndingWithConditions(float alignment, bool familyAlive, bool highPerf)
        {
            // This would need implementation with actual manager interfaces
            Debug.Log($"DEBUG: Setting conditions - Alignment: {alignment}, Family: {familyAlive}, Performance: {highPerf}");
            // Set conditions then call HandleGameCompletion()
        }
        
        /// <summary>
        /// Display current ending determination factors
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void DEBUG_ShowEndingFactors()
        {
            if (!isInitialized) return;
            
            Debug.Log("=== ENDING FACTORS ===");
            Debug.Log($"Point of No Return: {narrativeState?.IsPointOfNoReturnReached()}");
            Debug.Log($"Locked Path: {narrativeState?.GetLockedEndingPath()}");
            Debug.Log($"Alignment: {GetAlignmentScore()}");
            Debug.Log($"Family Safety: {familyPressure?.GetFamilySafety()}");
            Debug.Log($"Performance: {performance?.CurrentRating}");
            Debug.Log($"Would get ending: {DetermineEnding()}");
        }
        
        /// <summary>
        /// Test each of the 10 endings individually
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void DEBUG_TestAllEndings()
        {
            var allEndingTypes = System.Enum.GetValues(typeof(EndingType))
                .Cast<EndingType>();
            
            foreach (var ending in allEndingTypes)
            {
                Debug.Log($"Testing ending: {ending}");
                PresentEnding(ending);
                break; // Only test one at a time in editor
            }
        }
        
        #endregion
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            // GameEvents.OnDay30Complete -= HandleGameCompletion;
        }
    }
    
    /// <summary>
    /// Data structure for ending UI presentation
    /// </summary>
    public class EndingUIData
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string NarrativeText { get; set; }
        public Dictionary<AchievementCategory, List<string>> Achievements { get; set; }
        public Color ThemeColor { get; set; }
        public Sprite BackgroundImage { get; set; }
        public AudioClip Music { get; set; }
        public float Duration { get; set; }
    }
    
    /// <summary>
    /// Scenario decision data structure (should match DecisionTracker implementation)
    /// </summary>
    public class ScenarioDecision
    {
        public string ScenarioId { get; set; }
        public string DecisionId { get; set; }
        public int Day { get; set; }
        public string DecisionText { get; set; }
    }
}