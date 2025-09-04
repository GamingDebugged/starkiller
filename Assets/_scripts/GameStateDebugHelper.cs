using UnityEngine;
using System.Collections;

/// <summary>
/// Debug helper script to force the game into the correct state for testing
/// Add this to a GameObject in the MainGame scene
/// </summary>
public class GameStateDebugHelper : MonoBehaviour
{
    [Tooltip("Auto-force active gameplay mode on start")]
    public bool autoForceActiveGameplay = true;
    
    [Tooltip("Delay before forcing gameplay state")]
    public float autoForceDelay = 1.5f;
    
    [Tooltip("Log debug messages")]
    public bool debugLog = true;
    
    void Start()
    {
        if (autoForceActiveGameplay)
        {
            StartCoroutine(ForceActiveGameplayAfterDelay());
        }
    }
    
    private IEnumerator ForceActiveGameplayAfterDelay()
    {
        // Wait for other systems to initialize
        yield return new WaitForSeconds(autoForceDelay);
        
        LogMessage("Forcing active gameplay state");
        
        // Force game state
        GameStateController controller = GameStateController.Instance;
        if (controller != null)
        {
            controller.SetGameState(GameStateController.GameActivationState.ActiveGameplay);
            LogMessage("Set game state to ActiveGameplay");
        }
        
        // Force UI visibility
        CredentialChecker credentialChecker = FindFirstObjectByType<CredentialChecker>();
        if (credentialChecker != null)
        {
            credentialChecker.ForceUIVisibility();
            LogMessage("Forced UI visibility via CredentialChecker");
        }
        
        // Ensure UIManager has gameplay panel active
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null && uiManager.gameplayPanel != null)
        {
            uiManager.gameplayPanel.SetActive(true);
            LogMessage("Set gameplay panel active via UIManager");
            
            // Check for any canvas group and make visible
            CanvasGroup canvasGroup = uiManager.gameplayPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                LogMessage("Set gameplayPanel canvas group alpha to 1");
            }
        }
        
        // Reset any media transition managers
        EncounterMediaTransitionManager mediaManager = FindFirstObjectByType<EncounterMediaTransitionManager>();
        if (mediaManager != null)
        {
            // Call any reset methods it might have
            LogMessage("Found EncounterMediaTransitionManager - UI should now be visible");
        }
        
        // Force a new encounter
        MasterShipGenerator generator = FindFirstObjectByType<MasterShipGenerator>();
        if (generator != null)
        {
            generator.GetNextEncounter();
            LogMessage("Requested next encounter");
        }
        
        LogMessage("GameStateDebugHelper: Forced active gameplay state complete");
    }
    
    void LogMessage(string message)
    {
        if (debugLog)
        {
            Debug.Log($"[GameStateDebugHelper] {message}");
        }
    }
}