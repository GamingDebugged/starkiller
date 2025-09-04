using UnityEngine;
using UnityEditor;

// Simple migration tool without namespace dependencies
public class SimpleMediaMigrationTool : EditorWindow
{
    [MenuItem("Starkiller Base/Simple Media Migration")]
    public static void ShowWindow()
    {
        GetWindow<SimpleMediaMigrationTool>("Media Migration");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Simple Media Migration Tool", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "This tool will help you migrate your media assets.\n" +
            "Full functionality will be available after fixing any compilation errors.", 
            MessageType.Info);
        
        if(GUILayout.Button("Log Debug Info"))
        {
            Debug.Log("Media Migration Tool is working!");
            // Find all ScriptableObjects of relevant types
            var assets = Resources.FindObjectsOfTypeAll<ScriptableObject>();
            Debug.Log($"Found {assets.Length} ScriptableObjects in project.");
        }
    }
}