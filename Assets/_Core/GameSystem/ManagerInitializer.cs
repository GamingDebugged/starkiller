using UnityEngine;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Handles proper initialization and persistence of key game managers
    /// This solves DontDestroyOnLoad issues by ensuring managers are in root GameObjects
    /// </summary>
    public class ManagerInitializer : MonoBehaviour
    {
        [Header("Manager References")]
        [SerializeField] private DebugMonitor debugMonitor;
        [SerializeField] private MasterShipGenerator masterShipGenerator;
        [SerializeField] private EncounterSystemMigrationManager migrationManager;
        
        [Header("Settings")]
        [SerializeField] private bool initOnAwake = true;
        [SerializeField] private bool verbose = true;
        
        // Static instance for singleton pattern
        private static ManagerInitializer _instance;
        public static ManagerInitializer Instance => _instance;
        
        private void Awake()
        {
            // Singleton pattern implementation
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            
            // Mark the entire managers container as persistent
            DontDestroyOnLoad(gameObject);
            
            if (initOnAwake)
            {
                InitializeManagers();
            }
        }
        
        public void InitializeManagers()
        {
            if (verbose) Debug.Log("ManagerInitializer: Setting up manager hierarchy...");
            
            // Ensure this GameObject is at the root level
            if (transform.parent != null)
            {
                Debug.LogWarning("ManagerInitializer: Not at root level. Moving to root.");
                transform.SetParent(null);
            }
            
            // Process MasterShipGenerator
            EnsureManagerIsChild(masterShipGenerator, "MasterShipGenerator");
            
            // Process DebugMonitor
            EnsureManagerIsChild(debugMonitor, "DebugMonitor");
            
            // Process EncounterSystemMigrationManager
            EnsureManagerIsChild(migrationManager, "EncounterSystemMigrationManager");
            
            if (verbose) Debug.Log("ManagerInitializer: Manager hierarchy setup complete.");
        }
        
        /// <summary>
        /// Ensures a manager component is a child of this GameObject
        /// </summary>
        private void EnsureManagerIsChild(Component manager, string managerName)
        {
            if (manager == null)
            {
                Debug.LogWarning($"ManagerInitializer: {managerName} reference is null");
                return;
            }
            
            // If the manager is not already a child of this GameObject, reparent it
            if (manager.transform.parent != transform)
            {
                if (verbose) Debug.Log($"ManagerInitializer: Moving {managerName} to be a child of Managers");
                
                // Remember the original scale and rotation
                Vector3 originalScale = manager.transform.localScale;
                Quaternion originalRotation = manager.transform.localRotation;
                
                // Make it a child of this transform
                manager.transform.SetParent(transform, false);
                
                // Restore original scale and rotation
                manager.transform.localScale = originalScale;
                manager.transform.localRotation = originalRotation;
            }
        }
        
        /// <summary>
        /// Finds managers in the scene and makes them children of this GameObject
        /// </summary>
        public void FindAndOrganizeManagers()
        {
            if (verbose) Debug.Log("ManagerInitializer: Searching for managers in scene...");
            
            // Find DebugMonitor if not assigned
            if (debugMonitor == null)
            {
                debugMonitor = FindFirstObjectByType<DebugMonitor>();
                if (debugMonitor != null)
                {
                    EnsureManagerIsChild(debugMonitor, "DebugMonitor");
                }
            }
            
            // Find MasterShipGenerator if not assigned
            if (masterShipGenerator == null)
            {
                masterShipGenerator = MasterShipGenerator.Instance;
                if (masterShipGenerator == null)
                {
                    masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
                }
                
                if (masterShipGenerator != null)
                {
                    EnsureManagerIsChild(masterShipGenerator, "MasterShipGenerator");
                }
            }
            
            // Find EncounterSystemMigrationManager if not assigned
            if (migrationManager == null)
            {
                migrationManager = EncounterSystemMigrationManager.Instance;
                if (migrationManager == null)
                {
                    migrationManager = FindFirstObjectByType<EncounterSystemMigrationManager>();
                }
                
                if (migrationManager != null)
                {
                    EnsureManagerIsChild(migrationManager, "EncounterSystemMigrationManager");
                }
            }
        }
    }
}