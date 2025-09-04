using UnityEngine;
using UnityEditor;

namespace StarkillerBaseCommand.EditorTools
{
    [CustomEditor(typeof(StarkkillerCreator))]
    public class StarkkillerCreatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();
            
            // Add a space
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            // Get the target
            StarkkillerCreator creator = (StarkkillerCreator)target;
            
            // Create a section title
            EditorGUILayout.LabelField("System Creation Tools", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Add button for creating all systems
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
            if (GUILayout.Button("Create All Systems", GUILayout.Height(40)))
            {
                creator.CreateAllSystems();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space();
            
            // Add button for creating media database
            if (GUILayout.Button("Create Media Database"))
            {
                creator.CreateMediaDatabase();
            }
            
            // Add button for creating test scene
            if (GUILayout.Button("Create Test Scene"))
            {
                creator.CreateTestScene();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            // Create a section title
            EditorGUILayout.LabelField("Migration Tools", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Add button for running migration
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.2f);
            if (GUILayout.Button("Run Migration", GUILayout.Height(30)))
            {
                creator.RunMigration();
            }
            GUI.backgroundColor = Color.white;
            
            // Helpful instructions
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Instructions:\n" +
                "1. Click 'Create All Systems' to setup the new architecture\n" +
                "2. Click 'Run Migration' to transfer data from legacy systems\n" +
                "3. Click 'Create Test Scene' to test the new systems", 
                MessageType.Info);
        }
    }
}