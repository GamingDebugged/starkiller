using System.Collections;
using System.Collections.Generic;
using System.Text; 
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarkillerBaseCommand;

/// <summary>
/// Unified logbook manager for Starkiller Base Command.
/// Displays information from ScriptableObjects in a tabbed interface.
/// </summary>
public class StarkkillerLogBookManager : MonoBehaviour
{
    [Header("System References")]
    [Tooltip("Reference to the content manager for accessing game data")]
    public StarkkillerContentManager contentManager;
    
    [Tooltip("Reference to the media system for accessing images and videos")]
    public StarkkillerMediaSystem mediaSystem;
    
    [Header("UI Container")]
    [Tooltip("Main panel containing the logbook UI")]
    public GameObject logBookPanel;
    
    [Header("Tab Navigation")]
    [Tooltip("Container holding the tab buttons")]
    public GameObject tabButtonsContainer;
    
    [Tooltip("Rules tab button")]
    public Button rulesTabButton;
    
    [Tooltip("Ships tab button")]
    public Button shipsTabButton;
    
    [Tooltip("Captains tab button")]
    public Button captainsTabButton;
    
    [Tooltip("Destinations tab button")]
    public Button destinationsTabButton;
    
    [Tooltip("Contraband tab button")]
    public Button contrabandTabButton;
    
    [Header("Tab Content Areas")]
    [Tooltip("Panel for rules content")]
    public GameObject rulesContentPanel;
    
    [Tooltip("Panel for ships content")]
    public GameObject shipsContentPanel;
    
    [Tooltip("Panel for captains content")]
    public GameObject captainsContentPanel;
    
    [Tooltip("Panel for destinations content")]
    public GameObject destinationsContentPanel;
    
    [Tooltip("Panel for contraband content")]
    public GameObject contrabandContentPanel;
    
    [Header("Rules Tab UI")]
    [Tooltip("Text field for access codes")]
    public TMP_Text accessCodesText;
    
    [Tooltip("Text field for daily rules")]
    public TMP_Text dailyRulesText;
    
    [Header("Ship Data Display")]
    [Tooltip("Container for ship type entries")]
    public Transform shipEntriesContainer;
    
    [Tooltip("Prefab for ship type entries")]
    public GameObject shipEntryPrefab;
    
    [Header("Captain Data Display")]
    [Tooltip("Container for captain entries")]
    public Transform captainEntriesContainer;
    
    [Tooltip("Prefab for captain type entries")]
    public GameObject captainEntryPrefab;
    
    [Header("Destination Data Display")]
    [Tooltip("Container for destination entries")]
    public Transform destinationEntriesContainer;
    
    [Tooltip("Prefab for destination entries")]
    public GameObject destinationEntryPrefab;
    
    [Header("Contraband Data Display")]
    [Tooltip("Container for contraband entries")]
    public Transform contrabandEntriesContainer;
    
    [Tooltip("Prefab for contraband entries")]
    public GameObject contrabandEntryPrefab;
    
    [Header("Navigation Controls")]
    [Tooltip("Button to close the logbook")]
    public Button closeButton;
    
    // Current active tab
    public enum TabType { Rules, Ships, Captains, Destinations, Contraband }
    private TabType currentTab = TabType.Rules;
    
    // Tab indicator objects - can be activated to show which tab is selected
    private Dictionary<TabType, GameObject> tabIndicators = new Dictionary<TabType, GameObject>();
    
    // Cached entry game objects for cleanup
    private List<GameObject> shipEntryInstances = new List<GameObject>();
    private List<GameObject> captainEntryInstances = new List<GameObject>();
    private List<GameObject> destinationEntryInstances = new List<GameObject>();
    private List<GameObject> contrabandEntryInstances = new List<GameObject>();
    
    // Cache for performance
    private StringBuilder stringBuilder = new StringBuilder();
    
    /// <summary>
    /// Initialize the logbook manager
    /// </summary>
    void Start()
    {
        // Find references if not assigned
        if (contentManager == null)
            contentManager = FindFirstObjectByType<StarkkillerContentManager>();
                
        if (mediaSystem == null)
            mediaSystem = FindFirstObjectByType<StarkkillerMediaSystem>();
                
        // Log warnings if references are missing
        if (contentManager == null)
            Debug.LogWarning("StarkkillerLogBookManager: Content manager reference not found!");
                
        if (mediaSystem == null)
            Debug.LogWarning("StarkkillerLogBookManager: Media system reference not found!");
            
        // Setup UI references
        SetupTabIndicators();
        SetupButtonListeners();
        ConfigureScrollRects(); 
        ConfigureButtonNavigation();
            
        // Hide logbook initially
        if (logBookPanel)
            logBookPanel.SetActive(false);
        
        // Add TimeModifierBehavior to the logBookPanel if not already added
        if (logBookPanel != null && logBookPanel.GetComponent<TimeModifierBehavior>() == null)
        {
            TimeModifierBehavior timeModifier = logBookPanel.AddComponent<TimeModifierBehavior>();
            timeModifier.pauseTime = true; // Fully pause the game when logbook is open
            timeModifier.modifyOnEnable = true;
            timeModifier.resumeOnDisable = true;
        }
    }

    /// <summary>
    /// Set up tab indicator references for visual feedback
    /// </summary>
    private void SetupTabIndicators()
    {
        // Find tab indicator objects based on naming convention
        if (rulesTabButton?.transform.Find("Indicator") != null)
            tabIndicators[TabType.Rules] = rulesTabButton.transform.Find("Indicator").gameObject;
            
        if (shipsTabButton?.transform.Find("Indicator") != null)
            tabIndicators[TabType.Ships] = shipsTabButton.transform.Find("Indicator").gameObject;
            
        if (captainsTabButton?.transform.Find("Indicator") != null)
            tabIndicators[TabType.Captains] = captainsTabButton.transform.Find("Indicator").gameObject;
            
        if (destinationsTabButton?.transform.Find("Indicator") != null)
            tabIndicators[TabType.Destinations] = destinationsTabButton.transform.Find("Indicator").gameObject;
            
        if (contrabandTabButton?.transform.Find("Indicator") != null)
            tabIndicators[TabType.Contraband] = contrabandTabButton.transform.Find("Indicator").gameObject;
    }
    
    /// <summary>
    /// Set up button listeners for UI interactions
    /// </summary>
    private void SetupButtonListeners()
    {
        // Tab navigation
        if (rulesTabButton)
            rulesTabButton.onClick.AddListener(() => SwitchTab(TabType.Rules));
            
        if (shipsTabButton)
            shipsTabButton.onClick.AddListener(() => SwitchTab(TabType.Ships));
            
        if (captainsTabButton)
            captainsTabButton.onClick.AddListener(() => SwitchTab(TabType.Captains));
            
        if (destinationsTabButton)
            destinationsTabButton.onClick.AddListener(() => SwitchTab(TabType.Destinations));
            
        if (contrabandTabButton)
            contrabandTabButton.onClick.AddListener(() => SwitchTab(TabType.Contraband));
            
        // Close button
        if (closeButton)
            closeButton.onClick.AddListener(CloseLogBook);
    }
    
    /// <summary>
    /// Switch to the specified tab
    /// </summary>
    /// <param name="tab">The tab to display</param>
    public void SwitchTab(TabType tab)
    {
        currentTab = tab;
        
        // Explicitly hide ALL content panels first
        if (rulesContentPanel) rulesContentPanel.SetActive(false);
        if (shipsContentPanel) shipsContentPanel.SetActive(false);
        if (captainsContentPanel) captainsContentPanel.SetActive(false);
        if (destinationsContentPanel) destinationsContentPanel.SetActive(false);
        if (contrabandContentPanel) contrabandContentPanel.SetActive(false);
        
        // Then activate the selected panel
        switch (tab)
        {
            case TabType.Rules:
                if (rulesContentPanel) rulesContentPanel.SetActive(true);
                break;
            case TabType.Ships:
                if (shipsContentPanel) shipsContentPanel.SetActive(true);
                break;
            case TabType.Captains:
                if (captainsContentPanel) captainsContentPanel.SetActive(true);
                break;
            case TabType.Destinations:
                if (destinationsContentPanel) destinationsContentPanel.SetActive(true);
                break;
            case TabType.Contraband:
                if (contrabandContentPanel) contrabandContentPanel.SetActive(true);
                break;
            default:
                Debug.LogWarning("StarkillerLogBookManager: Unknown tab type selected!");
                break;
        }
        
        // Update tab indicators (existing code)
        foreach (var indicator in tabIndicators)
        {
            if (indicator.Value != null)
                indicator.Value.SetActive(indicator.Key == tab);
        }
    
        // Update content
        UpdateContent();
    }
    
    /// <summary>
    /// Open the logbook and refresh content
    /// </summary>
    public void OpenLogBook()
    {
        if (logBookPanel)
        {
            logBookPanel.SetActive(true);
            SwitchTab(currentTab); // This will also update content
        }
    }
    
    /// <summary>
    /// Configure all ScrollRect components with optimal settings
    /// </summary>
    private void ConfigureScrollRects()
    {
        // Get all ScrollRect components in content panels
        ScrollRect[] scrollRects = new ScrollRect[] {
            rulesContentPanel?.GetComponent<ScrollRect>(),
            shipsContentPanel?.GetComponent<ScrollRect>(),
            captainsContentPanel?.GetComponent<ScrollRect>(),
            destinationsContentPanel?.GetComponent<ScrollRect>(),
            contrabandContentPanel?.GetComponent<ScrollRect>()
        };
        
        foreach (var scrollRect in scrollRects)
        {
            if (scrollRect != null)
            {
                // Apply optimal settings
                scrollRect.movementType = ScrollRect.MovementType.Clamped;
                scrollRect.inertia = true;
                scrollRect.decelerationRate = 0.135f; // Smooth deceleration
                
                // Set scroll sensitivity
                scrollRect.scrollSensitivity = 10f;
                
                // Make sure content is properly set
                if (scrollRect.content == null)
                {
                    // Try to find content transform
                    Transform contentTransform = scrollRect.transform.Find("Content");
                    if (contentTransform != null)
                    {
                        scrollRect.content = contentTransform.GetComponent<RectTransform>();
                    }
                }
                
                // Add ContentSizeFitter if missing
                if (scrollRect.content != null)
                {
                    ContentSizeFitter fitter = scrollRect.content.GetComponent<ContentSizeFitter>();
                    if (fitter == null)
                    {
                        fitter = scrollRect.content.gameObject.AddComponent<ContentSizeFitter>();
                        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Configure button navigation for proper tab interaction
    /// </summary>
    private void ConfigureButtonNavigation()
    {
        // Get all tab buttons
        Button[] tabButtons = new Button[] {
            rulesTabButton,
            shipsTabButton,
            captainsTabButton,
            destinationsTabButton,
            contrabandTabButton
        };
        
        // Configure each button's navigation
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] == null) continue;
            
            // Get navigation
            Navigation nav = tabButtons[i].navigation;
            
            // Set to horizontal mode
            nav.mode = Navigation.Mode.Horizontal;
            
            // Set previous button (or wrap to end)
            int prevIndex = (i - 1 + tabButtons.Length) % tabButtons.Length;
            while (prevIndex != i && tabButtons[prevIndex] == null)
                prevIndex = (prevIndex - 1 + tabButtons.Length) % tabButtons.Length;
            
            // Set next button (or wrap to start)
            int nextIndex = (i + 1) % tabButtons.Length;
            while (nextIndex != i && tabButtons[nextIndex] == null)
                nextIndex = (nextIndex + 1) % tabButtons.Length;
            
            // Apply navigation
            nav.selectOnLeft = tabButtons[prevIndex];
            nav.selectOnRight = tabButtons[nextIndex];
            
            // Update button navigation
            tabButtons[i].navigation = nav;
        }
        
        // Make sure close button is properly isolated
        if (closeButton != null)
        {
            Navigation closeNav = closeButton.navigation;
            closeNav.mode = Navigation.Mode.None;
            closeButton.navigation = closeNav;
        }
    }

    /// <summary>
    /// Add event blockers to content panels to prevent click-through
    /// </summary>
    private void AddEventBlockers()
    {
        GameObject[] contentPanels = {
            rulesContentPanel,
            shipsContentPanel,
            captainsContentPanel,
            destinationsContentPanel,
            contrabandContentPanel
        };
        
        foreach (var panel in contentPanels)
        {
            if (panel == null) continue;
            
            // Add a GraphicRaycaster if missing
            GraphicRaycaster raycaster = panel.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
                panel.AddComponent<GraphicRaycaster>();
                
            // Add a background blocker if needed
            Transform blocker = panel.transform.Find("EventBlocker");
            if (blocker == null && panel.transform.childCount > 0)
            {
                // Create a transparent image that blocks events but doesn't show
                GameObject blockerObj = new GameObject("EventBlocker");
                blockerObj.transform.SetParent(panel.transform, false);
                blockerObj.transform.SetAsFirstSibling(); // Put it behind other content
                
                RectTransform blockerRect = blockerObj.AddComponent<RectTransform>();
                blockerRect.anchorMin = Vector2.zero;
                blockerRect.anchorMax = Vector2.one;
                blockerRect.sizeDelta = Vector2.zero; // Fill the parent
                
                Image blockerImage = blockerObj.AddComponent<Image>();
                blockerImage.color = new Color(0, 0, 0, 0); // Transparent
                blockerImage.raycastTarget = true; // Important: catches raycasts
            }
        }
    }

    /// <summary>
    /// Close the logbook
    /// </summary>
    public void CloseLogBook()
    {
        if (logBookPanel)
        {
            logBookPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Toggle the logbook visibility
    /// </summary>
    public void ToggleLogBook()
    {
        if (logBookPanel)
        {
            if (logBookPanel.activeSelf)
                CloseLogBook();
            else
                OpenLogBook();
        }
    }
    
    /// <summary>
    /// Update the content based on the current tab
    /// </summary>
    public void UpdateContent()
    {
        // Don't update if content manager is missing
        if (contentManager == null)
        {
            Debug.LogWarning("Cannot update logbook content - ContentManager not found");
            return;
        }
        
        switch (currentTab)
        {
            case TabType.Rules:
                UpdateRulesTab();
                break;
                
            case TabType.Ships:
                UpdateShipsTab();
                break;
                
            case TabType.Captains:
                UpdateCaptainsTab();
                break;
                
            case TabType.Destinations:
                UpdateDestinationsTab();
                break;
                
            case TabType.Contraband:
                UpdateContrabandTab();
                break;
        }
    }
    
    /// <summary>
    /// Update the Rules tab content
    /// </summary>
    private void UpdateRulesTab()
    {
        // Clear and build access codes text
        if (accessCodesText != null)
        {
            stringBuilder.Clear();
            stringBuilder.AppendLine("<b>VALID ACCESS CODES:</b>");
            
            foreach (string code in contentManager.currentAccessCodes)
            {
                stringBuilder.AppendLine("- " + code);
            }
            
            accessCodesText.text = stringBuilder.ToString();
        }
        
        // Clear and build daily rules text
        if (dailyRulesText != null)
        {
            stringBuilder.Clear();
            stringBuilder.AppendLine("<b>CURRENT DAY RULES:</b>");
            
            foreach (string rule in contentManager.currentDayRules)
            {
                // Skip access code rules as they're shown separately
                if (!rule.StartsWith("Valid access codes"))
                {
                    stringBuilder.AppendLine("- " + rule);
                }
            }
            
            // If we're on day 3+, add special security notice
            if (contentManager.currentDay >= 3)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("<b>SECURITY ALERT:</b>");
                stringBuilder.AppendLine("- All officers must verify ship origins");
                stringBuilder.AppendLine("- Report suspicious cargo immediately");
                stringBuilder.AppendLine("- Increased vigilance required");
            }
            
            dailyRulesText.text = stringBuilder.ToString();
        }
    }
    
    /// <summary>
    /// Update the Ships tab content
    /// </summary>
    private void UpdateShipsTab()
    {
        // Clear any existing ship entries
        ClearShipEntries();
        
        // Check if we have the ship entry prefab
        if (shipEntryPrefab == null || shipEntriesContainer == null)
        {
            Debug.LogWarning("Ship entry prefab or container is missing");
            return;
        }
        
        // Create a new entry for each ship type
        foreach (var shipType in contentManager.shipTypes)
        {
            if (shipType == null) continue;
            
            // Instantiate entry prefab
            GameObject entryObject = Instantiate(shipEntryPrefab, shipEntriesContainer);
            shipEntryInstances.Add(entryObject);
            
            // Find and configure UI elements
            Transform infoSection = entryObject.transform.Find("InfoSection");
            Transform imageSection = entryObject.transform.Find("ImageSection");
            
            // Set ship name and description
            if (infoSection != null)
            {
                TMP_Text nameText = infoSection.Find("NameText")?.GetComponent<TMP_Text>();
                TMP_Text descText = infoSection.Find("DescriptionText")?.GetComponent<TMP_Text>();
                
                if (nameText != null)
                    nameText.text = shipType.typeName;
                    
                if (descText != null)
                {
                    // Build description text with origins
                    stringBuilder.Clear();
                    
                    // Add ship category if available
                    if (shipType.category != null)
                    {
                        stringBuilder.AppendLine($"Category: {shipType.category.categoryName}");
                    }
                    
                    // Add crew size range
                    stringBuilder.AppendLine($"Crew: {shipType.minCrewSize}-{shipType.maxCrewSize}");
                    
                    // Add approved origins
                    stringBuilder.AppendLine("Approved Origins:");
                    if (shipType.commonOrigins != null && shipType.commonOrigins.Length > 0)
                    {
                        foreach (var origin in shipType.commonOrigins)
                        {
                            stringBuilder.AppendLine($"- {origin}");
                        }
                    }
                    else
                    {
                        stringBuilder.AppendLine("- No specific origins listed");
                    }
                    
                    descText.text = stringBuilder.ToString();
                }
            }
            
            // Set ship image if available
            if (imageSection != null && mediaSystem != null)
            {
                Image shipImage = imageSection.Find("ShipImage")?.GetComponent<Image>();
                
                if (shipImage != null)
                {
                    // Try to get image from the media system
                    Sprite sprite = mediaSystem.GetShipImage(shipType.typeName);
                    
                    // Fallback to ship's icon if available
                    if (sprite == null && shipType.shipIcon != null)
                    {
                        sprite = shipType.shipIcon;
                    }
                    
                    // Set the image if we found one
                    if (sprite != null)
                    {
                        shipImage.sprite = sprite;
                        shipImage.preserveAspect = true;
                        
                        // Make sure the image respects masking
                        shipImage.maskable = true;
                        
                        // Ensure the RectTransform is properly contained
                        RectTransform imageRT = shipImage.GetComponent<RectTransform>();
                        if (imageRT != null)
                        {
                            // Keep the image within its parent container
                            imageRT.anchorMin = new Vector2(0, 0.5f);
                            imageRT.anchorMax = new Vector2(1, 0.5f);
                            imageRT.pivot = new Vector2(0.5f, 0.5f);
                            // Adjust size as needed
                            imageRT.sizeDelta = new Vector2(-20f, 100f); // Width: fill with padding, Height: fixed
                        }
                        
                        shipImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        shipImage.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Ensure all images in the logbook are properly set up for masking
    /// </summary>
    private void EnsureImagesAreMaskable()
    {
        // Get all image components in the log book
        if (logBookPanel != null)
        {
            Image[] allImages = logBookPanel.GetComponentsInChildren<Image>(true);
            foreach (Image img in allImages)
            {
                // Make sure maskable is enabled
                img.maskable = true;
            }
        }
    }


    /// <summary>
    /// Update the Captains tab content
    /// </summary>
    private void UpdateCaptainsTab()
    {
        // Clear any existing captain entries
        ClearCaptainEntries();
        
        // Check if we have the captain entry prefab
        if (captainEntryPrefab == null || captainEntriesContainer == null)
        {
            Debug.LogWarning("Captain entry prefab or container is missing");
            return;
        }
        
        // Create a new entry for each captain type
        foreach (var captainType in contentManager.captainTypes)
        {
            if (captainType == null) continue;
            
            // Instantiate entry prefab
            GameObject entryObject = Instantiate(captainEntryPrefab, captainEntriesContainer);
            captainEntryInstances.Add(entryObject);
            
            // Find and configure UI elements
            Transform infoSection = entryObject.transform.Find("InfoSection");
            Transform imageSection = entryObject.transform.Find("ImageSection");
            
            // Set captain type name and description
            if (infoSection != null)
            {
                TMP_Text nameText = infoSection.Find("NameText")?.GetComponent<TMP_Text>();
                TMP_Text captainNameText = infoSection.Find("CaptainNameText")?.GetComponent<TMP_Text>();
                TMP_Text descText = infoSection.Find("DescriptionText")?.GetComponent<TMP_Text>();
                
                // Set captain type name (the category/type name)
                if (nameText != null)
                {
                    nameText.text = captainType.typeName;
                }
                
                // Set individual captain name (if available)
                if (captainNameText != null)
                {
                    if (captainType.captains != null && captainType.captains.Count > 0)
                    {
                        // Start with the first captain's name
                        captainNameText.text = captainType.captains[0].GetFullName();
                    }
                    else
                    {
                        // If no specific captains, leave blank or use a default
                        captainNameText.text = "";
                    }
                }
                    
                if (descText != null)
                {
                    // Build description text (factions and ranks)
                    stringBuilder.Clear();
                    
                    // Add factions
                    stringBuilder.AppendLine("Factions:");
                    if (captainType.factions != null && captainType.factions.Length > 0)
                    {
                        foreach (var faction in captainType.factions)
                        {
                            stringBuilder.AppendLine($"- {faction}");
                        }
                    }
                    else
                    {
                        stringBuilder.AppendLine("- No specific factions listed");
                    }
                    
                    // Add common ranks
                    stringBuilder.AppendLine("\nCommon Ranks:");
                    if (captainType.commonRanks != null && captainType.commonRanks.Length > 0)
                    {
                        foreach (var rank in captainType.commonRanks)
                        {
                            stringBuilder.AppendLine($"- {rank}");
                        }
                    }
                    else
                    {
                        stringBuilder.AppendLine("- No specific ranks listed");
                    }
                    
                    descText.text = stringBuilder.ToString();
                }
            }

            // Set captain portrait if available
            if (imageSection != null && mediaSystem != null)
            {
                Image captainImage = imageSection.Find("CaptainImage")?.GetComponent<Image>();
                
                if (captainImage != null)
                {
                    // Get faction for image lookup
                    string faction = captainType.factions != null && captainType.factions.Length > 0 
                        ? captainType.factions[0] 
                        : "imperium"; // Default faction
                    
                    // Try to get image from the media system
                    Sprite sprite = mediaSystem.GetCaptainPortrait(faction);
                    
                    // Set the image if we found one
                    if (sprite != null)
                    {
                        captainImage.sprite = sprite;
                        captainImage.preserveAspect = true;
                        captainImage.gameObject.SetActive(true);
                        
                        // Find the button component
                        Button imageButton = captainImage.GetComponent<Button>();
                        
                        // Add button functionality if captains exist
                        if (imageButton != null && captainType.captains != null && captainType.captains.Count > 0)
                        {
                            int currentCaptainIndex = 0;
                            imageButton.onClick.RemoveAllListeners();
                            imageButton.onClick.AddListener(() => 
                            {
                                // Cycle to next captain
                                currentCaptainIndex = (currentCaptainIndex + 1) % captainType.captains.Count;
                                
                                // Update individual captain name text
                                TMP_Text captainNameText = infoSection.Find("CaptainNameText")?.GetComponent<TMP_Text>();
                                if (captainNameText != null)
                                {
                                    captainNameText.text = captainType.captains[currentCaptainIndex].GetFullName();
                                }
                                
                                // Update portrait
                                captainImage.sprite = sprite;
                            });
                        }
                    }
                    else
                    {
                        captainImage.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Update the Destinations tab content
    /// </summary>
    private void UpdateDestinationsTab()
    {
        // Clear any existing destination entries
        ClearDestinationEntries();
        
        // Check if we have the destination entry prefab
        if (destinationEntryPrefab == null || destinationEntriesContainer == null)
        {
            Debug.LogWarning("Destination entry prefab or container is missing");
            return;
        }
        
        // Create entries for destinations
        HashSet<string> allOrigins = new HashSet<string>();
        
        foreach (var shipType in contentManager.shipTypes)
        {
            if (shipType != null && shipType.commonOrigins != null)
            {
                foreach (var origin in shipType.commonOrigins)
                {
                    if (!string.IsNullOrEmpty(origin))
                        allOrigins.Add(origin);
                }
            }
        }
        
        // Instantiate an entry for Starkiller Base
        GameObject baseEntry = Instantiate(destinationEntryPrefab, destinationEntriesContainer);
        destinationEntryInstances.Add(baseEntry);
        
        TMP_Text nameText = baseEntry.transform.Find("InfoSection/NameText")?.GetComponent<TMP_Text>();
        TMP_Text descText = baseEntry.transform.Find("InfoSection/DescriptionText")?.GetComponent<TMP_Text>();
        
        if (nameText != null)
            nameText.text = "Imperium Megastation";
        
        if (descText != null)
        {
            stringBuilder.Clear();
            stringBuilder.AppendLine("Primary Imperium military installation");
            stringBuilder.AppendLine("\nApproved Origins:");
            
            foreach (var origin in allOrigins)
            {
                stringBuilder.AppendLine($"- {origin}");
            }
            
            if (contentManager.currentDay >= 2)
            {
                stringBuilder.AppendLine("\n<b>NOTE:</b> Origin validation now required per Directive 7-A");
            }
            
            descText.text = stringBuilder.ToString();
        }
    }

    /// <summary>
    /// Clear all instantiated destination entries
    /// </summary>
    private void ClearDestinationEntries()
    {
        foreach (var entry in destinationEntryInstances)
        {
            if (entry != null)
                Destroy(entry);
        }
        
        destinationEntryInstances.Clear();
    }
    
    /// <summary>
    /// Update the Contraband tab content
    /// </summary>
    private void UpdateContrabandTab()
    {
        // Clear existing entries
        ClearContrabandEntries();
        
        // Check if we have the contraband entry prefab
        if (contrabandEntryPrefab == null || contrabandEntriesContainer == null)
        {
            Debug.LogWarning("Contraband entry prefab or container is missing");
            
            // Fallback to using the text directly if available
            if (contrabandContentPanel != null)
            {
                TMP_Text contentText = contrabandContentPanel.GetComponentInChildren<TMP_Text>();
                
                if (contentText != null)
                {
                    stringBuilder.Clear();
                    stringBuilder.AppendLine("<b>PROHIBITED ITEMS</b>\n");
                    stringBuilder.AppendLine("The following items are forbidden on Starkiller Base:");
                    
                    // List prohibited items by category
                    stringBuilder.AppendLine("\n<b>WEAPONS</b>");
                    stringBuilder.AppendLine("- Unauthorized blasters");
                    stringBuilder.AppendLine("- Explosives");
                    stringBuilder.AppendLine("- Projectile weapons");
                    stringBuilder.AppendLine("- Lightsabers and related technology");
                    
                    stringBuilder.AppendLine("\n<b>COMMUNICATIONS</b>");
                    stringBuilder.AppendLine("- Unregistered comm devices");
                    stringBuilder.AppendLine("- Jamming equipment");
                    stringBuilder.AppendLine("- Long-range transmitters");
                    
                    stringBuilder.AppendLine("\n<b>DATA</b>");
                    stringBuilder.AppendLine("- Unauthorized data storage");
                    stringBuilder.AppendLine("- Imperial archive materials");
                    stringBuilder.AppendLine("- First Order tactical information");
                    
                    stringBuilder.AppendLine("\n<b>BIOLOGICALS</b>");
                    stringBuilder.AppendLine("- Unregistered flora/fauna");
                    stringBuilder.AppendLine("- Unquarantined specimens");
                    stringBuilder.AppendLine("- Unidentified organic material");
                    
                    // Add day-specific notes
                    if (contentManager.currentDay >= 3)
                    {
                        stringBuilder.AppendLine("\n<b>SPECIAL ALERT:</b>");
                        stringBuilder.AppendLine("Recent intelligence suggests rebels may attempt to smuggle");
                        stringBuilder.AppendLine("contraband disguised as standard supplies.");
                        stringBuilder.AppendLine("All manifests must be thoroughly inspected.");
                    }
                    
                    contentText.text = stringBuilder.ToString();
                }
            }
            
            return;
        }
        
        // Create entries for contraband categories if the prefab is available
        string[] contrabandCategories = {
            "WEAPONS", "COMMUNICATIONS", "DATA", "BIOLOGICALS"
        };
        
        foreach (var category in contrabandCategories)
        {
            GameObject categoryEntry = Instantiate(contrabandEntryPrefab, contrabandEntriesContainer);
            contrabandEntryInstances.Add(categoryEntry);
            
            TMP_Text nameText = categoryEntry.transform.Find("InfoSection/NameText")?.GetComponent<TMP_Text>();
            TMP_Text descText = categoryEntry.transform.Find("InfoSection/DescriptionText")?.GetComponent<TMP_Text>();
            
            if (nameText != null)
                nameText.text = category;
            
            if (descText != null)
            {
                // Populate description based on category
                descText.text = GetContrabandDescription(category);
            }
        }
        
        // Add special alert for day 3+
        if (contentManager.currentDay >= 3)
        {
            GameObject alertEntry = Instantiate(contrabandEntryPrefab, contrabandEntriesContainer);
            contrabandEntryInstances.Add(alertEntry);
            
            TMP_Text nameText = alertEntry.transform.Find("InfoSection/NameText")?.GetComponent<TMP_Text>();
            TMP_Text descText = alertEntry.transform.Find("InfoSection/DescriptionText")?.GetComponent<TMP_Text>();
            
            if (nameText != null)
                nameText.text = "SPECIAL ALERT";
            
            if (descText != null)
            {
                descText.text = "Recent intelligence suggests rebels may attempt to smuggle contraband disguised as standard supplies.\n\nAll manifests must be thoroughly inspected.";
            }
        }
    }
    
    /// <summary>
    /// Get the description text for a contraband category
    /// </summary>
    private string GetContrabandDescription(string category)
    {
        switch (category)
        {
            case "WEAPONS":
                return "- Unauthorized blasters\n- Explosives\n- Projectile weapons\n- Lightsabers and related technology";
                
            case "COMMUNICATIONS":
                return "- Unregistered comm devices\n- Jamming equipment\n- Long-range transmitters\n- Encrypted communication tools";
                
            case "DATA":
                return "- Unauthorized data storage\n- Imperial archive materials\n- First Order tactical information\n- Rebel alliance intelligence";
                
            case "BIOLOGICALS":
                return "- Unregistered flora/fauna\n- Unquarantined specimens\n- Unidentified organic material\n- Prohibited medical substances";
                
            default:
                return "No specific items listed.";
        }
    }
    
    /// <summary>
    /// Clear all instantiated contraband entries
    /// </summary>
    private void ClearContrabandEntries()
    {
        foreach (var entry in contrabandEntryInstances)
        {
            if (entry != null)
                Destroy(entry);
        }
        
        contrabandEntryInstances.Clear();
    }
    
    /// <summary>
    /// Clear all instantiated ship entries
    /// </summary>
    private void ClearShipEntries()
    {
        foreach (var entry in shipEntryInstances)
        {
            if (entry != null)
                Destroy(entry);
        }
        
        shipEntryInstances.Clear();
    }
    
    private void DebugScrollRect(ScrollRect scrollRect, string name)
    {
        if (scrollRect == null)
        {
            Debug.LogError($"{name} ScrollRect is null");
            return;
        }
        
        Debug.Log($"--- {name} ScrollRect Debug ---");
        Debug.Log($"Content assigned: {scrollRect.content != null}");
        Debug.Log($"Viewport assigned: {scrollRect.viewport != null}");
        
        if (scrollRect.content != null)
        {
            ContentSizeFitter fitter = scrollRect.content.GetComponent<ContentSizeFitter>();
            Debug.Log($"Content has ContentSizeFitter: {fitter != null}");
            if (fitter != null)
            {
                Debug.Log($"Vertical fit mode: {fitter.verticalFit}");
            }
            
            VerticalLayoutGroup vlg = scrollRect.content.GetComponent<VerticalLayoutGroup>();
            Debug.Log($"Content has VerticalLayoutGroup: {vlg != null}");
            
            Debug.Log($"Content size: {scrollRect.content.rect.size}");
            Debug.Log($"Content child count: {scrollRect.content.childCount}");
        }
        
        if (scrollRect.viewport != null)
        {
            Mask mask = scrollRect.viewport.GetComponent<Mask>();
            RectMask2D rectMask = scrollRect.viewport.GetComponent<RectMask2D>();
            Debug.Log($"Viewport has Mask: {mask != null}");
            Debug.Log($"Viewport has RectMask2D: {rectMask != null}");
            Debug.Log($"Viewport size: {scrollRect.viewport.rect.size}");
        }
    }

    /// <summary>
    /// Clear all instantiated captain entries
    /// </summary>
    private void ClearCaptainEntries()
    {
        foreach (var entry in captainEntryInstances)
        {
            if (entry != null)
                Destroy(entry);
        }
        
        captainEntryInstances.Clear();
    }
}