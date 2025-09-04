using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using System;
using System.Linq;

namespace StarkillerBaseCommand.Encounters
{
    /// <summary>
    /// Manages media assets and selection for ship encounters
    /// Supports dynamic media selection, caching, and fallback mechanisms
    /// </summary>
    [CreateAssetMenu(fileName = "EncounterMediaManager", menuName = "Starkiller/Encounters/Encounter Media Manager", order = 2)]
    public class EncounterMediaManager : ScriptableObject
    {
        [Header("Media Databases")]
        [SerializeField] private List<Sprite> shipSprites = new List<Sprite>();
        [SerializeField] private List<Sprite> captainPortraits = new List<Sprite>();
        [SerializeField] private List<VideoClip> shipVideos = new List<VideoClip>();
        [SerializeField] private List<VideoClip> captainVideos = new List<VideoClip>();

        [Header("Media Categorization")]
        [SerializeField] private List<MediaCategory> mediaCategories = new List<MediaCategory>();

        [Header("Caching")]
        [SerializeField] private int maxCachedMedia = 50;
        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
        private Dictionary<string, VideoClip> videoCache = new Dictionary<string, VideoClip>();

        [Header("Fallback Assets")]
        [SerializeField] private Sprite defaultShipSprite;
        [SerializeField] private Sprite defaultCaptainPortrait;
        [SerializeField] private VideoClip defaultShipVideo;
        [SerializeField] private VideoClip defaultCaptainVideo;

        [Serializable]
        public class MediaCategory
        {
            public string categoryName;
            public List<Sprite> categorySprites = new List<Sprite>();
            public List<VideoClip> categoryVideos = new List<VideoClip>();
            public List<string> tags = new List<string>();
        }

        [Header("Media Selection Rules")]
        [SerializeField] private bool enableDynamicMediaSelection = true;
        [SerializeField] private bool enforceTagConsistency = true;

        /// <summary>
        /// Get a ship sprite based on ship type and optional tags
        /// </summary>
        public Sprite GetShipSprite(string shipType, string[] tags = null)
        {
            // Check cache first
            if (spriteCache.TryGetValue(shipType, out Sprite cachedSprite))
                return cachedSprite;

            Sprite selectedSprite = null;

            // Dynamic media selection
            if (enableDynamicMediaSelection && mediaCategories.Count > 0)
            {
                selectedSprite = FindMediaByTypeAndTags(shipSprites, shipType, tags);
            }

            // Fallback to direct list search
            if (selectedSprite == null)
            {
                selectedSprite = shipSprites.Find(s => s.name.Contains(shipType));
            }

            // Use default if no sprite found
            selectedSprite ??= defaultShipSprite;

            // Cache the result
            CacheMedia(shipType, selectedSprite);

            return selectedSprite;
        }

        /// <summary>
        /// Get a captain portrait based on faction and optional tags
        /// </summary>
        public Sprite GetCaptainPortrait(string faction, string[] tags = null)
        {
            // Check cache first
            if (spriteCache.TryGetValue(faction, out Sprite cachedPortrait))
                return cachedPortrait;

            Sprite selectedPortrait = null;

            // Dynamic media selection
            if (enableDynamicMediaSelection && mediaCategories.Count > 0)
            {
                selectedPortrait = FindMediaByTypeAndTags(captainPortraits, faction, tags);
            }

            // Fallback to direct list search
            if (selectedPortrait == null)
            {
                selectedPortrait = captainPortraits.Find(p => p.name.Contains(faction));
            }

            // Use default if no portrait found
            selectedPortrait ??= defaultCaptainPortrait;

            // Cache the result
            CacheMedia(faction, selectedPortrait);

            return selectedPortrait;
        }

        /// <summary>
        /// Get a ship video based on ship type and optional tags
        /// </summary>
        public VideoClip GetShipVideo(string shipType, string[] tags = null)
        {
            // Check cache first
            if (videoCache.TryGetValue(shipType, out VideoClip cachedVideo))
                return cachedVideo;

            VideoClip selectedVideo = null;

            // Dynamic media selection
            if (enableDynamicMediaSelection && mediaCategories.Count > 0)
            {
                selectedVideo = FindMediaByTypeAndTags(shipVideos, shipType, tags);
            }

            // Fallback to direct list search
            if (selectedVideo == null)
            {
                selectedVideo = shipVideos.Find(v => v.name.Contains(shipType));
            }

            // Use default if no video found
            selectedVideo ??= defaultShipVideo;

            // Cache the result
            CacheMedia(shipType, selectedVideo);

            return selectedVideo;
        }

        /// <summary>
        /// Get a captain video based on faction and optional tags
        /// </summary>
        public VideoClip GetCaptainVideo(string faction, string[] tags = null)
        {
            // Check cache first
            if (videoCache.TryGetValue(faction, out VideoClip cachedVideo))
                return cachedVideo;

            VideoClip selectedVideo = null;

            // Dynamic media selection
            if (enableDynamicMediaSelection && mediaCategories.Count > 0)
            {
                selectedVideo = FindMediaByTypeAndTags(captainVideos, faction, tags);
            }

            // Fallback to direct list search
            if (selectedVideo == null)
            {
                selectedVideo = captainVideos.Find(v => v.name.Contains(faction));
            }

            // Use default if no video found
            selectedVideo ??= defaultCaptainVideo;

            // Cache the result
            CacheMedia(faction, selectedVideo);

            return selectedVideo;
        }

        /// <summary>
        /// Find media by type and optional tags with advanced selection logic
        /// </summary>
        private T FindMediaByTypeAndTags<T>(List<T> mediaList, string type, string[] tags) where T : UnityEngine.Object
        {
            if (tags == null || tags.Length == 0)
                return mediaList.Find(m => m.name.Contains(type));

            // Find categories matching the type and tags
            var matchingCategories = mediaCategories.FindAll(category => 
                category.categoryName.Contains(type) && 
                (tags == null || tags.All(tag => category.tags.Contains(tag))));

            // If no matching categories and tag enforcement is on, return null
            if (enforceTagConsistency && matchingCategories.Count == 0)
                return null;

            // Collect media from matching categories
            List<T> candidateMedia = new List<T>();
            foreach (var category in matchingCategories)
            {
                if (category.categoryVideos is List<VideoClip> videoList)
                    candidateMedia.AddRange(videoList as IEnumerable<T>);
                if (category.categorySprites is List<Sprite> spriteList)
                    candidateMedia.AddRange(spriteList as IEnumerable<T>);
            }

            // If no candidates, fall back to name-based search
            return candidateMedia.Count > 0 
                ? candidateMedia[UnityEngine.Random.Range(0, candidateMedia.Count)] 
                : mediaList.Find(m => m.name.Contains(type));
        }

        /// <summary>
        /// Cache media to prevent repeated lookups
        /// </summary>
        private void CacheMedia<T>(string key, T media) where T : UnityEngine.Object
        {
            // Manage cache size
            if (media is Sprite sprite)
            {
                if (spriteCache.Count >= maxCachedMedia)
                {
                    var oldestKey = spriteCache.Keys.FirstOrDefault();
                    if (oldestKey != null)
                        spriteCache.Remove(oldestKey);
                }
                spriteCache[key] = sprite;
            }
            else if (media is VideoClip video)
            {
                if (videoCache.Count >= maxCachedMedia)
                {
                    var oldestKey = videoCache.Keys.FirstOrDefault();
                    if (oldestKey != null)
                        videoCache.Remove(oldestKey);
                }
                videoCache[key] = video;
            }
        }

        /// <summary>
        /// Clear media caches
        /// </summary>
        public void ClearMediaCaches()
        {
            spriteCache.Clear();
            videoCache.Clear();
        }

        /// <summary>
        /// Preload media for performance optimization
        /// </summary>
        public void PreloadMedia(string[] shipTypes, string[] factions)
        {
            foreach (var shipType in shipTypes)
            {
                GetShipSprite(shipType);
                GetShipVideo(shipType);
            }

            foreach (var faction in factions)
            {
                GetCaptainPortrait(faction);
                GetCaptainVideo(faction);
            }
        }
    }
}