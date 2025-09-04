using System.Collections;
using System.Collections.Generic;
using System.Text; 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;  
using TMPro;
using StarkillerBaseCommand;

public class MainMenuManager : MonoBehaviour
{
    [Header("Button References")]
    public Button newGameButton;
    public Button loadGameButton;
    public Button levelSelectButton;
    public Button settingsButton;
    
    [Header("Button Sprites")]
    public Sprite newGameNormal;
    public Sprite newGameHover;
    public Sprite loadGameNormal;
    public Sprite loadGameHover;
    public Sprite levelSelectNormal;
    public Sprite levelSelectHover;
    public Sprite settingsNormal;
    public Sprite settingsHover;
    
    [Header("Panel References")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    
    [Header("Audio")]
    public AudioSource buttonHoverSound;
    public AudioSource buttonClickSound;
    
    void Start()
    {
        // Set up button listeners
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);
        
        if (loadGameButton != null)
            loadGameButton.onClick.AddListener(OnLoadGameClicked);
        
        if (levelSelectButton != null)
            levelSelectButton.onClick.AddListener(OnLevelSelectClicked);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        
        // Set up hover events for each button
        SetupButtonHoverEvents(newGameButton, newGameNormal, newGameHover);
        SetupButtonHoverEvents(loadGameButton, loadGameNormal, loadGameHover);
        SetupButtonHoverEvents(levelSelectButton, levelSelectNormal, levelSelectHover);
        SetupButtonHoverEvents(settingsButton, settingsNormal, settingsHover);
        
        // Ensure only main menu is visible at start
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
            
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
    
    // Set up hover events for a button
    private void SetupButtonHoverEvents(Button button, Sprite normalSprite, Sprite hoverSprite)
    {
        if (button == null || normalSprite == null || hoverSprite == null)
            return;
            
        // Get the image component
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage == null)
            return;
            
        // Add event triggers for hover
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();
            
        // Create pointer enter event
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => {
            buttonImage.sprite = hoverSprite;
            if (buttonHoverSound != null)
                buttonHoverSound.Play();
        });
        
        // Create pointer exit event
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            buttonImage.sprite = normalSprite;
        });
        
        // Add the events to the trigger
        trigger.triggers.Add(enterEntry);
        trigger.triggers.Add(exitEntry);
    }
    
    // Button click handlers
    public void OnNewGameClicked()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();
            
        Debug.Log("New Game clicked - Starting new game...");
        // Load the first level or game scene
        // SceneManager.LoadScene("GameScene");
    }
    
    public void OnLoadGameClicked()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();
            
        Debug.Log("Load Game clicked - Opening save games...");
        // Open load game menu or load the saved game
    }
    
    public void OnLevelSelectClicked()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();
            
        Debug.Log("Level Select clicked - Opening level selection...");
        // Open level selection menu
    }
    
    public void OnSettingsClicked()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();
            
        Debug.Log("Settings clicked - Opening settings...");
        
        // Toggle settings panel
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
        }
    }
    
    // Method to return from settings to main menu
    public void OnBackFromSettings()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();
            
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }
}