using UnityEngine;
using System.Collections.Generic;
using StarkillerBaseCommand.Narrative;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Manages decision templates and provides context-aware feedback
    /// </summary>
    public class DecisionTemplateManager : MonoBehaviour
    {
        private static DecisionTemplateManager instance;
        public static DecisionTemplateManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindFirstObjectByType<DecisionTemplateManager>();
                return instance;
            }
        }
        
        [Header("Decision Templates")]
        [SerializeField] private List<DecisionTemplate> availableTemplates = new List<DecisionTemplate>();
        
        [Header("Common Templates")]
        [SerializeField] private DecisionTemplate acceptBribeTemplate;
        [SerializeField] private DecisionTemplate denyRebelTemplate;
        [SerializeField] private DecisionTemplate approveImperiumTemplate;
        [SerializeField] private DecisionTemplate useForceTemplate;
        
        [Header("Context Feedback")]
        [SerializeField] private bool showContextualFeedback = true;
        [SerializeField] private float feedbackDuration = 3f;
        
        private Queue<string> pendingFeedback = new Queue<string>();
        
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }
        
        /// <summary>
        /// Process a decision using a specific template
        /// </summary>
        public void ProcessTemplatedDecision(DecisionTemplate template, string additionalContext = "")
        {
            if (template == null)
            {
                Debug.LogWarning("Attempted to process null decision template!");
                return;
            }
            
            // Create decision from template
            var decision = template.CreateDecision(additionalContext);
            
            // Record it in the narrative tracker via NarrativeManager
            var narrativeManager = NarrativeManager.Instance;
            if (narrativeManager != null)
            {
                narrativeManager.RecordEnhancedDecision(
                    decision.decisionId,
                    decision.imperialPoints,
                    decision.insurgentPoints,
                    decision.decisionContext,
                    template.category,
                    template.basePressure
                );
            }
            
            // Show feedback
            if (showContextualFeedback)
            {
                ShowContextualFeedback(template, decision);
            }
            
            // Check for follow-up decisions
            var followUp = template.GetRandomFollowUp();
            if (followUp != null)
            {
                // Queue the follow-up for later
                StartCoroutine(TriggerFollowUpDecision(followUp, Random.Range(5f, 15f)));
            }
        }
        
        /// <summary>
        /// Process common decision types with templates
        /// </summary>
        public void ProcessBribeAccepted(int bribeAmount)
        {
            if (acceptBribeTemplate != null)
            {
                string context = string.Format("Accepted bribe of {0} credits", bribeAmount);
                ProcessTemplatedDecision(acceptBribeTemplate, context);
            }
            else
            {
                // Fallback to basic recording
                RecordBasicDecision("bribe_accepted", -5, 10, StarkillerBaseCommand.Narrative.DecisionCategory.Financial, StarkillerBaseCommand.Narrative.DecisionPressure.High);
            }
        }
        
        public void ProcessRebelDenied(string shipName)
        {
            if (denyRebelTemplate != null)
            {
                string context = string.Format("Denied access to suspected rebel ship: {0}", shipName);
                ProcessTemplatedDecision(denyRebelTemplate, context);
            }
            else
            {
                RecordBasicDecision("rebel_denied", 10, -5, StarkillerBaseCommand.Narrative.DecisionCategory.Political, StarkillerBaseCommand.Narrative.DecisionPressure.Medium);
            }
        }
        
        /// <summary>
        /// Show contextual feedback based on decision and current state
        /// </summary>
        private void ShowContextualFeedback(DecisionTemplate template, StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker.NarrativeState.DecisionRecord decision)
        {
            string feedback = template.GetFeedbackMessage();
            
            // Add context-specific modifications based on decision context
            if (!string.IsNullOrEmpty(decision.decisionContext))
            {
                if (decision.decisionContext.ToLower().Contains("time") || decision.decisionContext.ToLower().Contains("urgent"))
                {
                    feedback = "[TIME PRESSURE] " + feedback;
                }
                
                if (decision.decisionContext.ToLower().Contains("family") || decision.decisionContext.ToLower().Contains("personal"))
                {
                    feedback += "\nYour family situation influenced this decision.";
                }
                
                if (decision.decisionContext.ToLower().Contains("critical") || Mathf.Abs(decision.imperialPoints) + Mathf.Abs(decision.insurgentPoints) >= 20)
                {
                    feedback = "CRITICAL DECISION: " + feedback;
                }
            }
            
            // Show it via CredentialChecker if available
            CredentialChecker checker = FindFirstObjectByType<CredentialChecker>();
            if (checker != null)
            {
                Color feedbackColor = GetFeedbackColor(template);
                checker.ShowFeedback(feedback, feedbackColor);
            }
        }
        
        /// <summary>
        /// Get appropriate color for feedback based on alignment
        /// </summary>
        private Color GetFeedbackColor(DecisionTemplate template)
        {
            if (template.baseImperialPoints > template.baseInsurgentPoints)
                return new Color(0.2f, 0.5f, 1f); // Imperial blue
            else if (template.baseInsurgentPoints > template.baseImperialPoints)
                return new Color(1f, 0.3f, 0.2f); // Rebel red
            else
                return Color.yellow; // Neutral
        }
        
        /// <summary>
        /// Trigger a follow-up decision after delay
        /// </summary>
        private System.Collections.IEnumerator TriggerFollowUpDecision(DecisionTemplate followUp, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            // Check if game is still active
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null && gm.IsGameActive())
            {
                Debug.Log("Triggering follow-up decision: " + followUp.templateName);
                ProcessTemplatedDecision(followUp);
            }
        }
        
        /// <summary>
        /// Fallback method for basic decision recording
        /// </summary>
        private void RecordBasicDecision(string id, int imperial, int insurgent, 
            StarkillerBaseCommand.Narrative.DecisionCategory category, StarkillerBaseCommand.Narrative.DecisionPressure pressure)
        {
            var narrativeManager = NarrativeManager.Instance;
            if (narrativeManager != null)
            {
                narrativeManager.RecordEnhancedDecision(
                    id + "_" + System.Guid.NewGuid(),
                    imperial,
                    insurgent,
                    id.Replace('_', ' '),
                    category,
                    pressure
                );
            }
        }
    }
}