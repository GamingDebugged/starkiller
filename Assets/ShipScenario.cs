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
    
    [Header("Media Elements")]
    [Tooltip("Video clip to play during inspection scenarios")]
    public VideoClip inspectionVideo;
    [Tooltip("Alternative video clips for variety")]
    public VideoClip[] alternativeVideos;
    [Tooltip("Audio clip for special scenario events")]
    public AudioClip scenarioAudioClip;
    
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
    /// Get a video clip for this scenario (primarily for inspections)
    /// </summary>
    public VideoClip GetVideoClip()
    {
        // Return the main inspection video if available
        if (inspectionVideo != null)
        {
            return inspectionVideo;
        }
        
        // Otherwise try alternative videos
        if (alternativeVideos != null && alternativeVideos.Length > 0)
        {
            // Filter out null entries
            var validVideos = System.Array.FindAll(alternativeVideos, v => v != null);
            if (validVideos.Length > 0)
            {
                return validVideos[Random.Range(0, validVideos.Length)];
            }
        }
        
        return null;
    }
}