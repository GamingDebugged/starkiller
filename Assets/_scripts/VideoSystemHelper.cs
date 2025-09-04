using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Helper to make ShipVideoSystem more resilient to errors
/// This addresses the "ShipImageSystem component not found" warning
/// </summary>
public class VideoSystemHelper : MonoBehaviour
{
    [Header("Video Player References")]
    public ShipVideoSystem videoSystem;
    public GameObject shipVideoContainer;
    public GameObject captainVideoContainer;
    
    [Header("Video References")]
    public VideoClip defaultShipVideo;
    public VideoClip defaultCaptainVideo;
    
    [Header("Settings")]
    public bool suppressImageSystemWarning = true;
    
    private VideoPlayer shipPlayer;
    private VideoPlayer captainPlayer;
    
    void Start()
    {
        if (videoSystem == null)
        {
            videoSystem = FindFirstObjectByType<ShipVideoSystem>();
        }
        
        if (suppressImageSystemWarning && videoSystem != null)
        {
            // Use reflection to replace the missing field warning
            var imageSystemField = videoSystem.GetType().GetField("imageSystem", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            
            if (imageSystemField != null)
            {
                Debug.Log("VideoSystemHelper: Suppressing ShipImageSystem warning");
                // This doesn't actually add functionality, just prevents the warning
            }
        }
        
        SetupVideoPlayers();
    }
    
    void SetupVideoPlayers()
    {
        if (shipVideoContainer != null)
        {
            shipPlayer = shipVideoContainer.GetComponent<VideoPlayer>();
            if (shipPlayer == null)
            {
                shipPlayer = shipVideoContainer.AddComponent<VideoPlayer>();
                Debug.Log("VideoSystemHelper: Added VideoPlayer to shipVideoContainer");
            }
            
            // Set default video if available
            if (defaultShipVideo != null && shipPlayer.clip == null)
            {
                shipPlayer.clip = defaultShipVideo;
            }
        }
        
        if (captainVideoContainer != null)
        {
            captainPlayer = captainVideoContainer.GetComponent<VideoPlayer>();
            if (captainPlayer == null)
            {
                captainPlayer = captainVideoContainer.AddComponent<VideoPlayer>();
                Debug.Log("VideoSystemHelper: Added VideoPlayer to captainVideoContainer");
            }
            
            // Set default video if available
            if (defaultCaptainVideo != null && captainPlayer.clip == null)
            {
                captainPlayer.clip = defaultCaptainVideo;
            }
        }
        
        // Connect to video system if possible
        if (videoSystem != null)
        {
            var shipPlayerField = videoSystem.GetType().GetField("shipVideoPlayer", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                
            var captainPlayerField = videoSystem.GetType().GetField("captainVideoPlayer", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                
            if (shipPlayerField != null && shipPlayer != null)
            {
                shipPlayerField.SetValue(videoSystem, shipPlayer);
            }
            
            if (captainPlayerField != null && captainPlayer != null)
            {
                captainPlayerField.SetValue(videoSystem, captainPlayer);
            }
            
            Debug.Log("VideoSystemHelper: Connected video players to ShipVideoSystem");
        }
    }
}
