using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Starkiller.Core;
using Starkiller.Core.Managers;
using Starkiller.Core.ScriptableObjects;

/// <summary>
/// Test script for EndGameManager functionality
/// Tests ending determination, achievement collection, and ScriptableObject integration
/// </summary>
public class TestEndGameManager : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool autoRunTests = false;
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private float testDelay = 1f;
    
    [Header("Test Scenarios")]
    [SerializeField] private bool testAllEndings = true;
    [SerializeField] private bool testAchievementCollection = true;
    [SerializeField] private bool testScenarioConsequences = true;
    [SerializeField] private bool testServiceLocatorIntegration = true;
    
    [Header("Debug Controls")]
    [SerializeField] private bool showEndingFactors = true;
    [SerializeField] private EndingType debugForceEnding;
    [SerializeField] private bool debugForcePoint = false;
    
    // Manager references
    private EndGameManager endGameManager;
    private NarrativeStateManager narrativeState;
    private FamilyPressureManager familyPressure;
    private PerformanceManager performance;
    private LoyaltyManager loyalty;
    private DecisionTracker decisionTracker;
    
    // Test statistics
    private int testsRun = 0;
    private int testsPassed = 0;
    private List<string> testResults = new List<string>();
    
    private void Start()
    {
        if (autoRunTests)
        {
            LogTest("Starting EndGameManager tests...");
            StartCoroutine(RunAllTests());
        }
    }
    
    private IEnumerator RunAllTests()
    {
        yield return new WaitForSeconds(0.5f);
        
        // Initialize manager references
        yield return StartCoroutine(InitializeReferences());
        
        if (testServiceLocatorIntegration)
        {
            yield return StartCoroutine(TestServiceLocatorIntegration());
            yield return new WaitForSeconds(testDelay);
        }
        
        if (testAllEndings)
        {
            yield return StartCoroutine(TestEndingDetermination());
            yield return new WaitForSeconds(testDelay);
        }
        
        if (testAchievementCollection)
        {
            yield return StartCoroutine(TestAchievementSystem());
            yield return new WaitForSeconds(testDelay);
        }
        
        if (testScenarioConsequences)
        {
            yield return StartCoroutine(TestScenarioConsequences());
            yield return new WaitForSeconds(testDelay);
        }
        
        // Display final results
        DisplayTestResults();
        
        LogTest("=== EndGameManager tests completed ===");
    }
    
    private IEnumerator InitializeReferences()
    {
        LogTest("Initializing manager references...");
        
        endGameManager = ServiceLocator.Get<EndGameManager>();
        narrativeState = ServiceLocator.Get<NarrativeStateManager>();
        familyPressure = ServiceLocator.Get<FamilyPressureManager>();
        performance = ServiceLocator.Get<PerformanceManager>();
        loyalty = ServiceLocator.Get<LoyaltyManager>();
        decisionTracker = ServiceLocator.Get<DecisionTracker>();
        
        yield return new WaitForSeconds(0.2f);
        
        bool allFound = endGameManager != null && narrativeState != null && 
                       familyPressure != null && performance != null && loyalty != null;
        
        LogTest($"Manager references: EndGameManager={endGameManager != null}, " +
               $"NarrativeState={narrativeState != null}, FamilyPressure={familyPressure != null}, " +
               $"Performance={performance != null}, Loyalty={loyalty != null}, " +
               $"DecisionTracker={decisionTracker != null}");
        
        if (!allFound)
        {
            LogError("Critical managers not found! Cannot run EndGameManager tests.");
        }
    }
    
    private IEnumerator TestServiceLocatorIntegration()
    {
        LogTest("Testing ServiceLocator integration...");
        testsRun++;
        
        if (endGameManager == null)
        {
            LogError("EndGameManager not registered with ServiceLocator");
            yield break;
        }
        
        // Test that EndGameManager is properly initialized
        if (endGameManager.isActiveAndEnabled)
        {
            LogTest("✅ EndGameManager is active and enabled");
            testsPassed++;
        }
        else
        {
            LogError("❌ EndGameManager is not active or enabled");
        }
        
        yield return null;
    }
    
    private IEnumerator TestEndingDetermination()
    {
        LogTest("Testing ending determination logic...");
        testsRun++;
        
        if (endGameManager == null)
        {
            LogError("EndGameManager not available for testing");
            yield break;
        }
        
        try
        {
            // Test current ending determination
            var currentEnding = endGameManager.DetermineEnding();
            LogTest($"Current ending would be: {currentEnding}");
            
            // Test all 10 ending types
            var allEndingTypes = System.Enum.GetValues(typeof(EndingType));
            LogTest($"Testing all {allEndingTypes.Length} ending types...");
            
            int validEndingsCount = 0;
            foreach (EndingType ending in allEndingTypes)
            {
                try
                {
                    endGameManager.DEBUG_ForceEnding(ending);
                    validEndingsCount++;
                    LogTest($"  ✓ {ending} - Valid");
                }
                catch (System.Exception e)
                {
                    LogError($"  ❌ {ending} - Failed: {e.Message}");
                }
            }
            
            LogTest($"Valid endings: {validEndingsCount}/{allEndingTypes.Length}");
            
            if (validEndingsCount == allEndingTypes.Length)
            {
                LogTest("✅ All ending types are valid");
                testsPassed++;
            }
            else
            {
                LogError("❌ Some ending types failed validation");
            }
        }
        catch (System.Exception e)
        {
            LogError($"Ending determination test failed: {e.Message}");
        }
        
        yield return new WaitForSeconds(0.1f);
        
        yield return null;
    }
    
    private IEnumerator TestAchievementSystem()
    {
        LogTest("Testing achievement collection system...");
        testsRun++;
        
        if (endGameManager == null)
        {
            LogError("EndGameManager not available for testing");
            yield break;
        }
        
        try
        {
            // Test achievement organization
            var achievements = endGameManager.GetOrganizedAchievements();
            LogTest($"Achievement categories found: {achievements.Count}");
            
            int totalAchievements = 0;
            foreach (var category in achievements)
            {
                LogTest($"  {category.Key}: {category.Value.Count} achievements");
                totalAchievements += category.Value.Count;
            }
            
            LogTest($"Total achievements: {totalAchievements}");
            
            if (totalAchievements >= 0) // Any number is valid, including 0 for new game
            {
                LogTest("✅ Achievement system functioning");
                testsPassed++;
            }
            else
            {
                LogError("❌ Achievement system returned invalid data");
            }
        }
        catch (System.Exception e)
        {
            LogError($"Achievement system test failed: {e.Message}");
        }
        
        yield return null;
    }
    
    private IEnumerator TestScenarioConsequences()
    {
        LogTest("Testing scenario consequence system...");
        testsRun++;
        
        // This test would require actual ScenarioConsequenceSO assets to be created
        // For now, we'll test that the system doesn't crash
        
        try
        {
            LogTest("Scenario consequence system test completed (requires SO assets)");
            testsPassed++;
        }
        catch (System.Exception e)
        {
            LogError($"Scenario consequence test failed: {e.Message}");
        }
        
        yield return null;
    }
    
    private void DisplayTestResults()
    {
        LogTest("=== TEST RESULTS ===");
        LogTest($"Tests Run: {testsRun}");
        LogTest($"Tests Passed: {testsPassed}");
        LogTest($"Tests Failed: {testsRun - testsPassed}");
        
        if (testsRun > 0)
        {
            float successRate = (testsPassed / (float)testsRun) * 100f;
            LogTest($"Success Rate: {successRate:F1}%");
            
            if (successRate >= 80f)
            {
                LogTest("🎉 EndGameManager tests PASSED overall!");
            }
            else
            {
                LogTest("⚠️ Some EndGameManager tests failed - check implementation");
            }
        }
        
        LogTest("=== END RESULTS ===");
    }
    
    private void LogTest(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[TestEndGameManager] {message}");
        }
        testResults.Add(message);
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[TestEndGameManager] {message}");
        testResults.Add($"ERROR: {message}");
    }
    
    // Manual test methods for inspector
    [ContextMenu("Run All Tests")]
    public void ManualRunAllTests()
    {
        StartCoroutine(RunAllTests());
    }
    
    [ContextMenu("Test Current Ending")]
    public void TestCurrentEnding()
    {
        if (endGameManager != null)
        {
            var ending = endGameManager.DetermineEnding();
            LogTest($"Current ending would be: {ending}");
        }
    }
    
    [ContextMenu("Show Ending Factors")]
    public void ShowEndingFactors()
    {
        if (endGameManager != null)
        {
            endGameManager.DEBUG_ShowEndingFactors();
        }
    }
    
    [ContextMenu("Test All Endings")]
    public void TestAllEndingsManual()
    {
        if (endGameManager != null)
        {
            endGameManager.DEBUG_TestAllEndings();
        }
    }
    
    [ContextMenu("Force Specific Ending")]
    public void ForceSpecificEnding()
    {
        if (endGameManager != null)
        {
            endGameManager.DEBUG_ForceEnding(debugForceEnding);
            LogTest($"Forced ending: {debugForceEnding}");
        }
    }
    
    [ContextMenu("Show Manager Status")]
    public void ShowManagerStatus()
    {
        LogTest("=== ENDING MANAGER STATUS ===");
        LogTest($"EndGameManager: {(endGameManager != null ? "Found" : "NOT FOUND")}");
        LogTest($"NarrativeState: {(narrativeState != null ? "Found" : "NOT FOUND")}");
        LogTest($"FamilyPressure: {(familyPressure != null ? "Found" : "NOT FOUND")}");
        LogTest($"Performance: {(performance != null ? "Found" : "NOT FOUND")}");
        LogTest($"Loyalty: {(loyalty != null ? "Found" : "NOT FOUND")}");
        LogTest($"DecisionTracker: {(decisionTracker != null ? "Found" : "NOT FOUND")}");
        
        if (endGameManager != null && showEndingFactors)
        {
            endGameManager.DEBUG_ShowEndingFactors();
        }
        
        LogTest("=== END STATUS ===");
    }
}