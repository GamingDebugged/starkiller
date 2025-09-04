using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Bootstrapper that initializes all key managers and ensures persistence between scenes.
/// This class should be included in the first scene loaded and marked as DontDestroyOnLoad.
/// </summary>
public class BootstrapperManager : MonoBehaviour
{
    // Singleton pattern
    private static BootstrapperManager _instance;
    public static BootstrapperManager Instance => _instance;

    [Header("Manager Initialization")]
    [SerializeField] private bool createShipGeneratorManager = true;
    [SerializeField] private bool installMissingManagers = true;
    
    [Header("Scene Transition")]
    [SerializeField] private bool attachToSceneLoads = true;
    
    [Header("Debug Settings")]
    [SerializeField] private bool verboseLogging = true;
    
    private void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Make object persistent
        DontDestroyOnLoad(gameObject);
        LogMessage("BootstrapperManager initialized with DontDestroyOnLoad");
        
        // Attach to scene load events if needed
        if (attachToSceneLoads)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            LogMessage("Attached to sceneLoaded events");
        }
        
        // Immediately bootstrap critical managers
        BootstrapManagers();
    }
    
    private void Start()
    {
        // Perform a second bootstrapping on Start to ensure all objects have a chance to initialize
        BootstrapManagers();
    }
    
    /// <summary>
    /// Bootstrap all critical managers
    /// </summary>
    private void BootstrapManagers()
    {
        LogMessage("Bootstrapping managers...");
        
        // Check for ShipGeneratorManager
        if (createShipGeneratorManager && ShipGeneratorManager.Instance == null)
        {
            LogMessage("Creating ShipGeneratorManager...");
            CreateShipGeneratorManager();
        }
        
        // Check for other critical managers and create if missing
        if (installMissingManagers)
        {
            InstallMissingManagers();
        }
    }
    
    /// <summary>
    /// Create the ShipGeneratorManager
    /// </summary>
    private void CreateShipGeneratorManager()
    {
        // First check if it already exists
        if (ShipGeneratorManager.Instance != null)
        {
            LogMessage("ShipGeneratorManager already exists");
            return;
        }
        
        // Create a new GameObject for the manager
        GameObject managerObject = new GameObject("ShipGeneratorManager");
        ShipGeneratorManager manager = managerObject.AddComponent<ShipGeneratorManager>();
        
        // Ensure it's at the root level of the hierarchy for DontDestroyOnLoad to work
        managerObject.transform.SetParent(null);
        
        LogMessage("ShipGeneratorManager created");
    }
    
    /// <summary>
    /// Install any missing managers that are critical for gameplay
    /// </summary>
    private void InstallMissingManagers()
    {
        // Check for GameStateController
        if (GameStateController.Instance == null)
        {
            GameObject gameStateObject = new GameObject("GameStateController");
            gameStateObject.AddComponent<GameStateController>();
            gameStateObject.transform.SetParent(null);
            LogMessage("Created missing GameStateController");
        }
        
        // Check for ShipTimingController
        if (ShipTimingController.Instance == null)
        {
            GameObject timingObject = new GameObject("ShipTimingController");
            timingObject.AddComponent<ShipTimingController>();
            timingObject.transform.SetParent(null);
            LogMessage("Created missing ShipTimingController");
        }
        
        // Check other important singletons as needed
    }
    
    /// <summary>
    /// Called when a new scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LogMessage($"Scene loaded: {scene.name}, mode: {mode}");
        
        // When a new scene is loaded, ensure all managers are still functioning
        // Use a small delay to ensure all objects in the new scene have initialized
        Invoke("OnSceneFullyLoaded", 0.5f);
    }
    
    /// <summary>
    /// Called after a delay when a new scene is fully loaded
    /// </summary>
    private void OnSceneFullyLoaded()
    {
        LogMessage("Scene fully loaded - verifying managers");
        
        // Check if our managers are still valid
        bool managersValid = VerifyManagers();
        
        // If any managers are missing, bootstrap them again
        if (!managersValid)
        {
            LogMessage("Some managers are missing - bootstrapping again");
            BootstrapManagers();
        }
        
        // Force managers to sync references
        SyncManagerReferences();
    }
    
    /// <summary>
    /// Verify all critical managers still exist
    /// </summary>
    private bool VerifyManagers()
    {
        bool allValid = true;
        
        // Check ShipGeneratorManager
        if (ShipGeneratorManager.Instance == null)
        {
            LogMessage("ShipGeneratorManager is missing!");
            allValid = false;
        }
        
        // Check GameStateController
        if (GameStateController.Instance == null)
        {
            LogMessage("GameStateController is missing!");
            allValid = false;
        }
        
        // Check ShipTimingController
        if (ShipTimingController.Instance == null)
        {
            LogMessage("ShipTimingController is missing!");
            allValid = false;
        }
        
        return allValid;
    }
    
    /// <summary>
    /// Force all managers to sync their references
    /// </summary>
    private void SyncManagerReferences()
    {
        // Tell ShipGeneratorManager to sync references
        if (ShipGeneratorManager.Instance != null)
        {
            ShipGeneratorManager.Instance.ForceSyncReferences();
            LogMessage("Forced ShipGeneratorManager to sync references");
        }
        
        // Reset timing controller cooldowns
        if (ShipTimingController.Instance != null)
        {
            ShipTimingController.Instance.ResetCooldown();
            LogMessage("Reset ShipTimingController cooldowns");
        }
    }
    
    /// <summary>
    /// Log a message with optional prefix
    /// </summary>
    private void LogMessage(string message)
    {
        if (verboseLogging)
        {
            Debug.Log($"[BootstrapperManager] {message}");
        }
    }
    
    /// <summary>
    /// Public method to force a reference sync for debugging/recovery
    /// </summary>
    public void ForceReferenceSync()
    {
        LogMessage("Manual reference sync requested");
        SyncManagerReferences();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from scene load events
        if (attachToSceneLoads)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}