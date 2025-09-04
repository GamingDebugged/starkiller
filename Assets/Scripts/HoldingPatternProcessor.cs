using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Handles ships in holding pattern with visual feedback and timers
/// Acts as a central manager for all ships currently in holding pattern
/// </summary>
public class HoldingPatternProcessor : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject holdingPatternPanel;
    [SerializeField] private Transform holdingPatternEntriesContainer;
    [SerializeField] private GameObject holdingPatternEntryPrefab;
    [SerializeField] private TMP_Text notificationText;
    
    [Header("Settings")]
    [SerializeField] private int maxShipsInHoldingPattern = 3;
    [SerializeField] private float showNotificationTime = 3.0f;
    
    // Reference to other systems
    private MasterShipGenerator shipGenerator;
    private GameManager gameManager;
    
    // Track ships in holding pattern
    private List<HoldingPatternEntry> activeEntries = new List<HoldingPatternEntry>();
    
    // Singleton pattern
    private static HoldingPatternProcessor _instance;
    public static HoldingPatternProcessor Instance => _instance;
    
    private void Awake()
    {
        // Setup singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Find references if not assigned
        if (shipGenerator == null)
            shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
            
        // Find UI elements if not assigned
        FindUIReferences();
                
        // Hide panel by default
        if (holdingPatternPanel != null)
            holdingPatternPanel.SetActive(false);
            
        // Hide notification text by default
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Find UI references if they're not assigned
    /// </summary>
    private void FindUIReferences()
    {
        // Find holding pattern panel
        if (holdingPatternPanel == null)
        {
            holdingPatternPanel = GameObject.Find("Canvas/SafeArea/HoldingPatternPanel");
            if (holdingPatternPanel == null)
            {
                // Try a more general search
                holdingPatternPanel = GameObject.Find("HoldingPatternPanel");
                if (holdingPatternPanel == null)
                {
                    Debug.LogError("HoldingPatternProcessor: Could not find HoldingPatternPanel in scene!");
                }
                else
                {
                    Debug.Log("HoldingPatternProcessor: Found HoldingPatternPanel with general search");
                }
            }
            else
            {
                Debug.Log("HoldingPatternProcessor: Found HoldingPatternPanel at Canvas/SafeArea/HoldingPatternPanel");
            }
        }
        
        // Find entries container
        if (holdingPatternEntriesContainer == null && holdingPatternPanel != null)
        {
            Transform container = holdingPatternPanel.transform.Find("EntryContainer");
            if (container != null)
            {
                holdingPatternEntriesContainer = container;
                Debug.Log("HoldingPatternProcessor: Found EntryContainer as child of HoldingPatternPanel");
            }
            else
            {
                // Try a more general search
                GameObject containerObj = GameObject.Find("Canvas/SafeArea/HoldingPatternPanel/EntryContainer");
                if (containerObj != null)
                {
                    holdingPatternEntriesContainer = containerObj.transform;
                    Debug.Log("HoldingPatternProcessor: Found EntryContainer with path search");
                }
                else
                {
                    containerObj = GameObject.Find("EntryContainer");
                    if (containerObj != null)
                    {
                        holdingPatternEntriesContainer = containerObj.transform;
                        Debug.Log("HoldingPatternProcessor: Found EntryContainer with general search");
                    }
                    else
                    {
                        Debug.LogError("HoldingPatternProcessor: Could not find EntryContainer!");
                    }
                }
            }
        }
        
        // Find entry prefab
        if (holdingPatternEntryPrefab == null)
        {
            holdingPatternEntryPrefab = GameObject.Find("Canvas/SafeArea/GameplayPanel/HoldingPatternGroup/HoldingPatternEntry");
            if (holdingPatternEntryPrefab == null)
            {
                // Try a more general search
                holdingPatternEntryPrefab = GameObject.Find("HoldingPatternEntry");
                if (holdingPatternEntryPrefab == null)
                {
                    Debug.LogError("HoldingPatternProcessor: Could not find HoldingPatternEntry prefab in scene!");
                    
                    // Look for resources/prefabs
                    holdingPatternEntryPrefab = Resources.Load<GameObject>("Prefabs/HoldingPatternEntry");
                    if (holdingPatternEntryPrefab != null)
                    {
                        Debug.Log("HoldingPatternProcessor: Found HoldingPatternEntry prefab in Resources");
                    }
                }
                else
                {
                    Debug.Log("HoldingPatternProcessor: Found HoldingPatternEntry with general search");
                }
            }
            else
            {
                Debug.Log("HoldingPatternProcessor: Found HoldingPatternEntry in scene");
            }
        }
        
        // Find notification text
        if (notificationText == null && holdingPatternPanel != null)
        {
            TMP_Text[] texts = holdingPatternPanel.GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text text in texts)
            {
                if (text.gameObject.name.Contains("Notification") || text.gameObject.name.Contains("notification"))
                {
                    notificationText = text;
                    Debug.Log("HoldingPatternProcessor: Found notification text: " + text.gameObject.name);
                    break;
                }
            }
            
            if (notificationText == null)
            {
                GameObject notificationObj = GameObject.Find("NotificationText");
                if (notificationObj != null)
                {
                    notificationText = notificationObj.GetComponent<TMP_Text>();
                    if (notificationText != null)
                    {
                        Debug.Log("HoldingPatternProcessor: Found NotificationText with general search");
                    }
                }
            }
        }
        
        // Log results
        if (holdingPatternPanel == null)
            Debug.LogError("HoldingPatternProcessor: holdingPatternPanel reference is missing!");
            
        if (holdingPatternEntriesContainer == null)
            Debug.LogError("HoldingPatternProcessor: holdingPatternEntriesContainer reference is missing!");
            
        if (holdingPatternEntryPrefab == null)
            Debug.LogError("HoldingPatternProcessor: holdingPatternEntryPrefab reference is missing!");
    }
    
    /// <summary>
    /// Add a ship to the holding pattern
    /// </summary>
    public bool AddShipToHoldingPattern(MasterShipEncounter encounter)
    {
        // Check if we're at capacity
        if (activeEntries.Count >= maxShipsInHoldingPattern)
        {
            // Show notification that holding pattern is full
            ShowNotification("Holding pattern at maximum capacity!");
            return false;
        }
        
        // Try to find references again if missing
        if (holdingPatternEntryPrefab == null || holdingPatternEntriesContainer == null || holdingPatternPanel == null)
        {
            Debug.LogWarning("HoldingPatternProcessor: Missing references, attempting to find them again...");
            FindUIReferences();
        }
        
        // Make sure we have the prefab and container
        if (holdingPatternEntryPrefab == null || holdingPatternEntriesContainer == null)
        {
            Debug.LogError("HoldingPatternProcessor: Missing prefab or container references!");
            return false;
        }
        
        // Debug log for tracking ship info
        Debug.Log($"Adding ship to holding pattern: {encounter.shipType} - {encounter.captainName}");
        
        // Create a new entry object
        GameObject entryObject = Instantiate(holdingPatternEntryPrefab, holdingPatternEntriesContainer);
        HoldingPatternEntry entry = entryObject.GetComponent<HoldingPatternEntry>();
        
        if (entry == null)
        {
            Debug.LogError("HoldingPatternProcessor: Prefab missing HoldingPatternEntry component!");
            Destroy(entryObject);
            return false;
        }
        
        // Initialize the entry
        entry.Initialize(encounter, OnHoldingPatternComplete);
        
        // Add to tracking list
        activeEntries.Add(entry);
        
        // Show the panel if it was hidden
        if (holdingPatternPanel != null && !holdingPatternPanel.activeSelf)
        {
            holdingPatternPanel.SetActive(true);
            
            // Ensure the panel is visible in hierarchy
            Canvas[] parentCanvases = holdingPatternPanel.GetComponentsInParent<Canvas>(true);
            foreach (Canvas canvas in parentCanvases)
            {
                canvas.enabled = true;
            }
            
            // Ensure all parent objects are active
            Transform parent = holdingPatternPanel.transform.parent;
            while (parent != null)
            {
                parent.gameObject.SetActive(true);
                parent = parent.parent;
            }
            
            Debug.Log($"Holding pattern panel activated, active in hierarchy: {holdingPatternPanel.activeInHierarchy}");
        }
        
        // Inform player with notification
        ShowNotification($"Added {encounter.shipType} to holding pattern");
        
        return true;
    }
    
    /// <summary>
    /// Release a specific ship from holding pattern
    /// </summary>
    public void ReleaseShip(HoldingPatternEntry entry)
    {
        if (entry == null || !activeEntries.Contains(entry))
            return;
            
        MasterShipEncounter encounter = entry.GetEncounter();
        
        // Process story impact when releasing a story ship
        if (encounter != null && encounter.isStoryShip && !string.IsNullOrEmpty(encounter.storyTag))
        {
            if (gameManager != null)
            {
                // Get loyalty impacts
                SpecializedScenarioProvider storyProvider = FindFirstObjectByType<SpecializedScenarioProvider>();
                if (storyProvider != null)
                {
                    storyProvider.GetLoyaltyImpact(encounter.storyTag, out int imperialChange, out int rebellionChange);
                    
                    // Apply loyalty changes
                    gameManager.UpdateLoyalty(imperialChange, rebellionChange);
                    
                    // Record key decision - safely check if the method exists
                    if (gameManager.GetType().GetMethod("AddKeyDecision") != null)
                    {
                        gameManager.AddKeyDecision($"Released {GetStoryDescription(encounter)} from holding pattern");
                    }
                    
                    // Show a notification about the impact
                    if (imperialChange != 0 || rebellionChange != 0)
                    {
                        string message = GetLoyaltyChangeMessage(encounter.storyTag, imperialChange, rebellionChange);
                        ShowNotification(message);
                    }
                }
            }
        }
        
        // Remove from active entries
        activeEntries.Remove(entry);
        
        // Destroy the UI entry
        Destroy(entry.gameObject);
        
        // Process the ship's release
        if (shipGenerator != null && encounter != null)
        {
            // Put the ship back in the queue
            shipGenerator.ProcessHoldingPatternCompletion(encounter);
            
            // Show notification
            ShowNotification($"Released {encounter.shipType} from holding pattern");
        }
        
        // Hide panel if no more entries
        if (activeEntries.Count == 0 && holdingPatternPanel != null)
            holdingPatternPanel.SetActive(false);
    }
    
    /// <summary>
    /// Get a description for a story ship
    /// </summary>
    private string GetStoryDescription(MasterShipEncounter encounter)
    {
        switch (encounter.storyTag)
        {
            case "bounty_hunter":
                return "bounty hunter";
                
            case "imperium_traitor":
                return "fleeing Imperium officer";
                
            case "insurgent":
                return "insurgent sympathizer";
                
            default:
                return encounter.shipType;
        }
    }
    
    /// <summary>
    /// Get message describing loyalty change
    /// </summary>
    private string GetLoyaltyChangeMessage(string storyTag, int imperialChange, int rebellionChange)
    {
        string message = "";
        
        switch (storyTag)
        {
            case "bounty_hunter":
                if (imperialChange > 0)
                    message = "Helping a bounty hunter has gained you favor with the Imperium";
                break;
                
            case "imperium_traitor":
                if (imperialChange < 0)
                    message = "Helping an Imperium deserter is considered treasonous";
                if (rebellionChange > 0)
                    message += "\nYou've gained the trust of the resistance";
                break;
                
            case "insurgent":
                if (rebellionChange > 0)
                    message = "The insurgency will remember your assistance";
                if (imperialChange < 0)
                    message += "\nYour superiors might question your judgment";
                break;
        }
        
        return message;
    }
    
    /// <summary>
    /// Callback when holding pattern completes
    /// </summary>
    private void OnHoldingPatternComplete(HoldingPatternEntry entry)
    {
        if (entry == null)
        {
            Debug.LogWarning("HoldingPatternProcessor: OnHoldingPatternComplete called with null entry");
            return;
        }
        
        Debug.Log($"Holding pattern complete for ship: {entry.GetEncounter()?.shipType ?? "Unknown"}");
        ReleaseShip(entry);
    }
    
    /// <summary>
    /// Show a notification message
    /// </summary>
    private void ShowNotification(string message)
    {
        if (notificationText == null)
            return;
            
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        
        // Hide after delay
        StartCoroutine(HideNotificationAfterDelay(showNotificationTime));
    }
    
    /// <summary>
    /// Hide notification after delay
    /// </summary>
    private IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Get the number of ships currently in holding pattern
    /// </summary>
    public int GetHoldingPatternCount()
    {
        return activeEntries.Count;
    }
    
    /// <summary>
    /// Get the maximum number of ships allowed in holding pattern
    /// </summary>
    public int GetMaxHoldingPatternCapacity()
    {
        return maxShipsInHoldingPattern;
    }
    
    /// <summary>
    /// Clear all ships from holding pattern - used when starting a new day
    /// </summary>
    public void ClearAllHoldingPatterns()
    {
        // Process all ships currently in holding pattern
        foreach (var entry in activeEntries)
        {
            if (entry != null)
            {
                MasterShipEncounter encounter = entry.GetEncounter();
                
                // Put the ship back in the queue
                if (shipGenerator != null && encounter != null)
                {
                    shipGenerator.ProcessHoldingPatternCompletion(encounter);
                }
                
                // Destroy the UI entry
                Destroy(entry.gameObject);
            }
        }
        
        // Clear the list
        activeEntries.Clear();
        
        // Hide the panel
        if (holdingPatternPanel != null)
            holdingPatternPanel.SetActive(false);
    }
}