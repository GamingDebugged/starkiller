using UnityEngine;

/// <summary>
/// This script adapts MasterShipGenerator to work with GameManager.
/// It handles properly connecting the systems and ensuring only one generator is active.
/// </summary>
public class MasterShipGeneratorAdapter : MonoBehaviour
{
    void Start()
    {
        // Find the MasterShipGenerator
        MasterShipGenerator masterGenerator = GetComponent<MasterShipGenerator>();
        if (masterGenerator == null)
        {
            Debug.LogError("MasterShipGeneratorAdapter: No MasterShipGenerator found on this GameObject!");
            return;
        }
        
        // Find the GameManager
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("MasterShipGeneratorAdapter: GameManager not found in the scene!");
            return;
        }
        
        // Find the CredentialChecker (likely set in GameManager)
        CredentialChecker credentialChecker = null;
        if (gameManager.credentialChecker != null)
        {
            credentialChecker = gameManager.credentialChecker as CredentialChecker;
        }
        
        if (credentialChecker == null)
        {
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            Debug.LogWarning("MasterShipGeneratorAdapter: CredentialChecker not set in GameManager, found via FindFirstObjectByType");
        }
        
        if (credentialChecker == null)
        {
            Debug.LogError("MasterShipGeneratorAdapter: CredentialChecker not found!");
            return;
        }
        
        // Connect MasterShipGenerator to CredentialChecker
        // First, remove any existing connections to avoid duplicates
        masterGenerator.OnEncounterReady -= credentialChecker.DisplayEncounter;
        
        // Now add the event handler
        masterGenerator.OnEncounterReady += credentialChecker.DisplayEncounter;
        Debug.Log("MasterShipGeneratorAdapter: Connected MasterShipGenerator to CredentialChecker");
        
        // Disable competing systems
        DisableCompetingSystems();
        
        // Initialize with correct day
        if (gameManager.currentDay > 1)
        {
            masterGenerator.StartNewDay(gameManager.currentDay);
            Debug.Log($"MasterShipGeneratorAdapter: Set day to {gameManager.currentDay}");
        }
        
        Debug.Log("MasterShipGeneratorAdapter: Setup complete");
    }
    
    /// <summary>
    /// Disable any competing ship encounter generators to prevent conflicts
    /// </summary>
    private void DisableCompetingSystems()
    {
        // Try to disable ShipEncounterGenerator if it exists
        ShipEncounterGenerator shipGen = FindFirstObjectByType<ShipEncounterGenerator>();
        if (shipGen != null && shipGen.enabled)
        {
            shipGen.enabled = false;
            Debug.Log("MasterShipGeneratorAdapter: Disabled ShipEncounterGenerator");
            
            // Also try to disable its GameObject if it's separate
            if (shipGen.gameObject != this.gameObject)
            {
                shipGen.gameObject.SetActive(false);
                Debug.Log("MasterShipGeneratorAdapter: Disabled ShipEncounterGenerator GameObject");
            }
        }
        
        // Check for the legacy system on the same GameObject
        ShipEncounterSystem legacySystem = GetComponent<ShipEncounterSystem>();
        if (legacySystem != null && legacySystem.enabled)
        {
            legacySystem.enabled = false;
            Debug.Log("MasterShipGeneratorAdapter: Disabled ShipEncounterSystem on same GameObject");
        }
        
        // Also check for any other ShipEncounterSystem in the scene
        ShipEncounterSystem[] legacySystems = FindObjectsByType<ShipEncounterSystem>(FindObjectsSortMode.None);
        foreach (var system in legacySystems)
        {
            if (system != legacySystem && system.enabled)
            {
                system.enabled = false;
                Debug.Log($"MasterShipGeneratorAdapter: Disabled ShipEncounterSystem on {system.gameObject.name}");
            }
        }
    }
}