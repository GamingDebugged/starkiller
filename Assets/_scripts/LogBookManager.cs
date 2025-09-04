using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Ship Image Sprite Management
[System.Serializable]
public class ShipData
{
    public string name;
    public string description;
    public Sprite shipImage;
}

/// <summary>
/// Manages the log book interface with tabs for different information
/// </summary>
public class LogBookManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject logBookPanel;           // Main log book panel
    public TMP_Text contentText;              // Text area for displaying content
    
    [Header("Tab Buttons")]
    public Button rulesTabButton;             // Button for rules tab
    public Button shipTypesTabButton;         // Button for ship types tab
    public Button destinationsTabButton;      // Button for destinations tab
    
    [Header("Tab Indicators")]
    public GameObject rulesTabIndicator;      // Visual indicator for active rules tab
    public GameObject shipTypesTabIndicator;  // Visual indicator for active ship types tab
    public GameObject destinationsTabIndicator; // Visual indicator for active destinations tab
    
    [Header("Ship Display")]
    public GameObject shipsContainer;         // Container for ship entries
    public GameObject shipEntryPrefab;        // Prefab for ship entries with image
    public List<ShipData> shipDataList = new List<ShipData>(); // Ship data list
    public Sprite defaultShipImage;           // Default image if none assigned

    [Header("Navigation")]
    public Button closeButton;                // Button to close log book
    
    // Tab indices
    public enum TabType { Rules, ShipTypes, Destinations }
    private TabType currentTab = TabType.Rules;
    
    // Reference to ship encounter system for data
    private ShipEncounterSystem shipEncounterSystem;
    
    void Start()
    {
        // Find ShipEncounterSystem reference
        shipEncounterSystem = ShipEncounterSystem.Instance;
        if (shipEncounterSystem == null)
        {
            Debug.LogError("ShipEncounterSystem not found!");
        }
        
        // Setup button listeners
        if (rulesTabButton) rulesTabButton.onClick.AddListener(() => SwitchTab(TabType.Rules));
        if (shipTypesTabButton) shipTypesTabButton.onClick.AddListener(() => SwitchTab(TabType.ShipTypes));
        if (destinationsTabButton) destinationsTabButton.onClick.AddListener(() => SwitchTab(TabType.Destinations));
        if (closeButton) closeButton.onClick.AddListener(CloseLogBook);
        
        // Hide the panel initially
        if (logBookPanel) logBookPanel.SetActive(false);
        
        // Set initial tab
        SwitchTab(TabType.Rules);
    }
    
    /// <summary>
    /// Open the log book panel
    /// </summary>
    public void OpenLogBook()
    {
        if (logBookPanel)
        {
            logBookPanel.SetActive(true);
            UpdateContent();
        }
    }
    
    /// <summary>
    /// Close the log book panel
    /// </summary>
    public void CloseLogBook()
    {
        if (logBookPanel)
        {
            logBookPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Switch to the specified tab
    /// </summary>
    public void SwitchTab(TabType tab)
    {
        currentTab = tab;
        
        // Update tab indicators
        if (rulesTabIndicator) rulesTabIndicator.SetActive(tab == TabType.Rules);
        if (shipTypesTabIndicator) shipTypesTabIndicator.SetActive(tab == TabType.ShipTypes);
        if (destinationsTabIndicator) destinationsTabIndicator.SetActive(tab == TabType.Destinations);
        
        // Update content
        UpdateContent();
    }
    
    /// <summary>
    /// Update the content based on the current tab
    /// </summary>
    public void UpdateContent()
    {
        if (contentText == null || shipEncounterSystem == null) return;
        
        string content = "";
        
        switch(currentTab)
        {
            case TabType.Rules:
                content = GenerateRulesContent();
                break;
                
            case TabType.ShipTypes:
                content = GenerateShipTypesContent();
                break;
                
            case TabType.Destinations:
                content = GenerateDestinationsContent();
                break;
        }
        
        contentText.text = content;
    }
    
    /// <summary>
    /// Generate content for the Rules tab
    /// </summary>
    private string GenerateRulesContent()
    {
        string content = "<b>DAILY RULES AND PROTOCOLS</b>\n\n";
        
        // Add access codes
        content += "<b>VALID ACCESS CODES:</b>\n";
        foreach (var code in shipEncounterSystem.validAccessCodes)
        {
            content += "- " + code + "\n";
        }
        
        // Add current rules (if any)
        content += "\n<b>SPECIAL INSTRUCTIONS:</b>\n";
        List<string> rules = shipEncounterSystem.GetDailyRules(GetCurrentDay());
        foreach (var rule in rules)
        {
            if (!rule.StartsWith("Valid access codes"))  // Skip access codes as we already displayed them
            {
                content += "- " + rule + "\n";
            }
        }
        
        content += "\n<b>SECURITY ALERT:</b>\n";
        content += "- All officers must be vigilant for rebel sympathizers\n";
        content += "- Report suspicious behavior immediately\n";
        
        return content;
    }
    
    /// <summary>
    /// Generate content for the Ship Types tab
    /// </summary>
    private string GenerateShipTypesContent()
    {
        string content = "<b>APPROVED SHIP TYPES</b>\n\n";
        
        foreach (var shipType in shipEncounterSystem.shipTypes)
        {
            content += "<b>" + shipType.name + "</b>\n";
            
            // Add a generic description for each ship type
            switch(shipType.name)
            {
                case "Lambda Shuttle":
                    content += "Imperium officer transport\n";
                    break;
                case "Cargo Freighter":
                    content += "Standard cargo transport\n";
                    break;
                case "TIE Fighter":
                    content += "First Order short-range fighter\n";
                    break;
                case "Imperium Transport":
                    content += "Personnel carrier\n";
                    break;
                case "Supply Ship":
                    content += "Resource transport vessel\n";
                    break;
                case "Command Shuttle":
                    content += "High-ranking officer transport\n";
                    break;
                default:
                    content += "Authorized First Order vessel\n";
                    break;
            }
            
            content += "\n";
        }
        
        return content;
    }
    
    /// <summary>
    /// Generate content for the Destinations tab
    /// </summary>
    private string GenerateDestinationsContent()
    {
        string content = "<b>AUTHORIZED DESTINATIONS</b>\n\n";
        
        foreach (var dest in shipEncounterSystem.destinations)
        {
            content += "<b>" + dest.name + "</b>\n";
            content += "Approved origins:\n";
            
            foreach (var origin in dest.validOrigins)
            {
                content += "- " + origin + "\n";
            }
            
            content += "\n";
        }
        
        return content;
    }
    
    /// <summary>
    /// Get the current day from the GameManager
    /// </summary>
    private int GetCurrentDay()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            return gameManager.currentDay;
        }
        
        return 1; // Default to day 1 if GameManager not found
    }
}