using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This script handles UI navigation and transitions
public class UIManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;       // Main menu panel
    public GameObject gameplayPanel;       // Main gameplay panel
    public GameObject gameOverPanel;       // Game over panel
    public GameObject creditsPanel;        // Credits panel
    public GameObject tutorialPanel;       // Tutorial panel
    public GameObject dailyReportPanel;    // Daily report panel
    
    [Header("Menu Buttons")]
    public Button startGameButton;         // Button to start game
    public Button menuTutorialButton;      // Button to open tutorial
    public Button menuCreditsButton;       // Button to show credits
    public Button backToMenuButton;        // Button to return to main menu
    public Button quitGameButton;          // Button to quit game
    
    [Header("Audio")]
    public AudioSource buttonClickSound;   // Sound when button is clicked
    
    // Game manager reference
    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        // Find GameManager reference - using FindFirstObjectByType instead of FindObjectOfType (deprecated)
        gameManager = FindFirstObjectByType<GameManager>();
        
        // Set up button listeners
        if (startGameButton) startGameButton.onClick.AddListener(StartGame);
        if (menuTutorialButton) menuTutorialButton.onClick.AddListener(ShowTutorialPanel);
        if (menuCreditsButton) menuCreditsButton.onClick.AddListener(ShowCreditsPanel);
        if (backToMenuButton) backToMenuButton.onClick.AddListener(BackToMenu);
        if (quitGameButton) quitGameButton.onClick.AddListener(QuitGame);
        
        // Show main menu initially
        ShowPanel(mainMenuPanel);
        HidePanel(gameplayPanel);
        HidePanel(gameOverPanel);
        HidePanel(creditsPanel);
        HidePanel(tutorialPanel);
        if (dailyReportPanel) HidePanel(dailyReportPanel);
    }
    
    // Show the specified panel and hide others
    // Changed from private to public for testing
    public void ShowPanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(true);
    }
    
    // Hide the specified panel
    // Changed from private to public for testing
    public void HidePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }
    
    // Start the game - public for testing
    public void StartGame()
    {
        PlayButtonSound();
        
        // Show gameplay panel
        ShowPanel(gameplayPanel);
        HidePanel(mainMenuPanel);
        HidePanel(gameOverPanel);
        HidePanel(creditsPanel);
        HidePanel(tutorialPanel);
        if (dailyReportPanel) HidePanel(dailyReportPanel);
        
        // Start the game via GameManager
        if (gameManager != null)
            gameManager.StartGame();
    }
    
    // Show the tutorial panel - public for testing
    public void ShowTutorialPanel()
    {
        PlayButtonSound();
        
        ShowPanel(tutorialPanel);
        HidePanel(mainMenuPanel);
        HidePanel(gameplayPanel);
        HidePanel(gameOverPanel);
        HidePanel(creditsPanel);
        if (dailyReportPanel) HidePanel(dailyReportPanel);
    }
    
    // Show the credits panel - public for testing
    public void ShowCreditsPanel()
    {
        PlayButtonSound();
        
        ShowPanel(creditsPanel);
        HidePanel(mainMenuPanel);
        HidePanel(gameplayPanel);
        HidePanel(gameOverPanel);
        HidePanel(tutorialPanel);
        if (dailyReportPanel) HidePanel(dailyReportPanel);
    }
    
    // Return to the main menu - public for testing
    public void ShowMainMenu()
    {
        PlayButtonSound();
        
        ShowPanel(mainMenuPanel);
        HidePanel(gameplayPanel);
        HidePanel(gameOverPanel);
        HidePanel(creditsPanel);
        HidePanel(tutorialPanel);
        if (dailyReportPanel) HidePanel(dailyReportPanel);
    }
    
    // Called when the game is over - show game over panel
    public void GameOver()
    {
        ShowPanel(gameOverPanel);
    }
    
    // Return to the main menu - original method
    void BackToMenu()
    {
        ShowMainMenu();
    }
    
    // Quit the game
    void QuitGame()
    {
        PlayButtonSound();
        
        #if UNITY_EDITOR
        // In editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // In build, quit application
        Application.Quit();
        #endif
    }
    
    // Play button click sound
    void PlayButtonSound()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();
    }
    
    // Method for testing - shows the daily report panel
    public void ShowDailyReportForTesting()
    {
        PlayButtonSound();
        
        if (dailyReportPanel)
        {
            ShowPanel(dailyReportPanel);
            HidePanel(mainMenuPanel);
            HidePanel(gameplayPanel);
            HidePanel(gameOverPanel);
            HidePanel(creditsPanel);
            HidePanel(tutorialPanel);
            
            Debug.Log("Showing Daily Report Panel for testing");
        }
        else
        {
            Debug.LogWarning("Daily Report Panel not assigned in UIManager");
        }
    }
}