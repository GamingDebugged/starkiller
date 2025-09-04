using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the settings menu UI
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("Menu Object")]
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider videoVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;
    
    [Header("Mute Toggles")]
    [SerializeField] private Toggle masterMuteToggle;
    [SerializeField] private Toggle musicMuteToggle;
    [SerializeField] private Toggle sfxMuteToggle;
    [SerializeField] private Toggle videoMuteToggle;
    [SerializeField] private Toggle uiMuteToggle;
    
    [Header("Label Text")]
    [SerializeField] private TMP_Text masterVolumeValueText;
    [SerializeField] private TMP_Text musicVolumeValueText;
    [SerializeField] private TMP_Text sfxVolumeValueText;
    [SerializeField] private TMP_Text videoVolumeValueText;
    [SerializeField] private TMP_Text uiVolumeValueText;
    
    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button defaultsButton;
    
    void Start()
    {
        // Set up slider listeners
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            
        if (videoVolumeSlider != null)
            videoVolumeSlider.onValueChanged.AddListener(OnVideoVolumeChanged);
            
        if (uiVolumeSlider != null)
            uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);
            
        // Set up toggle listeners
        if (masterMuteToggle != null)
            masterMuteToggle.onValueChanged.AddListener(OnMasterMuteChanged);
            
        if (musicMuteToggle != null)
            musicMuteToggle.onValueChanged.AddListener(OnMusicMuteChanged);
            
        if (sfxMuteToggle != null)
            sfxMuteToggle.onValueChanged.AddListener(OnSFXMuteChanged);
            
        if (videoMuteToggle != null)
            videoMuteToggle.onValueChanged.AddListener(OnVideoMuteChanged);
            
        if (uiMuteToggle != null)
            uiMuteToggle.onValueChanged.AddListener(OnUIMuteChanged);
            
        // Set up button listeners
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);
            
        if (applyButton != null)
            applyButton.onClick.AddListener(ApplySettings);
            
        if (defaultsButton != null)
            defaultsButton.onClick.AddListener(ResetToDefaults);
            
        // Hide panel initially
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        // Initialize UI with current settings
        LoadSettingsToUI();
    }
    
    /// <summary>
    /// Update UI to reflect current settings
    /// </summary>
    private void LoadSettingsToUI()
    {
        if (AudioManager.Instance == null)
            return;
            
        // Set slider values
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
            
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
            
        if (videoVolumeSlider != null)
            videoVolumeSlider.value = AudioManager.Instance.GetVideoVolume();
            
        if (uiVolumeSlider != null)
            uiVolumeSlider.value = AudioManager.Instance.GetUIVolume();
            
        // Set toggle values
        if (masterMuteToggle != null)
            masterMuteToggle.isOn = AudioManager.Instance.IsMasterMuted();
            
        if (musicMuteToggle != null)
            musicMuteToggle.isOn = AudioManager.Instance.IsMusicMuted();
            
        if (sfxMuteToggle != null)
            sfxMuteToggle.isOn = AudioManager.Instance.IsSFXMuted();
            
        if (videoMuteToggle != null)
            videoMuteToggle.isOn = AudioManager.Instance.IsVideoMuted();
            
        if (uiMuteToggle != null)
            uiMuteToggle.isOn = AudioManager.Instance.IsUIMuted();
            
        // Update text labels
        UpdateVolumeLabels();
    }
    
    /// <summary>
    /// Update text labels with current volume values
    /// </summary>
    private void UpdateVolumeLabels()
    {
        if (masterVolumeValueText != null && masterVolumeSlider != null)
            masterVolumeValueText.text = Mathf.RoundToInt(masterVolumeSlider.value * 100) + "%";
            
        if (musicVolumeValueText != null && musicVolumeSlider != null)
            musicVolumeValueText.text = Mathf.RoundToInt(musicVolumeSlider.value * 100) + "%";
            
        if (sfxVolumeValueText != null && sfxVolumeSlider != null)
            sfxVolumeValueText.text = Mathf.RoundToInt(sfxVolumeSlider.value * 100) + "%";
            
        if (videoVolumeValueText != null && videoVolumeSlider != null)
            videoVolumeValueText.text = Mathf.RoundToInt(videoVolumeSlider.value * 100) + "%";
            
        if (uiVolumeValueText != null && uiVolumeSlider != null)
            uiVolumeValueText.text = Mathf.RoundToInt(uiVolumeSlider.value * 100) + "%";
    }
    
    /// <summary>
    /// Show the settings menu
    /// </summary>
    public void ShowSettings()
    {
        // Reload settings to UI to ensure it's up to date
        LoadSettingsToUI();
        
        // Show the panel
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    /// <summary>
    /// Close the settings menu
    /// </summary>
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
    
    /// <summary>
    /// Apply current settings
    /// </summary>
    public void ApplySettings()
    {
        if (AudioManager.Instance == null)
            return;
            
        // Apply all settings explicitly
        AudioManager.Instance.ApplyAllAudioSettings();
        
        // Close the menu
        CloseSettings();
    }
    
    /// <summary>
    /// Reset to default audio settings
    /// </summary>
    public void ResetToDefaults()
    {
        // Set default values
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = 1.0f;
            
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = 1.0f;
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = 1.0f;
            
        if (videoVolumeSlider != null)
            videoVolumeSlider.value = 1.0f;
            
        if (uiVolumeSlider != null)
            uiVolumeSlider.value = 1.0f;
            
        // Unmute all channels
        if (masterMuteToggle != null)
            masterMuteToggle.isOn = false;
            
        if (musicMuteToggle != null)
            musicMuteToggle.isOn = false;
            
        if (sfxMuteToggle != null)
            sfxMuteToggle.isOn = false;
            
        if (videoMuteToggle != null)
            videoMuteToggle.isOn = false;
            
        if (uiMuteToggle != null)
            uiMuteToggle.isOn = false;
            
        // Update labels
        UpdateVolumeLabels();
    }
    
    // SLIDER EVENT HANDLERS
    
    private void OnMasterVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(value);
            
        UpdateVolumeLabels();
    }
    
    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
            
        UpdateVolumeLabels();
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
            
        UpdateVolumeLabels();
    }
    
    private void OnVideoVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetVideoVolume(value);
            
        UpdateVolumeLabels();
    }
    
    private void OnUIVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetUIVolume(value);
            
        UpdateVolumeLabels();
    }
    
    // TOGGLE EVENT HANDLERS
    
    private void OnMasterMuteChanged(bool isOn)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.IsMasterMuted() != isOn)
            AudioManager.Instance.ToggleMasterMute();
    }
    
    private void OnMusicMuteChanged(bool isOn)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.IsMusicMuted() != isOn)
            AudioManager.Instance.ToggleMusicMute();
    }
    
    private void OnSFXMuteChanged(bool isOn)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.IsSFXMuted() != isOn)
            AudioManager.Instance.ToggleSFXMute();
    }
    
    private void OnVideoMuteChanged(bool isOn)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.IsVideoMuted() != isOn)
            AudioManager.Instance.ToggleVideoMute();
    }
    
    private void OnUIMuteChanged(bool isOn)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.IsUIMuted() != isOn)
            AudioManager.Instance.ToggleUIMute();
    }
}