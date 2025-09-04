using UnityEngine;

/// <summary>
/// Class representing a ship encounter in Starkiller Base Command
/// This is the runtime data generated from the ScriptableObjects
/// </summary>
[System.Serializable]
public class ShipEncounter
{
    // Basic ship information
    public string shipType;        // Type of ship
    public string shipName;        // Name of the specific ship (optional)
    public string destination;     // Where they're going
    public string origin;          // Where they're coming from
    public string accessCode;      // Their access code
    public string story;           // Their story/situation
    public string manifest;        // Cargo manifest
    public int crewSize;           // Number of crew members
    
    // Validation data
    public bool shouldApprove;     // Whether they should be approved
    public string invalidReason;   // Reason they should be denied (if applicable)
    
    // Captain information
    public string captainName;     // Name of the ship captain
    public string captainRank;     // Rank of the captain
    public string captainFaction;  // Faction the captain belongs to
    
    // Special elements
    public bool isStoryShip;       // Is this a special story ship
    public string storyTag;        // Tag for story type (insurgent, imperium, order)
    
    // Bribe information
    public bool offersBribe;       // Does this ship offer a bribe?
    public int bribeAmount;        // Amount of credits offered as bribe
    
    // Consequence information
    public string consequenceDescription; // What happens if wrong decision is made
    public int casualtiesIfWrong;  // Number of casualties if wrong decision made
    public int creditPenalty;      // Credits lost if wrong decision made
    
    // Reference to original scriptable objects (optional)
    [System.NonSerialized]
    public ShipType shipTypeData;
    [System.NonSerialized]
    public CaptainType captainTypeData;
    [System.NonSerialized]
    public ShipScenario scenarioData;
    
    // Helper method to get formatted ship info for display
    public virtual string GetShipInfo()
    {
        string info = $"<b>{shipType}</b> requesting access\n";
        
        if (!string.IsNullOrEmpty(captainName))
        {
            info += $"Captain: {captainName}\n";
        }
        
        info += $"\n<i>\"{story}\"</i>\n\n" +
               $"Destination: {destination}\n" +
               $"Origin: {origin}\n" +
               $"Crew Size: {crewSize}";
               
        return info;
    }
    
    // Helper method to get formatted credentials info for display
    public virtual string GetCredentialsInfo()
    {
        string credentials = $"<b>Access Code:</b> {accessCode}\n\n" +
                           $"<b>Manifest:</b> {manifest}";
                           
        // Add bribe information if applicable
        if (offersBribe)
        {
            credentials += $"\n\n<color=yellow>[Captain offers {bribeAmount} credits for quick approval]</color>";
        }
        
        return credentials;
    }
    
    // Get approval recommendation based on rules for this day
    public string GetApprovalRecommendation(string[] validAccessCodes, string[] approvedOrigins)
    {
        string recommendation = "";
        
        // Check access code
        bool validCode = System.Array.Exists(validAccessCodes, code => code == accessCode);
        if (!validCode)
        {
            recommendation += "- Invalid access code\n";
        }
        
        // Check origin
        bool validOrigin = System.Array.Exists(approvedOrigins, origin => origin == this.origin);
        if (!validOrigin)
        {
            recommendation += "- Origin not authorized\n";
        }
        
        // Other checks can be added here
        
        // Return overall recommendation
        if (string.IsNullOrEmpty(recommendation))
        {
            return "All credentials appear valid.";
        }
        else
        {
            return "The following issues were found:\n" + recommendation;
        }
    }
}