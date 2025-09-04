using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;
using System;
using System.IO;
using System.Reflection;
using System.Collections;
using UnityEngine.Video;
using System.Linq;
using StarkillerBaseCommand; // Add namespace for ManagerInitializer

/// <summary>
/// Monitors and logs game events to help with debugging and providing 
/// visibility into game state for external discussions.
/// Enhanced to track missing references, data flow issues, and UI state.
/// </summary>
public class DebugMonitor : MonoBehaviour
{
    [Header("UI References")]
    public GameObject debugPanel;
    public TMP_Text debugLogText;
    public Button copyButton;
    public Button clearButton;
    public Toggle autoScrollToggle;
    
    [Header("Log Settings")]
    public bool logToFile = true;
    public string logFileName = "debug_log.txt";
    public int maxDisplayLines = 100;
    public bool showTimestamps = true;
    
    [Header("Filter Settings")]
    public bool showGameState = true;
    public bool showEncounters = true;
    public bool showDecisions = true;
    public bool showConsequences = true;
    public bool showErrors = true;
    public bool showDataFlow = true;
    public bool showReferences = true;
    public bool showUI = true;
    
    [Header("System Monitoring")]
    public bool monitorReferences = true;
    public float systemCheckInterval = 10f; // Seconds between system checks
    public bool trackHoldingPatternIssues = true;
    public bool trackVideoPlayerIssues = true;
    public bool trackGameStateIssues = true;
    public bool trackConsequencePopups = true;
    
    // Static instance for singleton pattern
    private static DebugMonitor _instance;
    public static DebugMonitor Instance => _instance;
    
    // List of recent log entries
    private List<string> logEntries = new List<string>();
    private StringBuilder logBuffer = new StringBuilder();
    
    // File writer for logging to disk
    private StreamWriter logWriter;
    
    // Toggle state
    private bool isPanelVisible = false;
    
    // Key system references to monitor
    private GameManager gameManager;
    private MasterShipGenerator shipGenerator;
    private CredentialChecker credentialChecker;
    private ConsequenceManager consequenceManager;
    private HoldingPatternProcessor holdingPatternProcessor;
    
    // Reference validation tracking
    private Dictionary<string, bool> referenceStatus = new Dictionary<string, bool>();
    
    // Data flow tracking
    private Dictionary<string, System.Object> lastDataValues = new Dictionary<string, System.Object>();
    
    // UI tracking
    private bool wasConsequencePanelOpen = false;
    private int consequencePanelOpenCount = 0;
    private float consequencePanelOpenTime = 0f;
    private GameObject consequencePanel = null;
    
    private Dictionary<string, float> encounterDisplayTimes = new Dictionary<string, float>();
    private Queue<EncounterTimingRecord> recentEncounters = new Queue<EncounterTimingRecord>();
    private int maxEncounterRecords = 20;
    
    /// <summary>
    /// Track the timing of encounters displayed to the player  
    /// </summary>
    private class EncounterTimingRecord
    {
        public string ShipType { get; set; }
        public string ShipName { get; set; }
        public string AccessCode { get; set; }
        public string SourceMethod { get; set; }
        public float DisplayTime { get; set; }
        public float LifespanSeconds { get; set; } = -1f;
        public string OverwrittenBy { get; set; } = null;
        
        public override string ToString()
        {
            string status = LifespanSeconds >= 0 ? 
                $"Displayed for {LifespanSeconds:F1}s" : 
                "Active";
                
            if (!string.IsNullOrEmpty(OverwrittenBy))
            {
                status += $", Overwritten by {OverwrittenBy}";
            }
            
            return $"{ShipType} ({ShipName}) - AC:{AccessCode} - Source:{SourceMethod} - {status}";
        }
    }

    void Awake()
    {
        // Setup singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        
        // Check if we're a root GameObject
        if (transform.parent == null)
        {
            // Make this object persistent across scene loads
            DontDestroyOnLoad(gameObject);
            Debug.Log("DebugMonitor marked as persistent with DontDestroyOnLoad");
        }
        else
        {
            // Check if our parent has a ManagerInitializer component
            ManagerInitializer managerInit = GetComponentInParent<ManagerInitializer>();
            if (managerInit == null)
            {
                Debug.LogWarning("DebugMonitor: Not at root level and no ManagerInitializer found. This may cause persistence issues.");
            }
            else
            {
                Debug.Log("DebugMonitor: Found ManagerInitializer parent - persistence will be handled by it");
            }
        }
        
        // Initialize UI
        if (debugPanel) debugPanel.SetActive(false);
        
        // Set up button listeners
        if (copyButton) copyButton.onClick.AddListener(CopyToClipboard);
        if (clearButton) clearButton.onClick.AddListener(ClearLog);
        
        // Set up file logging
        if (logToFile)
        {
            try
            {
                logWriter = new StreamWriter(logFileName, true);
                logWriter.WriteLine("\n\n--- DEBUG LOG SESSION STARTED " + DateTime.Now + " ---\n");
                logWriter.Flush();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to initialize log file: " + e.Message);
                logToFile = false;
            }
        }
        
        // Find key system references
        FindAndMonitorSystemReferences();
        
        // Start periodic system check
        if (monitorReferences)
        {
            StartCoroutine(PeriodicSystemCheck());
        }
        
        // Start UI monitors
        if (trackConsequencePopups)
        {
            StartCoroutine(MonitorConsequencePopup());
        }
    }
    
    void Start()
    {
        // Hook into important system events after all objects are initialized
        ConnectToSystemEvents();
        
        // Perform initial validation
        if (monitorReferences)
        {
            ValidateAllReferences();
        }
        
        // Create a report button
        if (debugPanel)
        {
            Button reportButton = debugPanel.GetComponentsInChildren<Button>()
                .FirstOrDefault(b => b.name == "TimingReportButton");
            
            if (reportButton == null)
            {
                // Find the clear button to clone it
                Button templateButton = clearButton;
                if (templateButton != null)
                {
                    // Clone the button
                    GameObject newButtonObj = Instantiate(templateButton.gameObject, templateButton.transform.parent);
                    newButtonObj.name = "TimingReportButton";
                    
                    // Position it
                    RectTransform rect = newButtonObj.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        Vector2 pos = rect.anchoredPosition;
                        pos.y -= 40; // Position below the clear button
                        rect.anchoredPosition = pos;
                    }
                    
                    // Set the text
                    TMP_Text buttonText = newButtonObj.GetComponentInChildren<TMP_Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = "Timing Report";
                    }
                    
                    // Set the click handler
                    Button button = newButtonObj.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(ShowEncounterTimingReport);
                    }
                }
            }
        }
    }
    
    void OnDestroy()
    {
        if (logWriter != null)
        {
            logWriter.WriteLine("\n--- DEBUG LOG SESSION ENDED " + DateTime.Now + " ---");
            logWriter.Close();
        }
    }
    
    void Update()
    {
        // Toggle debug panel with F12 key
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ToggleDebugPanel();
        }
    }

    /// <summary>
    /// Find and store references to key game systems
    /// </summary>
    private void FindAndMonitorSystemReferences()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        credentialChecker = FindFirstObjectByType<CredentialChecker>();
        consequenceManager = FindFirstObjectByType<ConsequenceManager>();
        holdingPatternProcessor = FindFirstObjectByType<HoldingPatternProcessor>();

        // Look for the consequence panel
        if (consequenceManager != null)
        {
            // Try to find the consequence report panel using reflection
            var panelField = consequenceManager.GetType().GetField("consequenceReportPanel",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (panelField != null)
            {
                consequencePanel = panelField.GetValue(consequenceManager) as GameObject;
                LogReferences("ConsequenceReportPanel", consequencePanel != null);
            }
            else
            {
                // Fallback: try to find a panel with a name that seems related to consequences
                GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                foreach (var obj in allObjects)
                {
                    if (obj.name.ToLower().Contains("consequence") && obj.name.ToLower().Contains("panel"))
                    {
                        consequencePanel = obj;
                        LogReferences("ConsequenceReportPanel (found by name)", true);
                        break;
                    }
                }
            }
        }

        // Log what we found
        LogReferences("GameManager", gameManager != null);
        LogReferences("MasterShipGenerator", shipGenerator != null);
        LogReferences("CredentialChecker", credentialChecker != null);
        LogReferences("ConsequenceManager", consequenceManager != null);
        LogReferences("HoldingPatternProcessor", holdingPatternProcessor != null);
    }
    
    /// <summary>
    /// Connect to important system events to monitor data flow
    /// </summary>
    private void ConnectToSystemEvents()
    {
        // Connect to MasterShipGenerator's encounter event
        if (shipGenerator != null)
        {
            shipGenerator.OnEncounterReady += OnEncounterGenerated;
        }
        
        // Add additional event connections here as needed
    }
    
    /// <summary>
    /// Event handler for when an encounter is generated
    /// </summary>
    private void OnEncounterGenerated(MasterShipEncounter encounter)
    {
        if (encounter == null)
        {
            LogDataFlow("NULL ENCOUNTER generated by MasterShipGenerator!", LogLevel.Error);
            return;
        }
        
        LogDataFlow($"Encounter generated: {encounter.shipType} - {encounter.captainName}");
        
        // Monitor specific data points
        if (string.IsNullOrEmpty(encounter.shipType))
            LogDataFlow("Warning: Encounter has empty shipType", LogLevel.Warning);
            
        if (string.IsNullOrEmpty(encounter.captainName))
            LogDataFlow("Warning: Encounter has empty captainName", LogLevel.Warning);
            
        if (string.IsNullOrEmpty(encounter.accessCode))
            LogDataFlow("Warning: Encounter has empty accessCode", LogLevel.Warning);
    }
    
    /// <summary>
    /// Periodic check of system health and references
    /// </summary>
    private IEnumerator PeriodicSystemCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(systemCheckInterval);
            
            // Check main references again (in case they changed)
            ValidateAllReferences();
            
            // Check specific systems based on settings
            if (trackHoldingPatternIssues)
                CheckHoldingPatternSystem();
                
            if (trackVideoPlayerIssues)
                CheckVideoPlayerSystem();
                
            if (trackGameStateIssues)
                CheckGameStateSystem();
        }
    }
    
    /// <summary>
    /// Monitor the consequence popup panel visibility
    /// </summary>
    private IEnumerator MonitorConsequencePopup()
    {
        // Keep checking every frame
        while (true)
        {
            if (consequencePanel != null)
            {
                bool isOpen = consequencePanel.activeSelf;
                
                // If the state has changed
                if (isOpen != wasConsequencePanelOpen)
                {
                    if (isOpen)
                    {
                        // Panel was just opened
                        consequencePanelOpenCount++;
                        consequencePanelOpenTime = Time.time;
                        LogUI($"Consequence panel OPENED. Total opens: {consequencePanelOpenCount}");
                        
                        // Try to get the title and content
                        TMP_Text titleText = null;
                        TMP_Text bodyText = null;
                        
                        // Find the text components by name or by component
                        Transform titleTransform = consequencePanel.transform.Find("TitleText");
                        Transform bodyTransform = consequencePanel.transform.Find("BodyText");
                        
                        if (titleTransform != null)
                            titleText = titleTransform.GetComponent<TMP_Text>();
                            
                        if (bodyTransform != null)
                            bodyText = bodyTransform.GetComponent<TMP_Text>();
                            
                        // If not found by name, try to find by type (there may be only a few TMP_Text components)
                        if (titleText == null || bodyText == null)
                        {
                            TMP_Text[] texts = consequencePanel.GetComponentsInChildren<TMP_Text>();
                            if (texts.Length > 0)
                                titleText = texts[0]; // Assume first text is title
                            if (texts.Length > 1)
                                bodyText = texts[1]; // Assume second text is body
                        }
                        
                        // Log the content if found
                        if (titleText != null)
                            LogUI($"Consequence title: {titleText.text}");
                            
                        if (bodyText != null)
                        {
                            // Get a brief excerpt of the body text
                            string bodyExcerpt = bodyText.text;
                            if (bodyExcerpt.Length > 100)
                                bodyExcerpt = bodyExcerpt.Substring(0, 100) + "...";
                                
                            LogUI($"Consequence body: {bodyExcerpt}");
                        }
                    }
                    else
                    {
                        // Panel was just closed
                        float openDuration = Time.time - consequencePanelOpenTime;
                        LogUI($"Consequence panel CLOSED after {openDuration:F1} seconds");
                    }
                    
                    wasConsequencePanelOpen = isOpen;
                }
            }
            else if (trackConsequencePopups)
            {
                // Try to find the consequence panel again if we lost it
                if (consequenceManager != null)
                {
                    var panelField = consequenceManager.GetType().GetField("consequenceReportPanel", 
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        
                    if (panelField != null)
                    {
                        consequencePanel = panelField.GetValue(consequenceManager) as GameObject;
                        if (consequencePanel != null)
                            LogReferences("ConsequenceReportPanel (re-found)", true);
                    }
                }
            }
            
            yield return null; // Wait for next frame
        }
    }
    
    /// <summary>
    /// Validate all key system references
    /// </summary>
    private void ValidateAllReferences()
    {
        // Re-check main system references
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
            
        if (shipGenerator == null)
            shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            
        if (credentialChecker == null)
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            
        if (consequenceManager == null)
            consequenceManager = FindFirstObjectByType<ConsequenceManager>();
            
        if (holdingPatternProcessor == null)
            holdingPatternProcessor = FindFirstObjectByType<HoldingPatternProcessor>();
            
        // Log updated status
        LogReferences("GameManager", gameManager != null);
        LogReferences("MasterShipGenerator", shipGenerator != null);
        LogReferences("CredentialChecker", credentialChecker != null);
        LogReferences("ConsequenceManager", consequenceManager != null);
        LogReferences("HoldingPatternProcessor", holdingPatternProcessor != null);
    }
    
    /// <summary>
    /// Check the holding pattern system for issues
    /// </summary>
    private void CheckHoldingPatternSystem()
    {
        if (holdingPatternProcessor == null)
            return;
            
        // Check for specific references using reflection to avoid compile errors
        // if fields are private or the structure changes
        bool missingContainer = false;
        
        try {
            var containerField = holdingPatternProcessor.GetType().GetField("holdingPatternEntriesContainer", 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                
            if (containerField != null)
            {
                var container = containerField.GetValue(holdingPatternProcessor) as Transform;
                missingContainer = container == null;
                
                if (missingContainer)
                {
                    LogReferences("HoldingPatternEntriesContainer is NULL", false);
                }
            }
        }
        catch (Exception e)
        {
            LogError($"Error checking HoldingPattern references: {e.Message}");
        }
        
        // Check if the system has any entries
        try {
            var entriesField = holdingPatternProcessor.GetType().GetField("activeEntries", 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                
            if (entriesField != null)
            {
                var entries = entriesField.GetValue(holdingPatternProcessor) as IList;
                int entryCount = entries?.Count ?? 0;
                
                // We don't log this as an error because it's normal to have zero entries
                LogDataFlow($"HoldingPattern has {entryCount} active entries");
            }
        }
        catch (Exception e)
        {
            LogError($"Error checking HoldingPattern entries: {e.Message}");
        }
    }
    
    /// <summary>
    /// Check video player systems for issues
    /// </summary>
    private void CheckVideoPlayerSystem()
    {
        if (credentialChecker == null)
            return;
            
        // Check for video player issues
        try {
            var shipPlayerField = credentialChecker.GetType().GetField("shipVideoPlayer", 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var captainPlayerField = credentialChecker.GetType().GetField("captainVideoPlayer", 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                
            if (shipPlayerField != null)
            {
                var shipPlayer = shipPlayerField.GetValue(credentialChecker) as VideoPlayer;
                bool shipPlayerMissing = shipPlayer == null;
                
                if (shipPlayerMissing)
                {
                    LogReferences("ShipVideoPlayer is NULL", false);
                }
                else if (!shipPlayer.enabled)
                {
                    LogReferences("ShipVideoPlayer is disabled", false);
                }
            }
            
            if (captainPlayerField != null)
            {
                var captainPlayer = captainPlayerField.GetValue(credentialChecker) as VideoPlayer;
                bool captainPlayerMissing = captainPlayer == null;
                
                if (captainPlayerMissing)
                {
                    LogReferences("CaptainVideoPlayer is NULL", false);
                }
                else if (!captainPlayer.enabled)
                {
                    LogReferences("CaptainVideoPlayer is disabled", false);
                }
            }
        }
        catch (Exception e)
        {
            LogError($"Error checking VideoPlayer references: {e.Message}");
        }
    }
    
    /// <summary>
    /// Check game state system for issues
    /// </summary>
    private void CheckGameStateSystem()
    {
        try {
            GameStateController controller = FindFirstObjectByType<GameStateController>();
            
            if (controller != null)
            {
                // Use reflection to get the current state
                var stateField = controller.GetType().GetField("currentState", 
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    
                if (stateField != null)
                {
                    var state = stateField.GetValue(controller).ToString();
                    LogGameState($"Current GameState: {state}");
                }
                
                // Check for game activation - to spot inconsistencies
                var gameActiveMethod = controller.GetType().GetMethod("IsGameplayActive");
                if (gameActiveMethod != null)
                {
                    bool isActive = (bool)gameActiveMethod.Invoke(controller, null);
                    
                    if (isActive)
                    {
                        // Check if GameManager also thinks the game is active
                        if (gameManager != null)
                        {
                            var gmActiveMethod = gameManager.GetType().GetMethod("IsGameActive");
                            if (gmActiveMethod != null)
                            {
                                bool gmActive = (bool)gmActiveMethod.Invoke(gameManager, null);
                                
                                if (!gmActive)
                                {
                                    LogGameState("INCONSISTENCY: GameStateController says gameplay is active but GameManager says it's not!", LogLevel.Warning);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                LogReferences("GameStateController", false);
            }
        }
        catch (Exception e)
        {
            LogError($"Error checking GameState: {e.Message}");
        }
    }
    
    /// <summary>
    /// Log a reference status change
    /// </summary>
    private void LogReferences(string referenceName, bool isPresent)
    {
        // Check if status changed since last check
        if (referenceStatus.TryGetValue(referenceName, out bool previousStatus))
        {
            if (previousStatus != isPresent)
            {
                // Status changed
                string message = isPresent ? 
                    $"{referenceName} reference is now AVAILABLE" : 
                    $"{referenceName} reference is now MISSING";
                    
                LogEvent("References", message, isPresent ? LogLevel.Info : LogLevel.Warning);
            }
        }
        else
        {
            // First time checking this reference
            string message = isPresent ? 
                $"{referenceName} reference found" : 
                $"{referenceName} reference missing";
                
            LogEvent("References", message, isPresent ? LogLevel.Info : LogLevel.Warning);
        }
        
        // Update status
        referenceStatus[referenceName] = isPresent;
    }

    /// <summary>
    /// Track when an encounter is displayed to the player
    /// </summary>
    public void LogEncounterDisplayed(MasterShipEncounter encounter, string sourceMethod)
    {
        if (encounter == null)
        {
            LogError("Attempted to display NULL encounter!");
            return;
        }
        
        string shipKey = $"{encounter.shipType}_{encounter.shipName}_{encounter.accessCode}";
        float currentTime = Time.time;
        
        // Check if we're overwriting a recent encounter
        if (encounterDisplayTimes.TryGetValue(shipKey, out float lastDisplayTime))
        {
            // This is the same ship displayed again - likely not an overwrite but reuse
            float timeSinceLastDisplay = currentTime - lastDisplayTime;
            LogEncounter($"Re-displaying ship {encounter.shipType} after {timeSinceLastDisplay:F1}s", 
                timeSinceLastDisplay < 1f ? LogLevel.Warning : LogLevel.Info);
        }
        
        // Log any current encounter that might be getting overwritten
        EncounterTimingRecord currentActive = recentEncounters.Count > 0 ? 
            recentEncounters.LastOrDefault(r => r.LifespanSeconds < 0) : null;
            
        if (currentActive != null && currentActive.ShipType != encounter.shipType)
        {
            // We have an active encounter that's being replaced by a different one
            float lifespan = currentTime - currentActive.DisplayTime;
            currentActive.LifespanSeconds = lifespan;
            currentActive.OverwrittenBy = $"{encounter.shipType} ({encounter.shipName})";
            
            // Log a warning if the encounter was short-lived (overwritten quickly)
            if (lifespan < 5.0f)
            {
                LogEncounter($"WARNING: Encounter {currentActive.ShipType} overwritten after only {lifespan:F1} seconds!" +
                            $" New encounter: {encounter.shipType}", LogLevel.Warning);
            }
        }
        
        // Record this new encounter
        encounterDisplayTimes[shipKey] = currentTime;
        
        EncounterTimingRecord record = new EncounterTimingRecord
        {
            ShipType = encounter.shipType,
            ShipName = encounter.shipName,
            AccessCode = encounter.accessCode,
            SourceMethod = sourceMethod,
            DisplayTime = currentTime
        };
        
        recentEncounters.Enqueue(record);
        
        // Trim if we exceed the max
        while (recentEncounters.Count > maxEncounterRecords)
        {
            recentEncounters.Dequeue();
        }
        
        LogEncounter($"Displayed encounter: {encounter.shipType} - {encounter.shipName} from {sourceMethod}");
    }

    /// <summary>
    /// Generate a report specifically about encounter timing
    /// </summary>
    public string GenerateEncounterTimingReport()
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("=== ENCOUNTER TIMING REPORT ===");
        report.AppendLine($"Generated at: {DateTime.Now}");
        report.AppendLine($"Current game time: {Time.time:F1}s");
        report.AppendLine();
        
        report.AppendLine("RECENT ENCOUNTERS (newest to oldest):");
        int count = 1;
        foreach (var record in recentEncounters.Reverse())
        {
            report.AppendLine($"{count++}. {record}");
        }
        
        report.AppendLine();
        report.AppendLine("ENCOUNTER OVERWRITE ANALYSIS:");
        
        // Find potential problem patterns
        var shortLivedEncounters = recentEncounters.Where(r => r.LifespanSeconds > 0 && r.LifespanSeconds < 6.0f).ToList();
        var sourceMethods = recentEncounters.GroupBy(r => r.SourceMethod).ToDictionary(g => g.Key, g => g.Count());
        
        report.AppendLine($"Short-lived encounters (<6s): {shortLivedEncounters.Count}");
        if (shortLivedEncounters.Count > 0)
        {
            report.AppendLine("  Details:");
            foreach (var record in shortLivedEncounters)
            {
                report.AppendLine($"  - {record.ShipType} displayed for {record.LifespanSeconds:F1}s, " +
                                $"from {record.SourceMethod}, overwritten by {record.OverwrittenBy}");
            }
        }
        
        report.AppendLine();
        report.AppendLine("SOURCE METHOD DISTRIBUTION:");
        foreach (var source in sourceMethods)
        {
            report.AppendLine($"  - {source.Key}: {source.Value} encounters");
        }
        
        // Look for potential timing patterns in source methods
        report.AppendLine();
        report.AppendLine("TIMING PATTERNS:");
        var timeBetweenSameSources = new Dictionary<string, List<float>>();
        
        var groupedBySource = recentEncounters.GroupBy(r => r.SourceMethod).ToDictionary(g => g.Key, g => g.ToList());
        foreach (var sourceGroup in groupedBySource)
        {
            var sortedByTime = sourceGroup.Value.OrderBy(r => r.DisplayTime).ToList();
            var timeDiffs = new List<float>();
            
            for (int i = 1; i < sortedByTime.Count; i++)
            {
                timeDiffs.Add(sortedByTime[i].DisplayTime - sortedByTime[i-1].DisplayTime);
            }
            
            if (timeDiffs.Count > 0)
            {
                timeBetweenSameSources[sourceGroup.Key] = timeDiffs;
                
                report.AppendLine($"  - {sourceGroup.Key}:");
                report.AppendLine($"    Avg time between calls: {timeDiffs.Average():F2}s");
                report.AppendLine($"    Min time between calls: {timeDiffs.Min():F2}s");
                report.AppendLine($"    Max time between calls: {timeDiffs.Max():F2}s");
            }
        }
        
        // Check for race condition patterns between sources
        report.AppendLine();
        report.AppendLine("POTENTIAL RACE CONDITIONS:");
        bool foundRaceConditions = false;
        
        foreach (var source1 in groupedBySource.Keys)
        {
            foreach (var source2 in groupedBySource.Keys.Where(k => k != source1))
            {
                var records1 = groupedBySource[source1];
                var records2 = groupedBySource[source2];
                
                // Look for close timing between different sources
                foreach (var r1 in records1)
                {
                    var closeRecords = records2.Where(r2 => 
                        Math.Abs(r2.DisplayTime - r1.DisplayTime) < 5.0f).ToList();
                        
                    if (closeRecords.Any())
                    {
                        foundRaceConditions = true;
                        report.AppendLine($"  - Found potential race between {source1} and {source2}:");
                        
                        foreach (var r2 in closeRecords)
                        {
                            float timeDiff = Math.Abs(r2.DisplayTime - r1.DisplayTime);
                            report.AppendLine($"    * {r1.ShipType} and {r2.ShipType} within {timeDiff:F2}s of each other");
                        }
                    }
                }
            }
        }
        
        if (!foundRaceConditions)
        {
            report.AppendLine("  No obvious race conditions detected");
        }
        
        return report.ToString();
    }

    /// <summary>
    /// Add UI button to generate an encounter timing report
    /// </summary>
    public void ShowEncounterTimingReport()
    {
        string report = GenerateEncounterTimingReport();
        Debug.Log(report);
        
        // Copy to clipboard for easy sharing
        GUIUtility.systemCopyBuffer = report;
        LogSystem("Encounter timing report copied to clipboard");
        
        // You could also display this in a UI panel if needed
    }
    
    /// <summary>
    /// Toggle the debug panel visibility
    /// </summary>
    public void ToggleDebugPanel()
    {
        if (debugPanel)
        {
            isPanelVisible = !isPanelVisible;
            debugPanel.SetActive(isPanelVisible);

            // Update the displayed text
            if (isPanelVisible && debugLogText)
            {
                UpdateDisplayText();
            }
        }
    }
    
    /// <summary>
    /// Log an event to the debug monitor
    /// </summary>
    public void LogEvent(string category, string message, LogLevel level = LogLevel.Info)
    {
        // Skip if category is filtered
        if (!ShouldLogCategory(category))
            return;
            
        // Format the log entry
        string timestamp = showTimestamps ? DateTime.Now.ToString("[HH:mm:ss.fff] ") : "";
        string logEntry = $"{timestamp}[{category}] {message}";
        
        // Add to memory buffer
        logEntries.Add(logEntry);
        
        // Trim if we exceed max lines
        if (logEntries.Count > maxDisplayLines)
        {
            logEntries.RemoveAt(0);
        }
        
        // Update file log
        if (logToFile && logWriter != null)
        {
            logWriter.WriteLine(logEntry);
            logWriter.Flush();
        }
        
        // Update UI if visible
        if (isPanelVisible && debugLogText)
        {
            UpdateDisplayText();
        }
        
        // Also log to Unity console with appropriate level
        switch (level)
        {
            case LogLevel.Warning:
                Debug.LogWarning(logEntry);
                break;
            case LogLevel.Error:
                Debug.LogError(logEntry);
                break;
            default:
                Debug.Log(logEntry);
                break;
        }
    }
    
    /// <summary>
    /// Check if a category should be logged based on filter settings
    /// </summary>
    private bool ShouldLogCategory(string category)
    {
        if (category == "GameState" && !showGameState) return false;
        if (category == "Encounter" && !showEncounters) return false;
        if (category == "Decision" && !showDecisions) return false;
        if (category == "Consequence" && !showConsequences) return false;
        if (category == "Error" && !showErrors) return false;
        if (category == "DataFlow" && !showDataFlow) return false;
        if (category == "References" && !showReferences) return false;
        if (category == "UI" && !showUI) return false;
        
        return true;
    }
    
    /// <summary>
    /// Update the displayed text in the UI
    /// </summary>
    private void UpdateDisplayText()
    {
        if (debugLogText == null)
            return;
            
        // Build the text from log entries
        logBuffer.Clear();
        foreach (string entry in logEntries)
        {
            logBuffer.AppendLine(entry);
        }
        
        debugLogText.text = logBuffer.ToString();
        
        // Auto-scroll to bottom if enabled
        if (autoScrollToggle && autoScrollToggle.isOn)
        {
            Canvas.ForceUpdateCanvases();
            ScrollRect scrollRect = debugLogText.GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f; // Scroll to bottom
            }
        }
    }
    
    /// <summary>
    /// Copy the current log to clipboard
    /// </summary>
    public void CopyToClipboard()
    {
        // Build the text from log entries
        logBuffer.Clear();
        foreach (string entry in logEntries)
        {
            logBuffer.AppendLine(entry);
        }
        
        GUIUtility.systemCopyBuffer = logBuffer.ToString();
        LogEvent("System", "Log copied to clipboard", LogLevel.Info);
    }
    
    /// <summary>
    /// Clear the current log
    /// </summary>
    public void ClearLog()
    {
        logEntries.Clear();
        if (debugLogText) debugLogText.text = "";
        LogEvent("System", "Log cleared", LogLevel.Info);
    }
    
    /// <summary>
    /// Log a message for system-related events
    /// </summary>
    private void LogSystem(string message, LogLevel level = LogLevel.Info)
    {
        LogEvent("System", message, level);
    }
    
    // Convenience methods for common log categories
    
    /// <summary>
    /// Log a game state change
    /// </summary>
    public void LogGameState(string message, LogLevel level = LogLevel.Info)
    {
        LogEvent("GameState", message, level);
    }
    
    /// <summary>
    /// Log an encounter event
    /// </summary>
    public void LogEncounter(string message, LogLevel level = LogLevel.Info)
    {
        LogEvent("Encounter", message, level);
    }
    
    /// <summary>
    /// Log a decision event
    /// </summary>
    public void LogDecision(string message, LogLevel level = LogLevel.Info)
    {
        LogEvent("Decision", message, level);
    }
    
    /// <summary>
    /// Log a consequence event
    /// </summary>
    public void LogConsequence(string message, LogLevel level = LogLevel.Info)
    {
        LogEvent("Consequence", message, level);
    }
    
    /// <summary>
    /// Log a data flow event
    /// </summary>
    public void LogDataFlow(string message, LogLevel level = LogLevel.Info)
    {
        LogEvent("DataFlow", message, level);
    }
    
    /// <summary>
    /// Log a UI event
    /// </summary>
    public void LogUI(string message, LogLevel level = LogLevel.Info)
    {
        LogEvent("UI", message, level);
    }
    
    /// <summary>
    /// Log an error event
    /// </summary>
    public void LogError(string message)
    {
        LogEvent("Error", message, LogLevel.Error);
    }
    
    /// <summary>
    /// Track a value passing between systems
    /// </summary>
    public void TrackDataValue(string dataName, System.Object value)
    {
        // Skip tracking if value is the same
        if (lastDataValues.TryGetValue(dataName, out var lastValue) && 
            (lastValue == value || (lastValue != null && lastValue.Equals(value))))
        {
            return;
        }
        
        // Update stored value
        lastDataValues[dataName] = value;
        
        // Log the change
        string valueStr = value?.ToString() ?? "NULL";
        LogDataFlow($"Data '{dataName}' changed to: {valueStr}");
    }
    
    /// <summary>
    /// Log levels for formatting in console
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
    
    /// <summary>
    /// Add hooks to various methods in the game to monitor their execution
    /// </summary>
    public void AddSystemHooks()
    {
        if (credentialChecker != null)
        {
            // Use reflection to get the private processing flags
            var processingField = credentialChecker.GetType().GetField("isProcessingDecision",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var animationField = credentialChecker.GetType().GetField("isInDecisionAnimation",
                BindingFlags.Instance | BindingFlags.NonPublic);
                
            if (processingField != null)
            {
                bool isProcessing = (bool)processingField.GetValue(credentialChecker);
                LogDataFlow($"CredentialChecker.isProcessingDecision = {isProcessing}");
            }
            
            if (animationField != null)
            {
                bool isAnimating = (bool)animationField.GetValue(credentialChecker);
                LogDataFlow($"CredentialChecker.isInDecisionAnimation = {isAnimating}");
            }
        }
        
        if (holdingPatternProcessor != null)
        {
            // Check if the holding pattern entries container exists
            var containerField = holdingPatternProcessor.GetType().GetField("holdingPatternEntriesContainer",
                BindingFlags.Instance | BindingFlags.Public);
                
            if (containerField != null)
            {
                Transform container = containerField.GetValue(holdingPatternProcessor) as Transform;
                LogReferences("HoldingPatternEntriesContainer", container != null);
                
                if (container != null)
                {
                    LogDataFlow($"HoldingPatternEntriesContainer has {container.childCount} children");
                }
            }
        }
    }
    
    /// <summary>
    /// Generate a summary of current state for clipboard
    /// </summary>
    public string GenerateStateSummary()
    {
        StringBuilder summary = new StringBuilder();
        
        summary.AppendLine("=== STARKILLER BASE COMMAND DEBUG SUMMARY ===");
        summary.AppendLine($"Timestamp: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        summary.AppendLine();
        
        // Game state info
        summary.AppendLine("GAME STATE:");
        try
        {
            GameStateController controller = FindFirstObjectByType<GameStateController>();
            string gameState = controller != null ? 
                controller.GetType().GetField("currentState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(controller).ToString() 
                : "Unknown";
            
            summary.AppendLine($"- Current state: {gameState}");
            
            if (gameManager != null)
            {
                summary.AppendLine($"- Day: {gameManager.currentDay}");
                summary.AppendLine($"- Ships processed: {gameManager.GetShipsProcessed()}");
                summary.AppendLine($"- Strikes: {gameManager.GetStrikes()}/{gameManager.maxMistakes}");
                summary.AppendLine($"- Credits: {gameManager.GetCredits()}");
            }
        }
        catch (Exception e)
        {
            summary.AppendLine($"- Error getting game state: {e.Message}");
        }
        summary.AppendLine();
        
        // Recent incidents
        summary.AppendLine("RECENT INCIDENTS:");
        try
        {
            if (consequenceManager != null)
            {
                var incidentsField = consequenceManager.GetType().GetField("incidents", BindingFlags.Instance | BindingFlags.NonPublic);
                if (incidentsField != null)
                {
                    var incidents = incidentsField.GetValue(consequenceManager) as IList;
                    if (incidents != null && incidents.Count > 0)
                    {
                        // Show the last few incidents
                        int count = Math.Min(incidents.Count, 3);
                        for (int i = incidents.Count - count; i < incidents.Count; i++)
                        {
                            var incident = incidents[i];
                            Type incidentType = incident.GetType();
                            string title = (string)incidentType.GetField("title")?.GetValue(incident) ?? "Unknown";
                            string description = (string)incidentType.GetField("description")?.GetValue(incident) ?? "No description";
                            var dayField = incidentType.GetField("dayOccurred");
                            int day = dayField != null ? (int)dayField.GetValue(incident) : 0;
                            
                            summary.AppendLine($"- Day {day}: {title} - {description}");
                        }
                    }
                    else
                    {
                        summary.AppendLine("- No incidents recorded");
                    }
                }
            }
            else
            {
                summary.AppendLine("- ConsequenceManager not found");
            }
        }
        catch (Exception e)
        {
            summary.AppendLine($"- Error getting incidents: {e.Message}");
        }
        summary.AppendLine();
        
        // Current encounter
        summary.AppendLine("CURRENT ENCOUNTER:");
        try
        {
            if (credentialChecker != null)
            {
                var encounterField = credentialChecker.GetType().GetField("currentEncounter", 
                    BindingFlags.Instance | BindingFlags.NonPublic);
                    
                if (encounterField != null)
                {
                    var encounter = encounterField.GetValue(credentialChecker);
                    if (encounter != null)
                    {
                        Type encounterType = encounter.GetType();
                        string shipType = (string)encounterType.GetField("shipType")?.GetValue(encounter) ?? "Unknown";
                        string captainName = (string)encounterType.GetField("captainName")?.GetValue(encounter) ?? "Unknown";
                        string accessCode = (string)encounterType.GetField("accessCode")?.GetValue(encounter) ?? "Unknown";
                        bool shouldApprove = (bool)encounterType.GetField("shouldApprove")?.GetValue(encounter);
                        bool isStoryShip = (bool)encounterType.GetField("isStoryShip")?.GetValue(encounter);
                        string storyTag = (string)encounterType.GetField("storyTag")?.GetValue(encounter) ?? "None";
                        
                        summary.AppendLine($"- Ship Type: {shipType}");
                        summary.AppendLine($"- Captain: {captainName}");
                        summary.AppendLine($"- Access Code: {accessCode}");
                        summary.AppendLine($"- Should Approve: {shouldApprove}");
                        summary.AppendLine($"- Story Ship: {isStoryShip} ({storyTag})");
                    }
                    else
                    {
                        summary.AppendLine("- No active encounter");
                    }
                }
            }
            else
            {
                summary.AppendLine("- CredentialChecker not found");
            }
        }
        catch (Exception e)
        {
            summary.AppendLine($"- Error getting encounter: {e.Message}");
        }
        summary.AppendLine();
        
        // Holding pattern info
        summary.AppendLine("HOLDING PATTERN STATUS:");
        try
        {
            if (holdingPatternProcessor != null)
            {
                var entriesField = holdingPatternProcessor.GetType().GetField("activeEntries", 
                    BindingFlags.Instance | BindingFlags.NonPublic);
                    
                if (entriesField != null)
                {
                    var entries = entriesField.GetValue(holdingPatternProcessor) as IList;
                    int entryCount = entries?.Count ?? 0;
                    
                    summary.AppendLine($"- Active ships in holding pattern: {entryCount}");
                    
                    if (entryCount > 0)
                    {
                        for (int i = 0; i < entryCount; i++)
                        {
                            var entry = entries[i];
                            var getEncounterMethod = entry.GetType().GetMethod("GetEncounter");
                            
                            if (getEncounterMethod != null)
                            {
                                var encounter = getEncounterMethod.Invoke(entry, null);
                                if (encounter != null)
                                {
                                    Type encounterType = encounter.GetType();
                                    string shipType = (string)encounterType.GetField("shipType")?.GetValue(encounter) ?? "Unknown";
                                    string captainName = (string)encounterType.GetField("captainName")?.GetValue(encounter) ?? "Unknown";
                                    
                                    summary.AppendLine($"  - Ship {i+1}: {shipType} - {captainName}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    summary.AppendLine("- Unable to access active entries");
                }
                
                // Check if container exists
                var containerField = holdingPatternProcessor.GetType().GetField("holdingPatternEntriesContainer", 
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    
                if (containerField != null)
                {
                    var container = containerField.GetValue(holdingPatternProcessor) as Transform;
                    summary.AppendLine($"- Container reference: {(container != null ? "Valid" : "NULL")}");
                }
            }
            else
            {
                summary.AppendLine("- HoldingPatternProcessor not found");
            }
        }
        catch (Exception e)
        {
            summary.AppendLine($"- Error getting holding pattern info: {e.Message}");
        }
        summary.AppendLine();
        
        // Consequence popup status
        summary.AppendLine("CONSEQUENCE POPUP STATUS:");
        summary.AppendLine($"- Panel reference: {(consequencePanel != null ? "Valid" : "NULL")}");
        summary.AppendLine($"- Currently open: {wasConsequencePanelOpen}");
        summary.AppendLine($"- Open count: {consequencePanelOpenCount}");
        
        if (consequencePanelOpenTime > 0)
        {
            float openDuration = wasConsequencePanelOpen ? 
                Time.time - consequencePanelOpenTime : 0;
                
            if (wasConsequencePanelOpen)
            {
                summary.AppendLine($"- Current open duration: {openDuration:F1} seconds");
            }
            else
            {
                summary.AppendLine($"- Last open time: {consequencePanelOpenTime:F1}");
            }
        }
        summary.AppendLine();
        
        // System references
        summary.AppendLine("SYSTEM REFERENCES:");
        foreach (var reference in referenceStatus)
        {
            summary.AppendLine($"- {reference.Key}: {(reference.Value ? "Valid" : "NULL")}");
        }
        summary.AppendLine();
        
        // Recent logs
        summary.AppendLine("RECENT LOGS:");
        int logCount = Math.Min(logEntries.Count, 15); // Show last 15 entries
        for (int i = logEntries.Count - logCount; i < logEntries.Count; i++)
        {
            summary.AppendLine(logEntries[i]);
        }
        
        return summary.ToString();
    }
    
    /// <summary>
    /// Generate and copy a state summary to clipboard
    /// </summary>
    public void CopyStateSummary()
    {
        string summary = GenerateStateSummary();
        GUIUtility.systemCopyBuffer = summary;
        LogEvent("System", "State summary copied to clipboard", LogLevel.Info);
    }
}