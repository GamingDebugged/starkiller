using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarkillerBaseCommand;

/// <summary>
/// Ensures that there is one and only one MasterShipGenerator active in the scene
/// and all other systems properly reference it
/// </summary>
public class ShipSystemInitializer : MonoBehaviour
{
    // Singleton instance for ease of reference
    private static ShipSystemInitializer _instance;
    public static ShipSystemInitializer Instance => _instance;
    
    [Header("Debug Settings")]
    [SerializeField] private bool verboseLogging = true;
    
    // Reference to the single MasterShipGenerator
    private MasterShipGenerator masterShipGenerator;
    
    // References to other key systems
    private CredentialChecker credentialChecker;
    private GameManager gameManager;
    
    void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Make this a root GameObject to avoid DontDestroyOnLoad issues
        transform.SetParent(null);
        
        // Ensure singleton MasterShipGenerator exists
        EnsureMasterShipGenerator();
        
        // Find other key systems
        FindGameSystems();
        
        // Connect systems together
        ConnectSystems();
        
        if (verboseLogging)
            Debug.Log("ShipSystemInitializer completed initialization successfully");
    }
    
    /// <summary>
    /// Ensures that one and only one MasterShipGenerator exists
    /// </summary>
    private void EnsureMasterShipGenerator()
    {
        // First try to find an existing generator
        masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        
        if (masterShipGenerator != null)
        {
            if (verboseLogging)
                Debug.Log($"Found existing MasterShipGenerator with instanceID: {masterShipGenerator.GetInstanceID()}");
                
            // Make sure it's on a root GameObject
            masterShipGenerator.transform.SetParent(null);
        }
        else
        {
            if (verboseLogging)
                Debug.Log("No MasterShipGenerator found, creating one now");
                
            // Create a new GameObject for the generator
            GameObject generatorObject = new GameObject("MasterShipGenerator");
            
            // Add the MasterShipGenerator component
            masterShipGenerator = generatorObject.AddComponent<MasterShipGenerator>();
            
            // Make it a root GameObject
            generatorObject.transform.SetParent(null);
            
            // Don't destroy on load
            DontDestroyOnLoad(generatorObject);
            
            if (verboseLogging)
                Debug.Log($"Created new MasterShipGenerator with instanceID: {masterShipGenerator.GetInstanceID()}");
        }
    }
    
    /// <summary>
    /// Find all required game systems
    /// </summary>
    private void FindGameSystems()
    {
        // Find CredentialChecker
        credentialChecker = FindFirstObjectByType<CredentialChecker>();
        if (credentialChecker == null && verboseLogging)
        {
            Debug.LogWarning("ShipSystemInitializer: Could not find CredentialChecker!");
        }
        
        // Find GameManager
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null && verboseLogging)
        {
            Debug.LogWarning("ShipSystemInitializer: Could not find GameManager!");
        }
    }
    
    /// <summary>
    /// Connect all systems to the MasterShipGenerator
    /// </summary>
    private void ConnectSystems()
    {
        if (masterShipGenerator == null)
        {
            Debug.LogError("Cannot connect systems - MasterShipGenerator is null!");
            return;
        }
        
        // Connect CredentialChecker to MasterShipGenerator
        if (credentialChecker != null)
        {
            // Connect using reflection to avoid script edit requirements
            var field = credentialChecker.GetType().GetField("masterShipGenerator", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                
            if (field != null)
            {
                field.SetValue(credentialChecker, masterShipGenerator);
                if (verboseLogging)
                    Debug.Log("Connected CredentialChecker to MasterShipGenerator");
            }
            else if (verboseLogging)
            {
                Debug.LogWarning("Could not find masterShipGenerator field in CredentialChecker");
            }
            
            // Subscribe to events
            masterShipGenerator.OnEncounterReady -= credentialChecker.DisplayEncounter;
            masterShipGenerator.OnEncounterReady += credentialChecker.DisplayEncounter;
        }
        
        // Connect GameManager to MasterShipGenerator
        if (gameManager != null)
        {
            // Connect using reflection
            var field = gameManager.GetType().GetField("masterShipGenerator", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                
            if (field != null)
            {
                field.SetValue(gameManager, masterShipGenerator);
                if (verboseLogging)
                    Debug.Log("Connected GameManager to MasterShipGenerator");
            }
            else if (verboseLogging)
            {
                Debug.LogWarning("Could not find masterShipGenerator field in GameManager");
            }
        }
    }
    
    /// <summary>
    /// Force UI visibility in the CredentialChecker
    /// </summary>
    public void ForceUIVisibility()
    {
        if (credentialChecker != null)
        {
            credentialChecker.ForceUIVisibility();
        }
    }
}