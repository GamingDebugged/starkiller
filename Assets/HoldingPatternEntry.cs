using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a single entry in the holding pattern UI
/// Manages timer and controls for a specific ship
/// </summary>
public class HoldingPatternEntry : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text shipInfoText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private Button releaseButton;
    [SerializeField] private Image shipTypeImage;
    [SerializeField] private Image storyIndicator;
    
    // Reference to encounter
    private MasterShipEncounter encounter;
    
    // Timer values
    private float remainingTime;
    private float totalTime;
    
    // Callback for completion
    private System.Action<HoldingPatternEntry> onComplete;
    
    /// <summary>
    /// Initialize with encounter data
    /// </summary>
    public void Initialize(MasterShipEncounter shipEncounter, System.Action<HoldingPatternEntry> completeCallback)
    {
        this.encounter = shipEncounter;
        this.onComplete = completeCallback;
        
        // Set times
        this.totalTime = shipEncounter.holdingPatternTime;
        this.remainingTime = totalTime;
        
        // Setup UI
        UpdateUI();
        
        // Setup release button
        if (releaseButton != null)
        {
            releaseButton.onClick.RemoveAllListeners();
            releaseButton.onClick.AddListener(() => {
                if (onComplete != null)
                    onComplete.Invoke(this);
            });
        }
        
        // Set ship image if available
        if (shipTypeImage != null && shipEncounter.HasShipImage())
        {
            shipTypeImage.sprite = shipEncounter.shipImage;
            shipTypeImage.gameObject.SetActive(true);
        }
        else if (shipTypeImage != null)
        {
            shipTypeImage.gameObject.SetActive(false);
        }
        
        // Show story indicator if this is a story ship
        if (storyIndicator != null)
        {
            bool isStoryShip = shipEncounter.isStoryShip && !string.IsNullOrEmpty(shipEncounter.storyTag);
            storyIndicator.gameObject.SetActive(isStoryShip);
            
            // Set different colors based on story type
            if (isStoryShip)
            {
                switch (shipEncounter.storyTag)
                {
                    case "bounty_hunter":
                        storyIndicator.color = new Color(0.7f, 0.5f, 0.2f); // Golden brown
                        break;
                        
                    case "imperium_traitor":
                        storyIndicator.color = new Color(0.2f, 0.0f, 0.7f); // Deep purple
                        break;
                        
                    case "insurgent":
                        storyIndicator.color = new Color(0.0f, 0.6f, 0.2f); // Green
                        break;
                        
                    default:
                        storyIndicator.color = Color.white;
                        break;
                }
            }
        }

        // Display debug info
        Debug.Log($"HoldingPatternEntry initialized: {shipEncounter.shipType}, Timer: {totalTime}s");
    }
    
    void Update()
    {
        // Update timer
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            
            // Ensure it doesn't go negative
            if (remainingTime < 0)
                remainingTime = 0;
                
            // Update UI
            UpdateTimerUI();
            
            // Check if timer expired
            if (remainingTime <= 0 && onComplete != null)
            {
                onComplete.Invoke(this);
            }
        }
    }
    
    /// <summary>
    /// Update UI with current data
    /// </summary>
    private void UpdateUI()
    {
        if (encounter == null)
            return;
            
        // Update ship info text with more detailed information
        if (shipInfoText != null)
        {
            string storyIndicator = encounter.isStoryShip ? $" [<color=#FFD700>{encounter.storyTag}</color>]" : "";
            
            // Create a more detailed ship info display
            shipInfoText.text = $"<b>{encounter.shipType}{storyIndicator}</b>\n" +
                               $"<b>Captain:</b> {encounter.captainRank} {encounter.captainName}\n" +
                               $"<b>Origin:</b> {encounter.origin}\n" +
                               $"<b>Destination:</b> {encounter.destination}\n" +
                               $"<b>Access Code:</b> {encounter.accessCode}";
        }
        
        // Update timer UI
        UpdateTimerUI();
    }
    
    /// <summary>
    /// Update timer UI elements
    /// </summary>
    private void UpdateTimerUI()
    {
        // Update timer text
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        // Update timer slider
        if (timerSlider != null && totalTime > 0)
        {
            timerSlider.value = remainingTime / totalTime;
        }
    }
    
    /// <summary>
    /// Get the underlying encounter
    /// </summary>
    public MasterShipEncounter GetEncounter()
    {
        return encounter;
    }
}