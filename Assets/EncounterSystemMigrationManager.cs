using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StarkillerBaseCommand;

/// <summary>
/// Creates bridge classes that adapt MasterShipGenerator to work with legacy code
/// This manager ensures the bridge classes are properly created and configured
/// </summary>
public class EncounterSystemMigrationManager : MonoBehaviour
{
    [Header("References")]
    public MasterShipGenerator masterShipGenerator;
    public EncounterSystemManager systemManager;
    
    [Header("Settings")]
    [SerializeField] private bool autoCreateBridges = true;
    [SerializeField] private bool verboseLogging = true;
    
    // Static instance for singleton pattern
    private static EncounterSystemMigrationManager _instance;
    public static EncounterSystemMigrationManager Instance => _instance;
    
    // Bridges created
    private GameObject legacySystemBridge;
    private GameObject starkkillerSystemBridge;
    private GameObject generatorBridge;
    
    void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Check if we're a root GameObject
        if (transform.parent == null)
        {
            // Make this object persistent across scene loads
            DontDestroyOnLoad(gameObject);
            Debug.Log("EncounterSystemMigrationManager marked as persistent with DontDestroyOnLoad");
        }
        else
        {
            // Check if our parent has a ManagerInitializer component
            ManagerInitializer managerInit = GetComponentInParent<ManagerInitializer>();
            if (managerInit == null)
            {
                Debug.LogWarning("EncounterSystemMigrationManager: Not at root level and no ManagerInitializer found. This may cause persistence issues.");
            }
            else
            {
                Debug.Log("EncounterSystemMigrationManager: Found ManagerInitializer parent - persistence will be handled by it");
            }
        }
        
        FindSystemReferences();
        LogDetectedSystems();
    }
    
    /// <summary>
    /// Find references to required systems and components
    /// </summary>
    private void FindSystemReferences()
    {
        // Find MasterShipGenerator if not assigned
        if (masterShipGenerator == null)
        {
            masterShipGenerator = MasterShipGenerator.Instance;
            
            if (masterShipGenerator == null)
            {
                masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
                
                if (masterShipGenerator == null)
                {
                    Debug.LogError("EncounterSystemMigrationManager: Failed to find MasterShipGenerator");
                }
            }
        }
        
        // Find SystemManager if not assigned
        if (systemManager == null)
        {
            systemManager = FindFirstObjectByType<EncounterSystemManager>();
            
            if (systemManager == null)
            {
                Debug.LogWarning("EncounterSystemMigrationManager: Failed to find EncounterSystemManager");
            }
        }
    }
    
    /// <summary>
    /// Log detected systems for debugging
    /// </summary>
    private void LogDetectedSystems()
    {
        if (verboseLogging)
        {
            Debug.Log($"EncounterSystemMigrationManager: " +
                $"MasterShipGenerator: {(masterShipGenerator != null ? "Found" : "Missing")}, " +
                $"EncounterSystemManager: {(systemManager != null ? "Found" : "Missing")}");
        }
    }
    
    void Start()
    {
        // Create bridges if enabled
        if (autoCreateBridges)
        {
            CreateBridgeClasses();
        }
    }
    
    /// <summary>
    /// Create bridge classes for backward compatibility
    /// </summary>
    public void CreateBridgeClasses()
    {
        // Check what already exists in the scene
        ShipEncounterSystem existingShipSystem = FindFirstObjectByType<ShipEncounterSystem>();
        StarkkillerEncounterSystem existingStarkkillerSystem = FindFirstObjectByType<StarkkillerEncounterSystem>();
        ShipEncounterGenerator existingGenerator = FindFirstObjectByType<ShipEncounterGenerator>();
        
        if (verboseLogging)
        {
            Debug.Log($"Existing bridge components: " +
                $"ShipEncounterSystem: {existingShipSystem != null}, " +
                $"StarkkillerEncounterSystem: {existingStarkkillerSystem != null}, " +
                $"ShipEncounterGenerator: {existingGenerator != null}");
        }
        
        // Bridge classes are now created by direct scripts in the Assets folder
        // We just need to find them and connect them to our MasterShipGenerator
        
        // Try to find and configure ShipEncounterSystem
        if (masterShipGenerator != null)
        {
            // Try to connect to ShipEncounterSystem
            try
            {
                if (existingShipSystem != null)
                {
                    // Set reference directly
                    existingShipSystem.masterShipGenerator = masterShipGenerator;
                    if (verboseLogging)
                        Debug.Log("Connected masterShipGenerator to ShipEncounterSystem bridge");
                }
                
                // Try to connect to StarkkillerEncounterSystem
                if (existingStarkkillerSystem != null)
                {
                    // Set the field via reflection since it's private
                    var field = existingStarkkillerSystem.GetType().GetField("_masterShipGenerator", 
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    
                    if (field != null)
                    {
                        field.SetValue(existingStarkkillerSystem, masterShipGenerator);
                        if (verboseLogging)
                            Debug.Log("Connected masterShipGenerator to StarkkillerEncounterSystem bridge");
                    }
                    else
                    {
                        Debug.LogError("Could not find _masterShipGenerator field in StarkkillerEncounterSystem");
                    }
                }
                
                // Try to connect to ShipEncounterGenerator
                if (existingGenerator != null)
                {
                    existingGenerator.masterShipGenerator = masterShipGenerator;
                    if (verboseLogging)
                        Debug.Log("Connected masterShipGenerator to ShipEncounterGenerator bridge");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error connecting masterShipGenerator to bridge classes: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Cannot configure bridge classes - masterShipGenerator reference is missing!");
        }
    }
    
    /// <summary>
    /// Get the active ship encounter system for UI display or debugging
    /// </summary>
    public string GetActiveSystem()
    {
        if (systemManager != null)
        {
            return "Using " + systemManager.activeSystem.ToString();
        }
        else if (masterShipGenerator != null && masterShipGenerator.enabled)
        {
            return "Using MasterShipGenerator";
        }
        else
        {
            return "Using bridge classes with MasterShipGenerator";
        }
    }
    
    /// <summary>
    /// Force MasterShipGenerator as the active system
    /// </summary>
    public void ForceMasterGenerator()
    {
        if (systemManager != null)
        {
            systemManager.activeSystem = EncounterSystemManager.EncounterSystemType.MasterShipGenerator;
            
            // Use reflection to force an update
            var method = systemManager.GetType().GetMethod("EnableOnlyActiveSystem", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
            if (method != null)
            {
                method.Invoke(systemManager, null);
                Debug.Log("Forced MasterShipGenerator as active system");
            }
        }
        else if (masterShipGenerator != null)
        {
            masterShipGenerator.enabled = true;
            Debug.Log("Enabled MasterShipGenerator directly");
        }
    }
}
