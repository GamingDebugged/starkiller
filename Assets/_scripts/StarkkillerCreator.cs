using UnityEngine;
using StarkillerBaseCommand;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Utility component to create and set up Starkiller systems from the inspector
    /// Simply add this component to any GameObject and use the buttons in the inspector
    /// </summary>
    public class StarkkillerCreator : MonoBehaviour
    {
        [Header("Created Components")]
        public StarkkillerContentManager contentManager;
        public StarkkillerMediaSystem mediaSystem;
        public StarkkillerEncounterSystem encounterSystem;
        public LegacySystemsMigrator migrator;
        
        /// <summary>
        /// Create all systems
        /// </summary>
        public void CreateAllSystems()
        {
            // Create a parent GameObject if needed
            GameObject systemsParent = GameObject.Find("StarkkillerSystems");
            if (systemsParent == null)
            {
                systemsParent = new GameObject("StarkkillerSystems");
                Debug.Log("Created StarkkillerSystems parent GameObject");
            }
            
            // Create ContentManager if needed
            if (contentManager == null)
            {
                GameObject contentObj = new GameObject("StarkkillerContentManager");
                contentObj.transform.SetParent(systemsParent.transform);
                contentManager = contentObj.AddComponent<StarkkillerContentManager>();
                Debug.Log("Created StarkkillerContentManager");
            }
            
            // Create MediaSystem if needed
            if (mediaSystem == null)
            {
                GameObject mediaObj = new GameObject("StarkkillerMediaSystem");
                mediaObj.transform.SetParent(systemsParent.transform);
                mediaSystem = mediaObj.AddComponent<StarkkillerMediaSystem>();
                Debug.Log("Created StarkkillerMediaSystem");
            }
            
            // Create EncounterSystem if needed
            if (encounterSystem == null)
            {
                GameObject encounterObj = new GameObject("StarkkillerEncounterSystem");
                encounterObj.transform.SetParent(systemsParent.transform);
                encounterSystem = encounterObj.AddComponent<StarkkillerEncounterSystem>();
                Debug.Log("Created StarkkillerEncounterSystem");
            }
            
            // Connect systems
            if (encounterSystem != null)
            {
                encounterSystem.contentManager = contentManager;
                encounterSystem.mediaSystem = mediaSystem;
            }
            
            // Create the migrator if needed
            if (migrator == null)
            {
                GameObject migratorObj = new GameObject("LegacySystemsMigrator");
                migratorObj.transform.SetParent(systemsParent.transform);
                migrator = migratorObj.AddComponent<LegacySystemsMigrator>();
                Debug.Log("Created LegacySystemsMigrator");
            }
            
            // Find references using reflection
            var contentManagerField = typeof(LegacySystemsMigrator).GetField("contentManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var mediaSystemField = typeof(LegacySystemsMigrator).GetField("mediaSystem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var encounterSystemField = typeof(LegacySystemsMigrator).GetField("encounterSystem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (contentManagerField != null) contentManagerField.SetValue(migrator, contentManager);
            if (mediaSystemField != null) mediaSystemField.SetValue(migrator, mediaSystem);
            if (encounterSystemField != null) encounterSystemField.SetValue(migrator, encounterSystem);
            
            // Find legacy systems
            migrator.FindLegacySystems();
            
            // Try to find or create media database
            StarkkillerMediaDatabase mediaDatabase = Resources.Load<StarkkillerMediaDatabase>("StarkkillerMediaDatabase");
            if (mediaDatabase == null)
            {
                CreateMediaDatabase();
            }
            
            Debug.Log("Systems created and connected successfully");
        }
        
        /// <summary>
        /// Create a media database
        /// </summary>
        public void CreateMediaDatabase()
        {
            Debug.Log("Creating media database...");
            
            // Create the ScriptableObject in memory
            StarkkillerMediaDatabase mediaDatabase = ScriptableObject.CreateInstance<StarkkillerMediaDatabase>();
            
            // Ensure the Resources directory exists
            if (!System.IO.Directory.Exists("Assets/Resources"))
            {
                System.IO.Directory.CreateDirectory("Assets/Resources");
                Debug.Log("Created Resources directory");
            }
            
            #if UNITY_EDITOR
            // Save the asset to disk
            string assetPath = "Assets/Resources/StarkkillerMediaDatabase.asset";
            UnityEditor.AssetDatabase.CreateAsset(mediaDatabase, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log("Media database created at " + assetPath);
            #else
            Debug.LogWarning("Cannot create media database outside of editor");
            #endif
            
            // Assign to MediaSystem
            if (mediaSystem != null)
            {
                mediaSystem.mediaDatabase = mediaDatabase;
                Debug.Log("Assigned media database to MediaSystem");
            }
        }
        
        /// <summary>
        /// Create a test scene
        /// </summary>
        public void CreateTestScene()
        {
            Debug.Log("Creating test scene...");
            
            // Find or create systems first
            if (contentManager == null || mediaSystem == null || encounterSystem == null)
            {
                CreateAllSystems();
            }
            
            // Create test scene GameObject
            GameObject testSceneObj = GameObject.Find("StarkkillerTestScene");
            if (testSceneObj == null)
            {
                testSceneObj = new GameObject("StarkkillerTestScene");
                Debug.Log("Created StarkkillerTestScene GameObject");
            }
            
            // Add test scene component
            StarkkillerTestScene testScene = testSceneObj.GetComponent<StarkkillerTestScene>();
            if (testScene == null)
            {
                testScene = testSceneObj.AddComponent<StarkkillerTestScene>();
                Debug.Log("Added StarkkillerTestScene component");
            }
            
            // Connect systems
            testScene.contentManager = contentManager;
            testScene.mediaSystem = mediaSystem;
            testScene.encounterSystem = encounterSystem;
            
            Debug.Log("Test scene created and connected successfully");
        }
        
        /// <summary>
        /// Run migration using the migrator
        /// </summary>
        public void RunMigration()
        {
            if (migrator == null)
            {
                Debug.LogError("No migrator found. Please create systems first.");
                return;
            }
            
            migrator.MigrateAll();
            Debug.Log("Migration completed successfully");
        }
    }
}