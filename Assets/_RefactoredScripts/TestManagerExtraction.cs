using UnityEngine;
using Starkiller.Core;
using Starkiller.Core.Managers;
using Starkiller.Integration;

/// <summary>
/// Test script for the extracted managers (CreditsManager, DecisionTracker)
/// Place this on any GameObject to test the Phase 1 extraction
/// </summary>
public class TestManagerExtraction : MonoBehaviour
{
    [Header("Testing")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool autoRunTests = true;
    
    private void Start()
    {
        if (autoRunTests)
        {
            if (enableDebugLogs)
                Debug.Log("[TestManagerExtraction] Starting Phase 1 manager tests...");
            
            StartCoroutine(RunManagerTests());
        }
    }
    
    private System.Collections.IEnumerator RunManagerTests()
    {
        // Wait a frame for managers to initialize
        yield return null;
        
        // Test 1: ServiceLocator can find managers
        TestServiceLocatorIntegration();
        
        yield return new WaitForSeconds(1f);
        
        // Test 2: CreditsManager functionality
        TestCreditsManager();
        
        yield return new WaitForSeconds(1f);
        
        // Test 3: DecisionTracker functionality
        TestDecisionTracker();
        
        yield return new WaitForSeconds(1f);
        
        // Test 4: Event system integration
        TestEventIntegration();
        
        yield return new WaitForSeconds(1f);
        
        // Test 5: Bridge functionality (if available)
        TestGameManagerBridge();
        
        Debug.Log("🎉 [TestManagerExtraction] All Phase 1 tests completed!");
    }
    
    private void TestServiceLocatorIntegration()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing ServiceLocator integration...");
        
        // Test CreditsManager
        CreditsManager creditsManager = ServiceLocator.Get<CreditsManager>();
        if (creditsManager != null)
        {
            Debug.Log("✅ CreditsManager found via ServiceLocator");
        }
        else
        {
            Debug.LogError("❌ CreditsManager not found - you need to add it to the scene");
        }
        
        // Test DecisionTracker
        DecisionTracker decisionTracker = ServiceLocator.Get<DecisionTracker>();
        if (decisionTracker != null)
        {
            Debug.Log("✅ DecisionTracker found via ServiceLocator");
        }
        else
        {
            Debug.LogError("❌ DecisionTracker not found - you need to add it to the scene");
        }
    }
    
    private void TestCreditsManager()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing CreditsManager...");
        
        CreditsManager creditsManager = ServiceLocator.Get<CreditsManager>();
        if (creditsManager == null)
        {
            Debug.LogError("❌ CreditsManager not available for testing");
            return;
        }
        
        int initialCredits = creditsManager.CurrentCredits;
        
        // Test adding credits
        bool success = creditsManager.AddCredits(50, "Test Addition");
        if (success && creditsManager.CurrentCredits == initialCredits + 50)
        {
            Debug.Log("✅ CreditsManager add functionality working");
        }
        else
        {
            Debug.LogError("❌ CreditsManager add functionality failed");
        }
        
        // Test deducting credits
        success = creditsManager.DeductCredits(25, "Test Deduction");
        if (success && creditsManager.CurrentCredits == initialCredits + 25)
        {
            Debug.Log("✅ CreditsManager deduct functionality working");
        }
        else
        {
            Debug.LogError("❌ CreditsManager deduct functionality failed");
        }
        
        // Test salary calculation
        int salary = creditsManager.CalculateDailySalary(10, 8, 2);
        if (salary > 0)
        {
            Debug.Log($"✅ CreditsManager salary calculation working: {salary} credits");
        }
        else
        {
            Debug.LogWarning("⚠️ CreditsManager salary calculation returned 0");
        }
    }
    
    private void TestDecisionTracker()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing DecisionTracker...");
        
        DecisionTracker decisionTracker = ServiceLocator.Get<DecisionTracker>();
        if (decisionTracker == null)
        {
            Debug.LogError("❌ DecisionTracker not available for testing");
            return;
        }
        
        int initialCorrect = decisionTracker.CorrectDecisions;
        int initialWrong = decisionTracker.WrongDecisions;
        int initialStrikes = decisionTracker.CurrentStrikes;
        
        // Test correct decision
        decisionTracker.RecordDecision(true, "Test Correct Decision");
        if (decisionTracker.CorrectDecisions == initialCorrect + 1)
        {
            Debug.Log("✅ DecisionTracker correct decision tracking working");
        }
        else
        {
            Debug.LogError("❌ DecisionTracker correct decision tracking failed");
        }
        
        // Test wrong decision (adds strike)
        decisionTracker.RecordDecision(false, "Test Wrong Decision");
        if (decisionTracker.WrongDecisions == initialWrong + 1 && 
            decisionTracker.CurrentStrikes == initialStrikes + 1)
        {
            Debug.Log("✅ DecisionTracker wrong decision and strike tracking working");
        }
        else
        {
            Debug.LogError("❌ DecisionTracker wrong decision/strike tracking failed");
        }
        
        // Test accuracy calculation
        float accuracy = decisionTracker.AccuracyPercentage;
        if (accuracy >= 0 && accuracy <= 100)
        {
            Debug.Log($"✅ DecisionTracker accuracy calculation working: {accuracy:F1}%");
        }
        else
        {
            Debug.LogError("❌ DecisionTracker accuracy calculation failed");
        }
    }
    
    private void TestEventIntegration()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing event system integration...");
        
        // Subscribe to events temporarily
        bool creditsEventReceived = false;
        bool strikesEventReceived = false;
        
        CreditsManager.OnCreditsChanged += (credits, change) => creditsEventReceived = true;
        DecisionTracker.OnStrikesChanged += (strikes, max) => strikesEventReceived = true;
        
        // Trigger actions that should fire events
        CreditsManager creditsManager = ServiceLocator.Get<CreditsManager>();
        DecisionTracker decisionTracker = ServiceLocator.Get<DecisionTracker>();
        
        if (creditsManager != null)
        {
            creditsManager.AddCredits(1, "Event Test");
        }
        
        if (decisionTracker != null)
        {
            decisionTracker.AddStrike("Event Test");
        }
        
        // Check if events were received
        if (creditsEventReceived)
        {
            Debug.Log("✅ Credits event system working");
        }
        else
        {
            Debug.LogWarning("⚠️ Credits event system may not be working");
        }
        
        if (strikesEventReceived)
        {
            Debug.Log("✅ Strikes event system working");
        }
        else
        {
            Debug.LogWarning("⚠️ Strikes event system may not be working");
        }
        
        // Unsubscribe
        CreditsManager.OnCreditsChanged -= (credits, change) => creditsEventReceived = true;
        DecisionTracker.OnStrikesChanged -= (strikes, max) => strikesEventReceived = true;
    }
    
    private void TestGameManagerBridge()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing GameManagerBridge...");
        
        GameManagerBridge bridge = FindFirstObjectByType<GameManagerBridge>();
        if (bridge == null)
        {
            Debug.LogWarning("⚠️ GameManagerBridge not found - this is optional for Phase 1");
            return;
        }
        
        // Test bridge functionality
        int initialCredits = bridge.GetCurrentCredits();
        bool success = bridge.AddCredits(10, "Bridge Test");
        
        if (success)
        {
            Debug.Log("✅ GameManagerBridge delegation working");
        }
        else
        {
            Debug.LogWarning("⚠️ GameManagerBridge delegation may not be working");
        }
        
        // Test performance summary
        string summary = bridge.GetPerformanceSummary();
        if (!string.IsNullOrEmpty(summary))
        {
            Debug.Log("✅ GameManagerBridge performance summary working");
            Debug.Log($"Performance Summary:\n{summary}");
        }
        else
        {
            Debug.LogWarning("⚠️ GameManagerBridge performance summary empty");
        }
    }
    
    // Manual test methods callable from Inspector
    [ContextMenu("Run All Tests")]
    public void ManualRunAllTests()
    {
        StartCoroutine(RunManagerTests());
    }
    
    [ContextMenu("Test Credits Manager")]
    public void ManualTestCredits()
    {
        TestCreditsManager();
    }
    
    [ContextMenu("Test Decision Tracker")]
    public void ManualTestDecisions()
    {
        TestDecisionTracker();
    }
    
    [ContextMenu("Test Service Locator")]
    public void ManualTestServiceLocator()
    {
        TestServiceLocatorIntegration();
    }
    
    [ContextMenu("Show Manager Status")]
    public void ShowManagerStatus()
    {
        Debug.Log("=== MANAGER STATUS ===");
        
        CreditsManager cm = ServiceLocator.Get<CreditsManager>();
        Debug.Log($"CreditsManager: {(cm != null ? $"Found - {cm.CurrentCredits} credits" : "Not Found")}");
        
        DecisionTracker dt = ServiceLocator.Get<DecisionTracker>();
        Debug.Log($"DecisionTracker: {(dt != null ? $"Found - {dt.CurrentStrikes}/{dt.MaxStrikes} strikes" : "Not Found")}");
        
        GameStateManager gsm = ServiceLocator.Get<GameStateManager>();
        Debug.Log($"GameStateManager: {(gsm != null ? $"Found - {gsm.CurrentState}" : "Not Found")}");
        
        Debug.Log("=== END STATUS ===");
    }
}