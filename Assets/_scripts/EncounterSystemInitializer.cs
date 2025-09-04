using System.Collections;
using UnityEngine;

/// <summary>
/// Ensures proper initialization of the encounter system after daily briefing
/// Resolves timing issues between GameStateController and EncounterSystemCoordinator
/// </summary>
public class EncounterSystemInitializer : MonoBehaviour
{
    [Tooltip("Time to wait after briefing closes before forcing initialization")]
    public float initDelayAfterBriefing = 1.0f;
    
    [Tooltip("Enable debug logging")]
    public bool debugLogging = true;
    
    private bool initialized = false;
    
    void Start()
    {
        // Find the GameStateController to monitor state changes
        GameStateController gameStateController = FindFirstObjectByType<GameStateController>();
        if (gameStateController != null)
        {
            // Subscribe to state change events
            gameStateController.OnGameStateChanged += OnGameStateChanged;
            
            if (debugLogging)
                Debug.Log("EncounterSystemInitializer: Subscribed to GameStateController events");
        }
        else
        {
            if (debugLogging)
                Debug.LogWarning("EncounterSystemInitializer: GameStateController not found!");
        }
    }
    
    /// <summary>
    /// Handles game state changes to initialize encounter systems at the right time
    /// </summary>
    private void OnGameStateChanged(GameStateController.GameActivationState newState)
    {
        if (debugLogging)
            Debug.Log($"EncounterSystemInitializer: Game state changed to {newState}");
            
        // We only care about transition to active gameplay
        if (newState == GameStateController.GameActivationState.ActiveGameplay && !initialized)
        {
            // Wait a short delay then initialize systems
            StartCoroutine(InitializeSystems());
        }
    }
    
    /// <summary>
    /// Initialize all encounter systems
    /// </summary>
    private IEnumerator InitializeSystems()
    {
        // Wait for daily briefing to completely finish
        yield return new WaitForSeconds(initDelayAfterBriefing);
        
        if (debugLogging)
            Debug.Log("EncounterSystemInitializer: Beginning system initialization");
        
        // First ensure MasterShipGenerator exists and is properly configured
        MasterShipGenerator shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        CredentialChecker credentialChecker = FindFirstObjectByType<CredentialChecker>();
        
        if (shipGenerator == null)
        {
            Debug.LogError("EncounterSystemInitializer: MasterShipGenerator not found!");
            
            // Try to find it through EncounterSystemManager
            EncounterSystemManager systemManager = FindFirstObjectByType<EncounterSystemManager>();
            if (systemManager != null)
            {
                Component activeSystem = systemManager.GetActiveEncounterSystem();
                if (activeSystem is MasterShipGenerator)
                {
                    shipGenerator = (MasterShipGenerator)activeSystem;
                    if (debugLogging)
                        Debug.Log("EncounterSystemInitializer: Retrieved MasterShipGenerator from EncounterSystemManager");
                }
            }
        }
        
        // Connect MasterShipGenerator to CredentialChecker if needed
        if (shipGenerator != null && credentialChecker != null)
        {
            // Remove any existing connection to avoid duplicates
            shipGenerator.OnEncounterReady -= credentialChecker.DisplayEncounter;
            
            // Now connect them
            shipGenerator.OnEncounterReady += credentialChecker.DisplayEncounter;
            
            if (debugLogging)
                Debug.Log("EncounterSystemInitializer: Connected MasterShipGenerator to CredentialChecker");
        }
        
        // Force UI visibility in CredentialChecker
        if (credentialChecker != null)
        {
            if (debugLogging)
                Debug.Log("EncounterSystemInitializer: Forcing UI visibility in CredentialChecker");
                
            credentialChecker.ForceUIVisibility();
        }
        
        // Force a reload of the encounter state if needed
        if (shipGenerator != null)
        {
            // Check if there is already an active encounter
            bool hasActiveEncounter = credentialChecker != null && credentialChecker.HasActiveEncounter();
            
            if (!hasActiveEncounter)
            {
                if (debugLogging)
                    Debug.Log("EncounterSystemInitializer: No active encounter found, requesting one from generator");
                    
                // Check if timing controller allows generating a new ship
                ShipTimingController timingController = FindFirstObjectByType<ShipTimingController>();
                if (timingController != null)
                {
                    if (timingController.CanGenerateNewShip("EncounterSystemInitializer"))
                    {
                        if (debugLogging)
                            Debug.Log("EncounterSystemInitializer: Timing controller approved, generating encounter");
                            
                        shipGenerator.GetNextEncounter();
                    }
                    else
                    {
                        // Reset timing controller state if needed
                        if (debugLogging)
                            Debug.Log("EncounterSystemInitializer: Timing controller denied, forcing reset");
                            
                        timingController.ResetCooldown();
                        shipGenerator.GetNextEncounter();
                    }
                }
                else
                {
                    // No timing controller, just generate directly
                    shipGenerator.GetNextEncounter();
                }
            }
            else
            {
                if (debugLogging)
                    Debug.Log("EncounterSystemInitializer: Active encounter already exists");
            }
        }
        
        initialized = true;
        
        if (debugLogging)
            Debug.Log("EncounterSystemInitializer: System initialization complete");
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        GameStateController gameStateController = FindFirstObjectByType<GameStateController>();
        if (gameStateController != null)
        {
            gameStateController.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}