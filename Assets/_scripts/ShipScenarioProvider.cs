using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Provides scenarios when none are available in the regular system
/// This helps fix the "No valid scenarios found, using fallback" warnings
/// </summary>
public class ShipScenarioProvider : MonoBehaviour
{
    [Header("Fallback Scenarios")]
    [SerializeField] private ShipScenario defaultValidScenario;
    [SerializeField] private ShipScenario defaultInvalidScenario;
    
    [Header("Valid Scenarios")]
    [SerializeField] private List<ShipScenario> validScenarios = new List<ShipScenario>();
    
    [Header("Invalid Scenarios")]
    [SerializeField] private List<ShipScenario> invalidScenarios = new List<ShipScenario>();
    
    [Header("Story Scenarios")]
    [SerializeField] private List<ShipScenario> storyScenarios = new List<ShipScenario>();
    
    [Header("Settings")]
    [SerializeField] private bool createFallbackScenariosAutomatically = true;
    [SerializeField] private bool loadScenariosFromResources = true;
    [SerializeField] private bool registerWithShipGenerator = true;
    
    [Header("Debugging")]
    [SerializeField] private bool verboseLogging = false;
    [SerializeField] private int scenarioRequestCount = 0;
    [SerializeField] private int fallbackScenarioUsageCount = 0;
    
    // Singleton pattern
    private static ShipScenarioProvider _instance;
    public static ShipScenarioProvider Instance => _instance;
    
    // Reference to MasterShipGenerator
    private MasterShipGenerator shipGenerator;
    
    private void Awake()
    {
        // Setup singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Load scenarios from resources if needed
        if (loadScenariosFromResources)
        {
            LoadScenariosFromResources();
        }
        
        // Create fallback scenarios if needed
        if (createFallbackScenariosAutomatically)
        {
            EnsureFallbackScenariosExist();
        }
        
        Debug.Log($"ShipScenarioProvider initialized with {validScenarios.Count} valid, {invalidScenarios.Count} invalid, and {storyScenarios.Count} story scenarios");
    }
    
    private void Start()
    {
        // Find the ship generator
        if (shipGenerator == null)
        {
            shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        }
        
        // Register with the ship generator
        if (registerWithShipGenerator && shipGenerator != null)
        {
            RegisterWithMasterShipGenerator();
        }
    }
    
    /// <summary>
    /// Register this provider with MasterShipGenerator
    /// </summary>
    private void RegisterWithMasterShipGenerator()
    {
        if (shipGenerator == null)
        {
            Debug.LogWarning("Cannot register with MasterShipGenerator - reference not found!");
            return;
        }
        
        // Try to call the direct registration method if available
        try
        {
            var registerMethod = shipGenerator.GetType().GetMethod("RegisterScenarioProvider", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
                
            if (registerMethod != null)
            {
                registerMethod.Invoke(shipGenerator, new object[] { this });
                Debug.Log("Successfully registered with MasterShipGenerator via RegisterScenarioProvider method");
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error calling RegisterScenarioProvider: {e.Message}");
        }
        
        // Fall back to the reflection-based approach
        try
        {
            // Attempt to set the scenario provider on the ship generator
            var field = shipGenerator.GetType().GetField("scenarioProvider", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
                    
            if (field != null)
            {
                field.SetValue(shipGenerator, this);
                Debug.Log("Successfully registered with MasterShipGenerator via field reflection");
                return;
            }
            
            // Try setting a flag to signal that the provider is available
            var providerField = shipGenerator.GetType().GetField("hasScenarioProvider", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
                        
            if (providerField != null)
            {
                providerField.SetValue(shipGenerator, true);
                Debug.Log("Set hasScenarioProvider flag on MasterShipGenerator");
                return;
            }
            
            Debug.LogWarning("Could not register with MasterShipGenerator - no compatible registration method found");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error registering with MasterShipGenerator: {e.Message}");
        }
    }
    
    /// <summary>
    /// Load scenarios from resources
    /// </summary>
    private void LoadScenariosFromResources()
    {
        // Try multiple paths
        string[] paths = new string[] {
            "ScriptableObjects/Scenarios",
            "Scenarios", 
            "_ScriptableObjects/Scenarios",
            "Ships/Scenarios",
            "Data/Scenarios",
            "", // Try root Resources folder
            "Assets/Resources/Scenarios", // For editor-time loading
            "Assets/Resources/_ScriptableObjects/Scenarios" // For editor-time loading
        };
        
        bool loaded = false;
        
        // Log all resources folders we're checking
        Debug.Log("ShipScenarioProvider: Searching for scenarios in Resources folders...");
        
        foreach (string path in paths)
        {
            Debug.Log($"Attempting to load scenarios from: {path}");
            ShipScenario[] loadedScenarios = Resources.LoadAll<ShipScenario>(path);
            
            if (loadedScenarios != null && loadedScenarios.Length > 0)
            {
                Debug.Log($"Loaded {loadedScenarios.Length} scenarios from Resources/{path}");
                
                // Sort into valid, invalid, and story scenarios
                foreach (var scenario in loadedScenarios)
                {
                    if (scenario == null) continue;
                    
                    if (scenario.isStoryMission)
                    {
                        storyScenarios.Add(scenario);
                        Debug.Log($"Added story scenario: {scenario.scenarioName} with tag: {scenario.storyTag}");
                    }
                    else if (scenario.shouldBeApproved)
                    {
                        validScenarios.Add(scenario);
                        Debug.Log($"Added valid scenario: {scenario.scenarioName}");
                    }
                    else
                    {
                        invalidScenarios.Add(scenario);
                        Debug.Log($"Added invalid scenario: {scenario.scenarioName}");
                    }
                }
                
                loaded = true;
                break;
            }
        }
        
        if (!loaded)
        {
            Debug.LogWarning("Failed to load any scenarios from resources. Will create fallback scenarios.");
            
            // Force creation of fallback scenarios when no resources are found
            Debug.Log("Creating fallback scenarios due to resource loading failure");
            defaultValidScenario = CreateValidScenario();
            validScenarios.Add(defaultValidScenario);
                
            defaultInvalidScenario = CreateInvalidScenario();
            invalidScenarios.Add(defaultInvalidScenario);
                
            CreateBasicStoryScenarios();
        }
        
        // Final count report
        Debug.Log($"ShipScenarioProvider final counts - Valid: {validScenarios.Count}, Invalid: {invalidScenarios.Count}, Story: {storyScenarios.Count}");
    }
    
    /// <summary>
    /// Create fallback scenarios if they don't exist already
    /// </summary>
    private void EnsureFallbackScenariosExist()
    {
        // Create valid scenario if needed
        if (defaultValidScenario == null)
        {
            defaultValidScenario = CreateValidScenario();
            Debug.Log("Created valid fallback scenario");
            
            // Add to the valid scenarios list if not already there
            if (!validScenarios.Contains(defaultValidScenario))
            {
                validScenarios.Add(defaultValidScenario);
            }
        }
        
        // Create invalid scenario if needed
        if (defaultInvalidScenario == null)
        {
            defaultInvalidScenario = CreateInvalidScenario();
            Debug.Log("Created invalid fallback scenario");
            
            // Add to the invalid scenarios list if not already there
            if (!invalidScenarios.Contains(defaultInvalidScenario))
            {
                invalidScenarios.Add(defaultInvalidScenario);
            }
        }
        
        // Create basic story scenarios if needed
        if (storyScenarios.Count == 0)
        {
            CreateBasicStoryScenarios();
        }
    }
    
    /// <summary>
    /// Create basic story scenarios
    /// </summary>
    private void CreateBasicStoryScenarios()
    {
        // Insurgent scenario
        ShipScenario insurgentScenario = ScriptableObject.CreateInstance<ShipScenario>();
        insurgentScenario.scenarioName = "Insurgent Contact";
        insurgentScenario.type = ShipScenario.ScenarioType.StoryEvent;
        insurgentScenario.shouldBeApproved = false; // Should be denied
        insurgentScenario.invalidReason = "Suspected insurgent sympathizer";
        insurgentScenario.isStoryMission = true;
        insurgentScenario.storyTag = "insurgent";
        insurgentScenario.possibleStories = new string[] 
        { 
            "Ship requesting emergency docking for critical repairs.",
            "Transport claiming to have important information for base command.",
            "Vessel requesting refueling and supplies."
        };
        insurgentScenario.possibleManifests = new string[]
        {
            "Essential supplies and undisclosed cargo.",
            "Personnel transfer and communications equipment.",
            "Diplomatic package with restricted clearance."
        };
        insurgentScenario.possibleConsequences = new string[]
        {
            "Security breach! Insurgent sympathizers infiltrated the base.",
            "Intelligence compromised by unauthorized access.",
            "Restricted area security violated by insurgent agents."
        };
        insurgentScenario.dayFirstAppears = 3; // Start on day 3
        
        // Bounty hunter scenario
        ShipScenario bountyHunterScenario = ScriptableObject.CreateInstance<ShipScenario>();
        bountyHunterScenario.scenarioName = "Bounty Hunter";
        bountyHunterScenario.type = ShipScenario.ScenarioType.StoryEvent;
        bountyHunterScenario.shouldBeApproved = true; // Should be approved
        bountyHunterScenario.isStoryMission = true;
        bountyHunterScenario.storyTag = "bounty_hunter";
        bountyHunterScenario.possibleStories = new string[] 
        { 
            "Bounty hunter requesting entry with captured fugitive.",
            "Contracted security vessel returning from mission.",
            "Independent contractor with intelligence on insurgent activity."
        };
        bountyHunterScenario.possibleManifests = new string[]
        {
            "Prisoner containment pod and security equipment.",
            "Captured insurgent and evidence containers.",
            "Intelligence data and security clearance."
        };
        bountyHunterScenario.possibleConsequences = new string[]
        {
            "Important intelligence lost.",
            "Captured insurgent escaped due to security failure.",
            "Bounty hunter reports security breach to superiors."
        };
        bountyHunterScenario.dayFirstAppears = 2; // Start on day 2
        
        // Add to story scenarios
        storyScenarios.Add(insurgentScenario);
        storyScenarios.Add(bountyHunterScenario);
        
        Debug.Log("Created basic story scenarios");
    }
    
    /// <summary>
    /// Create a valid fallback scenario
    /// </summary>
    private ShipScenario CreateValidScenario()
    {
        ShipScenario scenario = ScriptableObject.CreateInstance<ShipScenario>();
        scenario.scenarioName = "Fallback Valid Scenario";
        scenario.type = ShipScenario.ScenarioType.Standard;
        scenario.shouldBeApproved = true;
        scenario.possibleStories = new string[] 
        { 
            "Standard supply run requesting clearance.",
            "Routine personnel transfer requesting docking permission.",
            "Scheduled maintenance visit for base equipment."
        };
        scenario.possibleManifests = new string[] 
        { 
            "Standard supplies and equipment.",
            "Crew of 5, technical equipment, standard supplies.",
            "Maintenance tools, replacement parts, technical crew."
        };
        scenario.possibleConsequences = new string[] 
        { 
            "No issues reported during inspection.",
            "Ship passes all security checks."
        };
        scenario.dayFirstAppears = 1;
        scenario.maxAppearances = -1;  // Unlimited appearances
        
        return scenario;
    }
    
    /// <summary>
    /// Create an invalid fallback scenario
    /// </summary>
    private ShipScenario CreateInvalidScenario()
    {
        ShipScenario scenario = ScriptableObject.CreateInstance<ShipScenario>();
        scenario.scenarioName = "Fallback Invalid Scenario";
        scenario.type = ShipScenario.ScenarioType.Invalid;
        scenario.shouldBeApproved = false;
        scenario.invalidReason = "Invalid access code";
        scenario.possibleStories = new string[] 
        { 
            "Ship requesting emergency landing.",
            "Unscheduled arrival claiming urgent business.",
            "Transport requesting immediate clearance."
        };
        scenario.possibleManifests = new string[] 
        { 
            "Unspecified cargo and personnel.",
            "Mixed cargo, details unavailable.",
            "Private freight, diplomatic clearance claimed."
        };
        scenario.possibleConsequences = new string[] 
        { 
            "Security breach attempted.",
            "Unauthorized access prevented.",
            "Security protocols maintained."
        };
        scenario.dayFirstAppears = 1;
        scenario.maxAppearances = -1;  // Unlimited appearances
        
        return scenario;
    }
    
    /// <summary>
    /// Get a valid scenario based on day
    /// </summary>
    public ShipScenario GetValidScenario(int currentDay = 1)
    {
        scenarioRequestCount++;
        
        // First try to find day-appropriate scenarios
        List<ShipScenario> dayValidScenarios = validScenarios.FindAll(s => s != null && s.dayFirstAppears <= currentDay);
        
        if (dayValidScenarios.Count > 0)
        {
            return dayValidScenarios[Random.Range(0, dayValidScenarios.Count)];
        }
        
        // Fallback to any valid scenario
        if (validScenarios.Count > 0)
        {
            List<ShipScenario> nonNullScenarios = validScenarios.FindAll(s => s != null);
            if (nonNullScenarios.Count > 0)
            {
                return nonNullScenarios[Random.Range(0, nonNullScenarios.Count)];
            }
        }
        
        // Last resort - use default or create new
        fallbackScenarioUsageCount++;
        if (verboseLogging)
        {
            Debug.LogWarning($"Using fallback valid scenario! Request #{scenarioRequestCount}, Fallback usage #{fallbackScenarioUsageCount}");
        }
        
        return defaultValidScenario != null ? defaultValidScenario : CreateValidScenario();
    }
    
    /// <summary>
    /// Get an invalid scenario based on day
    /// </summary>
    public ShipScenario GetInvalidScenario(int currentDay = 1)
    {
        scenarioRequestCount++;
        
        // First try to find day-appropriate scenarios
        List<ShipScenario> dayInvalidScenarios = invalidScenarios.FindAll(s => s != null && s.dayFirstAppears <= currentDay);
        
        if (dayInvalidScenarios.Count > 0)
        {
            return dayInvalidScenarios[Random.Range(0, dayInvalidScenarios.Count)];
        }
        
        // Fallback to any invalid scenario
        if (invalidScenarios.Count > 0)
        {
            List<ShipScenario> nonNullScenarios = invalidScenarios.FindAll(s => s != null);
            if (nonNullScenarios.Count > 0)
            {
                return nonNullScenarios[Random.Range(0, nonNullScenarios.Count)];
            }
        }
        
        // Last resort - use default or create new
        fallbackScenarioUsageCount++;
        if (verboseLogging)
        {
            Debug.LogWarning($"Using fallback invalid scenario! Request #{scenarioRequestCount}, Fallback usage #{fallbackScenarioUsageCount}");
        }
        
        return defaultInvalidScenario != null ? defaultInvalidScenario : CreateInvalidScenario();
    }
    
    /// <summary>
    /// Get a story scenario based on tag and day
    /// </summary>
    public ShipScenario GetStoryScenario(string storyTag, int currentDay = 1)
    {
        scenarioRequestCount++;
        
        // Find matching story scenarios for the current day
        List<ShipScenario> matchingScenarios = storyScenarios.FindAll(
            s => s != null && 
            s.isStoryMission && 
            s.storyTag == storyTag && 
            s.dayFirstAppears <= currentDay);
            
        if (matchingScenarios.Count > 0)
        {
            return matchingScenarios[Random.Range(0, matchingScenarios.Count)];
        }
        
        // If no matching scenario, create a basic story scenario based on tag
        fallbackScenarioUsageCount++;
        if (verboseLogging)
        {
            Debug.LogWarning($"Creating on-demand story scenario for tag '{storyTag}'! Request #{scenarioRequestCount}, Fallback usage #{fallbackScenarioUsageCount}");
        }
        
        ShipScenario newScenario = CreateStoryScenarioForTag(storyTag, currentDay);
        storyScenarios.Add(newScenario);
        return newScenario;
    }
    
    /// <summary>
    /// Create a story scenario for a specific tag
    /// </summary>
    private ShipScenario CreateStoryScenarioForTag(string storyTag, int currentDay)
    {
        ShipScenario scenario = ScriptableObject.CreateInstance<ShipScenario>();
        scenario.isStoryMission = true;
        scenario.storyTag = storyTag;
        scenario.dayFirstAppears = currentDay;
        scenario.type = ShipScenario.ScenarioType.StoryEvent;
        
        switch (storyTag)
        {
            case "insurgent":
                scenario.scenarioName = "Insurgent Contact";
                scenario.shouldBeApproved = false;
                scenario.invalidReason = "Suspected insurgent sympathizer";
                scenario.possibleStories = new string[] { "Ship requesting emergency docking for critical repairs." };
                scenario.possibleManifests = new string[] { "Essential supplies and undisclosed cargo." };
                scenario.possibleConsequences = new string[] { "Security breach! Insurgent sympathizers infiltrated the base." };
                break;
                
            case "bounty_hunter":
                scenario.scenarioName = "Bounty Hunter";
                scenario.shouldBeApproved = true;
                scenario.possibleStories = new string[] { "Bounty hunter requesting entry with captured fugitive." };
                scenario.possibleManifests = new string[] { "Prisoner containment pod and security equipment." };
                scenario.possibleConsequences = new string[] { "Important intelligence lost." };
                break;
                
            case "imperium_traitor":
                scenario.scenarioName = "Fleeing Officer";
                scenario.shouldBeApproved = false;
                scenario.invalidReason = "Officer wanted for treason";
                scenario.possibleStories = new string[] { "Imperium officer requesting emergency clearance." };
                scenario.possibleManifests = new string[] { "Personal effects and data files." };
                scenario.possibleConsequences = new string[] { "Imperium traitor escaped justice." };
                break;
                
            default:
                scenario.scenarioName = "Story Event";
                scenario.shouldBeApproved = true;
                scenario.possibleStories = new string[] { "Special mission ship requesting clearance." };
                scenario.possibleManifests = new string[] { "Classified cargo and personnel." };
                scenario.possibleConsequences = new string[] { "Mission compromised due to security breach." };
                break;
        }
        
        return scenario;
    }
    
    /// <summary>
    /// Get a scenario based on validity and day
    /// </summary>
    public ShipScenario GetScenario(bool shouldBeValid, int currentDay = 1)
    {
        return shouldBeValid ? GetValidScenario(currentDay) : GetInvalidScenario(currentDay);
    }
    
    /// <summary>
    /// Get scenario counts for debugging
    /// </summary>
    public string GetScenarioStats()
    {
        return $"Scenarios - Valid: {validScenarios.Count}, Invalid: {invalidScenarios.Count}, Story: {storyScenarios.Count}\n" + 
               $"Requests: {scenarioRequestCount}, Fallbacks used: {fallbackScenarioUsageCount}";
    }
    
    /// <summary>
    /// Add a new scenario to the appropriate list
    /// </summary>
    public void AddScenario(ShipScenario scenario)
    {
        if (scenario == null)
            return;
            
        if (scenario.isStoryMission)
        {
            if (!storyScenarios.Contains(scenario))
                storyScenarios.Add(scenario);
        }
        else if (scenario.shouldBeApproved)
        {
            if (!validScenarios.Contains(scenario))
                validScenarios.Add(scenario);
        }
        else
        {
            if (!invalidScenarios.Contains(scenario))
                invalidScenarios.Add(scenario);
        }
    }
}