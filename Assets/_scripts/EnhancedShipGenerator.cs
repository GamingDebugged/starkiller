using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to generate enhanced ship encounters with visual elements
public class EnhancedShipGenerator : MonoBehaviour
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
        public string faction;  // imperium, insurgent, smuggler, etc.
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
    public string[] firstNames = { "Parkin", "Trennic", "Flux", "Kiett", "Mallus", "Drawn", "loane" };
    
    private ShipEncounterSystem shipSystem;
    
    void Awake()
    {
        // Find the ShipEncounterSystem if not assigned
        if (shipSystem == null)
        {
            shipSystem = FindFirstObjectByType<ShipEncounterSystem>();
            if (shipSystem == null)
            {
                Debug.LogError("Could not find ShipEncounterSystem. Enhanced ships will not be available.");
            }
        }
    }
    
    // Generate an enhanced ship encounter
    public EnhancedShipEncounter GenerateEnhancedEncounter(int currentDay = 1, int imperialLoyalty = 0, int rebellionSympathy = 0)
    {
        // Use the existing ShipEncounterSystem to generate the base encounter
        ShipEncounter baseEncounter = shipSystem.GenerateEncounter(currentDay, imperialLoyalty, rebellionSympathy);
        
        // Enhance the encounter with visual elements
        return EnhanceShipEncounter(baseEncounter);
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
            faction = "imperium";
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
            faction = "imperium";
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
    public EnhancedShipEncounter EnhanceShipEncounter(ShipEncounter baseEncounter)
    {
        // Use the static conversion method
        EnhancedShipEncounter enhanced = EnhancedShipEncounter.FromShipEncounter(baseEncounter);
        
        // Get ship image
        enhanced.shipImage = GetShipImageForType(enhanced.shipType);
        
        // Determine faction
        string faction = "imperium"; // Default faction
        if (!string.IsNullOrEmpty(enhanced.storyTag))
        {
            faction = enhanced.storyTag;
        }
        else if (enhanced.origin.Contains("Insurgent") || enhanced.shipType.Contains("Insurgent"))
        {
            faction = "insurgent";
        }
        else if (enhanced.origin.Contains("Smuggler") || enhanced.shipType.Contains("Smuggler"))
        {
            faction = "smuggler";
        }
        
        // Generate captain portrait
        enhanced.captainPortrait = GetCaptainPortrait(faction);
        
        // Generate captain name if needed
        if (string.IsNullOrEmpty(enhanced.captainName))
        {
            enhanced.captainName = GenerateCaptainName(faction);
        }
        
        return enhanced;
    }
    
    // Create a test enhanced ship for development testing
    public EnhancedShipEncounter CreateTestEnhancedShip(bool validShip = true)
    {
        // Get a basic test ship first
        ShipEncounter testShip = shipSystem.CreateTestShip();
        
        // If we want an invalid ship, make it invalid
        if (!validShip)
        {
            testShip.accessCode = "XX-1234"; // Invalid access code
            testShip.shouldApprove = false;
            testShip.invalidReason = "Invalid access code";
        }
        
        // Enhance it with images
        return EnhanceShipEncounter(testShip);
    }
}