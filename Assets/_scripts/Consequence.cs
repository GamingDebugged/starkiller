using UnityEngine;
using StarkillerBaseCommand;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// ScriptableObject for consequences in Starkiller Base Command
    /// Defines what happens when the player makes incorrect decisions
    /// </summary>
    [CreateAssetMenu(fileName = "New Consequence", menuName = "Starkiller Base/Consequence")]
    public class Consequence : ScriptableObject
    {
        [Header("Basic Information")]
        public string title;
        public enum ConsequenceType { Security, Financial, Reputation, Family, Story, Special }
        public ConsequenceType type;
        
        public enum SeverityLevel { Minor, Moderate, Severe, Critical }
        public SeverityLevel severity;
        
        [Header("Description")]
        [TextArea(3, 6)]
        public string[] possibleDescriptions;
        
        [Header("Game Impact")]
        public int imperiumCasualties;
        public int creditPenalty;
        public int imperialLoyaltyChange;
        public int rebellionSympathyChange;
        
        [Header("Special Effects")]
        public bool triggersInspection;
        public bool affectsFamily;
        [TextArea(2, 4)]
        public string familyEffect;
        
        [Header("Long-term Effects")]
        public bool hasLongTermEffect;
        public int effectDuration; // In days
        [TextArea(2, 4)]
        public string longTermDescription;
        
        [Header("Ending Integration")]
        [TextArea(2, 4)]
        [Tooltip("How this consequence appears in the ending screen achievements")]
        public string endGameMessage;
        
        [Header("Visuals")]
        public Sprite consequenceIcon;
        public Color severityColor = Color.red;
        
        /// <summary>
        /// Get a random description for this consequence
        /// </summary>
        public string GetRandomDescription()
        {
            if (possibleDescriptions != null && possibleDescriptions.Length > 0)
            {
                string desc = possibleDescriptions[Random.Range(0, possibleDescriptions.Length)];
                return desc.Replace("{CASUALTIES}", imperiumCasualties.ToString())
                          .Replace("{CREDITS}", creditPenalty.ToString())
                          .Replace("{IMPERIAL_LOYALTY}", imperialLoyaltyChange.ToString())
                          .Replace("{REBEL_SYMPATHY}", rebellionSympathyChange.ToString());
            }
            
            return "Your mistake has led to consequences.";
        }
        
        /// <summary>
        /// Get a full report of this consequence
        /// </summary>
        public string GetFullReport()
        {
            // Get severity color in html format
            string colorHex = ColorUtility.ToHtmlStringRGB(severityColor);
            
            // Build report with rich text formatting
            string report = $"<color=#{colorHex}><b>{title}</b></color>\n\n";
            report += GetRandomDescription() + "\n\n";
            
            // Add impact details
            if (imperiumCasualties > 0)
            {
                report += $"<b>Imperium Casualties:</b> {imperiumCasualties}\n";
            }
            
            if (creditPenalty > 0)
            {
                report += $"<b>Credit Penalty:</b> {creditPenalty}\n";
            }
            
            if (imperialLoyaltyChange != 0)
            {
                string direction = imperialLoyaltyChange > 0 ? "Increased" : "Decreased";
                report += $"<b>Imperial Loyalty {direction}:</b> {Mathf.Abs(imperialLoyaltyChange)}\n";
            }
            
            if (rebellionSympathyChange != 0)
            {
                string direction = rebellionSympathyChange > 0 ? "Increased" : "Decreased";
                report += $"<b>Rebellion Sympathy {direction}:</b> {Mathf.Abs(rebellionSympathyChange)}\n";
            }
            
            // Add special effects
            if (triggersInspection)
            {
                report += "\n<b>Special Effect:</b> Security inspection has been triggered.\n";
            }
            
            // Add family effects
            if (affectsFamily)
            {
                report += $"\n<b>Family Impact:</b> {familyEffect}\n";
            }
            
            // Add long-term effects
            if (hasLongTermEffect)
            {
                report += $"\n<b>Long-term Effect ({effectDuration} days):</b> {longTermDescription}\n";
            }
            
            return report;
        }
        
        /// <summary>
        /// Get a short summary of this consequence (for listing)
        /// </summary>
        public string GetSummary()
        {
            string summary = $"{title} - ";
            
            switch (severity)
            {
                case SeverityLevel.Critical:
                    summary += "<color=red>CRITICAL</color>";
                    break;
                case SeverityLevel.Severe:
                    summary += "<color=#FF6600>SEVERE</color>";
                    break;
                case SeverityLevel.Moderate:
                    summary += "<color=yellow>MODERATE</color>";
                    break;
                case SeverityLevel.Minor:
                    summary += "<color=green>MINOR</color>";
                    break;
            }
            
            return summary;
        }
    }
}