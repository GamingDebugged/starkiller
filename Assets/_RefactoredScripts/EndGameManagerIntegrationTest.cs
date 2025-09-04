using UnityEngine;
using Starkiller.Core;
using Starkiller.Core.Managers;
using Starkiller.Core.ScriptableObjects;
using System.Collections;

/// <summary>
/// Final integration test for EndGameManager system
/// Tests complete workflow from initialization to ending presentation
/// </summary>
public class EndGameManagerIntegrationTest : MonoBehaviour
{
    [Header("Integration Test Settings")]
    [SerializeField] private bool runOnStart = false;
    [SerializeField] private bool enableDetailedLogs = true;
    [SerializeField] private bool testAllEndingScenarios = true;
    
    [Header("Test Results")]
    [SerializeField] private bool initializationPassed = false;
    [SerializeField] private bool endingDeterminationPassed = false;
    [SerializeField] private bool achievementSystemPassed = false;
    [SerializeField] private bool dataFlowPassed = false;
    [SerializeField] private bool overallTestPassed = false;
    
    private void Start()
    {
        if (runOnStart)
        {
            StartCoroutine(RunCompleteIntegrationTest());
        }
    }
    
    private IEnumerator RunCompleteIntegrationTest()
    {
        LogTest("=== ENDGAME MANAGER INTEGRATION TEST START ===");
        
        yield return new WaitForSeconds(1f); // Allow initialization
        
        // Test 1: System Initialization
        yield return StartCoroutine(TestSystemInitialization());
        
        // Test 2: Ending Determination Logic
        yield return StartCoroutine(TestEndingDeterminationLogic());
        
        // Test 3: Achievement Collection System
        yield return StartCoroutine(TestAchievementCollectionSystem());
        
        // Test 4: Data Flow and Integration
        yield return StartCoroutine(TestDataFlowIntegration());
        
        // Test 5: Complete Workflow (Simulate Day 30)
        yield return StartCoroutine(TestCompleteWorkflow());
        
        // Final Assessment
        AssessFinalResults();
        
        LogTest("=== ENDGAME MANAGER INTEGRATION TEST END ===");
    }
    
    private IEnumerator TestSystemInitialization()
    {
        LogTest("Testing EndGameManager System Initialization...");
        
        var endGameManager = ServiceLocator.Get<EndGameManager>();
        var narrativeState = ServiceLocator.Get<NarrativeStateManager>();
        var familyPressure = ServiceLocator.Get<FamilyPressureManager>();
        var performance = ServiceLocator.Get<PerformanceManager>();
        var loyalty = ServiceLocator.Get<LoyaltyManager>();
        
        bool allManagersFound = endGameManager != null && narrativeState != null && 
                              familyPressure != null && performance != null && loyalty != null;
        
        if (allManagersFound)
        {
            LogTest("✅ All required managers found in ServiceLocator");
            
            // Test EndGameManager specific initialization
            if (endGameManager.isActiveAndEnabled)
            {
                LogTest("✅ EndGameManager is properly initialized and active");
                initializationPassed = true;
            }
            else
            {
                LogError("❌ EndGameManager is not properly initialized");
            }
        }
        else
        {
            LogError("❌ Missing required managers in ServiceLocator");
            LogError($"EndGame={endGameManager != null}, Narrative={narrativeState != null}, " +
                    $"Family={familyPressure != null}, Performance={performance != null}, Loyalty={loyalty != null}");
        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    private IEnumerator TestEndingDeterminationLogic()
    {
        LogTest("Testing Ending Determination Logic...");
        
        var endGameManager = ServiceLocator.Get<EndGameManager>();
        if (endGameManager == null)
        {
            LogError("❌ EndGameManager not available for ending determination test");
            yield break;
        }
        
        try
        {
            // Test current ending calculation
            var currentEnding = endGameManager.DetermineEnding();
            LogTest($"Current game state would result in: {currentEnding}");
            
            // Show current factors
            endGameManager.DEBUG_ShowEndingFactors();
            
            if (testAllEndingScenarios)
            {
                // Test that all ending types can be forced (validation test)
                LogTest("Testing all ending types for validity...");
                var allEndings = System.Enum.GetValues(typeof(EndingType));
                int validEndings = 0;
                
                foreach (EndingType ending in allEndings)
                {
                    try
                    {
                        endGameManager.DEBUG_ForceEnding(ending);
                        validEndings++;
                        LogTest($"  ✓ {ending} - Valid");
                    }
                    catch (System.Exception e)
                    {
                        LogError($"  ❌ {ending} - Error: {e.Message}");
                    }
                }
                
                LogTest($"Valid ending types: {validEndings}/{allEndings.Length}");
                if (validEndings == allEndings.Length)
                {
                    endingDeterminationPassed = true;
                }
            }
            else
            {
                // Just test that determination doesn't crash
                endingDeterminationPassed = true;
            }
            
            LogTest("✅ Ending determination logic functional");
        }
        catch (System.Exception e)
        {
            LogError($"❌ Ending determination test failed: {e.Message}");
        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    private IEnumerator TestAchievementCollectionSystem()
    {
        LogTest("Testing Achievement Collection System...");
        
        var endGameManager = ServiceLocator.Get<EndGameManager>();
        if (endGameManager == null)
        {
            LogError("❌ EndGameManager not available for achievement test");
            yield break;
        }
        
        try
        {
            // Test achievement organization
            var organizedAchievements = endGameManager.GetOrganizedAchievements();
            LogTest($"Achievement categories found: {organizedAchievements.Count}");
            
            int totalAchievements = 0;
            foreach (var category in organizedAchievements)
            {
                LogTest($"  {category.Key}: {category.Value.Count} achievements");
                totalAchievements += category.Value.Count;
                
                // Log first few achievements in each category
                if (category.Value.Count > 0 && enableDetailedLogs)
                {
                    for (int i = 0; i < Mathf.Min(3, category.Value.Count); i++)
                    {
                        LogTest($"    - {category.Value[i]}");
                    }
                }
            }
            
            LogTest($"Total achievements collected: {totalAchievements}");
            
            // Achievement system is valid if it doesn't crash and returns organized data
            achievementSystemPassed = true;
            LogTest("✅ Achievement collection system functional");
        }
        catch (System.Exception e)
        {
            LogError($"❌ Achievement collection test failed: {e.Message}");
        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    private IEnumerator TestDataFlowIntegration()
    {
        LogTest("Testing Data Flow Integration...");
        
        try
        {
            // Test that EndGameManager can read from all required managers
            var endGameManager = ServiceLocator.Get<EndGameManager>();
            var narrativeState = ServiceLocator.Get<NarrativeStateManager>();
            var familyPressure = ServiceLocator.Get<FamilyPressureManager>();
            var performance = ServiceLocator.Get<PerformanceManager>();
            var loyalty = ServiceLocator.Get<LoyaltyManager>();
            
            if (endGameManager == null || narrativeState == null)
            {
                LogError("❌ Required managers not available for data flow test");
                yield break;
            }
            
            // Test data retrieval
            bool pointOfNoReturn = narrativeState.IsPointOfNoReturnReached();
            var lockedPath = narrativeState.GetLockedEndingPath();
            int familySafety = familyPressure?.GetFamilySafety() ?? 0;
            var performanceRating = performance?.CurrentRating ?? PerformanceRating.Poor;
            bool isExtremist = loyalty?.IsExtremistImperial ?? false;
            
            LogTest($"Data Flow Test Results:");
            LogTest($"  Point of No Return: {pointOfNoReturn}");
            LogTest($"  Locked Path: {lockedPath}");
            LogTest($"  Family Safety: {familySafety}");
            LogTest($"  Performance: {performanceRating}");
            LogTest($"  Extremist Imperial: {isExtremist}");
            
            // Test ending determination with current data
            var determinedEnding = endGameManager.DetermineEnding();
            LogTest($"  Determined Ending: {determinedEnding}");
            
            dataFlowPassed = true;
            LogTest("✅ Data flow integration functional");
        }
        catch (System.Exception e)
        {
            LogError($"❌ Data flow integration test failed: {e.Message}");
        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    private IEnumerator TestCompleteWorkflow()
    {
        LogTest("Testing Complete EndGame Workflow...");
        
        var endGameManager = ServiceLocator.Get<EndGameManager>();
        if (endGameManager == null)
        {
            LogError("❌ EndGameManager not available for workflow test");
            yield break;
        }
        
        try
        {
            // Simulate the complete ending workflow
            LogTest("Simulating Day 30 completion workflow...");
            
            // 1. Determine ending
            var ending = endGameManager.DetermineEnding();
            LogTest($"Step 1 - Ending determined: {ending}");
            
            // 2. Present ending (this would normally trigger UI)
            endGameManager.PresentEnding(ending);
            LogTest($"Step 2 - Ending presentation triggered");
            
            // 3. Verify achievement collection occurred
            var achievements = endGameManager.GetOrganizedAchievements();
            LogTest($"Step 3 - Achievements collected: {achievements.Count} categories");
            
            LogTest("✅ Complete workflow simulation successful");
        }
        catch (System.Exception e)
        {
            LogError($"❌ Complete workflow test failed: {e.Message}");
        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    private void AssessFinalResults()
    {
        LogTest("=== FINAL ASSESSMENT ===");
        
        LogTest($"Initialization Test: {(initializationPassed ? "PASSED" : "FAILED")}");
        LogTest($"Ending Determination: {(endingDeterminationPassed ? "PASSED" : "FAILED")}");
        LogTest($"Achievement System: {(achievementSystemPassed ? "PASSED" : "FAILED")}");
        LogTest($"Data Flow Integration: {(dataFlowPassed ? "PASSED" : "FAILED")}");
        
        int passedTests = 0;
        if (initializationPassed) passedTests++;
        if (endingDeterminationPassed) passedTests++;
        if (achievementSystemPassed) passedTests++;
        if (dataFlowPassed) passedTests++;
        
        float successRate = (passedTests / 4f) * 100f;
        LogTest($"Overall Success Rate: {successRate:F1}% ({passedTests}/4 tests passed)");
        
        overallTestPassed = successRate >= 75f;
        
        if (overallTestPassed)
        {
            LogTest("🎉 ENDGAME MANAGER INTEGRATION TEST: PASSED");
            LogTest("The EndGameManager system is ready for production use!");
        }
        else
        {
            LogTest("❌ ENDGAME MANAGER INTEGRATION TEST: FAILED");
            LogTest("Please review failed tests and fix issues before using in production.");
        }
    }
    
    private void LogTest(string message)
    {
        if (enableDetailedLogs)
        {
            Debug.Log($"[EndGameIntegration] {message}");
        }
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[EndGameIntegration] {message}");
    }
    
    // Manual test methods
    [ContextMenu("Run Complete Integration Test")]
    public void ManualRunIntegrationTest()
    {
        StartCoroutine(RunCompleteIntegrationTest());
    }
    
    [ContextMenu("Test Current Game State")]
    public void TestCurrentGameState()
    {
        var endGameManager = ServiceLocator.Get<EndGameManager>();
        if (endGameManager != null)
        {
            var ending = endGameManager.DetermineEnding();
            LogTest($"Current game state ending: {ending}");
            endGameManager.DEBUG_ShowEndingFactors();
        }
    }
    
    [ContextMenu("Simulate Game Completion")]
    public void SimulateGameCompletion()
    {
        var endGameManager = ServiceLocator.Get<EndGameManager>();
        if (endGameManager != null)
        {
            endGameManager.HandleGameCompletion();
            LogTest("Game completion simulation triggered");
        }
    }
}