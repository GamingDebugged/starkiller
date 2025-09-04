using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoPlayerHelper : MonoBehaviour
{
    [Header("Component References")]
    public GameObject shipVideoContainer;
    public GameObject captainVideoContainer;
    public CredentialChecker credentialChecker;
    
    void Start()
    {
        if (credentialChecker == null)
        {
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            if (credentialChecker == null)
            {
                Debug.LogError("VideoPlayerHelper: CredentialChecker not found!");
                return;
            }
        }
        
        // Get the video players
        VideoPlayer shipVideoPlayer = null;
        VideoPlayer captainVideoPlayer = null;
        
        if (shipVideoContainer != null)
        {
            shipVideoPlayer = shipVideoContainer.GetComponent<VideoPlayer>();
            if (shipVideoPlayer == null)
            {
                shipVideoPlayer = shipVideoContainer.AddComponent<VideoPlayer>();
                Debug.Log("Added VideoPlayer to shipVideoContainer");
            }
        }
        
        if (captainVideoContainer != null)
        {
            captainVideoPlayer = captainVideoContainer.GetComponent<VideoPlayer>();
            if (captainVideoPlayer == null)
            {
                captainVideoPlayer = captainVideoContainer.AddComponent<VideoPlayer>();
                Debug.Log("Added VideoPlayer to captainVideoContainer");
            }
        }
        
        // Set references in the credential checker
        credentialChecker.shipVideoPlayer = shipVideoPlayer;
        credentialChecker.captainVideoPlayer = captainVideoPlayer;
        
        Debug.Log("VideoPlayerHelper: Connected video players to CredentialChecker");
    }
}