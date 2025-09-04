using UnityEngine;

/// <summary>
/// Temporary script to disable conflicting encounter systems and use only EncounterManager
/// Add this to a GameObject in your scene to clean up the encounter conflicts
/// </summary>
public class EncounterSystemCleanup : MonoBehaviour
{
    [Header("Cleanup Settings")]
    [SerializeField] private bool disableLegacySystems = false; // DISABLED: MasterShipGenerator is the only working system
    [SerializeField] private bool enableDebugLogs = true;
    
    private void Awake()
    {
        if (disableLegacySystems)
        {
            CleanupConflictingSystems();
        }
    }
    
    private void CleanupConflictingSystems()
    {
        if (enableDebugLogs)
            Debug.Log("[EncounterSystemCleanup] Starting encounter system cleanup...");
        
        // Disable MasterShipGenerator (legacy system)
        var masterShipGen = FindFirstObjectByType<MasterShipGenerator>();
        if (masterShipGen != null)
        {
            masterShipGen.enabled = false;
            masterShipGen.gameObject.SetActive(false);
            if (enableDebugLogs)
                Debug.Log("[EncounterSystemCleanup] Disabled and deactivated MasterShipGenerator (legacy)");
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("[EncounterSystemCleanup] MasterShipGenerator not found - already disabled?");
        }
        
        // Handle EncounterSystemManager that depends on MasterShipGenerator
        var encounterSystemManager = FindFirstObjectByType<EncounterSystemManager>();
        if (encounterSystemManager != null)
        {
            encounterSystemManager.enabled = false;
            if (enableDebugLogs)
                Debug.Log("[EncounterSystemCleanup] Disabled EncounterSystemManager (was using MasterShipGenerator)");
        }
        
        // Disable other conflicting encounter systems
        DisableComponent<EncounterSystemMigrationManager>("EncounterSystemMigrationManager");
        DisableComponent<EncounterFlowManager>("EncounterFlowManager");
        DisableComponent<ShipEncounterSystem>("ShipEncounterSystem");
        
        // Ensure EncounterManager is enabled
        var encounterManager = FindFirstObjectByType<Starkiller.Core.Managers.EncounterManager>();
        if (encounterManager != null)
        {
            encounterManager.enabled = true;
            if (enableDebugLogs)
                Debug.Log("[EncounterSystemCleanup] EncounterManager is active (new system)");
        }
        else
        {
            Debug.LogError("[EncounterSystemCleanup] EncounterManager not found! This is the system we want to use.");
        }
        
        if (enableDebugLogs)
            Debug.Log("[EncounterSystemCleanup] Cleanup complete - only EncounterManager should be active");
    }
    
    private void DisableComponent<T>(string componentName) where T : MonoBehaviour
    {
        var component = FindFirstObjectByType<T>();
        if (component != null)
        {
            component.enabled = false;
            if (enableDebugLogs)
                Debug.Log($"[EncounterSystemCleanup] Disabled {componentName}");
        }
    }
    
    [ContextMenu("Force Cleanup Now")]
    public void ForceCleanup()
    {
        CleanupConflictingSystems();
    }
    
    [ContextMenu("Nuclear Option: Destroy MasterShipGenerator")]
    public void NuclearOption()
    {
        var masterShipGen = FindFirstObjectByType<MasterShipGenerator>();
        if (masterShipGen != null)
        {
            Debug.Log("[EncounterSystemCleanup] NUCLEAR OPTION: Destroying MasterShipGenerator GameObject");
            DestroyImmediate(masterShipGen.gameObject);
        }
        
        // Force enable EncounterManager
        var encounterManager = FindFirstObjectByType<Starkiller.Core.Managers.EncounterManager>();
        if (encounterManager != null)
        {
            encounterManager.enabled = true;
            encounterManager.gameObject.SetActive(true);
            Debug.Log("[EncounterSystemCleanup] Force enabled EncounterManager");
        }
    }
}