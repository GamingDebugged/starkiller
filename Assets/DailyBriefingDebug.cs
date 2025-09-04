using UnityEngine;

public class DailyBriefingDebug : MonoBehaviour
{
    public DailyBriefingManager briefingManager;

    void Start()
    {
        // Add a delay to ensure other components initialize first
        Invoke("CheckBriefing", 1.0f);
    }

    void CheckBriefing()
    {
        if (briefingManager == null)
        {
            briefingManager = FindFirstObjectByType<DailyBriefingManager>();
            if (briefingManager == null)
            {
                Debug.LogError("Could not find DailyBriefingManager in the scene!");
                return;
            }
        }

        // First, find the briefing panel if it's not already referenced
        if (briefingManager.briefingPanel == null)
        {
            Debug.LogWarning("briefingPanel reference is null in DailyBriefingManager! Attempting to find it...");
            
            // Try to find it in the scene
            GameObject briefingPanel = GameObject.Find("DailyBriefingPanel");
            if (briefingPanel != null)
            {
                Debug.Log("Found DailyBriefingPanel in scene, assigning to briefingManager");
                // Use reflection to set the private field if needed
                var field = typeof(DailyBriefingManager).GetField("briefingPanel", 
                    System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.NonPublic);
                    
                if (field != null)
                {
                    field.SetValue(briefingManager, briefingPanel);
                }
                else
                {
                    Debug.LogError("Could not find briefingPanel field in DailyBriefingManager!");
                }
            }
            else
            {
                Debug.LogError("DailyBriefingPanel not found in the scene!");
                return;
            }
        }

        // Ensure the panel is active before showing the briefing
        if (briefingManager.briefingPanel != null && !briefingManager.briefingPanel.activeSelf)
        {
            Debug.Log("Activating briefing panel before showing briefing");
            briefingManager.briefingPanel.SetActive(true);
        }

        // Check if the briefing panel is active
        if (briefingManager.briefingPanel != null)
        {
            Debug.Log("Briefing panel active state: " + briefingManager.briefingPanel.activeSelf);
        }
        else
        {
            Debug.LogError("briefingPanel reference is still null in DailyBriefingManager!");
            return;
        }

        // Force show the briefing for testing
        Debug.Log("Forcing briefing to show for day 1");
        briefingManager.ShowDailyBriefing(1);
    }
}