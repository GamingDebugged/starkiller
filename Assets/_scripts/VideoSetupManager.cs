using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

// This class sets up the video system in the scene
public class VideoSetupManager : MonoBehaviour
{
    [Header("GameObject References")]
    public GameObject shipVideoContainer;
    public GameObject captainVideoContainer;
    public GameObject shipVideoImageObject;
    public GameObject captainVideoImageObject;
    
    [Header("Component References")]
    public CredentialChecker credentialChecker;
    public GameManager gameManager;
    
    void Awake()
    {
        // Create render textures
        RenderTexture shipRenderTexture = new RenderTexture(512, 288, 24);
        shipRenderTexture.name = "ShipVideoRT";
        
        RenderTexture captainRenderTexture = new RenderTexture(256, 256, 24);
        captainRenderTexture.name = "CaptainVideoRT";
        
        // Find references if not assigned
        if (shipVideoContainer == null) shipVideoContainer = GameObject.Find("shipVideoContainer");
        if (captainVideoContainer == null) captainVideoContainer = GameObject.Find("captainVideoContainer");
        if (shipVideoImageObject == null) shipVideoImageObject = GameObject.Find("ShipVideoImage");
        if (captainVideoImageObject == null) captainVideoImageObject = GameObject.Find("CaptainVideoImage");
        
        if (credentialChecker == null) credentialChecker = FindFirstObjectByType<CredentialChecker>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();
        
        // Set up ship video
        if (shipVideoContainer != null)
        {
            VideoPlayer shipPlayer = shipVideoContainer.GetComponent<VideoPlayer>();
            if (shipPlayer == null) shipPlayer = shipVideoContainer.AddComponent<VideoPlayer>();
            
            shipPlayer.renderMode = VideoRenderMode.RenderTexture;
            shipPlayer.targetTexture = shipRenderTexture;
            shipPlayer.playOnAwake = false;
            shipPlayer.isLooping = true;
            shipPlayer.audioOutputMode = VideoAudioOutputMode.None;
            
            // Set the texture on the Raw Image
            if (shipVideoImageObject != null)
            {
                RawImage rawImage = shipVideoImageObject.GetComponent<RawImage>();
                if (rawImage != null)
                {
                    rawImage.texture = shipRenderTexture;
                }
            }
            
            // Set in CredentialChecker
            if (credentialChecker != null)
            {
                credentialChecker.shipVideoPlayer = shipPlayer;
                credentialChecker.shipVideoContainer = shipVideoContainer;
            }
        }
        
        // Set up captain video
        if (captainVideoContainer != null)
        {
            VideoPlayer captainPlayer = captainVideoContainer.GetComponent<VideoPlayer>();
            if (captainPlayer == null) captainPlayer = captainVideoContainer.AddComponent<VideoPlayer>();
            
            captainPlayer.renderMode = VideoRenderMode.RenderTexture;
            captainPlayer.targetTexture = captainRenderTexture;
            captainPlayer.playOnAwake = false;
            captainPlayer.isLooping = true;
            captainPlayer.audioOutputMode = VideoAudioOutputMode.None;
            
            // Set the texture on the Raw Image
            if (captainVideoImageObject != null)
            {
                RawImage rawImage = captainVideoImageObject.GetComponent<RawImage>();
                if (rawImage != null)
                {
                    rawImage.texture = captainRenderTexture;
                }
            }
            
            // Set in CredentialChecker
            if (credentialChecker != null)
            {
                credentialChecker.captainVideoPlayer = captainPlayer;
                credentialChecker.captainVideoContainer = captainVideoContainer;
            }
        }
        
        // Add the ShipVideoSystem component to the ShipEncounterSystem game object
        GameObject shipEncounterObject = GameObject.Find("ShipEncounterSystem");
        if (shipEncounterObject != null)
        {
            ShipVideoSystem videoSystem = shipEncounterObject.GetComponent<ShipVideoSystem>();
            if (videoSystem == null) videoSystem = shipEncounterObject.AddComponent<ShipVideoSystem>();
            
            // Set reference in GameManager
            if (gameManager != null)
            {
                gameManager.shipVideoSystem = videoSystem;
            }
        }
        
        Debug.Log("Video system setup completed");
    }
}