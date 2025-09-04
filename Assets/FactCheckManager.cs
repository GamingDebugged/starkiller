using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the FactCheck feature, allowing players to hover over encounter details
/// and quickly access relevant reference information in the LogBook
/// </summary>
public class FactCheckManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the LogBook manager")]
    public StarkkillerLogBookManager logBookManager;
    
    [Tooltip("Reference to the credential checker")]
    public CredentialChecker credentialChecker;
    
    [Header("UI Elements")]
    [Tooltip("Prefab for the FactCheck indicator")]
    public GameObject factCheckIndicatorPrefab;
    
    [Tooltip("Parent transform for spawning indicators")]
    public Transform indicatorParent;
    
    [Tooltip("Hover tooltip prefab")]
    public GameObject tooltipPrefab;
    
    [Header("Settings")]
    [Tooltip("How long to hover before showing tooltip (seconds)")]
    public float hoverTime = 0.5f;
    
    [Header("Debug Settings")]
    [Tooltip("Enable verbose debug logging")]
    public bool enableDebugLogs = true;
    
    // Dictionary mapping interactive elements to their LogBook tabs
    private Dictionary<RectTransform, StarkkillerLogBookManager.TabType> factCheckAreas = 
        new Dictionary<RectTransform, StarkkillerLogBookManager.TabType>();
    
    // Currently active tooltip
    private GameObject activeTooltip;
    
    // Hover timer
    private float currentHoverTime = 0f;
    
    // Currently hovered element
    private RectTransform currentHoverElement;
    
    // Are we currently tracking mouse position
    private bool isTrackingMouse = false;
    
    void Start()
    {
        // Find references if not assigned
        if (logBookManager == null)
        {
            logBookManager = FindAnyObjectByType<StarkkillerLogBookManager>();
            if (logBookManager != null && enableDebugLogs)
                Debug.Log("FactCheckManager: Found LogBookManager automatically");
            else if (enableDebugLogs)
                Debug.LogError("FactCheckManager: Could not find LogBookManager!");
        }
            
        if (credentialChecker == null)
        {
            credentialChecker = FindAnyObjectByType<CredentialChecker>();
            if (credentialChecker != null && enableDebugLogs)
                Debug.Log("FactCheckManager: Found CredentialChecker automatically");
            else if (enableDebugLogs)
                Debug.LogError("FactCheckManager: Could not find CredentialChecker!");
        }
            
        // Setup initial indicators based on the current encounter
        if (credentialChecker != null && credentialChecker.HasActiveEncounter())
        {
            Debug.Log("FactCheckManager: CredentialChecker has an active encounter, setting up indicators");
            SetupFactCheckAreasForCurrentEncounter();
        }
        else
        {
            Debug.Log("FactCheckManager: No active encounter yet, waiting to setup indicators");
        }
        
        // Start mouse tracking
        isTrackingMouse = true;
        
        // Force activate one test indicator just to confirm visibility
        CreateTestIndicator();
    }
    
    void Update()
    {
        if (!isTrackingMouse)
            return;
            
        // Check for hover over fact check areas
        if (Input.GetMouseButtonDown(0))
        {
            // Check if we clicked on a fact check area
            RectTransform clickedArea = GetFactCheckAreaUnderMouse();
            if (clickedArea != null)
            {
                if (enableDebugLogs)
                    Debug.Log($"FactCheckManager: Clicked on fact check area for {factCheckAreas[clickedArea]}");
                OpenLogBookForArea(clickedArea);
            }
        }
        
        // Handle hover logic
        RectTransform hoveredArea = GetFactCheckAreaUnderMouse();
        
        if (hoveredArea != null)
        {
            if (hoveredArea != currentHoverElement)
            {
                // Reset timer for new element
                currentHoverElement = hoveredArea;
                currentHoverTime = 0f;
                HideTooltip();
                
                if (enableDebugLogs)
                    Debug.Log($"FactCheckManager: Hovering over new fact check area for {factCheckAreas[hoveredArea]}");
            }
            else
            {
                // Increment timer for current element
                currentHoverTime += Time.deltaTime;
                
                // Show tooltip after delay
                if (currentHoverTime >= hoverTime && activeTooltip == null)
                {
                    if (enableDebugLogs)
                        Debug.Log($"FactCheckManager: Showing tooltip for {factCheckAreas[hoveredArea]}");
                    ShowTooltip(hoveredArea);
                }
            }
        }
        else if (currentHoverElement != null)
        {
            // No longer hovering over an element
            if (enableDebugLogs)
                Debug.Log("FactCheckManager: No longer hovering over any fact check area");
            currentHoverElement = null;
            HideTooltip();
        }
    }
    
    /// <summary>
    /// Create a test indicator to verify visibility
    /// </summary>
    private void CreateTestIndicator()
    {
        if (factCheckIndicatorPrefab == null || indicatorParent == null)
        {
            Debug.LogError("FactCheckManager: Cannot create test indicator - missing prefab or parent");
            return;
        }
        
        // Create a GameObject to act as our test area
        GameObject testArea = new GameObject("TestFactCheckArea");
        testArea.transform.SetParent(indicatorParent, false);
        
        // Add a RectTransform
        RectTransform testRect = testArea.AddComponent<RectTransform>();
        testRect.anchoredPosition = new Vector2(100, 100);
        testRect.sizeDelta = new Vector2(200, 50);
        
        // Add a transparent Image component for raycasting
        Image testImage = testArea.AddComponent<Image>();
        testImage.color = new Color(1, 0, 0, 0.2f); // Semi-transparent red for debugging
        testImage.raycastTarget = true;
        
        // Add a TextMeshPro component
        TextMeshProUGUI testText = testArea.AddComponent<TextMeshProUGUI>();
        testText.text = "TEST AREA (SHIP TYPE)";
        testText.color = Color.white;
        testText.fontSize = 14;
        testText.alignment = TextAlignmentOptions.Center;
        
        // Add to dictionary
        factCheckAreas[testRect] = StarkkillerLogBookManager.TabType.Ships;
        
        // Create indicator as a child
        GameObject indicator = Instantiate(factCheckIndicatorPrefab, testRect);
        RectTransform indicatorRect = indicator.GetComponent<RectTransform>();
        
        // Position at the right side
        indicatorRect.anchorMin = new Vector2(1, 0.5f);
        indicatorRect.anchorMax = new Vector2(1, 0.5f);
        indicatorRect.pivot = new Vector2(1, 0.5f);
        indicatorRect.anchoredPosition = new Vector2(-5, 0);
        
        Debug.Log("FactCheckManager: Created test indicator at position (100, 100)");
    }
    
    /// <summary>
    /// Setup fact check areas based on current encounter in CredentialChecker
    /// </summary>
    public void SetupFactCheckAreasForCurrentEncounter()
    {
        if (credentialChecker == null)
        {
            Debug.LogError("FactCheckManager: Cannot setup areas - credentialChecker is null");
            return;
        }
        
        Debug.Log("FactCheckManager: Setting up fact check areas for current encounter");
        
        // Create ship type fact check area
        if (credentialChecker.shipInfoPanel != null)
        {
            // Get the RectTransform of the shipInfoPanel
            RectTransform shipInfoRect = credentialChecker.shipInfoPanel.GetComponent<RectTransform>();
            if (shipInfoRect != null)
            {
                // Create a ship type fact check area
                CreateFactCheckAreaFromPanel(
                    shipInfoRect, 
                    "SHIP TYPE", 
                    StarkkillerLogBookManager.TabType.Ships,
                    new Vector2(0, 0.9f), // Anchor at top-left of panel
                    new Vector2(0.5f, 1f), // Cover top half width
                    new Vector2(10, -10) // Offset
                );
                
                // Create a captain fact check area
                CreateFactCheckAreaFromPanel(
                    shipInfoRect, 
                    "CAPTAIN", 
                    StarkkillerLogBookManager.TabType.Captains,
                    new Vector2(0, 0.5f), // Anchor at middle-left of panel
                    new Vector2(0.5f, 0.7f), // Cover middle section
                    new Vector2(10, -10) // Offset
                );
                
                // Create an origin fact check area
                CreateFactCheckAreaFromPanel(
                    shipInfoRect, 
                    "ORIGIN", 
                    StarkkillerLogBookManager.TabType.Destinations,
                    new Vector2(0, 0.3f), // Anchor at lower-left of panel
                    new Vector2(0.5f, 0.5f), // Cover lower section
                    new Vector2(10, -10) // Offset
                );
            }
        }
        
        // Create access code fact check area
        if (credentialChecker.credentialsPanel != null)
        {
            // Get the RectTransform of the credentialsPanel
            RectTransform credentialsRect = credentialChecker.credentialsPanel.GetComponent<RectTransform>();
            if (credentialsRect != null)
            {
                // Create an access code fact check area
                CreateFactCheckAreaFromPanel(
                    credentialsRect, 
                    "ACCESS CODE", 
                    StarkkillerLogBookManager.TabType.Rules,
                    new Vector2(0, 0.8f), // Anchor at top-left of panel
                    new Vector2(1f, 1f), // Cover top section
                    new Vector2(10, -10) // Offset
                );
                
                // Create a manifest fact check area
                CreateFactCheckAreaFromPanel(
                    credentialsRect, 
                    "MANIFEST", 
                    StarkkillerLogBookManager.TabType.Contraband,
                    new Vector2(0, 0.4f), // Anchor at middle-left of panel
                    new Vector2(1f, 0.7f), // Cover middle section
                    new Vector2(10, -10) // Offset
                );
            }
        }
    }
    
    /// <summary>
    /// Create a fact check area from a panel's RectTransform
    /// </summary>
    private void CreateFactCheckAreaFromPanel(
        RectTransform parentRect, 
        string label, 
        StarkkillerLogBookManager.TabType tabType,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offset)
    {
        if (factCheckIndicatorPrefab == null || indicatorParent == null)
        {
            Debug.LogError("FactCheckManager: Cannot create fact check area - missing prefab or parent");
            return;
        }
        
        // Create a GameObject for the area
        GameObject areaObj = new GameObject($"FactCheckArea_{label}");
        areaObj.transform.SetParent(indicatorParent, false);
        
        // Add a RectTransform positioned relative to parent
        RectTransform areaRect = areaObj.AddComponent<RectTransform>();
        
        // Convert parent rect's position and size to screen space
        Vector2 parentPos = parentRect.position;
        Vector2 parentSize = parentRect.rect.size;
        
        // Calculate area position and size based on anchor
        Vector2 areaMin = new Vector2(
            parentPos.x + (parentSize.x * anchorMin.x) + offset.x,
            parentPos.y + (parentSize.y * anchorMin.y) + offset.y
        );
        
        Vector2 areaMax = new Vector2(
            parentPos.x + (parentSize.x * anchorMax.x) - offset.x,
            parentPos.y + (parentSize.y * anchorMax.y) - offset.y
        );
        
        // Set position and size
        areaRect.position = (areaMin + areaMax) / 2f;
        areaRect.sizeDelta = areaMax - areaMin;
        
        // Add a transparent Image component for raycasting
        Image areaImage = areaObj.AddComponent<Image>();
        areaImage.color = new Color(0, 1, 0, 0.2f); // Semi-transparent green for debugging
        areaImage.raycastTarget = true;
        
        // Add a TextMeshPro component for debugging
        TextMeshProUGUI areaText = areaObj.AddComponent<TextMeshProUGUI>();
        areaText.text = label;
        areaText.color = Color.white;
        areaText.fontSize = 12;
        areaText.alignment = TextAlignmentOptions.Center;
        
        // Add to dictionary
        factCheckAreas[areaRect] = tabType;
        
        // Create indicator as a child
        GameObject indicator = Instantiate(factCheckIndicatorPrefab, areaRect);
        RectTransform indicatorRect = indicator.GetComponent<RectTransform>();
        
        // Position at the right side
        indicatorRect.anchorMin = new Vector2(1, 0.5f);
        indicatorRect.anchorMax = new Vector2(1, 0.5f);
        indicatorRect.pivot = new Vector2(1, 0.5f);
        indicatorRect.anchoredPosition = new Vector2(-5, 0);
        
        Debug.Log($"FactCheckManager: Created fact check area for {label} pointing to {tabType} tab");
    }
    
    /// <summary>
    /// Get the fact check area under the mouse cursor
    /// </summary>
    private RectTransform GetFactCheckAreaUnderMouse()
    {
        // Use EventSystem to check if we're hovering over a UI element
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            if (enableDebugLogs && results.Count > 0)
                Debug.Log($"FactCheckManager: Found {results.Count} raycast results under mouse");
            
            foreach (var result in results)
            {
                RectTransform resultRect = result.gameObject.GetComponent<RectTransform>();
                if (resultRect != null && factCheckAreas.ContainsKey(resultRect))
                {
                    if (enableDebugLogs)
                        Debug.Log($"FactCheckManager: Found fact check area for {factCheckAreas[resultRect]}");
                    return resultRect;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Show a tooltip for the hovered element
    /// </summary>
    private void ShowTooltip(RectTransform hoveredArea)
    {
        if (tooltipPrefab == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning("FactCheckManager: tooltipPrefab is null, cannot show tooltip");
            return;
        }
            
        // Create tooltip
        activeTooltip = Instantiate(tooltipPrefab, hoveredArea);
        
        // Position tooltip
        RectTransform tooltipRect = activeTooltip.GetComponent<RectTransform>();
        tooltipRect.anchorMin = new Vector2(0.5f, 0);
        tooltipRect.anchorMax = new Vector2(0.5f, 0);
        tooltipRect.pivot = new Vector2(0.5f, 1);
        tooltipRect.anchoredPosition = new Vector2(0, -5); // Just below the area
        
        // Ensure tooltip is active
        activeTooltip.SetActive(true);
        
        // Set tooltip text
        TMP_Text tooltipText = activeTooltip.GetComponentInChildren<TMP_Text>();
        if (tooltipText != null)
        {
            StarkkillerLogBookManager.TabType tabType = factCheckAreas[hoveredArea];
            tooltipText.text = $"Check {tabType} in LogBook";
            
            if (enableDebugLogs)
                Debug.Log($"FactCheckManager: Showing tooltip for {tabType}");
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning("FactCheckManager: Could not find text component in tooltip");
        }
    }
    
    /// <summary>
    /// Hide the active tooltip
    /// </summary>
    private void HideTooltip()
    {
        if (activeTooltip != null)
        {
            if (enableDebugLogs)
                Debug.Log("FactCheckManager: Hiding tooltip");
                
            Destroy(activeTooltip);
            activeTooltip = null;
        }
    }

    /// <summary>
    /// Open the LogBook to the tab associated with the clicked area
    /// </summary>
    private void OpenLogBookForArea(RectTransform clickedArea)
    {
        if (logBookManager == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning("FactCheckManager: LogBookManager is null, cannot open LogBook");
            return;
        }
            
        if (!factCheckAreas.ContainsKey(clickedArea))
        {
            if (enableDebugLogs)
                Debug.LogWarning("FactCheckManager: Clicked area not found in factCheckAreas dictionary");
            return;
        }
            
        // Get the tab type for this area
        StarkkillerLogBookManager.TabType tabType = factCheckAreas[clickedArea];
        
        if (enableDebugLogs)
            Debug.Log($"FactCheckManager: Opening LogBook to tab: {tabType}");
        
        // Open the LogBook and switch to the correct tab
        logBookManager.OpenLogBook();
        logBookManager.SwitchTab(tabType);
    }
    
    /// <summary>
    /// Set up fact check areas for a ship encounter
    /// </summary>
    public void SetupFactCheckAreas(MasterShipEncounter encounter)
    {
        // Clear existing areas
        ClearFactCheckAreas();
        
        if (encounter == null || credentialChecker == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning("FactCheckManager: Cannot set up fact check areas - encounter or credential checker is null");
            return;
        }
            
        if (enableDebugLogs)
            Debug.Log($"FactCheckManager: Setting up fact check areas for ship: {encounter.shipType}");
            
        // Set up areas based on the current UI
        SetupFactCheckAreasForCurrentEncounter();
    }
    
    /// <summary>
    /// Clear all fact check areas
    /// </summary>
    private void ClearFactCheckAreas()
    {
        if (enableDebugLogs)
            Debug.Log($"FactCheckManager: Clearing {factCheckAreas.Count} fact check areas");
            
        foreach (var area in factCheckAreas.Keys)
        {
            if (area != null)
                Destroy(area.gameObject);
        }
        
        factCheckAreas.Clear();
    }
}