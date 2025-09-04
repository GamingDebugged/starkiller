using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarkillerBaseCommand.Narrative
{
    /// <summary>
    /// Advanced Narrative Progression Tracker for Starkiller Base Command
    /// Manages complex narrative states, decision tracking, and branching
    /// </summary>
    [CreateAssetMenu(fileName = "NarrativeTracker", menuName = "Starkiller/Narrative/Advanced NarrativeTracker", order = 1)]
    public class AdvancedNarrativeTracker : ScriptableObject
    {
        [Serializable]
        public class NarrativeState
        {
            // Core Loyalty Metrics
            public int imperialLoyalty = 0;
            public int insurgentSympathy = 0;
            
            // Expanded Decision Tracking
            [Serializable]
            public class DecisionRecord
            {
                public string decisionId;
                public DateTime timestamp;
                public int imperialPoints;
                public int insurgentPoints;
                public string decisionContext;
            }
            
            public List<DecisionRecord> keyDecisions = new List<DecisionRecord>();
            
            // Narrative Progression Tracking
            public NarrativeBranch currentBranch = NarrativeBranch.Neutral;
            public int narrativeProgressionLevel = 0;
            
            // Unlocked Story Elements
            public HashSet<string> unlockedStoryTags = new HashSet<string>();
        }

        public enum NarrativeBranch
        {
            Neutral,
            ImperiumPath,
            InsurgentPath,
            ComplexResistance,
            DoubleCross,
            SilentDefiance
        }

        [SerializeField] 
        private NarrativeState currentState = new NarrativeState();

        // Configurable Narrative Progression Thresholds
        [Header("Narrative Progression Thresholds")]
        public int imperialLoyaltyThreshold = 50;
        public int insurgentSympathyThreshold = -50;
        public int complexResistanceThreshold = 25;

        // Events for tracking significant narrative changes
        public event Action<NarrativeBranch> OnNarrativeBranchChanged;
        public event Action<NarrativeState.DecisionRecord> OnKeyDecisionMade;
        public event Action<string> OnStoryTagUnlocked;

        /// <summary>
        /// Record a significant decision in the narrative
        /// </summary>
        public void RecordDecision(
            string decisionId, 
            int imperialPoints = 0, 
            int insurgentPoints = 0, 
            string decisionContext = "")
        {
            // Create decision record
            var decisionRecord = new NarrativeState.DecisionRecord
            {
                decisionId = decisionId,
                timestamp = DateTime.Now,
                imperialPoints = imperialPoints,
                insurgentPoints = insurgentPoints,
                decisionContext = decisionContext
            };

            // Add to decision history
            currentState.keyDecisions.Add(decisionRecord);

            // Update loyalty points
            currentState.imperialLoyalty += imperialPoints;
            currentState.insurgentSympathy += insurgentPoints;

            // Notify listeners of key decision
            OnKeyDecisionMade?.Invoke(decisionRecord);

            // Check for narrative branch and progression changes
            UpdateNarrativeProgression();
        }

        /// <summary>
        /// Unlock a specific story tag or progression element
        /// </summary>
        public void UnlockStoryTag(string storyTag)
        {
            if (currentState.unlockedStoryTags.Add(storyTag))
            {
                OnStoryTagUnlocked?.Invoke(storyTag);
            }
        }

        /// <summary>
        /// Determine and update the current narrative branch
        /// </summary>
        private void UpdateNarrativeProgression()
        {
            NarrativeBranch previousBranch = currentState.currentBranch;
            NarrativeBranch newBranch = DetermineNarrativeBranch();

            // Increment progression level if branch changes
            if (previousBranch != newBranch)
            {
                currentState.narrativeProgressionLevel++;
                currentState.currentBranch = newBranch;
                OnNarrativeBranchChanged?.Invoke(newBranch);
            }
        }

        /// <summary>
        /// Advanced branch determination with multiple paths
        /// </summary>
        private NarrativeBranch DetermineNarrativeBranch()
        {
            int loyaltyDifference = currentState.imperialLoyalty - currentState.insurgentSympathy;

            // Extremely nuanced branch determination
            if (Mathf.Abs(loyaltyDifference) < 10)
                return NarrativeBranch.Neutral;

            if (loyaltyDifference > imperialLoyaltyThreshold)
                return NarrativeBranch.ImperiumPath;

            if (loyaltyDifference < insurgentSympathyThreshold)
                return NarrativeBranch.InsurgentPath;

            // Complex scenarios
            if (Mathf.Abs(loyaltyDifference) < complexResistanceThreshold)
            {
                // Check for specific decision patterns
                var recentDecisions = currentState.keyDecisions
                    .TakeLast(5)
                    .ToList();

                // Example of detecting nuanced narrative states
                bool hasDoubleCrossSignals = recentDecisions.Any(d => 
                    d.decisionContext.Contains("betrayal") || 
                    d.decisionContext.Contains("manipulation"));

                return hasDoubleCrossSignals 
                    ? NarrativeBranch.DoubleCross 
                    : NarrativeBranch.ComplexResistance;
            }

            return NarrativeBranch.SilentDefiance;
        }

        /// <summary>
        /// Check if a specific story tag is unlocked
        /// </summary>
        public bool IsStoryTagUnlocked(string storyTag)
        {
            return currentState.unlockedStoryTags.Contains(storyTag);
        }

        /// <summary>
        /// Get current narrative state for external systems
        /// </summary>
        public NarrativeState GetCurrentState()
        {
            return new NarrativeState
            {
                imperialLoyalty = currentState.imperialLoyalty,
                insurgentSympathy = currentState.insurgentSympathy,
                keyDecisions = new List<NarrativeState.DecisionRecord>(currentState.keyDecisions),
                currentBranch = currentState.currentBranch,
                narrativeProgressionLevel = currentState.narrativeProgressionLevel,
                unlockedStoryTags = new HashSet<string>(currentState.unlockedStoryTags)
            };
        }

        /// <summary>
        /// Reset the narrative tracker to initial state
        /// </summary>
        public void Reset()
        {
            currentState = new NarrativeState();
        }

        /// <summary>
        /// Advanced condition checking with more context
        /// </summary>
        public bool CheckNarrativeCondition(
            Func<NarrativeState, bool> condition, 
            bool includeHistory = false)
        {
            // Option to include historical context in condition checking
            if (includeHistory)
            {
                // More complex condition checking
                return condition(currentState);
            }

            return condition(currentState);
        }

        /// <summary>
        /// Generate a debug report of current narrative state
        /// </summary>
        public string GenerateNarrativeReport()
        {
            return $"Narrative Report:\n" +
                   $"Branch: {currentState.currentBranch}\n" +
                   $"Progression Level: {currentState.narrativeProgressionLevel}\n" +
                   $"Imperial Loyalty: {currentState.imperialLoyalty}\n" +
                   $"Insurgent Sympathy: {currentState.insurgentSympathy}\n" +
                   $"Unlocked Story Tags: {string.Join(", ", currentState.unlockedStoryTags)}\n" +
                   $"Recent Decisions: {currentState.keyDecisions.TakeLast(3).Count()}";
        }
    }
}