using UnityEngine;
using System;
using System.Collections.Generic;

namespace Starkiller.Core.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject that tracks scenario outcomes for ending achievement display
    /// Maps player decisions in scenarios to achievement text shown in endings
    /// </summary>
    [CreateAssetMenu(fileName = "ScenarioConsequence", menuName = "Starkiller/Narrative/Scenario Consequence")]
    public class ScenarioConsequenceSO : ScriptableObject
    {
        [Header("Scenario Identification")]
        [SerializeField] private string scenarioId;
        [SerializeField] private string scenarioName;
        [TextArea(2, 3)]
        [SerializeField] private string scenarioDescription;
        [SerializeField] private int dayRange = 30; // Which day(s) this scenario can occur
        
        [Header("Decision Consequences")]
        [SerializeField] private List<ConsequenceMapping> consequenceMappings = new List<ConsequenceMapping>();
        
        [Header("Ending Achievement Configuration")]
        [SerializeField] private bool isKeyScenario = false; // Major scenarios that always appear in endings
        [SerializeField] private int achievementPriority = 0; // Higher priority achievements display first
        [SerializeField] private AchievementCategory defaultCategory = AchievementCategory.Neutral;
        
        // Properties
        public string ScenarioId => scenarioId;
        public string ScenarioName => scenarioName;
        public string Description => scenarioDescription;
        public int DayRange => dayRange;
        public bool IsKeyScenario => isKeyScenario;
        public int AchievementPriority => achievementPriority;
        
        /// <summary>
        /// Get the achievement text for a specific decision
        /// </summary>
        public string GetAchievementText(string decisionId)
        {
            var mapping = consequenceMappings.Find(m => m.decisionId == decisionId);
            return mapping != null ? mapping.achievementText : null;
        }
        
        /// <summary>
        /// Get the achievement category for a specific decision
        /// </summary>
        public AchievementCategory GetAchievementCategory(string decisionId)
        {
            var mapping = consequenceMappings.Find(m => m.decisionId == decisionId);
            return mapping != null ? mapping.category : defaultCategory;
        }
        
        /// <summary>
        /// Get achievement data for ending display
        /// </summary>
        public AchievementData GetAchievementData(string decisionId)
        {
            var mapping = consequenceMappings.Find(m => m.decisionId == decisionId);
            if (mapping == null) return null;
            
            return new AchievementData
            {
                Text = mapping.achievementText,
                Category = mapping.category,
                Priority = achievementPriority,
                IsKeyAchievement = isKeyScenario,
                ScenarioName = scenarioName
            };
        }
        
        /// <summary>
        /// Validate the scenario consequence data
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(scenarioId))
            {
                Debug.LogWarning($"ScenarioConsequenceSO: Missing scenario ID for {name}");
                return false;
            }
            
            if (consequenceMappings.Count == 0)
            {
                Debug.LogWarning($"ScenarioConsequenceSO: No consequence mappings for {scenarioId}");
                return false;
            }
            
            foreach (var mapping in consequenceMappings)
            {
                if (string.IsNullOrEmpty(mapping.decisionId) || string.IsNullOrEmpty(mapping.achievementText))
                {
                    Debug.LogWarning($"ScenarioConsequenceSO: Invalid mapping in {scenarioId}");
                    return false;
                }
            }
            
            return true;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor validation
        /// </summary>
        private void OnValidate()
        {
            // Ensure we have a scenario ID
            if (string.IsNullOrEmpty(scenarioId) && !string.IsNullOrEmpty(scenarioName))
            {
                scenarioId = scenarioName.Replace(" ", "").ToUpper();
            }
            
            // Validate priority range
            achievementPriority = Mathf.Clamp(achievementPriority, 0, 100);
            
            // Validate day range
            dayRange = Mathf.Clamp(dayRange, 1, 30);
        }
        #endif
    }
    
    /// <summary>
    /// Maps a decision to its consequence text and category
    /// </summary>
    [Serializable]
    public class ConsequenceMapping
    {
        [Header("Decision")]
        public string decisionId;
        public string decisionLabel;
        
        [Header("Achievement")]
        [TextArea(2, 3)]
        public string achievementText;
        public AchievementCategory category = AchievementCategory.Neutral;
        
        [Header("Additional Effects")]
        public int loyaltyImpact = 0; // -10 to +10
        public int rebellionImpact = 0; // -10 to +10
        public bool affectsFamily = false;
        public bool affectsPerformance = false;
    }
    
    /// <summary>
    /// Category for achievement display
    /// </summary>
    public enum AchievementCategory
    {
        Positive,   // Good outcomes, heroic actions
        Negative,   // Bad outcomes, moral compromises
        Neutral,    // Neutral outcomes, survival choices
        Imperial,   // Pro-Imperial actions
        Rebellion,  // Pro-Rebellion actions
        Family,     // Family-related decisions
        Duty,       // Duty-focused decisions
        Corruption  // Corrupt/bribery decisions
    }
    
    /// <summary>
    /// Achievement data for ending display
    /// </summary>
    public class AchievementData
    {
        public string Text { get; set; }
        public AchievementCategory Category { get; set; }
        public int Priority { get; set; }
        public bool IsKeyAchievement { get; set; }
        public string ScenarioName { get; set; }
    }
}