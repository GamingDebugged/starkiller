using UnityEngine;
using System.Collections;

/// <summary>
/// Simple startup initializer to fix common issues
/// Place this on a GameObject at the root of your hierarchy
/// </summary>
[DefaultExecutionOrder(-1000)] // Execute before everything else
public class StartupInitializer : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("=== StartupInitializer: Beginning initialization ===");
        
        // Ensure this GameObject persists
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        
        // Start initialization coroutine
        StartCoroutine(Initialize());
    }
    
    IEnumerator Initialize()
    {
        // Wait one frame to ensure all objects are created
        yield return null;
        
        // Fix missing MasterShipGenerator reference
        FixMasterShipGeneratorReferences();
        
        // Fix TestingFramework references
        FixTestingFramework();
        
        // Ensure managers are at root level
        FixManagerHierarchy();
        
        Debug.Log("=== StartupInitializer: Initialization complete ===");
    }
    
    void FixMasterShipGeneratorReferences()
    {
        Debug.Log("StartupInitializer: Fixing MasterShipGenerator references...");
        
        // Find MasterShipGenerator
        MasterShipGenerator masterGen = FindAnyObjectByType<MasterShipGenerator>();
        if (masterGen == null)
        {
            Debug.LogWarning("StartupInitializer: MasterShipGenerator not found!");
            return;
        }
        
        // Fix GameManager reference
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null && gm.masterShipGenerator == null)
        {
            gm.masterShipGenerator = masterGen;
            Debug.Log("StartupInitializer: Fixed GameManager->MasterShipGenerator reference");
        }
        
        // Fix DebugMonitor reference using reflection
        DebugMonitor debugMonitor = FindAnyObjectByType<DebugMonitor>();
        if (debugMonitor != null)
        {
            try
            {
                var field = typeof(DebugMonitor).GetField("masterShipGenerator", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(debugMonitor, masterGen);
                    Debug.Log("StartupInitializer: Fixed DebugMonitor->MasterShipGenerator reference");
                }
            }
            catch { }
        }
        
        // Fix CredentialChecker reference
        CredentialChecker credChecker = FindAnyObjectByType<CredentialChecker>();
        if (credChecker != null)
        {
            try
            {
                var field = typeof(CredentialChecker).GetField("masterShipGenerator", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(credChecker, masterGen);
                    Debug.Log("StartupInitializer: Fixed CredentialChecker->MasterShipGenerator reference");
                }
            }
            catch { }
        }
    }
    
    void FixTestingFramework()
    {
        Debug.Log("StartupInitializer: Fixing TestingFramework...");
        
        TestingFramework testFramework = FindAnyObjectByType<TestingFramework>();
        if (testFramework != null)
        {
            // ShipEncounterSystem doesn't exist anymore
            testFramework.shipSystem = null;
            
            // Make sure it has references to new systems
            if (testFramework.gameManager == null)
                testFramework.gameManager = FindAnyObjectByType<GameManager>();
                
            if (testFramework.credentialChecker == null)
                testFramework.credentialChecker = FindAnyObjectByType<CredentialChecker>();
                
            Debug.Log("StartupInitializer: TestingFramework references updated");
        }
    }
    
    void FixManagerHierarchy()
    {
        Debug.Log("StartupInitializer: Fixing manager hierarchy...");
        
        // Move important managers to root level
        MoveToRoot<DebugMonitor>("DebugMonitor");
        MoveToRoot<GameStateController>("GameStateController");
        MoveToRoot<EncounterSystemMigrationManager>("EncounterSystemMigrationManager");
        
        // For NarrativeManager, we need to use the full namespace
        var narrativeManager = FindAnyObjectByType<StarkillerBaseCommand.NarrativeManager>();
        if (narrativeManager != null && narrativeManager.transform.parent != null)
        {
            narrativeManager.transform.SetParent(null);
            DontDestroyOnLoad(narrativeManager.gameObject);
            Debug.Log("StartupInitializer: Moved NarrativeManager to root");
        }
    }
    
    void MoveToRoot<T>(string typeName) where T : Component
    {
        T component = FindAnyObjectByType<T>();
        if (component != null && component.transform.parent != null)
        {
            component.transform.SetParent(null);
            DontDestroyOnLoad(component.gameObject);
            Debug.Log($"StartupInitializer: Moved {typeName} to root");
        }
    }
}