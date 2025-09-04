using UnityEngine;
using System.Collections.Generic;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Helper script to update the GameManager with Imperial family support
    /// This should be added to the same GameObject as GameManager
    /// </summary>
    public class GameManagerImperialUpdater : MonoBehaviour
    {
        [Header("Imperium Family System")]
        public ImperialFamilySystem imperialFamilySystem;
        public FamilyDisplayManager displayManager;
        
        [Header("Cost Settings")]
        public int robotMaintenanceCost = 15;  // Cost for robot maintenance
        
        private GameManager gameManager;
        
        void Awake()
        {
            // Get the GameManager component
            gameManager = GetComponent<GameManager>();
            
            if (gameManager == null)
            {
                Debug.LogError("GameManagerImperialUpdater: No GameManager component found!");
                return;
            }
            
            // Find systems if not assigned
            if (imperialFamilySystem == null)
                imperialFamilySystem = FindFirstObjectByType<ImperialFamilySystem>();
                
            if (displayManager == null)
                displayManager = FindFirstObjectByType<FamilyDisplayManager>();
                
            // Add robot maintenance cost to GameManager
            AddRobotMaintenanceCost();
        }
        
        void Start()
        {
            // Update family system references
            UpdateFamilySystemReferences();
        }
        
        /// <summary>
        /// Add robot maintenance cost field to GameManager
        /// </summary>
        void AddRobotMaintenanceCost()
        {
            // For C# reflection demonstration, but in a real implementation
            // you would directly modify the GameManager class to include this field
            
            // Instead, let's use a dynamic approach
            
            // Set the robot maintenance cost in family system
            if (imperialFamilySystem != null)
            {
                imperialFamilySystem.robotMaintenanceCost = robotMaintenanceCost;
            }
            
            Debug.Log("GameManagerImperialUpdater: Robot maintenance cost set to " + robotMaintenanceCost);
        }
        
        /// <summary>
        /// Update family system references in GameManager
        /// </summary>
        void UpdateFamilySystemReferences()
        {
            if (gameManager == null)
                return;
                
            // Get the familySystem field using reflection
            var field = gameManager.GetType().GetField("familySystem");
            
            if (field != null && imperialFamilySystem != null)
            {
                // Set the field value
                field.SetValue(gameManager, imperialFamilySystem);
                Debug.Log("GameManagerImperialUpdater: Updated GameManager's family system reference");
            }
        }
        
        /// <summary>
        /// Method that can be called from GameManager's end day logic
        /// to ensure the daily report includes robot maintenance
        /// </summary>
        public void AddRobotMaintenanceToDailyExpenses(Dictionary<string, int> expenses)
        {
            if (imperialFamilySystem == null)
                return;
                
            // Check if the family system has family members
            if (imperialFamilySystem.familyMembers == null || imperialFamilySystem.familyMembers.Length == 0)
                return;
                
            // Check if robot needs maintenance
            bool robotNeedsMaintenance = false;
            foreach (var member in imperialFamilySystem.familyMembers)
            {
                // Check if this is the family droid by occupation
                if (member.occupation == "Family Droid" && member.needsEquipment)
                {
                    robotNeedsMaintenance = true;
                    break;
                }
            }
            
            // Add expense if needed and not already added
            if (robotNeedsMaintenance && !expenses.ContainsKey("Droid Maintenance"))
            {
                expenses.Add("Droid Maintenance", robotMaintenanceCost);
            }
        }
        
        /// <summary>
        /// This method can be called from GameManager to update holographic displays
        /// </summary>
        public void UpdateFamilyDisplays()
        {
            if (displayManager != null)
            {
                displayManager.UpdateDisplayStatus();
            }
        }
    }
}