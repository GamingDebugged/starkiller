using UnityEngine;
using Starkiller.Core.Managers;

namespace Starkiller.Core
{
    /// <summary>
    /// Helper script to ensure all required managers exist in the scene
    /// Attach this to a GameObject in your scene to automatically create missing managers
    /// </summary>
    public class ManagerSetupHelper : MonoBehaviour
    {
        [Header("Manager Creation Settings")]
        [SerializeField] private bool createMissingManagers = true;
        [SerializeField] private bool logCreatedManagers = true;
        
        private void Awake()
        {
            if (!createMissingManagers) return;
            
            // Check and create RefactoredDailyReportManager if missing
            if (FindFirstObjectByType<RefactoredDailyReportManager>() == null)
            {
                GameObject managerObj = new GameObject("RefactoredDailyReportManager");
                managerObj.AddComponent<RefactoredDailyReportManager>();
                
                if (logCreatedManagers)
                    Debug.Log("[ManagerSetupHelper] Created missing RefactoredDailyReportManager");
            }
            
            // Check and create PersonalDataLogManager if missing
            if (FindFirstObjectByType<PersonalDataLogManager>() == null)
            {
                GameObject managerObj = new GameObject("PersonalDataLogManager");
                managerObj.AddComponent<PersonalDataLogManager>();
                
                if (logCreatedManagers)
                    Debug.Log("[ManagerSetupHelper] Created missing PersonalDataLogManager");
            }
        }
    }
}