using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Quick fix to ensure PersonalDataLog shows after DailyReport
/// This replaces the functionality that PersonalDataLogForcer was providing
/// </summary>
public class PersonalDataLogQuickFix : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool autoHookToDailyReport = true;
    [SerializeField] private float delayBeforeShow = 0.5f;
    
    private DailyReportManager dailyReportManager;
    private Starkiller.Core.Managers.PersonalDataLogManager personalDataLogManager;
    private Button originalContinueButton;
    
    private void Start()
    {
        if (autoHookToDailyReport)
        {
            StartCoroutine(SetupDailyReportHook());
        }
    }
    
    private IEnumerator SetupDailyReportHook()
    {
        // Wait a frame to ensure everything is initialized
        yield return null;
        
        // Find DailyReportManager
        dailyReportManager = FindObjectOfType<DailyReportManager>();
        if (dailyReportManager == null)
        {
            Debug.LogError("[PersonalDataLogQuickFix] DailyReportManager not found!");
            yield break;
        }
        
        // Find PersonalDataLogManager
        personalDataLogManager = FindObjectOfType<Starkiller.Core.Managers.PersonalDataLogManager>();
        if (personalDataLogManager == null)
        {
            Debug.LogError("[PersonalDataLogQuickFix] PersonalDataLogManager not found!");
            yield break;
        }
        
        Debug.Log("[PersonalDataLogQuickFix] Found both managers, setting up hook...");
        
        // Hook into the continue button
        if (dailyReportManager.continueButton != null)
        {
            originalContinueButton = dailyReportManager.continueButton;
            
            // Add our interceptor before the existing listeners
            originalContinueButton.onClick.AddListener(InterceptContinueClick);
            
            Debug.Log("[PersonalDataLogQuickFix] Successfully hooked into DailyReport continue button");
        }
        else
        {
            Debug.LogError("[PersonalDataLogQuickFix] DailyReportManager continue button is null!");
        }
    }
    
    private void InterceptContinueClick()
    {
        Debug.Log("[PersonalDataLogQuickFix] Continue button clicked - ensuring PersonalDataLog shows");
        
        // The original continue button will handle closing the daily report
        // We just need to ensure PersonalDataLog shows afterwards
        StartCoroutine(ShowPersonalDataLogAfterDelay());
    }
    
    private IEnumerator ShowPersonalDataLogAfterDelay()
    {
        // Wait for daily report to close
        yield return new WaitForSeconds(delayBeforeShow);
        
        // Just show the PersonalDataLog - it has its own guard against rapid calls
        if (personalDataLogManager != null)
        {
            Debug.Log("[PersonalDataLogQuickFix] Triggering PersonalDataLog display...");
            personalDataLogManager.ShowDataLog();
        }
        else
        {
            Debug.LogError("[PersonalDataLogQuickFix] PersonalDataLogManager is null!");
        }
    }
    
    [ContextMenu("Test Show PersonalDataLog")]
    public void TestShowPersonalDataLog()
    {
        if (personalDataLogManager == null)
        {
            personalDataLogManager = FindObjectOfType<Starkiller.Core.Managers.PersonalDataLogManager>();
        }
        
        if (personalDataLogManager != null)
        {
            Debug.Log("[PersonalDataLogQuickFix] Manually showing PersonalDataLog...");
            personalDataLogManager.ShowDataLog();
        }
        else
        {
            Debug.LogError("[PersonalDataLogQuickFix] PersonalDataLogManager not found!");
        }
    }
    
    private void OnDestroy()
    {
        // Clean up our listener
        if (originalContinueButton != null)
        {
            originalContinueButton.onClick.RemoveListener(InterceptContinueClick);
        }
    }
}