using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// ScriptableObject for ship types in Starkiller Base Command
/// These define specific vessel types with their properties and validation rules
/// </summary>
[CreateAssetMenu(fileName = "New Ship Type", menuName = "Starkiller Base/Ship Type")]
public class ShipType : ScriptableObject
{
    [Header("Basic Information")]
    public string typeName;
    public ShipCategory category;
    
    public enum SizeClass { Small, Medium, Large, VeryLarge, Enormous }
    public SizeClass size;
    
    [Header("Ship Names")]
    [Tooltip("Specific ship names for this type (e.g., 'Executor', 'Devastator')")]
    public List<string> specificShipNames = new List<string>();
    
    [Header("Crew Information")]
    public int minCrewSize;
    public int maxCrewSize;
    
    [Header("Operations")]
    public string[] commonOrigins;
    public string[] validPurposes;
    public string[] suspiciousIndicators;
    
    [Header("Descriptions")]
    [TextArea(3, 6)]
    public string visualDescription;
    [TextArea(2, 4)]
    public string technicalSpecification;
    
    [Header("Visual Assets")]
    [Tooltip("Primary ship icon used in the log book and UI")]
    public Sprite shipIcon;
    [Tooltip("Different visual variants for ship images")]
    public Sprite[] shipVariations;
    [Tooltip("Default video clip to use for this ship type")]
    public VideoClip defaultShipVideo;
    [Tooltip("Multiple video clips to randomly choose from")]
    public VideoClip[] videoClipPossibilities;
    
    [Header("Story Elements")]
    public bool canBeInfiltrated; // Can this ship be used by Insurgents
    public bool canBeUsedByOrder; // Can this ship be used by The Order
    public bool canSmuggle; // Suitable for smuggling operations
    
    /// <summary>
    /// Get a formatted string with all technical details for display
    /// </summary>
    public string GetTechnicalDetails()
    {
        string details = $"<b>{typeName}</b>\n";
        details += $"Classification: {size} vessel\n";
        details += $"Standard Crew: {minCrewSize}-{maxCrewSize}\n";
        details += $"Category: {(category != null ? category.categoryName : "Unclassified")}\n\n";
        details += technicalSpecification;
        
        return details;
    }
    
    /// <summary>
    /// Get a random ship name from the list of specific names
    /// </summary>
    /// <returns>A random ship name or null if none are defined</returns>
    public string GetRandomShipName()
    {
        if (specificShipNames == null || specificShipNames.Count == 0)
            return null;
            
        return specificShipNames[Random.Range(0, specificShipNames.Count)];
    }
    
    /// <summary>
    /// Get a random video clip for this ship type
    /// </summary>
    /// <returns>A random video clip or the default if none available</returns>
    public VideoClip GetRandomShipVideo()
    {
        if (videoClipPossibilities == null || videoClipPossibilities.Length == 0)
            return defaultShipVideo;
            
        return videoClipPossibilities[Random.Range(0, videoClipPossibilities.Length)];
    }
}