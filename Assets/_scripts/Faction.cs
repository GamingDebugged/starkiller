using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject that defines a faction in Starkiller Base Command
/// This is the single source of truth for faction data
/// </summary>
[CreateAssetMenu(fileName = "New Faction", menuName = "Starkiller Base/Faction")]
public class Faction : ScriptableObject
{
    [Header("Basic Information")]
    [Tooltip("Internal ID used in code (e.g., 'imperium', 'the_order')")]
    public string factionID;
    
    [Tooltip("Display name shown to players (e.g., 'The Imperium', 'The Order')")]
    public string displayName;
    
    [TextArea(3, 5)]
    [Tooltip("Description of this faction")]
    public string description;
    
    [Header("Visual Identity")]
    [Tooltip("Primary color associated with this faction")]
    public Color primaryColor = Color.white;
    
    [Tooltip("Icon representing this faction")]
    public Sprite factionIcon;
    
    [Tooltip("Banner or flag image")]
    public Sprite factionBanner;
    
    [Header("Access Codes")]
    [Tooltip("Access code prefixes this faction can use (e.g., 'MIL', 'IMP')")]
    public string[] validAccessCodePrefixes;
    
    [Tooltip("Can this faction use emergency codes?")]
    public bool canUseEmergencyCodes = false;
    
    [Header("Relationships")]
    [Tooltip("Factions that are allied with this one")]
    public Faction[] alliedFactions;
    
    [Tooltip("Factions that are enemies of this one")]
    public Faction[] enemyFactions;
    
    [Tooltip("Factions that have neutral relations")]
    public Faction[] neutralFactions;
    
    [Header("Ship Categories")]
    [Tooltip("Ship categories this faction commonly uses")]
    public ShipCategory[] authorizedShipCategories;
    
    [Tooltip("Ship categories this faction NEVER uses")]
    public ShipCategory[] forbiddenShipCategories;
    
    [Header("Behavioral Traits")]
    [Range(0f, 1f)]
    [Tooltip("How likely this faction is to follow proper protocols (0 = chaotic, 1 = lawful)")]
    public float protocolAdherence = 0.5f;
    
    [Range(0f, 1f)]
    [Tooltip("How likely this faction is to carry contraband (0 = never, 1 = always)")]
    public float contrabandProbability = 0.1f;
    
    [Range(0f, 1f)]
    [Tooltip("Base suspicion level for this faction (0 = trusted, 1 = highly suspicious)")]
    public float baseSuspicionLevel = 0.2f;
    
    [Header("Special Properties")]
    [Tooltip("Does this faction have diplomatic immunity?")]
    public bool hasDiplomaticImmunity = false;
    
    [Tooltip("Is this faction exempt from random searches?")]
    public bool exemptFromSearches = false;
    
    [Tooltip("Does this faction get priority access?")]
    public bool hasPriorityAccess = false;
    
    [Tooltip("Can members of this faction be infiltrators?")]
    public bool canBeInfiltrated = true;
    
    [Header("Common Attributes")]
    [Tooltip("Common origins for ships of this faction")]
    public string[] commonOrigins;
    
    [Tooltip("Common destinations for ships of this faction")]
    public string[] commonDestinations;
    
    [Tooltip("Typical cargo types carried by this faction")]
    public string[] typicalCargo;
    
    [Tooltip("Documents this faction typically carries")]
    public string[] standardDocuments;

    [Tooltip("Faction's primary color")]
    public Color factionColor = Color.white;
    
    /// <summary>
    /// Check if this faction is allied with another
    /// </summary>
    public bool IsAlliedWith(Faction other)
    {
        if (other == null) return false;
        if (other == this) return true; // Always allied with self

        if (alliedFactions != null)
        {
            foreach (var ally in alliedFactions)
            {
                if (ally == other) return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// Check if this faction is hostile to another
    /// </summary>
    public bool IsHostileTo(Faction other)
    {
        if (other == null) return false;
        if (other == this) return false; // Never hostile to self
        
        if (enemyFactions != null)
        {
            foreach (var enemy in enemyFactions)
            {
                if (enemy == other) return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Get the relationship status with another faction
    /// </summary>
    public FactionRelationship GetRelationshipWith(Faction other)
    {
        if (other == null) return FactionRelationship.Unknown;
        if (other == this) return FactionRelationship.Self;
        
        if (IsAlliedWith(other)) return FactionRelationship.Allied;
        if (IsHostileTo(other)) return FactionRelationship.Hostile;
        
        // Check if explicitly neutral
        if (neutralFactions != null)
        {
            foreach (var neutral in neutralFactions)
            {
                if (neutral == other) return FactionRelationship.Neutral;
            }
        }
        
        // Default to neutral if not specified
        return FactionRelationship.Neutral;
    }
    
    /// <summary>
    /// Check if this faction can use a specific access code prefix
    /// </summary>
    public bool CanUseAccessCodePrefix(string prefix)
    {
        if (string.IsNullOrEmpty(prefix)) return false;
        
        if (validAccessCodePrefixes != null)
        {
            foreach (var validPrefix in validAccessCodePrefixes)
            {
                if (prefix.Equals(validPrefix, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if this faction can use a specific ship category
    /// </summary>
    public bool CanUseShipCategory(ShipCategory category)
    {
        if (category == null) return false;
        
        // First check if it's forbidden
        if (forbiddenShipCategories != null)
        {
            foreach (var forbidden in forbiddenShipCategories)
            {
                if (forbidden == category) return false;
            }
        }
        
        // Then check if it's authorized
        if (authorizedShipCategories != null)
        {
            foreach (var authorized in authorizedShipCategories)
            {
                if (authorized == category) return true;
            }
        }
        
        // If not explicitly authorized or forbidden, check the category's faction associations
        return category.IsFactionAssociated(factionID);
    }
    
    /// <summary>
    /// Get a random origin appropriate for this faction
    /// </summary>
    public string GetRandomOrigin()
    {
        if (commonOrigins == null || commonOrigins.Length == 0)
            return "Unknown Origin";
            
        return commonOrigins[Random.Range(0, commonOrigins.Length)];
    }
    
    /// <summary>
    /// Get a random destination appropriate for this faction
    /// </summary>
    public string GetRandomDestination()
    {
        if (commonDestinations == null || commonDestinations.Length == 0)
            return "Classified Destination";
            
        return commonDestinations[Random.Range(0, commonDestinations.Length)];
    }
    
    /// <summary>
    /// Get display information for debugging
    /// </summary>
    public override string ToString()
    {
        return $"{displayName} ({factionID})";
    }
}

/// <summary>
/// Enum defining faction relationships
/// </summary>
public enum FactionRelationship
{
    Unknown,
    Self,
    Allied,
    Neutral,
    Hostile
}