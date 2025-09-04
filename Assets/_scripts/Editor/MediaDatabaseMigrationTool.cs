using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Basic editor window for database migration - simplified to avoid type issues
/// </summary>
public class MediaDatabaseMigrationTool : EditorWindow
{
    private UnityEngine.Object selectedDatabase;
    private string statusMessage = "";
    
    [MenuItem("Starkiller Base/Media Database Migration Tool")]
    public static void ShowWindow()
    {
        GetWindow<MediaDatabaseMigrationTool>("Media Migration");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Media Database Migration Tool", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox(
            "This tool will help you migrate data from the old media format to the new format.\n\n" +
            "1. Select your media database\n" +
            "2. Make sure you have created ShipType and CaptainType assets\n" +
            "3. Move ship and captain information to the new assets\n\n" +
            "This tool is still under development - expect more functionality soon!",
            MessageType.Info);
        
        // Database selection
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Media Database", EditorStyles.boldLabel);
        selectedDatabase = EditorGUILayout.ObjectField(
            "Media Database", selectedDatabase, typeof(ScriptableObject), false);
        
        // Simple actions
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
        
        if(GUILayout.Button("Log Database Info"))
        {
            if (selectedDatabase != null)
            {
                Debug.Log($"Selected database: {selectedDatabase.name} (Type: {selectedDatabase.GetType().Name})");
                statusMessage = "Database information logged to console";
            }
            else
            {
                statusMessage = "Please select a database first";
            }
        }
        
        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
        }
    }
}