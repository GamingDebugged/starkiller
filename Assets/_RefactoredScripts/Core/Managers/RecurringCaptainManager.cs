using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages recurring captain encounters and their memory of previous interactions.
    /// Captains can appear 3-4 times maximum with at least 5 days between appearances.
    /// </summary>
    public class RecurringCaptainManager : MonoBehaviour
    {
        [Header("Recurring Captain Configuration")]
        [SerializeField] private int maxAppearancesPerCaptain = 4;
        [SerializeField] private int minDaysBetweenAppearances = 5;
        [SerializeField] private float recurringCaptainChance = 0.3f; // 30% chance for recurring captain
        
        [Header("Captain Pool")]
        [SerializeField] private List<RecurringCaptainData> captainMemory = new List<RecurringCaptainData>();
        
        // Events
        public System.Action<string, CaptainRelationship> OnCaptainRelationshipChanged;
        
        private NarrativeStateManager narrativeStateManager;
        private bool isInitialized = false;
        
        #region Initialization
        
        private void Start()
        {
            InitializeManager();
        }
        
        private void InitializeManager()
        {
            // Get references to other managers
            narrativeStateManager = ServiceLocator.Get<NarrativeStateManager>();
            
            // Register with ServiceLocator
            ServiceLocator.Register(this);
            
            isInitialized = true;
            Debug.Log("RecurringCaptainManager initialized successfully");
        }
        
        #endregion
        
        #region Captain Encounter Management
        
        /// <summary>
        /// Record a captain encounter and update their relationship data
        /// </summary>
        public void RecordCaptainEncounter(string captainId, PlayerDecision decision)
        {
            var captainData = GetOrCreateCaptainData(captainId);
            
            // Update encounter data
            captainData.encounterCount++;
            captainData.lastEncounterDay = GetCurrentDay();
            captainData.previousDecisions.Add(decision);
            captainData.lastDecision = decision;
            
            // Update relationship based on decision
            CaptainRelationship oldRelationship = captainData.currentRelationship;
            captainData.currentRelationship = CalculateRelationship(captainData.previousDecisions);
            
            Debug.Log($"Captain {captainId} encounter recorded - Decision: {decision}, " +
                     $"Relationship: {oldRelationship} → {captainData.currentRelationship}");
            
            // Update narrative state manager
            if (narrativeStateManager != null)
            {
                narrativeStateManager.UpdateCaptainRelationship(captainId, decision);
            }
            
            // Trigger event if relationship changed
            if (oldRelationship != captainData.currentRelationship)
            {
                OnCaptainRelationshipChanged?.Invoke(captainId, captainData.currentRelationship);
            }
        }
        
        /// <summary>
        /// Get or create captain data for tracking
        /// </summary>
        private RecurringCaptainData GetOrCreateCaptainData(string captainId)
        {
            var existingData = captainMemory.FirstOrDefault(c => c.captainId == captainId);
            
            if (existingData == null)
            {
                existingData = new RecurringCaptainData
                {
                    captainId = captainId,
                    currentRelationship = CaptainRelationship.FirstMeeting,
                    encounterCount = 0,
                    lastEncounterDay = 0,
                    lastDecision = PlayerDecision.None,
                    previousDecisions = new List<PlayerDecision>()
                };
                captainMemory.Add(existingData);
                Debug.Log($"Created new captain data for: {captainId}");
            }
            
            return existingData;
        }
        
        /// <summary>
        /// Calculate relationship based on previous decisions
        /// </summary>
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
                        positiveScore += 3; // Approval is highly valued
                        break;
                    case PlayerDecision.BriberyAccepted:
                        positiveScore += 4; // Bribes create strong positive relationships
                        break;
                    case PlayerDecision.Denied:
                        negativeScore += 2; // Denial creates resentment
                        break;
                    case PlayerDecision.HoldingPattern:
                        negativeScore += 1; // Delays are frustrating
                        break;
                    case PlayerDecision.TractorBeam:
                        negativeScore += 4; // Tractor beam is highly aggressive
                        break;
                }
            }
            
            // Weight recent decisions more heavily
            if (decisions.Count > 1)
            {
                var recentDecision = decisions.Last();
                switch (recentDecision)
                {
                    case PlayerDecision.Approved:
                    case PlayerDecision.BriberyAccepted:
                        positiveScore += 1;
                        break;
                    case PlayerDecision.Denied:
                    case PlayerDecision.TractorBeam:
                        negativeScore += 1;
                        break;
                }
            }
            
            // Determine relationship
            if (positiveScore > negativeScore + 2)
                return CaptainRelationship.Friendly;
            else if (negativeScore > positiveScore + 2)
                return CaptainRelationship.Hostile;
            else
                return CaptainRelationship.Neutral;
        }
        
        #endregion
        
        #region Captain Return Logic
        
        /// <summary>
        /// Check if a captain should return based on timing and story needs
        /// </summary>
        public bool ShouldCaptainReturn(string captainId, int currentDay)
        {
            var captainData = captainMemory.FirstOrDefault(c => c.captainId == captainId);
            
            if (captainData == null)
            {
                // New captain, no return logic needed
                return false;
            }
            
            // Check maximum appearances
            if (captainData.encounterCount >= maxAppearancesPerCaptain)
            {
                Debug.Log($"Captain {captainId} has reached maximum appearances ({maxAppearancesPerCaptain})");
                return false;
            }
            
            // Check minimum time between appearances
            int daysSinceLastEncounter = currentDay - captainData.lastEncounterDay;
            if (daysSinceLastEncounter < minDaysBetweenAppearances)
            {
                Debug.Log($"Captain {captainId} too recent (last: day {captainData.lastEncounterDay}, need {minDaysBetweenAppearances} days)");
                return false;
            }
            
            // Higher chance for hostile relationships (they want revenge)
            float returnChance = recurringCaptainChance;
            if (captainData.currentRelationship == CaptainRelationship.Hostile)
            {
                returnChance *= 1.5f; // 50% more likely to return
            }
            else if (captainData.currentRelationship == CaptainRelationship.Friendly)
            {
                returnChance *= 1.2f; // 20% more likely to return
            }
            
            // Adjust based on story significance
            if (HasStorySignificance(captainData))
            {
                returnChance *= 1.3f;
            }
            
            bool shouldReturn = Random.value < returnChance;
            
            if (shouldReturn)
            {
                Debug.Log($"Captain {captainId} selected for return - Relationship: {captainData.currentRelationship}, " +
                         $"Encounters: {captainData.encounterCount}, Days since last: {daysSinceLastEncounter}");
            }
            
            return shouldReturn;
        }
        
        /// <summary>
        /// Check if a captain has story significance that makes them more likely to return
        /// </summary>
        private bool HasStorySignificance(RecurringCaptainData captainData)
        {
            // Captains with extreme relationships are story significant
            if (captainData.currentRelationship != CaptainRelationship.Neutral)
                return true;
                
            // Captains involved in bribery are significant
            if (captainData.previousDecisions.Contains(PlayerDecision.BriberyAccepted))
                return true;
                
            // Captains who were treated aggressively are significant
            if (captainData.previousDecisions.Contains(PlayerDecision.TractorBeam))
                return true;
                
            return false;
        }
        
        /// <summary>
        /// Get a list of captains eligible for return encounters
        /// </summary>
        public List<string> GetEligibleReturningCaptains(int currentDay)
        {
            var eligibleCaptains = new List<string>();
            
            foreach (var captainData in captainMemory)
            {
                if (ShouldCaptainReturn(captainData.captainId, currentDay))
                {
                    eligibleCaptains.Add(captainData.captainId);
                }
            }
            
            return eligibleCaptains;
        }
        
        #endregion
        
        #region Dialog Management
        
        /// <summary>
        /// Get appropriate dialog for a returning captain
        /// </summary>
        public string GetAppropriateDialog(string captainId, DialogType dialogType)
        {
            var captainData = captainMemory.FirstOrDefault(c => c.captainId == captainId);
            
            if (captainData == null || captainData.currentRelationship == CaptainRelationship.FirstMeeting)
            {
                return GetDefaultDialog(dialogType);
            }
            
            // Return relationship-specific dialog based on last decision
            return GetRelationshipSpecificDialog(captainData, dialogType);
        }
        
        /// <summary>
        /// Get relationship-specific dialog
        /// </summary>
        private string GetRelationshipSpecificDialog(RecurringCaptainData captainData, DialogType dialogType)
        {
            string captainId = captainData.captainId;
            CaptainRelationship relationship = captainData.currentRelationship;
            PlayerDecision lastDecision = captainData.lastDecision;
            
            // Generate contextual dialog based on relationship and last interaction
            switch (dialogType)
            {
                case DialogType.Greeting:
                    return GetGreetingDialog(relationship, lastDecision, captainData.encounterCount);
                    
                case DialogType.Bribery:
                    return GetBriberyDialog(relationship, lastDecision);
                    
                case DialogType.Denial:
                    return GetDenialResponseDialog(relationship, lastDecision);
                    
                case DialogType.Approval:
                    return GetApprovalResponseDialog(relationship, lastDecision);
                    
                default:
                    return GetDefaultDialog(dialogType);
            }
        }
        
        private string GetGreetingDialog(CaptainRelationship relationship, PlayerDecision lastDecision, int encounterCount)
        {
            switch (relationship)
            {
                case CaptainRelationship.Friendly:
                    if (lastDecision == PlayerDecision.BriberyAccepted)
                        return "I knew we understood each other perfectly. *wink*";
                    else
                        return $"Good to see you again. Thanks to your help last time, my business is thriving.";
                        
                case CaptainRelationship.Hostile:
                    if (lastDecision == PlayerDecision.TractorBeam)
                        return $"My ship still bears the damage from your 'tractor beam'. I hope you're more reasonable today.";
                    else if (lastDecision == PlayerDecision.Denied)
                        return $"Those delays cost me thousands of credits. I hope you're more understanding this time.";
                    else
                        return $"I remember you. Let's hope this goes better than last time.";
                        
                case CaptainRelationship.Neutral:
                    return $"We meet again. This is my {GetOrdinalNumber(encounterCount + 1)} time requesting clearance from you.";
                    
                default:
                    return "Requesting clearance to dock.";
            }
        }
        
        private string GetBriberyDialog(CaptainRelationship relationship, PlayerDecision lastDecision)
        {
            switch (relationship)
            {
                case CaptainRelationship.Friendly:
                    if (lastDecision == PlayerDecision.BriberyAccepted)
                        return "I've brought an even better offer this time. You won't regret it.";
                    else
                        return "You've been fair to me before. Perhaps we can make this mutually beneficial?";
                        
                case CaptainRelationship.Hostile:
                    return "Look, I know we got off on the wrong foot. Let me make it worth your while.";
                    
                default:
                    return "I'm prepared to make this worth your time, if you know what I mean.";
            }
        }
        
        private string GetDenialResponseDialog(CaptainRelationship relationship, PlayerDecision lastDecision)
        {
            switch (relationship)
            {
                case CaptainRelationship.Friendly:
                    return "I... I thought we had an understanding. This is disappointing.";
                    
                case CaptainRelationship.Hostile:
                    return "Again?! You're making a powerful enemy here, checkpoint officer.";
                    
                default:
                    return "This is becoming a pattern with you. I'll remember this.";
            }
        }
        
        private string GetApprovalResponseDialog(CaptainRelationship relationship, PlayerDecision lastDecision)
        {
            switch (relationship)
            {
                case CaptainRelationship.Friendly:
                    return "You're a credit to the service. Thank you for your continued cooperation.";
                    
                case CaptainRelationship.Hostile:
                    return "Finally! Perhaps you're learning some sense after all.";
                    
                default:
                    return "Appreciated. I'll put in a good word for you with my contacts.";
            }
        }
        
        private string GetDefaultDialog(DialogType dialogType)
        {
            switch (dialogType)
            {
                case DialogType.Greeting:
                    return "Requesting clearance to dock.";
                case DialogType.Bribery:
                    return "Perhaps we can expedite this process?";
                case DialogType.Denial:
                    return "This is unacceptable!";
                case DialogType.Approval:
                    return "Thank you for your cooperation.";
                default:
                    return "Understood.";
            }
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Get current relationship with a captain
        /// </summary>
        public CaptainRelationship GetCaptainRelationship(string captainId)
        {
            var captainData = captainMemory.FirstOrDefault(c => c.captainId == captainId);
            return captainData?.currentRelationship ?? CaptainRelationship.FirstMeeting;
        }
        
        /// <summary>
        /// Get encounter count for a captain
        /// </summary>
        public int GetCaptainEncounterCount(string captainId)
        {
            var captainData = captainMemory.FirstOrDefault(c => c.captainId == captainId);
            return captainData?.encounterCount ?? 0;
        }
        
        /// <summary>
        /// Get all captain relationship data
        /// </summary>
        public List<RecurringCaptainData> GetAllCaptainData()
        {
            return new List<RecurringCaptainData>(captainMemory);
        }
        
        /// <summary>
        /// Get captains with specific relationship types
        /// </summary>
        public List<string> GetCaptainsByRelationship(CaptainRelationship relationship)
        {
            return captainMemory
                .Where(c => c.currentRelationship == relationship)
                .Select(c => c.captainId)
                .ToList();
        }
        
        /// <summary>
        /// Clear captain memory (for testing or new game)
        /// </summary>
        public void ClearCaptainMemory()
        {
            captainMemory.Clear();
            Debug.Log("Captain memory cleared");
        }
        
        #endregion
        
        #region Utility
        
        private int GetCurrentDay()
        {
            var dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
            return dayProgressionManager?.CurrentDay ?? 1;
        }
        
        private string GetOrdinalNumber(int number)
        {
            if (number <= 0) return number.ToString();
            
            switch (number % 100)
            {
                case 11:
                case 12:
                case 13:
                    return number + "th";
            }
            
            switch (number % 10)
            {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }
        }
        
        #endregion
    }
    
    #region Data Structures
    
    /// <summary>
    /// Data structure for tracking recurring captain information
    /// </summary>
    [System.Serializable]
    public class RecurringCaptainData
    {
        public string captainId;
        public CaptainRelationship currentRelationship;
        public int encounterCount;
        public int lastEncounterDay;
        public PlayerDecision lastDecision;
        public List<PlayerDecision> previousDecisions = new List<PlayerDecision>();
        
        /// <summary>
        /// Get a summary of this captain's history
        /// </summary>
        public string GetHistorySummary()
        {
            if (encounterCount == 0) return "No previous encounters";
            
            string summary = $"Encounters: {encounterCount}, Relationship: {currentRelationship}";
            
            if (previousDecisions.Count > 0)
            {
                var lastDecision = previousDecisions.Last();
                summary += $", Last: {lastDecision}";
            }
            
            return summary;
        }
    }
    
    /// <summary>
    /// Types of dialog that can be requested
    /// </summary>
    public enum DialogType
    {
        Greeting,
        Bribery,
        Denial,
        Approval,
        HoldingPattern,
        TractorBeam
    }
    
    #endregion
}