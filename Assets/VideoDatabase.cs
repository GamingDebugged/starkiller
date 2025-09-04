using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "VideoDatabase", menuName = "Starkiller/Video Database")]
public class VideoDatabase : ScriptableObject
{
    [System.Serializable]
    public class ShipVideoMapping
    {
        public string shipType;
        public VideoClip videoClip;
        [Tooltip("Optional specific ship names this video applies to")]
        public List<string> specificShipNames = new List<string>();
    }
    
    [System.Serializable]
    public class CaptainVideoMapping
    {
        public string faction;  // "imperium", "insurgent", etc.
        [Tooltip("Optional specific captain ranks this applies to")]
        public List<string> captainRanks = new List<string>();
        [Tooltip("Optional specific captain names this applies to")]
        public List<string> captainNames = new List<string>();
        public VideoClip[] videoPossibilities;
    }
    
    public List<ShipVideoMapping> shipVideos = new List<ShipVideoMapping>();
    public List<CaptainVideoMapping> captainVideos = new List<CaptainVideoMapping>();
    
    // Default fallback videos
    public VideoClip defaultShipVideo;
    public VideoClip defaultCaptainVideo;
    
    public VideoClip GetShipVideo(string shipType, string shipName = "")
    {
        // First check for specific ship name match
        if (!string.IsNullOrEmpty(shipName))
        {
            foreach (var mapping in shipVideos)
            {
                if (mapping.specificShipNames.Contains(shipName))
                    return mapping.videoClip;
            }
        }
        
        // Then look for ship type match
        foreach (var mapping in shipVideos)
        {
            if (mapping.shipType == shipType)
                return mapping.videoClip;
        }
        
        return defaultShipVideo;
    }
    
    public VideoClip GetCaptainVideo(string faction, string captainName = "", string captainRank = "")
    {
        // First check for specific captain name
        if (!string.IsNullOrEmpty(captainName))
        {
            foreach (var mapping in captainVideos)
            {
                if (mapping.captainNames.Contains(captainName) && 
                    mapping.videoPossibilities != null && 
                    mapping.videoPossibilities.Length > 0)
                {
                    return mapping.videoPossibilities[Random.Range(0, mapping.videoPossibilities.Length)];
                }
            }
        }
        
        // Then check for rank
        if (!string.IsNullOrEmpty(captainRank))
        {
            foreach (var mapping in captainVideos)
            {
                if (mapping.captainRanks.Contains(captainRank) && 
                    mapping.videoPossibilities != null && 
                    mapping.videoPossibilities.Length > 0)
                {
                    return mapping.videoPossibilities[Random.Range(0, mapping.videoPossibilities.Length)];
                }
            }
        }
        
        // Finally check for faction
        foreach (var mapping in captainVideos)
        {
            if (mapping.faction == faction && 
                mapping.videoPossibilities != null && 
                mapping.videoPossibilities.Length > 0)
            {
                return mapping.videoPossibilities[Random.Range(0, mapping.videoPossibilities.Length)];
            }
        }
        
        return defaultCaptainVideo;
    }
}