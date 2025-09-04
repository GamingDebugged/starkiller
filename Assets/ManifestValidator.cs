using UnityEngine;
using System.Collections.Generic;
using StarkillerBaseCommand;

public class ManifestValidator : MonoBehaviour
{
    public static bool ValidateManifest(CargoManifest manifest, MasterShipEncounter encounter, int currentDay, List<StarkkillerContentManager.DayRule> dayRules)
    {
        if (manifest == null)
            return false;
            
        // Check faction compatibility
        if (!IsFactionAuthorized(manifest, encounter.faction))
            return false;
            
        // Check day restrictions
        if (!IsValidForDay(manifest, currentDay))
            return false;
            
        // Check clearance level
        if (!HasSufficientClearance(manifest, encounter.accessCodeData))
            return false;
            
        // Check against day rules
        if (!CompliesWithDayRules(manifest, dayRules))
            return false;
            
        return true;
    }
    
    /// <summary>
    /// Check if the manifest is authorized for the given faction
    /// </summary>
    private static bool IsFactionAuthorized(CargoManifest manifest, string faction)
    {
        // Use the manifest's built-in validation method
        return manifest.IsValidForFaction(faction);
    }
    
    /// <summary>
    /// Check if the manifest is valid for the current day
    /// </summary>
    private static bool IsValidForDay(CargoManifest manifest, int currentDay)
    {
        // Use the manifest's built-in validation method
        return manifest.IsValidForDay(currentDay);
    }
    
    /// <summary>
    /// Check if the manifest has sufficient clearance level
    /// </summary>
    private static bool HasSufficientClearance(CargoManifest manifest, AccessCode accessCodeData)
    {
        // If no access code data, check if manifest requires special clearance
        if (accessCodeData == null)
        {
            // Only allow manifests that don't require special clearance
            return manifest.requiredClearanceLevel <= CargoManifest.ClearanceLevel.Standard;
        }
        
        // Map AccessCode levels to CargoManifest clearance levels
        CargoManifest.ClearanceLevel requiredLevel = manifest.requiredClearanceLevel;
        
        // Check based on access code level
        switch (accessCodeData.level)
        {
            case AccessCode.AccessLevel.Low:
                return requiredLevel <= CargoManifest.ClearanceLevel.Standard;
            case AccessCode.AccessLevel.Medium:
                return requiredLevel <= CargoManifest.ClearanceLevel.Restricted;
            case AccessCode.AccessLevel.High:
                return requiredLevel <= CargoManifest.ClearanceLevel.Classified;
            case AccessCode.AccessLevel.Unrestricted:
                return true; // Unrestricted access can view all manifests
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Check if the manifest complies with current day rules
    /// </summary>
    private static bool CompliesWithDayRules(CargoManifest manifest, List<StarkkillerContentManager.DayRule> dayRules)
    {
        if (dayRules == null || dayRules.Count == 0)
            return true; // No rules to violate
            
        foreach (var rule in dayRules)
        {
            switch (rule.ruleType)
            {
                case StarkkillerContentManager.DayRule.RuleType.CheckForContraband:
                    // If contraband check is active and manifest has contraband, it fails
                    if (manifest.hasContraband)
                        return false;
                    break;
                    
                case StarkkillerContentManager.DayRule.RuleType.VerifyManifest:
                    // If manifest verification is required, check for false entries
                    if (manifest.hasFalseEntries)
                        return false;
                    break;
                    
                case StarkkillerContentManager.DayRule.RuleType.ForceInspection:
                    // Force inspection means manifests with contraband are more likely to be caught
                    if (manifest.hasContraband && manifest.isEasilyDetectable)
                        return false;
                    break;
            }
        }
        
        // Check if manifest contains suspicious keywords based on day rules
        List<string> ruleKeywords = new List<string>();
        foreach (var rule in dayRules)
        {
            if (!string.IsNullOrEmpty(rule.ruleDescription))
            {
                // Extract keywords from rule descriptions
                string[] words = rule.ruleDescription.ToLower().Split(' ');
                ruleKeywords.AddRange(words);
            }
        }
        
        if (ruleKeywords.Count > 0 && manifest.ContainsSuspiciousContent(ruleKeywords.ToArray()))
        {
            return false;
        }
        
        return true;
    }
}