using UnityEngine;
using System.Collections.Generic;
using StarkillerBaseCommand;

/// <summary>
/// Bridge class to forward calls to MasterShipGenerator
/// This script is part of the migration strategy to replace legacy systems
/// </summary>
public class ShipEncounterSystem : MonoBehaviour
{
    public MasterShipGenerator masterShipGenerator;
    
    // Properties required by legacy code
    private List<string> _validAccessCodes = new List<string>();
    public List<string> validAccessCodes 
    { 
        get { return _validAccessCodes; } 
        set { _validAccessCodes = value; } 
    }
    
    // Add Length property to mimic array behavior for lists
    public int Length
    {
        get { return validAccessCodes.Count; }
    }
    
    // Add this extension method to make validAccessCodes appear to have a Length property
    public int GetLength() { return _validAccessCodes.Count; }
    
    public float validShipChance = 0.7f;
    public float storyShipChance = 0.2f;
    
    // Singleton pattern
    private static ShipEncounterSystem _instance;
    public static ShipEncounterSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ShipEncounterSystem>();
                
                if (_instance == null)
                {
                    GameObject go = new GameObject("ShipEncounterSystem_Bridge");
                    _instance = go.AddComponent<ShipEncounterSystem>();
                }
            }
            return _instance;
        }
    }
    
    [System.Serializable]
    public class Destination
    {
        public string name;
        public string[] validOrigins;
    }
    
    private List<ShipType> _shipTypes = new List<ShipType>();
    public List<ShipType> shipTypes 
    { 
        get { return _shipTypes; } 
        set { _shipTypes = value; }
    }
    
    // Add Length property to shipTypes to mimic array behavior
    public int ShipTypesLength
    {
        get { return _shipTypes.Count; }
    }
    
    public List<Destination> destinations = new List<Destination>();
    
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
            
        RegenerateAccessCodes();
    }
    
    public ShipEncounter GenerateEncounter(int currentDay = 1, int imperialLoyalty = 0, int rebellionSympathy = 0)
    {
        ShipEncounter ship = CreateTestShip();
        return ship;
    }
    
    public ShipEncounter CreateTestShip()
    {
        ShipEncounter testShip = new ShipEncounter();
        testShip.shipType = "Lambda Shuttle";
        testShip.destination = "Starkiller Base";
        testShip.origin = "Imperial Fleet";
        testShip.accessCode = "SK-7429";
        testShip.story = "Ship reports routine delivery.";
        testShip.manifest = "Supplies and personnel.";
        testShip.captainName = "Captain Test";
        testShip.shouldApprove = true;
        return testShip;
    }
    
    public void RegenerateAccessCodes()
    {
        _validAccessCodes.Clear();
        _validAccessCodes.Add("SK-1234");
        _validAccessCodes.Add("SK-5678");
        _validAccessCodes.Add("SK-9012");
    }
    
    public List<string> GetDailyRules(int currentDay)
    {
        List<string> rules = new List<string>();
        rules.Add("Valid access codes: " + string.Join(", ", _validAccessCodes));
        return rules;
    }
}