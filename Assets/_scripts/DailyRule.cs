using UnityEngine;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// ScriptableObject for daily rules in Starkiller Base Command
    /// These define the changing rules that apply on different game days
    /// </summary>
    [CreateAssetMenu(fileName = "New Daily Rule", menuName = "Starkiller Base/Daily Rule")]
    public class DailyRule : ScriptableObject
    {
        [Header("Basic Information")]
        public string ruleName;
        public int dayNumber; // Game day this rule applies
        
        [Header("Rule Definition")]
        [TextArea(3, 6)]
        public string ruleDescription; // Full description of rule
        public string[] requirementTexts; // Individual requirements as bullet points
        
        [Header("Rule Logic")]
        public string[] requiredDocuments; // Documents that must be checked
        public string[] bannedOrigins; // Origins that are not allowed
        public string[] bannedShipTypes; // Ship types not allowed
        public string[] specialRequirements; // Any special requirements
    }
}