using UnityEngine;
using System.Collections.Generic;
using StarkillerBaseCommand.Narrative;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Template for common decision types with pre-configured values
    /// </summary>
    [CreateAssetMenu(fileName = "DecisionTemplate", menuName = "Starkiller/Narrative/Decision Template")]
    public class DecisionTemplate : ScriptableObject
    {
        [Header("Template Info")]
        public string templateName = "New Decision Template";
        public string templateId = "template_default";
        
        [Header("Decision Configuration")]
        public StarkillerBaseCommand.Narrative.DecisionCategory category = StarkillerBaseCommand.Narrative.DecisionCategory.Moral;
        public StarkillerBaseCommand.Narrative.DecisionPressure basePressure = StarkillerBaseCommand.Narrative.DecisionPressure.Medium;
        
        [Header("Point Values")]
        [Tooltip("Points added to Imperial loyalty")]
        public int baseImperialPoints = 0;
        [Tooltip("Points added to Insurgent sympathy")]
        public int baseInsurgentPoints = 0;
        
        [Header("Context Modifiers")]
        [Tooltip("Additional pressure if under time pressure")]
        public bool increasePressureUnderTime = true;
        [Tooltip("Additional pressure if family in crisis")]
        public bool increasePressureIfFamilyCrisis = true;
        [Tooltip("Multiply points if player has low credits")]
        public float lowCreditsMultiplier = 1.5f;
        
        [Header("Feedback Messages")]
        [TextArea(2, 4)]
        public string imperialFeedback = "The Imperium appreciates your loyalty.";
        [TextArea(2, 4)]
        public string insurgentFeedback = "Your compassion has been noted.";
        [TextArea(2, 4)]
        public string neutralFeedback = "Decision logged.";
        
        [Header("Follow-up Decisions")]
        [Tooltip("Other decision templates that might trigger after this one")]
        public List<DecisionTemplate> possibleFollowUps = new List<DecisionTemplate>();
        [Range(0f, 1f)]
        public float followUpChance = 0.3f;
        
        /// <summary>
        /// Apply this template to create a decision record
        /// </summary>
        public StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker.NarrativeState.DecisionRecord CreateDecision(string specificContext = "")
        {
            // Calculate actual pressure based on context
            StarkillerBaseCommand.Narrative.DecisionPressure actualPressure = CalculateActualPressure();
            
            // Calculate actual points based on modifiers
            int imperialPoints = CalculateModifiedPoints(baseImperialPoints);
            int insurgentPoints = CalculateModifiedPoints(baseInsurgentPoints);
            
            // Create the decision record
            var decision = new StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker.NarrativeState.DecisionRecord
            {
                decisionId = $"{templateId}_{System.Guid.NewGuid()}",
                imperialPoints = imperialPoints,
                insurgentPoints = insurgentPoints,
                decisionContext = string.IsNullOrEmpty(specificContext) ? templateName : specificContext,
                timestamp = System.DateTime.Now
            };
            
            return decision;
        }
        
        /// <summary>
        /// Get appropriate feedback based on the decision's impact
        /// </summary>
        public string GetFeedbackMessage()
        {
            if (baseImperialPoints > baseInsurgentPoints)
                return imperialFeedback;
            else if (baseInsurgentPoints > baseImperialPoints)
                return insurgentFeedback;
            else
                return neutralFeedback;
        }
        
        /// <summary>
        /// Check if a follow-up decision should trigger
        /// </summary>
        public DecisionTemplate GetRandomFollowUp()
        {
            if (possibleFollowUps.Count == 0 || Random.value > followUpChance)
                return null;
                
            return possibleFollowUps[Random.Range(0, possibleFollowUps.Count)];
        }
        
        private StarkillerBaseCommand.Narrative.DecisionPressure CalculateActualPressure()
        {
            StarkillerBaseCommand.Narrative.DecisionPressure pressure = basePressure;
            
            // Get current game state
            GameManager gm = GameObject.FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                // Increase pressure if under time pressure
                // Note: You'll need to make remainingTime public in GameManager
                // if (increasePressureUnderTime && gm.remainingTime < 60f)
                // {
                //     pressure = (StarkillerBaseCommand.Narrative.DecisionPressure)Mathf.Min((int)pressure + 1, (int)StarkillerBaseCommand.Narrative.DecisionPressure.Critical);
                // }
            }
            
            // Check family status
            ImperialFamilySystem familySystem = GameObject.FindFirstObjectByType<ImperialFamilySystem>();
            if (familySystem != null && increasePressureIfFamilyCrisis)
            {
                // Note: You'll need to add IsInCrisis() method to ImperialFamilySystem
                // if (familySystem.IsInCrisis())
                // {
                //     pressure = (StarkillerBaseCommand.Narrative.DecisionPressure)Mathf.Min((int)pressure + 1, (int)StarkillerBaseCommand.Narrative.DecisionPressure.Critical);
                // }
            }
            
            return pressure;
        }
        
        private int CalculateModifiedPoints(int basePoints)
        {
            float modifier = 1f;
            
            // Check for low credits modifier
            GameManager gm = GameObject.FindFirstObjectByType<GameManager>();
            if (gm != null && gm.GetCredits() < 20) // Low credits threshold
            {
                modifier *= lowCreditsMultiplier;
            }
            
            return Mathf.RoundToInt(basePoints * modifier);
        }
    }
}