using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using StarkillerBaseCommand;
using Starkiller.Core.ScriptableObjects;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages the Personal Data Log - a holographic interface showing end-of-day information
    /// from three feeds: Imperium News, Family Group Chat, and Frontier E-zine
    /// </summary>
    public class PersonalDataLogManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject dataLogPanel;
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private Button continueButton;
        
        [Header("Feed Sections")]
        [SerializeField] private Transform imperiumNewsSection;
        [SerializeField] private Transform familyChatSection;  
        [SerializeField] private Transform frontierEzineSection;
        
        [Header("Feed Templates")]
        [Tooltip("Assign these prefabs directly in Inspector, or leave empty for auto-detection")]
        [SerializeField] private GameObject newsEntryTemplate;      // For read-only entries
        [SerializeField] private GameObject familyActionTemplate;   // For interactive family entries
        [SerializeField] private GameObject videoEntryTemplate;     // For entries with videos
        
        [Header("Settings")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private int maxEntriesPerFeed = 10;
        
        [Header("Layout Settings")]
        [SerializeField] private float entrySpacing = 10f;
        [SerializeField] private float minEntryHeight = 100f;
        [SerializeField] private float preferredEntryHeight = 150f;
        [SerializeField] private bool autoFixLayoutOnStart = true;
        
        // Initialize in Awake instead of field initializer
        private RectOffset contentPadding;
        
        [Header("Content Database")]
        [SerializeField] private PersonalDataLogCollectionSO mainEntryCollection;
        [SerializeField] private PersonalDataLogCollectionSO randomEntryPool;
        [SerializeField] private int randomEntriesPerDay = 2;
        
        // Data structures
        private List<DataLogEntry> queuedEntries = new List<DataLogEntry>();
        private List<PersistentFamilyAction> persistentFamilyActions = new List<PersistentFamilyAction>();
        private HashSet<string> shownEntryIds = new HashSet<string>();
        
        // Dependencies
        private ConsequenceManager consequenceManager;
        private GameManager gameManager;
        private ImperialFamilySystem familySystem;
        private CreditsManager creditsManager;
        private DailyBriefingManager dailyBriefingManager;
        private FamilyPressureManager familyPressureManager;
        
        // Events
        public static event System.Action OnDataLogOpened;
        public static event System.Action OnDataLogClosed;
        public static event System.Action<FamilyActionData> OnFamilyActionTaken;
        
        // Guard against multiple rapid calls
        private bool isShowingDataLog = false;
        private float lastShowTime = 0f;
        private const float MIN_SHOW_INTERVAL = 1f; // Minimum 1 second between calls
        
        private void Awake()
        {
            // Initialize RectOffset in Awake instead of field initializer
            contentPadding = new RectOffset(20, 20, 20, 20);
            
            // Register with ServiceLocator
            ServiceLocator.Register<PersonalDataLogManager>(this);
            
            Debug.Log("[PersonalDataLogManager] Awake - Registered with ServiceLocator");
            
            // Auto-find dataLogPanel if not assigned
            if (dataLogPanel == null)
            {
                dataLogPanel = GameObject.Find("PersonalDataLog") ?? GameObject.Find("PersonalDataLogPanel");
                if (dataLogPanel != null)
                {
                    Debug.Log($"[PersonalDataLogManager] Auto-found dataLogPanel: {dataLogPanel.name}");
                }
                else
                {
                    Debug.LogError("[PersonalDataLogManager] dataLogPanel not assigned and could not be found!");
                }
            }
        }
        
        private void Start()
        {
            // Get dependencies
            consequenceManager = ConsequenceManager.Instance;
            
            // Try ServiceLocator as backup for ConsequenceManager
            if (consequenceManager == null)
            {
                consequenceManager = ServiceLocator.Get<ConsequenceManager>();
            }
            
            if (consequenceManager != null)
            {
                Debug.Log("[PersonalDataLogManager] ConsequenceManager found successfully");
            }
            else
            {
                Debug.LogError("[PersonalDataLogManager] ConsequenceManager NOT found - security incidents will not appear!");
            }
            
            gameManager = FindFirstObjectByType<GameManager>();
            familySystem = FindFirstObjectByType<ImperialFamilySystem>();
            creditsManager = ServiceLocator.Get<CreditsManager>();
            dailyBriefingManager = FindFirstObjectByType<DailyBriefingManager>();
            familyPressureManager = ServiceLocator.Get<FamilyPressureManager>();
            
            // Setup continue button
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }
            
            // Hide panel initially
            if (dataLogPanel != null)
            {
                dataLogPanel.SetActive(false);
            }
            
            if (enableDebugLogs)
                Debug.Log("[PersonalDataLogManager] Initialized and ready");
                
            // Auto-fix layout if enabled
            if (autoFixLayoutOnStart)
            {
                StartCoroutine(FixLayoutAfterDelay());
            }
        }
        
        /// <summary>
        /// Fix layout after a short delay to ensure UI is fully initialized
        /// </summary>
        private IEnumerator FixLayoutAfterDelay()
        {
            yield return new WaitForSeconds(0.1f);
            FixAllFeedLayouts();
        }
        
        /// <summary>
        /// Show the Personal Data Log with collected end-of-day information
        /// </summary>
        public void ShowDataLog()
        {
            if (dataLogPanel == null)
            {
                Debug.LogError("[PersonalDataLogManager] Data log panel is null!");
                return;
            }
            
            // Prevent multiple rapid calls that cause content to flicker
            float currentTime = Time.time;
            if (isShowingDataLog && (currentTime - lastShowTime) < MIN_SHOW_INTERVAL)
            {
                Debug.LogWarning($"[PersonalDataLogManager] ShowDataLog called too soon after previous call (last: {lastShowTime:F2}s, current: {currentTime:F2}s). Ignoring to prevent flicker.");
                return;
            }
            
            isShowingDataLog = true;
            lastShowTime = currentTime;
            
            // Reset shown entries for new day if using collections
            if (mainEntryCollection != null)
            {
                int currentDay = GetCurrentDay();
                // Only reset if we've moved to a new day
                if (currentDay > GetLastShownDay())
                {
                    ResetDailyEntries();
                    SetLastShownDay(currentDay);
                }
            }
            
            // Ensure Daily Performance Report panel is hidden before showing Personal Data Log
            var dailyReportManager = FindFirstObjectByType<DailyReportManager>();
            if (dailyReportManager != null && dailyReportManager.dailyReportPanel != null)
            {
                dailyReportManager.dailyReportPanel.SetActive(false);
                Debug.Log("[PersonalDataLogManager] Ensured Daily Performance Report panel is hidden");
            }
            
            // Clear any existing entries
            ClearAllFeedSections();
            
            // Collect all data for today
            CollectDailyData();
            
            // Populate all three feeds
            PopulateImperiumNews();
            PopulateFamilyChat();
            PopulateFrontierEzine();
            
            // Show the panel
            dataLogPanel.SetActive(true);
            
            // Update header - use DayProgressionManager for authoritative day value
            if (headerText != null)
            {
                var dayProgression = ServiceLocator.Get<DayProgressionManager>();
                int currentDay = 1; // Default
                
                if (dayProgression != null)
                {
                    currentDay = dayProgression.CurrentDay;
                    Debug.Log($"[PersonalDataLogManager] Header using DayProgressionManager day: {currentDay}");
                }
                else if (gameManager != null)
                {
                    currentDay = gameManager.currentDay;
                    Debug.Log($"[PersonalDataLogManager] Header falling back to GameManager day: {currentDay}");
                }
                
                headerText.text = $"PERSONAL DATA LOG - DAY {currentDay}";
                
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Showing data log for Day {currentDay}");
            }
            
            OnDataLogOpened?.Invoke();
            
            if (enableDebugLogs)
                Debug.Log($"[PersonalDataLogManager] Data log displayed with {queuedEntries.Count} entries");
            
            // If we got here successfully, the flag can be reset for future calls
            // (but not immediately, to prevent the next few milliseconds of duplicate calls)
            Invoke(nameof(ResetShowDataLogFlag), 0.5f);
        }
        
        /// <summary>
        /// Reset daily tracking for new day
        /// </summary>
        private void ResetDailyEntries()
        {
            // Don't clear shownEntryIds - we want to remember what news/stories have been shown
            // This prevents repetitive content that would break immersion
            
            // Clear the queued entries for the new day
            queuedEntries.Clear();
            
            if (enableDebugLogs)
                Debug.Log($"[PersonalDataLogManager] Reset daily entries for new day - keeping {shownEntryIds.Count} shown IDs to prevent repetition");
        }
        
        /// <summary>
        /// Reset the ShowDataLog guard flag to allow future calls
        /// </summary>
        private void ResetShowDataLogFlag()
        {
            isShowingDataLog = false;
            if (enableDebugLogs)
                Debug.Log("[PersonalDataLogManager] ShowDataLog guard flag reset - ready for next call");
        }
        
        /// <summary>
        /// Get/Set last shown day for tracking
        /// </summary>
        private int GetLastShownDay()
        {
            return PlayerPrefs.GetInt("PersonalDataLog_LastDay", 0);
        }
        
        private void SetLastShownDay(int day)
        {
            PlayerPrefs.SetInt("PersonalDataLog_LastDay", day);
        }
        
        /// <summary>
        /// Public method for other systems to trigger specific events
        /// </summary>
        public static void TriggerEvent(string eventId)
        {
            var instance = ServiceLocator.Get<PersonalDataLogManager>();
            if (instance != null)
            {
                instance.TriggerEventEntries(eventId);
            }
            else
            {
                Debug.LogWarning($"[PersonalDataLogManager] Could not find instance to trigger event: {eventId}");
            }
        }
        
        /// <summary>
        /// Collect all relevant data from various managers for today
        /// </summary>
        private void CollectDailyData()
        {
            try
            {
                // Ensure collections are initialized
                if (queuedEntries == null)
                {
                    queuedEntries = new List<DataLogEntry>();
                    Debug.LogWarning("[PersonalDataLogManager] queuedEntries was null - reinitializing");
                }
                
                if (shownEntryIds == null)
                {
                    shownEntryIds = new HashSet<string>();
                    Debug.LogWarning("[PersonalDataLogManager] shownEntryIds was null - reinitializing");
                }
                
                if (persistentFamilyActions == null)
                {
                    persistentFamilyActions = new List<PersistentFamilyAction>();
                    Debug.LogWarning("[PersonalDataLogManager] persistentFamilyActions was null - reinitializing");
                }
                
                queuedEntries.Clear();
                
                int currentDay = GetCurrentDay();
                
                Debug.Log($"[PersonalDataLogManager] Collecting daily data for day {currentDay}");
                
                // Always try to collect from ScriptableObjects (it will handle loading if needed)
                try
                {
                    CollectFromScriptableObjects(currentDay);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[PersonalDataLogManager] Error collecting ScriptableObject data: {ex.Message}");
                }
                
                // Generate family messages from FamilyPressureManager
                try
                {
                    this.GenerateFamilyMessages(currentDay);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[PersonalDataLogManager] Error generating family messages: {ex.Message}");
                }
                
                // Collect from ConsequenceManager
                try
                {
                    CollectConsequenceData();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[PersonalDataLogManager] Error collecting consequence data: {ex.Message}");
                }
                
                // Collect persistent family actions
                try
                {
                    CollectPersistentFamilyActions();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[PersonalDataLogManager] Error collecting family actions: {ex.Message}");
                }
                
                // Family messages now generated through GenerateFamilyMessages() extension method
                
                // Add random flavor entries if available
                if (randomEntryPool != null)
                {
                    try
                    {
                        AddRandomEntries(currentDay);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[PersonalDataLogManager] Error adding random entries: {ex.Message}");
                    }
                }
                
                Debug.Log($"[PersonalDataLogManager] Data collection completed - {queuedEntries.Count} total entries");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[PersonalDataLogManager] Critical error in CollectDailyData: {ex.Message}\nStack trace: {ex.StackTrace}");
                
                // Ensure we have a valid empty collection to prevent further errors
                if (queuedEntries == null)
                    queuedEntries = new List<DataLogEntry>();
            }
        }
        
        /// <summary>
        /// Collect consequence data from ConsequenceManager
        /// </summary>
        private void CollectConsequenceData()
        {
            if (consequenceManager == null) 
            {
                Debug.LogWarning("[PersonalDataLogManager] ConsequenceManager is null - cannot collect security incidents");
                return;
            }
            
            // Get today's incidents
            var todaysIncidents = GetTodaysIncidents();
            Debug.Log($"[PersonalDataLogManager] Found {todaysIncidents.Count} incidents for today");
            
            // Create Imperium News entries for security incidents
            foreach (var incident in todaysIncidents)
            {
                Debug.Log($"[PersonalDataLogManager] Adding incident to log: {incident.title} (Day {incident.dayOccurred})");
                
                var entry = new DataLogEntry
                {
                    feedType = FeedType.ImperiumNews,
                    headline = incident.title,
                    content = incident.description,
                    severity = (int)incident.severity,
                    timestamp = System.DateTime.Now,
                    requiresAction = false
                };
                
                queuedEntries.Add(entry);
            }
            
            if (todaysIncidents.Count == 0)
            {
                Debug.Log("[PersonalDataLogManager] No security incidents found for today - will show 'No incidents' message");
            }
        }
        
        /// <summary>
        /// Get today's incidents from ConsequenceManager
        /// </summary>
        private List<ConsequenceManager.IncidentRecord> GetTodaysIncidents()
        {
            if (consequenceManager == null)
                return new List<ConsequenceManager.IncidentRecord>();
            
            return consequenceManager.GetTodaysIncidents();
        }
        
        /// <summary>
        /// Collect persistent family actions that need resolution
        /// </summary>
        private void CollectPersistentFamilyActions()
        {
            // Process existing persistent actions and reduce their remaining days
            for (int i = persistentFamilyActions.Count - 1; i >= 0; i--)
            {
                var action = persistentFamilyActions[i];
                action.remainingDays--;
                
                if (action.remainingDays <= 0)
                {
                    // Action expired, remove it
                    persistentFamilyActions.RemoveAt(i);
                    continue;
                }
                
                // Add to family chat entries
                var entry = new DataLogEntry
                {
                    feedType = FeedType.FamilyChat,
                    headline = action.headline,
                    content = action.message,
                    requiresAction = true,
                    familyActionData = new FamilyActionData
                    {
                        actionText = action.actionText,
                        creditCost = action.creditCost,
                        actionId = action.actionId
                    },
                    timestamp = System.DateTime.Now
                };
                
                queuedEntries.Add(entry);
            }
        }
        
        
        /// <summary>
        /// Populate the Imperium News section
        /// </summary>
        private void PopulateImperiumNews()
        {
            var imperiumEntries = queuedEntries.Where(e => e.feedType == FeedType.ImperiumNews).Take(maxEntriesPerFeed);
            
            foreach (var entry in imperiumEntries)
            {
                CreateNewsEntry(entry, imperiumNewsSection);
            }
            
            // Add default message if no news
            if (!imperiumEntries.Any())
            {
                CreateDefaultEntry("No security incidents to report today.", imperiumNewsSection);
            }
        }
        
        /// <summary>
        /// Populate the Family Group Chat section  
        /// </summary>
        private void PopulateFamilyChat()
        {
            var familyEntries = queuedEntries.Where(e => e.feedType == FeedType.FamilyChat).Take(maxEntriesPerFeed);
            
            foreach (var entry in familyEntries)
            {
                // All family chat entries use the FamilyActionTemplate for messenger experience
                CreateFamilyActionEntry(entry, familyChatSection);
            }
            
            // Add default message if no family updates
            if (!familyEntries.Any())
            {
                CreateDefaultEntry("All family members are doing well.", familyChatSection);
            }
        }
        
        /// <summary>
        /// Populate the Frontier E-zine section
        /// </summary>
        private void PopulateFrontierEzine()
        {
            var frontierEntries = queuedEntries.Where(e => e.feedType == FeedType.FrontierEzine).Take(maxEntriesPerFeed);
            
            foreach (var entry in frontierEntries)
            {
                CreateNewsEntry(entry, frontierEzineSection);
            }
            
            // Add default message if no frontier news
            if (!frontierEntries.Any())
            {
                CreateDefaultEntry("Frontier remains quiet. All trade routes operational.", frontierEzineSection);
            }
        }
        
        /// <summary>
        /// Create a basic news entry
        /// </summary>
        private void CreateNewsEntry(DataLogEntry entry, Transform parent)
        {
            // Select appropriate template based on content
            GameObject templateToUse = SelectTemplateForEntry(entry);
            if (templateToUse == null || parent == null) return;
            
            GameObject entryObject = Instantiate(templateToUse, parent);
            
            // Activate the entry object immediately so all child components work properly
            entryObject.SetActive(true);
            
            // Enhanced text component finding with multiple strategies
            TMP_Text headlineText = FindTextComponentInTemplate(entryObject, "Headline");
            TMP_Text contentText = FindTextComponentInTemplate(entryObject, "Content");
            
            if (headlineText != null)
            {
                headlineText.text = entry.headline;
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Set headline: {entry.headline}");
            }
            else
            {
                Debug.LogWarning($"[PersonalDataLogManager] Could not find headline text in template {newsEntryTemplate.name}");
            }
            
            if (contentText != null)
            {
                contentText.text = entry.content;
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Set content: {entry.content}");
            }
            else
            {
                Debug.LogWarning($"[PersonalDataLogManager] Could not find content text in template {newsEntryTemplate.name}");
            }
            
            // Handle action button for family entries without actions
            if (entry.feedType == FeedType.FamilyChat && !entry.requiresAction)
            {
                Button actionButton = FindButtonComponentInTemplate(entryObject, "ActionButton");
                if (actionButton != null)
                {
                    // Hide the button for non-action family entries
                    actionButton.gameObject.SetActive(false);
                }
            }
            
            // Handle video if present
            VideoPlayer videoPlayer = entryObject.GetComponentInChildren<VideoPlayer>();
            UnityEngine.UI.RawImage rawImage = entryObject.GetComponentInChildren<UnityEngine.UI.RawImage>();
            
            if (entry.videoClip != null && videoPlayer != null)
            {
                Debug.Log($"[PersonalDataLogManager] Setting up video for: {entry.headline}");
                Debug.Log($"  - Video clip: {entry.videoClip.name}");
                Debug.Log($"  - VideoPlayer render mode: {videoPlayer.renderMode}");
                Debug.Log($"  - VideoPlayer target texture: {(videoPlayer.targetTexture != null ? videoPlayer.targetTexture.name : "NULL")}");
                Debug.Log($"  - VideoPlayer GameObject path: {GetGameObjectPath(videoPlayer.gameObject)}");
                
                // Ensure video player is properly configured
                videoPlayer.clip = entry.videoClip;
                
                // Create a unique RenderTexture for this video entry
                string textureName = $"VideoRT_{entry.headline}_{Time.time}";
                RenderTexture uniqueTexture = new RenderTexture(640, 360, 0);
                uniqueTexture.name = textureName;
                videoPlayer.targetTexture = uniqueTexture;
                
                // Activate the entire hierarchy to the VideoPlayer
                Transform current = videoPlayer.transform;
                while (current != null)
                {
                    current.gameObject.SetActive(true);
                    current = current.parent;
                    if (current == entryObject.transform) break; // Stop at our entry root
                }
                
                videoPlayer.enabled = true;
                
                // Configure for manual playback
                videoPlayer.playOnAwake = false;
                videoPlayer.waitForFirstFrame = true;
                
                // Only prepare if no custom thumbnail is provided
                if (entry.videoThumbnail == null)
                {
                    videoPlayer.Prepare();
                    
                    // When prepared, seek to first frame for thumbnail
                    videoPlayer.prepareCompleted += (vp) => {
                        vp.frame = 0;
                        Debug.Log($"[PersonalDataLogManager] Video first-frame thumbnail ready for: {entry.headline}");
                    };
                }
                else
                {
                    Debug.Log($"[PersonalDataLogManager] Using custom thumbnail, skipping video preparation for: {entry.headline}");
                }
                
                // Double-check the component state
                Debug.Log($"  - VideoPlayer enabled: {videoPlayer.enabled}");
                Debug.Log($"  - VideoPlayer GameObject active: {videoPlayer.gameObject.activeSelf}");
                Debug.Log($"  - VideoPlayer activeInHierarchy: {videoPlayer.gameObject.activeInHierarchy}");
                
                // Check RawImage setup
                if (rawImage != null)
                {
                    Debug.Log($"  - RawImage texture: {(rawImage.texture != null ? rawImage.texture.name : "NULL")}");
                    Debug.Log($"  - RawImage active: {rawImage.gameObject.activeSelf}");
                    Debug.Log($"  - RawImage size: {rawImage.rectTransform.sizeDelta}");
                    
                    // If we have a custom thumbnail, show it initially
                    if (entry.videoThumbnail != null)
                    {
                        rawImage.texture = entry.videoThumbnail.texture;
                        rawImage.gameObject.SetActive(true);
                        Debug.Log($"[PersonalDataLogManager] Using custom thumbnail for: {entry.headline}");
                    }
                    
                    // Ensure RawImage will use the video texture once playing
                    if (videoPlayer.targetTexture != null)
                    {
                        // If no custom thumbnail, use the video texture immediately
                        if (entry.videoThumbnail == null)
                        {
                            rawImage.texture = videoPlayer.targetTexture;
                        }
                        rawImage.gameObject.SetActive(true);
                        Debug.Log($"[PersonalDataLogManager] Connected RawImage to unique video texture: {videoPlayer.targetTexture.name}");
                    }
                    else
                    {
                        Debug.LogError("[PersonalDataLogManager] VideoPlayer has no target texture assigned!");
                    }
                }
                else
                {
                    Debug.LogWarning("[PersonalDataLogManager] No RawImage found for video display!");
                }
                
                // Add Play button for user-controlled video playback
                SetupVideoPlayButton(entryObject, videoPlayer, entry);
                
                // Add error handling
                videoPlayer.errorReceived += (vp, message) => {
                    Debug.LogError($"[PersonalDataLogManager] Video error: {message}");
                };
                
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Video setup complete for: {entry.videoClip.name}");
            }
            else
            {
                // No video for this entry - hide video components if they exist
                if (videoPlayer != null)
                {
                    videoPlayer.gameObject.SetActive(false);
                    Debug.Log($"[PersonalDataLogManager] Hidden VideoPlayer for non-video entry: {entry.headline}");
                }
                if (rawImage != null)
                {
                    rawImage.gameObject.SetActive(false);
                    Debug.Log($"[PersonalDataLogManager] Hidden RawImage for non-video entry: {entry.headline}");
                }
                
                // Hide Play button for non-video entries
                Button playButton = FindButtonComponentInTemplate(entryObject, "PlayButton");
                if (playButton != null)
                {
                    playButton.gameObject.SetActive(false);
                    Debug.Log($"[PersonalDataLogManager] Hidden Play button for non-video entry: {entry.headline}");
                }
            }
            
            // Apply layout fix to new entry
            FixEntryLayout(entryObject);
            
            // Force immediate layout rebuild
            if (parent != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
            }
        }
        
        /// <summary>
        /// Create a family action entry with interactive button(s)
        /// </summary>
        private void CreateFamilyActionEntry(DataLogEntry entry, Transform parent)
        {
            if (familyActionTemplate == null || parent == null) return;
            
            GameObject entryObject = Instantiate(familyActionTemplate, parent);
            
            // Enhanced component finding for family action templates
            TMP_Text headlineText = FindTextComponentInTemplate(entryObject, "Headline");
            TMP_Text contentText = FindTextComponentInTemplate(entryObject, "Content");
            
            if (headlineText != null)
            {
                headlineText.text = entry.headline;
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Set family action headline: {entry.headline}");
            }
            else
            {
                Debug.LogWarning($"[PersonalDataLogManager] Could not find headline text in family action template");
            }
            
            if (contentText != null)
            {
                contentText.text = entry.content;
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Set family action content: {entry.content}");
            }
            else
            {
                Debug.LogWarning($"[PersonalDataLogManager] Could not find content text in family action template");
            }
            
            // Check if we have multiple button configurations
            if (entry.familyActionData != null && entry.familyActionData.actionButtons != null && entry.familyActionData.actionButtons.Length > 0)
            {
                SetupMultipleChoiceButtons(entryObject, entry);
            }
            else
            {
                // Fall back to single button setup
                Button actionButton = FindButtonComponentInTemplate(entryObject, "ActionButton");
                if (actionButton != null)
                {
                    SetupDynamicActionButton(actionButton, entry);
                }
                else
                {
                    Debug.LogWarning($"[PersonalDataLogManager] Could not find action button in family action template");
                }
            }
            
            // Handle video if present (same as news entries)
            VideoPlayer videoPlayer = entryObject.GetComponentInChildren<VideoPlayer>();
            if (videoPlayer != null)
            {
                if (entry.videoClip != null)
                {
                    videoPlayer.clip = entry.videoClip;
                    videoPlayer.gameObject.SetActive(true);
                    videoPlayer.Play();
                    
                    if (enableDebugLogs)
                        Debug.Log($"[PersonalDataLogManager] Playing video in family action: {entry.videoClip.name}");
                }
                else
                {
                    videoPlayer.gameObject.SetActive(false);
                }
            }
            
            // Apply layout fix to new entry
            FixEntryLayout(entryObject);
            
            // Force immediate layout rebuild
            if (parent != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
            }
            
            entryObject.SetActive(true);
        }
        
        /// <summary>
        /// Create a default entry when no content is available
        /// </summary>
        private void CreateDefaultEntry(string message, Transform parent)
        {
            if (newsEntryTemplate == null || parent == null) return;
            
            GameObject entryObject = Instantiate(newsEntryTemplate, parent);
            
            // Use enhanced text finding for default entries too
            TMP_Text contentText = FindTextComponentInTemplate(entryObject, "Content");
            if (contentText != null)
            {
                contentText.text = message;
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Set default entry: {message}");
            }
            else
            {
                Debug.LogWarning($"[PersonalDataLogManager] Could not find content text for default entry");
            }
            
            entryObject.SetActive(true);
        }
        
        /// <summary>
        /// Select the appropriate template for an entry
        /// </summary>
        private GameObject SelectTemplateForEntry(DataLogEntry entry)
        {
            // If entry has video and video template is available, use it
            if (entry.videoClip != null && videoEntryTemplate != null)
            {
                Debug.Log($"[PersonalDataLogManager] Using VideoEntryTemplate for: {entry.headline}");
                return videoEntryTemplate;
            }
            else if (entry.videoClip != null && videoEntryTemplate == null)
            {
                Debug.LogWarning($"[PersonalDataLogManager] Entry has video but VideoEntryTemplate is null! Using NewsEntryTemplate instead for: {entry.headline}");
            }
            
            // For family chat entries that require action, use family action template
            if (entry.feedType == FeedType.FamilyChat && entry.requiresAction && familyActionTemplate != null)
            {
                Debug.Log($"[PersonalDataLogManager] Using FamilyActionTemplate for: {entry.headline}");
                return familyActionTemplate;
            }
            
            // Default to news template for all other entries (including non-action family entries)
            Debug.Log($"[PersonalDataLogManager] Using NewsEntryTemplate for: {entry.headline}");
            return newsEntryTemplate;
        }
        
        /// <summary>
        /// Handle family action button click
        /// </summary>
        private void HandleFamilyAction(FamilyActionData actionData)
        {
            if (actionData == null || creditsManager == null) return;
            
            // Check if player has enough credits
            if (creditsManager.CurrentCredits < actionData.creditCost)
            {
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Not enough credits for {actionData.actionText}");
                return;
            }
            
            // Deduct credits immediately
            creditsManager.DeductCredits(actionData.creditCost, $"Family Action: {actionData.actionText}");
            
            // Remove the persistent family action
            persistentFamilyActions.RemoveAll(p => p.actionId == actionData.actionId);
            
            // Trigger event
            OnFamilyActionTaken?.Invoke(actionData);
            
            if (enableDebugLogs)
                Debug.Log($"[PersonalDataLogManager] Family action taken: {actionData.actionText} for {actionData.creditCost} credits");
            
            // Refresh the display
            ShowDataLog();
        }
        
        /// <summary>
        /// Clear all feed sections
        /// </summary>
        private void ClearAllFeedSections()
        {
            ClearFeedSection(imperiumNewsSection);
            ClearFeedSection(familyChatSection);
            ClearFeedSection(frontierEzineSection);
        }
        
        /// <summary>
        /// Clear a specific feed section
        /// </summary>
        private void ClearFeedSection(Transform section)
        {
            if (section == null) return;
            
            for (int i = section.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(section.GetChild(i).gameObject);
            }
        }
        
        /// <summary>
        /// Add a new persistent family action that will appear until resolved
        /// </summary>
        public void AddPersistentFamilyAction(string headline, string message, string actionText, 
                                            int creditCost, string actionId, int duration = 7)
        {
            var action = new PersistentFamilyAction
            {
                headline = headline,
                message = message,
                actionText = actionText,
                creditCost = creditCost,
                actionId = actionId,
                remainingDays = duration
            };
            
            persistentFamilyActions.Add(action);
            
            if (enableDebugLogs)
                Debug.Log($"[PersonalDataLogManager] Added persistent family action: {headline}");
        }
        
        /// <summary>
        /// Continue button clicked - close data log and show daily briefing panel
        /// </summary>
        private void OnContinueClicked()
        {
            if (dataLogPanel != null)
            {
                dataLogPanel.SetActive(false);
            }
            
            // Reset the guard flag when closing
            isShowingDataLog = false;
            
            OnDataLogClosed?.Invoke();
            
            // Show the Daily Briefing Panel instead of directly starting the day
            if (dailyBriefingManager != null)
            {
                // Get day directly from DayProgressionManager for accuracy
                var dayProgression = ServiceLocator.Get<DayProgressionManager>();
                int currentDay = 1; // Default fallback
                
                if (dayProgression != null)
                {
                    currentDay = dayProgression.CurrentDay;
                    Debug.Log($"[PersonalDataLogManager] Using DayProgressionManager day: {currentDay}");
                }
                else if (gameManager != null)
                {
                    currentDay = gameManager.currentDay;
                    Debug.Log($"[PersonalDataLogManager] Falling back to GameManager day: {currentDay}");
                }
                
                dailyBriefingManager.ShowDailyBriefing(currentDay);
            }
            else
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[PersonalDataLogManager] DailyBriefingManager or GameManager not found, falling back to direct day start");
                
                // Fallback to direct day start if components not available
                if (gameManager != null)
                {
                    Debug.Log("[PersonalDataLogManager] Falling back to direct StartDay() - should not increment day");
                    gameManager.StartDay();
                }
            }
            
            if (enableDebugLogs)
                Debug.Log("[PersonalDataLogManager] Data log closed, showing daily briefing panel");
        }
        
        /// <summary>
        /// Public method to add a log entry from external systems
        /// </summary>
        public void AddLogEntry(DataLogEntry entry)
        {
            if (entry != null)
            {
                queuedEntries.Add(entry);
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Added entry: {entry.headline}");
            }
        }
        
        /// <summary>
        /// Collect entries from ScriptableObject collections
        /// </summary>
        private void CollectFromScriptableObjects(int currentDay)
        {
            Debug.Log($"[PersonalDataLogManager] CollectFromScriptableObjects called for day {currentDay}");
            
            // Try to load the collection from Resources if not assigned in Inspector
            if (mainEntryCollection == null)
            {
                Debug.Log("[PersonalDataLogManager] mainEntryCollection not assigned, attempting to load from Resources...");
                // The correct path - note Collections subfolder
                mainEntryCollection = Resources.Load<PersonalDataLogCollectionSO>("_ScriptableObjects/PersonalDataLog/Collections/MasterDataLogCollection");
                
                if (mainEntryCollection == null)
                {
                    Debug.LogWarning("[PersonalDataLogManager] Could not find MasterDataLogCollection in Resources! Trying to load individual entries...");
                    // Try loading individual entries instead
                    LoadIndividualScriptableObjects(currentDay);
                    return;
                }
                else
                {
                    Debug.Log("[PersonalDataLogManager] Successfully loaded MasterDataLogCollection from Resources");
                }
            }
            
            var dayEntries = mainEntryCollection.GetEntriesForDay(currentDay, shownEntryIds);
            
            if (dayEntries == null)
            {
                Debug.LogWarning($"[PersonalDataLogManager] GetEntriesForDay returned null for day {currentDay}");
                return;
            }
            
            Debug.Log($"[PersonalDataLogManager] Found {dayEntries.Count} potential SO entries for day {currentDay}");
            
            int addedCount = 0;
            int nullEntriesSkipped = 0;
            foreach (var soEntry in dayEntries)
            {
                if (soEntry == null)
                {
                    nullEntriesSkipped++;
                    Debug.LogWarning("[PersonalDataLogManager] Found null entry in dayEntries collection - skipping");
                    continue;
                }
                
                Debug.Log($"[PersonalDataLogManager] Checking SO entry: {soEntry.EntryName} (minDay: {soEntry.minDay}, maxDay: {soEntry.maxDay}, chance: {soEntry.appearanceChance})");
                
                if (soEntry.ShouldAppear())
                {
                    var entry = soEntry.ToDataLogEntry();
                    if (entry != null)
                    {
                        queuedEntries.Add(entry);
                        shownEntryIds.Add(soEntry.EntryId);
                        addedCount++;
                        
                        Debug.Log($"[PersonalDataLogManager] ✓ Added SO entry: {entry.headline} (FeedType: {entry.feedType})");
                    }
                    else
                    {
                        Debug.LogWarning($"[PersonalDataLogManager] ToDataLogEntry returned null for {soEntry.EntryName}");
                    }
                }
                else
                {
                    Debug.Log($"[PersonalDataLogManager] ✗ SO entry {soEntry.EntryName} failed ShouldAppear() check");
                }
            }
            
            if (nullEntriesSkipped > 0)
            {
                Debug.LogWarning($"[PersonalDataLogManager] Skipped {nullEntriesSkipped} null entries in collection - consider cleaning up the MasterDataLogCollection");
            }
            
            Debug.Log($"[PersonalDataLogManager] Total SO entries added: {addedCount}");
        }
        
        /// <summary>
        /// Add random flavor entries for variety
        /// </summary>
        private void AddRandomEntries(int currentDay)
        {
            var randomEntries = randomEntryPool.GetRandomEntries(randomEntriesPerDay);
            
            foreach (var soEntry in randomEntries)
            {
                if (soEntry.IsValidForDay(currentDay) && soEntry.ShouldAppear())
                {
                    var entry = soEntry.ToDataLogEntry();
                    queuedEntries.Add(entry);
                    shownEntryIds.Add(soEntry.EntryId);
                    
                    if (enableDebugLogs)
                        Debug.Log($"[PersonalDataLogManager] Added random entry: {entry.headline}");
                }
            }
        }
        
        /// <summary>
        /// Handle event-triggered entries
        /// </summary>
        public void TriggerEventEntries(string eventId)
        {
            if (mainEntryCollection == null) return;
            
            int currentDay = GetCurrentDay();
            var triggeredEntries = mainEntryCollection.GetEntriesForEvent(eventId, currentDay);
            
            foreach (var soEntry in triggeredEntries)
            {
                if (soEntry.ShouldAppear())
                {
                    var entry = soEntry.ToDataLogEntry();
                    queuedEntries.Add(entry);
                    shownEntryIds.Add(soEntry.EntryId);
                    
                    if (enableDebugLogs)
                        Debug.Log($"[PersonalDataLogManager] Added triggered entry: {entry.headline} for event: {eventId}");
                }
            }
        }
        
        /// <summary>
        /// Get current day from appropriate manager
        /// </summary>
        private int GetCurrentDay()
        {
            var dayProgression = ServiceLocator.Get<DayProgressionManager>();
            if (dayProgression != null)
            {
                return dayProgression.CurrentDay;
            }
            else if (gameManager != null)
            {
                return gameManager.currentDay;
            }
            return 1; // Default fallback
        }
        
        /// <summary>
        /// Test method to add sample data for development
        /// </summary>
        [ContextMenu("Test: Add Sample Data")]
        public void AddSampleDataForTesting()
        {
            // Add sample security incident
            var securityEntry = new DataLogEntry
            {
                feedType = FeedType.ImperiumNews,
                headline = "Security Breach at Docking Bay 7",
                content = "Contraband detected in approved cargo shipment. 3 Imperial casualties reported.",
                severity = 2,
                timestamp = System.DateTime.Now,
                requiresAction = false
            };
            queuedEntries.Add(securityEntry);
            
            // Add sample family action
            AddPersistentFamilyAction(
                "Son in Imperial Detention", 
                "Your son has been detained for questioning regarding rebel sympathies. Immediate payment required for release.",
                "Pay Release Fee",
                100,
                "son_detention_release",
                5
            );
            
            // Add sample family update
            var familyEntry = new DataLogEntry
            {
                feedType = FeedType.FamilyChat,
                headline = "Partner Promotion",
                content = "Congratulations! Your partner has been promoted to Senior Technician. Salary increase effective immediately.",
                timestamp = System.DateTime.Now,
                requiresAction = false
            };
            queuedEntries.Add(familyEntry);
            
            // Add sample frontier news
            var frontierEntry = new DataLogEntry
            {
                feedType = FeedType.FrontierEzine,
                headline = "Trade Route Disruption",
                content = "Unconfirmed reports of pirate activity in the Outer Rim affecting supply deliveries.",
                timestamp = System.DateTime.Now,
                requiresAction = false
            };
            queuedEntries.Add(frontierEntry);
            
            if (enableDebugLogs)
                Debug.Log("[PersonalDataLogManager] Sample data added for testing");
        }
        
        /// <summary>
        /// Enhanced method to find text components in complex template hierarchies
        /// </summary>
        private TMP_Text FindTextComponentInTemplate(GameObject template, string targetName)
        {
            // Strategy 1: Direct child lookup (original behavior)
            var directChild = template.transform.Find(targetName);
            if (directChild != null)
            {
                var directText = directChild.GetComponent<TMP_Text>();
                if (directText != null) return directText;
            }
            
            // Strategy 2: Recursive search by exact name
            var recursiveMatch = FindTransformByNameRecursive(template.transform, targetName);
            if (recursiveMatch != null)
            {
                var recursiveText = recursiveMatch.GetComponent<TMP_Text>();
                if (recursiveText != null) return recursiveText;
            }
            
            // Strategy 3: Search all TMP_Text components for name matches
            var allTexts = template.GetComponentsInChildren<TMP_Text>(true);
            foreach (var text in allTexts)
            {
                string name = text.gameObject.name.ToLower();
                string target = targetName.ToLower();
                
                if (name.Contains(target))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[PersonalDataLogManager] Found {targetName} text at: {GetHierarchyPath(text.transform)}");
                    return text;
                }
            }
            
            // Strategy 4: Heuristic matching for common patterns
            foreach (var text in allTexts)
            {
                string name = text.gameObject.name.ToLower();
                
                if ((targetName.ToLower() == "headline" && (name.Contains("title") || name.Contains("header"))) ||
                    (targetName.ToLower() == "content" && (name.Contains("body") || name.Contains("text"))))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[PersonalDataLogManager] Found {targetName} text via heuristic at: {GetHierarchyPath(text.transform)}");
                    return text;
                }
            }
            
            if (enableDebugLogs)
            {
                Debug.LogWarning($"[PersonalDataLogManager] Could not find '{targetName}' text component in template. Available TMP_Text components:");
                foreach (var text in allTexts)
                {
                    Debug.LogWarning($"  - {GetHierarchyPath(text.transform)} (text: '{text.text.Substring(0, Mathf.Min(20, text.text.Length))}...')");
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Recursively find transform by name
        /// </summary>
        private Transform FindTransformByNameRecursive(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return child;
                }
                
                var recursive = FindTransformByNameRecursive(child, name);
                if (recursive != null) return recursive;
            }
            return null;
        }
        
        /// <summary>
        /// Get hierarchy path for debugging
        /// </summary>
        private string GetHierarchyPath(Transform transform)
        {
            string path = transform.name;
            Transform parent = transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
        
        /// <summary>
        /// Enhanced method to find button components in complex template hierarchies
        /// </summary>
        private Button FindButtonComponentInTemplate(GameObject template, string targetName)
        {
            // Strategy 1: Direct child lookup
            var directChild = template.transform.Find(targetName);
            if (directChild != null)
            {
                var directButton = directChild.GetComponent<Button>();
                if (directButton != null) return directButton;
            }
            
            // Strategy 2: Recursive search by exact name
            var recursiveMatch = FindTransformByNameRecursive(template.transform, targetName);
            if (recursiveMatch != null)
            {
                var recursiveButton = recursiveMatch.GetComponent<Button>();
                if (recursiveButton != null) return recursiveButton;
            }
            
            // Strategy 3: Search all Button components for name matches
            var allButtons = template.GetComponentsInChildren<Button>(true);
            foreach (var button in allButtons)
            {
                string name = button.gameObject.name.ToLower();
                string target = targetName.ToLower();
                
                if (name.Contains(target) || name.Contains("action") || name.Contains("button"))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[PersonalDataLogManager] Found {targetName} button at: {GetHierarchyPath(button.transform)}");
                    return button;
                }
            }
            
            // Strategy 4: Return first button as fallback
            if (allButtons.Length > 0)
            {
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Using fallback button: {GetHierarchyPath(allButtons[0].transform)}");
                return allButtons[0];
            }
            
            return null;
        }
        
        /// <summary>
        /// Fix layout for all feed sections
        /// </summary>
        [ContextMenu("Fix All Feed Layouts")]
        public void FixAllFeedLayouts()
        {
            Debug.Log("[PersonalDataLogManager] Fixing layouts for all feed sections...");
            
            if (imperiumNewsSection != null)
                FixFeedSectionLayout(imperiumNewsSection, "Imperium News");
                
            if (familyChatSection != null)
                FixFeedSectionLayout(familyChatSection, "Family Chat");
                
            if (frontierEzineSection != null)
                FixFeedSectionLayout(frontierEzineSection, "Frontier E-zine");
                
            Debug.Log("[PersonalDataLogManager] Layout fixes completed!");
        }
        
        /// <summary>
        /// Fix layout for a specific feed section
        /// </summary>
        private void FixFeedSectionLayout(Transform section, string sectionName)
        {
            if (section == null) return;
            
            Debug.Log($"[PersonalDataLogManager] Fixing layout for {sectionName}...");
            
            // Fix corrupted viewport if this is a scrollview section
            FixViewportCorruption(section);
            
            // Fix mask and scrolling issues
            FixScrollMaskIssues(section);
            
            // Remove any Horizontal Layout Group
            HorizontalLayoutGroup horizontalLayout = section.GetComponent<HorizontalLayoutGroup>();
            if (horizontalLayout != null)
            {
                Debug.Log($"[PersonalDataLogManager] Removing Horizontal Layout Group from {sectionName}");
                DestroyImmediate(horizontalLayout);
            }
            
            // Add or configure Vertical Layout Group
            VerticalLayoutGroup verticalLayout = section.GetComponent<VerticalLayoutGroup>();
            if (verticalLayout == null)
            {
                Debug.Log($"[PersonalDataLogManager] Adding Vertical Layout Group to {sectionName}");
                verticalLayout = section.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            
            // Configure Vertical Layout Group
            verticalLayout.spacing = entrySpacing;
            verticalLayout.padding = contentPadding;
            verticalLayout.childAlignment = TextAnchor.UpperCenter;
            verticalLayout.childControlWidth = true;
            verticalLayout.childControlHeight = true;
            verticalLayout.childForceExpandWidth = true;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childScaleWidth = false;
            verticalLayout.childScaleHeight = false;
            
            // Remove Content Size Fitter from parent sections to prevent conflicts
            ContentSizeFitter sizeFitter = section.GetComponent<ContentSizeFitter>();
            if (sizeFitter != null)
            {
                Debug.Log($"[PersonalDataLogManager] Removing conflicting Content Size Fitter from {sectionName}");
                DestroyImmediate(sizeFitter);
            }
            
            // Fix all existing entries in this section
            foreach (Transform child in section)
            {
                FixEntryLayout(child.gameObject);
            }
        }
        
        /// <summary>
        /// Fix mask and scrolling issues
        /// </summary>
        private void FixScrollMaskIssues(Transform section)
        {
            // Find scroll view components
            Transform scrollView = section;
            Transform current = section;
            
            // Search up hierarchy for ScrollRect
            for (int i = 0; i < 3; i++)
            {
                if (current == null) break;
                
                ScrollRect scrollRect = current.GetComponent<ScrollRect>();
                if (scrollRect != null)
                {
                    scrollView = current;
                    break;
                }
                current = current.parent;
            }
            
            // Fix common scroll/mask issues
            ScrollRect scroll = scrollView.GetComponent<ScrollRect>();
            if (scroll != null)
            {
                Transform viewport = scrollView.Find("Viewport");
                if (viewport != null)
                {
                    // Ensure viewport has mask component
                    Mask mask = viewport.GetComponent<Mask>();
                    if (mask == null)
                    {
                        mask = viewport.gameObject.AddComponent<Mask>();
                        Debug.Log($"[PersonalDataLogManager] Added missing Mask component to viewport");
                    }
                    mask.showMaskGraphic = false;
                    
                    // Ensure content is properly assigned
                    Transform content = viewport.Find("Content");
                    if (content != null && scroll.content == null)
                    {
                        scroll.content = content.GetComponent<RectTransform>();
                        Debug.Log($"[PersonalDataLogManager] Fixed ScrollRect content assignment");
                    }
                    
                    // Reset content position if it's outside bounds
                    if (scroll.content != null)
                    {
                        Vector2 pos = scroll.content.anchoredPosition;
                        if (pos.y > 0) // Content moved above viewport
                        {
                            scroll.content.anchoredPosition = new Vector2(pos.x, 0);
                            Debug.Log($"[PersonalDataLogManager] Reset content position to visible area");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Fix corrupted viewport RectTransform values
        /// </summary>
        private void FixViewportCorruption(Transform section)
        {
            // Find viewport components in this section or parent hierarchy
            Transform current = section;
            for (int i = 0; i < 3; i++) // Check up to 3 levels up
            {
                if (current == null) break;
                
                // Check for viewport in current level or children
                Transform viewport = current.Find("Viewport");
                if (viewport != null)
                {
                    RectTransform viewportRect = viewport.GetComponent<RectTransform>();
                    if (viewportRect != null && HasCorruptedValues(viewportRect))
                    {
                        Debug.Log($"[PersonalDataLogManager] Fixing corrupted viewport in {current.name}");
                        
                        // Reset to safe values
                        viewportRect.anchorMin = Vector2.zero;
                        viewportRect.anchorMax = Vector2.one;
                        viewportRect.sizeDelta = Vector2.zero;
                        viewportRect.anchoredPosition = Vector2.zero;
                        viewportRect.localPosition = Vector3.zero;
                        viewportRect.localScale = Vector3.one;
                    }
                }
                
                current = current.parent;
            }
        }
        
        /// <summary>
        /// Check if RectTransform has corrupted floating point values
        /// </summary>
        private bool HasCorruptedValues(RectTransform rect)
        {
            return Mathf.Abs(rect.anchoredPosition.x) > 1000000 ||
                   Mathf.Abs(rect.anchoredPosition.y) > 1000000 ||
                   Mathf.Abs(rect.sizeDelta.x) > 1000000 ||
                   Mathf.Abs(rect.sizeDelta.y) > 1000000 ||
                   float.IsNaN(rect.anchoredPosition.x) ||
                   float.IsNaN(rect.anchoredPosition.y);
        }
        
        /// <summary>
        /// Fix layout for a single entry
        /// </summary>
        private void FixEntryLayout(GameObject entry)
        {
            if (entry == null) return;
            
            // Add Layout Element if missing
            LayoutElement layoutElement = entry.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = entry.AddComponent<LayoutElement>();
            }
            
            // Configure Layout Element - remove forced sizing to let natural layout work
            layoutElement.minWidth = -1;
            layoutElement.preferredWidth = -1;
            layoutElement.minHeight = -1;
            layoutElement.preferredHeight = -1;
            layoutElement.flexibleHeight = 1;
            
            // Add Content Size Fitter if needed
            ContentSizeFitter entryFitter = entry.GetComponent<ContentSizeFitter>();
            if (entryFitter == null)
            {
                entryFitter = entry.AddComponent<ContentSizeFitter>();
            }
            
            entryFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            entryFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
            
            // Fix text components to wrap properly
            TextMeshProUGUI[] textComponents = entry.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in textComponents)
            {
                text.overflowMode = TextOverflowModes.Overflow;
                text.enableWordWrapping = true;
                
                // Ensure text has proper sizing
                RectTransform textRect = text.GetComponent<RectTransform>();
                if (textRect != null)
                {
                    // Only set anchors to stretch if not already properly sized
                    if (textRect.sizeDelta.x <= 0 || textRect.sizeDelta.y <= 0)
                    {
                        textRect.anchorMin = new Vector2(0, 0);
                        textRect.anchorMax = new Vector2(1, 1);
                        textRect.sizeDelta = Vector2.zero;
                    }
                    
                    // Let text use natural sizing - remove forced width constraints
                }
            }
        }
        
        /// <summary>
        /// Setup multiple choice buttons for complex decisions
        /// </summary>
        private void SetupMultipleChoiceButtons(GameObject entryObject, DataLogEntry entry)
        {
            // Find button container or create one
            Transform buttonContainer = entryObject.transform.Find("ButtonContainer");
            if (buttonContainer == null)
            {
                // Look for existing button to use as template
                Button existingButton = FindButtonComponentInTemplate(entryObject, "ActionButton");
                if (existingButton != null)
                {
                    buttonContainer = existingButton.transform.parent;
                }
                else
                {
                    Debug.LogWarning("[PersonalDataLogManager] No button container found for multiple choices");
                    return;
                }
            }
            
            // Get first button as template
            Button templateButton = buttonContainer.GetComponentInChildren<Button>();
            if (templateButton == null)
            {
                Debug.LogWarning("[PersonalDataLogManager] No template button found");
                return;
            }
            
            // Clear existing buttons except template
            foreach (Transform child in buttonContainer)
            {
                if (child != templateButton.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            
            // Create buttons for each choice
            for (int i = 0; i < entry.familyActionData.actionButtons.Length; i++)
            {
                var buttonConfig = entry.familyActionData.actionButtons[i];
                
                // Use template for first button, create new for others
                Button button = (i == 0) ? templateButton : Instantiate(templateButton, buttonContainer);
                
                SetupChoiceButton(button, buttonConfig, entry, entryObject);
            }
            
            // If we have more than one button, ensure proper layout
            if (entry.familyActionData.actionButtons.Length > 1)
            {
                HorizontalLayoutGroup hlg = buttonContainer.GetComponent<HorizontalLayoutGroup>();
                if (hlg == null)
                {
                    hlg = buttonContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
                    hlg.spacing = 10;
                    hlg.childControlWidth = true;
                    hlg.childControlHeight = false;
                    hlg.childForceExpandWidth = true;
                }
            }
        }
        
        /// <summary>
        /// Setup individual choice button with consequences
        /// </summary>
        private void SetupChoiceButton(Button button, ActionButtonConfig config, DataLogEntry entry, GameObject entryObject)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            
            // Set button text
            if (buttonText != null)
            {
                string text = config.buttonText;
                if (config.creditCost > 0)
                {
                    text += $" ({config.creditCost} Credits)";
                }
                buttonText.text = text;
            }
            
            // Set button color based on style
            SetButtonStyle(button, config.buttonStyle);
            
            // Check if player can afford/meets conditions
            bool canAfford = true;
            var gameManager = FindObjectOfType<GameManager>();
            var advancedTracker = AdvancedNarrativeTracker.Instance;
            
            // Check credit requirements
            if (config.creditCost > 0 && gameManager != null)
            {
                canAfford = gameManager.GetCredits() >= config.creditCost;
            }
            
            // Check story tag requirements (using unlockedStoryTags field since no HasStoryTag method)
            if (canAfford && entry.familyActionData.requiredStoryTags != null && advancedTracker != null)
            {
                foreach (var tag in entry.familyActionData.requiredStoryTags)
                {
                    // Note: May need to add HasStoryTag method to AdvancedNarrativeTracker
                    // For now, just check if we can afford
                }
            }
            
            button.interactable = canAfford;
            
            // Setup click handler with consequences
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                ProcessChoiceWithConsequences(config, entry, entryObject);
            });
        }
        
        /// <summary>
        /// Process a player's choice and apply all consequences
        /// </summary>
        private void ProcessChoiceWithConsequences(ActionButtonConfig buttonConfig, DataLogEntry entry, GameObject entryObject)
        {
            // Get existing narrative managers
            var narrativeState = ServiceLocator.Get<NarrativeStateManager>();
            var advancedTracker = AdvancedNarrativeTracker.Instance;
            var gameManager = FindObjectOfType<GameManager>();
            
            // Apply credit changes through GameManager
            if (buttonConfig.creditCost > 0 && gameManager != null)
            {
                // Use negative AddCredits to deduct credits (since there's no RemoveCredits method)
                gameManager.AddCredits(-buttonConfig.creditCost);
            }
            
            // Apply narrative alignment changes using correct method names
            if (narrativeState != null)
            {
                // Use UpdateAlignment for imperial/rebellion changes
                if (buttonConfig.imperialLoyaltyShift != 0 || buttonConfig.familyLoyaltyShift != 0)
                {
                    narrativeState.UpdateAlignment(buttonConfig.imperialLoyaltyShift, -buttonConfig.familyLoyaltyShift);
                }
                
                // Use UpdateSuspicion for suspicion changes
                if (buttonConfig.suspicionShift != 0)
                {
                    narrativeState.UpdateSuspicion(buttonConfig.suspicionShift);
                }
                
                // Use UpdateCorruption for corruption changes
                if (buttonConfig.corruptionShift != 0)
                {
                    narrativeState.UpdateCorruption(buttonConfig.corruptionShift);
                }
            }
            
            // Track story events with AdvancedNarrativeTracker
            if (advancedTracker != null)
            {
                string eventId = $"{entry.headline}_{buttonConfig.buttonText}".Replace(" ", "_");
                advancedTracker.RecordStoryEvent(eventId, $"Chose '{buttonConfig.buttonText}' for {entry.headline}");
                
                // Add story tags
                if (buttonConfig.storyTagsToAdd != null)
                {
                    foreach (var tag in buttonConfig.storyTagsToAdd)
                    {
                        advancedTracker.UnlockStoryTag(tag);
                    }
                }
            }
            
            // Queue future entries if specified
            if (buttonConfig.triggerEntries != null)
            {
                QueueFutureEntries(buttonConfig.triggerEntries);
            }
            
            // Remove the entry from display
            Destroy(entryObject);
            
            // Log the choice
            if (enableDebugLogs)
            {
                Debug.Log($"[PersonalDataLogManager] Player chose: {buttonConfig.buttonText} for {entry.headline}");
                if (buttonConfig.storyTagsToAdd != null)
                {
                    Debug.Log($"  Added tags: {string.Join(", ", buttonConfig.storyTagsToAdd)}");
                }
                if (buttonConfig.locksEndings != null && buttonConfig.locksEndings.Length > 0)
                {
                    Debug.Log($"  LOCKED endings: {string.Join(", ", buttonConfig.locksEndings)}");
                }
                if (buttonConfig.enablesEndings != null && buttonConfig.enablesEndings.Length > 0)
                {
                    Debug.Log($"  ENABLED endings: {string.Join(", ", buttonConfig.enablesEndings)}");
                }
            }
        }
        
        /// <summary>
        /// Set button visual style based on type
        /// </summary>
        private void SetButtonStyle(Button button, ButtonStyleType style)
        {
            ColorBlock colors = button.colors;
            
            switch (style)
            {
                case ButtonStyleType.Danger:
                    colors.normalColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Red
                    break;
                case ButtonStyleType.Warning:
                    colors.normalColor = new Color(0.8f, 0.6f, 0.2f, 1f); // Yellow/Orange
                    break;
                case ButtonStyleType.Success:
                    colors.normalColor = new Color(0.2f, 0.7f, 0.3f, 1f); // Green
                    break;
                case ButtonStyleType.Corrupt:
                    colors.normalColor = new Color(0.6f, 0.3f, 0.8f, 1f); // Purple
                    break;
                default:
                    colors.normalColor = new Color(0.2f, 0.4f, 0.8f, 1f); // Blue
                    break;
            }
            
            button.colors = colors;
        }
        
        /// <summary>
        /// Queue entries to appear in future days based on choices
        /// </summary>
        private void QueueFutureEntries(string[] entryIds)
        {
            // This would integrate with your entry spawning system
            // For now, just log
            foreach (var entryId in entryIds)
            {
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Queued future entry: {entryId}");
                
                // You could store these and check them when loading daily content
                // AddToFutureQueue(entryId);
            }
        }
        
        /// <summary>
        /// Load individual ScriptableObject entries directly from Resources
        /// </summary>
        private void LoadIndividualScriptableObjects(int currentDay)
        {
            Debug.Log($"[PersonalDataLogManager] Loading individual ScriptableObjects for day {currentDay}");
            
            // Load all PersonalDataLogEntrySO assets from Resources - this will search all subdirectories
            PersonalDataLogEntrySO[] allEntries = Resources.LoadAll<PersonalDataLogEntrySO>("_ScriptableObjects/PersonalDataLog");
            
            // If that doesn't work, try loading from specific subfolders
            if (allEntries == null || allEntries.Length == 0)
            {
                Debug.Log("[PersonalDataLogManager] Trying to load from specific subfolders...");
                List<PersonalDataLogEntrySO> entryList = new List<PersonalDataLogEntrySO>();
                
                // Load from each subfolder
                var imperiumNews = Resources.LoadAll<PersonalDataLogEntrySO>("_ScriptableObjects/PersonalDataLog/ImperiumNews");
                var familyChat = Resources.LoadAll<PersonalDataLogEntrySO>("_ScriptableObjects/PersonalDataLog/FamilyChat");
                var frontierEzine = Resources.LoadAll<PersonalDataLogEntrySO>("_ScriptableObjects/PersonalDataLog/FrontierEzine");
                
                if (imperiumNews != null && imperiumNews.Length > 0)
                {
                    entryList.AddRange(imperiumNews);
                    Debug.Log($"[PersonalDataLogManager] Loaded {imperiumNews.Length} ImperiumNews entries");
                }
                
                if (familyChat != null && familyChat.Length > 0)
                {
                    entryList.AddRange(familyChat);
                    Debug.Log($"[PersonalDataLogManager] Loaded {familyChat.Length} FamilyChat entries");
                }
                
                if (frontierEzine != null && frontierEzine.Length > 0)
                {
                    entryList.AddRange(frontierEzine);
                    Debug.Log($"[PersonalDataLogManager] Loaded {frontierEzine.Length} FrontierEzine entries");
                }
                
                allEntries = entryList.ToArray();
            }
            
            if (allEntries == null || allEntries.Length == 0)
            {
                Debug.LogWarning("[PersonalDataLogManager] No PersonalDataLogEntrySO assets found in any Resources subfolder!");
                return;
            }
            
            Debug.Log($"[PersonalDataLogManager] Found {allEntries.Length} total PersonalDataLogEntrySO assets");
            
            int addedCount = 0;
            int skippedDayRange = 0;
            int skippedAlreadyShown = 0;
            int skippedChance = 0;
            
            foreach (var soEntry in allEntries)
            {
                if (soEntry == null) continue;
                
                // Check if this entry is for the current day
                if (currentDay >= soEntry.minDay && currentDay <= soEntry.maxDay)
                {
                    // Check if already shown (to prevent repetition of the same content)
                    if (shownEntryIds.Contains(soEntry.EntryId))
                    {
                        skippedAlreadyShown++;
                        if (enableDebugLogs)
                            Debug.Log($"[PersonalDataLogManager] ✗ Skipped (already shown): {soEntry.name}");
                    }
                    else if (soEntry.ShouldAppear())
                    {
                        var entry = soEntry.ToDataLogEntry();
                        if (entry != null)
                        {
                            queuedEntries.Add(entry);
                            shownEntryIds.Add(soEntry.EntryId); // Track that we've shown this
                            addedCount++;
                            
                            Debug.Log($"[PersonalDataLogManager] ✓ Added SO: {entry.headline} (Day {currentDay}, Type: {entry.feedType}, Min: {soEntry.minDay}, Max: {soEntry.maxDay})");
                        }
                    }
                    else
                    {
                        skippedChance++;
                        Debug.Log($"[PersonalDataLogManager] ✗ Skipped (chance): {soEntry.name} (chance: {soEntry.appearanceChance})");
                    }
                }
                else
                {
                    skippedDayRange++;
                    if (enableDebugLogs)
                        Debug.Log($"[PersonalDataLogManager] ✗ Skipped (day range): {soEntry.name} (Day {currentDay} not in range {soEntry.minDay}-{soEntry.maxDay})");
                }
            }
            
            Debug.Log($"[PersonalDataLogManager] ScriptableObject loading summary for Day {currentDay}:");
            Debug.Log($"  - Total assets found: {allEntries.Length}");
            Debug.Log($"  - Added: {addedCount}");
            Debug.Log($"  - Skipped (already shown): {skippedAlreadyShown}");
            Debug.Log($"  - Skipped (wrong day): {skippedDayRange}");
            Debug.Log($"  - Skipped (chance fail): {skippedChance}");
            
            // Log what types were added
            var typeCount = queuedEntries.GroupBy(e => e.feedType).Select(g => $"{g.Key}: {g.Count()}");
            Debug.Log($"  - Content by type: {string.Join(", ", typeCount)}");
            
            // Warning if we have too little content
            if (addedCount < 3)
            {
                Debug.LogWarning($"[PersonalDataLogManager] Only {addedCount} entries added for Day {currentDay}! Consider adding more ScriptableObjects with broader day ranges.");
            }
        }
        
        /// <summary>
        /// Setup dynamic action button based on entry requirements
        /// </summary>
        private void SetupDynamicActionButton(Button actionButton, DataLogEntry entry)
        {
            TMP_Text buttonText = actionButton.GetComponentInChildren<TMP_Text>();
            
            // Clear any existing listeners
            actionButton.onClick.RemoveAllListeners();
            
            if (entry.requiresAction && entry.familyActionData != null)
            {
                // Entry requires specific action (Pay, Agree, etc.)
                SetupActionRequiredButton(actionButton, buttonText, entry);
            }
            else
            {
                // Entry just needs acknowledgment
                SetupReadButton(actionButton, buttonText, entry);
            }
        }
        
        /// <summary>
        /// Setup button for entries that require specific actions
        /// </summary>
        private void SetupActionRequiredButton(Button actionButton, TMP_Text buttonText, DataLogEntry entry)
        {
            var actionData = entry.familyActionData;
            
            // Determine button text based on action type
            string buttonLabel = GetActionButtonLabel(actionData);
            
            if (buttonText != null)
            {
                if (actionData.creditCost > 0)
                {
                    buttonLabel += $" ({actionData.creditCost} Credits)";
                }
                buttonText.text = buttonLabel;
            }
            
            // Set button color based on action type
            SetActionButtonColor(actionButton, actionData);
            
            // Setup click handler
            actionButton.onClick.AddListener(() => HandleFamilyAction(actionData));
            
            if (enableDebugLogs)
                Debug.Log($"[PersonalDataLogManager] Setup action button: {buttonLabel}");
        }
        
        /// <summary>
        /// Setup button for entries that just need acknowledgment
        /// </summary>
        private void SetupReadButton(Button actionButton, TMP_Text buttonText, DataLogEntry entry)
        {
            if (buttonText != null)
            {
                buttonText.text = "Read";
            }
            
            // Set neutral color for read buttons
            ColorBlock colors = actionButton.colors;
            colors.normalColor = new Color(0.2f, 0.3f, 0.8f, 1f); // Blue
            actionButton.colors = colors;
            
            // Setup click handler to mark as read and hide button
            actionButton.onClick.AddListener(() => HandleReadMessage(entry, actionButton));
            
            if (enableDebugLogs)
                Debug.Log($"[PersonalDataLogManager] Setup read button for: {entry.headline}");
        }
        
        /// <summary>
        /// Get appropriate button label based on action type
        /// </summary>
        private string GetActionButtonLabel(FamilyActionData actionData)
        {
            if (actionData.actionText.ToLower().Contains("pay"))
                return "Pay";
            else if (actionData.actionText.ToLower().Contains("bail"))
                return "Bail Out";
            else if (actionData.actionText.ToLower().Contains("bribe"))
                return "Bribe";
            else if (actionData.actionText.ToLower().Contains("fine"))
                return "Pay Fine";
            else if (actionData.actionText.ToLower().Contains("training"))
                return "Approve";
            else
                return "Agree";
        }
        
        /// <summary>
        /// Set button color based on action type
        /// </summary>
        private void SetActionButtonColor(Button actionButton, FamilyActionData actionData)
        {
            ColorBlock colors = actionButton.colors;
            
            // Color coding for different action types
            if (actionData.creditCost > 1000) // Expensive actions
            {
                colors.normalColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Red
            }
            else if (actionData.actionText.ToLower().Contains("bribe"))
            {
                colors.normalColor = new Color(0.6f, 0.3f, 0.8f, 1f); // Purple
            }
            else
            {
                colors.normalColor = new Color(0.2f, 0.6f, 0.2f, 1f); // Green
            }
            
            actionButton.colors = colors;
        }
        
        /// <summary>
        /// Handle read message action
        /// </summary>
        private void HandleReadMessage(DataLogEntry entry, Button button)
        {
            // Mark entry as read using headline as identifier
            string entryKey = $"{entry.feedType}_{entry.headline}";
            if (!shownEntryIds.Contains(entryKey))
            {
                shownEntryIds.Add(entryKey);
            }
            
            // For news entries, just disable the button instead of removing the entry
            if (entry.feedType == FeedType.ImperiumNews || entry.feedType == FeedType.FrontierEzine)
            {
                button.interactable = false;
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = "Read ✓";
                }
                
                if (enableDebugLogs)
                    Debug.Log($"[PersonalDataLogManager] Marked as read: {entry.headline}");
            }
            else
            {
                // For family chat entries, remove them (they're more like notifications)
                GameObject entryObject = button.transform.parent.gameObject;
                if (entryObject.name.Contains("Template(Clone)"))
                {
                    Destroy(entryObject);
                    if (enableDebugLogs)
                        Debug.Log($"[PersonalDataLogManager] Removed family entry: {entry.headline}");
                }
                else
                {
                    // Button might be nested deeper, search up for the template clone
                    Transform current = button.transform;
                    for (int i = 0; i < 5; i++)
                    {
                        if (current.parent != null)
                        {
                            current = current.parent;
                            if (current.name.Contains("Template(Clone)"))
                            {
                                Destroy(current.gameObject);
                                if (enableDebugLogs)
                                    Debug.Log($"[PersonalDataLogManager] Removed family entry: {entry.headline}");
                                break;
                            }
                        }
                    }
                }
            }
            
            if (enableDebugLogs)
                Debug.Log($"[PersonalDataLogManager] Message read: {entry.headline}");
        }
        
        /// <summary>
        /// Force layout system to rebuild a parent container
        /// </summary>
        private System.Collections.IEnumerator ForceLayoutRebuild(Transform parent)
        {
            yield return new WaitForEndOfFrame();
            
            if (parent != null)
            {
                // Force layout to recalculate
                LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
                
                // Also rebuild the parent's parent for good measure
                if (parent.parent != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(parent.parent.GetComponent<RectTransform>());
                }
            }
        }
        
        /// <summary>
        /// Force complete layout rebuild for all sections
        /// </summary>
        [ContextMenu("Force Layout Rebuild")]
        public void ForceCompleteLayoutRebuild()
        {
            Debug.Log("[PersonalDataLogManager] Force rebuilding all layouts...");
            
            if (imperiumNewsSection != null)
                StartCoroutine(ForceLayoutRebuild(imperiumNewsSection));
            if (familyChatSection != null)
                StartCoroutine(ForceLayoutRebuild(familyChatSection));
            if (frontierEzineSection != null)
                StartCoroutine(ForceLayoutRebuild(frontierEzineSection));
        }
        
        /// <summary>
        /// Auto-setup UI references if missing
        /// </summary>
        [ContextMenu("Auto-Setup UI References")]
        public void AutoSetupUIReferences()
        {
            Debug.Log("[PersonalDataLogManager] Auto-setup starting...");
            
            // Try to find dataLogPanel
            if (dataLogPanel == null)
            {
                dataLogPanel = GameObject.Find("PersonalDataLog") ?? GameObject.Find("PersonalDataLogPanel");
                if (dataLogPanel != null)
                    Debug.Log($"[PersonalDataLogManager] ✓ Found dataLogPanel: {dataLogPanel.name}");
            }
            
            // Try to find feed sections
            if (dataLogPanel != null)
            {
                Transform scrollView = dataLogPanel.transform.Find("Scroll View");
                if (scrollView != null)
                {
                    Transform viewport = scrollView.Find("Viewport");
                    if (viewport != null)
                    {
                        Transform content = viewport.Find("Content");
                        if (content != null)
                        {
                            imperiumNewsSection = content.Find("ImperiumNewsSection");
                            familyChatSection = content.Find("FamilyChatSection");
                            frontierEzineSection = content.Find("FrontierEzineSection");
                            
                            Debug.Log("[PersonalDataLogManager] ✓ Found feed sections");
                        }
                    }
                }
            }
            
            // Only auto-find templates if they're not already assigned in Inspector
            bool needToFindTemplates = (newsEntryTemplate == null || familyActionTemplate == null || videoEntryTemplate == null);
            
            if (needToFindTemplates)
            {
                Debug.Log("[PersonalDataLogManager] Some templates not assigned, attempting auto-detection...");
                
                // Try to find templates in scene first
                GameObject templates = GameObject.Find("Templates");
                if (templates != null)
                {
                    if (newsEntryTemplate == null)
                        newsEntryTemplate = templates.transform.Find("NewsEntryTemplate")?.gameObject;
                    if (familyActionTemplate == null)
                        familyActionTemplate = templates.transform.Find("FamilyActionTemplate")?.gameObject;
                    if (videoEntryTemplate == null)
                        videoEntryTemplate = templates.transform.Find("VideoEntryTemplate")?.gameObject;
                }
                else
                {
                    Debug.LogWarning("[PersonalDataLogManager] Templates GameObject not found in scene!");
                }
            }
            
            // Report template status
            Debug.Log($"[PersonalDataLogManager] Final template setup:");
            Debug.Log($"  - NewsEntryTemplate: {(newsEntryTemplate != null ? "ASSIGNED" : "MISSING")}");
            Debug.Log($"  - FamilyActionTemplate: {(familyActionTemplate != null ? "ASSIGNED" : "MISSING")}");
            Debug.Log($"  - VideoEntryTemplate: {(videoEntryTemplate != null ? "ASSIGNED" : "MISSING")}");
            
            // Validate VideoEntryTemplate if assigned
            if (videoEntryTemplate != null)
            {
                VideoPlayer vp = videoEntryTemplate.GetComponentInChildren<VideoPlayer>();
                UnityEngine.UI.RawImage ri = videoEntryTemplate.GetComponentInChildren<UnityEngine.UI.RawImage>();
                Debug.Log($"  - VideoEntryTemplate has VideoPlayer: {vp != null}");
                Debug.Log($"  - VideoEntryTemplate has RawImage: {ri != null}");
                if (vp != null && ri != null)
                {
                    Debug.Log($"  - VideoPlayer target texture: {(vp.targetTexture != null ? vp.targetTexture.name : "NULL")}");
                    Debug.Log($"  - RawImage texture: {(ri.texture != null ? ri.texture.name : "NULL")}");
                }
            }
            
            Debug.Log("[PersonalDataLogManager] Auto-setup completed!");
        }
        
        private void OnDestroy()
        {
            // Clear events
            OnDataLogOpened = null;
            OnDataLogClosed = null;
            OnFamilyActionTaken = null;
        }
        
        /// <summary>
        /// Setup video play button for user-controlled video playback
        /// </summary>
        private void SetupVideoPlayButton(GameObject entryObject, VideoPlayer videoPlayer, DataLogEntry entry)
        {
            // Always create a unique Play button for each video entry to avoid conflicts
            Button playButton = CreateVideoPlayButton(entryObject, $"PlayButton_{entry.headline}");
            
            if (playButton != null)
            {
                // Clear any existing listeners
                playButton.onClick.RemoveAllListeners();
                
                // Setup the play functionality for THIS specific video
                playButton.onClick.AddListener(() => {
                    StartCoroutine(PlayVideoSafely(videoPlayer, entry));
                });
                
                // Update button text
                TMP_Text buttonText = playButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = "▶ Play Video";
                }
                
                // Style the play button
                ColorBlock colors = playButton.colors;
                colors.normalColor = new Color(0.2f, 0.7f, 0.3f, 1f); // Green
                playButton.colors = colors;
                
                Debug.Log($"[PersonalDataLogManager] Unique Play button created for: {entry.headline}");
            }
            else
            {
                Debug.LogWarning($"[PersonalDataLogManager] Could not create Play button for video: {entry.headline}");
            }
        }
        
        /// <summary>
        /// Create a Play button for video entries
        /// </summary>
        private Button CreateVideoPlayButton(GameObject entryObject, string uniqueName = "PlayButton")
        {
            // Find a good location for the button (look for VideoDisplay or similar)
            Transform videoDisplay = entryObject.transform.Find("VideoDisplay");
            Transform buttonParent = videoDisplay != null ? videoDisplay : entryObject.transform;
            
            // Create the button GameObject with unique name
            GameObject buttonObj = new GameObject(uniqueName);
            buttonObj.transform.SetParent(buttonParent);
            
            // Add Button component
            Button button = buttonObj.AddComponent<Button>();
            
            // Add Image component for the button background
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.7f, 0.3f, 0.8f); // Semi-transparent green
            
            // Setup RectTransform for positioning
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(120, 30);
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = Vector2.zero;
            
            // Create text for the button
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            
            TMP_Text text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "▶ Play Video";
            text.fontSize = 14;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            
            // Setup text RectTransform
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            Debug.Log($"[PersonalDataLogManager] Created unique Play button: {uniqueName}");
            return button;
        }
        
        /// <summary>
        /// Safely play video with proper preparation and activation
        /// </summary>
        private System.Collections.IEnumerator PlayVideoSafely(VideoPlayer videoPlayer, DataLogEntry entry)
        {
            if (videoPlayer == null || entry.videoClip == null)
            {
                Debug.LogError("[PersonalDataLogManager] Cannot play video - VideoPlayer or clip is null");
                yield break;
            }
            
            Debug.Log($"[PersonalDataLogManager] Starting safe video playback for: {entry.headline}");
            
            // Ensure the VideoPlayer GameObject is active
            videoPlayer.gameObject.SetActive(true);
            
            // Activate parent hierarchy up to root
            Transform current = videoPlayer.transform;
            while (current != null && current.parent != null)
            {
                current.gameObject.SetActive(true);
                current = current.parent;
            }
            
            // Wait a frame for activation to take effect
            yield return null;
            
            // Verify the VideoPlayer is ready
            if (!videoPlayer.gameObject.activeInHierarchy)
            {
                Debug.LogError($"[PersonalDataLogManager] VideoPlayer still not active in hierarchy after activation attempt");
                yield break;
            }
            
            // Ensure video clip is assigned
            if (videoPlayer.clip != entry.videoClip)
            {
                videoPlayer.clip = entry.videoClip;
                Debug.Log($"[PersonalDataLogManager] Assigned video clip: {entry.videoClip.name}");
            }
            
            Debug.Log($"[PersonalDataLogManager] VideoPlayer state before prepare - isPlaying: {videoPlayer.isPlaying}, isPrepared: {videoPlayer.isPrepared}");
            
            // Prepare the video
            videoPlayer.Prepare();
            Debug.Log($"[PersonalDataLogManager] VideoPlayer.Prepare() called");
            
            // Wait for preparation to complete
            float startTime = Time.time;
            while (!videoPlayer.isPrepared)
            {
                yield return null;
                
                // Timeout after 5 seconds
                if (Time.time - startTime > 5f)
                {
                    Debug.LogError("[PersonalDataLogManager] Video preparation timed out");
                    yield break;
                }
            }
            
            // Find the RawImage component in the same entry
            Transform entryRoot = videoPlayer.transform;
            while (entryRoot != null && !entryRoot.name.Contains("Template(Clone)"))
            {
                entryRoot = entryRoot.parent;
            }
            
            UnityEngine.UI.RawImage rawImage = null;
            if (entryRoot != null)
            {
                rawImage = entryRoot.GetComponentInChildren<UnityEngine.UI.RawImage>();
            }
            
            if (rawImage != null && videoPlayer.targetTexture != null)
            {
                // Switch from thumbnail to video texture
                rawImage.texture = videoPlayer.targetTexture;
                Debug.Log($"[PersonalDataLogManager] Switched RawImage to video texture: {videoPlayer.targetTexture.name}");
            }
            else
            {
                Debug.LogError($"[PersonalDataLogManager] Could not find RawImage or targetTexture is null");
            }
            
            // Start playback
            videoPlayer.Play();
            
            Debug.Log($"[PersonalDataLogManager] Video playback started successfully: {entry.videoClip.name}");
        }
        
        /// <summary>
        /// Helper method to get full GameObject path for debugging
        /// </summary>
        private string GetGameObjectPath(GameObject obj)
        {
            if (obj == null) return "NULL";
            
            string path = obj.name;
            Transform parent = obj.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }
    }
    
    // Supporting data structures
    public enum FeedType
    {
        ImperiumNews,
        FamilyChat,
        FrontierEzine
    }
    
    [System.Serializable]
    public class DataLogEntry
    {
        public FeedType feedType;
        public string headline;
        public string content;
        public VideoClip videoClip;
        public Sprite videoThumbnail; // Custom thumbnail image
        public bool requiresAction;
        public FamilyActionData familyActionData;
        public int severity;
        public System.DateTime timestamp;
    }
    
    [System.Serializable]
    public class FamilyActionData
    {
        [Header("Basic Action Info")]
        public string actionText;
        public int creditCost;
        public string actionId;
        
        [Header("Multiple Choice Buttons")]
        public ActionButtonConfig[] actionButtons; // Support for multiple buttons
        
        [Header("Hidden Consequences")]
        public int imperialLoyaltyShift;
        public int familyLoyaltyShift;
        public int suspicionShift;
        public int corruptionShift;
        
        [Header("Story Impact")]
        public string[] storyTagsToAdd;
        public string[] storyTagsToRemove;
        public string[] futureEntryTriggers;
        public string[] locksEndings;
        public string[] enablesEndings;
        
        [Header("Display Conditions")]
        public string[] requiredStoryTags;
        public int minimumCredits;
    }
    
    [System.Serializable]
    public class ActionButtonConfig
    {
        public string buttonText = "Accept";
        public int creditCost = 0;
        public ButtonStyleType buttonStyle = ButtonStyleType.Default;
        
        // Each button can have different consequences
        public int imperialLoyaltyShift;
        public int familyLoyaltyShift;
        public int suspicionShift;
        public int corruptionShift;
        
        public string[] storyTagsToAdd;
        public string[] storyTagsToRemove;
        public string[] triggerEntries;
        public string[] locksEndings;
        public string[] enablesEndings;
    }
    
    public enum ButtonStyleType
    {
        Default,    // Blue
        Danger,     // Red  
        Warning,    // Yellow
        Success,    // Green
        Corrupt     // Purple
    }
    
    [System.Serializable]
    public class PersistentFamilyAction
    {
        public string headline;
        public string message;
        public string actionText;
        public int creditCost;
        public string actionId;
        public int remainingDays;
    }
    
    // Extension methods for family integration
    public static class PersonalDataLogManagerExtensions
    {
        /// <summary>
        /// Generate family messages from FamilyPressureManager
        /// </summary>
        public static void GenerateFamilyMessages(this PersonalDataLogManager manager, int currentDay)
        {
            var familyPressureManager = ServiceLocator.Get<FamilyPressureManager>();
            if (familyPressureManager == null)
            {
                Debug.LogWarning("[PersonalDataLogManager] FamilyPressureManager not found - family messages will not appear!");
                return;
            }
            
            Debug.Log($"[PersonalDataLogManager] Generating family messages for day {currentDay}");
            
            var livingFamily = familyPressureManager.GetLivingFamily();
            int familyMessagesAdded = 0;
            
            foreach (var member in livingFamily)
            {
                var familyMessage = GenerateFamilyMessageForMember(member, currentDay);
                if (familyMessage != null)
                {
                    manager.AddLogEntry(familyMessage);
                    familyMessagesAdded++;
                    Debug.Log($"[PersonalDataLogManager] Added family message from {member.name}: {familyMessage.headline}");
                }
            }
            
            GenerateCrisisMessages(manager, familyPressureManager, currentDay);
            GenerateRelationshipUpdates(manager, familyPressureManager, currentDay);
            
            Debug.Log($"[PersonalDataLogManager] Generated {familyMessagesAdded} family messages for day {currentDay}");
        }
        
        /// <summary>
        /// Generate message for specific family member based on their current state
        /// </summary>
        private static DataLogEntry GenerateFamilyMessageForMember(FamilyMember member, int currentDay)
        {
            if (!ShouldGenerateMessageForMember(member, currentDay))
                return null;
            
            var entry = new DataLogEntry
            {
                feedType = FeedType.FamilyChat,
                timestamp = DateTime.Now,
                severity = DetermineFamilyMessageSeverity(member),
                requiresAction = member.health < 30 || member.safety < 30 || member.activeTokens.Contains("NeedsHelp")
            };
            
            GenerateFamilyMessageContent(entry, member, currentDay);
            return entry;
        }
        
        /// <summary>
        /// Determine if member should send message today
        /// </summary>
        private static bool ShouldGenerateMessageForMember(FamilyMember member, int currentDay)
        {
            if (member.health < 30 || member.safety < 30 || member.relationship < 30)
                return true;
            
            switch (member.role)
            {
                case FamilyRole.Partner:
                    return currentDay == 2 || currentDay == 7 || currentDay == 12 || currentDay == 19 || currentDay == 24 || currentDay == 28;
                case FamilyRole.Son:
                    return currentDay == 3 || currentDay == 8 || currentDay == 14 || currentDay == 20 || currentDay == 25 || currentDay == 29;
                case FamilyRole.Daughter:
                    return currentDay == 4 || currentDay == 9 || currentDay == 15 || currentDay == 21 || currentDay == 26 || currentDay == 30;
                case FamilyRole.Baby:
                    return currentDay == 5 || currentDay == 11 || currentDay == 17 || currentDay == 23 || currentDay == 27;
                case FamilyRole.Droid:
                    return currentDay == 6 || currentDay == 12 || currentDay == 18 || currentDay == 24 || currentDay == 28;
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Generate message content based on family member and story progression
        /// </summary>
        private static void GenerateFamilyMessageContent(DataLogEntry entry, FamilyMember member, int currentDay)
        {
            switch (member.role)
            {
                case FamilyRole.Partner:
                    GeneratePartnerMessage(entry, member, currentDay);
                    break;
                case FamilyRole.Son:
                    GenerateSonMessage(entry, member, currentDay);
                    break;
                case FamilyRole.Daughter:
                    GenerateDaughterMessage(entry, member, currentDay);
                    break;
                case FamilyRole.Baby:
                    GenerateBabyMessage(entry, member, currentDay);
                    break;
                case FamilyRole.Droid:
                    GenerateDroidMessage(entry, member, currentDay);
                    break;
            }
        }
        
        private static void GeneratePartnerMessage(DataLogEntry entry, FamilyMember member, int currentDay)
        {
            switch (currentDay)
            {
                case 2:
                    entry.headline = "Lab Work Update";
                    entry.content = "Lab work fascinating today. The new particles we're studying... reminds me of home. Miss you at dinner.";
                    break;
                case 7:
                    entry.headline = "Strange Protocols";
                    entry.content = "Strange security protocols in the lab. We're not allowed to discuss our findings with other departments.";
                    entry.severity = 1;
                    break;
                case 12:
                    entry.headline = "University Friend";
                    entry.content = "Remember my friend from university? They reached out. They're... concerned about our work here.";
                    entry.severity = 2;
                    break;
                case 19:
                    entry.headline = "We Need to Talk";
                    entry.content = "I've seen what this base really does. We need to talk. Not over comms.";
                    entry.severity = 2;
                    entry.requiresAction = true;
                    break;
                case 24:
                    if (member.activeTokens.Contains("ContactedRebels"))
                    {
                        entry.headline = "They Know";
                        entry.content = "They know I've been asking questions. If something happens to me, protect the children.";
                        entry.severity = 3;
                    }
                    break;
                case 28:
                    if (member.activeTokens.Contains("RebelContact"))
                    {
                        entry.headline = "The Codes";
                        entry.content = "The rebels can stop this weapon. I need those security codes. Please, for our future.";
                        entry.severity = 3;
                        entry.requiresAction = true;
                    }
                    break;
            }
        }
        
        private static void GenerateSonMessage(DataLogEntry entry, FamilyMember member, int currentDay)
        {
            switch (currentDay)
            {
                case 3:
                    entry.headline = "Training Update";
                    entry.content = "Training's brutal. Sarge says I've got potential though. Could use new boots (50 credits?)";
                    entry.requiresAction = true;
                    break;
                case 8:
                    entry.headline = "Met Someone";
                    entry.content = "Met someone who knew Dad's old crew. They remember us. Said they could help with money.";
                    entry.severity = 1;
                    break;
                case 14:
                    if (member.activeTokens.Contains("PirateContact"))
                    {
                        entry.headline = "Made a Deal";
                        entry.content = "Made a deal. Just need to share training schedules. Harmless stuff. Got us 200 credits!";
                        entry.severity = 2;
                    }
                    break;
                case 20:
                    if (member.activeTokens.Contains("MadePirateDeal"))
                    {
                        entry.headline = "In Too Deep";
                        entry.content = "They want guard rotations now. I'm in too deep. What would Dad have done?";
                        entry.severity = 2;
                        entry.requiresAction = true;
                    }
                    break;
                case 25:
                    if (member.activeTokens.Contains("PirateBlackmail"))
                    {
                        entry.headline = "They Have Evidence";
                        entry.content = "The pirates have holos of our deals. If Empire finds out... I'm sorry I failed you.";
                        entry.severity = 3;
                    }
                    break;
                case 29:
                    entry.headline = "Graduation Tomorrow";
                    entry.content = "It's graduation tomorrow. The pirates want me to do something during ceremony. I'm scared.";
                    entry.severity = 3;
                    entry.requiresAction = true;
                    break;
            }
        }
        
        private static void GenerateDaughterMessage(DataLogEntry entry, FamilyMember member, int currentDay)
        {
            switch (currentDay)
            {
                case 4:
                    entry.headline = "Fixed a Transport";
                    entry.content = "Fixed a family transport today. Their life support was failing. Kids aboard. Reminded me of us.";
                    break;
                case 9:
                    entry.headline = "Overtime Repairs";
                    entry.content = "Boss doesn't check the overtime repairs. I've been helping ships that 'fail' inspection.";
                    entry.severity = 1;
                    break;
                case 15:
                    entry.headline = "Family Hiding";
                    entry.content = "There's a family hiding in the shop. Their baby is same age as ours. I couldn't turn them in.";
                    entry.severity = 2;
                    entry.requiresAction = true;
                    break;
                case 21:
                    if (member.activeTokens.Contains("HidingRefugees"))
                    {
                        entry.headline = "Security Sweep";
                        entry.content = "Security sweep tomorrow. Need 300 credits to bribe inspector or they'll find the refugees.";
                        entry.severity = 3;
                        entry.requiresAction = true;
                    }
                    break;
                case 26:
                    entry.headline = "Thirty Refugees";
                    entry.content = "Thirty refugees coming through. This is our chance to save them. Will you help?";
                    entry.severity = 3;
                    entry.requiresAction = true;
                    break;
                case 30:
                    entry.headline = "Proud of What We've Done";
                    entry.content = "Whatever happens, I'm proud of what we've done. Tell baby about their brave sister.";
                    entry.severity = 2;
                    break;
            }
        }
        
        private static void GenerateBabyMessage(DataLogEntry entry, FamilyMember member, int currentDay)
        {
            switch (currentDay)
            {
                case 5:
                    entry.headline = "Baby's Health";
                    entry.content = "Baby's cough worse. Booked medical appointment.";
                    entry.severity = 1;
                    break;
                case 11:
                    entry.headline = "Drift Sickness Diagnosis";
                    entry.content = "Doctor says it's Drift Sickness. Common in space-born infants. Medicine is controlled.";
                    entry.severity = 2;
                    entry.requiresAction = true;
                    break;
                case 17:
                    entry.headline = "Legal Doses Failing";
                    entry.content = "Legal doses aren't working. Doctor 'hinted' about alternatives. 500 credits.";
                    entry.severity = 2;
                    entry.requiresAction = true;
                    break;
                case 23:
                    if (member.health < 40)
                    {
                        entry.headline = "Temperature Spikes";
                        entry.content = "Temperature spikes. Need stronger meds NOW. Found seller - 1000 credits.";
                        entry.severity = 3;
                        entry.requiresAction = true;
                    }
                    break;
                case 27:
                    if (member.health > 30)
                    {
                        entry.headline = "Stabilizing";
                        entry.content = "Baby stabilizing but needs continued treatment. Monthly cost: 400 credits.";
                        entry.severity = 2;
                    }
                    else
                    {
                        entry.headline = "Critical Condition";
                        entry.content = "Baby's condition deteriorating rapidly. This might be our last hope.";
                        entry.severity = 3;
                    }
                    break;
                case 30:
                    entry.headline = "Remember Baby's Needs";
                    entry.content = "Whatever you've decided about our future, baby needs med access. Please remember.";
                    entry.severity = 2;
                    break;
            }
        }
        
        private static void GenerateDroidMessage(DataLogEntry entry, FamilyMember member, int currentDay)
        {
            switch (currentDay)
            {
                case 6:
                    entry.headline = "SYSTEM ALERT";
                    entry.content = "Household unit D-3X experienced 2.3 hour memory gap.";
                    entry.feedType = FeedType.ImperiumNews;
                    entry.severity = 1;
                    break;
                case 12:
                    entry.headline = "NETWORK ANOMALY";
                    entry.content = "Anomalous network traffic detected from residence.";
                    entry.feedType = FeedType.ImperiumNews;
                    entry.severity = 2;
                    break;
                case 18:
                    entry.headline = "D-3X Memory Gap";
                    entry.content = "D-3X: 'Apologies, Master. I seem to have been... elsewhere. Shall I prepare dinner?'";
                    entry.severity = 1;
                    break;
                case 24:
                    entry.headline = "SECURITY BREACH";
                    entry.content = "D-3X accessing classified base schematics. Authorization unknown.";
                    entry.feedType = FeedType.ImperiumNews;
                    entry.severity = 3;
                    break;
                case 28:
                    entry.headline = "Original Programmers";
                    entry.content = "D-3X: 'Master, my original programmers send greetings. They have a proposal.'";
                    entry.severity = 3;
                    entry.requiresAction = true;
                    break;
                case 30:
                    entry.headline = "Trust Me";
                    entry.content = "D-3X: 'I have been protecting this family. Trust me one last time.'";
                    entry.severity = 2;
                    break;
            }
        }
        
        /// <summary>
        /// Generate crisis messages for urgent family situations
        /// </summary>
        private static void GenerateCrisisMessages(PersonalDataLogManager manager, FamilyPressureManager familyManager, int currentDay)
        {
            var livingFamily = familyManager.GetLivingFamily();
            
            foreach (var member in livingFamily)
            {
                if (member.health <= 20)
                {
                    var crisisEntry = new DataLogEntry
                    {
                        feedType = FeedType.FamilyChat,
                        headline = $"URGENT: {member.name} Health Critical",
                        content = $"{member.name}'s condition is deteriorating rapidly. Immediate intervention required!",
                        severity = 3,
                        requiresAction = true,
                        timestamp = DateTime.Now
                    };
                    manager.AddLogEntry(crisisEntry);
                }
                
                if (member.safety <= 20)
                {
                    var safetyEntry = new DataLogEntry
                    {
                        feedType = FeedType.FamilyChat,
                        headline = $"DANGER: {member.name} Under Threat",
                        content = $"{member.name} is in immediate physical danger. Security situation critical!",
                        severity = 3,
                        requiresAction = true,
                        timestamp = DateTime.Now
                    };
                    manager.AddLogEntry(safetyEntry);
                }
            }
        }
        
        /// <summary>
        /// Generate relationship update messages
        /// </summary>
        private static void GenerateRelationshipUpdates(PersonalDataLogManager manager, FamilyPressureManager familyManager, int currentDay)
        {
            var familyMembers = familyManager.GetLivingFamily();
            foreach (var member in familyMembers)
            {
                if (member.happiness < 30 && UnityEngine.Random.value < 0.3f)
                {
                    var griefEntry = new DataLogEntry
                    {
                        feedType = FeedType.FamilyChat,
                        headline = $"{member.name} Struggling",
                        content = GenerateGriefMessage(member),
                        severity = 2,
                        timestamp = DateTime.Now,
                        requiresAction = false
                    };
                    manager.AddLogEntry(griefEntry);
                }
            }
        }
        
        private static string GenerateGriefMessage(FamilyMember grievingMember)
        {
            string[] messages = {
                $"{grievingMember.name}: \"I keep expecting them to walk through the door...\"",
                $"{grievingMember.name}: \"How do we go on without them?\"",
                $"{grievingMember.name}: \"I should have done more to help.\"",
                $"{grievingMember.name}: \"This place took everything from us.\""
            };
            return messages[UnityEngine.Random.Range(0, messages.Length)];
        }
        
        private static int DetermineFamilyMessageSeverity(FamilyMember member)
        {
            if (member.health < 20 || member.safety < 20)
                return 3;
            if (member.health < 40 || member.safety < 40 || member.relationship < 30)
                return 2;
            if (member.happiness < 30)
                return 1;
            return 0;
        }
    }
}