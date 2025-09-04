using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Helper script to update the GameManager with Imperial family support
/// This should be added to the same GameObject as GameManager
/// Specific to Starkiller Base Command project
/// </summary>
public class StarkillerUpdater : MonoBehaviour
{
    [Header("Imperial Family System")]
    public ImperialFamilySystem imperialFamilySystem;
    public FamilyDisplayManager displayManager;
    
    [Header("Cost Settings")]
    public int robotMaintenanceCost = 15;  // Cost for robot maintenance
    
    private GameManager gameManager;
    
    void OnEnable()
    {
        // Get the GameManager component
        gameManager = GetComponent<GameManager>();
        
        if (gameManager == null)
        {
            Debug.LogError("StarkillerUpdater: No GameManager component found!");
            return;
        }
        
        // Find systems if not assigned
        if (imperialFamilySystem == null)
            imperialFamilySystem = FindFirstObjectByType<ImperialFamilySystem>();
            
        if (displayManager == null)
            displayManager = FindFirstObjectByType<FamilyDisplayManager>();
            
        // Add robot maintenance cost to system
        AddDroidMaintenanceCost();
    }
    
    void Start()
    {
        // Update family system references
        UpdateSystemReferences();
    }
    
    /// <summary>
    /// Add robot maintenance cost field to GameManager
    /// </summary>
    void AddDroidMaintenanceCost()
    {
        // Direct approach - set the cost in the family system
        if (imperialFamilySystem != null)
        {
            imperialFamilySystem.robotMaintenanceCost = robotMaintenanceCost;
        }
        
        Debug.Log("StarkillerUpdater: Robot maintenance cost set to " + robotMaintenanceCost);
    }
    
    /// <summary>
    /// Update family system references in GameManager
    /// </summary>
    void UpdateSystemReferences()
    {
        if (gameManager == null)
            return;
            
        // Get the familySystem field using reflection
        var field = gameManager.GetType().GetField("familySystem");
        
        if (field != null && imperialFamilySystem != null)
        {
            // Set the field value
            field.SetValue(gameManager, imperialFamilySystem);
            Debug.Log("StarkillerUpdater: Updated GameManager's family system reference");
        }
    }
    
    /// <summary>
    /// Method that can be called from GameManager's end day logic
    /// to ensure the daily report includes droid maintenance
    /// </summary>
    public void AddDroidMaintenanceExpense(Dictionary<string, int> expenses)
    {
        if (imperialFamilySystem == null)
            return;
            
        // Check if the family system has family members
        if (imperialFamilySystem.familyMembers == null || imperialFamilySystem.familyMembers.Length == 0)
            return;
            
        // Check if droid needs maintenance
        bool droidNeedsMaintenance = false;
        foreach (var member in imperialFamilySystem.familyMembers)
        {
            // Check if this is the family droid by occupation
            if (member.occupation == "Family Droid" && member.needsEquipment)
            {
                droidNeedsMaintenance = true;
                break;
            }
        }
        
        // Add expense if needed and not already added
        if (droidNeedsMaintenance && !expenses.ContainsKey("Droid Maintenance"))
        {
            expenses.Add("Droid Maintenance", robotMaintenanceCost);
        }
    }
    
    /// <summary>
    /// This method can be called from GameManager to update holographic displays
    /// </summary>
    public void RefreshDisplays()
    {
        if (displayManager != null)
        {
            displayManager.UpdateDisplayStatus();
        }
    }
}
