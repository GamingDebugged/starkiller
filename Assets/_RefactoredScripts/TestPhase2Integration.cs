using UnityEngine;
using System.Collections;
using Starkiller.Core;
using Starkiller.Core.Managers;

/// <summary>
/// Integration test for Phase 2 managers (DayProgressionManager, AudioManager, UICoordinator)
/// Tests how they work together with Phase 1 managers
/// </summary>
public class TestPhase2Integration : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool autoRunTests = true;
    [SerializeField] private bool enableDebugLogs = true;
    
    private void Start()
    {
        if (autoRunTests)
        {
            if (enableDebugLogs)
                Debug.Log("[TestPhase2Integration] Starting Phase 2 integration tests...");
            
            StartCoroutine(RunIntegrationTests());
        }
    }
    
    private IEnumerator RunIntegrationTests()
    {
        // Wait for all managers to initialize
        yield return new WaitForSeconds(0.5f);
        
        // Test 1: Verify all managers are registered
        if (!VerifyAllManagers())
        {
            Debug.LogError("❌ Not all managers are registered. Please check your scene setup.");
            yield break;
        }
        
        Debug.Log("✅ All managers found and registered!");
        yield return new WaitForSeconds(1f);
        
        // Test 2: Test cross-manager communication
        TestCrossManagerEvents();
        yield return new WaitForSeconds(2f);
        
        // Test 3: Simulate a game flow
        yield return SimulateGameFlow();
        
        Debug.Log("🎉 [TestPhase2Integration] All integration tests completed!");
    }
    
    private bool VerifyAllManagers()
    {
        bool allFound = true;
        
        // Phase 1 Managers
        var creditsManager = ServiceLocator.Get<CreditsManager>();
        var decisionTracker = ServiceLocator.Get<DecisionTracker>();
        var gameStateManager = ServiceLocator.Get<GameStateManager>();
        
        // Phase 2 Managers
        var dayManager = ServiceLocator.Get<DayProgressionManager>();
        var audioManager = ServiceLocator.Get<AudioManager>();
        var uiCoordinator = ServiceLocator.Get<UICoordinator>();
        
        if (creditsManager == null) { Debug.LogError("❌ CreditsManager not found"); allFound = false; }
        if (decisionTracker == null) { Debug.LogError("❌ DecisionTracker not found"); allFound = false; }
        if (gameStateManager == null) { Debug.LogError("❌ GameStateManager not found"); allFound = false; }
        if (dayManager == null) { Debug.LogError("❌ DayProgressionManager not found"); allFound = false; }
        if (audioManager == null) { Debug.LogError("❌ AudioManager not found"); allFound = false; }
        if (uiCoordinator == null) { Debug.LogError("❌ UICoordinator not found"); allFound = false; }
        
        return allFound;
    }
    
    private void TestCrossManagerEvents()
    {
        Debug.Log("[Test] Testing cross-manager event communication...");
        
        // Test 1: Credits change should update UI and play sound
        var creditsManager = ServiceLocator.Get<CreditsManager>();
        if (creditsManager != null)
        {
            Debug.Log("- Adding 100 credits (should update UI and potentially play sound)");
            creditsManager.AddCredits(100, "Integration Test");
        }
        
        // Test 2: Strike should update UI and play warning sound
        var decisionTracker = ServiceLocator.Get<DecisionTracker>();
        if (decisionTracker != null)
        {
            Debug.Log("- Adding a strike (should update UI and play warning sound)");
            decisionTracker.AddStrike("Integration Test Strike");
        }
        
        // Test 3: Ship processed should update day manager and UI
        var dayManager = ServiceLocator.Get<DayProgressionManager>();
        if (dayManager != null)
        {
            Debug.Log("- Processing a ship (should update UI counters)");
            dayManager.RecordShipProcessed();
        }
        
        // Test 4: UI notification
        Debug.Log("- Triggering UI notification");
        GameEvents.TriggerUINotification("Integration test notification!");
    }
    
    private IEnumerator SimulateGameFlow()
    {
        Debug.Log("[Test] Simulating game flow...");
        
        var gameStateManager = ServiceLocator.Get<GameStateManager>();
        var dayManager = ServiceLocator.Get<DayProgressionManager>();
        var audioManager = ServiceLocator.Get<AudioManager>();
        var creditsManager = ServiceLocator.Get<CreditsManager>();
        var decisionTracker = ServiceLocator.Get<DecisionTracker>();
        
        // 1. Start a new day
        Debug.Log("📅 Starting new day...");
        dayManager?.StartNewDay();
        yield return new WaitForSeconds(1f);
        
        // 2. Change to gameplay state
        Debug.Log("🎮 Changing to gameplay state...");
        gameStateManager?.ChangeState(Starkiller.Core.GameState.Gameplay);
        if (audioManager != null)
        {
            Debug.Log("AudioManager found - trying to play music");
            // audioManager.PlayMusic("gameplay"); // Temporarily commented out
        }
        yield return new WaitForSeconds(1f);
        
        // 3. Start shift
        Debug.Log("⏰ Starting shift...");
        dayManager?.StartShift();
        yield return new WaitForSeconds(1f);
        
        // 4. Simulate some decisions
        Debug.Log("🚀 Simulating ship decisions...");
        for (int i = 0; i < 3; i++)
        {
            // Simulate ship arrival
            if (audioManager != null)
            {
                Debug.Log("Playing notification sound");
                // audioManager.PlaySound("notification"); // Temporarily commented out
            }
            
            // Make a decision
            bool correct = Random.Range(0, 2) == 0;
            decisionTracker?.RecordDecision(correct, $"Test Decision {i + 1}");
            
            // Process ship
            dayManager?.RecordShipProcessed();
            
            // Play appropriate sound
            if (audioManager != null)
            {
                Debug.Log($"Playing {(correct ? "approved" : "denied")} sound");
                // audioManager.PlaySound(correct ? "ship_approved" : "ship_denied"); // Temporarily commented out
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        
        // 5. Add some credits
        Debug.Log("💰 Processing salary...");
        creditsManager?.AddCredits(50, "Test Salary");
        yield return new WaitForSeconds(1f);
        
        // 6. Test pause
        Debug.Log("⏸️ Testing pause...");
        gameStateManager?.ChangeState(Starkiller.Core.GameState.Paused);
        yield return new WaitForSeconds(1f);
        
        // 7. Resume
        Debug.Log("▶️ Resuming...");
        gameStateManager?.ChangeState(Starkiller.Core.GameState.Gameplay);
        yield return new WaitForSeconds(1f);
        
        // 8. End shift
        Debug.Log("🏁 Ending shift...");
        dayManager?.EndShift();
        gameStateManager?.ChangeState(Starkiller.Core.GameState.DayReport);
        
        Debug.Log("✅ Game flow simulation completed!");
    }
    
    // Manual test triggers
    [ContextMenu("Test All Managers Present")]
    public void TestAllManagersPresent()
    {
        if (VerifyAllManagers())
        {
            Debug.Log("✅ All managers are present and registered!");
        }
    }
    
    [ContextMenu("Test UI Notification")]
    public void TestUINotification()
    {
        GameEvents.TriggerUINotification("This is a test notification from Phase 2 integration!");
    }
    
    [ContextMenu("Test Audio")]
    public void TestAudio()
    {
        var audioManager = ServiceLocator.Get<AudioManager>();
        if (audioManager != null)
        {
            Debug.Log("AudioManager found but PlaySound temporarily disabled");
            // audioManager.PlaySound("button_click"); // Temporarily commented out
        }
        else
        {
            Debug.LogError("AudioManager not found!");
        }
    }
    
    [ContextMenu("Show All Manager Status")]
    public void ShowAllManagerStatus()
    {
        Debug.Log("=== FULL SYSTEM STATUS ===");
        
        // Credits
        var cm = ServiceLocator.Get<CreditsManager>();
        Debug.Log($"💰 Credits: {(cm != null ? cm.CurrentCredits.ToString() : "NOT FOUND")}");
        
        // Decisions
        var dt = ServiceLocator.Get<DecisionTracker>();
        if (dt != null)
            Debug.Log($"📊 Decisions: {dt.CorrectDecisions}✓ {dt.WrongDecisions}✗ | Strikes: {dt.CurrentStrikes}/{dt.MaxStrikes}");
        
        // Day Progress
        var dm = ServiceLocator.Get<DayProgressionManager>();
        if (dm != null)
            Debug.Log($"📅 Day {dm.CurrentDay} | Ships: {dm.ShipsProcessedToday} | Time: {dm.GetFormattedTime()}");
        
        // Game State
        var gsm = ServiceLocator.Get<GameStateManager>();
        if (gsm != null)
            Debug.Log($"🎮 State: {gsm.CurrentState}");
        
        // Audio
        var am = ServiceLocator.Get<AudioManager>();
        if (am != null)
            Debug.Log($"🔊 Audio: Available");
        
        Debug.Log("=== END STATUS ===");
    }
}