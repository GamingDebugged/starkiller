using UnityEngine;
using UnityEditor;
using StarkillerBaseCommand;

namespace StarkillerBaseCommand.EditorTools
{
    /// <summary>
    /// Editor utility to help set up the encounter systems
    /// This script adds menu items to create required prefabs for the game
    /// </summary>
    public class EncounterSystemSetup
    {
        [MenuItem("Starkiller/Setup/Create EncounterSystemManager")]
        public static void CreateEncounterSystemManager()
        {
            // Create manager GameObject
            GameObject managerObject = new GameObject("EncounterSystemManager");
            EncounterSystemManager manager = managerObject.AddComponent<EncounterSystemManager>();
            
            // Set default values
            manager.activeSystem = EncounterSystemManager.EncounterSystemType.MasterShipGenerator;
            manager.verboseLogging = true;
            manager.initializationDelay = 0.2f;
            
            // Try to find references
            manager.SendMessage("FindSystemReferences", SendMessageOptions.DontRequireReceiver);
            
            // Create prefab
            string localPath = "Assets/_Core/Prefabs/EncounterSystemManager.prefab";
            
            // Make sure the directory exists
            string directory = System.IO.Path.GetDirectoryName(localPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            
            // Create the prefab
            bool success = false;
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(managerObject, localPath, out success);
            
            if (success)
            {
                Debug.Log("Created EncounterSystemManager prefab at: " + localPath);
                Selection.activeObject = prefab;
            }
            else
            {
                Debug.LogError("Failed to create EncounterSystemManager prefab!");
            }
            
            // Clean up temporary object
            Object.DestroyImmediate(managerObject);
        }
        
        [MenuItem("Starkiller/Setup/Create StarkkillerEncounterSystem")]
        public static void CreateStarkkillerEncounterSystem()
        {
            // Create manager GameObject
            GameObject systemObject = new GameObject("StarkkillerEncounterSystem");
            StarkkillerEncounterSystem system = systemObject.AddComponent<StarkkillerEncounterSystem>();
            
            // Try to find references if they exist
            var contentManager = FindFirstObjectOfType<StarkkillerContentManager>();
            var mediaSystem = FindFirstObjectOfType<StarkkillerMediaSystem>();
            
            if (contentManager != null)
                system.contentManager = contentManager;
                
            if (mediaSystem != null)
                system.mediaSystem = mediaSystem;
            
            // Create prefab
            string localPath = "Assets/_Core/Prefabs/StarkkillerEncounterSystem.prefab";
            
            // Make sure the directory exists
            string directory = System.IO.Path.GetDirectoryName(localPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            
            // Create the prefab
            bool success = false;
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(systemObject, localPath, out success);
            
            if (success)
            {
                Debug.Log("Created StarkkillerEncounterSystem prefab at: " + localPath);
                Selection.activeObject = prefab;
            }
            else
            {
                Debug.LogError("Failed to create StarkkillerEncounterSystem prefab!");
            }
            
            // Clean up temporary object
            Object.DestroyImmediate(systemObject);
        }
        
        [MenuItem("Starkiller/Setup/Create ShipEncounterSystem")]
        public static void CreateShipEncounterSystem()
        {
            // Create system GameObject
            GameObject systemObject = new GameObject("ShipEncounterSystem");
            ShipEncounterSystem system = systemObject.AddComponent<ShipEncounterSystem>();
            
            // Try to find references if they exist
            system.masterShipGenerator = FindFirstObjectOfType<MasterShipGenerator>();
            
            // Create prefab
            string localPath = "Assets/_Core/Prefabs/ShipEncounterSystem.prefab";
            
            // Make sure the directory exists
            string directory = System.IO.Path.GetDirectoryName(localPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            
            // Create the prefab
            bool success = false;
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(systemObject, localPath, out success);
            
            if (success)
            {
                Debug.Log("Created ShipEncounterSystem prefab at: " + localPath);
                Selection.activeObject = prefab;
            }
            else
            {
                Debug.LogError("Failed to create ShipEncounterSystem prefab!");
            }
            
            // Clean up temporary object
            Object.DestroyImmediate(systemObject);
        }
        
        [MenuItem("Starkiller/Setup/Create All Encounter Systems")]
        public static void CreateAllSystems()
        {
            CreateEncounterSystemManager();
            CreateStarkkillerEncounterSystem();
            CreateShipEncounterSystem();
            
            Debug.Log("Created all encounter system prefabs!");
        }
        
        [MenuItem("Starkiller/Setup/Fix Missing Script References")]
        public static void FixMissingScriptReferences()
        {
            // This will find all prefabs with missing scripts
            string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab");
            int fixedCount = 0;
            
            foreach (string guid in prefabPaths)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab == null)
                    continue;
                    
                // Check if this is one of our encounter system prefabs
                if (path.Contains("EncounterSystemManager") || 
                    path.Contains("StarkkillerEncounterSystem") || 
                    path.Contains("ShipEncounterSystem"))
                {
                    // Check for missing scripts
                    bool foundMissing = false;
                    Component[] components = prefab.GetComponents<Component>();
                    
                    foreach (Component component in components)
                    {
                        if (component == null)
                        {
                            foundMissing = true;
                            break;
                        }
                    }
                    
                    if (foundMissing)
                    {
                        Debug.Log("Found missing script in: " + path);
                        
                        // Recreate the prefab
                        if (path.Contains("EncounterSystemManager"))
                        {
                            CreateEncounterSystemManager();
                            fixedCount++;
                        }
                        else if (path.Contains("StarkkillerEncounterSystem"))
                        {
                            CreateStarkkillerEncounterSystem();
                            fixedCount++;
                        }
                        else if (path.Contains("ShipEncounterSystem"))
                        {
                            CreateShipEncounterSystem();
                            fixedCount++;
                        }
                    }
                }
            }
            
            Debug.Log($"Fixed {fixedCount} prefabs with missing script references.");
        }
        
        // Helper method that mimics the runtime FindFirstObjectByType
        private static T FindFirstObjectOfType<T>() where T : Object
        {
            return Object.FindObjectOfType<T>();
        }
    }
}