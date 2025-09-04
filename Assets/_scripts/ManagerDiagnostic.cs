using UnityEngine;
using Starkiller.Core.Managers;
using Starkiller.Core;

/// <summary>
/// Diagnostic tool to check what managers exist and their status
/// </summary>
public class ManagerDiagnostic : MonoBehaviour
{
    [Header("Diagnostic Controls")]
    [SerializeField] private KeyCode diagnosticKey = KeyCode.F9;
    
    private void Start()
    {
        // Immediate diagnostic on start
        Invoke(nameof(RunDiagnostic), 2f); // Wait 2 seconds for everything to initialize
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(diagnosticKey))
        {
            RunDiagnostic();
        }
    }
    
    public void RunDiagnostic()
    {
        Debug.Log("=== MANAGER DIAGNOSTIC ===");
        
        // Check PersonalDataLogManager
        var personalDataLogManager = FindFirstObjectByType<PersonalDataLogManager>();
        if (personalDataLogManager != null)
        {
            Debug.Log($"✅ PersonalDataLogManager FOUND: {personalDataLogManager.name}");
            
            // Check if registered with ServiceLocator
            var fromService = ServiceLocator.Get<PersonalDataLogManager>();
            if (fromService != null)
            {
                Debug.Log("✅ PersonalDataLogManager registered with ServiceLocator");
            }
            else
            {
                Debug.LogError("❌ PersonalDataLogManager NOT registered with ServiceLocator!");
                // Try to register it
                ServiceLocator.Register<PersonalDataLogManager>(personalDataLogManager);
                Debug.Log("🔧 Attempted to register PersonalDataLogManager");
            }
        }
        else
        {
            Debug.LogError("❌ PersonalDataLogManager NOT FOUND in scene!");
            
            // Create it manually
            GameObject managerObj = new GameObject("PersonalDataLogManager");
            var manager = managerObj.AddComponent<PersonalDataLogManager>();
            Debug.Log("🔧 Created PersonalDataLogManager manually");
        }
        
        // Check RefactoredDailyReportManager
        var refactoredDailyReport = FindFirstObjectByType<RefactoredDailyReportManager>();
        if (refactoredDailyReport != null)
        {
            Debug.Log($"✅ RefactoredDailyReportManager FOUND: {refactoredDailyReport.name}");
            
            var fromService = ServiceLocator.Get<RefactoredDailyReportManager>();
            if (fromService != null)
            {
                Debug.Log("✅ RefactoredDailyReportManager registered with ServiceLocator");
            }
            else
            {
                Debug.LogError("❌ RefactoredDailyReportManager NOT registered with ServiceLocator!");
            }
        }
        else
        {
            Debug.LogError("❌ RefactoredDailyReportManager NOT FOUND in scene!");
        }
        
        // Check ServiceLocator
        var serviceLocator = FindFirstObjectByType<ServiceLocator>();
        if (serviceLocator != null)
        {
            Debug.Log($"✅ ServiceLocator FOUND: {serviceLocator.name}");
        }
        else
        {
            Debug.LogError("❌ ServiceLocator NOT FOUND!");
        }
        
        // List all GameObjects with manager components
        Debug.Log("--- All Manager GameObjects ---");
        var allPersonalDataLogManagers = FindObjectsByType<PersonalDataLogManager>(FindObjectsSortMode.None);
        Debug.Log($"PersonalDataLogManager instances: {allPersonalDataLogManagers.Length}");
        foreach (var mgr in allPersonalDataLogManagers)
        {
            Debug.Log($"  - {mgr.name} (Active: {mgr.gameObject.activeInHierarchy})");
        }
        
        var allRefactoredManagers = FindObjectsByType<RefactoredDailyReportManager>(FindObjectsSortMode.None);
        Debug.Log($"RefactoredDailyReportManager instances: {allRefactoredManagers.Length}");
        foreach (var mgr in allRefactoredManagers)
        {
            Debug.Log($"  - {mgr.name} (Active: {mgr.gameObject.activeInHierarchy})");
        }
        
        Debug.Log("=== DIAGNOSTIC COMPLETE ===");
    }
}