using UnityEngine;

/// <summary>
/// Attaches to a MasterShipGenerator to ensure it is marked as DontDestroyOnLoad
/// and properly registered with the ShipGeneratorManager.
/// </summary>
public class MasterShipGeneratorDontDestroy : MonoBehaviour
{
    void Awake()
    {
        // Get the MasterShipGenerator component on this object
        MasterShipGenerator shipGenerator = GetComponent<MasterShipGenerator>();
        
        if (shipGenerator == null)
        {
            Debug.LogError("MasterShipGeneratorDontDestroy: Cannot find MasterShipGenerator on this GameObject!");
            return;
        }
        
        // Ensure this object is marked as DontDestroyOnLoad
        if (transform.root == transform)
        {
            DontDestroyOnLoad(gameObject);
            Debug.Log("MasterShipGeneratorDontDestroy: Successfully marked GameObject for DontDestroyOnLoad");
        }
        else
        {
            Debug.LogWarning("MasterShipGeneratorDontDestroy: GameObject is not a root object. Moving to root level.");
            
            // Try to make it a root object
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
    
    void Start()
    {
        // Register with the ShipGeneratorManager if available
        ShipGeneratorManager generatorManager = ShipGeneratorManager.Instance;
        if (generatorManager == null)
        {
            Debug.LogWarning("MasterShipGeneratorDontDestroy: ShipGeneratorManager not found. Creating one.");
            
            // Create the ShipGeneratorManager since it doesn't exist
            GameObject managerObject = new GameObject("ShipGeneratorManager");
            managerObject.AddComponent<ShipGeneratorManager>();
            DontDestroyOnLoad(managerObject);
            
            // Get the new instance
            generatorManager = ShipGeneratorManager.Instance;
        }
        
        // Force reference sync to ensure this generator is registered
        if (generatorManager != null)
        {
            generatorManager.ForceSyncReferences();
            Debug.Log("MasterShipGeneratorDontDestroy: Forced reference sync with ShipGeneratorManager");
        }
    }
}