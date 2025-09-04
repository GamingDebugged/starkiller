using UnityEngine;
using System.Collections.Generic;
using StarkillerBaseCommand;

/// <summary>
/// Bridge class to forward calls to MasterShipGenerator
/// This script is part of the migration strategy to replace legacy systems
/// </summary>
public class ShipEncounterGenerator : MonoBehaviour
{
    public MasterShipGenerator masterShipGenerator;
    
    // Singleton pattern
    private static ShipEncounterGenerator _instance;
    public static ShipEncounterGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ShipEncounterGenerator>();
                
                if (_instance == null)
                {
                    GameObject go = new GameObject("ShipEncounterGenerator_Bridge");
                    _instance = go.AddComponent<ShipEncounterGenerator>();
                }
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        if (masterShipGenerator == null)
            masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
    }
    
    public ShipEncounter GenerateRandomEncounter(bool forceValid = false)
    {
        if (masterShipGenerator != null)
        {
            MasterShipEncounter masterEncounter = masterShipGenerator.GenerateRandomEncounter(forceValid);
            
            // Convert to regular ShipEncounter for legacy compatibility
            ShipEncounter legacyEncounter = new ShipEncounter();
            legacyEncounter.shipType = masterEncounter.shipType;
            legacyEncounter.destination = masterEncounter.destination;
            legacyEncounter.origin = masterEncounter.origin;
            legacyEncounter.accessCode = masterEncounter.accessCode;
            legacyEncounter.story = masterEncounter.story;
            legacyEncounter.manifest = masterEncounter.manifest;
            legacyEncounter.crewSize = masterEncounter.crewSize;
            legacyEncounter.shouldApprove = masterEncounter.shouldApprove;
            legacyEncounter.invalidReason = masterEncounter.invalidReason;
            legacyEncounter.captainName = masterEncounter.captainName;
            legacyEncounter.captainRank = masterEncounter.captainRank;
            legacyEncounter.captainFaction = masterEncounter.captainFaction;
            legacyEncounter.isStoryShip = masterEncounter.isStoryShip;
            legacyEncounter.storyTag = masterEncounter.storyTag;
            legacyEncounter.offersBribe = masterEncounter.offersBribe;
            legacyEncounter.bribeAmount = masterEncounter.bribeAmount;
            legacyEncounter.consequenceDescription = masterEncounter.consequenceDescription;
            legacyEncounter.casualtiesIfWrong = masterEncounter.casualtiesIfWrong;
            legacyEncounter.creditPenalty = masterEncounter.creditPenalty;
            
            return legacyEncounter;
        }
        
        // Fallback to basic ship
        ShipEncounter ship = new ShipEncounter();
        ship.shipType = "Imperial Shuttle";
        ship.destination = "Imperium Base";
        ship.origin = "Imperial Fleet";
        ship.accessCode = "SK-7429";
        ship.story = "Routine mission.";
        ship.manifest = "Standard supplies.";
        ship.captainName = "Captain Default";
        ship.shouldApprove = forceValid;
        return ship;
    }
    
    public void StartNewDay(int day)
    {
        if (masterShipGenerator != null)
        {
            masterShipGenerator.StartNewDay(day);
        }
    }
}
