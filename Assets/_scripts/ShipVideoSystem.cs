using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// Ship video data class definition
[System.Serializable]
public class ShipVideoData
{
    public string shipTypeName;
    public VideoClip shipVideo;
}

// Captain video data class definition
[System.Serializable]
public class CaptainVideoData
{
    public string faction;
    public VideoClip[] captainVideos;
}

public class ShipVideoSystem : MonoBehaviour
{
    [Header("Video Database")]
    public VideoDatabase videoDatabase;
    
    [Header("Ship Videos")]
    public ShipVideoData[] shipVideos = new ShipVideoData[0];
    
    [Header("Captain Videos")]
    public CaptainVideoData[] captainVideos = new CaptainVideoData[0];
    public VideoClip[] defaultCaptainVideos;
    
    [Header("Fallback Images")]
    public Sprite defaultShipImage;
    public Sprite defaultCaptainImage;
    
    [Header("Component References")]
    [SerializeField] private ShipImageSystem imageSystem;
    private ShipImageSystemConnector connector;
    private bool triedFindingImageSystem = false;
    
    void Awake()
    {
        TryGetImageSystem();
    }
    
    // Try to get the image system component
    private void TryGetImageSystem()
    {
        if (!triedFindingImageSystem)
        {
            // Step 1: Check if already assigned in Inspector
            if (imageSystem != null)
            {
                Debug.Log("ShipVideoSystem: Using pre-assigned ShipImageSystem");
                triedFindingImageSystem = true;
                return;
            }
            
            // Step 2: Try to get it from this GameObject
            imageSystem = GetComponent<ShipImageSystem>();
            
            // Step 3: If not found, try to find it in the scene
            if (imageSystem == null)
            {
                imageSystem = FindFirstObjectByType<ShipImageSystem>();
            }
            
            // Step 4: If still not found, look for a connector and create one if needed
            if (imageSystem == null)
            {
                // Look for an existing connector
                connector = FindFirstObjectByType<ShipImageSystemConnector>();
                
                // If connector exists, use it
                if (connector != null)
                {
                    Debug.Log("ShipVideoSystem: Found ShipImageSystemConnector, using it to connect systems");
                    connector.videoSystem = this;
                    connector.FindComponents();
                    connector.ConnectSystems();
                    imageSystem = connector.imageSystem;
                }
                else
                {
                    // Create a connector if one doesn't exist
                    GameObject connectorObj = new GameObject("ShipSystemConnector");
                    connector = connectorObj.AddComponent<ShipImageSystemConnector>();
                    connector.videoSystem = this;
                    
                    // Set default images from this system
                    connector.defaultShipImage = defaultShipImage;
                    connector.defaultCaptainImage = defaultCaptainImage;
                    
                    // Initialize the connector
                    connector.FindComponents();
                    connector.ConnectSystems();
                    imageSystem = connector.imageSystem;
                    
                    Debug.Log("ShipVideoSystem: Created ShipImageSystemConnector to establish proper connections");
                }
            }
            
            // Log status
            if (imageSystem != null)
            {
                Debug.Log("ShipVideoSystem: Successfully connected to ShipImageSystem");
            }
            else
            {
                Debug.LogWarning("ShipVideoSystem: ShipImageSystem component not found! Will use fallback images for ships without videos.");
            }
            
            triedFindingImageSystem = true;
        }
    }
    
    // Video Preloading
    public void Start()
    {
        // Preload common videos
        if (videoDatabase != null && videoDatabase.defaultShipVideo != null)
        {
            VideoPlayer preloader = gameObject.AddComponent<VideoPlayer>();
            preloader.clip = videoDatabase.defaultShipVideo;
            preloader.Prepare();
            preloader.enabled = false;
        }
    }

    // Get ship video by ship type
    public VideoClip GetShipVideoForType(string shipType)
    {
        // Use VideoDatabase if available
        if (videoDatabase != null)
        {
            return videoDatabase.GetShipVideo(shipType);
        }
        
        // Fall back to the old system
        foreach (var data in shipVideos)
        {
            if (data.shipTypeName == shipType)
            {
                return data.shipVideo;
            }
        }
        return null;
    }
    
    // Get captain video by faction
    public VideoClip GetCaptainVideo(string faction, string captainName = "", string captainRank = "")
    {
        // Use VideoDatabase if available
        if (videoDatabase != null)
        {
            return videoDatabase.GetCaptainVideo(faction, captainName, captainRank);
        }
        
        // Default to imperium
        if (string.IsNullOrEmpty(faction))
        {
            faction = "imperium";
        }
        
        // Fall back to the old system
        foreach (var data in captainVideos)
        {
            if (data.faction.ToLower() == faction.ToLower() && 
                data.captainVideos != null && 
                data.captainVideos.Length > 0)
            {
                return data.captainVideos[Random.Range(0, data.captainVideos.Length)];
            }
        }
        
        // Return a default video
        if (defaultCaptainVideos != null && defaultCaptainVideos.Length > 0)
        {
            return defaultCaptainVideos[Random.Range(0, defaultCaptainVideos.Length)];
        }
        
        return null;
    }
    
    // Try to get a ship image using the ShipImageSystem or fallback to default
    private Sprite GetShipImageFallback(string shipType)
    {
        // Try again to get the image system if we haven't found it yet
        if (imageSystem == null && !triedFindingImageSystem)
        {
            TryGetImageSystem();
        }
        
        // Use image system if available
        if (imageSystem != null)
        {
            return imageSystem.GetShipImageForType(shipType);
        }
        
        // Use default image
        return defaultShipImage;
    }
    
    // Try to get a captain image using the ShipImageSystem or fallback to default
    private Sprite GetCaptainImageFallback(string faction)
    {
        // Try again to get the image system if we haven't found it yet
        if (imageSystem == null && !triedFindingImageSystem)
        {
            TryGetImageSystem();
        }
        
        // Use image system if available
        if (imageSystem != null)
        {
            return imageSystem.GetCaptainPortrait(faction);
        }
        
        // Use default image
        return defaultCaptainImage;
    }
    
    // Enhance an encounter with videos
    public VideoEnhancedShipEncounter EnhanceEncounterWithVideo(ShipEncounter baseEncounter)
    {
        // First use the image system for images if available
        EnhancedShipEncounter enhancedWithImages = null;
        
        if (imageSystem != null)
        {
            enhancedWithImages = imageSystem.EnhanceEncounter(baseEncounter);
        }
        else
        {
            // Create a basic enhanced encounter with fallback images
            enhancedWithImages = EnhancedShipEncounter.FromShipEncounter(baseEncounter);
            
            // Add fallback images
            enhancedWithImages.shipImage = GetShipImageFallback(baseEncounter.shipType);
            
            string faction = DetermineFaction(baseEncounter);
            enhancedWithImages.captainPortrait = GetCaptainImageFallback(faction);
        }
        
        // Now add videos
        VideoEnhancedShipEncounter videoEnhanced = VideoEnhancedShipEncounter.FromEnhancedEncounter(enhancedWithImages);
        
        // Determine faction
        string videofaction = DetermineFaction(baseEncounter);
        
        // Get ship video using the updated method with more parameters
        videoEnhanced.shipVideo = GetShipVideoForType(videoEnhanced.shipType);
        
        // Get captain video with more parameters
        videoEnhanced.captainVideo = GetCaptainVideo(
            videofaction,
            baseEncounter.captainName,
            baseEncounter.captainRank);
        
        return videoEnhanced;
    }
    
    // Helper to determine faction from encounter data
    private string DetermineFaction(ShipEncounter encounter)
    {
        // Default to imperial
        string faction = "imperium";
        
        // Check story tag first
        if (!string.IsNullOrEmpty(encounter.storyTag))
        {
            faction = encounter.storyTag;
        }
        // Check faction directly
        else if (!string.IsNullOrEmpty(encounter.captainFaction))
        {
            faction = encounter.captainFaction;
        }
        // Then check for keywords in ship type or origin
        else if (encounter.shipType.ToLower().Contains("insurgent") || 
                 encounter.origin.ToLower().Contains("insurgent"))
        {
            faction = "insurgent";
        }
        
        return faction;
    }
}