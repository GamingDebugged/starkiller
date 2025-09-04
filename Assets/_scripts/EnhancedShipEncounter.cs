using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enhanced ship encounter data with image support
/// Extends the base ShipEncounter with visual elements
/// </summary>
[System.Serializable]
public class EnhancedShipEncounter : ShipEncounter
{
    // Visual information
    public Sprite shipImage;           // Visual representation of the ship
    public Sprite captainPortrait;     // Portrait of the captain
    
    // This property is used to maintain compatibility with existing code
    public new string shipName 
    {
        get { return base.shipName; }
        set { base.shipName = value; }
    }
    
    // Helper methods to check for images
    public bool HasShipImage() => shipImage != null;
    public bool HasCaptainPortrait() => captainPortrait != null;
    
    // Override base methods to customize display
    public override string GetShipInfo()
    {
        return base.GetShipInfo();
    }
    
    // Override credentials info method
    public override string GetCredentialsInfo()
    {
        return base.GetCredentialsInfo();
    }
    
    // Convert from ShipEncounter to EnhancedShipEncounter
    public static EnhancedShipEncounter FromShipEncounter(ShipEncounter source)
    {
        if (source == null)
            return null;
            
        EnhancedShipEncounter enhanced = new EnhancedShipEncounter
        {
            // Copy all base properties
            shipType = source.shipType,
            destination = source.destination,
            origin = source.origin,
            accessCode = source.accessCode,
            story = source.story,
            manifest = source.manifest,
            crewSize = source.crewSize,
            shouldApprove = source.shouldApprove,
            invalidReason = source.invalidReason,
            captainName = source.captainName,
            captainRank = source.captainRank,
            captainFaction = source.captainFaction,
            isStoryShip = source.isStoryShip,
            storyTag = source.storyTag,
            offersBribe = source.offersBribe,
            bribeAmount = source.bribeAmount,
            consequenceDescription = source.consequenceDescription,
            casualtiesIfWrong = source.casualtiesIfWrong,
            creditPenalty = source.creditPenalty,
            
            // References to original data
            shipTypeData = source.shipTypeData,
            captainTypeData = source.captainTypeData,
            scenarioData = source.scenarioData
        };
        
        return enhanced;
    }
}