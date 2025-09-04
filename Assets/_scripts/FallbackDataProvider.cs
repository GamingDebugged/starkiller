using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Provides fallback data for ship generation
/// This helps fix the "No valid scenarios found, using fallback" warnings
/// </summary>
public class FallbackDataProvider : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool verboseLogging = true;
    
    private void Start()
    {
        LogFallbackStatus();
        
        // Find the ShipGeneratorCoordinator
        ShipGeneratorCoordinator coordinator = FindFirstObjectByType<ShipGeneratorCoordinator>();
        if (coordinator != null)
        {
            if (verboseLogging)
            {
                Debug.Log("FallbackDataProvider: Found ShipGeneratorCoordinator");
            }
        }
        else
        {
            Debug.LogWarning("FallbackDataProvider: ShipGeneratorCoordinator not found");
        }
    }
    
    private void LogFallbackStatus()
    {
        if (!verboseLogging) return;
        
        Debug.Log("FallbackDataProvider: Basic fallback ship data ready");
    }
    
    /// <summary>
    /// Get basic fallback data for a ship encounter
    /// </summary>
    public static Dictionary<string, object> GetBasicShipData(bool shouldBeValid = true)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        
        // Create ship data based on validity
        if (shouldBeValid)
        {
            data["shipType"] = "Orion Shuttle";
            data["destination"] = "Imperium Base";
            data["origin"] = "Imperial Fleet";
            data["accessCode"] = "SK-" + Random.Range(1000, 10000).ToString();
            data["story"] = "Ship requests routine clearance for standard procedures.";
            data["manifest"] = "Standard supplies and maintenance equipment.";
            data["captainName"] = "Captain Altus";
            data["captainRank"] = "Lieutenant";
            data["captainFaction"] = "imperium";
            data["shouldApprove"] = true;
            data["crewSize"] = 5;
        }
        else
        {
            data["shipType"] = "Unknown Transport";
            data["destination"] = "Imperium Base";
            data["origin"] = "Unverified Origin";
            data["accessCode"] = "XX-" + Random.Range(1000, 10000).ToString();
            data["story"] = "Ship insists on immediate landing without proper clearance.";
            data["manifest"] = "Undisclosed cargo and personnel.";
            data["captainName"] = "Captain Kelso";
            data["captainRank"] = "Unknown";
            data["captainFaction"] = "unknown";
            data["shouldApprove"] = false;
            data["invalidReason"] = "Invalid access code";
            data["crewSize"] = 3;
        }
        
        return data;
    }
    
    /// <summary>
    /// Create and fill a ShipEncounter with fallback data
    /// </summary>
    public static void FillFallbackData(ShipEncounter encounter, bool shouldBeValid = true)
    {
        if (encounter == null) return;
        
        Dictionary<string, object> data = GetBasicShipData(shouldBeValid);
        
        encounter.shipType = (string)data["shipType"];
        encounter.destination = (string)data["destination"];
        encounter.origin = (string)data["origin"];
        encounter.accessCode = (string)data["accessCode"];
        encounter.story = (string)data["story"];
        encounter.manifest = (string)data["manifest"];
        encounter.captainName = (string)data["captainName"];
        encounter.captainRank = (string)data["captainRank"];
        encounter.captainFaction = (string)data["captainFaction"];
        encounter.shouldApprove = (bool)data["shouldApprove"];
        encounter.crewSize = (int)data["crewSize"];
        
        if (!shouldBeValid)
        {
            encounter.invalidReason = (string)data["invalidReason"];
        }
    }
}
