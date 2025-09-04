using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Reflection;

/// <summary>
/// Quick fix for immediate issues
/// </summary>
public class QuickFix : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ApplyFixes());
    }
    
    IEnumerator ApplyFixes()
    {
        Debug.Log("=== QuickFix: Starting ===");
        
        // Wait for scene to load
        yield return new WaitForSeconds(0.5f);
        
        // Fix ShipTransitionController video player
        FixShipTransitionController();
        
        // Fix timing issues
        FixTimingIssues();
        
        // Fix references
        FixReferences();
        
        Debug.Log("=== QuickFix: Complete ===");
    }
    
    void FixShipTransitionController()
    {
        Debug.Log("QuickFix: Fixing ShipTransitionController...");
        
        ShipTransitionController[] controllers = FindObjectsByType<ShipTransitionController>(FindObjectsSortMode.None);
        foreach (var controller in controllers)
        {
            // Check if transitionVideoPlayer is null
            var field = typeof(ShipTransitionController).GetField("transitionVideoPlayer", 
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            if (field != null)
            {
                VideoPlayer currentPlayer = field.GetValue(controller) as VideoPlayer;
                if (currentPlayer == null)
                {
                    // Try to find a VideoPlayer component on the same GameObject
                    VideoPlayer player = controller.GetComponent<VideoPlayer>();
                    if (player != null)
                    {
                        field.SetValue(controller, player);
                        Debug.Log("QuickFix: Fixed ShipTransitionController video player reference");
                    }
                    else
                    {
                        // Create a new VideoPlayer component
                        player = controller.gameObject.AddComponent<VideoPlayer>();
                        field.SetValue(controller, player);
                        Debug.Log("QuickFix: Created new VideoPlayer for ShipTransitionController");
                    }
                }
            }
        }
    }
    
    void FixTimingIssues()
    {
        Debug.Log("QuickFix: Fixing timing issues...");
        
        // Fix GameManager timing
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.timeBetweenShips = 5f;
            // gm.shiftTimeLimit = 180f; // DISABLED: Keep default 30 seconds
            Debug.Log("QuickFix: Fixed GameManager timing");
        }
        
        // Fix ShipTimingController
        ShipTimingController timing = FindFirstObjectByType<ShipTimingController>();
        if (timing != null)
        {
            try
            {
                var type = typeof(ShipTimingController);
                
                // Set timeBetweenShips
                SetPrivateField(timing, "timeBetweenShips", 5f);
                
                // Set minimumEncounterDuration  
                SetPrivateField(timing, "minimumEncounterDuration", 3f);
                
                // Set holdingPatternCheckInterval
                SetPrivateField(timing, "holdingPatternCheckInterval", 1f);
                
                Debug.Log("QuickFix: Fixed ShipTimingController");
            }
            catch { }
        }
    }
    
    void FixReferences()
    {
        Debug.Log("QuickFix: Fixing references...");
        
        MasterShipGenerator masterGen = FindFirstObjectByType<MasterShipGenerator>();
        if (masterGen == null)
        {
            Debug.LogError("QuickFix: MasterShipGenerator not found!");
            return;
        }
        
        // Fix GameManager
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null && gm.masterShipGenerator == null)
        {
            gm.masterShipGenerator = masterGen;
            Debug.Log("QuickFix: Fixed GameManager reference");
        }
        
        // Fix DebugMonitor
        DebugMonitor debug = FindFirstObjectByType<DebugMonitor>();
        if (debug != null)
        {
            SetPrivateField(debug, "masterShipGenerator", masterGen);
            Debug.Log("QuickFix: Fixed DebugMonitor reference");
        }
        
        // Fix CredentialChecker
        CredentialChecker cred = FindFirstObjectByType<CredentialChecker>();
        if (cred != null)
        {
            SetPrivateField(cred, "masterShipGenerator", masterGen);
            Debug.Log("QuickFix: Fixed CredentialChecker reference");
        }
    }
    
    void SetPrivateField(object obj, string fieldName, object value)
    {
        var type = obj.GetType();
        var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }
}