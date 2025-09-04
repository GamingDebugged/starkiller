using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Reflection;

/// <summary>
/// Emergency fix for video players and UI elements
/// </summary>
public class EmergencyFix : MonoBehaviour
{
    private bool hasRun = false;
    
    void Start()
    {
        if (!hasRun)
        {
            hasRun = true;
            StartCoroutine(ApplyAllFixes());
        }
    }
    
    IEnumerator ApplyAllFixes()
    {
        Debug.Log("=== EmergencyFix: Starting comprehensive fixes ===");
        
        // Wait for scene initialization
        yield return new WaitForSeconds(0.5f);
        
        // Fix video players first
        FixVideoPlayers();
        
        // Fix UI text elements
        FixUITextElements();
        
        // Fix timing
        FixTiming();
        
        // Fix singleton instance
        FixMasterShipGeneratorInstance();
        
        // Force refresh the current encounter
        RefreshCurrentEncounter();
        
        Debug.Log("=== EmergencyFix: All fixes applied ===");
    }
    
    void FixVideoPlayers()
    {
        Debug.Log("EmergencyFix: Fixing video players...");
        
        // Find all video players in the scene
        VideoPlayer[] allPlayers = FindObjectsByType<VideoPlayer>(FindObjectsInactive.Include, FindObjectsSortMode.None); // Include inactive
        foreach (var player in allPlayers)
        {
            // Enable the GameObject if disabled
            if (!player.gameObject.activeInHierarchy)
            {
                player.gameObject.SetActive(true);
                Debug.Log($"EmergencyFix: Enabled VideoPlayer on {player.gameObject.name}");
            }
            
            // Enable the component if disabled
            if (!player.enabled)
            {
                player.enabled = true;
                Debug.Log($"EmergencyFix: Enabled VideoPlayer component on {player.gameObject.name}");
            }
        }
        
        // Specifically fix CredentialChecker's video players
        CredentialChecker credChecker = FindFirstObjectByType<CredentialChecker>();
        if (credChecker != null)
        {
            // Find child video players
            VideoPlayer[] childPlayers = credChecker.GetComponentsInChildren<VideoPlayer>(true);
            foreach (var player in childPlayers)
            {
                player.gameObject.SetActive(true);
                player.enabled = true;
            }
            
            // Force initialization
            try
            {
                var initMethod = typeof(CredentialChecker).GetMethod("InitializeVideoPlayers", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (initMethod != null)
                {
                    initMethod.Invoke(credChecker, null);
                    Debug.Log("EmergencyFix: Reinitialized CredentialChecker video players");
                }
            }
            catch { }
        }
    }
    
    void FixUITextElements()
    {
        Debug.Log("EmergencyFix: Fixing UI text elements...");
        
        CredentialChecker credChecker = FindFirstObjectByType<CredentialChecker>();
        if (credChecker == null) return;
        
        // Get all text fields via reflection
        var type = typeof(CredentialChecker);
        
        // Common text field names to look for
        string[] textFieldNames = {
            "shipNameText", "captainNameText", "manifestText", "accessCodeText",
            "shipDescriptionText", "destinationText", "originText", "crewSizeText",
            "shipTypeText", "registrationText", "cargoText"
        };
        
        foreach (string fieldName in textFieldNames)
        {
            // Try TextMeshProUGUI first
            var tmpField = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (tmpField != null && tmpField.FieldType == typeof(TextMeshProUGUI))
            {
                TextMeshProUGUI tmp = tmpField.GetValue(credChecker) as TextMeshProUGUI;
                if (tmp == null)
                {
                    // Try to find it in children
                    TextMeshProUGUI[] allTMP = credChecker.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (var textComponent in allTMP)
                    {
                        if (textComponent.name.ToLower().Contains(fieldName.ToLower().Replace("text", "")))
                        {
                            tmpField.SetValue(credChecker, textComponent);
                            Debug.Log($"EmergencyFix: Connected {fieldName} to {textComponent.name}");
                            break;
                        }
                    }
                }
            }
            
            // Try regular Text
            var textField = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (textField != null && textField.FieldType == typeof(Text))
            {
                Text text = textField.GetValue(credChecker) as Text;
                if (text == null)
                {
                    // Try to find it in children
                    Text[] allTexts = credChecker.GetComponentsInChildren<Text>(true);
                    foreach (var textComponent in allTexts)
                    {
                        if (textComponent.name.ToLower().Contains(fieldName.ToLower().Replace("text", "")))
                        {
                            textField.SetValue(credChecker, textComponent);
                            Debug.Log($"EmergencyFix: Connected {fieldName} to {textComponent.name}");
                            break;
                        }
                    }
                }
            }
        }
    }
    
    void FixTiming()
    {
        Debug.Log("EmergencyFix: Fixing timing...");
        
        // Fix GameManager
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.timeBetweenShips = 5f;
            // gm.shiftTimeLimit = 180f; // DISABLED: Keep default 30 seconds
        }
        
        // Fix ShipTimingController
        ShipTimingController timing = FindFirstObjectByType<ShipTimingController>();
        if (timing != null)
        {
            SetPrivateField(timing, "timeBetweenShips", 5f);
            SetPrivateField(timing, "minimumEncounterDuration", 3f);
            SetPrivateField(timing, "encounterStartTime", Time.time); // Reset encounter timer
        }
    }
    
    void FixMasterShipGeneratorInstance()
    {
        Debug.Log("EmergencyFix: Fixing MasterShipGenerator instance...");
        
        MasterShipGenerator masterGen = FindFirstObjectByType<MasterShipGenerator>();
        if (masterGen != null)
        {
            // Ensure it's set as the singleton instance
            var instanceProperty = typeof(MasterShipGenerator).GetProperty("Instance", 
                BindingFlags.Public | BindingFlags.Static);
            if (instanceProperty != null)
            {
                var currentInstance = instanceProperty.GetValue(null) as MasterShipGenerator;
                if (currentInstance == null)
                {
                    // Set instance via reflection
                    var instanceField = typeof(MasterShipGenerator).GetField("instance", 
                        BindingFlags.NonPublic | BindingFlags.Static);
                    if (instanceField != null)
                    {
                        instanceField.SetValue(null, masterGen);
                        Debug.Log("EmergencyFix: Set MasterShipGenerator singleton instance");
                    }
                }
            }
            
            // Prevent destruction
            if (masterGen.transform.parent == null)
            {
                DontDestroyOnLoad(masterGen.gameObject);
            }
        }
    }
    
    void RefreshCurrentEncounter()
    {
        Debug.Log("EmergencyFix: Refreshing current encounter display...");
        
        CredentialChecker credChecker = FindFirstObjectByType<CredentialChecker>();
        if (credChecker != null)
        {
            // Get current encounter
            var currentEncounterField = typeof(CredentialChecker).GetField("currentEncounter", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (currentEncounterField != null)
            {
                var encounter = currentEncounterField.GetValue(credChecker);
                if (encounter != null)
                {
                    // Force redisplay
                    var displayMethod = typeof(CredentialChecker).GetMethod("DisplayEncounter", 
                        BindingFlags.Public | BindingFlags.Instance, null, 
                        new System.Type[] { encounter.GetType() }, null);
                    if (displayMethod != null)
                    {
                        displayMethod.Invoke(credChecker, new object[] { encounter });
                        Debug.Log("EmergencyFix: Forced encounter redisplay");
                    }
                }
            }
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