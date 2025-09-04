using UnityEngine;
using UnityEditor;

namespace StarkillerBaseCommand.EditorTools
{
    [CustomEditor(typeof(LegacySystemsMigrator))]
    public class LegacySystemsMigratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();
            
            // Add a space
            EditorGUILayout.Space();
            
            // Get the target
            LegacySystemsMigrator migrator = (LegacySystemsMigrator)target;
            
            // Add buttons for finding systems
            if (GUILayout.Button("Find Legacy Systems"))
            {
                migrator.FindLegacySystems();
            }
            
            if (GUILayout.Button("Find New Systems"))
            {
                migrator.FindNewSystems();
            }
            
            // Add a space
            EditorGUILayout.Space();
            
            // Add buttons for migration
            if (GUILayout.Button("Migrate Ship Data"))
            {
                migrator.MigrateShipData();
            }
            
            if (GUILayout.Button("Migrate Media Data"))
            {
                migrator.MigrateMediaData();
            }
            
            if (GUILayout.Button("Migrate Gameplay Settings"))
            {
                migrator.MigrateGameplaySettings();
            }
            
            // Add a space and a big button for migrating all
            EditorGUILayout.Space();
            
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("MIGRATE ALL", GUILayout.Height(40)))
            {
                migrator.MigrateAll();
            }
            GUI.backgroundColor = Color.white;
            
            // Add a space and a button for disabling legacy systems
            EditorGUILayout.Space();
            
            GUI.backgroundColor = new Color(0.8f, 0.4f, 0.4f);
            if (GUILayout.Button("Disable Legacy Systems"))
            {
                migrator.DisableLegacySystems();
            }
            GUI.backgroundColor = Color.white;
        }
    }
}