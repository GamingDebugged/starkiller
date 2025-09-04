using UnityEngine;
using System.Collections;
using StarkillerBaseCommand;

/// <summary>
/// Manages all encounter systems to ensure only one is active at a time
/// This class resolves conflicts between the different generator systems
/// </summary>
public class EncounterSystemManager : MonoBehaviour
{
    [Header("System References")]
    [SerializeField] private MasterShipGenerator masterShipGenerator;
    [SerializeField] private ShipEncounterSystem legacyEncounterSystem; 
    [SerializeField] private StarkkillerEncounterSystem starkkillerEncounterSystem;
    [SerializeField] private ShipGeneratorCoordinator shipGeneratorCoordinator;
    [SerializeField] private CredentialChecker credentialChecker;
    
    [Header("Active System")]
    [Tooltip("Which ship encounter system to use as the primary system")]
    public EncounterSystemType activeSystem = EncounterSystemType.MasterShipGenerator;
    
    [Header("Debug Settings")]
    [Tooltip("Will log when systems are activated or deactivated")]
    public bool verboseLogging = true;
    [Tooltip("Add delay between system initialization to avoid race conditions")]
    public float initializationDelay = 0.2f;
    
    // Enum for all encounter systems
    public enum EncounterSystemType
    {
        MasterShipGenerator,          // Use the unified MasterShipGenerator
        LegacyEncounterSystem,        // Use original ShipEncounterSystem
        StarkkillerEncounterSystem,   // Use new StarkkillerEncounterSystem
        ShipGeneratorCoordinator      // Use ShipGeneratorCoordinator to manage other systems
    }
    
    // Static instance for singleton pattern
    private static EncounterSystemManager _instance;
    public static EncounterSystemManager Instance => _instance;
    
    // Flag to track initialization state
    private bool isInitialized = false;
    
    void Awake()
    {
        // Singleton pattern setup
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Multiple EncounterSystemManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        // Don't use DontDestroyOnLoad since this may be a child GameObject
        
        FindSystemReferences();
        LogDetectedSystems();
    }
    
    void Start()
    {
        // Delay initialization to avoid race conditions
        StartCoroutine(DelayedInitialization());
    }
    
    /// <summary>
    /// Returns the currently active encounter system as a Component
    /// </summary>
    public Component GetActiveEncounterSystem()
    {
        switch (activeSystem)
        {
            case EncounterSystemType.MasterShipGenerator:
                return masterShipGenerator;
            case EncounterSystemType.LegacyEncounterSystem:
                return legacyEncounterSystem;
            case EncounterSystemType.StarkkillerEncounterSystem:
                return starkkillerEncounterSystem;
            case EncounterSystemType.ShipGeneratorCoordinator:
                return shipGeneratorCoordinator;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// Finds all system references if not assigned in inspector
    /// </summary>
    private void FindSystemReferences()
    {
        if (masterShipGenerator == null)
            masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            
        if (legacyEncounterSystem == null)
            legacyEncounterSystem = FindFirstObjectByType<ShipEncounterSystem>();
            
        if (starkkillerEncounterSystem == null)
            starkkillerEncounterSystem = FindFirstObjectByType<StarkkillerEncounterSystem>();
            
        if (shipGeneratorCoordinator == null)
            shipGeneratorCoordinator = FindFirstObjectByType<ShipGeneratorCoordinator>();
            
        if (credentialChecker == null)
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
    }
    
    /// <summary>
    /// Logs all detected systems for debugging
    /// </summary>
    private void LogDetectedSystems()
    {
        string systemsMessage = "EncounterSystemManager detected systems: ";
        systemsMessage += masterShipGenerator != null ? "MasterShipGenerator ✓ " : "MasterShipGenerator ✗ ";
        systemsMessage += legacyEncounterSystem != null ? "LegacyEncounterSystem ✓ " : "LegacyEncounterSystem ✗ ";
        systemsMessage += starkkillerEncounterSystem != null ? "StarkkillerEncounterSystem ✓ " : "StarkkillerEncounterSystem ✗ ";
        systemsMessage += shipGeneratorCoordinator != null ? "ShipGeneratorCoordinator ✓ " : "ShipGeneratorCoordinator ✗ ";
        
        Debug.Log(systemsMessage);
        
        // Check if the selected active system is available
        bool activeSystemAvailable = 
            (activeSystem == EncounterSystemType.MasterShipGenerator && masterShipGenerator != null) ||
            (activeSystem == EncounterSystemType.LegacyEncounterSystem && legacyEncounterSystem != null) ||
            (activeSystem == EncounterSystemType.StarkkillerEncounterSystem && starkkillerEncounterSystem != null) ||
            (activeSystem == EncounterSystemType.ShipGeneratorCoordinator && shipGeneratorCoordinator != null);
            
        if (!activeSystemAvailable)
        {
            Debug.LogError("Selected active system is not available! Will attempt to use an available system instead.");
        }
    }
    
    /// <summary>
    /// Delayed initialization of systems to avoid race conditions
    /// </summary>
    private IEnumerator DelayedInitialization()
    {
        Debug.Log("EncounterSystemManager starting delayed initialization...");
        
        // Wait a moment to let all systems initialize normally first
        yield return new WaitForSeconds(initializationDelay);
        
        // Find any missing references again in case they were created after Awake
        FindSystemReferences();
        
        // Ensure only the active system is enabled
        EnableOnlyActiveSystem();
        
        // Short delay before ensuring credential checker has the right references
        yield return new WaitForSeconds(initializationDelay);
        
        // Ensure credential checker is connected to the active system
        ConnectCredentialChecker();
        
        yield return new WaitForSeconds(initializationDelay);
        
        // Check if a GameStateController exists and is in the active gameplay state before requesting encounters
        GameStateController gameStateController = FindFirstObjectByType<GameStateController>();
        bool shouldRequestInitialEncounter = true;
        
        if (gameStateController != null)
        {
            if (verboseLogging) 
                Debug.Log("Found GameStateController, checking game state before requesting encounter...");
                
            // Only request initial encounter if in active gameplay state
            if (!gameStateController.IsGameplayActive())
            {
                shouldRequestInitialEncounter = false;
                if (verboseLogging)
                    Debug.Log("Game is not in active gameplay state, deferring initial encounter request");
            }
        }
        
        // Request the first encounter to start the game
        if (shouldRequestInitialEncounter)
        {
            RequestInitialEncounter();
        }
        else
        {
            Debug.Log("Initial encounter request deferred until game enters active gameplay state");
        }
        
        isInitialized = true;
        Debug.Log("EncounterSystemManager initialization complete");
    }
    
    /// <summary>
    /// Enables only the active encounter system and disables others
    /// </summary>
    private void EnableOnlyActiveSystem()
    {
        // Instead of disabling game objects (which might disable other components 
        // like VideoPlayers), just disable the script components
        
        // Disable all system components first
        if (masterShipGenerator != null)
            masterShipGenerator.enabled = false;
            
        if (legacyEncounterSystem != null)
            legacyEncounterSystem.enabled = false;
            
        if (starkkillerEncounterSystem != null)
            starkkillerEncounterSystem.enabled = false;
            
        if (shipGeneratorCoordinator != null)
            shipGeneratorCoordinator.enabled = false;
        
        // Enable only the active system
        switch (activeSystem)
        {
            case EncounterSystemType.MasterShipGenerator:
                if (masterShipGenerator != null)
                {
                    masterShipGenerator.enabled = true;
                    if (verboseLogging) Debug.Log("Activated MasterShipGenerator as primary system");
                }
                else
                {
                    SelectFirstAvailableSystem();
                }
                break;
                
            case EncounterSystemType.LegacyEncounterSystem:
                if (legacyEncounterSystem != null)
                {
                    legacyEncounterSystem.enabled = true;
                    if (verboseLogging) Debug.Log("Activated LegacyEncounterSystem as primary system");
                }
                else
                {
                    SelectFirstAvailableSystem();
                }
                break;
                
            case EncounterSystemType.StarkkillerEncounterSystem:
                if (starkkillerEncounterSystem != null)
                {
                    starkkillerEncounterSystem.enabled = true;
                    if (verboseLogging) Debug.Log("Activated StarkkillerEncounterSystem as primary system");
                }
                else
                {
                    SelectFirstAvailableSystem();
                }
                break;
                
            case EncounterSystemType.ShipGeneratorCoordinator:
                if (shipGeneratorCoordinator != null)
                {
                    shipGeneratorCoordinator.enabled = true;
                    if (verboseLogging) Debug.Log("Activated ShipGeneratorCoordinator as primary system");
                }
                else
                {
                    SelectFirstAvailableSystem();
                }
                break;
        }
    }
    
    /// <summary>
    /// If the selected system is not available, select the first available one
    /// </summary>
    private void SelectFirstAvailableSystem()
    {
        if (masterShipGenerator != null)
        {
            masterShipGenerator.enabled = true;
            activeSystem = EncounterSystemType.MasterShipGenerator;
            if (verboseLogging) Debug.Log("Falling back to MasterShipGenerator");
        }
        else if (legacyEncounterSystem != null)
        {
            legacyEncounterSystem.enabled = true;
            activeSystem = EncounterSystemType.LegacyEncounterSystem;
            if (verboseLogging) Debug.Log("Falling back to LegacyEncounterSystem");
        }
        else if (starkkillerEncounterSystem != null)
        {
            starkkillerEncounterSystem.enabled = true;
            activeSystem = EncounterSystemType.StarkkillerEncounterSystem;
            if (verboseLogging) Debug.Log("Falling back to StarkkillerEncounterSystem");
        }
        else if (shipGeneratorCoordinator != null)
        {
            shipGeneratorCoordinator.enabled = true;
            activeSystem = EncounterSystemType.ShipGeneratorCoordinator;
            if (verboseLogging) Debug.Log("Falling back to ShipGeneratorCoordinator");
        }
        else
        {
            Debug.LogError("No encounter systems available! Game may not function correctly.");
        }
    }
    
    /// <summary>
    /// Ensures the credential checker is connected to the active system
    /// </summary>
    private void ConnectCredentialChecker()
    {
        if (credentialChecker == null)
        {
            Debug.LogError("CredentialChecker reference is missing!");
            return;
        }
        
        // Now connect the credential checker to the active system
        // This ensures only one system provides encounters
        
        switch (activeSystem)
        {
            case EncounterSystemType.MasterShipGenerator:
                if (masterShipGenerator != null)
                {
                    // Safely unsubscribe from any existing event handlers first
                    if (legacyEncounterSystem != null)
                    {
                        // Try to use reflection to find and unsubscribe from legacy events
                        var legacyEvents = legacyEncounterSystem.GetType().GetEvents();
                        foreach (var evt in legacyEvents)
                        {
                            if (verboseLogging) Debug.Log($"Found event in legacy system: {evt.Name}");
                        }
                    }
                    
                    if (starkkillerEncounterSystem != null)
                    {
                        // Try to use reflection to find and unsubscribe from starkiller events
                        var starkillerEvents = starkkillerEncounterSystem.GetType().GetEvents();
                        foreach (var evt in starkillerEvents)
                        {
                            if (verboseLogging) Debug.Log($"Found event in starkiller system: {evt.Name}");
                        }
                    }
                    
                    // Connect to MasterShipGenerator - first remove any existing subscribers
                    masterShipGenerator.OnEncounterReady -= credentialChecker.DisplayEncounter;
                    masterShipGenerator.OnEncounterReady += credentialChecker.DisplayEncounter;
                    
                    if (verboseLogging) 
                        Debug.Log("Connected CredentialChecker to MasterShipGenerator");
                }
                break;
                
            case EncounterSystemType.LegacyEncounterSystem:
                if (legacyEncounterSystem != null && credentialChecker != null)
                {
                    if (verboseLogging)
                        Debug.Log("Attempting to connect CredentialChecker to LegacyEncounterSystem");
                    
                    // Try to find the event and connect using reflection if needed
                    var eventInfo = legacyEncounterSystem.GetType().GetEvent("OnShipGenerated");
                    if (eventInfo != null)
                    {
                        // Use EncounterSystemMigrationManager to create a bridge if available
                        var migrationManager = FindFirstObjectByType<EncounterSystemMigrationManager>();
                        if (migrationManager != null)
                        {
                            migrationManager.CreateBridgeClasses();
                            if (verboseLogging)
                                Debug.Log("Used EncounterSystemMigrationManager to create bridge classes");
                        }
                        else
                        {
                            Debug.LogWarning("EncounterSystemMigrationManager not found - limited legacy system support");
                        }
                    }
                }
                break;
                
            case EncounterSystemType.StarkkillerEncounterSystem:
                if (starkkillerEncounterSystem != null && credentialChecker != null)
                {
                    if (verboseLogging)
                        Debug.Log("Attempting to connect CredentialChecker to StarkkillerEncounterSystem");
                    
                    // Similar approach for Starkiller system
                    // Would need a bridge or adapter method
                }
                break;
            
            default:
                Debug.LogWarning("Only MasterShipGenerator connection is fully implemented");
                break;
        }
    }
    
    /// <summary>
    /// Requests the initial encounter to start the game and tries multiple times if needed
    /// </summary>
    private void RequestInitialEncounter()
    {
        if (credentialChecker == null)
        {
            Debug.LogError("Cannot request initial encounter - CredentialChecker is null");
            return;
        }
        
        // Check if we should request a new encounter or if one might already be active
        if (credentialChecker.HasActiveEncounter())
        {
            Debug.Log("CredentialChecker already has an active encounter, skipping initial request");
            return;
        }
        
        // Try multiple times with increasing delays if needed
        StartCoroutine(EnsureInitialEncounterLoaded());
    }
    
    /// <summary>
    /// Ensures that an initial encounter is loaded, retrying several times if needed
    /// </summary>
    private IEnumerator EnsureInitialEncounterLoaded()
    {
        int maxRetries = 5;
        
        // First attempt
        if (verboseLogging)
            Debug.Log("Requesting initial encounter from active system");
            
        // Request the first encounter
        credentialChecker.NextEncounter();
        
        // Wait a moment to let it process
        yield return new WaitForSeconds(0.5f);
        
        // Check if we have an encounter
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            // Check if the credential checker has an encounter
            if (credentialChecker.HasActiveEncounter())
            {
                Debug.Log($"Successfully loaded initial encounter on attempt {attempt}");
                yield break;
            }
            
            // If not, try again with increasing delay
            float delay = attempt * 0.5f;
            Debug.LogWarning($"No active encounter found. Retrying in {delay}s (attempt {attempt}/{maxRetries})");
            
            yield return new WaitForSeconds(delay);
            
            // Try again with different approach if first attempt failed
            if (attempt > 1 && masterShipGenerator != null && activeSystem == EncounterSystemType.MasterShipGenerator)
            {
                // Try to request directly from the generator
                MasterShipEncounter emergencyEncounter = masterShipGenerator.GetNextEncounter();
                if (emergencyEncounter != null)
                {
                    Debug.Log("Got encounter directly from MasterShipGenerator");
                    credentialChecker.DisplayEncounter(emergencyEncounter);
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
            }
            
            // Try to get a new encounter through the normal channel
            credentialChecker.NextEncounter();
            
            // Wait for it to process
            yield return new WaitForSeconds(0.5f);
        }
        
        // Final attempt
        if (!credentialChecker.HasActiveEncounter())
        {
            Debug.LogError("Failed to load an initial encounter after multiple attempts!");
            
            // Force creation of emergency encounter
            Debug.Log("Creating emergency encounter");
            MasterShipEncounter emergencyEncounter = MasterShipEncounter.CreateTestEncounter();
            credentialChecker.DisplayEncounter(emergencyEncounter);
        }
    }
    
    /// <summary>
    /// Called by GameStateController when game state changes
    /// </summary>
    public void OnGameStateChanged(GameStateController.GameActivationState newState)
    {
        if (verboseLogging)
            Debug.Log($"EncounterSystemManager notified of game state change to: {newState}");
        
        // If the game enters active gameplay state and we're already initialized,
        // we might need to ensure an encounter is loaded
        if (newState == GameStateController.GameActivationState.ActiveGameplay && isInitialized)
        {
            if (credentialChecker != null && !credentialChecker.HasActiveEncounter())
            {
                Debug.Log("Game entered active state but no encounter is loaded. Requesting initial encounter...");
                RequestInitialEncounter();
            }
        }
    }
    
    /// <summary>
    /// Public method to manually switch the active system
    /// </summary>
    public void SetActiveSystem(EncounterSystemType newSystem)
    {
        if (newSystem == activeSystem)
            return;
            
        Debug.Log($"Manually switching active system from {activeSystem} to {newSystem}");
        activeSystem = newSystem;
        
        // Re-apply the configuration
        EnableOnlyActiveSystem();
        ConnectCredentialChecker();
    }
}