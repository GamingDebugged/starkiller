'StarkillerBaseCommand.ResourcePathManager' is missing the class attribute 'ExtensionOfNativeClass'!

UnityMCPBridge started on port 6400.
UnityEngine.Debug:Log (object)
UnityMCP.Editor.UnityMCPBridge:Start () (at Library/PackageCache/com.justinpbarnett.unity-mcp/Editor/UnityMCPBridge.cs:52)
UnityMCP.Editor.UnityMCPBridge:.cctor () (at Library/PackageCache/com.justinpbarnett.unity-mcp/Editor/UnityMCPBridge.cs:42)
UnityEditor.EditorAssemblies:ProcessInitializeOnLoadAttributes (System.Type[]) (at /Users/bokken/build/output/unity/unity/Editor/Mono/EditorAssemblies.cs:118)

=== GameReset: Resetting game state ===
UnityEngine.Debug:Log (object)
GameReset:Awake () (at Assets/Scripts/GameReset.cs:13)

[ServiceLocator] Registered service: PersonalDataLogManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.PersonalDataLogManager> (Starkiller.Core.Managers.PersonalDataLogManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.PersonalDataLogManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/PersonalDataLogManager.cs:56)

[DayProgressionManager] CurrentDay is 0 which is less than 1! Returning 1 as fallback.
UnityEngine.Debug:LogError (object)
Starkiller.Core.Managers.DayProgressionManager:get_CurrentDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:55)
GameManager:get_currentDay () (at Assets/_scripts/GameManager.cs:118)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:121)

ContentManager synced with GameManager day: 1
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:122)

ResourcePathManager: Found 13 AccessCode at Resources/_ScriptableObjects/AccessCodes
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.ResourcePathManager:LoadAll<StarkillerBaseCommand.AccessCode> (StarkillerBaseCommand.ResourcePathManager/ResourceType) (at Assets/ResourcePathManager.cs:150)
StarkillerBaseCommand.ResourceLoadingHelper:LoadAccessCodes () (at Assets/ResourcePathManager.cs:309)
StarkillerBaseCommand.StarkkillerContentManager:InitializeContentDatabases () (at Assets/_scripts/StarkkillerContentManager.cs:186)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:126)

ResourcePathManager: Total AccessCode loaded: 13
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.ResourcePathManager:LoadAll<StarkillerBaseCommand.AccessCode> (StarkillerBaseCommand.ResourcePathManager/ResourceType) (at Assets/ResourcePathManager.cs:172)
StarkillerBaseCommand.ResourceLoadingHelper:LoadAccessCodes () (at Assets/ResourcePathManager.cs:309)
StarkillerBaseCommand.StarkkillerContentManager:InitializeContentDatabases () (at Assets/_scripts/StarkkillerContentManager.cs:186)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:126)

Loaded 13 access codes
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:InitializeContentDatabases () (at Assets/_scripts/StarkkillerContentManager.cs:194)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:126)

Loaded 7 valid access codes for day 1
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:357)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Civilian Transport 3001 (CIV-3001) is VALID
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:364)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Emergency Override (EMG-0999) is VALID
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:364)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Fake Bounty (8NT) (8NT-8001) is INVALID - REVOKED
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:371)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Fake Military (M1L) (M1L-1001) is INVALID - REVOKED
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:371)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Fake Trade (TRO) (TRO-6001) is INVALID - REVOKED
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:371)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Fake VIP (V1P) (V1P-9001) is INVALID - REVOKED
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:371)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Licensed Bounty Hunter 8001 (BNT-8001) is INVALID - NOT YET ACTIVE
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:371)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Military Access 1001 (MIL-1001) is VALID
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:364)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Special Operations (SPL-7777) is VALID
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:364)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Standard Access 5001 (STD-5001) is VALID
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:364)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Temporary Pass Day 5-7 (TMP-4001) is INVALID - NOT YET ACTIVE
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:371)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: Trade Union Pass 6001 (TRD-6001) is VALID
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:364)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Day 1: VIP Clearance 9001 (VIP-9001) is VALID
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:364)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:131)

Ship video player configured successfully
UnityEngine.Debug:Log (object)
VideoPlayerSetup:ConfigureVideoPlayers () (at Assets/VideoPlayerSetup.cs:126)
VideoPlayerSetup:Awake () (at Assets/VideoPlayerSetup.cs:38)

Captain video player configured successfully
UnityEngine.Debug:Log (object)
VideoPlayerSetup:ConfigureVideoPlayers () (at Assets/VideoPlayerSetup.cs:148)
VideoPlayerSetup:Awake () (at Assets/VideoPlayerSetup.cs:38)

[ServiceLocator] Registered service: DecisionTracker
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.DecisionTracker> (Starkiller.Core.Managers.DecisionTracker) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.DecisionTracker:Awake () (at Assets/_RefactoredScripts/Core/Managers/DecisionTracker.cs:47)

[TimeManager] TimeManager initialized with default scale 1
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:Awake () (at Assets/TimeManager.cs:82)
UnityEngine.GameObject:AddComponent<TimeManager> ()
TimeManager:get_Instance () (at Assets/TimeManager.cs:60)
TimeModifierBehavior:ApplyTimeModification () (at Assets/TimeModifierBehavior.cs:56)
TimeModifierBehavior:OnEnable () (at Assets/TimeModifierBehavior.cs:29)

[TimeManager] Added time pause: DailyBriefingPanel_49b0de00 from DailyBriefingPanel
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:PauseTime (UnityEngine.GameObject,string) (at Assets/TimeManager.cs:107)
TimeModifierBehavior:ApplyTimeModification () (at Assets/TimeModifierBehavior.cs:64)
TimeModifierBehavior:OnEnable () (at Assets/TimeModifierBehavior.cs:29)

[TimeManager] Time PAUSED: 1 active modifiers
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:UpdateTimeState () (at Assets/TimeManager.cs:216)
TimeManager:PauseTime (UnityEngine.GameObject,string) (at Assets/TimeManager.cs:111)
TimeModifierBehavior:ApplyTimeModification () (at Assets/TimeModifierBehavior.cs:64)
TimeModifierBehavior:OnEnable () (at Assets/TimeModifierBehavior.cs:29)

[ServiceLocator] Registered service: AudioManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.AudioManager> (Starkiller.Core.Managers.AudioManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.AudioManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/AudioManager.cs:60)

[ServiceLocator] Registered service: GameOverManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.GameOverManager> (Starkiller.Core.Managers.GameOverManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.GameOverManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/GameOverManager.cs:96)

[ServiceLocator] Registered service: SalaryManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.SalaryManager> (Starkiller.Core.Managers.SalaryManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.SalaryManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/SalaryManager.cs:85)

ManagerInitializer: Made _Managers persistent across scenes
UnityEngine.Debug:Log (object)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:31)

ManagerInitializer managing 46 components:
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:75)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - ManagerInitializer on _Managers
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - EncounterSystemManager on EncounterSystem
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - ShipTransitionController on EncounterSystem
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - MasterShipGenerator on EncounterSystem
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - GameManagerIntegrationHelper on Diagnostics
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - EncounterSystemMigrationManager on MigrationManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - EncounterFlowManager on EncounterFlowManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - FactionManager on FactionManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - ShipSystemInitializer on StarkkillerSystems
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - StarkkillerContentManager on StarkkillerContentManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - StarkkillerMediaSystem on StarkkillerMediaSystem
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - LegacySystemsMigrator on LegacySystemsMigrator
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - ConsequenceManager on ConsequenceManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - AudioManager on AudioManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - ShipGeneratorCoordinator on ShipEncounterCoordinator
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - ShipTransitionController on ShipTransitionController
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - ShipTimingController on ShipTransitionController
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - GameManager on GameManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - GameStateController on GameManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - CaptainVideoResponseManager on GameManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - ImperialFamilySystem on FamilySystem
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - ReportSystemConnector on SystemConnector
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - NarrativeManager on NarrativeManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - NewsManager on NewsManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - DraggablePanelManager on DraggablePanelManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - VideoTransitionManager on VideoTransitionManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - GameStateManager on GameStateManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - CreditsManager on CreditsManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - TestManagerExtraction on ManagerTester
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - DayProgressionManager on DayProgressionManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - UICoordinator on UICoordinator
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - EncounterManager on EncounterManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - SaveGameManager on SaveGameManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - NotificationManager on NotificationManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - ShiftTimerManager on ShiftTimerManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - PerformanceManager on PerformanceManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - MoralChoiceManager on MoralChoiceManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - LoyaltyManager on LoyaltyManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - SalaryManager on SalaryManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - BriberyManager on BriberyManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - InspectionManager on InspectionManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - GameOverManager on GameOverManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - DailyReportManager on DailyReportManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

  - EncounterMediaTransitionManager on MediaTransitionManager
UnityEngine.Debug:Log (object)
ManagerInitializer:LogManagedComponents () (at Assets/Scripts/ManagerInitializer.cs:80)
ManagerInitializer:Awake () (at Assets/Scripts/ManagerInitializer.cs:41)

ShipScenarioProvider: Searching for scenarios in Resources folders...
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:169)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Attempting to load scenarios from: ScriptableObjects/Scenarios
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:173)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Attempting to load scenarios from: Scenarios
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:173)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Attempting to load scenarios from: _ScriptableObjects/Scenarios
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:173)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Loaded 12 scenarios from Resources/_ScriptableObjects/Scenarios
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:178)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added story scenario: BountyHunterChase with tag: HelpTraitor
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:188)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added story scenario: Defecting Officer with tag: defecting_officer
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:188)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added story scenario: Imperium Inspection with tag: imperium_inspection
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:188)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added valid scenario: Medical Emergency
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:193)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added story scenario: Rebel Infiltration Attempt with tag: insurgent_infiltration
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:188)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added invalid scenario: Sabotaged Ship
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:198)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added valid scenario: Standard Supply Run
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:193)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added invalid scenario: Suspicious Cargo
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:198)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added story scenario: Trade Union Negotiation Team with tag: trade_union_negotiation
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:188)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added valid scenario: Undercover Intelligence Operation
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:193)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added valid scenario: VIP Transport
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:193)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

Added story scenario: Visiting Family with tag: visitingfamily
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:188)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

ShipScenarioProvider final counts - Valid: 14, Invalid: 2, Story: 6
UnityEngine.Debug:Log (object)
ShipScenarioProvider:LoadScenariosFromResources () (at Assets/_scripts/ShipScenarioProvider.cs:223)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:54)

ShipScenarioProvider initialized with 14 valid, 2 invalid, and 6 story scenarios
UnityEngine.Debug:Log (object)
ShipScenarioProvider:Awake () (at Assets/_scripts/ShipScenarioProvider.cs:63)

[ServiceLocator] Registered service: InspectionManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.InspectionManager> (Starkiller.Core.Managers.InspectionManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.InspectionManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/InspectionManager.cs:105)

[InspectionManager] Initialized and registered with ServiceLocator
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.InspectionManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/InspectionManager.cs:108)

[ServiceLocator] Registered service: LoyaltyManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.LoyaltyManager> (Starkiller.Core.Managers.LoyaltyManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.LoyaltyManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/LoyaltyManager.cs:97)

EncounterSystemManager detected systems: MasterShipGenerator ✓ LegacyEncounterSystem ✗ StarkkillerEncounterSystem ✗ ShipGeneratorCoordinator ✓ 
UnityEngine.Debug:Log (object)
EncounterSystemManager:LogDetectedSystems () (at Assets/EncounterSystemManager.cs:119)
EncounterSystemManager:Awake () (at Assets/EncounterSystemManager.cs:58)

MasterShipGenerator: Found ManagerInitializer parent - persistence will be handled by it
UnityEngine.Debug:Log (object)
MasterShipGenerator:InitializeSingleton () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:190)
MasterShipGenerator:Awake () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:100)

MasterShipGenerator initialized. ContentManager found: True, MediaSystem found: True, GameManager found: True
UnityEngine.Debug:Log (object)
MasterShipGenerator:FindRequiredComponents () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:218)
MasterShipGenerator:Awake () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:101)

[DayProgressionManager] CurrentDay is 0 which is less than 1! Returning 1 as fallback.
UnityEngine.Debug:LogError (object)
Starkiller.Core.Managers.DayProgressionManager:get_CurrentDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:55)
GameManager:get_currentDay () (at Assets/_scripts/GameManager.cs:118)
MasterShipGenerator:SyncWithGameManager () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:228)
MasterShipGenerator:Awake () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:102)

MasterShipGenerator: Synced day with GameManager: 1
UnityEngine.Debug:Log (object)
MasterShipGenerator:SyncWithGameManager () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:229)
MasterShipGenerator:Awake () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:102)

[ServiceLocator] Registered service: NotificationManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.NotificationManager> (Starkiller.Core.Managers.NotificationManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.NotificationManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/NotificationManager.cs:73)

EncounterSystemMigrationManager: Found ManagerInitializer parent - persistence will be handled by it
UnityEngine.Debug:Log (object)
EncounterSystemMigrationManager:Awake () (at Assets/EncounterSystemMigrationManager.cs:57)

ShipImageSystemConnector: Creating ImageSystem to connect with VideoSystem
UnityEngine.Debug:Log (object)
ShipImageSystemConnector:ConnectSystems () (at Assets/_scripts/ShipImageSystemConnector.cs:54)
ShipVideoSystem:TryGetImageSystem () (at Assets/_scripts/ShipVideoSystem.cs:98)
ShipVideoSystem:Awake () (at Assets/_scripts/ShipVideoSystem.cs:45)

ShipImageSystemConnector: Created and configured ShipImageSystem
UnityEngine.Debug:Log (object)
ShipImageSystemConnector:ConnectSystems () (at Assets/_scripts/ShipImageSystemConnector.cs:114)
ShipVideoSystem:TryGetImageSystem () (at Assets/_scripts/ShipVideoSystem.cs:98)
ShipVideoSystem:Awake () (at Assets/_scripts/ShipVideoSystem.cs:45)

ShipVideoSystem: Created ShipImageSystemConnector to establish proper connections
UnityEngine.Debug:Log (object)
ShipVideoSystem:TryGetImageSystem () (at Assets/_scripts/ShipVideoSystem.cs:101)
ShipVideoSystem:Awake () (at Assets/_scripts/ShipVideoSystem.cs:45)

ShipVideoSystem: Successfully connected to ShipImageSystem
UnityEngine.Debug:Log (object)
ShipVideoSystem:TryGetImageSystem () (at Assets/_scripts/ShipVideoSystem.cs:108)
ShipVideoSystem:Awake () (at Assets/_scripts/ShipVideoSystem.cs:45)

Moving FactionManager to root level before applying DontDestroyOnLoad
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.GameObject) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:39)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.MonoBehaviour) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:16)
FactionManager:Awake () (at Assets/_scripts/FactionManager.cs:44)

Successfully moved FactionManager to root and applied DontDestroyOnLoad. Original position: (1024.45, 475.10, -6.15), Original rotation: (0.00000, 0.00000, 0.00000, 1.00000)
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.GameObject) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:51)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.MonoBehaviour) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:16)
FactionManager:Awake () (at Assets/_scripts/FactionManager.cs:44)

FactionManager: Loaded 19 faction entries
UnityEngine.Debug:Log (object)
FactionManager:BuildFactionLookup () (at Assets/_scripts/FactionManager.cs:71)
FactionManager:Awake () (at Assets/_scripts/FactionManager.cs:46)

[ServiceLocator] Registered service: EncounterManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.EncounterManager> (Starkiller.Core.Managers.EncounterManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.EncounterManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:77)

[EncounterManager] Initialized and registered with ServiceLocator
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:80)

[ShipTimingController] Found consequence panel through ConsequenceManager: True
UnityEngine.Debug:Log (object)
ShipTimingController:LogMessage (string) (at Assets/ShipTimingController.cs:723)
ShipTimingController:Awake () (at Assets/ShipTimingController.cs:107)

[ServiceLocator] Registered service: CreditsManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.CreditsManager> (Starkiller.Core.Managers.CreditsManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.CreditsManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/CreditsManager.cs:41)

[ServiceLocator] Registered service: SaveGameManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.SaveGameManager> (Starkiller.Core.Managers.SaveGameManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.SaveGameManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/SaveGameManager.cs:62)

[ServiceLocator] Registered service: PerformanceManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.PerformanceManager> (Starkiller.Core.Managers.PerformanceManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.PerformanceManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:112)

[PerformanceManager] Initialized and registered with ServiceLocator
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.PerformanceManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:118)

ShipGeneratorCoordinator - Available systems: Legacy System ✗ Enhanced Generator ✗ New Generator ✗ Video System ✓ 
UnityEngine.Debug:Log (object)
ShipGeneratorCoordinator:LogAvailableSystems () (at Assets/ShipGeneratorCoordinator.cs:77)
ShipGeneratorCoordinator:Awake () (at Assets/ShipGeneratorCoordinator.cs:66)

EncounterMediaTransitionManager Awake completed with canvas groups initialized
UnityEngine.Debug:Log (object)
EncounterMediaTransitionManager:Awake () (at Assets/Scripts/EncounterMediaTransitionManager.cs:81)

[ServiceLocator] Registered service: EncounterMediaTransitionManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.EncounterMediaTransitionManager> (Starkiller.Core.Managers.EncounterMediaTransitionManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.EncounterMediaTransitionManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:84)

[EncounterMediaTransitionManager] Initialized and registered with ServiceLocator
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:93)

TestingFramework - Encounter Systems Status:
UnityEngine.Debug:Log (object)
TestingFramework:LogMissingComponents () (at Assets/_scripts/TestingFramework.cs:100)
TestingFramework:Awake () (at Assets/_scripts/TestingFramework.cs:85)

  Legacy ShipEncounterSystem: Not Found
UnityEngine.Debug:Log (object)
TestingFramework:LogMissingComponents () (at Assets/_scripts/TestingFramework.cs:101)
TestingFramework:Awake () (at Assets/_scripts/TestingFramework.cs:85)

  ShipGeneratorCoordinator: Found
UnityEngine.Debug:Log (object)
TestingFramework:LogMissingComponents () (at Assets/_scripts/TestingFramework.cs:102)
TestingFramework:Awake () (at Assets/_scripts/TestingFramework.cs:85)

  MasterShipGenerator: Found
UnityEngine.Debug:Log (object)
TestingFramework:LogMissingComponents () (at Assets/_scripts/TestingFramework.cs:103)
TestingFramework:Awake () (at Assets/_scripts/TestingFramework.cs:85)

  StarkkillerEncounterSystem: Not Found
UnityEngine.Debug:Log (object)
TestingFramework:LogMissingComponents () (at Assets/_scripts/TestingFramework.cs:104)
TestingFramework:Awake () (at Assets/_scripts/TestingFramework.cs:85)

  EncounterSystemManager: Found
UnityEngine.Debug:Log (object)
TestingFramework:LogMissingComponents () (at Assets/_scripts/TestingFramework.cs:105)
TestingFramework:Awake () (at Assets/_scripts/TestingFramework.cs:85)

ReportPanelTracker.Awake called on DailyReportPanel
UnityEngine.Debug:Log (object)
ReportPanelTracker:Awake () (at Assets/ReportPanelTracker.cs:20)

[TimeManager] Added time pause: DailyReportPanel_5a629543 from DailyReportPanel
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:PauseTime (UnityEngine.GameObject,string) (at Assets/TimeManager.cs:107)
TimeModifierBehavior:ApplyTimeModification () (at Assets/TimeModifierBehavior.cs:64)
TimeModifierBehavior:OnEnable () (at Assets/TimeModifierBehavior.cs:29)
UnityEngine.GameObject:AddComponent<TimeModifierBehavior> ()
ReportPanelTracker:Awake () (at Assets/ReportPanelTracker.cs:30)

[TimeManager] Time PAUSED: 2 active modifiers
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:UpdateTimeState () (at Assets/TimeManager.cs:216)
TimeManager:PauseTime (UnityEngine.GameObject,string) (at Assets/TimeManager.cs:111)
TimeModifierBehavior:ApplyTimeModification () (at Assets/TimeModifierBehavior.cs:64)
TimeModifierBehavior:OnEnable () (at Assets/TimeModifierBehavior.cs:29)
UnityEngine.GameObject:AddComponent<TimeModifierBehavior> ()
ReportPanelTracker:Awake () (at Assets/ReportPanelTracker.cs:30)

Added TimeModifierBehavior to DailyReportPanel
UnityEngine.Debug:Log (object)
ReportPanelTracker:Awake () (at Assets/ReportPanelTracker.cs:34)

DailyReportPanel.OnEnable called
UnityEngine.Debug:Log (object)
ReportPanelTracker:OnEnable () (at Assets/ReportPanelTracker.cs:194)

[ServiceLocator] Registered service: BriberyManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.BriberyManager> (Starkiller.Core.Managers.BriberyManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.BriberyManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/BriberyManager.cs:88)

[BriberyManager] Initialized and registered with ServiceLocator
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.BriberyManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/BriberyManager.cs:94)

[ServiceLocator] Registered service: ShiftTimerManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.ShiftTimerManager> (Starkiller.Core.Managers.ShiftTimerManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.ShiftTimerManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:112)

[ShiftTimerManager] Initialized and registered with ServiceLocator
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:118)

[ServiceLocator] Registered service: DayProgressionManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.DayProgressionManager> (Starkiller.Core.Managers.DayProgressionManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.DayProgressionManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:112)

[DayProgressionManager] Initialized and registered with ServiceLocator
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:115)

Moving GameManager to root level before applying DontDestroyOnLoad
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.GameObject) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:39)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.MonoBehaviour) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:16)
GameStateController:Awake () (at Assets/GameStateController.cs:56)

Successfully moved GameManager to root and applied DontDestroyOnLoad. Original position: (960.00, 540.00, 0.00), Original rotation: (0.00000, 0.00000, 0.00000, 1.00000)
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.GameObject) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:51)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.MonoBehaviour) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:16)
GameStateController:Awake () (at Assets/GameStateController.cs:56)

[GameStateController] GameStateController initialized
UnityEngine.Debug:Log (object)
GameStateController:LogStatus (string) (at Assets/GameStateController.cs:311)
GameStateController:Awake () (at Assets/GameStateController.cs:68)

Moving NarrativeManager to root level before applying DontDestroyOnLoad
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.GameObject) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:39)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.MonoBehaviour) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:16)
StarkillerBaseCommand.NarrativeManager:Awake () (at Assets/NarrativeManager.cs:42)

Successfully moved NarrativeManager to root and applied DontDestroyOnLoad. Original position: (1024.45, 475.10, -6.15), Original rotation: (0.00000, 0.00000, 0.00000, 1.00000)
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.GameObject) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:51)
StarkillerBaseCommand.DontDestroyOnLoadHelper:SafeDontDestroyOnLoad (UnityEngine.MonoBehaviour) (at Assets/Scripts/DontDestroyOnLoadHelper.cs:16)
StarkillerBaseCommand.NarrativeManager:Awake () (at Assets/NarrativeManager.cs:42)

[ServiceLocator] Registered service: MoralChoiceManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.MoralChoiceManager> (Starkiller.Core.Managers.MoralChoiceManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.MoralChoiceManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/MoralChoiceManager.cs:95)

[ServiceLocator] Registered service: GameStateManager
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.GameStateManager> (Starkiller.Core.Managers.GameStateManager) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.GameStateManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/GameStateManager.cs:28)

[GameStateManager] Initialized and registered with ServiceLocator
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.GameStateManager:Awake () (at Assets/_RefactoredScripts/Core/Managers/GameStateManager.cs:31)

[ServiceLocator] Registered service: UICoordinator
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Managers.UICoordinator> (Starkiller.Core.Managers.UICoordinator) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Managers.UICoordinator:Awake () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:89)

[UICoordinator] Initialized and registered with ServiceLocator
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:Awake () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:92)

[ServiceLocator] Registered service: ScenarioMediaHelper
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<Starkiller.Core.Helpers.ScenarioMediaHelper> (Starkiller.Core.Helpers.ScenarioMediaHelper) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.Helpers.ScenarioMediaHelper:Awake () (at Assets/_RefactoredScripts/Core/Helpers/ScenarioMediaHelper.cs:90)

[PersonalDataLogManager] Initialized and ready
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.PersonalDataLogManager:Start () (at Assets/_RefactoredScripts/Core/Managers/PersonalDataLogManager.cs:81)

[TimeManager] Removed time modifier: DailyBriefingPanel_49b0de00 from DailyBriefingPanel
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:ResumeTimeForSource (UnityEngine.GameObject) (at Assets/TimeManager.cs:177)
TimeModifierBehavior:ResumeTime () (at Assets/TimeModifierBehavior.cs:87)
TimeModifierBehavior:OnDisable () (at Assets/TimeModifierBehavior.cs:37)
UnityEngine.GameObject:SetActive (bool)
DailyBriefingManager:Start () (at Assets/DailyBriefingManager.cs:179)

[TimeManager] Time PAUSED: 1 active modifiers
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:UpdateTimeState () (at Assets/TimeManager.cs:216)
TimeManager:ResumeTimeForSource (UnityEngine.GameObject) (at Assets/TimeManager.cs:184)
TimeModifierBehavior:ResumeTime () (at Assets/TimeModifierBehavior.cs:87)
TimeModifierBehavior:OnDisable () (at Assets/TimeModifierBehavior.cs:37)
UnityEngine.GameObject:SetActive (bool)
DailyBriefingManager:Start () (at Assets/DailyBriefingManager.cs:179)

ShipScenarioProvider successfully registered with MasterShipGenerator
UnityEngine.Debug:Log (object)
MasterShipGenerator:RegisterScenarioProvider (ShipScenarioProvider) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:317)
System.Reflection.MethodBase:Invoke (object,object[])
ShipScenarioProvider:RegisterWithMasterShipGenerator () (at Assets/_scripts/ShipScenarioProvider.cs:102)
ShipScenarioProvider:Start () (at Assets/_scripts/ShipScenarioProvider.cs:77)

Successfully registered with MasterShipGenerator via RegisterScenarioProvider method
UnityEngine.Debug:Log (object)
ShipScenarioProvider:RegisterWithMasterShipGenerator () (at Assets/_scripts/ShipScenarioProvider.cs:103)
ShipScenarioProvider:Start () (at Assets/_scripts/ShipScenarioProvider.cs:77)

MissingComponentException: There is no 'RectTransform' attached to the "DraggablePanelManager" game object, but a script is trying to access it.
You probably need to add a RectTransform to the game object "DraggablePanelManager". Or your script needs to check if the component is attached before using it.
UnityEngine.Object+MarshalledUnityObject.TryThrowEditorNullExceptionObject (UnityEngine.Object unityObj, System.String parameterName) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/UnityEngineObject.bindings.cs:882)
UnityEngine.Bindings.ThrowHelper.ThrowNullReferenceException (System.Object obj) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/BindingsHelpers.cs:61)
UnityEngine.Transform.get_localPosition () (at <eac12af5e0034b02b1bfe348a7feb8c6>:0)
AnnoyingPopup+<ShakeAnimation>d__17.MoveNext () (at Assets/AnnoyingPopup.cs:96)
UnityEngine.SetupCoroutine.InvokeMoveNext (System.Collections.IEnumerator enumerator, System.IntPtr returnValueAddress) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)
UnityEngine.MonoBehaviour:StartCoroutine(IEnumerator)
AnnoyingPopup:Start() (at Assets/AnnoyingPopup.cs:46)

[InspectionManager] Disabled inspection video auto-play
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.InspectionManager:DisableInspectionVideoAutoPlay () (at Assets/_RefactoredScripts/Core/Managers/InspectionManager.cs:165)
Starkiller.Core.Managers.InspectionManager:Start () (at Assets/_RefactoredScripts/Core/Managers/InspectionManager.cs:148)

[InspectionManager] Inspection system ready
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.InspectionManager:Start () (at Assets/_RefactoredScripts/Core/Managers/InspectionManager.cs:151)

EncounterSystemManager starting delayed initialization...
UnityEngine.Debug:Log (object)
EncounterSystemManager/<DelayedInitialization>d__18:MoveNext () (at Assets/EncounterSystemManager.cs:139)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
EncounterSystemManager:Start () (at Assets/EncounterSystemManager.cs:64)

MasterShipGenerator: Loading resources...
UnityEngine.Debug:Log (object)
MasterShipGenerator:LoadAllResources () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:873)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:123)

MasterShipGenerator: Using ContentManager to load resources
UnityEngine.Debug:Log (object)
MasterShipGenerator:LoadResourcesFromContentManager () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:905)
MasterShipGenerator:LoadAllResources () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:878)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:123)

Loaded 50 ship types from ContentManager
UnityEngine.Debug:Log (object)
MasterShipGenerator:LoadResourcesFromContentManager () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:911)
MasterShipGenerator:LoadAllResources () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:878)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:123)

Loaded 8 captain types from ContentManager
UnityEngine.Debug:Log (object)
MasterShipGenerator:LoadResourcesFromContentManager () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:918)
MasterShipGenerator:LoadAllResources () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:878)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:123)

Loaded 5 scenarios from ContentManager
UnityEngine.Debug:Log (object)
MasterShipGenerator:LoadResourcesFromContentManager () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:925)
MasterShipGenerator:LoadAllResources () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:878)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:123)

MasterShipGenerator: Generating encounters for day 1, using 7 access codes
UnityEngine.Debug:Log (object)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:382)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Automated Systems is NOT compatible with ship category Imperium Vessel
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Bounty Hunter is NOT compatible with ship category Imperium Vessel
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Corporate Representative is NOT compatible with ship category Imperium Vessel
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Imperium Officer is compatible with ship category Imperium Vessel
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1809)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Military Transport Pilot is compatible with ship category Imperium Vessel
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1809)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Pirates is NOT compatible with ship category Imperium Vessel
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Insurgent is NOT compatible with ship category Imperium Vessel
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Smuggler Captain is NOT compatible with ship category Imperium Vessel
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

ManifestManager: Manifest system disabled or no manifests available, returning null
UnityEngine.Debug:Log (object)
ManifestManager:SelectManifestForShip (ShipType,string,int) (at Assets/ManifestManager.cs:152)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:589)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

=== CAPTAIN SELECTED (DETAILED) ===
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:617)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Type: Imperium Officer
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:618)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

✅ selectedCaptain Reference: SET
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:619)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - firstName: 'idos'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:622)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - lastName: 'flek'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:623)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - GetFullName(): 'idos flek'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:624)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - rank: ''
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:625)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

✅ encounter.selectedCaptain: SET
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:627)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainName: 'idos flek'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:628)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainRank: 'Commander'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:629)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainFaction: 'Imperium'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:630)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Bribery Chance: 20%
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:631)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

===================================
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:632)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

=== ENCOUNTER CREATED ===
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:662)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Troop Carrier "Troop Carrier - Security Hub" (Imperium Vessel)
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:663)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Commander idos flek
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:664)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Scenario: Medical Emergency - Should Approve: True
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:665)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Access Code: STD-5001
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:666)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

========================
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:670)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

ManifestManager: Manifest system disabled or no manifests available, returning null
UnityEngine.Debug:Log (object)
ManifestManager:SelectManifestForShip (ShipType,string,int) (at Assets/ManifestManager.cs:152)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:467)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

=== ENCOUNTER VIDEO SELECTION ===
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:762)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Troop Carrier - "Troop Carrier - Security Hub"
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:763)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Commander idos flek (Imperium)
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:764)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Story Ship: False 
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:765)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting ship video for type: Troop Carrier, name: Troop Carrier - Security Hub
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

No ship video found for Troop Carrier, using default
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:485)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Ship_Troop Carrier_Troop Carrier - Security Hub -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship Video Selected: UItest2
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:769)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting captain video for faction: Imperium, name: idos flek, rank: Commander, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found captain video from ScriptableObject: IMC_Greet002
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Captain_Imperium_greeting_idos flek -> IMC_Greet002
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Video Selected: IMC_Greet002
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:773)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

=================================
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:782)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Automated Systems is NOT compatible with ship category InsurgentShips
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Bounty Hunter is NOT compatible with ship category InsurgentShips
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Corporate Representative is NOT compatible with ship category InsurgentShips
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Imperium Officer is NOT compatible with ship category InsurgentShips
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Military Transport Pilot is NOT compatible with ship category InsurgentShips
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Pirates is NOT compatible with ship category InsurgentShips
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Insurgent is NOT compatible with ship category InsurgentShips
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Smuggler Captain is NOT compatible with ship category InsurgentShips
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Type: Pirates
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:618)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - firstName: 'Black'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:622)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - lastName: 'Jack'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:623)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - GetFullName(): 'Black Jack'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:624)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainName: 'Black Jack'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:628)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainRank: 'First Mate'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:629)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainFaction: 'Pirate'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:630)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Bribery Chance: 70%
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:631)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Concorde "Concorde - The Raven" (InsurgentShips)
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:663)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: First Mate Black Jack
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:664)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Scenario: Sabotaged Ship - Should Approve: False
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:665)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Access Code: M1L-1001
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:666)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Invalid Reason: Invalid access code - suspected forgery
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:668)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Concorde - "Concorde - The Raven"
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:763)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: First Mate Black Jack (Pirate)
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:764)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting ship video for type: Concorde, name: Concorde - The Raven
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

No ship video found for Concorde, using default
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:485)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Ship_Concorde_Concorde - The Raven -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting captain video for faction: Pirate, name: Black Jack, rank: First Mate, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found captain video from ScriptableObject: TempCap_PirateCapt
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Captain_Pirate_greeting_Black Jack -> TempCap_PirateCapt
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Video Selected: TempCap_PirateCapt
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:773)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - firstName: 'Flambo'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:622)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - lastName: 'Yant'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:623)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - GetFullName(): 'Flambo Yant'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:624)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainName: 'Flambo Yant'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:628)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Battleship "Battleship - Operations Base" (Imperium Vessel)
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:663)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Commander Flambo Yant
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:664)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Scenario: Standard Supply Run - Should Approve: True
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:665)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Access Code: VIP-9001
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:666)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Battleship - "Battleship - Operations Base"
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:763)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Commander Flambo Yant (Imperium)
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:764)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting ship video for type: Battleship, name: Battleship - Operations Base
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found ship video from ScriptableObject: shiptest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:439)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Ship_Battleship_Battleship - Operations Base -> shiptest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:444)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship Video Selected: shiptest2
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:769)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting captain video for faction: Imperium, name: Flambo Yant, rank: Commander, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found captain video from ScriptableObject: imperium03-greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Captain_Imperium_greeting_Flambo Yant -> imperium03-greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Video Selected: imperium03-greeting
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:773)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Type: Military Transport Pilot
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:618)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - firstName: 'Military'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:622)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - lastName: 'One'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:623)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - GetFullName(): 'Military One'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:624)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainName: 'Military One'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:628)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainRank: 'Major'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:629)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainFaction: 'Mercenary'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:630)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Bribery Chance: 0%
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:631)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Battleship "Battleship - Devastator" (Imperium Vessel)
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:663)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Major Military One
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:664)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Access Code: TRD-6001
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:666)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Battleship - "Battleship - Devastator"
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:763)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Major Military One (Mercenary)
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:764)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting ship video for type: Battleship, name: Battleship - Devastator
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found ship video from ScriptableObject: shipttest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:439)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Ship_Battleship_Battleship - Devastator -> shipttest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:444)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship Video Selected: shipttest2
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:769)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting captain video for faction: Mercenary, name: Military One, rank: Major, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found captain video from ScriptableObject: TempCap_MilitaryTransport
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Captain_Mercenary_greeting_Military One -> TempCap_MilitaryTransport
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Video Selected: TempCap_MilitaryTransport
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:773)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - firstName: 'Mortis'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:622)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - lastName: 'Op'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:623)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - GetFullName(): 'Mortis Op'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:624)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainName: 'Mortis Op'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:628)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Corvette "Corvette - Tactical Unit" (Imperium Vessel)
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:663)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Commander Mortis Op
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:664)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Access Code: SPL-7777
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:666)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Corvette - "Corvette - Tactical Unit"
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:763)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Commander Mortis Op (Imperium)
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:764)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting ship video for type: Corvette, name: Corvette - Tactical Unit
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

No ship video found for Corvette, using default
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:485)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Ship_Corvette_Corvette - Tactical Unit -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting captain video for faction: Imperium, name: Mortis Op, rank: Commander, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found captain video from ScriptableObject: imperium01-greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Captain_Imperium_greeting_Mortis Op -> imperium01-greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Video Selected: imperium01-greeting
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:773)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Battleship "Battleship - Demeter" (Imperium Vessel)
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:663)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Battleship - "Battleship - Demeter"
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:763)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting ship video for type: Battleship, name: Battleship - Demeter
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found ship video from ScriptableObject: ShipTest4
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:439)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Ship_Battleship_Battleship - Demeter -> ShipTest4
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:444)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship Video Selected: ShipTest4
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:769)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Using cached captain video for Mercenary (greeting): TempCap_MilitaryTransport
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:784)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainRank: 'Lieutenant'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:629)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Super Stellar Destroyer "Super Stellar Destroyer - Control System" (Imperium Vessel)
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:663)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Lieutenant Military One
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:664)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Scenario: Suspicious Cargo - Should Approve: False
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:665)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Access Code: V1P-9001
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:666)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Super Stellar Destroyer - "Super Stellar Destroyer - Control System"
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:763)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Lieutenant Military One (Imperium)
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:764)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting ship video for type: Super Stellar Destroyer, name: Super Stellar Destroyer - Control System
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found ship video from legacy data: ShipTest4
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:455)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Ship_Super Stellar Destroyer_Super Stellar Destroyer - Control System -> ShipTest4
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:460)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting captain video for faction: Imperium, name: Military One, rank: Lieutenant, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Captain_Imperium_greeting_Military One -> TempCap_MilitaryTransport
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainRank: 'Colonel'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:629)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Colonel Mortis Op
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:664)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Colonel Mortis Op (Imperium)
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:764)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Using cached ship video for Battleship: shipttest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:425)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Using cached captain video for Imperium (greeting): imperium01-greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:784)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Automated Systems is NOT compatible with ship category Trade Union
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Bounty Hunter is NOT compatible with ship category Trade Union
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Corporate Representative is NOT compatible with ship category Trade Union
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Imperium Officer is NOT compatible with ship category Trade Union
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Military Transport Pilot is NOT compatible with ship category Trade Union
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Pirates is NOT compatible with ship category Trade Union
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Insurgent is NOT compatible with ship category Trade Union
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Smuggler Captain is NOT compatible with ship category Trade Union
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Type: Automated Systems
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:618)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - firstName: 'Control'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:622)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - lastName: 'System'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:623)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - GetFullName(): 'Control System'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:624)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainName: 'Control System'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:628)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainRank: 'Artificial Narrow Intelligence (ANI)'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:629)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainFaction: 'Automated Systems'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:630)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Bribery Chance: 10%
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:631)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Star Courier "Star Courier - Ebay" (Trade Union)
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:663)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Artificial Narrow Intelligence (ANI) Control System
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:664)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Access Code: EMG-0999
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:666)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Star Courier - "Star Courier - Ebay"
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:763)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Artificial Narrow Intelligence (ANI) Control System (Automated Systems)
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:764)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting ship video for type: Star Courier, name: Star Courier - Ebay
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found ship video from ScriptableObject: PlaceholderTradeUnionWide
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:439)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Ship_Star Courier_Star Courier - Ebay -> PlaceholderTradeUnionWide
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:444)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship Video Selected: PlaceholderTradeUnionWide
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:769)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting captain video for faction: Automated Systems, name: Control System, rank: Artificial Narrow Intelligence (ANI), context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found captain video from ScriptableObject: auto01_greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Captain_Automated Systems_greeting_Control System -> auto01_greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Video Selected: auto01_greeting
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:773)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Automated Systems is NOT compatible with ship category Special Interest Vessels
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Bounty Hunter is NOT compatible with ship category Special Interest Vessels
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Corporate Representative is NOT compatible with ship category Special Interest Vessels
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Imperium Officer is NOT compatible with ship category Special Interest Vessels
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Military Transport Pilot is NOT compatible with ship category Special Interest Vessels
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Pirates is NOT compatible with ship category Special Interest Vessels
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Insurgent is NOT compatible with ship category Special Interest Vessels
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Smuggler Captain is NOT compatible with ship category Special Interest Vessels
UnityEngine.Debug:Log (object)
MasterShipGenerator:SelectCaptainType (ShipScenario,ShipType) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1813)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:458)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Type: Insurgent
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:618)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - firstName: 'Fran'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:622)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - lastName: 'Duo'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:623)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

   - GetFullName(): 'Fran Duo'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:624)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainName: 'Fran Duo'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:628)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated encounter.captainFaction: 'Courier Service'
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:630)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Meteor Miner "Meteor Miner - Alert System" (Special Interest Vessels)
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:663)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Commander Fran Duo
UnityEngine.Debug:Log (object)
MasterShipEncounter:CreateFromScriptableObjects (ShipType,CaptainType,ShipScenario,bool,System.Collections.Generic.List`1<string>,StarkillerBaseCommand.StarkkillerContentManager) (at Assets/MasterShipEncounter.cs:664)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:461)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Ship: Meteor Miner - "Meteor Miner - Alert System"
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:763)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain: Commander Fran Duo (Courier Service)
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:764)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting ship video for type: Meteor Miner, name: Meteor Miner - Alert System
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

No ship video found for Meteor Miner, using default
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:485)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Ship_Meteor Miner_Meteor Miner - Alert System -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:768)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Getting captain video for faction: Courier Service, name: Fran Duo, rank: Commander, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Found captain video from ScriptableObject: testCaptain4
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Cached video: Captain_Courier Service_greeting_Fran Duo -> testCaptain4
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:772)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Captain Video Selected: testCaptain4
UnityEngine.Debug:Log (object)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:773)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:491)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:413)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

Generated 10 encounters for day 1 (0 story encounters)
UnityEngine.Debug:Log (object)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:436)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:132)

[ShipTimingController] Re-acquired MasterShipGenerator reference during cooldown reset: True
UnityEngine.Debug:Log (object)
ShipTimingController:LogMessage (string) (at Assets/ShipTimingController.cs:723)
ShipTimingController:ResetCooldown () (at Assets/ShipTimingController.cs:490)
MasterShipGenerator:NotifyTimingController () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:293)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:138)

[ShipTimingController] Re-acquired GameManager reference during cooldown reset: True
UnityEngine.Debug:Log (object)
ShipTimingController:LogMessage (string) (at Assets/ShipTimingController.cs:723)
ShipTimingController:ResetCooldown () (at Assets/ShipTimingController.cs:497)
MasterShipGenerator:NotifyTimingController () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:293)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:138)

[ShipTimingController] Ship cooldown and generation lock forcibly reset
UnityEngine.Debug:Log (object)
ShipTimingController:LogMessage (string) (at Assets/ShipTimingController.cs:723)
ShipTimingController:ResetCooldown () (at Assets/ShipTimingController.cs:518)
MasterShipGenerator:NotifyTimingController () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:293)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:138)

Notified ShipTimingController of our presence via ResetCooldown
UnityEngine.Debug:Log (object)
MasterShipGenerator:NotifyTimingController () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:294)
MasterShipGenerator:Start () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:138)

ManifestManager: Built faction pools for 0 factions
UnityEngine.Debug:Log (object)
ManifestManager:BuildFactionPools () (at Assets/ManifestManager.cs:142)
ManifestManager:LoadManifestsFromResources () (at Assets/ManifestManager.cs:95)
ManifestManager:Start () (at Assets/ManifestManager.cs:64)

ManifestSystemFix: Runtime manifest generation is enabled in ManifestManager
UnityEngine.Debug:Log (object)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:92)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

=== Testing Runtime Manifest Generation ===
UnityEngine.Debug:Log (object)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:105)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

Fallback manifest for Imperium: Personnel transport, Standard supplies, Communication equipment, Equipment parts for Imperial operations
UnityEngine.Debug:Log (object)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:113)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

ManifestManager: Manifest system disabled or no manifests available, returning null
UnityEngine.Debug:Log (object)
ManifestManager:SelectManifestForShip (ShipType,string,int) (at Assets/ManifestManager.cs:152)
ManifestManager:SelectManifestForShipWithFallback (ShipType,string,int) (at Assets/ManifestManager.cs:195)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:116)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

ManifestManager: No ScriptableObject manifest found for Imperium, using procedural generation
UnityEngine.Debug:Log (object)
ManifestManager:SelectManifestForShipWithFallback (ShipType,string,int) (at Assets/ManifestManager.cs:199)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:116)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

Selected manifest for Imperium: Imperium Cargo Manifest (Runtime: False)
UnityEngine.Debug:Log (object)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:119)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

Fallback manifest for Insurgent: Communication equipment, Equipment parts, Smuggled goods for resistance activities
UnityEngine.Debug:Log (object)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:113)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

ManifestManager: No ScriptableObject manifest found for Insurgent, using procedural generation
UnityEngine.Debug:Log (object)
ManifestManager:SelectManifestForShipWithFallback (ShipType,string,int) (at Assets/ManifestManager.cs:199)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:116)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

Selected manifest for Insurgent: Insurgent Cargo Manifest (Runtime: False)
UnityEngine.Debug:Log (object)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:119)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

Fallback manifest for Neutral: Power cells, Equipment parts for authorized operations
UnityEngine.Debug:Log (object)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:113)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

ManifestManager: No ScriptableObject manifest found for Neutral, using procedural generation
UnityEngine.Debug:Log (object)
ManifestManager:SelectManifestForShipWithFallback (ShipType,string,int) (at Assets/ManifestManager.cs:199)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:116)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

Selected manifest for Neutral: Neutral Cargo Manifest (Runtime: False)
UnityEngine.Debug:Log (object)
ManifestSystemFix:TestRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:119)
ManifestSystemFix:EnableRuntimeManifestGeneration () (at Assets/Scripts/ManifestSystemFix.cs:95)
ManifestSystemFix:Start () (at Assets/Scripts/ManifestSystemFix.cs:39)

VideoSystemHelper: Suppressing ShipImageSystem warning
UnityEngine.Debug:Log (object)
VideoSystemHelper:Start () (at Assets/_scripts/VideoSystemHelper.cs:40)

VideoSystemHelper: Added VideoPlayer to captainVideoContainer
UnityEngine.Debug:Log (object)
VideoSystemHelper:SetupVideoPlayers () (at Assets/_scripts/VideoSystemHelper.cs:72)
VideoSystemHelper:Start () (at Assets/_scripts/VideoSystemHelper.cs:45)

VideoSystemHelper: Connected video players to ShipVideoSystem
UnityEngine.Debug:Log (object)
VideoSystemHelper:SetupVideoPlayers () (at Assets/_scripts/VideoSystemHelper.cs:101)
VideoSystemHelper:Start () (at Assets/_scripts/VideoSystemHelper.cs:45)

DailyReportManager.Start() called - Panel active: True
UnityEngine.Debug:Log (object)
DailyReportManager:Start () (at Assets/_scripts/DailyReportManager.cs:58)

DailyReportManager.Start(): Panel is already active showing a report, not hiding it
UnityEngine.Debug:Log (object)
DailyReportManager:Start () (at Assets/_scripts/DailyReportManager.cs:83)

NewsTicker initialized with text: LATEST NEWS: SECURIT...
UnityEngine.Debug:Log (object)
NewsTicker:Start () (at Assets/NewsTicker.cs:45)

[EncounterManager] Next encounter scheduled in 12.0s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterManager:ScheduleNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:379)
Starkiller.Core.Managers.EncounterManager:Start () (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:105)

[EncounterManager] Encounter system ready
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterManager:Start () (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:108)

Initializing video players...
UnityEngine.Debug:Log (object)
CredentialChecker:InitializeVideoPlayers () (at Assets/_scripts/CredentialChecker.cs:596)
CredentialChecker:Start () (at Assets/_scripts/CredentialChecker.cs:160)

Ship video player initialized and enabled
UnityEngine.Debug:Log (object)
CredentialChecker:InitializeVideoPlayers () (at Assets/_scripts/CredentialChecker.cs:618)
CredentialChecker:Start () (at Assets/_scripts/CredentialChecker.cs:160)

Captain video player initialized and enabled
UnityEngine.Debug:Log (object)
CredentialChecker:InitializeVideoPlayers () (at Assets/_scripts/CredentialChecker.cs:642)
CredentialChecker:Start () (at Assets/_scripts/CredentialChecker.cs:160)

CredentialChecker: Subscribed to video synchronization events
UnityEngine.Debug:Log (object)
CredentialChecker:SubscribeToVideoSyncEvents () (at Assets/_scripts/CredentialChecker.cs:384)
CredentialChecker:Start () (at Assets/_scripts/CredentialChecker.cs:185)

CredentialChecker: Successfully subscribed to OnEncounterReady event
UnityEngine.Debug:Log (object)
CredentialChecker:Start () (at Assets/_scripts/CredentialChecker.cs:222)

[PerformanceManager] Daily metrics reset
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.PerformanceManager:ResetDailyMetrics () (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:417)
Starkiller.Core.Managers.PerformanceManager:Start () (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:139)

[PerformanceManager] Performance tracking system ready
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.PerformanceManager:Start () (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:142)

Loading consequences from Resources...
UnityEngine.Debug:Log (object)
ConsequenceManager:LoadConsequencesIfNeeded () (at Assets/ConsequenceManager.cs:124)
ConsequenceManager:Start () (at Assets/ConsequenceManager.cs:93)

Loaded 18 consequences from _ScriptableObjects/Consequences
UnityEngine.Debug:Log (object)
ConsequenceManager:LoadConsequencesIfNeeded () (at Assets/ConsequenceManager.cs:167)
ConsequenceManager:Start () (at Assets/ConsequenceManager.cs:93)

Consequence counts: Minor=4, Moderate=7, Severe=4, Critical=3
UnityEngine.Debug:Log (object)
ConsequenceManager:LoadConsequencesIfNeeded () (at Assets/ConsequenceManager.cs:176)
ConsequenceManager:Start () (at Assets/ConsequenceManager.cs:93)

ConsequenceManager initialized successfully
UnityEngine.Debug:Log (object)
ConsequenceManager:Start () (at Assets/ConsequenceManager.cs:110)

Video references connected!
UnityEngine.Debug:Log (object)
VideoSetupHelper:Start () (at Assets/_scripts/VideoSetupHelper.cs:39)

Generating news for day 1
UnityEngine.Debug:Log (object)
NewsManager:GenerateNewsForDay (int) (at Assets/NewsManager.cs:217)
NewsManager:Start () (at Assets/NewsManager.cs:96)

Updated news ticker with:     Today marks Day 1 of flawless Imperium operati...
UnityEngine.Debug:Log (object)
NewsManager:UpdateNewsTicker () (at Assets/NewsManager.cs:431)
NewsManager:GenerateNewsForDay (int) (at Assets/NewsManager.cs:247)
NewsManager:Start () (at Assets/NewsManager.cs:96)

NewsManager initialized
UnityEngine.Debug:Log (object)
NewsManager:Start () (at Assets/NewsManager.cs:101)

TestingFramework initialized successfully
UnityEngine.Debug:Log (object)
TestingFramework:Start () (at Assets/_scripts/TestingFramework.cs:134)

[BriberyManager] Bribery system ready
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.BriberyManager:Start () (at Assets/_RefactoredScripts/Core/Managers/BriberyManager.cs:114)

[ShiftTimerManager] Timer system ready
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:Start () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:148)

[CaptainIDCard] Stored original scale: (0.50, 0.50, 1.00)
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:Start () (at Assets/_scripts/CaptainIDCard.cs:105)

[CaptainIDCard] Panel was active in Inspector, setting up initial visual state
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:Start () (at Assets/_scripts/CaptainIDCard.cs:117)

[DayProgressionManager] *** INITIALIZATION *** Starting at day 1 (startingDay = 1)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:Start () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:123)

[DayProgressionManager] Started on day 1
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:Start () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:130)

[GameManager] Refactored managers initialized: CreditsManager: ✓, DecisionTracker: ✓, DayProgressionManager: ✓
UnityEngine.Debug:Log (object)
GameManager:InitializeRefactoredManagers () (at Assets/_scripts/GameManager.cs:430)
GameManager:Start () (at Assets/_scripts/GameManager.cs:175)

DailyReportPanel.OnDisable called
UnityEngine.Debug:Log (object)
ReportPanelTracker:OnDisable () (at Assets/ReportPanelTracker.cs:206)
UnityEngine.GameObject:SetActive (bool)
GameManager:Start () (at Assets/_scripts/GameManager.cs:190)

[TimeManager] Removed time modifier: DailyReportPanel_5a629543 from DailyReportPanel
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:ResumeTimeForSource (UnityEngine.GameObject) (at Assets/TimeManager.cs:177)
TimeModifierBehavior:ResumeTime () (at Assets/TimeModifierBehavior.cs:87)
TimeModifierBehavior:OnDisable () (at Assets/TimeModifierBehavior.cs:37)
UnityEngine.GameObject:SetActive (bool)
GameManager:Start () (at Assets/_scripts/GameManager.cs:190)

[TimeManager] Time RESUMED: Normal speed
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:UpdateTimeState () (at Assets/TimeManager.cs:230)
TimeManager:ResumeTimeForSource (UnityEngine.GameObject) (at Assets/TimeManager.cs:184)
TimeModifierBehavior:ResumeTime () (at Assets/TimeModifierBehavior.cs:87)
TimeModifierBehavior:OnDisable () (at Assets/TimeModifierBehavior.cs:37)
UnityEngine.GameObject:SetActive (bool)
GameManager:Start () (at Assets/_scripts/GameManager.cs:190)

[DayProgressionManager] Reset for new game - Starting at day 1
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:ResetForNewGame () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:590)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:442)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[GameManager] Starting shift for day 1
UnityEngine.Debug:Log (object)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:480)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[DayProgressionManager] Shift started - ShiftTimerManager will handle timer
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:StartShift () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:259)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:481)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[EncounterManager] Shift started - encounters enabled
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterManager:OnShiftStarted () (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:457)
Starkiller.Core.Managers.DayProgressionManager:StartShift () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:275)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:481)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[ShiftTimerManager] StartTimer called with timeLimit: 0, current shiftTimeLimit: 120, useDifficultyProfile: False
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:StartTimer (single) (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:200)
Starkiller.Core.Managers.ShiftTimerManager:OnShiftStarted () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:662)
Starkiller.Core.Managers.DayProgressionManager:StartShift () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:275)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:481)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[ShiftTimerManager] Using default shiftTimeLimit: 120s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:StartTimer (single) (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:218)
Starkiller.Core.Managers.ShiftTimerManager:OnShiftStarted () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:662)
Starkiller.Core.Managers.DayProgressionManager:StartShift () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:275)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:481)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[ShiftTimerManager] Timer started: 120.0s (Base: 120s, Bonus: 0s)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:StartTimer (single) (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:240)
Starkiller.Core.Managers.ShiftTimerManager:OnShiftStarted () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:662)
Starkiller.Core.Managers.DayProgressionManager:StartShift () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:275)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:481)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[ShiftTimerManager] Shift started - timer activated
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:OnShiftStarted () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:665)
Starkiller.Core.Managers.DayProgressionManager:StartShift () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:275)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:481)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[GameManager] Timer already active via events
UnityEngine.Debug:Log (object)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:497)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[GameManager] StartDay() - Force refreshing UI for day 1
UnityEngine.Debug:Log (object)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:518)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[GameManager] UpdateUI - Day: 1, Ships: 0/8
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:807)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:519)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[GameManager] UpdateUI - Synced strikes from DecisionTracker: 0
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:815)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:519)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[GameManager] UpdateUI: Day 1, Ships 0/8, Credits 30, Strikes 0/10
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:822)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:519)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[GameManager] UpdateUI - Day: 1, Ships: 0/8
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:807)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:531)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[GameManager] UpdateUI - Synced strikes from DecisionTracker: 0
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:815)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:531)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[GameManager] UpdateUI: Day 1, Ships 0/8, Credits 30, Strikes 0/10
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:822)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:531)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:459)
GameManager:Start () (at Assets/_scripts/GameManager.cs:226)

[GameStateController] Starting in MainMenu state (normal flow)
UnityEngine.Debug:Log (object)
GameStateController:LogStatus (string) (at Assets/GameStateController.cs:311)
GameStateController:Start () (at Assets/GameStateController.cs:96)

[MoralChoiceManager] moralChoicePanel is not assigned!
UnityEngine.Debug:LogError (object)
Starkiller.Core.Managers.MoralChoiceManager:ValidateUIReferences () (at Assets/_RefactoredScripts/Core/Managers/MoralChoiceManager.cs:770)
Starkiller.Core.Managers.MoralChoiceManager:Start () (at Assets/_RefactoredScripts/Core/Managers/MoralChoiceManager.cs:128)

[UICoordinator] Manager connections:
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:Start () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:106)

  - Credits Manager: Connected
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:Start () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:107)

  - Decision Tracker: Connected
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:Start () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:108)

  - Day Manager: Connected
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:Start () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:109)

  - Game State Manager: Connected
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:Start () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:110)

[UICoordinator] UpdateDayDisplay called - dayText: Available, dayManager: Available
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateDayDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:252)
Starkiller.Core.Managers.UICoordinator:RefreshAllUI () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:219)
Starkiller.Core.Managers.UICoordinator:Start () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:117)

[UICoordinator] Day text updated: 'Day Text' -> 'Day 1' (Manager Current Day: 1)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateDayDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:262)
Starkiller.Core.Managers.UICoordinator:RefreshAllUI () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:219)
Starkiller.Core.Managers.UICoordinator:Start () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:117)

[UICoordinator] UpdateShipsDisplay - Day 1: 0/8 (Until quota: 8)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateShipsDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:365)
Starkiller.Core.Managers.UICoordinator:RefreshAllUI () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:222)
Starkiller.Core.Managers.UICoordinator:Start () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:117)

[UICoordinator] Full UI refresh completed
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:RefreshAllUI () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:232)
Starkiller.Core.Managers.UICoordinator:Start () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:117)

[UICoordinator] UI system ready
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:Start () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:120)

[CaptainIDCard] Stored original scale: (1.00, 1.00, 1.00)
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:Start () (at Assets/_scripts/CaptainIDCard.cs:105)

========== [DayProgressionTest] Starting Complete Day Cycle Test ==========
UnityEngine.Debug:Log (object)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:127)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[DayProgressionTest] Starting test on Day 1
UnityEngine.Debug:Log (object)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:138)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[DayProgressionManager] *** DAILY RESET *** Ships today: 0 → 0, Total ships preserved: 0
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:ResetDailyTracking () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:458)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:182)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[DayProgressionManager] Daily tracking reset - Starting fresh day with 0 ships (Total processed: 0)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:ResetDailyTracking () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:461)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:182)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[DayProgressionManager] *** DAY INCREMENT *** Previous: 1, New: 2
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:184)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[DayProgressionManager] Starting day 2
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:187)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[UICoordinator] OnDayChanged event received - New Day: 2
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:OnDayChanged (int) (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:661)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:190)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[EncounterManager] Difficulty updated to 1.10
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterManager:UpdateDifficulty () (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:409)
Starkiller.Core.Managers.EncounterManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:448)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[EncounterManager] Day 2 started - encounter tracking reset
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:451)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[PerformanceManager] Daily metrics reset
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.PerformanceManager:ResetDailyMetrics () (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:417)
Starkiller.Core.Managers.PerformanceManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:490)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[PerformanceManager] Day 2 started - performance tracking reset
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.PerformanceManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:493)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[ShiftTimerManager] Statistics reset
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:ResetStatistics () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:548)
Starkiller.Core.Managers.ShiftTimerManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:684)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[ShiftTimerManager] Day 2 started - statistics reset, waiting for shift to start
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:687)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:141)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[DayProgressionTest] Day incremented to 2
UnityEngine.Debug:Log (object)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:143)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[DayProgressionTest] Shift started for Day 2, IsActive: True
UnityEngine.Debug:Log (object)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:147)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

MasterShipGenerator: Encounter on cooldown for 2.0 more seconds
UnityEngine.Debug:Log (object)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:576)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:153)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[DayProgressionTest] Ship generation failed!
UnityEngine.Debug:LogError (object)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:160)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[DayProgressionTest] Timer Active: True, Remaining: 120.0s
UnityEngine.Debug:Log (object)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:168)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

========== [DayProgressionTest] Complete Day Cycle Test Finished ==========
UnityEngine.Debug:Log (object)
DayProgressionTest:TestCompleteDayCycle () (at Assets/_scripts/DayProgressionTest.cs:171)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:25)

[DayProgressionTest] Before increment: Day 2
UnityEngine.Debug:Log (object)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:54)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[DayProgressionManager] *** DAILY RESET *** Ships today: 0 → 0, Total ships preserved: 0
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:ResetDailyTracking () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:458)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:182)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[DayProgressionManager] Daily tracking reset - Starting fresh day with 0 ships (Total processed: 0)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:ResetDailyTracking () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:461)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:182)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[DayProgressionManager] *** DAY INCREMENT *** Previous: 2, New: 3
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:184)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[DayProgressionManager] Starting day 3
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:187)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[UICoordinator] OnDayChanged event received - New Day: 3
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:OnDayChanged (int) (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:661)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:190)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[EncounterManager] Difficulty updated to 1.20
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterManager:UpdateDifficulty () (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:409)
Starkiller.Core.Managers.EncounterManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:448)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[EncounterManager] Day 3 started - encounter tracking reset
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/EncounterManager.cs:451)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[PerformanceManager] Daily metrics reset
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.PerformanceManager:ResetDailyMetrics () (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:417)
Starkiller.Core.Managers.PerformanceManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:490)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[PerformanceManager] Day 3 started - performance tracking reset
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.PerformanceManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/PerformanceManager.cs:493)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[ShiftTimerManager] Statistics reset
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:ResetStatistics () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:548)
Starkiller.Core.Managers.ShiftTimerManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:684)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[ShiftTimerManager] Day 3 started - statistics reset, waiting for shift to start
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:OnDayStarted (int) (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:687)
Starkiller.Core.Managers.DayProgressionManager:StartNewDay () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:191)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:56)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[DayProgressionTest] After increment: Day 3
UnityEngine.Debug:Log (object)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:59)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

✅ [DayProgressionTest] Day increment working correctly!
UnityEngine.Debug:Log (object)
DayProgressionTest:TestDirectDayIncrement () (at Assets/_scripts/DayProgressionTest.cs:63)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:31)

[DayProgressionTest] Requesting encounter from MasterShipGenerator
UnityEngine.Debug:Log (object)
DayProgressionTest:TestShipGeneration () (at Assets/_scripts/DayProgressionTest.cs:106)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:37)

MasterShipGenerator: Encounter on cooldown for 2.0 more seconds
UnityEngine.Debug:Log (object)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:576)
DayProgressionTest:TestShipGeneration () (at Assets/_scripts/DayProgressionTest.cs:108)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:37)

❌ [DayProgressionTest] No encounter available from MasterShipGenerator!
UnityEngine.Debug:LogError (object)
DayProgressionTest:TestShipGeneration () (at Assets/_scripts/DayProgressionTest.cs:115)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:37)

[DayProgressionTest] Force displaying first encounter
UnityEngine.Debug:Log (object)
DayProgressionTest:TestForceDisplayFirstEncounter () (at Assets/_scripts/DayProgressionTest.cs:177)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:43)

MasterShipGenerator: Encounter on cooldown for 2.0 more seconds
UnityEngine.Debug:Log (object)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:576)
DayProgressionTest:TestForceDisplayFirstEncounter () (at Assets/_scripts/DayProgressionTest.cs:184)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:43)

[DayProgressionTest] No encounter to display!
UnityEngine.Debug:LogError (object)
DayProgressionTest:TestForceDisplayFirstEncounter () (at Assets/_scripts/DayProgressionTest.cs:192)
DayProgressionTest:Update () (at Assets/_scripts/DayProgressionTest.cs:43)

GameReset: Fixing core components...
UnityEngine.Debug:Log (object)
GameReset:FixCoreComponents () (at Assets/Scripts/GameReset.cs:59)
GameReset/<ResetGame>d__2:MoveNext () (at Assets/Scripts/GameReset.cs:43)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

GameReset: Fixing game state...
UnityEngine.Debug:Log (object)
GameReset:FixGameState () (at Assets/Scripts/GameReset.cs:116)
GameReset/<ResetGame>d__2:MoveNext () (at Assets/Scripts/GameReset.cs:46)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

GameReset: Fixing timing...
UnityEngine.Debug:Log (object)
GameReset:FixTiming () (at Assets/Scripts/GameReset.cs:135)
GameReset/<ResetGame>d__2:MoveNext () (at Assets/Scripts/GameReset.cs:49)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

=== GameReset: Reset complete ===
UnityEngine.Debug:Log (object)
GameReset/<ResetGame>d__2:MoveNext () (at Assets/Scripts/GameReset.cs:51)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] Refreshing UI after day change - Expected: 2, Manager reports: 3
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:686)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] UpdateDayDisplay called - dayText: Available, dayManager: Available
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateDayDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:252)
Starkiller.Core.Managers.UICoordinator:RefreshAllUI () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:219)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:689)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] Day text updated: 'Day 1' -> 'Day 3' (Manager Current Day: 3)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateDayDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:262)
Starkiller.Core.Managers.UICoordinator:RefreshAllUI () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:219)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:689)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] UpdateShipsDisplay - Day 3: 0/8 (Until quota: 8)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateShipsDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:365)
Starkiller.Core.Managers.UICoordinator:RefreshAllUI () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:222)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:689)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] Full UI refresh completed
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:RefreshAllUI () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:232)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:689)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] Day display updated to: Day 3
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:693)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] Current day from manager: 3
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:696)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] Ships display reset - Processed: 0, Quota: 8
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:697)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] Ships text updated to: Ships: 0/8
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:698)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] Refreshing UI after day change - Expected: 3, Manager reports: 3
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:686)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] Day text updated: 'Day 3' -> 'Day 3' (Manager Current Day: 3)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateDayDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:262)
Starkiller.Core.Managers.UICoordinator:RefreshAllUI () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:219)
Starkiller.Core.Managers.UICoordinator/<RefreshUIAfterDayChange>d__71:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:689)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Requesting initial encounter
UnityEngine.Debug:Log (object)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:363)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

MasterShipGenerator: Encounter on cooldown for 1.5 more seconds
UnityEngine.Debug:Log (object)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:576)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: No reaction video playing - updating text immediately for Test Imperial Shuttle
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:517)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Updated ship info text for Test Imperial Shuttle
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:468)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Updated credentials text for Test Imperial Shuttle
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:479)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Access Code: STD-5001
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:480)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Manifest: Supply crates, replacement parts, and crew rotations.
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:481)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Manifest Source: Legacy String
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:482)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: SafeEnableButtons - canEnable: True (processing: False, animation: False, video: False)
UnityEngine.Debug:Log (object)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2634)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1129)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Setting decision buttons to ENABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2638)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1129)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: New encounter displayed - enabling decision buttons
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1132)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] 🔍 CAPTAIN SEARCH DEBUG:
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:862)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager]   Target Name: 'Test'
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:863)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager]   Target Full: 'Commander Test'
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:864)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager]   Available Captains in Imperium Officer:
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:865)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager]     [0] Mortis Op (first: 'Mortis', last: 'Op')
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:869)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager]     [1] idos flek (first: 'idos', last: 'flek')
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:869)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager]     [2] Baldy McBaldy (first: 'Baldy', last: 'McBaldy')
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:869)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager]     [3] Flambo Yant (first: 'Flambo', last: 'Yant')
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:869)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] ❌ NO MATCH FOUND for 'Test'!
UnityEngine.Debug:LogError (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:972)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] This should not happen if encounter.selectedCaptain was set correctly
UnityEngine.Debug:LogError (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:973)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Using FALLBACK: Mortis Op
UnityEngine.Debug:LogError (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:974)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Please check MasterShipEncounter.CreateFromScriptableObjects captain selection logic
UnityEngine.Debug:LogError (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:975)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] 🚀 ENCOUNTER PREPARED (MasterShipEncounter): Test
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:154)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] 👨‍✈️ Captain found: Mortis Op from type: Imperium Officer
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:155)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Playing greeting videos for Test Imperial Shuttle - Test
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:173)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Starting greeting sequence for Test Imperial Shuttle
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayGreetingSequence>d__54:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:184)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] No captain video for Test
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayGreetingSequence>d__54:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:193)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] No ship video for Test Imperial Shuttle
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayGreetingSequence>d__54:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:203)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Completed greeting sequence for Test Imperial Shuttle
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayGreetingSequence>d__54:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:207)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Notified new EncounterMediaTransitionManager of encounter
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1158)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Successfully displayed encounter for ship: Test Imperial Shuttle
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1161)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2440)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: SafeEnableButtons - canEnable: True (processing: False, animation: False, video: False)
UnityEngine.Debug:Log (object)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2634)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2460)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Setting decision buttons to ENABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2638)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2460)
CredentialChecker/<RequestInitialEncounterAfterDelay>d__56:MoveNext () (at Assets/_scripts/CredentialChecker.cs:364)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

Initial encounter request deferred until game enters active gameplay state
UnityEngine.Debug:Log (object)
EncounterSystemManager/<DelayedInitialization>d__18:MoveNext () (at Assets/EncounterSystemManager.cs:183)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

EncounterSystemManager initialization complete
UnityEngine.Debug:Log (object)
EncounterSystemManager/<DelayedInitialization>d__18:MoveNext () (at Assets/EncounterSystemManager.cs:187)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[TimeManager] Added time pause: DailyBriefingPanel_2e6668a0 from DailyBriefingPanel
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:PauseTime (UnityEngine.GameObject,string) (at Assets/TimeManager.cs:107)
TimeModifierBehavior:ApplyTimeModification () (at Assets/TimeModifierBehavior.cs:64)
TimeModifierBehavior:OnEnable () (at Assets/TimeModifierBehavior.cs:29)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[TimeManager] Time PAUSED: 1 active modifiers
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:UpdateTimeState () (at Assets/TimeManager.cs:216)
TimeManager:PauseTime (UnityEngine.GameObject,string) (at Assets/TimeManager.cs:111)
TimeModifierBehavior:ApplyTimeModification () (at Assets/TimeModifierBehavior.cs:64)
TimeModifierBehavior:OnEnable () (at Assets/TimeModifierBehavior.cs:29)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

New Game clicked - Starting new game...
UnityEngine.Debug:Log (object)
MainMenuManager:OnNewGameClicked () (at Assets/Scripts/MainMenuManager.cs:109)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[ShiftTimerStarter] StartShiftTimer called - ensuring timer starts
UnityEngine.Debug:Log (object)
ShiftTimerStarter:StartShiftTimer () (at Assets/Scripts/ShiftTimerStarter.cs:24)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[ShiftTimerStarter] Timer already active
UnityEngine.Debug:Log (object)
ShiftTimerStarter:StartShiftTimer () (at Assets/Scripts/ShiftTimerStarter.cs:39)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[ShiftTimerStarter] ✅ DayProgressionManager shift started
UnityEngine.Debug:Log (object)
ShiftTimerStarter:StartShiftTimer () (at Assets/Scripts/ShiftTimerStarter.cs:55)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[TimeManager] Removed time modifier: DailyBriefingPanel_2e6668a0 from DailyBriefingPanel
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:ResumeTimeForSource (UnityEngine.GameObject) (at Assets/TimeManager.cs:177)
TimeModifierBehavior:ResumeTime () (at Assets/TimeModifierBehavior.cs:87)
TimeModifierBehavior:OnDisable () (at Assets/TimeModifierBehavior.cs:37)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[TimeManager] Time RESUMED: Normal speed
UnityEngine.Debug:Log (object)
TimeManager:LogStatus (string) (at Assets/TimeManager.cs:290)
TimeManager:UpdateTimeState () (at Assets/TimeManager.cs:230)
TimeManager:ResumeTimeForSource (UnityEngine.GameObject) (at Assets/TimeManager.cs:184)
TimeModifierBehavior:ResumeTime () (at Assets/TimeModifierBehavior.cs:87)
TimeModifierBehavior:OnDisable () (at Assets/TimeModifierBehavior.cs:37)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[ShiftTimerManager] Time remaining: 117.0s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:Update () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:186)

[UICoordinator] Notification shown: Day 2 has begun. Good luck, operator!
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator/<DisplayNotification>d__66:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:625)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[ShiftTimerManager] Time remaining: 114.0s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:Update () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:186)

Daily briefing complete. Setting game state to Gameplay. Current day: 3
UnityEngine.Debug:Log (object)
GameManager/<ShowBriefingThenHide>d__83:MoveNext () (at Assets/_scripts/GameManager.cs:619)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[UICoordinator] Notification shown: Day 3 has begun. Good luck, operator!
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator/<DisplayNotification>d__66:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:625)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[ShiftTimerManager] Time remaining: 111.0s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:Update () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:186)

[CredentialChecker] Forcing game state to active gameplay
UnityEngine.Debug:Log (object)
CredentialChecker:LogStatus (string) (at Assets/_scripts/CredentialChecker.cs:843)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:673)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameStateController] Game state changing: MainMenu -> ActiveGameplay
UnityEngine.Debug:Log (object)
GameStateController:LogStatus (string) (at Assets/GameStateController.cs:311)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:187)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Game audio unmuted
UnityEngine.Debug:Log (object)
GameStateController:SetAudioMuted (bool) (at Assets/GameStateController.cs:290)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:198)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Skipping old captain video system - using new EncounterMediaTransitionManager
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:970)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: No reaction video playing - updating text immediately for Troop Carrier
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:517)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Updated ship info text for Troop Carrier
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:468)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Updated credentials text for Troop Carrier
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:479)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Access Code: STD-5001
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:480)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Manifest: Injured stormtroopers, medical droids, Infec tanks
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:481)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Manifest Source: Legacy String
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:482)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Skipping old ship video system - using new EncounterMediaTransitionManager
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1032)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Skipping old captain video system - using new EncounterMediaTransitionManager
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1085)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: SafeEnableButtons - canEnable: True (processing: False, animation: False, video: False)
UnityEngine.Debug:Log (object)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2634)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1129)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Setting decision buttons to ENABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2638)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1129)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: New encounter displayed - enabling decision buttons
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1132)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] ✅ Using stored captain reference: idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:844)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 🚀 ENCOUNTER PREPARED (MasterShipEncounter): idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:154)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 👨‍✈️ Captain found: idos flek from type: Imperium Officer
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:155)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Playing greeting videos for Troop Carrier - idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:173)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Starting greeting sequence for Troop Carrier
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayGreetingSequence>d__54:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:184)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Starting Captain video: IMC_Greet002, Length: 12.25s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayEncounterVideo>d__55:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:218)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Notified new EncounterMediaTransitionManager of encounter
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1158)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Successfully displayed encounter for ship: Troop Carrier
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1161)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

MasterShipGenerator: New encounter ready: Troop Carrier
UnityEngine.Debug:Log (object)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1386)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

MasterShipGenerator: Returning encounter Troop Carrier - idos flek
UnityEngine.Debug:Log (object)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:629)
GameStateController:HandleStateTransition (GameStateController/GameActivationState,GameStateController/GameActivationState) (at Assets/GameStateController.cs:224)
GameStateController:SetGameState (GameStateController/GameActivationState) (at Assets/GameStateController.cs:206)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:677)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

MasterShipGenerator: Encounter on cooldown for 2.0 more seconds
UnityEngine.Debug:Log (object)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:576)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:689)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CredentialChecker] Forcing UI elements to be visible
UnityEngine.Debug:Log (object)
CredentialChecker:LogStatus (string) (at Assets/_scripts/CredentialChecker.cs:843)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:704)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CredentialChecker] Restarted ship video player
UnityEngine.Debug:Log (object)
CredentialChecker:LogStatus (string) (at Assets/_scripts/CredentialChecker.cs:843)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:719)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CredentialChecker] Restarted captain video player
UnityEngine.Debug:Log (object)
CredentialChecker:LogStatus (string) (at Assets/_scripts/CredentialChecker.cs:843)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:729)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CredentialChecker] Set alpha to 1 on ShipVideoContainer
UnityEngine.Debug:Log (object)
CredentialChecker:LogStatus (string) (at Assets/_scripts/CredentialChecker.cs:843)
CredentialChecker:TrySetCanvasGroupAlpha (UnityEngine.GameObject,single) (at Assets/_scripts/CredentialChecker.cs:832)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:734)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CredentialChecker] Set alpha to 1 on captainVideoContainer
UnityEngine.Debug:Log (object)
CredentialChecker:LogStatus (string) (at Assets/_scripts/CredentialChecker.cs:843)
CredentialChecker:TrySetCanvasGroupAlpha (UnityEngine.GameObject,single) (at Assets/_scripts/CredentialChecker.cs:832)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:735)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CredentialChecker] Set alpha to 1 on captainVideoContainer
UnityEngine.Debug:Log (object)
CredentialChecker:LogStatus (string) (at Assets/_scripts/CredentialChecker.cs:843)
CredentialChecker:TrySetCanvasGroupAlpha (UnityEngine.GameObject,single) (at Assets/_scripts/CredentialChecker.cs:832)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:737)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: SafeEnableButtons - canEnable: True (processing: False, animation: False, video: False)
UnityEngine.Debug:Log (object)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2634)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:752)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Setting decision buttons to ENABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2638)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:752)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CredentialChecker] Refreshing display with current encounter
UnityEngine.Debug:Log (object)
CredentialChecker:LogStatus (string) (at Assets/_scripts/CredentialChecker.cs:843)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:777)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Skipping old captain video system - using new EncounterMediaTransitionManager
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:970)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: No reaction video playing - updating text immediately for Troop Carrier
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:517)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Updated ship info text for Troop Carrier
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:468)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Updated credentials text for Troop Carrier
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:479)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Access Code: STD-5001
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:480)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Manifest: Injured stormtroopers, medical droids, Infec tanks
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:481)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Manifest Source: Legacy String
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:482)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Skipping old ship video system - using new EncounterMediaTransitionManager
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1032)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Skipping old captain video system - using new EncounterMediaTransitionManager
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1085)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: SafeEnableButtons - canEnable: True (processing: False, animation: False, video: False)
UnityEngine.Debug:Log (object)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2634)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1129)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Setting decision buttons to ENABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2638)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1129)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: New encounter displayed - enabling decision buttons
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1132)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] ✅ Using stored captain reference: idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:844)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 🚀 ENCOUNTER PREPARED (MasterShipEncounter): idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:154)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 👨‍✈️ Captain found: idos flek from type: Imperium Officer
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:155)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Playing greeting videos for Troop Carrier - idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:173)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Starting greeting sequence for Troop Carrier
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayGreetingSequence>d__54:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:184)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Starting Captain video: IMC_Greet002, Length: 12.25s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayEncounterVideo>d__55:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:218)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Notified new EncounterMediaTransitionManager of encounter
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1158)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Successfully displayed encounter for ship: Troop Carrier
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1161)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:778)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CredentialChecker] UI visibility forced - all elements should now be visible
UnityEngine.Debug:Log (object)
CredentialChecker:LogStatus (string) (at Assets/_scripts/CredentialChecker.cs:843)
CredentialChecker:ForceUIVisibility () (at Assets/_scripts/CredentialChecker.cs:818)
CredentialChecker:ForceActiveGameplay () (at Assets/_scripts/CredentialChecker.cs:696)
CredentialChecker:IsGameStateReadyForProcessing () (at Assets/_scripts/CredentialChecker.cs:2682)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1276)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CaptainVideoResponseManager: No response video available for Approve
UnityEngine.Debug:Log (object)
CaptainVideoResponseManager:OnDecisionMade (CaptainVideoResponseManager/ResponseType) (at Assets/Scripts/CaptainVideoResponseManager.cs:239)
CaptainVideoResponseManager:<SubscribeToDecisionEvents>b__18_0 () (at Assets/Scripts/CaptainVideoResponseManager.cs:124)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Must wait 3.0 more seconds before making a decision
UnityEngine.Debug:Log (object)
CredentialChecker:CanProcessDecision () (at Assets/_scripts/CredentialChecker.cs:2360)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1265)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Captain video prepared, starting playback
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayEncounterVideo>d__55:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:238)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[ShiftTimerManager] Time remaining: 108.0s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:Update () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:186)

APPROVE clicked for ship: Troop Carrier, code: STD-5001, shouldApprove: True
UnityEngine.Debug:Log (object)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1298)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Setting decision buttons to DISABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1768)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Processing APPROVE decision for ship: Troop Carrier, code: STD-5001, shouldApprove: True
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1773)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

=== [DECISION] CredentialChecker.ProcessDecision() - Approved: True, Correct: True ===
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1835)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] ✅ Using stored captain reference: idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:844)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 🚀 ENCOUNTER PREPARED (MasterShipEncounter): idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:154)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 👨‍✈️ Captain found: idos flek from type: Imperium Officer
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:155)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Playing greeting videos for Troop Carrier - idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:173)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Starting greeting sequence for Troop Carrier
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayGreetingSequence>d__54:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:184)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Starting Captain video: IMC_Greet002, Length: 12.25s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayEncounterVideo>d__55:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:218)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Captain video prepared, starting playback
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayEncounterVideo>d__55:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:238)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] === CAPTAIN REACTION DEBUG START ===
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:272)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] ShowCaptainReactionVideo called - Approved: True, Bribery: False
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:273)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] _currentCaptain: idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:274)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] _currentCaptainType: Imperium Officer
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:275)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] _currentEncounter: NULL
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:276)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 🎬 Selecting reaction video for captain: idos flek
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:355)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] ✅ Approval reaction: approveResponse
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:376)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 🎯 Final selected reaction for idos flek:
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:388)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager]   - Type: Approval
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:389)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager]   - Video: approveResponse
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:390)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager]   - Dialog: 'about time'
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:391)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] === CAPTAIN REACTION DEBUG END ===
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:392)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Playing Approval reaction: "about time"
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:453)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:394)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Setting decision buttons to DISABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:OnReactionVideoStarted () (at Assets/_scripts/CredentialChecker.cs:409)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:456)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:394)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Reaction video started - blocking text updates and disabling buttons
UnityEngine.Debug:Log (object)
CredentialChecker:OnReactionVideoStarted () (at Assets/_scripts/CredentialChecker.cs:413)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:456)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:394)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Triggered approval reaction video
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1886)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Key Decision Made: ship_Troop Carrier_approved
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.NarrativeManager:HandleKeyDecision (StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker/NarrativeState/DecisionRecord) (at Assets/NarrativeManager.cs:219)
StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker:RecordDecision (string,int,int,string) (at Assets/NarrativeTracker.cs:94)
StarkillerBaseCommand.NarrativeManager:RecordEnhancedDecision (string,int,int,string,StarkillerBaseCommand.Narrative.DecisionCategory,StarkillerBaseCommand.Narrative.DecisionPressure) (at Assets/NarrativeManager.cs:91)
StarkillerBaseCommand.NarrativeManager:RecordShipDecision (MasterShipEncounter,bool) (at Assets/NarrativeManager.cs:158)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1913)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Notifying integration helper of decision before notifying ship generator
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1972)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Notifying GameManagerIntegrationHelper of ship processing
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1982)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Notifying GameManager of decision before notifying ship generator
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1988)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

=== [DECISION] GameManager.OnDecisionMade() - Approved: True, Correct: True ===
UnityEngine.Debug:Log (object)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:885)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameManager] Decision recorded via DecisionTracker: Correct decision
UnityEngine.Debug:Log (object)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:911)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[DayProgressionManager] Ship processed - Daily: (1/8), Total: 1
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:RecordShipProcessed () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:375)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:944)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[UICoordinator] UpdateShipsDisplay - Day 3: 1/8 (Until quota: 7)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateShipsDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:365)
Starkiller.Core.Managers.UICoordinator:OnShipProcessed (int) (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:711)
Starkiller.Core.Managers.DayProgressionManager:RecordShipProcessed () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:377)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:944)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameManager] Ship processing triggered via DayProgressionManager
UnityEngine.Debug:Log (object)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:945)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameManager] UpdateUI - Day: 3, Ships: 1/8
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:807)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:1153)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameManager] UpdateUI - Synced strikes from DecisionTracker: 0
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:815)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:1153)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameManager] UpdateUI: Day 3, Ships 1/8, Credits 30, Strikes 0/10
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:822)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:1153)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterFlow] Starting 2s cooldown after decision
UnityEngine.Debug:Log (object)
EncounterFlowManager/<DecisionCooldown>d__18:MoveNext () (at Assets/Scripts/EncounterFlowManager.cs:164)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
EncounterFlowManager:OnDecisionMade () (at Assets/Scripts/EncounterFlowManager.cs:155)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:1159)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[DayProgressionManager] Ship processed - Daily: (2/8), Total: 2
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:RecordShipProcessed () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:375)
Starkiller.Core.Managers.DayProgressionManager:OnDecisionCompleted (Starkiller.Core.DecisionType,Starkiller.Core.IEncounter) (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:470)
Starkiller.Core.GameEvents:TriggerDecisionMade (Starkiller.Core.DecisionType,Starkiller.Core.IEncounter) (at Assets/_RefactoredScripts/Core/GameEvents.cs:97)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1999)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[UICoordinator] UpdateShipsDisplay - Day 3: 2/8 (Until quota: 6)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateShipsDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:365)
Starkiller.Core.Managers.UICoordinator:OnShipProcessed (int) (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:711)
Starkiller.Core.Managers.DayProgressionManager:RecordShipProcessed () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:377)
Starkiller.Core.Managers.DayProgressionManager:OnDecisionCompleted (Starkiller.Core.DecisionType,Starkiller.Core.IEncounter) (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:470)
Starkiller.Core.GameEvents:TriggerDecisionMade (Starkiller.Core.DecisionType,Starkiller.Core.IEncounter) (at Assets/_RefactoredScripts/Core/GameEvents.cs:97)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1999)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CredentialChecker] Triggered GameEvents.OnDecisionMade with decision: Approve
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2000)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Notifying ship generator of decision
UnityEngine.Debug:Log (object)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2772)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

=== [GENERATOR DECISION] MasterShipGenerator.ProcessDecisionWithEncounter() - Approved: True, Ship: Troop Carrier ===
UnityEngine.Debug:Log (object)
MasterShipGenerator:ProcessDecisionWithEncounter (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:661)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2773)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

MasterShipGenerator.ProcessEncounterInternal() - Processing APPROVE decision
UnityEngine.Debug:Log (object)
MasterShipGenerator:ProcessEncounterInternal (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1154)
MasterShipGenerator:ProcessDecisionWithEncounter (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:664)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2773)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Correct decision! Ship approved
UnityEngine.Debug:Log (object)
MasterShipGenerator:ProcessEncounterInternal (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1196)
MasterShipGenerator:ProcessDecisionWithEncounter (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:664)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2773)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Clearing current encounter reference
UnityEngine.Debug:Log (object)
MasterShipGenerator:ProcessEncounterInternal (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1209)
MasterShipGenerator:ProcessDecisionWithEncounter (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:664)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2773)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

MasterShipGenerator: Skipping auto-generation - EncounterMediaTransitionManager will handle next encounter
UnityEngine.Debug:Log (object)
MasterShipGenerator:ProcessEncounterInternal (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1226)
MasterShipGenerator:ProcessDecisionWithEncounter (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:664)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2773)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1303)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] HideIDCard() called! Stack trace:
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:177)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1308)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

UnityEngine.Debug:ExtractStackTraceNoAlloc (byte*,int,string)
UnityEngine.StackTraceUtility:ExtractStackTrace () (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/StackTrace.cs:37)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:178)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1308)
UnityEngine.Events.InvokableCall:Invoke () (at /Users/bokken/build/output/unity/unity/Runtime/Export/UnityEvent/UnityEvent.cs:178)
UnityEngine.Events.UnityEvent:Invoke () (at /Users/bokken/build/output/unity/unity/artifacts/generated/UnityEvent/UnityEvent_0.cs:57)
UnityEngine.UI.Button:Press () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/UI/Core/Button.cs:70)
UnityEngine.UI.Button:OnPointerClick (UnityEngine.EventSystems.PointerEventData) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/UI/Core/Button.cs:114)
UnityEngine.EventSystems.ExecuteEvents:Execute (UnityEngine.EventSystems.IPointerClickHandler,UnityEngine.EventSystems.BaseEventData) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/ExecuteEvents.cs:57)
UnityEngine.EventSystems.ExecuteEvents:Execute<UnityEngine.EventSystems.IPointerClickHandler> (UnityEngine.GameObject,UnityEngine.EventSystems.BaseEventData,UnityEngine.EventSystems.ExecuteEvents/EventFunction`1<UnityEngine.EventSystems.IPointerClickHandler>) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/ExecuteEvents.cs:272)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:ProcessPointerButton (UnityEngine.InputSystem.UI.PointerModel/ButtonState&,UnityEngine.EventSystems.PointerEventData) (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:617)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:ProcessPointer (UnityEngine.InputSystem.UI.PointerModel&) (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:362)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:Process () (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:2270)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:178)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1308)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] isAnimating: False, gameObject.activeInHierarchy: True
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:179)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1308)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] idCardPanel.activeSelf: True
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:180)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1308)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] Starting AnimateHide coroutine
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:191)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1308)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Already processing a decision or in animation, ignoring approve click
UnityEngine.Debug:Log (object)
CredentialChecker:OnApproveClicked () (at Assets/_scripts/CredentialChecker.cs:1271)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[ShiftTimerManager] Time remaining: 105.0s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:Update () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:186)

[EncounterMediaTransitionManager] Showing dialog text: "about time"
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<ShowDialogText>d__59:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:547)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

Processing lock released after delay
UnityEngine.Debug:Log (object)
CredentialChecker/<ReleaseProcessingLockAfterDelay>d__86:MoveNext () (at Assets/_scripts/CredentialChecker.cs:2089)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Dialog text displayed for 0.5s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<ShowDialogText>d__59:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:584)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Dialog text hidden
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<ShowDialogText>d__59:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:600)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Attempting to play video: approveResponse
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:488)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Starting video: approveResponse, Length: 4.83333333333333s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoInternal>d__62:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:634)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoWithResult>d__60:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:609)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:490)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Starting video preparation...
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoInternal>d__62:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:652)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoWithResult>d__60:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:609)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:490)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Waiting for video preparation. isPrepared: False
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoInternal>d__62:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:660)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoWithResult>d__60:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:609)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:490)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Captain video playback completed
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayEncounterVideo>d__55:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:262)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Starting Ship video: UItest2, Length: 5s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayEncounterVideo>d__55:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:218)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Video prepared in 0.06074179s, starting playback
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoInternal>d__62:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:682)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Waiting for video completion. MaxWait: 6.833333s, isPlaying: True
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoInternal>d__62:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:696)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[CaptainIDCard] ToggleIDCardFromButton() called!
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:223)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] Toggle stack trace:
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:224)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

UnityEngine.Debug:ExtractStackTraceNoAlloc (byte*,int,string)
UnityEngine.StackTraceUtility:ExtractStackTrace () (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/StackTrace.cs:37)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:225)
UnityEngine.Events.InvokableCall:Invoke () (at /Users/bokken/build/output/unity/unity/Runtime/Export/UnityEvent/UnityEvent.cs:178)
UnityEngine.Events.UnityEvent:Invoke () (at /Users/bokken/build/output/unity/unity/artifacts/generated/UnityEvent/UnityEvent_0.cs:57)
UnityEngine.UI.Button:Press () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/UI/Core/Button.cs:70)
UnityEngine.UI.Button:OnPointerClick (UnityEngine.EventSystems.PointerEventData) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/UI/Core/Button.cs:114)
UnityEngine.EventSystems.ExecuteEvents:Execute (UnityEngine.EventSystems.IPointerClickHandler,UnityEngine.EventSystems.BaseEventData) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/ExecuteEvents.cs:57)
UnityEngine.EventSystems.ExecuteEvents:Execute<UnityEngine.EventSystems.IPointerClickHandler> (UnityEngine.GameObject,UnityEngine.EventSystems.BaseEventData,UnityEngine.EventSystems.ExecuteEvents/EventFunction`1<UnityEngine.EventSystems.IPointerClickHandler>) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/ExecuteEvents.cs:272)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:ProcessPointerButton (UnityEngine.InputSystem.UI.PointerModel/ButtonState&,UnityEngine.EventSystems.PointerEventData) (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:617)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:ProcessPointer (UnityEngine.InputSystem.UI.PointerModel&) (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:362)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:Process () (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:2270)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:225)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] idCardPanel.activeSelf: True
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:226)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] canvasGroup.alpha: 1
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:227)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] Panel is visually showing (alpha > 0.5), hiding it
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:232)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] HideIDCard() called! Stack trace:
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:177)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:233)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

UnityEngine.Debug:ExtractStackTraceNoAlloc (byte*,int,string)
UnityEngine.StackTraceUtility:ExtractStackTrace () (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/StackTrace.cs:37)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:178)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:233)
UnityEngine.Events.InvokableCall:Invoke () (at /Users/bokken/build/output/unity/unity/Runtime/Export/UnityEvent/UnityEvent.cs:178)
UnityEngine.Events.UnityEvent:Invoke () (at /Users/bokken/build/output/unity/unity/artifacts/generated/UnityEvent/UnityEvent_0.cs:57)
UnityEngine.UI.Button:Press () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/UI/Core/Button.cs:70)
UnityEngine.UI.Button:OnPointerClick (UnityEngine.EventSystems.PointerEventData) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/UI/Core/Button.cs:114)
UnityEngine.EventSystems.ExecuteEvents:Execute (UnityEngine.EventSystems.IPointerClickHandler,UnityEngine.EventSystems.BaseEventData) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/ExecuteEvents.cs:57)
UnityEngine.EventSystems.ExecuteEvents:Execute<UnityEngine.EventSystems.IPointerClickHandler> (UnityEngine.GameObject,UnityEngine.EventSystems.BaseEventData,UnityEngine.EventSystems.ExecuteEvents/EventFunction`1<UnityEngine.EventSystems.IPointerClickHandler>) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/ExecuteEvents.cs:272)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:ProcessPointerButton (UnityEngine.InputSystem.UI.PointerModel/ButtonState&,UnityEngine.EventSystems.PointerEventData) (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:617)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:ProcessPointer (UnityEngine.InputSystem.UI.PointerModel&) (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:362)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:Process () (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:2270)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:178)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:233)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] isAnimating: False, gameObject.activeInHierarchy: True
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:179)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:233)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] idCardPanel.activeSelf: True
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:180)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:233)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] Starting AnimateHide coroutine
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:191)
StarkillerBaseCommand.CaptainIDCard:ToggleIDCardFromButton () (at Assets/_scripts/CaptainIDCard.cs:233)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: SafeEnableButtons - canEnable: False (processing: False, animation: False, video: True)
UnityEngine.Debug:Log (object)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2634)
CredentialChecker/<ReleaseAnimationLockAfterDelay>d__87:MoveNext () (at Assets/_scripts/CredentialChecker.cs:2104)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Setting decision buttons to DISABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2643)
CredentialChecker/<ReleaseAnimationLockAfterDelay>d__87:MoveNext () (at Assets/_scripts/CredentialChecker.cs:2104)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

Animation lock released after delay
UnityEngine.Debug:Log (object)
CredentialChecker/<ReleaseAnimationLockAfterDelay>d__87:MoveNext () (at Assets/_scripts/CredentialChecker.cs:2108)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterFlow] Cooldown complete, ready for next encounter
UnityEngine.Debug:Log (object)
EncounterFlowManager/<DecisionCooldown>d__18:MoveNext () (at Assets/Scripts/EncounterFlowManager.cs:169)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[ShiftTimerManager] Time remaining: 102.0s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:Update () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:186)

[EncounterMediaTransitionManager] Video wait loop ended. ElapsedTime: 2.281004s, isPlaying: False, timeout: False
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoInternal>d__62:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:707)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Video playback completed (success: True)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoInternal>d__62:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:718)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

Asset Packages/com.justinpbarnett.unity-mcp/Python/__pycache__ has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/__pycache__/config.cpython-312.pyc has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/__pycache__/unity_connection.cpython-312.pyc has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/tools/__pycache__ has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/tools/__pycache__/__init__.cpython-312.pyc has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/tools/__pycache__/asset_tools.cpython-312.pyc has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/tools/__pycache__/editor_tools.cpython-312.pyc has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/tools/__pycache__/material_tools.cpython-312.pyc has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/tools/__pycache__/object_tools.cpython-312.pyc has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/tools/__pycache__/scene_tools.cpython-312.pyc has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/tools/__pycache__/script_tools.cpython-312.pyc has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/unity_mcp.egg-info/dependency_links.txt has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/unity_mcp.egg-info/PKG-INFO has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/unity_mcp.egg-info/requires.txt has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/unity_mcp.egg-info/SOURCES.txt has no meta file, but it's in an immutable folder. The asset will be ignored.

Asset Packages/com.justinpbarnett.unity-mcp/Python/unity_mcp.egg-info/top_level.txt has no meta file, but it's in an immutable folder. The asset will be ignored.

CredentialChecker: Reaction video completed - allowing text updates
UnityEngine.Debug:Log (object)
CredentialChecker:OnReactionVideoCompleted () (at Assets/_scripts/CredentialChecker.cs:428)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:518)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Completed Approval reaction sequence (video success: True)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:523)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Auto-advance enabled - triggering after 1s delay
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:531)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Delayed auto-advance triggered
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1172)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Triggering next encounter...
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1093)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[ServiceLocator] Registered service: CredentialChecker
UnityEngine.Debug:Log (object)
Starkiller.Core.ServiceLocator:Register<CredentialChecker> (CredentialChecker) (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:53)
Starkiller.Core.ServiceLocator:Get<CredentialChecker> () (at Assets/_RefactoredScripts/Core/ServiceLocator.cs:96)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1098)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Skipping old captain video system - using new EncounterMediaTransitionManager
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:970)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: No reaction video playing - updating text immediately for Concorde
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:517)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Updated ship info text for Concorde
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:468)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Updated credentials text for Concorde
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:479)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Access Code: M1L-1001
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:480)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Manifest: Power cells, Communication equipment, Equipment parts, Unauthorized weapons for authorized operations
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:481)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Manifest Source: Legacy String
UnityEngine.Debug:Log (object)
CredentialChecker:UpdateEncounterText (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:482)
CredentialChecker:UpdateEncounterTextWithSync (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:518)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:994)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Skipping old ship video system - using new EncounterMediaTransitionManager
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1032)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Skipping old captain video system - using new EncounterMediaTransitionManager
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1085)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: SafeEnableButtons - canEnable: True (processing: False, animation: False, video: False)
UnityEngine.Debug:Log (object)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2634)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1129)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Setting decision buttons to ENABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2638)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1129)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: New encounter displayed - enabling decision buttons
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1132)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] ✅ Using stored captain reference: Black Jack
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:844)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] 🚀 ENCOUNTER PREPARED (MasterShipEncounter): Black Jack
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:154)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] 👨‍✈️ Captain found: Black Jack from type: Pirates
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:155)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Playing greeting videos for Concorde - Black Jack
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:173)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Starting greeting sequence for Concorde
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayGreetingSequence>d__54:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:184)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Starting Captain video: TempCap_PirateCapt, Length: 4.66666666666667s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayEncounterVideo>d__55:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:218)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1156)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Notified new EncounterMediaTransitionManager of encounter
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1158)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Successfully displayed encounter for ship: Concorde
UnityEngine.Debug:Log (object)
CredentialChecker:DisplayEncounter (MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:1161)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1385)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

MasterShipGenerator: New encounter ready: Concorde
UnityEngine.Debug:Log (object)
MasterShipGenerator:DequeueNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1386)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:614)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

MasterShipGenerator: Returning encounter Concorde - Black Jack
UnityEngine.Debug:Log (object)
MasterShipGenerator:GetNextEncounter () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:629)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2425)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: SafeEnableButtons - canEnable: True (processing: False, animation: False, video: False)
UnityEngine.Debug:Log (object)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2634)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2460)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Setting decision buttons to ENABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2638)
CredentialChecker:NextEncounter () (at Assets/_scripts/CredentialChecker.cs:2460)
Starkiller.Core.Managers.EncounterMediaTransitionManager:TriggerNextEncounter () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1107)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<DelayedAutoAdvance>d__75:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1174)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Trigger flag reset - ready for next encounter
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<ResetTriggerFlag>d__73:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:1146)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[ShiftTimerManager] Time remaining: 99.0s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:Update () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:186)

Must wait 0.6 more seconds before making a decision
UnityEngine.Debug:Log (object)
CredentialChecker:CanProcessDecision () (at Assets/_scripts/CredentialChecker.cs:2360)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1321)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CaptainVideoResponseManager: No response video available for Deny
UnityEngine.Debug:Log (object)
CaptainVideoResponseManager:OnDecisionMade (CaptainVideoResponseManager/ResponseType) (at Assets/Scripts/CaptainVideoResponseManager.cs:239)
CaptainVideoResponseManager:<SubscribeToDecisionEvents>b__18_1 () (at Assets/Scripts/CaptainVideoResponseManager.cs:127)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

DENY clicked for ship: Concorde, code: M1L-1001, shouldApprove: False
UnityEngine.Debug:Log (object)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1354)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Setting decision buttons to DISABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1768)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Processing DENY decision for ship: Concorde, code: M1L-1001, shouldApprove: False
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1773)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

=== [DECISION] CredentialChecker.ProcessDecision() - Approved: False, Correct: True ===
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1835)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] ✅ Using stored captain reference: Black Jack
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:FindSpecificCaptainForEncounter (MasterShipEncounter,CaptainType) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:844)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:150)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 🚀 ENCOUNTER PREPARED (MasterShipEncounter): Black Jack
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:154)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 👨‍✈️ Captain found: Black Jack from type: Pirates
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:155)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Playing greeting videos for Concorde - Black Jack
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:173)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Starting greeting sequence for Concorde
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayGreetingSequence>d__54:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:184)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Starting Captain video: TempCap_PirateCapt, Length: 4.66666666666667s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayEncounterVideo>d__55:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:218)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Captain video prepared, starting playback
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayEncounterVideo>d__55:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:238)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PlayGreetingVideos (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:175)
Starkiller.Core.Managers.EncounterMediaTransitionManager:PrepareNextEncounter (MasterShipEncounter) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:161)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1884)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] === CAPTAIN REACTION DEBUG START ===
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:272)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] ShowCaptainReactionVideo called - Approved: False, Bribery: False
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:273)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] _currentCaptain: Black Jack
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:274)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] _currentCaptainType: Pirates
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:275)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] _currentEncounter: NULL
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:276)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 🎬 Selecting reaction video for captain: Black Jack
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:355)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] ❌ Denial reaction: denyResponse
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:382)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] 🎯 Final selected reaction for Black Jack:
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:388)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager]   - Type: Denial
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:389)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager]   - Video: denyResponse
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:390)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager]   - Dialog: 'deny'
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:391)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] === CAPTAIN REACTION DEBUG END ===
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:392)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Playing Denial reaction: "deny"
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:453)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:394)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Setting decision buttons to DISABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:OnReactionVideoStarted () (at Assets/_scripts/CredentialChecker.cs:409)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:456)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:394)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Reaction video started - blocking text updates and disabling buttons
UnityEngine.Debug:Log (object)
CredentialChecker:OnReactionVideoStarted () (at Assets/_scripts/CredentialChecker.cs:413)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:456)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager:ShowCaptainReactionVideo (bool,bool) (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:394)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1885)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Triggered denial reaction video
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1886)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Key Decision Made: ship_Concorde_denied
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.NarrativeManager:HandleKeyDecision (StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker/NarrativeState/DecisionRecord) (at Assets/NarrativeManager.cs:219)
StarkillerBaseCommand.Narrative.AdvancedNarrativeTracker:RecordDecision (string,int,int,string) (at Assets/NarrativeTracker.cs:94)
StarkillerBaseCommand.NarrativeManager:RecordEnhancedDecision (string,int,int,string,StarkillerBaseCommand.Narrative.DecisionCategory,StarkillerBaseCommand.Narrative.DecisionPressure) (at Assets/NarrativeManager.cs:91)
StarkillerBaseCommand.NarrativeManager:RecordShipDecision (MasterShipEncounter,bool) (at Assets/NarrativeManager.cs:158)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1913)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Notifying integration helper of decision before notifying ship generator
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1972)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Notifying GameManagerIntegrationHelper of ship processing
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1982)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Notifying GameManager of decision before notifying ship generator
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1988)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

=== [DECISION] GameManager.OnDecisionMade() - Approved: False, Correct: True ===
UnityEngine.Debug:Log (object)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:885)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameManager] Decision recorded via DecisionTracker: Correct decision
UnityEngine.Debug:Log (object)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:911)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[DayProgressionManager] Ship processed - Daily: (3/8), Total: 3
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:RecordShipProcessed () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:375)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:944)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[UICoordinator] UpdateShipsDisplay - Day 3: 3/8 (Until quota: 5)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateShipsDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:365)
Starkiller.Core.Managers.UICoordinator:OnShipProcessed (int) (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:711)
Starkiller.Core.Managers.DayProgressionManager:RecordShipProcessed () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:377)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:944)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameManager] Ship processing triggered via DayProgressionManager
UnityEngine.Debug:Log (object)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:945)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameManager] UpdateUI - Day: 3, Ships: 3/8
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:807)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:1153)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameManager] UpdateUI - Synced strikes from DecisionTracker: 0
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:815)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:1153)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[GameManager] UpdateUI: Day 3, Ships 3/8, Credits 30, Strikes 0/10
UnityEngine.Debug:Log (object)
GameManager:UpdateUI () (at Assets/_scripts/GameManager.cs:822)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:1153)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterFlow] Starting 2s cooldown after decision
UnityEngine.Debug:Log (object)
EncounterFlowManager/<DecisionCooldown>d__18:MoveNext () (at Assets/Scripts/EncounterFlowManager.cs:164)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
EncounterFlowManager:OnDecisionMade () (at Assets/Scripts/EncounterFlowManager.cs:155)
GameManager:OnDecisionMade (bool,bool) (at Assets/_scripts/GameManager.cs:1159)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1989)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[DayProgressionManager] Ship processed - Daily: (4/8), Total: 4
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.DayProgressionManager:RecordShipProcessed () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:375)
Starkiller.Core.Managers.DayProgressionManager:OnDecisionCompleted (Starkiller.Core.DecisionType,Starkiller.Core.IEncounter) (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:470)
Starkiller.Core.GameEvents:TriggerDecisionMade (Starkiller.Core.DecisionType,Starkiller.Core.IEncounter) (at Assets/_RefactoredScripts/Core/GameEvents.cs:97)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1999)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[UICoordinator] UpdateShipsDisplay - Day 3: 4/8 (Until quota: 4)
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.UICoordinator:UpdateShipsDisplay () (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:365)
Starkiller.Core.Managers.UICoordinator:OnShipProcessed (int) (at Assets/_RefactoredScripts/Core/Managers/UICoordinator.cs:711)
Starkiller.Core.Managers.DayProgressionManager:RecordShipProcessed () (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:377)
Starkiller.Core.Managers.DayProgressionManager:OnDecisionCompleted (Starkiller.Core.DecisionType,Starkiller.Core.IEncounter) (at Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs:470)
Starkiller.Core.GameEvents:TriggerDecisionMade (Starkiller.Core.DecisionType,Starkiller.Core.IEncounter) (at Assets/_RefactoredScripts/Core/GameEvents.cs:97)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:1999)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CredentialChecker] Triggered GameEvents.OnDecisionMade with decision: Deny
UnityEngine.Debug:Log (object)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2000)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Notifying ship generator of decision
UnityEngine.Debug:Log (object)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2772)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

=== [GENERATOR DECISION] MasterShipGenerator.ProcessDecisionWithEncounter() - Approved: False, Ship: Concorde ===
UnityEngine.Debug:Log (object)
MasterShipGenerator:ProcessDecisionWithEncounter (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:661)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2773)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

MasterShipGenerator.ProcessEncounterInternal() - Processing DENY decision
UnityEngine.Debug:Log (object)
MasterShipGenerator:ProcessEncounterInternal (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1154)
MasterShipGenerator:ProcessDecisionWithEncounter (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:664)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2773)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Correct decision! Ship denied
UnityEngine.Debug:Log (object)
MasterShipGenerator:ProcessEncounterInternal (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1196)
MasterShipGenerator:ProcessDecisionWithEncounter (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:664)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2773)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

Clearing current encounter reference
UnityEngine.Debug:Log (object)
MasterShipGenerator:ProcessEncounterInternal (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1209)
MasterShipGenerator:ProcessDecisionWithEncounter (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:664)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2773)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

MasterShipGenerator: Skipping auto-generation - EncounterMediaTransitionManager will handle next encounter
UnityEngine.Debug:Log (object)
MasterShipGenerator:ProcessEncounterInternal (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:1226)
MasterShipGenerator:ProcessDecisionWithEncounter (bool,MasterShipEncounter) (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:664)
CredentialChecker:SafelyNotifyShipGenerator (bool,MasterShipEncounter) (at Assets/_scripts/CredentialChecker.cs:2773)
CredentialChecker:ProcessDecision (bool) (at Assets/_scripts/CredentialChecker.cs:2003)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1359)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] HideIDCard() called! Stack trace:
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:177)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1364)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

UnityEngine.Debug:ExtractStackTraceNoAlloc (byte*,int,string)
UnityEngine.StackTraceUtility:ExtractStackTrace () (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/StackTrace.cs:37)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:178)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1364)
UnityEngine.Events.InvokableCall:Invoke () (at /Users/bokken/build/output/unity/unity/Runtime/Export/UnityEvent/UnityEvent.cs:178)
UnityEngine.Events.UnityEvent:Invoke () (at /Users/bokken/build/output/unity/unity/artifacts/generated/UnityEvent/UnityEvent_0.cs:57)
UnityEngine.UI.Button:Press () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/UI/Core/Button.cs:70)
UnityEngine.UI.Button:OnPointerClick (UnityEngine.EventSystems.PointerEventData) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/UI/Core/Button.cs:114)
UnityEngine.EventSystems.ExecuteEvents:Execute (UnityEngine.EventSystems.IPointerClickHandler,UnityEngine.EventSystems.BaseEventData) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/ExecuteEvents.cs:57)
UnityEngine.EventSystems.ExecuteEvents:Execute<UnityEngine.EventSystems.IPointerClickHandler> (UnityEngine.GameObject,UnityEngine.EventSystems.BaseEventData,UnityEngine.EventSystems.ExecuteEvents/EventFunction`1<UnityEngine.EventSystems.IPointerClickHandler>) (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/ExecuteEvents.cs:272)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:ProcessPointerButton (UnityEngine.InputSystem.UI.PointerModel/ButtonState&,UnityEngine.EventSystems.PointerEventData) (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:617)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:ProcessPointer (UnityEngine.InputSystem.UI.PointerModel&) (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:362)
UnityEngine.InputSystem.UI.InputSystemUIInputModule:Process () (at ./Library/PackageCache/com.unity.inputsystem/InputSystem/Plugins/UI/InputSystemUIInputModule.cs:2270)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:178)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1364)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] isAnimating: False, gameObject.activeInHierarchy: True
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:179)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1364)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] idCardPanel.activeSelf: True
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:180)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1364)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[CaptainIDCard] Starting AnimateHide coroutine
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.CaptainIDCard:HideIDCard () (at Assets/_scripts/CaptainIDCard.cs:191)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1364)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

CredentialChecker: Already processing a decision or in animation, ignoring deny click
UnityEngine.Debug:Log (object)
CredentialChecker:OnDenyClicked () (at Assets/_scripts/CredentialChecker.cs:1327)
UnityEngine.EventSystems.EventSystem:Update () (at ./Library/PackageCache/com.unity.ugui/Runtime/UGUI/EventSystem/EventSystem.cs:530)

[EncounterMediaTransitionManager] Showing dialog text: "deny"
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<ShowDialogText>d__59:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:547)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Attempting to play video: denyResponse
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:488)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Starting video: denyResponse, Length: 4.54166666666667s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoInternal>d__62:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:634)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoWithResult>d__60:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:609)
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionSequence>d__58:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:490)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Video prepared in 0.06126867s, starting playback
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoInternal>d__62:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:682)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[EncounterMediaTransitionManager] Waiting for video completion. MaxWait: 6.541667s, isPlaying: True
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.EncounterMediaTransitionManager/<PlayReactionVideoInternal>d__62:MoveNext () (at Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs:696)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

[ShiftTimerManager] Time remaining: 96.0s
UnityEngine.Debug:Log (object)
Starkiller.Core.Managers.ShiftTimerManager:Update () (at Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs:186)

CredentialChecker: SafeEnableButtons - canEnable: False (processing: False, animation: False, video: True)
UnityEngine.Debug:Log (object)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2634)
CredentialChecker/<HandlePostDecisionFlow>d__108:MoveNext () (at Assets/_scripts/CredentialChecker.cs:2861)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

CredentialChecker: Setting decision buttons to DISABLED
UnityEngine.Debug:Log (object)
CredentialChecker:SetButtonsInteractable (bool) (at Assets/_scripts/CredentialChecker.cs:2567)
CredentialChecker:SafeEnableButtons () (at Assets/_scripts/CredentialChecker.cs:2643)
CredentialChecker/<HandlePostDecisionFlow>d__108:MoveNext () (at Assets/_scripts/CredentialChecker.cs:2861)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

Buttons re-enabled after post-decision flow
UnityEngine.Debug:Log (object)
CredentialChecker/<HandlePostDecisionFlow>d__108:MoveNext () (at Assets/_scripts/CredentialChecker.cs:2869)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

MasterShipGenerator: Clearing singleton instance on destroy
UnityEngine.Debug:Log (object)
MasterShipGenerator:OnDestroy () (at Assets/_Core/ShipSystem/MasterShipGenerator.cs:110)

CredentialChecker: Unsubscribed from video synchronization events
UnityEngine.Debug:Log (object)
CredentialChecker:UnsubscribeFromVideoSyncEvents () (at Assets/_scripts/CredentialChecker.cs:398)
CredentialChecker:OnDestroy () (at Assets/_scripts/CredentialChecker.cs:528)

