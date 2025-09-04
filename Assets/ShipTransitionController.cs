using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using TMPro;

public class ShipTransitionController : MonoBehaviour
{
    [Header("Transition Videos")]
    [SerializeField] private VideoClip[] normalTransitionClips;
    [SerializeField] private VideoClip inspectionWarningClip;
    [SerializeField] private VideoClip storyMessageClip;
    [SerializeField] private VideoClip endOfDayWarningClip;
    
    [Header("UI References")]
    [SerializeField] private VideoPlayer transitionVideoPlayer;
    [SerializeField] private GameObject transitionVideoContainer;
    [SerializeField] private CanvasGroup transitionCanvasGroup;
    [SerializeField] private TMP_Text transitionMessageText;
    
    [Header("Settings")]
    [SerializeField] private float minTransitionDuration = 3f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    
    private bool isTransitioning = false;
    private static ShipTransitionController _instance;
    public static ShipTransitionController Instance => _instance;
    
    public bool IsTransitioning => isTransitioning;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        
        // Ensure container is hidden initially
        if (transitionVideoContainer)
            transitionVideoContainer.SetActive(false);
    }
    
    /// <summary>
    /// Play a normal transition between ships
    /// </summary>
    public IEnumerator PlayShipTransition(System.Action onComplete = null)
    {
        if (isTransitioning || normalTransitionClips == null || normalTransitionClips.Length == 0) 
        {
            onComplete?.Invoke();
            yield break;
        }
        
        isTransitioning = true;
        
        // Select random transition clip
        VideoClip clip = normalTransitionClips[Random.Range(0, normalTransitionClips.Length)];
        
        // Play the transition
        yield return PlayTransitionClip(clip, "Connecting to next ship...");
        
        isTransitioning = false;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// Play inspection warning transition
    /// </summary>
    public IEnumerator PlayInspectionTransition(string inspectionReason, System.Action onComplete = null)
    {
        if (isTransitioning) 
        {
            onComplete?.Invoke();
            yield break;
        }
        
        isTransitioning = true;
        
        VideoClip clip = inspectionWarningClip ?? normalTransitionClips[0];
        yield return PlayTransitionClip(clip, "INSPECTION ALERT: " + inspectionReason);
        
        isTransitioning = false;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// Play story message transition
    /// </summary>
    public IEnumerator PlayStoryTransition(string storyMessage, System.Action onComplete = null)
    {
        if (isTransitioning) 
        {
            onComplete?.Invoke();
            yield break;
        }
        
        isTransitioning = true;
        
        VideoClip clip = storyMessageClip ?? normalTransitionClips[0];
        yield return PlayTransitionClip(clip, storyMessage);
        
        isTransitioning = false;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// Core method to play a transition clip
    /// </summary>
    private IEnumerator PlayTransitionClip(VideoClip clip, string message = null)
    {
        // Setup video player
        transitionVideoPlayer.clip = clip;
        transitionVideoContainer.SetActive(true);
        
        // Set message if provided
        if (transitionMessageText != null && !string.IsNullOrEmpty(message))
        {
            transitionMessageText.text = message;
            transitionMessageText.gameObject.SetActive(true);
        }
        
        // Fade in
        yield return FadeCanvasGroup(transitionCanvasGroup, 0f, 1f, fadeInDuration);
        
        // Prepare and play video
        transitionVideoPlayer.Prepare();
        while (!transitionVideoPlayer.isPrepared)
            yield return null;
            
        transitionVideoPlayer.Play();
        
        // Wait for video or minimum duration
        float waitTime = clip != null ? 
            Mathf.Max(minTransitionDuration, (float)clip.length) : 
            minTransitionDuration;
            
        yield return new WaitForSeconds(waitTime);
        
        // Fade out
        yield return FadeCanvasGroup(transitionCanvasGroup, 1f, 0f, fadeOutDuration);
        
        // Hide container and message
        transitionVideoContainer.SetActive(false);
        if (transitionMessageText != null)
            transitionMessageText.gameObject.SetActive(false);
    }
    
    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        if (group == null) yield break;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        group.alpha = to;
    }
}