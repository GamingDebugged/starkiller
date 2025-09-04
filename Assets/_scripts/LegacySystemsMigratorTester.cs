using UnityEngine;
using StarkillerBaseCommand;

/// <summary>
/// Test class to verify the functionality of the LegacySystemsMigrator
/// </summary>
public class LegacySystemsMigratorTester : MonoBehaviour
{
    [SerializeField] private LegacySystemsMigrator migrator;
    
    [Header("Test Settings")]
    [SerializeField] private bool runTestOnStart = false;
    [SerializeField] private bool verboseLogging = true;
    
    private void Start()
    {
        // Find migrator if not assigned
        if (migrator == null)
        {
            migrator = FindFirstObjectByType<LegacySystemsMigrator>();
            if (migrator == null)
            {
                Debug.LogError("LegacySystemsMigratorTester: No LegacySystemsMigrator found in the scene!");
                return;
            }
        }
        
        // Run test if enabled
        if (runTestOnStart)
        {
            RunTest();
        }
    }
    
    /// <summary>
    /// Run test functions to verify the migrator
    /// </summary>
    public void RunTest()
    {
        if (migrator == null)
        {
            Debug.LogError("Cannot run test - migrator is null");
            return;
        }
        
        if (verboseLogging)
        {
            Debug.Log("Starting LegacySystemsMigrator tests");
        }
        
        // Test finding systems
        TestFindSystems();
        
        // Test migration methods
        TestMigration();
        
        if (verboseLogging)
        {
            Debug.Log("LegacySystemsMigrator tests completed");
        }
    }
    
    /// <summary>
    /// Test finding system references
    /// </summary>
    private void TestFindSystems()
    {
        if (verboseLogging)
        {
            Debug.Log("Testing FindSystemReferences");
        }
        
        // Test FindSystemReferences
        migrator.FindSystemReferences();
        
        // Test FindLegacySystems
        migrator.FindLegacySystems();
        
        // Test FindNewSystems
        migrator.FindNewSystems();
    }
    
    /// <summary>
    /// Test migration methods
    /// </summary>
    private void TestMigration()
    {
        if (verboseLogging)
        {
            Debug.Log("Testing migration methods");
        }
        
        // Test MigrateShipData
        migrator.MigrateShipData();
        
        // Test MigrateMediaData
        migrator.MigrateMediaData();
        
        // Test MigrateGameplaySettings
        migrator.MigrateGameplaySettings();
        
        // Test DisableLegacySystems
        migrator.DisableLegacySystems();
        
        // Test MigrateAll
        migrator.MigrateAll();
    }
    
    /// <summary>
    /// Button to run the test from the inspector
    /// </summary>
    [ContextMenu("Run Migration Test")]
    public void RunTestButton()
    {
        RunTest();
    }
}