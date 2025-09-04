using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

// This script sets up the video players for ship and captain videos
public class SetupVideoPlayers : MonoBehaviour
{
    [Header("Video Container References")]
    public GameObject shipVideoContainer;
    public GameObject captainVideoContainer;
    
    [Header("Video Image References")]
    public RawImage shipVideoImage;
    public RawImage captainVideoImage;
    
    [Header("Render Textures")]
    public RenderTexture shipRenderTexture;
    public RenderTexture captainRenderTexture;
    
    void Start()
    {
        // Setup ship video player
        if (shipVideoContainer != null)
        {
            VideoPlayer shipPlayer = shipVideoContainer.GetComponent<VideoPlayer>();
            if (shipPlayer == null)
            {
                shipPlayer = shipVideoContainer.AddComponent<VideoPlayer>();
            }
            
            // Configure ship video player
            shipPlayer.playOnAwake = false;
            shipPlayer.isLooping = false;
            shipPlayer.renderMode = VideoRenderMode.RenderTexture;
            shipPlayer.targetTexture = shipRenderTexture;
            
            // Set render texture to the RawImage
            if (shipVideoImage != null && shipRenderTexture != null)
            {
                shipVideoImage.texture = shipRenderTexture;
            }
        }
        
        // Setup captain video player
        if (captainVideoContainer != null)
        {
            VideoPlayer captainPlayer = captainVideoContainer.GetComponent<VideoPlayer>();
            if (captainPlayer == null)
            {
                captainPlayer = captainVideoContainer.AddComponent<VideoPlayer>();
            }
            
            // Configure captain video player
            captainPlayer.playOnAwake = false;
            captainPlayer.isLooping = false;
            captainPlayer.renderMode = VideoRenderMode.RenderTexture;
            captainPlayer.targetTexture = captainRenderTexture;
            
            // Set render texture to the RawImage
            if (captainVideoImage != null && captainRenderTexture != null)
            {
                captainVideoImage.texture = captainRenderTexture;
            }
        }
        
        Debug.Log("Video players setup completed");
    }
}