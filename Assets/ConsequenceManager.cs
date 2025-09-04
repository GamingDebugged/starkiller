using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using StarkillerBaseCommand;
using System.Linq;
using System.Collections;
using Starkiller.Core;
using Starkiller.Core.Managers;

/// <summary>
/// Manages consequences of player decisions in Starkiller Base Command
/// Tracks incidents, generates reports, applies effects to gameplay
/// </summary>
public class ConsequenceManager : MonoBehaviour
{
    [System.Serializable]
    public class IncidentRecord
    {
        public string title;
        public string description;
        public int casualties;
        public int creditPenalty;
        public int dayOccurred;
        public bool wasReported;
        public StarkillerBaseCommand.Consequence.SeverityLevel severity;
        public StarkillerBaseCommand.Consequence.ConsequenceType type;
        public bool hasLongTermEffect;
        public int remainingDays; // For long-term effects
    }
    
    [Header("Consequence Database")]
    [SerializeField] private List<StarkillerBaseCommand.Consequence> minorConsequences = new List<StarkillerBaseCommand.Consequence>();
    [SerializeField] private List<StarkillerBaseCommand.Consequence> moderateConsequences = new List<StarkillerBaseCommand.Consequence>();
    [SerializeField] private List<StarkillerBaseCommand.Consequence> severeConsequences = new List<StarkillerBaseCommand.Consequence>();
    [SerializeField] private List<StarkillerBaseCommand.Consequence> criticalConsequences = new List<StarkillerBaseCommand.Consequence>();
    
    [Header("Special Events")]
    [SerializeField] private bool enableInspections = true;
    [SerializeField] private float baseInspectionChance = 0.1f;
    [SerializeField] private float inspectionIncreasePerIncident = 0.05f;
    [SerializeField] private int maxIncidentsBeforeInspection = 3;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject consequenceReportPanel;
    [SerializeField] private TMP_Text reportTitleText;
    [SerializeField] private TMP_Text reportBodyText;
    [SerializeField] private Button dismissReportButton;
    [SerializeField] private Image severityIconImage;
    
    [Header("Game Impact")]
    [SerializeField] private float loyaltyImpactMultiplier = 1.0f;
    [SerializeField] private float familyImpactChance = 0.3f;
    
    // Runtime tracking
    private List<IncidentRecord> incidents = new List<IncidentRecord>();
    private List<IncidentRecord> activeEffects = new List<IncidentRecord>(); // Long-term effects
    private List<ConsequenceToken> consequenceTokens = new List<ConsequenceToken>(); // New token system
    private float currentInspectionChance;
    private int unreportedIncidents = 0;
    private int currentDay = 1;
    
    // Statistics tracking
    private int totalCasualties = 0;
    private int totalCreditLoss = 0;
    private int securityIncidents = 0;
    private int familyIncidents = 0;
    
    // Reference to game systems
    private GameManager gameManager;
    private ImperialFamilySystem familySystem;
    
    // Singleton pattern for easy access
    private static ConsequenceManager _instance;
    public static ConsequenceManager Instance => _instance;
    
    void Awake()
    {
        // Setup singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Register with ServiceLocator for PersonalDataLogManager
        ServiceLocator.Register<ConsequenceManager>(this);
    }
    
    void Start()
    {
        // Find necessary system references
        gameManager = FindFirstObjectByType<GameManager>();
        familySystem = FindFirstObjectByType<ImperialFamilySystem>();
        
        // Load consequences if any lists are empty
        LoadConsequencesIfNeeded();
        
        // Setup UI
        if (dismissReportButton)
        {
            dismissReportButton.onClick.AddListener(HideConsequenceReport);
        }
        
        // Initialize inspection chance
        currentInspectionChance = baseInspectionChance;
        
        // Hide report panel initially
        if (consequenceReportPanel)
        {
            consequenceReportPanel.SetActive(false);
        }
        
        Debug.Log("ConsequenceManager initialized successfully");
    }

    /// <summary>
    /// Load consequences from Resources if collections are empty
    /// </summary>
    private void LoadConsequencesIfNeeded()
    {
        // Check if any of our collections are empty
        if (minorConsequences.Count == 0 || 
            moderateConsequences.Count == 0 ||
            severeConsequences.Count == 0 ||
            criticalConsequences.Count == 0)
        {
            Debug.Log("Loading consequences from Resources...");
            
            // Try loading from multiple possible paths
            string[] paths = new string[] {
                "Consequences",
                "ScriptableObjects/Consequences",
                "_ScriptableObjects/Consequences"
            };
            
            foreach (string path in paths)
            {
                StarkillerBaseCommand.Consequence[] loadedConsequences = 
                    Resources.LoadAll<StarkillerBaseCommand.Consequence>(path);
                
                if (loadedConsequences != null && loadedConsequences.Length > 0)
                {
                    // Sort consequences by severity
                    foreach (var consequence in loadedConsequences)
                    {
                        switch (consequence.severity)
                        {
                            case StarkillerBaseCommand.Consequence.SeverityLevel.Minor:
                                if (!minorConsequences.Contains(consequence))
                                    minorConsequences.Add(consequence);
                                break;
                                
                            case StarkillerBaseCommand.Consequence.SeverityLevel.Moderate:
                                if (!moderateConsequences.Contains(consequence))
                                    moderateConsequences.Add(consequence);
                                break;
                                
                            case StarkillerBaseCommand.Consequence.SeverityLevel.Severe:
                                if (!severeConsequences.Contains(consequence))
                                    severeConsequences.Add(consequence);
                                break;
                                
                            case StarkillerBaseCommand.Consequence.SeverityLevel.Critical:
                                if (!criticalConsequences.Contains(consequence))
                                    criticalConsequences.Add(consequence);
                                break;
                        }
                    }
                    
                    Debug.Log($"Loaded {loadedConsequences.Length} consequences from {path}");
                    
                    // If we found consequences, no need to check other paths
                    if (loadedConsequences.Length > 0)
                        break;
                }
            }
            
            // Log what we found
            Debug.Log($"Consequence counts: Minor={minorConsequences.Count}, " +
                     $"Moderate={moderateConsequences.Count}, " +
                     $"Severe={severeConsequences.Count}, " +
                     $"Critical={criticalConsequences.Count}");
            
            // Create default consequences if we still have none
            if (minorConsequences.Count == 0 && 
                moderateConsequences.Count == 0 &&
                severeConsequences.Count == 0 &&
                criticalConsequences.Count == 0)
            {
                CreateDefaultConsequences();
            }
        }
    }
    
    /// <summary>
    /// Create default consequences if none were loaded
    /// </summary>
    private void CreateDefaultConsequences()
    {
        Debug.LogWarning("No consequences found in Resources. Creating default consequences.");
        
        // Minor consequences
        StarkillerBaseCommand.Consequence minorConsequence = ScriptableObject.CreateInstance<StarkillerBaseCommand.Consequence>();
        minorConsequence.title = "Minor Security Breach";
        minorConsequence.type = StarkillerBaseCommand.Consequence.ConsequenceType.Security;
        minorConsequence.severity = StarkillerBaseCommand.Consequence.SeverityLevel.Minor;
        minorConsequence.possibleDescriptions = new string[] { "A security protocol violation was detected." };
        minorConsequence.creditPenalty = 5;
        minorConsequence.severityColor = Color.green;
        minorConsequences.Add(minorConsequence);
        
        // Moderate consequences
        StarkillerBaseCommand.Consequence moderateConsequence = ScriptableObject.CreateInstance<StarkillerBaseCommand.Consequence>();
        moderateConsequence.title = "Unauthorized Entry";
        moderateConsequence.type = StarkillerBaseCommand.Consequence.ConsequenceType.Security;
        moderateConsequence.severity = StarkillerBaseCommand.Consequence.SeverityLevel.Moderate;
        moderateConsequence.possibleDescriptions = new string[] { "An unauthorized ship accessed sensitive areas." };
        moderateConsequence.creditPenalty = 15;
        moderateConsequence.imperiumCasualties = 0;
        moderateConsequence.severityColor = Color.yellow;
        moderateConsequences.Add(moderateConsequence);
        
        // Severe consequences
        StarkillerBaseCommand.Consequence severeConsequence = ScriptableObject.CreateInstance<StarkillerBaseCommand.Consequence>();
        severeConsequence.title = "Security Alert";
        severeConsequence.type = StarkillerBaseCommand.Consequence.ConsequenceType.Security;
        severeConsequence.severity = StarkillerBaseCommand.Consequence.SeverityLevel.Severe;
        severeConsequence.possibleDescriptions = new string[] { "A significant security breach occurred with severe implications." };
        severeConsequence.creditPenalty = 30;
        severeConsequence.imperiumCasualties = 5;
        severeConsequence.triggersInspection = true;
        severeConsequence.severityColor = new Color(1.0f, 0.5f, 0.0f); // Orange
        severeConsequences.Add(severeConsequence);
        
        // Critical consequences
        StarkillerBaseCommand.Consequence criticalConsequence = ScriptableObject.CreateInstance<StarkillerBaseCommand.Consequence>();
        criticalConsequence.title = "Critical Security Failure";
        criticalConsequence.type = StarkillerBaseCommand.Consequence.ConsequenceType.Security;
        criticalConsequence.severity = StarkillerBaseCommand.Consequence.SeverityLevel.Critical;
        criticalConsequence.possibleDescriptions = new string[] { "A catastrophic security breach has compromised base security." };
        criticalConsequence.creditPenalty = 50;
        criticalConsequence.imperiumCasualties = 15;
        criticalConsequence.triggersInspection = true;
        criticalConsequence.hasLongTermEffect = true;
        criticalConsequence.effectDuration = 3;
        criticalConsequence.longTermDescription = "Security protocols heightened for several days.";
        criticalConsequence.severityColor = Color.red;
        criticalConsequences.Add(criticalConsequence);
        
        Debug.Log("Created default consequences");
    }
    
    /// <summary>
    /// Start a new day - update long-term effects, process tokens, and check for inspections
    /// </summary>
    public void StartNewDay(int day)
    {
        currentDay = day;
        Debug.Log($"[ConsequenceManager] Starting day {day} - Previous day was {currentDay - 1}");
        
        // Process consequence tokens for delayed events
        ProcessDailyTokens(day);
        
        // Update long-term effects
        UpdateLongTermEffects();
        
        // Check for random inspection
        CheckForInspection();
    }
    
    /// <summary>
    /// Update long-term consequences, reducing their remaining days
    /// </summary>
    private void UpdateLongTermEffects()
    {
        List<IncidentRecord> expiredEffects = new List<IncidentRecord>();
        
        // Update all active effects
        foreach (var effect in activeEffects)
        {
            // Reduce remaining days
            effect.remainingDays--;
            
            // Check if effect has expired
            if (effect.remainingDays <= 0)
            {
                expiredEffects.Add(effect);
            }
        }
        
        // Remove expired effects
        foreach (var expired in expiredEffects)
        {
            activeEffects.Remove(expired);
            Debug.Log($"Long-term effect expired: {expired.title}");
        }
    }
    
    /// <summary>
    /// Record a consequence from an incorrect decision
    /// </summary>
    public void RecordIncident(MasterShipEncounter encounter, bool approved)
    {
        // Determine severity based on the encounter
        StarkillerBaseCommand.Consequence.SeverityLevel severity;
        
        if (encounter.casualtiesIfWrong > 20 || (encounter.isStoryShip && encounter.storyTag == "insurgent" && approved))
        {
            severity = StarkillerBaseCommand.Consequence.SeverityLevel.Critical;
        }
        else if (encounter.casualtiesIfWrong > 10 || (encounter.isStoryShip && encounter.storyTag == "insurgent"))
        {
            severity = StarkillerBaseCommand.Consequence.SeverityLevel.Severe;
        }
        else if (encounter.casualtiesIfWrong > 0 || encounter.isStoryShip)
        {
            severity = StarkillerBaseCommand.Consequence.SeverityLevel.Moderate;
        }
        else
        {
            severity = StarkillerBaseCommand.Consequence.SeverityLevel.Minor;
        }
        
        // Select an appropriate consequence
        StarkillerBaseCommand.Consequence consequence = SelectConsequence(severity);
        
        if (consequence == null)
        {
            Debug.LogWarning("No consequence found for severity: " + severity);
            return;
        }

        // Create the incident record
        IncidentRecord incident = new IncidentRecord
        {
            title = consequence.title,
            description = consequence.GetRandomDescription(),
            casualties = encounter.casualtiesIfWrong > 0 ? encounter.casualtiesIfWrong : consequence.imperiumCasualties,
            creditPenalty = encounter.creditPenalty > 0 ? encounter.creditPenalty : consequence.creditPenalty,
            dayOccurred = currentDay,
            wasReported = false,
            severity = severity,
            type = consequence.type,
            hasLongTermEffect = consequence.hasLongTermEffect,
            remainingDays = consequence.effectDuration
        };
        
        // Add to incident tracking
        incidents.Add(incident);
        
        // Update statistics
        totalCasualties += incident.casualties;
        totalCreditLoss += incident.creditPenalty;
        
        if (consequence.type == StarkillerBaseCommand.Consequence.ConsequenceType.Security)
        {
            securityIncidents++;
        }
        else if (consequence.type == StarkillerBaseCommand.Consequence.ConsequenceType.Family)
        {
            familyIncidents++;
        }
        
        // Update inspection chance
        if (enableInspections)
        {
            currentInspectionChance += inspectionIncreasePerIncident;
            unreportedIncidents++;
            
            // Force inspection if too many unreported incidents
            if (unreportedIncidents >= maxIncidentsBeforeInspection)
            {
                TriggerInspection();
                unreportedIncidents = 0;
            }
        }
        
        // Apply game impacts
        ApplyConsequenceEffects(consequence, encounter);
        
        // Show immediate report for severe/critical incidents
        if (severity == StarkillerBaseCommand.Consequence.SeverityLevel.Severe || 
            severity == StarkillerBaseCommand.Consequence.SeverityLevel.Critical)
        {
            ShowIncidentReport(incident, consequence);
            incident.wasReported = true;
            unreportedIncidents = 0; // Reset counter since incident was reported
        }
        
        // If consequence has long-term effects, add to active effects
        if (consequence.hasLongTermEffect && consequence.effectDuration > 0)
        {
            activeEffects.Add(incident);
        }
        
        Debug.Log($"[ConsequenceManager] Recorded {severity} incident: {incident.title} on day {currentDay}");
        Debug.Log($"[ConsequenceManager] Total incidents now: {incidents.Count}");
    }
    
    /// <summary>
    /// Generate daily consequence report text
    /// <summary>
    public string GenerateDailyReport()
    {
        string report = "<b>SECURITY & INCIDENT REPORT</b>\n\n";
        
        // Find all incidents that happened today and weren't reported
        List<IncidentRecord> todaysIncidents = incidents.FindAll(
            i => i.dayOccurred == currentDay && !i.wasReported);
        
        if (todaysIncidents.Count == 0 && activeEffects.Count == 0)
        {
            report += "No incidents to report.\n";
        }
        else
        {
            // Group by severity
            List<IncidentRecord> criticalIncidents = todaysIncidents.FindAll(
                i => i.severity == StarkillerBaseCommand.Consequence.SeverityLevel.Critical);
            List<IncidentRecord> severeIncidents = todaysIncidents.FindAll(
                i => i.severity == StarkillerBaseCommand.Consequence.SeverityLevel.Severe);
            List<IncidentRecord> moderateIncidents = todaysIncidents.FindAll(
                i => i.severity == StarkillerBaseCommand.Consequence.SeverityLevel.Moderate);
            List<IncidentRecord> minorIncidents = todaysIncidents.FindAll(
                i => i.severity == StarkillerBaseCommand.Consequence.SeverityLevel.Minor);
            
            // Report critical incidents
            if (criticalIncidents.Count > 0)
            {
                report += "<color=red><b>CRITICAL INCIDENTS</b></color>\n";
                foreach (var incident in criticalIncidents)
                {
                    report += $"- {incident.title}: {incident.description}\n";
                    if (incident.casualties > 0)
                    {
                        report += $"  Casualties: {incident.casualties}\n";
                    }
                    if (incident.creditPenalty > 0)
                    {
                        report += $"  Credit Penalty: {incident.creditPenalty}\n";
                    }
                    report += "\n";
                    incident.wasReported = true;
                }
            }
            
            // Report severe incidents
            if (severeIncidents.Count > 0)
            {
                report += "<color=#FF6600><b>SEVERE INCIDENTS</b></color>\n";
                foreach (var incident in severeIncidents)
                {
                    report += $"- {incident.title}: {incident.description}\n";
                    if (incident.casualties > 0)
                    {
                        report += $"  Casualties: {incident.casualties}\n";
                    }
                    if (incident.creditPenalty > 0)
                    {
                        report += $"  Credit Penalty: {incident.creditPenalty}\n";
                    }
                    report += "\n";
                    incident.wasReported = true;
                }
            }
            
            // Report moderate incidents
            if (moderateIncidents.Count > 0)
            {
                report += "<color=yellow><b>MODERATE INCIDENTS</b></color>\n";
                foreach (var incident in moderateIncidents)
                {
                    report += $"- {incident.title}: {incident.description}\n";
                    if (incident.casualties > 0)
                    {
                        report += $"  Casualties: {incident.casualties}\n";
                    }
                    if (incident.creditPenalty > 0)
                    {
                        report += $"  Credit Penalty: {incident.creditPenalty}\n";
                    }
                    report += "\n";
                    incident.wasReported = true;
                }
            }
            
            // Report minor incidents
            if (minorIncidents.Count > 0)
            {
                report += "<color=green><b>MINOR INCIDENTS</b></color>\n";
                foreach (var incident in minorIncidents)
                {
                    report += $"- {incident.title}: {incident.description}\n";
                    incident.wasReported = true;
                }
                report += "\n";
            }
            
            // Add active long-term effects
            if (activeEffects.Count > 0)
            {
                report += "<b>ACTIVE ONGOING EFFECTS</b>\n";
                foreach (var effect in activeEffects)
                {
                    report += $"- {effect.title} ({effect.remainingDays} days remaining)\n";
                    report += $"  {effect.description}\n";
                }
                report += "\n";
            }
        }
        
        // Add total casualty count if any
        if (totalCasualties > 0)
        {
            report += $"<b>Total Imperium casualties to date:</b> {totalCasualties}\n";
        }
        
        // Add credit loss
        if (totalCreditLoss > 0)
        {
            report += $"<b>Total credit penalties to date:</b> {totalCreditLoss}\n";
        }
        
        // Reset unreported incidents counter since we've generated a report
        unreportedIncidents = 0;
        
        // Also reset inspection chance when generating a daily report
        currentInspectionChance = baseInspectionChance;
        
        return report;
    }
    
    /// <summary>
    /// Check if an inspection should occur
    /// </summary>
    private void CheckForInspection()
    {
        if (!enableInspections)
            return;
            
        // Roll for inspection
        if (Random.value < currentInspectionChance)
        {
            TriggerInspection();
            
            // Reset inspection chance
            currentInspectionChance = baseInspectionChance;
        }
    }
    
    /// <summary>
    /// Trigger a surprise inspection
    /// </summary>
    private void TriggerInspection()
    {
        // Create inspection reason based on suspicious activity
        string inspectionReason = "Routine security verification.";
        
        if (incidents.Count > 0)
        {
            int securityIncidents = incidents.Count(i => i.type == StarkillerBaseCommand.Consequence.ConsequenceType.Security);
            if (securityIncidents > 0)
            {
                inspectionReason = $"Recent security incidents ({securityIncidents}) have triggered a verification protocol.";
            }
        }
        
        // Create an inspection incident record (this will be updated after the UI interaction)
        IncidentRecord inspection = new IncidentRecord
        {
            title = "Surprise Inspection",
            description = inspectionReason,
            casualties = 0,
            creditPenalty = 0,
            dayOccurred = currentDay,
            wasReported = true,
            severity = StarkillerBaseCommand.Consequence.SeverityLevel.Moderate,
            type = StarkillerBaseCommand.Consequence.ConsequenceType.Security
        };
        
        // Add incident to tracking
        incidents.Add(inspection);
        
        // Show the UI - GameManager will handle the presentation
        if (gameManager != null)
        {
            // Show inspection UI and handle the completion callback
            gameManager.ShowInspectionUI(inspectionReason, (irregularitiesFound) => {
                // Update incident record based on inspection result
                if (irregularitiesFound)
                {
                    inspection.description += " Irregularities were found during inspection.";
                    inspection.severity = StarkillerBaseCommand.Consequence.SeverityLevel.Severe;
                    inspection.creditPenalty = 10; // Add some penalty
                    
                    // Apply penalty
                    if (gameManager != null)
                    {
                        // Add a strike
                        gameManager.SendMessage("AddCredits", -inspection.creditPenalty, SendMessageOptions.DontRequireReceiver);
                        
                        // Show feedback
                        var credentialChecker = FindFirstObjectByType<CredentialChecker>();
                        if (credentialChecker)
                        {
                            credentialChecker.ShowFeedback("Inspection found irregularities in your work. One strike added.", Color.red);
                        }
                    }
                }
                else
                {
                    inspection.description += " No irregularities found.";
                    
                    // Show positive feedback
                    var credentialChecker = FindFirstObjectByType<CredentialChecker>();
                    if (credentialChecker)
                    {
                        credentialChecker.ShowFeedback("Inspection complete. Your work meets Imperium standards.", Color.green);
                    }
                }
                
                // Create a temporary consequence for the report
                StarkillerBaseCommand.Consequence tempConsequence = ScriptableObject.CreateInstance<StarkillerBaseCommand.Consequence>();
                tempConsequence.title = inspection.title;
                tempConsequence.severity = inspection.severity;
                tempConsequence.severityColor = inspection.severity == StarkillerBaseCommand.Consequence.SeverityLevel.Severe ? 
                    Color.red : Color.yellow;
                
                // Show inspection report
                ShowIncidentReport(inspection, tempConsequence);

                // Reset inspection-related counters
                unreportedIncidents = 0;
                currentInspectionChance = baseInspectionChance;
            });
        }
        else
        {
            Debug.LogWarning("GameManager not found, cannot show inspection UI");
        }
        
        Debug.Log("Triggered base inspection");
    }
    
    /// <summary>
    /// Apply the effects of a consequence
    /// </summary>
    private void ApplyConsequenceEffects(StarkillerBaseCommand.Consequence consequence, MasterShipEncounter encounter)
    {
        if (gameManager == null)
            return;
            
        // Apply credit penalty
        if (consequence.creditPenalty > 0)
        {
            // First try direct method call
            if (gameManager.GetType().GetMethod("AddCredits") != null)
            {
                try
                {
                    gameManager.GetType().GetMethod("AddCredits").Invoke(gameManager, new object[] { -consequence.creditPenalty });
                }
                catch
                {
                    // Fallback to SendMessage
                    gameManager.SendMessage("AddCredits", -consequence.creditPenalty, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        
        // Apply loyalty/sympathy changes
        if (consequence.imperialLoyaltyChange != 0 || consequence.rebellionSympathyChange != 0)
        {
            int adjustedImperialChange = Mathf.RoundToInt(consequence.imperialLoyaltyChange * loyaltyImpactMultiplier);
            int adjustedRebellionChange = Mathf.RoundToInt(consequence.rebellionSympathyChange * loyaltyImpactMultiplier);
            
            // If encounter is a story ship, modify the loyalty impact
            if (encounter.isStoryShip)
            {
                if (encounter.storyTag == "insurgent")
                {
                    // Adjust based on story context
                    adjustedImperialChange -= 1;
                    adjustedRebellionChange += 1;
                }
                else if (encounter.storyTag == "imperium")
                {
                    // Adjust based on story context
                    adjustedImperialChange += 1;
                    adjustedRebellionChange -= 1;
                }
            }
            
            // Update loyalties through GameManager
            if (gameManager.GetType().GetMethod("UpdateLoyalty") != null)
            {
                try
                {
                    gameManager.GetType().GetMethod("UpdateLoyalty").Invoke(
                        gameManager, new object[] { adjustedImperialChange, adjustedRebellionChange });
                }
                catch
                {
                    // Fallback to SendMessage with an int array
                    gameManager.SendMessage("UpdateLoyalty", 
                                           new int[] { adjustedImperialChange, adjustedRebellionChange }, 
                                           SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        
        // Handle family impact
        if (consequence.affectsFamily && (Random.value < familyImpactChance || 
                                         consequence.severity == StarkillerBaseCommand.Consequence.SeverityLevel.Critical))
        {
            ApplyFamilyImpact(consequence.familyEffect);
        }
        
        // Handle inspection trigger
        if (consequence.triggersInspection)
        {
            TriggerInspection();
        }
        
        // Add decision to key decisions in GameManager for story tracking
        if (consequence.severity >= StarkillerBaseCommand.Consequence.SeverityLevel.Severe)
        {
            if (gameManager.GetType().GetMethod("AddKeyDecision") != null)
            {
                string decisionText = $"Caused {consequence.severity} incident: {consequence.title}";
                try
                {
                    gameManager.GetType().GetMethod("AddKeyDecision").Invoke(gameManager, new object[] { decisionText });
                }
                catch
                {
                    // Just log it if we can't add the decision
                    Debug.Log($"Story impact: {decisionText}");
                }
            }
        }
    }
    
    /// <summary>
    /// Automatically hide the report panel after a delay
    /// </summary>
    private System.Collections.IEnumerator HideReportAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideConsequenceReport();
    }

    /// <summary>
    /// Apply impact to the player's family
    /// </summary>
    private void ApplyFamilyImpact(string effect)
    {
        if (string.IsNullOrEmpty(effect) || familySystem == null)
            return;
            
        // First try direct method call
        if (familySystem.GetType().GetMethod("AddFamilyEvent") != null)
        {
            try
            {
                familySystem.GetType().GetMethod("AddFamilyEvent").Invoke(familySystem, new object[] { effect });
                Debug.Log($"Applied family effect: {effect}");
            }
            catch (System.Exception ex)
            {
                // Fallback to SendMessage
                familySystem.SendMessage("AddFamilyEvent", effect, SendMessageOptions.DontRequireReceiver);
                Debug.LogWarning($"Used SendMessage fallback for family effect: {ex.Message}");
            }
        }
        else
        {
            // General fallback
            familySystem.SendMessage("AddFamilyEvent", effect, SendMessageOptions.DontRequireReceiver);
            Debug.Log($"Applied family effect via SendMessage: {effect}");
        }
    }
    
    /// <summary>
    /// Select an appropriate consequence based on severity
    /// </summary>
    private StarkillerBaseCommand.Consequence SelectConsequence(StarkillerBaseCommand.Consequence.SeverityLevel severity)
    {
        List<StarkillerBaseCommand.Consequence> availableConsequences;
        
        switch (severity)
        {
            case StarkillerBaseCommand.Consequence.SeverityLevel.Critical:
                availableConsequences = criticalConsequences;
                break;
                
            case StarkillerBaseCommand.Consequence.SeverityLevel.Severe:
                availableConsequences = severeConsequences;
                break;
                
            case StarkillerBaseCommand.Consequence.SeverityLevel.Moderate:
                availableConsequences = moderateConsequences;
                break;
                
            case StarkillerBaseCommand.Consequence.SeverityLevel.Minor:
            default:
                availableConsequences = minorConsequences;
                break;
        }
        
        if (availableConsequences == null || availableConsequences.Count == 0)
        {
            Debug.LogWarning($"No consequences available for severity {severity}, creating default");
            return CreateDefaultConsequenceForSeverity(severity);
        }
            
        return availableConsequences[Random.Range(0, availableConsequences.Count)];
    }
    
    /// <summary>
    /// Create a default consequence when none is available for a severity level
    /// </summary>
    private StarkillerBaseCommand.Consequence CreateDefaultConsequenceForSeverity(StarkillerBaseCommand.Consequence.SeverityLevel severity)
    {
        StarkillerBaseCommand.Consequence consequence = ScriptableObject.CreateInstance<StarkillerBaseCommand.Consequence>();
        
        switch (severity)
        {
            case StarkillerBaseCommand.Consequence.SeverityLevel.Critical:
                consequence.title = "Critical Security Breach";
                consequence.severity = StarkillerBaseCommand.Consequence.SeverityLevel.Critical;
                consequence.type = StarkillerBaseCommand.Consequence.ConsequenceType.Security;
                consequence.possibleDescriptions = new string[] { "A catastrophic security failure has occurred." };
                consequence.imperiumCasualties = 20;
                consequence.creditPenalty = 50;
                consequence.imperialLoyaltyChange = -3;
                consequence.triggersInspection = true;
                consequence.severityColor = Color.red;
                break;
                
            case StarkillerBaseCommand.Consequence.SeverityLevel.Severe:
                consequence.title = "Severe Security Incident";
                consequence.severity = StarkillerBaseCommand.Consequence.SeverityLevel.Severe;
                consequence.type = StarkillerBaseCommand.Consequence.ConsequenceType.Security;
                consequence.possibleDescriptions = new string[] { "A serious security breach has been detected." };
                consequence.imperiumCasualties = 10;
                consequence.creditPenalty = 30;
                consequence.imperialLoyaltyChange = -2;
                consequence.severityColor = new Color(1.0f, 0.5f, 0.0f); // Orange
                break;
                
            case StarkillerBaseCommand.Consequence.SeverityLevel.Moderate:
                consequence.title = "Security Violation";
                consequence.severity = StarkillerBaseCommand.Consequence.SeverityLevel.Moderate;
                consequence.type = StarkillerBaseCommand.Consequence.ConsequenceType.Security;
                consequence.possibleDescriptions = new string[] { "Security protocols have been violated." };
                consequence.imperiumCasualties = 3;
                consequence.creditPenalty = 15;
                consequence.imperialLoyaltyChange = -1;
                consequence.severityColor = Color.yellow;
                break;
                
            case StarkillerBaseCommand.Consequence.SeverityLevel.Minor:
            default:
                consequence.title = "Minor Security Issue";
                consequence.severity = StarkillerBaseCommand.Consequence.SeverityLevel.Minor;
                consequence.type = StarkillerBaseCommand.Consequence.ConsequenceType.Security;
                consequence.possibleDescriptions = new string[] { "A minor security issue has been logged." };
                consequence.imperiumCasualties = 0;
                consequence.creditPenalty = 5;
                consequence.severityColor = Color.green;
                break;
        }
        
        return consequence;
    }
    
    /// <summary>
    /// Show an incident report to the player
    /// </summary>
    private void ShowIncidentReport(IncidentRecord incident, StarkillerBaseCommand.Consequence consequence)
    {
        if (consequenceReportPanel == null || reportTitleText == null || reportBodyText == null)
        {
            Debug.LogError("Missing UI references for showing incident report!");
            return;
        }
        
        Debug.Log($"Showing incident report for: {incident.title}");
        
        // Set report content
        reportTitleText.text = incident.title;
        
        string severityColor;
        switch (incident.severity)
        {
            case StarkillerBaseCommand.Consequence.SeverityLevel.Critical:
                severityColor = "red";
                break;
            case StarkillerBaseCommand.Consequence.SeverityLevel.Severe:
                severityColor = "#FF6600"; // Orange
                break;
            case StarkillerBaseCommand.Consequence.SeverityLevel.Moderate:
                severityColor = "yellow";
                break;
            default:
                severityColor = "green";
                break;
        }
        
        string reportBody = $"<color={severityColor}><b>{incident.severity} INCIDENT</b></color>\n\n";
        reportBody += incident.description + "\n\n";
        
        if (incident.casualties > 0)
        {
            reportBody += $"<b>Casualties:</b> {incident.casualties} Imperium personnel\n";
        }
        
        if (incident.creditPenalty > 0)
        {
            reportBody += $"<b>Credit Penalty:</b> {incident.creditPenalty}\n";
        }
        
        if (incident.hasLongTermEffect)
        {
            reportBody += $"\n<b>Long-term Effect:</b> This incident will have ongoing effects for {incident.remainingDays} days.\n";
        }
        
        reportBody += $"\n<i>Incident recorded on Day {incident.dayOccurred}</i>";
        
        reportBodyText.text = reportBody;
        
        // Set the severity icon if available
        if (severityIconImage != null && consequence.consequenceIcon != null)
        {
            severityIconImage.sprite = consequence.consequenceIcon;
            severityIconImage.gameObject.SetActive(true);
        }
        else if (severityIconImage != null)
        {
            severityIconImage.gameObject.SetActive(false);
        }
        
        // Show the panel - extra logging to debug
        Debug.Log("Activating consequence report panel...");
        consequenceReportPanel.SetActive(true);
        Debug.Log($"Panel active state: {consequenceReportPanel.activeSelf}");
        
        // Force panel to be visible
        Canvas[] canvases = consequenceReportPanel.GetComponentsInParent<Canvas>(true);
        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = true;
        }
        
        // Automatically hide report after delay (only for less severe incidents)
        if (incident.severity != StarkillerBaseCommand.Consequence.SeverityLevel.Critical &&
            incident.severity != StarkillerBaseCommand.Consequence.SeverityLevel.Severe)
        {
            StartCoroutine(HideReportAfterDelay(4.0f));
        }
    }
    
    /// <summary>
    /// Hide the consequence report panel
    /// </summary>
    private void HideConsequenceReport()
    {
        if (consequenceReportPanel != null)
        {
            consequenceReportPanel.SetActive(false);
        }
    }
    
    

    /// <summary>
    /// Get total casualty count
    /// </summary>
    public int GetTotalCasualties()
    {
        return totalCasualties;
    }
    
    /// <summary>
    /// Get total credit loss
    /// </summary>
    public int GetTotalCreditLoss()
    {
        return totalCreditLoss;
    }
    
    /// <summary>
    /// Get incident count by severity
    /// </summary>
    public int GetIncidentCount(StarkillerBaseCommand.Consequence.SeverityLevel severity)
    {
        int count = 0;
        foreach (var incident in incidents)
        {
            if (incident.severity == severity)
                count++;
        }
        return count;
    }
    
    /// <summary>
    /// Get incident count by type
    /// </summary>
    public int GetIncidentCountByType(StarkillerBaseCommand.Consequence.ConsequenceType type)
    {
        int count = 0;
        foreach (var incident in incidents)
        {
            if (incident.type == type)
                count++;
        }
        return count;
    }
    
    /// <summary>
    /// Check if there are any active long-term effects
    /// </summary>
    public bool HasActiveLongTermEffects()
    {
        return activeEffects.Count > 0;
    }
    
    /// <summary>
    /// Get the number of active long-term effects
    /// </summary>
    public int GetActiveLongTermEffectCount()
    {
        return activeEffects.Count;
    }
    
    /// <summary>
    /// Get a list of all active long-term effects
    /// </summary>
    public List<string> GetActiveLongTermEffectDescriptions()
    {
        List<string> descriptions = new List<string>();
        foreach (var effect in activeEffects)
        {
            descriptions.Add($"{effect.title} ({effect.remainingDays} days): {effect.description}");
        }
        return descriptions;
    }
    
    /// <summary>
    /// Record a custom consequence for special events
    /// </summary>
    public void RecordCustomConsequence(string title, string description, StarkillerBaseCommand.Consequence.SeverityLevel severity, 
                                       int casualties = 0, int creditPenalty = 0)
    {
        // Create the incident
        IncidentRecord incident = new IncidentRecord
        {
            title = title,
            description = description,
            casualties = casualties,
            creditPenalty = creditPenalty,
            dayOccurred = currentDay,
            wasReported = true, // Custom consequences are considered reported immediately
            severity = severity,
            type = StarkillerBaseCommand.Consequence.ConsequenceType.Special
        };
        
        // Add to tracking
        incidents.Add(incident);
        
        // Update statistics
        totalCasualties += casualties;
        totalCreditLoss += creditPenalty;
        
        // Apply credit penalty
        if (creditPenalty > 0 && gameManager != null)
        {
            gameManager.SendMessage("AddCredits", -creditPenalty, SendMessageOptions.DontRequireReceiver);
        }
        
        Debug.Log($"Recorded custom consequence: {title}");
    }
    
    /// <summary>
    /// Get today's incidents for Personal Data Log
    /// </summary>
    public List<IncidentRecord> GetTodaysIncidents()
    {
        var todaysIncidents = incidents.FindAll(i => i.dayOccurred == currentDay);
        Debug.Log($"[ConsequenceManager] GetTodaysIncidents() called for day {currentDay}, found {todaysIncidents.Count} incidents");
        
        if (todaysIncidents.Count > 0)
        {
            foreach (var incident in todaysIncidents)
            {
                Debug.Log($"[ConsequenceManager] - {incident.title} (Severity: {incident.severity}, Day: {incident.dayOccurred})");
            }
        }
        
        return todaysIncidents;
    }
    
    /// <summary>
    /// Get all unreported incidents for Personal Data Log
    /// </summary>
    public List<IncidentRecord> GetUnreportedIncidents()
    {
        return incidents.FindAll(i => !i.wasReported);
    }
    
    /// <summary>
    /// Mark incidents as reported (used by Personal Data Log)
    /// </summary>
    public void MarkIncidentsAsReported(List<IncidentRecord> incidentsToMark)
    {
        foreach (var incident in incidentsToMark)
        {
            incident.wasReported = true;
        }
    }
    
    /// <summary>
    /// Get active long-term effects for Personal Data Log
    /// </summary>
    public List<IncidentRecord> GetActiveLongTermEffects()
    {
        return new List<IncidentRecord>(activeEffects);
    }
    
    #region Consequence Token System
    
    /// <summary>
    /// Add a consequence token for delayed event triggering
    /// </summary>
    public void AddConsequenceToken(string decisionId, int delayDays, ConsequenceData consequence)
    {
        var token = new ConsequenceToken
        {
            tokenId = System.Guid.NewGuid().ToString(),
            sourceDecision = decisionId,
            dayCreated = currentDay,
            triggerDay = currentDay + delayDays,
            hasTriggered = false,
            consequenceData = consequence
        };
        
        consequenceTokens.Add(token);
        Debug.Log($"Added consequence token: {decisionId} will trigger on day {token.triggerDay}");
    }
    
    /// <summary>
    /// Process all tokens that should trigger on the current day
    /// </summary>
    public void ProcessDailyTokens(int currentDay)
    {
        var tokensToTrigger = consequenceTokens.FindAll(t => 
            !t.hasTriggered && t.triggerDay <= currentDay);
            
        foreach (var token in tokensToTrigger)
        {
            TriggerConsequenceToken(token);
            token.hasTriggered = true;
        }
        
        // Clean up old triggered tokens (keep for a few days for tracking)
        var oldTokens = consequenceTokens.FindAll(t => 
            t.hasTriggered && (currentDay - t.triggerDay) > 5);
        foreach (var oldToken in oldTokens)
        {
            consequenceTokens.Remove(oldToken);
        }
    }
    
    /// <summary>
    /// Trigger a consequence token's effects
    /// </summary>
    private void TriggerConsequenceToken(ConsequenceToken token)
    {
        Debug.Log($"Triggering consequence token: {token.sourceDecision}");
        
        var data = token.consequenceData;
        
        // Trigger specific scenario if specified
        if (!string.IsNullOrEmpty(data.scenarioToTrigger))
        {
            TriggerNamedScenario(data.scenarioToTrigger);
        }
        
        // Create news headline for PersonalDataLog
        if (!string.IsNullOrEmpty(data.newsHeadline))
        {
            CreateNewsFromToken(token);
        }
        
        // Apply loyalty/suspicion impacts
        if (data.loyaltyImpact != 0 || data.suspicionIncrease != 0)
        {
            ApplyTokenLoyaltyEffects(data);
        }
        
        // Trigger family pressure if applicable
        if (data.affectsFamily)
        {
            TriggerTokenFamilyEffect(token);
        }
    }
    
    /// <summary>
    /// Create a news entry from a consequence token
    /// </summary>
    private void CreateNewsFromToken(ConsequenceToken token)
    {
        var personalDataLogManager = ServiceLocator.Get<PersonalDataLogManager>();
        if (personalDataLogManager == null) return;
        
        var data = token.consequenceData;
        var logEntry = new DataLogEntry
        {
            headline = data.newsHeadline,
            content = GenerateTokenNewsContent(token),
            feedType = FeedType.ImperiumNews, // Most consequence news is Imperial
            timestamp = System.DateTime.Now,
            requiresAction = data.suspicionIncrease > 5
        };
        
        personalDataLogManager.AddLogEntry(logEntry);
        Debug.Log($"Created news from token: {data.newsHeadline}");
    }
    
    /// <summary>
    /// Generate news content based on the token
    /// </summary>
    private string GenerateTokenNewsContent(ConsequenceToken token)
    {
        var data = token.consequenceData;
        string content = "";
        
        // Customize content based on source decision
        switch (token.sourceDecision)
        {
            case "SMUGGLER_APPROVED":
                content = "Imperial Security reports increased contraband trafficking through outer system checkpoints. New screening protocols are being implemented.";
                break;
            case "REBEL_SYMPATHIZER_HELPED":
                content = "Intelligence sources confirm rebel supply lines remain active despite recent security measures. Investigation ongoing.";
                break;
            case "BRIBE_ACCEPTED":
                content = "Internal Affairs announces random audits of checkpoint personnel following reports of irregular procedures.";
                break;
            case "MEDICAL_SUPPLIES_APPROVED":
                content = "Medical Command reports shortages of critical supplies in outer territories. Supply chain review initiated.";
                break;
            default:
                content = $"Security Command reviews recent checkpoint activities. Days since incident: {token.dayCreated}";
                break;
        }
        
        // Add consequences if severe
        if (data.loyaltyImpact < -2 || data.suspicionIncrease > 3)
        {
            content += "\n\nCommand emphasizes the importance of strict adherence to protocols.";
        }
        
        return content;
    }
    
    /// <summary>
    /// Apply loyalty and suspicion effects from a token
    /// </summary>
    private void ApplyTokenLoyaltyEffects(ConsequenceData data)
    {
        if (gameManager == null) return;
        
        // Apply loyalty changes
        if (data.loyaltyImpact != 0)
        {
            try
            {
                gameManager.SendMessage("UpdateLoyalty", new int[] { data.loyaltyImpact, 0 }, SendMessageOptions.DontRequireReceiver);
            }
            catch
            {
                Debug.LogWarning("Could not apply loyalty impact from consequence token");
            }
        }
        
        // Apply suspicion increases
        if (data.suspicionIncrease > 0)
        {
            // Increase inspection chance based on suspicion
            currentInspectionChance += (data.suspicionIncrease * 0.02f);
            
            // Add suspicious activity to tracking
            unreportedIncidents += data.suspicionIncrease > 5 ? 2 : 1;
            
            Debug.Log($"Suspicion increased by {data.suspicionIncrease}. Current inspection chance: {currentInspectionChance:P1}");
        }
    }
    
    /// <summary>
    /// Trigger family effect from a consequence token
    /// </summary>
    private void TriggerTokenFamilyEffect(ConsequenceToken token)
    {
        var familyPressureManager = ServiceLocator.Get<FamilyPressureManager>();
        if (familyPressureManager == null) return;
        
        // Create family pressure based on the token's source
        string reason = "";
        int amount = 0;
        
        switch (token.sourceDecision)
        {
            case "SMUGGLER_APPROVED":
                reason = "Increased security scrutiny affects family clearances";
                amount = 75;
                break;
            case "REBEL_SYMPATHIZER_HELPED":
                reason = "Family background check required due to security concerns";
                amount = 150;
                break;
            case "BRIBE_ACCEPTED":
                reason = "Audit investigation creates financial complications";
                amount = 100;
                break;
            default:
                reason = "Security situation affects family status";
                amount = 50;
                break;
        }
        
        familyPressureManager.TriggerFamilyExpense(reason, amount, 3);
        Debug.Log($"Token triggered family pressure: {reason} ({amount} credits)");
    }
    
    /// <summary>
    /// Trigger a named scenario from a consequence token
    /// </summary>
    private void TriggerNamedScenario(string scenarioName)
    {
        // This would integrate with the scenario system to trigger specific encounters
        Debug.Log($"Consequence token triggering scenario: {scenarioName}");
        
        // Implementation would depend on how scenarios are managed in the game
        // For now, we'll log it and potentially add it to a queue for the encounter system
    }
    
    /// <summary>
    /// Get all active consequence tokens
    /// </summary>
    public List<ConsequenceToken> GetActiveTokens()
    {
        return consequenceTokens.FindAll(t => !t.hasTriggered);
    }
    
    /// <summary>
    /// Check if there's a token of a specific type
    /// </summary>
    public bool HasTokenOfType(string tokenType)
    {
        return consequenceTokens.Exists(t => 
            !t.hasTriggered && t.sourceDecision.Contains(tokenType));
    }
    
    /// <summary>
    /// Get count of tokens that will trigger in the next N days
    /// </summary>
    public int GetUpcomingTokenCount(int days)
    {
        return consequenceTokens.Count(t => 
            !t.hasTriggered && 
            t.triggerDay <= (currentDay + days) && 
            t.triggerDay > currentDay);
    }
    
    #endregion
}

/// <summary>
/// Represents a delayed consequence that triggers after a specific number of days
/// </summary>
[System.Serializable]
public class ConsequenceToken
{
    public string tokenId;
    public string sourceDecision;
    public int dayCreated;
    public int triggerDay;
    public bool hasTriggered;
    public ConsequenceData consequenceData;
}

/// <summary>
/// Data structure for consequence token effects
/// </summary>
[System.Serializable]
public class ConsequenceData
{
    public string scenarioToTrigger;
    public string newsHeadline;
    public int loyaltyImpact;
    public int suspicionIncrease;
    public bool affectsFamily;
}