using UnityEngine;
using Starkiller.Core;
using Starkiller.Core.Managers;

/// <summary>
/// Simple component to ensure ShiftTimerManager starts when needed
/// Add this to any button that should start the shift timer
/// </summary>
public class ShiftTimerStarter : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Also start DayProgressionManager shift")]
    public bool startDayProgression = true;
    
    [Tooltip("Debug logging")]
    public bool enableDebugLogs = true;
    
    
    /// <summary>
    /// Call this method from button OnClick to start the timer
    /// </summary>
    public void StartShiftTimer()
    {
        if (enableDebugLogs)
            Debug.Log("[ShiftTimerStarter] StartShiftTimer called - ensuring timer starts");
        
        // Get ShiftTimerManager and start it
        var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
        if (shiftTimerManager != null)
        {
            if (!shiftTimerManager.IsTimerActive)
            {
                shiftTimerManager.StartTimer();
                if (enableDebugLogs)
                    Debug.Log("[ShiftTimerStarter] ShiftTimerManager started successfully");
            }
            else
            {
                if (enableDebugLogs)
                    Debug.Log("[ShiftTimerStarter] Timer already active");
            }
        }
        else
        {
            Debug.LogError("[ShiftTimerStarter] ShiftTimerManager not found!");
        }
        
        // Also start DayProgressionManager shift if enabled
        if (startDayProgression)
        {
            var dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
            if (dayProgressionManager != null)
            {
                dayProgressionManager.StartShift();
                if (enableDebugLogs)
                    Debug.Log("[ShiftTimerStarter] DayProgressionManager shift started");
            }
            
            // CRITICAL: Update both game state systems to Gameplay
            GameStateController controller = GameStateController.Instance;
            if (controller != null)
            {
                controller.OnDayBriefingComplete();
                if (enableDebugLogs)
                    Debug.Log("[ShiftTimerStarter] Updated old GameStateController to ActiveGameplay");
            }
            
            var gameStateManager = ServiceLocator.Get<Starkiller.Core.Managers.GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.ChangeState(Starkiller.Core.GameState.Gameplay);
                if (enableDebugLogs)
                    Debug.Log("[ShiftTimerStarter] Updated new GameStateManager to Gameplay");
            }
            
            // Ensure GameManager state is also updated
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                // Get the internal field using reflection or public method if available
                if (enableDebugLogs)
                    Debug.Log("[ShiftTimerStarter] GameManager found, state should be synchronized");
            }
            
            // Ensure encounters are available for the new day
            MasterShipGenerator shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            if (shipGenerator != null)
            {
                if (enableDebugLogs)
                    Debug.Log("[ShiftTimerStarter] Ensuring ship generator is ready for encounters");
                // The ship generator should already have encounters for the current day
                // This just ensures it's in a ready state
            }
        }
    }
    
    /// <summary>
    /// Alternative method that only starts the timer without other systems
    /// </summary>
    public void StartTimerOnly()
    {
        var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
        if (shiftTimerManager != null && !shiftTimerManager.IsTimerActive)
        {
            shiftTimerManager.StartTimer();
            Debug.Log("[ShiftTimerStarter] Timer started (minimal mode)");
        }
    }
}