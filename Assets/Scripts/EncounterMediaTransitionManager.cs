using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using StarkillerBaseCommand;

/// <summary>
/// Manages transitions between encounter media displays to ensure smooth experience
/// This handles fading between videos, images, and coordinates with the encounter system
/// </summary>
public class EncounterMediaTransitionManager : MonoBehaviour
{
    [Header("Media System References")]
    [SerializeField] private StarkkillerMediaSystem mediaSystem;
    [SerializeField] private MasterShipGenerator shipGenerator;

    [Header("UI References")]
    [SerializeField] private VideoPlayer shipVideoPlayer;
    [SerializeField] private VideoPlayer captainVideoPlayer;
    [SerializeField] private Image shipImageDisplay;
    [SerializeField] private Image captainImageDisplay;
    [SerializeField] private GameObject shipVideoContainer;
    [SerializeField] private GameObject captainVideoContainer;
    [SerializeField] private GameObject shipImageContainer;
    [SerializeField] private GameObject captainImageContainer;

    [Header("CanvasGroup References")]
    [SerializeField] private CanvasGroup shipMediaCanvasGroup;
    [SerializeField] private CanvasGroup captainMediaCanvasGroup;

    [Header("Transition Settings")]
    [Tooltip("Duration of fade transitions in seconds")]
    [SerializeField] private float transitionDuration = 0.5f;
    [Tooltip("Delay before starting the next encounter")]
    [SerializeField] private float encounterSwitchDelay = 1.5f;
    [Tooltip("Enable debug logging for transitions")]
    [SerializeField] private bool debugLogging = true;

    // Event that fires when a transition is complete
    public event Action OnTransitionComplete;

    // State tracking
    private bool isTransitioning = false;
    private MasterShipEncounter pendingEncounter = null;
    private StarkkillerMediaDatabase.EncounterMediaPackage pendingMediaPackage = null;

    // Cache of current media
    private VideoClip currentShipVideo = null;
    private VideoClip currentCaptainVideo = null;
    private Sprite currentShipImage = null;
    private Sprite currentCaptainImage = null;

    // Media transition states
    private enum MediaState
    {
        None,
        Image,
        Video
    }

    private MediaState currentShipMediaState = MediaState.None;
    private MediaState currentCaptainMediaState = MediaState.None;

    // Handling script activation
    private bool hasBeenInitialized = false;

    void Awake()
    {
        // Validate required components
        ValidateComponents();

        // Initialize canvas groups if not assigned
        InitializeCanvasGroups();

        // Set canvas group alpha values but not to zero to avoid completely invisible UI
        if (shipMediaCanvasGroup != null) shipMediaCanvasGroup.alpha = 1f;
        if (captainMediaCanvasGroup != null) captainMediaCanvasGroup.alpha = 1f;

        Debug.Log("EncounterMediaTransitionManager Awake completed with canvas groups initialized");
    }

    void Start()
    {
        // Delayed initialization to ensure other systems are ready
        StartCoroutine(DelayedInitialization());
    }

    /// <summary>
    /// Delayed initialization to ensure other systems are ready
    /// </summary>
    private IEnumerator DelayedInitialization()
    {
        yield return new WaitForSeconds(encounterSwitchDelay);

        // Check for references again in case they were created after Awake
        if (mediaSystem == null || shipGenerator == null)
        {
            ValidateComponents();
        }

        // For safety, make sure all media starts hidden
        ResetAllMediaVisibility();

        hasBeenInitialized = true;
        Debug.Log("EncounterMediaTransitionManager initialization complete");
    }

    /// <summary>
    /// Reset all media visibility to hidden 
    /// </summary>
    private void ResetAllMediaVisibility()
    {
        // Reset alpha values
        if (shipMediaCanvasGroup != null) shipMediaCanvasGroup.alpha = 0f;
        if (captainMediaCanvasGroup != null) captainMediaCanvasGroup.alpha = 0f;

        // Hide all containers
        if (shipVideoContainer != null) shipVideoContainer.SetActive(false);
        if (shipImageContainer != null) shipImageContainer.SetActive(false);
        if (captainVideoContainer != null) captainVideoContainer.SetActive(false);
        if (captainImageContainer != null) captainImageContainer.SetActive(false);

        // Stop any playing videos
        if (shipVideoPlayer != null && shipVideoPlayer.isPlaying) shipVideoPlayer.Stop();
        if (captainVideoPlayer != null && captainVideoPlayer.isPlaying) captainVideoPlayer.Stop();
    }

    private void ValidateComponents()
    {
        // Find mediaSystem if not assigned
        if (mediaSystem == null)
        {
            mediaSystem = FindFirstObjectByType<StarkkillerMediaSystem>();
            if (mediaSystem == null)
            {
                Debug.LogError("EncounterMediaTransitionManager: MediaSystem reference is missing!");
            }
            else if (debugLogging)
            {
                Debug.Log("EncounterMediaTransitionManager: Found MediaSystem reference");
            }
        }

        // Find shipGenerator if not assigned
        if (shipGenerator == null)
        {
            // Try to get from EncounterSystemManager first
            EncounterSystemManager systemManager = FindFirstObjectByType<EncounterSystemManager>();
            if (systemManager != null)
            {
                Component activeSystem = systemManager.GetActiveEncounterSystem();
                if (activeSystem is MasterShipGenerator)
                {
                    shipGenerator = (MasterShipGenerator)activeSystem;
                    if (debugLogging)
                        Debug.Log("EncounterMediaTransitionManager: Using MasterShipGenerator from EncounterSystemManager");
                }
            }

            // If still null, try to find directly
            if (shipGenerator == null)
            {
                shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
                if (shipGenerator == null)
                {
                    Debug.LogError("EncounterMediaTransitionManager: MasterShipGenerator reference is missing!");
                }
                else if (debugLogging)
                {
                    Debug.Log("EncounterMediaTransitionManager: Found MasterShipGenerator reference");
                }
            }
        }

        // Validate UI components are assigned
        if (shipVideoPlayer == null || captainVideoPlayer == null)
        {
            Debug.LogWarning("EncounterMediaTransitionManager: One or more VideoPlayer components are missing!");
        }

        if (shipImageDisplay == null || captainImageDisplay == null)
        {
            Debug.LogWarning("EncounterMediaTransitionManager: One or more Image components are missing!");
        }
    }

    private void InitializeCanvasGroups()
    {
        // If ship media canvas group is not assigned, try to add it
        if (shipMediaCanvasGroup == null && shipVideoContainer != null)
        {
            shipMediaCanvasGroup = shipVideoContainer.GetComponent<CanvasGroup>();
            if (shipMediaCanvasGroup == null)
            {
                shipMediaCanvasGroup = shipVideoContainer.AddComponent<CanvasGroup>();
                if (debugLogging)
                    Debug.Log("Added CanvasGroup to shipVideoContainer");
            }
        }

        // If captain media canvas group is not assigned, try to add it
        if (captainMediaCanvasGroup == null && captainVideoContainer != null)
        {
            captainMediaCanvasGroup = captainVideoContainer.GetComponent<CanvasGroup>();
            if (captainMediaCanvasGroup == null)
            {
                captainMediaCanvasGroup = captainVideoContainer.AddComponent<CanvasGroup>();
                if (debugLogging)
                    Debug.Log("Added CanvasGroup to captainVideoContainer");
            }
        }
    }

    /// <summary>
    /// Prepare for a transition to a new encounter
    /// </summary>
    public void PrepareNextEncounter(MasterShipEncounter nextEncounter)
    {
        if (!hasBeenInitialized)
        {
            Debug.LogWarning("EncounterMediaTransitionManager: Called PrepareNextEncounter before initialization complete");
            // Still proceed since this might be an initialization issue
        }

        if (nextEncounter == null)
        {
            Debug.LogError("EncounterMediaTransitionManager: Cannot prepare transition for null encounter!");
            return;
        }

        if (debugLogging)
        {
            Debug.Log($"Preparing next encounter media: {nextEncounter.shipType}, Captain {nextEncounter.captainName}");
        }

        // Store the pending encounter
        pendingEncounter = nextEncounter;

        // Pre-fetch and cache all media for the next encounter
        if (mediaSystem != null && mediaSystem.mediaDatabase != null)
        {
            try
            {
                pendingMediaPackage = mediaSystem.mediaDatabase.GetEncounterMediaPackage(
                    nextEncounter.shipType,
                    nextEncounter.shipName,
                    nextEncounter.captainName,
                    nextEncounter.captainRank,
                    nextEncounter.captainFaction,
                    nextEncounter.storyTag
                );

                if (debugLogging)
                {
                    Debug.Log("Media package prepared: " +
                        $"Ship video: {(pendingMediaPackage != null && pendingMediaPackage.HasShipVideo() ? "Yes" : "No")}, " +
                        $"Captain video: {(pendingMediaPackage != null && pendingMediaPackage.HasCaptainGreetingVideo() ? "Yes" : "No")}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error preparing media package: {ex.Message}");
                pendingMediaPackage = null;
            }
        }
        else
        {
            Debug.LogWarning("EncounterMediaTransitionManager: MediaSystem or database is null, cannot prepare media package");
            pendingMediaPackage = null;
        }
    }

    /// <summary>
    /// Begin transition to the next encounter with fade effects
    /// </summary>
    public void TransitionToNextEncounter()
    {
        if (!hasBeenInitialized)
        {
            Debug.LogWarning("EncounterMediaTransitionManager: Called TransitionToNextEncounter before initialization complete");
            // Force initialization
            hasBeenInitialized = true;
        }

        if (isTransitioning)
        {
            Debug.LogWarning("EncounterMediaTransitionManager: Already in transition, ignoring request");
            return;
        }

        if (pendingEncounter == null)
        {
            Debug.LogError("EncounterMediaTransitionManager: No pending encounter to transition to!");

            // Still fire the completion event so the game can continue
            if (OnTransitionComplete != null)
            {
                Debug.LogWarning("EncounterMediaTransitionManager: Firing completion event despite error");
                OnTransitionComplete();
            }
            return;
        }

        // Start the transition coroutine
        StartCoroutine(PerformTransition());
    }

    /// <summary>
    /// Coroutine to perform the transition between encounters
    /// </summary>
    private IEnumerator PerformTransition()
    {
        isTransitioning = true;

        // 1. Fade out current media
        yield return StartCoroutine(FadeOutCurrentMedia());

        // 2. Wait a short delay
        yield return new WaitForSeconds(encounterSwitchDelay);

        // 3. Switch to new media (without showing it yet)
        SwitchToNewMedia();

        // 4. Short delay to ensure media is loaded
        yield return new WaitForSeconds(encounterSwitchDelay);

        // 5. Fade in new media
        yield return StartCoroutine(FadeInNewMedia());

        // 6. Complete transition
        pendingEncounter = null;
        pendingMediaPackage = null;
        isTransitioning = false;

        // 7. Notify listeners that transition is complete
        OnTransitionComplete?.Invoke();
    }

    /// <summary>
    /// Show reaction video for captain based on decision
    /// </summary>
    public void ShowCaptainReactionVideo(bool isApproved, bool isBribery = false)
    {
        if (captainVideoPlayer == null || captainVideoContainer == null)
        {
            return;
        }

        VideoClip reactionVideo = null;

        // Select appropriate video
        if (isBribery && pendingMediaPackage != null && pendingMediaPackage.HasCaptainBriberyVideo())
        {
            reactionVideo = pendingMediaPackage.CaptainBriberyVideo;
        }
        else if (!isApproved && pendingMediaPackage != null && pendingMediaPackage.HasCaptainDenialVideo())
        {
            reactionVideo = pendingMediaPackage.CaptainDenialVideo;
        }
        else
        {
            // No appropriate reaction video
            return;
        }

        // Show the reaction video
        StartCoroutine(PlayReactionVideo(reactionVideo));
    }

    /// <summary>
    /// Play a reaction video, fading out the current video and fading in the reaction
    /// </summary>
    private IEnumerator PlayReactionVideo(VideoClip reactionClip)
    {
        if (reactionClip == null || captainVideoPlayer == null) yield break;

        // Fade out current video
        float startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration * 0.5f) // Faster transition for reactions
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / (transitionDuration * 0.5f));

            if (captainMediaCanvasGroup != null)
            {
                captainMediaCanvasGroup.alpha = Mathf.Lerp(1f, 0.3f, t); // Dim but not fully invisible
            }

            yield return null;
        }

        // Switch to reaction video
        captainVideoPlayer.clip = reactionClip;
        captainVideoPlayer.isLooping = false;
        captainVideoPlayer.Prepare();

        // Wait for preparation
        while (!captainVideoPlayer.isPrepared)
        {
            yield return null;
        }

        // Play the reaction
        captainVideoPlayer.Play();

        // Fade back in
        startTime = Time.time;
        elapsedTime = 0f;

        while (elapsedTime < transitionDuration * 0.5f)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / (transitionDuration * 0.5f));

            if (captainMediaCanvasGroup != null)
            {
                captainMediaCanvasGroup.alpha = Mathf.Lerp(0.3f, 1f, t);
            }

            yield return null;
        }

        // Ensure it's fully visible
        if (captainMediaCanvasGroup != null) captainMediaCanvasGroup.alpha = 1f;

        // Wait for the video to finish
        while (captainVideoPlayer.isPlaying)
        {
            yield return null;
        }

        // Optional: Return to normal greeting video after reaction
        if (currentCaptainVideo != null)
        {
            captainVideoPlayer.clip = currentCaptainVideo;
            captainVideoPlayer.isLooping = true;
            captainVideoPlayer.Play();
        }
    }

    /// <summary>
    /// Checks if a transition is currently in progress
    /// </summary>
    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    /// <summary>
    /// Fade out the currently displayed media
    /// </summary>
    private IEnumerator FadeOutCurrentMedia()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        // Get initial alpha values
        float shipAlpha = shipMediaCanvasGroup != null ? shipMediaCanvasGroup.alpha : 1f;
        float captainAlpha = captainMediaCanvasGroup != null ? captainMediaCanvasGroup.alpha : 1f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Fade out ship media
            if (shipMediaCanvasGroup != null)
            {
                shipMediaCanvasGroup.alpha = Mathf.Lerp(shipAlpha, 0f, t);
            }

            // Fade out captain media
            if (captainMediaCanvasGroup != null)
            {
                captainMediaCanvasGroup.alpha = Mathf.Lerp(captainAlpha, 0f, t);
            }

            yield return null;
        }

        // Ensure media is fully invisible
        if (shipMediaCanvasGroup != null) shipMediaCanvasGroup.alpha = 0f;
        if (captainMediaCanvasGroup != null) captainMediaCanvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Fade in the new media
    /// </summary>
    private IEnumerator FadeInNewMedia()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        // Start playing videos
        if (currentShipMediaState == MediaState.Video && shipVideoPlayer != null && shipVideoPlayer.isPrepared)
        {
            shipVideoPlayer.Play();
        }

        if (currentCaptainMediaState == MediaState.Video && captainVideoPlayer != null && captainVideoPlayer.isPrepared)
        {
            captainVideoPlayer.Play();
        }

        while (elapsedTime < transitionDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Fade in ship media
            if (shipMediaCanvasGroup != null)
            {
                shipMediaCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            }

            // Fade in captain media
            if (captainMediaCanvasGroup != null)
            {
                captainMediaCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            }

            yield return null;
        }

        // Ensure media is fully visible
        if (shipMediaCanvasGroup != null) shipMediaCanvasGroup.alpha = 1f;
        if (captainMediaCanvasGroup != null) captainMediaCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Switch the media to the pending encounter's media
    /// </summary>
    private void SwitchToNewMedia()
    {
        if (pendingMediaPackage == null)
        {
            Debug.LogError("EncounterMediaTransitionManager: No pending media package to switch to!");
            return;
        }

        // Update ship media
        UpdateShipMedia();

        // Update captain media
        UpdateCaptainMedia();

        if (debugLogging)
        {
            Debug.Log("Media switched to new encounter");
        }
    }

    /// <summary>
    /// Update the ship media display with the pending media package
    /// </summary>
    private void UpdateShipMedia()
    {
        // Determine if we should use video or image
        bool useShipVideo = pendingMediaPackage.HasShipVideo() && shipVideoPlayer != null;

        if (useShipVideo)
        {
            // Use ship video
            currentShipMediaState = MediaState.Video;
            currentShipVideo = pendingMediaPackage.ShipVideo;

            // Configure video player
            shipVideoPlayer.clip = currentShipVideo;
            shipVideoPlayer.isLooping = true;
            shipVideoPlayer.Prepare();

            // Show video container, hide image container
            if (shipVideoContainer != null) shipVideoContainer.SetActive(true);
            if (shipImageContainer != null) shipImageContainer.SetActive(false);
        }
        else if (pendingMediaPackage.ShipImage != null && shipImageDisplay != null)
        {
            // Use ship image
            currentShipMediaState = MediaState.Image;
            currentShipImage = pendingMediaPackage.ShipImage;

            // Configure image display
            shipImageDisplay.sprite = currentShipImage;

            // Show image container, hide video container
            if (shipImageContainer != null) shipImageContainer.SetActive(true);
            if (shipVideoContainer != null) shipVideoContainer.SetActive(false);
        }
        else
        {
            // No valid media
            currentShipMediaState = MediaState.None;

            // Hide both containers
            if (shipVideoContainer != null) shipVideoContainer.SetActive(false);
            if (shipImageContainer != null) shipImageContainer.SetActive(false);
        }
    }

    /// <summary>
    /// Update the captain media display with the pending media package
    /// </summary>
    private void UpdateCaptainMedia()
    {
        // Determine if we should use video or image
        bool useCaptainVideo = pendingMediaPackage.HasCaptainGreetingVideo() && captainVideoPlayer != null;

        if (useCaptainVideo)
        {
            // Use captain video
            currentCaptainMediaState = MediaState.Video;
            currentCaptainVideo = pendingMediaPackage.CaptainGreetingVideo;

            // Configure video player
            captainVideoPlayer.clip = currentCaptainVideo;
            captainVideoPlayer.isLooping = true;
            captainVideoPlayer.Prepare();

            // Show video container, hide image container
            if (captainVideoContainer != null) captainVideoContainer.SetActive(true);
            if (captainImageContainer != null) captainImageContainer.SetActive(false);
        }
        else if (pendingMediaPackage.CaptainImage != null && captainImageDisplay != null)
        {
            // Use captain image
            currentCaptainMediaState = MediaState.Image;
            currentCaptainImage = pendingMediaPackage.CaptainImage;

            // Configure image display
            captainImageDisplay.sprite = currentCaptainImage;

            // Show image container, hide video container
            if (captainImageContainer != null) captainImageContainer.SetActive(true);
            if (captainVideoContainer != null) captainVideoContainer.SetActive(false);
        }
        else
        {
            // No valid media
            currentCaptainMediaState = MediaState.None;

            // Hide both containers
            if (captainVideoContainer != null) captainVideoContainer.SetActive(false);
            if (captainImageContainer != null) captainImageContainer.SetActive(false);
        }
    }
}