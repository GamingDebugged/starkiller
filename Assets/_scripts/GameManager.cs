using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using StarkillerBaseCommand;
using Starkiller.Core;
using Starkiller.Core.Managers;

// Add GameState enum for tracking game state
public enum GameState
{
    Title,
    Gameplay,
    DayEnd,
    GameOver,
    Paused
}

public class GameManager : MonoBehaviour
{
    // INSPECTOR VARIABLES - Drag & drop these in Unity Editor
    [Header("UI References")]
    public TMP_Text gameStatusText;            // Text showing current game status (updated to TMP_Text)
    public TMP_Text gameTimerText;             // Text showing remaining time for shift (updated to TMP_Text)
    public TMP_Text dailyBriefingText;         // Text showing daily rule changes (updated to TMP_Text)
    public GameObject gameOverPanel;           // Panel to show when game is over
    public TMP_Text finalScoreText;            // Text to display final score (updated to TMP_Text)
    public Button restartButton;               // Button to restart the game
    public GameObject dailyReportPanel;        // Panel for end of day report
    public TMP_Text salaryText;                // Text showing daily earnings (updated to TMP_Text)
    public TMP_Text expensesText;              // Text showing daily expenses (updated to TMP_Text)
    public Button continueButton;              // Button to continue to next day
    public AudioSource quotaReachedSound;       // Sound for quota reached
    
    [Header("Game Settings")]
    public int maxMistakes = 3;            // Number of mistakes before game over
    public float timeBetweenShips = 2f;    // Time between ship encounters
    public int requiredShipsPerDay = 8;    // Minimum ships to process for base salary
    public int baseSalary = 30;            // Base salary for meeting quota
    public int bonusPerExtraShip = 5;      // Bonus credits per extra ship processed
    public int penaltyPerMistake = 5;      // Credits deducted per mistake

    // Protection Level Errors for correctDecisions and wrongDecisions
    public int CorrectDecisions { get { return correctDecisions; } }
    public int WrongDecisions { get { return wrongDecisions; } }

    // public getters for correctDecisions and wrongDecisions
    public int GetCorrectDecisions() { return correctDecisions; }
    public int GetWrongDecisions() { return wrongDecisions; }
    
    [Header("Imperium Family Settings")]
    public int medicalCareCost = 30;       // Cost for specialized medical care
    public int equipmentCost = 25;         // Cost for specialized equipment
    public int trainingCost = 20;        // Cost for training/bribes
    public int premiumQuartersCost = 15;   // Cost for better living quarters
    public int childcareCost = 10;         // Cost for childcare services
    
    [Header("Component References")]
    public CredentialChecker credentialChecker; // Reference to the credential checker
    public ImperialFamilySystem familySystem;  // Reference to the Imperial family system
    public GameObject moralChoicePanel;        // Panel for moral choice events
    public UIManager uiManager;                // Reference to the UI Manager
    public EnhancedShipGenerator enhancedShipGenerator; // Reference to the enhanced ship generator
    public ShipVideoSystem shipVideoSystem;  // Reference to the ship video system
    private StarkillerBaseCommand.StarkkillerContentManager contentManager; // Reference to the content manager
    public ShipEncounterGenerator shipEncounterGenerator;  // Reference to the ship encounter generator
    private ConsequenceManager consequenceManager; // Reference to the consequence manager
    

    [Header("New System References")]
    public MasterShipGenerator masterShipGenerator;

    // Reference to DailyReportManager (using RefactoredDailyReportManager instead of old one)
    private Starkiller.Core.Managers.RefactoredDailyReportManager refactoredDailyReportManager;
    
    // GAME STATE VARIABLES
    private int currentStrikes = 0;        // Current number of mistakes
    private int shipsProcessed = 0;        // Number of ships processed today (daily counter)
    private int totalShipsProcessed = 0;   // Total ships processed across all days (game counter)
    private int correctDecisions = 0;      // Number of correct decisions today
    private int wrongDecisions = 0;        // Number of wrong decisions today
    private int score = 0;                 // Player's total score
    private int credits = 30;              // Player's current credits
    private bool gameActive = false;       // Is the game currently active?
    private float remainingTime;           // Remaining time in shift
    private int dayIncrementCount = 0;     // Just counting the days
    
    // Current game state
    private GameState currentGameState = GameState.Title;
    
    // Getter methods for CredentialChecker to access
    public int GetShipsProcessed() { return shipsProcessed; } // Daily count for UI/quota
    public int GetTotalShipsProcessed() { return totalShipsProcessed; } // Game total for final score
    public int GetCredits() { return credits; }
    public int GetStrikes() { return currentStrikes; }
    public int GetCurrentStrikes() { return currentStrikes; } // Alternative name for consistency
    public int GetCurrentDay() { return currentDay; }
    
    // Family system support methods
    public void SetStrikes(int newStrikes) { currentStrikes = Mathf.Max(0, newStrikes); }
    public void RemoveStrike() { currentStrikes = Mathf.Max(0, currentStrikes - 1); }
    
    // Performance modifier for family support
    public float performanceModifier = 1.0f;
    public void SetPerformanceModifier(float modifier) { performanceModifier = modifier; }
    public float GetPerformanceModifier() { return performanceModifier; }
    
    // Reference to the TimeManager
    private TimeManager timeManager;

    // References to refactored managers
    private CreditsManager creditsManager;
    private DecisionTracker decisionTracker;
    private DayProgressionManager dayProgressionManager;

    // DEPRECATED: currentDay is now managed by DayProgressionManager
    // This property redirects to DayProgressionManager for backward compatibility
    public int currentDay 
    { 
        get 
        {
            var dayProgression = dayProgressionManager ?? FindFirstObjectByType<Starkiller.Core.Managers.DayProgressionManager>();
            return dayProgression != null ? dayProgression.CurrentDay : 1;
        }
        set 
        {
            Debug.LogWarning($"[GameManager] Setting currentDay directly is deprecated. Use DayProgressionManager instead.");
            var dayProgression = dayProgressionManager ?? FindFirstObjectByType<Starkiller.Core.Managers.DayProgressionManager>();
            if (dayProgression != null)
            {
                dayProgression.SetCurrentDay(value);
            }
        }
    }
    
    private bool isPaused = false;         // Is the game paused?
    public bool isTransitioningDay = false; // Flag to prevent multiple day transitions
    
    // Story tracking
    private List<string> keyDecisions = new List<string>();    // Track important decisions
    private int imperialLoyalty = 0;                           // Track loyalty to the Imperium (-10 to 10)
    private int rebellionSympathy = 0;                         // Track sympathy for Insurgents (-10 to 10)
    
    // Current ship data - can be either MasterShipEncounter or legacy ShipEncounter
    private object currentShip; // Keep as object for compatibility with legacy systems

    // Connect to new logbook manager
    private StarkkillerLogBookManager logBookManager;

    [Header("Casualty Tracking")]
    public int totalCasualties = 0;

    // Use this for initialization
    void Start()
    {
        // Make sure UI elements are assigned
        if (!ValidateReferences())
        {
            Debug.LogError("Missing UI references. Check the Inspector!");
            return;
        }
        
        // Find content manager if not assigned
        if (contentManager == null)
            contentManager = FindFirstObjectByType<StarkillerBaseCommand.StarkkillerContentManager>();

        // Find ShipEncounterGenerator if not assigned
        if (shipEncounterGenerator == null)
            shipEncounterGenerator = FindFirstObjectByType<ShipEncounterGenerator>();
            
        // Find RefactoredDailyReportManager via ServiceLocator
        refactoredDailyReportManager = ServiceLocator.Get<Starkiller.Core.Managers.RefactoredDailyReportManager>();
        if (refactoredDailyReportManager == null)
        {
            Debug.LogWarning("[GameManager] RefactoredDailyReportManager not found in ServiceLocator - daily reports may not work properly");
        }

        // Find ConsequenceManager if not assigned
        if (consequenceManager == null)
            consequenceManager = FindFirstObjectByType<ConsequenceManager>();

        // Initialize refactored managers
        InitializeRefactoredManagers();

        // Setup button listeners
        if (restartButton) restartButton.onClick.RemoveAllListeners(); 
        if (restartButton) restartButton.onClick.AddListener(RestartGame);
        
        // Remove any existing listeners before adding a new one
        if (continueButton) 
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(StartNextDay);
        }
        
        // Initially hide panels
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (dailyReportPanel) dailyReportPanel.SetActive(false);
        if (moralChoicePanel) moralChoicePanel.SetActive(false);
        
        // Set Imperial family cost values (in case they were changed in inspector)
        if (familySystem)
        {
            familySystem.medicalCareCost = medicalCareCost;
            familySystem.equipmentCost = equipmentCost;
            familySystem.trainingCost = trainingCost;
            familySystem.premiumQuartersCost = premiumQuartersCost;
            familySystem.childcareCost = childcareCost;
        }
        
        // Find TimeManager if not assigned
        if (timeManager == null)
            timeManager = TimeManager.Instance;

        // Initialize the logbook manager
        if (logBookManager == null)
            logBookManager = FindFirstObjectByType<StarkkillerLogBookManager>();

        // Sync the ShipEncounterGenerator if available
        if (shipEncounterGenerator != null && dayProgressionManager != null)
        {
            shipEncounterGenerator.StartNewDay(dayProgressionManager.CurrentDay);
        }

        // Ensure EncounterFlowManager exists
        if (EncounterFlowManager.Instance == null)
        {
            GameObject flowManagerObj = new GameObject("EncounterFlowManager");
            flowManagerObj.AddComponent<EncounterFlowManager>();
            Debug.Log("Created EncounterFlowManager instance");
        }

        // Subscribe to ShiftTimerManager events
        Starkiller.Core.Managers.ShiftTimerManager.OnTimerExpired += OnShiftTimerExpired;
        Debug.Log("[GameManager] Subscribed to ShiftTimerManager.OnTimerExpired event");

        // Start the game
        StartGame();
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        Starkiller.Core.Managers.ShiftTimerManager.OnTimerExpired -= OnShiftTimerExpired;
        Debug.Log("[GameManager] Unsubscribed from ShiftTimerManager.OnTimerExpired event");
    }

    // Add to your GameManager or similar initialization script
    public class GameManagerInitializer : MonoBehaviour
    {
        [SerializeField] private MasterShipGenerator
    masterShipGeneratorPrefab;

        private void Awake()
        {
            // If MasterShipGenerator doesn't exist yet, instantiate it
            if (MasterShipGenerator.Instance == null)
            {
                var instance = Instantiate(masterShipGeneratorPrefab,
    transform);
                // No need for DontDestroyOnLoad here if parent already has 
            }
        }
    }

    /// <summary>
    /// Handler for when ShiftTimerManager reports timer expiration
    /// </summary>
    private void OnShiftTimerExpired()
    {
        Debug.Log("[GameManager] ShiftTimerManager timer expired - shift end is handled by DayProgressionManager, no action needed");
        
        // NOTE: The new manager architecture handles shift ending automatically:
        // ShiftTimerManager -> DayProgressionManager.EndShift() -> RefactoredDailyReportManager
        // We don't need to call EndShift() here anymore to avoid double calls
        
        if (currentGameState != GameState.Gameplay)
        {
            Debug.LogWarning($"[GameManager] Timer expired but game state is {currentGameState} - this suggests a state synchronization issue");
        }
    }

    public void EndShift()
    {
        // Debug information
        Debug.Log($"EndShift called - Current game state: {currentGameState}");

        if (currentGameState != GameState.Gameplay)
        {
            Debug.LogWarning("EndShift called but game is not in Gameplay state. Ignoring call.");
            return;
        }

        // CRITICAL: Clear any blocking UI states before showing daily report
        // This prevents the Continue button from being unclickable
        var mediaTransitionManager = FindFirstObjectByType<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
        if (mediaTransitionManager != null)
        {
            Debug.Log("EndShift: Stopping any active reactions and clearing UI blocks");
            mediaTransitionManager.StopCurrentReaction();
        }
        
        // Also force unblock the CredentialChecker UI
        var credentialChecker = FindFirstObjectByType<CredentialChecker>();
        if (credentialChecker != null)
        {
            Debug.Log("EndShift: Force unblocking CredentialChecker UI");
            credentialChecker.ForceUnblockUI();
        }

        // Notify GameStateController
        GameStateController controller = GameStateController.Instance;
        if (controller != null)
        {
            controller.OnDayEnd();
        }

        // Calculate daily salary
        int salary = CalculateSalary();

        // Log details
        Debug.Log($"EndShift: Calculated salary: {salary}, Ships processed: {shipsProcessed}/{requiredShipsPerDay}");

        // Show end of day report
        ShowDailyReport(salary);

        // Set game state to DayEnd
        currentGameState = GameState.DayEnd;
        gameActive = false;

        Debug.Log("EndShift completed - Day ended successfully");
    }

    void Update()
    {
        // Check if time is paused by TimeManager - but only during specific game states
        if (timeManager != null && timeManager.IsTimePaused() && (currentGameState == GameState.DayEnd || currentGameState == GameState.Paused))
        {
            // Only respect time pausing during day end or explicit pause states
            return;
        }

        if (gameActive && !isPaused && currentGameState == GameState.Gameplay)
        {
            // Check if game over manager says the game is over
            var gameOverManager = Starkiller.Core.ServiceLocator.Get<Starkiller.Core.Managers.GameOverManager>();
            if (gameOverManager != null && gameOverManager.IsGameOver)
            {
                // Game is over, stop all gameplay
                gameActive = false;
                currentGameState = GameState.GameOver;
                
                // CRITICAL: Also update the new GameStateManager for state synchronization
                var gameStateManager = ServiceLocator.Get<Starkiller.Core.Managers.GameStateManager>();
                if (gameStateManager != null)
                {
                    gameStateManager.ChangeState(Starkiller.Core.GameState.GameOver);
                }
                
                return;
            }
            
            // Check if ShiftTimerManager is handling the timer
            var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
            if (shiftTimerManager != null && shiftTimerManager.IsTimerActive)
            {
                // Let ShiftTimerManager handle the timer - sync our remainingTime for compatibility
                remainingTime = shiftTimerManager.RemainingTime;
                
                // Update timer display using ShiftTimerManager's formatted time
                if (gameTimerText)
                {
                    gameTimerText.text = shiftTimerManager.GetFormattedTime();
                }
                else
                {
                    Debug.LogWarning("[GameManager] gameTimerText is null! Timer UI not assigned.");
                }
                
                // Sync ship quota from difficulty profile
                var daySettings = shiftTimerManager.GetCurrentDaySettings();
                if (daySettings != null && daySettings.shipQuota > 0)
                {
                    requiredShipsPerDay = daySettings.shipQuota;
                }
                
                // Check if ShiftTimerManager says shift should end
                if (shiftTimerManager.IsShiftEnded)
                {
                    Debug.Log($"ShiftTimerManager ended shift - GameManager syncing");
                    EndShift();
                }
            }
            else
            {
                // Fallback to legacy timer system if ShiftTimerManager not available
                if (shiftTimerManager == null)
                {
                    Debug.LogWarning("[GameManager] ShiftTimerManager not found! Using fallback timer.");
                }
                else
                {
                    Debug.LogWarning("[GameManager] ShiftTimerManager found but timer not active. Using fallback timer.");
                }
                
                remainingTime -= Time.deltaTime;
                
                // Update timer display (legacy path - should not normally be used)
                if (gameTimerText)
                {
                    // Clamp to prevent negative display
                    float displayTime = Mathf.Max(0f, remainingTime);
                    int minutes = Mathf.FloorToInt(displayTime / 60);
                    int seconds = Mathf.FloorToInt(displayTime % 60);
                    gameTimerText.text = string.Format("Shift Time: {0:00}:{1:00}", minutes, seconds);
                }
                else
                {
                    Debug.LogWarning("[GameManager] gameTimerText is null! Timer UI not assigned.");
                }
                
                // End shift if time runs out
                if (remainingTime <= 0.01f)
                {
                    Debug.Log($"Timer expired: {remainingTime} seconds - Calling EndShift()");
                    remainingTime = 0f;  // Force to exactly zero
                    EndShift();
                }
            }
        }
    }



    // Checks if all required references are assigned
    bool ValidateReferences()
    {
        bool result = true;
        
        // For testing purposes, we'll make gameStatusText optional
        // This lets us test core functionality without all UI elements in place
        if (gameStatusText == null)
        {
            Debug.LogWarning("gameStatusText reference not assigned - UI updates will be skipped");
            // Not returning false as we want to continue for testing
        }
        
        // Also check timer text reference
        if (gameTimerText == null)
        {
            Debug.LogWarning("gameTimerText reference not assigned - timer display will not work");
        }
        
        if (credentialChecker == null)
        {
            Debug.LogError("Missing CredentialChecker reference!");
            result = false;
        }
        
        return result;
    }

    /// <summary>
    /// Initialize references to refactored managers
    /// </summary>
    private void InitializeRefactoredManagers()
    {
        // Get manager references from ServiceLocator
        creditsManager = Starkiller.Core.ServiceLocator.Get<CreditsManager>();
        decisionTracker = Starkiller.Core.ServiceLocator.Get<DecisionTracker>();
        dayProgressionManager = Starkiller.Core.ServiceLocator.Get<DayProgressionManager>();

        // Log which managers are available
        Debug.Log($"[GameManager] Refactored managers initialized: " +
                 $"CreditsManager: {(creditsManager != null ? "✓" : "✗")}, " +
                 $"DecisionTracker: {(decisionTracker != null ? "✓" : "✗")}, " +
                 $"DayProgressionManager: {(dayProgressionManager != null ? "✓" : "✗")}");
    }

    // Starts a new game
    public void StartGame()
    {
        // Reset DayProgressionManager first - it's the source of truth for days
        if (dayProgressionManager != null)
        {
            dayProgressionManager.ResetForNewGame();
        }
        
        // Reset game state
        currentStrikes = 0;
        shipsProcessed = 0;
        score = 0;
        credits = 30;
        // Day is now managed by DayProgressionManager
        imperialLoyalty = 0;
        rebellionSympathy = 0;
        keyDecisions.Clear();
        
        // Set initial game state
        currentGameState = GameState.Title;
        
        // Start first day
        StartDay();
    }
    
    // Restart the game after game over
    void RestartGame()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        StartGame();
    }

    // Starts a new day
    public void StartDay()
    {
        // Reset daily counters (local tracking only - DayProgressionManager handles the real counts)
        shipsProcessed = 0;
        correctDecisions = 0;
        wrongDecisions = 0;
        
        // Update family system for the new day
        var familyPressureManager = ServiceLocator.Get<FamilyPressureManager>();
        if (familyPressureManager != null)
        {
            familyPressureManager.DailyUpdate(currentDay);
        }
        
        // Ensure DayProgressionManager starts the shift for the current day
        if (dayProgressionManager != null)
        {
            Debug.Log($"[GameManager] Starting shift for day {dayProgressionManager.CurrentDay}");
            dayProgressionManager.StartShift();
        }
        
        // Initialize timer - ShiftTimerManager will start via events
        var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
        if (shiftTimerManager != null)
        {
            // Check if timer is active - if not, manually start it as fallback
            if (!shiftTimerManager.IsTimerActive)
            {
                Debug.Log($"[GameManager] Timer not active - manually starting as fallback");
                shiftTimerManager.StartTimer();
                remainingTime = shiftTimerManager.RemainingTime;
            }
            else
            {
                Debug.Log($"[GameManager] Timer already active via events");
                remainingTime = shiftTimerManager.RemainingTime;
            }
        }
        else
        {
            // Fallback to legacy timer (DEPRECATED - ShiftTimerManager should always be used)
            const float fallbackTimeLimit = 30f;
            remainingTime = fallbackTimeLimit;
            Debug.Log($"[GameManager] Using legacy timer system: {fallbackTimeLimit}s");
        }
        
        gameActive = true;
        isPaused = false;
        currentGameState = GameState.Gameplay;
        
        // Hide end of day report
        if (dailyReportPanel) dailyReportPanel.SetActive(false);
        
        // Force UI refresh to ensure HUD updates after day transitions
        Debug.Log($"[GameManager] StartDay() - Force refreshing UI for day {currentDay}");
        UpdateUI();
        
        // Update credential checker log book with today's rules
        if (logBookManager != null)
        {
            logBookManager.UpdateContent();
        }
        
        // Show daily briefing with new rules
        ShowDailyBriefing();
        
        // Update UI
        UpdateUI();
        
        // Start spawning ships
        StartCoroutine(SpawnNextShip());
        
        // Random chance for moral choice event during day
        if (Random.value < 0.3f) // 30% chance each day
        {
            float randomTime = Random.Range(60f, remainingTime - 60f);
            StartCoroutine(TriggerMoralChoiceEvent(randomTime));
        }
    }

    // Spawn the next ship after a delay
    IEnumerator SpawnNextShip()
    {
        yield return new WaitForSeconds(timeBetweenShips);
        
        if (gameActive && !isPaused && currentGameState == GameState.Gameplay)
        {
            // Use centralized request instead of direct generation
            RequestNextEncounter("SpawnNextShip Coroutine");
        }
    }

    // Shows the daily briefing with new rules
    void ShowDailyBriefing()
    {
        if (dailyBriefingText == null) return;
        
        string briefing = $"<b>DAY {currentDay} BRIEFING</b>\n\n";
        
        // Get today's rules from the ShipEncounterSystem
        ShipEncounterSystem ses = ShipEncounterSystem.Instance;
        if (ses != null)
        {
            // Add daily access codes
            briefing += "<b>TODAY'S ACCESS CODES:</b> ";
            foreach (var code in ses.validAccessCodes)
            {
                briefing += code + " ";
            }
            
            // Add special instructions for the day
            string[] possibleInstructions = new string[]
            {
                "Be vigilant for Insurgent sympathisers.",
                "All shipments from Bounty Hunters require additional inspection.",
                "Report any suspicious cargo manifests immediately.",
                "The Order has requested increased security protocols.",
                "Orion shuttles have priority clearance today.",
                "Watch for smugglers using stolen access codes."
            };
            
            briefing += "\n\n<b>SPECIAL INSTRUCTIONS:</b>\n";
            briefing += possibleInstructions[Random.Range(0, possibleInstructions.Length)];
            
            // Add daily quota info
            briefing += $"\n\n<b>DAILY QUOTA:</b> {requiredShipsPerDay} ships";
        }
        
        dailyBriefingText.text = briefing;
        
        // Show briefing for a few seconds then hide
        StartCoroutine(ShowBriefingThenHide(8f));
    }
    
    // Show briefing then hide after delay
    IEnumerator ShowBriefingThenHide(float delay)
    {
        // Pause the game while showing briefing
        isPaused = true;
        
        // Show briefing panel (you'll need to create this)
        GameObject briefingPanel = GameObject.Find("BriefingPanel");
        if (briefingPanel) briefingPanel.SetActive(true);
        
        yield return new WaitForSeconds(delay);
        
        // Hide briefing panel
        if (briefingPanel) briefingPanel.SetActive(false);
        
        // Unpause the game
        isPaused = false;
        
        // Set state to gameplay
        currentGameState = GameState.Gameplay;
        
        Debug.Log($"Daily briefing complete. Setting game state to Gameplay. Current day: {currentDay}");
    }
    
    // Trigger a moral choice event after a random delay
    IEnumerator TriggerMoralChoiceEvent(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (gameActive && !isPaused && currentGameState == GameState.Gameplay)
        {
            PresentMoralChoice();
        }
    }
    
    /// <summary>
    /// Called when the daily briefing is completed
    /// </summary>
    public void OnDailyBriefingCompleted()
    {
        Debug.Log("GameManager: Daily briefing completed, starting shift for current day (NOT incrementing day)");
        
        // Notify GameStateController that we're continuing the current day
        GameStateController controller = GameStateController.Instance;
        if (controller != null)
        {
            controller.OnDayBriefingComplete();
        }
        
        // CRITICAL: Also update the new GameStateManager for state synchronization
        var gameStateManager = ServiceLocator.Get<Starkiller.Core.Managers.GameStateManager>();
        if (gameStateManager != null)
        {
            Debug.Log("GameManager: Updating new GameStateManager - transitioning through DailyBriefing to Gameplay");
            // Must transition through DailyBriefing state first due to validation rules
            if (gameStateManager.CurrentState == Starkiller.Core.GameState.MainMenu)
            {
                gameStateManager.ChangeState(Starkiller.Core.GameState.DailyBriefing);
            }
            gameStateManager.ChangeState(Starkiller.Core.GameState.Gameplay);
        }
        
        // CRITICAL: Start the shift for the current day
        if (dayProgressionManager != null)
        {
            Debug.Log("GameManager: Starting shift via DayProgressionManager");
            dayProgressionManager.StartShift();
        }
        else
        {
            Debug.LogError("GameManager: DayProgressionManager not found - cannot start shift!");
        }
        
        // Set game state to gameplay
        currentGameState = GameState.Gameplay;
        gameActive = true;
        
        // Start the shift for the current day (don't increment day number)
        if (dayProgressionManager != null)
        {
            Debug.Log($"GameManager: Starting shift for current day {dayProgressionManager.CurrentDay}");
            dayProgressionManager.StartShift();
        }
        
        // Update UI
        UpdateUI();
        
        // Trigger the first encounter instead of generating it immediately
        FirstEncounterTrigger firstEncounterTrigger = FindFirstObjectByType<FirstEncounterTrigger>();
        if (firstEncounterTrigger != null)
        {
            Debug.Log("GameManager: Notifying FirstEncounterTrigger");
            firstEncounterTrigger.OnDailyBriefingCompleted();
        }
    }

    // Present a moral choice event to the player
    void PresentMoralChoice()
    {
        // Pause the game
        isPaused = true;
        
        if (moralChoicePanel == null) 
        {
            Debug.LogWarning("Moral choice panel not assigned!");
            isPaused = false;
            return;
        }
        
        // Set up the moral choice scenario
        TMP_Text choiceText = moralChoicePanel.GetComponentInChildren<TMP_Text>();
        Button[] choiceButtons = moralChoicePanel.GetComponentsInChildren<Button>();
        
        if (choiceText != null && choiceButtons.Length >= 2)
        {
            // Pick a random moral scenario
            string[] scenarios = new string[]
            {
                "A ship captain discreetly offers you 20 credits to overlook an expired access code. He claims it's for medical supplies urgently needed on the base.",
                "You recognise a passenger on board as a childhood friend. Their papers seem in order but the system marks them as a potential rebel sympathiser.",
                "An Imperium officer requests you expedite their clearance despite questionable manifest details. Denying them could affect your standing with superiors."
            };
            
            // Set scenario text
            choiceText.text = scenarios[Random.Range(0, scenarios.Length)];
            
            // Set button listeners
            choiceButtons[0].onClick.RemoveAllListeners();
            choiceButtons[1].onClick.RemoveAllListeners();
            
            // Option 1 typically represents "follow the rules" choice
            choiceButtons[0].onClick.AddListener(() => ResolveMoralChoice(1));
            
            // Option 2 typically represents "bend the rules" choice
            choiceButtons[1].onClick.AddListener(() => ResolveMoralChoice(2));
            
            // Show the panel
            moralChoicePanel.SetActive(true);
        }
        else
        {
            // If UI elements not found, just unpause
            isPaused = false;
        }
    }
    
    // Resolve the moral choice
    void ResolveMoralChoice(int option)
    {
        // Hide the panel
        if (moralChoicePanel) moralChoicePanel.SetActive(false);
        
        // Process the player's choice
        switch (option)
        {
            case 1: // "Follow the rules" option
                imperialLoyalty += 2;
                rebellionSympathy -= 1;
                keyDecisions.Add("Followed imperium protocols");
                break;
                
            case 2: // "Bend the rules" option
                imperialLoyalty -= 1;
                rebellionSympathy += 2;
                keyDecisions.Add("Bent the rules for personal reasons");
                
                // If the choice involved a bribe, add credits
                if (moralChoicePanel.GetComponentInChildren<TMP_Text>().text.Contains("credits"))
                {
                    credits += 20;
                }
                break;
        }
        
        // Add to narrative tracker
        string decisionId = "moral_choice_" + System.Guid.NewGuid().ToString();
        string context = "Moral choice during shift";
        
        switch (option)
        {
            case 1: // Follow rules
                NarrativeManager.Instance.RecordDecision(decisionId, 20, -10, context + " - followed protocol");
                break;
            case 2: // Bend rules
                NarrativeManager.Instance.RecordDecision(decisionId, -10, 20, context + " - bent rules");
                break;
        }

        // Unpause the game
        isPaused = false;
    }

    // Spawn the next ship after a delay
    public void RequestNextEncounter(string requestSource = "Unknown")
    {
        // Use the EncounterFlowManager if available
        EncounterFlowManager flowManager = EncounterFlowManager.Instance;
        if (flowManager != null)
        {
            Debug.Log($"[GameManager] Delegating encounter request from '{requestSource}' to EncounterFlowManager");
            flowManager.RequestNextEncounter(requestSource);
            return;
        }
        
        // Fallback to original behavior if no flow manager
        Debug.LogWarning("[GameManager] EncounterFlowManager not found, using direct generation");
        GenerateNewShipEncounter();
    }

    // Updates UI elements with current game state
    void UpdateUI()
    {
        // Initialize dayProgressionManager if null
        if (dayProgressionManager == null)
        {
            dayProgressionManager = FindFirstObjectByType<Starkiller.Core.Managers.DayProgressionManager>();
            if (dayProgressionManager != null)
            {
                Debug.Log($"[GameManager] UpdateUI: Found and cached DayProgressionManager reference");
            }
            else
            {
                Debug.LogError("[GameManager] UpdateUI: Could not find DayProgressionManager in scene!");
            }
        }
        
        // Always sync with DayProgressionManager - it's the single source of truth
        var dayProgression = dayProgressionManager ?? FindFirstObjectByType<Starkiller.Core.Managers.DayProgressionManager>();
        if (dayProgression != null)
        {
            shipsProcessed = dayProgression.ShipsProcessedToday;
            requiredShipsPerDay = dayProgression.GetCurrentDayQuota();
            
            Debug.Log($"[GameManager] UpdateUI - Day: {dayProgression.CurrentDay}, Ships: {shipsProcessed}/{requiredShipsPerDay}");
        }
        
        // Also sync strikes with DecisionTracker - it's the source of truth for strikes
        var tracker = decisionTracker ?? FindFirstObjectByType<Starkiller.Core.Managers.DecisionTracker>();
        if (tracker != null)
        {
            currentStrikes = tracker.CurrentStrikes;
            Debug.Log($"[GameManager] UpdateUI - Synced strikes from DecisionTracker: {currentStrikes}");
        }
        
        // Update game status text
        if (gameStatusText && dayProgression != null)
        {
            gameStatusText.text = $"Day: {dayProgression.CurrentDay} | Ships: {shipsProcessed}/{requiredShipsPerDay} | Credits: {credits} | Strikes: {currentStrikes}/{maxMistakes}";
            Debug.Log($"[GameManager] UpdateUI: Day {dayProgression.CurrentDay}, Ships {shipsProcessed}/{requiredShipsPerDay}, Credits {credits}, Strikes {currentStrikes}/{maxMistakes}");
        }
        else if (gameStatusText != null && dayProgression == null)
        {
            // Fallback when DayProgressionManager is not available
            gameStatusText.text = $"Day: 1 | Ships: {shipsProcessed}/{requiredShipsPerDay} | Credits: {credits} | Strikes: {currentStrikes}/{maxMistakes}";
            Debug.LogWarning("[GameManager] UpdateUI: Using fallback day value - DayProgressionManager not found!");
        }
        else
        {
            Debug.LogWarning("[GameManager] UpdateUI: gameStatusText is null! UI references may not be assigned.");
        }
    }
    
    /// <summary>
    /// Force UI refresh - call this if UI seems stuck
    /// </summary>
    [ContextMenu("Force UI Refresh")]
    public void ForceUIRefresh()
    {
        Debug.Log($"[GameManager] ForceUIRefresh called - Current state: Day {currentDay}, Ships {shipsProcessed}, Credits {credits}, Strikes {currentStrikes}");
        UpdateUI();
    }
    
    [ContextMenu("Test Day Transition")]
    public void TestDayTransition()
    {
        Debug.Log("[GameManager] Testing day transition...");
        StartNextDay();
    }
    
    [ContextMenu("Force Timer Check")]
    public void ForceTimerCheck()
    {
        var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
        if (shiftTimerManager != null)
        {
            Debug.Log($"[GameManager] Timer Status: Active={shiftTimerManager.IsTimerActive}, Remaining={shiftTimerManager.RemainingTime}s");
            if (!shiftTimerManager.IsTimerActive)
            {
                Debug.Log("[GameManager] Timer inactive - starting it now");
                shiftTimerManager.StartTimer();
            }
        }
        else
        {
            Debug.LogError("[GameManager] ShiftTimerManager not found!");
        }
    }
    
    // Add a key decision to the decision history
    public void AddKeyDecision(string decision)
    {
        if (!string.IsNullOrEmpty(decision))
        {
            keyDecisions.Add(decision);
            Debug.Log($"Key decision added: {decision}");
        }
    }

    // Called when the player makes a decision via the CredentialChecker
    public void OnDecisionMade(bool approved, bool correctDecision)
    {
        Debug.Log($"=== [DECISION] GameManager.OnDecisionMade() - Approved: {approved}, Correct: {correctDecision} ===");

        if (!gameActive || isPaused) return;

        // Define these variables at the beginning of the method for scope access
        int creditPenalty = 0;
        int casualtyCount = 0;

        // Initialize decisionTracker if null
        if (decisionTracker == null)
        {
            decisionTracker = Starkiller.Core.ServiceLocator.Get<DecisionTracker>();
            if (decisionTracker != null)
            {
                Debug.Log("[GameManager] Found and cached DecisionTracker reference");
            }
        }
        
        // Use refactored managers if available, otherwise fallback to old system
        if (decisionTracker != null)
        {
            // Use new refactored system
            string reason = correctDecision ? "Correct decision" : "Incorrect decision";
            DecisionType decisionType = approved ? DecisionType.Approve : DecisionType.Deny;
            
            decisionTracker.RecordDecision(correctDecision, reason, decisionType);
            Debug.Log($"[GameManager] Decision recorded via DecisionTracker: {reason}");
        }
        else
        {
            // Fallback to old system
            if (correctDecision)
            {
                // Player made the right decision
                score += 10;
                correctDecisions++;
                Debug.Log("Correct decision! Adding 10 points to score.");
            }
            else
            {
                // Player made a mistake
                currentStrikes++;
                score -= 5;
                wrongDecisions++;
                
                Debug.Log($"Incorrect decision! Strike added. Total strikes: {currentStrikes}/{maxMistakes}");
                
                if (currentStrikes >= maxMistakes)
                {
                    GameOver("You have been terminated due to excessive errors.");
                    return;
                }
            }
        }

        // Process ship count using refactored manager if available
        if (dayProgressionManager != null)
        {
            // The DayProgressionManager handles ship counting via GameEvents.TriggerDecisionMade() 
            // which calls OnDecisionCompleted() -> RecordShipProcessed()
            // No need to call RecordShipProcessed() here as it would double-count
            Debug.Log("[GameManager] Ship processing delegated to DayProgressionManager via GameEvents");
        }
        else
        {
            // Fallback to old system
            shipsProcessed++; // Daily counter for quota tracking
            totalShipsProcessed++; // Total counter for final score
            Debug.Log($"=== [SHIP PROCESSED] Daily: {shipsProcessed}/{requiredShipsPerDay}, Total: {totalShipsProcessed} ===");
        }

        // Handle credit penalties from wrong decisions
        if (!correctDecision)
        {
            // Try to get penalty information from the current ship
            if (currentShip != null)
            {
                // For MasterShipEncounter
                if (currentShip is MasterShipEncounter)
                {
                    MasterShipEncounter masterShip = currentShip as MasterShipEncounter;
                    creditPenalty = masterShip.creditPenalty;
                    casualtyCount = masterShip.casualtiesIfWrong;
                }
                // Legacy ShipEncounter - get penalties if available
                else if (currentShip.GetType().GetProperty("creditPenalty") != null)
                {
                    creditPenalty = (int)currentShip.GetType().GetProperty("creditPenalty").GetValue(currentShip, null);
                    
                    if (currentShip.GetType().GetProperty("casualtiesIfWrong") != null)
                    {
                        casualtyCount = (int)currentShip.GetType().GetProperty("casualtiesIfWrong").GetValue(currentShip, null);
                    }
                }
            }

            Debug.Log($"Decision penalties: {creditPenalty} credits, {casualtyCount} casualties");

            // Apply credit penalty using refactored manager if available
            if (creditPenalty > 0)
            {
                if (creditsManager != null)
                {
                    creditsManager.DeductCredits(creditPenalty, "Incorrect decision penalty");
                    Debug.Log($"[GameManager] Credits deducted via CreditsManager: -{creditPenalty}");
                }
                else
                {
                    // Fallback to old system
                    credits -= creditPenalty;
                    Debug.Log($"Credits deducted: -{creditPenalty} (New balance: {credits})");
                }
            }

            // Apply casualties if you have a system for that
            if (casualtyCount > 0)
            {
                Debug.Log($"Casualties incurred: {casualtyCount}");
                totalCasualties += casualtyCount;
            }

            // Record consequence with ConsequenceManager
            if (consequenceManager != null && currentShip != null)
            {
                // Handle consequence recording as before
            }
        }
        
        // Now that we have the penalties, record the incident with the ConsequenceManager
        if (consequenceManager != null && currentShip != null)
        {
            if (currentShip is MasterShipEncounter)
            {
                consequenceManager.RecordIncident(currentShip as MasterShipEncounter, approved);
            }
            else
            {
                // For legacy ship encounters, create a temp MasterShipEncounter with the penalties
                MasterShipEncounter tempEncounter = new MasterShipEncounter();
                tempEncounter.creditPenalty = creditPenalty;
                tempEncounter.casualtiesIfWrong = casualtyCount;
                
                // Copy other important properties if accessible
                if (currentShip.GetType().GetProperty("isStoryShip") != null)
                {
                    tempEncounter.isStoryShip = (bool)currentShip.GetType().GetProperty("isStoryShip").GetValue(currentShip, null);
                    
                    if (currentShip.GetType().GetProperty("storyTag") != null)
                    {
                        tempEncounter.storyTag = (string)currentShip.GetType().GetProperty("storyTag").GetValue(currentShip, null);
                    }
                }
                
                consequenceManager.RecordIncident(tempEncounter, approved);
            }
        }

        // Increment the ship counter
        if (shipsProcessed == requiredShipsPerDay && !isTransitioningDay && currentGameState == GameState.Gameplay)
        {
            // Show feedback to player that quota is reached, but continue gameplay
            if (credentialChecker != null)
            {
                credentialChecker.ShowFeedback("Daily quota reached! Continue processing ships until shift ends.", Color.green);
            }

            // Play quota reached sound
            if (quotaReachedSound != null)
            {
                quotaReachedSound.Play();
            }

            Debug.Log("Daily quota reached, but continuing until shift time ends.");
        }


        /*
        // Check if we need to transition to the next day
        if (shipsProcessed >= requiredShipsPerDay && !isTransitioningDay)
        {
            // Only transition if we're in gameplay state, not already in a transition
            if (currentGameState == GameState.Gameplay)
            {
                Debug.Log("Daily quota reached. Ending day.");
                // Trigger a day end sequence
                EndCurrentDay();
                return;
            }
        }
        */

        // Record the decision for story purposes
        if (currentShip != null)
        {
            // Check what type of ship we're dealing with and handle accordingly
            if (currentShip is MasterShipEncounter masterShip)
            {
                // For MasterShipEncounter
                if (masterShip.isStoryShip)
                {
                    if (approved)
                    {
                        if (masterShip.storyTag == "insurgent")
                        {
                            rebellionSympathy += 1;
                            imperialLoyalty -= 1;
                            keyDecisions.Add("Approved an insurgent sympathiser");
                        }
                        else if (masterShip.storyTag == "imperium")
                        {
                            imperialLoyalty += 1;
                            keyDecisions.Add("Supported high-ranking Imperium officer");
                        }
                    }
                    else
                    {
                        if (masterShip.storyTag == "insurgent")
                        {
                            imperialLoyalty += 1;
                            keyDecisions.Add("Denied access to an insurgent sympathiser");
                        }
                        else if (masterShip.storyTag == "imperium")
                        {
                            imperialLoyalty -= 1;
                            keyDecisions.Add("Denied a high-ranking Imperium officer");
                        }
                    }
                }
            }
            else if (currentShip is ShipEncounter legacyShip)
            {
                // For legacy ShipEncounter objects
                if (legacyShip.isStoryShip)
                {
                    if (approved)
                    {
                        if (legacyShip.storyTag == "insurgent")
                        {
                            rebellionSympathy += 1;
                            imperialLoyalty -= 1;
                            keyDecisions.Add("Approved an insurgent sympathiser");
                        }
                        else if (legacyShip.storyTag == "imperium")
                        {
                            imperialLoyalty += 1;
                            keyDecisions.Add("Supported high-ranking Imperium officer");
                        }
                    }
                    else
                    {
                        if (legacyShip.storyTag == "insurgent")
                        {
                            imperialLoyalty += 1;
                            keyDecisions.Add("Denied access to an insurgent sympathiser");
                        }
                        else if (legacyShip.storyTag == "imperium")
                        {
                            imperialLoyalty -= 1;
                            keyDecisions.Add("Denied a high-ranking Imperium officer");
                        }
                    }
                }
            }
        }
        
        // Clear the current ship reference
        currentShip = null;
        
        // Update UI before spawning next ship
        UpdateUI();

        // Notify the flow manager about the decision
        EncounterFlowManager flowManager = EncounterFlowManager.Instance;
        if (flowManager != null)
        {
            flowManager.OnDecisionMade();
            // Flow manager will handle the next encounter request after cooldown
        }
        else
        {
            // Fallback to timing controller
            ShipTimingController timingController = ShipTimingController.Instance;
            if (timingController != null)
            {
                timingController.OnShipProcessed();
                StartCoroutine(SpawnNextShipWithTimingController());
            }
            else
            {
                // Fall back to original behavior if no managers found
                StartCoroutine(SpawnNextShip());
            }
        }

        // And add this new coroutine:
        IEnumerator SpawnNextShipWithTimingController()
        {
            ShipTimingController timingController = ShipTimingController.Instance;
            
            // Wait until the timing controller says we can proceed
            while (timingController != null && !timingController.CanGenerateNewShip())
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            // Now we can generate the new encounter
            if (gameActive && !isPaused && currentGameState == GameState.Gameplay)
            {
                RequestNextEncounter("SpawnNextShip Coroutine");
                UpdateUI();
            }
        }
    }
    
    // Handle accepting a bribe
    public void OnAcceptBribeClicked()
    {
        if (!gameActive || isPaused || currentShip == null) return;
        
        // Convert to MasterShipEncounter if possible
        if (currentShip is MasterShipEncounter masterShip)
        {
            if (masterShip.offersBribe)
            {
                Debug.Log($"Accepting bribe of {masterShip.bribeAmount} credits");
                
                // Add credits
                credits += masterShip.bribeAmount;
                rebellionSympathy += 1;
                imperialLoyalty -= 1;
                keyDecisions.Add("Accepted a bribe of " + masterShip.bribeAmount + " credits");
                
                // Process the bribe in MasterShipGenerator
                if (masterShipGenerator != null)
                {
                    masterShipGenerator.ProcessBriberyAccepted(masterShip);
                }
                
                // Update UI
                UpdateUI();
                return;
            }
        }
        else if (currentShip is ShipEncounter legacyShip)
        {
            if (legacyShip.offersBribe)
            {
                Debug.Log($"Accepting bribe of {legacyShip.bribeAmount} credits");
                
                // Add credits
                credits += legacyShip.bribeAmount;
                rebellionSympathy += 1;
                imperialLoyalty -= 1;
                keyDecisions.Add("Accepted a bribe of " + legacyShip.bribeAmount + " credits");
                
                // Update UI
                UpdateUI();
            }
        }
    }

    // Handle putting ship in holding pattern
    public void OnHoldingPatternClicked()
    {
        if (!gameActive || isPaused || currentShip == null) return;
        
        Debug.Log("Putting ship in holding pattern");
        
        // Convert to MasterShipEncounter if possible
        if (currentShip is MasterShipEncounter masterShip)
        {
            // Check if we have the HoldingPatternProcessor
            HoldingPatternProcessor holdingProcessor = FindFirstObjectByType<HoldingPatternProcessor>();
            
            if (holdingProcessor != null)
            {
                // Try to add the ship to the holding pattern
                bool added = holdingProcessor.AddShipToHoldingPattern(masterShip);
                
                if (added)
                {
                    // Process the holding pattern in MasterShipGenerator
                    if (masterShipGenerator != null)
                    {
                        masterShipGenerator.ProcessHoldingPattern(masterShip);
                    }
                    
                    // Clear currentShip reference
                    currentShip = null;
                }
                else
                {
                    // Show feedback that holding pattern is full
                    if (credentialChecker != null)
                    {
                        credentialChecker.ShowFeedback("Holding pattern is at maximum capacity", Color.red);
                    }
                }
            }
            else
            {
                // Fall back to old processing if no processor found
                if (masterShipGenerator != null)
                {
                    masterShipGenerator.ProcessHoldingPattern(masterShip);
                }
                
                // Show feedback
                if (credentialChecker != null)
                {
                    credentialChecker.ShowFeedback("Ship placed in holding pattern", Color.cyan);
                }
                
                // Clear currentShip reference
                currentShip = null;
            }
        }
        else
        {
            // Legacy handling for non-MasterShipEncounter
            // Show feedback
            if (credentialChecker != null)
            {
                credentialChecker.ShowFeedback("Ship placed in holding pattern", Color.cyan);
            }
        }
    }


    // Handle using tractor beam
    public void OnTractorBeamClicked()
    {
        if (!gameActive || isPaused || currentShip == null) return;
        
        Debug.Log("Activating tractor beam");
        
        // Convert to MasterShipEncounter if possible
        if (currentShip is MasterShipEncounter masterShip)
        {
            if (masterShip.canBeCaptured || (masterShip.isStoryShip && masterShip.storyTag == "insurgent"))
            {
                // Process the tractor beam in MasterShipGenerator
                if (masterShipGenerator != null)
                {
                    masterShipGenerator.ProcessTractorBeam(masterShip);
                }
                
                // Show feedback
                if (credentialChecker != null)
                {
                    credentialChecker.ShowFeedback("Ship captured with tractor beam", new Color(0.4f, 0.8f, 1f));
                }
            }
            else
            {
                // Show error feedback
                if (credentialChecker != null)
                {
                    credentialChecker.ShowFeedback("This ship cannot be captured", Color.red);
                }
            }
        }
    }
    
    // End the current day and prepare for the next day
    private void EndCurrentDay()
    {
        // Change game state to prevent more processing
        currentGameState = GameState.DayEnd;
        
        // CRITICAL: Also update the new GameStateManager for state synchronization
        var gameStateManager = ServiceLocator.Get<Starkiller.Core.Managers.GameStateManager>();
        if (gameStateManager != null)
        {
            Debug.Log("GameManager: Updating new GameStateManager to DayReport state");
            gameStateManager.ChangeState(Starkiller.Core.GameState.DayReport);
        }
        
        // Stop current gameplay
        gameActive = false;
        
        // Use RefactoredDailyReportManager instead of old DailyReportManager
        if (refactoredDailyReportManager != null)
        {
            Debug.Log("[GameManager] Using RefactoredDailyReportManager to generate daily report");
            refactoredDailyReportManager.GenerateDailyReport();
        }
        else
        {
            Debug.LogError("[GameManager] RefactoredDailyReportManager not available - cannot show daily report!");
        }
        
        Debug.Log($"EndCurrentDay called - Day {currentDay} completed with {shipsProcessed} ships processed");
        
        // Day will advance when the player confirms on the summary screen
        // This prevents auto-advancing without player interaction
    }
    
    // Creates a new random ship encounter
    void GenerateNewShipEncounter()
    {
        // First check if we have the MasterShipGenerator (new system)
        MasterShipGenerator masterGenerator = FindFirstObjectByType<MasterShipGenerator>();
        if (masterGenerator != null)
        {
            Debug.Log("GameManager: Using MasterShipGenerator for encounter generation");
            
            // Get the next encounter from the master generator
            MasterShipEncounter masterEncounter = masterGenerator.GetNextEncounter();
            
            // Store a reference to track in our decision handling
            if (masterEncounter != null)
            {
                // We need to keep track of this for story decisions
                currentShip = masterEncounter;
                
                Debug.Log($"Generated new ship: {masterEncounter.shipType} - {masterEncounter.captainName}");
                
                // Display the encounter if we have a credential checker
                if (credentialChecker != null)
                {
                    credentialChecker.DisplayEncounter(masterEncounter);
                }
            }
            else
            {
                Debug.LogWarning("MasterShipGenerator returned null encounter!");
            }
            
            return;
        }
        
        // If we get here, we don't have MasterShipGenerator - use legacy systems
        
        // Try ShipEncounterGenerator first (second best option)
        if (shipEncounterGenerator != null)
        {
            currentShip = shipEncounterGenerator.GenerateRandomEncounter();
            
            if (currentShip is MasterShipEncounter masterEncounter)
            {
                credentialChecker.DisplayEncounter(masterEncounter);
            }
            else
            {
                // For other encounter types, leave the original handling
                credentialChecker.DisplayEncounter(currentShip as ShipEncounter);
            }
            return;
        }
        
        // Try ShipGeneratorCoordinator next
        ShipGeneratorCoordinator coordinator = ShipGeneratorCoordinator.Instance;
        if (coordinator != null)
        {
            currentShip = coordinator.GenerateShipEncounter(currentDay, imperialLoyalty, rebellionSympathy);
            
            // Display the encounter
            if (currentShip is MasterShipEncounter masterEncounter)
            {
                credentialChecker.DisplayEncounter(masterEncounter);
            }
            else if (currentShip is ShipEncounter shipEncounter)
            {
                credentialChecker.DisplayEncounter(shipEncounter);
            }
            else if (currentShip is VideoEnhancedShipEncounter videoEncounter)
            {
                credentialChecker.DisplayEncounter(videoEncounter);
            }
            else if (currentShip is EnhancedShipEncounter enhancedEncounter)
            {
                credentialChecker.DisplayEncounter(enhancedEncounter);
            }
            return;
        }
        
        // Fall back to the legacy system as last resort
        ShipEncounterSystem ses = ShipEncounterSystem.Instance;
        if (ses != null)
        {
            currentShip = ses.GenerateEncounter(currentDay, imperialLoyalty, rebellionSympathy);
            
            // Display the encounter in the credential checker
            if (currentShip is MasterShipEncounter masterEncounter)
            {
                credentialChecker.DisplayEncounter(masterEncounter);
            }
            else if (currentShip is ShipEncounter shipEncounter)
            {
                credentialChecker.DisplayEncounter(shipEncounter);
            }
            else if (currentShip is VideoEnhancedShipEncounter videoEncounter)
            {
                credentialChecker.DisplayEncounter(videoEncounter);
            }
            else if (currentShip is EnhancedShipEncounter enhancedEncounter)
            {
                credentialChecker.DisplayEncounter(enhancedEncounter);
            }
        }
        else
        {
            Debug.LogError("Cannot generate new ship - no generator systems found!");
            
            // Create a fallback encounter as a last resort
            MasterShipEncounter fallback = MasterShipEncounter.CreateTestEncounter();
            currentShip = fallback;
            
            // Display the fallback encounter
            credentialChecker.DisplayEncounter(fallback);
        }
    }
    
    // Calculate daily salary
    int CalculateSalary()
    {
        int salary = baseSalary;
        
        // Get actual ship count from DayProgressionManager if available
        int actualShipsProcessed = shipsProcessed;
        if (dayProgressionManager != null)
        {
            actualShipsProcessed = dayProgressionManager.ShipsProcessedToday;
        }
        
        // Add bonus for exceeding quota
        if (actualShipsProcessed > requiredShipsPerDay)
        {
            int extraShips = actualShipsProcessed - requiredShipsPerDay;
            salary += extraShips * bonusPerExtraShip;
        }
        
        // Subtract penalty for mistakes
        salary -= currentStrikes * penaltyPerMistake;
        
        // Ensure salary doesn't go negative
        salary = Mathf.Max(0, salary);
        
        return salary;
    }
    
    // Show the end of day report
    void ShowDailyReport(int salary)
    {
        Debug.Log("ShowDailyReport called with salary: " + salary);
        
        if (dailyReportPanel == null)
        {
            Debug.LogError("Cannot show daily report - dailyReportPanel is null!");
            return;
        }
        
        // Create a dictionary to store expenses
        Dictionary<string, int> expenses = CalculateExpenses();
        
        // Calculate total expenses using LINQ
        int totalExpenses = expenses.Values.Sum();
        Debug.Log($"Total expenses calculated: {totalExpenses}");
        
        // Use BOTH systems: original DailyReportManager for UI, RefactoredDailyReportManager for logic
        
        // First, use the original DailyReportManager to display the UI with actual GameManager data
        DailyReportManager originalDailyReportManager = FindFirstObjectByType<DailyReportManager>();
        if (originalDailyReportManager != null)
        {
            Debug.Log("[GameManager] Using original DailyReportManager for UI display");
            
            // Calculate family status
            FamilyStatusInfo familyStatus = null;
            if (familySystem != null)
            {
                familyStatus = new FamilyStatusInfo(); // Create basic family status
            }
            
            // Show the properly populated daily report UI
            originalDailyReportManager.ShowDailyReport(
                currentDay,           // day
                credits,             // currentCredits (before salary)
                salary,              // salary
                totalExpenses,       // expenses
                shipsProcessed,      // shipsProcessed
                requiredShipsPerDay, // requiredShips
                currentStrikes,      // currentStrikes
                penaltyPerMistake,   // penaltyPerMistake
                baseSalary,          // baseSalary
                familyStatus,        // familyStatus
                expenses             // expenseBreakdown
            );
        }
        
        // Second, also trigger the RefactoredDailyReportManager for day progression logic (but don't show its UI)
        if (refactoredDailyReportManager != null)
        {
            Debug.Log("[GameManager] Also triggering RefactoredDailyReportManager for day progression logic (no UI)");
            // Generate the report data but don't display it (original DailyReportManager handles UI)
            refactoredDailyReportManager.GenerateDailyReport(false);
        }
        
        // Update credits accounting for income and expenses
        credits += salary;
        credits -= totalExpenses;
        Debug.Log($"Updated credits: {credits} (previous + salary - expenses)");
        
        // Last check to ensure panel is active
        if (dailyReportPanel && !dailyReportPanel.activeSelf)
        {
            Debug.LogError("CRITICAL: DailyReportPanel still not active after all attempts! Forcing it active as final resort.");
            dailyReportPanel.SetActive(true);
        }
        
        // Check for game over due to insufficient credits
        if (credits < 0)
        {
            Debug.Log("Credits below zero, triggering GameOver");
            GameOver("Your family has been reassigned to a remote outpost due to financial mismanagement.");
        }
    }
    
    // Calculate daily expenses
    Dictionary<string, int> CalculateExpenses()
    {
        Dictionary<string, int> expenses = new Dictionary<string, int>();
        
        if (familySystem)
        {
            // Use the family system's expense calculator
            expenses = familySystem.CalculateExpenses();
        }
        else
        {
            // Fallback to default expenses
            expenses.Add("Premium Quarters", premiumQuartersCost);
            expenses.Add("Childcare", childcareCost);
            
            // Add medical care if needed
            if (currentDay % 3 == 0)
            {
                expenses.Add("Medical Care", medicalCareCost);
            }
            
            // Add equipment if needed
            if (currentDay % 4 == 1)
            {
                expenses.Add("Equipment", equipmentCost);
            }
            
            // Add training if needed
            if (currentDay % 5 == 2)
            {
                expenses.Add("Training", trainingCost);
            }
        }
        
        return expenses;
    }
    
    // Start the next day
    public void StartNextDay()
    {
        // Check if game over manager says the game is over
        var gameOverManager = Starkiller.Core.ServiceLocator.Get<Starkiller.Core.Managers.GameOverManager>();
        if (gameOverManager != null && gameOverManager.IsGameOver)
        {
            Debug.LogWarning("StartNextDay() called but game is over. Ignoring.");
            return;
        }
        
        // Prevent multiple calls during transition
        if (isTransitioningDay)
        {
            Debug.LogWarning("StartNextDay() called while already transitioning day. Ignoring.");
            return;
        }
        
        isTransitioningDay = true;
        
        try
        {
            Debug.Log($"[GameManager] StartNextDay() called - delegating to DayProgressionManager");
            
            // DayProgressionManager is now the single source of truth for day management
            if (dayProgressionManager == null)
            {
                dayProgressionManager = FindFirstObjectByType<Starkiller.Core.Managers.DayProgressionManager>();
            }
            
            if (dayProgressionManager != null)
            {
                int previousDay = dayProgressionManager.CurrentDay;
                dayProgressionManager.StartNewDay();
                int newDay = dayProgressionManager.CurrentDay;
                
                Debug.Log($"=== [DAY INCREMENT] Via DayProgressionManager - Day before: {previousDay}, After: {newDay} ===");
                dayIncrementCount++;
            }
            else
            {
                Debug.LogError("[GameManager] DayProgressionManager not found! Cannot progress to next day.");
                isTransitioningDay = false;
                return;
            }
            
            // DON'T notify GameStateController here - that's for starting a brand new game
            // Day transitions during gameplay should not trigger the DayStart->StartGame() flow
            Debug.Log("[GameManager] Day incremented, continuing with existing game session");

            // Get the new day number from DayProgressionManager
            int newDayNumber = dayProgressionManager.CurrentDay;
            
            // Notify MasterShipGenerator of day change (highest priority)
            if (masterShipGenerator != null)
            {
                Debug.Log($"Notifying MasterShipGenerator of new day: {newDayNumber}");
                masterShipGenerator.StartNewDay(newDayNumber);
            }
            
            // Notify content manager of day change
            if (contentManager != null)
            {
                Debug.Log($"Notifying ContentManager of new day: {newDayNumber}");
                contentManager.StartNewDay(newDayNumber);
            }
            
            // Notify legacy ship encounter generator if available
            if (shipEncounterGenerator != null)
            {
                Debug.Log($"Notifying ShipEncounterGenerator of new day: {newDayNumber}");
                shipEncounterGenerator.StartNewDay(newDayNumber);
            }
            
            // DON'T hide the daily report panel - let it stay open for the player
            // The panel will be hidden when the daily briefing starts
            
            // Reset daily counters (but preserve total ship count)
            shipsProcessed = 0; // Daily count reset for new quota
            correctDecisions = 0;
            wrongDecisions = 0;
            // Note: totalShipsProcessed is preserved across days
            
            Debug.Log($"Reset daily counters for new day - Daily ships: {shipsProcessed}, Total ships: {totalShipsProcessed}");
            
            // Clear the encounter display for a fresh start
            CredentialChecker credentialChecker = FindFirstObjectByType<CredentialChecker>();
            if (credentialChecker != null)
            {
                Debug.Log("GameManager: Clearing encounter display for new day");
                credentialChecker.ClearEncounterDisplay();
            }
            
            // Clear the EncounterMediaTransitionManager as well
            var encounterMediaManager = Starkiller.Core.Managers.EncounterMediaTransitionManager.Instance;
            if (encounterMediaManager != null)
            {
                Debug.Log("GameManager: Clearing EncounterMediaTransitionManager for new day");
                encounterMediaManager.ClearForNewDay();
            }
            
            // After advancing the day, start the transition sequence (Video/Inspection → Personal Log → Daily Briefing)
            StartDayTransitionSequence(dayProgressionManager.CurrentDay);
            Debug.Log($"GameManager: Cleared decks and starting day transition sequence for Day {dayProgressionManager.CurrentDay}");
        }
        finally
        {
            // Release the lock after a short delay to prevent rapid successive calls
            StartCoroutine(ReleaseTransitionLock(0.5f));
        }
    }

    private IEnumerator ReleaseTransitionLock(float delay)
    {
        yield return new WaitForSeconds(delay);
        isTransitioningDay = false;
    }
    
    /// <summary>
    /// Start the day transition sequence: Inspection/Video → Personal Log → Daily Briefing
    /// </summary>
    private void StartDayTransitionSequence(int day)
    {
        Debug.Log($"[GameManager] StartDayTransitionSequence called for Day {day}");
        StartCoroutine(DayTransitionSequenceCoroutine(day));
    }
    
    /// <summary>
    /// Coroutine that manages the day transition sequence
    /// </summary>
    private System.Collections.IEnumerator DayTransitionSequenceCoroutine(int day)
    {
        Debug.Log($"[GameManager] Starting day transition sequence for Day {day}");
        
        // Step 1: Check for inspection/video content
        var inspectionManager = ServiceLocator.Get<Starkiller.Core.Managers.InspectionManager>();
        bool inspectionShown = false;
        
        if (inspectionManager != null)
        {
            Debug.Log($"[GameManager] InspectionManager found via ServiceLocator");
            
            // Check if there should be an inspection (could be based on consequences, random chance, etc.)
            if (ShouldShowInspection(day))
            {
                Debug.Log($"[GameManager] Showing inspection for Day {day}");
                bool inspectionComplete = false;
                
                inspectionManager.ShowInspectionUI($"End of Day {day - 1} Security Review", (result) => {
                    inspectionComplete = true;
                    Debug.Log($"[GameManager] Inspection completed with result: {result}");
                });
                
                // Wait for inspection to complete
                while (!inspectionComplete)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                inspectionShown = true;
            }
        }
        else
        {
            Debug.LogWarning("[GameManager] InspectionManager not found in ServiceLocator");
        }
        
        if (!inspectionShown)
        {
            Debug.Log($"[GameManager] No inspection needed for Day {day}");
        }
        
        // Step 2: Show Personal Data Log (consequences, family messages, etc.)
        var personalDataLogManager = ServiceLocator.Get<Starkiller.Core.Managers.PersonalDataLogManager>();
        bool personalLogShown = false;
        
        if (personalDataLogManager != null)
        {
            Debug.Log($"[GameManager] PersonalDataLogManager found via ServiceLocator");
            
            // Check if there are consequences or personal messages to show
            if (ShouldShowPersonalLog(day))
            {
                Debug.Log($"[GameManager] Showing Personal Data Log for Day {day}");
                bool logComplete = false;
                
                // Subscribe to personal log completion
                System.Action onLogClosed = () => {
                    logComplete = true;
                    Debug.Log($"[GameManager] Personal Data Log closed");
                };
                
                Starkiller.Core.Managers.PersonalDataLogManager.OnDataLogClosed += onLogClosed;
                personalDataLogManager.ShowDataLog();
                
                // Wait for personal log to complete
                while (!logComplete)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                
                // Unsubscribe
                Starkiller.Core.Managers.PersonalDataLogManager.OnDataLogClosed -= onLogClosed;
                personalLogShown = true;
            }
        }
        else
        {
            Debug.LogWarning("[GameManager] PersonalDataLogManager not found in ServiceLocator");
        }
        
        if (!personalLogShown)
        {
            Debug.Log($"[GameManager] No personal log content for Day {day}");
        }
        
        // Step 3: Show Daily Briefing
        Debug.Log($"[GameManager] Showing Daily Briefing for Day {day}");
        ShowDailyBriefing(day);
    }
    
    /// <summary>
    /// Determine if inspection should be shown
    /// </summary>
    private bool ShouldShowInspection(int day)
    {
        // Show inspection if there were consequences from previous day, or randomly
        var consequenceManager = FindFirstObjectByType<ConsequenceManager>();
        if (consequenceManager != null)
        {
            var unreportedIncidents = consequenceManager.GetUnreportedIncidents();
            Debug.Log($"[GameManager] Found {unreportedIncidents.Count} unreported incidents");
            if (unreportedIncidents.Count > 0)
            {
                Debug.Log($"[GameManager] Inspection needed due to {unreportedIncidents.Count} unreported incidents");
                return true;
            }
        }
        else
        {
            Debug.LogWarning("[GameManager] ConsequenceManager not found!");
        }
        
        // Random inspection chance (e.g., 20% chance after day 1)
        if (day > 1 && UnityEngine.Random.Range(0f, 1f) < 0.2f)
        {
            Debug.Log($"[GameManager] Random inspection triggered for Day {day}");
            return true;
        }
        
        Debug.Log($"[GameManager] No inspection needed for Day {day}");
        return false;
    }
    
    /// <summary>
    /// Determine if personal data log should be shown
    /// </summary>
    private bool ShouldShowPersonalLog(int day)
    {
        // Always show personal log after day 1 (could have family messages, consequences, etc.)
        if (day > 1)
        {
            Debug.Log($"[GameManager] Personal log needed for Day {day}");
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Show the Daily Briefing for the specified day
    /// </summary>
    private void ShowDailyBriefing(int day)
    {
        Debug.Log($"[GameManager] ShowDailyBriefing called for Day {day}");
        
        // Find and trigger the Daily Briefing Manager
        DailyBriefingManager dailyBriefingManager = FindFirstObjectByType<DailyBriefingManager>();
        if (dailyBriefingManager != null)
        {
            Debug.Log($"[GameManager] Found DailyBriefingManager - showing briefing for Day {day}");
            dailyBriefingManager.ShowDailyBriefing(day);
        }
        else
        {
            Debug.LogError("[GameManager] DailyBriefingManager not found - cannot show daily briefing!");
            
            // Fallback: Start the day directly if no briefing manager
            Debug.Log("[GameManager] Fallback: Starting day directly without briefing");
            StartDay();
        }
    }

    // Ends the game
    public void GameOver(string reason)
    {
        // Set game state
        currentGameState = GameState.GameOver;
        
        // CRITICAL: Also update the new GameStateManager for state synchronization
        var gameStateManager = ServiceLocator.Get<Starkiller.Core.Managers.GameStateManager>();
        if (gameStateManager != null)
        {
            Debug.Log("GameManager: Updating new GameStateManager to GameOver state");
            gameStateManager.ChangeState(Starkiller.Core.GameState.GameOver);
        }
        
        gameActive = false;
        
        Debug.Log("GAME OVER! " + reason);
        
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            
            // Update final score
            if (finalScoreText)
            {
                finalScoreText.text = $"GAME OVER\n\n{reason}\n\nFinal Score: {score}\nDays Survived: {currentDay}\nTotal Ships Processed: {shipsProcessed}";
                
                // Add ending based on loyalty/rebellion sympathy
                if (imperialLoyalty > 5 && rebellionSympathy < 0)
                {
                    finalScoreText.text += "\n\nYou remained loyal to the Imperium until the end.";
                }
                else if (rebellionSympathy > 5 && imperialLoyalty < 0)
                {
                    finalScoreText.text += "\n\nYour sympathy for the Insurgency has been noted in your permanent record.";
                }
            }
        }
        
        // Notify the UI Manager
        if (uiManager)
        {
            uiManager.GameOver();
        }
    }
    
    /// <summary>
    /// Shows the inspection UI to the player
    /// </summary>
    public void ShowInspectionUI(string inspectionReason = "", System.Action<bool> onInspectionComplete = null)
    {
        isPaused = true;
        
        // Find an existing panel we can use
        GameObject feedbackPanel = GameObject.Find("FeedbackPanel");
        
        if (feedbackPanel == null)
        {
            Debug.LogError("FeedbackPanel not found! Falling back to alternative approach.");
            
            // Call the callback directly since we can't show UI
            isPaused = false;
            if (onInspectionComplete != null)
            {
                bool irregularitiesFound = keyDecisions.Contains("Accepted a bribe") || rebellionSympathy > 3;
                onInspectionComplete.Invoke(irregularitiesFound);
            }
            return;
        }
        
        // Get the feedback text component
        TMP_Text feedbackText = feedbackPanel.GetComponentInChildren<TMP_Text>();
        if (feedbackText == null)
        {
            Debug.LogError("Feedback text component not found!");
            isPaused = false;
            if (onInspectionComplete != null)
            {
                bool irregularitiesFound = keyDecisions.Contains("Accepted a bribe") || rebellionSympathy > 3;
                onInspectionComplete.Invoke(irregularitiesFound);
            }
            return;
        }
        
        // Show the panel
        feedbackPanel.SetActive(true);
        
        // Set the inspection message
        string message = "ATTENTION OFFICER: A surprise inspection of Imperium Command is underway.\n\nAll operations temporarily suspended.";
        if (!string.IsNullOrEmpty(inspectionReason))
        {
            message += $"\n\nREASON: {inspectionReason}";
        }
        feedbackText.text = message;
        
        // Auto-dismiss after delay
        StartCoroutine(DismissInspectionUI(6.0f, onInspectionComplete));
    }

    /// <summary>
    /// Coroutine to dismiss the inspection UI after a delay
    /// </summary>
    private IEnumerator DismissInspectionUI(float delay, System.Action<bool> onInspectionComplete)
    {
        yield return new WaitForSeconds(delay);
        
        // Find the feedback panel
        GameObject feedbackPanel = GameObject.Find("FeedbackPanel");
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
        
        // Resume game
        isPaused = false;
        
        // Determine inspection result and invoke callback
        bool irregularitiesFound = keyDecisions.Contains("Accepted a bribe") || rebellionSympathy > 3;
        if (onInspectionComplete != null)
        {
            onInspectionComplete.Invoke(irregularitiesFound);
        }
    } 

    
    // Check if the game is currently active
    public bool IsGameActive()
    {
        return gameActive;
    }

    // Check if the game is currently paused
    public bool IsPaused()
    {
        return isPaused;
    }

    // Pause the game
    public void PauseGame()
    {
        isPaused = true;
    }

    // Resume the game
    public void ResumeGame()
    {
        isPaused = false;
    }

    // Add credits to the player's balance
    public void AddCredits(int amount)
    {
        credits += amount;
        UpdateUI();
    }

    // Accept a bribe of the given amount
    public void AcceptBribe(int amount)
    {
        // Add credits
        credits += amount;
        rebellionSympathy += 1;
        imperialLoyalty -= 1;
        keyDecisions.Add("Accepted a bribe of " + amount + " credits");
        
        // Update UI
        UpdateUI();
    }

    // Update the player's loyalty and sympathy values
    public void UpdateLoyalty(int imperialChange, int rebellionChange)
    {
        // Update loyalty values
        imperialLoyalty += imperialChange;
        rebellionSympathy += rebellionChange;
        
        // Clamp values to prevent extreme swings
        imperialLoyalty = Mathf.Clamp(imperialLoyalty, -10, 10);
        rebellionSympathy = Mathf.Clamp(rebellionSympathy, -10, 10);
        
        // Log loyalty changes
        Debug.Log($"Loyalty updated - Imperial: {imperialLoyalty}, Rebellion: {rebellionSympathy}");
        
        // Show feedback if significant change
        if (Mathf.Abs(imperialChange) >= 2 || Mathf.Abs(rebellionChange) >= 2)
        {
            string message = "";
            
            if (imperialChange >= 2)
                message += "Your superiors are pleased with your decision. ";
            else if (imperialChange <= -2)
                message += "The Imperium is displeased with your actions. ";
                
            if (rebellionChange >= 2)
                message += "The resistance will remember this.";
                
            // Show feedback if we have a message
            if (!string.IsNullOrEmpty(message) && credentialChecker != null)
            {
                credentialChecker.ShowFeedback(message, Color.yellow);
            }
        }
    }
}