using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using StarkillerBaseCommand;

/// <summary>
/// Adapter to help transition from legacy encounter systems to MasterShipGenerator
/// This provides bridge functionality for systems that still reference removed classes
/// </summary>
public class LegacySystemsAdapter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MasterShipGenerator masterShipGenerator;
    [SerializeField] private EncounterSystemManager systemManager;
    
    [Header("Settings")]
    [SerializeField] private bool verboseLogging = true;
    [SerializeField] private bool suppressWarnings = false;
    
    // Singleton pattern
    private static LegacySystemsAdapter _instance;
    public static LegacySystemsAdapter Instance
    {
        get 
        {
            if (_instance == null)
            {
                // Try to find an existing instance
                _instance = FindFirstObjectByType<LegacySystemsAdapter>();
                
                // If still null, try to create a new instance
                if (_instance == null)
                {
                    GameObject go = new GameObject("LegacySystemsAdapter");
                    _instance = go.AddComponent<LegacySystemsAdapter>();
                    Debug.Log("LegacySystemsAdapter created automatically");
                }
            }
            return _instance;
        }
    }
    
    // This will be accessed by Unity's reflection system
    // to check if ShipEncounterSystem exists
    [System.NonSerialized]
    public static Type ShipEncounterSystemType = typeof(MasterShipGenerator);
    
    // This will be accessed by Unity's reflection system
    // to check if StarkkillerEncounterSystem exists
    [System.NonSerialized]
    public static Type StarkkillerEncounterSystemType = typeof(MasterShipGenerator);
    
    // This will be accessed by Unity's reflection system
    // to check if ShipEncounterGenerator exists
    [System.NonSerialized]
    public static Type ShipEncounterGeneratorType = typeof(MasterShipGenerator);
    
    // Proxy getInstance methods to handle null checks
    public static MasterShipGenerator GetShipEncounterSystem()
    {
        MasterShipGenerator generator = null;
        
        // First try to get it from system manager
        EncounterSystemManager manager = FindFirstObjectByType<EncounterSystemManager>();
        if (manager != null)
        {
            var component = manager.GetActiveEncounterSystem();
            if (component is MasterShipGenerator)
                generator = (MasterShipGenerator)component;
        }
        
        // If that failed, try direct lookup
        if (generator == null)
            generator = FindFirstObjectByType<MasterShipGenerator>();
        
        // Log warnings if we couldn't find it
        if (generator == null && Instance.verboseLogging && !Instance.suppressWarnings)
        {
            Debug.LogWarning("LegacySystemsAdapter: Legacy code requested ShipEncounterSystem but no MasterShipGenerator was found!");
        }
        
        return generator;
    }
    
    public static MasterShipGenerator GetStarkkillerEncounterSystem()
    {
        // This just redirects to the same generator
        return GetShipEncounterSystem();
    }
    
    public static MasterShipGenerator GetShipEncounterGenerator()
    {
        // This just redirects to the same generator
        return GetShipEncounterSystem();
    }
    
    void Awake()
    {
        // Ensure singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        
        // Find references if not assigned
        if (masterShipGenerator == null)
            masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            
        if (systemManager == null)
            systemManager = FindFirstObjectByType<EncounterSystemManager>();
            
        // Log initialization
        Debug.Log("LegacySystemsAdapter initialized - providing transition compatibility for removed systems");
    }
    
    /// <summary>
    /// Generate a dummy scenario that MasterShipGenerator can use
    /// </summary>
    public ShipScenario GenerateDummyScenario(bool shouldBeValid)
    {
        ShipScenario scenario = ScriptableObject.CreateInstance<ShipScenario>();
        scenario.scenarioName = shouldBeValid ? "Valid Scenario" : "Invalid Scenario";
        scenario.type = shouldBeValid ? ShipScenario.ScenarioType.Standard : ShipScenario.ScenarioType.Invalid;
        scenario.shouldBeApproved = shouldBeValid;
        scenario.dayFirstAppears = 1;
        
        if (!shouldBeValid)
        {
            scenario.invalidReason = "Invalid access code";
        }
        
        // Set basic properties
        scenario.possibleStories = new string[] { "Ship requesting standard clearance." };
        scenario.possibleManifests = new string[] { "Standard supplies and equipment." };
        
        return scenario;
    }
    
    /// <summary>
    /// Get a ship encounter from the MasterShipGenerator as a replacement for legacy systems
    /// This can be called by other scripts to get encounters without direct references
    /// </summary>
    public MasterShipEncounter GetShipEncounter(bool forceValid = false)
    {
        // Try to use the MasterShipGenerator first
        if (masterShipGenerator != null)
        {
            return masterShipGenerator.GenerateRandomEncounter(forceValid);
        }
        
        // If master generator not found, try the system manager
        if (systemManager != null)
        {
            Component activeSystem = systemManager.GetActiveEncounterSystem();
            if (activeSystem is MasterShipGenerator)
            {
                return ((MasterShipGenerator)activeSystem).GenerateRandomEncounter(forceValid);
            }
        }
        
        // Last resort - create a fallback encounter
        Debug.LogWarning("LegacySystemsAdapter: No generator found! Creating fallback encounter.");
        return MasterShipEncounter.CreateTestEncounter();
    }
    
    /// <summary>
    /// Used by legacy systems that need access to the valid access codes
    /// </summary>
    public List<string> GetValidAccessCodes()
    {
        // Grab them from StarkkillerContentManager if available
        StarkkillerContentManager contentManager = FindFirstObjectByType<StarkkillerContentManager>();
        if (contentManager != null && contentManager.currentAccessCodes != null && contentManager.currentAccessCodes.Count > 0)
        {
            return contentManager.currentAccessCodes;
        }
        
        // Fallback - generate some codes
        List<string> codes = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            codes.Add("SK-" + UnityEngine.Random.Range(1000, 10000));
        }
        
        return codes;
    }
    
    /// <summary>
    /// Generate a random valid ship for compatibility with legacy systems
    /// </summary>
    public object GenerateValidShip()
    {
        return GetShipEncounter(true); // Force it to be valid
    }
    
    /// <summary>
    /// Generate a random invalid ship for compatibility with legacy systems
    /// </summary>
    public object GenerateInvalidShip()
    {
        return GetShipEncounter(false); // Not forcing valid = can be invalid
    }
}
