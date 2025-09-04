using UnityEngine;
using StarkillerBaseCommand;

/// <summary>
/// Simple test script for the LegacySystemsMigrator
/// This script sets up a minimal testing environment for the migrator
/// </summary>
public class MigratorTestStarter : MonoBehaviour
{
    [Header("References")]
    public LegacySystemsMigrator migrator;
    public LegacySystemsMigratorTester tester;
    
    [Header("Test Settings")]
    public bool createComponentsOnStart = true;
    public bool runMigratorTestOnStart = true;
    
    void Start()
    {
        if (createComponentsOnStart)
        {
            CreateRequiredComponents();
        }
        
        if (runMigratorTestOnStart && tester != null)
        {
            // Wait a frame to ensure everything is initialized
            Invoke("RunMigratorTest", 0.5f);
        }
    }
    
    void RunMigratorTest()
    {
        if (tester != null)
        {
            Debug.Log("MigratorTestStarter: Running migrator test");
            tester.RunTest();
        }
        else
        {
            Debug.LogError("MigratorTestStarter: LegacySystemsMigratorTester reference is missing!");
        }
    }
    
    /// <summary>
    /// Create the minimal components needed for testing the migrator
    /// </summary>
    void CreateRequiredComponents()
    {
        // Check if migrator exists
        if (migrator == null)
        {
            GameObject migratorObj = GameObject.Find("LegacySystemsMigrator");
            if (migratorObj == null)
            {
                migratorObj = new GameObject("LegacySystemsMigrator");
                migrator = migratorObj.AddComponent<LegacySystemsMigrator>();
                Debug.Log("Created LegacySystemsMigrator GameObject");
            }
            else
            {
                migrator = migratorObj.GetComponent<LegacySystemsMigrator>();
                if (migrator == null)
                {
                    migrator = migratorObj.AddComponent<LegacySystemsMigrator>();
                    Debug.Log("Added LegacySystemsMigrator component to existing GameObject");
                }
            }
        }
        
        // Check if tester exists
        if (tester == null)
        {
            GameObject testerObj = GameObject.Find("LegacySystemsMigratorTester");
            if (testerObj == null)
            {
                testerObj = new GameObject("LegacySystemsMigratorTester");
                tester = testerObj.AddComponent<LegacySystemsMigratorTester>();
                Debug.Log("Created LegacySystemsMigratorTester GameObject");
            }
            else
            {
                tester = testerObj.GetComponent<LegacySystemsMigratorTester>();
                if (tester == null)
                {
                    tester = testerObj.AddComponent<LegacySystemsMigratorTester>();
                    Debug.Log("Added LegacySystemsMigratorTester component to existing GameObject");
                }
            }
            
            // Note: Cannot directly set private migrator field, but the tester will find it in Start()
            // Make sure both objects are created before Start() runs
        }
        
        // Create a minimal Starkiller content manager for testing
        if (FindFirstObjectByType<StarkkillerContentManager>() == null)
        {
            GameObject contentObj = new GameObject("StarkkillerContentManager");
            contentObj.AddComponent<StarkkillerContentManager>();
            Debug.Log("Created StarkkillerContentManager GameObject");
        }
        
        // Create a minimal Starkiller media system for testing
        if (FindFirstObjectByType<StarkkillerMediaSystem>() == null)
        {
            GameObject mediaObj = new GameObject("StarkkillerMediaSystem");
            mediaObj.AddComponent<StarkkillerMediaSystem>();
            Debug.Log("Created StarkkillerMediaSystem GameObject");
        }
        
        // Create a minimal Starkiller encounter system for testing
        if (FindFirstObjectByType<StarkkillerEncounterSystem>() == null)
        {
            GameObject encounterObj = new GameObject("StarkkillerEncounterSystem");
            encounterObj.AddComponent<StarkkillerEncounterSystem>();
            Debug.Log("Created StarkkillerEncounterSystem GameObject");
        }
        
        // Create a minimal legacy ship encounter system for testing
        if (FindFirstObjectByType<ShipEncounterSystem>() == null)
        {
            GameObject legacyObj = new GameObject("ShipEncounterSystem");
            legacyObj.AddComponent<ShipEncounterSystem>();
            Debug.Log("Created ShipEncounterSystem GameObject");
        }
        
        Debug.Log("All required components have been created for testing");
    }
    
    [ContextMenu("Create Required Components")]
    public void CreateRequiredComponentsMenu()
    {
        CreateRequiredComponents();
    }
    
    [ContextMenu("Run Migrator Test")]
    public void RunMigratorTestMenu()
    {
        RunMigratorTest();
    }
}