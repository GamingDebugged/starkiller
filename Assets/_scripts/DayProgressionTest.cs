using UnityEngine;
using Starkiller.Core.Managers;
using Starkiller.Core;

/// <summary>
/// Simple test script to bypass all intermediate panels and directly test day progression
/// Add this to a GameObject and call the public methods from Unity Inspector or other scripts
/// </summary>
public class DayProgressionTest : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    [Header("Quick Test Buttons")]
    [SerializeField] private bool runCompleteDayCycleTest = false;
    [SerializeField] private bool runDirectDayIncrementTest = false;
    [SerializeField] private bool runShipGenerationTest = false;
    [SerializeField] private bool runForceDisplayTest = false;
    
    private void Update()
    {
        if (runCompleteDayCycleTest)
        {
            runCompleteDayCycleTest = false;
            TestCompleteDayCycle();
        }
        
        if (runDirectDayIncrementTest)
        {
            runDirectDayIncrementTest = false;
            TestDirectDayIncrement();
        }
        
        if (runShipGenerationTest)
        {
            runShipGenerationTest = false;
            TestShipGeneration();
        }
        
        if (runForceDisplayTest)
        {
            runForceDisplayTest = false;
            TestForceDisplayFirstEncounter();
        }
    }
    
    [ContextMenu("Test: Direct Day Increment")]
    public void TestDirectDayIncrement()
    {
        var dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
        if (dayProgressionManager != null)
        {
            int previousDay = dayProgressionManager.CurrentDay;
            Debug.Log($"[DayProgressionTest] Before increment: Day {previousDay}");
            
            dayProgressionManager.StartNewDay();
            
            int newDay = dayProgressionManager.CurrentDay;
            Debug.Log($"[DayProgressionTest] After increment: Day {newDay}");
            
            if (newDay == previousDay + 1)
            {
                Debug.Log("✅ [DayProgressionTest] Day increment working correctly!");
            }
            else
            {
                Debug.LogError($"❌ [DayProgressionTest] Day increment failed! Expected {previousDay + 1}, got {newDay}");
            }
        }
        else
        {
            Debug.LogError("[DayProgressionTest] DayProgressionManager not found!");
        }
    }
    
    [ContextMenu("Test: Direct Start Shift")]
    public void TestDirectStartShift()
    {
        var dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
        if (dayProgressionManager != null)
        {
            Debug.Log($"[DayProgressionTest] Starting shift for Day {dayProgressionManager.CurrentDay}");
            dayProgressionManager.StartShift();
            
            if (dayProgressionManager.IsShiftActive)
            {
                Debug.Log("✅ [DayProgressionTest] Shift started successfully!");
            }
            else
            {
                Debug.LogError("❌ [DayProgressionTest] Shift failed to start!");
            }
        }
        else
        {
            Debug.LogError("[DayProgressionTest] DayProgressionManager not found!");
        }
    }
    
    [ContextMenu("Test: Check Ship Generation")]
    public void TestShipGeneration()
    {
        var masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        if (masterShipGenerator != null)
        {
            Debug.Log("[DayProgressionTest] Requesting encounter from MasterShipGenerator");
            
            var encounter = masterShipGenerator.GetNextEncounter();
            if (encounter != null)
            {
                Debug.Log($"✅ [DayProgressionTest] Got encounter: {encounter.shipType} - {encounter.captainName}");
            }
            else
            {
                Debug.LogError("❌ [DayProgressionTest] No encounter available from MasterShipGenerator!");
            }
        }
        else
        {
            Debug.LogError("[DayProgressionTest] MasterShipGenerator not found!");
        }
    }
    
    [ContextMenu("Test: Complete Day Cycle")]
    public void TestCompleteDayCycle()
    {
        Debug.Log("========== [DayProgressionTest] Starting Complete Day Cycle Test ==========");
        
        // Step 1: Check current state
        var dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
        if (dayProgressionManager == null)
        {
            Debug.LogError("[DayProgressionTest] DayProgressionManager not found!");
            return;
        }
        
        int startDay = dayProgressionManager.CurrentDay;
        Debug.Log($"[DayProgressionTest] Starting test on Day {startDay}");
        
        // Step 2: Increment day
        dayProgressionManager.StartNewDay();
        int newDay = dayProgressionManager.CurrentDay;
        Debug.Log($"[DayProgressionTest] Day incremented to {newDay}");
        
        // Step 3: Start shift
        dayProgressionManager.StartShift();
        Debug.Log($"[DayProgressionTest] Shift started for Day {newDay}, IsActive: {dayProgressionManager.IsShiftActive}");
        
        // Step 4: Test ship generation
        var masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        if (masterShipGenerator != null)
        {
            var encounter = masterShipGenerator.GetNextEncounter();
            if (encounter != null)
            {
                Debug.Log($"[DayProgressionTest] Ship generation working: {encounter.shipType}");
            }
            else
            {
                Debug.LogError("[DayProgressionTest] Ship generation failed!");
            }
        }
        
        // Step 5: Test timer
        var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
        if (shiftTimerManager != null)
        {
            Debug.Log($"[DayProgressionTest] Timer Active: {shiftTimerManager.IsTimerActive}, Remaining: {shiftTimerManager.RemainingTime:F1}s");
        }
        
        Debug.Log("========== [DayProgressionTest] Complete Day Cycle Test Finished ==========");
    }
    
    [ContextMenu("Test: Force Display First Encounter")]
    public void TestForceDisplayFirstEncounter()
    {
        Debug.Log("[DayProgressionTest] Force displaying first encounter");
        
        var masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        var credentialChecker = FindFirstObjectByType<CredentialChecker>();
        
        if (masterShipGenerator != null && credentialChecker != null)
        {
            var encounter = masterShipGenerator.GetNextEncounter();
            if (encounter != null)
            {
                Debug.Log($"[DayProgressionTest] Displaying encounter: {encounter.shipType} - {encounter.captainName}");
                credentialChecker.DisplayEncounterSafe(encounter);
            }
            else
            {
                Debug.LogError("[DayProgressionTest] No encounter to display!");
            }
        }
        else
        {
            Debug.LogError("[DayProgressionTest] Required components not found!");
        }
    }
    
    [ContextMenu("Test: Check Timer Status")]
    public void TestCheckTimerStatus()
    {
        Debug.Log("[DayProgressionTest] ========== CHECKING ALL DAY SOURCES ==========");
        
        // Check DayProgressionManager
        var dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
        if (dayProgressionManager != null)
        {
            Debug.Log($"[DayProgressionTest] DayProgressionManager Day: {dayProgressionManager.CurrentDay}");
            Debug.Log($"[DayProgressionTest] DayProgressionManager Shift Active: {dayProgressionManager.IsShiftActive}");
            Debug.Log($"[DayProgressionTest] DayProgressionManager Ships Today: {dayProgressionManager.ShipsProcessedToday}");
        }
        else
        {
            Debug.LogError("[DayProgressionTest] DayProgressionManager not found!");
        }
        
        // Check GameManager
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            Debug.Log($"[DayProgressionTest] GameManager currentDay: {gameManager.currentDay}");
            Debug.Log($"[DayProgressionTest] GameManager GetCurrentDay(): {gameManager.GetCurrentDay()}");
        }
        else
        {
            Debug.LogError("[DayProgressionTest] GameManager not found!");
        }
        
        // Check ContentManager
        var contentManager = FindFirstObjectByType<StarkillerBaseCommand.StarkkillerContentManager>();
        if (contentManager != null)
        {
            Debug.Log($"[DayProgressionTest] ContentManager currentDay: {contentManager.currentDay}");
        }
        else
        {
            Debug.LogError("[DayProgressionTest] ContentManager not found!");
        }
        
        // Check Timer
        var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
        if (shiftTimerManager != null)
        {
            Debug.Log($"[DayProgressionTest] Timer Active: {shiftTimerManager.IsTimerActive}");
            Debug.Log($"[DayProgressionTest] Remaining Time: {shiftTimerManager.RemainingTime:F1}s");
            Debug.Log($"[DayProgressionTest] Total Time: {shiftTimerManager.TotalShiftTime:F1}s");
            Debug.Log($"[DayProgressionTest] Is Shift Ended: {shiftTimerManager.IsShiftEnded}");
        }
        else
        {
            Debug.LogError("[DayProgressionTest] ShiftTimerManager not found!");
        }
        
        Debug.Log("[DayProgressionTest] ========== END DAY SOURCE CHECK ==========");
    }
}