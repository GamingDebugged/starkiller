using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using StarkillerBaseCommand;

public class FactionManager : MonoBehaviour
{
    [Header("Faction Database")]
    public List<Faction> allFactions = new List<Faction>();
    
    [Header("Settings")]
    public Faction defaultFaction;
    
    private static FactionManager _instance;
    public static FactionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<FactionManager>();
                if (_instance == null)
                {
                    Debug.LogWarning("FactionManager not found in scene!");
                }
            }
            return _instance;
        }
    }
    
    private Dictionary<string, Faction> factionLookup;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Use safe DontDestroyOnLoad helper
        this.SafeDontDestroyOnLoad();
        
        BuildFactionLookup();
    }
    
    private void BuildFactionLookup()
    {
        factionLookup = new Dictionary<string, Faction>();
        
        foreach (var faction in allFactions)
        {
            if (faction != null && !string.IsNullOrEmpty(faction.factionID))
            {
                string key = faction.factionID.ToLower();
                if (!factionLookup.ContainsKey(key))
                {
                    factionLookup.Add(key, faction);
                }
                
                string displayKey = faction.displayName.ToLower();
                if (!factionLookup.ContainsKey(displayKey))
                {
                    factionLookup.Add(displayKey, faction);
                }
            }
        }
        
        Debug.Log($"FactionManager: Loaded {factionLookup.Count} faction entries");
    }
    
    public Faction GetFaction(string factionIdentifier)
    {
        if (string.IsNullOrEmpty(factionIdentifier))
            return defaultFaction;
            
        string key = factionIdentifier.ToLower();
        
        if (factionLookup != null && factionLookup.ContainsKey(key))
        {
            return factionLookup[key];
        }
        
        Debug.LogWarning($"FactionManager: Faction '{factionIdentifier}' not found, using default");
        return defaultFaction;
    }
    
    public List<Faction> GetFactionsForAccessCode(string accessCodePrefix)
    {
        List<Faction> validFactions = new List<Faction>();
        
        foreach (var faction in allFactions)
        {
            if (faction != null && faction.CanUseAccessCodePrefix(accessCodePrefix))
            {
                validFactions.Add(faction);
            }
        }
        
        return validFactions;
    }
    
    public List<Faction> GetFactionsForShipCategory(ShipCategory category)
    {
        List<Faction> validFactions = new List<Faction>();
        
        foreach (var faction in allFactions)
        {
            if (faction != null && faction.CanUseShipCategory(category))
            {
                validFactions.Add(faction);
            }
        }
        
        return validFactions;
    }
    
    public bool ValidateEncounterConsistency(ShipCategory shipCategory, string captainFaction, string accessCode)
    {
        if (shipCategory == null)
        {
            Debug.LogError("FactionManager: Cannot validate - shipCategory is null");
            return false;
        }
        
        Faction captainFactionObj = GetFaction(captainFaction);
        if (captainFactionObj == null)
        {
            Debug.LogWarning($"FactionManager: Captain faction '{captainFaction}' not found");
            return false;
        }
        
        if (!captainFactionObj.CanUseShipCategory(shipCategory))
        {
            Debug.LogWarning($"FactionManager: Faction '{captainFactionObj.displayName}' cannot use ship category '{shipCategory.categoryName}'");
            return false;
        }
        
        if (!string.IsNullOrEmpty(accessCode))
        {
            string prefix = accessCode.Split('-')[0];
            if (!captainFactionObj.CanUseAccessCodePrefix(prefix))
            {
                Debug.LogWarning($"FactionManager: Faction '{captainFactionObj.displayName}' cannot use access code prefix '{prefix}'");
                return false;
            }
        }
        
        return true;
    }
    
    public List<Faction> GetCompatibleFactions(ShipCategory shipCategory, string accessCodePrefix)
    {
        HashSet<Faction> compatibleFactions = new HashSet<Faction>();
        
        var shipFactions = GetFactionsForShipCategory(shipCategory);
        var codeFactions = GetFactionsForAccessCode(accessCodePrefix);
        
        foreach (var faction in shipFactions)
        {
            if (codeFactions.Contains(faction))
            {
                compatibleFactions.Add(faction);
            }
        }
        
        return compatibleFactions.ToList();
    }
    
    public void LoadFactionsFromResources()
    {
        allFactions.Clear();
        
        string[] searchPaths = {
            "Factions",
            "_ScriptableObjects/Factions",
            "ScriptableObjects/Factions",
            "Data/Factions"
        };
        
        foreach (string path in searchPaths)
        {
            Faction[] loadedFactions = Resources.LoadAll<Faction>(path);
            if (loadedFactions != null && loadedFactions.Length > 0)
            {
                allFactions.AddRange(loadedFactions);
                Debug.Log($"FactionManager: Loaded {loadedFactions.Length} factions from {path}");
            }
        }
        
        BuildFactionLookup();
        
        if (allFactions.Count == 0)
        {
            Debug.LogWarning("FactionManager: No factions found in Resources!");
        }
    }
}