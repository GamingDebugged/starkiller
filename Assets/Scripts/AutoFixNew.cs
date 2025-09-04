using UnityEngine;
using System.Collections;
using System.Reflection;

/// <summary>
/// Automatic fix script to resolve common initialization issues
/// </summary>
public class AutoFixNew : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ApplyFixes());
    }
    
    IEnumerator ApplyFixes()
    {
        // Wait a frame to ensure everything is initialized
        yield return null;
        
        Debug.Log("=== AutoFix: Starting automatic fixes ===");
        
        // 1. Fix timing issues
        FixTimingSettings();
        
        // 2. Fix missing references
        FixMissingReferences();
        
        // 3. Fix video players
        FixVideoPlayers();
        
        // 4. Clean up TestingFramework warnings
        FixTestingFramework();
        
        // 5. Ensure proper game state
        EnsureProperGameState();
        
        Debug.Log("=== AutoFix: All fixes applied ===");
        
        // Self-destruct after fixes are applied
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    
    void FixTimingSettings()
    {
        Debug.Log("AutoFix: Fixing timing settings...");
        
        // Fix GameManager timing
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.timeBetweenShips = 5f; // 5 seconds between ships
            // gm.shiftTimeLimit = 180f; // 3 minutes per shift - DISABLED: Keep default 30 seconds
            Debug.Log("AutoFix: GameManager timing fixed (kept 30s shift limit)");
        }
        
        // Fix ShipTimingController using reflection
        ShipTimingController timingController = FindAnyObjectByType<ShipTimingController>();
        if (timingController != null)
        {
            try
            {
                var type = typeof(ShipTimingController);
                
                // Set timeBetweenShips
                var timeBetweenShipsField = type.GetField("timeBetweenShips", BindingFlags.NonPublic | BindingFlags.Instance);
                if (timeBetweenShipsField != null)
                    timeBetweenShipsField.SetValue(timingController, 5f);
                
                // Set minimumEncounterDuration
                var minDurationField = type.GetField("minimumEncounterDuration", BindingFlags.NonPublic | BindingFlags.Instance);
                if (minDurationField != null)
                    minDurationField.SetValue(timingController, 3f);
                
                // Set verboseLogging
                var verboseField = type.GetField("verboseLogging", BindingFlags.NonPublic | BindingFlags.Instance);
                if (verboseField != null)
                    verboseField.SetValue(timingController, false);
                
                Debug.Log("AutoFix: ShipTimingController timing fixed");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"AutoFix: Could not fix ShipTimingController: {e.Message}");
            }
        }
        
        // Fix MasterShipGenerator using reflection
        MasterShipGenerator masterGen = FindAnyObjectByType<MasterShipGenerator>();
        if (masterGen != null)
        {
            try
            {
                var type = typeof(MasterShipGenerator);
                var encountersField = type.GetField("encountersPerDay", BindingFlags.NonPublic | BindingFlags.Instance);
                if (encountersField != null)
                {
                    encountersField.SetValue(masterGen, 20);
                    Debug.Log("AutoFix: MasterShipGenerator encounters per day fixed");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"AutoFix: Could not fix MasterShipGenerator: {e.Message}");
            }
        }
    }
    
    void FixMissingReferences()
    {
        Debug.Log("AutoFix: Fixing missing references...");
        
        // Get main components
        MasterShipGenerator masterGen = FindAnyObjectByType<MasterShipGenerator>();
        CredentialChecker credChecker = FindAnyObjectByType<CredentialChecker>();
        GameManager gm = FindAnyObjectByType<GameManager>();
        
        // Fix CredentialChecker's MasterShipGenerator reference
        if (credChecker != null && masterGen != null)
        {
            try
            {
                // Use reflection to set private field
                var field = typeof(CredentialChecker).GetField("masterShipGenerator", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(credChecker, masterGen);
                    Debug.Log("AutoFix: Fixed CredentialChecker->MasterShipGenerator reference");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"AutoFix: Could not fix CredentialChecker reference: {e.Message}");
            }
        }
        
        // Fix GameManager's MasterShipGenerator reference
        if (gm != null && masterGen != null)
        {
            gm.masterShipGenerator = masterGen;
            Debug.Log("AutoFix: Fixed GameManager->MasterShipGenerator reference");
        }
        
        // Fix DebugMonitor reference
        DebugMonitor debugMonitor = FindAnyObjectByType<DebugMonitor>();
        if (debugMonitor != null && masterGen != null)
        {
            try
            {
                var field = typeof(DebugMonitor).GetField("masterShipGenerator", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(debugMonitor, masterGen);
                    Debug.Log("AutoFix: Fixed DebugMonitor->MasterShipGenerator reference");
                }
            }
            catch { }
        }
    }
    
    void FixVideoPlayers()
    {
        Debug.Log("AutoFix: Fixing video players...");
        
        CredentialChecker credChecker = FindAnyObjectByType<CredentialChecker>();
        if (credChecker != null)
        {
            // Ensure video players are initialized
            try
            {
                var initMethod = typeof(CredentialChecker).GetMethod("InitializeVideoPlayers", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (initMethod != null)
                {
                    initMethod.Invoke(credChecker, null);
                    Debug.Log("AutoFix: Reinitialized video players");
                }
            }
            catch { }
        }
    }
    
    void FixTestingFramework()
    {
        Debug.Log("AutoFix: Fixing TestingFramework...");
        
        TestingFramework testFramework = FindAnyObjectByType<TestingFramework>();
        if (testFramework != null)
        {
            // Since ShipEncounterSystem doesn't exist anymore, null out the reference
            testFramework.shipSystem = null;
            
            // Make sure it has references to new systems
            if (testFramework.gameManager == null)
                testFramework.gameManager = FindAnyObjectByType<GameManager>();
                
            if (testFramework.credentialChecker == null)
                testFramework.credentialChecker = FindAnyObjectByType<CredentialChecker>();
                
            Debug.Log("AutoFix: TestingFramework references updated");
        }
    }
    
    void EnsureProperGameState()
    {
        Debug.Log("AutoFix: Ensuring proper game state...");
        
        GameStateController stateController = FindAnyObjectByType<GameStateController>();
        if (stateController != null)
        {
            // Check actual enum values available
            var currentState = stateController.GetCurrentState();
            Debug.Log($"AutoFix: Current game state is {currentState}");
            
            // Only try to fix if we know what states are available
            // This will depend on your actual GameStateController implementation
        }
        
        // Try to reduce DebugMonitor verbosity using reflection
        DebugMonitor debugMonitor = FindAnyObjectByType<DebugMonitor>();
        if (debugMonitor != null)
        {
            try
            {
                var type = typeof(DebugMonitor);
                var verboseField = type.GetField("verboseLogging", BindingFlags.NonPublic | BindingFlags.Instance);
                if (verboseField != null)
                {
                    verboseField.SetValue(debugMonitor, false);
                    Debug.Log("AutoFix: Disabled verbose logging in DebugMonitor");
                }
            }
            catch { }
        }
    }
}