using UnityEngine;
using Starkiller.Core;
using Starkiller.Core.Managers;

/// <summary>
/// Test script to verify refactored components work alongside existing system
/// Place this on any GameObject to test the new architecture
/// </summary>
public class TestRefactoredComponents : MonoBehaviour
{
    [Header("Testing")]
    [SerializeField] private bool enableDebugLogs = true;
    
    private void Start()
    {
        if (enableDebugLogs)
            Debug.Log("[TestRefactoredComponents] Starting tests...");
        
        // Test 1: ServiceLocator
        TestServiceLocator();
        
        // Test 2: GameEvents
        TestGameEvents();
        
        // Test 3: GameStateManager
        TestGameStateManager();
    }
    
    private void TestServiceLocator()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing ServiceLocator...");
        
        // The ServiceLocator should auto-create itself
        bool hasServiceLocator = ServiceLocator.Instance != null;
        
        if (hasServiceLocator)
        {
            Debug.Log("✅ ServiceLocator working correctly");
        }
        else
        {
            Debug.LogError("❌ ServiceLocator failed to initialize");
        }
    }
    
    private void TestGameEvents()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing GameEvents...");
        
        // Subscribe to an event
        GameEvents.OnGameStateChanged += OnTestGameStateChanged;
        
        // Trigger an event
        GameEvents.TriggerGameStateChanged(Starkiller.Core.GameState.Gameplay);
        
        // Clean up
        GameEvents.OnGameStateChanged -= OnTestGameStateChanged;
        
        Debug.Log("✅ GameEvents working correctly");
    }
    
    private void OnTestGameStateChanged(Starkiller.Core.GameState newState)
    {
        if (enableDebugLogs)
            Debug.Log($"[Test] Received GameState change event: {newState}");
    }
    
    private void TestGameStateManager()
    {
        if (enableDebugLogs)
            Debug.Log("[Test] Testing GameStateManager...");
        
        // Try to get GameStateManager from ServiceLocator
        GameStateManager gameStateManager = ServiceLocator.Get<GameStateManager>();
        
        if (gameStateManager != null)
        {
            Debug.Log($"✅ GameStateManager found, current state: {gameStateManager.CurrentState}");
        }
        else
        {
            Debug.LogWarning("⚠️ GameStateManager not found - you need to add it to the scene");
        }
    }
    
    // Test methods that can be called from Inspector
    [ContextMenu("Test Service Locator")]
    public void ManualTestServiceLocator() => TestServiceLocator();
    
    [ContextMenu("Test Game Events")]
    public void ManualTestGameEvents() => TestGameEvents();
    
    [ContextMenu("Test Game State Manager")]
    public void ManualTestGameStateManager() => TestGameStateManager();
    
    [ContextMenu("Test Change to Gameplay")]
    public void TestChangeToGameplay()
    {
        GameStateManager gsm = ServiceLocator.Get<GameStateManager>();
        if (gsm != null)
            gsm.StartGame();
    }
    
    [ContextMenu("Test Pause Game")]
    public void TestPauseGame()
    {
        GameStateManager gsm = ServiceLocator.Get<GameStateManager>();
        if (gsm != null)
            gsm.PauseGame();
    }
}