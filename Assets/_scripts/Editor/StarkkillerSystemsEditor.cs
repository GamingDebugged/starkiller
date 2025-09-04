using UnityEngine;
using UnityEditor;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Editor tools for creating and managing Starkiller systems
    /// </summary>
    [InitializeOnLoad]
    public static class StarkkillerSystemsEditor
    {
        [MenuItem("Starkiller/Create Systems")]
        public static void CreateSystems()
        {
            // Create a parent GameObject
            GameObject systemsParent = GameObject.Find("StarkkillerSystems");
            if (systemsParent == null)
            {
                systemsParent = new GameObject("StarkkillerSystems");
                Undo.RegisterCreatedObjectUndo(systemsParent, "Create StarkkillerSystems parent");
            }
            
            // Create ContentManager
            GameObject contentObj = new GameObject("StarkkillerContentManager");
            Undo.RegisterCreatedObjectUndo(contentObj, "Create StarkkillerContentManager");
            contentObj.transform.SetParent(systemsParent.transform);
            StarkkillerContentManager contentManager = contentObj.AddComponent<StarkkillerContentManager>();
            Undo.RegisterCreatedObjectUndo(contentManager, "Add StarkkillerContentManager component");
            
            // Create MediaSystem
            GameObject mediaObj = new GameObject("StarkkillerMediaSystem");
            Undo.RegisterCreatedObjectUndo(mediaObj, "Create StarkkillerMediaSystem");
            mediaObj.transform.SetParent(systemsParent.transform);
            StarkkillerMediaSystem mediaSystem = mediaObj.AddComponent<StarkkillerMediaSystem>();
            Undo.RegisterCreatedObjectUndo(mediaSystem, "Add StarkkillerMediaSystem component");
            
            // Create EncounterSystem
            GameObject encounterObj = new GameObject("StarkkillerEncounterSystem");
            Undo.RegisterCreatedObjectUndo(encounterObj, "Create StarkkillerEncounterSystem");
            encounterObj.transform.SetParent(systemsParent.transform);
            StarkkillerEncounterSystem encounterSystem = encounterObj.AddComponent<StarkkillerEncounterSystem>();
            Undo.RegisterCreatedObjectUndo(encounterSystem, "Add StarkkillerEncounterSystem component");
            
            // Connect systems
            encounterSystem.contentManager = contentManager;
            encounterSystem.mediaSystem = mediaSystem;
            
            // Create the migrator
            GameObject migratorObj = new GameObject("LegacySystemsMigrator");
            Undo.RegisterCreatedObjectUndo(migratorObj, "Create LegacySystemsMigrator");
            migratorObj.transform.SetParent(systemsParent.transform);
            LegacySystemsMigrator migrator = migratorObj.AddComponent<LegacySystemsMigrator>();
            Undo.RegisterCreatedObjectUndo(migrator, "Add LegacySystemsMigrator component");
            
            // Configure migrator using reflection to set private fields
            var contentManagerField = typeof(LegacySystemsMigrator).GetField("contentManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var mediaSystemField = typeof(LegacySystemsMigrator).GetField("mediaSystem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var encounterSystemField = typeof(LegacySystemsMigrator).GetField("encounterSystem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (contentManagerField != null) contentManagerField.SetValue(migrator, contentManager);
            if (mediaSystemField != null) mediaSystemField.SetValue(migrator, mediaSystem);
            if (encounterSystemField != null) encounterSystemField.SetValue(migrator, encounterSystem);
            
            // Find legacy systems
            migrator.FindLegacySystems();
            
            // Create media database if it doesn't exist
            StarkkillerMediaDatabase mediaDatabase = AssetDatabase.LoadAssetAtPath<StarkkillerMediaDatabase>("Assets/Resources/StarkkillerMediaDatabase.asset");
            if (mediaDatabase == null)
            {
                // Ensure the Resources directory exists
                if (!System.IO.Directory.Exists("Assets/Resources"))
                {
                    System.IO.Directory.CreateDirectory("Assets/Resources");
                }
                
                // Create the database
                mediaDatabase = ScriptableObject.CreateInstance<StarkkillerMediaDatabase>();
                AssetDatabase.CreateAsset(mediaDatabase, "Assets/Resources/StarkkillerMediaDatabase.asset");
                AssetDatabase.SaveAssets();
            }
            
            // Assign the database to the media system
            mediaSystem.mediaDatabase = mediaDatabase;
            
            Debug.Log("Starkiller systems created successfully. Use the LegacySystemsMigrator to migrate data from the old systems.");
            
            // Select the parent object
            Selection.activeGameObject = systemsParent;
        }
        
        [MenuItem("Starkiller/Create Media Database")]
        public static void CreateMediaDatabase()
        {
            // Check if database already exists
            StarkkillerMediaDatabase mediaDatabase = AssetDatabase.LoadAssetAtPath<StarkkillerMediaDatabase>("Assets/Resources/StarkkillerMediaDatabase.asset");
            if (mediaDatabase != null)
            {
                Debug.Log("Media database already exists at Assets/Resources/StarkkillerMediaDatabase.asset");
                Selection.activeObject = mediaDatabase;
                return;
            }
            
            // Ensure the Resources directory exists
            if (!System.IO.Directory.Exists("Assets/Resources"))
            {
                System.IO.Directory.CreateDirectory("Assets/Resources");
            }
            
            // Create the database
            mediaDatabase = ScriptableObject.CreateInstance<StarkkillerMediaDatabase>();
            AssetDatabase.CreateAsset(mediaDatabase, "Assets/Resources/StarkkillerMediaDatabase.asset");
            AssetDatabase.SaveAssets();
            
            Debug.Log("Media database created at Assets/Resources/StarkkillerMediaDatabase.asset");
            Selection.activeObject = mediaDatabase;
        }
        
        [MenuItem("Starkiller/Create Test Scene")]
        public static void CreateTestScene()
        {
            // Create a parent GameObject
            GameObject testSceneObj = new GameObject("StarkkillerTestScene");
            Undo.RegisterCreatedObjectUndo(testSceneObj, "Create StarkkillerTestScene");
            
            // Add test scene component
            StarkkillerTestScene testScene = testSceneObj.AddComponent<StarkkillerTestScene>();
            Undo.RegisterCreatedObjectUndo(testScene, "Add StarkkillerTestScene component");
            
            // Find systems
            StarkkillerContentManager contentManager = Object.FindFirstObjectByType<StarkkillerContentManager>();
            StarkkillerMediaSystem mediaSystem = Object.FindFirstObjectByType<StarkkillerMediaSystem>();
            StarkkillerEncounterSystem encounterSystem = Object.FindFirstObjectByType<StarkkillerEncounterSystem>();
            
            // Set references
            testScene.contentManager = contentManager;
            testScene.mediaSystem = mediaSystem;
            testScene.encounterSystem = encounterSystem;
            
            Debug.Log("Test scene created. Add UI elements and assign them in the inspector.");
            
            // Select the test scene object
            Selection.activeGameObject = testSceneObj;
        }
    }
}