using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Video;

/// <summary>
/// ScriptableObject for ship scenarios in Starkiller Base Command
/// These define different encounter situations that can occur
/// </summary>
[CreateAssetMenu(fileName = "New Scenario", menuName = "Starkiller Base/Scenario")]
public class ShipScenario : ScriptableObject
{
    [Header("Basic Information")]
    public string scenarioName;
    public enum ScenarioType { Standard, Problem, Invalid, StoryEvent, Inspection }
    public ScenarioType type;
    
    [Header("Applicability")]
    public ShipType[] applicableShipTypes;
    public CaptainType[] applicableCaptainTypes;
    public int dayFirstAppears = 1; // For story progression
    public int maxAppearances = -1; // -1 for unlimited
    
    [Header("Scenario Content")]
    [TextArea(3, 6)]
    public string[] possibleStories; // Different narrative descriptions
    public string[] possibleManifests; // Cargo/passenger descriptions
    
    [Header("Special Rules")]
    public bool shouldBeApproved = true; // Should this be approved or denied
    public string invalidReason; // If this is an invalid scenario
    public bool offersBribe;
    [Range(0, 1)]
    public float bribeChanceMultiplier = 1f;
    
    [Header("Consequences")]
    public SeverityLevel severityLevel = SeverityLevel.Minor;
    
    public enum SeverityLevel { Minor, Moderate, Severe }
    [TextArea(2, 4)]
    public string[] possibleConsequences; // What happens if player makes wrong decision
    public int minCasualties;
    public int maxCasualties;
    
    [Header("Story Elements")]
    public bool isStoryMission;
    public string storyTag; // E.g., "insurgent", "order", "imperium_corruption"
    
    [Header("Presentation Videos")]
    [Tooltip("Captain's initial greeting/presentation video")]
    public VideoClip greetingVideo;
    [Tooltip("Story situation video - shows the narrative context")]
    public VideoClip storyVideo;
    [Tooltip("Alternative greeting videos for replay variety")]
    public VideoClip[] alternativeGreetingVideos;
    
    [Header("Player Decision Response Videos")]
    [Tooltip("Video when player approves the ship")]
    public VideoClip approveVideo;
    [Tooltip("Video when player denies the ship")]
    public VideoClip denyVideo;
    [Tooltip("Video when ship is placed in holding pattern")]
    public VideoClip holdingPatternVideo;
    [Tooltip("Video when ship is captured with tractor beam")]
    public VideoClip tractorBeamVideo;
    
    [Header("Consequence Videos")]
    [Tooltip("Videos showing consequences of correct decisions")]
    public VideoClip[] correctDecisionConsequenceVideos;
    [Tooltip("Videos showing consequences of wrong decisions")]
    public VideoClip[] wrongDecisionConsequenceVideos;
    
    [Header("Special Event Videos")]
    [Tooltip("Video clip to play during inspection scenarios")]
    public VideoClip inspectionVideo;
    [Tooltip("Additional special event videos")]
    public VideoClip[] specialEventVideos;
    
    [Header("Audio Elements")]
    [Tooltip("Audio clip for scenario introduction")]
    public AudioClip introAudioClip;
    [Tooltip("Audio clip for dramatic moments")]
    public AudioClip dramaticAudioClip;
    [Tooltip("Audio clip for consequence moments")]
    public AudioClip consequenceAudioClip;
    
    /// <summary>
    /// Check if this scenario can be applied to the given ship and captain
    /// </summary>
    public bool IsApplicableTo(ShipType shipType, CaptainType captainType)
    {
        bool shipApplicable = System.Array.Exists(applicableShipTypes, s => s == shipType) || applicableShipTypes.Length == 0;
        bool captainApplicable = System.Array.Exists(applicableCaptainTypes, c => c == captainType) || applicableCaptainTypes.Length == 0;
        
        return shipApplicable && captainApplicable;
    }
    
    /// <summary>
    /// Check if this scenario is an inspection type
    /// </summary>
    public bool IsInspectionScenario()
    {
        return type == ScenarioType.Inspection;
    }
    
    /// <summary>
    /// Get a random story description from this scenario
    /// </summary>
    public string GetRandomStory()
    {
        if (possibleStories.Length > 0)
        {
            return possibleStories[Random.Range(0, possibleStories.Length)];
        }
        return "Ship requesting standard docking clearance.";
    }
    
    /// <summary>
    /// Get a random manifest from this scenario
    /// </summary>
    public string GetRandomManifest()
    {
        if (possibleManifests.Length > 0)
        {
            return possibleManifests[Random.Range(0, possibleManifests.Length)];
        }
        return "Standard supplies and personnel.";
    }
    
    /// <summary>
    /// Get a random consequence description from this scenario
    /// </summary>
    public string GetRandomConsequence(out int casualties)
    {
        casualties = Random.Range(minCasualties, maxCasualties + 1);
        
        if (possibleConsequences.Length > 0)
        {
            string consequence = possibleConsequences[Random.Range(0, possibleConsequences.Length)];
            return consequence.Replace("{CASUALTIES}", casualties.ToString());
        }
        
        if (casualties > 0)
        {
            return $"Security breach resulted in {casualties} Imperium casualties.";
        }
        return "Security protocols were violated.";
    }
    
    /// <summary>
    /// Get the greeting video for initial presentation
    /// </summary>
    public VideoClip GetGreetingVideo()
    {
        // Return main greeting video if available
        if (greetingVideo != null)
        {
            return greetingVideo;
        }
        
        // Try alternative greeting videos
        if (alternativeGreetingVideos != null && alternativeGreetingVideos.Length > 0)
        {
            var validVideos = System.Array.FindAll(alternativeGreetingVideos, v => v != null);
            if (validVideos.Length > 0)
            {
                return validVideos[Random.Range(0, validVideos.Length)];
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Get the story video that shows the narrative situation
    /// </summary>
    public VideoClip GetStoryVideo()
    {
        return storyVideo;
    }
    
    /// <summary>
    /// Get video for player decision response
    /// </summary>
    public VideoClip GetDecisionResponseVideo(MasterShipEncounter.DecisionState decision)
    {
        switch (decision)
        {
            case MasterShipEncounter.DecisionState.Approved:
                return approveVideo;
            case MasterShipEncounter.DecisionState.Denied:
                return denyVideo;
            case MasterShipEncounter.DecisionState.HoldingPattern:
                return holdingPatternVideo;
            case MasterShipEncounter.DecisionState.TractorBeam:
                return tractorBeamVideo;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// Get consequence video for PersonalDataLog based on decision correctness
    /// </summary>
    public VideoClip GetConsequenceVideo(bool wasCorrectDecision)
    {
        VideoClip[] videoArray = wasCorrectDecision ? correctDecisionConsequenceVideos : wrongDecisionConsequenceVideos;
        
        if (videoArray != null && videoArray.Length > 0)
        {
            var validVideos = System.Array.FindAll(videoArray, v => v != null);
            if (validVideos.Length > 0)
            {
                return validVideos[Random.Range(0, validVideos.Length)];
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Get inspection or special event video
    /// </summary>
    public VideoClip GetSpecialEventVideo()
    {
        // Return inspection video if this is an inspection scenario
        if (type == ScenarioType.Inspection && inspectionVideo != null)
        {
            return inspectionVideo;
        }
        
        // Otherwise try special event videos
        if (specialEventVideos != null && specialEventVideos.Length > 0)
        {
            var validVideos = System.Array.FindAll(specialEventVideos, v => v != null);
            if (validVideos.Length > 0)
            {
                return validVideos[Random.Range(0, validVideos.Length)];
            }
        }
        
        return inspectionVideo; // Fallback for backwards compatibility
    }
    
    /// <summary>
    /// Get audio clip for different scenario moments
    /// </summary>
    public AudioClip GetAudioClip(AudioMoment moment)
    {
        switch (moment)
        {
            case AudioMoment.Introduction:
                return introAudioClip;
            case AudioMoment.Dramatic:
                return dramaticAudioClip;
            case AudioMoment.Consequence:
                return consequenceAudioClip;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// Audio moment types for different parts of the scenario
    /// </summary>
    public enum AudioMoment
    {
        Introduction,
        Dramatic,
        Consequence
    }
    
    /// <summary>
    /// Check if this scenario has complete video content for story presentation
    /// </summary>
    public bool HasCompleteVideoContent()
    {
        return greetingVideo != null && 
               (approveVideo != null || denyVideo != null) &&
               (correctDecisionConsequenceVideos?.Length > 0 || wrongDecisionConsequenceVideos?.Length > 0);
    }
    
    /// <summary>
    /// Get a video clip for this scenario (legacy method for backwards compatibility)
    /// </summary>
    public VideoClip GetVideoClip()
    {
        // Prioritize story presentation videos for scenarios
        if (isStoryMission)
        {
            return GetGreetingVideo() ?? GetStoryVideo() ?? GetSpecialEventVideo();
        }
        
        // Fall back to special events
        return GetSpecialEventVideo();
    }
}