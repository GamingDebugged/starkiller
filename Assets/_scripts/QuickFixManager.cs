using UnityEngine;
using Starkiller.Core.Managers;
using Starkiller.Core;

/// <summary>
/// Quick fix to ensure critical managers exist in the scene
/// Add this to any GameObject in your scene (like GameManager)
/// </summary>
public class QuickFixManager : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("[QuickFixManager] Checking for missing managers...");
        
        // Fix 1: Create RefactoredDailyReportManager if missing
        if (FindFirstObjectByType<RefactoredDailyReportManager>() == null)
        {
            GameObject managerObj = new GameObject("RefactoredDailyReportManager");
            managerObj.AddComponent<RefactoredDailyReportManager>();
            DontDestroyOnLoad(managerObj);
            Debug.Log("[QuickFixManager] Created RefactoredDailyReportManager");
        }
        
        // Fix 2: Create PersonalDataLogManager if missing
        if (FindFirstObjectByType<PersonalDataLogManager>() == null)
        {
            GameObject managerObj = new GameObject("PersonalDataLogManager");
            managerObj.AddComponent<PersonalDataLogManager>();
            DontDestroyOnLoad(managerObj);
            Debug.Log("[QuickFixManager] Created PersonalDataLogManager");
        }
        
        // Fix 3: Register CredentialChecker with ServiceLocator
        var credentialChecker = FindFirstObjectByType<CredentialChecker>();
        if (credentialChecker != null)
        {
            ServiceLocator.Register<CredentialChecker>(credentialChecker);
            Debug.Log("[QuickFixManager] Registered CredentialChecker with ServiceLocator");
        }
        
        // Fix 4: Ensure ServiceLocator is on root GameObject
        var serviceLocator = FindFirstObjectByType<ServiceLocator>();
        if (serviceLocator != null && serviceLocator.transform.parent != null)
        {
            serviceLocator.transform.SetParent(null);
            DontDestroyOnLoad(serviceLocator.gameObject);
            Debug.Log("[QuickFixManager] Moved ServiceLocator to root");
        }
    }
    
    private void Start()
    {
        // Double-check that managers are registered
        var refactoredDailyReport = ServiceLocator.Get<RefactoredDailyReportManager>();
        if (refactoredDailyReport != null)
        {
            Debug.Log("[QuickFixManager] RefactoredDailyReportManager is properly registered!");
        }
        else
        {
            Debug.LogError("[QuickFixManager] RefactoredDailyReportManager still not found!");
        }
        
        var personalDataLog = ServiceLocator.Get<PersonalDataLogManager>();
        if (personalDataLog != null)
        {
            Debug.Log("[QuickFixManager] PersonalDataLogManager is properly registered!");
        }
        else
        {
            Debug.LogError("[QuickFixManager] PersonalDataLogManager still not found!");
        }
    }
}