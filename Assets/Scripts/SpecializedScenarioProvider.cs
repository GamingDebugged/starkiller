using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using StarkillerBaseCommand;

/// <summary>
/// Provides specialized scenarios for story events like bounty hunters and traitors
/// Helps manage the story impact of decisions made with these specialized ships
/// </summary>
public class SpecializedScenarioProvider : MonoBehaviour
{
    [Header("Story Ship Types")]
    [SerializeField] private List<ShipScenario> bountyHunterScenarios = new List<ShipScenario>();
    [SerializeField] private List<ShipScenario> imperiumTraitorScenarios = new List<ShipScenario>();
    [SerializeField] private List<ShipScenario> insurgentScenarios = new List<ShipScenario>();
    
    [Header("Loyalty Impact Settings")]
    [SerializeField] private int bountyHunterApprovalImperialBonus = 2;
    [SerializeField] private int bountyHunterDenialImperialPenalty = -1;
    
    [SerializeField] private int traitorApprovalImperialPenalty = -3;
    [SerializeField] private int traitorApprovalRebellionBonus = 2;
    
    [SerializeField] private int insurgentApprovalImperialPenalty = -3;
    [SerializeField] private int insurgentApprovalRebellionBonus = 3;
    [SerializeField] private int insurgentDenialImperialBonus = 1;
    
    // Cache for story tags and their associated scenarios
    private Dictionary<string, List<ShipScenario>> scenarioCache = new Dictionary<string, List<ShipScenario>>();
    
    // Singleton pattern
    private static SpecializedScenarioProvider _instance;
    public static SpecializedScenarioProvider Instance => _instance;
    
    private void Awake()
    {
        // Setup singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Initialize cache
        InitializeScenarioCache();
    }
    
    /// <summary>
    /// Initialize the scenario cache for quick lookup
    /// </summary>
    private void InitializeScenarioCache()
    {
        // Add bounty hunter scenarios
        scenarioCache["bounty_hunter"] = bountyHunterScenarios;
        
        // Add imperium traitor scenarios
        scenarioCache["imperium_traitor"] = imperiumTraitorScenarios;
        
        // Add insurgent scenarios
        scenarioCache["insurgent"] = insurgentScenarios;
    }
    
    /// <summary>
    /// Get a random scenario by story tag
    /// </summary>
    public ShipScenario GetRandomScenario(string storyTag, int currentDay)
    {
        if (string.IsNullOrEmpty(storyTag) || !scenarioCache.ContainsKey(storyTag))
            return null;
            
        List<ShipScenario> availableScenarios = new List<ShipScenario>();
        
        // Find scenarios available for the current day
        foreach (var scenario in scenarioCache[storyTag])
        {
            if (scenario.dayFirstAppears <= currentDay)
            {
                availableScenarios.Add(scenario);
            }
        }
        
        // If no scenarios available, return null
        if (availableScenarios.Count == 0)
            return null;
            
        // Return a random scenario
        return availableScenarios[Random.Range(0, availableScenarios.Count)];
    }
    
    /// <summary>
    /// Get the loyalty impact for a decision on a ship with the given story tag
    /// </summary>
    public void GetLoyaltyImpact(string storyTag, out int imperialChange, out int rebellionChange, bool isApproval = true)
    {
        imperialChange = 0;
        rebellionChange = 0;
        
        switch (storyTag)
        {
            case "bounty_hunter":
                if (isApproval)
                {
                    // Helping bounty hunters improves Imperial standing
                    imperialChange = bountyHunterApprovalImperialBonus;
                }
                else
                {
                    // Denying bounty hunters has its own impact
                    imperialChange = bountyHunterDenialImperialPenalty;
                }
                break;
                
            case "imperium_traitor":
                // Helping traitors decreases Imperial standing and increases Rebellion sympathy
                imperialChange = traitorApprovalImperialPenalty;
                rebellionChange = traitorApprovalRebellionBonus;
                break;
                
            case "insurgent":
                if (isApproval)
                {
                    // Helping insurgents decreases Imperial standing and increases Rebellion sympathy
                    imperialChange = insurgentApprovalImperialPenalty;
                    rebellionChange = insurgentApprovalRebellionBonus;
                }
                else
                {
                    // Denying insurgents has its own impact 
                    imperialChange = insurgentDenialImperialBonus;
                }
                break;
        }
    }
    
    /// <summary>
    /// Create a specialized story encounter
    /// </summary>
    public MasterShipEncounter CreateStoryEncounter(string storyTag, int currentDay)
    {
        // Get a scenario for this story type
        ShipScenario scenario = GetRandomScenario(storyTag, currentDay);
        if (scenario == null)
        {
            Debug.LogWarning($"No scenario found for story tag: {storyTag} on day {currentDay}");
            return null;
        }
        
        // Find the MasterShipGenerator to create the encounter
        MasterShipGenerator generator = MasterShipGenerator.Instance;
        if (generator == null)
        {
            Debug.LogError("MasterShipGenerator not found!");
            return null;
        }
        
        // Generate a story encounter using the MasterShipGenerator
        MasterShipEncounter encounter = generator.GenerateStoryEncounter(storyTag);
        
        // Set special properties for story ships
        if (encounter != null)
        {
            // Set holding pattern time based on story type
            switch (storyTag)
            {
                case "bounty_hunter":
                    encounter.holdingPatternTime = 45f; // Bounty hunters are impatient
                    break;
                    
                case "imperium_traitor":
                    encounter.holdingPatternTime = 90f; // Traitors are willing to wait longer
                    break;
                    
                case "insurgent":
                    encounter.holdingPatternTime = 60f; // Insurgents are cautious
                    break;
            }
            
            // Set can be captured flag for certain types
            encounter.canBeCaptured = storyTag == "imperium_traitor" || storyTag == "insurgent";
        }
        
        return encounter;
    }
    
    /// <summary>
    /// Check if a ship has a story tag that would make it story-significant
    /// </summary>
    public bool IsStorySignificant(string storyTag)
    {
        return !string.IsNullOrEmpty(storyTag) && 
               (storyTag == "bounty_hunter" || 
                storyTag == "imperium_traitor" || 
                storyTag == "insurgent");
    }
}