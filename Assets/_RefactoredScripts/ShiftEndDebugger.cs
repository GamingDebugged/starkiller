using UnityEngine;
using Starkiller.Core.Managers;
using Starkiller.Core;

/// <summary>
/// Debug helper to check if shift end sequence is working properly
/// </summary>
public class ShiftEndDebugger : MonoBehaviour
{
    [Header("Debug Controls")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool showManagerStatus = true;
    
    private ShiftTimerManager _shiftTimerManager;
    private DayProgressionManager _dayProgressionManager;
    private RefactoredDailyReportManager _dailyReportManager;
    
    private void Start()
    {
        // Get manager references
        _shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
        _dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
        _dailyReportManager = ServiceLocator.Get<RefactoredDailyReportManager>();
        
        // Subscribe to events for debugging
        if (_shiftTimerManager != null)
        {
            ShiftTimerManager.OnTimerExpired += OnTimerExpired;
        }
        
        if (_dayProgressionManager != null)
        {
            DayProgressionManager.OnShiftEnded += OnShiftEnded;
        }
        
        if (_dailyReportManager != null)
        {
            RefactoredDailyReportManager.OnDailyReportGenerated += OnDailyReportGenerated;
        }
        
        // Check manager status
        if (showManagerStatus)
        {
            CheckManagerStatus();
        }
    }
    
    private void CheckManagerStatus()
    {
        Debug.Log("=== SHIFT END DEBUGGER - MANAGER STATUS ===");
        
        Debug.Log($"ShiftTimerManager: {(_shiftTimerManager != null ? "✓ FOUND" : "✗ MISSING")}");
        if (_shiftTimerManager != null)
        {
            Debug.Log($"  - Timer Active: {_shiftTimerManager.IsTimerActive}");
            Debug.Log($"  - Remaining Time: {_shiftTimerManager.RemainingTime:F1}s");
            Debug.Log($"  - Shift Ended: {_shiftTimerManager.IsShiftEnded}");
        }
        
        Debug.Log($"DayProgressionManager: {(_dayProgressionManager != null ? "✓ FOUND" : "✗ MISSING")}");
        if (_dayProgressionManager != null)
        {
            Debug.Log($"  - Current Day: {_dayProgressionManager.CurrentDay}");
            Debug.Log($"  - Shift Active: {_dayProgressionManager.IsShiftActive}");
            Debug.Log($"  - Ships Processed: {_dayProgressionManager.ShipsProcessedToday}");
        }
        
        Debug.Log($"RefactoredDailyReportManager: {(_dailyReportManager != null ? "✓ FOUND" : "✗ MISSING")}");
        if (_dailyReportManager != null)
        {
            Debug.Log($"  - Report Active: {_dailyReportManager.IsReportActive}");
            Debug.Log($"  - Current Day: {_dailyReportManager.CurrentDay}");
        }
        
        Debug.Log("=== END MANAGER STATUS ===");
    }
    
    private void OnTimerExpired()
    {
        if (enableDebugLogs)
            Debug.Log("[ShiftEndDebugger] 🔥 Timer expired event received!");
    }
    
    private void OnShiftEnded()
    {
        if (enableDebugLogs)
            Debug.Log("[ShiftEndDebugger] 🔥 Shift ended event received!");
    }
    
    private void OnDailyReportGenerated(DailyReport report)
    {
        if (enableDebugLogs)
            Debug.Log($"[ShiftEndDebugger] 🔥 Daily report generated for day {report.Day}!");
    }
    
    [ContextMenu("Check Manager Status")]
    private void DebugCheckManagerStatus()
    {
        CheckManagerStatus();
    }
    
    [ContextMenu("Force Timer to 5 seconds")]
    private void ForceTimerToFiveSeconds()
    {
        if (_shiftTimerManager != null)
        {
            _shiftTimerManager.SetRemainingTime(5f);
            Debug.Log("[ShiftEndDebugger] Timer set to 5 seconds!");
        }
        else
        {
            Debug.LogError("[ShiftEndDebugger] ShiftTimerManager not found!");
        }
    }
    
    [ContextMenu("Force End Shift")]
    private void ForceEndShift()
    {
        if (_dayProgressionManager != null)
        {
            _dayProgressionManager.EndShift();
            Debug.Log("[ShiftEndDebugger] Shift ended manually!");
        }
        else
        {
            Debug.LogError("[ShiftEndDebugger] DayProgressionManager not found!");
        }
    }
    
    [ContextMenu("Force Generate Daily Report")]
    private void ForceGenerateDailyReport()
    {
        if (_dailyReportManager != null)
        {
            _dailyReportManager.GenerateDailyReport();
            Debug.Log("[ShiftEndDebugger] Daily report generated manually!");
        }
        else
        {
            Debug.LogError("[ShiftEndDebugger] RefactoredDailyReportManager not found!");
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (_shiftTimerManager != null)
        {
            ShiftTimerManager.OnTimerExpired -= OnTimerExpired;
        }
        
        if (_dayProgressionManager != null)
        {
            DayProgressionManager.OnShiftEnded -= OnShiftEnded;
        }
        
        if (_dailyReportManager != null)
        {
            RefactoredDailyReportManager.OnDailyReportGenerated -= OnDailyReportGenerated;
        }
    }
}