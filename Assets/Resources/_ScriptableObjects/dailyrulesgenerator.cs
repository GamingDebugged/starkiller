using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates the daily rules and regulations for Starkiller Base Command
/// This controls the changing validation rules as days progress
/// </summary>
public class DailyRulesGenerator : MonoBehaviour
{
    [System.Serializable]
    public class RuleSet
    {
        public string ruleName;
        [TextArea(2, 5)]
        public string ruleDescription;
        public int firstAppearanceDay = 1;
        public bool isMandatory = false;
    }
    
    [Header("Rule Settings")]
    [SerializeField] private List<RuleSet> possibleRules = new List<RuleSet>();
    [SerializeField] private int baseRuleCount = 3; // Basic number of rules per day
    [SerializeField] private int maxRuleCount = 7;  // Maximum rules at higher difficulty
    
    [Header("Access Code Settings")]
    [SerializeField] private string[] defaultPrefixes = { "SK-" };
    [SerializeField] private int baseAccessCodeCount = 3;
    [SerializeField] private int maxAccessCodeCount = 5;
    
    [Header("Current Day Rules")]
    [SerializeField] private List<string> currentRules = new List<string>();
    [SerializeField] private List<string> currentAccessCodes = new List<string>();
    [SerializeField] private List<string> validShipTypes = new List<string>();
    [SerializeField] private List<string> validOrigins = new List<string>();
    
    [Header("Current Day")]
    [SerializeField] private int currentDay = 1;
    
    // Singleton pattern for easy access
    private static DailyRulesGenerator _instance;
    public static DailyRulesGenerator Instance
    {
        get
        {
            return _instance;
        }
    }
    
    // Public accessors
    public List<string> CurrentRules => currentRules;
    public List<string> CurrentAccessCodes => currentAccessCodes;
    public List<string> ValidShipTypes => validShipTypes;
    public List<string> ValidOrigins => validOrigins;
    
    void Awake()
    {
        // Setup singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
    }
    
    void Start()
    {
        // Generate initial rules
        GenerateRulesForDay(currentDay);
    }
    
    /// <summary>
    /// Generate rules for a new day
    /// </summary>
    public void GenerateRulesForDay(int day)
    {
        currentDay = day;
        
        // Clear previous rules
        currentRules.Clear();
        
        // Generate access codes
        GenerateAccessCodes();
        
        // Add access code rule
        string accessCodeRule = "Valid access codes: " + string.Join(", ", currentAccessCodes);
        currentRules.Add(accessCodeRule);
        
        // Calculate how many rules to apply based on day
        int ruleCount = Mathf.Min(baseRuleCount + (day / 2), maxRuleCount);
        
        // Get all rules available for this day
        List<RuleSet> availableRules = new List<RuleSet>();
        List<RuleSet> mandatoryRules = new List<RuleSet>();
        
        foreach (var rule in possibleRules)
        {
            if (rule.firstAppearanceDay <= day)
            {
                if (rule.isMandatory)
                {
                    mandatoryRules.Add(rule);
                }
                else
                {
                    availableRules.Add(rule);
                }
            }
        }
        
        // Add all mandatory rules
        foreach (var rule in mandatoryRules)
        {
            currentRules.Add(rule.ruleDescription);
        }
        
        // Randomly select from available rules until we reach our rule count
        int remainingRules = ruleCount - mandatoryRules.Count;
        
        // Shuffle available rules
        for (int i = 0; i < availableRules.Count; i++)
        {
            int randomIndex = Random.Range(i, availableRules.Count);
            RuleSet temp = availableRules[i];
            availableRules[i] = availableRules[randomIndex];
            availableRules[randomIndex] = temp;
        }
        
        // Add random rules up to our count
        for (int i = 0; i < Mathf.Min(remainingRules, availableRules.Count); i++)
        {
            currentRules.Add(availableRules[i].ruleDescription);
        }
        
        // Special day-based rules
        if (day % 3 == 0)
        {
            currentRules.Add("Imperium inspection day. All documentation must be perfect.");
        }
        
        if (day % 5 == 0)
        {
            currentRules.Add("The Order representatives expected. Priority access must be granted.");
        }
        
        // Generate list of approved ship types for today
        GenerateValidShipTypes();
        
        // Generate list of valid origins for today
        GenerateValidOrigins();
    }
    
    /// <summary>
    /// Generate new access codes for the day
    /// </summary>
    private void GenerateAccessCodes()
    {
        currentAccessCodes.Clear();
        
        // Number of codes increases slightly with day
        int codeCount = Mathf.Min(baseAccessCodeCount + (currentDay / 4), maxAccessCodeCount);
        
        // Possible prefixes expands with day
        List<string> prefixes = new List<string>(defaultPrefixes);
        
        if (currentDay > 3)
        {
            prefixes.Add("IM-"); // Imperium prefix
        }
        
        if (currentDay > 5)
        {
            prefixes.Add("FO-"); // First Order prefix
        }
        
        // Generate codes
        for (int i = 0; i < codeCount; i++)
        {
            string prefix = prefixes[Random.Range(0, prefixes.Count)];
            string code = prefix + Random.Range(1000, 10000).ToString();
            currentAccessCodes.Add(code);
        }
    }
    
    /// <summary>
    /// Generate list of valid ship types for today
    /// </summary>
    private void GenerateValidShipTypes()
    {
        validShipTypes.Clear();
        
        // Get all ship types from the ShipEncounterGenerator
        ShipEncounterGenerator generator = ShipEncounterGenerator.Instance;
        
        if (generator != null)
        {
            // This would be better with a direct reference or event system
            // For now, we'll just add standard ship types
            
            validShipTypes.Add("Orion Shuttle");
            validShipTypes.Add("Imperium Scout Ship");
            validShipTypes.Add("Colonial Freighter");
            
            // Add some variety based on day
            if (currentDay % 2 == 0)
            {
                validShipTypes.Add("Imperium Frigate");
                validShipTypes.Add("Corvette");
            }
            else
            {
                validShipTypes.Add("Stellar Destroyer");
                validShipTypes.Add("Mining Cargo Vessel");
            }
            
            // Special cases
            if (currentDay % 5 == 0)
            {
                validShipTypes.Add("Pilgrimage Drifter"); // The Order ships
            }
        }
        else
        {
            // Fallback ship types
            validShipTypes.Add("Orion Shuttle");
            validShipTypes.Add("Imperium Scout Ship");
            validShipTypes.Add("Imperium Frigate");
            validShipTypes.Add("Mining Cargo Vessel");
            validShipTypes.Add("Stellar Destroyer");
        }
    }
    
    /// <summary>
    /// Generate list of valid origins for today
    /// </summary>
    private void GenerateValidOrigins()
    {
        validOrigins.Clear();
        
        // Base origins always allowed
        validOrigins.Add("Central Fleet");
        validOrigins.Add("Imperium Academy");
        validOrigins.Add("Stellar Destroyer Finalizer");
        
        // Add variety based on day
        if (currentDay % 2 == 0)
        {
            validOrigins.Add("Mining Colony");
            validOrigins.Add("Imperium Outpost");
        }
        else
        {
            validOrigins.Add("Kuat Shipyards");
            validOrigins.Add("Imperium Supply Depot");
        }
        
        // Special case
        if (currentDay % 5 == 0)
        {
            validOrigins.Add("Order Temple"); // The Order origins
        }
    }
    
    /// <summary>
    /// Check if a ship type is valid for today
    /// </summary>
    public bool IsShipTypeValid(string shipType)
    {
        return validShipTypes.Contains(shipType);
    }
    
    /// <summary>
    /// Check if an origin is valid for today
    /// </summary>
    public bool IsOriginValid(string origin)
    {
        return validOrigins.Contains(origin);
    }
    
    /// <summary>
    /// Check if an access code is valid for today
    /// </summary>
    public bool IsAccessCodeValid(string accessCode)
    {
        return currentAccessCodes.Contains(accessCode);
    }
}