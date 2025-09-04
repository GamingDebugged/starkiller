using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;

/// <summary>
/// ScriptableObject for captain types in Starkiller Base Command
/// These define the personalities and behaviors of ship captains
/// </summary>
[CreateAssetMenu(fileName = "New Captain Type", menuName = "Starkiller Base/Captain Type")]
public class CaptainType : ScriptableObject
{
    [Header("Basic Information")]
    public string typeName;
    public string[] factions;
    
    [Header("Faction Objects (Optional)")]
    [Tooltip("New faction object references - use alongside string factions")]
    public Faction[] factionObjects;
    
    [System.Serializable]
    public class Captain
    {
        [Header("Identity")]
        public string firstName;
        public string lastName;
        public string rank;
        public int authorityLevel; // 1-10 scale of influence/power
        
        [Header("Appearance")]
        public Sprite portrait;
        
        [Header("Behavior")]
        [Range(0, 1)]
        public float briberyChance = 0.2f;
        public int minBribeAmount = 10;
        public int maxBribeAmount = 30;
        [Range(0, 1)]
        public float deceptionChance = 0.1f;
        
        [System.Serializable]
        public class DialogEntry
        {
            [TextArea(1, 3)]
            public string phrase;
            public VideoClip associatedVideo;
        }
        [Header("Dialog Options")]
        public List<DialogEntry> greetingDialogs = new List<DialogEntry>();
        public List<DialogEntry> approvedDialogs = new List<DialogEntry>();
        public List<DialogEntry> deniedDialogs = new List<DialogEntry>();
        public List<DialogEntry> bribeDialogs = new List<DialogEntry>();
        public List<DialogEntry> holdingPatternDialogs = new List<DialogEntry>();
        public List<DialogEntry> tractorBeamDialogs = new List<DialogEntry>();
        
        [Header("Relationship-Based Dialog")]
        [Tooltip("Dialog when returning after previous approval")]
        public List<DialogEntry> returningAfterApprovalDialogs = new List<DialogEntry>();
        [Tooltip("Dialog when returning after previous denial")]
        public List<DialogEntry> returningAfterDenialDialogs = new List<DialogEntry>();
        [Tooltip("Dialog when returning after being placed in holding pattern")]
        public List<DialogEntry> returningAfterHoldingDialogs = new List<DialogEntry>();
        [Tooltip("Dialog when returning after tractor beam was used")]
        public List<DialogEntry> returningAfterTractorBeamDialogs = new List<DialogEntry>();
        [Tooltip("Dialog when returning after bribery was accepted")]
        public List<DialogEntry> returningAfterBriberyDialogs = new List<DialogEntry>();
        
        // Helper methods
        public string GetFullName()
        {
            return $"{rank} {firstName} {lastName}".Trim();
        }
        
        public DialogEntry GetRandomGreeting()
        {
            if (greetingDialogs == null || greetingDialogs.Count == 0)
                return null;
                
            return greetingDialogs[Random.Range(0, greetingDialogs.Count)];
        }
        
        public DialogEntry GetRandomApprovalResponse()
        {
            if (approvedDialogs == null || approvedDialogs.Count == 0)
                return null;
                
            return approvedDialogs[Random.Range(0, approvedDialogs.Count)];
        }
        
        public DialogEntry GetRandomDenialResponse()
        {
            if (deniedDialogs == null || deniedDialogs.Count == 0)
                return null;
                
            return deniedDialogs[Random.Range(0, deniedDialogs.Count)];
        }
        
        public DialogEntry GetRandomBriberyPhrase()
        {
            if (bribeDialogs == null || bribeDialogs.Count == 0)
                return null;
                
            return bribeDialogs[Random.Range(0, bribeDialogs.Count)];
        }
        
        public DialogEntry GetRandomHoldingPatternResponse()
        {
            if (holdingPatternDialogs == null || holdingPatternDialogs.Count == 0)
                return null;
                
            return holdingPatternDialogs[Random.Range(0, holdingPatternDialogs.Count)];
        }
        
        public DialogEntry GetRandomTractorBeamResponse()
        {
            if (tractorBeamDialogs == null || tractorBeamDialogs.Count == 0)
                return null;
                
            return tractorBeamDialogs[Random.Range(0, tractorBeamDialogs.Count)];
        }
        
        // Relationship-based dialog methods
        public DialogEntry GetReturningAfterApprovalDialog()
        {
            if (returningAfterApprovalDialogs == null || returningAfterApprovalDialogs.Count == 0)
                return GetRandomGreeting(); // Fallback to greeting
                
            return returningAfterApprovalDialogs[Random.Range(0, returningAfterApprovalDialogs.Count)];
        }
        
        public DialogEntry GetReturningAfterDenialDialog()
        {
            if (returningAfterDenialDialogs == null || returningAfterDenialDialogs.Count == 0)
                return GetRandomGreeting(); // Fallback to greeting
                
            return returningAfterDenialDialogs[Random.Range(0, returningAfterDenialDialogs.Count)];
        }
        
        public DialogEntry GetReturningAfterHoldingDialog()
        {
            if (returningAfterHoldingDialogs == null || returningAfterHoldingDialogs.Count == 0)
                return GetRandomGreeting(); // Fallback to greeting
                
            return returningAfterHoldingDialogs[Random.Range(0, returningAfterHoldingDialogs.Count)];
        }
        
        public DialogEntry GetReturningAfterTractorBeamDialog()
        {
            if (returningAfterTractorBeamDialogs == null || returningAfterTractorBeamDialogs.Count == 0)
                return GetRandomGreeting(); // Fallback to greeting
                
            return returningAfterTractorBeamDialogs[Random.Range(0, returningAfterTractorBeamDialogs.Count)];
        }
        
        public DialogEntry GetReturningAfterBriberyDialog()
        {
            if (returningAfterBriberyDialogs == null || returningAfterBriberyDialogs.Count == 0)
                return GetRandomGreeting(); // Fallback to greeting
                
            return returningAfterBriberyDialogs[Random.Range(0, returningAfterBriberyDialogs.Count)];
        }
        
        /// <summary>
        /// Get dialog based on the captain's relationship with the player
        /// </summary>
        public DialogEntry GetDialogForRelationship(CaptainRelationship relationship, PlayerDecision lastDecision)
        {
            // If this is a returning captain, use relationship-specific dialog
            if (relationship != CaptainRelationship.FirstMeeting)
            {
                switch (lastDecision)
                {
                    case PlayerDecision.Approved:
                        return GetReturningAfterApprovalDialog();
                    case PlayerDecision.Denied:
                        return GetReturningAfterDenialDialog();
                    case PlayerDecision.HoldingPattern:
                        return GetReturningAfterHoldingDialog();
                    case PlayerDecision.TractorBeam:
                        return GetReturningAfterTractorBeamDialog();
                    case PlayerDecision.BriberyAccepted:
                        return GetReturningAfterBriberyDialog();
                    default:
                        return GetRandomGreeting();
                }
            }
            
            // First meeting - use standard greeting
            return GetRandomGreeting();
        }
    }
    
    [Header("Captain Roster")]
    public List<Captain> captains = new List<Captain>();
    
    [Header("Name Generation")]
    public string[] possibleFirstNames;
    public string[] possibleLastNames;
    public string[] commonRanks;
    
    [Header("Behavior Patterns")]
    [TextArea(2, 4)]
    public string[] typicalBehaviors;
    [TextArea(2, 4)]
    public string[] dialoguePatterns;
    
    /// <summary>
    /// Check if this captain type belongs to a specific faction (checks both strings and objects)
    /// </summary>
    public bool BelongsToFaction(string factionIdentifier)
    {
        if (string.IsNullOrEmpty(factionIdentifier)) return false;
        
        string lowerIdentifier = factionIdentifier.ToLower();
        
        // Check string factions first (original system)
        if (factions != null)
        {
            foreach (var faction in factions)
            {
                if (!string.IsNullOrEmpty(faction) && faction.ToLower() == lowerIdentifier)
                    return true;
            }
        }
        
        // Then check faction objects if available
        if (factionObjects != null)
        {
            foreach (var faction in factionObjects)
            {
                if (faction != null && 
                    (faction.factionID.ToLower() == lowerIdentifier || 
                     faction.displayName.ToLower() == lowerIdentifier))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if this captain type belongs to a specific faction object
    /// </summary>
    public bool BelongsToFaction(Faction faction)
    {
        if (faction == null) return false;
        
        // Check faction objects
        if (factionObjects != null && factionObjects.Contains(faction))
            return true;
            
        // Also check string factions for compatibility
        if (factions != null)
        {
            string factionIdLower = faction.factionID.ToLower();
            string displayNameLower = faction.displayName.ToLower();
            
            foreach (var factionString in factions)
            {
                if (!string.IsNullOrEmpty(factionString))
                {
                    string lowerString = factionString.ToLower();
                    if (lowerString == factionIdLower || lowerString == displayNameLower)
                        return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Get all factions (combining both string and object systems)
    /// </summary>
    public List<string> GetAllFactionIdentifiers()
    {
        HashSet<string> allFactions = new HashSet<string>();
        
        // Add string factions
        if (factions != null)
        {
            foreach (var faction in factions)
            {
                if (!string.IsNullOrEmpty(faction))
                    allFactions.Add(faction);
            }
        }
        
        // Add faction object IDs
        if (factionObjects != null)
        {
            foreach (var faction in factionObjects)
            {
                if (faction != null && !string.IsNullOrEmpty(faction.factionID))
                    allFactions.Add(faction.factionID);
            }
        }
        
        return allFactions.ToList();
    }
    
    /// <summary>
    /// Get the primary faction (preferring objects over strings)
    /// </summary>
    public Faction GetPrimaryFactionObject()
    {
        if (factionObjects != null && factionObjects.Length > 0 && factionObjects[0] != null)
            return factionObjects[0];
            
        // Try to convert string faction to object if FactionManager is available
        if (factions != null && factions.Length > 0 && !string.IsNullOrEmpty(factions[0]))
        {
            if (FactionManager.Instance != null)
                return FactionManager.Instance.GetFaction(factions[0]);
        }
        
        return null;
    }
    
    /// <summary>
    /// Sync faction objects with faction strings (editor helper)
    /// </summary>
    [ContextMenu("Sync Faction Objects from Strings")]
    public void SyncFactionObjectsFromStrings()
    {
        if (factions == null || factions.Length == 0)
        {
            Debug.LogWarning("No string factions to sync from.");
            return;
        }
        
        if (FactionManager.Instance == null)
        {
            Debug.LogError("FactionManager not found in scene. Please add it first.");
            return;
        }
        
        List<Faction> syncedFactions = new List<Faction>();
        
        foreach (string factionString in factions)
        {
            if (!string.IsNullOrEmpty(factionString))
            {
                Faction faction = FactionManager.Instance.GetFaction(factionString);
                if (faction != null && !syncedFactions.Contains(faction))
                {
                    syncedFactions.Add(faction);
                    Debug.Log($"Synced faction '{factionString}' to {faction.displayName}");
                }
                else if (faction == null)
                {
                    Debug.LogWarning($"Could not find faction object for '{factionString}'");
                }
            }
        }
        
        factionObjects = syncedFactions.ToArray();
        Debug.Log($"Sync complete. Added {factionObjects.Length} faction objects.");
    }
    
    /// <summary>
    /// Get a random captain from this type
    /// </summary>
    public Captain GetRandomCaptain()
    {
        if (captains == null || captains.Count == 0)
            return GenerateRandomCaptain();
            
        return captains[Random.Range(0, captains.Count)];
    }
    
    /// <summary>
    /// Get a captain with a specific authority level (or closest available)
    /// </summary>
    public Captain GetCaptainByAuthorityLevel(int authorityLevel)
    {
        if (captains == null || captains.Count == 0)
            return GenerateRandomCaptain();
            
        // Find closest match to desired authority level
        Captain closestMatch = captains[0];
        int smallestDifference = Mathf.Abs(authorityLevel - closestMatch.authorityLevel);
        
        foreach (var captain in captains)
        {
            int difference = Mathf.Abs(authorityLevel - captain.authorityLevel);
            if (difference < smallestDifference)
            {
                closestMatch = captain;
                smallestDifference = difference;
            }
        }
        
        return closestMatch;
    }
    
    /// <summary>
    /// Generate a procedural captain if none are explicitly defined
    /// </summary>
    private Captain GenerateRandomCaptain()
    {
        Captain newCaptain = new Captain();
        
        // Generate basic name if no other data is available
        if (possibleFirstNames == null || possibleFirstNames.Length == 0 || 
            possibleLastNames == null || possibleLastNames.Length == 0)
        {
            newCaptain.firstName = "Captain";
            newCaptain.lastName = Random.Range(1000, 9999).ToString();
        }
        else
        {
            newCaptain.firstName = possibleFirstNames[Random.Range(0, possibleFirstNames.Length)];
            newCaptain.lastName = possibleLastNames[Random.Range(0, possibleLastNames.Length)];
        }
        
        // Select a random rank
        if (commonRanks != null && commonRanks.Length > 0)
        {
            newCaptain.rank = commonRanks[Random.Range(0, commonRanks.Length)];
        }
        else
        {
            newCaptain.rank = "Captain";
        }
        
        // Set default behavior values
        newCaptain.authorityLevel = Random.Range(1, 11); // 1-10 scale
        newCaptain.briberyChance = Random.Range(0f, 0.3f);
        newCaptain.minBribeAmount = Random.Range(10, 21);
        newCaptain.maxBribeAmount = Random.Range(30, 51);
        newCaptain.deceptionChance = Random.Range(0f, 0.2f);
        
        return newCaptain;
    }
    
    /// <summary>
    /// Legacy method for backwards compatibility
    /// </summary>
    public string GenerateRandomName()
    {
        return GetRandomCaptain().GetFullName();
    }
    
    #region Legacy Compatibility Properties
    
    // These properties provide backward compatibility with code that directly accessed the old fields
    
    [HideInInspector]
    public float briberyChance 
    {
        get 
        {
            // Return the bribery chance from the first captain, or the old default value
            if (captains != null && captains.Count > 0) 
                return captains[0].briberyChance;
            return 0.2f;
        }
        set 
        {
            // Apply to all captains if needed
            if (captains != null)
                foreach (var captain in captains)
                    captain.briberyChance = value;
        }
    }
    
    [HideInInspector]
    public int minBribeAmount 
    {
        get 
        {
            if (captains != null && captains.Count > 0) 
                return captains[0].minBribeAmount;
            return 10;
        }
        set 
        {
            if (captains != null)
                foreach (var captain in captains)
                    captain.minBribeAmount = value;
        }
    }
    
    [HideInInspector]
    public int maxBribeAmount 
    {
        get 
        {
            if (captains != null && captains.Count > 0) 
                return captains[0].maxBribeAmount;
            return 30;
        }
        set 
        {
            if (captains != null)
                foreach (var captain in captains)
                    captain.maxBribeAmount = value;
        }
    }
    
    [HideInInspector]
    public Sprite[] portraitOptions
    {
        get
        {
            if (captains != null && captains.Count > 0)
            {
                // Collect all portraits from captains
                Sprite[] portraits = new Sprite[captains.Count];
                for (int i = 0; i < captains.Count; i++)
                {
                    portraits[i] = captains[i].portrait;
                }
                return portraits;
            }
            return new Sprite[0];
        }
    }
    
    // Support for legacy bribery phrases (now maps to bribeDialogs)
    [HideInInspector]
    public string[] briberyPhrases
    {
        get
        {
            if (captains != null && captains.Count > 0 && captains[0].bribeDialogs.Count > 0)
            {
                string[] phrases = new string[captains[0].bribeDialogs.Count];
                for (int i = 0; i < captains[0].bribeDialogs.Count; i++)
                {
                    phrases[i] = captains[0].bribeDialogs[i].phrase;
                }
                return phrases;
            }
            return new string[0];
        }
        set
        {
            if (value != null && value.Length > 0 && captains != null && captains.Count > 0)
            {
                // Apply phrases to all captains
                foreach (var captain in captains)
                {
                    captain.bribeDialogs = new List<Captain.DialogEntry>();
                    foreach (var phrase in value)
                    {
                        captain.bribeDialogs.Add(new Captain.DialogEntry { phrase = phrase });
                    }
                }
            }
        }
    }
    
    // Legacy property for backward compatibility with old dialog naming
    [HideInInspector]
    public List<Captain.DialogEntry> briberyDialogs
    {
        get
        {
            if (captains != null && captains.Count > 0)
                return captains[0].bribeDialogs;
            return new List<Captain.DialogEntry>();
        }
        set
        {
            if (captains != null && captains.Count > 0)
                captains[0].bribeDialogs = value;
        }
    }
    
    // Legacy property for backward compatibility with old dialog naming
    [HideInInspector]
    public List<Captain.DialogEntry> denialDialogs
    {
        get
        {
            if (captains != null && captains.Count > 0)
                return captains[0].deniedDialogs;
            return new List<Captain.DialogEntry>();
        }
        set
        {
            if (captains != null && captains.Count > 0)
                captains[0].deniedDialogs = value;
        }
    }
    
    #endregion
}

/// <summary>
/// Represents the relationship state between a captain and the player
/// </summary>
public enum CaptainRelationship
{
    FirstMeeting,   // Captain has never appeared before
    Friendly,       // Previous interactions were positive (approved, bribed)
    Neutral,        // Mixed or minimal previous interactions
    Hostile         // Previous interactions were negative (denied, held, tractor beam)
}

/// <summary>
/// Represents the player's decision on a captain's previous encounter
/// </summary>
public enum PlayerDecision
{
    None,
    Approved,
    Denied,
    HoldingPattern,
    TractorBeam,
    BriberyAccepted
}