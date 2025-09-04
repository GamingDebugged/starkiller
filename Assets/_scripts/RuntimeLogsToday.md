UnityMCPBridge started on port 6400.
UnityEngine.Debug:Log (object)
UnityMCP.Editor.UnityMCPBridge:Start () (at Library/PackageCache/com.justinpbarnett.unity-mcp/Editor/UnityMCPBridge.cs:52)
UnityMCP.Editor.UnityMCPBridge:.cctor () (at Library/PackageCache/com.justinpbarnett.unity-mcp/Editor/UnityMCPBridge.cs:42)
UnityEditor.EditorAssemblies:ProcessInitializeOnLoadAttributes (System.Type[]) (at /Users/bokken/build/output/unity/unity/Editor/Mono/EditorAssemblies.cs:118)

ContentManager synced with GameManager day: 1
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:115)

Generated 5 new access codes for day 1
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerContentManager:RegenerateAccessCodes () (at Assets/_scripts/StarkkillerContentManager.cs:352)
StarkillerBaseCommand.StarkkillerContentManager:Awake () (at Assets/_scripts/StarkkillerContentManager.cs:124)

Ship video player configured successfully
UnityEngine.Debug:Log (object)
VideoPlayerSetup:ConfigureVideoPlayers () (at Assets/VideoPlayerSetup.cs:126)
VideoPlayerSetup:Awake () (at Assets/VideoPlayerSetup.cs:38)

Captain video player configured successfully
UnityEngine.Debug:Log (object)
VideoPlayerSetup:ConfigureVideoPlayers () (at Assets/VideoPlayerSetup.cs:148)
VideoPlayerSetup:Awake () (at Assets/VideoPlayerSetup.cs:38)

ShipVideoSystem: ShipImageSystem component not found! Will use fallback images for ships without videos.
UnityEngine.Debug:LogWarning (object)
ShipVideoSystem:TryGetImageSystem () (at Assets/_scripts/ShipVideoSystem.cs:63)
ShipVideoSystem:Awake () (at Assets/_scripts/ShipVideoSystem.cs:43)

MasterShipGenerator initialized. ContentManager found: True, MediaSystem found: True, GameManager found: True
UnityEngine.Debug:Log (object)
MasterShipGenerator:Awake () (at Assets/MasterShipGenerator.cs:87)

MasterShipGenerator: Synced day with GameManager: 1
UnityEngine.Debug:Log (object)
MasterShipGenerator:Awake () (at Assets/MasterShipGenerator.cs:93)

=== Timer Diagnostic Started ===
UnityEngine.Debug:Log (object)
TimerDiagnostic:Start () (at Assets/TimerDiagnostic.cs:16)

DailyReportPanel reference: DailyReportPanel
UnityEngine.Debug:Log (object)
TimerDiagnostic:Start () (at Assets/TimerDiagnostic.cs:31)

DailyReportManager component: Found
UnityEngine.Debug:Log (object)
TimerDiagnostic:Start () (at Assets/TimerDiagnostic.cs:37)

DEBUG: Changed shift timer from 60 to 40 seconds
UnityEngine.Debug:Log (object)
TimerDiagnostic:Start () (at Assets/TimerDiagnostic.cs:49)

DEBUG: Set remainingTime to 40 seconds
UnityEngine.Debug:Log (object)
TimerDiagnostic:Start () (at Assets/TimerDiagnostic.cs:59)

=== Timer Diagnostic Completed ===
UnityEngine.Debug:Log (object)
TimerDiagnostic:Start () (at Assets/TimerDiagnostic.cs:63)

GameManager.dailyReportPanel is assigned to DailyReportPanel
UnityEngine.Debug:Log (object)
DailyReportDiagnostic:Start () (at Assets/DailyReportDiagnostic.cs:33)

DailyReportManager component found on panel
UnityEngine.Debug:Log (object)
DailyReportDiagnostic:Start () (at Assets/DailyReportDiagnostic.cs:43)

DailyReportManager.continueButton is assigned
UnityEngine.Debug:Log (object)
DailyReportDiagnostic:Start () (at Assets/DailyReportDiagnostic.cs:52)

ConsequenceCompatibilityManager: Started and ready for use
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.ConsequenceCompatibilityManager:Start () (at Assets/_scripts/ConsequenceCompatibilityManager.cs:41)

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

MasterShipGenerator: Loading resources...
UnityEngine.Debug:Log (object)
MasterShipGenerator:LoadAllResources () (at Assets/MasterShipGenerator.cs:204)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:102)

MasterShipGenerator: Using ContentManager to load resources
UnityEngine.Debug:Log (object)
MasterShipGenerator:LoadAllResources () (at Assets/MasterShipGenerator.cs:209)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:102)

MasterShipGenerator: Generating encounters for day 1, using 5 access codes
UnityEngine.Debug:Log (object)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:491)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting ship video for type: Battleship, name: Supreme
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Found ship video from ScriptableObject: ShipTest4
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:439)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Ship_Battleship_Supreme -> ShipTest4
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:444)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting captain video for faction: Imperium, name: Military One, rank: Flight Officer, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Found captain video from ScriptableObject: TempCap_MilitaryTransport
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Captain_Imperium_greeting_Military One -> TempCap_MilitaryTransport
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

No valid scenarios found, using fallback
UnityEngine.Debug:LogWarning (object)
MasterShipGenerator:SelectScenario (bool) (at Assets/MasterShipGenerator.cs:1067)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:558)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting ship video for type: Cruiser, name: Cruiser 364
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

No ship video found for Cruiser, using default
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:485)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Ship_Cruiser_Cruiser 364 -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting captain video for faction: TradeUnion, name: Trader Joe, rank: Representative, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Found captain video from ScriptableObject: TempCap_TradeUnion
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Captain_TradeUnion_greeting_Trader Joe -> TempCap_TradeUnion
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting ship video for type: Orion Shuttle, name: Paladin
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Found ship video from legacy data: shipTest
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:455)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Ship_Orion Shuttle_Paladin -> shipTest
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:460)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting captain video for faction: Mercenary, name: Military One, rank: Major, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Captain_Mercenary_greeting_Military One -> TempCap_MilitaryTransport
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting ship video for type: Helios Hauler, name: Helios Hauler 354
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

No ship video found for Helios Hauler, using default
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:485)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Ship_Helios Hauler_Helios Hauler 354 -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Using cached captain video for TradeUnion (greeting): TempCap_TradeUnion
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:784)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting ship video for type: Galactic Express, name: Galactic Express 196
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

No ship video found for Galactic Express, using default
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:485)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Ship_Galactic Express_Galactic Express 196 -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting captain video for faction: Imperium, name: Mortis Op, rank: Captain, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Found captain video from ScriptableObject: IMC_Greet001
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Captain_Imperium_greeting_Mortis Op -> IMC_Greet001
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting ship video for type: Atlas Conveyor, name: Atlas Conveyor 853
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

No ship video found for Atlas Conveyor, using default
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:485)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Ship_Atlas Conveyor_Atlas Conveyor 853 -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Using cached captain video for Imperium (greeting): IMC_Greet001
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:784)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting ship video for type: Super Stellar Destroyer, name: Finaliser
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Found ship video from legacy data: ShipTest4
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:455)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Ship_Super Stellar Destroyer_Finaliser -> ShipTest4
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:460)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting captain video for faction: Imperium, name: idos flek, rank: Lieutenant, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Found captain video from ScriptableObject: IMC_Greet002
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Captain_Imperium_greeting_idos flek -> IMC_Greet002
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting ship video for type: Pilgrimage Drifter, name: Grace
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

No ship video found for Pilgrimage Drifter, using default
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:485)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Ship_Pilgrimage Drifter_Grace -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Using cached captain video for Mercenary (greeting): TempCap_MilitaryTransport
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:784)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting ship video for type: Cruiser, name: Cruiser 339
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Ship_Cruiser_Cruiser 339 -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting captain video for faction: The Order, name: Jod Baran, rank: Acolyte, context: greeting
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:788)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Found captain video from ScriptableObject: TempCap_OrderPriest
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:798)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Captain_The Order_greeting_Jod Baran -> TempCap_OrderPriest
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:803)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Getting ship video for type: Monitor, name: Fimhra
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:429)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

No ship video found for Monitor, using default
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:485)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Cached video: Ship_Monitor_Fimhra -> UItest2
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:CacheVideoSelection (string,UnityEngine.Video.VideoClip) (at Assets/_scripts/StarkkillerMediaDatabase.cs:282)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetShipVideo (string,string,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:490)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:553)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Using cached captain video for Imperium (greeting): TempCap_MilitaryTransport
UnityEngine.Debug:Log (object)
StarkillerBaseCommand.StarkkillerMediaDatabase:GetCaptainVideo (string,string,string,bool,bool,bool) (at Assets/_scripts/StarkkillerMediaDatabase.cs:784)
MasterShipEncounter:EnhanceWithVideos (StarkillerBaseCommand.StarkkillerMediaDatabase) (at Assets/MasterShipEncounter.cs:556)
MasterShipGenerator:GenerateRandomEncounter (bool) (at Assets/MasterShipGenerator.cs:573)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:522)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

Generated 10 encounters for day 1 (0 story encounters)
UnityEngine.Debug:Log (object)
MasterShipGenerator:GenerateEncountersForDay (int) (at Assets/MasterShipGenerator.cs:542)
MasterShipGenerator:Start () (at Assets/MasterShipGenerator.cs:112)

MasterShipGeneratorAdapter: Connected MasterShipGenerator to CredentialChecker
UnityEngine.Debug:Log (object)
MasterShipGeneratorAdapter:Start () (at Assets/Scripts/MasterShipGeneratorAdapter.cs:52)

MasterShipGeneratorAdapter: Setup complete
UnityEngine.Debug:Log (object)
MasterShipGeneratorAdapter:Start () (at Assets/Scripts/MasterShipGeneratorAdapter.cs:64)

Initializing video players...
UnityEngine.Debug:Log (object)
CredentialChecker:InitializeVideoPlayers () (at Assets/_scripts/CredentialChecker.cs:262)
CredentialChecker:Start () (at Assets/_scripts/CredentialChecker.cs:112)

Ship video player initialized and enabled
UnityEngine.Debug:Log (object)
CredentialChecker:InitializeVideoPlayers () (at Assets/_scripts/CredentialChecker.cs:273)
CredentialChecker:Start () (at Assets/_scripts/CredentialChecker.cs:112)

Captain video player initialized and enabled
UnityEngine.Debug:Log (object)
CredentialChecker:InitializeVideoPlayers () (at Assets/_scripts/CredentialChecker.cs:286)
CredentialChecker:Start () (at Assets/_scripts/CredentialChecker.cs:112)

CredentialChecker: Successfully subscribed to OnEncounterReady event
UnityEngine.Debug:Log (object)
CredentialChecker:Start () (at Assets/_scripts/CredentialChecker.cs:175)

TestingFramework initialized successfully
UnityEngine.Debug:Log (object)
TestingFramework:Start () (at Assets/_scripts/TestingFramework.cs:82)

ShipEncounterSystem not found in scene!
UnityEngine.Debug:LogError (object)
ShipEncounterSystem:get_Instance () (at Assets/_scripts/ShipEncounterSystem.cs:66)
GameManager:ShowDailyBriefing () (at Assets/_scripts/GameManager.cs:324)
GameManager:StartDay () (at Assets/_scripts/GameManager.cs:301)
GameManager:StartGame () (at Assets/_scripts/GameManager.cs:269)
GameManager:Start () (at Assets/_scripts/GameManager.cs:169)

There are no audio listeners in the scene. Please ensure there is always one audio listener in the scene

Video references connected!
UnityEngine.Debug:Log (object)
VideoSetupHelper:Start () (at Assets/_scripts/VideoSetupHelper.cs:39)

DEBUG: Timer countdown - 40 seconds remaining
UnityEngine.Debug:Log (object)
TimerDiagnostic:Update () (at Assets/TimerDiagnostic.cs:81)

Briefing panel active state: False
UnityEngine.Debug:Log (object)
DailyBriefingDebug:CheckBriefing () (at Assets/DailyBriefingDebug.cs:28)

Forcing briefing to show for day 1
UnityEngine.Debug:Log (object)
DailyBriefingDebug:CheckBriefing () (at Assets/DailyBriefingDebug.cs:36)

Coroutine couldn't be started because the the game object 'DailyBriefingPanel' is inactive!
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
DailyBriefingManager:UpdateNewsTicker (int) (at Assets/DailyBriefingManager.cs:1134)
DailyBriefingManager:GenerateDailyContent (int) (at Assets/DailyBriefingManager.cs:851)
DailyBriefingManager:ShowDailyBriefing (int) (at Assets/DailyBriefingManager.cs:195)
DailyBriefingDebug:CheckBriefing () (at Assets/DailyBriefingDebug.cs:37)

Coroutine couldn't be started because the the game object 'DailyBriefingPanel' is inactive!
UnityEngine.MonoBehaviour:StartCoroutine (System.Collections.IEnumerator)
DailyBriefingManager:ShowDailyBriefing (int) (at Assets/DailyBriefingManager.cs:198)
DailyBriefingDebug:CheckBriefing () (at Assets/DailyBriefingDebug.cs:37)

Daily briefing complete. Setting game state to Gameplay. Current day: 1
UnityEngine.Debug:Log (object)
GameManager/<ShowBriefingThenHide>d__67:MoveNext () (at Assets/_scripts/GameManager.cs:379)
UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr) (at /Users/bokken/build/output/unity/unity/Runtime/Export/Scripting/Coroutines.cs:17)

DEBUG: Timer countdown - 35 seconds remaining
UnityEngine.Debug:Log (object)
TimerDiagnostic:Update () (at Assets/TimerDiagnostic.cs:81)

DEBUG: Timer countdown - 30 seconds remaining
UnityEngine.Debug:Log (object)
TimerDiagnostic:Update () (at Assets/TimerDiagnostic.cs:81)

Timer countdown: 00:25 remaining
UnityEngine.Debug:Log (object)
GameManager:Update () (at Assets/_scripts/GameManager.cs:216)

DEBUG: Timer countdown - 25 seconds remaining
UnityEngine.Debug:Log (object)
TimerDiagnostic:Update () (at Assets/TimerDiagnostic.cs:81)

Timer countdown: 00:20 remaining
UnityEngine.Debug:Log (object)
GameManager:Update () (at Assets/_scripts/GameManager.cs:216)

DEBUG: Timer countdown - 20 seconds remaining
UnityEngine.Debug:Log (object)
TimerDiagnostic:Update () (at Assets/TimerDiagnostic.cs:81)

Timer countdown: 00:15 remaining
UnityEngine.Debug:Log (object)
GameManager:Update () (at Assets/_scripts/GameManager.cs:216)

DEBUG: Timer countdown - 15 seconds remaining
UnityEngine.Debug:Log (object)
TimerDiagnostic:Update () (at Assets/TimerDiagnostic.cs:81)

Timer countdown: 00:10 remaining
UnityEngine.Debug:Log (object)
GameManager:Update () (at Assets/_scripts/GameManager.cs:216)

DEBUG: Timer countdown - 10 seconds remaining
UnityEngine.Debug:Log (object)
TimerDiagnostic:Update () (at Assets/TimerDiagnostic.cs:81)

Timer countdown: 00:05 remaining
UnityEngine.Debug:Log (object)
GameManager:Update () (at Assets/_scripts/GameManager.cs:216)

DEBUG: Timer countdown - 5 seconds remaining
UnityEngine.Debug:Log (object)
TimerDiagnostic:Update () (at Assets/TimerDiagnostic.cs:81)

DEBUG: Timer about to expire!
UnityEngine.Debug:Log (object)
TimerDiagnostic:Update () (at Assets/TimerDiagnostic.cs:87)

Timer expired: 0.009689988 seconds - Calling EndShift()
UnityEngine.Debug:Log (object)
GameManager:Update () (at Assets/_scripts/GameManager.cs:223)

EndShift called - Current game state: Gameplay
UnityEngine.Debug:Log (object)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:175)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

EndShift: Calculated salary: 50, Ships processed: 0/8
UnityEngine.Debug:Log (object)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:187)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

ShowDailyReport called with salary: 50
UnityEngine.Debug:Log (object)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1009)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

Total expenses calculated: 85
UnityEngine.Debug:Log (object)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1022)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

Got family status from familySystem
UnityEngine.Debug:Log (object)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1036)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

Using DailyReportManager to show the report
UnityEngine.Debug:Log (object)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1047)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

DailyReportManager.ShowDailyReport called
UnityEngine.Debug:Log (object)
DailyReportManager:ShowDailyReport (int,int,int,int,int,int,int,int,int,FamilyStatusInfo,System.Collections.Generic.Dictionary`2<string, int>) (at Assets/_scripts/DailyReportManager.cs:85)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1049)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

DailyReportPanel activated
UnityEngine.Debug:Log (object)
DailyReportManager:ShowDailyReport (int,int,int,int,int,int,int,int,int,FamilyStatusInfo,System.Collections.Generic.Dictionary`2<string, int>) (at Assets/_scripts/DailyReportManager.cs:105)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1049)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

Continue button is assigned and should be active
UnityEngine.Debug:Log (object)
DailyReportManager:ShowDailyReport (int,int,int,int,int,int,int,int,int,FamilyStatusInfo,System.Collections.Generic.Dictionary`2<string, int>) (at Assets/_scripts/DailyReportManager.cs:190)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1049)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

DailyReportManager.ShowDailyReport UI update completed successfully
UnityEngine.Debug:Log (object)
DailyReportManager:ShowDailyReport (int,int,int,int,int,int,int,int,int,FamilyStatusInfo,System.Collections.Generic.Dictionary`2<string, int>) (at Assets/_scripts/DailyReportManager.cs:205)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1049)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

Updated credits: -5 (previous + salary - expenses)
UnityEngine.Debug:Log (object)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1100)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

Credits below zero, triggering GameOver
UnityEngine.Debug:Log (object)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1112)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

GAME OVER! Your family has been reassigned to a remote outpost due to financial mismanagement.
UnityEngine.Debug:Log (object)
GameManager:GameOver (string) (at Assets/_scripts/GameManager.cs:1234)
GameManager:ShowDailyReport (int) (at Assets/_scripts/GameManager.cs:1113)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:190)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

EndShift completed - Day ended successfully
UnityEngine.Debug:Log (object)
GameManager:EndShift () (at Assets/_scripts/GameManager.cs:196)
GameManager:Update () (at Assets/_scripts/GameManager.cs:225)

