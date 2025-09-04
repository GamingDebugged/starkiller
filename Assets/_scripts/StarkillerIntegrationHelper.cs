using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Helper script to integrate all Starkiller Base Command enhanced features
/// Add this to a dedicated GameObject in your scene to automatically set up components
/// </summary>
public class StarkillerIntegrationHelper : MonoBehaviour
{
    [Header("Integration Settings")]
    public bool setupLogBookTabs = true;
    public bool setupImperialFamily = true;
    public bool setupHolographicEffects = true;
    
    [Header("Component References")]
    public GameManager gameManager;
    public CredentialChecker credentialChecker;
    public GameObject logBookPanel;
    public GameObject familySystemObject;
    
    void Start()
    {
        // Log start message
        Debug.Log("StarkillerIntegrationHelper: Starting integration of enhanced features");
        
        // Find key components if not assigned
        FindKeyComponents();
        
        // Set up requested features
        if (setupLogBookTabs)
            SetupLogBookTabs();
            
        if (setupImperialFamily)
            SetupImperialFamily();
            
        if (setupHolographicEffects)
            SetupHolographicEffects();
            
        Debug.Log("StarkillerIntegrationHelper: Integration complete");
    }
    
    /// <summary>
    /// Find key components if they weren't assigned in the inspector
    /// </summary>
    void FindKeyComponents()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
            
        if (credentialChecker == null)
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            
        if (logBookPanel == null && credentialChecker != null)
            logBookPanel = credentialChecker.logBookPanel;
            
        if (familySystemObject == null)
        {
            var familySystem = FindFirstObjectByType<ImperialFamilySystem>();
            if (familySystem != null)
                familySystemObject = familySystem.gameObject;
        }
        
        // Log found components
        Debug.Log($"Found GameManager: {gameManager != null}");
        Debug.Log($"Found CredentialChecker: {credentialChecker != null}");
        Debug.Log($"Found LogBookPanel: {logBookPanel != null}");
        Debug.Log($"Found FamilySystem: {familySystemObject != null}");
    }
    
    /// <summary>
    /// Set up the tabbed log book
    /// </summary>
    void SetupLogBookTabs()
    {
        Debug.Log("Setting up tabbed log book");
        
        // Check if we have required components
        if (logBookPanel == null || credentialChecker == null)
        {
            Debug.LogError("Cannot set up log book tabs - missing required components");
            return;
        }
        
        // Check if we already have a LogBookManager
        LogBookManager existingManager = FindFirstObjectByType<LogBookManager>();
        if (existingManager != null)
        {
            Debug.Log("LogBookManager already exists, using existing component");
            
            // Add connector to sync with CredentialChecker
            ConnectLogBookManager connector = existingManager.gameObject.GetComponent<ConnectLogBookManager>();
            if (connector == null)
            {
                connector = existingManager.gameObject.AddComponent<ConnectLogBookManager>();
                connector.logBookManager = existingManager;
                connector.credentialChecker = credentialChecker;
            }
            
            return;
        }
        
        // Create new log book manager
        GameObject logBookManagerObj = new GameObject("LogBookManager");
        LogBookManager logBookManager = logBookManagerObj.AddComponent<LogBookManager>();
        
        // Create connector
        ConnectLogBookManager connectLogBook = logBookManagerObj.AddComponent<ConnectLogBookManager>();
        connectLogBook.logBookManager = logBookManager;
        connectLogBook.credentialChecker = credentialChecker;
        
        Debug.Log("Created LogBookManager and ConnectLogBookManager");
    }
    
    /// <summary>
    /// Set up the Imperial family system
    /// </summary>
    void SetupImperialFamily()
    {
        Debug.Log("Setting up Imperial family system");
        
        // Check if we have required components
        if (gameManager == null)
        {
            Debug.LogError("Cannot set up Imperial family - missing GameManager");
            return;
        }
        
        // Check if we already have ImperialFamilySystem
        ImperialFamilySystem existingSystem = FindFirstObjectByType<ImperialFamilySystem>();
        if (existingSystem != null)
        {
            Debug.Log("ImperialFamilySystem already exists, using existing component");
            
            // Add GameManagerImperialUpdater to sync with GameManager
            GameManagerImperialUpdater updater = gameManager.gameObject.GetComponent<GameManagerImperialUpdater>();
            if (updater == null)
            {
                updater = gameManager.gameObject.AddComponent<GameManagerImperialUpdater>();
                updater.imperialFamilySystem = existingSystem;
            }
            
            return;
        }
        
        // Get existing family system
        var originalSystem = FindFirstObjectByType<ImperialFamilySystem>();
        
        // TODO: If we want to actually convert an existing FamilySystem to ImperialFamilySystem,
        // we would need to implement the conversion logic here
        
        Debug.Log("Imperial family setup - Please manually set up the ImperialFamilySystem or use the existing system");
    }
    
    /// <summary>
    /// Set up holographic effects for UI
    /// </summary>
    void SetupHolographicEffects()
    {
        Debug.Log("Setting up holographic effects");
        
        // Find all family member displays
        FamilyMemberDisplay[] displays = FindObjectsByType<FamilyMemberDisplay>(FindObjectsSortMode.None);
        
        if (displays != null && displays.Length > 0)
        {
            foreach (var display in displays)
            {
                // Check if this display already has scan lines
                bool hasScanLines = false;
                foreach (Transform child in display.transform)
                {
                    if (child.GetComponent<ImperialScanLines>() != null)
                    {
                        hasScanLines = true;
                        break;
                    }
                }
                
                // Add scan lines if needed
                if (!hasScanLines)
                {
                    display.AddScanlineEffect();
                    Debug.Log($"Added scan line effect to {display.name}");
                }
            }
        }
        else
        {
            Debug.LogWarning("No FamilyMemberDisplay components found to add holographic effects");
        }
    }
}
