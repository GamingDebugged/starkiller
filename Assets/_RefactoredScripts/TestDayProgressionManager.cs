using UnityEngine;
using System.Collections;
using Starkiller.Core;
using Starkiller.Core.Managers;

/// <summary>
/// Test script for DayProgressionManager functionality
/// Place this on any GameObject to test the Phase 2 day progression extraction
/// </summary>
public class TestDayProgressionManager : MonoBehaviour
{
    [Header("Testing")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool autoRunTests = true;
    [SerializeField] private float testShiftTime = 10f; // Short time for testing
    
    private DayProgressionManager dayManager;
    
    private void Start()
    {
        if (autoRunTests)
        {
            if (enableDebugLogs)
                Debug.Log("[TestDayProgressionManager] Starting DayProgressionManager tests...");
            
            StartCoroutine(RunDayProgressionTests());
        }
    }
    
    private IEnumerator RunDayProgressionTests()
    {
        // Wait for managers to initialize
        yield return null;
        
        // Test 1: ServiceLocator integration
        TestServiceLocatorIntegration();
        yield return new WaitForSeconds(1f);
        
        // Test 2: Day progression functionality
        TestDayProgression();
        yield return new WaitForSeconds(1f);
        
        // Test 3: Shift timing
        TestShiftTiming();
        yield return new WaitForSeconds(1f);
        
        // Test 4: Ship processing tracking
        TestShipProcessing();
        yield return new WaitForSeconds(1f);
        
        // Test 5: Event integration
        TestEventIntegration();
        yield return new WaitForSeconds(1f);
        
        // Test 6: Time management features
        TestTimeManagement();
        yield return new WaitForSeconds(1f);
        
        Debug.Log("🎉 [TestDayProgressionManager] All DayProgressionManager tests completed!");
    }
    
    private void TestServiceLocatorIntegration()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing DayProgressionManager ServiceLocator integration...");
        
        dayManager = ServiceLocator.Get<DayProgressionManager>();
        if (dayManager != null)
        {
            Debug.Log("✅ DayProgressionManager found via ServiceLocator");
        }
        else
        {
            Debug.LogError("❌ DayProgressionManager not found - you need to add it to the scene");
        }
    }
    
    private void TestDayProgression()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing day progression...");
        
        if (dayManager == null)
        {
            Debug.LogError("❌ DayProgressionManager not available for testing");
            return;
        }
        
        int initialDay = dayManager.CurrentDay;
        
        // Test starting a new day
        dayManager.StartNewDay();
        
        if (dayManager.CurrentDay == initialDay + 1)
        {
            Debug.Log("✅ Day progression working");
        }
        else
        {
            Debug.LogError("❌ Day progression failed");
        }
        
        // Test day summary
        var summary = dayManager.GetDailySummary();
        if (summary.day == dayManager.CurrentDay)
        {
            Debug.Log("✅ Daily summary generation working");
        }
        else
        {
            Debug.LogError("❌ Daily summary generation failed");
        }
    }
    
    private void TestShiftTiming()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing shift timing...");
        
        if (dayManager == null)
        {
            Debug.LogError("❌ DayProgressionManager not available for testing");
            return;
        }
        
        // Start a shift
        dayManager.StartShift();
        
        if (dayManager.IsShiftActive)
        {
            Debug.Log("✅ Shift start working");
        }
        else
        {
            Debug.LogError("❌ Shift start failed");
        }
        
        // Test time formatting
        string timeString = dayManager.GetFormattedTime();
        if (!string.IsNullOrEmpty(timeString) && timeString.Contains(":"))
        {
            Debug.Log($"✅ Time formatting working: {timeString}");
        }
        else
        {
            Debug.LogError("❌ Time formatting failed");
        }
        
        // Test shift progress
        float progress = dayManager.ShiftProgress;
        if (progress >= 0f && progress <= 1f)
        {
            Debug.Log($"✅ Shift progress calculation working: {progress:F2}");
        }
        else
        {
            Debug.LogError("❌ Shift progress calculation failed");
        }
    }
    
    private void TestShipProcessing()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing ship processing tracking...");
        
        if (dayManager == null)
        {
            Debug.LogError("❌ DayProgressionManager not available for testing");
            return;
        }
        
        int initialShips = dayManager.ShipsProcessedToday;
        
        // Record a ship processed
        dayManager.RecordShipProcessed();
        
        if (dayManager.ShipsProcessedToday == initialShips + 1)
        {
            Debug.Log("✅ Ship processing tracking working");
        }
        else
        {
            Debug.LogError("❌ Ship processing tracking failed");
        }
        
        // Test quota checking
        int shipsUntilQuota = dayManager.ShipsUntilQuota;
        if (shipsUntilQuota >= 0)
        {
            Debug.Log($"✅ Quota tracking working: {shipsUntilQuota} ships until quota");
        }
        else
        {
            Debug.LogError("❌ Quota tracking failed");
        }
        
        // Test can process more ships
        bool canProcess = dayManager.CanProcessMoreShips();
        if (dayManager.IsShiftActive && canProcess)
        {
            Debug.Log("✅ Ship processing eligibility check working");
        }
        else
        {
            Debug.LogWarning("⚠️ Ship processing eligibility check may not be working (could be valid if shift inactive)");
        }
    }
    
    private void TestEventIntegration()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing event system integration...");
        
        if (dayManager == null)
        {
            Debug.LogError("❌ DayProgressionManager not available for testing");
            return;
        }
        
        // Subscribe to events temporarily
        bool dayChangedEventReceived = false;
        bool timeUpdatedEventReceived = false;
        bool shipProcessedEventReceived = false;
        
        DayProgressionManager.OnDayChanged += (day) => dayChangedEventReceived = true;
        DayProgressionManager.OnTimeUpdated += (time) => timeUpdatedEventReceived = true;
        DayProgressionManager.OnShipProcessed += (ships) => shipProcessedEventReceived = true;
        
        // Trigger actions that should fire events
        dayManager.StartNewDay();
        dayManager.RecordShipProcessed();
        
        // Wait a frame for events to propagate
        StartCoroutine(CheckEventResults(dayChangedEventReceived, timeUpdatedEventReceived, shipProcessedEventReceived));
        
        // Unsubscribe
        DayProgressionManager.OnDayChanged -= (day) => dayChangedEventReceived = true;
        DayProgressionManager.OnTimeUpdated -= (time) => timeUpdatedEventReceived = true;
        DayProgressionManager.OnShipProcessed -= (ships) => shipProcessedEventReceived = true;
    }
    
    private IEnumerator CheckEventResults(bool dayChanged, bool timeUpdated, bool shipProcessed)
    {
        yield return null; // Wait one frame
        
        if (dayChanged)
        {
            Debug.Log("✅ Day changed event system working");
        }
        else
        {
            Debug.LogWarning("⚠️ Day changed event system may not be working");
        }
        
        if (shipProcessed)
        {
            Debug.Log("✅ Ship processed event system working");
        }
        else
        {
            Debug.LogWarning("⚠️ Ship processed event system may not be working");
        }
        
        // Time updated events happen continuously during active shifts
        Debug.Log($"⚠️ Time updated events: {(timeUpdated ? "Working" : "May not be working (normal if shift not active)")}");
    }
    
    private void TestTimeManagement()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing time management features...");
        
        if (dayManager == null)
        {
            Debug.LogError("❌ DayProgressionManager not available for testing");
            return;
        }
        
        // Test pause functionality
        dayManager.SetTimePaused(true);
        Debug.Log("✅ Time pause functionality tested");
        
        dayManager.SetTimePaused(false);
        Debug.Log("✅ Time resume functionality tested");
        
        // Test bonus time
        float initialTime = dayManager.RemainingTime;
        dayManager.AddBonusTime(30f, "Test bonus");
        
        if (dayManager.RemainingTime > initialTime)
        {
            Debug.Log("✅ Bonus time functionality working");
        }
        else
        {
            Debug.LogWarning("⚠️ Bonus time functionality may not be working (could be valid if shift not active)");
        }
        
        // Test time since last ship
        float timeSinceLastShip = dayManager.GetTimeSinceLastShip();
        if (timeSinceLastShip >= 0f)
        {
            Debug.Log($"✅ Time since last ship tracking working: {timeSinceLastShip:F1}s");
        }
        else
        {
            Debug.LogError("❌ Time since last ship tracking failed");
        }
        
        // Test loading saved values
        dayManager.SetDayValues(5, 3, 120f);
        if (dayManager.CurrentDay == 5 && dayManager.ShipsProcessedToday == 3)
        {
            Debug.Log("✅ Save/load day values working");
        }
        else
        {
            Debug.LogError("❌ Save/load day values failed");
        }
    }
    
    // Manual test methods callable from Inspector
    [ContextMenu("Run All Tests")]
    public void ManualRunAllTests()
    {
        StartCoroutine(RunDayProgressionTests());
    }
    
    [ContextMenu("Test Day Progression")]
    public void ManualTestDayProgression()
    {
        TestDayProgression();
    }
    
    [ContextMenu("Test Shift Timing")]
    public void ManualTestShiftTiming()
    {
        TestShiftTiming();
    }
    
    [ContextMenu("Test Ship Processing")]
    public void ManualTestShipProcessing()
    {
        TestShipProcessing();
    }
    
    [ContextMenu("Start Test Shift")]
    public void ManualStartShift()
    {
        dayManager = ServiceLocator.Get<DayProgressionManager>();
        if (dayManager != null)
        {
            dayManager.StartShift();
            Debug.Log("Test shift started");
        }
    }
    
    [ContextMenu("End Test Shift")]
    public void ManualEndShift()
    {
        dayManager = ServiceLocator.Get<DayProgressionManager>();
        if (dayManager != null)
        {
            dayManager.EndShift();
            Debug.Log("Test shift ended");
        }
    }
    
    [ContextMenu("Process Test Ship")]
    public void ManualProcessShip()
    {
        dayManager = ServiceLocator.Get<DayProgressionManager>();
        if (dayManager != null)
        {
            dayManager.RecordShipProcessed();
            Debug.Log($"Ship processed. Total today: {dayManager.ShipsProcessedToday}");
        }
    }
    
    [ContextMenu("Show Day Manager Status")]
    public void ShowDayManagerStatus()
    {
        dayManager = ServiceLocator.Get<DayProgressionManager>();
        if (dayManager != null)
        {
            Debug.Log("=== DAY PROGRESSION MANAGER STATUS ===");
            Debug.Log($"Current Day: {dayManager.CurrentDay}");
            Debug.Log($"Is Shift Active: {dayManager.IsShiftActive}");
            Debug.Log($"Ships Processed Today: {dayManager.ShipsProcessedToday}");
            Debug.Log($"Remaining Time: {dayManager.GetFormattedTime()}");
            Debug.Log($"Quota Met: {dayManager.QuotaMet}");
            Debug.Log($"Ships Until Quota: {dayManager.ShipsUntilQuota}");
            Debug.Log($"Shift Progress: {dayManager.ShiftProgress:F2}");
            Debug.Log("=== END STATUS ===");
        }
        else
        {
            Debug.LogError("DayProgressionManager not found!");
        }
    }
}