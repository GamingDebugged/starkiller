using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to manage ship encounters with images
public class ShipImageSystem : MonoBehaviour
{
    [System.Serializable]
    public class ShipImageData
    {
        public string shipTypeName;
        public Sprite shipImage;
    }
    
    [System.Serializable]
    public class CaptainData
    {
        public string faction;
        public string[] possibleNames;
        public Sprite[] portraits;
    }
    
    [Header("Ship Images")]
    public ShipImageData[] shipImages = new ShipImageData[0];
    
    [Header("Captain Data")]
    public CaptainData[] captainData = new CaptainData[0];
    public Sprite[] defaultPortraits;
    
    [Header("Default Imperial Names")]
    public string[] rankTitles = { "Commander", "Captain", "Lieutenant", "Major", "Officer" };
    public string[] firstNames = { "Tarkin", "Krennic", "Hux", "Piett", "Kallus", "Thrawn", "Sloane" };
    
    private ShipEncounterSystem shipSystem;
    
    void Awake()
    {
        shipSystem = GetComponent<ShipEncounterSystem>();
    }
    
    // Get ship image by ship type
    public Sprite GetShipImageForType(string shipType)
    {
        foreach (var data in shipImages)
        {
            if (data.shipTypeName == shipType)
            {
                return data.shipImage;
            }
        }
        return null;
    }
    
    // Get captain portrait by faction
    public Sprite GetCaptainPortrait(string faction)
    {
        // Default to imperial
        if (string.IsNullOrEmpty(faction))
        {
            faction = "imperial";
        }
        
        // Find matching faction
        foreach (var data in captainData)
        {
            if (data.faction.ToLower() == faction.ToLower() && 
                data.portraits != null && 
                data.portraits.Length > 0)
            {
                return data.portraits[Random.Range(0, data.portraits.Length)];
            }
        }
        
        // Return a default portrait
        if (defaultPortraits != null && defaultPortraits.Length > 0)
        {
            return defaultPortraits[Random.Range(0, defaultPortraits.Length)];
        }
        
        return null;
    }
    
    // Generate a captain name
    public string GenerateCaptainName(string faction)
    {
        // Default to imperial
        if (string.IsNullOrEmpty(faction))
        {
            faction = "imperial";
        }
        
        // Find matching faction
        foreach (var data in captainData)
        {
            if (data.faction.ToLower() == faction.ToLower() && 
                data.possibleNames != null && 
                data.possibleNames.Length > 0)
            {
                return data.possibleNames[Random.Range(0, data.possibleNames.Length)];
            }
        }
        
        // Generate default imperial name
        string rank = rankTitles[Random.Range(0, rankTitles.Length)];
        string name = firstNames[Random.Range(0, firstNames.Length)];
        return rank + " " + name;
    }
    
    // Enhance a ship encounter with images
    public EnhancedShipEncounter EnhanceEncounter(ShipEncounter baseEncounter)
    {
        EnhancedShipEncounter enhanced = new EnhancedShipEncounter();
        
        // Copy basic properties
        enhanced.shipType = baseEncounter.shipType;
        enhanced.destination = baseEncounter.destination;
        enhanced.origin = baseEncounter.origin;
        enhanced.accessCode = baseEncounter.accessCode;
        enhanced.story = baseEncounter.story;
        enhanced.manifest = baseEncounter.manifest;
        enhanced.shouldApprove = baseEncounter.shouldApprove;
        enhanced.invalidReason = baseEncounter.invalidReason;
        enhanced.captainName = baseEncounter.captainName;
        enhanced.isStoryShip = baseEncounter.isStoryShip;
        enhanced.storyTag = baseEncounter.storyTag;
        enhanced.offersBribe = baseEncounter.offersBribe;
        enhanced.bribeAmount = baseEncounter.bribeAmount;
        
        // Get ship image
        enhanced.shipImage = GetShipImageForType(enhanced.shipType);
        
        // Determine faction
        string faction = "imperial";
        if (!string.IsNullOrEmpty(enhanced.storyTag))
        {
            faction = enhanced.storyTag;
        }
        else if (enhanced.origin.Contains("Rebel") || enhanced.shipType.Contains("Rebel"))
        {
            faction = "rebel";
        }
        
        // Generate captain image
        enhanced.captainPortrait = GetCaptainPortrait(faction);
        
        // Generate captain name if needed
        if (string.IsNullOrEmpty(enhanced.captainName))
        {
            enhanced.captainName = GenerateCaptainName(faction);
        }
        
        return enhanced;
    }
}