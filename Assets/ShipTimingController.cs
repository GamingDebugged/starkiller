using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls the timing between ship appearances and encounter durations
/// Acts as a middleware between GameManager and ship generation systems
/// </summary>
public class ShipTimingController : MonoBehaviour
{
    [Header("Timing Settings")]
    [Tooltip("Time in seconds between ship appearances")]
    [SerializeField] private float timeBetweenShips = 5f;
    
    [Tooltip("Minimum time (in seconds) a ship encounter should be displayed")]
    [SerializeField] private float minimumEncounterDuration = 10f;
    
    [Tooltip("Time for holding pattern ships (in seconds)")]
    [SerializeField] private float holdingPatternDuration = 60f;
    
    [Tooltip("Additional delay after consequence panel is closed")]
    [SerializeField] private float postConsequenceDelay = 3f;
    
    [Header("System References")]
    [SerializeField] private CredentialChecker credentialChecker;
    [SerializeField] private EncounterMediaTransitionManager transitionManager;
    
    [Header("Debug")]
    [SerializeField] private bool verboseLogging = true;
    
    // State tracking
    private bool isWaitingForNextShip = false;
    private float currentCooldown = 0f;
    
    // Encounter timing
    private float currentEncounterStartTime = 0f;
    private bool isEncounterActive = false;
    
    // Ship generator references
    private MasterShipGenerator masterGenerator;
    private GameManager gameManager;
    
    // Holding pattern management
    private class HoldingPatternEntry
    {
        public MasterShipEncounter encounter;
        public float entryTime;
    }
    
    private List<HoldingPatternEntry> holdingPatternShips = new List<HoldingPatternEntry>();
    
    // Consequence panel tracking
    private bool wasConsequencePanelOpen = false;
    private float consequencePanelCloseTime = 0f;
    private GameObject consequencePanel = null;
    
    // Lock to prevent multiple ship generation systems from competing
    private bool isGenerationLocked = false;
    private string generationLockedBy = "";
    
    // Singleton pattern
    private static ShipTimingController _instance;
    public static ShipTimingController Instance => _instance;
    
    // Track initialization state
    private bool isFullyInitialized = false;
    
    void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Make this object persistent if it's a root GameObject
        if (transform.root == transform)
        {
            DontDestroyOnLoad(gameObject);
            LogMessage("ShipTimingController marked as persistent");
        }
        
        // Find references if not assigned
        if (credentialChecker == null)
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            
        if (transitionManager == null)
            transitionManager = FindFirstObjectByType<EncounterMediaTransitionManager>();
            
        // Try to find the consequence panel
        consequencePanel = GameObject.Find("ConsequencePanel");
        if (consequencePanel == null)
        {
            // Try to find through ConsequenceManager
            ConsequenceManager consequenceManager = FindFirstObjectByType<ConsequenceManager>();
            if (consequenceManager != null)
            {
                var panelField = consequenceManager.GetType().GetField("consequenceReportPanel", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    
                if (panelField != null)
                {
                    consequencePanel = panelField.GetValue(consequenceManager) as GameObject;
                    LogMessage($"Found consequence panel through ConsequenceManager: {consequencePanel != null}");
                }
            }
        }
    }
    
    void Start()
    {
        // Start delayed initialization
        StartCoroutine(DelayedInitialization(0.5f));
    }
    
    /// <summary>
    /// Initialize with a delay to ensure all systems are ready
    /// </summary>
    private IEnumerator DelayedInitialization(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Find references
        bool foundNewReferences = false;
        
        // First, try to get generator from ShipGeneratorManager if available
        ShipGeneratorManager generatorManager = ShipGeneratorManager.Instance;
        if (generatorManager != null && generatorManager.HasValidShipGenerator())
        {
            masterGenerator = generatorManager.GetShipGenerator();
            if (masterGenerator != null)
            {
                LogMessage("Acquired MasterShipGenerator from ShipGeneratorManager");
                foundNewReferences = true;
            }
        }
        
        // Fallback to direct search if manager not found
        if (masterGenerator == null)
        {
            masterGenerator = FindFirstObjectByType<MasterShipGenerator>();
            if (masterGenerator != null)
            {
                LogMessage("Acquired MasterShipGenerator through direct search");
                foundNewReferences = true;
            }
        }
        
        // Find GameManager
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                LogMessage("Acquired GameManager");
                foundNewReferences = true;
            }
        }
        
        // Try to get encounter system manager for better system coordination
        EncounterSystemManager systemManager = FindFirstObjectByType<EncounterSystemManager>();
        if (systemManager != null)
        {
            // Get the active system from the manager
            Component activeSystem = systemManager.GetActiveEncounterSystem();
            if (activeSystem is MasterShipGenerator)
            {
                masterGenerator = (MasterShipGenerator)activeSystem;
                LogMessage("Using MasterShipGenerator from EncounterSystemManager");
                foundNewReferences = true;
            }
        }
        
        // Get timing setting from GameManager if available
        if (gameManager != null && gameManager.timeBetweenShips > 0)
        {
            timeBetweenShips = gameManager.timeBetweenShips;
            LogMessage($"Using GameManager timeBetweenShips value: {timeBetweenShips}");
        }
        
        LogMessage($"Initialized with minimum encounter duration: {minimumEncounterDuration}s");
        
        // Ensure game state is correct
        GameStateController gameStateController = GameStateController.Instance;
        if (gameStateController != null && !gameStateController.IsGameplayActive())
        {
            LogMessage("Game not in active gameplay state. Waiting for state change.");
            
            // Subscribe to state changes to initialize when game becomes active
            gameStateController.OnGameStateChanged += OnGameStateChanged;
        }
        else if (gameStateController != null && gameStateController.IsGameplayActive())
        {
            LogMessage("Game already in active gameplay state. Proceeding with full initialization.");
            CompleteInitialization();
        }
        
        // If we found new references, notify the ShipGeneratorManager to sync everything
        if (foundNewReferences && ShipGeneratorManager.Instance != null)
        {
            ShipGeneratorManager.Instance.ForceSyncReferences();
        }
    }
    
    /// <summary>
    /// Handle game state changes
    /// </summary>
    private void OnGameStateChanged(GameStateController.GameActivationState newState)
    {
        if (newState == GameStateController.GameActivationState.ActiveGameplay)
        {
            LogMessage("Game entered active gameplay state - completing initialization");
            CompleteInitialization();
        }
    }
    
    /// <summary>
    /// Completes the initialization process 
    /// </summary>
    private void CompleteInitialization()
    {
        // Skip if already initialized
        if (isFullyInitialized)
            return;
            
        LogMessage("Completing full initialization");
        
        // Ensure UI Manager shows the gameplay panel
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            if (uiManager.gameplayPanel != null)
            {
                uiManager.ShowPanel(uiManager.gameplayPanel);
                LogMessage("Showing gameplay panel via UIManager");
            }
        }
        
        // Reset all cooldowns and locks
        ResetCooldown();
        
        // Mark as initialized
        isFullyInitialized = true;
    }
    
    void Update()
    {
        // Check consequence panel state
        if (consequencePanel != null)
        {
            bool isOpen = consequencePanel.activeSelf;
            
            // If panel was open and now closed, record the time
            if (wasConsequencePanelOpen && !isOpen)
            {
                consequencePanelCloseTime = Time.time;
                LogMessage($"Consequence panel closed at {consequencePanelCloseTime}");
            }
            
            wasConsequencePanelOpen = isOpen;
        }
        
        // Count down the cooldown timer
        if (isWaitingForNextShip && currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            
            // Debug logging for last second of countdown
            if (verboseLogging && currentCooldown <= 1.0f && currentCooldown > 0)
            {
                LogMessage($"Ship countdown: {currentCooldown:F1} seconds remaining");
            }
            
            // When cooldown expires, we're no longer waiting
            if (currentCooldown <= 0)
            {
                isWaitingForNextShip = false;
                LogMessage("Ship cooldown complete - next ship can now appear");
                
                // Also release generation lock if it's still held
                if (isGenerationLocked)
                {
                    isGenerationLocked = false;
                    LogMessage($"Released generation lock from {generationLockedBy} after cooldown");
                }
            }
        }
        
        // Check for holding pattern ships that should be released
        CheckHoldingPatternShips();
        
        // Update encounter timing
        if (isEncounterActive)
        {
            // Check if current encounter has been displayed for minimum duration
            float currentDuration = Time.time - currentEncounterStartTime;
            
            if (verboseLogging && currentDuration > 0 && currentDuration % 5f < 0.1f)
            {
                LogMessage($"Current encounter has been displayed for {currentDuration:F1}s");
            }
        }
    }
    
    /// <summary>
    /// Request a new ship - respects the timing between ships
    /// </summary>
    /// <param name="requestSource">Name of the method/system requesting the ship</param>
    /// <returns>True if request was approved, false if delayed</returns>
    public bool RequestNextShip(string requestSource = "Unknown")
    {
        // Don't generate immediately after consequence panel closes
        if (Time.time - consequencePanelCloseTime < postConsequenceDelay)
        {
            LogMessage($"Ship request from {requestSource} denied - too soon after consequence panel closed ({Time.time - consequencePanelCloseTime:F1}s < {postConsequenceDelay}s)");
            return false;
        }
        
        // Check if consequence panel is currently open
        if (consequencePanel != null && consequencePanel.activeSelf)
        {
            LogMessage($"Ship request from {requestSource} denied - consequence panel is open");
            return false;
        }
        
        // Check if we're still in cooldown
        if (isWaitingForNextShip)
        {
            LogMessage($"Ship request from {requestSource} denied - still cooling down ({currentCooldown:F1}s remaining)");
            return false;
        }
        
        // Check if another system has the generation lock
        if (isGenerationLocked)
        {
            LogMessage($"Ship request from {requestSource} denied - generation locked by {generationLockedBy}");
            return false;
        }
        
        // Acquire the generation lock
        isGenerationLocked = true;
        generationLockedBy = requestSource;
        LogMessage($"Ship request from {requestSource} approved - acquired generation lock");
        
        // Start cooldown timer
        StartCooldown();
        
        // Return true to indicate request was processed
        return true;
    }
    
    /// <summary>
    /// Called when a ship is processed to start the cooldown timer
    /// </summary>
    public void OnShipProcessed(string source = "Unknown")
    {
        StartCooldown();
        
        // Release the generation lock if it was acquired
        if (isGenerationLocked)
        {
            isGenerationLocked = false;
            LogMessage($"Released generation lock from {generationLockedBy} after processing");
        }
    }
    
    /// <summary>
    /// Start the cooldown timer
    /// </summary>
    private void StartCooldown()
    {
        isWaitingForNextShip = true;
        currentCooldown = timeBetweenShips;
        LogMessage($"Starting ship cooldown: {timeBetweenShips}s");
    }
    
    /// <summary>
    /// Check if it's okay to generate a new ship now
    /// </summary>
    /// <param name="source">Name of the method/system checking for generation</param>
    /// <returns>True if can generate, false otherwise</returns>
    public bool CanGenerateNewShip(string source = "Unknown")
    {
        // If consequence panel is open, wait
        if (consequencePanel != null && consequencePanel.activeSelf)
        {
            if (verboseLogging)
            {
                LogMessage($"Cannot generate new ship: Consequence panel is open");
            }
            return false;
        }
        
        // If recently closed panel, wait
        if (Time.time - consequencePanelCloseTime < postConsequenceDelay)
        {
            if (verboseLogging)
            {
                LogMessage($"Cannot generate new ship: Too soon after consequence panel closed ({Time.time - consequencePanelCloseTime:F1}s < {postConsequenceDelay}s)");
            }
            return false;
        }
        
        // If transition manager is in transition, wait for it to complete
        if (transitionManager != null && transitionManager.IsTransitioning())
        {
            if (verboseLogging)
            {
                LogMessage("Cannot generate new ship: Media transition in progress");
            }
            return false;
        }
        
        // If an encounter is active, check if it's been displayed long enough
        if (isEncounterActive)
        {
            float currentDuration = Time.time - currentEncounterStartTime;
            
            if (currentDuration < minimumEncounterDuration)
            {
                if (verboseLogging)
                {
                    LogMessage($"Cannot generate new ship: Current encounter has only been displayed for {currentDuration:F1}s of {minimumEncounterDuration}s minimum");
                }
                return false;
            }
        }
        
        // If we're in cooldown, respect that
        if (isWaitingForNextShip)
        {
            if (verboseLogging)
            {
                LogMessage($"Cannot generate new ship: Still in cooldown period ({currentCooldown:F1}s remaining)");
            }
            return false;
        }
        
        // If another system has the generation lock, wait
        if (isGenerationLocked && generationLockedBy != source)
        {
            if (verboseLogging)
            {
                LogMessage($"Cannot generate new ship: Generation lock held by {generationLockedBy}");
            }
            return false;
        }
        
        // All checks passed, acquire the lock if not already held
        if (!isGenerationLocked)
        {
            isGenerationLocked = true;
            generationLockedBy = source;
            LogMessage($"Generation lock acquired by {source}");
        }
        
        // All checks passed
        return true;
    }
    
    /// <summary>
    /// Force reset the cooldown (for emergency situations or game state changes)
    /// </summary>
    public void ResetCooldown()
    {
        isWaitingForNextShip = false;
        currentCooldown = 0f;
        
        // Also release generation lock
        isGenerationLocked = false;
        generationLockedBy = "";
        
        // Ensure all references are valid by checking persistence manager first
        ShipGeneratorManager persistenceManager = ShipGeneratorManager.Instance;
        
        if (persistenceManager != null && persistenceManager.HasValidShipGenerator())
        {
            masterGenerator = persistenceManager.GetShipGenerator();
            LogMessage("Retrieved MasterShipGenerator from ShipGeneratorManager during reset");
        }
        else
        {
            // Fall back to direct search
            if (masterGenerator == null)
            {
                masterGenerator = FindFirstObjectByType<MasterShipGenerator>();
                LogMessage($"Re-acquired MasterShipGenerator reference during cooldown reset: {masterGenerator != null}");
            }
        }
        
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
            LogMessage($"Re-acquired GameManager reference during cooldown reset: {gameManager != null}");
        }
        
        if (credentialChecker == null)
        {
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            LogMessage($"Re-acquired CredentialChecker reference during cooldown reset: {credentialChecker != null}");
        }
        
        if (transitionManager == null)
        {
            transitionManager = FindFirstObjectByType<EncounterMediaTransitionManager>();
            LogMessage($"Re-acquired EncounterMediaTransitionManager reference during cooldown reset: {transitionManager != null}");
        }
        
        // Notify ShipGeneratorManager that we've reset references
        if (persistenceManager != null)
        {
            persistenceManager.ForceSyncReferences();
        }
        
        LogMessage("Ship cooldown and generation lock forcibly reset");
    }
    
    /// <summary>
    /// Notify the controller that a new encounter has started
    /// </summary>
    public void NotifyEncounterStarted(MasterShipEncounter encounter)
    {
        if (encounter == null)
        {
            Debug.LogWarning("Attempted to start null encounter");
            return;
        }
        
        // Log the encounter start with detailed info
        if (verboseLogging)
        {
            LogMessage($"Starting encounter: {encounter.shipType} - {encounter.shipName}, captain: {encounter.captainName}");
        }
        
        // Update timing
        currentEncounterStartTime = Time.time;
        isEncounterActive = true;
        
        // Update encounter timing in DebugMonitor if available
        DebugMonitor debugMonitor = DebugMonitor.Instance;
        if (debugMonitor != null)
        {
            debugMonitor.LogEncounterDisplayed(encounter, generationLockedBy);
        }
    }
    
    /// <summary>
    /// Notify the controller that the current encounter has ended
    /// </summary>
    public void NotifyEncounterEnded()
    {
        if (!isEncounterActive)
        {
            // No active encounter to end
            return;
        }
        
        // Calculate how long this encounter was displayed
        float displayDuration = Time.time - currentEncounterStartTime;
        
        if (verboseLogging)
        {
            LogMessage($"Ending encounter after {displayDuration:F1}s");
        }
        
        // Update timing
        isEncounterActive = false;
        
        // Start cooldown for next ship
        StartCooldown();
        
        // Release generation lock
        isGenerationLocked = false;
        generationLockedBy = "";
    }
    
    /// <summary>
    /// Add a ship to the holding pattern
    /// </summary>
    public void AddShipToHoldingPattern(MasterShipEncounter encounter)
    {
        if (encounter == null)
        {
            Debug.LogWarning("Attempted to add null encounter to holding pattern");
            return;
        }
        
        // Create a new holding pattern entry
        HoldingPatternEntry entry = new HoldingPatternEntry
        {
            encounter = encounter,
            entryTime = Time.time
        };
        
        // Add to list
        holdingPatternShips.Add(entry);
        
        // Set the encounter's holding pattern flag
        encounter.isInHoldingPattern = true;
        
        if (verboseLogging)
        {
            LogMessage($"Added ship to holding pattern: {encounter.shipType} - {encounter.shipName}. Total ships in holding: {holdingPatternShips.Count}");
        }
        
        // Log to DebugMonitor if available
        DebugMonitor debugMonitor = DebugMonitor.Instance;
        if (debugMonitor != null)
        {
            debugMonitor.TrackDataValue("ShipsInHoldingPattern", holdingPatternShips.Count);
        }
    }
    
    /// <summary>
    /// Check if any ships in the holding pattern should be released
    /// </summary>
    private void CheckHoldingPatternShips()
    {
        if (holdingPatternShips.Count == 0)
            return;
            
        // Check each ship in holding pattern
        for (int i = holdingPatternShips.Count - 1; i >= 0; i--)
        {
            HoldingPatternEntry entry = holdingPatternShips[i];
            float timeInHolding = Time.time - entry.entryTime;
            
            // Check if ship has been in holding pattern long enough
            if (timeInHolding >= holdingPatternDuration)
            {
                // Get the encounter from the entry
                MasterShipEncounter encounter = entry.encounter;
                
                // Remove from holding pattern
                holdingPatternShips.RemoveAt(i);
                
                if (verboseLogging)
                {
                    LogMessage($"Releasing ship from holding pattern: {encounter.shipType} - {encounter.shipName} after {timeInHolding:F1}s");
                }
                
                // Reset holding pattern flag
                encounter.isInHoldingPattern = false;
                
                // Get the ship generator from the manager first if possible
                ShipGeneratorManager persistenceManager = ShipGeneratorManager.Instance;
                if (persistenceManager != null && persistenceManager.HasValidShipGenerator())
                {
                    masterGenerator = persistenceManager.GetShipGenerator();
                }
                
                // Notify the ship generator to requeue this ship
                if (masterGenerator != null)
                {
                    masterGenerator.ProcessHoldingPatternCompletion(encounter);
                }
                else
                {
                    LogMessage("WARNING: Cannot process holding pattern completion - MasterShipGenerator is null");
                    // Try to find it directly
                    masterGenerator = FindFirstObjectByType<MasterShipGenerator>();
                    if (masterGenerator != null)
                    {
                        masterGenerator.ProcessHoldingPatternCompletion(encounter);
                    }
                }
                
                // Log to DebugMonitor if available
                DebugMonitor debugMonitor = DebugMonitor.Instance;
                if (debugMonitor != null)
                {
                    debugMonitor.TrackDataValue("ShipsInHoldingPattern", holdingPatternShips.Count);
                }
            }
        }
    }
    
    /// <summary>
    /// Get the number of ships currently in the holding pattern
    /// </summary>
    public int GetHoldingPatternCount()
    {
        return holdingPatternShips.Count;
    }
    
    /// <summary>
    /// Check if there is an active encounter currently
    /// </summary>
    public bool IsEncounterActive()
    {
        return isEncounterActive;
    }
    
    /// <summary>
    /// Get the time the current encounter has been displayed
    /// </summary>
    public float GetCurrentEncounterDisplayTime()
    {
        if (!isEncounterActive)
            return 0f;
            
        return Time.time - currentEncounterStartTime;
    }
    
    /// <summary>
    /// Log a message with the ShipTimingController prefix
    /// </summary>
    private void LogMessage(string message)
    {
        if (verboseLogging)
        {
            // Try to log using DebugMonitor if available
            DebugMonitor debugMonitor = DebugMonitor.Instance;
            if (debugMonitor != null)
            {
                debugMonitor.LogDataFlow(message);
            }
            else
            {
                Debug.Log($"[ShipTimingController] {message}");
            }
        }
    }
}