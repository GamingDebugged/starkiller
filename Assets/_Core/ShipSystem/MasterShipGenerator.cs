using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StarkillerBaseCommand;
using Starkiller.Core;
using Starkiller.Core.Managers;


    /// <summary>
    /// Unified generator for MasterShipEncounter objects.
    /// 
    /// This class handles the creation, management, and processing of ship encounters in the game.
    /// It replaces both ShipEncounterSystem and ShipEncounterGenerator with a single, cleaner approach.
    /// </summary>
    public class MasterShipGenerator : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Content References")]
        [SerializeField] private StarkkillerContentManager contentManager;
        [SerializeField] private StarkkillerMediaSystem mediaSystem;
        
        [Header("Ship Data")]
        [SerializeField] private List<ShipType> allShipTypes = new List<ShipType>();
        [SerializeField] private List<CaptainType> allCaptainTypes = new List<CaptainType>();
        [SerializeField] private List<ShipScenario> allScenarios = new List<ShipScenario>();
        
        [Header("Game Settings")]
        [Tooltip("Probability of generating a valid ship (0-1)")]
        [Range(0f, 1f)]
        [SerializeField] private float validShipChance = 0.7f;
        [Tooltip("Probability of generating a story ship (0-1)")]
        [Range(0f, 1f)]
        [SerializeField] private float storyShipChance = 0.2f;
        [Tooltip("Number of encounters to generate per day")]
        [SerializeField] private int encountersPerDay = 10;
        
        [Header("Current Game State")]
        [SerializeField] private int currentDay = 1;
        [SerializeField] private List<string> currentAccessCodes = new List<string>();
        [SerializeField] private List<StarkkillerContentManager.DayRule> currentDayRules = new List<StarkkillerContentManager.DayRule>();
        #endregion

        #region Private Fields
        // Queue of pending encounters
        private Queue<MasterShipEncounter> pendingEncounters = new Queue<MasterShipEncounter>();
        
        // List of encounters for the current day
        private List<MasterShipEncounter> currentDayEncounters = new List<MasterShipEncounter>();
        
        // Currently active encounter
        private MasterShipEncounter currentEncounter;
        
        // Reference to the scenario provider
        private ShipScenarioProvider scenarioProvider;
        // Flag indicating if scenario provider is available
        private bool hasScenarioProvider = false;

        // Tracking of used content to prevent repetition
        private List<ShipScenario> usedScenarios = new List<ShipScenario>();
        private List<ShipType> recentShipTypes = new List<ShipType>();
        private List<CaptainType> recentCaptainTypes = new List<CaptainType>();
        
        // Track if we've loaded resources already
        private bool resourcesLoaded = false;
        
        // Flag to prevent repeated day processing
        private bool isUpdatingDay = false;
        
        // Reference to GameManager
        private GameManager gameManager;
        #endregion

        #region Events
        /// <summary>
        /// Event for when a new encounter is ready
        /// </summary>
        public delegate void EncounterReadyHandler(MasterShipEncounter encounter);
        public event EncounterReadyHandler OnEncounterReady;
        #endregion

        #region Singleton
        // Singleton pattern for easy access
        private static MasterShipGenerator _instance;
        public static MasterShipGenerator Instance { 
            get {
                // Check if current instance is still valid
                if (_instance == null || _instance.gameObject == null)
                {
                    _instance = FindFirstObjectByType<MasterShipGenerator>();
                    if (_instance == null) {
                        Debug.LogError("MasterShipGenerator: No instance found in scene! Critical functionality will fail.");
                    }
                    else
                    {
                        Debug.Log("MasterShipGenerator: Found replacement instance after previous was destroyed");
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Unity Lifecycle Methods
        void Awake()
        {
            InitializeSingleton();
            FindRequiredComponents();
            SyncWithGameManager();
        }
        
        private void OnDestroy()
        {
            // Only clear the instance if this is actually the singleton instance
            if (_instance == this)
            {
                Debug.Log("MasterShipGenerator: Clearing singleton instance on destroy");
                _instance = null;
            }
        }
        
        void Start()
        {
            try
            {
                // Look for ShipGeneratorManager to register ourselves
                RegisterWithShipGeneratorManager();
                
                // Load resources
                LoadAllResources();
                
                // Find ShipScenarioProvider if available
                FindScenarioProvider();

                // Validate resources were loaded properly
                ValidateResources();
                
                // Create default encounters for day 1
                GenerateEncountersForDay(currentDay);
                
                // Verify that we generated at least some encounters
                ValidateEncounters();
                
                // Register with any ShipTimingController instances to ensure they have valid references
                NotifyTimingController();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"MasterShipGenerator: Error during initialization: {e.Message}\n{e.StackTrace}");
                
                // Create fallback resources and encounters for recovery
                CreateDefaultResourcesIfNeeded();
                CreateFallbackEncounters(5);
            }
        }
        #endregion

        #region Initialization Methods
        /// <summary>
        /// Initializes the singleton pattern for this instance
        /// </summary>
        private void InitializeSingleton()
        {
            // Singleton setup with proper scene transition handling
            if (_instance != null)
            {
                if (_instance != this)
                {
                    // Check if the existing instance is still valid
                    if (_instance == null || _instance.gameObject == null)
                    {
                        Debug.LogWarning("MasterShipGenerator: Previous instance was destroyed, taking over as singleton");
                        _instance = this;
                    }
                    else
                    {
                        Debug.LogWarning("MasterShipGenerator: Another valid instance exists, destroying this duplicate");
                        Destroy(gameObject);
                        return;
                    }
                }
            }
            else
            {
                _instance = this;
            }
            
            // Don't call DontDestroyOnLoad directly - let ManagerInitializer handle it
            // This prevents "Not a root GameObject" errors
            // The ManagerInitializer will call DontDestroyOnLoad on the parent GameObject
            
            // Check if we're a root GameObject (for backward compatibility)
            if (transform.parent == null)
            {
                Debug.Log("MasterShipGenerator is at root level, marking as persistent with DontDestroyOnLoad");
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Check if our parent has a ManagerInitializer component
                ManagerInitializer managerInit = GetComponentInParent<ManagerInitializer>();
                if (managerInit == null)
                {
                    Debug.LogWarning("MasterShipGenerator: Not at root level and no ManagerInitializer found. This may cause persistence issues.");
                }
                else
                {
                    Debug.Log("MasterShipGenerator: Found ManagerInitializer parent - persistence will be handled by it");
                }
            }

            // Add an instance tracker for debugging/verification
            DebugMonitor debugMonitor = DebugMonitor.Instance;
            if (debugMonitor != null) {
                debugMonitor.TrackDataValue("MasterShipGenerator.InstanceID", GetInstanceID().ToString());
            }
        }

        /// <summary>
        /// Finds and initializes required component references
        /// </summary>
        private void FindRequiredComponents()
        {
            // Find references if not assigned
            if (contentManager == null)
                contentManager = FindFirstObjectByType<StarkkillerContentManager>();
                
            if (mediaSystem == null)
                mediaSystem = FindFirstObjectByType<StarkkillerMediaSystem>();
                
            // Try to find GameManager
            if (gameManager == null)
                gameManager = FindFirstObjectByType<GameManager>();
            
            // Log what we found
            Debug.Log($"MasterShipGenerator initialized. ContentManager found: {contentManager != null}, MediaSystem found: {mediaSystem != null}, GameManager found: {gameManager != null}");
        }

        /// <summary>
        /// Syncs current day with GameManager if available
        /// </summary>
        private void SyncWithGameManager()
        {
            if (gameManager != null)
            {
                currentDay = gameManager.currentDay;
                Debug.Log($"MasterShipGenerator: Synced day with GameManager: {currentDay}");
            }
        }

        /// <summary>
        /// Registers with ShipGeneratorManager if available
        /// </summary>
        private void RegisterWithShipGeneratorManager()
        {
            ShipGeneratorManager generatorManager = ShipGeneratorManager.Instance;
            if (generatorManager != null)
            {
                Debug.Log("MasterShipGenerator: Found ShipGeneratorManager - ensuring registration");
            }
        }

        /// <summary>
        /// Finds and initializes ScenarioProvider if available
        /// </summary>
        private void FindScenarioProvider()
        {
            if (scenarioProvider == null)
            {
                scenarioProvider = FindFirstObjectByType<ShipScenarioProvider>();
                if (scenarioProvider != null)
                {
                    hasScenarioProvider = true;
                    Debug.Log("Found ShipScenarioProvider for scenario generation");
                }
            }
        }

        /// <summary>
        /// Validates that resources were loaded properly
        /// </summary>
        private void ValidateResources()
        {
            if (allShipTypes.Count == 0 || allCaptainTypes.Count == 0 || allScenarios.Count == 0)
            {
                Debug.LogWarning("MasterShipGenerator: Some resources failed to load. Creating default resources.");
                CreateDefaultResourcesIfNeeded();
            }
        }

        /// <summary>
        /// Verifies that we generated at least some encounters
        /// </summary>
        private void ValidateEncounters()
        {
            if (pendingEncounters.Count == 0)
            {
                Debug.LogWarning("MasterShipGenerator: Failed to generate encounters! Creating fallback encounters.");
                CreateFallbackEncounters(5); // Create 5 fallback encounters
            }
        }

        /// <summary>
        /// Notifies the ShipTimingController of our presence
        /// </summary>
        private void NotifyTimingController()
        {
            ShipTimingController timingController = ShipTimingController.Instance;
            if (timingController != null)
            {
                timingController.ResetCooldown();
                Debug.Log("Notified ShipTimingController of our presence via ResetCooldown");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Check if encounters are available for processing
        /// </summary>
        public bool HasEncountersAvailable()
        {
            return pendingEncounters.Count > 0 || currentDayEncounters.Count > 0;
        }

        /// <summary>
        /// Allows external providers to register themselves with the generator
        /// </summary>
        public void RegisterScenarioProvider(ShipScenarioProvider provider)
        {
            if (provider != null)
            {
                scenarioProvider = provider;
                hasScenarioProvider = true;
                Debug.Log("ShipScenarioProvider successfully registered with MasterShipGenerator");
            }
        }

        /// <summary>
        /// Update for a new day - called by GameManager
        /// </summary>
        public void StartNewDay(int day)
        {
            // Prevent multiple calls during the same update
            if (isUpdatingDay)
            {
                Debug.LogWarning($"MasterShipGenerator: StartNewDay called while already updating day. Ignoring redundant call.");
                return;
            }
            
            isUpdatingDay = true;
            
            try
            {
                // Log that we're updating the day
                Debug.Log($"MasterShipGenerator: StartNewDay called with day={day}, current day was {currentDay}");
                
                // Update internal day counter
                currentDay = day;
                
                // Load resources if not already loaded
                if (!resourcesLoaded)
                    LoadAllResources();
                    
                // Generate encounters for the new day
                GenerateEncountersForDay(day);
                
                Debug.Log($"MasterShipGenerator: Day update complete. Current day: {currentDay}, Encounters generated: {pendingEncounters.Count}");
            }
            finally
            {
                // Release the updating lock
                isUpdatingDay = false;
            }
        }

        /// <summary>
        /// Generate encounters for a specific day
        /// </summary>
        public void GenerateEncountersForDay(int day)
        {
            currentDay = day;
            
            // Ensure resources are loaded
            if (!resourcesLoaded)
                LoadAllResources();
                
            // Clear existing queues
            pendingEncounters.Clear();
            currentDayEncounters.Clear();
            
            // Update access codes (either from content manager or generate new)
            if (contentManager != null)
            {
                // Get the current values from content manager
                currentAccessCodes = new List<string>(contentManager.currentAccessCodes);
                currentDayRules = new List<StarkkillerContentManager.DayRule>(contentManager.specialRules);
                
                // Log what day we're generating encounters for
                Debug.Log($"MasterShipGenerator: Generating encounters for day {day}, using {currentAccessCodes.Count} access codes");
            }
            else
            {
                RegenerateAccessCodes();
            }
            
            // Clear tracking of used content
            usedScenarios.Clear();
            recentShipTypes.Clear();
            recentCaptainTypes.Clear();
            
            // Determine number of story encounters for this day
            int storyEncounters = 0;
            List<ShipScenario> dayStoryScenarios = new List<ShipScenario>();
            
            foreach (var scenario in allScenarios)
            {
                if (scenario.isStoryMission && scenario.dayFirstAppears == day)
                {
                    storyEncounters++;
                    dayStoryScenarios.Add(scenario);
                }
            }
            
            // Create list to hold all encounters before queuing
            List<MasterShipEncounter> allDayEncounters = new List<MasterShipEncounter>();
            
            // Generate standard encounters
            for (int i = 0; i < encountersPerDay - storyEncounters; i++)
            {
                MasterShipEncounter encounter = GenerateRandomEncounter();
                allDayEncounters.Add(encounter);
            }
            
            // Generate story encounters
            foreach (var storyScenario in dayStoryScenarios)
            {
                MasterShipEncounter storyEncounter = GenerateStoryEncounter(storyScenario.storyTag);
                
                // Insert at random positions in the list
                int position = Random.Range(0, allDayEncounters.Count + 1);
                allDayEncounters.Insert(position, storyEncounter);
            }
            
            // Store all encounters for the current day
            currentDayEncounters.AddRange(allDayEncounters);
            
            // Now queue all encounters in their finalized order
            foreach (var encounter in allDayEncounters)
            {
                pendingEncounters.Enqueue(encounter);
            }
            
            Debug.Log($"Generated {pendingEncounters.Count} encounters for day {day} ({storyEncounters} story encounters)");
        }

        /// <summary>
        /// Generate a random ship encounter
        /// </summary>
        public MasterShipEncounter GenerateRandomEncounter(bool forceValid = false)
        {
            // Ensure resources are loaded
            if (!resourcesLoaded)
                LoadAllResources();
                
            // Randomly determine if this ship should have valid credentials
            bool shouldBeValid = forceValid || (Random.value < validShipChance);
            
            // Select appropriate scenario type
            ShipScenario scenario = SelectScenario(shouldBeValid);
            
            // Select appropriate ship type based on scenario
            ShipType shipType = SelectShipType(scenario);
            
            // Select appropriate captain based on scenario and ship
            CaptainType captainType = SelectCaptainType(scenario, shipType);
            
            // Build the encounter
            MasterShipEncounter encounter = MasterShipEncounter.CreateFromScriptableObjects(
                shipType, captainType, scenario, shouldBeValid, currentAccessCodes, contentManager);
            
            // Assign CargoManifest if ManifestManager is available
            if (ManifestManager.Instance != null)
            {
                encounter.manifestData = ManifestManager.Instance.SelectManifestForShip(
                    shipType, 
                    encounter.faction, 
                    currentDay
                );
                
                // If no ScriptableObject manifest was assigned but we have a string manifest,
                // and the encounter should be suspicious, try to generate contraband
                if (encounter.manifestData == null && !string.IsNullOrEmpty(encounter.manifest) && !shouldBeValid)
                {
                    string fallbackManifest = ManifestManager.Instance.GenerateFallbackManifest(
                        encounter.faction, 
                        true // shouldHaveContraband for invalid encounters
                    );
                    if (!string.IsNullOrEmpty(fallbackManifest))
                    {
                        encounter.manifest = fallbackManifest;
                    }
                }
            }
            
            // Enhance with videos from media system if available
            if (mediaSystem != null && mediaSystem.mediaDatabase != null)
            {
                encounter.EnhanceWithVideos(mediaSystem.mediaDatabase);
            }
            
            return encounter;
        }

        /// <summary>
        /// Generate a story-based encounter for specific progression moments
        /// </summary>
        public MasterShipEncounter GenerateStoryEncounter(string storyTag)
        {
            // Ensure resources are loaded
            if (!resourcesLoaded)
                LoadAllResources();
                
            // Find scenarios matching the story tag for the current day
            List<ShipScenario> matchingScenarios = new List<ShipScenario>();
            
            foreach (var scenario in allScenarios)
            {
                if (scenario.isStoryMission && 
                    scenario.storyTag == storyTag && 
                    scenario.dayFirstAppears <= currentDay)
                {
                    matchingScenarios.Add(scenario);
                }
            }
            
            // If no matching scenarios, create a fallback
            if (matchingScenarios.Count == 0)
            {
                Debug.LogWarning($"No story scenarios found for tag: {storyTag} on day {currentDay}");
                
                // Create a fallback encounter that's always valid
                MasterShipEncounter fallback = GenerateRandomEncounter(true);
                fallback.isStoryShip = true;
                fallback.storyTag = storyTag;
                return fallback;
            }
            
            // Select a story scenario
            ShipScenario selectedScenario = matchingScenarios[Random.Range(0, matchingScenarios.Count)];
            
            // Select appropriate ship and captain for this scenario
            ShipType shipType = SelectShipType(selectedScenario);
            CaptainType captainType = SelectCaptainType(selectedScenario, shipType);
            
            // Build the story encounter
            MasterShipEncounter encounter = MasterShipEncounter.CreateFromScriptableObjects(
                shipType, captainType, selectedScenario, selectedScenario.shouldBeApproved, 
                currentAccessCodes, contentManager);
            
            // Assign CargoManifest for story encounters if ManifestManager is available
            if (ManifestManager.Instance != null)
            {
                encounter.manifestData = ManifestManager.Instance.SelectManifestForShip(
                    shipType, 
                    encounter.faction, 
                    currentDay
                );
            }
            
            // Enhance with videos from media system if available
            if (mediaSystem != null && mediaSystem.mediaDatabase != null)
            {
                encounter.EnhanceWithVideos(mediaSystem.mediaDatabase);
            }
            
            return encounter;
        }

        /// <summary>
        /// Get the next pending encounter - always returns a valid encounter (fallback created if needed)
        /// </summary>
        public MasterShipEncounter GetNextEncounter()
        {
            try
            {
                // Get caller info for diagnostics
                string callerMethod = GetCallerMethodName();
                
                // Check if we're still in cooldown from the last encounter
                if (!this.CanDisplayNewEncounter())
                {
                    float cooldown = this.GetEncounterCooldownRemaining();
                    Debug.Log($"MasterShipGenerator: Encounter on cooldown for {cooldown:F1} more seconds");
                    return null;
                }

                // Check if gameplay is active
                if (!IsGameplayActive())
                {
                    return CreateWaitingForGameplayEncounter();
                }
                    
                // Check with timing controller if we can generate a new ship
                if (!CanGenerateNewShip(callerMethod))
                {
                    return HandleTimingControllerDelay();
                }

                // Ensure resources are loaded
                if (!resourcesLoaded)
                    LoadAllResources();
                    
                // Ensure we have a queue to work with
                if (pendingEncounters == null)
                {
                    Debug.LogError("MasterShipGenerator: pendingEncounters queue is null! Creating new queue.");
                    pendingEncounters = new Queue<MasterShipEncounter>();
                }
                
                // Generate more encounters if running low
                if (pendingEncounters.Count < 5 && currentDayEncounters.Count < 3)
                {
                    Debug.Log("Running low on encounters, generating more");
                    GenerateEncountersForDay(currentDay);
                }
                
                MasterShipEncounter encounter = null;
                
                if (pendingEncounters.Count > 0)
                {
                    encounter = DequeueNextEncounter();
                }
                else
                {
                    // If we have no more encounters, create a fallback
                    Debug.LogWarning("No more encounters pending! Creating a test encounter as fallback");
                    encounter = CreateFallbackEncounter();
                }
                
                // Only record the encounter as displayed if we actually have a valid encounter to return
                if (encounter != null && 
                    encounter.shipType != "Waiting for Gameplay" && 
                    encounter.shipType != "System Delay")
                {
                    this.RecordEncounterDisplayed();
                    Debug.Log($"MasterShipGenerator: Returning encounter {encounter.shipType} - {encounter.captainName}");
                }
                
                return encounter;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"MasterShipGenerator: Error in GetNextEncounter: {e.Message}\n{e.StackTrace}");
                return CreateErrorRecoveryEncounter(e);
            }
        }

        /// <summary>
        /// Process a decision with an explicitly provided encounter
        /// This method should be called by CredentialChecker to avoid sync issues
        /// </summary>
        public void ProcessDecisionWithEncounter(bool approved, MasterShipEncounter encounter)
        {
            try {
                if (encounter == null)
                {
                    HandleNullEncounterForDecision(approved);
                    return;
                }
                
                // If the provided encounter doesn't match our current one, update ours
                if (currentEncounter != encounter)
                {
                    Debug.Log("MasterShipGenerator: Syncing encounter from CredentialChecker");
                    currentEncounter = encounter;
                }
                
                Debug.Log($"=== [GENERATOR DECISION] MasterShipGenerator.ProcessDecisionWithEncounter() - Approved: {approved}, Ship: {encounter.shipType} ===");

                // Process the encounter
                ProcessEncounterInternal(approved, encounter);
            }
            catch (System.Exception e) {
                Debug.LogError($"MasterShipGenerator: Critical error in ProcessDecisionWithEncounter: {e.Message}\n{e.StackTrace}");
                
                // Try to maintain game stability by clearing current encounter
                currentEncounter = null;
                
                // Try to queue a new encounter after a delay
                var mediaTransitionManagerApprove = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
                if (mediaTransitionManagerApprove == null)
                {
                    StartCoroutine(DelayedNextEncounter(1.0f));
                }
            }
        }

        /// <summary>
        /// Process a bribery acceptance from the player
        /// </summary>
        public void ProcessBriberyAccepted(MasterShipEncounter encounter)
        {
            if (encounter == null)
            {
                Debug.LogError("MasterShipGenerator: Cannot process bribery with null encounter!");
                return;
            }
            
            // Update state if provided encounter doesn't match current one
            if (currentEncounter != encounter)
            {
                currentEncounter = encounter;
            }
            
            Debug.Log($"Bribery accepted! Player gained {encounter.bribeAmount} credits");
            
            // Here we would update player credits
            // gameManager.AddCredits(encounter.bribeAmount);
            
            // Automatically approve the ship after accepting bribe
            ProcessEncounterInternal(true, encounter);
        }

        /// <summary>
        /// Process a holding pattern decision from the player
        /// </summary>
        public void ProcessHoldingPattern(MasterShipEncounter encounter)
        {
            if (encounter == null)
            {
                Debug.LogError("MasterShipGenerator: Cannot process holding pattern with null encounter!");
                return;
            }
            
            Debug.Log($"MasterShipGenerator: Processing holding pattern for ship: {encounter.shipType}");
            
            // Set decision state
            encounter.playerDecision = MasterShipEncounter.DecisionState.HoldingPattern;
            encounter.isInHoldingPattern = true;
            
            // Trigger captain reaction video for holding pattern
            var mediaTransitionManager = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
            if (mediaTransitionManager == null)
            {
                mediaTransitionManager = FindFirstObjectByType<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
            }
            
            if (mediaTransitionManager != null)
            {
                mediaTransitionManager.PrepareNextEncounter(encounter);
                mediaTransitionManager.ShowCaptainReactionForDecision(Starkiller.Core.DecisionType.HoldingPattern);
                Debug.Log("MasterShipGenerator: Triggered holding pattern reaction video");
            }
            else
            {
                Debug.LogWarning("MasterShipGenerator: Could not find EncounterMediaTransitionManager for holding pattern reaction");
            }
            
            // Notify timing controller that encounter has ended
            NotifyTimingControllerEncounterEnded();

            // Find the HoldingPatternProcessor
            HoldingPatternProcessor holdingProcessor = FindFirstObjectByType<HoldingPatternProcessor>();
            if (holdingProcessor != null)
            {
                // Add the ship to the holding pattern
                bool added = holdingProcessor.AddShipToHoldingPattern(encounter);
                
                // If it wasn't added (e.g., holding pattern is full), re-queue it immediately
                if (!added)
                {
                    encounter.isInHoldingPattern = false;
                    pendingEncounters.Enqueue(encounter);
                    Debug.Log($"Ship {encounter.shipType} could not be added to holding pattern, re-queued");
                }
                else
                {
                    Debug.Log($"Ship {encounter.shipType} added to holding pattern");
                }
            }
            else
            {
                // If no processor is found, just re-queue the encounter immediately
                Debug.LogWarning("No HoldingPatternProcessor found! Ship will be immediately re-queued.");
                encounter.isInHoldingPattern = false;
                pendingEncounters.Enqueue(encounter);
            }
        }

        /// <summary>
        /// Process a ship that has completed its holding pattern and should be requeued
        /// </summary>
        public void ProcessHoldingPatternCompletion(MasterShipEncounter encounter)
        {
            if (encounter == null)
            {
                Debug.LogWarning("MasterShipGenerator: Attempt to process null encounter after holding pattern!");
                return;
            }
            
            // Reset the holding pattern state
            encounter.isInHoldingPattern = false;
            
            // Re-queue this encounter
            pendingEncounters.Enqueue(encounter);
            
            Debug.Log($"Ship {encounter.shipType} released from holding pattern and added back to queue");
        }

        /// <summary>
        /// Re-queue an encounter that should be processed again later
        /// </summary>
        public void RequeueEncounter(MasterShipEncounter encounter)
        {
            if (encounter == null)
            {
                Debug.LogWarning("MasterShipGenerator: Attempt to re-queue null encounter!");
                return;
            }
            
            // Reset any flags that might prevent normal processing
            encounter.isInHoldingPattern = false;
            
            // Re-add to the queue
            pendingEncounters.Enqueue(encounter);
            
            Debug.Log($"Ship {encounter.shipType} re-queued for later processing");
        }

        /// <summary>
        /// Process a tractor beam capture from the player
        /// </summary>
        public void ProcessTractorBeam(MasterShipEncounter encounter)
        {
            if (encounter == null)
            {
                Debug.LogError("MasterShipGenerator: Cannot process tractor beam with null encounter!");
                return;
            }
            
            Debug.Log($"Ship captured with tractor beam: {encounter.shipType} - {encounter.captainName}");
            
            // Trigger captain reaction video for tractor beam
            var mediaTransitionManager = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
            if (mediaTransitionManager == null)
            {
                mediaTransitionManager = FindFirstObjectByType<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
            }
            
            if (mediaTransitionManager != null)
            {
                mediaTransitionManager.PrepareNextEncounter(encounter);
                mediaTransitionManager.ShowCaptainReactionForDecision(Starkiller.Core.DecisionType.TractorBeam);
                Debug.Log("MasterShipGenerator: Triggered tractor beam reaction video");
            }
            else
            {
                Debug.LogWarning("MasterShipGenerator: Could not find EncounterMediaTransitionManager for tractor beam reaction");
            }
            
            // Update story progression if this was a story ship
            if (encounter.isStoryShip)
            {
                Debug.Log($"Story ship captured! Tag: {encounter.storyTag}");
                // Here we would update story progression
                // gameManager.AdvanceStoryLine(encounter.storyTag, "captured");
            }
            
            // Reset current encounter
            currentEncounter = null;
            
            // Get next encounter (only if old system is active)
            var mediaTransitionManagerCapture = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
            if (mediaTransitionManagerCapture == null)
            {
                StartCoroutine(DelayedNextEncounter(1.5f));
            }
        }
        #endregion

        #region Resource Management
        /// <summary>
        /// Loads all necessary resources from ContentManager or directly from Resources folder
        /// </summary>
        private void LoadAllResources()
        {
            if (resourcesLoaded)
                return;

            Debug.Log("MasterShipGenerator: Loading resources...");
            
            // Try to use ContentManager first if available
            if (contentManager != null)
            {
                LoadResourcesFromContentManager();
            }
            else
            {
                Debug.LogWarning("MasterShipGenerator: ContentManager not found, trying to load resources directly...");
                
                // Try to load resources directly from Resources folder
                LoadResourcesDirectly();
            }
            
            // Create default resources if we still don't have any
            CreateDefaultResourcesIfNeeded();
            
            // Generate access codes if we don't have any
            if (currentAccessCodes.Count == 0)
            {
                RegenerateAccessCodes();
            }
            
            resourcesLoaded = true;
        }

        /// <summary>
        /// Loads resources from the ContentManager
        /// </summary>
        private void LoadResourcesFromContentManager()
        {
            Debug.Log("MasterShipGenerator: Using ContentManager to load resources");
            
            // Copy ship types from content manager
            if (allShipTypes.Count == 0 && contentManager.shipTypes.Count > 0)
            {
                allShipTypes.AddRange(contentManager.shipTypes);
                Debug.Log($"Loaded {allShipTypes.Count} ship types from ContentManager");
            }
                
            // Copy captain types from content manager
            if (allCaptainTypes.Count == 0 && contentManager.captainTypes.Count > 0)
            {
                allCaptainTypes.AddRange(contentManager.captainTypes);
                Debug.Log($"Loaded {allCaptainTypes.Count} captain types from ContentManager");
            }
                
            // Copy scenarios from content manager
            if (allScenarios.Count == 0 && contentManager.scenarios.Count > 0)
            {
                allScenarios.AddRange(contentManager.scenarios);
                Debug.Log($"Loaded {allScenarios.Count} scenarios from ContentManager");
            }
                
            // Copy game settings from content manager
            validShipChance = contentManager.validShipChance;
            storyShipChance = contentManager.storyShipChance;
            
            // Copy current state from content manager
            currentDay = contentManager.currentDay;
            currentAccessCodes = new List<string>(contentManager.currentAccessCodes);
            currentDayRules = new List<StarkkillerContentManager.DayRule>(contentManager.specialRules);
        }

        /// <summary>
        /// Try to load resources using centralized resource loading
        /// </summary>
        private void LoadResourcesDirectly()
        {
            Debug.Log("MasterShipGenerator: Loading resources using centralized ResourceLoadingHelper");
            
            // Load ship types using centralized helper
            if (allShipTypes.Count == 0)
            {
                List<ShipType> loadedShipTypes = ResourceLoadingHelper.LoadShipTypes();
                if (loadedShipTypes.Count > 0)
                {
                    allShipTypes.AddRange(loadedShipTypes);
                    Debug.Log($"Loaded {loadedShipTypes.Count} ship types using ResourceLoadingHelper");
                }
                else
                {
                    Debug.LogWarning("No ship types loaded from ResourceLoadingHelper");
                }
            }
            
            // Load captain types using centralized helper
            if (allCaptainTypes.Count == 0)
            {
                List<CaptainType> loadedCaptainTypes = ResourceLoadingHelper.LoadCaptainTypes();
                if (loadedCaptainTypes.Count > 0)
                {
                    allCaptainTypes.AddRange(loadedCaptainTypes);
                    Debug.Log($"Loaded {loadedCaptainTypes.Count} captain types using ResourceLoadingHelper");
                }
                else
                {
                    Debug.LogWarning("No captain types loaded from ResourceLoadingHelper");
                }
            }
            
            // Load scenarios using centralized helper
            if (allScenarios.Count == 0)
            {
                List<ShipScenario> loadedScenarios = ResourceLoadingHelper.LoadShipScenarios();
                if (loadedScenarios.Count > 0)
                {
                    allScenarios.AddRange(loadedScenarios);
                    Debug.Log($"Loaded {loadedScenarios.Count} scenarios using ResourceLoadingHelper");
                }
                else
                {
                    Debug.LogWarning("No scenarios loaded from ResourceLoadingHelper");
                }
            }
        }

        /// <summary>
        /// Create default resources if loading failed
        /// </summary>
        private void CreateDefaultResourcesIfNeeded()
        {
            // Create default ship types if needed
            if (allShipTypes.Count == 0)
            {
                Debug.LogWarning("Creating default ship types...");
                
                string[][] defaultShipData = new string[][] {
                    new string[] { "Imperial Shuttle", "Imperial", "2", "8", "Star Destroyer,Imperial Base,Coruscant" },
                    new string[] { "Cargo Freighter", "Merchant", "5", "15", "Trade Port,Outer Rim,Merchant Guild" },
                    new string[] { "Diplomatic Vessel", "Diplomatic", "10", "30", "Senate,Republic Center,Neutral Zone" }
                };
                
                foreach (string[] data in defaultShipData)
                {
                    ShipType newType = ScriptableObject.CreateInstance<ShipType>();
                    newType.typeName = data[0];
                    
                    // Create a category if needed
                    ShipCategory category = ScriptableObject.CreateInstance<ShipCategory>();
                    category.categoryName = data[1];
                    
                    newType.category = category;
                    newType.minCrewSize = int.Parse(data[2]);
                    newType.maxCrewSize = int.Parse(data[3]);
                    newType.commonOrigins = data[4].Split(',');
                    
                    allShipTypes.Add(newType);
                }
                
                Debug.Log($"Created {allShipTypes.Count} default ship types");
            }
            
            // Create default captain types if needed
            if (allCaptainTypes.Count == 0)
            {
                Debug.LogWarning("Creating default captain types...");
                
                string[][] defaultCaptainData = new string[][] {
                    new string[] { "Imperial Officer", "Imperium", "Captain,Commander,Lieutenant", "Tarkin,Piett,Sloane,Thrawn", "0.1" },
                    new string[] { "Merchant Captain", "Merchant,Trade", "Captain,Trader,Pilot", "Han,Lando,Dash,Wedge", "0.3" },
                    new string[] { "Diplomat", "Neutral,Republic", "Ambassador,Envoy,Senator", "Organa,Mon Mothma,Bel Iblis", "0.05" }
                };
                
                foreach (string[] data in defaultCaptainData)
                {
                    CaptainType newType = ScriptableObject.CreateInstance<CaptainType>();
                    newType.typeName = data[0];
                    newType.factions = data[1].Split(',');
                    newType.commonRanks = data[2].Split(',');
                    
                    // Set first/last names
                    string[] names = data[3].Split(',');
                    newType.possibleFirstNames = names;
                    newType.possibleLastNames = names;
                    
                    // Set bribery chance
                    newType.briberyChance = float.Parse(data[4]);
                    newType.minBribeAmount = 10;
                    newType.maxBribeAmount = 50;
                    
                    allCaptainTypes.Add(newType);
                }
                
                Debug.Log($"Created {allCaptainTypes.Count} default captain types");
            }
            
            // Create default scenarios if needed
            if (allScenarios.Count == 0)
            {
                Debug.LogWarning("Creating default scenarios...");
                
                // Create a default valid scenario
                ShipScenario validScenario = ScriptableObject.CreateInstance<ShipScenario>();
                validScenario.scenarioName = "Standard Delivery";
                validScenario.type = ShipScenario.ScenarioType.Standard;
                validScenario.shouldBeApproved = true;
                validScenario.dayFirstAppears = 1;
                validScenario.possibleStories = new string[] { 
                    "Ship requesting routine clearance for supply delivery.",
                    "Regular transport vessel on scheduled run." 
                };
                validScenario.possibleManifests = new string[] {
                    "Standard supplies and provisions for the base.",
                    "Personnel and equipment for routine rotation."
                };
                validScenario.possibleConsequences = new string[] {
                    "Supplies delayed, causing minor inconvenience.",
                    "Important personnel unable to reach their post."
                };
                
                // Create a default invalid scenario
                ShipScenario invalidScenario = ScriptableObject.CreateInstance<ShipScenario>();
                invalidScenario.scenarioName = "Invalid Documents";
                invalidScenario.type = ShipScenario.ScenarioType.Invalid;
                invalidScenario.shouldBeApproved = false;
                invalidScenario.invalidReason = "Invalid access code";
                invalidScenario.dayFirstAppears = 1;
                invalidScenario.possibleStories = new string[] { 
                    "Ship requesting emergency clearance.",
                    "Transport claims its documentation was corrupted." 
                };
                invalidScenario.possibleManifests = new string[] {
                    "Medical supplies and equipment.",
                    "Critical parts for base operations."
                };
                invalidScenario.possibleConsequences = new string[] {
                    "Security breach detected in sector 7.",
                    "Unauthorized personnel infiltrated the base."
                };
                
                // Add scenarios to list
                allScenarios.Add(validScenario);
                allScenarios.Add(invalidScenario);
                
                Debug.Log($"Created {allScenarios.Count} default scenarios");
            }
        }

        /// <summary>
        /// Creates fallback encounters to ensure the game can run even if resource loading fails
        /// </summary>
        private void CreateFallbackEncounters(int count)
        {
            pendingEncounters.Clear(); // Clear any partial queue

            for (int i = 0; i < count; i++)
            {
                MasterShipEncounter fallback = MasterShipEncounter.CreateTestEncounter();

                // Alternate valid and invalid encounters
                fallback.shouldApprove = (i % 2 == 0);

                // Set invalid reason for ships that should be denied
                if (!fallback.shouldApprove)
                {
                    fallback.invalidReason = "Invalid clearance code";
                    fallback.accessCode = "OLD-" + Random.Range(1000, 10000);
                }

                pendingEncounters.Enqueue(fallback);
            }

            Debug.Log($"Created {count} fallback encounters for error recovery");
        }
        #endregion

        #region Encounter Processing
        /// <summary>
        /// Internal method to process an encounter without duplicate code
        /// </summary>
        private void ProcessEncounterInternal(bool approved, MasterShipEncounter encounterToProcess)
        {
            try {
                if (encounterToProcess == null) {
                    Debug.LogError("MasterShipGenerator: Cannot process null encounter in ProcessEncounterInternal");
                    return;
                }
                
                // Debug log for tracking
                Debug.Log($"MasterShipGenerator.ProcessEncounterInternal() - Processing {(approved ? "APPROVE" : "DENY")} decision");
                
                // Special handling for access codes with invalid prefixes (OLD-, XX-, etc.)
                bool hasInvalidPrefix = encounterToProcess.HasInvalidAccessCodePrefix();
                
                // Override shouldApprove if the access code has an invalid prefix
                bool shouldApprove = encounterToProcess.shouldApprove;
                if (hasInvalidPrefix)
                {
                    shouldApprove = false;
                    
                    // Update the invalid reason if it's not set yet
                    if (string.IsNullOrEmpty(encounterToProcess.invalidReason))
                    {
                        encounterToProcess.invalidReason = "Invalid access code prefix";
                    }
                    
                    Debug.Log($"MasterShipGenerator: Invalid access code prefix detected: {encounterToProcess.accessCode} - Ship should be denied");
                }
                
                // Check if decision was correct (using our adjusted shouldApprove value)
                bool correctDecision = (approved == shouldApprove);
                
                // Apply consequences
                if (!correctDecision)
                {
                    // Apply penalties based on encounter data
                    int credits = encounterToProcess.creditPenalty;
                    int casualties = encounterToProcess.casualtiesIfWrong;
                    
                    Debug.LogWarning($"Incorrect decision! Penalty: {credits} credits, {casualties} casualties");
                    
                    // Here we could update game state, trigger UI feedback, etc.
                }
                else
                {
                    if (!approved && hasInvalidPrefix)
                    {
                        Debug.Log($"Correct decision! Ship denied due to invalid access code prefix: {encounterToProcess.accessCode}");
                    }
                    else
                    {
                        Debug.Log($"Correct decision! Ship {(approved ? "approved" : "denied")}");
                    }
                }
                
                // Note: We don't set decision state here - it was already set in CredentialChecker
                
                // Store encounter for debugging purposes (especially for null reference issues)
                MasterShipEncounter processedEncounter = encounterToProcess;
                
                // Double-check to make sure we're not double-clearing the current encounter
                if (currentEncounter == encounterToProcess)
                {
                    // Clear current encounter reference
                    Debug.Log("Clearing current encounter reference");
                    currentEncounter = null;
                }
                else
                {
                    Debug.Log("Current encounter already cleared or different from processed encounter");
                }
                
                // Only auto-generate next encounter if the new media system isn't handling it
                var mediaTransitionManager = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
                if (mediaTransitionManager == null)
                {
                    // Old system - generate next encounter after a short delay
                    StartCoroutine(DelayedNextEncounter(1.0f));
                }
                else
                {
                    Debug.Log("MasterShipGenerator: Skipping auto-generation - EncounterMediaTransitionManager will handle next encounter");
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"MasterShipGenerator: Error in ProcessEncounterInternal: {e.Message}\n{e.StackTrace}");
                
                // Safety cleanup
                currentEncounter = null;
                
                // Try to queue a new encounter after a delay to recover
                var mediaTransitionManagerError = ServiceLocator.Get<Starkiller.Core.Managers.EncounterMediaTransitionManager>();
                if (mediaTransitionManagerError == null)
                {
                    StartCoroutine(DelayedNextEncounter(2.0f));
                }
            }
        }

        /// <summary>
        /// Get the next encounter after a delay
        /// </summary>
        private IEnumerator DelayedNextEncounter(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            // Check if we need to generate more encounters
            if (pendingEncounters.Count == 0)
            {
                // Generate another day's worth
                Debug.Log("Generating more encounters as we've run out");
                GenerateEncountersForDay(currentDay);
            }
            
            // Get the next encounter
            if (pendingEncounters.Count > 0)
            {
                GetNextEncounter();
            }
        }

        /// <summary>
        /// Generate new valid access codes for the current day
        /// NOTE: This is now handled by ContentManager using AccessCode ScriptableObjects
        /// </summary>
        private void RegenerateAccessCodes()
        {
            // This method is deprecated - ContentManager now handles access codes
            Debug.LogWarning("RegenerateAccessCodes called but ContentManager should handle this");
            
            // If contentManager is not available, we can't generate codes
            if (contentManager != null)
            {
                // Force contentManager to regenerate its codes
                contentManager.RegenerateAccessCodes();
                
                // Then copy them to our local list
                currentAccessCodes = new List<string>(contentManager.currentAccessCodes);
            }
            else
            {
                Debug.LogError("MasterShipGenerator: No ContentManager available to generate access codes!");
                // Create at least one emergency fallback code
                currentAccessCodes.Clear();
                currentAccessCodes.Add("EMG-0999");
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the name of the calling method for diagnostics
        /// </summary>
        private string GetCallerMethodName()
        {
            string callerMethod = "GetNextEncounter";
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            if (stackTrace.FrameCount > 1 && stackTrace.GetFrame(1).GetMethod() != null)
            {
                callerMethod = stackTrace.GetFrame(1).GetMethod().Name;
            }
            return callerMethod;
        }

        /// <summary>
        /// Checks if the game is in an active gameplay state
        /// </summary>
        private bool IsGameplayActive()
        {
            GameStateController controller = GameStateController.Instance;
            return controller == null || controller.IsGameplayActive();
        }

        /// <summary>
        /// Creates a waiting encounter for when the game is not in active state
        /// </summary>
        private MasterShipEncounter CreateWaitingForGameplayEncounter()
        {
            Debug.Log("MasterShipGenerator: Game not in active gameplay state, deferring encounter generation");

            // Create a test encounter but don't notify yet - it will be picked up when the game becomes active
            MasterShipEncounter pendingEncounter = MasterShipEncounter.CreateTestEncounter();
            pendingEncounter.shipType = "Waiting for Gameplay";
            pendingEncounter.story = "Please start the game to begin processing ships.";
            
            // Don't notify listeners yet - just return the encounter
            return pendingEncounter;
        }

        /// <summary>
        /// Checks with the timing controller if we can generate a new ship
        /// </summary>
        private bool CanGenerateNewShip(string callerMethod)
        {
            ShipTimingController timingController = ShipTimingController.Instance;
            return timingController == null || timingController.CanGenerateNewShip(callerMethod);
        }

        /// <summary>
        /// Handles the case where the timing controller delays ship generation
        /// </summary>
        private MasterShipEncounter HandleTimingControllerDelay()
        {
            Debug.Log($"MasterShipGenerator: Ship generation delayed by timing controller");
            
            // If we already have a current encounter, just return it again
            if (currentEncounter != null)
            {
                Debug.Log($"MasterShipGenerator: Returning existing encounter due to timing constraints");
                return currentEncounter;
            }
            
            // Create a waiting encounter
            MasterShipEncounter waitingEncounter = new MasterShipEncounter();
            waitingEncounter.shipType = "Incoming Ship";
            waitingEncounter.shipName = "Waiting";
            waitingEncounter.story = "Ship approaching - standby...";
            waitingEncounter.shouldApprove = true; // Default to valid for testing
            
            // Don't notify listeners yet - just return the waiting encounter
            return waitingEncounter;
        }

        /// <summary>
        /// Gets the next encounter from the queue
        /// </summary>
        private MasterShipEncounter DequeueNextEncounter()
        {
            currentEncounter = pendingEncounters.Dequeue();
            
            // Double-check for null (shouldn't happen, but being defensive)
            if (currentEncounter == null)
            {
                Debug.LogError("MasterShipGenerator: Dequeued a null encounter! Creating fallback encounter.");
                currentEncounter = MasterShipEncounter.CreateTestEncounter();
            }
            
            // Notify listeners that a new encounter is ready
            if (OnEncounterReady != null)
            {
                OnEncounterReady.Invoke(currentEncounter);
                Debug.Log($"MasterShipGenerator: New encounter ready: {currentEncounter.shipType}");
            }
            else
            {
                Debug.LogWarning("MasterShipGenerator: OnEncounterReady event has no subscribers!");
            }
            
            // Notify timing controller about the new encounter
            NotifyTimingControllerEncounterStarted(currentEncounter);
            
            return currentEncounter;
        }

        /// <summary>
        /// Creates a fallback encounter when no more are in the queue
        /// </summary>
        private MasterShipEncounter CreateFallbackEncounter()
        {
            // Create a test encounter with useful debug information
            currentEncounter = MasterShipEncounter.CreateTestEncounter();
            currentEncounter.shipType = "Emergency Fallback Ship";
            currentEncounter.accessCode = "SK-9999";
            currentEncounter.story = "Emergency fallback ship generated due to missing encounters.";
            currentEncounter.shouldApprove = true; // Always make fallbacks valid for easier testing
            
            // Add a timestamp to make each fallback unique for debugging
            long timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            currentEncounter.shipName = $"Fallback-{timestamp}";
            
            // Log this special fallback creation with details for debugging
            Debug.LogWarning($"Created EMERGENCY fallback encounter: {currentEncounter.shipName} - This should not happen in production!");
            
            // Enhance with videos if possible
            if (mediaSystem != null && mediaSystem.mediaDatabase != null)
            {
                currentEncounter.EnhanceWithVideos(mediaSystem.mediaDatabase);
            }
            
            // Always notify listeners, even if it might be null (we'll handle errors in the listener)
            try
            {
                OnEncounterReady?.Invoke(currentEncounter);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error notifying listeners of fallback encounter: {e.Message}");
            }
            
            // Notify timing controller about the new encounter
            NotifyTimingControllerEncounterStarted(currentEncounter);
            
            return currentEncounter;
        }

        /// <summary>
        /// Creates an error recovery encounter when an exception occurs
        /// </summary>
        private MasterShipEncounter CreateErrorRecoveryEncounter(System.Exception e)
        {
            // Create emergency fallback encounter with clear error details
            MasterShipEncounter emergencyFallback = MasterShipEncounter.CreateTestEncounter();
            emergencyFallback.shipType = "CRITICAL ERROR SHIP";
            emergencyFallback.accessCode = "ERROR-" + Random.Range(1000, 10000);
            emergencyFallback.story = $"Emergency recovery ship created after error: {e.Message.Substring(0, System.Math.Min(e.Message.Length, 100))}...";
            emergencyFallback.shouldApprove = true; // Always make error ships valid
            emergencyFallback.shipName = "ErrorRecovery-" + System.DateTime.Now.Ticks;
            
            currentEncounter = emergencyFallback;
            
            // Log with high visibility
            Debug.LogError($"CREATED EMERGENCY ERROR RECOVERY SHIP: {emergencyFallback.shipName}");
            Debug.LogError($"Error details: {e.Message}");
            
            // Attempt to notify listeners, but don't crash if this fails
            try
            {
                OnEncounterReady?.Invoke(currentEncounter);
            }
            catch (System.Exception notifyError)
            {
                Debug.LogError($"MasterShipGenerator: Failed to notify listeners of emergency encounter: {notifyError.Message}");
            }
            
            // Notify timing controller that encounter has started
            NotifyTimingControllerEncounterStarted(currentEncounter);

            // Add debugging info for DebugMonitor
            DebugMonitor debugMonitor = DebugMonitor.Instance;
            if (debugMonitor != null)
            {
                // Get the calling method name
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
                string callerMethod = "Unknown";
                
                // Look 2 frames up to find who called GetNextEncounter
                if (stackTrace.FrameCount > 2)
                {
                    var frame = stackTrace.GetFrame(2);
                    if (frame != null && frame.GetMethod() != null)
                    {
                        var method = frame.GetMethod();
                        callerMethod = $"{method.DeclaringType.Name}.{method.Name}";
                    }
                }
                
                debugMonitor.TrackDataValue("GetNextEncounter.CalledBy", callerMethod);
            }

            return currentEncounter;
        }

        /// <summary>
        /// Handles the case where we need to process a decision with a null encounter
        /// </summary>
        private void HandleNullEncounterForDecision(bool approved)
        {
            Debug.LogError("MasterShipGenerator: Cannot process decision with null encounter!");
            
            // Track this error for debugging
            DebugMonitor debugMonitor = DebugMonitor.Instance;
            if (debugMonitor != null) {
                debugMonitor.TrackDataValue("ProcessDecisionWithEncounter", "Null encounter received");
                
                // Add stack trace for diagnostic purposes
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
                debugMonitor.TrackDataValue("ProcessDecisionWithEncounter.StackTrace", stackTrace.ToString());
            }
            
            // Try to recover by using the current encounter if available
            if (currentEncounter != null)
            {
                Debug.Log("MasterShipGenerator: Attempting to recover using current encounter...");
                ProcessEncounterInternal(approved, currentEncounter);
            }
            else
            {
                // Create a new encounter as a last resort
                Debug.LogWarning("MasterShipGenerator: Creating a new encounter for recovery...");
                GetNextEncounter();
            }
        }

        /// <summary>
        /// Tries to recover when there's no active encounter to process
        /// </summary>
        private void RecoverFromMissingEncounter()
        {
            // Try to get the next encounter as a recovery measure
            if (pendingEncounters.Count > 0)
            {
                GetNextEncounter();
            }
            else
            {
                // Generate new encounters if we're out
                GenerateEncountersForDay(currentDay);
                if (pendingEncounters.Count > 0)
                {
                    GetNextEncounter();
                }
            }
        }

        /// <summary>
        /// Notifies the timing controller that an encounter has started
        /// </summary>
        private void NotifyTimingControllerEncounterStarted(MasterShipEncounter encounter)
        {
            ShipTimingController timingController = ShipTimingController.Instance;
            if (timingController != null && encounter != null)
            {
                timingController.NotifyEncounterStarted(encounter);
            }
        }

        /// <summary>
        /// Notifies the timing controller that an encounter has ended
        /// </summary>
        private void NotifyTimingControllerEncounterEnded()
        {
            ShipTimingController timingController = ShipTimingController.Instance;
            if (timingController != null)
            {
                timingController.NotifyEncounterEnded();
            }
        }

        /// <summary>
        /// Select a scenario based on validity requirements and day progression
        /// </summary>
        private ShipScenario SelectScenario(bool shouldBeValid)
        {
            // First check if we have a scenario provider available
            if (hasScenarioProvider && scenarioProvider != null)
            {
                // Use the provider to get an appropriate scenario
                ShipScenario scenario = scenarioProvider.GetScenario(shouldBeValid, currentDay);
                if (scenario != null)
                {
                    // Track usage of the scenario
                    usedScenarios.Add(scenario);
                    return scenario;
                }
            }
            
            // If we get here, either no provider is available or it returned null
            // Fall back to original logic
            List<ShipScenario> validScenarios = new List<ShipScenario>();
            
            foreach (var scenario in allScenarios)
            {
                // Skip null or story scenarios for random encounters
                if (scenario == null || scenario.isStoryMission)
                    continue;
                    
                // Check if the scenario matches our validity needs
                if (scenario.shouldBeApproved == shouldBeValid)
                {
                    // Check if the scenario is available for the current day
                    if (scenario.dayFirstAppears <= currentDay)
                    {
                        // Check if we've used this scenario too many times
                        if (scenario.maxAppearances < 0 || 
                            CountScenarioAppearances(scenario, usedScenarios) < scenario.maxAppearances)
                        {
                            validScenarios.Add(scenario);
                        }
                    }
                }
            }
            
            // If no valid scenarios found, try to get one from provider again as a last resort
            if (validScenarios.Count == 0)
            {
                Debug.LogWarning("No valid scenarios found in local collection");
                
                // Try provider again with forced creation
                if (hasScenarioProvider && scenarioProvider != null)
                {
                    ShipScenario emergencyScenario = scenarioProvider.GetScenario(shouldBeValid, currentDay);
                    if (emergencyScenario != null)
                    {
                        Debug.Log("Using emergency scenario from provider");
                        usedScenarios.Add(emergencyScenario);
                        return emergencyScenario;
                    }
                }
                
                // As a final fallback, create one directly
                Debug.LogWarning("Creating fallback scenario directly");
                return CreateFallbackScenario(shouldBeValid);
            }
            
            // Select a random scenario from the valid ones
            ShipScenario selected = validScenarios[Random.Range(0, validScenarios.Count)];
            usedScenarios.Add(selected);
            return selected;
        }
        
        /// <summary>
        /// Select a ship type that works with the given scenario
        /// </summary>
        private ShipType SelectShipType(ShipScenario scenario)
        {
            // Safety check for null scenario
            if (scenario == null)
            {
                Debug.LogError("MasterShipGenerator: Cannot select ship type - scenario is null!");
                return CreateDefaultShipType();
            }
            
            // Safety check for empty ship type list
            if (allShipTypes == null || allShipTypes.Count == 0)
            {
                Debug.LogError("MasterShipGenerator: No ship types available!");
                return CreateDefaultShipType();
            }
            
            List<ShipType> validShips = new List<ShipType>();
            
            // If the scenario specifies applicable ship types, use those
            if (scenario.applicableShipTypes != null && scenario.applicableShipTypes.Length > 0)
            {
                foreach (var shipType in scenario.applicableShipTypes)
                {
                    if (shipType != null)
                        validShips.Add(shipType);
                }
            }
            
            // If no valid ships from scenario, use all available ship types
            if (validShips.Count == 0)
            {
                foreach (var shipType in allShipTypes)
                {
                    if (shipType != null)
                        validShips.Add(shipType);
                }
            }
            
            // If recentShipTypes is null, initialize it
            if (recentShipTypes == null)
                recentShipTypes = new List<ShipType>();
                
            // Remove recently used ships if possible to ensure variety
            if (recentShipTypes.Count > 0)
            {
                List<ShipType> recentShipsToRemove = new List<ShipType>();
                foreach (var recentShip in recentShipTypes)
                {
                    if (recentShip != null && validShips.Count > 3) // Keep at least a few options
                    {
                        recentShipsToRemove.Add(recentShip);
                    }
                }
                
                // Remove ships in a separate loop to avoid collection modification during enumeration
                foreach (var shipToRemove in recentShipsToRemove)
                {
                    validShips.Remove(shipToRemove);
                }
            }
            
            // If no valid ships found after all filtering, create a default one
            if (validShips.Count == 0)
            {
                Debug.LogWarning("MasterShipGenerator: No valid ships found, using fallback");
                return CreateDefaultShipType();
            }
            
            // Select a random ship from the valid ones
            ShipType selected = validShips[Random.Range(0, validShips.Count)];
            
            // Track recently used ships
            recentShipTypes.Add(selected);
            if (recentShipTypes.Count > 5) // Keep track of last 5 ships
            {
                recentShipTypes.RemoveAt(0);
            }
            
            return selected;
        }
        
        /// <summary>
        /// Create a default ship type for error recovery
        /// </summary>
        private ShipType CreateDefaultShipType()
        {
            ShipType defaultShip = ScriptableObject.CreateInstance<ShipType>();
            defaultShip.typeName = "Backup Imperial Vessel";
            
            // Create a category
            ShipCategory category = ScriptableObject.CreateInstance<ShipCategory>();
            category.categoryName = "Imperium";
            defaultShip.category = category;
            
            defaultShip.minCrewSize = 10;
            defaultShip.maxCrewSize = 50;
            defaultShip.commonOrigins = new string[] { "Imperial Fleet", "Starkiller Base" };
            
            return defaultShip;
        }
        
        /// <summary>
        /// Select a captain type that works with the given scenario and ship
        /// </summary>
        private CaptainType SelectCaptainType(ShipScenario scenario, ShipType shipType)
        {
            // Safety check for null parameters
            if (scenario == null)
            {
                Debug.LogError("MasterShipGenerator: Cannot select captain type - scenario is null!");
                return CreateDefaultCaptainType();
            }
            
            if (shipType == null)
            {
                Debug.LogError("MasterShipGenerator: Cannot select captain type - shipType is null!");
                return CreateDefaultCaptainType();
            }
            
            // Safety check for empty captain list
            if (allCaptainTypes == null || allCaptainTypes.Count == 0)
            {
                Debug.LogError("MasterShipGenerator: No captain types available!");
                return CreateDefaultCaptainType();
            }
            
            List<CaptainType> validCaptains = new List<CaptainType>();
            
            // If the scenario specifies applicable captain types, use those
            if (scenario.applicableCaptainTypes != null && scenario.applicableCaptainTypes.Length > 0)
            {
                foreach (var captainType in scenario.applicableCaptainTypes)
                {
                    if (captainType != null)
                        validCaptains.Add(captainType);
                }
            }
            
            // If no valid captains from scenario, filter by ship type using proper faction validation
            if (validCaptains.Count == 0)
            {
                foreach (var captain in allCaptainTypes)
                {
                    if (captain == null) continue; // Skip null entries
                    
                    // Use proper faction validation with ShipCategory's compatibleCaptainFactions
                    if (shipType.category != null && captain.factions != null && captain.factions.Length > 0)
                    {
                        bool isCompatible = false;
                        foreach (var captainFaction in captain.factions)
                        {
                            if (captainFaction != null && shipType.category.IsCaptainCompatible(captainFaction))
                            {
                                isCompatible = true;
                                break;
                            }
                        }
                        
                        if (isCompatible)
                        {
                            validCaptains.Add(captain);
                            Debug.Log($"Captain {captain.typeName} is compatible with ship category {shipType.category.categoryName}");
                        }
                        else
                        {
                            Debug.Log($"Captain {captain.typeName} is NOT compatible with ship category {shipType.category.categoryName}");
                        }
                    }
                    else
                    {
                        // If no ship category or captain factions, add all non-null captains as fallback
                        validCaptains.Add(captain);
                        Debug.Log($"Added captain {captain.typeName} as fallback (no category/faction validation possible)");
                    }
                }
            }
            
            // Remove recently used captains if possible to ensure variety
            if (recentCaptainTypes != null && recentCaptainTypes.Count > 0)
            {
                foreach (var recentCaptain in recentCaptainTypes)
                {
                    if (recentCaptain != null && validCaptains.Count > 3) // Keep at least a few options
                    {
                        validCaptains.Remove(recentCaptain);
                    }
                }
            }
            
            // If no valid captains found, use all non-null captains
            if (validCaptains.Count == 0)
            {
                foreach (var captain in allCaptainTypes)
                {
                    if (captain != null)
                        validCaptains.Add(captain);
                }
            }
            
            // If still no valid captains, create a default one
            if (validCaptains.Count == 0)
            {
                Debug.LogWarning("MasterShipGenerator: No valid captains found, using fallback");
                return CreateDefaultCaptainType();
            }
            
            // Select a random captain from the valid ones
            CaptainType selected = validCaptains[Random.Range(0, validCaptains.Count)];
            
            // Initialize recentCaptainTypes if null
            if (recentCaptainTypes == null)
                recentCaptainTypes = new List<CaptainType>();
                
            // Track recently used captains
            recentCaptainTypes.Add(selected);
            if (recentCaptainTypes.Count > 5) // Keep track of last 5 captains
            {
                recentCaptainTypes.RemoveAt(0);
            }
            
            return selected;
        }
        
        /// <summary>
        /// Create a fallback captain type for error recovery
        /// </summary>
        private CaptainType CreateDefaultCaptainType()
        {
            CaptainType defaultCaptain = ScriptableObject.CreateInstance<CaptainType>();
            defaultCaptain.typeName = "Backup Captain";
            defaultCaptain.factions = new string[] { "Imperium" };
            defaultCaptain.commonRanks = new string[] { "Captain" };
            defaultCaptain.possibleFirstNames = new string[] { "John", "Jane" };
            defaultCaptain.possibleLastNames = new string[] { "Doe" };
            return defaultCaptain;
        }
        
        /// <summary>
        /// Create a fallback scenario when no suitable scenario is found
        /// </summary>
        private ShipScenario CreateFallbackScenario(bool shouldBeValid)
        {
            ShipScenario fallback = ScriptableObject.CreateInstance<ShipScenario>();
            
            if (shouldBeValid)
            {
                fallback.scenarioName = "Standard Delivery";
                fallback.type = ShipScenario.ScenarioType.Standard;
                fallback.shouldBeApproved = true;
                
                string[] possibleStories = new string[] {
                    "Ship requesting routine clearance for supply delivery.",
                    "Regular transport vessel on scheduled run.",
                    "Logistics vessel with standard authorisation.",
                    "Supply ship with scheduled delivery.",
                    "Personnel transport on regular duty rotation."
                };
                fallback.possibleStories = possibleStories;
                
                string[] possibleManifests = new string[] {
                    "Standard supplies and provisions for the base.",
                    "Personnel and equipment for routine rotation.",
                    "Food supplies, spare parts, and maintenance equipment.",
                    "Medical supplies, rations, and basic equipment.",
                    "Personnel transfer documents and standard cargo."
                };
                fallback.possibleManifests = possibleManifests;
            }
            else
            {
                fallback.scenarioName = "Invalid Authorisation";
                fallback.type = ShipScenario.ScenarioType.Invalid;
                fallback.shouldBeApproved = false;
                fallback.invalidReason = GetRandomInvalidReason();
                
                string[] possibleStories = new string[] {
                    "Ship requesting emergency clearance with unusual urgency.",
                    "Vessel claiming critical system failure requiring immediate docking.",
                    "Transport insisting on immediate access for unscheduled delivery.",
                    "Ship citing special authority clearance not in the system.",
                    "Unscheduled vessel demanding priority docking clearance."
                };
                fallback.possibleStories = possibleStories;
                
                string[] possibleManifests = new string[] {
                    "Cargo manifest with multiple discrepancies.",
                    "Unverified shipment with vague description.",
                    "Personnel transfer with incomplete documentation.",
                    "Special cargo requiring inspection before delivery.",
                    "Unscheduled delivery with authorization pending."
                };
                fallback.possibleManifests = possibleManifests;
            }
            
            // Always set consequence and day-related fields
            fallback.possibleConsequences = new string[] { 
                shouldBeValid ? "Delay in critical supplies." : "Security breach detected." 
            };
            fallback.dayFirstAppears = 1;
            fallback.maxAppearances = -1;
            
            return fallback;
        }

        /// <summary>
        /// Get a random invalid reason for variety
        /// </summary>
        private string GetRandomInvalidReason()
        {
            string[] reasons = new string[] {
                "Invalid access code",
                "Expired authorisation",
                "Mismatched credentials",
                "Unscheduled arrival",
                "Security protocol violation",
                "Missing clearance documentation",
                "Suspicious manifest discrepancies"
            };
            
            return reasons[Random.Range(0, reasons.Length)];
        }
        
        /// <summary>
        /// Count the number of times a specific scenario has been used
        /// </summary>
        private int CountScenarioAppearances(ShipScenario scenario, List<ShipScenario> list)
        {
            int count = 0;
            foreach (var used in list)
            {
                if (used == scenario)
                    count++;
            }
            return count;
        }
        
        /// <summary>
        /// Validate that we have all the data we need
        /// </summary>
        private void ValidateData()
        {
            // Check that we have at least one of each data type
            if (allShipTypes.Count == 0)
            {
                Debug.LogError("No ship types found. Please assign ship types in the inspector or assign a ContentManager.");
            }
            
            if (allCaptainTypes.Count == 0)
            {
                Debug.LogError("No captain types found. Please assign captain types in the inspector or assign a ContentManager.");
            }
            
            if (allScenarios.Count == 0)
            {
                Debug.LogError("No scenarios found. Please assign scenarios in the inspector or assign a ContentManager.");
            }
        }
        #endregion
    }
