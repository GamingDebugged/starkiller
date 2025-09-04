using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Starkiller.Core.Helpers
{
    /// <summary>
    /// Helper script that manages scenario-based media playback and UI display
    /// Supports multiple scenario types with a single adaptable panel
    /// </summary>
    public class ScenarioMediaHelper : MonoBehaviour
    {
        [Header("UI Panel References")]
        [SerializeField] private GameObject scenarioPanel; // Multi-purpose panel (formerly inspection panel)
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text mainMessageText;
        [SerializeField] private TMP_Text reasonText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text footerText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private UnityEngine.UI.Button continueButton; // Button to close panel manually
        
        [Header("Video Components")]
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private RawImage videoDisplay;
        [SerializeField] private GameObject videoFrame;
        [SerializeField] private GameObject videoSection;
        
        [Header("Audio Components")]
        [SerializeField] private AudioSource audioSource;
        
        [Header("Scenario Type Configurations")]
        [SerializeField] private ScenarioUIConfig[] scenarioConfigs;
        
        [Header("Default Settings")]
        [SerializeField] private float defaultDisplayDuration = 6f;
        [SerializeField] private bool autoPlayVideo = true;
        [SerializeField] private bool autoHidePanel = false; // Changed to false - wait for user interaction
        [SerializeField] private bool pauseOtherVideos = true; // Pause other video players when showing scenario
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;
        
        [Header("Video Pause Management")]
        [SerializeField] private string[] videoPlayerGameObjectNames = {"CaptainVideoPlayer", "ShipVideoPlayer", "BackgroundVideoPlayer"};
        
        // Current scenario state
        private ShipScenario _currentScenario;
        private bool _isPanelActive = false;
        private Coroutine _autoHideCoroutine;
        private Action _onCompleteCallback;
        private List<VideoPlayer> _pausedVideoPlayers = new List<VideoPlayer>();
        
        // Events
        public static event Action<ShipScenario> OnScenarioPanelOpened;
        public static event Action<ShipScenario> OnScenarioPanelClosed;
        public static event Action<ShipScenario, VideoClip> OnScenarioVideoStarted;
        
        // Singleton pattern for easy access
        private static ScenarioMediaHelper _instance;
        public static ScenarioMediaHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ScenarioMediaHelper>();
                }
                return _instance;
            }
        }
        
        private void Awake()
        {
            _instance = this;
            
            // Ensure panel starts hidden
            if (scenarioPanel != null)
                scenarioPanel.SetActive(false);
                
            // Setup continue button
            SetupContinueButton();
                
            // Register with ServiceLocator
            ServiceLocator.Register<ScenarioMediaHelper>(this);
        }
        
        /// <summary>
        /// Setup the continue button to close the panel
        /// </summary>
        private void SetupContinueButton()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(OnContinueButtonClicked);
            }
        }
        
        /// <summary>
        /// Handle continue button click
        /// </summary>
        private void OnContinueButtonClicked()
        {
            StartCoroutine(CloseScenarioWithFade());
        }
        
        /// <summary>
        /// Display a scenario with appropriate UI and media
        /// </summary>
        public void ShowScenario(ShipScenario scenario, string customMessage = "", Action onComplete = null)
        {
            if (scenario == null)
            {
                Debug.LogError("[ScenarioMediaHelper] Cannot show null scenario");
                return;
            }
            
            if (_isPanelActive)
            {
                Debug.LogWarning("[ScenarioMediaHelper] Panel already active, closing previous scenario");
                CloseScenario();
            }
            
            _currentScenario = scenario;
            _isPanelActive = true;
            _onCompleteCallback = onComplete;
            
            // Pause other video players
            if (pauseOtherVideos)
            {
                PauseOtherVideoPlayers();
            }
            
            // Configure UI based on scenario type
            ConfigureUIForScenario(scenario, customMessage);
            
            // Show panel with fade
            StartCoroutine(ShowPanelSequence(scenario, onComplete));
            
            OnScenarioPanelOpened?.Invoke(scenario);
        }
        
        /// <summary>
        /// Configure UI elements based on scenario type
        /// </summary>
        private void ConfigureUIForScenario(ShipScenario scenario, string customMessage)
        {
            // Find matching config for scenario type
            ScenarioUIConfig config = GetConfigForType(scenario.type);
            
            // Apply configuration
            if (titleText != null)
                titleText.text = config.titleText;
                
            if (mainMessageText != null)
                mainMessageText.text = string.IsNullOrEmpty(customMessage) ? config.defaultMessage : customMessage;
                
            if (reasonText != null)
                reasonText.text = scenario.GetRandomStory();
                
            if (statusText != null)
                statusText.text = config.statusText;
                
            if (footerText != null)
                footerText.text = config.footerText;
                
            if (backgroundImage != null)
                backgroundImage.color = config.backgroundColor;
                
            if (iconImage != null && config.icon != null)
            {
                iconImage.sprite = config.icon;
                iconImage.enabled = true;
            }
            else if (iconImage != null)
            {
                iconImage.enabled = false;
            }
            
            // Configure video section visibility
            bool hasVideo = scenario.GetVideoClip() != null;
            if (videoSection != null)
                videoSection.SetActive(hasVideo && config.showVideo);
                
            // Setup video if available
            if (hasVideo && videoPlayer != null && config.showVideo)
            {
                SetupVideo(scenario.GetVideoClip());
            }
            
            // Setup audio if available
            if (scenario.scenarioAudioClip != null && audioSource != null)
            {
                audioSource.clip = scenario.scenarioAudioClip;
                if (config.playAudio)
                    audioSource.Play();
            }
        }
        
        /// <summary>
        /// Get UI configuration for scenario type
        /// </summary>
        private ScenarioUIConfig GetConfigForType(ShipScenario.ScenarioType type)
        {
            // Try to find specific config
            foreach (var config in scenarioConfigs)
            {
                if (config.scenarioType == type)
                    return config;
            }
            
            // Return default config if not found
            return new ScenarioUIConfig
            {
                scenarioType = type,
                titleText = type.ToString().ToUpper(),
                defaultMessage = "Processing scenario...",
                statusText = "IN PROGRESS",
                footerText = "Please wait...",
                backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f),
                showVideo = true,
                playAudio = true
            };
        }
        
        /// <summary>
        /// Setup video player with clip
        /// </summary>
        private void SetupVideo(VideoClip clip)
        {
            if (videoPlayer == null || clip == null) return;
            
            videoPlayer.clip = clip;
            videoPlayer.isLooping = true;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            
            if (videoDisplay != null && videoPlayer.targetTexture != null)
            {
                videoDisplay.texture = videoPlayer.targetTexture;
            }
            
            if (autoPlayVideo)
            {
                StartCoroutine(PlayVideoWhenReady());
            }
        }
        
        /// <summary>
        /// Play video when prepared
        /// </summary>
        private IEnumerator PlayVideoWhenReady()
        {
            videoPlayer.Prepare();
            
            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }
            
            videoPlayer.Play();
            OnScenarioVideoStarted?.Invoke(_currentScenario, videoPlayer.clip);
        }
        
        /// <summary>
        /// Show panel with fade animation
        /// </summary>
        private IEnumerator ShowPanelSequence(ShipScenario scenario, Action onComplete)
        {
            // Activate panel
            scenarioPanel.SetActive(true);
            
            // Fade in
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                float elapsedTime = 0f;
                
                while (elapsedTime < fadeInDuration)
                {
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
                    elapsedTime += Time.unscaledDeltaTime;
                    yield return null;
                }
                
                canvasGroup.alpha = 1f;
            }
            
            // Check if scenario config requires user input
            var config = GetConfigForType(scenario.type);
            bool shouldWaitForInput = config.waitForUserInput || !autoHidePanel;
            
            // Show continue button if waiting for user input
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(shouldWaitForInput);
            }
            
            // Auto-hide after duration only if not waiting for user input
            if (!shouldWaitForInput)
            {
                float duration = GetDisplayDuration(scenario);
                _autoHideCoroutine = StartCoroutine(AutoHideAfterDelay(duration, onComplete));
            }
        }
        
        /// <summary>
        /// Get display duration for scenario
        /// </summary>
        private float GetDisplayDuration(ShipScenario scenario)
        {
            var config = GetConfigForType(scenario.type);
            return config.displayDuration > 0 ? config.displayDuration : defaultDisplayDuration;
        }
        
        /// <summary>
        /// Auto-hide panel after delay
        /// </summary>
        private IEnumerator AutoHideAfterDelay(float delay, Action onComplete)
        {
            yield return new WaitForSeconds(delay);
            
            // Fade out
            if (canvasGroup != null)
            {
                float elapsedTime = 0f;
                
                while (elapsedTime < fadeOutDuration)
                {
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
                    elapsedTime += Time.unscaledDeltaTime;
                    yield return null;
                }
                
                canvasGroup.alpha = 0f;
            }
            
            CloseScenario();
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// Close the scenario panel
        /// </summary>
        public void CloseScenario()
        {
            if (_autoHideCoroutine != null)
            {
                StopCoroutine(_autoHideCoroutine);
                _autoHideCoroutine = null;
            }
            
            // Stop video
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            
            // Stop audio
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            
            // Resume other video players
            ResumeOtherVideoPlayers();
            
            // Hide panel
            if (scenarioPanel != null)
                scenarioPanel.SetActive(false);
                
            _isPanelActive = false;
            
            if (_currentScenario != null)
            {
                OnScenarioPanelClosed?.Invoke(_currentScenario);
                _currentScenario = null;
            }
            
            // Call completion callback
            _onCompleteCallback?.Invoke();
            _onCompleteCallback = null;
        }
        
        /// <summary>
        /// Close scenario with fade animation
        /// </summary>
        private IEnumerator CloseScenarioWithFade()
        {
            // Fade out
            if (canvasGroup != null)
            {
                float elapsedTime = 0f;
                
                while (elapsedTime < fadeOutDuration)
                {
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
                    elapsedTime += Time.unscaledDeltaTime;
                    yield return null;
                }
                
                canvasGroup.alpha = 0f;
            }
            
            CloseScenario();
        }
        
        /// <summary>
        /// Pause other video players in the scene
        /// </summary>
        private void PauseOtherVideoPlayers()
        {
            _pausedVideoPlayers.Clear();
            
            // Find video players by name
            foreach (string playerName in videoPlayerGameObjectNames)
            {
                GameObject playerObject = GameObject.Find(playerName);
                if (playerObject != null)
                {
                    VideoPlayer player = playerObject.GetComponent<VideoPlayer>();
                    if (player != null && player.isPlaying)
                    {
                        player.Pause();
                        _pausedVideoPlayers.Add(player);
                        Debug.Log($"[ScenarioMediaHelper] Paused video player: {playerName}");
                    }
                }
            }
            
            // Also find all VideoPlayer components in the scene
            VideoPlayer[] allPlayers = FindObjectsOfType<VideoPlayer>();
            foreach (VideoPlayer player in allPlayers)
            {
                // Skip our own video player and already paused ones
                if (player != videoPlayer && player.isPlaying && !_pausedVideoPlayers.Contains(player))
                {
                    player.Pause();
                    _pausedVideoPlayers.Add(player);
                    Debug.Log($"[ScenarioMediaHelper] Paused video player: {player.name}");
                }
            }
        }
        
        /// <summary>
        /// Resume previously paused video players
        /// </summary>
        private void ResumeOtherVideoPlayers()
        {
            foreach (VideoPlayer player in _pausedVideoPlayers)
            {
                if (player != null)
                {
                    player.Play();
                    Debug.Log($"[ScenarioMediaHelper] Resumed video player: {player.name}");
                }
            }
            
            _pausedVideoPlayers.Clear();
        }
        
        /// <summary>
        /// Check if panel is currently active
        /// </summary>
        public bool IsPanelActive => _isPanelActive;
        
        /// <summary>
        /// Get current scenario being displayed
        /// </summary>
        public ShipScenario CurrentScenario => _currentScenario;
        
        private void OnDestroy()
        {
            // Clear instance reference
            if (_instance == this)
                _instance = null;
        }
    }
    
    /// <summary>
    /// Configuration for different scenario type UI presentations
    /// </summary>
    [System.Serializable]
    public class ScenarioUIConfig
    {
        public ShipScenario.ScenarioType scenarioType;
        
        [Header("Text Configuration")]
        public string titleText = "SCENARIO";
        [TextArea(2, 4)]
        public string defaultMessage = "Processing scenario...";
        public string statusText = "IN PROGRESS";
        public string footerText = "Please wait...";
        
        [Header("Visual Configuration")]
        public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        public Sprite icon;
        public bool showVideo = true;
        
        [Header("Audio Configuration")]
        public bool playAudio = true;
        
        [Header("Timing")]
        public float displayDuration = 6f;
        public bool waitForUserInput = false; // If true, panel stays open until user clicks continue
    }
}