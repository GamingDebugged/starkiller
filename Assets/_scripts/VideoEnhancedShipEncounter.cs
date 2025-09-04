using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Enhanced ship encounter data with video support
/// Further extends EnhancedShipEncounter with video playback capabilities
/// </summary>
[System.Serializable]
public class VideoEnhancedShipEncounter : EnhancedShipEncounter
{
    // Video references
    public VideoClip shipVideo;        // Video clip of the ship
    public VideoClip captainVideo;     // Video clip of the captain
    public VideoClip scenarioVideo;    // Video clip for the scenario

    // Helper methods to check for videos
    public bool HasShipVideo() => shipVideo != null;
    public bool HasCaptainVideo() => captainVideo != null;
    
    // Convert from EnhancedShipEncounter to VideoEnhancedShipEncounter
    public static VideoEnhancedShipEncounter FromEnhancedEncounter(EnhancedShipEncounter source)
    {
        if (source == null)
            return null;
            
        VideoEnhancedShipEncounter videoEnhanced = new VideoEnhancedShipEncounter
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
            
            // EnhancedShipEncounter properties
            shipImage = source.shipImage,
            captainPortrait = source.captainPortrait,
            
            // References to original data
            shipTypeData = source.shipTypeData,
            captainTypeData = source.captainTypeData,
            scenarioData = source.scenarioData
        };
        
        return videoEnhanced;
    }
}