using UnityEngine;
using UnityEngine.Video;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Unified media management system for Starkiller Base Command
    /// Handles both static images and videos for all game content
    /// </summary>
    public class StarkkillerMediaSystem : MonoBehaviour
    {
        [Header("Media References")]
        [Tooltip("The main media database containing all images and videos")]
        public StarkkillerMediaDatabase mediaDatabase;

        [Header("Media Preferences")]
        [Tooltip("Whether to use videos when available")]
        public bool useVideos = true;
        [Tooltip("Whether to use enhanced visual effects")]
        public bool useEnhancedEffects = true;

        private static StarkkillerMediaSystem _instance;
        public static StarkkillerMediaSystem Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogWarning("StarkkillerMediaSystem not found in scene!");
                return _instance;
            }
        }

        private void Awake()
        {
            // Singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            
            // Find media database if not assigned
            if (mediaDatabase == null)
            {
                Debug.Log("StarkkillerMediaSystem: Attempting to load media database from Resources");
                mediaDatabase = Resources.Load<StarkkillerMediaDatabase>("StarkkillerMediaDatabase");
                
                if (mediaDatabase == null)
                {
                    Debug.LogError("StarkkillerMediaSystem: Media database not found! Please create and assign a StarkkillerMediaDatabase ScriptableObject.");
                }
                else
                {
                    Debug.Log($"StarkkillerMediaSystem: Successfully loaded media database with {mediaDatabase.shipTypes.Count} ship types");
                }
            }
        }

        #region Ship Media
        /// <summary>
        /// Get the appropriate ship image based on ship type and preferences
        /// </summary>
        public Sprite GetShipImage(string shipType, string shipName = "")
        {
            Debug.Log($"MediaSystem.GetShipImage called for '{shipType}', name='{shipName}'");
            
            if (mediaDatabase == null) 
            {
                Debug.LogError("MediaSystem.GetShipImage: mediaDatabase is NULL!");
                return null;
            }
            
            Sprite result = mediaDatabase.GetShipImage(shipType, shipName);
            Debug.Log($"MediaSystem.GetShipImage result: {(result != null ? "Found image" : "NULL")}");
            return result;
        }

        /// <summary>
        /// Get the appropriate ship video based on ship type and preferences
        /// </summary>
        public VideoClip GetShipVideo(string shipType, string shipName = "")
        {
            Debug.Log($"MediaSystem.GetShipVideo called for '{shipType}', name='{shipName}', useVideos={useVideos}");
            
            if (mediaDatabase == null) 
            {
                Debug.LogError("MediaSystem.GetShipVideo: mediaDatabase is NULL!");
                return null;
            }
            
            if (!useVideos)
            {
                Debug.Log("MediaSystem.GetShipVideo: videos are disabled");
                return null;
            }
            
            VideoClip result = mediaDatabase.GetShipVideo(shipType, shipName);
            Debug.Log($"MediaSystem.GetShipVideo result: {(result != null ? "Found video" : "NULL")}");
            return result;
        }

        /// <summary>
        /// Get a random variation of a ship image
        /// </summary>
        public Sprite GetRandomShipVariation(string shipType)
        {
            if (mediaDatabase == null) return null;
            
            Sprite[] variations = mediaDatabase.GetShipVariations(shipType);
            if (variations != null && variations.Length > 0)
            {
                return variations[Random.Range(0, variations.Length)];
            }
            
            // Fall back to standard image if no variations
            return GetShipImage(shipType);
        }
        
        /// <summary>
        /// Get a random ship name for the given ship type
        /// </summary>
        public string GetRandomShipName(string shipTypeName)
        {
            if (mediaDatabase == null) return null;
            
            // Find the matching ship type
            foreach (var shipType in mediaDatabase.shipTypes)
            {
                if (shipType.typeName == shipTypeName)
                {
                    return shipType.GetRandomShipName();
                }
            }
            
            return null;
        }
        #endregion

        #region Captain Media
        /// <summary>
        /// Get a random captain from a faction
        /// </summary>
        public CaptainType.Captain GetRandomCaptain(string faction)
        {
            if (mediaDatabase == null) return null;
            
            // Find all captain types that match this faction
            foreach (var captainType in mediaDatabase.captainTypes)
            {
                foreach (var typeFaction in captainType.factions)
                {
                    if (typeFaction.ToLower() == faction.ToLower())
                    {
                        return captainType.GetRandomCaptain();
                    }
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Get the appropriate captain portrait based on faction and preferences
        /// </summary>
        public Sprite GetCaptainPortrait(string faction, string captainName = "", string captainRank = "")
        {
            if (mediaDatabase == null) return null;
            
            return mediaDatabase.GetCaptainImage(faction, captainName, captainRank);
        }

        /// <summary>
        /// Get the appropriate captain video based on faction and preferences
        /// </summary>
        public VideoClip GetCaptainVideo(string faction, string captainName = "", string captainRank = "", bool isGreeting = true, bool isBribery = false)
        {
            if (mediaDatabase == null || !useVideos) return null;
            
            return mediaDatabase.GetCaptainVideo(faction, captainName, captainRank, isGreeting, isBribery);
        }
        #endregion

        #region Scenario Media
        /// <summary>
        /// Get an appropriate scenario image based on scenario tag
        /// </summary>
        public Sprite GetScenarioImage(string scenarioTag)
        {
            if (mediaDatabase == null) return null;
            
            return mediaDatabase.GetScenarioImage(scenarioTag);
        }

        /// <summary>
        /// Get an appropriate scenario video based on scenario tag
        /// </summary>
        public VideoClip GetScenarioVideo(string scenarioTag)
        {
            if (mediaDatabase == null || !useVideos) return null;
            
            return mediaDatabase.GetScenarioVideo(scenarioTag);
        }
        #endregion

        #region UI Elements
        /// <summary>
        /// Get the appropriate UI icon for the given purpose
        /// </summary>
        public Sprite GetUIIcon(string iconType)
        {
            if (mediaDatabase == null) return null;
            
            switch (iconType.ToLower())
            {
                case "approved":
                    return mediaDatabase.approvedImage;
                case "denied":
                    return mediaDatabase.deniedImage;
                case "pending":
                    return mediaDatabase.pendingImage;
                case "warning":
                    return mediaDatabase.warningImage;
                case "log":
                    return mediaDatabase.logEntryIcon;
                case "credits":
                    return mediaDatabase.creditsIcon;
                case "loyalty":
                    return mediaDatabase.loyaltyIcon;
                default:
                    return null;
            }
        }
        #endregion

        #region Encounter Enhancement
        /// <summary>
        /// Enhance a ship encounter with appropriate media
        /// </summary>
        public EnhancedShipEncounter EnhanceEncounterWithMedia(ShipEncounter baseEncounter)
        {
            EnhancedShipEncounter enhanced = new EnhancedShipEncounter();
            
            // Copy basic properties from base encounter
            enhanced.shipType = baseEncounter.shipType;
            enhanced.shipName = baseEncounter.shipName;
            enhanced.destination = baseEncounter.destination;
            enhanced.origin = baseEncounter.origin;
            enhanced.accessCode = baseEncounter.accessCode;
            enhanced.story = baseEncounter.story;
            enhanced.manifest = baseEncounter.manifest;
            enhanced.shouldApprove = baseEncounter.shouldApprove;
            enhanced.invalidReason = baseEncounter.invalidReason;
            enhanced.captainName = baseEncounter.captainName;
            enhanced.captainRank = baseEncounter.captainRank;
            enhanced.isStoryShip = baseEncounter.isStoryShip;
            enhanced.storyTag = baseEncounter.storyTag;
            enhanced.offersBribe = baseEncounter.offersBribe;
            enhanced.bribeAmount = baseEncounter.bribeAmount;
            enhanced.crewSize = baseEncounter.crewSize;
            enhanced.captainFaction = baseEncounter.captainFaction;
            enhanced.shipTypeData = baseEncounter.shipTypeData;
            enhanced.captainTypeData = baseEncounter.captainTypeData;
            enhanced.scenarioData = baseEncounter.scenarioData;
            
            // Determine faction from encounter data
            string faction = DetermineFaction(baseEncounter);
            
            // Add static media elements
            enhanced.shipImage = GetShipImage(baseEncounter.shipType, enhanced.shipName);
            enhanced.captainPortrait = GetCaptainPortrait(faction, baseEncounter.captainName, baseEncounter.captainRank);
            
            // If this encounter is further enhanced to a VideoEnhancedShipEncounter,
            // the video fields will be populated there
            
            return enhanced;
        }

        /// <summary>
        /// Further enhance an encounter with video media
        /// </summary>
        public VideoEnhancedShipEncounter EnhanceEncounterWithVideo(EnhancedShipEncounter enhancedEncounter)
        {
            if (!useVideos)
            {
                Debug.LogWarning("StarkkillerMediaSystem: Video enhancement requested but videos are disabled");
                return null;
            }
            
            VideoEnhancedShipEncounter videoEnhanced = VideoEnhancedShipEncounter.FromEnhancedEncounter(enhancedEncounter);
            
            // Determine faction
            string faction = "";
            if (!string.IsNullOrEmpty(enhancedEncounter.storyTag))
            {
                faction = enhancedEncounter.storyTag;
            }
            else if (!string.IsNullOrEmpty(enhancedEncounter.captainFaction))
            {
                faction = enhancedEncounter.captainFaction;
            }
            else if (enhancedEncounter.origin.ToLower().Contains("insurgent") || 
                     enhancedEncounter.shipType.ToLower().Contains("insurgent"))
            {
                faction = "insurgent";
            }
            else
            {
                faction = "imperium";
            }
            
            // Add videos if available
            videoEnhanced.shipVideo = GetShipVideo(enhancedEncounter.shipType, enhancedEncounter.shipName);
            videoEnhanced.captainVideo = GetCaptainVideo(
                faction,
                enhancedEncounter.captainName,
                enhancedEncounter.captainRank);
            
            // Add scenario video if this is a story ship
            if (enhancedEncounter.isStoryShip && !string.IsNullOrEmpty(enhancedEncounter.storyTag))
            {
                videoEnhanced.scenarioVideo = GetScenarioVideo(enhancedEncounter.storyTag);
            }
            
            return videoEnhanced;
        }

        /// <summary>
        /// Directly enhance a ship encounter with both images and videos
        /// </summary>
        public VideoEnhancedShipEncounter FullyEnhanceEncounter(ShipEncounter baseEncounter)
        {
            EnhancedShipEncounter enhanced = EnhanceEncounterWithMedia(baseEncounter);
            return EnhanceEncounterWithVideo(enhanced);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Determine faction from encounter data
        /// </summary>
        private string DetermineFaction(ShipEncounter encounter)
        {
            // Default to imperial
            string faction = "imperium";
            
            // Check for explicit faction information
            if (!string.IsNullOrEmpty(encounter.captainFaction))
            {
                faction = encounter.captainFaction;
            }
            // Check story tag
            else if (!string.IsNullOrEmpty(encounter.storyTag))
            {
                faction = encounter.storyTag;
            }
            // Then check for keywords in ship type or origin
            else if (encounter.shipType.ToLower().Contains("insurgent") || 
                     encounter.origin.ToLower().Contains("insurgent"))
            {
                faction = "insurgent";
            }
            else if (encounter.shipType.ToLower().Contains("rebel") || 
                     encounter.origin.ToLower().Contains("rebel"))
            {
                faction = "rebel";
            }
            
            return faction;
        }
        #endregion
    }
}