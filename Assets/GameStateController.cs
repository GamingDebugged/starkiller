using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using StarkillerBaseCommand;


/// <summary>
/// Controls overall game state and ensures systems don't activate prematurely.
/// This controller ensures that game systems wait until appropriate to begin processing.
/// </summary>
public class GameStateController : MonoBehaviour
{
    public enum GameActivationState
    {
        MainMenu,       // Player is in the main menu
        DayStart,       // Day is starting (briefing)
        ActiveGameplay, // Active gameplay (processing ships)
        DayEnd,         // Day is ending (report)
        Paused          // Game is paused
    }
    
    [Header("Current State")]
    [SerializeField] private GameActivationState currentState = GameActivationState.MainMenu;
    
    [Header("System References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private MasterShipGenerator shipGenerator;
    [SerializeField] private CredentialChecker credentialChecker;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    // Event delegates for state changes
    public delegate void GameStateChangeHandler(GameActivationState newState);
    public event GameStateChangeHandler OnGameStateChanged;
    
    // Singleton pattern
    private static GameStateController _instance;
    public static GameStateController Instance => _instance;
    
    private void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Make object persistent using safe helper
        this.SafeDontDestroyOnLoad();
        
        // Find references if not assigned
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
            
        if (shipGenerator == null)
            shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            
        if (credentialChecker == null)
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            
        LogStatus("GameStateController initialized");
    }
    
    private void Start()
    {
        // Check if UI is already showing
        bool uiAlreadyActive = false;
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        
        if (uiManager != null && uiManager.gameplayPanel != null && uiManager.gameplayPanel.activeSelf)
        {
            uiAlreadyActive = true;
            LogStatus("UI gameplay panel is already active at startup");
        }
        
        // If UI is already showing gameplay panel, go to active gameplay
        // This helps when playing directly in scene or when restarting the game
        if (uiAlreadyActive)
        {
            LogStatus("Starting in ACTIVE GAMEPLAY state based on UI status");
            SetGameState(GameActivationState.ActiveGameplay);
            
            // Initialize gameplay systems with a short delay
            StartCoroutine(DelayedInitialization(0.5f));
        }
        else
        {
            // Normal behavior - start in menu
            LogStatus("Starting in MainMenu state (normal flow)");
            SetGameState(GameActivationState.MainMenu);
        }
    }
    
    // Helper coroutine to initialize gameplay systems with delay
    private IEnumerator DelayedInitialization(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Initialize gameplay systems
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
        
        if (shipGenerator != null)
        {
            shipGenerator.GetNextEncounter();
        }
        
        if (credentialChecker != null)
        {
            credentialChecker.ForceUIVisibility();
        }
        
        LogStatus("Delayed initialization complete");
    }
    

    /// <summary>
    /// Called by the Start Game Button to transition from main menu to active gameplay
    /// </summary>
    public void StartGame()
    {
        if (currentState == GameActivationState.MainMenu)
        {
            LogStatus("Starting game from main menu");
            SetGameState(GameActivationState.DayStart);
            
            // Notify GameManager to start the game - with a slight delay
            StartCoroutine(DelayedGameStart(0.5f));
        }
    }
    
    /// <summary>
    /// Called by GameManager when day briefing is complete
    /// </summary>
    public void OnDayBriefingComplete()
    {
        if (currentState == GameActivationState.DayStart)
        {
            LogStatus("Day briefing complete, starting active gameplay");
            SetGameState(GameActivationState.ActiveGameplay);
        }
    }
    
    /// <summary>
    /// Called by GameManager when day ends
    /// </summary>
    public void OnDayEnd()
    {
        if (currentState == GameActivationState.ActiveGameplay)
        {
            LogStatus("Day ended, showing report");
            SetGameState(GameActivationState.DayEnd);
        }
    }
    
    /// <summary>
    /// Called by GameManager when starting a new day
    /// </summary>
    public void OnNewDayStart()
    {
        if (currentState == GameActivationState.DayEnd)
        {
            LogStatus("Starting new day");
            SetGameState(GameActivationState.DayStart);
        }
    }
    
    /// <summary>
    /// Set the game state with notification
    /// </summary>
    public void SetGameState(GameActivationState newState)
    {
        // Skip if same state
        if (newState == currentState)
            return;
            
        // Log state change
        LogStatus($"Game state changing: {currentState} -> {newState}");
        
        // Update audio based on state
        if (newState != GameActivationState.ActiveGameplay && newState != GameActivationState.DayStart)
        {
            // Mute video audio when not in active gameplay or briefing
            SetAudioMuted(true);
        }
        else
        {
            // Unmute audio for active gameplay
            SetAudioMuted(false);
        }

        // Update state
        GameActivationState oldState = currentState;
        currentState = newState;
        
        // Handle state-specific transitions
        HandleStateTransition(oldState, newState);
        
        // Notify subscribers
        OnGameStateChanged?.Invoke(newState);
    }
    
    /// <summary>
    /// Handle specific functionality when transitioning between states
    /// </summary>
    private void HandleStateTransition(GameActivationState oldState, GameActivationState newState)
    {
        // Handle transitions to ActiveGameplay state
        if (newState == GameActivationState.ActiveGameplay)
        {
            // Ensure systems are ready for active gameplay
            if (shipGenerator != null)
            {
                // Ensure first encounter is ready
                shipGenerator.GetNextEncounter();
            }
        }
        
        // Handle transitions to MainMenu state
        if (newState == GameActivationState.MainMenu)
        {
            // Disable active gameplay systems
            if (shipGenerator != null)
            {
                // Could disable certain updates or processing
            }
        }
    }
    
    /// <summary>
    /// Check if the game is in an active state where gameplay should proceed
    /// </summary>
    public bool IsGameplayActive()
    {
        return currentState == GameActivationState.ActiveGameplay;
    }
    
    /// <summary>
    /// Get the current game state
    /// </summary>
    public GameActivationState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// Delay game start to ensure all systems are ready
    /// </summary>
    private IEnumerator DelayedGameStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
    }
    
    /// <summary>
    /// Mutes or unmutes all audio in the game, including video player audio
    /// </summary>
    public void SetAudioMuted(bool muted)
    {
        // First set the AudioListener mute state
        AudioListener.pause = muted;
        
        // Then find and update all video players in the scene
        VideoPlayer[] allVideoPlayers = FindObjectsByType<VideoPlayer>(FindObjectsSortMode.None);
        foreach (VideoPlayer player in allVideoPlayers)
        {
            if (player.controlledAudioTrackCount > 0)
            {
                player.SetDirectAudioMute(0, muted);
            }
        }
        
        // Set a flag to track audio state
        _isAudioMuted = muted;
        
        // Log the change
        Debug.Log($"Game audio {(muted ? "muted" : "unmuted")}");
    }

    /// <summary>
    /// Gets whether audio is currently muted
    /// </summary>
    public bool IsAudioMuted()
    {
        return _isAudioMuted;
    }

    // Add this private field at the top of the class
    private bool _isAudioMuted = false;

    /// <summary>
    /// Log a debug message
    /// </summary>
    private void LogStatus(string message)
    {
        if (debugMode)
        {
            Debug.Log($"[GameStateController] {message}");
        }
    }
}