#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using StarkillerBaseCommand;

/// <summary>
/// Custom editor utility for creating and managing Starkiller Base Command data
/// This provides convenient tools for creating ship, captain, and scenario assets
/// </summary>
public class ImperialDataEditor : EditorWindow
{
    private enum EditorTab
    {
        Ships,
        Captains,
        Scenarios,
        Consequences,
        Rules
    }
    
    private EditorTab currentTab = EditorTab.Ships;
    private Vector2 scrollPosition;
    
    // Asset creation fields
    private string newAssetName = "";
    private string baseFolder = "Assets/ScriptableObjects";
    
    // Filtered views
    private string searchFilter = "";
    private bool showImperium = true;
    private bool showCivilian = true;
    private bool showInsurgent = true;
    private bool showOrder = true;
    
    // Ship category quick selection
    private ShipCategory selectedCategory;
    private List<ShipCategory> allCategories = new List<ShipCategory>();
    
    [MenuItem("Starkiller Base/Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<ImperialDataEditor>("Imperial Data Editor");
    }
    
    void OnEnable()
    {
        // Load all ship categories
        LoadAllShipCategories();
    }
    
    void OnGUI()
    {
        GUILayout.Label("Starkiller Base Command Data Editor", EditorStyles.boldLabel);
        
        // Tab selection
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(currentTab == EditorTab.Ships, "Ships", EditorStyles.toolbarButton))
            currentTab = EditorTab.Ships;
        if (GUILayout.Toggle(currentTab == EditorTab.Captains, "Captains", EditorStyles.toolbarButton))
            currentTab = EditorTab.Captains;
        if (GUILayout.Toggle(currentTab == EditorTab.Scenarios, "Scenarios", EditorStyles.toolbarButton))
            currentTab = EditorTab.Scenarios;
        if (GUILayout.Toggle(currentTab == EditorTab.Consequences, "Consequences", EditorStyles.toolbarButton))
            currentTab = EditorTab.Consequences;
        if (GUILayout.Toggle(currentTab == EditorTab.Rules, "Rules", EditorStyles.toolbarButton))
            currentTab = EditorTab.Rules;
        EditorGUILayout.EndHorizontal();
        
        // Search filter
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Search:", GUILayout.Width(50));
        searchFilter = EditorGUILayout.TextField(searchFilter);
        if (GUILayout.Button("Clear", GUILayout.Width(50)))
            searchFilter = "";
        EditorGUILayout.EndHorizontal();
        
        // Faction filters
        EditorGUILayout.BeginHorizontal();
        showImperium = GUILayout.Toggle(showImperium, "Imperium");
        showCivilian = GUILayout.Toggle(showCivilian, "Civilian");
        showInsurgent = GUILayout.Toggle(showInsurgent, "Insurgent");
        showOrder = GUILayout.Toggle(showOrder, "The Order");
        EditorGUILayout.EndHorizontal();
        
        // Main content based on selected tab
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        switch (currentTab)
        {
            case EditorTab.Ships:
                DrawShipsTab();
                break;
            case EditorTab.Captains:
                DrawCaptainsTab();
                break;
            case EditorTab.Scenarios:
                DrawScenariosTab();
                break;
            case EditorTab.Consequences:
                DrawConsequencesTab();
                break;
            case EditorTab.Rules:
                DrawRulesTab();
                break;
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    #region Ship Tab
    
    private void DrawShipsTab()
    {
        // Create new ship type section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Create New Ship Type", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Name:", GUILayout.Width(50));
        newAssetName = EditorGUILayout.TextField(newAssetName);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Category:", GUILayout.Width(70));
        selectedCategory = EditorGUILayout.ObjectField(selectedCategory, typeof(ShipCategory), false) as ShipCategory;
        
        if (GUILayout.Button("New Category", GUILayout.Width(100)))
        {
            CreateNewCategory();
        }
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("Create Ship Type"))
        {
            CreateNewShipType();
        }
        
        // Existing ship types
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Existing Ship Types", EditorStyles.boldLabel);
        
        // Loop through all ship type assets
        string[] shipTypeGuids = AssetDatabase.FindAssets("t:ShipType");
        foreach (string guid in shipTypeGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ShipType shipType = AssetDatabase.LoadAssetAtPath<ShipType>(path);
            
            // Apply filters
            if (!string.IsNullOrEmpty(searchFilter) && 
                !shipType.typeName.ToLower().Contains(searchFilter.ToLower()))
                continue;
                
            // Check faction filters based on category
            if (shipType.category != null)
            {
                string categoryName = shipType.category.categoryName.ToLower();
                if ((!showImperium && categoryName.Contains("imperium")) ||
                    (!showCivilian && categoryName.Contains("civilian")) ||
                    (!showInsurgent && categoryName.Contains("insurgent")) ||
                    (!showOrder && categoryName.Contains("order")))
                    continue;
            }
            
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            
            // Display basic info
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(shipType.typeName, EditorStyles.boldLabel);
            if (shipType.category != null)
                EditorGUILayout.LabelField($"Category: {shipType.category.categoryName}");
            EditorGUILayout.LabelField($"Size: {shipType.size}");
            EditorGUILayout.EndVertical();
            
            // Actions
            if (GUILayout.Button("Edit", GUILayout.Width(50)))
            {
                Selection.activeObject = shipType;
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
    
    private void CreateNewShipType()
    {
        if (string.IsNullOrEmpty(newAssetName))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a name for the ship type.", "OK");
            return;
        }
        
        // Create folder if it doesn't exist
        string folder = $"{baseFolder}/ShipTypes";
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        
        // Create asset
        ShipType shipType = ScriptableObject.CreateInstance<ShipType>();
        shipType.typeName = newAssetName;
        shipType.category = selectedCategory;
        shipType.size = ShipType.SizeClass.Medium;
        
        // Set default values
        shipType.minCrewSize = 10;
        shipType.maxCrewSize = 50;
        shipType.commonOrigins = new string[] { "Central Fleet" };
        shipType.validPurposes = new string[] { "Standard Operations" };
        shipType.suspiciousIndicators = new string[] { "Unauthorized Modifications" };
        shipType.visualDescription = $"A standard {newAssetName} vessel.";
        
        // Save asset
        string path = $"{folder}/{newAssetName.Replace(" ", "")}.asset";
        AssetDatabase.CreateAsset(shipType, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Select the new asset
        Selection.activeObject = shipType;
        
        // Clear name field
        newAssetName = "";
    }
    
    private void CreateNewCategory()
    {
        // Create folder if it doesn't exist
        string folder = $"{baseFolder}/ShipCategories";
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        
        // Create asset
        ShipCategory category = ScriptableObject.CreateInstance<ShipCategory>();
        category.categoryName = "New Category";
        category.categoryDescription = "Description of this category.";
        category.validOrigins = new string[] { "Central Fleet" };
        category.requiredDocuments = new string[] { "Access Code" };
        category.validAccessCodePrefixes = new string[] { "SK-" };
        
        // Save asset
        string path = $"{folder}/NewCategory.asset";
        int counter = 1;
        while (File.Exists(path))
        {
            path = $"{folder}/NewCategory{counter}.asset";
            counter++;
        }
        
        AssetDatabase.CreateAsset(category, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Select the new asset
        Selection.activeObject = category;
        selectedCategory = category;
        
        // Reload categories
        LoadAllShipCategories();
    }
    
    private void LoadAllShipCategories()
    {
        allCategories.Clear();
        string[] categoryGuids = AssetDatabase.FindAssets("t:ShipCategory");
        foreach (string guid in categoryGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ShipCategory category = AssetDatabase.LoadAssetAtPath<ShipCategory>(path);
            allCategories.Add(category);
        }
    }
    
    #endregion
    
    #region Captain Tab
    
    private void DrawCaptainsTab()
    {
        // Create new captain type section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Create New Captain Type", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Name:", GUILayout.Width(50));
        newAssetName = EditorGUILayout.TextField(newAssetName);
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("Create Captain Type"))
        {
            CreateNewCaptainType();
        }
        
        // Existing captain types
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Existing Captain Types", EditorStyles.boldLabel);
        
        // Loop through all captain type assets
        string[] captainTypeGuids = AssetDatabase.FindAssets("t:CaptainType");
        foreach (string guid in captainTypeGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CaptainType captainType = AssetDatabase.LoadAssetAtPath<CaptainType>(path);
            
            // Apply search filter
            if (!string.IsNullOrEmpty(searchFilter) && 
                !captainType.typeName.ToLower().Contains(searchFilter.ToLower()))
                continue;
                
            // Apply faction filters
            if (captainType.factions != null && captainType.factions.Length > 0)
            {
                bool showCaptain = false;
                foreach (string faction in captainType.factions)
                {
                    string lowerFaction = faction.ToLower();
                    if ((showImperium && lowerFaction.Contains("imperium")) ||
                        (showCivilian && lowerFaction.Contains("civilian")) ||
                        (showInsurgent && lowerFaction.Contains("insurgent")) ||
                        (showOrder && lowerFaction.Contains("order")))
                    {
                        showCaptain = true;
                        break;
                    }
                }
                
                if (!showCaptain)
                    continue;
            }
            
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            
            // Display basic info
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(captainType.typeName, EditorStyles.boldLabel);
            if (captainType.factions != null && captainType.factions.Length > 0)
                EditorGUILayout.LabelField($"Factions: {string.Join(", ", captainType.factions)}");
            if (captainType.briberyChance > 0)
                EditorGUILayout.LabelField($"Bribery Chance: {captainType.briberyChance:P0}");
            EditorGUILayout.EndVertical();
            
            // Actions
            if (GUILayout.Button("Edit", GUILayout.Width(50)))
            {
                Selection.activeObject = captainType;
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
    
    private void CreateNewCaptainType()
    {
        if (string.IsNullOrEmpty(newAssetName))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a name for the captain type.", "OK");
            return;
        }
        
        // Create folder if it doesn't exist
        string folder = $"{baseFolder}/CaptainTypes";
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        
        // Create asset
        CaptainType captainType = ScriptableObject.CreateInstance<CaptainType>();
        captainType.typeName = newAssetName;
        captainType.factions = new string[] { "Imperium" };
        
        // Set default values
        captainType.commonRanks = new string[] { "Captain", "Commander" };
        captainType.possibleFirstNames = new string[] { "John", "Jane" };
        captainType.possibleLastNames = new string[] { "Smith", "Doe" };
        captainType.typicalBehaviors = new string[] { "Professional", "Formal" };
        captainType.dialoguePatterns = new string[] { "Our documents are in order, officer." };
        captainType.briberyChance = 0.1f;
        captainType.minBribeAmount = 10;
        captainType.maxBribeAmount = 30;
        captainType.briberyPhrases = new string[] { "Perhaps {AMOUNT} credits would help expedite our clearance?" };
        
        // Save asset
        string path = $"{folder}/{newAssetName.Replace(" ", "")}.asset";
        AssetDatabase.CreateAsset(captainType, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Select the new asset
        Selection.activeObject = captainType;
        
        // Clear name field
        newAssetName = "";
    }
    
    #endregion
    
    #region Scenario Tab
    
    private void DrawScenariosTab()
    {
        // Create new scenario section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Create New Scenario", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Name:", GUILayout.Width(50));
        newAssetName = EditorGUILayout.TextField(newAssetName);
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("Create Scenario"))
        {
            CreateNewScenario();
        }
        
        // Existing scenarios
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Existing Scenarios", EditorStyles.boldLabel);
        
        // Loop through all scenario assets
        string[] scenarioGuids = AssetDatabase.FindAssets("t:ShipScenario");
        foreach (string guid in scenarioGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ShipScenario scenario = AssetDatabase.LoadAssetAtPath<ShipScenario>(path);
            
            // Apply search filter
            if (!string.IsNullOrEmpty(searchFilter) && 
                !scenario.scenarioName.ToLower().Contains(searchFilter.ToLower()))
                continue;
                
            // Apply faction filters based on story tag
            if (!string.IsNullOrEmpty(scenario.storyTag))
            {
                string tag = scenario.storyTag.ToLower();
                if ((!showImperium && tag.Contains("imperium")) ||
                    (!showCivilian && tag.Contains("civilian")) ||
                    (!showInsurgent && tag.Contains("insurgent")) ||
                    (!showOrder && tag.Contains("order")))
                    continue;
            }
            
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            
            // Display basic info
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(scenario.scenarioName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Type: {scenario.type}");
            EditorGUILayout.LabelField($"Should Approve: {scenario.shouldBeApproved}");
            if (scenario.isStoryMission)
                EditorGUILayout.LabelField($"Story Mission: {scenario.storyTag}");
            EditorGUILayout.EndVertical();
            
            // Actions
            if (GUILayout.Button("Edit", GUILayout.Width(50)))
            {
                Selection.activeObject = scenario;
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
    
    private void CreateNewScenario()
    {
        if (string.IsNullOrEmpty(newAssetName))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a name for the scenario.", "OK");
            return;
        }
        
        // Create folder if it doesn't exist
        string folder = $"{baseFolder}/Scenarios";
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        
        // Create asset
        ShipScenario scenario = ScriptableObject.CreateInstance<ShipScenario>();
        scenario.scenarioName = newAssetName;
        scenario.type = ShipScenario.ScenarioType.Standard;
        
        // Set default values
        scenario.possibleStories = new string[] { "Ship requesting routine clearance." };
        scenario.possibleManifests = new string[] { "Standard supplies and personnel." };
        scenario.shouldBeApproved = true;
        scenario.dayFirstAppears = 1;
        scenario.severityLevel = ShipScenario.SeverityLevel.Minor;
        scenario.possibleConsequences = new string[] { "Minor security issue resolved." };
        
        // Save asset
        string path = $"{folder}/{newAssetName.Replace(" ", "")}.asset";
        AssetDatabase.CreateAsset(scenario, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Select the new asset
        Selection.activeObject = scenario;
        
        // Clear name field
        newAssetName = "";
    }
    
    #endregion
    
    #region Consequence Tab
    
    private void DrawConsequencesTab()
    {
        // Create new consequence section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Create New Consequence", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Title:", GUILayout.Width(50));
        newAssetName = EditorGUILayout.TextField(newAssetName);
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("Create Consequence"))
        {
            CreateNewConsequence();
        }
        
        // Existing consequences
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Existing Consequences", EditorStyles.boldLabel);
        
        // Loop through all consequence assets
        string[] consequenceGuids = AssetDatabase.FindAssets("t:Consequence");
        foreach (string guid in consequenceGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Consequence consequence = AssetDatabase.LoadAssetAtPath<Consequence>(path);
            
            // Apply search filter
            if (!string.IsNullOrEmpty(searchFilter) && 
                !consequence.title.ToLower().Contains(searchFilter.ToLower()))
                continue;
                
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            
            // Display basic info
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(consequence.title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Severity: {consequence.severity}");
            if (consequence.imperiumCasualties > 0)
                EditorGUILayout.LabelField($"Casualties: {consequence.imperiumCasualties}");
            EditorGUILayout.EndVertical();
            
            // Actions
            if (GUILayout.Button("Edit", GUILayout.Width(50)))
            {
                Selection.activeObject = consequence;
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
    
    private void CreateNewConsequence()
    {
        if (string.IsNullOrEmpty(newAssetName))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a title for the consequence.", "OK");
            return;
        }
        
        // Create folder if it doesn't exist
        string folder = $"{baseFolder}/Consequences";
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        
        // Create asset
        Consequence consequence = ScriptableObject.CreateInstance<Consequence>();
        consequence.title = newAssetName;
        
        // Set default values
        consequence.possibleDescriptions = new string[] { "A security incident has occurred." };
        consequence.severity = Consequence.SeverityLevel.Minor;
        
        // Save asset
        string path = $"{folder}/{newAssetName.Replace(" ", "")}.asset";
        AssetDatabase.CreateAsset(consequence, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Select the new asset
        Selection.activeObject = consequence;
        
        // Clear name field
        newAssetName = "";
    }
    
    #endregion
    
    #region Rules Tab
    
    private void DrawRulesTab()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rules Management", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox("This tab allows you to create and manage security rules for the game.", MessageType.Info);
        
        // Add rule management UI here when implemented
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rule management will be implemented in a future update.");
    }
    
    #endregion
}
#endif