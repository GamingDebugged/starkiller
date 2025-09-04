#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class VideoSystemInitializer : MonoBehaviour
{
    void Awake()
    {
        if (Application.isEditor)
        {
            Debug.Log("Starting video system initialization...");
            SetupVideoSystem();
            
            // Destroy this component after initialization
            DestroyImmediate(this);
        }
    }
    
    void SetupVideoSystem()
    {
        // Get required objects
        GameObject shipVideoContainer = GameObject.Find("shipVideoContainer");
        GameObject captainVideoContainer = GameObject.Find("captainVideoContainer");
        GameObject shipVideoImage = GameObject.Find("ShipVideoImage");
        GameObject captainVideoImage = GameObject.Find("CaptainVideoImage");
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        CredentialChecker credentialChecker = FindFirstObjectByType<CredentialChecker>();
        GameObject shipEncounterSystem = GameObject.Find("ShipEncounterSystem");
        
        if (shipVideoContainer == null || captainVideoContainer == null)
        {
            Debug.LogError("Video containers not found in scene!");
            return;
        }
        
        // Create Render Textures
        RenderTexture shipRT = new RenderTexture(512, 288, 24);
        RenderTexture captainRT = new RenderTexture(256, 256, 24);
        
        // Create video players
        VideoPlayer shipPlayer = shipVideoContainer.GetComponent<VideoPlayer>();
        if (shipPlayer == null)
        {
            shipPlayer = shipVideoContainer.AddComponent<VideoPlayer>();
            Debug.Log("Added VideoPlayer to ship container");
        }
        
        VideoPlayer captainPlayer = captainVideoContainer.GetComponent<VideoPlayer>();
        if (captainPlayer == null)
        {
            captainPlayer = captainVideoContainer.AddComponent<VideoPlayer>();
            Debug.Log("Added VideoPlayer to captain container");
        }
        
        // Configure the video players
        shipPlayer.renderMode = VideoRenderMode.RenderTexture;
        shipPlayer.targetTexture = shipRT;
        shipPlayer.playOnAwake = false;
        shipPlayer.isLooping = true;
        
        captainPlayer.renderMode = VideoRenderMode.RenderTexture;
        captainPlayer.targetTexture = captainRT;
        captainPlayer.playOnAwake = false;
        captainPlayer.isLooping = true;
        
        // Assign render textures to RawImages
        if (shipVideoImage != null)
        {
            RawImage shipRawImage = shipVideoImage.GetComponent<RawImage>();
            if (shipRawImage != null)
            {
                shipRawImage.texture = shipRT;
                Debug.Log("Assigned render texture to ship video image");
            }
        }
        
        if (captainVideoImage != null)
        {
            RawImage captainRawImage = captainVideoImage.GetComponent<RawImage>();
            if (captainRawImage != null)
            {
                captainRawImage.texture = captainRT;
                Debug.Log("Assigned render texture to captain video image");
            }
        }
        
        // Add ShipVideoSystem to ShipEncounterSystem
        if (shipEncounterSystem != null)
        {
            ShipVideoSystem videoSystem = shipEncounterSystem.GetComponent<ShipVideoSystem>();
            if (videoSystem == null)
            {
                videoSystem = shipEncounterSystem.AddComponent<ShipVideoSystem>();
                Debug.Log("Added ShipVideoSystem to ShipEncounterSystem");
            }
            
            // Connect to GameManager
            if (gameManager != null)
            {
                gameManager.shipVideoSystem = videoSystem;
                Debug.Log("Connected ShipVideoSystem to GameManager");
            }
        }
        
        // Update CredentialChecker references
        if (credentialChecker != null)
        {
            credentialChecker.shipVideoPlayer = shipPlayer;
            credentialChecker.captainVideoPlayer = captainPlayer;
            credentialChecker.shipVideoContainer = shipVideoContainer;
            credentialChecker.captainVideoContainer = captainVideoContainer;
            Debug.Log("Updated CredentialChecker video references");
        }
        
        Debug.Log("Video system initialization complete!");
    }
}
#endif