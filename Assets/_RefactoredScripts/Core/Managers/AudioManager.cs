using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages all audio playback including sound effects, music, and ambient sounds
    /// Extracted from GameManager for focused audio responsibility
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambientSource;
        [SerializeField] private AudioSource uiSource;
        
        [Header("Audio Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;
        [SerializeField] private float ambientVolume = 0.5f;
        [SerializeField] private float uiVolume = 0.8f;
        
        [Header("Music Management")]
        [SerializeField] private bool loopMusic = true;
        [SerializeField] private float musicFadeTime = 2f;
        [SerializeField] private AudioClip[] gameplayMusicTracks;
        [SerializeField] private AudioClip[] menuMusicTracks;
        
        [Header("Sound Effects")]
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip shipApprovedSound;
        [SerializeField] private AudioClip shipDeniedSound;
        [SerializeField] private AudioClip alarmSound;
        [SerializeField] private AudioClip notificationSound;
        [SerializeField] private AudioClip errorSound;
        [SerializeField] private AudioClip successSound;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool muteAllAudio = false;
        
        // Audio management
        private Dictionary<string, AudioClip> namedSounds = new Dictionary<string, AudioClip>();
        private Coroutine currentMusicFade;
        private string currentMusicTrack = "";
        private float currentMusicTime = 0f;
        
        // Events
        public static event Action<string> OnSoundPlayed;
        public static event Action<string> OnMusicChanged;
        public static event Action<float> OnVolumeChanged;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<AudioManager>(this);
            
            // Initialize audio sources if not assigned
            InitializeAudioSources();
            
            if (enableDebugLogs)
                Debug.Log("[AudioManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Subscribe to game events
            GameEvents.OnGameStateChanged += OnGameStateChanged;
            GameEvents.OnDecisionMade += OnDecisionMade;
            GameEvents.OnPlaySound += PlaySound;
            GameEvents.OnPlayMusic += PlayMusic;
            GameEvents.OnStopAllSounds += StopAllSounds;
            
            // Load named sounds dictionary
            LoadNamedSounds();
            
            // Apply initial volume settings
            ApplyVolumeSettings();
            
            if (enableDebugLogs)
                Debug.Log("[AudioManager] Audio system ready");
        }
        
        /// <summary>
        /// Initialize audio sources if they're not manually assigned
        /// </summary>
        private void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("Music Source");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFX Source");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            
            if (ambientSource == null)
            {
                GameObject ambientObj = new GameObject("Ambient Source");
                ambientObj.transform.SetParent(transform);
                ambientSource = ambientObj.AddComponent<AudioSource>();
                ambientSource.loop = true;
                ambientSource.playOnAwake = false;
            }
            
            if (uiSource == null)
            {
                GameObject uiObj = new GameObject("UI Source");
                uiObj.transform.SetParent(transform);
                uiSource = uiObj.AddComponent<AudioSource>();
                uiSource.loop = false;
                uiSource.playOnAwake = false;
            }
        }
        
        /// <summary>
        /// Load sound effects into named dictionary for easy access
        /// </summary>
        private void LoadNamedSounds()
        {
            namedSounds.Clear();
            
            if (buttonClickSound != null) namedSounds["button_click"] = buttonClickSound;
            if (shipApprovedSound != null) namedSounds["ship_approved"] = shipApprovedSound;
            if (shipDeniedSound != null) namedSounds["ship_denied"] = shipDeniedSound;
            if (alarmSound != null) namedSounds["alarm"] = alarmSound;
            if (notificationSound != null) namedSounds["notification"] = notificationSound;
            if (errorSound != null) namedSounds["error"] = errorSound;
            if (successSound != null) namedSounds["success"] = successSound;
            
            if (enableDebugLogs)
                Debug.Log($"[AudioManager] Loaded {namedSounds.Count} named sound effects");
        }
        
        /// <summary>
        /// Play a sound effect by name or AudioClip
        /// </summary>
        public void PlaySound(string soundName)
        {
            if (muteAllAudio) return;
            
            if (namedSounds.TryGetValue(soundName.ToLower(), out AudioClip clip))
            {
                PlaySoundClip(clip);
                
                if (enableDebugLogs)
                    Debug.Log($"[AudioManager] Playing sound: {soundName}");
            }
            else
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[AudioManager] Sound not found: {soundName}");
            }
        }
        
        /// <summary>
        /// Play a sound effect from an AudioClip
        /// </summary>
        public void PlaySoundClip(AudioClip clip)
        {
            if (muteAllAudio || clip == null || sfxSource == null) return;
            
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
            OnSoundPlayed?.Invoke(clip.name);
        }
        
        /// <summary>
        /// Play UI sound effect
        /// </summary>
        public void PlayUISound(string soundName)
        {
            if (muteAllAudio) return;
            
            if (namedSounds.TryGetValue(soundName.ToLower(), out AudioClip clip))
            {
                uiSource.PlayOneShot(clip, uiVolume * masterVolume);
                
                if (enableDebugLogs)
                    Debug.Log($"[AudioManager] Playing UI sound: {soundName}");
            }
        }
        
        /// <summary>
        /// Play background music by name or from array
        /// </summary>
        public void PlayMusic(string musicName)
        {
            if (muteAllAudio) return;
            
            AudioClip musicClip = null;
            
            // Check if it's a preset music type
            switch (musicName.ToLower())
            {
                case "gameplay":
                    musicClip = GetRandomTrack(gameplayMusicTracks);
                    break;
                case "menu":
                    musicClip = GetRandomTrack(menuMusicTracks);
                    break;
                default:
                    // Try to find by name in Resources or assigned clips
                    break;
            }
            
            if (musicClip != null)
            {
                PlayMusicClip(musicClip);
            }
            else
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[AudioManager] Music not found: {musicName}");
            }
        }
        
        /// <summary>
        /// Play a specific music clip
        /// </summary>
        public void PlayMusicClip(AudioClip musicClip)
        {
            if (muteAllAudio || musicClip == null || musicSource == null) return;
            
            // Fade out current music if playing
            if (musicSource.isPlaying && currentMusicFade == null)
            {
                currentMusicFade = StartCoroutine(FadeMusicTo(musicClip));
            }
            else
            {
                // Play immediately if no music is playing
                musicSource.clip = musicClip;
                musicSource.loop = loopMusic;
                musicSource.volume = musicVolume * masterVolume;
                musicSource.Play();
                
                currentMusicTrack = musicClip.name;
                OnMusicChanged?.Invoke(currentMusicTrack);
                
                if (enableDebugLogs)
                    Debug.Log($"[AudioManager] Playing music: {musicClip.name}");
            }
        }
        
        /// <summary>
        /// Fade between music tracks
        /// </summary>
        private IEnumerator FadeMusicTo(AudioClip newClip)
        {
            float startVolume = musicSource.volume;
            
            // Fade out current music
            for (float t = 0; t < musicFadeTime / 2; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / (musicFadeTime / 2));
                yield return null;
            }
            
            // Switch to new clip
            musicSource.clip = newClip;
            musicSource.loop = loopMusic;
            musicSource.Play();
            currentMusicTrack = newClip.name;
            
            // Fade in new music
            float targetVolume = musicVolume * masterVolume;
            for (float t = 0; t < musicFadeTime / 2; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(0, targetVolume, t / (musicFadeTime / 2));
                yield return null;
            }
            
            musicSource.volume = targetVolume;
            OnMusicChanged?.Invoke(currentMusicTrack);
            
            if (enableDebugLogs)
                Debug.Log($"[AudioManager] Faded to music: {newClip.name}");
            
            currentMusicFade = null;
        }
        
        /// <summary>
        /// Stop all audio
        /// </summary>
        public void StopAllSounds()
        {
            if (musicSource != null) musicSource.Stop();
            if (sfxSource != null) sfxSource.Stop();
            if (ambientSource != null) ambientSource.Stop();
            if (uiSource != null) uiSource.Stop();
            
            if (currentMusicFade != null)
            {
                StopCoroutine(currentMusicFade);
                currentMusicFade = null;
            }
            
            if (enableDebugLogs)
                Debug.Log("[AudioManager] All audio stopped");
        }
        
        /// <summary>
        /// Pause all audio
        /// </summary>
        public void PauseAllAudio()
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                currentMusicTime = musicSource.time;
                musicSource.Pause();
            }
            if (ambientSource != null && ambientSource.isPlaying) ambientSource.Pause();
            
            if (enableDebugLogs)
                Debug.Log("[AudioManager] All audio paused");
        }
        
        /// <summary>
        /// Resume all audio
        /// </summary>
        public void ResumeAllAudio()
        {
            if (musicSource != null && !musicSource.isPlaying && musicSource.clip != null)
            {
                musicSource.time = currentMusicTime;
                musicSource.UnPause();
            }
            if (ambientSource != null && !ambientSource.isPlaying && ambientSource.clip != null)
            {
                ambientSource.UnPause();
            }
            
            if (enableDebugLogs)
                Debug.Log("[AudioManager] All audio resumed");
        }
        
        /// <summary>
        /// Set master volume
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            OnVolumeChanged?.Invoke(masterVolume);
            
            if (enableDebugLogs)
                Debug.Log($"[AudioManager] Master volume set to {masterVolume:F2}");
        }
        
        /// <summary>
        /// Set music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
                musicSource.volume = musicVolume * masterVolume;
            
            if (enableDebugLogs)
                Debug.Log($"[AudioManager] Music volume set to {musicVolume:F2}");
        }
        
        /// <summary>
        /// Set sound effects volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            
            if (enableDebugLogs)
                Debug.Log($"[AudioManager] SFX volume set to {sfxVolume:F2}");
        }
        
        /// <summary>
        /// Apply volume settings to all audio sources
        /// </summary>
        private void ApplyVolumeSettings()
        {
            if (musicSource != null) musicSource.volume = musicVolume * masterVolume;
            if (ambientSource != null) ambientSource.volume = ambientVolume * masterVolume;
        }
        
        /// <summary>
        /// Get a random track from an array
        /// </summary>
        private AudioClip GetRandomTrack(AudioClip[] tracks)
        {
            if (tracks == null || tracks.Length == 0) return null;
            return tracks[UnityEngine.Random.Range(0, tracks.Length)];
        }
        
        /// <summary>
        /// Handle game state changes
        /// </summary>
        private void OnGameStateChanged(Starkiller.Core.GameState newState)
        {
            switch (newState)
            {
                case Starkiller.Core.GameState.MainMenu:
                    PlayMusic("menu");
                    break;
                    
                case Starkiller.Core.GameState.Gameplay:
                    PlayMusic("gameplay");
                    break;
                    
                case Starkiller.Core.GameState.Paused:
                    PauseAllAudio();
                    break;
                    
                case Starkiller.Core.GameState.GameOver:
                    PlaySound("error");
                    break;
            }
        }
        
        /// <summary>
        /// Handle decision events
        /// </summary>
        private void OnDecisionMade(Starkiller.Core.DecisionType decision, Starkiller.Core.IEncounter encounter)
        {
            switch (decision)
            {
                case Starkiller.Core.DecisionType.Approve:
                    PlaySound("ship_approved");
                    break;
                    
                case Starkiller.Core.DecisionType.Deny:
                    PlaySound("ship_denied");
                    break;
                    
                case Starkiller.Core.DecisionType.TractorBeam:
                    PlaySound("alarm");
                    break;
            }
        }
        
        /// <summary>
        /// Mute or unmute all audio
        /// </summary>
        public void SetAudioMuted(bool muted)
        {
            muteAllAudio = muted;
            
            if (muted)
            {
                StopAllSounds();
            }
            
            if (enableDebugLogs)
                Debug.Log($"[AudioManager] Audio {(muted ? "muted" : "unmuted")}");
        }
        
        /// <summary>
        /// Check if audio is currently muted
        /// </summary>
        public bool IsAudioMuted => muteAllAudio;
        
        /// <summary>
        /// Get current music track name
        /// </summary>
        public string CurrentMusicTrack => currentMusicTrack;
        
        /// <summary>
        /// Check if music is currently playing
        /// </summary>
        public bool IsMusicPlaying => musicSource != null && musicSource.isPlaying;
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            GameEvents.OnGameStateChanged -= OnGameStateChanged;
            GameEvents.OnDecisionMade -= OnDecisionMade;
            GameEvents.OnPlaySound -= PlaySound;
            GameEvents.OnPlayMusic -= PlayMusic;
            GameEvents.OnStopAllSounds -= StopAllSounds;
            
            // Clear event subscriptions
            OnSoundPlayed = null;
            OnMusicChanged = null;
            OnVolumeChanged = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Play Button Click")]
        private void TestButtonClick() => PlaySound("button_click");
        
        [ContextMenu("Test: Play Ship Approved")]
        private void TestShipApproved() => PlaySound("ship_approved");
        
        [ContextMenu("Test: Play Ship Denied")]
        private void TestShipDenied() => PlaySound("ship_denied");
        
        [ContextMenu("Test: Play Gameplay Music")]
        private void TestGameplayMusic() => PlayMusic("gameplay");
        
        [ContextMenu("Test: Play Menu Music")]
        private void TestMenuMusic() => PlayMusic("menu");
        
        [ContextMenu("Test: Stop All Audio")]
        private void TestStopAll() => StopAllSounds();
        
        [ContextMenu("Test: Mute Audio")]
        private void TestMute() => SetAudioMuted(true);
        
        [ContextMenu("Test: Unmute Audio")]
        private void TestUnmute() => SetAudioMuted(false);
        
        [ContextMenu("Show Audio Status")]
        private void ShowAudioStatus()
        {
            Debug.Log("=== AUDIO MANAGER STATUS ===");
            Debug.Log($"Master Volume: {masterVolume:F2}");
            Debug.Log($"Music Volume: {musicVolume:F2}");
            Debug.Log($"SFX Volume: {sfxVolume:F2}");
            Debug.Log($"Audio Muted: {muteAllAudio}");
            Debug.Log($"Current Music: {(string.IsNullOrEmpty(currentMusicTrack) ? "None" : currentMusicTrack)}");
            Debug.Log($"Music Playing: {IsMusicPlaying}");
            Debug.Log($"Named Sounds: {namedSounds.Count}");
            Debug.Log("=== END STATUS ===");
        }
    }
}