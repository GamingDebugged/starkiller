using System.Collections;
using UnityEngine;
using StarkillerBaseCommand;

/// <summary>
/// Ensures the first encounter is displayed after the daily briefing
/// Fixes the issue where player must blindly press Approve/Deny
/// </summary>
public class FirstEncounterTrigger : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Delay after briefing closes before showing first encounter")]
    public float encounterDelay = 1.0f;
    
    [Tooltip("Enable debug logging")]
    public bool debugLogging = true;
    
    private bool firstEncounterTriggered = false;
    
    void Start()
    {
        // Find the DailyBriefingManager to monitor when briefing ends
        DailyBriefingManager briefingManager = FindFirstObjectByType<DailyBriefingManager>();
        if (briefingManager != null)
        {
            // Subscribe to the GameManager's day start events
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                if (debugLogging)
                    Debug.Log("FirstEncounterTrigger: Ready to monitor for daily briefing completion");
            }
        }
    }
    
    /// <summary>
    /// Called by GameManager when daily briefing is completed
    /// </summary>
    public void OnDailyBriefingCompleted()
    {
        // Reset the trigger flag for the new day
        firstEncounterTriggered = false;
        
        if (debugLogging)
            Debug.Log($"FirstEncounterTrigger: Daily briefing completed, resetting flag and triggering first encounter");
            
        StartCoroutine(TriggerFirstEncounter());
    }
    
    /// <summary>
    /// Triggers the first encounter with appropriate delay and safety checks
    /// </summary>
    private IEnumerator TriggerFirstEncounter()
    {
        // Prevent multiple triggers
        if (firstEncounterTriggered)
        {
            if (debugLogging)
                Debug.LogWarning("FirstEncounterTrigger: Already triggered, skipping");
            yield break;
        }
        
        firstEncounterTriggered = true;
        
        // Wait for the briefing to fully close and systems to initialize
        yield return new WaitForSeconds(encounterDelay);
        
        // Find the MasterShipGenerator
        MasterShipGenerator shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        if (shipGenerator == null)
        {
            Debug.LogError("FirstEncounterTrigger: MasterShipGenerator not found!");
            yield break;
        }
        
        // Find the CredentialChecker
        CredentialChecker credentialChecker = FindFirstObjectByType<CredentialChecker>();
        if (credentialChecker == null)
        {
            Debug.LogError("FirstEncounterTrigger: CredentialChecker not found!");
            yield break;
        }
        
        // Ensure game state is active
        GameStateController gameStateController = GameStateController.Instance;
        if (gameStateController != null)
        {
            // Wait until game state is truly active
            int attempts = 0;
            while (!gameStateController.IsGameplayActive() && attempts < 10)
            {
                if (debugLogging)
                    Debug.Log("FirstEncounterTrigger: Waiting for game state to be active...");
                    
                yield return new WaitForSeconds(0.5f);
                attempts++;
            }
            
            if (!gameStateController.IsGameplayActive())
            {
                Debug.LogWarning("FirstEncounterTrigger: Game state not active after waiting, forcing activation");
                gameStateController.SetGameState(GameStateController.GameActivationState.ActiveGameplay);
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        // Reset any timing cooldowns
        ShipTimingController timingController = FindFirstObjectByType<ShipTimingController>();
        if (timingController != null)
        {
            if (debugLogging)
                Debug.Log("FirstEncounterTrigger: Resetting timing controller");
            timingController.ResetCooldown();
        }
        
        // Ensure timer is started for the day
        var shiftTimerManager = Starkiller.Core.ServiceLocator.Get<Starkiller.Core.Managers.ShiftTimerManager>();
        if (shiftTimerManager != null)
        {
            if (!shiftTimerManager.IsTimerActive)
            {
                if (debugLogging)
                    Debug.Log("FirstEncounterTrigger: Timer not active - starting timer");
                shiftTimerManager.StartTimer();
            }
            else
            {
                if (debugLogging)
                    Debug.Log("FirstEncounterTrigger: Timer already active");
            }
        }
        else
        {
            Debug.LogError("FirstEncounterTrigger: ShiftTimerManager not found!");
        }
        
        // Force the CredentialChecker to be ready
        credentialChecker.ForceUIVisibility();
        credentialChecker.ForceActiveGameplay();
        
        // Small additional delay to ensure everything is ready
        yield return new WaitForSeconds(0.2f);
        
        // Get the first encounter
        if (debugLogging)
            Debug.Log("FirstEncounterTrigger: Requesting first encounter from MasterShipGenerator");
            
        MasterShipEncounter firstEncounter = shipGenerator.GetNextEncounter();
        
        if (firstEncounter != null)
        {
            if (debugLogging)
                Debug.Log($"FirstEncounterTrigger: Got encounter: {firstEncounter.shipType} - {firstEncounter.captainName}");
                
            // Display the encounter safely with a small delay
            credentialChecker.DisplayEncounterSafe(firstEncounter);
            
            if (debugLogging)
                Debug.Log("FirstEncounterTrigger: First encounter displayed successfully");
                
            // Start the continuous encounter spawning by calling GameManager.StartDay()
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                if (debugLogging)
                    Debug.Log("FirstEncounterTrigger: Starting continuous encounter spawning");
                gameManager.StartDay();
            }
        }
        else
        {
            Debug.LogError("FirstEncounterTrigger: Failed to get first encounter from MasterShipGenerator!");
        }
        
        // Reset flag for next day
        firstEncounterTriggered = false;
    }
    
    /// <summary>
    /// Reset the trigger for the next day
    /// </summary>
    public void Reset()
    {
        firstEncounterTriggered = false;
        if (debugLogging)
            Debug.Log("FirstEncounterTrigger: Reset for new day");
    }
}