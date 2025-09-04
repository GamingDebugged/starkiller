using UnityEngine;

/// <summary>
/// Ensures managers are properly initialized as persistent singletons.
/// Place this on a root GameObject containing all your manager components.
/// </summary>
public class ManagerInitializer : MonoBehaviour
{
    [Header("Initialization Settings")]
    [SerializeField] private bool makePersistent = true;
    [SerializeField] private bool verboseLogging = true;
    
    [Header("Manager References (Auto-populated)")]
    [SerializeField] private Component[] managedComponents;
    
    private void Awake()
    {
        // Ensure we're at the root level
        if (transform.parent != null)
        {
            Debug.LogError($"ManagerInitializer must be on a root GameObject! Current parent: {transform.parent.name}");
            return;
        }
        
        // Make this GameObject persistent
        if (makePersistent)
        {
            DontDestroyOnLoad(gameObject);
            if (verboseLogging)
            {
                Debug.Log($"ManagerInitializer: Made {gameObject.name} persistent across scenes");
            }
        }
        
        // Find all manager components on this GameObject and its children
        FindManagedComponents();
        
        // Log what we're managing
        if (verboseLogging)
        {
            LogManagedComponents();
        }
    }
    
    private void FindManagedComponents()
    {
        // Get all components that might be managers
        managedComponents = GetComponentsInChildren<Component>(true);
        
        // Filter to only include likely manager components
        System.Collections.Generic.List<Component> managers = new System.Collections.Generic.List<Component>();
        
        foreach (var comp in managedComponents)
        {
            if (comp == null) continue;
            
            string typeName = comp.GetType().Name;
            
            // Check if this looks like a manager class
            if (typeName.Contains("Manager") || 
                typeName.Contains("Generator") || 
                typeName.Contains("Controller") ||
                typeName.Contains("System") ||
                typeName.Contains("Coordinator"))
            {
                managers.Add(comp);
            }
        }
        
        managedComponents = managers.ToArray();
    }
    
    private void LogManagedComponents()
    {
        Debug.Log($"ManagerInitializer managing {managedComponents.Length} components:");
        foreach (var comp in managedComponents)
        {
            if (comp != null)
            {
                Debug.Log($"  - {comp.GetType().Name} on {comp.gameObject.name}");
            }
        }
    }
    
    /// <summary>
    /// Call this to ensure a specific manager is initialized
    /// </summary>
    public T EnsureManager<T>() where T : Component
    {
        T manager = GetComponentInChildren<T>();
        if (manager == null)
        {
            Debug.LogError($"ManagerInitializer: Could not find manager of type {typeof(T).Name}");
        }
        return manager;
    }
}
