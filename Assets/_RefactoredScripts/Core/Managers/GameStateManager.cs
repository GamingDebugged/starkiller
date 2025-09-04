using UnityEngine;
using System;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Focused manager for game state only
    /// Replaces state management portion of the massive GameManager
    /// Can be tested alongside existing GameManager
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        
        private GameState _currentState = GameState.MainMenu;
        private GameState _previousState = GameState.MainMenu;
        
        public GameState CurrentState => _currentState;
        public GameState PreviousState => _previousState;
        
        // Events for state changes
        public static event Action<GameState, GameState> OnStateChanged;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<GameStateManager>(this);
            
            if (enableDebugLogs)
                Debug.Log("[GameStateManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Set initial state
            ChangeState(GameState.MainMenu);
        }
        
        /// <summary>
        /// Change the game state with proper validation and events
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (_currentState == newState)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[GameStateManager] Already in state: {newState}");
                return;
            }
            
            // Validate state transition
            if (!IsValidTransition(_currentState, newState))
            {
                Debug.LogError($"[GameStateManager] Invalid state transition: {_currentState} -> {newState}");
                return;
            }
            
            _previousState = _currentState;
            _currentState = newState;
            
            if (enableDebugLogs)
                Debug.Log($"[GameStateManager] State changed: {_previousState} -> {_currentState}");
            
            // Trigger events
            OnStateChanged?.Invoke(_previousState, _currentState);
            GameEvents.TriggerGameStateChanged(_currentState);
            
            // Handle state-specific logic
            OnStateEntered(_currentState);
        }
        
        /// <summary>
        /// Check if state transition is valid
        /// </summary>
        private bool IsValidTransition(GameState from, GameState to)
        {
            switch (from)
            {
                case GameState.MainMenu:
                    return to == GameState.DailyBriefing;
                    
                case GameState.DailyBriefing:
                    return to == GameState.Gameplay || to == GameState.MainMenu;
                    
                case GameState.Gameplay:
                    return to == GameState.Paused || to == GameState.DayReport || to == GameState.GameOver;
                    
                case GameState.Paused:
                    return to == GameState.Gameplay || to == GameState.MainMenu;
                    
                case GameState.DayReport:
                    return to == GameState.DailyBriefing || to == GameState.GameOver || to == GameState.MainMenu;
                    
                case GameState.GameOver:
                    return to == GameState.MainMenu;
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Handle logic when entering a new state
        /// </summary>
        private void OnStateEntered(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;
                    
                case GameState.DailyBriefing:
                    Time.timeScale = 1f;
                    break;
                    
                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    GameEvents.TriggerGameStarted();
                    break;
                    
                case GameState.Paused:
                    Time.timeScale = 0f;
                    GameEvents.TriggerGamePaused();
                    break;
                    
                case GameState.DayReport:
                    Time.timeScale = 1f;
                    break;
                    
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    GameEvents.TriggerGameEnded();
                    break;
            }
        }
        
        /// <summary>
        /// Public methods for common state changes
        /// </summary>
        public void StartGame() => ChangeState(GameState.Gameplay);
        public void PauseGame() => ChangeState(GameState.Paused);
        public void ResumeGame() => ChangeState(GameState.Gameplay);
        public void EndDay() => ChangeState(GameState.DayReport);
        public void GameOver() => ChangeState(GameState.GameOver);
        public void ReturnToMenu() => ChangeState(GameState.MainMenu);
        
        /// <summary>
        /// Check if currently in a specific state
        /// </summary>
        public bool IsInState(GameState state) => _currentState == state;
        
        /// <summary>
        /// Check if in any of multiple states
        /// </summary>
        public bool IsInAnyState(params GameState[] states)
        {
            foreach (var state in states)
            {
                if (_currentState == state) return true;
            }
            return false;
        }
        
        private void OnDestroy()
        {
            OnStateChanged = null;
        }
        
        // Debug methods for editor testing
        [ContextMenu("Test: Go to Gameplay")]
        private void TestGoToGameplay() => ChangeState(GameState.Gameplay);
        
        [ContextMenu("Test: Pause Game")]
        private void TestPauseGame() => ChangeState(GameState.Paused);
        
        [ContextMenu("Test: Resume Game")]
        private void TestResumeGame() => ChangeState(GameState.Gameplay);
    }
}