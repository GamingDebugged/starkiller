using UnityEngine;
using UnityEditor;
using TMPro;

/// <summary>
/// Quick UI Reference Fix - Automatically runs when Unity compiles
/// Fixes common UI references that get lost during scene changes
/// </summary>
[InitializeOnLoad]
public class QuickUIFix
{
    static QuickUIFix()
    {
        // This runs every time Unity compiles
        EditorApplication.delayCall += RunUIValidation;
    }
    
    private static void RunUIValidation()
    {
        // Only run in play mode or if specifically requested
        if (EditorApplication.isPlaying) return;
        
        // Check if we have the common missing references issue
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            var serializedObject = new SerializedObject(gameManager);
            var gameStatusTextProp = serializedObject.FindProperty("gameStatusText");
            var gameTimerTextProp = serializedObject.FindProperty("gameTimerText");
            
            bool needsFix = (gameStatusTextProp != null && gameStatusTextProp.objectReferenceValue == null) ||
                           (gameTimerTextProp != null && gameTimerTextProp.objectReferenceValue == null);
            
            if (needsFix)
            {
                Debug.LogWarning("[QuickUIFix] Detected missing UI references in GameManager. Use Starkiller > Validate UI References to fix automatically.");
            }
        }
    }
    
    [MenuItem("Starkiller/Quick Fix UI References")]
    public static void QuickFixUIReferences()
    {
        Debug.Log("[QuickUIFix] Starting automatic UI reference repair...");
        
        int referencesFixed = 0;
        
        // Fix GameManager references
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            referencesFixed += FixGameManagerReferences(gameManager);
        }
        
        // Fix ShiftTimerManager references
        var shiftTimerManager = Object.FindFirstObjectByType<Starkiller.Core.Managers.ShiftTimerManager>();
        if (shiftTimerManager != null)
        {
            referencesFixed += FixShiftTimerManagerReferences(shiftTimerManager);
        }
        
        if (referencesFixed > 0)
        {
            Debug.Log($"[QuickUIFix] ✅ Fixed {referencesFixed} UI references!");
            EditorUtility.DisplayDialog("UI Fix Complete", 
                $"Successfully reconnected {referencesFixed} UI references!\n\nHUD text should now update properly.", 
                "OK");
        }
        else
        {
            Debug.Log("[QuickUIFix] All UI references are already properly connected.");
            EditorUtility.DisplayDialog("UI References OK", 
                "All UI references are already properly connected.", 
                "OK");
        }
    }
    
    private static int FixGameManagerReferences(GameManager gameManager)
    {
        SerializedObject serializedObject = new SerializedObject(gameManager);
        int fixedCount = 0;
        
        // Fix gameStatusText
        var gameStatusTextProp = serializedObject.FindProperty("gameStatusText");
        if (gameStatusTextProp != null && gameStatusTextProp.objectReferenceValue == null)
        {
            TMP_Text statusText = FindUIText("status", "hud", "game");
            if (statusText != null)
            {
                gameStatusTextProp.objectReferenceValue = statusText;
                fixedCount++;
                Debug.Log($"[QuickUIFix] Fixed gameStatusText → {statusText.name}");
            }
        }
        
        // Fix gameTimerText
        var gameTimerTextProp = serializedObject.FindProperty("gameTimerText");
        if (gameTimerTextProp != null && gameTimerTextProp.objectReferenceValue == null)
        {
            TMP_Text timerText = FindUIText("timer", "time", "shift");
            if (timerText != null)
            {
                gameTimerTextProp.objectReferenceValue = timerText;
                fixedCount++;
                Debug.Log($"[QuickUIFix] Fixed gameTimerText → {timerText.name}");
            }
        }
        
        if (fixedCount > 0)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(gameManager);
        }
        
        return fixedCount;
    }
    
    private static int FixShiftTimerManagerReferences(Starkiller.Core.Managers.ShiftTimerManager shiftTimerManager)
    {
        SerializedObject serializedObject = new SerializedObject(shiftTimerManager);
        int fixedCount = 0;
        
        var gameTimerTextProp = serializedObject.FindProperty("gameTimerText");
        if (gameTimerTextProp != null && gameTimerTextProp.objectReferenceValue == null)
        {
            TMP_Text timerText = FindUIText("timer", "time", "shift");
            if (timerText != null)
            {
                gameTimerTextProp.objectReferenceValue = timerText;
                fixedCount++;
                Debug.Log($"[QuickUIFix] Fixed ShiftTimerManager gameTimerText → {timerText.name}");
            }
        }
        
        if (fixedCount > 0)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(shiftTimerManager);
        }
        
        return fixedCount;
    }
    
    private static TMP_Text FindUIText(params string[] searchTerms)
    {
        TMP_Text[] allTexts = Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        
        foreach (TMP_Text text in allTexts)
        {
            string name = text.gameObject.name.ToLower();
            foreach (string term in searchTerms)
            {
                if (name.Contains(term.ToLower()))
                {
                    return text;
                }
            }
        }
        
        return null;
    }
}