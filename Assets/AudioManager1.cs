using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using StarkillerBaseCommand;

/// <summary>
/// Manages all audio settings and controls for the game
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Volume Settings")]
    [SerializeField] [Range(0f, 1f)] private float masterVolume = 1.0f;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 1.0f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1.0f;
    [SerializeField] [Range(0f, 1f)] private float videoVolume = 1.0f;
    [SerializeField] [Range(0f, 1f)] private float uiVolume = 1.0f;
    
    [Header("Mute Settings")]
    [SerializeField] private bool masterMuted = false;
    [SerializeField] private bool musicMuted = false;
    [SerializeField] private bool sfxMuted = false;
    [SerializeField] private bool videoMuted = false;
    [SerializeField] private bool uiMuted = false;
    
    // Mixer parameter names
    private const string MasterVolumeParam = "MasterVolume";
    private const string MusicVolumeParam = "MusicVolume";
    private const string SFXVolumeParam = "SFXVolume";
    private const string UIVolumeParam = "UIVolume";
    
    // Singleton pattern
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;
    
    // Track active video players
    private List<VideoPlayer> activeVideoPlayers = new List<VideoPlayer>();
    
    void Awake()
    {
        // Setup singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Don't destroy between scenes
        DontDestroyOnLoad(gameObject);
        
        // Load saved settings
        LoadAudioSettings();
        
        // Apply initial settings
        ApplyAllAudioSettings();
    }
    
    /// <summary>
    /// Load audio settings from PlayerPrefs
    /// </summary>
    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        videoVolume = PlayerPrefs.GetFloat("VideoVolume", 1.0f);
        uiVolume = PlayerPrefs.GetFloat("UIVolume", 1.0f);
        
        masterMuted = PlayerPrefs.GetInt("MasterMuted", 0) == 1;
        musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        sfxMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;
        videoMuted = PlayerPrefs.GetInt("VideoMuted", 0) == 1;
        uiMuted = PlayerPrefs.GetInt("UIMuted", 0) == 1;
    }
    
    /// <summary>
    /// Save current audio settings to PlayerPrefs
    /// </summary>
    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("VideoVolume", videoVolume);
        PlayerPrefs.SetFloat("UIVolume", uiVolume);
        
        PlayerPrefs.SetInt("MasterMuted", masterMuted ? 1 : 0);
        PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);
        PlayerPrefs.SetInt("SFXMuted", sfxMuted ? 1 : 0);
        PlayerPrefs.SetInt("VideoMuted", videoMuted ? 1 : 0);
        PlayerPrefs.SetInt("UIMuted", uiMuted ? 1 : 0);
        
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Apply all audio settings
    /// </summary>
    public void ApplyAllAudioSettings()
    {
        // Apply to audio mixer (converting to decibels)
        if (audioMixer != null)
        {
            float masterVolumeDB = masterMuted ? -80f : Mathf.Log10(Mathf.Max(0.0001f, masterVolume)) * 20f;
            float musicVolumeDB = musicMuted ? -80f : Mathf.Log10(Mathf.Max(0.0001f, musicVolume)) * 20f;
            float sfxVolumeDB = sfxMuted ? -80f : Mathf.Log10(Mathf.Max(0.0001f, sfxVolume)) * 20f;
            float uiVolumeDB = uiMuted ? -80f : Mathf.Log10(Mathf.Max(0.0001f, uiVolume)) * 20f;
            
            audioMixer.SetFloat(MasterVolumeParam, masterVolumeDB);
            audioMixer.SetFloat(MusicVolumeParam, musicVolumeDB);
            audioMixer.SetFloat(SFXVolumeParam, sfxVolumeDB);
            audioMixer.SetFloat(UIVolumeParam, uiVolumeDB);
        }
        
        // Update all video players
        UpdateAllVideoPlayers();
        
        // Save settings
        SaveAudioSettings();
    }
    
    /// <summary>
    /// Register a video player to be managed by the audio system
    /// </summary>
    public void RegisterVideoPlayer(VideoPlayer videoPlayer)
    {
        if (videoPlayer != null && !activeVideoPlayers.Contains(videoPlayer))
        {
            activeVideoPlayers.Add(videoPlayer);
            UpdateVideoPlayerSettings(videoPlayer);
        }
    }
    
    /// <summary>
    /// Unregister a video player
    /// </summary>
    public void UnregisterVideoPlayer(VideoPlayer videoPlayer)
    {
        if (videoPlayer != null)
        {
            activeVideoPlayers.Remove(videoPlayer);
        }
    }
    
    /// <summary>
    /// Update audio settings for all video players
    /// </summary>
    private void UpdateAllVideoPlayers()
    {
        // Remove any null references
        activeVideoPlayers.RemoveAll(player => player == null);
        
        // Update each video player
        foreach (var player in activeVideoPlayers)
        {
            UpdateVideoPlayerSettings(player);
        }
    }
    
    /// <summary>
    /// Update audio settings for a specific video player
    /// </summary>
    private void UpdateVideoPlayerSettings(VideoPlayer player)
    {
        if (player == null || player.controlledAudioTrackCount <= 0)
            return;
            
        // Calculate effective volume (master & video volume combined)
        float effectiveVolume = masterMuted || videoMuted ? 0f : (masterVolume * videoVolume);
        player.SetDirectAudioVolume(0, effectiveVolume);
        player.SetDirectAudioMute(0, masterMuted || videoMuted);
    }
    
    // PUBLIC SETTERS
    
    /// <summary>
    /// Set master volume level
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyAllAudioSettings();
    }
    
    /// <summary>
    /// Set music volume level
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyAllAudioSettings();
    }
    
    /// <summary>
    /// Set SFX volume level
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyAllAudioSettings();
    }
    
    /// <summary>
    /// Set video volume level
    /// </summary>
    public void SetVideoVolume(float volume)
    {
        videoVolume = Mathf.Clamp01(volume);
        ApplyAllAudioSettings();
    }
    
    /// <summary>
    /// Set UI volume level
    /// </summary>
    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
        ApplyAllAudioSettings();
    }
    
    /// <summary>
    /// Toggle master audio mute state
    /// </summary>
    public void ToggleMasterMute()
    {
        masterMuted = !masterMuted;
        ApplyAllAudioSettings();
    }
    
    /// <summary>
    /// Toggle music mute state
    /// </summary>
    public void ToggleMusicMute()
    {
        musicMuted = !musicMuted;
        ApplyAllAudioSettings();
    }
    
    /// <summary>
    /// Toggle SFX mute state
    /// </summary>
    public void ToggleSFXMute()
    {
        sfxMuted = !sfxMuted;
        ApplyAllAudioSettings();
    }
    
    /// <summary>
    /// Toggle video audio mute state
    /// </summary>
    public void ToggleVideoMute()
    {
        videoMuted = !videoMuted;
        ApplyAllAudioSettings();
    }
    
    /// <summary>
    /// Toggle UI audio mute state
    /// </summary>
    public void ToggleUIMute()
    {
        uiMuted = !uiMuted;
        ApplyAllAudioSettings();
    }
    
    // PUBLIC GETTERS
    
    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
    public float GetVideoVolume() => videoVolume;
    public float GetUIVolume() => uiVolume;
    
    public bool IsMasterMuted() => masterMuted;
    public bool IsMusicMuted() => musicMuted;
    public bool IsSFXMuted() => sfxMuted;
    public bool IsVideoMuted() => videoMuted;
    public bool IsUIMuted() => uiMuted;
}