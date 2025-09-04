using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using StarkillerBaseCommand;

/// <summary>
/// Manager for faction-aware CargoManifest system
/// Handles manifest selection, validation, and backwards compatibility
/// </summary>
public class ManifestManager : MonoBehaviour
{
    private static ManifestManager _instance;
    public static ManifestManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<ManifestManager>();
            return _instance;
        }
    }
    
    [Header("Manifest Database")]
    [SerializeField] private List<CargoManifest> allManifests = new List<CargoManifest>();
    [SerializeField] private bool useManifestSystem = true;
    
    [Header("Faction Manifest Pools")]
    private Dictionary<string, List<CargoManifest>> factionManifestPools = new Dictionary<string, List<CargoManifest>>();
    private Dictionary<string, int> dailyManifestUsage = new Dictionary<string, int>();
    
    [Header("Settings")]
    [SerializeField] private bool allowCrossFactionManifests = true;
    [SerializeField] private float universalManifestChance = 0.3f;
    [SerializeField] private float contrabandChance = 0.15f;
    
    [Header("Fallback Generation")]
    [SerializeField] private string[] commonCargoItems = {
        "Medical supplies", "Food rations", "Equipment parts", "Personnel transport",
        "Construction materials", "Communication equipment", "Power cells", "Standard supplies"
    };
    
    [SerializeField] private string[] contrabandItems = {
        "Unauthorized weapons", "Restricted technology", "Classified documents", 
        "Illegal substances", "Smuggled goods", "Unregistered personnel"
    };
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    void Start()
    {
        LoadManifestsFromResources();
    }
    
    /// <summary>
    /// Initialize manifests from external source (called by ContentManager)
    /// </summary>
    public void InitializeManifests(CargoManifest[] manifests)
    {
        if (manifests == null || manifests.Length == 0)
        {
            Debug.LogWarning("ManifestManager: No manifests provided to InitializeManifests");
            return;
        }
        
        allManifests.Clear();
        allManifests.AddRange(manifests);
        BuildFactionPools();
        Debug.Log($"ManifestManager: Initialized with {allManifests.Count} manifests");
    }
    
    /// <summary>
    /// Load manifests using centralized resource loading
    /// </summary>
    private void LoadManifestsFromResources()
    {
        if (allManifests.Count > 0)
            return; // Already loaded
            
        // Use centralized resource loading
        allManifests = ResourceLoadingHelper.LoadCargoManifests();
        
        BuildFactionPools();
        
        if (allManifests.Count == 0)
        {
            Debug.LogWarning("ManifestManager: No manifests found. System will use fallback generation.");
            
            // Keep system enabled but use runtime generation
            useManifestSystem = true;
        }
        else
        {
            Debug.Log($"ManifestManager: Loaded {allManifests.Count} manifests");
        }
    }
    
    /// <summary>
    /// Build faction-specific manifest pools for efficient lookup
    /// </summary>
    private void BuildFactionPools()
    {
        factionManifestPools.Clear();
        
        foreach (var manifest in allManifests)
        {
            if (manifest == null) continue;
            
            if (manifest.factionRestriction == CargoManifest.FactionRestriction.Universal)
            {
                // Add to all faction pools
                foreach (var faction in GetAllKnownFactions())
                {
                    AddManifestToFactionPool(faction, manifest);
                }
            }
            else if (manifest.allowedFactions != null)
            {
                // Add to allowed factions only
                foreach (var faction in manifest.allowedFactions)
                {
                    if (!string.IsNullOrEmpty(faction))
                    {
                        AddManifestToFactionPool(faction, manifest);
                    }
                }
            }
        }
        
        Debug.Log($"ManifestManager: Built faction pools for {factionManifestPools.Count} factions");
    }
    
    /// <summary>
    /// Select an appropriate manifest for a ship
    /// </summary>
    public CargoManifest SelectManifestForShip(ShipType shipType, string faction, int currentDay)
    {
        if (!useManifestSystem || allManifests.Count == 0)
        {
            Debug.Log("ManifestManager: Manifest system disabled or no manifests available, returning null");
            return null;
        }
        
        List<CargoManifest> validManifests = GetValidManifestsForCriteria(
            faction, 
            shipType?.category?.categoryName ?? "Unknown", 
            currentDay
        );
        
        if (validManifests.Count == 0)
        {
            Debug.LogWarning($"ManifestManager: No valid manifests found for faction {faction} on day {currentDay}");
            return null;
        }
        
        // Filter by daily usage limits
        validManifests = FilterByDailyUsage(validManifests);
        
        if (validManifests.Count == 0)
        {
            Debug.LogWarning($"ManifestManager: All valid manifests have reached daily usage limits");
            return null;
        }
        
        // Weighted random selection based on manifest priority
        CargoManifest selectedManifest = SelectWeightedRandomManifest(validManifests);
        
        if (selectedManifest != null)
        {
            RecordManifestUsage(selectedManifest);
            Debug.Log($"ManifestManager: Selected manifest '{selectedManifest.manifestName}' for {faction}");
        }
        
        return selectedManifest;
    }
    
    /// <summary>
    /// Enhanced version of SelectManifestForShip that uses fallback when no manifests are loaded
    /// </summary>
    public CargoManifest SelectManifestForShipWithFallback(ShipType shipType, string faction, int currentDay)
    {
        // First try the normal manifest selection
        var manifest = SelectManifestForShip(shipType, faction, currentDay);
        
        if (manifest == null && useManifestSystem)
        {
            Debug.Log($"ManifestManager: No ScriptableObject manifest found for {faction}, using procedural generation");
            
            // Create a runtime manifest using the fallback generation
            manifest = CreateRuntimeManifest(faction, shipType, currentDay);
        }
        
        return manifest;
    }
    
    /// <summary>
    /// Create a runtime manifest when no ScriptableObject exists
    /// </summary>
    private CargoManifest CreateRuntimeManifest(string faction, ShipType shipType, int currentDay)
    {
        // Create a new manifest instance at runtime
        CargoManifest runtimeManifest = ScriptableObject.CreateInstance<CargoManifest>();
        
        // Generate a unique code
        runtimeManifest.manifestCode = $"RUNTIME_{faction}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        runtimeManifest.manifestName = $"{faction} Cargo Manifest";
        
        // Set faction restrictions
        runtimeManifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        runtimeManifest.allowedFactions = new string[] { faction };
        
        // Determine if this should have contraband
        bool shouldHaveContraband = Random.value < contrabandChance;
        runtimeManifest.hasContraband = shouldHaveContraband;
        
        if (shouldHaveContraband)
        {
            runtimeManifest.contrabandType = CargoManifest.ContrabandType.Weapons; // Use valid enum value
            // Note: contrabandSeverity doesn't exist in CargoManifest, so we skip it
            
            // Set contraband items
            int contrabandCount = Random.Range(1, 3);
            runtimeManifest.contrabandItems = new string[contrabandCount];
            for (int i = 0; i < contrabandCount; i++)
            {
                runtimeManifest.contrabandItems[i] = contrabandItems[Random.Range(0, contrabandItems.Length)];
            }
        }
        
        // Generate manifest content using the existing fallback method
        string manifestContent = GenerateFallbackManifest(faction, shouldHaveContraband);
        runtimeManifest.manifestDescription = manifestContent; // Use manifestDescription instead of manifestText
        
        // Set declared items from the generated content
        string[] items = manifestContent.Split(',');
        if (items.Length > 0)
        {
            runtimeManifest.declaredItems = new string[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                runtimeManifest.declaredItems[i] = items[i].Trim();
            }
        }
        
        // Set ship category if available
        if (shipType != null && shipType.category != null)
        {
            runtimeManifest.requiredShipCategories = new string[] { shipType.category.categoryName };
        }
        
        // Set day validity using the existing fields
        runtimeManifest.firstAppearanceDay = 1;
        runtimeManifest.lastAppearanceDay = -1; // No end date
        
        // Set other properties
        runtimeManifest.priority = CargoManifest.ManifestPriority.Normal;
        runtimeManifest.isEasilyDetectable = !shouldHaveContraband;
        runtimeManifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        
        return runtimeManifest;
    }
    
    /// <summary>
    /// Generate a fallback string-based manifest when no ScriptableObject is available
    /// </summary>
    public string GenerateFallbackManifest(string faction, bool shouldHaveContraband = false)
    {
        System.Text.StringBuilder manifest = new System.Text.StringBuilder();
        
        // Add common cargo
        int itemCount = Random.Range(2, 5);
        List<string> selectedItems = new List<string>();
        
        for (int i = 0; i < itemCount; i++)
        {
            string item = commonCargoItems[Random.Range(0, commonCargoItems.Length)];
            if (!selectedItems.Contains(item))
            {
                selectedItems.Add(item);
            }
        }
        
        // Add contraband if specified
        if (shouldHaveContraband && Random.value < contrabandChance)
        {
            string contraband = contrabandItems[Random.Range(0, contrabandItems.Length)];
            selectedItems.Add(contraband);
        }
        
        manifest.Append(string.Join(", ", selectedItems));
        
        // Add faction-specific context
        switch (faction.ToLower())
        {
            case "imperium":
            case "imperial":
                manifest.Append(" for Imperial operations");
                break;
            case "insurgent":
            case "rebel":
                manifest.Append(" for resistance activities");
                break;
            default:
                manifest.Append(" for authorized operations");
                break;
        }
        
        return manifest.ToString();
    }
    
    /// <summary>
    /// Check if a manifest contains contraband based on current day rules
    /// </summary>
    public bool CheckForContraband(CargoManifest manifest, List<string> currentDayRules)
    {
        if (manifest == null)
            return false;
            
        // Direct contraband flag
        if (manifest.hasContraband)
            return true;
            
        // Check for suspicious content based on day rules
        if (currentDayRules != null && currentDayRules.Count > 0)
        {
            string[] dayRuleArray = currentDayRules.ToArray();
            return manifest.ContainsSuspiciousContent(dayRuleArray);
        }
        
        return false;
    }
    
    /// <summary>
    /// Validate a manifest against current game state
    /// </summary>
    public bool ValidateManifest(CargoManifest manifest, string faction, string shipCategory, int currentDay, List<string> dayRules)
    {
        if (manifest == null)
            return false;
            
        // Check faction validity
        if (!manifest.IsValidForFaction(faction))
        {
            Debug.Log($"ManifestManager: Manifest {manifest.manifestName} invalid for faction {faction}");
            return false;
        }
        
        // Check ship category validity
        if (!manifest.IsValidForShipCategory(shipCategory))
        {
            Debug.Log($"ManifestManager: Manifest {manifest.manifestName} invalid for ship category {shipCategory}");
            return false;
        }
        
        // Check day validity
        if (!manifest.IsValidForDay(currentDay))
        {
            Debug.Log($"ManifestManager: Manifest {manifest.manifestName} invalid for day {currentDay}");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Reset daily usage tracking (call this at the start of each day)
    /// </summary>
    public void ResetDailyUsage()
    {
        dailyManifestUsage.Clear();
        Debug.Log("ManifestManager: Reset daily usage tracking");
    }
    
    #region Private Helper Methods
    
    private List<CargoManifest> GetValidManifestsForCriteria(string faction, string shipCategory, int currentDay)
    {
        List<CargoManifest> validManifests = new List<CargoManifest>();
        
        // First try faction-specific manifests
        if (factionManifestPools.ContainsKey(faction))
        {
            foreach (var manifest in factionManifestPools[faction])
            {
                if (ValidateManifest(manifest, faction, shipCategory, currentDay, null))
                {
                    validManifests.Add(manifest);
                }
            }
        }
        
        // If no faction-specific manifests and cross-faction is allowed, try universal
        if (validManifests.Count == 0 && allowCrossFactionManifests)
        {
            foreach (var manifest in allManifests)
            {
                if (manifest.factionRestriction == CargoManifest.FactionRestriction.Universal &&
                    ValidateManifest(manifest, faction, shipCategory, currentDay, null))
                {
                    validManifests.Add(manifest);
                }
            }
        }
        
        return validManifests;
    }
    
    private List<CargoManifest> FilterByDailyUsage(List<CargoManifest> manifests)
    {
        List<CargoManifest> filtered = new List<CargoManifest>();
        
        foreach (var manifest in manifests)
        {
            if (manifest.maxDailyAppearances < 0) // No limit
            {
                filtered.Add(manifest);
                continue;
            }
            
            string key = manifest.manifestCode;
            int currentUsage = dailyManifestUsage.ContainsKey(key) ? dailyManifestUsage[key] : 0;
            
            if (currentUsage < manifest.maxDailyAppearances)
            {
                filtered.Add(manifest);
            }
        }
        
        return filtered;
    }
    
    private CargoManifest SelectWeightedRandomManifest(List<CargoManifest> manifests)
    {
        if (manifests.Count == 0)
            return null;
            
        if (manifests.Count == 1)
            return manifests[0];
        
        // Simple weighted selection based on priority
        List<CargoManifest> weightedList = new List<CargoManifest>();
        
        foreach (var manifest in manifests)
        {
            int weight = GetManifestWeight(manifest.priority);
            for (int i = 0; i < weight; i++)
            {
                weightedList.Add(manifest);
            }
        }
        
        return weightedList[Random.Range(0, weightedList.Count)];
    }
    
    private int GetManifestWeight(CargoManifest.ManifestPriority priority)
    {
        switch (priority)
        {
            case CargoManifest.ManifestPriority.Critical: return 4;
            case CargoManifest.ManifestPriority.High: return 3;
            case CargoManifest.ManifestPriority.Normal: return 2;
            case CargoManifest.ManifestPriority.Low: return 1;
            default: return 2;
        }
    }
    
    private void RecordManifestUsage(CargoManifest manifest)
    {
        string key = manifest.manifestCode;
        if (dailyManifestUsage.ContainsKey(key))
        {
            dailyManifestUsage[key]++;
        }
        else
        {
            dailyManifestUsage[key] = 1;
        }
    }
    
    private void AddManifestToFactionPool(string faction, CargoManifest manifest)
    {
        if (!factionManifestPools.ContainsKey(faction))
        {
            factionManifestPools[faction] = new List<CargoManifest>();
        }
        
        if (!factionManifestPools[faction].Contains(manifest))
        {
            factionManifestPools[faction].Add(manifest);
        }
    }
    
    private string[] GetAllKnownFactions()
    {
        // Return common faction names - in a real implementation, this might come from a FactionManager
        return new string[] { "Imperium", "Imperial", "Insurgent", "Rebel", "Neutral", "Merchant", "Trade" };
    }
    
    #endregion
    
    #region Debug and Editor Methods
    
    [ContextMenu("Debug Manifest Pools")]
    public void DebugManifestPools()
    {
        foreach (var pool in factionManifestPools)
        {
            Debug.Log($"Faction '{pool.Key}': {pool.Value.Count} manifests");
            foreach (var manifest in pool.Value)
            {
                Debug.Log($"  - {manifest.manifestName} ({manifest.contrabandType})");
            }
        }
    }
    
    [ContextMenu("Test Manifest Selection")]
    public void TestManifestSelection()
    {
        if (allManifests.Count == 0)
        {
            Debug.LogWarning("No manifests loaded for testing");
            return;
        }
        
        string[] testFactions = { "Imperium", "Insurgent", "Neutral" };
        
        foreach (string faction in testFactions)
        {
            var manifest = SelectManifestForShip(null, faction, 1);
            if (manifest != null)
            {
                Debug.Log($"Test: {faction} -> {manifest.manifestName} (Contraband: {manifest.hasContraband})");
            }
            else
            {
                Debug.Log($"Test: {faction} -> No manifest selected");
            }
        }
    }
    
    #endregion
    
    #region Sample Manifest Creation
    
    /// <summary>
    /// Debug method to create sample manifests - OPTIONAL
    /// </summary>
    [ContextMenu("Create Sample Manifests")]
    public void CreateSampleManifests()
    {
        #if UNITY_EDITOR
        string folderPath = "Assets/Resources/_ScriptableObjects/CargoManifests";
        
        // Create folder if it doesn't exist
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
            UnityEditor.AssetDatabase.Refresh();
        }
        
        // Create sample manifests for different factions
        string[] factions = { "Imperium", "Insurgent", "Neutral", "Merchant" };
        string[] cargoTypes = { "Standard", "Medical", "Military", "Supplies" };
        
        int created = 0;
        
        foreach (string faction in factions)
        {
            foreach (string cargoType in cargoTypes)
            {
                string manifestName = $"{faction}_{cargoType}_Manifest";
                string path = $"{folderPath}/{manifestName}.asset";
                
                // Check if already exists
                if (UnityEditor.AssetDatabase.LoadAssetAtPath<CargoManifest>(path) == null)
                {
                    CargoManifest manifest = ScriptableObject.CreateInstance<CargoManifest>();
                    
                    manifest.manifestCode = $"{faction.ToUpper()}_{cargoType.ToUpper()}_{UnityEngine.Random.Range(1000, 9999)}";
                    manifest.manifestName = $"{faction} {cargoType} Cargo";
                    manifest.manifestDescription = GenerateSampleManifestText(faction, cargoType);
                    
                    // Set declared items
                    manifest.declaredItems = manifest.manifestDescription.Split(',');
                    for (int i = 0; i < manifest.declaredItems.Length; i++)
                    {
                        manifest.declaredItems[i] = manifest.declaredItems[i].Trim();
                    }
                    
                    manifest.factionRestriction = faction == "Neutral" ? 
                        CargoManifest.FactionRestriction.Universal : 
                        CargoManifest.FactionRestriction.FactionSpecific;
                    
                    manifest.allowedFactions = new string[] { faction };
                    manifest.hasContraband = cargoType == "Military" && Random.value > 0.5f;
                    
                    if (manifest.hasContraband)
                    {
                        manifest.contrabandType = CargoManifest.ContrabandType.Weapons;
                        manifest.contrabandItems = new string[] { "Unauthorized weapons", "Restricted ammunition" };
                    }
                    
                    manifest.firstAppearanceDay = 1;
                    manifest.lastAppearanceDay = -1;
                    manifest.priority = CargoManifest.ManifestPriority.Normal;
                    
                    UnityEditor.AssetDatabase.CreateAsset(manifest, path);
                    created++;
                }
            }
        }
        
        if (created > 0)
        {
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log($"ManifestManager: Created {created} sample manifests in {folderPath}");
            
            // Reload manifests
            allManifests.Clear();
            LoadManifestsFromResources();
        }
        else
        {
            Debug.Log("ManifestManager: Sample manifests already exist");
        }
        #endif
    }
    
    private string GenerateSampleManifestText(string faction, string cargoType)
    {
        switch (cargoType)
        {
            case "Medical":
                return $"Medical supplies, Emergency equipment, Field hospitals for {faction} operations";
            case "Military":
                return $"Equipment parts, Communication arrays, Defense systems for {faction} forces";
            case "Supplies":
                return $"Food rations, Water purification units, Basic necessities for {faction} personnel";
            default:
                return $"Standard cargo, General supplies, Equipment for {faction} facilities";
        }
    }
    
    #endregion
}