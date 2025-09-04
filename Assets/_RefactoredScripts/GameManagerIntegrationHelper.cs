using UnityEngine;
using Starkiller.Core.Managers;
using Starkiller.Core;

/// <summary>
/// Helper class to bridge the old GameManager with the new refactored manager architecture.
/// This provides backward compatibility while transitioning to the new system.
/// </summary>
public class GameManagerIntegrationHelper : MonoBehaviour
{
    [Header("Legacy Support")]
    [SerializeField] private bool enableLegacyBridge = true;
    [SerializeField] private bool enableDebugLogs = true;
    
    // Manager references
    private PerformanceManager _performanceManager;
    private CreditsManager _creditsManager;
    private SalaryManager _salaryManager;
    private BriberyManager _briberyManager;
    private InspectionManager _inspectionManager;
    private GameOverManager _gameOverManager;
    private RefactoredDailyReportManager _dailyReportManager;
    private DayProgressionManager _dayProgressionManager;
    private ShiftTimerManager _shiftTimerManager;
    
    // Legacy GameManager reference
    private GameManager _legacyGameManager;
    
    private void Start()
    {
        if (!enableLegacyBridge) return;
        
        // Get all manager references
        _performanceManager = ServiceLocator.Get<PerformanceManager>();
        _creditsManager = ServiceLocator.Get<CreditsManager>();
        _salaryManager = ServiceLocator.Get<SalaryManager>();
        _briberyManager = ServiceLocator.Get<BriberyManager>();
        _inspectionManager = ServiceLocator.Get<InspectionManager>();
        _gameOverManager = ServiceLocator.Get<GameOverManager>();
        _dailyReportManager = ServiceLocator.Get<RefactoredDailyReportManager>();
        _dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
        _shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
        
        // Find legacy GameManager
        _legacyGameManager = FindObjectOfType<GameManager>();
        
        if (enableDebugLogs)
        {
            Debug.Log("[GameManagerIntegrationHelper] Integration bridge activated");
            LogManagerStatus();
        }
    }
    
    private void LogManagerStatus()
    {
        Debug.Log("=== MANAGER INTEGRATION STATUS ===");
        Debug.Log($"PerformanceManager: {(_performanceManager != null ? "✓" : "✗")}");
        Debug.Log($"CreditsManager: {(_creditsManager != null ? "✓" : "✗")}");
        Debug.Log($"SalaryManager: {(_salaryManager != null ? "✓" : "✗")}");
        Debug.Log($"BriberyManager: {(_briberyManager != null ? "✓" : "✗")}");
        Debug.Log($"InspectionManager: {(_inspectionManager != null ? "✓" : "✗")}");
        Debug.Log($"GameOverManager: {(_gameOverManager != null ? "✓" : "✗")}");
        Debug.Log($"DailyReportManager: {(_dailyReportManager != null ? "✓" : "✗")}");
        Debug.Log($"DayProgressionManager: {(_dayProgressionManager != null ? "✓" : "✗")}");
        Debug.Log($"ShiftTimerManager: {(_shiftTimerManager != null ? "✓" : "✗")}");
        Debug.Log($"Legacy GameManager: {(_legacyGameManager != null ? "✓" : "✗")}");
        Debug.Log("=== END STATUS ===");
    }
    
    /// <summary>
    /// Compatibility methods for legacy code that calls GameManager
    /// </summary>
    
    // Get methods that CredentialChecker and other scripts might call
    public int GetShipsProcessed()
    {
        return _performanceManager?.ShipsProcessed ?? 0;
    }
    
    public int GetCredits()
    {
        return _creditsManager?.CurrentCredits ?? 0;
    }
    
    public int GetStrikes()
    {
        return _performanceManager?.CurrentStrikes ?? 0;
    }
    
    public int GetCorrectDecisions()
    {
        return _performanceManager?.CorrectDecisions ?? 0;
    }
    
    public int GetWrongDecisions()
    {
        return _performanceManager?.WrongDecisions ?? 0;
    }
    
    public int GetCurrentDay()
    {
        return _dayProgressionManager?.CurrentDay ?? 1;
    }
    
    public float GetRemainingTime()
    {
        return _shiftTimerManager?.RemainingTime ?? 0f;
    }
    
    public bool IsGameActive()
    {
        return _dayProgressionManager?.IsShiftActive ?? false;
    }
    
    // Action methods that might be called by other scripts
    public void RecordCorrectDecision(bool approved = true, string reason = "")
    {
        _performanceManager?.RecordDecision(approved, true, null, reason);
    }
    
    public void RecordWrongDecision(bool approved = false, string reason = "")
    {
        _performanceManager?.RecordDecision(approved, false, null, reason);
    }
    
    public void AddStrike(string reason = "")
    {
        var decisionTracker = ServiceLocator.Get<DecisionTracker>();
        decisionTracker?.AddStrike(reason);
    }
    
    public void AddCredits(int amount, string reason = "")
    {
        _creditsManager?.AddCredits(amount, reason);
    }
    
    public void DeductCredits(int amount, string reason = "")
    {
        _creditsManager?.DeductCredits(amount, reason);
    }
    
    public void RecordShipProcessed()
    {
        _dayProgressionManager?.RecordShipProcessed();
    }
    
    public void TriggerGameOver(string reason)
    {
        _gameOverManager?.TriggerGameOver(GameOverReason.ManualTrigger, reason);
    }
    
    public void EndShift()
    {
        _dayProgressionManager?.EndShift();
    }
    
    public void GenerateDailyReport()
    {
        _dailyReportManager?.GenerateDailyReport();
    }
    
    // Testing methods for TestingFramework
    public void TestAddStrike()
    {
        AddStrike("Test strike from framework");
    }
    
    public void TestProgressDay()
    {
        EndShift();
    }
    
    public void TestTriggerGameOver()
    {
        TriggerGameOver("Test game over from framework");
    }
    
    /// <summary>
    /// Update legacy GameManager to use new managers
    /// Call this method to synchronize old GameManager with new system
    /// </summary>
    public void SyncLegacyGameManager()
    {
        if (_legacyGameManager == null) return;
        
        // This is where you would update the legacy GameManager's state
        // to match the new managers' state
        
        if (enableDebugLogs)
            Debug.Log("[GameManagerIntegrationHelper] Legacy GameManager synchronized");
    }
    
    // Debug methods
    [ContextMenu("Test Manager Integration")]
    public void TestManagerIntegration()
    {
        Debug.Log("=== TESTING MANAGER INTEGRATION ===");
        Debug.Log($"Ships Processed: {GetShipsProcessed()}");
        Debug.Log($"Credits: {GetCredits()}");
        Debug.Log($"Strikes: {GetStrikes()}");
        Debug.Log($"Correct Decisions: {GetCorrectDecisions()}");
        Debug.Log($"Wrong Decisions: {GetWrongDecisions()}");
        Debug.Log($"Current Day: {GetCurrentDay()}");
        Debug.Log($"Remaining Time: {GetRemainingTime():F1}s");
        Debug.Log($"Game Active: {IsGameActive()}");
        Debug.Log("=== END INTEGRATION TEST ===");
    }
    
    [ContextMenu("Sync Legacy GameManager")]
    public void TestSyncLegacyGameManager()
    {
        SyncLegacyGameManager();
    }
}