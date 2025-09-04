using UnityEngine;
using UnityEditor;
using System.IO;

namespace StarkillerBaseCommand.EditorTools
{
    public class StarkkillerDataTools : EditorWindow
    {
        private Vector2 scrollPos;
        private Texture2D headerImage;
        
        [MenuItem("Starkiller/Data Tools")]
        public static void ShowWindow()
        {
            GetWindow<StarkkillerDataTools>("Starkiller Data Tools");
        }
        
        private void OnEnable()
        {
            // Try to load a header image from the project
            headerImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/UItest.png");
        }
        
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            // Header
            GUILayout.Space(10);
            if (headerImage != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(headerImage, GUILayout.Height(100));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(10);
            GUILayout.Label("Starkiller Base Command - Data Management Tools", EditorStyles.boldLabel);
            GUILayout.Space(20);
            
            // Data Model Section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Data Models", EditorStyles.boldLabel);
            
            GUILayout.Label("Create or modify ScriptableObject data models:", EditorStyles.wordWrappedLabel);
            
            if (GUILayout.Button("Generate Data Model"))
            {
                DataModelGenerator.ShowWindow();
            }
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(10);
            
            // Excel Conversion Section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Excel Conversion", EditorStyles.boldLabel);
            
            GUILayout.Label("Convert Excel data files to CSV format:", EditorStyles.wordWrappedLabel);
            
            if (GUILayout.Button("Convert Excel to CSV"))
            {
                ExcelConverter.ShowWindow();
            }
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(10);
            
            // Data Import Section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Data Import", EditorStyles.boldLabel);
            
            GUILayout.Label("Import data from CSV files to create ScriptableObjects:", EditorStyles.wordWrappedLabel);
            
            if (GUILayout.Button("Import CSV Data"))
            {
                CSVImporter.ShowWindow();
            }
            
            if (GUILayout.Button("Import Game Data"))
            {
                DataImporter.ShowWindow();
            }
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(10);
            
            // Data Utilities Section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Data Utilities", EditorStyles.boldLabel);
            
            GUILayout.Label("Manage your game data:", EditorStyles.wordWrappedLabel);
            
            if (GUILayout.Button("Create Default Folders"))
            {
                CreateDefaultFolders();
            }
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(20);
            
            EditorGUILayout.LabelField("These tools help automate the process of importing and managing game data.", EditorStyles.wordWrappedLabel);
            
            EditorGUILayout.EndScrollView();
        }
        
        private void CreateDefaultFolders()
        {
            // Create necessary folders for data storage
            string[] folders = new string[] 
            {
                "Assets/_ScriptableObjects",
                "Assets/_ScriptableObjects/ShipTypes",
                "Assets/_ScriptableObjects/CaptainTypes",
                "Assets/_ScriptableObjects/Locations",
                "Assets/_ScriptableObjects/AccessCodes",
                "Assets/_ScriptableObjects/Manifests",
                "Assets/_ScriptableObjects/Scenarios",
                "Assets/_ScriptableObjects/DayRules",
                "Assets/_ScriptableObjects/Consequences",
                "Assets/_ScriptableObjects/ContrabandItems",
                "Assets/_ScriptableObjects/MediaAssets",
                "Assets/_Temp",
                "Assets/_Temp/CSV",
                "Assets/Resources"
            };
            
            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Folders Created", "Default folders created successfully.", "OK");
        }
    }
}