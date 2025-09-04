using UnityEngine;

/// <summary>
/// Helper script that connects ShipVideoSystem to ShipImageSystem
/// This helps fix the "ShipImageSystem component not found" warning
/// by providing a bridge between systems.
/// </summary>
public class ShipImageSystemConnector : MonoBehaviour
{
    [Header("System References")]
    public ShipVideoSystem videoSystem;
    public ShipImageSystem imageSystem;
    
    [Header("Default Images")]
    public Sprite defaultShipImage;
    public Sprite defaultCaptainImage;
    
    void Start()
    {
        FindComponents();
        ConnectSystems();
    }
    
    public void FindComponents()
    {
        // Find VideoSystem if not assigned
        if (videoSystem == null)
        {
            videoSystem = GetComponent<ShipVideoSystem>();
            
            if (videoSystem == null)
            {
                videoSystem = FindFirstObjectByType<ShipVideoSystem>();
            }
        }
        
        // Find ImageSystem if not assigned
        if (imageSystem == null)
        {
            imageSystem = GetComponent<ShipImageSystem>();
            
            if (imageSystem == null)
            {
                imageSystem = FindFirstObjectByType<ShipImageSystem>();
            }
        }
    }
    
    public void ConnectSystems()
    {
        // Check if we have the VideoSystem but not the ImageSystem
        if (videoSystem != null && imageSystem == null)
        {
            Debug.Log("ShipImageSystemConnector: Creating ImageSystem to connect with VideoSystem");
            
            // First check if there's an existing GameObject with ShipImageSystem component
            GameObject existingObject = GameObject.Find("ShipImageSystem");
            
            if (existingObject != null)
            {
                // Check if it has the ShipImageSystem component
                imageSystem = existingObject.GetComponent<ShipImageSystem>();
                
                if (imageSystem == null)
                {
                    // Add the component to existing object
                    imageSystem = existingObject.AddComponent<ShipImageSystem>();
                }
            }
            else
            {
                // Create a new GameObject for the ShipImageSystem
                GameObject imageSystemObject = new GameObject("ShipImageSystem");
                imageSystem = imageSystemObject.AddComponent<ShipImageSystem>();
                
                // Set it as a child of this object for organization
                imageSystemObject.transform.SetParent(transform);
            }
            
            // Set default images
            if (defaultShipImage != null)
            {
                // Add basic data to the image system
                ShipImageSystem.ShipImageData imageData = new ShipImageSystem.ShipImageData();
                imageData.shipTypeName = "Default";
                imageData.shipImage = defaultShipImage;
                
                // Create a new array with this single element
                ShipImageSystem.ShipImageData[] newArray = new ShipImageSystem.ShipImageData[1];
                newArray[0] = imageData;
                
                // Assign to the image system
                imageSystem.shipImages = newArray;
            }
            
            if (defaultCaptainImage != null)
            {
                // Add basic captain data to the image system
                ShipImageSystem.CaptainData captainData = new ShipImageSystem.CaptainData();
                captainData.faction = "imperial";
                captainData.portraits = new Sprite[] { defaultCaptainImage };
                
                // Create a new array with this single element
                ShipImageSystem.CaptainData[] newArray = new ShipImageSystem.CaptainData[1];
                newArray[0] = captainData;
                
                // Assign to the image system
                imageSystem.captainData = newArray;
                
                // Also set default portraits
                imageSystem.defaultPortraits = new Sprite[] { defaultCaptainImage };
            }
            
            Debug.Log("ShipImageSystemConnector: Created and configured ShipImageSystem");
        }
    }
}