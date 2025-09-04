using UnityEngine;

/// <summary>
/// Creates and installs the ShipGeneratorManager at runtime if it doesn't already exist.
/// This ensures that the manager is available early in the game lifecycle.
/// </summary>
public class ShipGeneratorManagerInstaller : MonoBehaviour
{
    [Header("Installer Settings")]
    [SerializeField] private bool installOnAwake = true;
    [SerializeField] private bool destroyAfterInstallation = true;
    
    [Header("Debug Settings")]
    [SerializeField] private bool verboseLogging = true;

    private void Awake()
    {
        if (installOnAwake)
        {
            InstallManager();
        }
    }

    /// <summary>
    /// Manually trigger installation of the ShipGeneratorManager
    /// </summary>
    public void InstallManager()
    {
        // Check if the manager already exists
        if (ShipGeneratorManager.Instance != null)
        {
            LogMessage("ShipGeneratorManager already exists. Skipping installation.");
            
            if (destroyAfterInstallation)
            {
                LogMessage("Destroying installer as configured.");
                Destroy(this.gameObject);
            }
            
            return;
        }
        
        // Create a new GameObject for the manager
        LogMessage("Creating new ShipGeneratorManager instance");
        GameObject managerObject = new GameObject("ShipGeneratorManager");
        ShipGeneratorManager manager = managerObject.AddComponent<ShipGeneratorManager>();
        
        // Ensure it's at the root level of the hierarchy for DontDestroyOnLoad to work
        managerObject.transform.SetParent(null);
        
        LogMessage("ShipGeneratorManager installed successfully");
        
        // Destroy this installer if configured to do so
        if (destroyAfterInstallation)
        {
            LogMessage("Installation complete. Destroying installer.");
            Destroy(this.gameObject);
        }
    }
    
    /// <summary>
    /// Log a message with optional prefix
    /// </summary>
    private void LogMessage(string message)
    {
        if (verboseLogging)
        {
            Debug.Log($"[ShipGeneratorManagerInstaller] {message}");
        }
    }
}