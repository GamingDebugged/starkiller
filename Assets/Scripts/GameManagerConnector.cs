using UnityEngine;

/// <summary>
/// Connects the GameManager to the MasterShipGenerator to ensure consistent ship encounter generation.
/// This script should be attached to the StarkkillerEncounterSystem GameObject.
/// </summary>
public class GameManagerConnector : MonoBehaviour
{
    [Tooltip("Reference to GameManager")]
    public GameManager gameManager;
    
    [Tooltip("Reference to MasterShipGenerator")]
    public MasterShipGenerator masterShipGenerator;
    
    [Tooltip("Reference to CredentialChecker")]
    public CredentialChecker credentialChecker;
    
    void Start()
    {
        // Find components if not assigned
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
            
        if (masterShipGenerator == null)
            masterShipGenerator = GetComponent<MasterShipGenerator>();
            
        if (credentialChecker == null && gameManager != null)
            credentialChecker = gameManager.credentialChecker as CredentialChecker;
            
        // Connect systems
        ConnectSystems();
        
        // Disable conflicting systems
        DisableConflictingSystems();
    }
    
    /// <summary>
    /// Connect MasterShipGenerator to other systems
    /// </summary>
    private void ConnectSystems()
    {
        if (masterShipGenerator == null)
        {
            Debug.LogError("MasterShipGenerator not found - cannot connect systems");
            return;
        }
        
        // Connect MasterShipGenerator to CredentialChecker
        if (credentialChecker != null)
        {
            // First remove any existing connections to avoid duplicate events
            masterShipGenerator.OnEncounterReady -= credentialChecker.DisplayEncounter;
            
            // Now connect them
            masterShipGenerator.OnEncounterReady += credentialChecker.DisplayEncounter;
            Debug.Log("Connected MasterShipGenerator to CredentialChecker");
        }
        else
        {
            Debug.LogWarning("CredentialChecker not found - cannot connect to MasterShipGenerator");
        }
        
        // Patch the GameManager's GenerateNewShipEncounter method
        if (gameManager != null)
        {
            // We'll modify GameManager.GenerateNewShipEncounter to call MasterShipGenerator
            PatchGameManager();
        }
        else
        {
            Debug.LogWarning("GameManager not found - cannot patch GenerateNewShipEncounter");
        }
    }
    
    /// <summary>
    /// Disable any systems that might conflict with MasterShipGenerator
    /// </summary>
    private void DisableConflictingSystems()
    {
        // Find and disable ShipEncounterGenerator
        ShipEncounterGenerator shipGenComponent = FindFirstObjectByType<ShipEncounterGenerator>();
        if (shipGenComponent != null && shipGenComponent.enabled)
        {
            shipGenComponent.enabled = false;
            Debug.Log("Disabled ShipEncounterGenerator to prevent conflicts");
        }
        
        // Find and disable ShipEncounterSystem
        ShipEncounterSystem shipSysComponent = GetComponent<ShipEncounterSystem>();
        if (shipSysComponent != null && shipSysComponent.enabled)
        {
            shipSysComponent.enabled = false;
            Debug.Log("Disabled ShipEncounterSystem to prevent conflicts");
        }
        
        // Find GameObject with ShipEncounterGenerator
        GameObject shipGenObject = GameObject.Find("ShipEncounterGenerator");
        if (shipGenObject != null)
        {
            shipGenObject.SetActive(false);
            Debug.Log("Disabled ShipEncounterGenerator GameObject to prevent conflicts");
        }
    }
    
    /// <summary>
    /// Patch the GameManager to use MasterShipGenerator
    /// </summary>
    private void PatchGameManager()
    {
        // First, add a simple Monobehaviour patch to the GameManager
        GameManagerPatch patch = gameManager.gameObject.GetComponent<GameManagerPatch>();
        if (patch == null)
        {
            patch = gameManager.gameObject.AddComponent<GameManagerPatch>();
            Debug.Log("Added GameManagerPatch component to GameManager");
        }
        
        // Set the patch's reference to MasterShipGenerator
        patch.masterShipGenerator = masterShipGenerator;
    }
}

/// <summary>
/// Simple patch for GameManager to redirect ship encounter generation to MasterShipGenerator
/// </summary>
public class GameManagerPatch : MonoBehaviour
{
    [HideInInspector]
    public MasterShipGenerator masterShipGenerator;
    
    // This will be called by MonoBehaviour.SendMessage from the GameManager
    public void GenerateNewShipEncounter()
    {
        if (masterShipGenerator != null)
        {
            Debug.Log("GameManagerPatch: Redirecting to MasterShipGenerator.GetNextEncounter()");
            masterShipGenerator.GetNextEncounter();
        }
        else
        {
            Debug.LogError("GameManagerPatch: MasterShipGenerator reference is missing!");
        }
    }
}