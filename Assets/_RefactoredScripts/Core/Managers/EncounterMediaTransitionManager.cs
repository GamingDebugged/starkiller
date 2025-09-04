using UnityEngine;
using UnityEngine.Video;
using TMPro;
using System;
using System.Collections;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages video transitions and captain reactions during encounters
    /// Plays appropriate captain videos based on player decisions
    /// </summary>
    public class EncounterMediaTransitionManager : MonoBehaviour
    {
        [Header("Video Player References")]
        [SerializeField] private VideoPlayer captainVideoPlayer;
        [SerializeField] private VideoPlayer shipVideoPlayer;
        [SerializeField] private GameObject videoLoadingOverlay;
        
        [Header("Captain Dialog References")]
        [SerializeField] private TMP_Text captainDialogText;
        [SerializeField] private GameObject captainDialogPanel;
        [SerializeField] private CanvasGroup captainDialogCanvasGroup;
        
        [Header("Transition Settings")]
        [SerializeField] private float reactionVideoDelay = 0.5f; // Delay before showing reaction
        [SerializeField] private float dialogDisplayDuration = 3f; // How long to show dialog text
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;
        [SerializeField] private bool showDialogText = true;
        [SerializeField] private bool pauseGameDuringReaction = false;
        [SerializeField] private bool autoAdvanceAfterReaction = true; // Auto-move to next encounter
        [SerializeField] private float autoAdvanceDelay = 1.0f; // Delay before auto-advancing
        
        [Header("Audio Settings")]
        [SerializeField] private AudioSource captainAudioSource;
        [SerializeField] private float defaultVolume = 0.8f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableReactionLogging = true;
        
        // Current encounter state
        private ShipEncounter _currentEncounter;
        private CaptainType.Captain _currentCaptain;
        private CaptainType _currentCaptainType;
        private bool _isPlayingReaction = false;
        private Coroutine _reactionCoroutine;
        private bool _isTriggeringNextEncounter = false; // Prevent rapid successive calls
        
        // Events
        public static event Action<CaptainType.Captain.DialogEntry> OnCaptainReactionStarted;
        public static event Action<CaptainType.Captain.DialogEntry> OnCaptainReactionCompleted;
        public static event Action<string> OnDialogTextDisplayed;
        
        // Text synchronization events
        public static event Action OnReactionVideoStarted;    // Fired when reaction video starts playing
        public static event Action OnReactionVideoCompleted;  // Fired when reaction video completes
        public static event Action OnNewEncounterReadyForTextUpdate;  // Fired when safe to update text
        
        // Singleton pattern for easy access
        private static EncounterMediaTransitionManager _instance;
        public static EncounterMediaTransitionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<EncounterMediaTransitionManager>();
                }
                return _instance;
            }
        }
        
        // Public properties
        public bool IsPlayingReaction => _isPlayingReaction;
        public ShipEncounter CurrentEncounter => _currentEncounter;
        
        private void Awake()
        {
            _instance = this;
            
            // Register with ServiceLocator
            ServiceLocator.Register<EncounterMediaTransitionManager>(this);
            
            // Setup audio
            if (captainAudioSource == null && captainVideoPlayer != null)
            {
                captainAudioSource = captainVideoPlayer.GetComponent<AudioSource>();
            }
            
            if (enableDebugLogs)
                Debug.Log("[EncounterMediaTransitionManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Initially hide dialog panel
            if (captainDialogPanel != null)
                captainDialogPanel.SetActive(false);
                
            // Setup video player events
            if (captainVideoPlayer != null)
            {
                captainVideoPlayer.loopPointReached += OnVideoCompleted;
            }
        }
        
        /// <summary>
        /// Prepare for the next encounter with captain data
        /// </summary>
        public void PrepareNextEncounter(ShipEncounter encounter)
        {
            _currentEncounter = encounter;
            
            // Try to get captain data from encounter
            if (encounter != null)
            {
                // This would depend on your encounter structure
                // You might need to get the captain from the encounter's captain type
                _currentCaptainType = GetCaptainTypeFromEncounter(encounter);
                
                // Find the SPECIFIC captain that matches this encounter, not a random one
                _currentCaptain = FindSpecificCaptainForShipEncounter(encounter, _currentCaptainType);
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[EncounterMediaTransitionManager] 🚀 ENCOUNTER PREPARED (ShipEncounter): {encounter.captainName ?? "Unknown Captain"}");
                    Debug.Log($"[EncounterMediaTransitionManager] 👨‍✈️ Captain found: {_currentCaptain?.GetFullName() ?? "⚠️ NONE"} from type: {_currentCaptainType?.typeName ?? "⚠️ NONE"}");
                    if (_currentCaptain == null)
                        Debug.LogError($"[EncounterMediaTransitionManager] ❌ CRITICAL: No captain found for encounter {encounter.captainName}!");
                }
            }
        }
        
        /// <summary>
        /// Prepare for the next encounter with MasterShipEncounter data
        /// </summary>
        public void PrepareNextEncounter(MasterShipEncounter encounter)
        {
            _currentEncounter = null; // Clear old encounter
            
            // Try to get captain data from MasterShipEncounter
            if (encounter != null)
            {
                // Get captain type from the encounter's captain type data reference
                _currentCaptainType = GetCaptainTypeFromMasterEncounter(encounter);
                
                // Find the SPECIFIC captain that matches this encounter, not a random one
                _currentCaptain = FindSpecificCaptainForEncounter(encounter, _currentCaptainType);
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[EncounterMediaTransitionManager] 🚀 ENCOUNTER PREPARED (MasterShipEncounter): {encounter.captainName ?? "Unknown Captain"}");
                    Debug.Log($"[EncounterMediaTransitionManager] 👨‍✈️ Captain found: {_currentCaptain?.GetFullName() ?? "⚠️ NONE"} from type: {_currentCaptainType?.typeName ?? "⚠️ NONE"}");
                    if (_currentCaptain == null)
                        Debug.LogError($"[EncounterMediaTransitionManager] ❌ CRITICAL: No captain found for MasterShipEncounter {encounter.captainName}!");
                }
                    
                // Auto-play greeting videos for new encounters
                PlayGreetingVideos(encounter);
            }
        }
        
        /// <summary>
        /// Play greeting videos for a new encounter
        /// </summary>
        public void PlayGreetingVideos(MasterShipEncounter encounter)
        {
            if (encounter == null) return;
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Playing greeting videos for {encounter.shipType} - {encounter.captainName}");
            
            StartCoroutine(PlayGreetingSequence(encounter));
        }
        
        /// <summary>
        /// Play the greeting video sequence
        /// </summary>
        private IEnumerator PlayGreetingSequence(MasterShipEncounter encounter)
        {
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Starting greeting sequence for {encounter.shipType}");
            
            // Play captain greeting video if available
            if (encounter.HasCaptainVideo() && captainVideoPlayer != null)
            {
                yield return PlayEncounterVideo(captainVideoPlayer, encounter.captainVideo, "Captain");
            }
            else if (enableDebugLogs)
            {
                Debug.Log($"[EncounterMediaTransitionManager] No captain video for {encounter.captainName}");
            }
            
            // Play ship video if available
            if (encounter.HasShipVideo() && shipVideoPlayer != null)
            {
                yield return PlayEncounterVideo(shipVideoPlayer, encounter.shipVideo, "Ship");
            }
            else if (enableDebugLogs)
            {
                Debug.Log($"[EncounterMediaTransitionManager] No ship video for {encounter.shipType}");
            }
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Completed greeting sequence for {encounter.shipType}");
        }
        
        /// <summary>
        /// Play an encounter video (ship or captain)
        /// </summary>
        private IEnumerator PlayEncounterVideo(VideoPlayer videoPlayer, VideoClip videoClip, string videoType)
        {
            if (videoPlayer == null || videoClip == null) yield break;
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Starting {videoType} video: {videoClip.name}, Length: {videoClip.length}s");
            
            // Setup video
            videoPlayer.clip = videoClip;
            videoPlayer.isLooping = false;
            
            // Setup audio
            if (captainAudioSource != null && videoPlayer == captainVideoPlayer)
            {
                captainAudioSource.volume = defaultVolume;
            }
            
            // Prepare video
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] {videoType} video prepared, starting playback");
            
            // Play video
            videoPlayer.Play();
            
            // Wait for video to complete with timeout protection
            float maxWaitTime = (float)videoClip.length + 2f; // Video length + 2 second buffer
            float elapsedTime = 0f;
            
            while (videoPlayer.isPlaying && elapsedTime < maxWaitTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            
            // Force stop if video is still playing after timeout
            if (videoPlayer.isPlaying)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[EncounterMediaTransitionManager] {videoType} video timeout - forcing stop");
                videoPlayer.Stop();
            }
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] {videoType} video playback completed");
        }
        
        /// <summary>
        /// Show captain reaction video based on player decision
        /// </summary>
        public void ShowCaptainReactionVideo(bool approved, bool bribery = false)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[EncounterMediaTransitionManager] === CAPTAIN REACTION DEBUG START ===");
                Debug.Log($"[EncounterMediaTransitionManager] ShowCaptainReactionVideo called - Approved: {approved}, Bribery: {bribery}");
                Debug.Log($"[EncounterMediaTransitionManager] _currentCaptain: {(_currentCaptain != null ? _currentCaptain.GetFullName() : "NULL")}");
                Debug.Log($"[EncounterMediaTransitionManager] _currentCaptainType: {(_currentCaptainType != null ? _currentCaptainType.typeName : "NULL")}");
                Debug.Log($"[EncounterMediaTransitionManager] _currentEncounter: {(_currentEncounter != null ? $"{_currentEncounter.shipName} - {_currentEncounter.captainName}" : "NULL")}");
            }
                
            if (_currentCaptain == null)
            {
                Debug.LogError("[EncounterMediaTransitionManager] ❌ CRITICAL: No current captain for reaction video!");
                
                // Enhanced debugging to restore captain
                var credentialChecker = ServiceLocator.Get<CredentialChecker>();
                if (credentialChecker == null)
                    credentialChecker = FindObjectOfType<CredentialChecker>();
                
                if (credentialChecker != null)
                {
                    var currentMasterEncounter = credentialChecker.GetCurrentEncounter();
                    if (currentMasterEncounter != null)
                    {
                        Debug.Log($"[EncounterMediaTransitionManager] 🔄 Attempting captain restoration from encounter: {currentMasterEncounter.shipName}");
                        Debug.Log($"[EncounterMediaTransitionManager] Encounter captain name: '{currentMasterEncounter.captainName}'");
                        Debug.Log($"[EncounterMediaTransitionManager] Encounter faction: {currentMasterEncounter.faction}");
                        
                        _currentCaptainType = GetCaptainTypeFromMasterEncounter(currentMasterEncounter);
                        Debug.Log($"[EncounterMediaTransitionManager] Determined captain type: {(_currentCaptainType != null ? _currentCaptainType.typeName : "NULL")}");
                        
                        _currentCaptain = FindSpecificCaptainForEncounter(currentMasterEncounter, _currentCaptainType);
                        
                        if (_currentCaptain != null)
                        {
                            Debug.Log($"[EncounterMediaTransitionManager] ✅ Successfully restored captain: {_currentCaptain.GetFullName()}");
                        }
                        else
                        {
                            // Last resort - try to get any captain from the captain type
                            if (_currentCaptainType != null)
                            {
                                Debug.LogWarning("[EncounterMediaTransitionManager] ⚠️ Exact captain not found - using fallback from captain type");
                                _currentCaptain = _currentCaptainType.GetRandomCaptain();
                                
                                if (_currentCaptain != null)
                                {
                                    Debug.Log($"[EncounterMediaTransitionManager] ✅ Fallback captain obtained: {_currentCaptain.GetFullName()}");
                                }
                                else
                                {
                                    Debug.LogError("[EncounterMediaTransitionManager] ❌ FAILED - even fallback captain is null");
                                    return;
                                }
                            }
                            else
                            {
                                Debug.LogError("[EncounterMediaTransitionManager] ❌ FAILED to restore captain - no captain type available");
                                return;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("[EncounterMediaTransitionManager] ❌ No current encounter available from CredentialChecker");
                        return;
                    }
                }
                else
                {
                    Debug.LogError("[EncounterMediaTransitionManager] ❌ CredentialChecker not available");
                    return;
                }
            }
            
            if (_isPlayingReaction)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[EncounterMediaTransitionManager] Already playing reaction video");
                return;
            }
            
            // Determine which reaction to play
            CaptainType.Captain.DialogEntry reaction = null;
            string reactionType = "";
            
            Debug.Log($"[EncounterMediaTransitionManager] 🎬 Selecting reaction video for captain: {_currentCaptain.GetFullName()}");
            
            if (bribery)
            {
                reaction = _currentCaptain.GetRandomBriberyPhrase();
                reactionType = "Bribery";
                Debug.Log($"[EncounterMediaTransitionManager] 💰 Bribery reaction: {(reaction != null ? reaction.associatedVideo?.name ?? "No video clip" : "NULL")}");
                
                // If no bribery video, fall back to approval video since bribery results in approval
                if (reaction == null)
                {
                    Debug.Log($"[EncounterMediaTransitionManager] No bribery video for {_currentCaptain.GetFullName()}, using approval video instead");
                    reaction = _currentCaptain.GetRandomApprovalResponse();
                    reactionType = "Approval (Bribery Fallback)";
                    Debug.Log($"[EncounterMediaTransitionManager] ✅ Fallback approval reaction: {(reaction != null ? reaction.associatedVideo?.name ?? "No video clip" : "NULL")}");
                }
            }
            else if (approved)
            {
                reaction = _currentCaptain.GetRandomApprovalResponse();
                reactionType = "Approval";
                Debug.Log($"[EncounterMediaTransitionManager] ✅ Approval reaction: {(reaction != null ? reaction.associatedVideo?.name ?? "No video clip" : "NULL")}");
            }
            else
            {
                reaction = _currentCaptain.GetRandomDenialResponse();
                reactionType = "Denial";
                Debug.Log($"[EncounterMediaTransitionManager] ❌ Denial reaction: {(reaction != null ? reaction.associatedVideo?.name ?? "No video clip" : "NULL")}");
            }
            
            // Additional debug info about the selected reaction
            if (reaction != null)
            {
                Debug.Log($"[EncounterMediaTransitionManager] 🎯 Final selected reaction for {_currentCaptain.GetFullName()}:");
                Debug.Log($"[EncounterMediaTransitionManager]   - Type: {reactionType}");
                Debug.Log($"[EncounterMediaTransitionManager]   - Video: {(reaction.associatedVideo != null ? reaction.associatedVideo.name : "NO VIDEO CLIP")}");
                Debug.Log($"[EncounterMediaTransitionManager]   - Dialog: '{reaction.phrase}'");
                Debug.Log($"[EncounterMediaTransitionManager] === CAPTAIN REACTION DEBUG END ===");
                
                _reactionCoroutine = StartCoroutine(PlayReactionSequence(reaction, reactionType));
            }
            else
            {
                Debug.LogError($"[EncounterMediaTransitionManager] ❌ CRITICAL: No {reactionType} reaction available for {_currentCaptain.GetFullName()}!");
                Debug.Log($"[EncounterMediaTransitionManager] === CAPTAIN REACTION DEBUG END ===");
                
                // If still no reaction and this is a bribery/approval, we need to continue the flow
                if (bribery || approved)
                {
                    Debug.Log("[EncounterMediaTransitionManager] No reaction video available, triggering next encounter directly");
                    
                    // Trigger next encounter after a short delay to maintain flow
                    StartCoroutine(TriggerNextEncounterAfterDelay());
                }
            }
        }
        
        /// <summary>
        /// Show captain reaction for specific decision types
        /// </summary>
        public void ShowCaptainReactionForDecision(Starkiller.Core.DecisionType decision)
        {
            if (_currentCaptain == null) return;
            
            CaptainType.Captain.DialogEntry reaction = null;
            string reactionType = decision.ToString();
            
            switch (decision)
            {
                case Starkiller.Core.DecisionType.Approve:
                    reaction = _currentCaptain.GetRandomApprovalResponse();
                    break;
                case Starkiller.Core.DecisionType.Deny:
                    reaction = _currentCaptain.GetRandomDenialResponse();
                    break;
                case Starkiller.Core.DecisionType.HoldingPattern:
                    reaction = _currentCaptain.GetRandomHoldingPatternResponse();
                    break;
                case Starkiller.Core.DecisionType.TractorBeam:
                    reaction = _currentCaptain.GetRandomTractorBeamResponse();
                    break;
            }
            
            if (reaction != null)
            {
                _reactionCoroutine = StartCoroutine(PlayReactionSequence(reaction, reactionType));
            }
        }
        
        /// <summary>
        /// Play the complete reaction sequence
        /// </summary>
        private IEnumerator PlayReactionSequence(CaptainType.Captain.DialogEntry reaction, string reactionType)
        {
            _isPlayingReaction = true;
            bool videoPlayedSuccessfully = false;
            
            if (enableReactionLogging)
                Debug.Log($"[EncounterMediaTransitionManager] Playing {reactionType} reaction: \"{reaction.phrase}\"");
            
            // Notify that reaction video is starting (blocks text updates)
            OnReactionVideoStarted?.Invoke();
            
            // Disable UI interactions during reaction
            var canvasGroup = captainVideoPlayer?.GetComponentInParent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
            }
            
            // Pause game if enabled
            if (pauseGameDuringReaction)
            {
                var gameStateManager = ServiceLocator.Get<GameStateManager>();
                gameStateManager?.PauseGame();
            }
            
            // Trigger start event
            OnCaptainReactionStarted?.Invoke(reaction);
            
            // Wait for initial delay
            yield return new WaitForSeconds(reactionVideoDelay);
            
            // Show dialog text if enabled
            if (showDialogText && !string.IsNullOrEmpty(reaction.phrase))
            {
                yield return ShowDialogText(reaction.phrase);
            }
            
            // Play video if available
            if (reaction.associatedVideo != null && captainVideoPlayer != null)
            {
                if (enableReactionLogging)
                    Debug.Log($"[EncounterMediaTransitionManager] Attempting to play video: {reaction.associatedVideo.name}");
                
                yield return StartCoroutine(PlayReactionVideoWithResult(reaction.associatedVideo, result => videoPlayedSuccessfully = result));
            }
            else if (enableReactionLogging)
            {
                Debug.LogWarning($"[EncounterMediaTransitionManager] No video found for {reactionType} reaction");
                videoPlayedSuccessfully = false; // No video to play
            }
            
            // Wait for transition duration to ensure proper timing
            yield return new WaitForSeconds(0.5f);
            
            // Re-enable UI interactions
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
            }
            
            // Resume game if it was paused
            if (pauseGameDuringReaction)
            {
                var gameStateManager = ServiceLocator.Get<GameStateManager>();
                gameStateManager?.ResumeGame();
            }
            
            // Trigger completion event
            OnCaptainReactionCompleted?.Invoke(reaction);
            
            // Notify that reaction video is completed (allows text updates)
            OnReactionVideoCompleted?.Invoke();
            
            _isPlayingReaction = false;
            
            if (enableReactionLogging)
                Debug.Log($"[EncounterMediaTransitionManager] Completed {reactionType} reaction sequence (video success: {videoPlayedSuccessfully})");
            
            // Auto-advance to next encounter if enabled
            if (autoAdvanceAfterReaction)
            {
                float delayTime = videoPlayedSuccessfully ? autoAdvanceDelay : (autoAdvanceDelay + 1.0f); // Extra delay if video failed
                
                if (enableDebugLogs)
                    Debug.Log($"[EncounterMediaTransitionManager] Auto-advance enabled - triggering after {delayTime}s delay");
                
                StartCoroutine(DelayedAutoAdvance(delayTime));
            }
            else if (enableDebugLogs)
            {
                Debug.Log("[EncounterMediaTransitionManager] Auto-advance disabled - waiting for player action");
            }
        }
        
        /// <summary>
        /// Display dialog text with fade animation
        /// </summary>
        private IEnumerator ShowDialogText(string dialogText)
        {
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Showing dialog text: \"{dialogText}\"");
            
            if (captainDialogText == null) 
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[EncounterMediaTransitionManager] Captain Dialog Text not assigned!");
                yield break;
            }
            
            // Set text and show panel
            captainDialogText.text = dialogText;
            if (captainDialogPanel != null)
            {
                captainDialogPanel.SetActive(true);
                if (enableDebugLogs)
                    Debug.Log("[EncounterMediaTransitionManager] Dialog panel activated");
            }
            else if (enableDebugLogs)
            {
                Debug.LogWarning("[EncounterMediaTransitionManager] Captain Dialog Panel not assigned!");
            }
            
            // Fade in
            if (captainDialogCanvasGroup != null)
            {
                yield return FadeCanvasGroup(captainDialogCanvasGroup, 0f, 1f, fadeInDuration);
            }
            else
            {
                // If no canvas group, just wait a moment for text to appear
                yield return new WaitForSeconds(0.1f);
            }
            
            // Trigger dialog event
            OnDialogTextDisplayed?.Invoke(dialogText);
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Dialog text displayed for {dialogDisplayDuration}s");
            
            // Wait for display duration
            yield return new WaitForSeconds(dialogDisplayDuration);
            
            // Fade out
            if (captainDialogCanvasGroup != null)
            {
                yield return FadeCanvasGroup(captainDialogCanvasGroup, 1f, 0f, fadeOutDuration);
            }
            
            // Hide panel
            if (captainDialogPanel != null)
                captainDialogPanel.SetActive(false);
                
            if (enableDebugLogs)
                Debug.Log("[EncounterMediaTransitionManager] Dialog text hidden");
        }
        
        /// <summary>
        /// Play the reaction video with result callback
        /// </summary>
        private IEnumerator PlayReactionVideoWithResult(VideoClip reactionVideo, System.Action<bool> onComplete)
        {
            bool success = false;
            yield return StartCoroutine(PlayReactionVideoInternal(reactionVideo, result => success = result));
            onComplete?.Invoke(success);
        }
        
        /// <summary>
        /// Play the reaction video
        /// </summary>
        private IEnumerator PlayReactionVideo(VideoClip reactionVideo)
        {
            bool success = false;
            yield return StartCoroutine(PlayReactionVideoInternal(reactionVideo, result => success = result));
        }
        
        /// <summary>
        /// Internal method to play the reaction video with success tracking
        /// </summary>
        private IEnumerator PlayReactionVideoInternal(VideoClip reactionVideo, System.Action<bool> onComplete)
        {
            if (captainVideoPlayer == null) 
            {
                onComplete?.Invoke(false);
                yield break;
            }
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Starting video: {reactionVideo.name}, Length: {reactionVideo.length}s");
            
            // Show loading overlay if available
            if (videoLoadingOverlay != null)
                videoLoadingOverlay.SetActive(true);
            
            // Setup video
            captainVideoPlayer.clip = reactionVideo;
            captainVideoPlayer.isLooping = false;
            
            // Setup audio
            if (captainAudioSource != null)
            {
                captainAudioSource.volume = defaultVolume;
            }
            
            // Prepare video
            if (enableDebugLogs)
                Debug.Log("[EncounterMediaTransitionManager] Starting video preparation...");
            
            captainVideoPlayer.Prepare();
            
            float prepareTimeout = 10f; // 10 second timeout for preparation
            float prepareTime = 0f;
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Waiting for video preparation. isPrepared: {captainVideoPlayer.isPrepared}");
            
            while (!captainVideoPlayer.isPrepared && prepareTime < prepareTimeout)
            {
                prepareTime += Time.unscaledDeltaTime;
                yield return null;
            }
            
            if (!captainVideoPlayer.isPrepared)
            {
                if (enableDebugLogs)
                    Debug.LogError($"[EncounterMediaTransitionManager] Video preparation timeout after {prepareTime}s! Video playback failed.");
                
                // Hide loading overlay since we're not playing video
                if (videoLoadingOverlay != null)
                    videoLoadingOverlay.SetActive(false);
                
                onComplete?.Invoke(false); // Video failed to prepare
                yield break;
            }
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Video prepared in {prepareTime}s, starting playback");
            
            // Hide loading overlay
            if (videoLoadingOverlay != null)
                videoLoadingOverlay.SetActive(false);
            
            // Play video
            captainVideoPlayer.Play();
            
            // Wait for video to complete with timeout protection
            float maxWaitTime = (float)reactionVideo.length + 2f; // Video length + 2 second buffer
            float elapsedTime = 0f;
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Waiting for video completion. MaxWait: {maxWaitTime}s, isPlaying: {captainVideoPlayer.isPlaying}");
            
            while (captainVideoPlayer.isPlaying && elapsedTime < maxWaitTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            
            bool videoCompletedNormally = elapsedTime < maxWaitTime;
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Video wait loop ended. ElapsedTime: {elapsedTime}s, isPlaying: {captainVideoPlayer.isPlaying}, timeout: {!videoCompletedNormally}");
            
            // Force stop if video is still playing after timeout
            if (captainVideoPlayer.isPlaying)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[EncounterMediaTransitionManager] Video timeout - forcing stop");
                captainVideoPlayer.Stop();
            }
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Video playback completed (success: {videoCompletedNormally})");
            
            onComplete?.Invoke(videoCompletedNormally); // Report success/failure
        }
        
        /// <summary>
        /// Handle video completion
        /// </summary>
        private void OnVideoCompleted(VideoPlayer player)
        {
            if (enableDebugLogs)
                Debug.Log("[EncounterMediaTransitionManager] Reaction video completed");
                
            // Log auto-advance status
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] Auto-advance enabled: {autoAdvanceAfterReaction}, Is playing reaction: {_isPlayingReaction}");
        }
        
        /// <summary>
        /// Fade a canvas group
        /// </summary>
        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration)
        {
            if (canvasGroup == null) yield break;
            
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(from, to, elapsedTime / duration);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            canvasGroup.alpha = to;
        }
        
        /// <summary>
        /// Get captain type from encounter (using actual encounter structure)
        /// </summary>
        private CaptainType GetCaptainTypeFromEncounter(ShipEncounter encounter)
        {
            // Try to get from encounter's captain type data reference
            if (encounter.captainTypeData != null)
            {
                return encounter.captainTypeData;
            }
            
            // Try to get from scenario data if available
            if (encounter.scenarioData != null)
            {
                // Get captain types applicable to this scenario
                if (encounter.scenarioData.applicableCaptainTypes != null && 
                    encounter.scenarioData.applicableCaptainTypes.Length > 0)
                {
                    return encounter.scenarioData.applicableCaptainTypes[0];
                }
            }
            
            // Fallback: try to find captain type by faction
            if (!string.IsNullOrEmpty(encounter.captainFaction))
            {
                return FindCaptainTypeByFaction(encounter.captainFaction);
            }
            
            return null;
        }
        
        /// <summary>
        /// Find the specific captain that matches the ShipEncounter's captain data
        /// </summary>
        private CaptainType.Captain FindSpecificCaptainForShipEncounter(ShipEncounter encounter, CaptainType captainType)
        {
            if (encounter == null || captainType == null)
                return null;
                
            // If no captains are defined in the CaptainType, generate one
            if (captainType.captains == null || captainType.captains.Count == 0)
            {
                if (enableDebugLogs)
                    Debug.Log($"[EncounterMediaTransitionManager] No captains defined in {captainType.typeName}, using generated captain");
                return captainType.GetRandomCaptain(); // This will generate one
            }
            
            // Try to find a captain that matches the encounter's captain name and rank
            string targetFullName = $"{encounter.captainRank} {encounter.captainName}".Trim();
            
            foreach (var captain in captainType.captains)
            {
                string captainFullName = captain.GetFullName();
                
                // Exact match preferred
                if (captainFullName.Equals(targetFullName, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[EncounterMediaTransitionManager] Found exact captain match: {captainFullName}");
                    return captain;
                }
                
                // Also try matching just the name parts
                if (captain.firstName.Equals(encounter.captainName, System.StringComparison.OrdinalIgnoreCase) ||
                    captain.lastName.Equals(encounter.captainName, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[EncounterMediaTransitionManager] Found partial captain match: {captainFullName} for {encounter.captainName}");
                    return captain;
                }
            }
            
            // If no exact match found, return the first captain as fallback
            if (enableDebugLogs)
                Debug.LogWarning($"[EncounterMediaTransitionManager] No matching captain found for '{targetFullName}', using first available: {captainType.captains[0].GetFullName()}");
            
            return captainType.captains[0];
        }
        
        /// <summary>
        /// Find the specific captain that matches the encounter's captain data
        /// </summary>
        private CaptainType.Captain FindSpecificCaptainForEncounter(MasterShipEncounter encounter, CaptainType captainType)
        {
            if (encounter == null || captainType == null)
                return null;
            
            // PRIORITY 1: First check if encounter has a stored captain reference (most reliable)
            if (encounter.selectedCaptain != null)
            {
                if (enableDebugLogs)
                    Debug.Log($"[EncounterMediaTransitionManager] ✅ Using stored captain reference: {encounter.selectedCaptain.GetFullName()}");
                return encounter.selectedCaptain;
            }
                
            // If no captains are defined in the CaptainType, generate one
            if (captainType.captains == null || captainType.captains.Count == 0)
            {
                if (enableDebugLogs)
                    Debug.Log($"[EncounterMediaTransitionManager] No captains defined in {captainType.typeName}, using generated captain");
                return captainType.GetRandomCaptain(); // This will generate one
            }
            
            // PRIORITY 2: Try to find a captain that matches the encounter's captain name
            string targetFullName = $"{encounter.captainRank} {encounter.captainName}".Trim();
            string targetNameOnly = encounter.captainName?.Trim() ?? "";
            
            if (enableDebugLogs)
            {
                Debug.Log($"[EncounterMediaTransitionManager] 🔍 CAPTAIN SEARCH DEBUG:");
                Debug.Log($"[EncounterMediaTransitionManager]   Target Name: '{targetNameOnly}'");
                Debug.Log($"[EncounterMediaTransitionManager]   Target Full: '{targetFullName}'");
                Debug.Log($"[EncounterMediaTransitionManager]   Available Captains in {captainType.typeName}:");
                for (int i = 0; i < captainType.captains.Count; i++)
                {
                    var cap = captainType.captains[i];
                    Debug.Log($"[EncounterMediaTransitionManager]     [{i}] {cap.GetFullName()} (first: '{cap.firstName}', last: '{cap.lastName}')");
                }
            }
            
            // ENHANCED MATCHING ALGORITHM - Try multiple approaches in order of reliability
            
            // Method 1: Exact full name match (most reliable)
            foreach (var captain in captainType.captains)
            {
                string captainFullName = captain.GetFullName();
                if (captainFullName.Equals(targetFullName, System.StringComparison.OrdinalIgnoreCase) ||
                    captainFullName.Equals(targetNameOnly, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[EncounterMediaTransitionManager] ✅ EXACT MATCH found: {captainFullName}");
                    return captain;
                }
            }
            
            // Method 2: First+Last name combination match
            foreach (var captain in captainType.captains)
            {
                string captainFirstLast = $"{captain.firstName} {captain.lastName}".Trim();
                if (captainFirstLast.Equals(targetNameOnly, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[EncounterMediaTransitionManager] ✅ FIRST+LAST MATCH found: {captain.GetFullName()}");
                    return captain;
                }
            }
            
            // Method 3: Check if target contains both first and last name (handle spacing variations)
            foreach (var captain in captainType.captains)
            {
                if (!string.IsNullOrEmpty(targetNameOnly) && 
                    targetNameOnly.Contains(captain.firstName, System.StringComparison.OrdinalIgnoreCase) &&
                    targetNameOnly.Contains(captain.lastName, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[EncounterMediaTransitionManager] ✅ CONTAINS BOTH NAMES found: {captain.GetFullName()} matches '{targetNameOnly}'");
                    return captain;
                }
            }
            
            // Method 4: Individual name part matching (less reliable, but catches variations)
            foreach (var captain in captainType.captains)
            {
                string[] nameParts = targetNameOnly.Split(new char[] { ' ', '_', '-' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in nameParts)
                {
                    if (!string.IsNullOrEmpty(part) && part.Length > 2) // Only match parts longer than 2 chars to avoid false positives
                    {
                        if (captain.firstName.Equals(part, System.StringComparison.OrdinalIgnoreCase) ||
                            captain.lastName.Equals(part, System.StringComparison.OrdinalIgnoreCase))
                        {
                            if (enableDebugLogs)
                                Debug.Log($"[EncounterMediaTransitionManager] ✅ NAME PART MATCH found: {captain.GetFullName()} matches part '{part}'");
                            return captain;
                        }
                    }
                }
            }
            
            // Method 5: Try to match by video reference (if encounter came from this captain type)
            if (encounter.captainTypeData != null && encounter.captainTypeData == captainType && encounter.captainVideo != null)
            {
                foreach (var captain in captainType.captains)
                {
                    if (captain.greetingDialogs != null)
                    {
                        foreach (var dialog in captain.greetingDialogs)
                        {
                            if (dialog.associatedVideo == encounter.captainVideo)
                            {
                                if (enableDebugLogs)
                                    Debug.Log($"[EncounterMediaTransitionManager] ✅ VIDEO MATCH found: {captain.GetFullName()}");
                                return captain;
                            }
                        }
                    }
                }
            }
            
            // Method 6: Fuzzy matching for common variations (e.g., "idos" vs "Idos")
            foreach (var captain in captainType.captains)
            {
                // Check if any part of the target name is similar to captain names (case-insensitive, partial matches)
                string lowerTarget = targetNameOnly.ToLower().Replace(" ", "");
                string lowerFirst = captain.firstName.ToLower().Replace(" ", "");
                string lowerLast = captain.lastName.ToLower().Replace(" ", "");
                
                if (lowerTarget.Contains(lowerFirst) || lowerTarget.Contains(lowerLast) ||
                    lowerFirst.Contains(lowerTarget) || lowerLast.Contains(lowerTarget))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[EncounterMediaTransitionManager] ✅ FUZZY MATCH found: {captain.GetFullName()} fuzzy matches '{targetNameOnly}'");
                    return captain;
                }
            }
            
            // LAST RESORT: Log detailed failure and return first captain
            if (enableDebugLogs)
            {
                Debug.LogError($"[EncounterMediaTransitionManager] ❌ NO MATCH FOUND for '{targetNameOnly}'!");
                Debug.LogError($"[EncounterMediaTransitionManager] This should not happen if encounter.selectedCaptain was set correctly");
                Debug.LogError($"[EncounterMediaTransitionManager] Using FALLBACK: {captainType.captains[0].GetFullName()}");
                Debug.LogError($"[EncounterMediaTransitionManager] Please check MasterShipEncounter.CreateFromScriptableObjects captain selection logic");
            }
            
            return captainType.captains[0];
        }
        
        /// <summary>
        /// Get captain type from MasterShipEncounter
        /// </summary>
        private CaptainType GetCaptainTypeFromMasterEncounter(MasterShipEncounter encounter)
        {
            // Try to get from encounter's captain type data reference
            if (encounter.captainTypeData != null)
            {
                return encounter.captainTypeData;
            }
            
            // Try to get from scenario data if available
            if (encounter.scenarioData != null)
            {
                // Get captain types applicable to this scenario
                if (encounter.scenarioData.applicableCaptainTypes != null && 
                    encounter.scenarioData.applicableCaptainTypes.Length > 0)
                {
                    return encounter.scenarioData.applicableCaptainTypes[0];
                }
            }
            
            // Fallback: try to find captain type by faction
            if (!string.IsNullOrEmpty(encounter.captainFaction))
            {
                return FindCaptainTypeByFaction(encounter.captainFaction);
            }
            
            return null;
        }
        
        /// <summary>
        /// Find captain type by faction (implement based on your needs)
        /// </summary>
        private CaptainType FindCaptainTypeByFaction(string faction)
        {
            // Load all captain types and find one that matches the faction
            CaptainType[] allCaptainTypes = Resources.LoadAll<CaptainType>("_ScriptableObjects/CaptainTypes");
            
            foreach (var captainType in allCaptainTypes)
            {
                if (captainType.BelongsToFaction(faction))
                {
                    return captainType;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Stop current reaction sequence
        /// </summary>
        public void StopCurrentReaction()
        {
            if (_reactionCoroutine != null)
            {
                StopCoroutine(_reactionCoroutine);
                _reactionCoroutine = null;
            }
            
            if (captainVideoPlayer != null && captainVideoPlayer.isPlaying)
            {
                captainVideoPlayer.Stop();
            }
            
            if (captainDialogPanel != null)
                captainDialogPanel.SetActive(false);
            
            // CRITICAL: Re-enable UI interactions that may have been disabled
            var canvasGroup = captainVideoPlayer?.GetComponentInParent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
                if (enableDebugLogs)
                    Debug.Log("[EncounterMediaTransitionManager] Re-enabled canvas group interactions in StopCurrentReaction");
            }
            
            // Notify that reaction is complete to unblock any waiting systems
            OnReactionVideoCompleted?.Invoke();
            
            _isPlayingReaction = false;
        }
        
        /// <summary>
        /// Set current captain manually (for testing or special cases)
        /// </summary>
        public void SetCurrentCaptain(CaptainType.Captain captain, CaptainType captainType)
        {
            _currentCaptain = captain;
            _currentCaptainType = captainType;
            
            if (enableDebugLogs)
                Debug.Log($"[EncounterMediaTransitionManager] 👨‍✈️ Captain SET: {captain?.GetFullName()} (Type: {captainType?.typeName})");
        }
        
        /// <summary>
        /// Trigger the next encounter after reaction completes
        /// </summary>
        private void TriggerNextEncounter()
        {
            // Prevent rapid successive calls that could cause flickering
            if (_isTriggeringNextEncounter)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[EncounterMediaTransitionManager] TriggerNextEncounter called while already triggering - ignoring to prevent flickering");
                return;
            }
            
            _isTriggeringNextEncounter = true;
            
            if (enableDebugLogs)
                Debug.Log("[EncounterMediaTransitionManager] Triggering next encounter...");
            
            try
            {
                // First check if we have a CredentialChecker
                var credentialChecker = ServiceLocator.Get<CredentialChecker>();
                if (credentialChecker == null)
                {
                    credentialChecker = FindObjectOfType<CredentialChecker>();
                }
                
                if (credentialChecker != null)
                {
                    // Use CredentialChecker to handle the next encounter flow
                    credentialChecker.NextEncounter();
                }
                else
                {
                    // Fallback: directly trigger MasterShipGenerator
                    var shipGenerator = MasterShipGenerator.Instance;
                    if (shipGenerator == null)
                    {
                        shipGenerator = FindObjectOfType<MasterShipGenerator>();
                    }
                    
                    if (shipGenerator != null)
                    {
                        if (enableDebugLogs)
                            Debug.Log("[EncounterMediaTransitionManager] Using fallback - calling MasterShipGenerator directly");
                        shipGenerator.GetNextEncounter();
                    }
                    else
                    {
                        Debug.LogWarning("[EncounterMediaTransitionManager] Could not find CredentialChecker or MasterShipGenerator to trigger next encounter!");
                    }
                }
            }
            finally
            {
                // Reset the flag after a short delay to allow the encounter to be processed
                StartCoroutine(ResetTriggerFlag());
            }
        }
        
        /// <summary>
        /// Reset the trigger flag after a short delay
        /// </summary>
        private IEnumerator ResetTriggerFlag()
        {
            yield return new WaitForSeconds(0.5f); // Half second cooldown
            _isTriggeringNextEncounter = false;
            
            if (enableDebugLogs)
                Debug.Log("[EncounterMediaTransitionManager] Trigger flag reset - ready for next encounter");
        }
        
        /// <summary>
        /// Trigger next encounter after a delay (used when no reaction video is available)
        /// </summary>
        private IEnumerator TriggerNextEncounterAfterDelay()
        {
            if (enableDebugLogs)
                Debug.Log("[EncounterMediaTransitionManager] Waiting before triggering next encounter (no video fallback)");
                
            // Wait a bit to allow other systems to process
            yield return new WaitForSeconds(1.5f);
            
            // Trigger the next encounter
            TriggerNextEncounter();
        }
        
        /// <summary>
        /// Delayed auto-advance after video completion
        /// </summary>
        private IEnumerator DelayedAutoAdvance(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (enableDebugLogs)
                Debug.Log("[EncounterMediaTransitionManager] Delayed auto-advance triggered");
            
            TriggerNextEncounter();
        }
        
        /// <summary>
        /// Clear the manager state for a new day
        /// </summary>
        public void ClearForNewDay()
        {
            if (enableDebugLogs)
                Debug.Log("[EncounterMediaTransitionManager] Clearing state for new day");
            
            // Stop any ongoing reactions (this now includes UI unblocking)
            StopCurrentReaction();
            
            // Only clear captain state if we're not in the middle of processing an encounter
            if (!_isPlayingReaction)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[EncounterMediaTransitionManager] 🧹 CLEARING captain state - Previous: {(_currentCaptain != null ? _currentCaptain.GetFullName() : "NULL")}");
                }
                
                _currentEncounter = null;
                _currentCaptain = null;
                _currentCaptainType = null;
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.Log("[EncounterMediaTransitionManager] ⚠️ Skipping captain state clear - reaction video in progress");
                }
            }
            
            _isTriggeringNextEncounter = false;
            
            // Stop any playing videos
            if (captainVideoPlayer != null && captainVideoPlayer.isPlaying)
            {
                captainVideoPlayer.Stop();
            }
            
            // FAILSAFE: Ensure UI is never left in a blocked state
            var canvasGroup = captainVideoPlayer?.GetComponentInParent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
                if (enableDebugLogs)
                    Debug.Log("[EncounterMediaTransitionManager] Ensured canvas group is not blocking (failsafe)");
            }
            
            // Hide dialog UI
            if (captainDialogPanel != null)
                captainDialogPanel.SetActive(false);
            if (captainDialogText != null)
                captainDialogText.text = "";
            
            if (enableDebugLogs)
                Debug.Log("[EncounterMediaTransitionManager] State cleared for new day");
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from video events
            if (captainVideoPlayer != null)
            {
                captainVideoPlayer.loopPointReached -= OnVideoCompleted;
            }
            
            // Clear instance reference
            if (_instance == this)
                _instance = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Play Approval Reaction")]
        private void TestApprovalReaction()
        {
            ShowCaptainReactionVideo(true, false);
        }
        
        [ContextMenu("Test: Play Denial Reaction")]
        private void TestDenialReaction()
        {
            ShowCaptainReactionVideo(false, false);
        }
        
        [ContextMenu("Test: Play Bribery Reaction")]
        private void TestBriberyReaction()
        {
            ShowCaptainReactionVideo(true, true);
        }
        
        [ContextMenu("Test: Play Holding Pattern Reaction")]
        private void TestHoldingPatternReaction()
        {
            ShowCaptainReactionForDecision(Starkiller.Core.DecisionType.HoldingPattern);
        }
        
        [ContextMenu("Test: Play Tractor Beam Reaction")]
        private void TestTractorBeamReaction()
        {
            ShowCaptainReactionForDecision(Starkiller.Core.DecisionType.TractorBeam);
        }
        
        [ContextMenu("Debug: Check Current Captain")]
        private void DebugCheckCurrentCaptain()
        {
            if (_currentCaptain == null)
            {
                Debug.LogError("[EncounterMediaTransitionManager] No current captain set!");
                return;
            }
            
            Debug.Log($"[EncounterMediaTransitionManager] Current Captain: {_currentCaptain.GetFullName()}");
            Debug.Log($"[EncounterMediaTransitionManager] Greeting videos: {_currentCaptain.greetingDialogs.Count}");
            Debug.Log($"[EncounterMediaTransitionManager] Approval videos: {_currentCaptain.approvedDialogs.Count}");
            Debug.Log($"[EncounterMediaTransitionManager] Denial videos: {_currentCaptain.deniedDialogs.Count}");
            Debug.Log($"[EncounterMediaTransitionManager] Bribe videos: {_currentCaptain.bribeDialogs.Count}");
            Debug.Log($"[EncounterMediaTransitionManager] Holding Pattern videos: {_currentCaptain.holdingPatternDialogs.Count}");
            Debug.Log($"[EncounterMediaTransitionManager] Tractor Beam videos: {_currentCaptain.tractorBeamDialogs.Count}");
        }
        
        [ContextMenu("Debug: Stop Current Reaction")]
        private void DebugStopReaction()
        {
            StopCurrentReaction();
        }
        
        [ContextMenu("Debug: Test Dialog Text")]
        private void DebugTestDialogText()
        {
            StartCoroutine(ShowDialogText("This is a test dialog message from the captain!"));
        }
        
        [ContextMenu("Debug: Check UI References")]
        private void DebugCheckUIReferences()
        {
            Debug.Log("=== EncounterMediaTransitionManager UI References ===");
            Debug.Log($"Captain Video Player: {(captainVideoPlayer != null ? captainVideoPlayer.name : "NOT ASSIGNED")}");
            Debug.Log($"Captain Dialog Text: {(captainDialogText != null ? captainDialogText.name : "NOT ASSIGNED")}");
            Debug.Log($"Captain Dialog Panel: {(captainDialogPanel != null ? captainDialogPanel.name : "NOT ASSIGNED")}");
            Debug.Log($"Captain Dialog Canvas Group: {(captainDialogCanvasGroup != null ? captainDialogCanvasGroup.name : "NOT ASSIGNED")}");
            Debug.Log($"Show Dialog Text Enabled: {showDialogText}");
            Debug.Log($"Dialog Display Duration: {dialogDisplayDuration}s");
        }
    }
}