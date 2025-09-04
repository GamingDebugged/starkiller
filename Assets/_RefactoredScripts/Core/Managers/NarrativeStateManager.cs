using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages the narrative state and alignment tracking for the 30-day campaign.
    /// Tracks player's political alignment, story beats, and determines ending eligibility.
    /// </summary>
    public class NarrativeStateManager : MonoBehaviour
    {
        [Header("Alignment Tracking")]
        [SerializeField] [Range(-100, 100)] private int imperialLoyalty = 0;    // -100 to +100
        [SerializeField] [Range(-100, 100)] private int rebellionSympathy = 0;  // -100 to +100
        [SerializeField] [Range(0, 100)] private int corruptionLevel = 0;       // 0 to 100
        [SerializeField] [Range(0, 100)] private int suspicionLevel = 0;        // 0 to 100
        
        [Header("Major Story Beats")]
        [SerializeField] private List<string> completedStoryBeats = new List<string>();
        [SerializeField] private bool pointOfNoReturnReached = false;
        [SerializeField] private EndingPath lockedEndingPath = EndingPath.None;
        
        [Header("Captain Relationships")]
        [SerializeField] private List<CaptainRelationshipData> captainRelationships = new List<CaptainRelationshipData>();
        
        [Header("Ending Configuration")]
        [SerializeField] private int pointOfNoReturnStartDay = 23;
        [SerializeField] private int pointOfNoReturnEndDay = 27;
        
        // Events
        public System.Action<int, int> OnAlignmentChanged; // imperial, rebellion
        public System.Action<string> OnStoryBeatCompleted;
        public System.Action<EndingPath> OnEndingPathLocked;
        
        private GameManager gameManager;
        private FamilyPressureManager familyPressureManager;
        private ConsequenceManager consequenceManager;
        private bool isInitialized = false;
        
        #region Initialization
        
        private void Start()
        {
            InitializeManager();
        }
        
        private void InitializeManager()
        {
            // Get references to other managers
            gameManager = FindFirstObjectByType<GameManager>();
            familyPressureManager = ServiceLocator.Get<FamilyPressureManager>();
            consequenceManager = ConsequenceManager.Instance;
            
            if (gameManager == null)
            {
                Debug.LogError("NarrativeStateManager: GameManager not found!");
                return;
            }
            
            // Register with ServiceLocator
            ServiceLocator.Register(this);
            
            // Subscribe to day progression events
            DayProgressionManager.OnDayStarted += HandleDayStarted;
            
            isInitialized = true;
            Debug.Log($"NarrativeStateManager initialized - Loyalty: {imperialLoyalty}, Rebellion: {rebellionSympathy}");
        }
        
        #endregion
        
        #region Day Progression Handling
        
        private void HandleDayStarted(int currentDay)
        {
            if (!isInitialized) return;
            
            // Check for point of no return scenarios
            if (currentDay >= pointOfNoReturnStartDay && currentDay <= pointOfNoReturnEndDay && !pointOfNoReturnReached)
            {
                CheckForPointOfNoReturn(currentDay);
            }
            
            // Update suspicion level based on recent activities
            UpdateSuspicionLevel(currentDay);
        }
        
        private void CheckForPointOfNoReturn(int currentDay)
        {
            // Determine if player is eligible for point of no return scenarios
            EndingPath suggestedPath = GetSuggestedEndingPath();
            
            if (suggestedPath != EndingPath.None)
            {
                Debug.Log($"Point of No Return opportunity available: {suggestedPath} path on day {currentDay}");
                // This would trigger specific scenarios based on the suggested path
                // Implementation would depend on scenario management system
            }
        }
        
        private void UpdateSuspicionLevel(int currentDay)
        {
            // Gradually reduce suspicion if no recent incidents
            if (consequenceManager != null)
            {
                var recentIncidents = consequenceManager.GetTodaysIncidents();
                if (recentIncidents.Count == 0)
                {
                    suspicionLevel = Mathf.Max(0, suspicionLevel - 1);
                }
            }
        }
        
        #endregion
        
        #region Alignment Management
        
        /// <summary>
        /// Update imperial loyalty and rebellion sympathy
        /// </summary>
        public void UpdateAlignment(int imperialChange, int rebellionChange)
        {
            int oldImperial = imperialLoyalty;
            int oldRebellion = rebellionSympathy;
            
            imperialLoyalty = Mathf.Clamp(imperialLoyalty + imperialChange, -100, 100);
            rebellionSympathy = Mathf.Clamp(rebellionSympathy + rebellionChange, -100, 100);
            
            Debug.Log($"Alignment updated - Imperial: {oldImperial} → {imperialLoyalty} ({imperialChange:+0;-0;0}), " +
                     $"Rebellion: {oldRebellion} → {rebellionSympathy} ({rebellionChange:+0;-0;0})");
            
            OnAlignmentChanged?.Invoke(imperialLoyalty, rebellionSympathy);
        }
        
        /// <summary>
        /// Update corruption level
        /// </summary>
        public void UpdateCorruption(int change)
        {
            int oldCorruption = corruptionLevel;
            corruptionLevel = Mathf.Clamp(corruptionLevel + change, 0, 100);
            
            Debug.Log($"Corruption updated: {oldCorruption} → {corruptionLevel} ({change:+0;-0;0})");
            
            // High corruption affects suspicion
            if (corruptionLevel > 50)
            {
                UpdateSuspicion(1);
            }
        }
        
        /// <summary>
        /// Update suspicion level
        /// </summary>
        public void UpdateSuspicion(int change)
        {
            int oldSuspicion = suspicionLevel;
            suspicionLevel = Mathf.Clamp(suspicionLevel + change, 0, 100);
            
            Debug.Log($"Suspicion updated: {oldSuspicion} → {suspicionLevel} ({change:+0;-0;0})");
            
            // High suspicion triggers investigations
            if (suspicionLevel > 75 && consequenceManager != null)
            {
                // This would trigger investigation scenarios
                Debug.Log("High suspicion level may trigger investigation");
            }
        }
        
        #endregion
        
        #region Story Beat Management
        
        /// <summary>
        /// Record completion of a major story beat
        /// </summary>
        public void RecordStoryBeat(string beatId)
        {
            if (!completedStoryBeats.Contains(beatId))
            {
                completedStoryBeats.Add(beatId);
                Debug.Log($"Story beat completed: {beatId}");
                OnStoryBeatCompleted?.Invoke(beatId);
                
                // Check if this story beat affects alignment significantly
                ApplyStoryBeatEffects(beatId);
            }
        }
        
        private void ApplyStoryBeatEffects(string beatId)
        {
            // Apply narrative effects based on specific story beats
            switch (beatId)
            {
                case "THE_DEFECTOR":
                    UpdateAlignment(-2, +5); // Helping Imperial defector
                    break;
                case "THE_PURGE_ORDER":
                    UpdateAlignment(+5, -2); // Following purge orders
                    break;
                case "THE_CHILD_TRANSPORT":
                    UpdateAlignment(-1, +3); // Helping families
                    break;
                case "THE_WEAPONS_INSPECTOR":
                    UpdateAlignment(+3, -1); // Loyalty test passed
                    break;
                case "THE_SPY_EXTRACTION":
                    UpdateAlignment(-5, +8); // Major rebel assistance
                    LockEndingPath(EndingPath.Rebel);
                    break;
                case "THE_FAMILY_BETRAYAL":
                    UpdateAlignment(+8, -5); // Major imperial loyalty
                    LockEndingPath(EndingPath.Imperial);
                    break;
                default:
                    Debug.Log($"No specific effects defined for story beat: {beatId}");
                    break;
            }
        }
        
        /// <summary>
        /// Lock the ending path (point of no return)
        /// </summary>
        public void LockEndingPath(EndingPath path)
        {
            if (!pointOfNoReturnReached)
            {
                lockedEndingPath = path;
                pointOfNoReturnReached = true;
                
                Debug.Log($"Ending path locked: {path}");
                OnEndingPathLocked?.Invoke(path);
                
                // Trigger appropriate consequences
                TriggerEndingPathConsequences(path);
            }
        }
        
        private void TriggerEndingPathConsequences(EndingPath path)
        {
            if (consequenceManager == null) return;
            
            // Create consequence tokens for the locked path
            var consequenceData = new ConsequenceData();
            
            switch (path)
            {
                case EndingPath.Rebel:
                    consequenceData.newsHeadline = "Security Alert: Increased Rebel Activity Detected";
                    consequenceData.loyaltyImpact = -3;
                    consequenceData.suspicionIncrease = 10;
                    consequenceData.affectsFamily = true;
                    break;
                    
                case EndingPath.Imperial:
                    consequenceData.newsHeadline = "Command Commends Loyalty: Exemplary Service Recognized";
                    consequenceData.loyaltyImpact = +3;
                    consequenceData.suspicionIncrease = 0;
                    consequenceData.affectsFamily = false;
                    break;
                    
                case EndingPath.Neutral:
                    consequenceData.newsHeadline = "Personnel Review: Standard Performance Evaluation";
                    consequenceData.loyaltyImpact = 0;
                    consequenceData.suspicionIncrease = 2;
                    consequenceData.affectsFamily = false;
                    break;
                    
                case EndingPath.Corrupt:
                    consequenceData.newsHeadline = "Internal Affairs: Random Audit Procedures Implemented";
                    consequenceData.loyaltyImpact = -1;
                    consequenceData.suspicionIncrease = 15;
                    consequenceData.affectsFamily = true;
                    break;
            }
            
            consequenceManager.AddConsequenceToken($"ENDING_PATH_{path}", 2, consequenceData);
        }
        
        #endregion
        
        #region Captain Relationship Management
        
        /// <summary>
        /// Update relationship with a specific captain
        /// </summary>
        public void UpdateCaptainRelationship(string captainId, PlayerDecision decision)
        {
            var relationship = captainRelationships.FirstOrDefault(r => r.captainId == captainId);
            
            if (relationship == null)
            {
                relationship = new CaptainRelationshipData
                {
                    captainId = captainId,
                    relationship = CaptainRelationship.FirstMeeting,
                    encounterCount = 0,
                    lastDecision = PlayerDecision.None,
                    previousDecisions = new List<PlayerDecision>()
                };
                captainRelationships.Add(relationship);
            }
            
            // Update relationship data
            relationship.encounterCount++;
            relationship.previousDecisions.Add(decision);
            relationship.lastDecision = decision;
            
            // Determine new relationship state
            relationship.relationship = CalculateRelationship(relationship.previousDecisions);
            
            Debug.Log($"Captain {captainId} relationship updated to {relationship.relationship} " +
                     $"(encounters: {relationship.encounterCount}, last: {decision})");
        }
        
        private CaptainRelationship CalculateRelationship(List<PlayerDecision> decisions)
        {
            if (decisions.Count == 0) return CaptainRelationship.FirstMeeting;
            
            int positiveScore = 0;
            int negativeScore = 0;
            
            foreach (var decision in decisions)
            {
                switch (decision)
                {
                    case PlayerDecision.Approved:
                    case PlayerDecision.BriberyAccepted:
                        positiveScore += 2;
                        break;
                    case PlayerDecision.Denied:
                        negativeScore += 2;
                        break;
                    case PlayerDecision.HoldingPattern:
                        negativeScore += 1;
                        break;
                    case PlayerDecision.TractorBeam:
                        negativeScore += 3;
                        break;
                }
            }
            
            if (positiveScore > negativeScore + 1)
                return CaptainRelationship.Friendly;
            else if (negativeScore > positiveScore + 1)
                return CaptainRelationship.Hostile;
            else
                return CaptainRelationship.Neutral;
        }
        
        /// <summary>
        /// Get relationship data for a specific captain
        /// </summary>
        public CaptainRelationshipData GetCaptainRelationship(string captainId)
        {
            return captainRelationships.FirstOrDefault(r => r.captainId == captainId);
        }
        
        #endregion
        
        #region Ending Determination
        
        /// <summary>
        /// Determine the appropriate ending based on current narrative state
        /// </summary>
        public EndingType DetermineEnding()
        {
            // Primary factor: Point of No Return choice
            if (pointOfNoReturnReached && lockedEndingPath != EndingPath.None)
            {
                return GetEndingFromLockedPath();
            }
            
            // Secondary factors: Loyalty, corruption, family status
            float alignmentScore = (imperialLoyalty - rebellionSympathy) / 200f; // -1 to 1
            float corruptionScore = corruptionLevel / 100f; // 0 to 1
            float familyScore = GetFamilyStatusScore(); // 0 to 1
            
            return CalculateEndingFromScores(alignmentScore, corruptionScore, familyScore);
        }
        
        private EndingType GetEndingFromLockedPath()
        {
            switch (lockedEndingPath)
            {
                case EndingPath.Rebel:
                    // Choose between rebel endings based on family status
                    return GetFamilyStatusScore() > 0.7f ? EndingType.FreedomFighter : EndingType.Martyr;
                    
                case EndingPath.Imperial:
                    // Choose between imperial endings based on corruption
                    return corruptionLevel < 30 ? EndingType.ImperialHero : EndingType.BridgeCommander;
                    
                case EndingPath.Neutral:
                    // Choose between neutral endings based on suspicion
                    return suspicionLevel < 50 ? EndingType.GrayMan : EndingType.Compromised;
                    
                case EndingPath.Corrupt:
                    // High corruption path
                    return EndingType.Compromised;
                    
                default:
                    return EndingType.GrayMan;
            }
        }
        
        private EndingType CalculateEndingFromScores(float alignmentScore, float corruptionScore, float familyScore)
        {
            // Complex decision tree based on all factors
            
            // High corruption overrides other factors
            if (corruptionScore > 0.7f)
            {
                return EndingType.Compromised;
            }
            
            // Strong rebel sympathy
            if (alignmentScore < -0.6f)
            {
                if (familyScore > 0.8f)
                    return EndingType.FreedomFighter;
                else if (familyScore > 0.4f)
                    return EndingType.Underground;
                else
                    return EndingType.Refugee;
            }
            
            // Strong imperial loyalty
            if (alignmentScore > 0.6f)
            {
                if (corruptionScore < 0.2f && familyScore > 0.6f)
                    return EndingType.GoodSoldier;
                else if (corruptionScore < 0.3f)
                    return EndingType.TrueBeliever;
                else
                    return EndingType.BridgeCommander;
            }
            
            // Moderate rebel sympathy
            if (alignmentScore < -0.2f)
            {
                return familyScore > 0.5f ? EndingType.Refugee : EndingType.Underground;
            }
            
            // Moderate imperial loyalty
            if (alignmentScore > 0.2f)
            {
                return familyScore > 0.5f ? EndingType.GoodSoldier : EndingType.TrueBeliever;
            }
            
            // Neutral/balanced
            return suspicionLevel > 50 ? EndingType.Compromised : EndingType.GrayMan;
        }
        
        private float GetFamilyStatusScore()
        {
            if (familyPressureManager == null) return 0.5f;
            
            float happiness = familyPressureManager.GetFamilyHappiness() / 100f;
            float safety = familyPressureManager.GetFamilySafety() / 100f;
            
            return (happiness + safety) / 2f;
        }
        
        /// <summary>
        /// Check if player is eligible for a specific ending
        /// </summary>
        public bool IsEligibleForEnding(EndingType ending)
        {
            switch (ending)
            {
                case EndingType.FreedomFighter:
                    return rebellionSympathy > 60 && GetFamilyStatusScore() > 0.7f && suspicionLevel < 50;
                    
                case EndingType.Martyr:
                    return rebellionSympathy > 70 && GetFamilyStatusScore() < 0.5f;
                    
                case EndingType.ImperialHero:
                    return imperialLoyalty > 70 && corruptionLevel < 20 && suspicionLevel < 30;
                    
                case EndingType.BridgeCommander:
                    return imperialLoyalty > 60 && corruptionLevel > 40;
                    
                case EndingType.Compromised:
                    return corruptionLevel > 60 || suspicionLevel > 70;
                    
                default:
                    return true; // Most endings are generally accessible
            }
        }
        
        /// <summary>
        /// Get suggested ending path based on current alignment
        /// </summary>
        public EndingPath GetSuggestedEndingPath()
        {
            float alignmentScore = (imperialLoyalty - rebellionSympathy) / 200f;
            
            if (corruptionLevel > 60)
                return EndingPath.Corrupt;
            else if (alignmentScore < -0.5f)
                return EndingPath.Rebel;
            else if (alignmentScore > 0.5f)
                return EndingPath.Imperial;
            else
                return EndingPath.Neutral;
        }
        
        #endregion
        
        #region Public API
        
        public int GetImperialLoyalty() => imperialLoyalty;
        public int GetRebellionSympathy() => rebellionSympathy;
        public int GetCorruptionLevel() => corruptionLevel;
        public int GetSuspicionLevel() => suspicionLevel;
        public bool IsPointOfNoReturnReached() => pointOfNoReturnReached;
        public EndingPath GetLockedEndingPath() => lockedEndingPath;
        public List<string> GetCompletedStoryBeats() => new List<string>(completedStoryBeats);
        
        /// <summary>
        /// Get narrative summary for display in PersonalDataLog
        /// </summary>
        public string GetNarrativeSummary()
        {
            string summary = "<b>Personal Status Review</b>\n\n";
            
            // Alignment status
            if (imperialLoyalty > rebellionSympathy + 20)
                summary += "Loyalty Assessment: <color=blue>Imperial Aligned</color>\n";
            else if (rebellionSympathy > imperialLoyalty + 20)
                summary += "Loyalty Assessment: <color=red>Questionable Allegiance</color>\n";
            else
                summary += "Loyalty Assessment: <color=yellow>Under Review</color>\n";
            
            // Corruption status
            if (corruptionLevel > 50)
                summary += "Conduct Review: <color=orange>Irregularities Noted</color>\n";
            else if (corruptionLevel > 20)
                summary += "Conduct Review: <color=yellow>Minor Concerns</color>\n";
            else
                summary += "Conduct Review: <color=green>Exemplary Service</color>\n";
            
            // Family status
            float familyScore = GetFamilyStatusScore();
            if (familyScore > 0.7f)
                summary += "Family Status: <color=green>Stable</color>\n";
            else if (familyScore > 0.3f)
                summary += "Family Status: <color=yellow>Manageable Stress</color>\n";
            else
                summary += "Family Status: <color=red>Critical Concerns</color>\n";
            
            // Point of no return
            if (pointOfNoReturnReached)
            {
                summary += $"\n<b>Career Path: {lockedEndingPath} Track</b>\n";
            }
            
            return summary;
        }
        
        #endregion
        
        #region Utility
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            DayProgressionManager.OnDayStarted -= HandleDayStarted;
        }
        
        #endregion
    }
    
    #region Data Structures
    
    /// <summary>
    /// Tracks relationship data with a specific captain
    /// </summary>
    [System.Serializable]
    public class CaptainRelationshipData
    {
        public string captainId;
        public CaptainRelationship relationship;
        public int encounterCount;
        public PlayerDecision lastDecision;
        public List<PlayerDecision> previousDecisions = new List<PlayerDecision>();
    }
    
    /// <summary>
    /// Ending paths that can be locked during point of no return
    /// </summary>
    public enum EndingPath
    {
        None,
        Rebel,      // Committed to rebellion
        Imperial,   // Committed to Empire
        Neutral,    // Avoiding both sides
        Corrupt     // Focus on personal gain
    }
    
    /// <summary>
    /// Final ending types (10 total as designed)
    /// </summary>
    public enum EndingType
    {
        // Rebel Endings (Far Left)
        FreedomFighter,     // Successfully escape with family to rebel base
        Martyr,             // Family escapes while you stay behind
        
        // Survivor Endings (Left-Center)
        Refugee,            // Flee to frontier with family
        Underground,        // Remain at post while secretly helping resistance
        
        // Neutral Endings (Center)
        GrayMan,            // Keep head down, avoid all sides
        Compromised,        // Under investigation but not yet caught
        
        // Imperial Endings (Right-Center)
        GoodSoldier,        // Promoted for loyalty, family safe
        TrueBeliever,       // Honored by Empire, family relationships strained
        
        // Zealot Endings (Far Right)
        BridgeCommander,    // Ultimate promotion by sacrificing everything
        ImperialHero        // Public commendation, family "relocated for protection"
    }
    
    #endregion
}