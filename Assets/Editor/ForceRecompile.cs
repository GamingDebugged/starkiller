using UnityEngine;
using UnityEditor;

/// <summary>
/// Emergency script to force Unity to recompile and clear cached errors
/// </summary>
public class ForceRecompile : EditorWindow
{
    [MenuItem("Tools/Force Recompile and Clear Cache", priority = 0)]
    public static void ForceRecompileProject()
    {
        Debug.Log("[ForceRecompile] Starting forced recompilation...");
        
        // Force reimport of the problematic file
        string problemFile = "Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs";
        AssetDatabase.ImportAsset(problemFile, ImportAssetOptions.ForceUpdate);
        
        // Clear console to remove stale errors
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        if (logEntries != null)
        {
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (clearMethod != null)
            {
                clearMethod.Invoke(null, null);
            }
        }
        
        // Force Unity to refresh
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        
        Debug.Log("[ForceRecompile] Recompilation requested. Check if Starkiller menus return after compilation completes.");
        
        EditorUtility.DisplayDialog("Force Recompile", 
            "Unity is now recompiling the project.\n\n" +
            "Your Starkiller menus should return once compilation is complete.\n\n" +
            "If the error persists, try:\n" +
            "1. Right-click on EncounterMediaTransitionManager.cs and select 'Reimport'\n" +
            "2. Close and reopen Unity\n" +
            "3. Delete the Library folder and let Unity rebuild", 
            "OK");
    }
    
    [MenuItem("Tools/Show Compilation Error Details", priority = 1)]
    public static void ShowErrorDetails()
    {
        string filePath = "Assets/_RefactoredScripts/Core/Managers/EncounterMediaTransitionManager.cs";
        TextAsset file = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
        
        if (file != null)
        {
            string[] lines = file.text.Split('\n');
            if (lines.Length > 274)
            {
                Debug.Log($"[ErrorDetails] Line 274 content: '{lines[273]}'");
                
                // Check for the specific issue
                if (lines[273].Contains("captainTypeName"))
                {
                    Debug.LogError("[ErrorDetails] Found 'captainTypeName' in file - this should be 'typeName'!");
                    Debug.Log("[ErrorDetails] The file may not have saved properly or Unity is using a cached version.");
                }
                else if (lines[273].Contains("typeName"))
                {
                    Debug.Log("[ErrorDetails] File appears correct with 'typeName'. This may be a Unity caching issue.");
                }
            }
        }
    }
}