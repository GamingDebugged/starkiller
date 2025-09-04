using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Consolidated database for all visual assets in Starkiller Base Command
    /// Handles both images and videos for ships, captains, and scenarios
    /// </summary>
    [CreateAssetMenu(fileName = "StarkkillerMediaDatabase", menuName = "Starkiller/Media Database")]
    public class StarkkillerMediaDatabase : ScriptableObject
    {
        [Header("Media Selection Settings")]
        [Tooltip("When true, always prioritizes assets from ShipType and CaptainType objects")]
        public bool prioritizeScriptableObjects = true;
        
        [Tooltip("When true, persists the same video across state changes (e.g., approval/denial)")]
        public bool persistVideoSelections = true;
        
        // Cache for currently active media to minimize jarring transitions
        private static Dictionary<string, VideoClip> activeVideoCache = new Dictionary<string, VideoClip>();
        
        [Header("Ship Type References")]
        [Tooltip("References to ShipType assets for direct access")]
        public List<ShipType> shipTypes = new List<ShipType>();

        [Header("Captain Type References")]
        [Tooltip("References to CaptainType assets for direct access")]
        public List<CaptainType> captainTypes = new List<CaptainType>();
        #region Legacy Media Support
        // Original classes retained for backward compatibility
        [System.Serializable]
        public class ShipMediaData
        {
            public string shipTypeName;
            [Tooltip("Static image of this ship type")]
            public Sprite shipImage;
            [Tooltip("Video clip of this ship type (optional)")]
            public VideoClip shipVideo;
            [Tooltip("Optional ship variations for this type")]
            public Sprite[] shipVariations;
            [Tooltip("Optional specific ship names this video applies to")]
            public List<string> specificShipNames = new List<string>();
        }

        [System.Serializable]
        public class CaptainMediaData
        {
            public string faction;  // "imperium", "insurgent", etc.
            [Tooltip("Static portraits for this faction")]
            public Sprite[] portraits;
            [Tooltip("Video clips for this faction (optional)")]
            public VideoClip[] captainVideos;
            [Tooltip("Optional specific captain ranks this applies to")]
            public List<string> captainRanks = new List<string>();
            [Tooltip("Optional specific captain names this applies to")]
            public List<string> captainNames = new List<string>();
        }

        // We use the same class for legacy media to avoid serialization issues
        [Header("Legacy Ship Media")]
        [Tooltip("Legacy ship media data - for migration purposes")]
        public List<ShipMediaData> legacyShipMedia = new List<ShipMediaData>();
        public Sprite defaultShipImage;
        public VideoClip defaultShipVideo;

        [Header("Legacy Captain Media")]
        [Tooltip("Legacy captain media data - for migration purposes")]
        public List<CaptainMediaData> legacyCaptainMedia = new List<CaptainMediaData>();
        public Sprite defaultCaptainImage;
        public VideoClip defaultCaptainVideo;
        
        // Backward compatibility properties
        [HideInInspector]
        public List<ShipMediaData> shipMedia 
        { 
            get 
            { 
                // Initialize if null to prevent errors in existing code
                if (legacyShipMedia == null)
                    legacyShipMedia = new List<ShipMediaData>();
                return legacyShipMedia; 
            } 
        }
        
        [HideInInspector]
        public List<CaptainMediaData> captainMedia 
        { 
            get 
            { 
                // Initialize if null to prevent errors in existing code
                if (legacyCaptainMedia == null)
                    legacyCaptainMedia = new List<CaptainMediaData>();
                return legacyCaptainMedia; 
            } 
        }
        #endregion

        #region Scenario Media
        [System.Serializable]
        public class ScenarioMediaData
        {
            public string scenarioTag;
            [Tooltip("Static images for this scenario type")]
            public Sprite[] scenarioImages;
            [Tooltip("Video clips for this scenario (optional)")]
            public VideoClip[] scenarioVideos;
        }

        [Header("Scenario Media")]
        public List<ScenarioMediaData> scenarioMedia = new List<ScenarioMediaData>();
        public Sprite defaultScenarioImage;
        public VideoClip defaultScenarioVideo;
        #endregion

        #region UI Elements
        [Header("UI Elements")]
        public Sprite approvedImage;
        public Sprite deniedImage;
        public Sprite pendingImage;
        public Sprite warningImage;
        public Sprite logEntryIcon;
        public Sprite creditsIcon;
        public Sprite loyaltyIcon;
        #endregion
        
        #region Encounter Media Preparation
        /// <summary>
        /// Pre-select and cache all media for an encounter to ensure consistency
        /// </summary>
        public void PrepareMediaForEncounter(string shipType, string shipName, string captainName, 
                                            string captainRank, string faction, string scenarioTag)
        {
            Debug.Log($"Preparing and caching all media for encounter: {shipType}, {captainName}");
            
            // Get and cache ship video
            VideoClip shipVid = GetShipVideo(shipType, shipName, true);
            string shipCacheKey = GetVideoCacheKey("Ship", shipType, shipName);
            CacheVideoSelection(shipCacheKey, shipVid);
            
            // Get and cache captain greeting video
            VideoClip captainGreetingVid = GetCaptainVideo(faction, captainName, captainRank, true, false, true);
            string captainGreetingCacheKey = GetVideoCacheKey("Captain", $"{faction}_greeting", captainName);
            CacheVideoSelection(captainGreetingCacheKey, captainGreetingVid);
            
            // Get and cache captain denial video
            VideoClip captainDenialVid = GetCaptainVideo(faction, captainName, captainRank, false, false, true);
            string captainDenialCacheKey = GetVideoCacheKey("Captain", $"{faction}_denial", captainName);
            CacheVideoSelection(captainDenialCacheKey, captainDenialVid);
            
            // Get and cache bribery video if applicable
            VideoClip captainBriberyVid = GetCaptainVideo(faction, captainName, captainRank, false, true, true);
            string captainBriberyCacheKey = GetVideoCacheKey("Captain", $"{faction}_bribery", captainName);
            CacheVideoSelection(captainBriberyCacheKey, captainBriberyVid);
            
            // Get and cache scenario video if applicable
            if (!string.IsNullOrEmpty(scenarioTag))
            {
                VideoClip scenarioVid = GetScenarioVideo(scenarioTag, true);
                string scenarioCacheKey = GetVideoCacheKey("Scenario", scenarioTag, "");
                CacheVideoSelection(scenarioCacheKey, scenarioVid);
            }
            
            Debug.Log("Media preparation and caching complete for encounter");
        }

        /// <summary>
        /// Get consistent media package for an encounter
        /// </summary>
        public EncounterMediaPackage GetEncounterMediaPackage(string shipType, string shipName, string captainName, 
                                                        string captainRank, string faction, string scenarioTag)
        {
            // Ensure all media is prepared and cached
            PrepareMediaForEncounter(shipType, shipName, captainName, captainRank, faction, scenarioTag);
            
            // Create the media package
            EncounterMediaPackage package = new EncounterMediaPackage
            {
                ShipImage = GetShipImage(shipType, shipName),
                ShipVideo = GetShipVideo(shipType, shipName),
                
                CaptainImage = GetCaptainImage(faction, captainName, captainRank),
                CaptainGreetingVideo = GetCaptainVideo(faction, captainName, captainRank, true, false),
                CaptainDenialVideo = GetCaptainVideo(faction, captainName, captainRank, false, false),
                CaptainBriberyVideo = GetCaptainVideo(faction, captainName, captainRank, false, true),
                
                ScenarioImage = !string.IsNullOrEmpty(scenarioTag) ? GetScenarioImage(scenarioTag) : null,
                ScenarioVideo = !string.IsNullOrEmpty(scenarioTag) ? GetScenarioVideo(scenarioTag) : null
            };
            
            return package;
        }

        /// <summary>
        /// Container for all media related to an encounter
        /// </summary>
        public class EncounterMediaPackage
        {
            // Ship media
            public Sprite ShipImage { get; set; }
            public VideoClip ShipVideo { get; set; }
            
            // Captain media
            public Sprite CaptainImage { get; set; }
            public VideoClip CaptainGreetingVideo { get; set; }
            public VideoClip CaptainDenialVideo { get; set; }
            public VideoClip CaptainBriberyVideo { get; set; }
            
            // Scenario media
            public Sprite ScenarioImage { get; set; }
            public VideoClip ScenarioVideo { get; set; }
            
            public EncounterMediaPackage()
            {
                // Initialize all to null
                ShipImage = null;
                ShipVideo = null;
                CaptainImage = null;
                CaptainGreetingVideo = null;
                CaptainDenialVideo = null;
                CaptainBriberyVideo = null;
                ScenarioImage = null;
                ScenarioVideo = null;
            }
            
            /// <summary>
            /// Check if this package has ship video
            /// </summary>
            public bool HasShipVideo() => ShipVideo != null;
            
            /// <summary>
            /// Check if this package has captain greeting video
            /// </summary>
            public bool HasCaptainGreetingVideo() => CaptainGreetingVideo != null;
            
            /// <summary>
            /// Check if this package has captain denial video
            /// </summary>
            public bool HasCaptainDenialVideo() => CaptainDenialVideo != null;
            
            /// <summary>
            /// Check if this package has captain bribery video
            /// </summary>
            public bool HasCaptainBriberyVideo() => CaptainBriberyVideo != null;
            
            /// <summary>
            /// Check if this package has scenario video
            /// </summary>
            public bool HasScenarioVideo() => ScenarioVideo != null;
        }
        #endregion

        #region Media Cache Management
        
        /// <summary>
        /// Clear the video selection cache to force fresh selection
        /// </summary>
        public void ClearMediaCache()
        {
            activeVideoCache.Clear();
            Debug.Log("Media cache cleared");
        }
        
        /// <summary>
        /// Manually cache a video selection to ensure it persists
        /// </summary>
        public void CacheVideoSelection(string cacheKey, VideoClip clip)
        {
            if (clip != null)
            {
                // Update or add the cache entry
                if (activeVideoCache.ContainsKey(cacheKey))
                {
                    activeVideoCache[cacheKey] = clip;
                }
                else
                {
                    activeVideoCache.Add(cacheKey, clip);
                }
                
                Debug.Log($"Cached video: {cacheKey} -> {clip.name}");
            }
        }
        
        /// <summary>
        /// Generates a consistent cache key for videos
        /// </summary>
        private string GetVideoCacheKey(string type, string typeName, string specificName)
        {
            return $"{type}_{typeName}_{specificName}";
        }
        
        #endregion

        #region Ship Media Retrieval
        /// <summary>
        /// Get ship image by ship type name with improved prioritization
        /// </summary>
        public Sprite GetShipImage(string shipTypeName, string shipName = "")
        {
            if (string.IsNullOrEmpty(shipTypeName))
            {
                Debug.LogWarning("GetShipImage called with null or empty shipTypeName");
                return defaultShipImage;
            }
            
            Debug.Log($"Getting ship image for type: {shipTypeName}, name: {shipName}");
            
            Sprite result = null;
            
            // Option 1: Check ScriptableObject assets (ShipType) if preferred
            if (prioritizeScriptableObjects)
            {
                result = GetShipImageFromScriptableObjects(shipTypeName, shipName);
                if (result != null)
                {
                    Debug.Log($"Found ship image from ScriptableObject: {result.name}");
                    return result;
                }
            }
            
            // Option 2: Check legacy data
            result = GetShipImageFromLegacyData(shipTypeName, shipName);
            if (result != null)
            {
                Debug.Log($"Found ship image from legacy data: {result.name}");
                return result;
            }
            
            // Option 3: If we prioritize legacy, now check ScriptableObjects as fallback
            if (!prioritizeScriptableObjects)
            {
                result = GetShipImageFromScriptableObjects(shipTypeName, shipName);
                if (result != null)
                {
                    Debug.Log($"Found ship image from ScriptableObject fallback: {result.name}");
                    return result;
                }
            }
            
            // If we still don't have a result, use default
            Debug.Log($"No ship image found for {shipTypeName}, using default");
            return defaultShipImage;
        }
        
        // Helper to get ship image from ScriptableObjects
        private Sprite GetShipImageFromScriptableObjects(string shipTypeName, string shipName = "")
        {
            // First try to get it from the ShipType assets
            foreach (var shipType in shipTypes)
            {
                if (shipType == null) continue;
                
                if (shipType.typeName == shipTypeName)
                {
                    // If we have a ship name, check if it matches one of the specific ships
                    if (!string.IsNullOrEmpty(shipName) && 
                        shipType.specificShipNames != null && 
                        shipType.specificShipNames.Contains(shipName))
                    {
                        // We have a match with a specific ship name, return icon
                        if (shipType.shipIcon != null)
                            return shipType.shipIcon;
                    }
                    
                    // Return the ship icon if a specific name wasn't matched
                    if (shipType.shipIcon != null)
                        return shipType.shipIcon;
                        
                    // If no icon, try ship variations
                    if (shipType.shipVariations != null && shipType.shipVariations.Length > 0)
                        return shipType.shipVariations[0];
                }
            }
            
            return null;
        }
        
        // Helper to get ship image from legacy data
        private Sprite GetShipImageFromLegacyData(string shipTypeName, string shipName = "")
        {
            // Check for specific ship name match in legacy data
            if (!string.IsNullOrEmpty(shipName))
            {
                foreach (var data in legacyShipMedia)
                {
                    if (data.specificShipNames != null && 
                        data.specificShipNames.Contains(shipName) && 
                        data.shipImage != null)
                    {
                        return data.shipImage;
                    }
                }
            }
            
            // Look for ship type match in legacy data
            foreach (var data in legacyShipMedia)
            {
                if (data.shipTypeName == shipTypeName && data.shipImage != null)
                {
                    return data.shipImage;
                }
            }
            
            return null;
        }
        /// <summary>
        /// Get ship video by ship type with improved caching and prioritization
        /// </summary>
        public VideoClip GetShipVideo(string shipTypeName, string shipName = "", bool forceRefresh = false)
        {
            if (string.IsNullOrEmpty(shipTypeName))
            {
                Debug.LogWarning("GetShipVideo called with null or empty shipTypeName");
                return defaultShipVideo;
            }
            
            // Check cache first to ensure persistent selections
            string cacheKey = GetVideoCacheKey("Ship", shipTypeName, shipName);
            
            if (persistVideoSelections && !forceRefresh && activeVideoCache.ContainsKey(cacheKey))
            {
                VideoClip cachedClip = activeVideoCache[cacheKey];
                Debug.Log($"Using cached ship video for {shipTypeName}: {cachedClip.name}");
                return cachedClip;
            }
            
            Debug.Log($"Getting ship video for type: {shipTypeName}, name: {shipName}");
            
            VideoClip result = null;
            
            // Option 1: Check ScriptableObject assets (ShipType) if preferred
            if (prioritizeScriptableObjects)
            {
                result = GetShipVideoFromScriptableObjects(shipTypeName, shipName);
                if (result != null)
                {
                    Debug.Log($"Found ship video from ScriptableObject: {result.name}");
                    
                    // Cache the result for consistent selection
                    if (persistVideoSelections)
                    {
                        CacheVideoSelection(cacheKey, result);
                    }
                    
                    return result;
                }
            }
            
            // Option 2: Check legacy data
            result = GetShipVideoFromLegacyData(shipTypeName, shipName);
            if (result != null)
            {
                Debug.Log($"Found ship video from legacy data: {result.name}");
                
                // Cache the result for consistent selection
                if (persistVideoSelections)
                {
                    CacheVideoSelection(cacheKey, result);
                }
                
                return result;
            }
            
            // Option 3: If we prioritize legacy, now check ScriptableObjects as fallback
            if (!prioritizeScriptableObjects)
            {
                result = GetShipVideoFromScriptableObjects(shipTypeName, shipName);
                if (result != null)
                {
                    Debug.Log($"Found ship video from ScriptableObject fallback: {result.name}");
                    
                    // Cache the result for consistent selection
                    if (persistVideoSelections)
                    {
                        CacheVideoSelection(cacheKey, result);
                    }
                    
                    return result;
                }
            }
            
            // If we still don't have a result, use default
            Debug.Log($"No ship video found for {shipTypeName}, using default");
            
            // Cache the default for consistent selection
            if (persistVideoSelections)
            {
                CacheVideoSelection(cacheKey, defaultShipVideo);
            }
            
            return defaultShipVideo;
        }
        
        // Helper to get ship video from ScriptableObjects
        private VideoClip GetShipVideoFromScriptableObjects(string shipTypeName, string shipName = "")
        {
            // First try to get it from the ShipType assets
            foreach (var shipType in shipTypes)
            {
                if (shipType == null) continue;
                
                if (shipType.typeName == shipTypeName)
                {
                    // If a specific ship name is provided
                    if (!string.IsNullOrEmpty(shipName) && 
                        shipType.specificShipNames != null && 
                        shipType.specificShipNames.Contains(shipName))
                    {
                        // We have a match with this specific ship name
                        // Return default or random video clip
                        return shipType.GetRandomShipVideo();
                    }
                    
                    // Return a random video for this ship type
                    if (shipType.videoClipPossibilities != null && shipType.videoClipPossibilities.Length > 0)
                    {
                        return shipType.GetRandomShipVideo();
                    }
                    
                    // If no video possibilities, try default video
                    if (shipType.defaultShipVideo != null)
                        return shipType.defaultShipVideo;
                }
            }
            
            return null;
        }
        
        // Helper to get ship video from legacy data
        private VideoClip GetShipVideoFromLegacyData(string shipTypeName, string shipName = "")
        {
            // First check for specific ship name match in legacy data
            if (!string.IsNullOrEmpty(shipName))
            {
                foreach (var data in legacyShipMedia)
                {
                    if (data.specificShipNames != null && 
                        data.specificShipNames.Contains(shipName) && 
                        data.shipVideo != null)
                    {
                        return data.shipVideo;
                    }
                }
            }
            
            // Then look for ship type match in legacy data
            foreach (var data in legacyShipMedia)
            {
                if (data.shipTypeName == shipTypeName && data.shipVideo != null)
                {
                    return data.shipVideo;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Get ship variations
        /// </summary>
        public Sprite[] GetShipVariations(string shipTypeName)
        {
            if (string.IsNullOrEmpty(shipTypeName))
            {
                return null;
            }
            
            // First try to get it from the ShipType assets
            foreach (var shipType in shipTypes)
            {
                if (shipType == null) continue;
                
                if (shipType.typeName == shipTypeName && 
                    shipType.shipVariations != null && 
                    shipType.shipVariations.Length > 0)
                {
                    return shipType.shipVariations;
                }
            }
            
            // If not found in ShipType assets, fall back to legacy data
            foreach (var data in legacyShipMedia)
            {
                if (data.shipTypeName == shipTypeName && 
                    data.shipVariations != null && 
                    data.shipVariations.Length > 0)
                {
                    return data.shipVariations;
                }
            }
            
            return null;
        }
        #endregion

        #region Captain Media Retrieval
        /// <summary>
        /// Get captain image by faction, name, and rank with improved prioritization
        /// </summary>
        public Sprite GetCaptainImage(string faction, string captainName = "", string captainRank = "")
        {
            // Default to imperial if no faction specified
            if (string.IsNullOrEmpty(faction))
            {
                faction = "imperium";
            }
            
            Debug.Log($"Getting captain image for faction: {faction}, name: {captainName}, rank: {captainRank}");
            
            Sprite result = null;
            
            // Option 1: Check ScriptableObject assets (CaptainType) if preferred
            if (prioritizeScriptableObjects)
            {
                result = GetCaptainImageFromScriptableObjects(faction, captainName, captainRank);
                if (result != null)
                {
                    Debug.Log($"Found captain image from ScriptableObject: {result.name}");
                    return result;
                }
            }
            
            // Option 2: Check legacy data
            result = GetCaptainImageFromLegacyData(faction, captainName, captainRank);
            if (result != null)
            {
                Debug.Log($"Found captain image from legacy data: {result.name}");
                return result;
            }
            
            // Option 3: If we prioritize legacy, now check ScriptableObjects as fallback
            if (!prioritizeScriptableObjects)
            {
                result = GetCaptainImageFromScriptableObjects(faction, captainName, captainRank);
                if (result != null)
                {
                    Debug.Log($"Found captain image from ScriptableObject fallback: {result.name}");
                    return result;
                }
            }
            
            // If we still don't have a result, return default
            Debug.Log($"No captain image found for {faction}, {captainName}, {captainRank}, using default");
            return defaultCaptainImage;
        }
        
        // Helper to get captain image from ScriptableObjects
        private Sprite GetCaptainImageFromScriptableObjects(string faction, string captainName, string captainRank)
        {
            // First, try to find by name match in captainTypes
            if (!string.IsNullOrEmpty(captainName))
            {
                foreach (var captainType in captainTypes)
                {
                    if (captainType == null || captainType.captains == null) continue;
                    
                    foreach (var captain in captainType.captains)
                    {
                        if (captain.GetFullName() == captainName && captain.portrait != null)
                        {
                            return captain.portrait;
                        }
                    }
                }
            }
            
            // Then, try to find by rank match in captainTypes
            if (!string.IsNullOrEmpty(captainRank))
            {
                foreach (var captainType in captainTypes)
                {
                    if (captainType == null || captainType.captains == null) continue;
                    
                    foreach (var captain in captainType.captains)
                    {
                        if (captain.rank == captainRank && captain.portrait != null)
                        {
                            return captain.portrait;
                        }
                    }
                }
            }
            
            // Then, try to find by faction match in captainTypes
            foreach (var captainType in captainTypes)
            {
                if (captainType == null || captainType.captains == null || 
                    captainType.factions == null || captainType.captains.Count == 0) 
                    continue;
                
                bool factionMatch = false;
                foreach (var typeFaction in captainType.factions)
                {
                    if (typeFaction.ToLower() == faction.ToLower())
                    {
                        factionMatch = true;
                        break;
                    }
                }
                
                if (factionMatch && captainType.captains.Count > 0)
                {
                    var randomCaptain = captainType.captains[Random.Range(0, captainType.captains.Count)];
                    if (randomCaptain.portrait != null)
                    {
                        return randomCaptain.portrait;
                    }
                }
            }
            
            return null;
        }
        
        // Helper to get captain image from legacy data
        private Sprite GetCaptainImageFromLegacyData(string faction, string captainName, string captainRank)
        {
            // Check for specific captain name
            if (!string.IsNullOrEmpty(captainName))
            {
                foreach (var data in legacyCaptainMedia)
                {
                    if (data.captainNames != null && 
                        data.captainNames.Contains(captainName) && 
                        data.portraits != null && 
                        data.portraits.Length > 0)
                    {
                        return data.portraits[Random.Range(0, data.portraits.Length)];
                    }
                }
            }
            
            // Then check for specific rank
            if (!string.IsNullOrEmpty(captainRank))
            {
                foreach (var data in legacyCaptainMedia)
                {
                    if (data.captainRanks != null && 
                        data.captainRanks.Contains(captainRank) && 
                        data.portraits != null && 
                        data.portraits.Length > 0)
                    {
                        return data.portraits[Random.Range(0, data.portraits.Length)];
                    }
                }
            }
            
            // Finally check faction
            foreach (var data in legacyCaptainMedia)
            {
                if (data.faction.ToLower() == faction.ToLower() && 
                    data.portraits != null && 
                    data.portraits.Length > 0)
                {
                    return data.portraits[Random.Range(0, data.portraits.Length)];
                }
            }
            
            return null;
        }

        /// <summary>
        /// Get captain video by faction, name, and rank with improved caching and prioritization
        /// </summary>
        public VideoClip GetCaptainVideo(string faction, string captainName = "", string captainRank = "", 
                                        bool isGreeting = true, bool isBribery = false, bool forceRefresh = false)
        {
            // Default to imperial if no faction specified
            if (string.IsNullOrEmpty(faction))
            {
                faction = "imperium";
            }
            
            // Create a context string to distinguish between different types of videos
            string context = isGreeting ? "greeting" : (isBribery ? "bribery" : "denial");
            
            // Check cache first to ensure persistent selections
            string cacheKey = GetVideoCacheKey("Captain", $"{faction}_{context}", captainName);
            
            if (persistVideoSelections && !forceRefresh && activeVideoCache.ContainsKey(cacheKey))
            {
                VideoClip cachedClip = activeVideoCache[cacheKey];
                Debug.Log($"Using cached captain video for {faction} ({context}): {cachedClip.name}");
                return cachedClip;
            }
            
            Debug.Log($"Getting captain video for faction: {faction}, name: {captainName}, rank: {captainRank}, context: {context}");
            
            VideoClip result = null;
            
            // Option 1: Check ScriptableObject assets (CaptainType) if preferred
            if (prioritizeScriptableObjects)
            {
                result = GetCaptainVideoFromScriptableObjects(faction, captainName, captainRank, isGreeting, isBribery);
                if (result != null)
                {
                    Debug.Log($"Found captain video from ScriptableObject: {result.name}");
                    
                    // Cache the result for consistent selection
                    if (persistVideoSelections)
                    {
                        CacheVideoSelection(cacheKey, result);
                    }
                    
                    return result;
                }
            }
            
            // Option 2: Check legacy data
            result = GetCaptainVideoFromLegacyData(faction, captainName, captainRank);
            if (result != null)
            {
                Debug.Log($"Found captain video from legacy data: {result.name}");
                
                // Cache the result for consistent selection
                if (persistVideoSelections)
                {
                    CacheVideoSelection(cacheKey, result);
                }
                
                return result;
            }
            
            // Option 3: If we prioritize legacy, now check ScriptableObjects as fallback
            if (!prioritizeScriptableObjects)
            {
                result = GetCaptainVideoFromScriptableObjects(faction, captainName, captainRank, isGreeting, isBribery);
                if (result != null)
                {
                    Debug.Log($"Found captain video from ScriptableObject fallback: {result.name}");
                    
                    // Cache the result for consistent selection
                    if (persistVideoSelections)
                    {
                        CacheVideoSelection(cacheKey, result);
                    }
                    
                    return result;
                }
            }
            
            // If we still don't have a result, use default
            Debug.Log($"No captain video found for {faction}, {captainName}, {captainRank}, using default");
            
            // Cache the default for consistent selection
            if (persistVideoSelections)
            {
                CacheVideoSelection(cacheKey, defaultCaptainVideo);
            }
            
            return defaultCaptainVideo;
        }
        
        // Helper to get captain video from ScriptableObjects
        private VideoClip GetCaptainVideoFromScriptableObjects(string faction, string captainName, string captainRank,
                                                        bool isGreeting, bool isBribery)
        {
            // First, try to find by name match in captainTypes
            if (!string.IsNullOrEmpty(captainName))
            {
                foreach (var captainType in captainTypes)
                {
                    if (captainType == null || captainType.captains == null) continue;
                    
                    foreach (var captain in captainType.captains)
                    {
                        if (captain.GetFullName() == captainName)
                        {
                            // Choose the appropriate dialog based on context
                            CaptainType.Captain.DialogEntry dialog = null;
                            
                            if (isBribery && captain.bribeDialogs != null && captain.bribeDialogs.Count > 0)
                            {
                                dialog = captain.GetRandomBriberyPhrase();
                            }
                            else if (!isGreeting && captain.deniedDialogs != null && captain.deniedDialogs.Count > 0)
                            {
                                dialog = captain.GetRandomDenialResponse();
                            }
                            else if (captain.greetingDialogs != null && captain.greetingDialogs.Count > 0)
                            {
                                dialog = captain.GetRandomGreeting();
                            }
                            
                            if (dialog != null && dialog.associatedVideo != null)
                            {
                                return dialog.associatedVideo;
                            }
                        }
                    }
                }
            }

            // Then, try to find by rank match
            if (!string.IsNullOrEmpty(captainRank))
            {
                foreach (var captainType in captainTypes)
                {
                    if (captainType == null || captainType.captains == null) continue;
                    
                    foreach (var captain in captainType.captains)
                    {
                        if (captain.rank == captainRank)
                        {
                            // Choose the appropriate dialog based on context
                            CaptainType.Captain.DialogEntry dialog = null;
                            
                            if (isBribery && captain.bribeDialogs != null && captain.bribeDialogs.Count > 0)
                            {
                                dialog = captain.GetRandomBriberyPhrase();
                            }
                            else if (!isGreeting && captain.deniedDialogs != null && captain.deniedDialogs.Count > 0)
                            {
                                dialog = captain.GetRandomDenialResponse();
                            }
                            else if (captain.greetingDialogs != null && captain.greetingDialogs.Count > 0)
                            {
                                dialog = captain.GetRandomGreeting();
                            }
                            
                            if (dialog != null && dialog.associatedVideo != null)
                            {
                                return dialog.associatedVideo;
                            }
                        }
                    }
                }
            }

            // Then, try by faction
            foreach (var captainType in captainTypes)
            {
                if (captainType == null || captainType.captains == null || captainType.factions == null) continue;
                
                bool factionMatch = false;
                foreach (var typeFaction in captainType.factions)
                {
                    if (typeFaction.ToLower() == faction.ToLower())
                    {
                        factionMatch = true;
                        break;
                    }
                }
                
                if (factionMatch && captainType.captains.Count > 0)
                {
                    // Choose a consistent captain for this encounter based on a hash of the faction, name, and rank
                    int captainIndex = 0;
                    if (!string.IsNullOrEmpty(captainName))
                    {
                        captainIndex = Mathf.Abs(captainName.GetHashCode()) % captainType.captains.Count;
                    }
                    else
                    {
                        string hashInput = faction + captainRank;
                        captainIndex = Mathf.Abs(hashInput.GetHashCode()) % captainType.captains.Count;
                    }
                    
                    var selectedCaptain = captainType.captains[captainIndex];
                    
                    // Choose the appropriate dialog based on context
                    CaptainType.Captain.DialogEntry dialog = null;
                    
                    if (isBribery && selectedCaptain.bribeDialogs != null && selectedCaptain.bribeDialogs.Count > 0)
                    {
                        dialog = selectedCaptain.GetRandomBriberyPhrase();
                    }
                    else if (!isGreeting && selectedCaptain.deniedDialogs != null && selectedCaptain.deniedDialogs.Count > 0)
                    {
                        dialog = selectedCaptain.GetRandomDenialResponse();
                    }
                    else if (selectedCaptain.greetingDialogs != null && selectedCaptain.greetingDialogs.Count > 0)
                    {
                        dialog = selectedCaptain.GetRandomGreeting();
                    }
                    
                    if (dialog != null && dialog.associatedVideo != null)
                    {
                        return dialog.associatedVideo;
                    }
                }
            }
            
            return null;
        }

        // Helper to get captain video from legacy data
        private VideoClip GetCaptainVideoFromLegacyData(string faction, string captainName, string captainRank)
        {
            // First check for specific captain name
            if (!string.IsNullOrEmpty(captainName))
            {
                foreach (var data in legacyCaptainMedia)
                {
                    if (data.captainNames != null && 
                        data.captainNames.Contains(captainName) && 
                        data.captainVideos != null && 
                        data.captainVideos.Length > 0)
                    {
                        return data.captainVideos[Random.Range(0, data.captainVideos.Length)];
                    }
                }
            }
            
            // Then check for specific rank
            if (!string.IsNullOrEmpty(captainRank))
            {
                foreach (var data in legacyCaptainMedia)
                {
                    if (data.captainRanks != null && 
                        data.captainRanks.Contains(captainRank) && 
                        data.captainVideos != null && 
                        data.captainVideos.Length > 0)
                    {
                        return data.captainVideos[Random.Range(0, data.captainVideos.Length)];
                    }
                }
            }
            
            // Finally check faction in legacy data
            foreach (var data in legacyCaptainMedia)
            {
                if (data.faction.ToLower() == faction.ToLower() && 
                    data.captainVideos != null && 
                    data.captainVideos.Length > 0)
                {
                    return data.captainVideos[Random.Range(0, data.captainVideos.Length)];
                }
            }
            
            return null;
        }
        #endregion

        
        #region Scenario Media Retrieval Methods
        /// <summary>
        /// Get scenario image by scenario tag with improved prioritization
        /// </summary>
        public Sprite GetScenarioImage(string scenarioTag)
        {
            if (string.IsNullOrEmpty(scenarioTag))
            {
                Debug.LogWarning("GetScenarioImage called with null or empty scenarioTag");
                return defaultScenarioImage;
            }
            
            Debug.Log($"Getting scenario image for tag: {scenarioTag}");
            
            foreach (var data in scenarioMedia)
            {
                if (data.scenarioTag == scenarioTag && 
                    data.scenarioImages != null && 
                    data.scenarioImages.Length > 0)
                {
                    Sprite result = data.scenarioImages[Random.Range(0, data.scenarioImages.Length)];
                    Debug.Log($"Found scenario image: {result.name}");
                    return result;
                }
            }
            
            // If still here, no match found
            Debug.Log($"No scenario image found for {scenarioTag}, using default");
            return defaultScenarioImage;
        }

        /// <summary>
        /// Get scenario video by scenario tag with improved caching and prioritization
        /// </summary>
        public VideoClip GetScenarioVideo(string scenarioTag, bool forceRefresh = false)
        {
            if (string.IsNullOrEmpty(scenarioTag))
            {
                Debug.LogWarning("GetScenarioVideo called with null or empty scenarioTag");
                return defaultScenarioVideo;
            }
            
            // Check cache first to ensure persistent selections
            string cacheKey = GetVideoCacheKey("Scenario", scenarioTag, "");
            
            if (persistVideoSelections && !forceRefresh && activeVideoCache.ContainsKey(cacheKey))
            {
                VideoClip cachedClip = activeVideoCache[cacheKey];
                Debug.Log($"Using cached scenario video for {scenarioTag}: {cachedClip.name}");
                return cachedClip;
            }
            
            Debug.Log($"Getting scenario video for tag: {scenarioTag}");
            
            foreach (var data in scenarioMedia)
            {
                if (data.scenarioTag == scenarioTag && 
                    data.scenarioVideos != null && 
                    data.scenarioVideos.Length > 0)
                {
                    VideoClip result = data.scenarioVideos[Random.Range(0, data.scenarioVideos.Length)];
                    Debug.Log($"Found scenario video: {result.name}");
                    
                    // Cache the result for consistent selection
                    if (persistVideoSelections)
                    {
                        CacheVideoSelection(cacheKey, result);
                    }
                    
                    return result;
                }
            }
            
            // If still here, no match found
            Debug.Log($"No scenario video found for {scenarioTag}, using default");
            
            // Cache the default for consistent selection
            if (persistVideoSelections)
            {
                CacheVideoSelection(cacheKey, defaultScenarioVideo);
            }
            
            return defaultScenarioVideo;
        }
        #endregion

        #region Data Migration Tools
        /// <summary>
        /// Migrate legacy ship media data to ShipType assets
        /// </summary>
        public void MigrateLegacyShipMediaToAssets()
        {
            foreach (var legacyShip in legacyShipMedia)
            {
                // Find matching ShipType asset
                ShipType matchingShipType = null;
                foreach (var shipType in shipTypes)
                {
                    if (shipType.typeName == legacyShip.shipTypeName)
                    {
                        matchingShipType = shipType;
                        break;
                    }
                }
                
                // If we didn't find a matching ship type, skip this entry
                if (matchingShipType == null)
                {
                    Debug.LogWarning($"No matching ShipType asset found for '{legacyShip.shipTypeName}' during migration");
                    continue;
                }
                
                // Update the ship type with legacy data
                if (matchingShipType.shipIcon == null && legacyShip.shipImage != null)
                {
                    matchingShipType.shipIcon = legacyShip.shipImage;
                }
                
                if (matchingShipType.defaultShipVideo == null && legacyShip.shipVideo != null)
                {
                    matchingShipType.defaultShipVideo = legacyShip.shipVideo;
                }
                
                // Copy ship variations if needed
                if ((matchingShipType.shipVariations == null || matchingShipType.shipVariations.Length == 0) &&
                    legacyShip.shipVariations != null && legacyShip.shipVariations.Length > 0)
                {
                    matchingShipType.shipVariations = legacyShip.shipVariations;
                }
                
                // Copy specific ship names if needed
                if (matchingShipType.specificShipNames == null)
                {
                    matchingShipType.specificShipNames = new List<string>();
                }
                
                foreach (var shipName in legacyShip.specificShipNames)
                {
                    if (!matchingShipType.specificShipNames.Contains(shipName))
                    {
                        matchingShipType.specificShipNames.Add(shipName);
                    }
                }
            }
            
            Debug.Log("Legacy ship media data migration completed");
        }

        /// <summary>
        /// Migrate legacy captain media data to CaptainType assets
        /// </summary>
        public void MigrateLegacyCaptainMediaToAssets()
        {
            foreach (var legacyCaptain in legacyCaptainMedia)
            {
                // Find a matching CaptainType asset by faction
                CaptainType matchingCaptainType = null;
                foreach (var captainType in captainTypes)
                {
                    foreach (var typeFaction in captainType.factions)
                    {
                        if (typeFaction.ToLower() == legacyCaptain.faction.ToLower())
                        {
                            matchingCaptainType = captainType;
                            break;
                        }
                    }
                    
                    if (matchingCaptainType != null)
                        break;
                }
                
                // If we didn't find a matching captain type, skip this entry
                if (matchingCaptainType == null)
                {
                    Debug.LogWarning($"No matching CaptainType asset found for faction '{legacyCaptain.faction}' during migration");
                    continue;
                }
                
                // Create new captains from legacy data if the CaptainType has no captains yet
                if (matchingCaptainType.captains == null || matchingCaptainType.captains.Count == 0)
                {
                    matchingCaptainType.captains = new List<CaptainType.Captain>();
                    
                    // Process ranks
                    foreach (var rank in legacyCaptain.captainRanks)
                    {
                        // Create a captain for each rank if we have portraits available
                        if (legacyCaptain.portraits != null && legacyCaptain.portraits.Length > 0)
                        {
                            for (int i = 0; i < legacyCaptain.portraits.Length; i++)
                            {
                                CaptainType.Captain newCaptain = new CaptainType.Captain();
                                newCaptain.rank = rank;
                                
                                // Set a default name
                                if (matchingCaptainType.possibleFirstNames != null && matchingCaptainType.possibleFirstNames.Length > 0)
                                {
                                    newCaptain.firstName = matchingCaptainType.possibleFirstNames[Random.Range(0, matchingCaptainType.possibleFirstNames.Length)];
                                }
                                else
                                {
                                    newCaptain.firstName = "Unknown";
                                }
                                
                                if (matchingCaptainType.possibleLastNames != null && matchingCaptainType.possibleLastNames.Length > 0)
                                {
                                    newCaptain.lastName = matchingCaptainType.possibleLastNames[Random.Range(0, matchingCaptainType.possibleLastNames.Length)];
                                }
                                else
                                {
                                    newCaptain.lastName = "Officer " + Random.Range(1000, 9999);
                                }

                                // Set the portrait
                                newCaptain.portrait = legacyCaptain.portraits[i];

                                // Add to the list
                                matchingCaptainType.captains.Add(newCaptain);

                                // Stop if we've created too many captains
                                if (matchingCaptainType.captains.Count >= 10)
                                    break;
                            }
                        }
                    }
                }

                // Add dialog options with videos if available
                if (legacyCaptain.captainVideos != null && legacyCaptain.captainVideos.Length > 0)
                {
                    foreach (var captain in matchingCaptainType.captains)
                    {
                        // Add greeting dialog
                        captain.greetingDialogs = new List<CaptainType.Captain.DialogEntry>();
                        
                        for (int i = 0; i < legacyCaptain.captainVideos.Length; i++)
                        {
                            CaptainType.Captain.DialogEntry dialog = new CaptainType.Captain.DialogEntry();
                            dialog.phrase = $"Standard greeting {i+1}";
                            dialog.associatedVideo = legacyCaptain.captainVideos[i];
                            captain.greetingDialogs.Add(dialog);
                        }
                    }
                }
            }

            Debug.Log("Legacy captain media data migration completed");
        }
        #endregion
    }
}