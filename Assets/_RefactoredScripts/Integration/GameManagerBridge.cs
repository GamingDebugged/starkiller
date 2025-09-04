using UnityEngine;
using Starkiller.Core;
using Starkiller.Core.Managers;

namespace Starkiller.Integration
{
    /// <summary>
    /// Bridge component that allows existing GameManager to delegate operations 
    /// to the new refactored managers while maintaining backward compatibility
    /// 
    /// Add this component to your existing GameManager GameObject
    /// </summary>
    public class GameManagerBridge : MonoBehaviour
    {
        [Header("Bridge Settings")]
        [SerializeField] private bool enableDelegation = true;
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool fallbackToOriginal = true; // If new managers not found, use original logic
        
        [Header("Manager References")]
        [SerializeField] private CreditsManager creditsManager;
        [SerializeField] private DecisionTracker decisionTracker;
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private DayProgressionManager dayProgressionManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private UICoordinator uiCoordinator;
        
        // Cached references
        private GameManager originalGameManager;
        
        private void Awake()
        {
            // Get reference to the original GameManager on this GameObject
            originalGameManager = GetComponent<GameManager>();
            
            if (originalGameManager == null)
            {
                Debug.LogError("[GameManagerBridge] No GameManager found on this GameObject!");
            }
            
            if (enableDebugLogs)
                Debug.Log("[GameManagerBridge] Bridge initialized");
        }
        
        private void Start()
        {
            // Get manager references from ServiceLocator if not manually assigned
            if (enableDelegation)
            {
                if (creditsManager == null)
                    creditsManager = ServiceLocator.Get<CreditsManager>();
                
                if (decisionTracker == null)
                    decisionTracker = ServiceLocator.Get<DecisionTracker>();
                
                if (gameStateManager == null)
                    gameStateManager = ServiceLocator.Get<GameStateManager>();
                
                if (dayProgressionManager == null)
                    dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
                
                if (audioManager == null)
                    audioManager = ServiceLocator.Get<AudioManager>();
                
                if (uiCoordinator == null)
                    uiCoordinator = ServiceLocator.Get<UICoordinator>();
                
                LogBridgeStatus();
            }
        }
        
        /// <summary>
        /// Add credits through the new CreditsManager or fallback to original
        /// Call this instead of directly modifying credits in GameManager
        /// </summary>
        public bool AddCredits(int amount, string reason = "")
        {
            if (enableDelegation && creditsManager != null)
            {
                return creditsManager.AddCredits(amount, reason);
            }
            else if (fallbackToOriginal && originalGameManager != null)
            {
                // Fallback to original GameManager logic
                // You would need to expose the original AddCredits method or access the credits field
                if (enableDebugLogs)
                    Debug.LogWarning($"[GameManagerBridge] Falling back to original credit system for +{amount}");
                
                // This is a placeholder - you'd need to implement the actual fallback
                return true;
            }
            
            Debug.LogError("[GameManagerBridge] No credit management system available!");
            return false;
        }
        
        /// <summary>
        /// Deduct credits through the new CreditsManager or fallback to original
        /// </summary>
        public bool DeductCredits(int amount, string reason = "")
        {
            if (enableDelegation && creditsManager != null)
            {
                return creditsManager.DeductCredits(amount, reason);
            }
            else if (fallbackToOriginal && originalGameManager != null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[GameManagerBridge] Falling back to original credit system for -{amount}");
                
                // Fallback logic would go here
                return true;
            }
            
            Debug.LogError("[GameManagerBridge] No credit management system available!");
            return false;
        }
        
        /// <summary>
        /// Record a decision through the new DecisionTracker or fallback to original
        /// </summary>
        public void RecordDecision(bool wasCorrect, string reason = "")
        {
            if (enableDelegation && decisionTracker != null)
            {
                decisionTracker.RecordDecision(wasCorrect, reason);
            }
            else if (fallbackToOriginal && originalGameManager != null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[GameManagerBridge] Falling back to original decision tracking for {(wasCorrect ? "correct" : "wrong")} decision");
                
                // Fallback logic would go here
            }
            else
            {
                Debug.LogError("[GameManagerBridge] No decision tracking system available!");
            }
        }
        
        /// <summary>
        /// Get current credits from the appropriate system
        /// </summary>
        public int GetCurrentCredits()
        {
            if (enableDelegation && creditsManager != null)
            {
                return creditsManager.CurrentCredits;
            }
            else if (fallbackToOriginal && originalGameManager != null)
            {
                // Return credits from original system
                // You'd need to access the credits field from GameManager
                if (enableDebugLogs)
                    Debug.LogWarning("[GameManagerBridge] Getting credits from original system");
                
                return 0; // Placeholder
            }
            
            return 0;
        }
        
        /// <summary>
        /// Get current strikes from the appropriate system
        /// </summary>
        public int GetCurrentStrikes()
        {
            if (enableDelegation && decisionTracker != null)
            {
                return decisionTracker.CurrentStrikes;
            }
            else if (fallbackToOriginal && originalGameManager != null)
            {
                // Return strikes from original system
                if (enableDebugLogs)
                    Debug.LogWarning("[GameManagerBridge] Getting strikes from original system");
                
                return 0; // Placeholder
            }
            
            return 0;
        }
        
        /// <summary>
        /// Change game state through the new GameStateManager or fallback to original
        /// </summary>
        public void ChangeGameState(Starkiller.Core.GameState newState)
        {
            if (enableDelegation && gameStateManager != null)
            {
                gameStateManager.ChangeState(newState);
            }
            else if (fallbackToOriginal && originalGameManager != null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[GameManagerBridge] Falling back to original state management for {newState}");
                
                // Convert new state to old state and call original method
                // This would need mapping between the two GameState enums
            }
            else
            {
                Debug.LogError("[GameManagerBridge] No game state management system available!");
            }
        }
        
        /// <summary>
        /// Process end of day through the new managers
        /// </summary>
        public void ProcessEndOfDay(int shipsProcessed)
        {
            if (enableDelegation)
            {
                // Process salary through CreditsManager
                if (creditsManager != null && decisionTracker != null)
                {
                    creditsManager.ProcessDailySalary(
                        shipsProcessed, 
                        decisionTracker.DailyCorrectDecisions, 
                        decisionTracker.DailyWrongDecisions
                    );
                    
                    if (enableDebugLogs)
                        Debug.Log($"[GameManagerBridge] Processed end of day: {shipsProcessed} ships, salary calculated");
                }
            }
            else if (fallbackToOriginal && originalGameManager != null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[GameManagerBridge] Using original end of day processing");
                
                // Call original end of day logic
            }
        }
        
        /// <summary>
        /// Get performance summary from the new systems
        /// </summary>
        public string GetPerformanceSummary()
        {
            if (enableDelegation)
            {
                string summary = "";
                
                if (creditsManager != null)
                    summary += creditsManager.GetCreditSummary() + "\n\n";
                
                if (decisionTracker != null)
                    summary += decisionTracker.GetDailyPerformanceSummary();
                
                return summary;
            }
            
            return "Performance summary not available";
        }
        
        /// <summary>
        /// Enable or disable delegation to new managers
        /// </summary>
        public void SetDelegationEnabled(bool enabled)
        {
            enableDelegation = enabled;
            
            if (enableDebugLogs)
                Debug.Log($"[GameManagerBridge] Delegation {(enabled ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Log the status of available managers
        /// </summary>
        private void LogBridgeStatus()
        {
            if (!enableDebugLogs) return;
            
            Debug.Log($"[GameManagerBridge] Manager Status:");
            Debug.Log($"  CreditsManager: {(creditsManager != null ? "AVAILABLE" : "NOT FOUND")}");
            Debug.Log($"  DecisionTracker: {(decisionTracker != null ? "AVAILABLE" : "NOT FOUND")}");
            Debug.Log($"  GameStateManager: {(gameStateManager != null ? "AVAILABLE" : "NOT FOUND")}");
            Debug.Log($"  DayProgressionManager: {(dayProgressionManager != null ? "AVAILABLE" : "NOT FOUND")}");
            Debug.Log($"  AudioManager: {(audioManager != null ? "AVAILABLE" : "NOT FOUND")}");
            Debug.Log($"  UICoordinator: {(uiCoordinator != null ? "AVAILABLE" : "NOT FOUND")}");
            Debug.Log($"  Delegation: {(enableDelegation ? "ENABLED" : "DISABLED")}");
            Debug.Log($"  Fallback: {(fallbackToOriginal ? "ENABLED" : "DISABLED")}");
        }
        
        // Public methods for manual testing
        [ContextMenu("Test Bridge - Add 100 Credits")]
        public void TestAddCredits() => AddCredits(100, "Bridge Test");
        
        [ContextMenu("Test Bridge - Record Correct Decision")]
        public void TestCorrectDecision() => RecordDecision(true, "Bridge Test");
        
        [ContextMenu("Test Bridge - Record Wrong Decision")]
        public void TestWrongDecision() => RecordDecision(false, "Bridge Test");
        
        [ContextMenu("Show Bridge Status")]
        public void ShowBridgeStatus() => LogBridgeStatus();
        
        [ContextMenu("Show Performance Summary")]
        public void ShowPerformanceSummary() => Debug.Log(GetPerformanceSummary());
    }
}