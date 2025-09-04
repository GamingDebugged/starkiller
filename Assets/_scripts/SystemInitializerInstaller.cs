using UnityEngine;

/// <summary>
/// Creates and installs the EncounterSystemInitializer component
/// when loaded into the scene.
/// </summary>
[DefaultExecutionOrder(-100)] // Run very early to ensure initializer is ready
public class SystemInitializerInstaller : MonoBehaviour
{
    [Tooltip("Whether to automatically add the initializer")]
    public bool addInitializer = true;
    
    [Tooltip("The name to give the initializer GameObject")]
    public string initializerName = "EncounterSystemInitializer";
    
    [Tooltip("Log debug information")]
    public bool debugLogging = true;
    
    void Awake()
    {
        if (addInitializer)
        {
            // Check if the initializer already exists
            EncounterSystemInitializer existingInitializer = FindFirstObjectByType<EncounterSystemInitializer>();
            if (existingInitializer == null)
            {
                // Create a new GameObject
                GameObject initializerObject = new GameObject(initializerName);
                
                // Add our initializer component
                initializerObject.AddComponent<EncounterSystemInitializer>();
                
                // Make it persistent
                DontDestroyOnLoad(initializerObject);
                
                if (debugLogging)
                    Debug.Log("SystemInitializerInstaller: Created and installed EncounterSystemInitializer");
            }
            else if (debugLogging)
            {
                Debug.Log("SystemInitializerInstaller: EncounterSystemInitializer already exists, skipping installation");
            }
        }
    }
}