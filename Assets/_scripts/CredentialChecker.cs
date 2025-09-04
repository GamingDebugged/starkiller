using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using StarkillerBaseCommand;
using UnityEngine.EventSystems;
using Starkiller.Core;
using Starkiller.Core.Managers;

/// <summary>
/// UI controller for ship credential checking
/// Updated to work with the MasterShipEncounter class
/// </summary>
public class CredentialChecker : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text shipInfoPanel;        // Displays ship information
    public TMP_Text credentialsPanel;     // Displays credentials to check
    public TMP_Text feedbackText;         // Shows feedback after decision
    public GameObject feedbackPanel;      // Panel for feedback messages
    public TMP_Text captainDialogText;    // Displays captain dialog
    
    [Header("Reference Books")]
    public GameObject logBookPanel;       // Panel for log book
    public TMP_Text logBookContent;       // Text content of log book
    
    [Header("UI Buttons")]
    public Button approveButton;          // Button to approve
    public Button denyButton;             // Button to deny
    public Button openLogBookButton;      // Button to open log book
    public Button closeLogBookButton;     // Button to close log book
    public Button acceptBribeButton;      // Button to accept bribe (optional)
    public Button holdingPatternButton;   // Button to put ship in holding pattern
    public Button tractorBeamButton;      // Button to capture ship with tractor beam
    
    [Header("ID Card System")]
    [SerializeField] private CaptainIDCard captainIDCard;
    [SerializeField] private Button captainVideoButton;    // Button component on captain video
    [SerializeField] private Button captainImageButton;    // Button component on captain image

    [Header("Debugging")]
    [Tooltip("Enable detailed debug logs for encounter processing")]
    public bool verboseDebugLogs = true;  // Toggle for detailed logging
    
    // Processing lock to prevent race conditions
    private bool isProcessingDecision = false;
    
    [Header("Visual Feedback")]
    public GameObject approveStamp;       // Visual stamp for approval
    public GameObject denyStamp;          // Visual stamp for denial
    public GameObject tractorBeam;       // Visual stamp for tractor beam
    public GameObject holdingPattern;     // Visual stamp for holding pattern
    
    [Header("Ship Visuals")]
    public Image shipImageDisplay;        // Image component to display ship
    public Image captainImageDisplay;     // Image component to display captain
    public GameObject shipImageContainer; // Container for ship image (to hide if no image)
    public GameObject captainImageContainer; // Container for captain image (to hide if no image)

    [Header("Video Components")]
    public VideoPlayer shipVideoPlayer;         // Video player for ship videos
    public VideoPlayer captainVideoPlayer;      // Video player for captain videos
    public GameObject shipVideoContainer;       // Container for ship video (to hide if no video)
    public GameObject captainVideoContainer;    // Container for captain video (to hide if no video)
    
    [Header("Audio")]
    public AudioSource approveSound;      // Sound for approved ship
    public AudioSource denySound;         // Sound for denied ship
    public AudioSource errorSound;        // Sound for error
    public AudioSource bribeSound;        // Sound for accepting bribe
    public AudioSource holdingPatternSound; // Sound for holding pattern
    public AudioSource tractorBeamSound;  // Sound for tractor beam activation
    
    [Header("Game Status")]
    public TMP_Text gameStatusText;      // Reference to the status bar text component

    [Header("System References")]
    [SerializeField] private MasterShipGenerator masterShipGenerator;
    [SerializeField] private StarkkillerContentManager contentManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameManagerIntegrationHelper integrationHelper;
    
    // Current ship encounter data
    private MasterShipEncounter currentEncounter;
    
    // Flag to track if we've initialized video players
    private bool videoPlayersInitialized = false;
    
    // Flag to detect if we're currently in a decision animation
    private bool isInDecisionAnimation = false;
    
    // Minimum encounter display time tracking
    private float encounterDisplayTime = 0f;
    [Header("Timing Settings")]
    [SerializeField] private float minimumDisplayTime = 10f; // Configurable minimum display time
    
    // Video playback tracking to prevent duplicates
    private bool shipVideoStarted = false;
    
    // Text synchronization state
    private bool isReactionVideoPlaying = false;
    private MasterShipEncounter pendingTextEncounter = null;
    private bool captainVideoStarted = false;

    /// <summary>
    /// Get the current encounter (used by ID card system)
    /// </summary>
    public MasterShipEncounter GetCurrentEncounter()
    {
        return currentEncounter;
    }

    private IEnumerator RetryGetShipGenerator(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("CredentialChecker: Retrying to get ShipGenerator after delay");

        // Try to get the generator using the singleton pattern
        masterShipGenerator = MasterShipGenerator.Instance;
        Debug.Log($"CredentialChecker: Retry found ShipGenerator: {masterShipGenerator != null}");

        if (masterShipGenerator == null)
        {
            // Try with FindFirstObjectByType as a last resort
            masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            Debug.Log($"CredentialChecker: Last resort FindFirstObjectByType for ShipGenerator: {masterShipGenerator != null}");
        }

        // If we found it, subscribe to its event
        if (masterShipGenerator != null)
        {
            // Unsubscribe first to avoid duplicates
            masterShipGenerator.OnEncounterReady -= DisplayEncounter;

            // Then subscribe
            masterShipGenerator.OnEncounterReady += DisplayEncounter;
            Debug.Log("CredentialChecker: Successfully subscribed to OnEncounterReady event after retry");

            // Hide error message if it was shown
            if (feedbackPanel)
            {
                feedbackPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Initializes the CredentialChecker
    /// </summary>
    void Start()
    {
        try
        {
            // Find system references if not assigned - ALWAYS use singleton pattern
            InitializeSystemReferences();

            // Initialize video players
            InitializeVideoPlayers();

            if (masterShipGenerator == null)
            {
                Debug.LogError("CredentialChecker: MasterShipGenerator not found! Some features will not work.");

                // Show error to user
                if (feedbackPanel && feedbackText)
                {
                    feedbackText.text = "ERROR: Ship generator not found. Game may not function correctly.";
                    feedbackText.color = Color.red;
                    feedbackPanel.SetActive(true);
                }

                // Try one more time with delay to ensure generator is initialized
                StartCoroutine(RetryInitializeGeneratorWithDelay(1.0f));
            }

            // Setup button listeners
            SetupButtonListeners();

            // Set up captain click handlers for ID card
            SetupCaptainClickHandlers();

            // Subscribe to video synchronization events
            SubscribeToVideoSyncEvents();

            // Initialize UI elements
            // Hide feedback and log book panels initially
            if (feedbackPanel) feedbackPanel.SetActive(false);
            if (logBookPanel) logBookPanel.SetActive(false);

            // Hide stamps initially
            if (approveStamp) approveStamp.SetActive(false);
            if (denyStamp) denyStamp.SetActive(false);

            // Hide ship/captain image containers initially if they exist
            if (shipImageContainer) shipImageContainer.SetActive(false);
            if (captainImageContainer) captainImageContainer.SetActive(false);

            // Hide video containers initially if they exist
            if (shipVideoContainer) shipVideoContainer.SetActive(false);
            if (captainVideoContainer) captainVideoContainer.SetActive(false);

            // Subscribe to events from the generator
            // Always verify we have a valid reference before subscribing
            if (masterShipGenerator == null)
            {
                masterShipGenerator = MasterShipGenerator.Instance;
                Debug.Log("CredentialChecker: Updated MasterShipGenerator reference from singleton.");
            }

            // Only subscribe if we have a valid reference
            if (masterShipGenerator != null)
            {
                try
                {
                    // First unsubscribe to avoid duplicate registrations
                    masterShipGenerator.OnEncounterReady -= DisplayEncounter;

                    // Then subscribe
                    masterShipGenerator.OnEncounterReady += DisplayEncounter;
                    Debug.Log("CredentialChecker: Successfully subscribed to OnEncounterReady event");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"CredentialChecker: Error subscribing to OnEncounterReady event: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("CredentialChecker: Failed to subscribe to OnEncounterReady - masterShipGenerator is null");
            }

            // Initial UI update
            UpdateLogBook();

            // Request the first encounter once everything is initialized
            // Add a slight delay to give other systems time to initialize
            StartCoroutine(RequestInitialEncounterAfterDelay(0.5f));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CredentialChecker: Error during initialization: {e.Message}\n{e.StackTrace}");

            // Show error to user
            if (feedbackPanel && feedbackText)
            {
                feedbackText.text = "ERROR: Failed to initialize credential checker. Please restart the game.";
                feedbackText.color = Color.red;
                feedbackPanel.SetActive(true);
            }
            // Try to get reference, with retry logic if needed
            if (masterShipGenerator == null)
            {
                masterShipGenerator = MasterShipGenerator.Instance;

                if (masterShipGenerator == null)
                {
                    // Start retry coroutine with shorter wait time
                    StartCoroutine(RetryGetShipGenerator(0.5f));
                }
            }
        }
    }
    
    /// <summary>
    /// Initializes system references using singleton pattern
    /// </summary>
    private void InitializeSystemReferences()
    {
        // Find MasterShipGenerator reference
        if (masterShipGenerator == null)
        {
            masterShipGenerator = MasterShipGenerator.Instance;
            Debug.Log($"CredentialChecker: Found MasterShipGenerator through singleton: {masterShipGenerator != null}");
            
            if (masterShipGenerator == null)
            {
                // Try to find using FindFirstObjectByType as fallback
                masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
                Debug.Log($"CredentialChecker: Tried FindFirstObjectByType for MasterShipGenerator: {masterShipGenerator != null}");
                
                if (masterShipGenerator == null)
                {
                    Debug.LogError("CredentialChecker: Failed to find MasterShipGenerator. Critical functionality will fail.");
                }
            }
        }
         
                
        // Find ContentManager reference
        if (contentManager == null)
        {
            contentManager = FindFirstObjectByType<StarkkillerContentManager>();
            Debug.Log($"CredentialChecker: Found StarkkillerContentManager: {contentManager != null}");
        }
        
        // Find GameManager reference
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
            Debug.Log($"CredentialChecker: Found GameManager: {gameManager != null}");
        }
        
        // Find GameManagerIntegrationHelper reference
        if (integrationHelper == null)
        {
            integrationHelper = FindFirstObjectByType<GameManagerIntegrationHelper>();
            Debug.Log($"CredentialChecker: Found GameManagerIntegrationHelper: {integrationHelper != null}");
        }
    }
    
    /// <summary>
    /// Retries initializing the MasterShipGenerator reference after a delay
    /// </summary>
    private IEnumerator RetryInitializeGeneratorWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("CredentialChecker: Retrying generator initialization after delay");
        
        // Try to get the generator using the singleton pattern again
        if (masterShipGenerator == null)
        {
            masterShipGenerator = MasterShipGenerator.Instance;
            Debug.Log($"CredentialChecker: Retry found MasterShipGenerator: {masterShipGenerator != null}");
            
            if (masterShipGenerator == null)
            {
                // Try with FindFirstObjectByType as a last resort
                masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
                Debug.Log($"CredentialChecker: Last resort FindFirstObjectByType for MasterShipGenerator: {masterShipGenerator != null}");
            }
        }
        
        // If we found it, subscribe to its event
        if (masterShipGenerator != null)
        {
            // Unsubscribe first to avoid duplicates
            masterShipGenerator.OnEncounterReady -= DisplayEncounter;
            
            // Then subscribe
            masterShipGenerator.OnEncounterReady += DisplayEncounter;
            Debug.Log("CredentialChecker: Successfully subscribed to OnEncounterReady event after retry");
            
            // Hide error message if it was shown
            if (feedbackPanel)
            {
                feedbackPanel.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Requests the initial encounter after a delay
    /// </summary>
    IEnumerator RequestInitialEncounterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Before requesting an encounter, check if we already have one
        if (currentEncounter == null)
        {
            Debug.Log("CredentialChecker: Requesting initial encounter");
            NextEncounter();
            UpdateGameStatus();
        }
        else
        {
            Debug.Log($"CredentialChecker: Already have encounter: {currentEncounter.shipType}");
        }
    }
    
    /// <summary>
    /// Subscribe to video synchronization events from EncounterMediaTransitionManager
    /// </summary>
    private void SubscribeToVideoSyncEvents()
    {
        // Subscribe to reaction video events
        Starkiller.Core.Managers.EncounterMediaTransitionManager.OnReactionVideoStarted += OnReactionVideoStarted;
        Starkiller.Core.Managers.EncounterMediaTransitionManager.OnReactionVideoCompleted += OnReactionVideoCompleted;
        Starkiller.Core.Managers.EncounterMediaTransitionManager.OnNewEncounterReadyForTextUpdate += OnNewEncounterReadyForTextUpdate;
        
        if (verboseDebugLogs)
            Debug.Log("CredentialChecker: Subscribed to video synchronization events");
    }
    
    /// <summary>
    /// Unsubscribe from video synchronization events
    /// </summary>
    private void UnsubscribeFromVideoSyncEvents()
    {
        // Unsubscribe from reaction video events
        Starkiller.Core.Managers.EncounterMediaTransitionManager.OnReactionVideoStarted -= OnReactionVideoStarted;
        Starkiller.Core.Managers.EncounterMediaTransitionManager.OnReactionVideoCompleted -= OnReactionVideoCompleted;
        Starkiller.Core.Managers.EncounterMediaTransitionManager.OnNewEncounterReadyForTextUpdate -= OnNewEncounterReadyForTextUpdate;
        
        if (verboseDebugLogs)
            Debug.Log("CredentialChecker: Unsubscribed from video synchronization events");
    }
    
    /// <summary>
    /// Called when reaction video starts playing - blocks text updates and disables buttons
    /// </summary>
    private void OnReactionVideoStarted()
    {
        isReactionVideoPlaying = true;
        
        // CRITICAL: Disable all decision buttons to prevent clicks during reaction videos
        SetButtonsInteractable(false);
        
        if (verboseDebugLogs)
        {
            Debug.Log("CredentialChecker: Reaction video started - blocking text updates and disabling buttons");
            if (currentEncounter != null)
            {
                Debug.Log($"CredentialChecker: Current encounter during video block: {currentEncounter.shipType} - {currentEncounter.accessCode} - Manifest: {currentEncounter.GetManifestDisplay()}");
            }
        }
    }
    
    /// <summary>
    /// Called when reaction video completes - allows text updates
    /// </summary>
    private void OnReactionVideoCompleted()
    {
        isReactionVideoPlaying = false;
        if (verboseDebugLogs)
            Debug.Log("CredentialChecker: Reaction video completed - allowing text updates");
        
        // If there's a pending text encounter, update now
        if (pendingTextEncounter != null)
        {
            if (verboseDebugLogs)
            {
                Debug.Log($"CredentialChecker: Updating text with pending encounter: {pendingTextEncounter.shipType} - {pendingTextEncounter.accessCode}");
                Debug.Log($"CredentialChecker: Pending encounter manifest: {pendingTextEncounter.GetManifestDisplay()}");
            }
            UpdateEncounterText(pendingTextEncounter);
            pendingTextEncounter = null;
        }
    }
    
    /// <summary>
    /// Called when it's safe to update text to the next encounter
    /// </summary>
    private void OnNewEncounterReadyForTextUpdate()
    {
        if (verboseDebugLogs)
            Debug.Log("CredentialChecker: Ready for text update to next encounter");
        
        // Additional logic can be added here if needed for text coordination
    }
    
    /// <summary>
    /// Update encounter text (credentials and ship info)
    /// </summary>
    private void UpdateEncounterText(MasterShipEncounter encounter)
    {
        if (encounter == null) return;
        
        try
        {
            // Update ship info panel
            if (shipInfoPanel)
            {
                shipInfoPanel.text = encounter.GetShipInfo();
                if (verboseDebugLogs)
                    Debug.Log($"CredentialChecker: Updated ship info text for {encounter.shipType}");
            }
            
            // Update credentials panel (includes manifest)
            if (credentialsPanel)
            {
                string credentialsText = encounter.GetCredentialsInfo();
                credentialsPanel.text = credentialsText;
                
                if (verboseDebugLogs)
                {
                    Debug.Log($"CredentialChecker: Updated credentials text for {encounter.shipType}");
                    Debug.Log($"CredentialChecker: Access Code: {encounter.accessCode}");
                    Debug.Log($"CredentialChecker: Manifest: {encounter.GetManifestDisplay()}");
                    Debug.Log($"CredentialChecker: Manifest Source: {(encounter.manifestData != null ? "ScriptableObject" : "Legacy String")}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CredentialChecker: Error updating encounter text: {e.Message}");
        }
    }
    
    /// <summary>
    /// Update encounter text with video synchronization logic
    /// </summary>
    private void UpdateEncounterTextWithSync(MasterShipEncounter encounter)
    {
        if (encounter == null) return;
        
        // If a reaction video is currently playing, delay the text update
        if (isReactionVideoPlaying)
        {
            if (verboseDebugLogs)
            {
                Debug.Log($"CredentialChecker: Reaction video playing - delaying text update for {encounter.shipType}");
                Debug.Log($"CredentialChecker: New encounter pending - Access Code: {encounter.accessCode}, Manifest: {encounter.GetManifestDisplay()}");
                if (currentEncounter != null)
                {
                    Debug.Log($"CredentialChecker: Current encounter will remain displayed - Access Code: {currentEncounter.accessCode}, Manifest: {currentEncounter.GetManifestDisplay()}");
                }
            }
            pendingTextEncounter = encounter;
            return;
        }
        
        // If no reaction video is playing, update immediately
        if (verboseDebugLogs)
            Debug.Log($"CredentialChecker: No reaction video playing - updating text immediately for {encounter.shipType}");
        UpdateEncounterText(encounter);
    }
    
    /// <summary>
    /// Clean up when destroyed
    /// </summary>
    void OnDestroy()
    {
        try {
            // Unsubscribe from video sync events
            UnsubscribeFromVideoSyncEvents();
            
            // Don't try to access Instance in OnDestroy as it may already be destroyed
            // Just use our cached reference if available
            
            // Unsubscribe from event - ensure we unsubscribe from both references to be safe
            if (masterShipGenerator != null)
            {
                masterShipGenerator.OnEncounterReady -= DisplayEncounter;
                Debug.Log("CredentialChecker: Unsubscribed from masterShipGenerator reference");
            }
            
            // No need to access singleton instance in OnDestroy - it may already be destroyed
            
            // Unregister video players from AudioManager
            if (AudioManager.Instance != null)
            {
                if (shipVideoPlayer != null)
                    AudioManager.Instance.UnregisterVideoPlayer(shipVideoPlayer);
                    
                if (captainVideoPlayer != null)
                    AudioManager.Instance.UnregisterVideoPlayer(captainVideoPlayer);
            }
        }
        catch (System.Exception e) {
            Debug.LogError($"CredentialChecker: Error during cleanup: {e.Message}\n{e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Updates the game status UI with information from GameManager
    /// </summary>
    private void UpdateGameStatus()
    {
        if (gameStatusText == null)
            return;
            
        // Try to use the integration helper first, then fall back to GameManager
        if (integrationHelper != null)
        {
            gameStatusText.text = $"DAY: {integrationHelper.GetCurrentDay()} | " +
                                $"SHIPS: {integrationHelper.GetShipsProcessed()}/8 | " +
                                $"CREDITS: {integrationHelper.GetCredits()} | " +
                                $"STRIKES: {integrationHelper.GetStrikes()}/3";
        }
        else if (gameManager != null)
        {
            // Fallback to GameManager if integration helper not available
            gameStatusText.text = $"DAY: {gameManager.currentDay} | " +
                                $"SHIPS: {gameManager.GetShipsProcessed()}/{gameManager.requiredShipsPerDay} | " +
                                $"CREDITS: {gameManager.GetCredits()} | " +
                                $"STRIKES: {gameManager.GetStrikes()}/{gameManager.maxMistakes}";
        }
        else
        {
            // Fallback if neither found
            gameStatusText.text = "DAY: 1 | SHIPS: 0/8 | CREDITS: 30 | STRIKES: 0/3";
        }
    }

    /// <summary>
    /// Initializes video players and ensures they are enabled
    /// </summary>
    private void InitializeVideoPlayers()
    {
        if (videoPlayersInitialized)
            return;
                
        Debug.Log("Initializing video players...");
        
        try
        {
            // Ensure video players are properly set up
            if (shipVideoPlayer != null)
            {
                shipVideoPlayer.enabled = true;
                shipVideoPlayer.playOnAwake = false;
                shipVideoPlayer.isLooping = true;
                shipVideoPlayer.renderMode = VideoRenderMode.RenderTexture;
                
                // Setup audio output
                shipVideoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
                shipVideoPlayer.controlledAudioTrackCount = 1;
                
                // Register with AudioManager if available
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.RegisterVideoPlayer(shipVideoPlayer);
                }
                
                Debug.Log("Ship video player initialized and enabled");
            }
            else
            {
                Debug.LogWarning("Ship video player is null!");
            }
            
            if (captainVideoPlayer != null)
            {
                captainVideoPlayer.enabled = true;
                captainVideoPlayer.playOnAwake = false;
                captainVideoPlayer.isLooping = true;
                captainVideoPlayer.renderMode = VideoRenderMode.RenderTexture;
                
                // Setup audio output
                captainVideoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
                captainVideoPlayer.controlledAudioTrackCount = 1;
                
                // Register with AudioManager if available
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.RegisterVideoPlayer(captainVideoPlayer);
                }
                
                Debug.Log("Captain video player initialized and enabled");
            }
            else
            {
                Debug.LogWarning("Captain video player is null!");
            }
            
            // Ensure containers are active but initially hidden
            if (shipVideoContainer != null)
            {
                shipVideoContainer.SetActive(false);
            }
            
            if (captainVideoContainer != null)
            {
                captainVideoContainer.SetActive(false);
            }
            
            videoPlayersInitialized = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error initializing video players: {e.Message}");
        }
    }
    
    /// <summary>
    /// Force set the game to active gameplay state (useful for testing)
    /// </summary>
    public void ForceActiveGameplay()
    {
        LogStatus("Forcing game state to active gameplay");
        GameStateController controller = GameStateController.Instance;
        if (controller != null)
        {
            controller.SetGameState(GameStateController.GameActivationState.ActiveGameplay);
        }
        else
        {
            Debug.LogWarning("GameStateController not found - cannot force gameplay state");
        }
        
        // Ensure systems are ready
        if (masterShipGenerator != null)
        {
            // Ensure first encounter is ready
            if (masterShipGenerator != null) {
                masterShipGenerator.GetNextEncounter();
            } else {
                Debug.LogError("CredentialChecker: Cannot get next encounter - masterShipGenerator is null");
            }
        }
        
        // Force UI visibility in case elements are hidden
        ForceUIVisibility();
    }
    
    /// <summary>
    /// Force UI elements to be visible if they were incorrectly hidden
    /// </summary>
    public void ForceUIVisibility()
    {
        LogStatus("Forcing UI elements to be visible");
        
        // Ensure UI containers are active
        if (shipImageContainer) shipImageContainer.SetActive(true);
        if (captainImageContainer) captainImageContainer.SetActive(true);
        if (shipVideoContainer) shipVideoContainer.SetActive(true);
        if (captainVideoContainer) captainVideoContainer.SetActive(true);
        
        // Refresh video players to ensure they're ready
        if (shipVideoPlayer != null)
        {
            shipVideoPlayer.enabled = true;
            if (shipVideoPlayer.clip != null && !shipVideoPlayer.isPlaying)
            {
                shipVideoPlayer.Play();
                LogStatus("Restarted ship video player");
            }
        }
        
        if (captainVideoPlayer != null)
        {
            captainVideoPlayer.enabled = true;
            if (captainVideoPlayer.clip != null && !captainVideoPlayer.isPlaying)
            {
                captainVideoPlayer.Play();
                LogStatus("Restarted captain video player");
            }
        }
        
        // Check for canvas groups and ensure they're visible
        TrySetCanvasGroupAlpha(shipVideoContainer, 1f);
        TrySetCanvasGroupAlpha(captainVideoContainer, 1f);
        TrySetCanvasGroupAlpha(shipImageContainer, 1f);
        TrySetCanvasGroupAlpha(captainImageContainer, 1f);
        
        // Find parent canvas groups that might be affecting visibility
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in allCanvases)
        {
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup != null && canvasGroup.alpha < 1f)
            {
                LogStatus($"Found canvas with low alpha: {canvas.name}, setting to 1");
                canvasGroup.alpha = 1f;
            }
        }
        
        // Enable decision buttons safely
        SafeEnableButtons();
        
        // Reset internal states that might be blocking interactions
        isProcessingDecision = false;
        isInDecisionAnimation = false;
        
        // Also find the main UI panel and ensure it's active
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null && uiManager.gameplayPanel != null)
        {
            uiManager.gameplayPanel.SetActive(true);
            TrySetCanvasGroupAlpha(uiManager.gameplayPanel, 1f);
            LogStatus("Forcing gameplay panel visible via UIManager");
        }
        
        // Clear any active feedback
        if (feedbackPanel) feedbackPanel.SetActive(false);
        
        // Hide stamps if visible
        if (approveStamp) approveStamp.SetActive(false);
        if (denyStamp) denyStamp.SetActive(false);
        
        // Try to trigger a display refresh with current encounter
        if (currentEncounter != null)
        {
            LogStatus("Refreshing display with current encounter");
            DisplayEncounter(currentEncounter);
        }
        else
        {
            // If no encounter, try to find MasterShipGenerator and request a new one
            // ALWAYS use singleton pattern for consistent reference
            MasterShipGenerator generator = MasterShipGenerator.Instance;
            
            if (generator != null)
            {
                masterShipGenerator = generator; // Update the reference in case it was lost
                LogStatus("No current encounter, connecting to MasterShipGenerator and requesting new encounter");
                
                // Connect to generator events
                generator.OnEncounterReady -= DisplayEncounter;
                generator.OnEncounterReady += DisplayEncounter;
                
                // Request new encounter
                ShipTimingController timingController = FindFirstObjectByType<ShipTimingController>();
                if (timingController != null)
                {
                    // Force reset timing controller to ensure we can get a new ship
                    timingController.ResetCooldown();
                }
                
                NextEncounter();
            }
            else
            {
                LogStatus("MasterShipGenerator not found, cannot request new encounter");
                
                // Last resort: create a fallback encounter
                MasterShipEncounter fallback = MasterShipEncounter.CreateTestEncounter();
                DisplayEncounter(fallback);
            }
        }
        
        // Update sync between video audio and game audio
        SyncVideoAudioWithGameAudio();
        
        LogStatus("UI visibility forced - all elements should now be visible");
    }
    
    /// <summary>
    /// Helper to set canvas group alpha on an object if it has one
    /// </summary>
    private void TrySetCanvasGroupAlpha(GameObject container, float alpha)
    {
        if (container == null) return;
        
        CanvasGroup canvasGroup = container.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
            LogStatus($"Set alpha to {alpha} on {container.name}");
        }
    }

    /// <summary>
    /// Log a status message for debugging
    /// </summary>
    private void LogStatus(string message)
    {
        if (verboseDebugLogs)
        {
            Debug.Log($"[CredentialChecker] {message}");
        }
    }

    /// <summary>
    /// Displays the ship encounter data in the UI
    /// </summary>
    public void DisplayEncounter(MasterShipEncounter encounter)
    {
        try
        {
            // Timing Check
            EncounterTimingController timingController = EncounterTimingController.Instance;
            if (timingController != null && !timingController.CanDisplayNewEncounter())
            {
                Debug.LogWarning("CredentialChecker: Cannot display new encounter yet (timing restriction)");
                return;
            }
            
            // Make sure video players are initialized
            if (!videoPlayersInitialized)
            {
                InitializeVideoPlayers();
            }

            // Record the encounter display time for minimum display enforcement
            encounterDisplayTime = Time.time;
            
            // Reset video playback flags for new encounter
            shipVideoStarted = false;
            captainVideoStarted = false;
            
            // Make sure video players are initialized
            if (!videoPlayersInitialized)
            {
                InitializeVideoPlayers();
            }
            
            if (timingController != null)
            {
                timingController.OnEncounterDisplayed(encounter);
            }
            
            // Early exit if encounter is null
            if (encounter == null)
            {
                Debug.LogError("CredentialChecker: Attempt to display null encounter! Creating fallback encounter.");

                // Create a fallback encounter
                encounter = MasterShipEncounter.CreateTestEncounter();

                // Show an error message to the user
                if (feedbackPanel && feedbackText)
                {
                    feedbackText.text = "ERROR: Failed to load ship data. Using test ship.";
                    feedbackText.color = Color.red;
                    feedbackPanel.SetActive(true);

                    // Hide feedback after delay
                    StartCoroutine(HideFeedbackAfterDelay(4f));
                }
            }
            
            // Always force-enable video players before use
            if (shipVideoPlayer != null)
                shipVideoPlayer.enabled = true;

            SyncVideoAudioWithGameAudio();

            // Add after the "Make sure video players are initialized" section
            DebugMonitor debugMonitor = DebugMonitor.Instance;
            if (debugMonitor != null)
            {
                // Get the calling method name using stack trace
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
                string callerMethod = "Unknown";
                
                // Get the method that called DisplayEncounter
                if (stackTrace.FrameCount > 1)
                {
                    var frame = stackTrace.GetFrame(1);
                    if (frame != null && frame.GetMethod() != null)
                    {
                        callerMethod = frame.GetMethod().Name;
                    }
                }
                
                debugMonitor.LogEncounterDisplayed(encounter, callerMethod);
            }

            // Handle captain video/image
            if (captainVideoPlayer != null && encounter.HasCaptainVideo())
            {
                try
                {
                    // Use video if available
                    captainVideoPlayer.clip = encounter.captainVideo;
                    if (captainVideoContainer) captainVideoContainer.SetActive(true);

                    // Update captain dialog text if we have a dialog text component
                    if (captainDialogText != null)
                    {
                        string dialogText = encounter.GetCurrentCaptainDialogText();
                        captainDialogText.text = $"\"{dialogText}\"";
                    }

                    // Explicitly ensure both video player and container are enabled
                    captainVideoPlayer.enabled = true;
                    if (captainVideoContainer) captainVideoContainer.SetActive(true);

                    // Skip old video system when new EncounterMediaTransitionManager is active
                    var mediaTransitionMgr = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
                    if (mediaTransitionMgr == null)
                    {
                        try
                        {
                            // Small delay to ensure the video player is fully ready
                            StartCoroutine(PlayVideoWithDelay(captainVideoPlayer, 0.1f));
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"Error starting captain video playback: {e.Message}");
                        }
                    }
                    else
                    {
                        if (verboseDebugLogs)
                            Debug.Log("CredentialChecker: Skipping old captain video system - using new EncounterMediaTransitionManager");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error playing captain video: {e.Message}");
                    if (captainVideoContainer) captainVideoContainer.SetActive(false);

                    // Try to fall back to image
                    if (captainImageDisplay != null && encounter.HasCaptainPortrait())
                    {
                        captainImageDisplay.sprite = encounter.captainPortrait;
                        if (captainImageContainer) captainImageContainer.SetActive(true);
                    }
                }
            }
            
            // Store the encounter reference
            currentEncounter = encounter;
            
            // Re-enable captain clicks for next encounter
            EnableCaptainClicks();
            
            // Update text panels with synchronization logic
            UpdateEncounterTextWithSync(encounter);
            
            // First, hide all visual containers
            if (shipImageContainer) shipImageContainer.SetActive(false);
            if (captainImageContainer) captainImageContainer.SetActive(false);
            if (shipVideoContainer) shipVideoContainer.SetActive(false);
            if (captainVideoContainer) captainVideoContainer.SetActive(false);
            
            // Handle ship video/image
            if (shipVideoPlayer != null && encounter.HasShipVideo())
            {
                try
                {
                    // Use video if available
                    shipVideoPlayer.clip = encounter.shipVideo;
                    if (shipVideoContainer) shipVideoContainer.SetActive(true);
                    
                    // Explicitly ensure both video player and container are enabled
                    shipVideoPlayer.enabled = true;
                    if (shipVideoContainer) shipVideoContainer.SetActive(true);
                    
                    // Skip old video system when new EncounterMediaTransitionManager is active
                    var mediaTransitionMgr = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
                    if (mediaTransitionMgr == null)
                    {
                        try
                        {
                            // Small delay to ensure the video player is fully ready
                            StartCoroutine(PlayVideoWithDelay(shipVideoPlayer, 0.1f));
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"Error starting ship video playback: {e.Message}");
                        }
                    }
                    else
                    {
                        if (verboseDebugLogs)
                            Debug.Log("CredentialChecker: Skipping old ship video system - using new EncounterMediaTransitionManager");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error playing ship video: {e.Message}");
                    if (shipVideoContainer) shipVideoContainer.SetActive(false);
                    
                    // Try to fall back to image
                    if (shipImageDisplay != null && encounter.HasShipImage())
                    {
                        shipImageDisplay.sprite = encounter.shipImage;
                        if (shipImageContainer) shipImageContainer.SetActive(true);
                    }
                }
            }
            else if (shipImageDisplay != null && encounter.HasShipImage())
            {
                // Fall back to image if no video
                shipImageDisplay.sprite = encounter.shipImage;
                if (shipImageContainer) shipImageContainer.SetActive(true);
            }
            
            // Handle captain video/image
            if (captainVideoPlayer != null && encounter.HasCaptainVideo())
            {
                try
                {
                    // Use video if available
                    captainVideoPlayer.clip = encounter.captainVideo;
                    if (captainVideoContainer) captainVideoContainer.SetActive(true);
                    
                    // Explicitly ensure both video player and container are enabled
                    captainVideoPlayer.enabled = true;
                    if (captainVideoContainer) captainVideoContainer.SetActive(true);
                    
                    // Skip old video system when new EncounterMediaTransitionManager is active
                    var mediaTransitionMgr = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
                    if (mediaTransitionMgr == null)
                    {
                        try
                        {
                            // Small delay to ensure the video player is fully ready
                            StartCoroutine(PlayVideoWithDelay(captainVideoPlayer, 0.1f));
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"Error starting captain video playback: {e.Message}");
                        }
                    }
                    else
                    {
                        if (verboseDebugLogs)
                            Debug.Log("CredentialChecker: Skipping old captain video system - using new EncounterMediaTransitionManager");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error playing captain video: {e.Message}");
                    if (captainVideoContainer) captainVideoContainer.SetActive(false);
                    
                    // Try to fall back to image
                    if (captainImageDisplay != null && encounter.HasCaptainPortrait())
                    {
                        captainImageDisplay.sprite = encounter.captainPortrait;
                        if (captainImageContainer) captainImageContainer.SetActive(true);
                    }
                }
            }
            else if (captainImageDisplay != null && encounter.HasCaptainPortrait())
            {
                // Fall back to image if no video
                captainImageDisplay.sprite = encounter.captainPortrait;
                if (captainImageContainer) captainImageContainer.SetActive(true);
            }
            
            // Show bribe button if ship offers bribe
            if (acceptBribeButton)
            {
                acceptBribeButton.gameObject.SetActive(encounter.offersBribe);
            }
            
            // Show holding pattern button (always available)
            if (holdingPatternButton)
            {
                holdingPatternButton.gameObject.SetActive(true);
            }
            
            // Show tractor beam button only for capturable ships or story ships
            if (tractorBeamButton)
            {
                bool canCapture = encounter.canBeCaptured || 
                                 (encounter.isStoryShip && encounter.storyTag == "insurgent");
                tractorBeamButton.gameObject.SetActive(canCapture);
            }
                
            // Enable buttons for decision (new encounter is ready)
            SafeEnableButtons();
            
            if (verboseDebugLogs)
                Debug.Log("CredentialChecker: New encounter displayed - enabling decision buttons");
            
            // Hide any stamps that might be showing
            if (approveStamp) approveStamp.SetActive(false);
            if (denyStamp) denyStamp.SetActive(false);
            
            // Notify FactCheckManager about the new encounter
            FactCheckManager factCheckManager = FindFirstObjectByType<FactCheckManager>();
            if (factCheckManager != null)
            {
                factCheckManager.SetupFactCheckAreas(encounter);
            }

            // Reset encounter decision state
            encounter.playerDecision = MasterShipEncounter.DecisionState.None;
            encounter.isInHoldingPattern = false;
            
            // Update game status bar
            UpdateGameStatus();
            
            // Tell the new EncounterMediaTransitionManager about this encounter
            var encounterMediaMgr = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
            if (encounterMediaMgr != null)
            {
                encounterMediaMgr.PrepareNextEncounter(encounter);
                if (verboseDebugLogs)
                    Debug.Log("CredentialChecker: Notified new EncounterMediaTransitionManager of encounter");
            }
            
            Debug.Log($"CredentialChecker: Successfully displayed encounter for ship: {encounter.shipType}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CredentialChecker: Error displaying encounter: {e.Message}\n{e.StackTrace}");
            
            // Try to recover by creating a fallback encounter and displaying it
            MasterShipEncounter fallback = MasterShipEncounter.CreateTestEncounter();
            currentEncounter = fallback;
            
            // Create a simple fallback display
            if (shipInfoPanel)
                shipInfoPanel.text = "ERROR: Failed to display encounter.\n\nEmergency Test Ship\nCaptain: Test\n\nRequesting emergency clearance.";
                
            if (credentialsPanel)
                credentialsPanel.text = "Access Code: TEST-1234\n\nManifest: Emergency supplies.";
            
            // Hide all visual elements in case of error
            if (shipImageContainer) shipImageContainer.SetActive(false);
            if (captainImageContainer) captainImageContainer.SetActive(false);
            if (shipVideoContainer) shipVideoContainer.SetActive(false);
            if (captainVideoContainer) captainVideoContainer.SetActive(false);
            
            // Enable basic buttons
            SafeEnableButtons();
            
            // Hide stamps
            if (approveStamp) approveStamp.SetActive(false);
            if (denyStamp) denyStamp.SetActive(false);
            
            // Show error to user
            ShowFeedback("ERROR: Problem displaying ship data", Color.red);
        }
    }
    
    /// <summary>
    /// Safely display an encounter with additional checks and delay to prevent race conditions
    /// </summary>
    public void DisplayEncounterSafe(MasterShipEncounter encounter)
    {
        if (encounter == null)
        {
            Debug.LogWarning("CredentialChecker: Attempted to display null encounter");
            return;
        }
        
        // Add a small delay to prevent race conditions
        StartCoroutine(DisplayEncounterDelayed(encounter, 0.1f));
    }

    /// <summary>
    /// Display an encounter after a delay
    /// </summary>
    private IEnumerator DisplayEncounterDelayed(MasterShipEncounter encounter, float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayEncounter(encounter);
    }

    /// <summary>
    /// Overloaded method to support legacy ShipEncounter types
    /// </summary>
    public void DisplayEncounter(ShipEncounter oldEncounter)
    {
        // Convert legacy encounter to master encounter
        MasterShipEncounter masterEncounter = MasterShipEncounter.FromLegacyEncounter(oldEncounter);

        // Display the master encounter
        DisplayEncounter(masterEncounter);
    }
    
    /// <summary>
    /// Overloaded method to support legacy EnhancedShipEncounter types
    /// </summary>
    public void DisplayEncounter(EnhancedShipEncounter oldEnhancedEncounter)
    {
        // Convert legacy enhanced encounter to master encounter
        MasterShipEncounter masterEncounter = MasterShipEncounter.FromLegacyEncounter(oldEnhancedEncounter);
        
        // Display the master encounter
        DisplayEncounter(masterEncounter);
    }
    
    /// <summary>
    /// Overloaded method to support legacy VideoEnhancedShipEncounter types
    /// </summary>
    public void DisplayEncounter(VideoEnhancedShipEncounter oldVideoEncounter)
    {
        // Convert legacy video encounter to master encounter
        MasterShipEncounter masterEncounter = MasterShipEncounter.FromLegacyEncounter(oldVideoEncounter);
        
        // Display the master encounter
        DisplayEncounter(masterEncounter);
    }
    
    /// <summary>
    /// Called when approve button is clicked
    /// </summary>
    public void OnApproveClicked()
    {
        // Disable captain clicks during decision
        DisableCaptainClicks();

        // Check minimum display time requirement
        if (!CanProcessDecision()) return;
        
        // Prevent multiple clicks during processing or animation
        if (isProcessingDecision || isInDecisionAnimation)
        {
            if (verboseDebugLogs)
                Debug.Log("CredentialChecker: Already processing a decision or in animation, ignoring approve click");
            return;
        }
        
        // Check if game state allows processing
        if (!IsGameStateReadyForProcessing())
        {
            return;
        }

        // Verify we have a valid encounter
        if (currentEncounter == null)
        {
            Debug.LogError("CredentialChecker: Trying to approve with no active encounter!");
            
            // Show user feedback
            ShowFeedback("ERROR: No ship to process", Color.red);
            if (errorSound) errorSound.Play();
            
            // Try to get a new encounter
            NextEncounter();
            return;
        }
        
        // Debug logging of the current encounter being approved
        if (verboseDebugLogs)
        {
            Debug.Log($"APPROVE clicked for ship: {currentEncounter.shipType}, " +
                      $"code: {currentEncounter.accessCode}, " +
                      $"shouldApprove: {currentEncounter.shouldApprove}");
        }
        
        ProcessDecision(true);
        
        // Hide ID card after decision is processed
        if (captainIDCard != null)
        {
            captainIDCard.HideIDCard();
        }
    }
    
    /// <summary>
    /// Called when deny button is clicked
    /// </summary>
    public void OnDenyClicked()
    {
        // Disable captain clicks during decision
        DisableCaptainClicks();

        // Check minimum display time requirement
        if (!CanProcessDecision()) return;
        
        // Prevent multiple clicks during processing or animation
        if (isProcessingDecision || isInDecisionAnimation)
        {
            if (verboseDebugLogs)
                Debug.Log("CredentialChecker: Already processing a decision or in animation, ignoring deny click");
            return;
        }
        
        // Check if game state allows processing
        if (!IsGameStateReadyForProcessing())
        {
            return;
        }

        // Verify we have a valid encounter
        if (currentEncounter == null)
        {
            Debug.LogError("CredentialChecker: Trying to deny with no active encounter!");
            
            // Show user feedback
            ShowFeedback("ERROR: No ship to process", Color.red);
            if (errorSound) errorSound.Play();
            
            // Try to get a new encounter
            NextEncounter();
            return;
        }
        
        // Debug logging of the current encounter being denied
        if (verboseDebugLogs)
        {
            Debug.Log($"DENY clicked for ship: {currentEncounter.shipType}, " +
                      $"code: {currentEncounter.accessCode}, " +
                      $"shouldApprove: {currentEncounter.shouldApprove}");
        }
        
        ProcessDecision(false);
        
        // Hide ID card after decision is processed
        if (captainIDCard != null)
        {
            captainIDCard.HideIDCard();
        }
    }
    
    /// <summary>
    /// Called when accept bribe button is clicked
    /// </summary>
    void OnAcceptBribeClicked()
    {
        // Disable captain clicks during decision
        DisableCaptainClicks();

        // Prevent multiple clicks during processing or animation
        if (isProcessingDecision || isInDecisionAnimation)
        {
            if (verboseDebugLogs)
                Debug.Log("CredentialChecker: Already processing a decision or in animation, ignoring bribe click");
            return;
        }
        
        // Set processing lock
        isProcessingDecision = true;
        
        if (currentEncounter == null)
        {
            Debug.LogError("CredentialChecker: Trying to accept bribe with no active encounter!");
            
            // Show user feedback
            ShowFeedback("ERROR: No ship to process", Color.red);
            if (errorSound) errorSound.Play();
            
            // Try to get a new encounter
            NextEncounter();
            
            isProcessingDecision = false; // Release lock
            return;
        }
        
        if (!currentEncounter.offersBribe) 
        {
            Debug.LogWarning("CredentialChecker: Ship does not offer bribe!");
            
            // Show user feedback
            ShowFeedback("ERROR: This ship does not offer a bribe", Color.red);
            if (errorSound) errorSound.Play();
            
            isProcessingDecision = false; // Release lock
            return;
        }
        
        // Debug logging
        if (verboseDebugLogs)
        {
            Debug.Log($"BRIBE ACCEPTED for ship: {currentEncounter.shipType}, " +
                    $"bribe amount: {currentEncounter.bribeAmount}");
        }
        
        // Disable buttons to prevent multiple clicks
        SetButtonsInteractable(false);
        
        // Hide the bribe button
        if (acceptBribeButton) acceptBribeButton.gameObject.SetActive(false);
        
        // Play bribe sound
        if (bribeSound) bribeSound.Play();
        
        // Create a local copy of the current encounter to ensure we process the correct one
        MasterShipEncounter bribeEncounter = currentEncounter;
        
        // Show feedback
        ShowFeedback($"Accepted {bribeEncounter.bribeAmount} credits bribe", Color.yellow);
        
        // Play bribe reaction video
        var mediaTransitionManager = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
        if (mediaTransitionManager == null)
        {
            mediaTransitionManager = FindFirstObjectByType<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
        }
        
        if (mediaTransitionManager != null)
        {
            mediaTransitionManager.PrepareNextEncounter(bribeEncounter);
            mediaTransitionManager.ShowCaptainReactionVideo(true, true); // approved=true, bribery=true
            Debug.Log("CredentialChecker: Triggered bribery reaction video");
        }
        else
        {
            Debug.LogWarning("CredentialChecker: Could not find EncounterMediaTransitionManager for bribery reaction");
        }
        
        // Set decision state
        bribeEncounter.playerDecision = MasterShipEncounter.DecisionState.BriberyAccepted;
        
        // Notify GameManager of bribe acceptance via integration helper
        if (integrationHelper != null)
        {
            integrationHelper.AddCredits(bribeEncounter.bribeAmount, "Bribe accepted");
        }
        else if (gameManager != null)
        {
            gameManager.OnAcceptBribeClicked();
        }
        
        // Record bribe decision using existing methods
        if (NarrativeManager.Instance != null)
        {
            string context = $"Accepted bribe of {bribeEncounter.bribeAmount} credits";
            NarrativeManager.Instance.RecordDecision(
                "bribe_accepted",
                -5, // Imperial loyalty decreases
                10, // Insurgent sympathy increases
                context
            );
        }

        // Trigger captain's approval reaction video for bribery acceptance
        var briberyMediaManager = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
        if (briberyMediaManager != null)
        {
            if (verboseDebugLogs)
                Debug.Log("CredentialChecker: Triggering approval reaction video for bribe acceptance");
            
            // Show captain reaction video - true for approved, true for bribery
            briberyMediaManager.ShowCaptainReactionVideo(true, true);
        }
        else
        {
            Debug.LogWarning("CredentialChecker: EncounterMediaTransitionManager not found - cannot play bribery reaction video");
        }

        // Clear current encounter
        currentEncounter = null;
        
        // Notify the generator of the bribe acceptance (which will auto-approve)
        if (masterShipGenerator != null && bribeEncounter != null)
        {
            masterShipGenerator.ProcessBriberyAccepted(bribeEncounter);
        }
        
        // Release the processing lock
        isProcessingDecision = false;
        
        // Hide ID card after decision is processed
        if (captainIDCard != null)
        {
            captainIDCard.HideIDCard();
        }
        
        // Request next encounter after a delay (bribery needs special handling)
        StartCoroutine(RequestNextEncounterAfterBribery());
    }
    
    /// <summary>
    /// Called when holding pattern button is clicked
    /// </summary>
    public void OnHoldingPatternClicked()
    {
        // Disable captain clicks during decision
        DisableCaptainClicks();

        // Prevent multiple clicks during processing or animation
        if (isProcessingDecision || isInDecisionAnimation)
        {
            if (verboseDebugLogs)
                Debug.Log("CredentialChecker: Already processing a decision or in animation, ignoring holding pattern click");
            return;
        }
        
        // Set processing lock
        isProcessingDecision = true;
        
        if (currentEncounter == null) 
        {
            Debug.LogError("CredentialChecker: Trying to put in holding pattern with no active encounter!");
            
            // Show user feedback
            ShowFeedback("ERROR: No ship to place in holding pattern", Color.red);
            if (errorSound) errorSound.Play();
            
            // Try to get a new encounter
            NextEncounter();
            
            isProcessingDecision = false; // Release lock
            return;
        }
        
        // Debug logging
        if (verboseDebugLogs)
        {
            Debug.Log($"HOLDING PATTERN for ship: {currentEncounter.shipType}, " +
                    $"hold time: {currentEncounter.holdingPatternTime}s");
        }

        // Check if the ship is already in a holding pattern
        Debug.Log($"Holding Pattern clicked for ship: {currentEncounter.shipType}");
        
        // Create a local copy of the current encounter to ensure we process the correct one
        MasterShipEncounter encounterToProcess = currentEncounter;
        
        // Disable buttons to prevent multiple clicks
        SetButtonsInteractable(false);
        
        // Play holding pattern sound
        if (holdingPatternSound) holdingPatternSound.Play();
        
        // Show feedback
        ShowFeedback($"Ship placed in holding pattern for {encounterToProcess.holdingPatternTime}s", Color.cyan);
        
        // Set decision state
        encounterToProcess.playerDecision = MasterShipEncounter.DecisionState.HoldingPattern;
        encounterToProcess.isInHoldingPattern = true;
        
        // Start the animation
        isInDecisionAnimation = true;
        if (holdingPattern) 
        {
            StartCoroutine(ShowStamp(holdingPattern));
        }
        
        // Clear current encounter reference before notifying generator
        MasterShipEncounter holdingPatternEncounter = currentEncounter;
        currentEncounter = null;
        
        // Notify ship generator - THIS IS THE IMPORTANT PART
        if (masterShipGenerator != null && holdingPatternEncounter != null)
        {
            masterShipGenerator.ProcessHoldingPattern(holdingPatternEncounter);
        }
        
        // Release animation lock after delay and trigger next encounter
        StartCoroutine(ReleaseAnimationLockAfterDelay(1.5f));
        // Wait longer for holding pattern response video to complete before loading next encounter
        StartCoroutine(HandlePostHoldingPatternFlow(6.0f));
        
        // Release the processing lock
        isProcessingDecision = false;
        
        // Hide ID card after decision is processed
        if (captainIDCard != null)
        {
            captainIDCard.HideIDCard();
        }
    }

    /// <summary>
    /// Called when tractor beam button is clicked
    /// </summary>
    void OnTractorBeamClicked()
    {
        // Disable captain clicks during decision
        DisableCaptainClicks();

        // Prevent multiple clicks during processing or animation
        if (isProcessingDecision || isInDecisionAnimation)
        {
            if (verboseDebugLogs)
                Debug.Log("CredentialChecker: Already processing a decision or in animation, ignoring tractor beam click");
            return;
        }
        
        // Set processing lock
        isProcessingDecision = true;
        
        if (currentEncounter == null) 
        {
            Debug.LogError("CredentialChecker: Trying to use tractor beam with no active encounter!");
            
            // Show user feedback
            ShowFeedback("ERROR: No ship to capture with tractor beam", Color.red);
            if (errorSound) errorSound.Play();
            
            // Try to get a new encounter
            NextEncounter();
            
            isProcessingDecision = false; // Release lock
            return;
        }
        
        // Create a local copy of the current encounter to ensure we process the correct one
        MasterShipEncounter encounterToProcess = currentEncounter;
        
        // Check if this ship can be captured
        bool canCapture = encounterToProcess.canBeCaptured || 
                         (encounterToProcess.isStoryShip && encounterToProcess.storyTag == "insurgent");
        
        // Debug logging
        if (verboseDebugLogs)
        {
            Debug.Log($"TRACTOR BEAM for ship: {encounterToProcess.shipType}, " +
                     $"can be captured: {canCapture}");
        }
                         
        if (!canCapture)
        {
            // Show error feedback if ship cannot be captured
            ShowFeedback("ERROR: Ship cannot be captured with tractor beam", Color.red);
            if (errorSound) errorSound.Play();
            isProcessingDecision = false; // Release lock
            return;
        }
        
        // Disable buttons to prevent multiple clicks
        SetButtonsInteractable(false);
        
        // Play tractor beam sound
        if (tractorBeamSound) tractorBeamSound.Play();
        
        // Show feedback
        ShowFeedback("Ship captured with tractor beam", new Color(0.4f, 0.8f, 1f)); // Light blue
        
        // Set decision state
        encounterToProcess.playerDecision = MasterShipEncounter.DecisionState.TractorBeam;
        
        // Start the animation
        isInDecisionAnimation = true;
        if (tractorBeam) 
        {
            StartCoroutine(ShowStamp(tractorBeam));
        }
        
        // Clear current encounter reference before notifying generator
        MasterShipEncounter tractorBeamEncounter = currentEncounter;
        currentEncounter = null;
        
        // Notify ship generator
        if (masterShipGenerator != null && tractorBeamEncounter != null)
        {
            masterShipGenerator.ProcessTractorBeam(tractorBeamEncounter);
        }
        
        // Record tactical decision using existing methods
        if (NarrativeManager.Instance != null)
        {
            string context = $"Used tractor beam on {encounterToProcess.shipType}";
            NarrativeManager.Instance.RecordDecision(
                "tractor_beam_use",
                5, // Imperial loyalty increases (showing force)
                -5, // Insurgent sympathy decreases
                context
            );
        }

        // Release animation lock after delay
        StartCoroutine(ReleaseAnimationLockAfterDelay(1.5f));
        
        // Release the processing lock
        isProcessingDecision = false;
        
        // Hide ID card after decision is processed
        if (captainIDCard != null)
        {
            captainIDCard.HideIDCard();
        }
    }
    
    /// <summary>
    /// Process the player's decision
    /// </summary>
    void ProcessDecision(bool approved)
    {
        // Check if gameplay is active
        GameStateController controller = GameStateController.Instance;
        if (controller != null && !controller.IsGameplayActive())
        {
            Debug.Log("CredentialChecker: Game not in active gameplay state, ignoring decision");
            return;
        }
        
        // Check if game state allows processing
        if (!IsGameStateReadyForProcessing())
        {
            return;
        }

        // Prevent multiple processing attempts or during animation
        if (isProcessingDecision || isInDecisionAnimation)
        {
            Debug.LogWarning("CredentialChecker: Already processing a decision or in animation");
            return;
        }
        
        // IMMEDIATELY make a safe local copy and clear the current reference
        MasterShipEncounter encounterToProcess = currentEncounter;
        currentEncounter = null; // Clear immediately to prevent double processing
        
        // Now check if we had a valid encounter to process
        if (encounterToProcess == null)
        {
            Debug.LogWarning("CredentialChecker: No active encounter to process decision!");
            
            // Try to get a new encounter
            NextEncounter();
            
            // Release the locks
            isProcessingDecision = false;
            isInDecisionAnimation = false;
            return;
        }
        
        // Set the processing lock to prevent race conditions
        isProcessingDecision = true;
        // Start animation lock
        isInDecisionAnimation = true;
        
        // IMMEDIATELY disable buttons to prevent multiple clicks
        SetButtonsInteractable(false);
        
        // Debug logging for decision processing
        if (verboseDebugLogs)
        {
            Debug.Log($"Processing {(approved ? "APPROVE" : "DENY")} decision for " +
                    $"ship: {encounterToProcess.shipType}, " +
                    $"code: {encounterToProcess.accessCode}, " +
                    $"shouldApprove: {encounterToProcess.shouldApprove}");
        }
        
        // Special handling for access codes with invalid prefixes (OLD-, XX-, etc.)
        bool hasInvalidPrefix = encounterToProcess.HasInvalidAccessCodePrefix();
        
        // Check for contraband using the new ManifestManager system
        bool hasContraband = false;
        if (ManifestManager.Instance != null && contentManager != null)
        {
            hasContraband = ManifestManager.Instance.CheckForContraband(
                encounterToProcess.manifestData, 
                contentManager.currentDayRules
            );
        }
        else
        {
            // Fallback to the encounter's own contraband detection
            hasContraband = encounterToProcess.CheckForContraband();
        }
        
        // Override shouldApprove if the access code has an invalid prefix or contraband is detected
        bool shouldApprove = encounterToProcess.shouldApprove;
        if (hasInvalidPrefix)
        {
            shouldApprove = false;
            
            // Update the invalid reason if it's not set yet
            if (string.IsNullOrEmpty(encounterToProcess.invalidReason))
            {
                encounterToProcess.invalidReason = "Invalid access code prefix";
            }
            
            // Add debug logging for invalid prefix detection
            if (verboseDebugLogs)
            {
                Debug.Log($"Invalid access code prefix detected: {encounterToProcess.accessCode} - Will be denied");
            }
        }
        else if (hasContraband)
        {
            shouldApprove = false;
            
            // Update the invalid reason for contraband
            if (string.IsNullOrEmpty(encounterToProcess.invalidReason))
            {
                encounterToProcess.invalidReason = "Contraband detected in manifest";
            }
            
            // Add debug logging for contraband detection
            if (verboseDebugLogs)
            {
                Debug.Log($"Contraband detected in manifest for ship: {encounterToProcess.shipType} - Will be denied");
            }
        }
        
        bool correctDecision = (approved == shouldApprove);
        
        // Log the decision for debugging
        Debug.Log($"=== [DECISION] CredentialChecker.ProcessDecision() - Approved: {approved}, Correct: {correctDecision} ===");
        
        // Show appropriate stamp
        if (approved && approveStamp)
        {
            StartCoroutine(ShowStamp(approveStamp));
        }
        else if (!approved && denyStamp)
        {
            StartCoroutine(ShowStamp(denyStamp));
        }

        // Add narrative tracking
        if (encounterToProcess.isStoryShip && NarrativeManager.Instance != null)
        {
            string context = $"{(approved ? "Approved" : "Denied")} {encounterToProcess.shipType} from {encounterToProcess.captainName}";
            int imperialPoints = 0;
            int insurgentPoints = 0;
            
            // Calculate points based on decision and story tag
            if (encounterToProcess.storyTag == "imperium")
            {
                imperialPoints = approved ? 10 : -5;
                insurgentPoints = approved ? -5 : 5;
            }
            else if (encounterToProcess.storyTag == "insurgent")
            {
                imperialPoints = approved ? -5 : 10;
                insurgentPoints = approved ? 10 : -5;
            }
            
            NarrativeManager.Instance.RecordDecision(
                $"ship_decision_{encounterToProcess.shipType}_{approved}",
                imperialPoints,
                insurgentPoints,
                context
            );
        }

        // Play reaction video based on decision
        var transitionManager = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
        if (transitionManager == null)
        {
            transitionManager = FindFirstObjectByType<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
        }
        
        if (transitionManager != null && encounterToProcess != null)
        {
            // Prepare the encounter for the transition manager
            transitionManager.PrepareNextEncounter(encounterToProcess);
            transitionManager.ShowCaptainReactionVideo(approved, false);
            Debug.Log($"CredentialChecker: Triggered {(approved ? "approval" : "denial")} reaction video");
        }
        else
        {
            Debug.LogWarning("CredentialChecker: Could not find EncounterMediaTransitionManager for reaction video");
        }

        // Show appropriate feedback
        if (correctDecision)
        {
            // For denied ships with invalid prefixes or contraband, provide specific feedback
            if (!approved && hasInvalidPrefix)
            {
                ShowFeedback($"Ship Denied - Correct Decision (Invalid code: {encounterToProcess.accessCode})", Color.green);
            }
            else if (!approved && hasContraband)
            {
                ShowFeedback("Ship Denied - Correct Decision (Contraband detected)", Color.green);
            }
            else
            {
                ShowFeedback(approved ? "Ship Approved - Correct Decision" : "Ship Denied - Correct Decision", Color.green);
            }
            
            // Enhanced narrative tracking with categories
            if (NarrativeManager.Instance != null && encounterToProcess != null)
            {
                NarrativeManager.Instance.RecordShipDecision(encounterToProcess, approved);
            }

            // Play appropriate sound
            if (approved && approveSound)
                approveSound.Play();
            else if (!approved && denySound)
                denySound.Play();
        }
        else
        {
            // Incorrect decision
            string errorMessage = "ERROR: ";
            
            if (approved)
            {
                // Incorrectly approved - check if it was an invalid prefix or contraband issue
                if (hasInvalidPrefix)
                {
                    errorMessage += $"Access code {encounterToProcess.accessCode} is invalid";
                }
                else if (hasContraband)
                {
                    errorMessage += "Contraband detected in manifest";
                }
                else
                {
                    errorMessage += encounterToProcess.invalidReason;
                }
            }
            else
            {
                // Incorrectly denied
                errorMessage += "Valid ship denied without cause";
            }
            
            ShowFeedback(errorMessage, Color.red);
            
            // Record a consequence in the ConsequenceManager if decision was incorrect
            ConsequenceManager consequenceManager = FindFirstObjectByType<ConsequenceManager>();
            if (consequenceManager != null && encounterToProcess != null)
            {
                consequenceManager.RecordIncident(encounterToProcess, approved);
            }

            // Play error sound
            if (errorSound)
                errorSound.Play();
        }
        
        // Set the decision state before processing
        if (approved)
            encounterToProcess.playerDecision = MasterShipEncounter.DecisionState.Approved;
        else
            encounterToProcess.playerDecision = MasterShipEncounter.DecisionState.Denied;
        
        // IMPORTANT: Record the decision through integration helper first
        if (integrationHelper != null)
        {
            Debug.Log("Notifying integration helper of decision before notifying ship generator");
            if (correctDecision)
            {
                integrationHelper.RecordCorrectDecision(approved, "Ship decision");
            }
            else
            {
                integrationHelper.RecordWrongDecision(approved, "Ship decision");
            }
            integrationHelper.RecordShipProcessed();
            Debug.Log("Notifying GameManagerIntegrationHelper of ship processing");
        }
        
        // Also notify GameManager for backward compatibility
        if (gameManager != null)
        {
            Debug.Log("Notifying GameManager of decision before notifying ship generator");
            gameManager.OnDecisionMade(approved, correctDecision);
        }
        
        // Then update the game status UI
        UpdateGameStatus();
        
        // Trigger the GameEvents.OnDecisionMade event so DayProgressionManager can track ships
        // We need to wrap MasterShipEncounter in an IEncounter adapter
        var decisionType = approved ? Starkiller.Core.DecisionType.Approve : Starkiller.Core.DecisionType.Deny;
        var encounterWrapper = new MasterShipEncounterWrapper(encounterToProcess);
        Starkiller.Core.GameEvents.TriggerDecisionMade(decisionType, encounterWrapper);
        Debug.Log($"[CredentialChecker] Triggered GameEvents.OnDecisionMade with decision: {decisionType}");
        
        // Notify ship generator about the decision using our safe helper method
        SafelyNotifyShipGenerator(approved, encounterToProcess);

        // Release animation lock after a delay
        StartCoroutine(ReleaseAnimationLockAfterDelay(1.5f));
        
        // Force a slight delay before allowing new decisions
        StartCoroutine(ReleaseProcessingLockAfterDelay(0.5f));
    }
    
    /// <summary>
    /// Show feedback to the player
    /// </summary>
    public void ShowFeedback(string message, Color color)
    {
        if (feedbackPanel && feedbackText)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            feedbackPanel.SetActive(true);
            
            // Hide feedback after delay
            StartCoroutine(HideFeedbackAfterDelay(2f));
        }
    }
    
    /// <summary>
    /// Show a stamp with animation and hide after delay
    /// </summary>
    IEnumerator ShowStamp(GameObject stamp)
    {
        if (stamp == null) yield break;
        
        // Show the stamp
        stamp.SetActive(true);
        
        // Animate it (simple scale animation)
        RectTransform rt = stamp.GetComponent<RectTransform>();
        if (rt)
        {
            // Start small
            rt.localScale = Vector3.zero;
            
            // Grow to normal size
            float duration = 0.3f;
            float startTime = Time.time;
            
            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return null;
            }
            
            // Ensure it ends at exactly scale 1
            rt.localScale = Vector3.one;
        }
        
        // Wait a bit before hiding
        yield return new WaitForSeconds(1.5f);
        
        // Hide the stamp
        stamp.SetActive(false);
    }
    
    /// <summary>
    /// Hide feedback panel after delay
    /// </summary>
    IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedbackPanel)
            feedbackPanel.SetActive(false);
    }
    
    /// <summary>
    /// Release the processing lock after a delay
    /// </summary>
    private IEnumerator ReleaseProcessingLockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Release the processing lock
        isProcessingDecision = false;
        
        if (verboseDebugLogs)
        {
            Debug.Log("Processing lock released after delay");
        }
    }
    
    /// <summary>
    /// Release the animation lock after a delay
    /// </summary>
    private IEnumerator ReleaseAnimationLockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Release the animation lock
        isInDecisionAnimation = false;
        
        // Re-enable buttons after holding pattern animation
        SafeEnableButtons();
        
        if (verboseDebugLogs)
        {
            Debug.Log("Animation lock released after delay");
        }
    }

    /// <summary>
    /// Handle the transition flow after placing a ship in holding pattern
    /// </summary>
    private IEnumerator HandlePostHoldingPatternFlow(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Show brief feedback message about moving to holding pattern
        ShowFeedback("Ship moved to holding pattern. Processing next encounter...", Color.cyan);
        
        // Wait a moment for the feedback to be visible
        yield return new WaitForSeconds(1.0f);
        
        // Request the next encounter to display
        NextEncounter();
        
        if (verboseDebugLogs)
        {
            Debug.Log("Post-holding pattern flow complete, next encounter requested");
        }
    }
    
    /// <summary>
    /// Request next encounter after bribery acceptance
    /// </summary>
    private IEnumerator RequestNextEncounterAfterBribery()
    {
        // Wait for the reaction video or animation to complete
        yield return new WaitForSeconds(3.0f);
        
        // Check if EncounterMediaTransitionManager is handling auto-advance
        var mediaTransitionManager = FindFirstObjectByType<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
        if (mediaTransitionManager != null)
        {
            // If auto-advance is enabled in the media manager, let it handle the transition
            // This prevents duplicate encounter requests
            Debug.Log("CredentialChecker: EncounterMediaTransitionManager detected with auto-advance - skipping manual request");
            yield break;
        }
        
        Debug.Log("CredentialChecker: Requesting next encounter after bribery acceptance (no auto-advance detected)");
        
        // Try GameManager first
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.RequestNextEncounter("CredentialChecker-PostBribery");
            yield break;
        }
        
        // Fallback to direct NextEncounter
        NextEncounter();
    }

    /// <summary>
    /// Play a video after a short delay to ensure it's ready
    /// </summary>
    private IEnumerator PlayVideoWithDelay(VideoPlayer videoPlayer, float delay)
    {
        // Check if this video player has already been started for this encounter
        if (videoPlayer == shipVideoPlayer && shipVideoStarted) yield break;
        if (videoPlayer == captainVideoPlayer && captainVideoStarted) yield break;
        
        yield return new WaitForSeconds(delay);
        
        if (videoPlayer != null)
        {
            // Set flags to prevent duplicate starts
            if (videoPlayer == shipVideoPlayer) shipVideoStarted = true;
            if (videoPlayer == captainVideoPlayer) captainVideoStarted = true;
            
            // Double-check enabled state
            videoPlayer.enabled = true;
            
            // Ensure audio state is correct
            SyncVideoAudioWithGameAudio();
            
            try
            {
                videoPlayer.Play();
                
                // Log success
                if (verboseDebugLogs)
                {
                    Debug.Log($"Successfully started video playback: {videoPlayer.clip?.name}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error playing video after delay: {e.Message}");
            }
        }
    }
    
    /// <summary>
    /// Set mute state for all video players
    /// </summary>
    public void SetVideoPlayersMute(bool mute)
    {
        if (shipVideoPlayer != null)
        {
            shipVideoPlayer.SetDirectAudioMute(0, mute);
        }
        
        if (captainVideoPlayer != null)
        {
            captainVideoPlayer.SetDirectAudioMute(0, mute);
        }
    }

    /// <summary>
    /// Sync video player audio state with game audio
    /// </summary>
    public void SyncVideoAudioWithGameAudio()
    {
        // Check if game audio is muted
        bool audioMuted = false;
        
        // Try to get audio state from a manager
        GameStateController controller = GameStateController.Instance;
        if (controller != null)
        {
            audioMuted = !controller.IsGameplayActive() || controller.IsAudioMuted();
        }
        
        // Set mute state on video players
        SetVideoPlayersMute(audioMuted);
    }

    /// <summary>
    /// Update the log book content with current day's rules
    /// </summary>
    public void UpdateLogBook()
    {
        if (logBookContent == null)
        {
            Debug.LogWarning("CredentialChecker: logBookContent is null, cannot update log book");
            return;
        }
        
        // Set default content with fallback
        string content = "<b>DAILY LOG BOOK</b>\n\n";
        
        // Get data from content manager if available
        if (contentManager != null)
        {
            // Add valid access codes
            content += "<b>VALID ACCESS CODES:</b>\n";
            if (contentManager.currentAccessCodes != null && contentManager.currentAccessCodes.Count > 0)
            {
                foreach (string code in contentManager.currentAccessCodes)
                {
                    content += "- " + code + "\n";
                }
            }
            else
            {
                content += "- No valid access codes defined\n";
            }
            
            // Add current day rules
            content += "\n<b>CURRENT DAY RULES:</b>\n";
            if (contentManager.currentDayRules != null && contentManager.currentDayRules.Count > 0)
            {
                foreach (string rule in contentManager.currentDayRules)
                {
                    if (rule != null)
                        content += "- " + rule + "\n";
                }
            }
            else
            {
                content += "- No special rules for today\n";
            }
            
            // Add approved ship types
            content += "\n<b>APPROVED SHIP TYPES:</b>\n";
            if (contentManager.shipTypes != null && contentManager.shipTypes.Count > 0)
            {
                bool anyShipsAdded = false;
                
                foreach (var type in contentManager.shipTypes)
                {
                    if (type == null) continue;
                    
                    anyShipsAdded = true;
                    content += "- " + type.typeName + "\n";
                    
                    // Add common origins for this ship type
                    if (type.commonOrigins != null && type.commonOrigins.Length > 0)
                    {
                        content += "  Approved origins: ";
                        for (int i = 0; i < type.commonOrigins.Length; i++)
                        {
                            content += type.commonOrigins[i];
                            if (i < type.commonOrigins.Length - 1)
                                content += ", ";
                        }
                        content += "\n";
                    }
                }
                
                if (!anyShipsAdded)
                {
                    content += "- No ship types defined\n";
                }
            }
            else
            {
                content += "- No ship types defined\n";
            }
        }
        else
        {
            // Fallback if no content manager
            content += "Unable to load daily rules.\nPlease ensure Content Manager is available.";
        }
        
        // Set the text content
        logBookContent.text = content;
    }
    
    /// <summary>
    /// Open the log book
    /// </summary>
    void OpenLogBook()
    {
        if (logBookPanel)
            logBookPanel.SetActive(true);
    }
    
    /// <summary>
    /// Close the log book
    /// </summary>
    void CloseLogBook()
    {
        if (logBookPanel)
            logBookPanel.SetActive(false);
    }

    /// <summary>
    /// Check if enough time has passed to process a decision
    /// </summary>
    private bool CanProcessDecision()
    {
        if (Time.time - encounterDisplayTime < minimumDisplayTime)
        {
            float remainingTime = minimumDisplayTime - (Time.time - encounterDisplayTime);
            Debug.Log($"Must wait {remainingTime:F1} more seconds before making a decision");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Manual encounter request button handler
    /// </summary>
    public void OnNextEncounterClicked()
    {
        if (isProcessingDecision || isInDecisionAnimation)
        {
            Debug.Log("Cannot request next encounter while processing");
            return;
        }
        
        if (currentEncounter != null)
        {
            Debug.Log("Current encounter still active");
            return;
        }
        
        NextEncounter();
    }

    /// <summary>
    /// Load next encounter manually (used by Next Button)
    /// </summary>
    public void NextEncounter()
    {
        // Check with timing controller first
        ShipTimingController timingController = ShipTimingController.Instance;
        if (timingController != null && !timingController.RequestNextShip())
        {
            Debug.Log("CredentialChecker: Ship request denied by timing controller");
            return;
        }

        // Ensure we have the latest reference to MasterShipGenerator
        if (masterShipGenerator == null)
        {
            // First try the singleton pattern
            masterShipGenerator = MasterShipGenerator.Instance;

            // If that fails, try FindFirstObjectByType
            if (masterShipGenerator == null)
            {
                masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            }

            if (masterShipGenerator != null)
            {
                Debug.Log("CredentialChecker: NextEncounter recovered masterShipGenerator reference");
                // Resubscribe to events
                masterShipGenerator.OnEncounterReady -= DisplayEncounter;
                masterShipGenerator.OnEncounterReady += DisplayEncounter;
            }
        }

        if (masterShipGenerator != null)
        {
            try
            {
                // Attempt to get next encounter
                MasterShipEncounter encounter = masterShipGenerator.GetNextEncounter();

                // If we got a valid encounter but DisplayEncounter wasn't called via event,
                // we'll need to call it directly
                if (encounter != null && currentEncounter != encounter)
                {
                    Debug.Log("CredentialChecker: Displaying encounter directly since event may not have fired");
                    DisplayEncounter(encounter);
                    UpdateGameStatus();
                }
                else if (encounter == null)
                {
                    Debug.LogWarning("CredentialChecker: NextEncounter received null encounter from generator");
                    // Create a fallback encounter for stability
                    MasterShipEncounter fallback = MasterShipEncounter.CreateTestEncounter();
                    DisplayEncounter(fallback);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CredentialChecker: Error getting next encounter: {e.Message}");
                // Create emergency fallback encounter
                MasterShipEncounter emergencyFallback = MasterShipEncounter.CreateTestEncounter();
                DisplayEncounter(emergencyFallback);
            }
        }
        else
        {
            Debug.LogError("CredentialChecker: Cannot get next encounter - ship generator not found!");

            // Create a fallback encounter even if we can't find the generator
            MasterShipEncounter fallback = MasterShipEncounter.CreateTestEncounter();
            DisplayEncounter(fallback);
        }
        
        SafeEnableButtons();
    
        // Reset processing locks
        isProcessingDecision = false;
        isInDecisionAnimation = false;
    }
    
    //// <summary>
    /// Called when the continue button is clicked
    /// </summary>
    public void OnContinueClicked()
    {
        // Hide the daily report UI
        gameObject.SetActive(false);
        
        // Clear the current encounter display before starting new day
        CredentialChecker credentialChecker = FindFirstObjectByType<CredentialChecker>();
        if (credentialChecker != null)
        {
            // Clear the current encounter reference using reflection
            var currentEncounterField = credentialChecker.GetType().GetField("currentEncounter", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (currentEncounterField != null)
            {
                currentEncounterField.SetValue(credentialChecker, null);
            }
            
            // Get references to UI panels
            var shipNamePanel = credentialChecker.GetType().GetField("shipNamePanel", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(credentialChecker) as TMP_Text;
            var shipDescriptionPanel = credentialChecker.GetType().GetField("shipDescriptionPanel", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(credentialChecker) as TMP_Text;
            var credentialsPanel = credentialChecker.GetType().GetField("credentialsPanel", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(credentialChecker) as TMP_Text;
            
            // Clear text displays
            if (shipNamePanel) shipNamePanel.text = "";
            if (shipDescriptionPanel) shipDescriptionPanel.text = "Ready for next shift...";
            if (credentialsPanel) credentialsPanel.text = "";
            
            // Get references to visual containers
            var shipImageContainer = credentialChecker.GetType().GetField("shipImageContainer", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(credentialChecker) as GameObject;
            var captainImageContainer = credentialChecker.GetType().GetField("captainImageContainer", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(credentialChecker) as GameObject;
            var shipVideoContainer = credentialChecker.GetType().GetField("shipVideoContainer", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(credentialChecker) as GameObject;
            var captainVideoContainer = credentialChecker.GetType().GetField("captainVideoContainer", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(credentialChecker) as GameObject;
            
            // Hide visual containers
            if (shipImageContainer) shipImageContainer.SetActive(false);
            if (captainImageContainer) captainImageContainer.SetActive(false);
            if (shipVideoContainer) shipVideoContainer.SetActive(false);
            if (captainVideoContainer) captainVideoContainer.SetActive(false);
            
            // Get and hide stamps
            var approveStamp = credentialChecker.GetType().GetField("approveStamp", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(credentialChecker) as GameObject;
            var denyStamp = credentialChecker.GetType().GetField("denyStamp", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(credentialChecker) as GameObject;
            
            if (approveStamp) approveStamp.SetActive(false);
            if (denyStamp) denyStamp.SetActive(false);
            
            // Get and hide feedback panel
            var feedbackPanel = credentialChecker.GetType().GetField("feedbackPanel", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(credentialChecker) as GameObject;
            if (feedbackPanel) feedbackPanel.SetActive(false);
            
            Debug.Log("DailyReportManager: Cleared encounter display for new day");
        }
        
        // Make sure we're not already transitioning
        if (integrationHelper != null)
        {
            // Use integration helper to progress day
            integrationHelper.EndShift();
        }
        else if (gameManager != null && !gameManager.isTransitioningDay)
        {
            gameManager.StartNextDay();
        }
    }

    /// <summary>
    /// Enable/disable decision buttons with visual feedback
    /// </summary>
    void SetButtonsInteractable(bool interactable)
    {
        // Safety check: Don't enable buttons during reaction videos
        if (interactable && isReactionVideoPlaying)
        {
            if (verboseDebugLogs)
                Debug.LogWarning("CredentialChecker: Attempted to enable buttons during reaction video - ignoring to prevent accidental clicks");
            return;
        }
        
        // Safety check: Don't enable buttons while processing decisions or in animation
        if (interactable && (isProcessingDecision || isInDecisionAnimation))
        {
            if (verboseDebugLogs)
                Debug.LogWarning("CredentialChecker: Attempted to enable buttons while processing decision or in animation - ignoring to prevent accidental clicks");
            return;
        }
        
        if (verboseDebugLogs)
            Debug.Log($"CredentialChecker: Setting decision buttons to {(interactable ? "ENABLED" : "DISABLED")}");
        
        // Core decision buttons
        if (approveButton) 
        {
            approveButton.interactable = interactable;
            SetButtonVisualState(approveButton, interactable);
        }
        if (denyButton) 
        {
            denyButton.interactable = interactable;
            SetButtonVisualState(denyButton, interactable);
        }
        
        // Optional action buttons
        if (acceptBribeButton) 
        {
            acceptBribeButton.interactable = interactable;
            SetButtonVisualState(acceptBribeButton, interactable);
        }
        if (holdingPatternButton) 
        {
            holdingPatternButton.interactable = interactable;
            SetButtonVisualState(holdingPatternButton, interactable);
        }
        if (tractorBeamButton) 
        {
            tractorBeamButton.interactable = interactable;
            SetButtonVisualState(tractorBeamButton, interactable);
        }
    }

    /// <summary>
    /// Set visual state for a button to provide clear feedback
    /// </summary>
    private void SetButtonVisualState(Button button, bool interactable)
    {
        if (button == null) return;
        
        // Get the button's colors
        ColorBlock colors = button.colors;
        
        if (interactable)
        {
            // Ensure normal state is visible
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        }
        else
        {
            // Make disabled state clearly visible
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        
        button.colors = colors;
    }
    
    /// <summary>
    /// Safely enable buttons only when all conditions are met
    /// </summary>
    private void SafeEnableButtons()
    {
        // Only enable if not processing decision, not in animation, and not playing reaction video
        bool canEnable = !isProcessingDecision && !isInDecisionAnimation && !isReactionVideoPlaying;
        
        if (verboseDebugLogs)
            Debug.Log($"CredentialChecker: SafeEnableButtons - canEnable: {canEnable} (processing: {isProcessingDecision}, animation: {isInDecisionAnimation}, video: {isReactionVideoPlaying})");
        
        if (canEnable)
        {
            SetButtonsInteractable(true);
        }
        else
        {
            // Keep buttons disabled until all conditions are clear
            SetButtonsInteractable(false);
        }
    }
    
    /// <summary>
    /// Force clear all UI blocking states - called when shift ends
    /// </summary>
    public void ForceUnblockUI()
    {
        Debug.Log("CredentialChecker: Force unblocking UI for shift end");
        
        // Clear all blocking flags
        isProcessingDecision = false;
        isInDecisionAnimation = false;
        isReactionVideoPlaying = false;
        
        // Re-enable buttons
        SetButtonsInteractable(true);
        
        // Stop any coroutines that might be running
        StopAllCoroutines();
        
        // Clear any pending encounters
        pendingTextEncounter = null;
    }
    
    /// <summary>
    /// Check if the game state allows for ship processing
    /// </summary>
    private bool IsGameStateReadyForProcessing()
    {
        // Check if the game state controller exists and if gameplay is active
        GameStateController controller = GameStateController.Instance;
        if (controller != null && !controller.IsGameplayActive())
        {
            // If not in active gameplay, prompt the user to start the game
            ShowFeedback("Please start the game to begin processing ships", Color.yellow);
            
            // Add a method to manually activate gameplay for testing
            ForceActiveGameplay(); // Call our own method, not the controller's
            
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// Checks if the credential checker has an active encounter
    /// Used by EncounterSystemManager to ensure proper initialization
    /// </summary>
    /// <returns>True if there is an active encounter, false otherwise</returns>
    public bool HasActiveEncounter()
    {
        // Check if we have a valid encounter reference
        if (currentEncounter == null)
        {
            return false;
        }
        
        // Verify the encounter has valid data
        bool hasValidData = !string.IsNullOrEmpty(currentEncounter.shipType) && 
                           !string.IsNullOrEmpty(currentEncounter.accessCode);
        
        if (verboseDebugLogs && !hasValidData)
        {
            Debug.LogWarning("CredentialChecker: Current encounter exists but has invalid data");
        }
        
        return hasValidData;
    }
    
    /// <summary>
    /// Set up all button listeners
    /// </summary>
    private void SetupButtonListeners()
    {
        // Core decision buttons
        if (approveButton) approveButton.onClick.AddListener(OnApproveClicked);
        if (denyButton) denyButton.onClick.AddListener(OnDenyClicked);
        if (openLogBookButton) openLogBookButton.onClick.AddListener(OpenLogBook);
        if (closeLogBookButton) closeLogBookButton.onClick.AddListener(CloseLogBook);
        
        // Special action buttons
        if (acceptBribeButton) 
        {
            acceptBribeButton.onClick.AddListener(OnAcceptBribeClicked);
            acceptBribeButton.gameObject.SetActive(false); // Hide initially
        }
        
        if (holdingPatternButton)
        {
            holdingPatternButton.onClick.AddListener(OnHoldingPatternClicked);
            holdingPatternButton.gameObject.SetActive(false); // Hide initially
        }
        
        if (tractorBeamButton)
        {
            tractorBeamButton.onClick.AddListener(OnTractorBeamClicked);
            tractorBeamButton.gameObject.SetActive(false); // Hide initially
        }
    }
    
    /// <summary>
    /// Safely notify the ship generator about a decision, with error handling and recovery
    /// </summary>
    private void SafelyNotifyShipGenerator(bool approved, MasterShipEncounter encounterToProcess)
    {
        if (masterShipGenerator == null)
        {
            // Try to recover the reference
            masterShipGenerator = MasterShipGenerator.Instance;
            
            if (masterShipGenerator == null)
            {
                masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
                
                if (masterShipGenerator == null)
                {
                    Debug.LogError("CredentialChecker: Failed to find MasterShipGenerator reference for processing decision");
                    return;
                }
            }
            
            Debug.Log("CredentialChecker: Recovered MasterShipGenerator reference for processing decision");
        }
        
        try
        {
            Debug.Log("Notifying ship generator of decision");
            masterShipGenerator.ProcessDecisionWithEncounter(approved, encounterToProcess);
            
            // Start the transition flow after decision
            StartCoroutine(HandlePostDecisionFlow(encounterToProcess));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CredentialChecker: Error processing decision: {e.Message}\n{e.StackTrace}");
            
            // Try to recover
            try
            {
                masterShipGenerator = MasterShipGenerator.Instance;
                
                if (masterShipGenerator != null)
                {
                    Debug.Log("CredentialChecker: Reconnected to MasterShipGenerator, retrying process decision");
                    masterShipGenerator.ProcessDecisionWithEncounter(approved, encounterToProcess);
                }
            }
            catch (System.Exception retryEx)
            {
                Debug.LogError($"CredentialChecker: Failed to process decision on retry: {retryEx.Message}");
            }
        }
    }

    // Add this new method to CredentialChecker:
    private IEnumerator HandlePostDecisionFlow(MasterShipEncounter processedEncounter)
    {
        // Wait for reaction video to complete (if playing)
        EncounterMediaTransitionManager mediaTransitionManager = FindFirstObjectByType<EncounterMediaTransitionManager>();
        if (mediaTransitionManager != null)
        {
            // Wait approximately for reaction video duration
            yield return new WaitForSeconds(3f);
        }

        // Check for special breakpoints
        bool shouldPlayTransition = true;

        // Check for inspection trigger
        ConsequenceManager consequenceManager = FindFirstObjectByType<ConsequenceManager>();
        if (consequenceManager != null && Random.value < 0.1f) // 10% chance for testing
        {
            ShipTransitionController transitionController = ShipTransitionController.Instance;
            if (transitionController != null)
            {
                yield return transitionController.PlayInspectionTransition("Random security check initiated");
                shouldPlayTransition = false;
            }
        }

        // Check for story message
        if (processedEncounter != null && processedEncounter.isStoryShip)
        {
            SpecializedScenarioProvider storyProvider = SpecializedScenarioProvider.Instance;
            if (storyProvider != null && storyProvider.IsStorySignificant(processedEncounter.storyTag))
            {
                ShipTransitionController transitionController = ShipTransitionController.Instance;
                if (transitionController != null)
                {
                    string storyMessage = GetStoryMessage(processedEncounter.storyTag);
                    yield return transitionController.PlayStoryTransition(storyMessage);
                    shouldPlayTransition = false;
                }
            }
        }

        // Play normal transition if no special events
        if (shouldPlayTransition)
        {
            ShipTransitionController transitionController = ShipTransitionController.Instance;
            if (transitionController != null)
            {
                yield return transitionController.PlayShipTransition();
            }
            else
            {
                // Fallback delay if no transition controller
                yield return new WaitForSeconds(2f);
            }
        }

        // Now request the next encounter
        // NextEncounter();
        
        // Re-enable buttons for the next encounter
        SafeEnableButtons();
        
        // Also ensure processing locks are released
        isProcessingDecision = false;
        isInDecisionAnimation = false;
        
        if (verboseDebugLogs)
        {
            Debug.Log("Buttons re-enabled after post-decision flow");
        }
    }

    // Add this helper method too:
    private string GetStoryMessage(string storyTag)
    {
        switch (storyTag)
        {
            case "bounty_hunter":
                return "Intelligence Report: Bounty hunter activity detected in sector";
            case "imperium_traitor":
                return "Security Alert: Possible internal security breach";
            case "insurgent":
                return "Warning: Rebel sympathizer activity increasing";
            default:
                return "Special encounter logged";
        }
    }

    #region Captain ID Card System

    /// <summary>
    /// Set up click handlers for captain video/image to show ID card
    /// </summary>
    private void SetupCaptainClickHandlers()
    {
        // Set up video click handler
        if (captainVideoContainer != null)
        {
            // Add button component if it doesn't exist
            Button videoButton = captainVideoContainer.GetComponent<Button>();
            if (videoButton == null)
            {
                videoButton = captainVideoContainer.AddComponent<Button>();
            }
            captainVideoButton = videoButton;
            
            // Clear existing listeners and add new one
            captainVideoButton.onClick.RemoveAllListeners();
            captainVideoButton.onClick.AddListener(OnCaptainClicked);
            
            // Make sure it's interactable
            captainVideoButton.interactable = true;
        }
        
        // Set up image click handler
        if (captainImageContainer != null)
        {
            // Add button component if it doesn't exist
            Button imageButton = captainImageContainer.GetComponent<Button>();
            if (imageButton == null)
            {
                imageButton = captainImageContainer.AddComponent<Button>();
            }
            captainImageButton = imageButton;
            
            // Clear existing listeners and add new one
            captainImageButton.onClick.RemoveAllListeners();
            captainImageButton.onClick.AddListener(OnCaptainClicked);
            
            // Make sure it's interactable
            captainImageButton.interactable = true;
        }
    }

    /// <summary>
    /// Handle captain portrait/video click
    /// </summary>
    private void OnCaptainClicked()
    {
        // Don't allow ID card during decision process
        if (isProcessingDecision || isInDecisionAnimation)
        {
            Debug.Log("Cannot view ID card while processing decision");
            return;
        }
        
        // Show the ID card if we have an encounter
        if (currentEncounter != null && captainIDCard != null)
        {
            captainIDCard.ShowIDCard(currentEncounter);
        }
    }

    /// <summary>
    /// Disable captain clicks during decision process
    /// </summary>
    private void DisableCaptainClicks()
    {
        if (captainVideoButton != null) captainVideoButton.interactable = false;
        if (captainImageButton != null) captainImageButton.interactable = false;
    }

    /// <summary>
    /// Enable captain clicks when not in decision process
    /// </summary>
    private void EnableCaptainClicks()
    {
        if (captainVideoButton != null) captainVideoButton.interactable = true;
        if (captainImageButton != null) captainImageButton.interactable = true;
    }

    #endregion

    #region Day Transition

    /// <summary>
    /// Clear the current encounter display for a fresh start on new day
    /// </summary>
    public void ClearEncounterDisplay()
    {
        Debug.Log("CredentialChecker: Clearing encounter display for new day");
        
        // Clear the current encounter
        currentEncounter = null;
        pendingTextEncounter = null;
        
        // Clear all text displays
        if (shipInfoPanel) shipInfoPanel.text = "";
        if (credentialsPanel) credentialsPanel.text = "";
        if (feedbackText) feedbackText.text = "";
        if (captainDialogText) captainDialogText.text = "";
        if (logBookContent) logBookContent.text = "";
        
        // Stop any playing videos
        if (shipVideoPlayer && shipVideoPlayer.isPlaying)
        {
            shipVideoPlayer.Stop();
        }
        if (captainVideoPlayer && captainVideoPlayer.isPlaying)
        {
            captainVideoPlayer.Stop();
        }
        
        // Hide video containers
        if (shipVideoContainer) shipVideoContainer.SetActive(false);
        if (captainVideoContainer) captainVideoContainer.SetActive(false);
        
        // Clear captain and ship images
        if (captainImageDisplay) captainImageDisplay.sprite = null;
        if (captainImageContainer) captainImageContainer.SetActive(false);
        if (shipImageDisplay) shipImageDisplay.sprite = null;
        if (shipImageContainer) shipImageContainer.SetActive(false);
        
        // Reset tracking flags
        shipVideoStarted = false;
        captainVideoStarted = false;
        isReactionVideoPlaying = false;
        isInDecisionAnimation = false;
        
        // Disable all decision buttons
        SetButtonsInteractable(false);
        
        // Hide captain ID card if it's showing
        if (captainIDCard != null)
        {
            captainIDCard.HideIDCard();
        }
        
        Debug.Log("CredentialChecker: Encounter display cleared");
        
        // CRITICAL: Restart encounter generation for new day
        // The new EncounterManager needs a manual trigger to start generating encounters
        var encounterManager = Starkiller.Core.ServiceLocator.Get<Starkiller.Core.Managers.EncounterManager>();
        if (encounterManager != null)
        {
            Debug.Log("CredentialChecker: Triggering encounter generation restart for new day");
            // The EncounterManager will automatically start generating encounters in its Update() 
            // when CanGenerateEncounter() returns true, which should happen once game state is Gameplay
        }
        else
        {
            Debug.LogWarning("CredentialChecker: EncounterManager not found - encounters may not restart properly");
        }
    }

    #endregion
    
    /// <summary>
    /// Wrapper class to adapt MasterShipEncounter to IEncounter interface
    /// </summary>
    private class MasterShipEncounterWrapper : Starkiller.Core.IEncounter
    {
        private readonly MasterShipEncounter _encounter;
        
        public MasterShipEncounterWrapper(MasterShipEncounter encounter)
        {
            _encounter = encounter;
        }
        
        public string ShipType => _encounter?.shipType ?? "Unknown";
        public string CaptainName => _encounter?.captainName ?? "Unknown";
        public string AccessCode => _encounter?.accessCode ?? "Unknown";
        public bool IsValid => _encounter?.shouldApprove ?? false;
    }
}