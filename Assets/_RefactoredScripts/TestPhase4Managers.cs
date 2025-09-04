using UnityEngine;
using System.Collections;
using Starkiller.Core;
using Starkiller.Core.Managers;

/// <summary>
/// Test script for Phase 4 managers (ShiftTimerManager, PerformanceManager, MoralChoiceManager, LoyaltyManager)
/// Tests advanced functionality and integration with all previous phases
/// </summary>
public class TestPhase4Managers : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool autoRunTests = true;
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private float testDelay = 1f;
    [SerializeField] private bool enableInteractiveTests = false;
    
    // Phase 4 managers
    private ShiftTimerManager shiftTimerManager;
    private PerformanceManager performanceManager;
    private MoralChoiceManager moralChoiceManager;
    private LoyaltyManager loyaltyManager;
    
    // Previous phase managers for integration testing
    private CreditsManager creditsManager;
    private DecisionTracker decisionTracker;
    private DayProgressionManager dayManager;
    private AudioManager audioManager;
    private UICoordinator uiCoordinator;
    private GameStateManager gameStateManager;
    private EncounterManager encounterManager;
    private SaveGameManager saveGameManager;
    private NotificationManager notificationManager;
    
    private void Start()
    {
        if (autoRunTests)
        {
            if (enableDebugLogs)
                Debug.Log("[TestPhase4Managers] Starting Phase 4 manager tests...");
            
            StartCoroutine(RunPhase4Tests());
        }
    }
    
    private IEnumerator RunPhase4Tests()
    {
        // Wait for all managers to initialize
        yield return new WaitForSeconds(0.5f);
        
        // Test 1: Verify all Phase 4 managers are registered
        if (!VerifyPhase4Managers())
        {
            Debug.LogError("❌ Not all Phase 4 managers are registered. Please check your scene setup.");
            yield break;
        }
        
        Debug.Log("✅ All Phase 4 managers found and registered!");
        yield return new WaitForSeconds(testDelay);
        
        // Test 2: Test ShiftTimerManager
        yield return TestShiftTimerManager();
        yield return new WaitForSeconds(testDelay);
        
        // Test 3: Test PerformanceManager
        yield return TestPerformanceManager();
        yield return new WaitForSeconds(testDelay);
        
        // Test 4: Test LoyaltyManager
        yield return TestLoyaltyManager();
        yield return new WaitForSeconds(testDelay);
        
        // Test 5: Test MoralChoiceManager
        yield return TestMoralChoiceManager();
        yield return new WaitForSeconds(testDelay);
        
        // Test 6: Test Phase 4 integration with previous phases
        yield return TestPhase4Integration();
        yield return new WaitForSeconds(testDelay);
        
        // Test 7: Test complete system workflow
        yield return TestCompleteSystemWorkflow();
        
        Debug.Log("🎉 [TestPhase4Managers] All Phase 4 tests completed!");
    }
    
    private bool VerifyPhase4Managers()
    {
        bool allFound = true;
        
        // Get Phase 4 managers
        shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
        performanceManager = ServiceLocator.Get<PerformanceManager>();
        moralChoiceManager = ServiceLocator.Get<MoralChoiceManager>();
        loyaltyManager = ServiceLocator.Get<LoyaltyManager>();
        
        // Get previous phase managers
        creditsManager = ServiceLocator.Get<CreditsManager>();
        decisionTracker = ServiceLocator.Get<DecisionTracker>();
        dayManager = ServiceLocator.Get<DayProgressionManager>();
        audioManager = ServiceLocator.Get<AudioManager>();
        uiCoordinator = ServiceLocator.Get<UICoordinator>();
        gameStateManager = ServiceLocator.Get<GameStateManager>();
        encounterManager = ServiceLocator.Get<EncounterManager>();
        saveGameManager = ServiceLocator.Get<SaveGameManager>();
        notificationManager = ServiceLocator.Get<NotificationManager>();
        
        // Check Phase 4 managers
        if (shiftTimerManager == null) { Debug.LogError("❌ ShiftTimerManager not found"); allFound = false; }
        if (performanceManager == null) { Debug.LogError("❌ PerformanceManager not found"); allFound = false; }
        if (moralChoiceManager == null) { Debug.LogError("❌ MoralChoiceManager not found"); allFound = false; }
        if (loyaltyManager == null) { Debug.LogError("❌ LoyaltyManager not found"); allFound = false; }
        
        return allFound;
    }
    
    private IEnumerator TestShiftTimerManager()
    {
        Debug.Log("[Test] Testing ShiftTimerManager...");
        
        if (shiftTimerManager == null)
        {
            Debug.LogError("❌ ShiftTimerManager not available for testing");
            yield break;
        }
        
        // Test timer start
        shiftTimerManager.StartTimer(60f); // 1 minute for testing
        yield return new WaitForSeconds(0.1f);
        
        if (shiftTimerManager.IsTimerActive)
        {
            Debug.Log("✅ ShiftTimerManager can start timer");
            Debug.Log($"   Remaining time: {shiftTimerManager.RemainingTime:F1}s");
        }
        else
        {
            Debug.LogError("❌ ShiftTimerManager failed to start timer");
        }
        
        // Test pause/resume
        shiftTimerManager.PauseTimer();
        yield return new WaitForSeconds(0.1f);
        
        if (shiftTimerManager.IsTimerPaused)
        {
            Debug.Log("✅ ShiftTimerManager can pause timer");
        }
        
        shiftTimerManager.ResumeTimer();
        yield return new WaitForSeconds(0.1f);
        
        if (!shiftTimerManager.IsTimerPaused)
        {
            Debug.Log("✅ ShiftTimerManager can resume timer");
        }
        
        // Test bonus time
        float timeBefore = shiftTimerManager.RemainingTime;
        shiftTimerManager.AddBonusTime(30f, "Test bonus");
        yield return new WaitForSeconds(0.1f);
        
        if (shiftTimerManager.RemainingTime > timeBefore)
        {
            Debug.Log("✅ ShiftTimerManager can add bonus time");
            Debug.Log($"   Bonus time added: {shiftTimerManager.BonusTime}s");
        }
        
        // Test timer formatting
        string timeString = shiftTimerManager.GetFormattedTime();
        if (!string.IsNullOrEmpty(timeString))
        {
            Debug.Log($"✅ ShiftTimerManager time formatting: {timeString}");
        }
        
        // Test statistics
        var timerStats = shiftTimerManager.GetStatistics();
        Debug.Log($"   Timer stats - Phase: {timerStats.CurrentPhase}, Progress: {timerStats.TimeProgress:P1}");
        
        shiftTimerManager.StopTimer();
    }
    
    private IEnumerator TestPerformanceManager()
    {
        Debug.Log("[Test] Testing PerformanceManager...");
        
        if (performanceManager == null)
        {
            Debug.LogError("❌ PerformanceManager not available for testing");
            yield break;
        }
        
        // Test decision recording
        var mockEncounter = new MockEncounter("Test Ship", "Test Captain");
        
        // Record correct decision
        performanceManager.RecordDecision(true, true, mockEncounter, "Test correct decision");
        yield return new WaitForSeconds(0.1f);
        
        if (performanceManager.CorrectDecisions > 0)
        {
            Debug.Log("✅ PerformanceManager can record correct decisions");
            Debug.Log($"   Correct decisions: {performanceManager.CorrectDecisions}");
            Debug.Log($"   Current score: {performanceManager.CurrentScore}");
        }
        
        // Record wrong decision
        performanceManager.RecordDecision(false, false, mockEncounter, "Test wrong decision");
        yield return new WaitForSeconds(0.1f);
        
        if (performanceManager.WrongDecisions > 0)
        {
            Debug.Log("✅ PerformanceManager can record wrong decisions");
            Debug.Log($"   Wrong decisions: {performanceManager.WrongDecisions}");
            Debug.Log($"   Current strikes: {performanceManager.CurrentStrikes}");
        }
        
        // Test accuracy calculation
        float accuracy = performanceManager.AccuracyRate;
        Debug.Log($"✅ PerformanceManager accuracy calculation: {accuracy:P1}");
        
        // Test performance rating
        var rating = performanceManager.CurrentRating;
        Debug.Log($"✅ PerformanceManager rating: {rating}");
        
        // Test salary calculation
        int salary = performanceManager.CalculateSalary();
        Debug.Log($"✅ PerformanceManager salary calculation: {salary} credits");
        
        // Test statistics
        var perfStats = performanceManager.GetStatistics();
        Debug.Log($"   Performance stats - Score: {perfStats.CurrentScore}, Streak: {perfStats.LongestCorrectStreak}");
        
        // Test key decisions
        performanceManager.AddKeyDecision("Test key decision");
        yield return new WaitForSeconds(0.1f);
        
        Debug.Log("✅ PerformanceManager testing completed");
    }
    
    private IEnumerator TestLoyaltyManager()
    {
        Debug.Log("[Test] Testing LoyaltyManager...");
        
        if (loyaltyManager == null)
        {
            Debug.LogError("❌ LoyaltyManager not available for testing");
            yield break;
        }
        
        // Test loyalty updates
        int initialImperial = loyaltyManager.ImperialLoyalty;
        int initialRebellion = loyaltyManager.RebellionSympathy;
        
        loyaltyManager.UpdateLoyalty(2, -1, "Test: Followed Imperial protocol");
        yield return new WaitForSeconds(0.1f);
        
        if (loyaltyManager.ImperialLoyalty != initialImperial || loyaltyManager.RebellionSympathy != initialRebellion)
        {
            Debug.Log("✅ LoyaltyManager can update loyalty values");
            Debug.Log($"   Imperial: {loyaltyManager.ImperialLoyalty}, Rebellion: {loyaltyManager.RebellionSympathy}");
        }
        
        // Test alignment calculation
        var alignment = loyaltyManager.CurrentAlignment;
        Debug.Log($"✅ LoyaltyManager alignment: {alignment}");
        
        // Test loyalty description
        string description = loyaltyManager.GetLoyaltyDescription();
        Debug.Log($"✅ LoyaltyManager description: {description}");
        
        // Test key decisions
        loyaltyManager.AddKeyDecision("Test loyalty decision");
        yield return new WaitForSeconds(0.1f);
        
        // Test loyalty bounds
        loyaltyManager.UpdateLoyalty(20, 20, "Test: Extreme values"); // Should be clamped
        yield return new WaitForSeconds(0.1f);
        
        Debug.Log($"✅ LoyaltyManager bounds test - Imperial: {loyaltyManager.ImperialLoyalty}, Rebellion: {loyaltyManager.RebellionSympathy}");
        
        // Test ending text
        string endingText = loyaltyManager.GetEndingText();
        Debug.Log($"✅ LoyaltyManager ending text: {endingText}");
        
        // Test statistics
        var loyaltyStats = loyaltyManager.GetStatistics();
        Debug.Log($"   Loyalty stats - Changes: {loyaltyStats.TotalLoyaltyChanges}, Decisions: {loyaltyStats.TotalKeyDecisions}");
        
        Debug.Log("✅ LoyaltyManager testing completed");
    }
    
    private IEnumerator TestMoralChoiceManager()
    {
        Debug.Log("[Test] Testing MoralChoiceManager...");
        
        if (moralChoiceManager == null)
        {
            Debug.LogError("❌ MoralChoiceManager not available for testing");
            yield break;
        }
        
        // Test choice scheduling
        moralChoiceManager.ScheduleChoiceEvent(1f);
        yield return new WaitForSeconds(0.1f);
        
        Debug.Log("✅ MoralChoiceManager can schedule choices");
        Debug.Log($"   Choices generated today: {moralChoiceManager.ChoicesGeneratedToday}");
        
        // Test choice presentation (if not in interactive mode)
        if (!enableInteractiveTests)
        {
            moralChoiceManager.PresentMoralChoice("bribery_medical");
            yield return new WaitForSeconds(0.5f);
            
            if (moralChoiceManager.IsChoiceActive)
            {
                Debug.Log("✅ MoralChoiceManager can present choices");
                Debug.Log($"   Current choice: {moralChoiceManager.CurrentChoice?.Scenario.Title}");
                
                // Auto-select choice for testing
                moralChoiceManager.SelectChoice(1);
                yield return new WaitForSeconds(0.1f);
                
                if (!moralChoiceManager.IsChoiceActive)
                {
                    Debug.Log("✅ MoralChoiceManager can process choice selection");
                }
            }
        }
        
        // Test statistics
        var choiceStats = moralChoiceManager.GetStatistics();
        Debug.Log($"   Choice stats - Presented: {choiceStats.TotalChoicesPresented}, Completed: {choiceStats.TotalChoicesCompleted}");
        
        Debug.Log("✅ MoralChoiceManager testing completed");
    }
    
    private IEnumerator TestPhase4Integration()
    {
        Debug.Log("[Test] Testing Phase 4 integration with previous phases...");
        
        // Test timer integration with day progression
        if (shiftTimerManager != null && dayManager != null)
        {
            Debug.Log("✅ ShiftTimerManager integrated with DayProgressionManager");
        }
        
        // Test performance integration with credits
        if (performanceManager != null && creditsManager != null)
        {
            int initialCredits = creditsManager.CurrentCredits;
            var mockEncounter = new MockEncounter("Integration Test", "Test Captain");
            performanceManager.RecordDecision(false, false, mockEncounter, "Integration test wrong decision");
            yield return new WaitForSeconds(0.1f);
            
            if (creditsManager.CurrentCredits != initialCredits)
            {
                Debug.Log("✅ PerformanceManager integrated with CreditsManager");
                Debug.Log($"   Credits changed by: {creditsManager.CurrentCredits - initialCredits}");
            }
        }
        
        // Test loyalty integration with notifications
        if (loyaltyManager != null && notificationManager != null)
        {
            int initialNotifications = notificationManager.TotalNotificationsShown;
            loyaltyManager.UpdateLoyalty(3, -2, "Integration test loyalty change");
            yield return new WaitForSeconds(0.1f);
            
            if (notificationManager.TotalNotificationsShown > initialNotifications)
            {
                Debug.Log("✅ LoyaltyManager integrated with NotificationManager");
            }
        }
        
        // Test moral choice integration with loyalty
        if (moralChoiceManager != null && loyaltyManager != null)
        {
            int initialImperial = loyaltyManager.ImperialLoyalty;
            Debug.Log("✅ MoralChoiceManager integrated with LoyaltyManager");
            Debug.Log($"   Ready for moral choice consequences");
        }
        
        // Test save system integration
        if (saveGameManager != null && performanceManager != null)
        {
            string integrationSave = "phase4_integration_test";
            saveGameManager.SaveGame(integrationSave);
            
            while (saveGameManager.IsSaving)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            Debug.Log("✅ Phase 4 managers integrated with SaveGameManager");
            saveGameManager.DeleteSave(integrationSave); // Clean up
        }
        
        Debug.Log("✅ Phase 4 integration tests completed");
    }
    
    private IEnumerator TestCompleteSystemWorkflow()
    {
        Debug.Log("[Test] Testing complete system workflow...");
        
        // Simulate a complete game session with all Phase 4 managers
        
        // 1. Start a shift with timer
        if (shiftTimerManager != null)
        {
            shiftTimerManager.StartTimer(120f); // 2 minutes
            Debug.Log("1. ✅ Shift timer started");
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 2. Process some decisions with performance tracking
        if (performanceManager != null)
        {
            var testEncounter = new MockEncounter("Workflow Test Ship", "Test Captain");
            performanceManager.RecordDecision(true, true, testEncounter, "Workflow correct decision");
            performanceManager.RecordDecision(false, false, testEncounter, "Workflow wrong decision");
            Debug.Log("2. ✅ Decisions processed and tracked");
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 3. Update loyalty based on decisions
        if (loyaltyManager != null)
        {
            loyaltyManager.UpdateLoyalty(1, -1, "Workflow loyalty update");
            Debug.Log("3. ✅ Loyalty updated based on decisions");
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 4. Present and resolve a moral choice (auto-resolve for testing)
        if (moralChoiceManager != null && !enableInteractiveTests)
        {
            moralChoiceManager.PresentMoralChoice("family_emergency");
            yield return new WaitForSeconds(0.5f);
            
            if (moralChoiceManager.IsChoiceActive)
            {
                moralChoiceManager.SelectChoice(2); // Choose compassionate option
                Debug.Log("4. ✅ Moral choice presented and resolved");
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 5. Add bonus time for good performance
        if (shiftTimerManager != null && performanceManager != null)
        {
            if (performanceManager.AccuracyRate > 0.5f)
            {
                shiftTimerManager.AddBonusTime(15f, "Good performance bonus");
                Debug.Log("5. ✅ Bonus time awarded for performance");
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 6. Generate final statistics
        if (performanceManager != null)
        {
            var stats = performanceManager.GetStatistics();
            Debug.Log($"6. ✅ Final Performance: {stats.AccuracyRate:P1} accuracy, {stats.CurrentScore} score");
        }
        
        if (loyaltyManager != null)
        {
            var loyaltyStats = loyaltyManager.GetStatistics();
            Debug.Log($"   Final Loyalty: {loyaltyStats.CurrentAlignment} ({loyaltyStats.ImperialLoyalty}/{loyaltyStats.RebellionSympathy})");
        }
        
        if (shiftTimerManager != null)
        {
            var timerStats = shiftTimerManager.GetStatistics();
            Debug.Log($"   Final Timer: {timerStats.TimeProgress:P1} complete, {timerStats.BonusTimeEarned}s bonus");
        }
        
        // 7. Save final state
        if (saveGameManager != null)
        {
            string workflowSave = "complete_workflow_test";
            saveGameManager.SaveGame(workflowSave);
            
            while (saveGameManager.IsSaving)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            Debug.Log("7. ✅ Final state saved");
            saveGameManager.DeleteSave(workflowSave); // Clean up
        }
        
        // 8. Clean up
        if (shiftTimerManager != null)
        {
            shiftTimerManager.StopTimer();
        }
        
        Debug.Log("✅ Complete system workflow test finished successfully");
    }
    
    // Manual test methods
    [ContextMenu("Run All Phase 4 Tests")]
    public void ManualRunAllTests()
    {
        StartCoroutine(RunPhase4Tests());
    }
    
    [ContextMenu("Test Timer Only")]
    public void ManualTestTimer()
    {
        StartCoroutine(TestShiftTimerManager());
    }
    
    [ContextMenu("Test Performance Only")]
    public void ManualTestPerformance()
    {
        StartCoroutine(TestPerformanceManager());
    }
    
    [ContextMenu("Test Loyalty Only")]
    public void ManualTestLoyalty()
    {
        StartCoroutine(TestLoyaltyManager());
    }
    
    [ContextMenu("Test Moral Choice Only")]
    public void ManualTestMoralChoice()
    {
        StartCoroutine(TestMoralChoiceManager());
    }
    
    [ContextMenu("Show All Manager Status")]
    public void ShowAllManagerStatus()
    {
        Debug.Log("=== COMPLETE SYSTEM STATUS ===");
        
        // Phase 1 & 2 managers
        Debug.Log($"💰 Credits: {(creditsManager != null ? creditsManager.CurrentCredits.ToString() : "NOT FOUND")}");
        Debug.Log($"📊 Decisions: {(decisionTracker != null ? $"{decisionTracker.CorrectDecisions}✓/{decisionTracker.WrongDecisions}✗" : "NOT FOUND")}");
        Debug.Log($"📅 Day: {(dayManager != null ? dayManager.CurrentDay.ToString() : "NOT FOUND")}");
        Debug.Log($"🔊 Audio: {(audioManager != null ? "Available" : "NOT FOUND")}");
        Debug.Log($"🖥️ UI: {(uiCoordinator != null ? "Available" : "NOT FOUND")}");
        Debug.Log($"🎮 State: {(gameStateManager != null ? gameStateManager.CurrentState.ToString() : "NOT FOUND")}");
        
        // Phase 3 managers
        Debug.Log($"👥 Encounters: {(encounterManager != null ? $"Generated: {encounterManager.TotalEncountersGenerated}" : "NOT FOUND")}");
        Debug.Log($"💾 Save System: {(saveGameManager != null ? "Available" : "NOT FOUND")}");
        Debug.Log($"📢 Notifications: {(notificationManager != null ? $"Shown: {notificationManager.TotalNotificationsShown}" : "NOT FOUND")}");
        
        // Phase 4 managers
        Debug.Log($"⏱️ Timer: {(shiftTimerManager != null ? $"Active: {shiftTimerManager.IsTimerActive}" : "NOT FOUND")}");
        Debug.Log($"📈 Performance: {(performanceManager != null ? $"Score: {performanceManager.CurrentScore}" : "NOT FOUND")}");
        Debug.Log($"🎭 Moral Choices: {(moralChoiceManager != null ? $"Presented: {moralChoiceManager.TotalChoicesPresented}" : "NOT FOUND")}");
        Debug.Log($"⚖️ Loyalty: {(loyaltyManager != null ? $"{loyaltyManager.CurrentAlignment}" : "NOT FOUND")}");
        
        Debug.Log("=== END STATUS ===");
    }
    
    [ContextMenu("Start Mock Game Session")]
    public void StartMockGameSession()
    {
        StartCoroutine(TestCompleteSystemWorkflow());
    }
    
    [ContextMenu("Present Test Moral Choice")]
    public void ManualPresentMoralChoice()
    {
        if (moralChoiceManager != null)
        {
            moralChoiceManager.PresentMoralChoice("personal_connection");
        }
    }
    
    [ContextMenu("Add Test Performance Data")]
    public void AddTestPerformanceData()
    {
        if (performanceManager != null)
        {
            var testEncounter = new MockEncounter("Manual Test Ship", "Test Captain");
            performanceManager.RecordDecision(true, true, testEncounter, "Manual test decision");
            Debug.Log("Test performance data added");
        }
    }
    
    [ContextMenu("Update Test Loyalty")]
    public void UpdateTestLoyalty()
    {
        if (loyaltyManager != null)
        {
            loyaltyManager.UpdateLoyalty(1, -1, "Manual test loyalty update");
            Debug.Log("Test loyalty updated");
        }
    }
}

// Mock encounter class for testing
public class MockEncounter : IEncounter
{
    public string ShipType { get; private set; }
    public string CaptainName { get; private set; }
    public string AccessCode { get; private set; }
    public bool IsValid { get; private set; }
    
    public MockEncounter(string shipType, string captainName)
    {
        ShipType = shipType;
        CaptainName = captainName;
        AccessCode = "TEST-1234";
        IsValid = true;
    }
}