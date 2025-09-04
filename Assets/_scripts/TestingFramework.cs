using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Starkiller.Core.Managers;
using Starkiller.Core.Helpers;

// This script helps with testing navigation and functionality
public class TestingFramework : MonoBehaviour
{
    [Header("UI Manager Reference")]
    public UIManager uiManager;

    [Header("Test Buttons")]
    public Button mainMenuButton;
    public Button gameplayButton;
    public Button gameOverButton;
    public Button creditsButton;
    public Button tutorialButton;
    public Button dailyReportButton;
    public Button logBookButton;
    public Button dailyBriefingButton;

    [Header("Game Testing")]
    public Button generateShipButton;
    public Button generateInvalidShipButton;
    public Button addStrikeButton;
    public Button progressDayButton;
    
    [Header("Advanced Encounter Testing")]
    public Button generateStoryShipButton;
    public Button generateSpecialShipButton;
    public Button generateTutorialShipButton;
    public Button generateEmergencyShipButton;
    public Button cycleThroughSystemsButton;
    public Button showCurrentSystemButton;
    
    [Header("Inspection Testing")]
    public Button triggerInspectionButton;
    public Button testInspectionVideoButton;
    public Button showInspectionPanelButton;
    public Button hideInspectionPanelButton;
    public Button debugVideoPlayerButton;

    [Header("Component References")]
    public CredentialChecker credentialChecker;
    public GameManager gameManager;
    public ShipEncounterSystem shipSystem; // Legacy system
    public DailyBriefingManager dailyBriefingManager;
    
    [Header("Coordinator Reference")]
    public ShipGeneratorCoordinator shipCoordinator;
    
    [Header("New System References")]
    public MasterShipGenerator masterShipGenerator;
    public StarkkillerEncounterSystem starkkillerSystem;
    public EncounterSystemManager encounterSystemManager;
    
    [Header("Inspection System References")]
    public InspectionManager inspectionManager;
    public ScenarioMediaHelper scenarioMediaHelper;
    
    // Track current test encounter type
    private int currentTestEncounterType = 0;
    
    void Awake()
    {
        // Find missing references to ensure connectivity
        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();
            
        if (credentialChecker == null)
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
            
        if (shipSystem == null)
            shipSystem = FindFirstObjectByType<ShipEncounterSystem>();
            
        if (shipCoordinator == null)
            shipCoordinator = FindFirstObjectByType<ShipGeneratorCoordinator>();
            
        if (dailyBriefingManager == null)
            dailyBriefingManager = FindFirstObjectByType<DailyBriefingManager>();
            
        // Find new system references
        if (masterShipGenerator == null)
            masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            
        if (starkkillerSystem == null)
            starkkillerSystem = FindFirstObjectByType<StarkkillerEncounterSystem>();
            
        if (encounterSystemManager == null)
            encounterSystemManager = FindFirstObjectByType<EncounterSystemManager>();
            
        // Find inspection system references
        if (inspectionManager == null)
            inspectionManager = FindFirstObjectByType<InspectionManager>();
            
        if (scenarioMediaHelper == null)
            scenarioMediaHelper = FindFirstObjectByType<ScenarioMediaHelper>();
            
        // Log missing components
        LogMissingComponents();
    }
    
    void LogMissingComponents()
    {
        if (uiManager == null || credentialChecker == null || gameManager == null)
        {
            Debug.LogError("TestingFramework: Critical components missing!");
            
            if (uiManager == null) Debug.LogError("TestingFramework: UIManager not found");
            if (credentialChecker == null) Debug.LogError("TestingFramework: CredentialChecker not found");
            if (gameManager == null) Debug.LogError("TestingFramework: GameManager not found");
        }
        
        // Log status of encounter systems
        Debug.Log("TestingFramework - Encounter Systems Status:");
        Debug.Log($"  Legacy ShipEncounterSystem: {(shipSystem != null ? "Found" : "Not Found")}");
        Debug.Log($"  ShipGeneratorCoordinator: {(shipCoordinator != null ? "Found" : "Not Found")}");
        Debug.Log($"  MasterShipGenerator: {(masterShipGenerator != null ? "Found" : "Not Found")}");
        Debug.Log($"  StarkkillerEncounterSystem: {(starkkillerSystem != null ? "Found" : "Not Found")}");
        Debug.Log($"  EncounterSystemManager: {(encounterSystemManager != null ? "Found" : "Not Found")}");
    }
    
    void Start()
    {
        // Setup button listeners
        if (mainMenuButton) mainMenuButton.onClick.AddListener(ShowMainMenu);
        if (gameplayButton) gameplayButton.onClick.AddListener(ShowGameplay);
        if (gameOverButton) gameOverButton.onClick.AddListener(ShowGameOver);
        if (creditsButton) creditsButton.onClick.AddListener(ShowCredits);
        if (tutorialButton) tutorialButton.onClick.AddListener(ShowTutorial);
        if (dailyReportButton) dailyReportButton.onClick.AddListener(ShowDailyReport);
        if (logBookButton) logBookButton.onClick.AddListener(ShowLogBook);
        if (dailyBriefingButton) dailyBriefingButton.onClick.AddListener(ShowDailyBriefing);
        
        // Game test buttons
        if (generateShipButton) generateShipButton.onClick.AddListener(GenerateValidShip);
        if (generateInvalidShipButton) generateInvalidShipButton.onClick.AddListener(GenerateInvalidShip);
        if (addStrikeButton) addStrikeButton.onClick.AddListener(AddStrike);
        if (progressDayButton) progressDayButton.onClick.AddListener(ProgressDay);
        
        // Advanced encounter test buttons
        if (generateStoryShipButton) generateStoryShipButton.onClick.AddListener(GenerateStoryShip);
        if (generateSpecialShipButton) generateSpecialShipButton.onClick.AddListener(GenerateSpecialShip);
        if (generateTutorialShipButton) generateTutorialShipButton.onClick.AddListener(GenerateTutorialShip);
        if (generateEmergencyShipButton) generateEmergencyShipButton.onClick.AddListener(GenerateEmergencyShip);
        if (cycleThroughSystemsButton) cycleThroughSystemsButton.onClick.AddListener(CycleThroughSystems);
        if (showCurrentSystemButton) showCurrentSystemButton.onClick.AddListener(ShowCurrentSystem);
        
        // Inspection test buttons
        if (triggerInspectionButton) triggerInspectionButton.onClick.AddListener(TriggerInspection);
        if (testInspectionVideoButton) testInspectionVideoButton.onClick.AddListener(TestInspectionVideo);
        if (showInspectionPanelButton) showInspectionPanelButton.onClick.AddListener(ShowInspectionPanel);
        if (hideInspectionPanelButton) hideInspectionPanelButton.onClick.AddListener(HideInspectionPanel);
        if (debugVideoPlayerButton) debugVideoPlayerButton.onClick.AddListener(DebugVideoPlayer);
        
        Debug.Log("TestingFramework initialized successfully");
    }

    // Navigation testing methods (unchanged)
    public void ShowMainMenu()
    {
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager not assigned to TestingFramework");
            return;
        }
        
        uiManager.ShowMainMenu();
    }
    
    public void ShowGameplay()
    {
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager not assigned to TestingFramework");
            return;
        }
        
        uiManager.StartGame();
    }
    
    public void ShowGameOver()
    {
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager not assigned to TestingFramework");
            return;
        }
        
        uiManager.GameOver();
    }
    
    public void ShowCredits()
    {
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager not assigned to TestingFramework");
            return;
        }
        
        uiManager.ShowCreditsPanel();
    }
    
    public void ShowTutorial()
    {
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager not assigned to TestingFramework");
            return;
        }
        
        uiManager.ShowTutorialPanel();
    }
    
    public void ShowDailyReport()
    {
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager not assigned to TestingFramework");
            return;
        }
        
        uiManager.ShowDailyReportForTesting();
    }
    
    public void ShowLogBook()
    {
        if (credentialChecker == null)
        {
            Debug.LogWarning("CredentialChecker not assigned to TestingFramework");
            return;
        }

        var logBookPanel = credentialChecker.logBookPanel;
        if (logBookPanel == null)
        {
            Debug.LogWarning("Log book panel not found");
            return;
        }

        logBookPanel.SetActive(!logBookPanel.activeSelf);
        
        if (logBookPanel.activeSelf)
        {
            credentialChecker.UpdateLogBook();
        }
    }
    
    public void ShowDailyBriefing()
    {
        if (dailyBriefingManager == null)
        {
            Debug.LogWarning("DailyBriefingManager not assigned to TestingFramework");
            return;
        }
        
        int currentDay = (gameManager != null) ? gameManager.currentDay : 1;
        
        Debug.Log("TestingFramework: Showing daily briefing for day " + currentDay);
        dailyBriefingManager.ShowDailyBriefing(currentDay);
    }
    
    // Updated ship generation methods to use the best available system
    public void GenerateValidShip()
    {
        Debug.Log("TestingFramework: Generating valid ship encounter");
        
        // Try MasterShipGenerator first (preferred)
        if (masterShipGenerator != null)
        {
            var encounter = masterShipGenerator.GenerateRandomEncounter(true); // forceValid = true
            if (encounter != null)
            {
                credentialChecker.DisplayEncounter(encounter);
                Debug.Log("Generated valid ship using MasterShipGenerator");
                return;
            }
        }
        
        // Try ShipGeneratorCoordinator
        if (shipCoordinator != null)
        {
            shipCoordinator.DisplayTestShip(true);
            Debug.Log("Generated valid ship using ShipGeneratorCoordinator");
            return;
        }
        
        // Fall back to legacy system
        if (shipSystem != null && credentialChecker != null)
        {
            ShipEncounter testShip = shipSystem.CreateTestShip();
            credentialChecker.DisplayEncounter(testShip);
            Debug.Log("Generated valid ship using legacy ShipEncounterSystem");
            return;
        }
        
        Debug.LogError("No ship generation system available!");
    }
    
    public void GenerateInvalidShip()
    {
        Debug.Log("TestingFramework: Generating invalid ship encounter");
        
        // Try MasterShipGenerator first
        if (masterShipGenerator != null)
        {
            var encounter = masterShipGenerator.GenerateRandomEncounter(false); // forceValid = false
            if (encounter != null)
            {
                credentialChecker.DisplayEncounter(encounter);
                Debug.Log("Generated invalid ship using MasterShipGenerator");
                return;
            }
        }
        
        // Try ShipGeneratorCoordinator
        if (shipCoordinator != null)
        {
            shipCoordinator.DisplayTestShip(false);
            Debug.Log("Generated invalid ship using ShipGeneratorCoordinator");
            return;
        }
        
        // Fall back to legacy system
        if (shipSystem != null && credentialChecker != null)
        {
            ShipEncounter testShip = shipSystem.CreateTestShip();
            testShip.accessCode = "XX-1234"; // Invalid access code
            testShip.shouldApprove = false;
            testShip.invalidReason = "Invalid access code";
            credentialChecker.DisplayEncounter(testShip);
            Debug.Log("Generated invalid ship using legacy ShipEncounterSystem");
            return;
        }
        
        Debug.LogError("No ship generation system available!");
    }
    
    // New methods for testing different encounter types
    public void GenerateStoryShip()
    {
        Debug.Log("TestingFramework: Generating story encounter");
        
        if (masterShipGenerator != null)
        {
            // Use GenerateStoryEncounter with a story tag
            var encounter = masterShipGenerator.GenerateStoryEncounter("test_story");
            if (encounter != null)
            {
                credentialChecker.DisplayEncounter(encounter);
                Debug.Log("Generated story encounter");
                return;
            }
        }
        
        Debug.LogWarning("MasterShipGenerator not available for story encounters");
        GenerateValidShip(); // Fall back to regular ship
    }
    
    public void GenerateSpecialShip()
    {
        Debug.Log("TestingFramework: Generating special encounter");
        
        if (masterShipGenerator != null)
        {
            // Generate a random encounter and modify it to be special
            var encounter = masterShipGenerator.GenerateRandomEncounter(true);
            if (encounter != null)
            {
                encounter.isStoryShip = true;
                encounter.storyTag = "special_encounter";
                credentialChecker.DisplayEncounter(encounter);
                Debug.Log("Generated special encounter");
                return;
            }
        }
        
        Debug.LogWarning("MasterShipGenerator not available for special encounters");
        GenerateValidShip();
    }
    
    public void GenerateTutorialShip()
    {
        Debug.Log("TestingFramework: Generating tutorial encounter");
        
        // Use the static CreateTestEncounter method from MasterShipEncounter
        var encounter = MasterShipEncounter.CreateTestEncounter();
        encounter.shipType = "Tutorial Ship";
        encounter.story = "This is a tutorial ship for training purposes.";
        encounter.shouldApprove = true;
        
        if (credentialChecker != null)
        {
            credentialChecker.DisplayEncounter(encounter);
            Debug.Log("Generated tutorial encounter");
        }
    }
    
    public void GenerateEmergencyShip()
    {
        Debug.Log("TestingFramework: Generating emergency encounter");
        
        // Create an emergency test encounter
        var encounter = MasterShipEncounter.CreateTestEncounter();
        encounter.shipType = "Emergency Response Vessel";
        encounter.story = "EMERGENCY: Immediate docking required!";
        encounter.shouldApprove = true;
        encounter.isStoryShip = true;
        encounter.storyTag = "emergency";
        
        if (credentialChecker != null)
        {
            credentialChecker.DisplayEncounter(encounter);
            Debug.Log("Generated emergency encounter");
        }
    }
    
    // System management methods
    public void CycleThroughSystems()
    {
        if (encounterSystemManager == null)
        {
            Debug.LogWarning("EncounterSystemManager not found");
            return;
        }
        
        // Get current system
        var currentSystem = encounterSystemManager.activeSystem;
        
        // Cycle to next system
        var systems = System.Enum.GetValues(typeof(EncounterSystemManager.EncounterSystemType));
        int currentIndex = (int)currentSystem;
        int nextIndex = (currentIndex + 1) % systems.Length;
        var nextSystem = (EncounterSystemManager.EncounterSystemType)nextIndex;
        
        encounterSystemManager.SetActiveSystem(nextSystem);
        Debug.Log($"Switched to encounter system: {nextSystem}");
    }
    
    public void ShowCurrentSystem()
    {
        if (encounterSystemManager != null)
        {
            Debug.Log($"Current active encounter system: {encounterSystemManager.activeSystem}");
            
            // Also show which component is active
            var activeComponent = encounterSystemManager.GetActiveEncounterSystem();
            if (activeComponent != null)
            {
                Debug.Log($"Active component: {activeComponent.GetType().Name}");
            }
        }
        else
        {
            Debug.Log("EncounterSystemManager not found - checking individual systems:");
            
            if (masterShipGenerator != null && masterShipGenerator.enabled)
                Debug.Log("MasterShipGenerator is active");
            if (starkkillerSystem != null && starkkillerSystem.enabled)
                Debug.Log("StarkkillerEncounterSystem is active");
            if (shipCoordinator != null && shipCoordinator.enabled)
                Debug.Log("ShipGeneratorCoordinator is active");
            if (shipSystem != null && shipSystem.enabled)
                Debug.Log("Legacy ShipEncounterSystem is active");
        }
    }
    
    // Existing game testing methods
    public void AddStrike()
    {
        // Try to use GameManagerIntegrationHelper first
        var integrationHelper = FindFirstObjectByType<GameManagerIntegrationHelper>();
        if (integrationHelper != null)
        {
            integrationHelper.TestAddStrike();
            Debug.Log("TestingFramework: Added strike via GameManagerIntegrationHelper");
            return;
        }
        
        // Fallback to GameManager
        if (gameManager == null)
        {
            Debug.LogWarning("GameManager not assigned to TestingFramework");
            return;
        }
        
        gameManager.OnDecisionMade(true, false);
    }
    
    public void ProgressDay()
    {
        // Try to use GameManagerIntegrationHelper first
        var integrationHelper = FindFirstObjectByType<GameManagerIntegrationHelper>();
        if (integrationHelper != null)
        {
            integrationHelper.TestProgressDay();
            Debug.Log("TestingFramework: Progressed day via GameManagerIntegrationHelper");
            return;
        }
        
        // Fallback to GameManager
        if (gameManager == null)
        {
            Debug.LogWarning("GameManager not assigned to TestingFramework");
            return;
        }
        
        if (gameManager.GetType().GetMethod("EndShift")?.IsPublic == true)
        {
            gameManager.EndShift();
        }
        else if (uiManager != null)
        {
            uiManager.ShowDailyReportForTesting();
        }
        else
        {
            Debug.LogWarning("Cannot progress day - no direct access to EndShift and UIManager not found");
        }
    }
    
    // === INSPECTION TESTING METHODS ===
    
    /// <summary>
    /// Trigger an inspection scenario with a valid inspection-type ship
    /// </summary>
    public void TriggerInspection()
    {
        Debug.Log("TestingFramework: Triggering inspection scenario");
        
        // Method 1: Use InspectionManager if available
        if (inspectionManager != null)
        {
            // Try to find an inspection scenario
            var inspectionScenarios = Resources.LoadAll<ShipScenario>("_ScriptableObjects/Scenarios")
                .Where(s => s.IsInspectionScenario()).ToArray();
                
            if (inspectionScenarios.Length > 0)
            {
                var scenario = inspectionScenarios[UnityEngine.Random.Range(0, inspectionScenarios.Length)];
                Debug.Log($"Using inspection scenario: {scenario.scenarioName}");
                // Use TriggerInspection with InspectionType.Scheduled and the scenario
                inspectionManager.TriggerInspection(InspectionType.Scheduled, "Testing inspection", scenario);
                return;
            }
        }
        
        // Method 2: Generate inspection encounter via MasterShipGenerator
        if (masterShipGenerator != null)
        {
            var encounter = masterShipGenerator.GenerateRandomEncounter(true);
            if (encounter != null)
            {
                // Force it to be an inspection
                encounter.isStoryShip = true;
                encounter.storyTag = "inspection";
                
                if (credentialChecker != null)
                {
                    credentialChecker.DisplayEncounter(encounter);
                    Debug.Log("Generated inspection encounter via MasterShipGenerator");
                }
                return;
            }
        }
        
        Debug.LogWarning("Could not trigger inspection - no inspection system available");
    }
    
    /// <summary>
    /// Test inspection video playback directly
    /// </summary>
    public void TestInspectionVideo()
    {
        Debug.Log("TestingFramework: Testing inspection video playback");
        
        // Find VideoPlayer in InspectionVideoPlayer
        var videoPlayerObj = GameObject.Find("InspectionVideoPlayer");
        if (videoPlayerObj == null)
        {
            Debug.LogError("InspectionVideoPlayer GameObject not found!");
            return;
        }
        
        var videoPlayer = videoPlayerObj.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component not found on InspectionVideoPlayer!");
            return;
        }
        
        // Load a test video from inspection scenarios
        var inspectionScenarios = Resources.LoadAll<ShipScenario>("_ScriptableObjects/Scenarios")
            .Where(s => s.IsInspectionScenario() && s.inspectionVideo != null).ToArray();
            
        if (inspectionScenarios.Length > 0)
        {
            var scenario = inspectionScenarios[0];
            videoPlayer.clip = scenario.inspectionVideo;
            videoPlayer.enabled = true;
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.Play();
            
            Debug.Log($"Playing test video: {scenario.inspectionVideo.name} from scenario: {scenario.scenarioName}");
            Debug.Log($"VideoPlayer state - Active: {videoPlayer.gameObject.activeSelf}, Enabled: {videoPlayer.enabled}, IsPlaying: {videoPlayer.isPlaying}");
        }
        else
        {
            Debug.LogWarning("No inspection scenarios with videos found!");
        }
    }
    
    /// <summary>
    /// Show the inspection panel manually
    /// </summary>
    public void ShowInspectionPanel()
    {
        Debug.Log("TestingFramework: Showing inspection panel");
        
        var inspectionPanel = GameObject.Find("InspectionPanel");
        if (inspectionPanel == null)
        {
            Debug.LogError("InspectionPanel not found!");
            return;
        }
        
        inspectionPanel.SetActive(true);
        Debug.Log("InspectionPanel activated");
    }
    
    /// <summary>
    /// Hide the inspection panel manually
    /// </summary>
    public void HideInspectionPanel()
    {
        Debug.Log("TestingFramework: Hiding inspection panel");
        
        var inspectionPanel = GameObject.Find("InspectionPanel");
        if (inspectionPanel == null)
        {
            Debug.LogError("InspectionPanel not found!");
            return;
        }
        
        inspectionPanel.SetActive(false);
        Debug.Log("InspectionPanel deactivated");
    }
    
    /// <summary>
    /// Debug the video player state and configuration
    /// </summary>
    public void DebugVideoPlayer()
    {
        Debug.Log("TestingFramework: Debugging VideoPlayer state");
        
        // Find VideoPlayer
        var videoPlayerObj = GameObject.Find("InspectionVideoPlayer");
        if (videoPlayerObj == null)
        {
            Debug.LogError("InspectionVideoPlayer GameObject not found!");
            return;
        }
        
        var videoPlayer = videoPlayerObj.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component not found!");
            return;
        }
        
        // Log comprehensive state
        Debug.Log("=== VIDEO PLAYER DEBUG INFO ===");
        Debug.Log($"GameObject Active: {videoPlayerObj.activeSelf}");
        Debug.Log($"Component Enabled: {videoPlayer.enabled}");
        Debug.Log($"Current Clip: {(videoPlayer.clip != null ? videoPlayer.clip.name : "NULL")}");
        Debug.Log($"Is Playing: {videoPlayer.isPlaying}");
        Debug.Log($"Is Prepared: {videoPlayer.isPrepared}");
        Debug.Log($"Render Mode: {videoPlayer.renderMode}");
        Debug.Log($"Target Camera: {videoPlayer.targetCamera}");
        Debug.Log($"Target Render Texture: {videoPlayer.targetTexture}");
        Debug.Log($"Audio Output Mode: {videoPlayer.audioOutputMode}");
        Debug.Log($"Playback Speed: {videoPlayer.playbackSpeed}");
        Debug.Log($"Loop: {videoPlayer.isLooping}");
        Debug.Log($"Play On Awake: {videoPlayer.playOnAwake}");
        
        // Check parent hierarchy
        Transform parent = videoPlayerObj.transform.parent;
        while (parent != null)
        {
            Debug.Log($"Parent: {parent.name} (Active: {parent.gameObject.activeSelf})");
            parent = parent.parent;
        }
        
        // Check available inspection videos
        var scenarios = Resources.LoadAll<ShipScenario>("_ScriptableObjects/Scenarios");
        var inspectionScenariosWithVideo = scenarios.Where(s => s.IsInspectionScenario() && s.inspectionVideo != null).ToArray();
        Debug.Log($"Found {inspectionScenariosWithVideo.Length} inspection scenarios with videos:");
        foreach (var scenario in inspectionScenariosWithVideo)
        {
            Debug.Log($"  - {scenario.scenarioName}: {scenario.inspectionVideo.name}");
        }
    }
}