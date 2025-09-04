using UnityEngine;

/// <summary>
/// A diagnostic script to check if DailyReport references are correctly set up
/// Attach this to any GameObject to check connections
/// </summary>
public class DailyReportDiagnostic : MonoBehaviour
{
    void Start()
    {
        // Check for GameManager
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in scene!");
            return;
        }
        
        // Check if dailyReportPanel is assigned
        System.Reflection.FieldInfo panelField = gameManager.GetType().GetField("dailyReportPanel", 
                                                System.Reflection.BindingFlags.Public | 
                                                System.Reflection.BindingFlags.Instance);
        
        if (panelField != null)
        {
            GameObject panel = panelField.GetValue(gameManager) as GameObject;
            if (panel == null)
            {
                Debug.LogError("GameManager.dailyReportPanel is null!");
            }
            else
            {
                Debug.Log($"GameManager.dailyReportPanel is assigned to {panel.name}");
                
                // Check if panel has DailyReportManager component
                DailyReportManager manager = panel.GetComponent<DailyReportManager>();
                if (manager == null)
                {
                    Debug.LogError($"DailyReportPanel GameObject does not have a DailyReportManager component!");
                }
                else
                {
                    Debug.Log("DailyReportManager component found on panel");
                    
                    // Check if continue button is assigned
                    if (manager.continueButton == null)
                    {
                        Debug.LogError("DailyReportManager.continueButton is not assigned!");
                    }
                    else
                    {
                        Debug.Log("DailyReportManager.continueButton is assigned");
                    }
                }
            }
        }
    }
}