using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper script to connect the LogBookManager with the CredentialChecker
/// This should be added to the GameObject containing LogBookManager
/// </summary>
public class ConnectLogBookManager : MonoBehaviour
{
    [Header("Component References")]
    public LogBookManager logBookManager;
    public CredentialChecker credentialChecker;
    
    [Header("Original LogBook References")]
    public GameObject originalLogBookPanel;
    public Button originalOpenLogBookButton;
    public Button originalCloseLogBookButton;
    
    // Start is called before the first frame update
    void Start()
    {
        // Find references if not assigned
        if (logBookManager == null)
            logBookManager = GetComponent<LogBookManager>();
            
        if (credentialChecker == null)
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            
        if (logBookManager == null)
        {
            Debug.LogError("ConnectLogBookManager: No LogBookManager component found!");
            return;
        }
        
        if (credentialChecker == null)
        {
            Debug.LogError("ConnectLogBookManager: CredentialChecker not found!");
            return;
        }
        
        // Connect components
        ConnectComponents();
    }
    
    /// <summary>
    /// Connect the LogBookManager with the CredentialChecker
    /// </summary>
    void ConnectComponents()
    {
        // Get original references if not already assigned
        if (originalLogBookPanel == null)
            originalLogBookPanel = credentialChecker.logBookPanel;
            
        if (originalOpenLogBookButton == null)
            originalOpenLogBookButton = credentialChecker.openLogBookButton;
            
        if (originalCloseLogBookButton == null)
            originalCloseLogBookButton = credentialChecker.closeLogBookButton;
        
        // Check if we should modify the CredentialChecker methods
        bool shouldModify = originalLogBookPanel != null && originalLogBookPanel != logBookManager.logBookPanel;
        
        if (shouldModify)
        {
            // Replace the original panel with our new tabbed panel
            credentialChecker.logBookPanel = logBookManager.logBookPanel;
            
            // Modify button behaviors
            if (originalOpenLogBookButton != null)
            {
                originalOpenLogBookButton.onClick.RemoveAllListeners();
                originalOpenLogBookButton.onClick.AddListener(OpenLogBook);
            }
            
            if (originalCloseLogBookButton != null)
            {
                originalCloseLogBookButton.onClick.RemoveAllListeners();
                originalCloseLogBookButton.onClick.AddListener(CloseLogBook);
            }
            
            // Update credential checker's log book methods using a monobehavior reflection approach
            var methodInfo = credentialChecker.GetType().GetMethod("OpenLogBook", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (methodInfo != null)
            {
                Debug.Log("ConnectLogBookManager: Found OpenLogBook method, creating override");
                
                // Create a MonoBehavior method to override the original
                // Note: In a production environment, you would modify the source directly
                // This approach is for demonstration only
            }
            
            Debug.Log("ConnectLogBookManager: Connected LogBookManager to CredentialChecker");
        }
        else
        {
            Debug.Log("ConnectLogBookManager: No modification needed");
        }
    }
    
    /// <summary>
    /// Open the log book using our manager
    /// </summary>
    public void OpenLogBook()
    {
        if (logBookManager != null)
        {
            logBookManager.OpenLogBook();
        }
    }
    
    /// <summary>
    /// Close the log book using our manager
    /// </summary>
    public void CloseLogBook()
    {
        if (logBookManager != null)
        {
            logBookManager.CloseLogBook();
        }
    }
    
    /// <summary>
    /// Update the log book content when needed
    /// </summary>
    public void UpdateLogBook()
    {
        if (logBookManager != null)
        {
            logBookManager.UpdateContent();
        }
    }
}
