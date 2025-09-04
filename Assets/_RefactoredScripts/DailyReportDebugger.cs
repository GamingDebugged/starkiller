using UnityEngine;
using Starkiller.Core.Managers;
using Starkiller.Core;

/// <summary>
/// Debug utility to help investigate daily report issues
/// </summary>
public class DailyReportDebugger : MonoBehaviour
{
    [Header("Debug Options")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool enableVerboseLogging = false;
    
    [Header("Manual Testing")]
    [SerializeField] private bool testOnStart = false;
    
    // Manager references
    private RefactoredDailyReportManager _dailyReportManager;
    private DayProgressionManager _dayProgressionManager;
    private ShiftTimerManager _shiftTimerManager;
    private GameManagerIntegrationHelper _integrationHelper;
    
    private void Start()
    {
        if (testOnStart)
        {
            StartCoroutine(TestAfterDelay(2f));
        }
    }
    
    private System.Collections.IEnumerator TestAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TestDailyReportSystem();
    }
    
    /// <summary>
    /// Test the daily report system end-to-end
    /// </summary>
    [ContextMenu("Test Daily Report System")]
    public void TestDailyReportSystem()
    {
        Debug.Log("=== DAILY REPORT SYSTEM TEST ===");
        
        // 1. Find all required managers
        FindManagers();
        
        // 2. Check manager status
        CheckManagerStatus();
        
        // 3. Check event subscriptions
        CheckEventSubscriptions();
        
        // 4. Check UI setup
        CheckUISetup();
        
        // 5. Test manual report generation
        TestManualReportGeneration();
        
        Debug.Log("=== END DAILY REPORT TEST ===");
    }
    
    /// <summary>
    /// Find all required managers
    /// </summary>
    private void FindManagers()
    {
        Debug.Log("--- Finding Managers ---");
        
        // Try ServiceLocator first
        _dailyReportManager = ServiceLocator.Get<RefactoredDailyReportManager>();
        _dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
        _shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
        
        // Try FindFirstObjectByType as fallback
        if (_dailyReportManager == null)
            _dailyReportManager = FindFirstObjectByType<RefactoredDailyReportManager>();
        if (_dayProgressionManager == null)
            _dayProgressionManager = FindFirstObjectByType<DayProgressionManager>();
        if (_shiftTimerManager == null)
            _shiftTimerManager = FindFirstObjectByType<ShiftTimerManager>();
        
        _integrationHelper = FindFirstObjectByType<GameManagerIntegrationHelper>();
        
        Debug.Log($"RefactoredDailyReportManager: {(_dailyReportManager != null ? "✓" : "✗")}");
        Debug.Log($"DayProgressionManager: {(_dayProgressionManager != null ? "✓" : "✗")}");
        Debug.Log($"ShiftTimerManager: {(_shiftTimerManager != null ? "✓" : "✗")}");
        Debug.Log($"GameManagerIntegrationHelper: {(_integrationHelper != null ? "✓" : "✗")}");
    }
    
    /// <summary>
    /// Check manager status and configuration
    /// </summary>
    private void CheckManagerStatus()
    {
        Debug.Log("--- Manager Status ---");
        
        if (_dailyReportManager != null)
        {
            Debug.Log($"Daily Report Active: {_dailyReportManager.IsReportActive}");
            Debug.Log($"Day Transitioning: {_dailyReportManager.IsTransitioningDay}");
            Debug.Log($"Current Day: {_dailyReportManager.CurrentDay}");
        }
        else
        {
            Debug.LogError("RefactoredDailyReportManager not found!");
        }
        
        if (_dayProgressionManager != null)
        {
            Debug.Log($"Shift Active: {_dayProgressionManager.IsShiftActive}");
            Debug.Log($"Current Day: {_dayProgressionManager.CurrentDay}");
            Debug.Log($"Remaining Time: {_dayProgressionManager.RemainingTime:F1}s");
        }
        else
        {
            Debug.LogError("DayProgressionManager not found!");
        }
        
        if (_shiftTimerManager != null)
        {
            Debug.Log($"Timer Active: {_shiftTimerManager.IsTimerActive}");
            Debug.Log($"Shift Ended: {_shiftTimerManager.IsShiftEnded}");
            Debug.Log($"Remaining Time: {_shiftTimerManager.RemainingTime:F1}s");
        }
        else
        {
            Debug.LogError("ShiftTimerManager not found!");
        }
    }
    
    /// <summary>
    /// Check event subscriptions
    /// </summary>
    private void CheckEventSubscriptions()
    {
        Debug.Log("--- Event Subscriptions ---");
        
        if (_dailyReportManager != null && _dayProgressionManager != null)
        {
            // This is harder to test directly, but we can check if the managers exist
            Debug.Log("Both managers exist - event subscription should be working");
        }
        else
        {
            Debug.LogError("Missing managers - event subscription will fail");
        }
    }
    
    /// <summary>
    /// Check UI setup
    /// </summary>
    private void CheckUISetup()
    {
        Debug.Log("--- UI Setup ---");
        
        if (_dailyReportManager != null)
        {
            // Use reflection to check private fields
            var panelField = _dailyReportManager.GetType().GetField("dailyReportPanel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (panelField != null)
            {
                var panel = panelField.GetValue(_dailyReportManager) as GameObject;
                Debug.Log($"Daily Report Panel: {(panel != null ? "✓ Found" : "✗ Not assigned")}");
                
                if (panel != null)
                {
                    Debug.Log($"Panel Active: {panel.activeSelf}");
                }
            }
            else
            {
                Debug.LogError("Could not access dailyReportPanel field");
            }
        }
    }
    
    /// <summary>
    /// Test manual report generation
    /// </summary>
    private void TestManualReportGeneration()
    {
        Debug.Log("--- Manual Report Generation Test ---");
        
        if (_dailyReportManager != null)
        {
            try
            {
                Debug.Log("Attempting to generate daily report...");
                _dailyReportManager.GenerateDailyReport();
                Debug.Log("Report generation call completed");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error generating daily report: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
            }
        }
        else
        {
            Debug.LogError("Cannot test report generation - RefactoredDailyReportManager not found");
        }
    }
    
    /// <summary>
    /// Trigger shift end manually
    /// </summary>
    [ContextMenu("Trigger Shift End")]
    public void TriggerShiftEnd()
    {
        Debug.Log("=== MANUAL SHIFT END TRIGGER ===");
        
        if (_dayProgressionManager != null)
        {
            Debug.Log("Calling DayProgressionManager.EndShift()...");
            _dayProgressionManager.EndShift();
        }
        else if (_integrationHelper != null)
        {
            Debug.Log("Calling GameManagerIntegrationHelper.EndShift()...");
            _integrationHelper.EndShift();
        }
        else
        {
            Debug.LogError("No manager available to trigger shift end");
        }
    }
    
    /// <summary>
    /// Force timer expiration
    /// </summary>
    [ContextMenu("Force Timer Expiration")]
    public void ForceTimerExpiration()
    {
        Debug.Log("=== FORCE TIMER EXPIRATION ===");
        
        if (_shiftTimerManager != null)
        {
            // Use reflection to call private TimerExpired method
            var method = _shiftTimerManager.GetType().GetMethod("TimerExpired", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (method != null)
            {
                Debug.Log("Calling ShiftTimerManager.TimerExpired()...");
                method.Invoke(_shiftTimerManager, null);
            }
            else
            {
                Debug.LogError("Could not access TimerExpired method");
            }
        }
        else
        {
            Debug.LogError("ShiftTimerManager not found - cannot force timer expiration");
        }
    }
}