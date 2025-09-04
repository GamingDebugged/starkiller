using UnityEngine;
using System.Collections.Generic;
using StarkillerBaseCommand;
using StarkillerBaseCommand.ShipSystem;

/// <summary>
/// Bridge class to forward calls to MasterShipGenerator
/// This script is part of the migration strategy to replace legacy systems
/// </summary>
public class StarkkillerEncounterSystem : MonoBehaviour
{
    // Reference to the MasterShipGenerator (will be acquired through ShipGeneratorManager)
    private MasterShipGenerator _masterShipGenerator;
    
    // Properties needed by legacy code
    public StarkkillerContentManager contentManager;
    public StarkkillerMediaSystem mediaSystem;
    public float validShipChance = 0.7f;
    public float storyShipChance = 0.2f;
    
    // Flag to avoid duplicate setup
    private bool isInitialized = false;
    
    void Awake()
    {
        // Find content manager and media system
        contentManager = FindFirstObjectByType<StarkkillerContentManager>();
        mediaSystem = FindFirstObjectByType<StarkkillerMediaSystem>();
        
        Debug.Log("StarkkillerEncounterSystem bridge initialized");
    }
    
    void Start()
    {
        // Properly initialize with a slight delay to ensure all systems are loaded
        Invoke("InitializeWithDelay", 0.2f);
    }
    
    /// <summary>
    /// Initialize with a delay to ensure other systems are ready
    /// </summary>
    private void InitializeWithDelay()
    {
        if (isInitialized)
            return;
            
        isInitialized = true;
        
        // First attempt to get the generator from ShipGeneratorManager (preferred method)
        ShipGeneratorManager generatorManager = ShipGeneratorManager.Instance;
        if (generatorManager != null && generatorManager.HasValidShipGenerator())
        {
            _masterShipGenerator = generatorManager.GetShipGenerator();
            Debug.Log("StarkkillerEncounterSystem: Successfully obtained MasterShipGenerator from ShipGeneratorManager");
        }
        else
        {
            // Fallback to direct reference finding
            _masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            Debug.Log("StarkkillerEncounterSystem: Using direct MasterShipGenerator reference (not from manager)");
        }
        
        // Log the result
        if (_masterShipGenerator != null)
        {
            Debug.Log($"StarkkillerEncounterSystem: Connected to MasterShipGenerator (instanceID: {_masterShipGenerator.GetInstanceID()})");
        }
        else
        {
            Debug.LogError("StarkkillerEncounterSystem: Failed to find MasterShipGenerator!");
        }
    }
    
    /// <summary>
    /// Get the next ship encounter
    /// </summary>
    public ShipEncounter GetNextEncounter()
    {
        // Verify we have a valid reference
        EnsureValidShipGenerator();
        
        if (_masterShipGenerator != null)
        {
            // Use the master generator but convert the result to a legacy ShipEncounter
            MasterShipEncounter masterEncounter = _masterShipGenerator.GetNextEncounter();
            
            // Protection against null reference
            if (masterEncounter == null)
            {
                Debug.LogError("StarkkillerEncounterSystem: MasterShipGenerator returned null encounter!");
                return CreateTestShip();
            }
            
            // Convert to regular ShipEncounter for legacy compatibility
            ShipEncounter legacyEncounter = new ShipEncounter();
            legacyEncounter.shipType = masterEncounter.shipType;
            legacyEncounter.destination = masterEncounter.destination;
            legacyEncounter.origin = masterEncounter.origin;
            legacyEncounter.accessCode = masterEncounter.accessCode;
            legacyEncounter.story = masterEncounter.story;
            legacyEncounter.manifest = masterEncounter.manifest;
            legacyEncounter.crewSize = masterEncounter.crewSize;
            legacyEncounter.shouldApprove = masterEncounter.shouldApprove;
            legacyEncounter.invalidReason = masterEncounter.invalidReason;
            legacyEncounter.captainName = masterEncounter.captainName;
            legacyEncounter.captainRank = masterEncounter.captainRank;
            legacyEncounter.captainFaction = masterEncounter.captainFaction;
            legacyEncounter.isStoryShip = masterEncounter.isStoryShip;
            legacyEncounter.storyTag = masterEncounter.storyTag;
            legacyEncounter.offersBribe = masterEncounter.offersBribe;
            legacyEncounter.bribeAmount = masterEncounter.bribeAmount;
            legacyEncounter.consequenceDescription = masterEncounter.consequenceDescription;
            legacyEncounter.casualtiesIfWrong = masterEncounter.casualtiesIfWrong;
            legacyEncounter.creditPenalty = masterEncounter.creditPenalty;
            
            return legacyEncounter;
        }
        
        // Fallback to creating a basic ship if generator is not available
        return CreateTestShip();
    }
    
    /// <summary>
    /// Create a test ship for debugging
    /// </summary>
    public ShipEncounter CreateTestShip()
    {
        ShipEncounter testShip = new ShipEncounter();
        testShip.shipType = "Starkiller Test Ship";
        testShip.destination = "Starkiller Base";
        testShip.origin = "Star Destroyer Finalizer";
        testShip.accessCode = "SK-7429";
        testShip.story = "Ship reports routine supplies delivery.";
        testShip.manifest = "Standard supplies and personnel rotation.";
        testShip.captainName = "Captain Thrawn";
        testShip.shouldApprove = true;
        return testShip;
    }
    
    /// <summary>
    /// Process player decision about a ship
    /// </summary>
    public void ProcessDecision(bool approved)
    {
        // Verify we have a valid reference
        EnsureValidShipGenerator();
        
        if (_masterShipGenerator != null)
        {
            Debug.Log($"StarkkillerEncounterSystem: Forwarding decision ({approved}) to MasterShipGenerator");
            _masterShipGenerator.ProcessDecision(approved);
        }
        else
        {
            Debug.LogError("StarkkillerEncounterSystem: Cannot process decision - no MasterShipGenerator available!");
        }
    }
    
    /// <summary>
    /// Ensure we have a valid reference to the MasterShipGenerator
    /// </summary>
    private void EnsureValidShipGenerator()
    {
        if (_masterShipGenerator == null)
        {
            Debug.LogWarning("StarkkillerEncounterSystem: Lost reference to MasterShipGenerator, attempting to reconnect...");
            
            // Try to get it from the manager first
            ShipGeneratorManager generatorManager = ShipGeneratorManager.Instance;
            if (generatorManager != null && generatorManager.HasValidShipGenerator())
            {
                _masterShipGenerator = generatorManager.GetShipGenerator();
                Debug.Log("StarkkillerEncounterSystem: Successfully reconnected to MasterShipGenerator via manager");
            }
            else
            {
                // Last resort - direct find
                _masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
                Debug.Log("StarkkillerEncounterSystem: Reconnected to MasterShipGenerator via direct reference");
            }
        }
    }
    
    /// <summary>
    /// OnDestroy is called when this object is destroyed
    /// </summary>
    void OnDestroy()
    {
        // Clear reference to prevent memory leaks
        _masterShipGenerator = null;
    }
}
