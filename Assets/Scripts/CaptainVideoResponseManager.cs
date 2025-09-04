using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using StarkillerBaseCommand;

/// <summary>
/// Manages captain video responses based on player decisions
/// Handles transitions between greeting videos and response videos for:
/// - Approve
/// - Deny
/// - Holding Pattern
/// - Tractor Beam
/// - Bribery (both offering and accepting/rejecting)
/// </summary>
public class CaptainVideoResponseManager : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private VideoPlayer captainVideoPlayer;
    [SerializeField] private VideoTransitionManager videoTransitionManager;
    [SerializeField] private CredentialChecker credentialChecker;
    [SerializeField] private StarkkillerMediaSystem mediaSystem;
    
    [Header("Video Response Settings")]
    [Tooltip("Return to greeting video after response plays")]
    [SerializeField] private bool returnToGreetingAfterResponse = true;
    
    [Tooltip("Delay before returning to greeting video")]
    [SerializeField] private float returnToGreetingDelay = 1.0f;
    
    [Header("Debug")]
    [SerializeField] private bool debugLogging = true;
    
    // Current media package and video clips
    private StarkkillerMediaDatabase.EncounterMediaPackage currentMediaPackage;
    private VideoClip currentGreetingVideo;
    private VideoClip currentResponseVideo;
    
    // Track the specific captain for consistent responses
    private CaptainType.Captain currentCaptain;
    
    // State tracking
    private bool isPlayingResponse = false;
    private bool isInBriberyMode = false;
    private Coroutine currentResponseCoroutine;
    
    // Video response types
    public enum ResponseType
    {
        Approve,
        Deny,
        HoldingPattern,
        TractorBeam,
        BriberyOffer,
        BriberyAccepted,
        BriberyRejected
    }
    
    void Awake()
    {
        // Validate references
        if (captainVideoPlayer == null)
        {
            GameObject captainVideoContainer = GameObject.Find("captainVideoContainer");
            if (captainVideoContainer != null)
            {
                captainVideoPlayer = captainVideoContainer.GetComponent<VideoPlayer>();
            }
            
            if (captainVideoPlayer == null)
            {
                Debug.LogError("CaptainVideoResponseManager: Captain video player not found!");
            }
        }
        
        if (videoTransitionManager == null)
        {
            videoTransitionManager = FindFirstObjectByType<VideoTransitionManager>();
        }
        
        if (credentialChecker == null)
        {
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
        }
        
        if (mediaSystem == null)
        {
            mediaSystem = FindFirstObjectByType<StarkkillerMediaSystem>();
        }
    }
    
    void OnEnable()
    {
        // Subscribe to decision events
        SubscribeToDecisionEvents();
        
        // Subscribe to video player events
        if (captainVideoPlayer != null)
        {
            captainVideoPlayer.loopPointReached += OnVideoFinished;
        }
    }
    
    void OnDisable()
    {
        // Unsubscribe from events
        UnsubscribeFromDecisionEvents();
        
        if (captainVideoPlayer != null)
        {
            captainVideoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
    
    /// <summary>
    /// Subscribe to credential checker decision events
    /// </summary>
    private void SubscribeToDecisionEvents()
    {
        if (credentialChecker == null) return;
        
        // Hook into the button click events
        if (credentialChecker.approveButton != null)
            credentialChecker.approveButton.onClick.AddListener(() => OnDecisionMade(ResponseType.Approve));
            
        if (credentialChecker.denyButton != null)
            credentialChecker.denyButton.onClick.AddListener(() => OnDecisionMade(ResponseType.Deny));
            
        if (credentialChecker.holdingPatternButton != null)
            credentialChecker.holdingPatternButton.onClick.AddListener(() => OnDecisionMade(ResponseType.HoldingPattern));
            
        if (credentialChecker.tractorBeamButton != null)
            credentialChecker.tractorBeamButton.onClick.AddListener(() => OnDecisionMade(ResponseType.TractorBeam));
            
        if (credentialChecker.acceptBribeButton != null)
            credentialChecker.acceptBribeButton.onClick.AddListener(() => OnBriberyDecision(true));
    }
    
    /// <summary>
    /// Unsubscribe from credential checker decision events
    /// </summary>
    private void UnsubscribeFromDecisionEvents()
    {
        if (credentialChecker == null) return;
        
        if (credentialChecker.approveButton != null)
            credentialChecker.approveButton.onClick.RemoveAllListeners();
            
        if (credentialChecker.denyButton != null)
            credentialChecker.denyButton.onClick.RemoveAllListeners();
            
        if (credentialChecker.holdingPatternButton != null)
            credentialChecker.holdingPatternButton.onClick.RemoveAllListeners();
            
        if (credentialChecker.tractorBeamButton != null)
            credentialChecker.tractorBeamButton.onClick.RemoveAllListeners();
            
        if (credentialChecker.acceptBribeButton != null)
            credentialChecker.acceptBribeButton.onClick.RemoveAllListeners();
    }
    
    /// <summary>
    /// Set the current encounter's media package
    /// Called when a new encounter starts
    /// </summary>
    public void SetEncounterMedia(MasterShipEncounter encounter)
    {
        if (encounter == null || mediaSystem == null || mediaSystem.mediaDatabase == null)
        {
            Debug.LogWarning("CaptainVideoResponseManager: Cannot set media - missing encounter or media system");
            return;
        }
        
        // Get the media package for this encounter
        currentMediaPackage = mediaSystem.mediaDatabase.GetEncounterMediaPackage(
            encounter.shipType,
            encounter.shipName,
            encounter.captainName,
            encounter.captainRank,
            encounter.captainFaction,
            encounter.storyTag
        );
        
        // Cache the greeting video
        if (currentMediaPackage != null && currentMediaPackage.HasCaptainGreetingVideo())
        {
            currentGreetingVideo = currentMediaPackage.CaptainGreetingVideo;
        }
        
        // Find and store the specific captain from the encounter
        currentCaptain = FindCaptainFromEncounter(encounter);
        
        // Check if this encounter involves bribery - using the correct property name
        isInBriberyMode = encounter.offersBribe;
        
        if (debugLogging)
        {
            Debug.Log($"CaptainVideoResponseManager: Set media for {encounter.captainName}, " +
                     $"Bribery mode: {isInBriberyMode}, " +
                     $"Captain found: {(currentCaptain != null ? currentCaptain.GetFullName() : "null")}");
        }
    }
    
    /// <summary>
    /// Play the initial greeting video
    /// </summary>
    public void PlayGreetingVideo()
    {
        if (currentGreetingVideo == null || captainVideoPlayer == null) return;
        
        captainVideoPlayer.clip = currentGreetingVideo;
        captainVideoPlayer.isLooping = true; // Greeting videos can loop
        captainVideoPlayer.Play();
        
        isPlayingResponse = false;
    }
    
    /// <summary>
    /// Handle when a decision button is clicked
    /// </summary>
    private void OnDecisionMade(ResponseType responseType)
    {
        if (isPlayingResponse)
        {
            if (debugLogging)
                Debug.Log("CaptainVideoResponseManager: Already playing a response, ignoring new request");
            return;
        }
        
        // Get the appropriate video clip
        VideoClip responseClip = GetResponseVideo(responseType);
        
        if (responseClip != null)
        {
            PlayResponseVideo(responseClip, responseType);
        }
        else if (debugLogging)
        {
            Debug.Log($"CaptainVideoResponseManager: No response video available for {responseType}");
        }
    }
    
    /// <summary>
    /// Handle bribery decision (accept or reject)
    /// </summary>
    private void OnBriberyDecision(bool accepted)
    {
        ResponseType responseType = accepted ? ResponseType.BriberyAccepted : ResponseType.BriberyRejected;
        OnDecisionMade(responseType);
    }
    
    /// <summary>
    /// Play a bribery offer video when captain attempts to bribe
    /// </summary>
    public void PlayBriberyOfferVideo()
    {
        if (currentMediaPackage != null && currentMediaPackage.HasCaptainBriberyVideo())
        {
            PlayResponseVideo(currentMediaPackage.CaptainBriberyVideo, ResponseType.BriberyOffer);
        }
    }
    
    /// <summary>
    /// Get the appropriate response video based on the response type
    /// </summary>
    private VideoClip GetResponseVideo(ResponseType responseType)
    {
        // If we have a specific captain, get their specific response
        if (currentCaptain != null)
        {
            return GetCaptainSpecificResponse(currentCaptain, responseType);
        }
        
        // Fallback to media package videos if no specific captain
        if (currentMediaPackage == null) return null;
        
        // Based on what's available in the current system
        switch (responseType)
        {
            case ResponseType.Approve:
                // Use greeting video as approval for now (system doesn't have approval videos)
                return currentMediaPackage.HasCaptainGreetingVideo() ? 
                       currentMediaPackage.CaptainGreetingVideo : null;
                       
            case ResponseType.Deny:
                // This exists in the current system
                return currentMediaPackage.HasCaptainDenialVideo() ? 
                       currentMediaPackage.CaptainDenialVideo : null;
                       
            case ResponseType.HoldingPattern:
                // Use greeting video for holding pattern (not in current system)
                return currentMediaPackage.HasCaptainGreetingVideo() ? 
                       currentMediaPackage.CaptainGreetingVideo : null;
                       
            case ResponseType.TractorBeam:
                // Use denial video for tractor beam capture (dramatic response)
                return currentMediaPackage.HasCaptainDenialVideo() ? 
                       currentMediaPackage.CaptainDenialVideo : null;
                       
            case ResponseType.BriberyOffer:
                // This exists in the current system
                return currentMediaPackage.HasCaptainBriberyVideo() ? 
                       currentMediaPackage.CaptainBriberyVideo : null;
                       
            case ResponseType.BriberyAccepted:
                // Use greeting video as positive response
                return currentMediaPackage.HasCaptainGreetingVideo() ? 
                       currentMediaPackage.CaptainGreetingVideo : null;
                       
            case ResponseType.BriberyRejected:
                // Use denial video as negative response
                return currentMediaPackage.HasCaptainDenialVideo() ? 
                       currentMediaPackage.CaptainDenialVideo : null;
                       
            default:
                return null;
        }
    }
    
    /// <summary>
    /// Play a response video with proper transitions
    /// </summary>
    private void PlayResponseVideo(VideoClip responseClip, ResponseType responseType)
    {
        if (responseClip == null || captainVideoPlayer == null) return;
        
        // Stop any existing response coroutine
        if (currentResponseCoroutine != null)
        {
            StopCoroutine(currentResponseCoroutine);
        }
        
        // Start the response playback
        currentResponseCoroutine = StartCoroutine(PlayResponseSequence(responseClip, responseType));
    }
    
    /// <summary>
    /// Coroutine to play response video sequence
    /// </summary>
    private IEnumerator PlayResponseSequence(VideoClip responseClip, ResponseType responseType)
    {
        isPlayingResponse = true;
        
        if (debugLogging)
            Debug.Log($"CaptainVideoResponseManager: Playing {responseType} response video");
        
        // Use transition manager if available for smooth transition
        if (videoTransitionManager != null)
        {
            videoTransitionManager.TransitionCaptainVideo(responseClip);
            
            // Wait for transition to complete
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            // Direct play without transition
            captainVideoPlayer.clip = responseClip;
            captainVideoPlayer.isLooping = false; // Response videos don't loop
            captainVideoPlayer.Play();
        }
        
        // Set video to not loop
        captainVideoPlayer.isLooping = false;
        
        // Wait for video to finish playing
        while (captainVideoPlayer.isPlaying)
        {
            yield return null;
        }
        
        // Add delay before returning to greeting
        yield return new WaitForSeconds(returnToGreetingDelay);
        
        // Return to greeting video if enabled
        if (returnToGreetingAfterResponse && currentGreetingVideo != null)
        {
            if (videoTransitionManager != null)
            {
                videoTransitionManager.TransitionCaptainVideo(currentGreetingVideo);
            }
            else
            {
                captainVideoPlayer.clip = currentGreetingVideo;
                captainVideoPlayer.isLooping = true;
                captainVideoPlayer.Play();
            }
            
            if (debugLogging)
                Debug.Log("CaptainVideoResponseManager: Returned to greeting video");
        }
        
        isPlayingResponse = false;
        currentResponseCoroutine = null;
    }
    
    /// <summary>
    /// Called when a video finishes playing
    /// </summary>
    private void OnVideoFinished(VideoPlayer vp)
    {
        if (vp == captainVideoPlayer && isPlayingResponse)
        {
            if (debugLogging)
                Debug.Log("CaptainVideoResponseManager: Response video finished");
        }
    }
    
    /// <summary>
    /// Stop any currently playing response
    /// </summary>
    public void StopCurrentResponse()
    {
        if (currentResponseCoroutine != null)
        {
            StopCoroutine(currentResponseCoroutine);
            currentResponseCoroutine = null;
        }
        
        isPlayingResponse = false;
    }
    
    /// <summary>
    /// Check if a response video is currently playing
    /// </summary>
    public bool IsPlayingResponse()
    {
        return isPlayingResponse;
    }
    
    /// <summary>
    /// Reset the manager state (useful when changing encounters)
    /// </summary>
    public void Reset()
    {
        StopCurrentResponse();
        currentMediaPackage = null;
        currentGreetingVideo = null;
        currentResponseVideo = null;
        currentCaptain = null;
        isInBriberyMode = false;
    }
    
    /// <summary>
    /// Find the specific captain from the encounter data
    /// </summary>
    private CaptainType.Captain FindCaptainFromEncounter(MasterShipEncounter encounter)
    {
        if (encounter == null) return null;
        
        // PRIORITY 1: First check if we have a direct reference to the selected captain (most reliable)
        if (encounter.selectedCaptain != null)
        {
            if (debugLogging)
                Debug.Log($"CaptainVideoResponseManager: ✅ Using stored captain reference: {encounter.selectedCaptain.GetFullName()}");
            return encounter.selectedCaptain;
        }
        
        // PRIORITY 2: Fallback to searching by name if no direct reference
        if (encounter.captainTypeData == null) 
        {
            if (debugLogging)
                Debug.LogWarning($"CaptainVideoResponseManager: No captain type data available for encounter");
            return null;
        }
        
        string targetName = encounter.captainName?.Trim() ?? "";
        
        if (debugLogging)
        {
            Debug.Log($"CaptainVideoResponseManager: 🔍 Searching for captain matching: '{targetName}'");
            Debug.Log($"CaptainVideoResponseManager: Available captains in {encounter.captainTypeData.typeName}:");
            for (int i = 0; i < encounter.captainTypeData.captains.Count; i++)
            {
                var cap = encounter.captainTypeData.captains[i];
                Debug.Log($"CaptainVideoResponseManager:     [{i}] {cap.GetFullName()} (first: '{cap.firstName}', last: '{cap.lastName}')");
            }
        }
        
        // ENHANCED MATCHING - Use same algorithm as EncounterMediaTransitionManager
        
        // Method 1: Exact full name match
        foreach (var captain in encounter.captainTypeData.captains)
        {
            string captainFullName = captain.GetFullName();
            if (captainFullName.Equals(targetName, System.StringComparison.OrdinalIgnoreCase))
            {
                if (debugLogging)
                    Debug.Log($"CaptainVideoResponseManager: ✅ EXACT MATCH found: {captainFullName}");
                return captain;
            }
        }
        
        // Method 2: First+Last name combination match
        foreach (var captain in encounter.captainTypeData.captains)
        {
            string captainFirstLast = $"{captain.firstName} {captain.lastName}".Trim();
            if (captainFirstLast.Equals(targetName, System.StringComparison.OrdinalIgnoreCase))
            {
                if (debugLogging)
                    Debug.Log($"CaptainVideoResponseManager: ✅ FIRST+LAST MATCH found: {captain.GetFullName()}");
                return captain;
            }
        }
        
        // Method 3: Check if target contains both first and last name
        foreach (var captain in encounter.captainTypeData.captains)
        {
            if (!string.IsNullOrEmpty(targetName) && 
                targetName.Contains(captain.firstName, System.StringComparison.OrdinalIgnoreCase) &&
                targetName.Contains(captain.lastName, System.StringComparison.OrdinalIgnoreCase))
            {
                if (debugLogging)
                    Debug.Log($"CaptainVideoResponseManager: ✅ CONTAINS BOTH NAMES found: {captain.GetFullName()}");
                return captain;
            }
        }
        
        // Method 4: Individual name part matching
        foreach (var captain in encounter.captainTypeData.captains)
        {
            string[] nameParts = targetName.Split(new char[] { ' ', '_', '-' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in nameParts)
            {
                if (!string.IsNullOrEmpty(part) && part.Length > 2)
                {
                    if (captain.firstName.Equals(part, System.StringComparison.OrdinalIgnoreCase) ||
                        captain.lastName.Equals(part, System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (debugLogging)
                            Debug.Log($"CaptainVideoResponseManager: ✅ NAME PART MATCH found: {captain.GetFullName()} matches part '{part}'");
                        return captain;
                    }
                }
            }
        }
        
        // Method 5: Fuzzy matching for common variations
        foreach (var captain in encounter.captainTypeData.captains)
        {
            string lowerTarget = targetName.ToLower().Replace(" ", "");
            string lowerFirst = captain.firstName.ToLower().Replace(" ", "");
            string lowerLast = captain.lastName.ToLower().Replace(" ", "");
            
            if (lowerTarget.Contains(lowerFirst) || lowerTarget.Contains(lowerLast) ||
                lowerFirst.Contains(lowerTarget) || lowerLast.Contains(lowerTarget))
            {
                if (debugLogging)
                    Debug.Log($"CaptainVideoResponseManager: ✅ FUZZY MATCH found: {captain.GetFullName()}");
                return captain;
            }
        }
        
        // No match found
        if (debugLogging)
        {
            Debug.LogError($"CaptainVideoResponseManager: ❌ NO MATCH FOUND for '{targetName}'!");
            Debug.LogError($"CaptainVideoResponseManager: This indicates encounter.selectedCaptain was not set correctly");
        }
        
        return null;
    }
    
    /// <summary>
    /// Get a response video specific to the current captain
    /// </summary>
    private VideoClip GetCaptainSpecificResponse(CaptainType.Captain captain, ResponseType responseType)
    {
        if (captain == null) return null;
        
        CaptainType.Captain.DialogEntry dialog = null;
        
        switch (responseType)
        {
            case ResponseType.Approve:
                // Use approved dialog
                if (captain.approvedDialogs != null && captain.approvedDialogs.Count > 0)
                {
                    dialog = captain.approvedDialogs[Random.Range(0, captain.approvedDialogs.Count)];
                }
                break;
                
            case ResponseType.Deny:
                // Use denied dialog
                if (captain.deniedDialogs != null && captain.deniedDialogs.Count > 0)
                {
                    dialog = captain.deniedDialogs[Random.Range(0, captain.deniedDialogs.Count)];
                }
                break;
                
            case ResponseType.HoldingPattern:
                // Use holding pattern dialog
                if (captain.holdingPatternDialogs != null && captain.holdingPatternDialogs.Count > 0)
                {
                    dialog = captain.holdingPatternDialogs[Random.Range(0, captain.holdingPatternDialogs.Count)];
                }
                break;
                
            case ResponseType.TractorBeam:
                // Use tractor beam dialog
                if (captain.tractorBeamDialogs != null && captain.tractorBeamDialogs.Count > 0)
                {
                    dialog = captain.tractorBeamDialogs[Random.Range(0, captain.tractorBeamDialogs.Count)];
                }
                break;
                
            case ResponseType.BriberyOffer:
            case ResponseType.BriberyAccepted:
            case ResponseType.BriberyRejected:
                // Use bribe dialog
                if (captain.bribeDialogs != null && captain.bribeDialogs.Count > 0)
                {
                    dialog = captain.bribeDialogs[Random.Range(0, captain.bribeDialogs.Count)];
                }
                break;
        }
        
        if (dialog != null && dialog.associatedVideo != null)
        {
            if (debugLogging)
            {
                Debug.Log($"CaptainVideoResponseManager: Found specific response for {captain.GetFullName()}, type: {responseType}");
            }
            return dialog.associatedVideo;
        }
        
        // If no specific dialog found, return null to use fallback
        return null;
    }
}