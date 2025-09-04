using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarkillerBaseCommand;

/// <summary>
/// Manages the daily briefing screen that appears at the start of each day
/// Shows rules, news, and information about the current day
/// </summary>
public class DailyBriefingManager : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Main panel for the daily briefing")]
    public GameObject briefingPanel;
    
    [Tooltip("Header showing day and date")]
    public TMP_Text headerText;
    
    [Tooltip("Terminal ID and time display")]
    public TMP_Text terminalInfoText;
    
    [Tooltip("Security clearance text")]
    public TMP_Text securityClearanceText;
    
    [Tooltip("Current time display")]
    public TMP_Text timeDisplay;
    
    [Tooltip("Bootup screen shown during initialization")]
    public GameObject bootupScreen;
    
    [Header("Content Sections")]
    [Tooltip("Imperium greeting text")]
    public TMP_Text imperialAddressText;
    
    [Tooltip("Access codes and security rules")]
    public TMP_Text securityProtocolsText;
    
    [Tooltip("News items related to previous day's decisions")]
    public TMP_Text intelligenceBriefingText;
    
    [Tooltip("Base announcements and recreational activities")]
    public TMP_Text stationAnnouncementsText;
    
    [Tooltip("Family updates and personal messages")]
    public TMP_Text personnelUpdatesText;
    
    [Tooltip("Daily objectives text")]
    public TMP_Text dailyObjectivesText;
    
    [Tooltip("Performance metrics text")]
    public TMP_Text performanceMetricsText;
    
    [Header("UI Navigation")]
    [Tooltip("Button to begin the day's shift")]
    public Button beginShiftButton;
    
    [Tooltip("Navigation buttons for different sections")]
    public Button[] sectionButtons;
    
    [Tooltip("Names for section buttons")]
    public string[] sectionNames = new string[] 
    {
        "IMPERIAL ADDRESS",
        "SECURITY PROTOCOLS",
        "INTELLIGENCE BRIEFING",
        "ANNOUNCEMENTS",
        "PERSONNEL UPDATES"
    };

    [Tooltip("Content panels for each section")]
    public GameObject[] sectionPanels;
    
    [Header("Visual Elements")]
    [Tooltip("Imperium logo image")]
    public Image imperialLogo;
    
    [Tooltip("Scrolling news ticker text")]
    public TMP_Text newsTicker;
    
    [Tooltip("Background panel with watermark")]
    public Image backgroundPanel;
    
    [Tooltip("Imperium emblem watermark")]
    public GameObject imperialWatermark;
    
    [Header("Audio")]
    [Tooltip("Terminal startup sound")]
    public AudioClip startupSound;
    
    [Tooltip("Button click sound")]
    public AudioClip buttonClickSound;
    
    [Tooltip("Begin shift sound")]
    public AudioClip beginShiftSound;
    
    [Tooltip("Background ambient sound")]
    public AudioClip ambientSound;
    
    [Header("Animation")]
    [Tooltip("Boot-up animation duration")]
    public float bootupDuration = 3.0f;
    
    [Tooltip("Minimum time player must view briefing before continuing")]
    public float minimumViewTime = 5.0f;
    
    [Header("Content Generation")]
    [Tooltip("Possible imperial greetings")]
    public string[] imperialGreetings;
    
    [Tooltip("Possible station announcements")]
    public string[] stationAnnouncementTemplates;
    
    [Tooltip("Possible family update templates")]
    public string[] familyUpdateTemplates;
    
    [Tooltip("Possible bootup messages")]
    public string[] bootupMessages = new string[]
    {
        "System Check: In Progress",
        "Security Protocols: Loading",
        "Biometric Authentication: Complete",
        "Establishing Secure Connection...",
        "Imperium Network Initializing...",
        "Loading Personnel Records...",
        "Checking Clearance Level..."
    };
    
    // References to other game systems
    private GameManager gameManager;
    private StarkkillerContentManager contentManager;
    private ImperialFamilySystem familySystem;
    
    // Internal state
    private bool briefingReady = false;
    private float briefingStartTime;
    private int currentSectionIndex = 0;
    private AudioSource audioSource;
    private Coroutine timeUpdateCoroutine;

    // News items based on game events and player decisions
    private List<string> newsItems = new List<string>();
    
    // Current day star date (for display)
    private string currentStarDate;

    private TimeManager timeManager;
    
    void Start()
    {
        // Find references if not assigned
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
            
        if (contentManager == null)
            contentManager = FindFirstObjectByType<StarkkillerContentManager>();
            
        if (familySystem == null)
            familySystem = FindFirstObjectByType<ImperialFamilySystem>();
            
        // Find TimeManager if not assigned
        if (timeManager == null)
        timeManager = TimeManager.Instance;

        // Get audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        // Set up button listeners
        SetupButtons();
        
        // Hide the briefing panel initially
        if (briefingPanel)
            briefingPanel.SetActive(false);
    }
    
    private void OnDestroy()
    {
        // Make sure to stop the time update coroutine when this object is destroyed
        if (timeUpdateCoroutine != null)
        {
            StopCoroutine(timeUpdateCoroutine);
            timeUpdateCoroutine = null;
        }
    }

    /// <summary>
    /// Shows the daily briefing at the start of a new day
    /// </summary>
    public void ShowDailyBriefing(int day)
    {
        // First ensure the panel is active
        if (briefingPanel == null)
        {
            Debug.LogError("DailyBriefingManager: briefingPanel reference is missing!");
            // Try to find it in the scene
            briefingPanel = GameObject.Find("DailyBriefingPanel");
            if (briefingPanel == null)
            {
                Debug.LogError("DailyBriefingPanel not found in the scene! Cannot show daily briefing.");
                return;
            }
        }
        
        // Activate the panel
        if (!briefingPanel.activeSelf)
        {
            Debug.Log($"Activating briefingPanel before showing briefing for day {day}");
            briefingPanel.SetActive(true);
        }
        
        // Generate star date
        GenerateStarDate(day);
        
        // Generate content for the briefing
        GenerateDailyContent(day);
        
        // Pause time while showing briefing - but only after game has started
        // This prevents the TimeModifierBehavior from pausing during initialization
        if (timeManager != null && Time.timeSinceLevelLoad > 2f)
        {
            timeManager.PauseTime(gameObject, "DailyBriefing");
        }
        
        // Make sure the briefing panel is active before starting coroutines
        if (briefingPanel.activeSelf)
        {
            // Start boot-up animation
            StartCoroutine(BootupSequence());
        }
        else
        {
            Debug.LogError("Cannot start BootupSequence - briefingPanel is inactive!");
        }
    }

    // And in your HideBriefing method:
    private IEnumerator HideBriefing()
    {
        // Start fade out animation
        CanvasGroup canvasGroup = briefingPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            float duration = 0.5f;
            float fadeStartTime = Time.time; // Record start time

            while (Time.time < fadeStartTime + duration)
            {
                float t = (Time.time - fadeStartTime) / duration;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
        }
        else
        {
            // Simple delay if no canvas group
            yield return new WaitForSeconds(0.5f);
        }
        
        // Stop time update
        if (timeUpdateCoroutine != null)
        {
            StopCoroutine(timeUpdateCoroutine);
            timeUpdateCoroutine = null;
        }
        
        // Hide the panel
        if (briefingPanel)
            briefingPanel.SetActive(false);
            
        // Stop ambient sound
        if (audioSource)
            audioSource.Stop();
        
        // Resume time
        if (timeManager != null)
        {
            timeManager.ResumeTime("DailyBriefing");
        }
        
        // Set the game state to active gameplay
        GameStateController gameStateController = GameStateController.Instance;
        if (gameStateController != null)
        {
            Debug.Log("DailyBriefingManager: Setting game state to ActiveGameplay");
            gameStateController.SetGameState(GameStateController.GameActivationState.ActiveGameplay);
            
            // Wait for state change to take effect
            yield return new WaitForSeconds(0.5f);
        }
        
        // Reset any timing controllers to ensure no cooldowns are active
        ShipTimingController timingController = FindFirstObjectByType<ShipTimingController>();
        if (timingController != null)
        {
            Debug.Log("DailyBriefingManager: Resetting ShipTimingController cooldowns");
            timingController.ResetCooldown();
        }
        
        // Force UI visibility in CredentialChecker
        CredentialChecker credentialChecker = FindFirstObjectByType<CredentialChecker>();
        if (credentialChecker != null)
        {
            Debug.Log("DailyBriefingManager: Forcing UI visibility in CredentialChecker");
            credentialChecker.ForceUIVisibility();
        }
        
        // Tell the game manager to start the day
        Debug.Log($"DailyBriefingManager: HideBriefing completed - gameManager is {(gameManager != null ? "NOT NULL" : "NULL")}");
        if (gameManager != null)
        {
            Debug.Log("DailyBriefingManager: Notifying GameManager that briefing is complete");
            gameManager.OnDailyBriefingCompleted();
        }
        else
        {
            Debug.LogError("DailyBriefingManager: gameManager is NULL! Cannot call OnDailyBriefingCompleted()");
            // Try to find it again
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                Debug.Log("DailyBriefingManager: Found GameManager on retry, calling OnDailyBriefingCompleted()");
                gameManager.OnDailyBriefingCompleted();
            }
            else
            {
                Debug.LogError("DailyBriefingManager: Still cannot find GameManager!");
            }
        }
    }   


    /// <summary>
    /// Sets up button listeners and text on UI buttons
    /// </summary>
    private void SetupButtons()
    {
        // Begin shift button
        if (beginShiftButton)
            beginShiftButton.onClick.AddListener(OnBeginShiftClicked);
            
        // Section navigation buttons
        if (sectionButtons != null)
        {
            for (int i = 0; i < sectionButtons.Length; i++)
            {
                if (sectionButtons[i] != null)
                {
                    int sectionIndex = i; // Capture index for lambda
                    sectionButtons[i].onClick.AddListener(() => ShowSection(sectionIndex));
                    
                    // Set button text if there's a TextMeshProUGUI component
                    TMP_Text buttonText = sectionButtons[i].GetComponentInChildren<TMP_Text>();
                    if (buttonText != null && i < sectionNames.Length)
                    {
                        buttonText.text = sectionNames[i];
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Shows a specific section of the briefing
    /// </summary>
    private void ShowSection(int sectionIndex)
    {
        // Make sure the index is valid
        if (sectionIndex < 0 || sectionPanels == null || sectionIndex >= sectionPanels.Length)
            return;
            
        // Play button sound
        if (audioSource && buttonClickSound)
            audioSource.PlayOneShot(buttonClickSound);
            
        // Update current section
        currentSectionIndex = sectionIndex;
        
        // Hide all panels
        if (sectionPanels != null)
        {
            foreach (var panel in sectionPanels)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }
        
        // Show the selected panel
        if (sectionPanels != null && sectionIndex < sectionPanels.Length && sectionPanels[sectionIndex] != null)
            sectionPanels[sectionIndex].SetActive(true);
            
        // Update button visuals to show which is selected
        UpdateButtonVisuals();
    }
    
    /// <summary>
    /// Updates button visuals to show which section is selected
    /// </summary>
    private void UpdateButtonVisuals()
    {
        if (sectionButtons == null)
            return;
            
        for (int i = 0; i < sectionButtons.Length; i++)
        {
            if (sectionButtons[i] != null)
            {
                ColorBlock colors = sectionButtons[i].colors;
                
                if (i == currentSectionIndex)
                {
                    // Selected button - Imperium red
                    colors.normalColor = new Color(0.8f, 0.2f, 0.2f);
                    colors.highlightedColor = new Color(0.9f, 0.3f, 0.3f);
                    colors.pressedColor = new Color(0.7f, 0.15f, 0.15f);
                    colors.selectedColor = new Color(0.8f, 0.2f, 0.2f);
                }
                else
                {
                    // Unselected button - Dark gray
                    colors.normalColor = new Color(0.3f, 0.3f, 0.3f);
                    colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f);
                    colors.pressedColor = new Color(0.25f, 0.25f, 0.25f);
                    colors.selectedColor = new Color(0.3f, 0.3f, 0.3f);
                }
                
                sectionButtons[i].colors = colors;
                
                // Also update indicator if present
                Transform indicator = sectionButtons[i].transform.Find("Indicator");
                if (indicator != null)
                    indicator.gameObject.SetActive(i == currentSectionIndex);
            }
        }
    }
    
    /// <summary>
    /// Begin shift button click handler
    /// </summary>
    private void OnBeginShiftClicked()
    {
        Debug.Log($"DailyBriefingManager: OnBeginShiftClicked() called - briefingReady: {briefingReady}, timeSince: {Time.time - briefingStartTime:F1}s, minTime: {minimumViewTime}s");
        
        // Make sure briefing is ready and minimum viewing time has passed
        if (!briefingReady || Time.time - briefingStartTime < minimumViewTime)
        {
            Debug.LogWarning($"DailyBriefingManager: Begin shift blocked - briefingReady: {briefingReady}, timeSince: {Time.time - briefingStartTime:F1}s < minTime: {minimumViewTime}s");
            return;
        }
        
        // Make sure minimum viewing time has passed
        if (Time.time - briefingStartTime < minimumViewTime)
        {
            Debug.LogWarning($"DailyBriefingManager: Begin shift blocked by minimum viewing time");
            return;
        }
            
        Debug.Log("DailyBriefingManager: Begin shift conditions met, starting HideBriefing()");
            
        // Play sound effect
        if (audioSource && beginShiftSound)
            audioSource.PlayOneShot(beginShiftSound);
            
        // Hide the briefing
        StartCoroutine(HideBriefing());
    }
    
    /// <summary>
    /// Start real-time clock update
    /// </summary>
    private void StartTimeUpdate()
    {
        if (timeUpdateCoroutine != null)
        {
            StopCoroutine(timeUpdateCoroutine);
        }
        
        timeUpdateCoroutine = StartCoroutine(UpdateTimeDisplay());
    }
    
    /// <summary>
    /// Coroutine to continuously update the time display
    /// </summary>
    private IEnumerator UpdateTimeDisplay()
    {
        while (true)
        {
            if (timeDisplay != null)
            {
                // Update time (format: HH:MM:SS)
                System.DateTime now = System.DateTime.Now;
                timeDisplay.text = now.ToString("HH:mm:ss");
                
                // Make it pulse
                if (now.Second % 2 == 0)
                {
                    timeDisplay.color = new Color(1f, 1f, 1f, 0.9f);
                }
                else
                {
                    timeDisplay.color = new Color(1f, 1f, 1f, 0.6f);
                }
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    /// <summary>
    /// Boot-up animation sequence
    /// </summary>
    private IEnumerator BootupSequence()
    {
        // Show the panel
        if (briefingPanel)
            briefingPanel.SetActive(true);
            
        // Reset alpha if there's a canvas group
        CanvasGroup canvasGroup = briefingPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
        
        // Show bootup screen
        if (bootupScreen)
        {
            bootupScreen.SetActive(true);
            
            // Setup bootup text elements
            TMP_Text[] bootupTexts = bootupScreen.GetComponentsInChildren<TMP_Text>();
            
            // Play startup sound
            if (audioSource && startupSound)
                audioSource.PlayOneShot(startupSound);
                
            // Show bootup progress messages sequentially
            for (int i = 0; i < bootupMessages.Length; i++)
            {
                // Find a text component to use
                if (i < bootupTexts.Length)
                {
                    bootupTexts[i].text = bootupMessages[i];
                    bootupTexts[i].alpha = 0;
                    
                    // Fade in
                    float fadeTime = 0.4f;
                    float fadeStartTime = Time.time;
                    
                    while (Time.time < fadeStartTime + fadeTime)
                    {
                        float t = (Time.time - fadeStartTime) / fadeTime;
                        bootupTexts[i].alpha = Mathf.Lerp(0, 1, t);
                        yield return null;
                    }
                    
                    bootupTexts[i].alpha = 1;
                }
                
                yield return new WaitForSeconds(0.5f);
            }
            
            // Final delay before transition
            yield return new WaitForSeconds(1.0f);

            // Hide bootup screen with fade
            float duration = 0.5f;
            float bootupFadeStartTime = Time.time;

            CanvasGroup bootupCanvas = bootupScreen.GetComponent<CanvasGroup>();
            if (bootupCanvas != null)
            {
                while (Time.time < bootupFadeStartTime + duration)
                {
                    float t = (Time.time - bootupFadeStartTime) / duration;
                    bootupCanvas.alpha = Mathf.Lerp(1f, 0f, t);
                    yield return null;
                }
            }
            
            bootupScreen.SetActive(false);
        }
        else
        {
            // Play startup sound
            if (audioSource && startupSound)
                audioSource.PlayOneShot(startupSound);
                
            // Simple delay if no boot screen is available
            yield return new WaitForSeconds(bootupDuration);
        }
        
        // Start time display updating
        StartTimeUpdate();
        
        // Start ambient sound
        if (audioSource && ambientSound)
        {
            audioSource.clip = ambientSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // Display the first section
        ShowSection(0);
        
        // Record start time for minimum view duration
        briefingStartTime = Time.time;
        briefingReady = true;
    }
    
    /// <summary>
    /// Generates a star date for the given day
    /// </summary>
    private void GenerateStarDate(int day)
    {
        // Use a deterministic formula based on the day
        int year = 34 + (day / 100);
        int segment = 980 + (day % 100);
        int unit = day % 10;
        
        currentStarDate = string.Format("{0}.{1}.{2}", year, segment, unit);
        
        // Update header text
        if (headerText)
            headerText.text = string.Format("GALACTIC STANDARD DATE: {0}", currentStarDate);
                
        // Generate a unique terminal ID (between 10000 and 99999)
        string terminalID = string.Format("SK-{0}", UnityEngine.Random.Range(10000, 99999));
        
        // Update terminal info text
        if (terminalInfoText)
            terminalInfoText.text = string.Format("TERMINAL ID: {0}", terminalID);
                
        // Set security clearance (varies by day)
        string[] clearanceLevels = { "ALPHA", "BETA", "GAMMA", "DELTA" };
        int clearanceIndex = (day % clearanceLevels.Length);
        int clearanceLevel = (day % 5) + 1;
        
        if (securityClearanceText)
            securityClearanceText.text = string.Format("SECURITY CLEARANCE: {0}-{1}", 
                clearanceLevels[clearanceIndex], clearanceLevel);
    }
    
    /// <summary>
    /// Generates content for the daily briefing based on the day and game state
    /// </summary>
    private void GenerateDailyContent(int day)
    {
        // Get valid access codes and rules from ContentManager
        List<string> accessCodes = new List<string>();
        List<string> securityRules = new List<string>();
        
        if (contentManager != null)
        {
            accessCodes = contentManager.currentAccessCodes;
            
            // Extract rules that aren't just about access codes
            foreach (var rule in contentManager.currentDayRules)
            {
                if (!rule.StartsWith("Valid access codes"))
                    securityRules.Add(rule);
            }
        }
        else
        {
            // Fallback if ContentManager not available
            accessCodes.Add("SK-" + UnityEngine.Random.Range(1000, 9999));
            accessCodes.Add("IM-" + UnityEngine.Random.Range(1000, 9999));
            
            securityRules.Add("All ships must have valid clearance codes");
            securityRules.Add("Verify manifest contents match claimed cargo");
        }
        
        // Generate imperial greeting
        string greeting = "Glory to the Imperium, Officer.";
        if (imperialGreetings != null && imperialGreetings.Length > 0)
            greeting = imperialGreetings[day % imperialGreetings.Length];
            
        // Generate news based on previous day's events
        GenerateNewsItems(day);
        
        // Generate station announcements
        List<string> announcements = GenerateStationAnnouncements(day);
        
        // Generate family updates
        List<string> familyUpdates = GenerateFamilyUpdates(day);
        
        // Update imperial address text
        if (imperialAddressText)
        {
            imperialAddressText.text = $"<b>{greeting}</b>\n\n" +
            "Your position at Starkiller Base Command is crucial to maintaining order and security in this sector. " +
            "Each ship you process represents a potential threat or asset to our operations.\n\n" +
            "Remain vigilant. The insurgency grows desperate in the face of our might.\n\n" +
            "<align=right>Director Flux\nImperium Megastation Command</align>";
        }

        // Update performance metrics
        if (performanceMetricsText && gameManager != null)
        {
            int credits = gameManager.GetCredits();
            
            // Calculate loyalty rating based on correct vs wrong decisions
            string loyaltyRating = "STANDARD";
            int correct = GetCorrectDecisions();
            int wrong = GetWrongDecisions();
            
            if (correct > wrong * 2)
                loyaltyRating = "COMMENDABLE";
            else if (correct > wrong)
                loyaltyRating = "FAVORABLE";
            else if (wrong > correct)
                loyaltyRating = "UNDER REVIEW";
            
            // Calculate efficiency rating
            int efficiency = CalculateEfficiency();
            
            performanceMetricsText.text = "<b>PERFORMANCE METRICS</b>\n\n" +
                $"<b>Credits:</b> {credits}\n" +
                $"<b>Loyalty:</b> {loyaltyRating}\n" +
                $"<b>Efficiency:</b> {efficiency}%";
        }
        
        // Update security protocols text
        if (securityProtocolsText)
        {
            StringBuilder protocolText = new StringBuilder();
            
            protocolText.AppendLine("<b>AUTHORIZED ACCESS CODES</b>\n");
            
            // Create a grid of access codes
            if (accessCodes.Count > 0)
            {
                foreach (var code in accessCodes)
                {
                    protocolText.AppendLine($"• <color=#FFD700>{code}</color>");
                }
                
                protocolText.AppendLine("\n<size=10>Any other codes are INVALID and ships presenting them must be DENIED.</size>");
            }
            
            protocolText.AppendLine("\n<b>DAILY SECURITY DIRECTIVES</b>\n");
            
            foreach (var rule in securityRules)
            {
                protocolText.AppendLine($"• {rule}");
            }
            
            protocolText.AppendLine("\n<b>PROTOCOL REMINDERS</b>\n" +
                "1. Invalid access codes are NEVER to be approved\n" +
                "2. Ships from non-Imperium origins require additional scrutiny\n" +
                "3. Manifest discrepancies must be flagged for inspection\n\n" +
                "<size=10><color=#FF5555>Failure to follow protocols will result in disciplinary action</color></size>");
            
            securityProtocolsText.text = protocolText.ToString();
        }
        
        // Update intelligence briefing text
        if (intelligenceBriefingText)
        {
            StringBuilder briefingText = new StringBuilder();
            
            briefingText.AppendLine("<b>INTELLIGENCE REPORTS</b>\n");
            
            foreach (var newsItem in newsItems)
            {
                string[] parts = newsItem.Split(new string[] { ":</b>" }, StringSplitOptions.None);
                
                if (parts.Length > 1)
                {
                    string header = parts[0].Replace("<b>", "") + ":";
                    string content = parts[1];
                    
                    briefingText.AppendLine($"<b>{header}</b>");
                    briefingText.AppendLine($"{content}\n");
                }
                else
                {
                    briefingText.AppendLine(newsItem + "\n");
                }
            }
            
            // Add security alert based on day
            briefingText.AppendLine("<b><color=#FFD700>SECURITY ALERT</color></b>\n");
            
            if (day % 3 == 0)
            {
                briefingText.AppendLine("Intelligence suggests insurgent operatives may attempt to infiltrate using falsified credentials. Be particularly alert for:\n");
                briefingText.AppendLine("• Ships claiming Imperium origins but without proper authorization");
                briefingText.AppendLine("• Manifest inconsistencies indicating hidden cargo");
                briefingText.AppendLine("• Captains unable to provide detailed information about their origin or cargo");
            }
            else if (day % 3 == 1)
            {
                briefingText.AppendLine("Increased smuggling activity detected in nearby sectors. Thoroughly check all manifests for contraband and unauthorized cargo.");
            }
            else
            {
                briefingText.AppendLine("Surveillance has detected unusual communications from Trade Union vessels. Apply additional scrutiny to all non-Imperium ships.");
            }
            
            // Add insurgent activity warning
            briefingText.AppendLine("\n<b><color=#FFD700>⚠ INSURGENT ACTIVITY INCREASED</color></b>\n");
            briefingText.AppendLine("Recent checkpoint data indicates a 17% increase in suspicious activity. Maintain heightened vigilance in all security procedures.");
            
            intelligenceBriefingText.text = briefingText.ToString();
        }
        
        // Update station announcements text
        if (stationAnnouncementsText)
        {
            StringBuilder announcementText = new StringBuilder();
            
            announcementText.AppendLine("<b>STATION ANNOUNCEMENTS</b>\n");
            
            foreach (var announcement in announcements)
            {
                // Parse announcement to extract the title
                string[] parts = announcement.Split(new string[] { ":</b>" }, StringSplitOptions.None);
                
                if (parts.Length > 1)
                {
                    string title = parts[0].Replace("<b>", "");
                    string content = parts[1].Trim();
                    
                    // Add a random time to the announcement
                    int hour = UnityEngine.Random.Range(0, 24);
                    int minute = UnityEngine.Random.Range(0, 60);
                    string time = $"{hour:00}:{minute:00}";
                    
                    announcementText.AppendLine($"<b>{title}</b> <size=10><color=#888888>{time}</color></size>");
                    announcementText.AppendLine($"{content}\n");
                }
                else
                {
                    announcementText.AppendLine(announcement + "\n");
                }
            }
            
            // Add recreational activities in a grid layout
            announcementText.AppendLine("<b>RECREATIONAL ACTIVITIES</b>\n");
            
            // Format as a grid of activities
            announcementText.AppendLine("<b>Imperium Choir Practice</b>");
            announcementText.AppendLine("<size=10>Recreation Hall B • 19:00</size>\n");
            
            announcementText.AppendLine("<b>Tactical Simulations</b>");
            announcementText.AppendLine("<size=10>Training Room 4 • 20:30</size>\n");
            
            announcementText.AppendLine("<b>Officer's Lounge</b>");
            announcementText.AppendLine("<size=10>Level 7, Sector C • All Hours</size>\n");
            
            announcementText.AppendLine("<b>Physical Training</b>");
            announcementText.AppendLine("<size=10>Gymnasium • 06:00-22:00</size>");
            
            stationAnnouncementsText.text = announcementText.ToString();
        }
        
        // Update personnel updates text
        if (personnelUpdatesText)
        {
            StringBuilder personnelText = new StringBuilder();
            
            personnelText.AppendLine("<b>FAMILY STATUS</b>\n");
            
            foreach (var update in familyUpdates)
            {
                personnelText.AppendLine(update + "\n");
            }
            
            // Add benefits information
            personnelText.AppendLine("<b>OFFICER BENEFITS</b>\n");
            personnelText.AppendLine("Your family has the following active benefits:\n");
            
            // Get active benefits from family system if available
            if (familySystem != null)
            {
                FamilyStatusInfo status = familySystem.GetFamilyStatusInfo();
                
                if (status != null)
                {
                    // Premium quarters status
                    personnelText.AppendLine("<b>Premium Quarters</b>");
                    if (status.HasPremiumQuarters)
                        personnelText.AppendLine("<color=#4CAF50>Active</color>\n");
                    else
                        personnelText.AppendLine("<color=#FFA500>Inactive</color>\n");
                    
                    // Medical care status
                    personnelText.AppendLine("<b>Medical Coverage</b>");
                    if (status.HasMedicalCare)
                        personnelText.AppendLine("<color=#4CAF50>Active • Enhanced</color>\n");
                    else
                        personnelText.AppendLine("<color=#FFA500>Standard</color>\n");
                    
                    // Education status
                    personnelText.AppendLine("<b>Imperium Academy</b>");
                    if (status.HasImperialEducation && status.HasChild)
                        personnelText.AppendLine("<color=#4CAF50>Active • Child Enrolled</color>\n");
                    else
                        personnelText.AppendLine("<color=#FFA500>Standard</color>\n");
                    
                    // Recreation access
                    personnelText.AppendLine("<b>Recreation Access</b>");
                    personnelText.AppendLine("<color=#FFA500>Standard</color>");
                    
                    // Add warning if any family status is poor
                    if (status.ChildHealthStatus < 70 || status.SpouseHealthStatus < 70)
                    {
                        personnelText.AppendLine("\n\n<color=#FF5555>NOTICE: Family medical checkup recommended</color>");
                    }
                    
                    personnelText.AppendLine("\n<size=10>Imperium Family Services: Serving those who serve the Imperium</size>");
                }
                else
                {
                    // Fallback if no status available
                    personnelText.AppendLine("• Premium Quarters: <color=#FFA500>Inactive</color>");
                    personnelText.AppendLine("• Medical Care: <color=#FFA500>Standard</color>");
                    personnelText.AppendLine("• Education: <color=#FFA500>Standard</color>");
                }
            }
            else
            {
                // Fallback if no family system
                personnelText.AppendLine("• Standard Officer Package Active");
            }
            
            // Add personal messages section
            personnelText.AppendLine("\n<b>PERSONAL MESSAGES</b>\n");
            
            // Random message about quarters inspection
            personnelText.AppendLine("<b>Family Quarter Inspection</b> <size=10><color=#888888>07:30</color></size>");
            personnelText.AppendLine("Routine safety inspection completed. All systems nominal.\n");
            
            // Random message about duty roster
            personnelText.AppendLine("<b>Duty Roster Update</b> <size=10><color=#888888>06:15</color></size>");
            personnelText.AppendLine("Your requested weekend leave has been approved for next cycle.");
            
            personnelUpdatesText.text = personnelText.ToString();
        }
        
        // Update news ticker
        UpdateNewsTicker(day);
    }
    
    /// <summary>
    /// Helper method to get correct decisions count from GameManager
    /// </summary>
    private int GetCorrectDecisions()
    {
        // Use the getter method if available, otherwise return a safe default
        if (gameManager != null && gameManager.GetType().GetMethod("GetCorrectDecisions") != null)
        {
            // Try to use reflection to call the getter
            return (int)gameManager.GetType().GetMethod("GetCorrectDecisions").Invoke(gameManager, null);
        }
        
        // Fallback value
        return 0;
    }

    /// <summary>
    /// Helper method to get wrong decisions count from GameManager
    /// </summary>
    private int GetWrongDecisions()
    {
        // Use the getter method if available, otherwise return a safe default
        if (gameManager != null && gameManager.GetType().GetMethod("GetWrongDecisions") != null)
        {
            // Try to use reflection to call the getter
            return (int)gameManager.GetType().GetMethod("GetWrongDecisions").Invoke(gameManager, null);
        }
        
        // Fallback value
        return 0;
    }

    /// <summary>
    /// Generates news items based on previous game events
    /// </summary>
    private void GenerateNewsItems(int day)
    {
        newsItems.Clear();
        
        // First day just has standard news
        if (day <= 1)
        {
            newsItems.Add("<b>IMPERIAL CONTROL EXPANDING:</b> Starkiller Base operations reach full capacity as security operations intensify.");
            newsItems.Add("<b>INSURGENT CELL DISBANDED:</b> Imperium forces neutralize rebel operatives in Sector 7.");
            return;
        }
        
        // Get key decisions if game manager available
        List<string> playerDecisions = new List<string>();
        int correctDecisions = 0;
        int wrongDecisions = 0;
        bool acceptedBribe = false;
        bool capturedInsurgents = false;
        bool interceptedContraband = false;
        
        if (gameManager != null)
        {
            // Get decision counts
            correctDecisions = GetCorrectDecisions();
            wrongDecisions = GetWrongDecisions();
            
            // Check if specific events happened
            acceptedBribe = HasAcceptedBribe(gameManager);
            capturedInsurgents = HasCapturedInsurgents(gameManager);
            interceptedContraband = HasInterceptedContraband(gameManager);
        }
        
        // Generate news based on player performance
        if (correctDecisions > wrongDecisions * 2)
        {
            newsItems.Add("<b>SECURITY EFFICIENCY RISES:</b> Checkpoint processing shows marked improvement under current administration.");
        }
        else if (wrongDecisions > correctDecisions)
        {
            newsItems.Add("<b>SECURITY PROTOCOLS UNDER REVIEW:</b> Command notes concerning lapses in checkpoint procedures.");
        }
        
        // Add news about specific events
        if (capturedInsurgents)
        {
            newsItems.Add("<b>INSURGENT CELL DISMANTLED:</b> Rebel operatives captured thanks to vigilant security checkpoint protocols.");
        }
        
        if (interceptedContraband)
        {
            newsItems.Add("<b>CONTRABAND INTERCEPTED:</b> Weapons and illegal technology seized at security checkpoints.");
        }
        
        if (acceptedBribe)
        {
            // This is a subtle hint that bribery was detected
            newsItems.Add("<b>INTERNAL AFFAIRS INVESTIGATION:</b> Routine loyalty screening of security personnel underway in multiple sectors.");
        }
        
        // Always add some generic news if we don't have enough
        if (newsItems.Count < 2)
        {
            string[] genericNews = new string[]
            {
                "<b>IMPERIAL FLEET MANEUVERS:</b> Star Destroyers conduct training exercises in nearby systems.",
                "<b>SUPPLY CHAIN SECURED:</b> New convoy system ensures steady resource flow to Imperium installations.",
                "<b>LOYALTY INITIATIVES LAUNCHED:</b> New programs to recognize exceptional service to the Imperium.",
                "<b>REBEL ACTIVITY DIMINISHING:</b> Intelligence reports suggest insurgent resources critically low.",
                "<b>TECHNOLOGICAL UPGRADES:</b> Imperium facilities receiving enhanced security systems."
            };
            
            newsItems.Add(genericNews[UnityEngine.Random.Range(0, genericNews.Length)]);
        }
    }
    
    /// <summary>
    /// Generates station announcements for the day
    /// </summary>
    private List<string> GenerateStationAnnouncements(int day)
    {
        List<string> announcements = new List<string>();
        
        // Use templates if available
        if (stationAnnouncementTemplates != null && stationAnnouncementTemplates.Length > 0)
        {
            // Randomly select 3-4 announcements
            int count = UnityEngine.Random.Range(3, 5);
            
            // Create a copy of the templates array to avoid duplicates
            List<string> availableTemplates = new List<string>(stationAnnouncementTemplates);
            
            for (int i = 0; i < count && availableTemplates.Count > 0; i++)
            {
                // Pick a random template
                int index = UnityEngine.Random.Range(0, availableTemplates.Count);
                string template = availableTemplates[index];
                
                // Remove it from available templates to avoid duplicates
                availableTemplates.RemoveAt(index);
                
                // Add to announcements
                announcements.Add(template);
            }
        }
        else
        {
            // Fallback announcements
            announcements.Add("<b>Cafeteria Notice:</b> Bantha Burgers back on the menu in Officers' Mess");
            announcements.Add("<b>Power Management:</b> Scheduled maintenance in Residential Quadrant 3");
            announcements.Add("<b>Mandatory Loyalty Seminar:</b> Tomorrow at 0800 hours in Hall C");
        }
        
        // Add day-specific announcement
        if (day % 5 == 0)
        {
            announcements.Add("<b>Inspection Alert:</b> Imperium Command representatives will conduct routine inspections today");
        }
        else if (day % 7 == 0)
        {
            announcements.Add("<b>Security Drill:</b> Intruder response drill scheduled for Sector 8 at 1400 hours");
        }
        
        return announcements;
    }
    
    /// <summary>
    /// Generates family updates based on family system state
    /// </summary>
    private List<string> GenerateFamilyUpdates(int day)
    {
        List<string> updates = new List<string>();
        
        // Generate based on family system if available
        if (familySystem != null)
        {
            FamilyStatusInfo status = familySystem.GetFamilyStatusInfo();
            
            if (status != null)
            {
                // Child update
                if (status.HasChild)
                {
                    if (status.HasImperialEducation)
                    {
                        updates.Add("Your son's academic performance at Imperium Academy exceeds expectations");
                    }
                    else
                    {
                        updates.Add("Your son's school performance meets standard requirements");
                    }
                    
                    // Add health note if applicable
                    if (status.ChildHealthStatus < 70)
                    {
                        updates.Add("Medical notice: Your son visited the medical bay yesterday for respiratory issues");
                    }
                }
                
                // Spouse update
                if (status.HasSpouse)
                {
                    if (status.SpouseAssignment == "Imperium")
                    {
                        updates.Add("Your spouse has been commended for exceptional service in Imperium Communications");
                    }
                    else
                    {
                        updates.Add("Your spouse's work assignment continues as scheduled");
                    }
                    
                    // Add health note if applicable
                    if (status.SpouseHealthStatus < 70)
                    {
                        updates.Add("Medical notice: Your spouse has been placed on monitored medical treatment");
                    }
                }
                
                // Housing update
                if (status.HasPremiumQuarters)
                {
                    updates.Add("Residential notice: Your premium quarters passed security inspection");
                }
                else
                {
                    updates.Add("Residential notice: Standard quarters inspection scheduled for next cycle");
                }
            }
        }
        
        // Use templates if available and we need more updates
        if (updates.Count < 2 && familyUpdateTemplates != null && familyUpdateTemplates.Length > 0)
        {
            // Pick a random template
            string template = familyUpdateTemplates[UnityEngine.Random.Range(0, familyUpdateTemplates.Length)];
            updates.Add(template);
        }
        
        // Make sure we have at least one update
        if (updates.Count == 0)
        {
            updates.Add("Personal schedule: Your mandatory medical screening is due next week");
        }
        
        return updates;
    }
    
    /// <summary>
    /// Updates the scrolling news ticker based on the day
    /// </summary>
    private void UpdateNewsTicker(int day)
    {
        if (newsTicker == null)
        {
            Debug.LogWarning("Cannot update news ticker - newsTicker is null");
            return;
        }
        
        // Check if newsTicker's GameObject is active in hierarchy
        if (!newsTicker.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot update news ticker - GameObject is inactive");
            return;
        }
        
        string tickerText = "IMPERIUM NEWS NETWORK • ";
        
        // Add news items to ticker
        foreach (var news in newsItems)
        {
            // Strip HTML tags for ticker
            string plainNews = news.Replace("<b>", "").Replace("</b>", "");
            tickerText += plainNews + " • ";
        }
        
        // Add some standard propaganda
        tickerText += "STARKILLER BASE EFFICIENCY UP 7% THIS QUARTER • ";
        tickerText += "SUPREME LEADER COMMENDS SECURITY FORCES • ";
        tickerText += "LOYALTY IS REWARDED • ";
        tickerText += "REPORT SUSPICIOUS ACTIVITY IMMEDIATELY • ";
        
        // Repeat for seamless scrolling
        tickerText += "IMPERIUM NEWS NETWORK • ";
        
        newsTicker.text = tickerText;
        
        // Add animation for ticker if it doesn't have one already and we're on an active GameObject
        RectTransform tickerRect = newsTicker.GetComponent<RectTransform>();
        if (tickerRect != null && newsTicker.gameObject.activeInHierarchy)
        {
            // Check if there's already an animation
            Animator tickerAnimator = newsTicker.GetComponent<Animator>();
            if (tickerAnimator == null)
            {
                try {
                    // Add a simple animation directly via coroutine
                    StartCoroutine(AnimateNewsTicker(tickerRect));
                }
                catch (System.Exception e) {
                    Debug.LogError($"Failed to start news ticker animation: {e.Message}");
                }
            }
        }
    }
    
    /// <summary>
    /// Animate the news ticker with a sliding effect
    /// </summary>
    private IEnumerator AnimateNewsTicker(RectTransform tickerRect)
    {
        float scrollSpeed = 100f; // Pixels per second
        Vector3 startPosition = tickerRect.localPosition;
        float textWidth = newsTicker.preferredWidth;
        
        while (true)
        {
            // Move the ticker left
            tickerRect.localPosition -= new Vector3(scrollSpeed * Time.deltaTime, 0, 0);
            
            // If it has moved beyond its width, reset position
            if (tickerRect.localPosition.x < startPosition.x - textWidth)
            {
                tickerRect.localPosition = startPosition;
            }
            
            yield return null;
        }
    }
    
    /// <summary>
    /// Calculate current efficiency percentage for display
    /// </summary>
    private int CalculateEfficiency()
    {
        // Default to 100% if no game manager is available
        if (gameManager == null)
            return 100;
            
        // Calculate efficiency based on correct vs wrong decisions
        int totalDecisions = GetCorrectDecisions() + GetWrongDecisions();
        if (totalDecisions == 0)
            return 100; // No decisions made yet
            
        int efficiency = Mathf.RoundToInt((float)GetCorrectDecisions() / totalDecisions * 100f);
        
        // Clamp to 0-100 range
        return Mathf.Clamp(efficiency, 0, 100);
    }
    
    /// <summary>
    /// Method to be called by GameManager when daily briefing is completed
    /// </summary>
    public void OnDailyBriefingCompleted()
    {
        // Notify the GameManager that briefing has been completed
        if (gameManager != null)
        {
            gameManager.OnDailyBriefingCompleted();
        }
    }
    
    /// <summary>
    /// Helper method to check if player has accepted bribe
    /// </summary>
    private bool HasAcceptedBribe(GameManager manager)
    {
        // This is just a stub implementation - you would need to implement proper tracking in GameManager
        return false;
    }
    
    /// <summary>
    /// Helper method to check if player has captured insurgents
    /// </summary>
    private bool HasCapturedInsurgents(GameManager manager)
    {
        // This is just a stub implementation - you would need to implement proper tracking in GameManager
        return false;
    }
    
    /// <summary>
    /// Helper method to check if player has intercepted contraband
    /// </summary>
    private bool HasInterceptedContraband(GameManager manager)
    {
        // This is just a stub implementation - you would need to implement proper tracking in GameManager
        return false;
    }
}