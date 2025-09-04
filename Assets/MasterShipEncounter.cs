using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using StarkillerBaseCommand;
using System.Text;

/// <summary>
/// Comprehensive ship encounter class for Starkiller Base Command
/// Contains all necessary data, validation logic, and media references in a single class
/// </summary>
[System.Serializable]
public class MasterShipEncounter
{
    #region Ship Information
    // Basic ship information
    public string shipType;            // Type of ship
    public string shipName;            // Name of the specific ship
    public string destination;         // Where they're going
    public string origin;              // Where they're coming from
    public string accessCode;          // Their access code
    public string story;               // Their story/situation
    public string manifest;            // Cargo manifest (legacy string format)
    public CargoManifest manifestData; // Enhanced ScriptableObject manifest
    public int crewSize;               // Number of crew members
    public string faction;             // Ship's faction (for access code validation)
    #endregion

    #region Validation Data
    // Validation data
    public bool shouldApprove;         // Whether they should be approved
    public string invalidReason;       // Reason they should be denied (if applicable)
    
    // New: Reference to the AccessCode ScriptableObject
    [System.NonSerialized]
    public AccessCode accessCodeData;  // The actual AccessCode object if found
    #endregion

    #region Captain Information
    // Captain information
    public string captainName;         // Name of the ship captain
    public string captainRank;         // Rank of the captain
    public string captainFaction;      // Faction the captain belongs to
    #endregion

    #region Special Elements
    // Special elements
    public bool isStoryShip;           // Is this a special story ship
    public string storyTag;            // Tag for story type (insurgent, imperium, order)
    #endregion

    #region Bribe Information
    // Bribe information
    public bool offersBribe;           // Does this ship offer a bribe?
    public int bribeAmount;            // Amount of credits offered as bribe
    #endregion

    #region Consequence Information
    // Consequence information
    public string consequenceDescription; // What happens if wrong decision is made
    public int casualtiesIfWrong;      // Number of casualties if wrong decision made
    public int creditPenalty;          // Credits lost if wrong decision made
    #endregion
    
    #region Player Decision Tracking
    // Track player's decision
    public enum DecisionState { None, Approved, Denied, HoldingPattern, TractorBeam, BriberyAccepted }
    
    [System.NonSerialized]
    public DecisionState playerDecision = DecisionState.None;
    
    // Track if this is a special ship that can be captured
    public bool canBeCaptured = false;  // Whether this ship can be captured with tractor beam
    
    [System.NonSerialized]
    public bool isInHoldingPattern = false; // Whether ship is in holding pattern
    
    public float holdingPatternTime = 60f; // Default holding time in seconds
    #endregion

    #region Media Content
    // Images
    public Sprite shipImage;           // Visual representation of the ship
    public Sprite captainPortrait;     // Portrait of the captain
    
    // Videos
    public VideoClip shipVideo;        // Video clip of the ship
    public VideoClip captainVideo;     // Video clip of the captain
    public VideoClip scenarioVideo;    // Video clip for the scenario
    #endregion

    #region Reference to Source Objects
    // Reference to original scriptable objects
    [System.NonSerialized]
    public ShipType shipTypeData;
    [System.NonSerialized]
    public CaptainType captainTypeData;
    [System.NonSerialized]
    public ShipScenario scenarioData;
    
    // Store the actual captain that was selected
    [System.NonSerialized]
    public CaptainType.Captain selectedCaptain;
    #endregion

    #region Media Check Methods
    // Helper methods to check for available media
    public bool HasShipImage() => shipImage != null;
    public bool HasCaptainPortrait() => captainPortrait != null;
    public bool HasShipVideo() => shipVideo != null;
    public bool HasCaptainVideo() => captainVideo != null;
    public bool HasScenarioVideo() => scenarioVideo != null;
    #endregion

    #region Display Methods
    /// <summary>
    /// Get formatted ship info for display in the UI
    /// </summary>
    public string GetShipInfo()
    {
        StringBuilder info = new StringBuilder();
        
        // Add ship type (bold)
        info.Append("<b>").Append(shipName).Append("</b>   requesting access\n");
        
        // Add captain if available
        if (!string.IsNullOrEmpty(captainName))
        {
            info.Append("Captain: ");
            if (!string.IsNullOrEmpty(captainRank))
                info.Append(captainRank).Append(" ");
            info.Append(captainName).Append("\n");
        }
        
        // Add faction information
        if (!string.IsNullOrEmpty(faction))
        {
            info.Append("Faction: ").Append(faction).Append("\n");
        }
        
        // Add story (italics)
        info.Append("\n<i>\"").Append(story).Append("\"</i>\n\n");
        
        // Add basic ship details
        info.Append("Destination: ").Append(destination).Append("\n");
        info.Append("Origin: ").Append(origin).Append("\n");
        info.Append("Crew Size: ").Append(crewSize);
        
        return info.ToString();
    }
    
    /// <summary>
    /// Get the appropriate dialog entry based on the current encounter context
    /// </summary>
    public CaptainType.Captain.DialogEntry GetCurrentCaptainDialog()
    {
        if (HasCaptainVideo() && captainTypeData != null)
        {
            // Find the specific captain by name
            CaptainType.Captain matchingCaptain = captainTypeData.captains.Find(c => 
                c.GetFullName() == captainName);

            if (matchingCaptain != null)
            {
                // Determine which dialog to return based on context
                switch (playerDecision)
                {
                    case DecisionState.None:
                        return matchingCaptain.GetRandomGreeting();
                    case DecisionState.Denied:
                        return matchingCaptain.GetRandomDenialResponse();
                    case DecisionState.BriberyAccepted:
                        return matchingCaptain.GetRandomBriberyPhrase();
                    default:
                        return matchingCaptain.GetRandomGreeting();
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Get formatted credentials info for display in the UI
    /// </summary>
    public string GetCredentialsInfo()
    {
        StringBuilder credentials = new StringBuilder();
        
        // Access code (bold)
        credentials.Append("<b>Access Code:</b> ").Append(accessCode);
        
        // Add code type and level if we have the AccessCode data
        if (accessCodeData != null)
        {
            credentials.Append(" (").Append(accessCodeData.type.ToString());
            if (accessCodeData.level != AccessCode.AccessLevel.Low)
            {
                credentials.Append(", Level ").Append(accessCodeData.level.ToString());
            }
            credentials.Append(")");
        }
        
        credentials.Append("\n\n");
        
        // Manifest (bold header)
        credentials.Append("<b>Manifest:</b> ").Append(GetManifestDisplay());
        
        // Add bribe information if applicable (yellow)
        if (offersBribe)
        {
            credentials.Append("\n\n<color=yellow>[Captain offers ")
                      .Append(bribeAmount)
                      .Append(" credits for quick approval]</color>");
        }
        
        return credentials.ToString();
    }
    
    /// <summary>
    /// Get approval recommendation based on current day rules
    /// </summary>
    public string GetApprovalRecommendation(List<string> validAccessCodes, List<StarkkillerContentManager.DayRule> currentRules)
    {
        StringBuilder recommendation = new StringBuilder();
        List<string> issues = new List<string>();
        
        // Check access code validity
        if (!validAccessCodes.Contains(accessCode))
        {
            issues.Add("- Invalid access code");
        }
        
        // Check faction authorization if we have AccessCode data
        if (accessCodeData != null && !string.IsNullOrEmpty(faction))
        {
            bool factionAuthorized = false;
            if (accessCodeData.authorizedFactions != null)
            {
                foreach (string authFaction in accessCodeData.authorizedFactions)
                {
                    if (authFaction.Equals(faction, System.StringComparison.OrdinalIgnoreCase))
                    {
                        factionAuthorized = true;
                        break;
                    }
                }
            }
            
            if (!factionAuthorized)
            {
                issues.Add($"- {faction} faction not authorized for code {accessCode}");
            }
        }
        
        // Check origin based on daily rules
        bool hasOriginRule = currentRules.Exists(r => 
            r.ruleType == StarkkillerContentManager.DayRule.RuleType.VerifyOrigin || 
            r.ruleDescription.ToLower().Contains("origin"));
            
        if (hasOriginRule && !IsOriginValidForDestination(origin, destination))
        {
            issues.Add("- Origin not authorized for this destination");
        }
        
        // Check for contraband based on daily rules
        bool hasContrabandRule = currentRules.Exists(r => 
            r.ruleType == StarkkillerContentManager.DayRule.RuleType.CheckForContraband || 
            r.ruleDescription.ToLower().Contains("contraband"));
            
        if (hasContrabandRule && CheckForContraband())
        {
            issues.Add("- Suspected contraband in manifest");
        }
        
        // Add any other day-specific rules checks here
        
        // Return overall recommendation
        if (issues.Count == 0)
        {
            return "All credentials appear valid.";
        }
        else
        {
            recommendation.Append("The following issues were found:\n");
            foreach (string issue in issues)
            {
                recommendation.Append(issue).Append("\n");
            }
            return recommendation.ToString();
        }
    }
    #endregion

    #region Validation Methods
    /// <summary>
    /// Checks if the access code has an invalid prefix like OLD-, XX-, etc.
    /// </summary>
    public bool HasInvalidAccessCodePrefix()
    {
        if (string.IsNullOrEmpty(accessCode))
            return false;
            
        string[] invalidPrefixes = {"XX-", "OLD-", "REJ-", "TMP-", "ERR-"};
        
        foreach (string prefix in invalidPrefixes)
        {
            if (accessCode.StartsWith(prefix))
                return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Checks if the access code looks like a red herring (similar to valid code but wrong)
    /// </summary>
    public bool IsRedHerringCode()
    {
        if (string.IsNullOrEmpty(accessCode))
            return false;
            
        // Check for common red herring patterns
        string[] redHerringPatterns = {
            "M1L-", "8NT-", "TRO-", "V1P-", "ClV-", "ENG-", "5PL-", "1MP-", "C1V-"
        };
        
        foreach (string pattern in redHerringPatterns)
        {
            if (accessCode.StartsWith(pattern))
                return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Checks if origin is valid for destination
    /// </summary>
    private bool IsOriginValidForDestination(string origin, string destination)
    {
        // Basic validation logic - could be expanded with more complex rules
        // Typically this would check against a list of valid origins for each destination
        
        // For now, use a simple logic placeholder
        if (shipTypeData != null && shipTypeData.commonOrigins != null)
        {
            foreach (string validOrigin in shipTypeData.commonOrigins)
            {
                if (validOrigin == origin)
                    return true;
            }
            return false;
        }
        
        // Fallback logic: if we can't find specific rules, check for name patterns
        return origin.Contains(destination) || destination.Contains(origin);
    }
    
    /// <summary>
    /// Get the manifest display text (backwards compatible)
    /// </summary>
    public string GetManifestDisplay()
    {
        // Prefer ScriptableObject manifest if available
        if (manifestData != null)
        {
            return manifestData.GetDisplayText();
        }
        
        // Fallback to legacy string manifest
        return manifest ?? "No manifest available";
    }
    
    /// <summary>
    /// Check if this encounter has manifest data (either format)
    /// </summary>
    public bool HasManifestData()
    {
        return manifestData != null || !string.IsNullOrEmpty(manifest);
    }
    
    /// <summary>
    /// Check for contraband using both new and legacy systems
    /// </summary>
    public bool CheckForContraband()
    {
        // First check new ScriptableObject manifest
        if (manifestData != null)
        {
            return manifestData.hasContraband;
        }
        
        // Fallback to legacy string-based contraband detection
        return ContainsContraband(manifest);
    }
    
    /// <summary>
    /// Checks if the manifest contains contraband based on common contraband keywords (legacy method)
    /// </summary>
    private bool ContainsContraband(string manifest)
    {
        if (string.IsNullOrEmpty(manifest))
            return false;
            
        string[] contrabandTerms = new string[]
        {
            "weapons", "blaster", "explosive", "restricted", "banned",
            "smuggled", "illegal", "contraband", "Jazz", "undeclared"
        };
        
        string lowerManifest = manifest.ToLower();
        
        foreach (string term in contrabandTerms)
        {
            if (lowerManifest.Contains(term.ToLower()))
                return true;
        }
        
        return false;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    /// Create a master ship encounter from scriptable objects
    /// </summary>
    public static MasterShipEncounter CreateFromScriptableObjects(
        ShipType shipType, 
        CaptainType captainType, 
        ShipScenario scenario, 
        bool shouldBeValid,
        List<string> validAccessCodes,
        StarkkillerContentManager contentManager = null)
    {
        // Validate parameters
        if (shipType == null || captainType == null || scenario == null)
        {
            Debug.LogError($"MasterShipEncounter.CreateFromScriptableObjects: Null parameter found! shipType: {shipType != null}, captainType: {captainType != null}, scenario: {scenario != null}");
            return CreateTestEncounter(); // Use test encounter as fallback
        }
        
        MasterShipEncounter encounter = new MasterShipEncounter();
        
        // Set references to original scriptable objects
        encounter.shipTypeData = shipType;
        encounter.captainTypeData = captainType;
        encounter.scenarioData = scenario;
        
        // Basic ship information
        encounter.shipType = shipType.typeName;
        encounter.crewSize = Random.Range(shipType.minCrewSize, shipType.maxCrewSize + 1);
        encounter.destination = "Starkiller Base"; // Updated destination
        
        // Set faction from ship category or captain
        if (shipType.category != null)
        {
            encounter.faction = shipType.category.categoryName;
        }
        else if (captainType.factions != null && captainType.factions.Length > 0)
        {
            encounter.faction = captainType.factions[0];
        }
        else
        {
            encounter.faction = "Unknown";
        }

        // Get the ship name - Generate descriptive names using shipType + system names
        // This creates names like "Ferry - Control System", "Battleship - Military One"
        string[] systemNames = {
            "Control System", "Military One", "Defense Grid", "Command Center", 
            "Security Hub", "Operations Base", "Tactical Unit", "Strategic Point",
            "Guardian Station", "Sentinel Post", "Watch Tower", "Alert System"
        };
        
        // Use descriptive naming pattern: ShipType - SystemName
        string systemName = systemNames[Random.Range(0, systemNames.Length)];
        encounter.shipName = $"{shipType.typeName} - {systemName}";
        
        // Fallback: If we want to occasionally use specific ship names from the ShipType,
        // we can do so 30% of the time for variety
        if (shipType.specificShipNames != null && shipType.specificShipNames.Count > 0 && Random.value < 0.3f)
        {
            string specificName = shipType.specificShipNames[Random.Range(0, shipType.specificShipNames.Count)];
            encounter.shipName = $"{shipType.typeName} - {specificName}";
        }
        
        // Ensure common origins aren't null
        string[] commonOrigins = shipType.commonOrigins ?? new string[0];
        
        // Origin based on ship type and validity
        if (shouldBeValid && commonOrigins.Length > 0)
        {
            encounter.origin = commonOrigins[Random.Range(0, commonOrigins.Length)];
        }
        else if (!shouldBeValid)
        {
            // Sometimes use invalid origins for invalid ships
            if (Random.value > 0.6f)
            {
                string[] invalidOrigins = {"Unregistered System", "Outer Rim", "Unknown Space", "Restricted Sector"};
                encounter.origin = invalidOrigins[Random.Range(0, invalidOrigins.Length)];
            }
            else
            {
                // Or use valid-looking origin for a different challenge
                encounter.origin = commonOrigins.Length > 0 ? 
                    commonOrigins[Random.Range(0, commonOrigins.Length)] : 
                    "Central Fleet";
            }
        }
        else
        {
            encounter.origin = "Central Fleet";
        }
        
        // Access code generation with new AccessCode system
        if (shouldBeValid && validAccessCodes != null && validAccessCodes.Count > 0)
        {
            // For valid ships, pick a valid access code
            encounter.accessCode = validAccessCodes[Random.Range(0, validAccessCodes.Count)];
            
            // Try to get the AccessCode data if ContentManager is available
            if (contentManager != null)
            {
                encounter.accessCodeData = contentManager.GetAccessCodeData(encounter.accessCode);
                
                // Validate faction authorization
                if (encounter.accessCodeData != null && !contentManager.IsCodeAuthorizedForFaction(encounter.accessCode, encounter.faction))
                {
                    // For story purposes, insurgents might use stolen codes
                    if (scenario.storyTag == "insurgent" && Random.value > 0.5f)
                    {
                        // Keep the unauthorized code - this is a stolen code scenario
                        encounter.invalidReason = "Faction mismatch - possible stolen credentials";
                        encounter.shouldApprove = false;
                    }
                    else
                    {
                        // Try to find a code this faction IS authorized for
                        List<string> authorizedCodes = new List<string>();
                        foreach (string code in validAccessCodes)
                        {
                            if (contentManager.IsCodeAuthorizedForFaction(code, encounter.faction))
                            {
                                authorizedCodes.Add(code);
                            }
                        }
                        
                        if (authorizedCodes.Count > 0)
                        {
                            encounter.accessCode = authorizedCodes[Random.Range(0, authorizedCodes.Count)];
                            encounter.accessCodeData = contentManager.GetAccessCodeData(encounter.accessCode);
                        }
                    }
                }
            }
        }
        else
        {
            // For invalid ships, generate invalid or red herring codes
            if (Random.value > 0.5f)
            {
                // Generate red herring codes
                string[] redHerringCodes = {
                    "M1L-1001", "8NT-8001", "TRO-6001", "V1P-9001", 
                    "ClV-3001", "ENG-0999", "5PL-7777", "1MP-2001"
                };
                
                encounter.accessCode = redHerringCodes[Random.Range(0, redHerringCodes.Length)];
                encounter.invalidReason = "Invalid access code - suspected forgery";
            }
            else
            {
                // Generate obviously invalid codes
                string[] invalidPrefixes = {"XX-", "OLD-", "REJ-", "ERR-"};
                encounter.accessCode = invalidPrefixes[Random.Range(0, invalidPrefixes.Length)] + 
                                   Random.Range(1000, 10000).ToString();
                encounter.invalidReason = "Invalid access code prefix";
            }
        }
        
        // Story and manifest from scenario
        encounter.story = scenario.GetRandomStory();
        encounter.manifest = scenario.GetRandomManifest();
        
        // Try to assign a CargoManifest from ManifestManager if available
        if (ManifestManager.Instance != null)
        {
            encounter.manifestData = ManifestManager.Instance.SelectManifestForShip(
                shipType, 
                encounter.faction, 
                1 // Default to day 1, this should be passed from game state
            );
        }
        
        // Captain information - get a specific captain and store the reference
        CaptainType.Captain selectedCaptain = captainType.GetRandomCaptain();
        encounter.selectedCaptain = selectedCaptain;
        encounter.captainName = selectedCaptain.GetFullName();
        
        // Use the captain's rank if available, otherwise use a random common rank
        if (!string.IsNullOrEmpty(selectedCaptain.rank))
        {
            encounter.captainRank = selectedCaptain.rank;
        }
        else if (captainType.commonRanks.Length > 0)
        {
            encounter.captainRank = captainType.commonRanks[Random.Range(0, captainType.commonRanks.Length)];
        }
        
        if (captainType.factions.Length > 0)
        {
            encounter.captainFaction = captainType.factions[Random.Range(0, captainType.factions.Length)];
        }
        
        // ENHANCED CAPTAIN LOGGING - Critical for debugging captain response issues
        Debug.Log($"=== CAPTAIN SELECTED (DETAILED) ===");
        Debug.Log($"Captain Type: {captainType.typeName}");
        Debug.Log($"✅ selectedCaptain Reference: {(selectedCaptain != null ? "SET" : "NULL")}");
        if (selectedCaptain != null)
        {
            Debug.Log($"   - firstName: '{selectedCaptain.firstName}'");
            Debug.Log($"   - lastName: '{selectedCaptain.lastName}'");
            Debug.Log($"   - GetFullName(): '{selectedCaptain.GetFullName()}'");
            Debug.Log($"   - rank: '{selectedCaptain.rank}'");
        }
        Debug.Log($"✅ encounter.selectedCaptain: {(encounter.selectedCaptain != null ? "SET" : "NULL")}");
        Debug.Log($"Generated encounter.captainName: '{encounter.captainName}'");
        Debug.Log($"Generated encounter.captainRank: '{encounter.captainRank}'");
        Debug.Log($"Generated encounter.captainFaction: '{encounter.captainFaction}'");
        Debug.Log($"Bribery Chance: {captainType.briberyChance * 100f}%");
        Debug.Log($"===================================");
        
        // Validity information
        encounter.shouldApprove = shouldBeValid;
        if (!shouldBeValid && string.IsNullOrEmpty(encounter.invalidReason))
        {
            encounter.invalidReason = scenario.invalidReason;
        }
        
        // Story elements
        encounter.isStoryShip = scenario.isStoryMission;
        encounter.storyTag = scenario.storyTag;
        
        // Bribe information
        encounter.offersBribe = scenario.offersBribe && 
            Random.value < (captainType.briberyChance * scenario.bribeChanceMultiplier);
        if (encounter.offersBribe)
        {
            encounter.bribeAmount = Random.Range(captainType.minBribeAmount, captainType.maxBribeAmount + 1);
        }
        
        // Consequence information
        encounter.consequenceDescription = scenario.GetRandomConsequence(out int casualties);
        encounter.casualtiesIfWrong = casualties;
        encounter.creditPenalty = casualties * 5; // Simple formula, adjust as needed
        
        // Set visual elements from scriptable objects
        encounter.SetupMediaContent();
        
        // Log encounter summary
        Debug.Log($"=== ENCOUNTER CREATED ===");
        Debug.Log($"Ship: {encounter.shipType} \"{encounter.shipName}\" ({encounter.faction})");
        Debug.Log($"Captain: {encounter.captainRank} {encounter.captainName}");
        Debug.Log($"Scenario: {scenario.scenarioName} - Should Approve: {encounter.shouldApprove}");
        Debug.Log($"Access Code: {encounter.accessCode}");
        if (!encounter.shouldApprove) {
            Debug.Log($"Invalid Reason: {encounter.invalidReason}");
        }
        Debug.Log($"========================");
        
        return encounter;
    }
    
    /// <summary>
    /// Create a test encounter for development and testing
    /// </summary>
    public static MasterShipEncounter CreateTestEncounter()
    {
        MasterShipEncounter testShip = new MasterShipEncounter
        {
            shipType = "No Active Ships",
            shipName = "System Ready", 
            destination = "Awaiting Connections",
            origin = "Ship Detection System",
            accessCode = "", // No access code when no ship
            story = "Click Unlock to Connect To Incoming Ships",
            manifest = "", // No manifest when no ship
            captainName = "",
            captainRank = "",
            captainFaction = "System",
            faction = "System",
            shouldApprove = true,
            crewSize = 0, // No crew when no ship
            offersBribe = false, // No bribe option when no ship
            bribeAmount = 0,
            consequenceDescription = "This is a test consequence",
            casualtiesIfWrong = 10,
            creditPenalty = 50,
            canBeCaptured = Random.value > 0.7f,
            holdingPatternTime = 30f
        };
        
        return testShip;
    }
    #endregion

    #region Media Setup
    /// <summary>
    /// Setup media content based on ship data
    /// </summary>
    public void SetupMediaContent()
    {
        // Setup ship image
        if (shipTypeData != null)
        {
            // First try variations if available
            if (shipTypeData.shipVariations != null && shipTypeData.shipVariations.Length > 0)
            {
                shipImage = shipTypeData.shipVariations[Random.Range(0, shipTypeData.shipVariations.Length)];
            }
            // Fall back to main ship icon
            else if (shipTypeData.shipIcon != null)
            {
                shipImage = shipTypeData.shipIcon;
            }
        }
        
        // Setup captain portrait
        if (captainTypeData != null && captainTypeData.portraitOptions != null && 
            captainTypeData.portraitOptions.Length > 0)
        {
            captainPortrait = captainTypeData.portraitOptions[Random.Range(0, captainTypeData.portraitOptions.Length)];
        }
        
        // Setup scenario videos if this is a story encounter
        SetupScenarioVideos();
    }
    
    /// <summary>
    /// Setup videos from scenario data if available
    /// </summary>
    private void SetupScenarioVideos()
    {
        if (scenarioData == null) return;
        
        // For story missions, prioritize scenario videos over generic ones
        if (isStoryShip && scenarioData.HasCompleteVideoContent())
        {
            // Use scenario greeting video as captain video
            VideoClip scenarioGreeting = scenarioData.GetGreetingVideo();
            if (scenarioGreeting != null)
            {
                captainVideo = scenarioGreeting;
            }
            
            // Use scenario story video
            VideoClip scenarioStory = scenarioData.GetStoryVideo();
            if (scenarioStory != null)
            {
                scenarioVideo = scenarioStory;
            }
        }
        
        // Always check for special event videos
        VideoClip specialVideo = scenarioData.GetSpecialEventVideo();
        if (specialVideo != null && scenarioVideo == null)
        {
            scenarioVideo = specialVideo;
        }
    }
    
    /// <summary>
    /// Get the current dialog text for the captain based on encounter state
    /// </summary>
    public string GetCurrentCaptainDialogText()
    {
        CaptainType.Captain.DialogEntry dialog = GetCurrentCaptainDialog();
        return dialog != null ? dialog.phrase : 
            "Requesting clearance to dock."; // Default fallback text
    }
    
    /// <summary>
    /// Get video for player decision response from scenario
    /// </summary>
    public VideoClip GetDecisionResponseVideo(DecisionState decision)
    {
        if (scenarioData != null)
        {
            return scenarioData.GetDecisionResponseVideo(decision);
        }
        return null;
    }
    
    /// <summary>
    /// Get consequence video for PersonalDataLog based on decision correctness
    /// </summary>
    public VideoClip GetConsequenceVideo(bool wasCorrectDecision)
    {
        if (scenarioData != null)
        {
            return scenarioData.GetConsequenceVideo(wasCorrectDecision);
        }
        return null;
    }
    
    /// <summary>
    /// Get audio clip for scenario moments
    /// </summary>
    public AudioClip GetScenarioAudio(ShipScenario.AudioMoment moment)
    {
        if (scenarioData != null)
        {
            return scenarioData.GetAudioClip(moment);
        }
        return null;
    }
    
    /// <summary>
    /// Check if this encounter has scenario-specific videos
    /// </summary>
    public bool HasScenarioVideos()
    {
        return scenarioData != null && scenarioData.HasCompleteVideoContent();
    }

    /// <summary>
    /// Enhance with videos from the media database
    /// </summary>
    public void EnhanceWithVideos(StarkkillerMediaDatabase mediaDatabase)
    {
        if (mediaDatabase == null)
            return;
            
        // IMPORTANT: Preserve the original ship name before any video lookup operations
        string originalShipName = shipName;
        
        // Log encounter details
        Debug.Log($"=== ENCOUNTER VIDEO SELECTION ===");
        Debug.Log($"Ship: {shipType} - \"{shipName}\"");
        Debug.Log($"Captain: {captainRank} {captainName} ({captainFaction})");
        Debug.Log($"Story Ship: {isStoryShip} {(isStoryShip ? "Tag: " + storyTag : "")}");
        Debug.Log($"Has Scenario Videos: {HasScenarioVideos()}");
        
        // For story ships with scenario videos, prioritize scenario content
        if (isStoryShip && HasScenarioVideos())
        {
            Debug.Log($"Using scenario videos for story encounter");
            // Scenario videos were already set in SetupScenarioVideos()
            // Only add ship video from media database as scenarios don't typically have ship videos
            if (shipVideo == null)
            {
                shipVideo = mediaDatabase.GetShipVideo(shipType, shipName);
                Debug.Log($"Ship Video (from media): {(shipVideo != null ? shipVideo.name : "DEFAULT/NONE")}");
            }
        }
        else
        {
            // Regular encounters use media database videos
            // Add ship video
            if (shipVideo == null)
            {
                shipVideo = mediaDatabase.GetShipVideo(shipType, shipName);
                Debug.Log($"Ship Video Selected: {(shipVideo != null ? shipVideo.name : "DEFAULT/NONE")}");
            }
            
            // Add captain video (only if not already set by scenario)
            if (captainVideo == null)
            {
                captainVideo = mediaDatabase.GetCaptainVideo(captainFaction, captainName, captainRank);
                Debug.Log($"Captain Video Selected: {(captainVideo != null ? captainVideo.name : "DEFAULT/NONE")}");
            }
            
            // Add scenario video if this is a story ship (fallback to media database)
            if (isStoryShip && !string.IsNullOrEmpty(storyTag) && scenarioVideo == null)
            {
                scenarioVideo = mediaDatabase.GetScenarioVideo(storyTag);
                Debug.Log($"Scenario Video (from media): {(scenarioVideo != null ? scenarioVideo.name : "DEFAULT/NONE")}");
            }
        }
        
        Debug.Log($"Final Video State:");
        Debug.Log($"  Ship Video: {(shipVideo != null ? shipVideo.name : "NONE")}");
        Debug.Log($"  Captain Video: {(captainVideo != null ? captainVideo.name : "NONE")}");
        Debug.Log($"  Scenario Video: {(scenarioVideo != null ? scenarioVideo.name : "NONE")}");
        Debug.Log($"=================================");
        
        // CRITICAL FIX: Ensure the original ship name is preserved after video enhancement
        // This prevents the ship name from being overwritten during video lookup operations
        shipName = originalShipName;
    }
    #endregion

    #region Conversion From Legacy
    /// <summary>
    /// Convert from legacy ShipEncounter to MasterShipEncounter
    /// </summary>
    public static MasterShipEncounter FromLegacyEncounter(ShipEncounter source)
    {
        if (source == null)
        {
            Debug.LogWarning("MasterShipEncounter: Attempt to convert null ShipEncounter");
            
            // Create a test encounter since we can't convert from null
            return CreateTestEncounter();
        }
            
        MasterShipEncounter master = new MasterShipEncounter();
        
        try
        {
            // Copy basic properties
            master.shipType = source.shipType;
            master.shipName = source.shipName;
            master.destination = source.destination;
            master.origin = source.origin;
            master.accessCode = source.accessCode;
            master.story = source.story;
            master.manifest = source.manifest;
            master.crewSize = source.crewSize;
            master.shouldApprove = source.shouldApprove;
            master.invalidReason = source.invalidReason;
            master.captainName = source.captainName;
            master.captainRank = source.captainRank;
            master.captainFaction = source.captainFaction;
            master.faction = source.captainFaction; // Use captain faction as ship faction
            master.isStoryShip = source.isStoryShip;
            master.storyTag = source.storyTag;
            master.offersBribe = source.offersBribe;
            master.bribeAmount = source.bribeAmount;
            master.consequenceDescription = source.consequenceDescription;
            master.casualtiesIfWrong = source.casualtiesIfWrong;
            master.creditPenalty = source.creditPenalty;
            
            // Copy references to original data if available
            if (source.shipTypeData != null)
                master.shipTypeData = source.shipTypeData;
                
            if (source.captainTypeData != null)
                master.captainTypeData = source.captainTypeData;
                
            if (source.scenarioData != null)
                master.scenarioData = source.scenarioData;
            
            // Copy media if this is an enhanced encounter
            if (source is EnhancedShipEncounter enhancedSource)
            {
                master.shipImage = enhancedSource.shipImage;
                master.captainPortrait = enhancedSource.captainPortrait;
                
                // Copy videos if this is a video-enhanced encounter
                if (source is VideoEnhancedShipEncounter videoSource)
                {
                    master.shipVideo = videoSource.shipVideo;
                    master.captainVideo = videoSource.captainVideo;
                    master.scenarioVideo = videoSource.scenarioVideo;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error converting legacy encounter: {e.Message}");
            
            // Create a recovery encounter on error
            return CreateTestEncounter();
        }
        
        return master;
    }
    #endregion
}