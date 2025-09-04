using System.Collections.Generic;
using UnityEngine;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Central content management system for Starkiller Base Command
    /// Handles access to all game content data including ships, captains, scenarios, etc.
    /// </summary>
    public class StarkkillerContentManager : MonoBehaviour
    {
        #region Content Databases
        [Header("Content Databases")]
        [Tooltip("Ship types and categories")]
        public List<ShipType> shipTypes = new List<ShipType>();
        public List<ShipCategory> shipCategories = new List<ShipCategory>();
        
        [Header("Access Codes")]
        [Tooltip("All access code assets")]
        public List<AccessCode> allAccessCodes = new List<AccessCode>();

        [Tooltip("Include red herring codes in the database")]
        public bool includeRedHerrings = true;

        [Tooltip("Captain types")]
        public List<CaptainType> captainTypes = new List<CaptainType>();
        
        [Tooltip("Scenarios")]
        public List<ShipScenario> scenarios = new List<ShipScenario>();
        
        [Tooltip("Consequences")]
        public List<Consequence> consequences = new List<Consequence>();
        #endregion

        #region Game State
        [Header("Game State")]
        [Tooltip("Valid access codes for the current day")]
        public List<string> currentAccessCodes = new List<string>();
        
        [Tooltip("Current game day")]
        public int currentDay = 1;
        
        [Tooltip("Current imperium loyalty")]
        public int imperialLoyalty = 50;
        
        [Tooltip("Current insurgent sympathy")]
        public int rebelSympathy = 0;
        
        [Tooltip("Special events and rules active for the current day")]
        public List<string> currentDayRules = new List<string>();
        #endregion

        #region Game Settings
        [Header("Game Settings")]
        [Tooltip("Probability of generating a valid ship (0-1)")]
        [Range(0f, 1f)]
        public float validShipChance = 0.7f;
        
        [Tooltip("Probability of generating a story ship (0-1)")]
        [Range(0f, 1f)]
        public float storyShipChance = 0.2f;
        
        [Tooltip("Special rules that become active on specific days")]
        public List<DayRule> specialRules = new List<DayRule>();
        #endregion

        #region Day Rule Definition
        [System.Serializable]
        public class DayRule
        {
            public int activateOnDay;
            public string ruleDescription;
            public RuleType ruleType;
            
            public enum RuleType
            {
                VerifyOrigin,
                VerifyManifest,
                CheckForContraband,
                CheckForIntelligence,
                ForceInspection,
                AccessCodeChange
            }
        }
        #endregion

        // Singleton pattern
        private static StarkkillerContentManager _instance;
        public static StarkkillerContentManager Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogWarning("StarkkillerContentManager not found in scene!");
                return _instance;
            }
        }

        // Reference to GameManager to sync day value
        private GameManager gameManager;
        
        // Flag to prevent multiple day updates
        private bool isUpdatingDay = false;

        private void Awake()
        {
            // Singleton setup
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;

            // Try to find the GameManager
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                // Sync with GameManager's day value
                currentDay = gameManager.currentDay;
                Debug.Log($"ContentManager synced with GameManager day: {currentDay}");
            }
            
            // Initialize content databases
            InitializeContentDatabases();
            
            // Generate initial access codes - only if not in play mode
            if (!Application.isPlaying || Time.frameCount < 5) 
            {
                RegenerateAccessCodes();
            }
        }

        #region Content Database Initialization
        private void InitializeContentDatabases()
        {
            // Load ShipTypes if not assigned
            if (shipTypes.Count == 0)
            {
                List<ShipType> loadedShipTypes = ResourceLoadingHelper.LoadShipTypes();
                if (loadedShipTypes.Count > 0)
                {
                    shipTypes.AddRange(loadedShipTypes);
                    Debug.Log($"Loaded {loadedShipTypes.Count} ship types using centralized resource loading");
                }
                else
                {
                    Debug.LogWarning("No ship types found using centralized resource loading");
                }
            }
            
            // Load ShipCategories if not assigned
            if (shipCategories.Count == 0)
            {
                string[] categoryPaths = new string[] {
                    "Assets/Resources/_ScriptableObjects/ShipCategories",
                    "Resources/_ScriptableObjects/ShipCategories",
                    "ScriptableObjects/ShipCategories",
                    "_ScriptableObjects/ShipCategories",
                    "ShipCategories"
                };
                
                bool loaded = false;
                foreach (string path in categoryPaths)
                {
                    ShipCategory[] loadedCategories = Resources.LoadAll<ShipCategory>(path);
                    if (loadedCategories != null && loadedCategories.Length > 0)
                    {
                        shipCategories.AddRange(loadedCategories);
                        Debug.Log($"Loaded {loadedCategories.Length} ship categories from resources/{path}");
                        loaded = true;
                        break;
                    }
                }
                
                if (!loaded)
                {
                    Debug.LogWarning("No ship categories found in resources or asset paths");
                }
            }
            
            // Load AccessCodes if not assigned
            if (allAccessCodes.Count == 0)
            {
                allAccessCodes = ResourceLoadingHelper.LoadAccessCodes();
                
                if (allAccessCodes.Count == 0)
                {
                    Debug.LogWarning("No access codes found in resources");
                }
                else
                {
                    Debug.Log($"Loaded {allAccessCodes.Count} access codes");
                }
            }
            
            // Load CaptainTypes if not assigned
            if (captainTypes.Count == 0)
            {
                List<CaptainType> loadedCaptainTypes = ResourceLoadingHelper.LoadCaptainTypes();
                if (loadedCaptainTypes.Count > 0)
                {
                    captainTypes.AddRange(loadedCaptainTypes);
                    Debug.Log($"Loaded {loadedCaptainTypes.Count} captain types using centralized resource loading");
                }
                else
                {
                    Debug.LogWarning("No captain types found using centralized resource loading");
                }
            }
            
            // Load Scenarios if not assigned
            if (scenarios.Count == 0)
            {
                List<ShipScenario> loadedScenarios = ResourceLoadingHelper.LoadShipScenarios();
                if (loadedScenarios.Count > 0)
                {
                    scenarios.AddRange(loadedScenarios);
                    Debug.Log($"Loaded {loadedScenarios.Count} scenarios using centralized resource loading");
                }
                else
                {
                    Debug.LogWarning("No scenarios found using centralized resource loading");
                }
            }
            
            // Load Consequences if not assigned
            if (consequences.Count == 0)
            {
                string[] consequencePaths = new string[] {
                    "Assets/Resources/_ScriptableObjects/Consequences",
                    "Resources/_ScriptableObjects/Consequences",
                    "ScriptableObjects/Consequences",
                    "_ScriptableObjects/Consequences",
                    "Consequences"
                };
                
                bool loaded = false;
                foreach (string path in consequencePaths)
                {
                    Consequence[] loadedConsequences = Resources.LoadAll<Consequence>(path);
                    if (loadedConsequences != null && loadedConsequences.Length > 0)
                    {
                        consequences.AddRange(loadedConsequences);
                        Debug.Log($"Loaded {loadedConsequences.Length} consequences from resources/{path}");
                        loaded = true;
                        break;
                    }
                }
                
                if (!loaded)
                {
                    Debug.LogWarning("No consequences found in resources or asset paths");
                }
            }
            
            // Load and initialize CargoManifests through ManifestManager
            InitializeManifestManager();
        }
        
        /// <summary>
        /// Initialize the ManifestManager with loaded cargo manifests
        /// </summary>
        private void InitializeManifestManager()
        {
            // Load CargoManifests if ManifestManager is available
            if (ManifestManager.Instance != null)
            {
                // Use ResourcePathManager for efficient loading
                CargoManifest[] manifests = ResourcePathManager.LoadAll<CargoManifest>(ResourcePathManager.ResourceType.CargoManifests);
                
                if (manifests != null && manifests.Length > 0)
                {
                    // Initialize ManifestManager with the loaded manifests
                    ManifestManager.Instance.InitializeManifests(manifests);
                    Debug.Log($"Initialized ManifestManager with {manifests.Length} cargo manifests");
                }
                else
                {
                    Debug.LogWarning("No cargo manifests found in resources - ManifestManager will use fallback generation");
                }
            }
            else
            {
                Debug.LogWarning("ManifestManager not found - cargo manifest system will not be available");
            }
        }
        #endregion

        #region Daily Game State Management
        /// <summary>
        /// Handle a new day - this is called by GameManager, we DON'T set the day value here
        /// </summary>
        public void StartNewDay(int day)
        {
            // Prevent multiple calls during the same frame/update
            if (isUpdatingDay)
            {
                Debug.LogWarning($"[ContentManager] StartNewDay called while already updating day. Ignoring redundant call.");
                return;
            }
            
            isUpdatingDay = true;
            
            try
            {
                // Log when this method is called to debug
                Debug.Log($"[ContentManager] StartNewDay called with day={day}, current day was {currentDay}");
            
                // Set the day value from the parameter (which should come from GameManager)
                currentDay = day;
            
                // Generate new access codes
                RegenerateAccessCodes();
            
                // Update daily rules
                UpdateDailyRules();
            
                // Notify other systems
                NotifyNewDay();
                
                Debug.Log($"[ContentManager] Day update complete. Current day now: {currentDay}");
            }
            finally
            {
                // Set updating flag back to false
                isUpdatingDay = false;
            }
        }

        /// <summary>
        /// Generate new valid access codes
        /// </summary>
        public void RegenerateAccessCodes()
        {
            currentAccessCodes.Clear();
            
            // Load valid codes for current day
            foreach (AccessCode code in allAccessCodes)
            {
                if (IsAccessCodeValidForDay(code, currentDay))
                {
                    currentAccessCodes.Add(code.codeValue);
                }
            }
            
            // Fallback if no codes are valid
            if (currentAccessCodes.Count == 0)
            {
                Debug.LogWarning($"No valid access codes for day {currentDay}! Using emergency fallback.");
                // Generate a glitched code or use emergency code
                currentAccessCodes.Add("XXX-????"); // Glitched code
                currentAccessCodes.Add("EMG-0999"); // Emergency fallback
            }
            
            Debug.Log($"Loaded {currentAccessCodes.Count} valid access codes for day {currentDay}");
            
            // Log which codes are valid for debugging
            foreach (AccessCode code in allAccessCodes)
            {
                if (IsAccessCodeValidForDay(code, currentDay))
                {
                    Debug.Log($"Day {currentDay}: {code.codeName} ({code.codeValue}) is VALID");
                }
                else
                {
                    string reason = code.isRevoked ? "REVOKED" : 
                                   currentDay < code.validFromDay ? "NOT YET ACTIVE" : 
                                   "EXPIRED";
                    Debug.Log($"Day {currentDay}: {code.codeName} ({code.codeValue}) is INVALID - {reason}");
                }
            }
        }

        private bool IsAccessCodeValidForDay(AccessCode code, int day)
        {
            if (code.isRevoked) return false;
            if (day < code.validFromDay) return false;
            if (code.validUntilDay != -1 && day > code.validUntilDay) return false;
            return true;
        }

        // Get AccessCode data for validation
        public AccessCode GetAccessCodeData(string codeValue)
        {
            return allAccessCodes.Find(c => c.codeValue == codeValue);
        }

        // Check if a code is authorized for a specific faction
        public bool IsCodeAuthorizedForFaction(string codeValue, string faction)
        {
            AccessCode codeData = GetAccessCodeData(codeValue);
            if (codeData == null) return false;
            
            // No faction restrictions means anyone can use it
            if (codeData.authorizedFactions == null || codeData.authorizedFactions.Length == 0)
                return true;
            
            return System.Array.Exists(codeData.authorizedFactions, 
                f => f.Equals(faction, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Update the daily rules based on current game day
        /// </summary>
        private void UpdateDailyRules()
        {
            currentDayRules.Clear();
            
            // Add access codes rule
            string accessCodeRule = "Valid access codes: " + string.Join(", ", currentAccessCodes);
            currentDayRules.Add(accessCodeRule);
            
            // Add day-specific rules
            foreach (var rule in specialRules)
            {
                if (rule.activateOnDay <= currentDay)
                {
                    currentDayRules.Add(rule.ruleDescription);
                }
            }
        }

        /// <summary>
        /// Notify other game systems about a new day
        /// </summary>
        private void NotifyNewDay()
        {
            // Find and notify relevant systems - but NOT systems that would start their own day tracking
            ConsequenceManager securityManager = FindFirstObjectByType<ConsequenceManager>();
            if (securityManager != null)
            {
                // Use reflection to invoke StartNewDay if it exists
                var method = securityManager.GetType().GetMethod("StartNewDay");
                if (method != null)
                {
                    method.Invoke(securityManager, new object[] { currentDay });
                }
            }
            
            // Notify ManifestManager to reset daily usage tracking
            if (ManifestManager.Instance != null)
            {
                ManifestManager.Instance.ResetDailyUsage();
                Debug.Log("Notified ManifestManager to reset daily usage for new day");
            }
            
            // Remove notification to ShipEncounterSystem which will now be controlled by GameManager
        }
        #endregion

        #region Content Selection Methods
        /// <summary>
        /// Get a ship type by name
        /// </summary>
        public ShipType GetShipType(string typeName)
        {
            foreach (var type in shipTypes)
            {
                if (type.typeName == typeName)
                {
                    return type;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a random ship type, optionally filtered by category
        /// </summary>
        public ShipType GetRandomShipType(string categoryName = "")
        {
            List<ShipType> validTypes = new List<ShipType>();
            
            foreach (var type in shipTypes)
            {
                // If category specified, filter by it
                if (!string.IsNullOrEmpty(categoryName))
                {
                    if (type.category != null && type.category.categoryName == categoryName)
                    {
                        validTypes.Add(type);
                    }
                }
                else
                {
                    validTypes.Add(type);
                }
            }
            
            if (validTypes.Count > 0)
            {
                return validTypes[Random.Range(0, validTypes.Count)];
            }
            
            // If no matches or empty list, return null
            return null;
        }

        /// <summary>
        /// Get a captain type by name
        /// </summary>
        public CaptainType GetCaptainType(string typeName)
        {
            foreach (var type in captainTypes)
            {
                if (type.typeName == typeName)
                {
                    return type;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a random captain type, optionally filtered by faction
        /// </summary>
        public CaptainType GetRandomCaptainType(string faction = "")
        {
            List<CaptainType> validTypes = new List<CaptainType>();
            
            foreach (var type in captainTypes)
            {
                // If faction specified, filter by it
                if (!string.IsNullOrEmpty(faction))
                {
                    if (type.factions != null)
                    {
                        foreach (var captainFaction in type.factions)
                        {
                            if (captainFaction.ToLower().Contains(faction.ToLower()))
                            {
                                validTypes.Add(type);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    validTypes.Add(type);
                }
            }
            
            if (validTypes.Count > 0)
            {
                return validTypes[Random.Range(0, validTypes.Count)];
            }
            
            // If no matches or empty list, return null
            return null;
        }

        /// <summary>
        /// Get a scenario by name
        /// </summary>
        public ShipScenario GetScenario(string scenarioName)
        {
            foreach (var scenario in scenarios)
            {
                if (scenario.scenarioName == scenarioName)
                {
                    return scenario;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a random scenario, optionally filtered by type
        /// </summary>
        public ShipScenario GetRandomScenario(ShipScenario.ScenarioType type = ShipScenario.ScenarioType.Standard, string storyTag = "")
        {
            // Make sure we have the current day
            if (gameManager != null)
            {
                currentDay = gameManager.currentDay;
            }
            
            List<ShipScenario> validScenarios = new List<ShipScenario>();
            
            foreach (var scenario in scenarios)
            {
                bool typeMatch = (scenario.type == type);
                bool storyMatch = string.IsNullOrEmpty(storyTag) || scenario.storyTag == storyTag;
                bool dayMatch = scenario.dayFirstAppears <= currentDay;
                
                // Add if all criteria match
                if (typeMatch && storyMatch && dayMatch)
                {
                    validScenarios.Add(scenario);
                }
            }
            
            if (validScenarios.Count > 0)
            {
                return validScenarios[Random.Range(0, validScenarios.Count)];
            }
            
            // If no matches or empty list, return null
            return null;
        }

        /// <summary>
        /// Get a consequence by severity
        /// </summary>
        public Consequence GetRandomConsequence(Consequence.SeverityLevel severity)
        {
            List<Consequence> validConsequences = new List<Consequence>();
            
            foreach (var consequence in consequences)
            {
                if (consequence.severity == severity)
                {
                    validConsequences.Add(consequence);
                }
            }
            
            if (validConsequences.Count > 0)
            {
                return validConsequences[Random.Range(0, validConsequences.Count)];
            }
            
            // If no matches or empty list, return null
            return null;
        }
        #endregion

        #region Access Code Validation
        /// <summary>
        /// Check if an access code is valid
        /// </summary>
        public bool IsAccessCodeValid(string accessCode)
        {
            return currentAccessCodes.Contains(accessCode);
        }
        
        /// <summary>
        /// Get a random valid access code from the current list
        /// </summary>
        public string GetRandomValidAccessCode()
        {
            if (currentAccessCodes.Count == 0)
            {
                Debug.LogWarning("No valid access codes available! Generating default code.");
                RegenerateAccessCodes();
            }
            
            if (currentAccessCodes.Count > 0)
            {
                return currentAccessCodes[Random.Range(0, currentAccessCodes.Count)];
            }
            else
            {
                // Emergency fallback
                return "EMG-0999";
            }
        }
        #endregion

        #region Destination and Origin Validation
        /// <summary>
        /// Check if an origin is valid for a destination
        /// </summary>
        public bool IsOriginValidForDestination(string origin, string destination)
        {
            // By default, all origins are valid
            bool hasOriginRule = currentDayRules.Exists(rule => rule.Contains("Verify origin"));
            
            // If there's no rule about origins, all are valid
            if (!hasOriginRule)
            {
                return true;
            }
            
            // Otherwise, check ship types for specific rules
            foreach (var type in shipTypes)
            {
                if (type.commonOrigins != null)
                {
                    foreach (var validOrigin in type.commonOrigins)
                    {
                        if (validOrigin == origin)
                        {
                            return true;
                        }
                    }
                }
            }
            
            // If no specific rules matched, use a fallback rule
            // (e.g., origin contains destination name)
            return origin.Contains(destination) || destination.Contains(origin);
        }
        #endregion

        #region Manifest and Cargo Validation
        /// <summary>
        /// Check if a manifest contains contraband
        /// </summary>
        public bool ManifestContainsContraband(string manifest)
        {
            // Check if we have the rule active
            bool hasContrabandRule = currentDayRules.Exists(rule => rule.Contains("contraband") || rule.Contains("Contraband"));
            
            // If there's no rule about contraband, no contraband is detected
            if (!hasContrabandRule)
            {
                return false;
            }
            
            // Otherwise, check for suspicious terms
            string[] contrabandTerms = {
                "weapons", "blaster", "explosive", "restricted", "banned",
                "smuggled", "illegal", "contraband", "spice", "undeclared"
            };
            
            foreach (var term in contrabandTerms)
            {
                if (manifest.ToLower().Contains(term.ToLower()))
                {
                    return true;
                }
            }
            
            return false;
        }
        #endregion
    }
}