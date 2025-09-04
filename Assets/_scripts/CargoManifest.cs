using UnityEngine;
using System.Collections.Generic;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Enhanced ScriptableObject for cargo manifests in Starkiller Base Command
    /// Supports faction awareness, contraband detection, and validation
    /// </summary>
    [CreateAssetMenu(fileName = "New Cargo Manifest", menuName = "Starkiller Base/Cargo Manifest")]
    public class CargoManifest : ScriptableObject
    {
        [Header("Basic Information")]
        public string manifestName;
        public string manifestCode; // Reference code
        
        [Header("Manifest Content")]
        [TextArea(3, 6)]
        public string manifestDescription; // Full text description for display
        
        [Header("Cargo Details")]
        public string[] declaredItems; // What they claim to be carrying
        public string[] actualItems; // What they're actually carrying (if different)
        public int totalCargoUnits = 1;
        public float cargoWeight = 1.0f;
        
        [Header("Faction and Restrictions")]
        public FactionRestriction factionRestriction = FactionRestriction.Universal;
        public string[] allowedFactions; // Which factions can use this manifest
        public string[] requiredShipCategories; // Which ship types this applies to
        
        [Header("Contraband and Issues")]
        public bool hasContraband = false; // Whether manifest contains contraband
        public bool hasFalseEntries = false; // Whether manifest is falsified
        public ContrabandType contrabandType = ContrabandType.None;
        public string[] contrabandItems; // Specific contraband items
        public bool isEasilyDetectable = true; // Whether contraband is obvious
        
        [Header("Clearance and Authorization")]
        public string clearanceCode; // Associated clearance
        public string authorizedBy; // Who authorized it
        public ClearanceLevel requiredClearanceLevel = ClearanceLevel.Standard;
        
        [Header("Day Progression")]
        public int firstAppearanceDay = 1; // When this manifest can first appear
        public int lastAppearanceDay = -1; // When it stops appearing (-1 = no limit)
        public int maxDailyAppearances = -1; // How many times per day (-1 = unlimited)
        
        [Header("Validation Rules")]
        public bool requiresSpecialValidation = false;
        public string[] validationKeywords; // Keywords that validate or invalidate this manifest
        public string[] suspiciousKeywords; // Keywords that make this manifest suspicious
        
        [Header("Gameplay Impact")]
        public int reputationImpactIfWrong = -10; // Reputation change if processed incorrectly
        public int creditRewardIfCorrect = 5; // Credits for correct processing
        public ManifestPriority priority = ManifestPriority.Normal;
        
        public enum FactionRestriction
        {
            Universal,      // Any faction can use this
            FactionSpecific, // Only allowed factions can use this
            Restricted      // Special restrictions apply
        }
        
        public enum ContrabandType
        {
            None,
            Weapons,
            Drugs,
            Technology,
            Intelligence,
            Personnel,
            Mixed
        }
        
        public enum ClearanceLevel
        {
            Public,
            Standard,
            Restricted,
            Classified,
            TopSecret
        }
        
        public enum ManifestPriority
        {
            Low,
            Normal,
            High,
            Critical
        }
        
        #region Validation Methods
        
        /// <summary>
        /// Check if this manifest is valid for the given faction
        /// </summary>
        public bool IsValidForFaction(string faction)
        {
            if (factionRestriction == FactionRestriction.Universal)
                return true;
                
            if (allowedFactions == null || allowedFactions.Length == 0)
                return factionRestriction == FactionRestriction.Universal;
                
            foreach (string allowedFaction in allowedFactions)
            {
                if (allowedFaction.Equals(faction, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if this manifest is valid for the given ship category
        /// </summary>
        public bool IsValidForShipCategory(string shipCategory)
        {
            if (requiredShipCategories == null || requiredShipCategories.Length == 0)
                return true; // No restrictions
                
            foreach (string category in requiredShipCategories)
            {
                if (category.Equals(shipCategory, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if this manifest is valid for the current day
        /// </summary>
        public bool IsValidForDay(int currentDay)
        {
            if (currentDay < firstAppearanceDay)
                return false;
                
            if (lastAppearanceDay > 0 && currentDay > lastAppearanceDay)
                return false;
                
            return true;
        }
        
        /// <summary>
        /// Check if the manifest contains any of the given suspicious keywords
        /// </summary>
        public bool ContainsSuspiciousContent(string[] dayRuleKeywords)
        {
            if (suspiciousKeywords == null || suspiciousKeywords.Length == 0)
                return false;
                
            if (dayRuleKeywords == null || dayRuleKeywords.Length == 0)
                return false;
                
            foreach (string suspicious in suspiciousKeywords)
            {
                foreach (string ruleKeyword in dayRuleKeywords)
                {
                    if (suspicious.Equals(ruleKeyword, System.StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Get the display text for this manifest (backwards compatible)
        /// </summary>
        public string GetDisplayText()
        {
            if (!string.IsNullOrEmpty(manifestDescription))
                return manifestDescription;
                
            // Fallback: Generate from declared items
            if (declaredItems != null && declaredItems.Length > 0)
            {
                return string.Join(", ", declaredItems);
            }
            
            return manifestName ?? "Unknown Cargo";
        }
        
        /// <summary>
        /// Check if this manifest should trigger special validation
        /// </summary>
        public bool ShouldTriggerValidation(List<string> currentDayRules)
        {
            if (!requiresSpecialValidation)
                return false;
                
            if (validationKeywords == null || validationKeywords.Length == 0)
                return false;
                
            foreach (string keyword in validationKeywords)
            {
                foreach (string rule in currentDayRules)
                {
                    if (rule.ToLower().Contains(keyword.ToLower()))
                        return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Get contraband detection difficulty (for gameplay)
        /// </summary>
        public float GetContrabandDetectionDifficulty()
        {
            if (!hasContraband)
                return 0f;
                
            float baseDifficulty = isEasilyDetectable ? 0.2f : 0.8f;
            
            // Adjust based on contraband type
            switch (contrabandType)
            {
                case ContrabandType.Weapons:
                    return baseDifficulty * 0.7f; // Easier to detect
                case ContrabandType.Technology:
                    return baseDifficulty * 1.2f; // Harder to detect
                case ContrabandType.Intelligence:
                    return baseDifficulty * 1.5f; // Much harder to detect
                default:
                    return baseDifficulty;
            }
        }
        
        /// <summary>
        /// Get a detailed inspection report for this manifest
        /// </summary>
        public string GetInspectionReport()
        {
            System.Text.StringBuilder report = new System.Text.StringBuilder();
            
            report.AppendLine($"MANIFEST INSPECTION REPORT");
            report.AppendLine($"Manifest Code: {manifestCode}");
            report.AppendLine($"Authorized By: {authorizedBy}");
            report.AppendLine($"Clearance Level: {requiredClearanceLevel}");
            
            if (declaredItems != null && declaredItems.Length > 0)
            {
                report.AppendLine($"Declared Items: {string.Join(", ", declaredItems)}");
            }
            
            if (hasContraband)
            {
                report.AppendLine($"⚠️ CONTRABAND DETECTED: {contrabandType}");
                if (contrabandItems != null && contrabandItems.Length > 0)
                {
                    report.AppendLine($"Contraband Items: {string.Join(", ", contrabandItems)}");
                }
            }
            
            if (hasFalseEntries)
            {
                report.AppendLine($"⚠️ FALSE ENTRIES DETECTED");
                if (actualItems != null && actualItems.Length > 0)
                {
                    report.AppendLine($"Actual Items: {string.Join(", ", actualItems)}");
                }
            }
            
            return report.ToString();
        }
        
        #endregion
        
        #region Editor Utilities
        
        #if UNITY_EDITOR
        /// <summary>
        /// Validate the manifest configuration in the editor
        /// </summary>
        void OnValidate()
        {
            // Ensure manifest has a name
            if (string.IsNullOrEmpty(manifestName))
                manifestName = name;
                
            // Ensure manifest code exists
            if (string.IsNullOrEmpty(manifestCode))
                manifestCode = $"MANIFEST-{UnityEngine.Random.Range(1000, 9999)}";
                
            // Validate contraband settings
            if (hasContraband && (contrabandItems == null || contrabandItems.Length == 0))
            {
                Debug.LogWarning($"Manifest {manifestName} has contraband but no contraband items specified");
            }
            
            // Validate faction restrictions
            if (factionRestriction == FactionRestriction.FactionSpecific && 
                (allowedFactions == null || allowedFactions.Length == 0))
            {
                Debug.LogWarning($"Manifest {manifestName} is faction-specific but no allowed factions specified");
            }
        }
        #endif
        
        #endregion
    }
}