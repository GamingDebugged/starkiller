using UnityEngine;
using UnityEngine.Video;

public class VideoSetupHelper : MonoBehaviour
{
    public CredentialChecker credentialChecker;
    public GameObject shipVideoContainer;
    public GameObject captainVideoContainer;
    
    void Start()
    {
        if (credentialChecker == null)
        {
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            if (credentialChecker == null)
            {
                Debug.LogError("CredentialChecker not found!");
                return;
            }
        }
        
        if (shipVideoContainer == null)
            shipVideoContainer = GameObject.Find("shipVideoContainer");
            
        if (captainVideoContainer == null)
            captainVideoContainer = GameObject.Find("captainVideoContainer");
            
        if (shipVideoContainer != null && captainVideoContainer != null)
        {
            VideoPlayer shipPlayer = shipVideoContainer.GetComponent<VideoPlayer>();
            VideoPlayer captainPlayer = captainVideoContainer.GetComponent<VideoPlayer>();
            
            // Set references in credential checker
            credentialChecker.shipVideoPlayer = shipPlayer;
            credentialChecker.captainVideoPlayer = captainPlayer;
            credentialChecker.shipVideoContainer = shipVideoContainer;
            credentialChecker.captainVideoContainer = captainVideoContainer;
            
            Debug.Log("Video references connected!");
        }
    }
}