using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// Simple game reset to fix broken state
/// </summary>
[DefaultExecutionOrder(-2000)] // Run before everything
public class GameReset : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("=== GameReset: Resetting game state ===");
        
        // Reset time scale
        Time.timeScale = 1f;
        
        // Clear any existing singletons
        ResetSingletons();
        
        // Start reset coroutine
        StartCoroutine(ResetGame());
    }
    
    void ResetSingletons()
    {
        // Clear MasterShipGenerator instance
        var instanceField = typeof(MasterShipGenerator).GetField("instance", 
            BindingFlags.NonPublic | BindingFlags.Static);
        if (instanceField != null)
        {
            instanceField.SetValue(null, null);
            Debug.Log("GameReset: Cleared MasterShipGenerator singleton");
        }
    }
    
    IEnumerator ResetGame()
    {
        // Wait for initialization
        yield return null;
        
        // Find and fix core components
        FixCoreComponents();
        
        // Fix game state
        FixGameState();
        
        // Fix timing
        FixTiming();
        
        Debug.Log("=== GameReset: Reset complete ===");
        
        // Self-destruct
        Destroy(gameObject, 2f);
    }
    
    void FixCoreComponents()
    {
        Debug.Log("GameReset: Fixing core components...");
        
        // Ensure MasterShipGenerator exists and is properly set up
        MasterShipGenerator masterGen = FindFirstObjectByType<MasterShipGenerator>();
        if (masterGen != null)
        {
            // Move to root if needed
            if (masterGen.transform.parent != null)
            {
                masterGen.transform.SetParent(null);
            }
            
            // Set as singleton instance
            var instanceField = typeof(MasterShipGenerator).GetField("instance", 
                BindingFlags.NonPublic | BindingFlags.Static);
            if (instanceField != null)
            {
                instanceField.SetValue(null, masterGen);
            }
            
            // Fix references
            FixReferences(masterGen);
        }
        else
        {
            Debug.LogError("GameReset: MasterShipGenerator not found!");
        }
    }
    
    void FixReferences(MasterShipGenerator masterGen)
    {
        // Fix GameManager
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.masterShipGenerator = masterGen;
            gm.timeBetweenShips = 5f;
            // gm.shiftTimeLimit = 180f; // DISABLED: Keep default 30 seconds
        }
        
        // Fix CredentialChecker
        CredentialChecker cred = FindFirstObjectByType<CredentialChecker>();
        if (cred != null)
        {
            SetPrivateField(cred, "masterShipGenerator", masterGen);
        }
        
        // Fix DebugMonitor
        DebugMonitor debug = FindFirstObjectByType<DebugMonitor>();
        if (debug != null)
        {
            SetPrivateField(debug, "masterShipGenerator", masterGen);
        }
    }
    
    void FixGameState()
    {
        Debug.Log("GameReset: Fixing game state...");
        
        GameStateController stateController = FindFirstObjectByType<GameStateController>();
        if (stateController != null)
        {
            // Reset to main menu state
            stateController.SetGameState(GameStateController.GameActivationState.MainMenu);
            
            // Move to root if needed
            if (stateController.transform.parent != null)
            {
                stateController.transform.SetParent(null);
                DontDestroyOnLoad(stateController.gameObject);
            }
        }
    }
    
    void FixTiming()
    {
        Debug.Log("GameReset: Fixing timing...");
        
        ShipTimingController timing = FindFirstObjectByType<ShipTimingController>();
        if (timing != null)
        {
            // Reset all timing values
            SetPrivateField(timing, "timeBetweenShips", 5f);
            SetPrivateField(timing, "minimumEncounterDuration", 3f);
            SetPrivateField(timing, "holdingPatternCheckInterval", 1f);
            SetPrivateField(timing, "encounterStartTime", -1f);
            SetPrivateField(timing, "lastEncounterEndTime", -1f);
            
            // Clear any pending actions
            timing.StopAllCoroutines();
        }
        
        // Reset MasterShipGenerator timing
        MasterShipGenerator masterGen = FindFirstObjectByType<MasterShipGenerator>();
        if (masterGen != null)
        {
            SetPrivateField(masterGen, "encountersPerDay", 20);
            SetPrivateField(masterGen, "isProcessingDecision", false);
            
            // Clear encounter queue
            var queueField = typeof(MasterShipGenerator).GetField("encounterQueue", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (queueField != null)
            {
                var queue = queueField.GetValue(masterGen);
                if (queue != null)
                {
                    var clearMethod = queue.GetType().GetMethod("Clear");
                    if (clearMethod != null)
                    {
                        clearMethod.Invoke(queue, null);
                        Debug.Log("GameReset: Cleared encounter queue");
                    }
                }
            }
        }
    }
    
    void SetPrivateField(object obj, string fieldName, object value)
    {
        if (obj == null) return;
        
        var type = obj.GetType();
        var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }
}