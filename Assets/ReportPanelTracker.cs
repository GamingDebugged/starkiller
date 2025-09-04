using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Enhanced script to monitor and fix the daily report panel's active state
/// Attach this to the DailyReportPanel GameObject
/// </summary>
public class ReportPanelTracker : MonoBehaviour
{
    // Track when the panel becomes active or inactive
    private bool wasActive = false;
    
    // Added to ensure panel stays visible
    private float activationTime = 0f;
    private bool needsForceActivation = false;
    private const float ACTIVATION_CHECK_DELAY = 0.5f;
    
    void Awake()
    {
        Debug.Log("ReportPanelTracker.Awake called on " + gameObject.name);
        
        // Only add TimeModifierBehavior to panels that should pause time
        // Don't add it to DailyBriefingPanel as it handles its own pausing
        bool shouldAddTimeModifier = gameObject.name.Contains("DailyReport") || 
                                    gameObject.name.Contains("LogBook") ||
                                    gameObject.name.Contains("GameOver");
        
        if (shouldAddTimeModifier && GetComponent<TimeModifierBehavior>() == null)
        {
            TimeModifierBehavior timeModifier = gameObject.AddComponent<TimeModifierBehavior>();
            timeModifier.pauseTime = true;
            timeModifier.modifyOnEnable = true;
            timeModifier.resumeOnDisable = true;
            Debug.Log("Added TimeModifierBehavior to " + gameObject.name);
        }
    }
    
    void Update()
    {
        // Check if activation state changed
        if (gameObject.activeSelf != wasActive)
        {
            if (gameObject.activeSelf)
            {
                Debug.Log($"*** DailyReportPanel activated at {Time.time} seconds ***");
                activationTime = Time.time;
                needsForceActivation = true;
                
                // Check if panel has key components
                Button continueButton = GetComponentInChildren<Button>();
                if (continueButton != null)
                {
                    Debug.Log("Continue button found on panel");
                    
                    // Re-add click listener just in case
                    AddContinueButtonListener(continueButton);
                }
                else
                {
                    Debug.LogError("Continue button NOT found on panel!");
                }
                
                // Force panel to be fully visible
                ForceActivatePanel();
            }
            else
            {
                Debug.Log($"*** DailyReportPanel deactivated at {Time.time} seconds ***");
                needsForceActivation = false;
            }
            
            wasActive = gameObject.activeSelf;
        }
        
        // Check if we need to force activation after a short delay
        // This handles cases where something else might be deactivating the panel
        if (needsForceActivation && Time.time > activationTime + ACTIVATION_CHECK_DELAY)
        {
            ForceActivatePanel();
            needsForceActivation = false;
        }
    }
    
    // Force the panel to be fully visible and interactive
    private void ForceActivatePanel()
    {
        // Make sure the panel is active
        if (!gameObject.activeSelf)
        {
            Debug.LogWarning("Panel not active, forcing activation");
            gameObject.SetActive(true);
        }
        
        // Check if panel is properly rendering
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"Panel has Canvas component: {canvas.enabled}");
            if (!canvas.enabled)
            {
                canvas.enabled = true;
                Debug.Log("Enabled Canvas component");
            }
        }
        
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            Debug.Log($"Panel has CanvasGroup: Alpha={canvasGroup.alpha}, Interactable={canvasGroup.interactable}, BlocksRaycasts={canvasGroup.blocksRaycasts}");
            
            // Ensure the panel is fully visible and interactive
            if (canvasGroup.alpha < 1f)
            {
                Debug.LogWarning("Panel CanvasGroup alpha < 1, setting to 1");
                canvasGroup.alpha = 1f;
            }
            
            if (!canvasGroup.interactable)
            {
                Debug.LogWarning("Panel CanvasGroup not interactable, enabling");
                canvasGroup.interactable = true;
            }
            
            if (!canvasGroup.blocksRaycasts)
            {
                Debug.LogWarning("Panel CanvasGroup not blocking raycasts, enabling");
                canvasGroup.blocksRaycasts = true;
            }
        }
        
        // Check visibility of all child objects with Image components
        Image[] images = GetComponentsInChildren<Image>(true);
        foreach (Image img in images)
        {
            if (img.color.a < 0.1f)
            {
                Debug.LogWarning($"Image {img.name} has very low alpha ({img.color.a}), increasing to 1.0");
                Color c = img.color;
                c.a = 1.0f;
                img.color = c;
            }
            
            if (!img.gameObject.activeSelf)
            {
                Debug.LogWarning($"Image {img.name} GameObject is inactive, activating");
                img.gameObject.SetActive(true);
            }
        }
    }
    
    // Add a click listener to the continue button if it's missing one
    private void AddContinueButtonListener(Button continueButton)
    {
        if (continueButton == null) return;
        
        // Check if the button already has a click listener
        if (continueButton.onClick.GetPersistentEventCount() == 0)
        {
            Debug.LogWarning("Continue button has no listeners, adding generic listener");
            
            // Try to get the DailyReportManager
            DailyReportManager reportManager = GetComponent<DailyReportManager>();
            if (reportManager != null)
            {
                continueButton.onClick.AddListener(reportManager.OnContinueClicked);
                Debug.Log("Added OnContinueClicked listener from DailyReportManager");
            }
            else
            {
                // Add a generic listener as fallback
                continueButton.onClick.AddListener(() => {
                    Debug.Log("Continue button clicked via fallback listener");
                    gameObject.SetActive(false);
                    
                    // Try to find the GameManager and call StartNextDay
                    GameManager gameManager = FindFirstObjectByType<GameManager>();
                    if (gameManager != null && !gameManager.isTransitioningDay)
                    {
                        gameManager.StartNextDay();
                    }
                });
                Debug.Log("Added fallback click listener to continue button");
            }
        }
        else
        {
            Debug.Log($"Continue button already has {continueButton.onClick.GetPersistentEventCount()} listeners");
        }
    }
    
    // Track when the panel is enabled/disabled
    void OnEnable()
    {
        Debug.Log("DailyReportPanel.OnEnable called");
        
        // Force the panel to be fully visible
        ForceActivatePanel();
        
        // Set a flag to force activation again after a short delay
        needsForceActivation = true;
        activationTime = Time.time;
    }
    
    void OnDisable()
    {
        Debug.Log("DailyReportPanel.OnDisable called");
        needsForceActivation = false;
    }
}