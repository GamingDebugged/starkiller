using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Unity Editor script to validate and reconnect UI references that may have been lost
/// Specifically targets gameStatusText and gameTimerText references in GameManager
/// </summary>
public class UIReferenceValidator : EditorWindow
{
    [MenuItem("Starkiller/Validate UI References")]
    public static void ShowWindow()
    {
        GetWindow<UIReferenceValidator>("UI Reference Validator");
    }

    private void OnGUI()
    {
        GUILayout.Label("UI Reference Validator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("This tool will attempt to automatically reconnect lost UI references in:");
        GUILayout.Label("• GameManager (gameStatusText, gameTimerText)");
        GUILayout.Label("• ShiftTimerManager (gameTimerText)");
        GUILayout.Space(10);

        if (GUILayout.Button("Validate and Fix UI References"))
        {
            ValidateUIReferences();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Debug Current UI State"))
        {
            DebugCurrentUIState();
        }
    }

    private void ValidateUIReferences()
    {
        Debug.Log("[UIReferenceValidator] Starting UI reference validation...");

        // Find GameManager
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            Debug.Log("[UIReferenceValidator] Found GameManager, checking references...");
            FixGameManagerReferences(gameManager);
        }
        else
        {
            Debug.LogError("[UIReferenceValidator] GameManager not found in scene!");
        }

        // Find ShiftTimerManager
        var shiftTimerManager = FindFirstObjectByType<Starkiller.Core.Managers.ShiftTimerManager>();
        if (shiftTimerManager != null)
        {
            Debug.Log("[UIReferenceValidator] Found ShiftTimerManager, checking references...");
            FixShiftTimerManagerReferences(shiftTimerManager);
        }
        else
        {
            Debug.LogWarning("[UIReferenceValidator] ShiftTimerManager not found in scene");
        }

        Debug.Log("[UIReferenceValidator] UI reference validation complete!");
    }

    private void FixGameManagerReferences(GameManager gameManager)
    {
        // Use SerializedObject for proper editor modification
        SerializedObject serializedObject = new SerializedObject(gameManager);
        
        bool needsUpdate = false;

        // Check gameStatusText
        SerializedProperty gameStatusTextProp = serializedObject.FindProperty("gameStatusText");
        if (gameStatusTextProp != null && gameStatusTextProp.objectReferenceValue == null)
        {
            TMP_Text statusText = FindUIComponent<TMP_Text>("gameStatusText", "status", "hud");
            if (statusText != null)
            {
                gameStatusTextProp.objectReferenceValue = statusText;
                needsUpdate = true;
                Debug.Log($"[UIReferenceValidator] Reconnected gameStatusText to: {statusText.name}");
            }
            else
            {
                Debug.LogWarning("[UIReferenceValidator] Could not find suitable gameStatusText component");
            }
        }

        // Check gameTimerText
        SerializedProperty gameTimerTextProp = serializedObject.FindProperty("gameTimerText");
        if (gameTimerTextProp != null && gameTimerTextProp.objectReferenceValue == null)
        {
            TMP_Text timerText = FindUIComponent<TMP_Text>("gameTimerText", "timer", "time");
            if (timerText != null)
            {
                gameTimerTextProp.objectReferenceValue = timerText;
                needsUpdate = true;
                Debug.Log($"[UIReferenceValidator] Reconnected gameTimerText to: {timerText.name}");
            }
            else
            {
                Debug.LogWarning("[UIReferenceValidator] Could not find suitable gameTimerText component");
            }
        }

        if (needsUpdate)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(gameManager);
            Debug.Log("[UIReferenceValidator] GameManager references updated and marked dirty");
        }
        else
        {
            Debug.Log("[UIReferenceValidator] GameManager references are already properly assigned");
        }
    }

    private void FixShiftTimerManagerReferences(Starkiller.Core.Managers.ShiftTimerManager shiftTimerManager)
    {
        SerializedObject serializedObject = new SerializedObject(shiftTimerManager);
        
        bool needsUpdate = false;

        // Check gameTimerText
        SerializedProperty gameTimerTextProp = serializedObject.FindProperty("gameTimerText");
        if (gameTimerTextProp != null && gameTimerTextProp.objectReferenceValue == null)
        {
            TMP_Text timerText = FindUIComponent<TMP_Text>("gameTimerText", "timer", "time");
            if (timerText != null)
            {
                gameTimerTextProp.objectReferenceValue = timerText;
                needsUpdate = true;
                Debug.Log($"[UIReferenceValidator] Reconnected ShiftTimerManager gameTimerText to: {timerText.name}");
            }
        }

        if (needsUpdate)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(shiftTimerManager);
            Debug.Log("[UIReferenceValidator] ShiftTimerManager references updated and marked dirty");
        }
    }

    private T FindUIComponent<T>(params string[] searchTerms) where T : Component
    {
        // Find all components of type T in the scene
        T[] allComponents = FindObjectsByType<T>(FindObjectsSortMode.None);
        
        foreach (T component in allComponents)
        {
            string objectName = component.gameObject.name.ToLower();
            
            // Check if any search term matches the object name
            foreach (string term in searchTerms)
            {
                if (objectName.Contains(term.ToLower()))
                {
                    return component;
                }
            }
        }

        return null;
    }

    private void DebugCurrentUIState()
    {
        Debug.Log("=== UI STATE DEBUG ===");

        // Find and debug GameManager
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            SerializedObject serializedObject = new SerializedObject(gameManager);
            
            var gameStatusTextProp = serializedObject.FindProperty("gameStatusText");
            var gameTimerTextProp = serializedObject.FindProperty("gameTimerText");
            
            Debug.Log($"GameManager.gameStatusText: {(gameStatusTextProp?.objectReferenceValue != null ? gameStatusTextProp.objectReferenceValue.name : "NULL")}");
            Debug.Log($"GameManager.gameTimerText: {(gameTimerTextProp?.objectReferenceValue != null ? gameTimerTextProp.objectReferenceValue.name : "NULL")}");
        }

        // Find all TMP_Text components in scene
        TMP_Text[] allTextComponents = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        Debug.Log($"Found {allTextComponents.Length} TMP_Text components in scene:");
        
        foreach (TMP_Text textComp in allTextComponents)
        {
            Debug.Log($"  - {textComp.gameObject.name} (Text: '{textComp.text}')");
        }

        Debug.Log("=== END UI DEBUG ===");
    }
}