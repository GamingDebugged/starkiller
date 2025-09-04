using UnityEngine;
using StarkillerBaseCommand;

/// <summary>
/// Quick fix for the encounter system timing issues
/// </summary>
public class EncounterSystemQuickFix : MonoBehaviour
{
    [Header("Quick Actions")]
    [SerializeField] private bool disableTimingController = false;
    [SerializeField] private bool forceGenerateEncounter = false;
    [SerializeField] private bool showDebugInfo = false;
    
    [Header("Timing Overrides")]
    [SerializeField] private float timeBetweenShips = 0f;
    [SerializeField] private float minimumEncounterDuration = 0f;
    
    [Header("Found Components")]
    [SerializeField] private ShipTimingController timingController;
    [SerializeField] private MasterShipGenerator shipGenerator;
    [SerializeField] private CredentialChecker credentialChecker;
    
    void Start()
    {
        FindComponents();
        
        if (disableTimingController)
        {
            DisableTimingController();
        }
        else
        {
            ApplyTimingOverrides();
        }
    }
    
    void Update()
    {
        if (forceGenerateEncounter)
        {
            forceGenerateEncounter = false;
            ForceNewEncounter();
        }
        
        if (showDebugInfo && Input.GetKeyDown(KeyCode.F1))
        {
            ShowDebugInfo();
        }
    }
    
    private void FindComponents()
    {
        timingController = FindFirstObjectByType<ShipTimingController>();
        shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        credentialChecker = FindFirstObjectByType<CredentialChecker>();
        
        Debug.Log($"[EncounterFix] Found components - Timing: {timingController != null}, Generator: {shipGenerator != null}, Checker: {credentialChecker != null}");
    }
    
    private void DisableTimingController()
    {
        if (timingController != null)
        {
            timingController.enabled = false;
            Debug.Log("[EncounterFix] Disabled ShipTimingController - encounters will generate immediately");
        }
    }
    
    private void ApplyTimingOverrides()
    {
        if (timingController != null)
        {
            // Use reflection to set private fields
            var timeBetweenField = timingController.GetType().GetField("timeBetweenShips", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (timeBetweenField != null && timeBetweenShips >= 0)
            {
                timeBetweenField.SetValue(timingController, timeBetweenShips);
                Debug.Log($"[EncounterFix] Set timeBetweenShips to {timeBetweenShips}");
            }
            
            var minDurationField = timingController.GetType().GetField("minimumEncounterDuration", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (minDurationField != null && minimumEncounterDuration >= 0)
            {
                minDurationField.SetValue(timingController, minimumEncounterDuration);
                Debug.Log($"[EncounterFix] Set minimumEncounterDuration to {minimumEncounterDuration}");
            }
            
            // Reset cooldown
            timingController.ResetCooldown();
        }
    }
    
    [ContextMenu("Force New Encounter")]
    public void ForceNewEncounter()
    {
        Debug.Log("[EncounterFix] Forcing new encounter...");
        
        // Reset timing controller
        if (timingController != null)
        {
            timingController.ResetCooldown();
        }
        
        // Generate a real encounter
        if (shipGenerator != null)
        {
            // First, ensure resources are loaded
            var loadMethod = shipGenerator.GetType().GetMethod("LoadAllResources", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (loadMethod != null)
            {
                loadMethod.Invoke(shipGenerator, null);
            }
            
            // Generate encounters for the current day if none exist
            var pendingField = shipGenerator.GetType().GetField("pendingEncounters", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (pendingField != null)
            {
                var queue = pendingField.GetValue(shipGenerator) as System.Collections.Generic.Queue<MasterShipEncounter>;
                if (queue != null && queue.Count == 0)
                {
                    Debug.Log("[EncounterFix] No pending encounters, generating new ones...");
                    shipGenerator.GenerateEncountersForDay(1);
                }
            }
            
            // Get the next encounter
            MasterShipEncounter encounter = shipGenerator.GetNextEncounter();
            
            // Make sure it's not a placeholder
            if (encounter != null && encounter.shipType != "Incoming Ship")
            {
                Debug.Log($"[EncounterFix] Generated encounter: {encounter.shipType} - {encounter.captainName}");
                
                // Display it
                if (credentialChecker != null)
                {
                    credentialChecker.DisplayEncounter(encounter);
                }
            }
            else
            {
                Debug.LogWarning("[EncounterFix] Got placeholder encounter, trying direct generation...");
                
                // Try to generate a random encounter directly
                encounter = shipGenerator.GenerateRandomEncounter();
                if (encounter != null)
                {
                    Debug.Log($"[EncounterFix] Direct generation: {encounter.shipType} - {encounter.captainName}");
                    if (credentialChecker != null)
                    {
                        credentialChecker.DisplayEncounter(encounter);
                    }
                }
            }
        }
    }
    
    private void ShowDebugInfo()
    {
        Debug.Log("=== Encounter System Debug Info ===");
        
        if (shipGenerator != null)
        {
            // Check pending encounters
            var pendingField = shipGenerator.GetType().GetField("pendingEncounters", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (pendingField != null)
            {
                var queue = pendingField.GetValue(shipGenerator) as System.Collections.Generic.Queue<MasterShipEncounter>;
                Debug.Log($"Pending encounters: {queue?.Count ?? 0}");
            }
            
            // Check resource loading
            var resourcesField = shipGenerator.GetType().GetField("resourcesLoaded", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (resourcesField != null)
            {
                bool loaded = (bool)resourcesField.GetValue(shipGenerator);
                Debug.Log($"Resources loaded: {loaded}");
            }
        }
        
        if (timingController != null)
        {
            Debug.Log($"Timing controller enabled: {timingController.enabled}");
            Debug.Log($"Can generate ship: {timingController.CanGenerateNewShip("Debug")}");
        }
        
        if (credentialChecker != null)
        {
            Debug.Log($"Has active encounter: {credentialChecker.HasActiveEncounter()}");
        }
    }
}
