using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace StarkillerBaseCommand.EditorTools
{
    public class CSVImporter : EditorWindow
    {
        private string csvFolderPath = "";
        private string[] csvFiles = new string[0];
        private int selectedFileIndex = 0;
        private bool showPreview = false;
        private string[][] previewData = null;
        private string[] headers = null;
        private Vector2 scrollPosition;
        private string[] scriptableObjectTypes = new string[] {
            "ShipType", "CaptainType", "ShipScenario", "LocationData", 
            "AccessCode", "CargoManifest", "DailyRule", "Consequence", 
            "ContrabandItem", "MediaAsset"
        };
        private int selectedTypeIndex = 0;
        private string outputFolder = "";
        
        // Field mappings
        private Dictionary<string, FieldMapping> fieldMappings = new Dictionary<string, FieldMapping>();
        private Dictionary<string, int> headerToColumnIndex = new Dictionary<string, int>();
        
        [MenuItem("Starkiller/Data Import/Import CSV")]
        public static void ShowWindow()
        {
            GetWindow<CSVImporter>("CSV Importer");
        }
        
        private void OnEnable()
        {
            // Set default paths
            csvFolderPath = EditorPrefs.GetString("StarkkillerCSVFolder", Application.dataPath + "/_Temp/CSV");
            outputFolder = EditorPrefs.GetString("StarkkillerOutputFolder", "Assets/_ScriptableObjects");
            
            // Initialize field mappings
            InitializeFieldMappings();
            
            // Find CSV files
            RefreshCSVFiles();
        }
        
        private void InitializeFieldMappings()
        {
            fieldMappings.Clear();
            
            // Ship Type mappings
            var shipTypeMapping = new FieldMapping("ShipType");
            shipTypeMapping.AddMapping("ID", "typeName", FieldType.String);
            shipTypeMapping.AddMapping("Type Name", "typeName", FieldType.String);
            shipTypeMapping.AddMapping("Category", "category", FieldType.Reference, "ShipCategory");
            shipTypeMapping.AddMapping("Size", "size", FieldType.Enum, "ShipType.SizeClass");
            shipTypeMapping.AddMapping("Crew Min", "minCrewSize", FieldType.Int);
            shipTypeMapping.AddMapping("Crew Max", "maxCrewSize", FieldType.Int);
            shipTypeMapping.AddMapping("Common Origins", "commonOrigins", FieldType.StringArray);
            shipTypeMapping.AddMapping("Valid Purposes", "validPurposes", FieldType.StringArray);
            shipTypeMapping.AddMapping("Suspicious Indicators", "suspiciousIndicators", FieldType.StringArray);
            shipTypeMapping.AddMapping("Visual Description", "visualDescription", FieldType.String);
            shipTypeMapping.AddMapping("Technical Specs", "technicalSpecification", FieldType.String);
            shipTypeMapping.AddMapping("Can Be Infiltrated", "canBeInfiltrated", FieldType.Bool);
            shipTypeMapping.AddMapping("Can Be Used By Order", "canBeUsedByOrder", FieldType.Bool);
            shipTypeMapping.AddMapping("Can Smuggle", "canSmuggle", FieldType.Bool);
            fieldMappings["ShipType"] = shipTypeMapping;
            
            // Captain Type mappings
            var captainTypeMapping = new FieldMapping("CaptainType");
            captainTypeMapping.AddMapping("ID", "typeName", FieldType.String);
            captainTypeMapping.AddMapping("Name", "specialTraits", FieldType.String);
            captainTypeMapping.AddMapping("Type Name", "typeName", FieldType.String);
            captainTypeMapping.AddMapping("Faction", "factions", FieldType.StringArray);
            captainTypeMapping.AddMapping("Factions", "factions", FieldType.StringArray);
            captainTypeMapping.AddMapping("Ranks", "commonRanks", FieldType.StringArray);
            captainTypeMapping.AddMapping("Possible Ranks", "commonRanks", FieldType.StringArray);
            captainTypeMapping.AddMapping("First Names", "possibleFirstNames", FieldType.StringArray);
            captainTypeMapping.AddMapping("First Name Options", "possibleFirstNames", FieldType.StringArray);
            captainTypeMapping.AddMapping("Last Names", "possibleLastNames", FieldType.StringArray);
            captainTypeMapping.AddMapping("Last Name Options", "possibleLastNames", FieldType.StringArray);
            captainTypeMapping.AddMapping("Bribery Chance", "briberyChance", FieldType.Float);
            captainTypeMapping.AddMapping("Min Bribe", "minBribeAmount", FieldType.Int);
            captainTypeMapping.AddMapping("Max Bribe", "maxBribeAmount", FieldType.Int);
            captainTypeMapping.AddMapping("Behaviors", "typicalBehaviors", FieldType.StringArray);
            captainTypeMapping.AddMapping("Typical Behaviors", "typicalBehaviors", FieldType.StringArray);
            captainTypeMapping.AddMapping("Dialogue", "dialoguePatterns", FieldType.StringArray);
            captainTypeMapping.AddMapping("Dialogue Patterns", "dialoguePatterns", FieldType.StringArray);
            captainTypeMapping.AddMapping("Bribery Phrases", "briberyPhrases", FieldType.StringArray);
            captainTypeMapping.AddMapping("Special Traits", "specialTraits", FieldType.String);
            fieldMappings["CaptainType"] = captainTypeMapping;
            
            // Scenario mappings
            var scenarioMapping = new FieldMapping("ShipScenario");
            scenarioMapping.AddMapping("ID", "scenarioName", FieldType.String);
            scenarioMapping.AddMapping("Name", "scenarioName", FieldType.String);
            scenarioMapping.AddMapping("Scenario Name", "scenarioName", FieldType.String);
            scenarioMapping.AddMapping("Type", "type", FieldType.Enum, "ShipScenario.ScenarioType");
            scenarioMapping.AddMapping("Story Mission", "isStoryMission", FieldType.Bool);
            scenarioMapping.AddMapping("First Day", "dayFirstAppears", FieldType.Int);
            scenarioMapping.AddMapping("Day First Appears", "dayFirstAppears", FieldType.Int);
            scenarioMapping.AddMapping("Max Appearances", "maxAppearances", FieldType.Int);
            scenarioMapping.AddMapping("Story Tag", "storyTag", FieldType.String);
            scenarioMapping.AddMapping("Should Approve", "shouldBeApproved", FieldType.Bool);
            scenarioMapping.AddMapping("Invalid Reason", "invalidReason", FieldType.String);
            scenarioMapping.AddMapping("Offers Bribe", "offersBribe", FieldType.Bool);
            scenarioMapping.AddMapping("Bribe Multiplier", "bribeChanceMultiplier", FieldType.Float);
            scenarioMapping.AddMapping("Stories", "possibleStories", FieldType.StringArray);
            scenarioMapping.AddMapping("Possible Stories", "possibleStories", FieldType.StringArray);
            scenarioMapping.AddMapping("Manifests", "possibleManifests", FieldType.StringArray);
            scenarioMapping.AddMapping("Possible Manifests", "possibleManifests", FieldType.StringArray);
            scenarioMapping.AddMapping("Severity", "severityLevel", FieldType.Enum, "ShipScenario.SeverityLevel");
            scenarioMapping.AddMapping("Severity Level", "severityLevel", FieldType.Enum, "ShipScenario.SeverityLevel");
            scenarioMapping.AddMapping("Consequences", "possibleConsequences", FieldType.StringArray);
            fieldMappings["ShipScenario"] = scenarioMapping;
            
            // Location mappings
            var locationMapping = new FieldMapping("LocationData");
            locationMapping.AddMapping("ID", "locationCode", FieldType.String);
            locationMapping.AddMapping("Name", "locationName", FieldType.String);
            locationMapping.AddMapping("Type", "type", FieldType.Enum, "LocationData.LocationType");
            locationMapping.AddMapping("Affiliation", "affiliation", FieldType.Enum, "LocationData.Affiliation");
            locationMapping.AddMapping("Is Secret", "isSecretLocation", FieldType.Bool);
            locationMapping.AddMapping("Description", "description", FieldType.String);
            fieldMappings["LocationData"] = locationMapping;
            
            // Access Code mappings
            var accessCodeMapping = new FieldMapping("AccessCode");
            accessCodeMapping.AddMapping("ID", "codeName", FieldType.String);
            accessCodeMapping.AddMapping("Code", "codeValue", FieldType.String);
            accessCodeMapping.AddMapping("Type", "type", FieldType.Enum, "AccessCode.CodeType");
            accessCodeMapping.AddMapping("Valid From Day", "validFromDay", FieldType.Int);
            accessCodeMapping.AddMapping("Valid Until Day", "validUntilDay", FieldType.Int);
            accessCodeMapping.AddMapping("Revoked", "isRevoked", FieldType.Bool);
            accessCodeMapping.AddMapping("Access Level", "level", FieldType.Enum, "AccessCode.AccessLevel");
            accessCodeMapping.AddMapping("Authorized Factions", "authorizedFactions", FieldType.StringArray);
            fieldMappings["AccessCode"] = accessCodeMapping;
            
            // Cargo Manifest mappings
            var manifestMapping = new FieldMapping("CargoManifest");
            manifestMapping.AddMapping("ID", "manifestCode", FieldType.String);
            manifestMapping.AddMapping("Name", "manifestName", FieldType.String);
            manifestMapping.AddMapping("Declared Items", "declaredItems", FieldType.StringArray);
            manifestMapping.AddMapping("Actual Items", "actualItems", FieldType.StringArray);
            manifestMapping.AddMapping("Has Contraband", "hasContraband", FieldType.Bool);
            manifestMapping.AddMapping("Is Falsified", "hasFalseEntries", FieldType.Bool);
            manifestMapping.AddMapping("Clearance Code", "clearanceCode", FieldType.String);
            manifestMapping.AddMapping("Authorized By", "authorizedBy", FieldType.String);
            manifestMapping.AddMapping("Description", "manifestDescription", FieldType.String);
            fieldMappings["CargoManifest"] = manifestMapping;
            
            // Daily Rule mappings
            var dayRuleMapping = new FieldMapping("DailyRule");
            dayRuleMapping.AddMapping("ID", "ruleName", FieldType.String);
            dayRuleMapping.AddMapping("Day", "dayNumber", FieldType.Int);
            dayRuleMapping.AddMapping("Description", "ruleDescription", FieldType.String);
            dayRuleMapping.AddMapping("Requirements", "requirementTexts", FieldType.StringArray);
            dayRuleMapping.AddMapping("Required Documents", "requiredDocuments", FieldType.StringArray);
            dayRuleMapping.AddMapping("Banned Origins", "bannedOrigins", FieldType.StringArray);
            dayRuleMapping.AddMapping("Banned Ships", "bannedShipTypes", FieldType.StringArray);
            dayRuleMapping.AddMapping("Special Requirements", "specialRequirements", FieldType.StringArray);
            fieldMappings["DailyRule"] = dayRuleMapping;
            
            // Consequence mappings
            var consequenceMapping = new FieldMapping("Consequence");
            consequenceMapping.AddMapping("ID", "consequenceName", FieldType.String);
            consequenceMapping.AddMapping("Type", "type", FieldType.Enum, "Consequence.ConsequenceType");
            consequenceMapping.AddMapping("Severity", "severity", FieldType.Enum, "Consequence.Severity");
            consequenceMapping.AddMapping("Affects Salary", "affectsSalary", FieldType.Bool);
            consequenceMapping.AddMapping("Credit Penalty", "creditPenalty", FieldType.Int);
            consequenceMapping.AddMapping("Affects Morale", "affectsMorale", FieldType.Bool);
            consequenceMapping.AddMapping("Morale Penalty", "moralePenalty", FieldType.Int);
            consequenceMapping.AddMapping("Triggers Investigation", "triggersInvestigation", FieldType.Bool);
            consequenceMapping.AddMapping("Description", "consequenceDescription", FieldType.String);
            consequenceMapping.AddMapping("Messages", "consequenceMessages", FieldType.StringArray);
            fieldMappings["Consequence"] = consequenceMapping;
            
            // Contraband Item mappings
            var contrabandMapping = new FieldMapping("ContrabandItem");
            contrabandMapping.AddMapping("ID", "itemName", FieldType.String);
            contrabandMapping.AddMapping("Name", "itemName", FieldType.String);
            contrabandMapping.AddMapping("Type", "type", FieldType.Enum, "ContrabandItem.ContrabandType");
            contrabandMapping.AddMapping("Severity", "severity", FieldType.Enum, "ContrabandItem.Severity");
            contrabandMapping.AddMapping("Visible In Scans", "visibleInScans", FieldType.Bool);
            contrabandMapping.AddMapping("Common Disguises", "commonDisguises", FieldType.StringArray);
            contrabandMapping.AddMapping("Common Containers", "commonContainers", FieldType.StringArray);
            contrabandMapping.AddMapping("Description", "itemDescription", FieldType.String);
            fieldMappings["ContrabandItem"] = contrabandMapping;
            
            // Media Asset mappings
            var mediaAssetMapping = new FieldMapping("MediaAsset");
            mediaAssetMapping.AddMapping("ID", "assetName", FieldType.String);
            mediaAssetMapping.AddMapping("Type", "type", FieldType.Enum, "MediaAsset.AssetType");
            mediaAssetMapping.AddMapping("Associated ID", "associatedId", FieldType.String);
            mediaAssetMapping.AddMapping("Description", "assetDescription", FieldType.String);
            fieldMappings["MediaAsset"] = mediaAssetMapping;
        }
        
        private void RefreshCSVFiles()
        {
            if (Directory.Exists(csvFolderPath))
            {
                csvFiles = Directory.GetFiles(csvFolderPath, "*.csv");
                if (csvFiles.Length > 0 && selectedFileIndex >= csvFiles.Length)
                {
                    selectedFileIndex = 0;
                }
            }
            else
            {
                csvFiles = new string[0];
            }
        }
        
        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label("CSV Import Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Select CSV folder
            EditorGUILayout.BeginHorizontal();
            csvFolderPath = EditorGUILayout.TextField("CSV Folder:", csvFolderPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select CSV Folder", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    csvFolderPath = path;
                    EditorPrefs.SetString("StarkkillerCSVFolder", csvFolderPath);
                    RefreshCSVFiles();
                }
            }
            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                RefreshCSVFiles();
            }
            EditorGUILayout.EndHorizontal();
            
            // Select ScriptableObject Type
            selectedTypeIndex = EditorGUILayout.Popup("ScriptableObject Type:", selectedTypeIndex, scriptableObjectTypes);
            
            // Select Output Folder
            EditorGUILayout.BeginHorizontal();
            outputFolder = EditorGUILayout.TextField("Output Folder:", outputFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.SaveFolderPanel("Select Output Folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    // Convert to a relative path if possible
                    if (path.StartsWith(Application.dataPath))
                    {
                        path = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                    outputFolder = path;
                    EditorPrefs.SetString("StarkkillerOutputFolder", outputFolder);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // Select CSV file
            if (csvFiles.Length > 0)
            {
                string[] fileNames = csvFiles.Select(Path.GetFileName).ToArray();
                selectedFileIndex = EditorGUILayout.Popup("CSV File:", selectedFileIndex, fileNames);
                
                // Preview button
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Preview CSV"))
                {
                    previewData = ReadCSV(csvFiles[selectedFileIndex]);
                    if (previewData != null && previewData.Length > 0)
                    {
                        headers = previewData[0];
                        showPreview = true;
                        
                        // Set up column mapping
                        headerToColumnIndex.Clear();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            headerToColumnIndex[headers[i]] = i;
                        }
                    }
                }
                
                if (previewData != null && GUILayout.Button("Import"))
                {
                    ImportCSV();
                }
                EditorGUILayout.EndHorizontal();
                
                // Show preview
                if (showPreview && previewData != null)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label("Preview", EditorStyles.boldLabel);
                    
                    if (previewData.Length > 1)
                    {
                        // Display first 5 rows + header
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        
                        // Headers
                        EditorGUILayout.BeginHorizontal();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            EditorGUILayout.LabelField(headers[i], EditorStyles.boldLabel, GUILayout.MaxWidth(150));
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        // Data rows (up to 5)
                        for (int row = 1; row < Mathf.Min(6, previewData.Length); row++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            for (int col = 0; col < previewData[row].Length && col < headers.Length; col++)
                            {
                                EditorGUILayout.LabelField(previewData[row][col], GUILayout.MaxWidth(150));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        
                        EditorGUILayout.EndVertical();
                        
                        // Show total row count
                        EditorGUILayout.LabelField($"Total Rows: {previewData.Length - 1}");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("No data rows found in CSV.");
                    }
                }
                
                // Show field mappings
                if (showPreview && headers != null)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label("Field Mappings", EditorStyles.boldLabel);
                    
                    string selectedType = scriptableObjectTypes[selectedTypeIndex];
                    if (fieldMappings.ContainsKey(selectedType))
                    {
                        FieldMapping mapping = fieldMappings[selectedType];
                        
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField("CSV Header", "ScriptableObject Field", EditorStyles.boldLabel);
                        
                        foreach (var field in mapping.Mappings)
                        {
                            EditorGUILayout.BeginHorizontal();
                            
                            // Display if this field exists in the CSV
                            bool fieldExists = headerToColumnIndex.ContainsKey(field.CsvHeader);
                            GUI.color = fieldExists ? Color.green : Color.red;
                            
                            EditorGUILayout.LabelField(field.CsvHeader, GUILayout.Width(150));
                            EditorGUILayout.LabelField("→", GUILayout.Width(20));
                            EditorGUILayout.LabelField($"{field.ObjectField} ({field.Type})", GUILayout.Width(200));
                            
                            GUI.color = Color.white;
                            EditorGUILayout.EndHorizontal();
                        }
                        
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"No field mappings found for {selectedType}");
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No CSV files found in the selected folder.");
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private string[][] ReadCSV(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length == 0)
                {
                    return null;
                }
                
                string[][] data = new string[lines.Length][];
                for (int i = 0; i < lines.Length; i++)
                {
                    data[i] = ParseCSVLine(lines[i]);
                }
                
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading CSV: {ex.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to read CSV file: {ex.Message}", "OK");
                return null;
            }
        }
        
        private string[] ParseCSVLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            int startIndex = 0;
            
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (line[i] == ',' && !inQuotes)
                {
                    result.Add(CleanCSVField(line.Substring(startIndex, i - startIndex)));
                    startIndex = i + 1;
                }
            }
            
            // Add the last field
            result.Add(CleanCSVField(line.Substring(startIndex)));
            
            return result.ToArray();
        }
        
        private string CleanCSVField(string field)
        {
            // Remove surrounding quotes and trim whitespace
            if (field.StartsWith("\"") && field.EndsWith("\""))
            {
                field = field.Substring(1, field.Length - 2);
            }
            
            // Replace escaped quotes
            field = field.Replace("\"\"", "\"");
            
            return field.Trim();
        }
        
        private void ImportCSV()
        {
            if (previewData == null || previewData.Length <= 1)
            {
                EditorUtility.DisplayDialog("Error", "No data to import.", "OK");
                return;
            }
            
            string selectedType = scriptableObjectTypes[selectedTypeIndex];
            if (!fieldMappings.ContainsKey(selectedType))
            {
                EditorUtility.DisplayDialog("Error", $"No field mappings found for {selectedType}", "OK");
                return;
            }
            
            // Determine output folder
            string typeFolder = Path.Combine(outputFolder, selectedType + "s");
            if (!Directory.Exists(typeFolder))
            {
                Directory.CreateDirectory(typeFolder);
            }
            
            FieldMapping mapping = fieldMappings[selectedType];
            int successCount = 0;
            
            EditorUtility.DisplayProgressBar("Importing Data", "Starting import...", 0f);
            
            try
            {
                // Skip header row
                for (int i = 1; i < previewData.Length; i++)
                {
                    float progress = (float)i / (previewData.Length - 1);
                    EditorUtility.DisplayProgressBar("Importing Data", $"Processing row {i} of {previewData.Length - 1}", progress);
                    
                    string[] row = previewData[i];
                    
                    // Skip empty rows
                    if (row.Length == 0 || string.IsNullOrWhiteSpace(row[0]))
                    {
                        continue;
                    }
                    
                    // Create ScriptableObject instance
                    ScriptableObject obj = CreateScriptableObject(selectedType);
                    if (obj == null)
                    {
                        continue;
                    }
                    
                    // Map fields
                    bool success = MapFields(obj, row, mapping);
                    if (!success)
                    {
                        continue;
                    }
                    
                    // Determine asset name from primary field
                    string assetName = GetAssetName(obj, mapping);
                    if (string.IsNullOrEmpty(assetName))
                    {
                        continue;
                    }
                    
                    // Create a valid filename
                    string safeFileName = Regex.Replace(assetName, @"[^\w\d]", "_");
                    string assetPath = $"{typeFolder}/{safeFileName}.asset";
                    
                    // Create the asset
                    AssetDatabase.CreateAsset(obj, assetPath);
                    successCount++;
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                EditorUtility.DisplayDialog("Import Complete", $"Successfully imported {successCount} {selectedType} assets.", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error importing data: {ex.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to import data: {ex.Message}", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        
        private ScriptableObject CreateScriptableObject(string typeName)
        {
            try
            {
                System.Type type = null;
                
                // Try with known types first
                switch (typeName)
                {
                    case "ShipType":
                        type = typeof(ShipType);
                        break;
                    case "CaptainType":
                        type = typeof(CaptainType);
                        break;
                    case "ShipScenario":
                        type = typeof(ShipScenario);
                        break;
                    case "LocationData":
                        type = typeof(LocationData);
                        break;
                    case "AccessCode":
                        type = typeof(AccessCode);
                        break;
                    case "CargoManifest":
                        type = typeof(CargoManifest);
                        break;
                    case "DailyRule":
                        type = typeof(DailyRule);
                        break;
                    case "Consequence":
                        type = typeof(Consequence);
                        break;
                    case "ContrabandItem":
                        type = typeof(ContrabandItem);
                        break;
                    case "MediaAsset":
                        type = typeof(MediaAsset);
                        break;
                    default:
                        // Try to find the type in the Assembly
                        type = System.Type.GetType($"StarkillerBaseCommand.{typeName}, Assembly-CSharp");
                        if (type == null)
                        {
                            type = System.Type.GetType($"{typeName}, Assembly-CSharp");
                        }
                        break;
                }
                
                if (type == null)
                {
                    Debug.LogError($"Type not found: {typeName}");
                    return null;
                }
                
                return ScriptableObject.CreateInstance(type);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating ScriptableObject: {ex.Message}");
                return null;
            }
        }
        
        private bool MapFields(ScriptableObject obj, string[] row, FieldMapping mapping)
        {
            try
            {
                foreach (var field in mapping.Mappings)
                {
                    if (!headerToColumnIndex.ContainsKey(field.CsvHeader))
                    {
                        continue;
                    }
                    
                    int columnIndex = headerToColumnIndex[field.CsvHeader];
                    if (columnIndex >= row.Length)
                    {
                        continue;
                    }
                    
                    string value = row[columnIndex].Trim();
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    
                    // Get the field info
                    var fieldInfo = obj.GetType().GetField(field.ObjectField);
                    if (fieldInfo == null)
                    {
                        Debug.LogWarning($"Field not found on {obj.GetType().Name}: {field.ObjectField}");
                        continue;
                    }
                    
                    // Set the value based on field type
                    switch (field.Type)
                    {
                        case FieldType.String:
                            fieldInfo.SetValue(obj, value);
                            break;
                            
                        case FieldType.Int:
                            int intValue;
                            if (int.TryParse(value, out intValue))
                            {
                                fieldInfo.SetValue(obj, intValue);
                            }
                            break;
                            
                        case FieldType.Float:
                            float floatValue;
                            if (float.TryParse(value, out floatValue))
                            {
                                fieldInfo.SetValue(obj, floatValue);
                            }
                            break;
                            
                        case FieldType.Bool:
                            bool boolValue = value.ToUpper() == "TRUE";
                            fieldInfo.SetValue(obj, boolValue);
                            break;
                            
                        case FieldType.StringArray:
                            string[] arrayValue = value.Split(',').Select(s => s.Trim()).ToArray();
                            fieldInfo.SetValue(obj, arrayValue);
                            break;
                            
                        case FieldType.Enum:
                            // Get the enum type
                            string[] enumTypeParts = field.TypeInfo.Split('.');
                            Type enumType;
                            
                            if (enumTypeParts.Length > 1)
                            {
                                // Nested enum (e.g., ShipScenario.SeverityLevel)
                                string containingTypeName = enumTypeParts[0];
                                string enumName = enumTypeParts[1];
                                
                                Type containingType = System.Type.GetType($"StarkillerBaseCommand.{containingTypeName}, Assembly-CSharp");
                                if (containingType == null)
                                {
                                    containingType = System.Type.GetType($"{containingTypeName}, Assembly-CSharp");
                                }
                                
                                if (containingType != null)
                                {
                                    enumType = containingType.GetNestedType(enumName);
                                }
                                else
                                {
                                    Debug.LogWarning($"Containing type not found: {containingTypeName}");
                                    continue;
                                }
                            }
                            else
                            {
                                // Regular enum
                                enumType = System.Type.GetType($"StarkillerBaseCommand.{field.TypeInfo}, Assembly-CSharp");
                                if (enumType == null)
                                {
                                    enumType = System.Type.GetType($"{field.TypeInfo}, Assembly-CSharp");
                                }
                            }
                            
                            if (enumType != null && enumType.IsEnum)
                            {
                                try
                                {
                                    var enumValue = Enum.Parse(enumType, value, true);
                                    fieldInfo.SetValue(obj, enumValue);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogWarning($"Failed to parse enum value '{value}' for {field.ObjectField}: {ex.Message}");
                                }
                            }
                            break;
                            
                        case FieldType.Reference:
                            // References to other ScriptableObjects - not implemented in this simplified version
                            Debug.LogWarning($"Reference type fields not fully supported in this importer: {field.ObjectField}");
                            break;
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error mapping fields: {ex.Message}");
                return false;
            }
        }
        
        private string GetAssetName(ScriptableObject obj, FieldMapping mapping)
        {
            // Find the primary field (usually the name field)
            string primaryField = mapping.PrimaryField;
            if (string.IsNullOrEmpty(primaryField))
            {
                // Default primary fields by type
                switch (obj.GetType().Name)
                {
                    case "ShipType":
                        primaryField = "typeName";
                        break;
                    case "CaptainType":
                        primaryField = "typeName";
                        break;
                    case "ShipScenario":
                        primaryField = "scenarioName";
                        break;
                    default:
                        primaryField = "name";
                        break;
                }
            }
            
            // Get the field value
            var fieldInfo = obj.GetType().GetField(primaryField);
            if (fieldInfo != null)
            {
                object value = fieldInfo.GetValue(obj);
                if (value != null)
                {
                    return value.ToString();
                }
            }
            
            // Fallback to a GUID
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
    
    // Field mapping class
    public class FieldMapping
    {
        public string ObjectType { get; private set; }
        public string PrimaryField { get; set; }
        public List<FieldInfo> Mappings { get; private set; }
        
        public FieldMapping(string objectType)
        {
            ObjectType = objectType;
            Mappings = new List<FieldInfo>();
            
            // Set default primary field
            switch (objectType)
            {
                case "ShipType":
                    PrimaryField = "typeName";
                    break;
                case "CaptainType":
                    PrimaryField = "typeName";
                    break;
                case "ShipScenario":
                    PrimaryField = "scenarioName";
                    break;
                default:
                    PrimaryField = "name";
                    break;
            }
        }
        
        public void AddMapping(string csvHeader, string objectField, FieldType type, string typeInfo = "")
        {
            Mappings.Add(new FieldInfo
            {
                CsvHeader = csvHeader,
                ObjectField = objectField,
                Type = type,
                TypeInfo = typeInfo
            });
        }
        
        public class FieldInfo
        {
            public string CsvHeader { get; set; }
            public string ObjectField { get; set; }
            public FieldType Type { get; set; }
            public string TypeInfo { get; set; }
        }
    }
    
    public enum FieldType
    {
        String,
        Int,
        Float,
        Bool,
        StringArray,
        Enum,
        Reference
    }
}