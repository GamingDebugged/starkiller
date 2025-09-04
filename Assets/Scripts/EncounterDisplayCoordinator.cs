using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarkillerBaseCommand;

/// <summary>
/// Coordinates encounter displays to prevent duplicate displays of the same encounter
/// from different systems. Acts as a central authority that all display systems should check with.
/// </summary>
public class EncounterDisplayCoordinator : MonoBehaviour
{
    // Private fields
    private bool isProcessingEncounter = false;
    private float processingTimeout = 1.0f; // Timeout in seconds to auto-release lock
    private string currentProcessor = "None";
    private MasterShipEncounter currentEncounter;
    
    // Debug settings
    [SerializeField] private bool verboseLogging = true;
    
    // Coroutine reference for timeout
    private Coroutine timeoutCoroutine;
    
    // Singleton pattern
    private static EncounterDisplayCoordinator _instance;
    public static EncounterDisplayCoordinator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EncounterDisplayCoordinator>();
                
                if (_instance == null)
                {
                    GameObject go = new GameObject("EncounterDisplayCoordinator");
                    _instance = go.AddComponent<EncounterDisplayCoordinator>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("EncounterDisplayCoordinator initialized");
    }
    
    /// <summary>
    /// Request permission to display an encounter. Only one system can display at a time.
    /// </summary>
    /// <param name="requestingSystem">Name of the system requesting permission</param>
    /// <param name="encounter">The encounter to be displayed</param>
    /// <returns>True if permission granted, false if denied</returns>
    public bool RequestEncounterDisplay(string requestingSystem, MasterShipEncounter encounter)
    {
        // If we're already processing and it's the same encounter, deny duplicate display
        if (isProcessingEncounter && currentEncounter != null && encounter != null && 
            currentEncounter.shipType == encounter.shipType && 
            currentEncounter.captainName == encounter.captainName)
        {
            if (verboseLogging)
            {
                Debug.LogWarning($"EncounterDisplayCoordinator: Denied duplicate display request from {requestingSystem} " +
                               $"for ship {encounter.shipType} - already being processed by {currentProcessor}");
            }
            return false;
        }
        
        // If processing a different encounter, log but still deny
        if (isProcessingEncounter)
        {
            if (verboseLogging)
            {
                Debug.LogWarning($"EncounterDisplayCoordinator: Denied display request from {requestingSystem} " +
                               $"because {currentProcessor} is already processing an encounter");
            }
            return false;
        }
        
        // Grant permission and set lock
        isProcessingEncounter = true;
        currentProcessor = requestingSystem;
        currentEncounter = encounter;
        
        if (verboseLogging)
        {
            Debug.Log($"EncounterDisplayCoordinator: Granted display permission to {requestingSystem} " +
                     $"for ship {encounter.shipType}");
        }
        
        // Start timeout coroutine to auto-release after delay
        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
        }
        timeoutCoroutine = StartCoroutine(AutoReleaseAfterDelay(processingTimeout));
        
        return true;
    }
    
    /// <summary>
    /// Notifies that an encounter display has completed
    /// </summary>
    /// <param name="systemName">Name of the system that completed the display</param>
    public void NotifyDisplayComplete(string systemName)
    {
        // Check if the notifying system is the one that has the lock
        if (isProcessingEncounter && currentProcessor == systemName)
        {
            ReleaseDisplayLock();
            
            if (verboseLogging)
            {
                Debug.Log($"EncounterDisplayCoordinator: Display completed by {systemName}, released lock");
            }
        }
        else if (isProcessingEncounter)
        {
            if (verboseLogging)
            {
                Debug.LogWarning($"EncounterDisplayCoordinator: Received completion from {systemName} " +
                               $"but lock is held by {currentProcessor}");
            }
        }
    }
    
    /// <summary>
    /// Manually release the display lock regardless of who holds it
    /// </summary>
    public void ReleaseDisplayLock()
    {
        // Stop the timeout coroutine if running
        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
            timeoutCoroutine = null;
        }
        
        // Release the lock
        if (isProcessingEncounter)
        {
            if (verboseLogging)
            {
                Debug.Log($"EncounterDisplayCoordinator: Released display lock held by {currentProcessor}");
            }
            
            isProcessingEncounter = false;
            currentProcessor = "None";
            currentEncounter = null;
        }
    }
    
    /// <summary>
    /// Automatically release the lock after a timeout to prevent deadlocks
    /// </summary>
    private IEnumerator AutoReleaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (isProcessingEncounter)
        {
            Debug.LogWarning($"EncounterDisplayCoordinator: Auto-releasing lock after timeout. " +
                           $"Lock was held by {currentProcessor}");
            
            ReleaseDisplayLock();
        }
        
        timeoutCoroutine = null;
    }
    
    /// <summary>
    /// Check if an encounter is currently being processed
    /// </summary>
    public bool IsProcessingEncounter()
    {
        return isProcessingEncounter;
    }
    
    /// <summary>
    /// Get the name of the system currently processing an encounter
    /// </summary>
    public string GetCurrentProcessor()
    {
        return currentProcessor;
    }
    
    /// <summary>
    /// Reset all state (for use in testing or error recovery)
    /// </summary>
    public void ResetState()
    {
        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
            timeoutCoroutine = null;
        }
        
        isProcessingEncounter = false;
        currentProcessor = "None";
        currentEncounter = null;
        
        Debug.Log("EncounterDisplayCoordinator: Reset complete");
    }
}