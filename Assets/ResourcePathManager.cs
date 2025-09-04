using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Global namespace types used in this file
using AccessCode = StarkillerBaseCommand.AccessCode;
using CargoManifest = StarkillerBaseCommand.CargoManifest;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Centralized resource path management to eliminate redundant loading attempts
    /// and standardize resource access across the project
    /// </summary>
    public static class ResourcePathManager
    {
        // Define standard paths for each resource type
        private static readonly Dictionary<ResourceType, string[]> ResourcePaths = new Dictionary<ResourceType, string[]>
        {
            {
                ResourceType.AccessCodes, new string[]
                {
                    "_ScriptableObjects/AccessCodes",
                    "ScriptableObjects/AccessCodes",
                    "AccessCodes",
                    "Data/AccessCodes"
                }
            },
            {
                ResourceType.CargoManifests, new string[]
                {
                    "_ScriptableObjects/CargoManifests",
                    "ScriptableObjects/CargoManifests",
                    "CargoManifests",
                    "Data/CargoManifests",
                    "Manifests"
                }
            },
            {
                ResourceType.Scenarios, new string[]
                {
                    "_ScriptableObjects/Scenarios",
                    "ScriptableObjects/Scenarios",
                    "Scenarios",
                    "Ships/Scenarios",
                    "Data/Scenarios"
                }
            },
            {
                ResourceType.ShipTypes, new string[]
                {
                    "_ScriptableObjects/ShipTypes",
                    "ScriptableObjects/ShipTypes",
                    "ShipTypes",
                    "Ships/Types",
                    "Data/ShipTypes"
                }
            },
            {
                ResourceType.CaptainTypes, new string[]
                {
                    "_ScriptableObjects/CaptainTypes",
                    "ScriptableObjects/CaptainTypes",
                    "CaptainTypes",
                    "Captains/Types",
                    "Data/CaptainTypes"
                }
            },
            {
                ResourceType.Factions, new string[]
                {
                    "_ScriptableObjects/Factions",
                    "ScriptableObjects/Factions",
                    "Factions",
                    "Data/Factions"
                }
            }
        };
        
        // Cache loaded resources to avoid duplicate loading
        private static Dictionary<string, object> resourceCache = new Dictionary<string, object>();
        
        // Track which paths actually contain resources
        private static Dictionary<ResourceType, string> validPaths = new Dictionary<ResourceType, string>();
        
        public enum ResourceType
        {
            AccessCodes,
            CargoManifests,
            Scenarios,
            ShipTypes,
            CaptainTypes,
            Factions
        }
        
        /// <summary>
        /// Load all resources of a specific type
        /// </summary>
        public static T[] LoadAll<T>(ResourceType resourceType) where T : UnityEngine.Object
        {
            string cacheKey = $"{resourceType}_{typeof(T).Name}";
            
            // Check cache first
            if (resourceCache.ContainsKey(cacheKey))
            {
                return resourceCache[cacheKey] as T[];
            }
            
            List<T> allResources = new List<T>();
            HashSet<string> loadedNames = new HashSet<string>();
            
            // Check if we already know the valid path
            if (validPaths.ContainsKey(resourceType))
            {
                string validPath = validPaths[resourceType];
                T[] resources = Resources.LoadAll<T>(validPath);
                if (resources != null && resources.Length > 0)
                {
                    allResources.AddRange(resources);
                    Debug.Log($"ResourcePathManager: Loaded {resources.Length} {typeof(T).Name} from known path: Resources/{validPath}");
                }
            }
            else
            {
                // Try all possible paths
                if (ResourcePaths.ContainsKey(resourceType))
                {
                    foreach (string path in ResourcePaths[resourceType])
                    {
                        try
                        {
                            T[] resources = Resources.LoadAll<T>(path);
                            
                            if (resources != null && resources.Length > 0)
                            {
                                // Filter out duplicates by name
                                foreach (var resource in resources)
                                {
                                    if (resource != null && !loadedNames.Contains(resource.name))
                                    {
                                        allResources.Add(resource);
                                        loadedNames.Add(resource.name);
                                    }
                                }
                                
                                // Remember this as a valid path
                                if (!validPaths.ContainsKey(resourceType))
                                {
                                    validPaths[resourceType] = path;
                                    Debug.Log($"ResourcePathManager: Found {resources.Length} {typeof(T).Name} at Resources/{path}");
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"ResourcePathManager: Error loading from {path}: {e.Message}");
                        }
                    }
                }
            }
            
            // Cache the results
            T[] resultArray = allResources.ToArray();
            resourceCache[cacheKey] = resultArray;
            
            if (resultArray.Length == 0)
            {
                Debug.LogWarning($"ResourcePathManager: No {typeof(T).Name} resources found for {resourceType}");
            }
            else
            {
                Debug.Log($"ResourcePathManager: Total {typeof(T).Name} loaded: {resultArray.Length}");
            }
            
            return resultArray;
        }
        
        /// <summary>
        /// Load a single resource by name
        /// </summary>
        public static T Load<T>(ResourceType resourceType, string resourceName) where T : UnityEngine.Object
        {
            // First try to load from known valid path
            if (validPaths.ContainsKey(resourceType))
            {
                string fullPath = $"{validPaths[resourceType]}/{resourceName}";
                T resource = Resources.Load<T>(fullPath);
                if (resource != null)
                    return resource;
            }
            
            // Otherwise try all paths
            if (ResourcePaths.ContainsKey(resourceType))
            {
                foreach (string basePath in ResourcePaths[resourceType])
                {
                    string fullPath = $"{basePath}/{resourceName}";
                    T resource = Resources.Load<T>(fullPath);
                    if (resource != null)
                    {
                        // Remember this path for future use
                        if (!validPaths.ContainsKey(resourceType))
                        {
                            validPaths[resourceType] = basePath;
                        }
                        return resource;
                    }
                }
            }
            
            Debug.LogWarning($"ResourcePathManager: Could not find {resourceName} of type {typeof(T).Name}");
            return null;
        }
        
        /// <summary>
        /// Clear the resource cache
        /// </summary>
        public static void ClearCache()
        {
            resourceCache.Clear();
            Debug.Log("ResourcePathManager: Resource cache cleared");
        }
        
        /// <summary>
        /// Get the valid path for a resource type (if known)
        /// </summary>
        public static string GetValidPath(ResourceType resourceType)
        {
            return validPaths.ContainsKey(resourceType) ? validPaths[resourceType] : null;
        }
        
        /// <summary>
        /// Debug method to print all valid paths
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DebugPrintValidPaths()
        {
            Debug.Log("=== ResourcePathManager Valid Paths ===");
            foreach (var kvp in validPaths)
            {
                Debug.Log($"{kvp.Key}: Resources/{kvp.Value}");
            }
            
            if (validPaths.Count == 0)
            {
                Debug.Log("No valid paths discovered yet");
            }
        }
        
        /// <summary>
        /// Create required folder structure in Resources
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void CreateResourceFolderStructure()
        {
            #if UNITY_EDITOR
            string resourcesPath = "Assets/Resources";
            
            // Ensure Resources folder exists
            if (!System.IO.Directory.Exists(resourcesPath))
            {
                System.IO.Directory.CreateDirectory(resourcesPath);
                UnityEditor.AssetDatabase.Refresh();
            }
            
            // Create standard folder structure
            string scriptableObjectsPath = $"{resourcesPath}/_ScriptableObjects";
            if (!System.IO.Directory.Exists(scriptableObjectsPath))
            {
                System.IO.Directory.CreateDirectory(scriptableObjectsPath);
            }
            
            // Create subfolders for each resource type
            string[] subfolders = {
                "AccessCodes",
                "CargoManifests", 
                "Scenarios",
                "ShipTypes",
                "CaptainTypes",
                "Factions"
            };
            
            foreach (string folder in subfolders)
            {
                string folderPath = $"{scriptableObjectsPath}/{folder}";
                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                    Debug.Log($"Created folder: {folderPath}");
                }
            }
            
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log("ResourcePathManager: Resource folder structure created");
            #endif
        }
    }
    
    /// <summary>
    /// Helper class to refactor existing code to use ResourcePathManager
    /// </summary>
    public static class ResourceLoadingHelper
    {
        /// <summary>
        /// Load AccessCodes using centralized path management
        /// </summary>
        public static List<AccessCode> LoadAccessCodes()
        {
            var codes = ResourcePathManager.LoadAll<AccessCode>(ResourcePathManager.ResourceType.AccessCodes);
            return codes != null ? codes.ToList() : new List<AccessCode>();
        }
        
        /// <summary>
        /// Load CargoManifests using centralized path management
        /// </summary>
        public static List<CargoManifest> LoadCargoManifests()
        {
            var manifests = ResourcePathManager.LoadAll<CargoManifest>(ResourcePathManager.ResourceType.CargoManifests);
            return manifests != null ? manifests.ToList() : new List<CargoManifest>();
        }
        
        /// <summary>
        /// Load Scenarios using centralized path management
        /// </summary>
        public static List<global::ShipScenario> LoadScenarios()
        {
            var scenarios = ResourcePathManager.LoadAll<global::ShipScenario>(ResourcePathManager.ResourceType.Scenarios);
            return scenarios != null ? scenarios.ToList() : new List<global::ShipScenario>();
        }
        
        /// <summary>
        /// Load ShipScenarios using centralized path management (alias for LoadScenarios)
        /// </summary>
        public static List<global::ShipScenario> LoadShipScenarios()
        {
            return LoadScenarios();
        }
        
        /// <summary>
        /// Load ShipTypes using centralized path management
        /// </summary>
        public static List<global::ShipType> LoadShipTypes()
        {
            var shipTypes = ResourcePathManager.LoadAll<global::ShipType>(ResourcePathManager.ResourceType.ShipTypes);
            return shipTypes != null ? shipTypes.ToList() : new List<global::ShipType>();
        }
        
        /// <summary>
        /// Load CaptainTypes using centralized path management
        /// </summary>
        public static List<global::CaptainType> LoadCaptainTypes()
        {
            var captainTypes = ResourcePathManager.LoadAll<global::CaptainType>(ResourcePathManager.ResourceType.CaptainTypes);
            return captainTypes != null ? captainTypes.ToList() : new List<global::CaptainType>();
        }
    }
}