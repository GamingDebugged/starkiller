using UnityEngine;
using System.Linq;

/// <summary>
/// ScriptableObject for ship categories in Starkiller Base Command
/// These define broad groups of ships with similar validation rules
/// </summary>
[CreateAssetMenu(fileName = "New Ship Category", menuName = "Starkiller Base/Ship Category")]
public class ShipCategory : ScriptableObject
{
    [Header("Basic Information")]
    public string categoryName;
    [TextArea(2, 5)]
    public string categoryDescription;
    
    [Header("Faction Associations")]
    [Tooltip("Which factions can legitimately use ships in this category")]
    public string[] associatedFactions;
    // Examples:
    // "Imperium" category → ["imperium", "military"]
    // "The Order" category → ["the_order", "religious"]
    // "Merchant" category → ["civilian", "merchant", "trade"]
    
    [Tooltip("Which captain factions are compatible with this ship category")]
    public string[] compatibleCaptainFactions;
    
    [Header("Validation Rules")]
    public string[] validOrigins;
    public string[] requiredDocuments;
    public string[] validAccessCodePrefixes;
    
    [Header("Security Settings")]
    [Range(0, 1)]
    public float suspicionBaseLevel;
    public bool requiresSpecialClearance;
    public bool priorityAccess;
    
    [Header("Appearance")]
    public Color categoryColor = Color.white; // For UI display
    
    [Header("Special Rules")]
    public bool canCarryContraband;
    public bool exemptFromRandomSearches;
    
    /// <summary>
    /// Check if a given faction is associated with this category
    /// </summary>
    public bool IsFactionAssociated(string faction)
    {
        if (associatedFactions == null || associatedFactions.Length == 0)
            return false;
            
        return associatedFactions.Any(f => f.Equals(faction, System.StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary>
    /// Check if a captain faction is compatible with this ship category
    /// </summary>
    public bool IsCaptainCompatible(string captainFaction)
    {
        if (compatibleCaptainFactions == null || compatibleCaptainFactions.Length == 0)
        {
            // If no specific compatibility is set, fall back to associated factions
            return IsFactionAssociated(captainFaction);
        }
        
        return compatibleCaptainFactions.Any(f => f.Equals(captainFaction, System.StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary>
    /// Check if an access code prefix is valid for this category
    /// </summary>
    public bool IsAccessCodeValid(string accessCode)
    {
        if (string.IsNullOrEmpty(accessCode) || validAccessCodePrefixes == null || validAccessCodePrefixes.Length == 0)
            return false;
            
        // Check if the access code starts with any valid prefix
        return validAccessCodePrefixes.Any(prefix => accessCode.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary>
    /// Get a compatible faction for generating consistent encounters
    /// </summary>
    public string GetPrimaryFaction()
    {
        if (associatedFactions != null && associatedFactions.Length > 0)
            return associatedFactions[0];
            
        return categoryName.ToLower();
    }
}