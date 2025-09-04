using UnityEngine;
using Starkiller.Core.Managers;
using Starkiller.Core;

/// <summary>
/// Debug helper to diagnose PersonalDataLog display issues
/// </summary>
public class PersonalDataLogDebugHelper : MonoBehaviour
{
    [Header("Manual Testing")]
    [SerializeField] private KeyCode testKey = KeyCode.F9;
    
    [Header("References")]
    [SerializeField] private PersonalDataLogManager personalDataLogManager;
    
    private void Start()
    {
        Debug.Log("[PersonalDataLogDebugHelper] Starting diagnostics...");
        
        // Check if PersonalDataLogManager exists in scene
        personalDataLogManager = FindObjectOfType<PersonalDataLogManager>();
        if (personalDataLogManager != null)
        {
            Debug.Log($"[PersonalDataLogDebugHelper] ✓ PersonalDataLogManager found: {personalDataLogManager.name}");
        }
        else
        {
            Debug.LogError("[PersonalDataLogDebugHelper] ✗ PersonalDataLogManager NOT FOUND in scene!");
        }
        
        // Check ServiceLocator registration (if available)
        try
        {
            var serviceLocator = FindObjectOfType<ServiceLocator>();
            if (serviceLocator != null)
            {
                Debug.Log("[PersonalDataLogDebugHelper] ✓ ServiceLocator found in scene");
            }
            else
            {
                Debug.LogWarning("[PersonalDataLogDebugHelper] ⚠️ ServiceLocator not found in scene");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[PersonalDataLogDebugHelper] ServiceLocator check failed: {e.Message}");
        }
        
        // Check DailyReportManager
        var dailyReportManager = FindObjectOfType<DailyReportManager>();
        if (dailyReportManager != null)
        {
            Debug.Log($"[PersonalDataLogDebugHelper] ✓ DailyReportManager found: {dailyReportManager.name}");
        }
        else
        {
            Debug.LogWarning("[PersonalDataLogDebugHelper] ⚠️ DailyReportManager not found");
        }
        
        // Check RefactoredDailyReportManager
        var refactoredDRM = FindObjectOfType<RefactoredDailyReportManager>();
        if (refactoredDRM != null)
        {
            Debug.Log("[PersonalDataLogDebugHelper] ✓ RefactoredDailyReportManager found in scene");
        }
        else
        {
            Debug.LogWarning("[PersonalDataLogDebugHelper] ⚠️ RefactoredDailyReportManager not found in scene");
        }
        
        Debug.Log($"[PersonalDataLogDebugHelper] Press {testKey} to manually trigger PersonalDataLog");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            TestShowPersonalDataLog();
        }
    }
    
    [ContextMenu("Test Show PersonalDataLog")]
    public void TestShowPersonalDataLog()
    {
        Debug.Log("[PersonalDataLogDebugHelper] === MANUAL TEST: Showing PersonalDataLog ===");
        
        // Method 1: Direct reference
        if (personalDataLogManager != null)
        {
            Debug.Log("[PersonalDataLogDebugHelper] Calling ShowDataLog() directly on scene instance...");
            personalDataLogManager.ShowDataLog();
            return;
        }
        
        // Method 2: FindObjectOfType fallback
        personalDataLogManager = FindObjectOfType<PersonalDataLogManager>();
        if (personalDataLogManager != null)
        {
            Debug.Log("[PersonalDataLogDebugHelper] Calling ShowDataLog() via FindObjectOfType...");
            personalDataLogManager.ShowDataLog();
            return;
        }
        
        Debug.LogError("[PersonalDataLogDebugHelper] Cannot show PersonalDataLog - no manager found!");
    }
    
    [ContextMenu("Trace Daily Report Flow")]
    public void TraceDailyReportFlow()
    {
        Debug.Log("[PersonalDataLogDebugHelper] === TRACING DAILY REPORT FLOW ===");
        
        // Check GameManager
        var gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            // Use reflection to access currentGameState if it's not public
            var stateField = gameManager.GetType().GetField("currentGameState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (stateField != null)
            {
                var state = stateField.GetValue(gameManager);
                Debug.Log($"[PersonalDataLogDebugHelper] GameManager state: {state}");
            }
            Debug.Log($"[PersonalDataLogDebugHelper] Current day: {gameManager.currentDay}");
        }
        
        // Check what happens when Continue is clicked
        var dailyReportManager = FindObjectOfType<DailyReportManager>();
        if (dailyReportManager != null && dailyReportManager.continueButton != null)
        {
            var listeners = dailyReportManager.continueButton.onClick.GetPersistentEventCount();
            Debug.Log($"[PersonalDataLogDebugHelper] Continue button has {listeners} persistent listeners");
            
            // Log button state
            Debug.Log($"[PersonalDataLogDebugHelper] Continue button interactable: {dailyReportManager.continueButton.interactable}");
        }
    }
    
    [ContextMenu("Force Show PersonalDataLog")]
    public void ForceShowPersonalDataLog()
    {
        if (personalDataLogManager == null)
        {
            personalDataLogManager = FindObjectOfType<PersonalDataLogManager>();
        }
        
        if (personalDataLogManager != null)
        {
            Debug.Log("[PersonalDataLogDebugHelper] PersonalDataLogManager found - attempting manual ShowDataLog...");
            personalDataLogManager.ShowDataLog();
        }
        else
        {
            Debug.LogError("[PersonalDataLogDebugHelper] Cannot show - PersonalDataLogManager not found!");
        }
    }
}