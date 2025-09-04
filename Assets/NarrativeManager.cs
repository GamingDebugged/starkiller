using UnityEngine;
using StarkillerBaseCommand.Narrative;
using System.Linq;

namespace StarkillerBaseCommand
{
    public class NarrativeManager : MonoBehaviour
    {
        private static NarrativeManager instance;
        public static NarrativeManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindFirstObjectByType<NarrativeManager>();
                return instance;
            }
        }

        [Header("Narrative Tracker Reference")]
        [SerializeField] private StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker narrativeTracker;
        
        [Header("Decision Template Manager")]
        [SerializeField] private DecisionTemplateManager templateManager;
        
        [Header("Enhanced Decision Tracking")]
        [SerializeField] private bool useTemplatesWhenAvailable = true;
        [SerializeField] private bool trackDecisionChains = true;
        
        // Current decision chain tracking
        private string currentDecisionChainId = null;
        private int decisionChainLength = 0;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            this.SafeDontDestroyOnLoad();
        }

        void Start()
        {
            // Find components if not assigned
            if (narrativeTracker == null)
            {
                // Try to find the ScriptableObject instance
                narrativeTracker = Resources.FindObjectsOfTypeAll<StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker>().FirstOrDefault();
            }
                
            if (templateManager == null)
                templateManager = DecisionTemplateManager.Instance;
            
            // Subscribe to narrative events
            if (narrativeTracker != null)
            {
                narrativeTracker.OnNarrativeBranchChanged += HandleBranchChange;
                narrativeTracker.OnKeyDecisionMade += HandleKeyDecision;
                narrativeTracker.OnStoryTagUnlocked += HandleStoryTagUnlocked;
            }
        }

        // Original method for backwards compatibility
        public void RecordDecision(string decisionId, int imperialPoints, int insurgentPoints, string context)
        {
            // Try to determine category and pressure from context
            StarkillerBaseCommand.Narrative.DecisionCategory category = DetermineCategory(decisionId, context);
            StarkillerBaseCommand.Narrative.DecisionPressure pressure = DeterminePressure(imperialPoints, insurgentPoints);
            
            RecordEnhancedDecision(decisionId, imperialPoints, insurgentPoints, context, category, pressure);
        }
        
        // New enhanced method
        public void RecordEnhancedDecision(
            string decisionId, 
            int imperialPoints, 
            int insurgentPoints, 
            string context,
            StarkillerBaseCommand.Narrative.DecisionCategory category,
            StarkillerBaseCommand.Narrative.DecisionPressure pressure)
        {
            if (narrativeTracker != null)
            {
                // Track decision chains
                string parentId = trackDecisionChains ? currentDecisionChainId : null;
                
                // Use the original RecordDecision method from the ScriptableObject
                narrativeTracker.RecordDecision(
                    decisionId, 
                    imperialPoints, 
                    insurgentPoints, 
                    context
                );
                
                // Update chain tracking
                if (pressure >= StarkillerBaseCommand.Narrative.DecisionPressure.High)
                {
                    // High pressure decisions start new chains
                    currentDecisionChainId = decisionId;
                    decisionChainLength = 1;
                }
                else if (currentDecisionChainId != null)
                {
                    // Continue current chain
                    decisionChainLength++;
                    
                    // End chain after 5 decisions
                    if (decisionChainLength >= 5)
                    {
                        currentDecisionChainId = null;
                        decisionChainLength = 0;
                    }
                }
            }
        }
        
        // Specific decision type handlers with enhanced context
        public void RecordShipDecision(MasterShipEncounter encounter, bool approved)
        {
            string decisionId = $"ship_{encounter.shipType}_{(approved ? "approved" : "denied")}";
            string context = $"{(approved ? "Approved" : "Denied")} {encounter.shipType} - Captain: {encounter.captainName}";
            
            // Determine category based on ship type
            StarkillerBaseCommand.Narrative.DecisionCategory category = StarkillerBaseCommand.Narrative.DecisionCategory.Tactical;
            if (encounter.offersBribe)
                category = StarkillerBaseCommand.Narrative.DecisionCategory.Financial;
            else if (encounter.isStoryShip)
                category = StarkillerBaseCommand.Narrative.DecisionCategory.Political;
            
            // Determine pressure based on encounter properties
            StarkillerBaseCommand.Narrative.DecisionPressure pressure = StarkillerBaseCommand.Narrative.DecisionPressure.Medium;
            if (encounter.isStoryShip || encounter.creditPenalty > 20)
                pressure = StarkillerBaseCommand.Narrative.DecisionPressure.High;
            if (encounter.casualtiesIfWrong > 0)
                pressure = StarkillerBaseCommand.Narrative.DecisionPressure.Critical;
            
            // Calculate points
            int imperialPoints = 0;
            int insurgentPoints = 0;
            
            if (encounter.isStoryShip)
            {
                if (encounter.storyTag == "imperium")
                {
                    imperialPoints = approved ? 10 : -5;
                    insurgentPoints = approved ? -5 : 5;
                }
                else if (encounter.storyTag == "insurgent")
                {
                    imperialPoints = approved ? -5 : 10;
                    insurgentPoints = approved ? 10 : -5;
                }
            }
            
            RecordEnhancedDecision(decisionId, imperialPoints, insurgentPoints, context, category, pressure);
        }
        
        // Helper methods
        private StarkillerBaseCommand.Narrative.DecisionCategory DetermineCategory(string decisionId, string context)
        {
            string lowerContext = context.ToLower();
            string lowerId = decisionId.ToLower();
            
            if (lowerId.Contains("bribe") || lowerContext.Contains("credits"))
                return StarkillerBaseCommand.Narrative.DecisionCategory.Financial;
            if (lowerId.Contains("moral") || lowerContext.Contains("family"))
                return StarkillerBaseCommand.Narrative.DecisionCategory.Moral;
            if (lowerId.Contains("imperium") || lowerId.Contains("insurgent") || lowerId.Contains("rebel"))
                return StarkillerBaseCommand.Narrative.DecisionCategory.Political;
            
            return StarkillerBaseCommand.Narrative.DecisionCategory.Tactical;
        }
        
        private StarkillerBaseCommand.Narrative.DecisionPressure DeterminePressure(int imperialPoints, int insurgentPoints)
        {
            int totalMagnitude = Mathf.Abs(imperialPoints) + Mathf.Abs(insurgentPoints);
            
            if (totalMagnitude >= 20)
                return StarkillerBaseCommand.Narrative.DecisionPressure.Critical;
            if (totalMagnitude >= 10)
                return StarkillerBaseCommand.Narrative.DecisionPressure.High;
            if (totalMagnitude >= 5)
                return StarkillerBaseCommand.Narrative.DecisionPressure.Medium;
            
            return StarkillerBaseCommand.Narrative.DecisionPressure.Low;
        }
        
        // Original methods maintained for compatibility
        public bool IsStoryTagUnlocked(string tag)
        {
            return narrativeTracker != null && narrativeTracker.IsStoryTagUnlocked(tag);
        }

        public void UnlockStoryTag(string tag)
        {
            if (narrativeTracker != null)
                narrativeTracker.UnlockStoryTag(tag);
        }

        // Event handlers
        private void HandleBranchChange(StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker.NarrativeBranch newBranch)
        {
            Debug.Log($"Narrative Branch Changed to: {newBranch}");
            
            // Reset decision chains on major branch changes
            if (newBranch == StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker.NarrativeBranch.ImperiumPath || 
                newBranch == StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker.NarrativeBranch.InsurgentPath)
            {
                currentDecisionChainId = null;
                decisionChainLength = 0;
            }
        }

        private void HandleKeyDecision(StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker.NarrativeState.DecisionRecord decision)
        {
            Debug.Log($"Key Decision Made: {decision.decisionId}");
        }

        private void HandleStoryTagUnlocked(string tag)
        {
            Debug.Log($"Story Tag Unlocked: {tag}");
        }

        void OnDestroy()
        {
            if (narrativeTracker != null)
            {
                narrativeTracker.OnNarrativeBranchChanged -= HandleBranchChange;
                narrativeTracker.OnKeyDecisionMade -= HandleKeyDecision;
                narrativeTracker.OnStoryTagUnlocked -= HandleStoryTagUnlocked;
            }
        }
    }
}