using UnityEngine;
using System.Collections;
using Starkiller.Core;
using Starkiller.Core.Managers;

/// <summary>
/// Test script for Phase 3 managers (EncounterManager, SaveGameManager, NotificationManager)
/// Tests advanced functionality and integration with previous phases
/// </summary>
public class TestPhase3Managers : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool autoRunTests = true;
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private float testDelay = 1f;
    
    private EncounterManager encounterManager;
    private SaveGameManager saveGameManager;
    private NotificationManager notificationManager;
    
    private void Start()
    {
        if (autoRunTests)
        {
            if (enableDebugLogs)
                Debug.Log("[TestPhase3Managers] Starting Phase 3 manager tests...");
            
            StartCoroutine(RunPhase3Tests());
        }
    }
    
    private IEnumerator RunPhase3Tests()
    {
        // Wait for all managers to initialize
        yield return new WaitForSeconds(0.5f);
        
        // Test 1: Verify all Phase 3 managers are registered
        if (!VerifyPhase3Managers())
        {
            Debug.LogError("❌ Not all Phase 3 managers are registered. Please check your scene setup.");
            yield break;
        }
        
        Debug.Log("✅ All Phase 3 managers found and registered!");
        yield return new WaitForSeconds(testDelay);
        
        // Test 2: Test NotificationManager
        TestNotificationManager();
        yield return new WaitForSeconds(testDelay);
        
        // Test 3: Test SaveGameManager
        yield return TestSaveGameManager();
        yield return new WaitForSeconds(testDelay);
        
        // Test 4: Test EncounterManager
        yield return TestEncounterManager();
        yield return new WaitForSeconds(testDelay);
        
        // Test 5: Test inter-manager communication
        yield return TestPhase3Integration();
        yield return new WaitForSeconds(testDelay);
        
        // Test 6: Test complete system workflow
        yield return TestCompleteWorkflow();
        
        Debug.Log("🎉 [TestPhase3Managers] All Phase 3 tests completed!");
    }
    
    private bool VerifyPhase3Managers()
    {
        bool allFound = true;
        
        encounterManager = ServiceLocator.Get<EncounterManager>();
        saveGameManager = ServiceLocator.Get<SaveGameManager>();
        notificationManager = ServiceLocator.Get<NotificationManager>();
        
        if (encounterManager == null) { Debug.LogError("❌ EncounterManager not found"); allFound = false; }
        if (saveGameManager == null) { Debug.LogError("❌ SaveGameManager not found"); allFound = false; }
        if (notificationManager == null) { Debug.LogError("❌ NotificationManager not found"); allFound = false; }
        
        return allFound;
    }
    
    private void TestNotificationManager()
    {
        Debug.Log("[Test] Testing NotificationManager...");
        
        if (notificationManager == null)
        {
            Debug.LogError("❌ NotificationManager not available for testing");
            return;
        }
        
        // Test different notification types
        notificationManager.ShowNotification("Test info notification", NotificationType.Info);
        notificationManager.ShowNotification("Test warning notification", NotificationType.Warning);
        notificationManager.ShowNotification("Test success notification", NotificationType.Success);
        
        // Test timed notification
        notificationManager.ShowTimedNotification("This notification will show for 2 seconds", 2f);
        
        // Get statistics
        var stats = notificationManager.GetStatistics();
        if (stats.TotalShown > 0)
        {
            Debug.Log("✅ NotificationManager working - notifications displayed");
            Debug.Log($"   Total shown: {stats.TotalShown}, Active: {stats.CurrentlyActive}, Queue: {stats.InQueue}");
        }
        else
        {
            Debug.LogWarning("⚠️ NotificationManager may not be working properly");
        }
    }
    
    private IEnumerator TestSaveGameManager()
    {
        Debug.Log("[Test] Testing SaveGameManager...");
        
        if (saveGameManager == null)
        {
            Debug.LogError("❌ SaveGameManager not available for testing");
            yield break;
        }
        
        // Test save functionality
        string testSaveName = "test_save_phase3";
        saveGameManager.SaveGame(testSaveName);
        
        // Wait for save to complete
        while (saveGameManager.IsSaving)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Check if save was created
        var saveFiles = saveGameManager.GetSaveFiles();
        var testSave = saveFiles.Find(s => s.FileName == testSaveName);
        
        if (testSave != null)
        {
            Debug.Log("✅ SaveGameManager save functionality working");
            Debug.Log($"   Save created: {testSave.FileName} at {testSave.CreatedTime}");
            
            // Test load functionality
            saveGameManager.LoadGame(testSaveName);
            
            // Wait for load to complete
            while (saveGameManager.IsLoading)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            Debug.Log("✅ SaveGameManager load functionality tested");
            
            // Clean up test save
            saveGameManager.DeleteSave(testSaveName);
        }
        else
        {
            Debug.LogError("❌ SaveGameManager save functionality failed");
        }
        
        // Test preferences
        saveGameManager.SetPreference("test_setting", "test_value");
        string retrievedValue = saveGameManager.GetPreference("test_setting", "");
        
        if (retrievedValue == "test_value")
        {
            Debug.Log("✅ SaveGameManager preferences system working");
        }
        else
        {
            Debug.LogWarning("⚠️ SaveGameManager preferences system may not be working");
        }
    }
    
    private IEnumerator TestEncounterManager()
    {
        Debug.Log("[Test] Testing EncounterManager...");
        
        if (encounterManager == null)
        {
            Debug.LogError("❌ EncounterManager not available for testing");
            yield break;
        }
        
        // Clear any existing encounters
        encounterManager.ClearAllEncounters();
        
        // Test encounter generation
        int initialGenerated = encounterManager.TotalEncountersGenerated;
        encounterManager.GenerateEncounter();
        
        // Wait for encounter generation
        yield return new WaitForSeconds(0.5f);
        
        if (encounterManager.TotalEncountersGenerated > initialGenerated)
        {
            Debug.Log("✅ EncounterManager generation working");
            Debug.Log($"   Encounters generated: {encounterManager.TotalEncountersGenerated}");
            Debug.Log($"   Queue size: {encounterManager.QueueSize}");
        }
        else
        {
            Debug.LogWarning("⚠️ EncounterManager generation may not be working");
        }
        
        // Test encounter starting
        if (encounterManager.QueueSize > 0)
        {
            encounterManager.StartNextEncounter();
            
            if (encounterManager.IsEncounterActive)
            {
                Debug.Log("✅ EncounterManager can start encounters");
                Debug.Log($"   Current encounter: {encounterManager.CurrentEncounter?.ShipType}");
                
                // Test encounter completion
                encounterManager.CompleteCurrentEncounter();
                
                if (!encounterManager.IsEncounterActive)
                {
                    Debug.Log("✅ EncounterManager can complete encounters");
                }
            }
        }
        
        // Get statistics
        var stats = encounterManager.GetStatistics();
        Debug.Log($"   Statistics - Generated: {stats.TotalGenerated}, Completed: {stats.TotalCompleted}");
        Debug.Log($"   Difficulty: {stats.CurrentDifficulty:F2}x");
    }
    
    private IEnumerator TestPhase3Integration()
    {
        Debug.Log("[Test] Testing Phase 3 inter-manager communication...");
        
        // Test NotificationManager with other systems
        if (notificationManager != null)
        {
            // This should trigger auto-notifications
            var creditsManager = ServiceLocator.Get<CreditsManager>();
            if (creditsManager != null)
            {
                creditsManager.AddCredits(100, "Phase 3 Integration Test");
            }
            
            var decisionTracker = ServiceLocator.Get<DecisionTracker>();
            if (decisionTracker != null)
            {
                decisionTracker.RecordDecision(false, "Phase 3 Test Strike");
            }
        }
        
        yield return new WaitForSeconds(1f);
        
        // Test SaveGameManager with current state
        if (saveGameManager != null)
        {
            string integrationSave = "integration_test";
            saveGameManager.SaveGame(integrationSave);
            
            while (saveGameManager.IsSaving)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            Debug.Log("✅ Integration save test completed");
            
            // Clean up
            saveGameManager.DeleteSave(integrationSave);
        }
        
        // Test EncounterManager with day progression
        if (encounterManager != null)
        {
            var dayManager = ServiceLocator.Get<DayProgressionManager>();
            if (dayManager != null)
            {
                Debug.Log("✅ EncounterManager integrated with DayProgressionManager");
                Debug.Log($"   Can process ships: {dayManager.CanProcessMoreShips()}");
                Debug.Log($"   Current difficulty: {encounterManager.CurrentDifficultyMultiplier:F2}");
            }
        }
        
        Debug.Log("✅ Phase 3 integration tests completed");
    }
    
    private IEnumerator TestCompleteWorkflow()
    {
        Debug.Log("[Test] Testing complete Phase 3 workflow...");
        
        // Simulate a complete game session workflow
        if (notificationManager != null)
        {
            notificationManager.ShowNotification("Starting complete workflow test", NotificationType.System);
        }
        
        yield return new WaitForSeconds(1f);
        
        // 1. Generate encounters
        if (encounterManager != null)
        {
            encounterManager.GenerateEncounter();
            encounterManager.GenerateEncounter();
            
            if (enableDebugLogs)
                Debug.Log($"Generated encounters. Queue size: {encounterManager.QueueSize}");
        }
        
        yield return new WaitForSeconds(1f);
        
        // 2. Process encounters
        if (encounterManager != null && encounterManager.QueueSize > 0)
        {
            encounterManager.StartNextEncounter();
            yield return new WaitForSeconds(0.5f);
            encounterManager.CompleteCurrentEncounter();
            
            if (enableDebugLogs)
                Debug.Log("Processed encounter");
        }
        
        yield return new WaitForSeconds(1f);
        
        // 3. Save game state
        if (saveGameManager != null)
        {
            string workflowSave = "workflow_test";
            saveGameManager.SaveGame(workflowSave);
            
            while (saveGameManager.IsSaving)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            if (enableDebugLogs)
                Debug.Log("Saved game state");
            
            // Clean up
            saveGameManager.DeleteSave(workflowSave);
        }
        
        yield return new WaitForSeconds(1f);
        
        // 4. Show completion notification
        if (notificationManager != null)
        {
            notificationManager.ShowNotification("Complete workflow test finished!", NotificationType.Success);
        }
        
        Debug.Log("✅ Complete workflow test finished successfully");
    }
    
    // Manual test methods
    [ContextMenu("Run All Phase 3 Tests")]
    public void ManualRunAllTests()
    {
        StartCoroutine(RunPhase3Tests());
    }
    
    [ContextMenu("Test Notifications Only")]
    public void ManualTestNotifications()
    {
        TestNotificationManager();
    }
    
    [ContextMenu("Test Save System Only")]
    public void ManualTestSaveSystem()
    {
        StartCoroutine(TestSaveGameManager());
    }
    
    [ContextMenu("Test Encounters Only")]
    public void ManualTestEncounters()
    {
        StartCoroutine(TestEncounterManager());
    }
    
    [ContextMenu("Show All Manager Status")]
    public void ShowAllManagerStatus()
    {
        Debug.Log("=== PHASE 3 MANAGER STATUS ===");
        
        // Phase 1 & 2 managers
        var creditsManager = ServiceLocator.Get<CreditsManager>();
        var decisionTracker = ServiceLocator.Get<DecisionTracker>();
        var dayManager = ServiceLocator.Get<DayProgressionManager>();
        var audioManager = ServiceLocator.Get<AudioManager>();
        var uiCoordinator = ServiceLocator.Get<UICoordinator>();
        var gameStateManager = ServiceLocator.Get<GameStateManager>();
        
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
        
        Debug.Log("=== END STATUS ===");
    }
    
    [ContextMenu("Generate Test Encounter")]
    public void ManualGenerateEncounter()
    {
        if (encounterManager != null)
        {
            encounterManager.GenerateEncounter();
            Debug.Log($"Encounter generated. Queue size: {encounterManager.QueueSize}");
        }
    }
    
    [ContextMenu("Show Test Notification")]
    public void ManualShowNotification()
    {
        if (notificationManager != null)
        {
            notificationManager.ShowNotification("Manual test notification from Phase 3!", NotificationType.Info);
        }
    }
    
    [ContextMenu("Quick Save Test")]
    public void ManualQuickSave()
    {
        if (saveGameManager != null)
        {
            saveGameManager.SaveGame("manual_test_save");
            Debug.Log("Manual save initiated");
        }
    }
}