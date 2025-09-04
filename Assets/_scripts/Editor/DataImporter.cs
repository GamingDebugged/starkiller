using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using System.Linq;

namespace StarkillerBaseCommand.EditorTools
{
    public class DataImporter : EditorWindow
    {
        // Path to the data file
        private string excelFilePath = "";
        private string csvFolderPath = "";
        private bool showConversionOptions = false;
        private bool showImportOptions = false;
        private Vector2 scrollPosition;
        
        // Data structure to store headers and data from CSV files
        private Dictionary<string, List<string[]>> csvData = new Dictionary<string, List<string[]>>();
        private Dictionary<string, string[]> csvHeaders = new Dictionary<string, string[]>();
        
        // Importers for each scriptable object type
        private Dictionary<string, IDataImporter> dataImporters = new Dictionary<string, IDataImporter>();
        
        // Mappings for sheet names to scriptable object types
        private Dictionary<string, Type> sheetToTypeMap = new Dictionary<string, Type>();
        
        // Fields to be mapped
        private Dictionary<string, Dictionary<string, string>> fieldMappings = new Dictionary<string, Dictionary<string, string>>();
        
        // Progress tracking
        private float importProgress = 0f;
        private string progressMessage = "";
        
        [MenuItem("Starkiller/Data Import/Import Game Data")]
        public static void ShowWindow()
        {
            GetWindow<DataImporter>("Game Data Importer");
        }
        
        private void OnEnable()
        {
            // Set default paths
            excelFilePath = EditorPrefs.GetString("StarkkillerExcelPath", "");
            csvFolderPath = EditorPrefs.GetString("StarkkillerCSVFolder", Application.dataPath + "/_Temp/CSV");
            
            // Initialize sheet to type mappings
            InitializeSheetToTypeMap();
        }
        
        private void InitializeSheetToTypeMap()
        {
            // Map sheet names to scriptable object types
            sheetToTypeMap.Clear();
            sheetToTypeMap.Add("CAPTAIN TYPES", typeof(CaptainType));
            sheetToTypeMap.Add("ORIGINS & DESTINATIONS", typeof(LocationData));
            sheetToTypeMap.Add("ACCESS CODES", typeof(AccessCode));
            sheetToTypeMap.Add("MANIFESTS", typeof(CargoManifest));
            sheetToTypeMap.Add("SCENARIOS", typeof(ShipScenario));
            sheetToTypeMap.Add("DAY RULES", typeof(DailyRule));
            sheetToTypeMap.Add("CONSEQUENCES", typeof(Consequence));
            sheetToTypeMap.Add("CONTRABAND ITEMS", typeof(ContrabandItem));
            sheetToTypeMap.Add("MEDIA ASSETS", typeof(MediaAsset));
            
            // Initialize importers
            dataImporters.Clear();
            dataImporters.Add("CAPTAIN TYPES", new CaptainTypeImporter());
            dataImporters.Add("ORIGINS & DESTINATIONS", new LocationImporter());
            dataImporters.Add("ACCESS CODES", new AccessCodeImporter());
            dataImporters.Add("MANIFESTS", new ManifestImporter());
            dataImporters.Add("SCENARIOS", new ScenarioImporter());
            dataImporters.Add("DAY RULES", new DayRuleImporter());
            dataImporters.Add("CONSEQUENCES", new ConsequenceImporter());
            dataImporters.Add("CONTRABAND ITEMS", new ContrabandImporter());
            dataImporters.Add("MEDIA ASSETS", new MediaAssetImporter());
            
            // Initialize field mappings
            InitializeFieldMappings();
        }
        
        private void InitializeFieldMappings()
        {
            fieldMappings.Clear();
            
            // Captain Type mappings
            var captainTypeMap = new Dictionary<string, string>();
            captainTypeMap.Add("ID", "typeName");
            captainTypeMap.Add("Name", "specialTraits"); // Using this field as a description name
            captainTypeMap.Add("Faction", "factions");
            captainTypeMap.Add("Ranks", "commonRanks");
            captainTypeMap.Add("First Names", "possibleFirstNames");
            captainTypeMap.Add("Last Names", "possibleLastNames");
            captainTypeMap.Add("Bribery Chance", "briberyChance");
            captainTypeMap.Add("Min Bribe", "minBribeAmount");
            captainTypeMap.Add("Max Bribe", "maxBribeAmount");
            captainTypeMap.Add("Behaviors", "typicalBehaviors");
            captainTypeMap.Add("Dialogue", "dialoguePatterns");
            captainTypeMap.Add("Bribery Phrases", "briberyPhrases");
            captainTypeMap.Add("Special Traits", "specialTraits");
            fieldMappings.Add("CAPTAIN TYPES", captainTypeMap);
            
            // Location mappings
            var locationMap = new Dictionary<string, string>();
            locationMap.Add("ID", "locationCode");
            locationMap.Add("Name", "locationName");
            locationMap.Add("Type", "type");
            locationMap.Add("Affiliation", "affiliation");
            locationMap.Add("Is Secret", "isSecretLocation");
            locationMap.Add("Description", "description");
            fieldMappings.Add("ORIGINS & DESTINATIONS", locationMap);
            
            // Access Code mappings
            var accessCodeMap = new Dictionary<string, string>();
            accessCodeMap.Add("ID", "codeName");
            accessCodeMap.Add("Code", "codeValue");
            accessCodeMap.Add("Type", "type");
            accessCodeMap.Add("Valid From Day", "validFromDay");
            accessCodeMap.Add("Valid Until Day", "validUntilDay");
            accessCodeMap.Add("Revoked", "isRevoked");
            accessCodeMap.Add("Access Level", "level");
            accessCodeMap.Add("Authorized Factions", "authorizedFactions");
            fieldMappings.Add("ACCESS CODES", accessCodeMap);
            
            // Manifest mappings
            var manifestMap = new Dictionary<string, string>();
            manifestMap.Add("ID", "manifestCode");
            manifestMap.Add("Name", "manifestName");
            manifestMap.Add("Declared Items", "declaredItems");
            manifestMap.Add("Actual Items", "actualItems");
            manifestMap.Add("Has Contraband", "hasContraband");
            manifestMap.Add("Is Falsified", "hasFalseEntries");
            manifestMap.Add("Clearance Code", "clearanceCode");
            manifestMap.Add("Authorized By", "authorizedBy");
            manifestMap.Add("Description", "manifestDescription");
            fieldMappings.Add("MANIFESTS", manifestMap);
            
            // Scenario mappings
            var scenarioMap = new Dictionary<string, string>();
            scenarioMap.Add("ID", "scenarioName");
            scenarioMap.Add("Name", "scenarioName");
            scenarioMap.Add("Type", "type");
            scenarioMap.Add("Story Mission", "isStoryMission");
            scenarioMap.Add("First Day", "dayFirstAppears");
            scenarioMap.Add("Story Tag", "storyTag");
            scenarioMap.Add("Should Approve", "shouldBeApproved");
            scenarioMap.Add("Invalid Reason", "invalidReason");
            scenarioMap.Add("Offers Bribe", "offersBribe");
            scenarioMap.Add("Bribe Multiplier", "bribeChanceMultiplier");
            scenarioMap.Add("Stories", "possibleStories");
            scenarioMap.Add("Manifests", "possibleManifests");
            scenarioMap.Add("Severity", "severityLevel");
            scenarioMap.Add("Consequences", "possibleConsequences");
            fieldMappings.Add("SCENARIOS", scenarioMap);
            
            // Day Rule mappings
            var ruleMap = new Dictionary<string, string>();
            ruleMap.Add("ID", "ruleName");
            ruleMap.Add("Day", "dayNumber");
            ruleMap.Add("Description", "ruleDescription");
            ruleMap.Add("Requirements", "requirementTexts");
            ruleMap.Add("Required Documents", "requiredDocuments");
            ruleMap.Add("Banned Origins", "bannedOrigins");
            ruleMap.Add("Banned Ships", "bannedShipTypes");
            ruleMap.Add("Special Requirements", "specialRequirements");
            fieldMappings.Add("DAY RULES", ruleMap);
            
            // Consequence mappings
            var consequenceMap = new Dictionary<string, string>();
            consequenceMap.Add("ID", "consequenceName");
            consequenceMap.Add("Type", "type");
            consequenceMap.Add("Severity", "severity");
            consequenceMap.Add("Affects Salary", "affectsSalary");
            consequenceMap.Add("Credit Penalty", "creditPenalty");
            consequenceMap.Add("Affects Morale", "affectsMorale");
            consequenceMap.Add("Morale Penalty", "moralePenalty");
            consequenceMap.Add("Triggers Investigation", "triggersInvestigation");
            consequenceMap.Add("Description", "consequenceDescription");
            consequenceMap.Add("Messages", "consequenceMessages");
            fieldMappings.Add("CONSEQUENCES", consequenceMap);
            
            // Contraband mappings
            var contrabandMap = new Dictionary<string, string>();
            contrabandMap.Add("ID", "itemName");
            contrabandMap.Add("Name", "itemName");
            contrabandMap.Add("Type", "type");
            contrabandMap.Add("Severity", "severity");
            contrabandMap.Add("Visible In Scans", "visibleInScans");
            contrabandMap.Add("Common Disguises", "commonDisguises");
            contrabandMap.Add("Common Containers", "commonContainers");
            contrabandMap.Add("Description", "itemDescription");
            fieldMappings.Add("CONTRABAND ITEMS", contrabandMap);
            
            // Media Asset mappings
            var mediaAssetMap = new Dictionary<string, string>();
            mediaAssetMap.Add("ID", "assetName");
            mediaAssetMap.Add("Type", "type");
            mediaAssetMap.Add("Associated ID", "associatedId");
            mediaAssetMap.Add("Description", "assetDescription");
            fieldMappings.Add("MEDIA ASSETS", mediaAssetMap);
        }
        
        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label("Starkiller Base Command - Game Data Importer", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            DrawFilePaths();
            EditorGUILayout.Space();
            
            DrawConversionOptions();
            EditorGUILayout.Space();
            
            DrawImportOptions();
            EditorGUILayout.Space();
            
            DrawProgressBar();
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawFilePaths()
        {
            GUILayout.Label("File Paths", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            excelFilePath = EditorGUILayout.TextField("Excel File:", excelFilePath);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string path = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
                if (!string.IsNullOrEmpty(path))
                {
                    excelFilePath = path;
                    EditorPrefs.SetString("StarkkillerExcelPath", excelFilePath);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            csvFolderPath = EditorGUILayout.TextField("CSV Folder:", csvFolderPath);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string path = EditorUtility.OpenFolderPanel("Select CSV Export Folder", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    csvFolderPath = path;
                    EditorPrefs.SetString("StarkkillerCSVFolder", csvFolderPath);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawConversionOptions()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            showConversionOptions = EditorGUILayout.Foldout(showConversionOptions, "Excel Conversion Options", true);
            EditorGUILayout.EndHorizontal();
            
            if (showConversionOptions)
            {
                EditorGUILayout.LabelField("Convert Excel file to CSV files (requires Excel on macOS)");
                EditorGUILayout.Space();
                
                if (GUILayout.Button("Convert Excel to CSV"))
                {
                    ConvertExcelToCSV();
                }
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawImportOptions()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            showImportOptions = EditorGUILayout.Foldout(showImportOptions, "Import Options", true);
            EditorGUILayout.EndHorizontal();
            
            if (showImportOptions)
            {
                EditorGUILayout.Space();
                
                if (GUILayout.Button("Read CSV Data"))
                {
                    ReadAllCSVFiles();
                }
                
                EditorGUILayout.Space();
                
                if (csvData.Count > 0)
                {
                    EditorGUILayout.LabelField("Available Data Sheets:", EditorStyles.boldLabel);
                    
                    foreach (var sheetName in csvData.Keys)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(sheetName, GUILayout.Width(150));
                        EditorGUILayout.LabelField(csvData[sheetName].Count.ToString() + " rows", GUILayout.Width(80));
                        
                        GUI.enabled = sheetToTypeMap.ContainsKey(sheetName);
                        if (GUILayout.Button("Import", GUILayout.Width(80)))
                        {
                            ImportSheet(sheetName);
                        }
                        GUI.enabled = true;
                        
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUILayout.Space();
                    
                    if (GUILayout.Button("Import All Supported Sheets"))
                    {
                        ImportAllSheets();
                    }
                }
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawProgressBar()
        {
            if (importProgress > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(progressMessage);
                Rect rect = EditorGUILayout.GetControlRect(false, 20);
                EditorGUI.ProgressBar(rect, importProgress, $"{(importProgress * 100).ToString("F0")}%");
            }
        }
        
        private void ConvertExcelToCSV()
        {
            if (string.IsNullOrEmpty(excelFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Please select an Excel file first!", "OK");
                return;
            }
            
            if (!File.Exists(excelFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Excel file does not exist: " + excelFilePath, "OK");
                return;
            }
            
            // Ensure CSV folder exists
            if (!Directory.Exists(csvFolderPath))
            {
                Directory.CreateDirectory(csvFolderPath);
            }
            
            progressMessage = "Converting Excel to CSV...";
            importProgress = 0.1f;
            EditorUtility.DisplayProgressBar("Converting Excel to CSV", "Starting conversion...", importProgress);
            
            try
            {
                // On macOS, we can use AppleScript to control Excel
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    ConvertExcelToCSVMac();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Excel conversion is currently only supported on macOS.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error converting Excel to CSV: " + ex.Message);
                EditorUtility.DisplayDialog("Error", "Failed to convert Excel to CSV: " + ex.Message, "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                importProgress = 0f;
            }
        }
        
        private void ConvertExcelToCSVMac()
        {
            string fileName = Path.GetFileNameWithoutExtension(excelFilePath);
            string tempScriptPath = Path.Combine(Path.GetTempPath(), "excel_to_csv.scpt");
            
            // Create AppleScript to control Excel and export each sheet as CSV
            string script = @"
                tell application ""Microsoft Excel""
                    set excelFile to """ + excelFilePath.Replace("\\", "\\\\").Replace("\"", "\\\"") + @"""
                    set csvFolder to """ + csvFolderPath.Replace("\\", "\\\\").Replace("\"", "\\\"") + @"""
                    
                    open excelFile
                    set workbook to active workbook
                    
                    set sheetCount to count of worksheets of workbook
                    repeat with i from 1 to sheetCount
                        set currentSheet to worksheet i of workbook
                        set sheetName to name of currentSheet
                        
                        set csvPath to csvFolder & ""/"" & sheetName & "".csv""
                        
                        save workbook in csvPath as CSV file format
                    end repeat
                    
                    close workbook saving no
                    quit
                end tell
            ";
            
            // Write the script to a temporary file
            File.WriteAllText(tempScriptPath, script);
            
            // Execute the AppleScript
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "osascript";
            process.StartInfo.Arguments = tempScriptPath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            
            importProgress = 0.3f;
            EditorUtility.DisplayProgressBar("Converting Excel to CSV", "Running AppleScript...", importProgress);
            
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            
            // Clean up
            if (File.Exists(tempScriptPath))
            {
                File.Delete(tempScriptPath);
            }
            
            importProgress = 1f;
            EditorUtility.DisplayProgressBar("Converting Excel to CSV", "Conversion completed", importProgress);
            
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("AppleScript error: " + error);
                EditorUtility.DisplayDialog("Error", "AppleScript error: " + error, "OK");
                return;
            }
            
            Debug.Log("Excel conversion completed. Output: " + output);
            EditorUtility.DisplayDialog("Success", "Excel file successfully converted to CSV files!", "OK");
            
            // Refresh the Asset Database
            AssetDatabase.Refresh();
        }
        
        private void ReadAllCSVFiles()
        {
            if (!Directory.Exists(csvFolderPath))
            {
                EditorUtility.DisplayDialog("Error", "CSV folder does not exist: " + csvFolderPath, "OK");
                return;
            }
            
            csvData.Clear();
            csvHeaders.Clear();
            
            string[] csvFiles = Directory.GetFiles(csvFolderPath, "*.csv");
            
            if (csvFiles.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No CSV files found in the specified folder!", "OK");
                return;
            }
            
            progressMessage = "Reading CSV files...";
            importProgress = 0f;
            EditorUtility.DisplayProgressBar("Reading CSV Files", "Starting...", importProgress);
            
            try
            {
                for (int i = 0; i < csvFiles.Length; i++)
                {
                    string filePath = csvFiles[i];
                    string sheetName = Path.GetFileNameWithoutExtension(filePath);
                    
                    importProgress = (float)(i + 1) / csvFiles.Length;
                    EditorUtility.DisplayProgressBar("Reading CSV Files", "Reading " + sheetName, importProgress);
                    
                    ReadCSVFile(sheetName, filePath);
                }
                
                Debug.Log("Successfully read " + csvData.Count + " CSV files.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error reading CSV files: " + ex.Message);
                EditorUtility.DisplayDialog("Error", "Failed to read CSV files: " + ex.Message, "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        
        private void ReadCSVFile(string sheetName, string filePath)
        {
            try
            {
                List<string[]> data = new List<string[]>();
                string[] headers = null;
                
                // Read all lines
                string[] lines = File.ReadAllLines(filePath);
                
                if (lines.Length <= 1)
                {
                    Debug.LogWarning("CSV file is empty or contains only headers: " + filePath);
                    return;
                }
                
                // Parse headers (first line)
                headers = ParseCSVLine(lines[0]);
                
                // Parse data (remaining lines)
                for (int i = 1; i < lines.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        string[] rowData = ParseCSVLine(lines[i]);
                        
                        // Ensure row has the correct number of columns
                        if (rowData.Length == headers.Length)
                        {
                            data.Add(rowData);
                        }
                        else
                        {
                            Debug.LogWarning($"Skipping row {i} in {sheetName}: Column count mismatch. Expected {headers.Length}, got {rowData.Length}");
                        }
                    }
                }
                
                // Store the data
                csvData[sheetName] = data;
                csvHeaders[sheetName] = headers;
                
                Debug.Log($"Read {data.Count} rows from {sheetName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading CSV file {filePath}: {ex.Message}");
                throw;
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
        
        private void ImportSheet(string sheetName)
        {
            if (!csvData.ContainsKey(sheetName) || !csvHeaders.ContainsKey(sheetName))
            {
                EditorUtility.DisplayDialog("Error", "No data found for sheet: " + sheetName, "OK");
                return;
            }
            
            if (!sheetToTypeMap.ContainsKey(sheetName))
            {
                EditorUtility.DisplayDialog("Error", "No ScriptableObject type mapping found for sheet: " + sheetName, "OK");
                return;
            }
            
            if (!dataImporters.ContainsKey(sheetName))
            {
                EditorUtility.DisplayDialog("Error", "No importer found for sheet: " + sheetName, "OK");
                return;
            }
            
            progressMessage = $"Importing {sheetName}...";
            importProgress = 0f;
            EditorUtility.DisplayProgressBar($"Importing {sheetName}", "Starting import...", importProgress);
            
            try
            {
                string[] headers = csvHeaders[sheetName];
                List<string[]> rows = csvData[sheetName];
                IDataImporter importer = dataImporters[sheetName];
                
                // Create the output folder if it doesn't exist
                string outputFolder = $"Assets/_ScriptableObjects/{sheetName}";
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }
                
                for (int i = 0; i < rows.Count; i++)
                {
                    importProgress = (float)(i + 1) / rows.Count;
                    EditorUtility.DisplayProgressBar($"Importing {sheetName}", $"Processing row {i + 1} of {rows.Count}", importProgress);
                    
                    string[] row = rows[i];
                    
                    // Create a dictionary of column name to value
                    Dictionary<string, string> rowData = new Dictionary<string, string>();
                    for (int j = 0; j < headers.Length && j < row.Length; j++)
                    {
                        rowData[headers[j]] = row[j];
                    }
                    
                    // Import the row
                    importer.ImportRow(rowData, fieldMappings[sheetName], outputFolder);
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                Debug.Log($"Successfully imported {rows.Count} {sheetName}.");
                EditorUtility.DisplayDialog("Success", $"Successfully imported {rows.Count} {sheetName}!", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error importing {sheetName}: {ex.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to import {sheetName}: {ex.Message}", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                importProgress = 0f;
            }
        }
        
        private void ImportAllSheets()
        {
            List<string> supportedSheets = new List<string>();
            
            foreach (var sheetName in csvData.Keys)
            {
                if (sheetToTypeMap.ContainsKey(sheetName) && dataImporters.ContainsKey(sheetName))
                {
                    supportedSheets.Add(sheetName);
                }
            }
            
            if (supportedSheets.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No supported sheets found in the data!", "OK");
                return;
            }
            
            progressMessage = "Importing all sheets...";
            importProgress = 0f;
            EditorUtility.DisplayProgressBar("Importing All Sheets", "Starting import...", importProgress);
            
            try
            {
                for (int i = 0; i < supportedSheets.Count; i++)
                {
                    string sheetName = supportedSheets[i];
                    importProgress = (float)i / supportedSheets.Count;
                    EditorUtility.DisplayProgressBar("Importing All Sheets", $"Importing {sheetName}...", importProgress);
                    
                    ImportSheet(sheetName);
                }
                
                Debug.Log($"Successfully imported all {supportedSheets.Count} supported sheets.");
                EditorUtility.DisplayDialog("Success", $"Successfully imported all {supportedSheets.Count} supported sheets!", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error importing sheets: {ex.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to import sheets: {ex.Message}", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                importProgress = 0f;
            }
        }
    }
    
    // Interface for data importers
    public interface IDataImporter
    {
        void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder);
    }
    
    // Ship Type Importer
    public class ShipTypeImporter : IDataImporter
    {
        public void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder)
        {
            string typeName = rowData["Type Name"];
            if (string.IsNullOrEmpty(typeName))
            {
                Debug.LogWarning("Skipping row with empty Type Name");
                return;
            }
            
            // Create the ScriptableObject
            ShipType shipType = ScriptableObject.CreateInstance<ShipType>();
            
            // Map string fields
            shipType.typeName = typeName;
            
            // Map int fields
            if (rowData.ContainsKey("Crew Min") && !string.IsNullOrEmpty(rowData["Crew Min"]))
            {
                shipType.minCrewSize = int.Parse(rowData["Crew Min"]);
            }
            
            if (rowData.ContainsKey("Crew Max") && !string.IsNullOrEmpty(rowData["Crew Max"]))
            {
                shipType.maxCrewSize = int.Parse(rowData["Crew Max"]);
            }
            
            // Map boolean fields
            if (rowData.ContainsKey("Is Imperial") && !string.IsNullOrEmpty(rowData["Is Imperial"]))
            {
                shipType.canBeUsedByOrder = rowData["Is Imperial"].ToUpper() == "TRUE";
            }
            
            if (rowData.ContainsKey("Is Insurgent") && !string.IsNullOrEmpty(rowData["Is Insurgent"]))
            {
                shipType.canBeInfiltrated = rowData["Is Insurgent"].ToUpper() == "TRUE";
            }
            
            if (rowData.ContainsKey("Is Smuggler") && !string.IsNullOrEmpty(rowData["Is Smuggler"]))
            {
                shipType.canSmuggle = rowData["Is Smuggler"].ToUpper() == "TRUE";
            }
            
            // Map array fields
            if (rowData.ContainsKey("Common Origins") && !string.IsNullOrEmpty(rowData["Common Origins"]))
            {
                shipType.commonOrigins = rowData["Common Origins"].Split(',').Select(s => s.Trim()).ToArray();
            }
            
            // Create a valid filename
            string safeFileName = Regex.Replace(typeName, @"[^\w\d]", "_");
            string assetPath = $"{outputFolder}/{safeFileName}.asset";
            
            // Create the asset
            AssetDatabase.CreateAsset(shipType, assetPath);
            Debug.Log($"Created ShipType: {typeName} at {assetPath}");
        }
    }
    
    // Captain Type Importer
    public class CaptainTypeImporter : IDataImporter
    {
        public void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder)
        {
            string typeName = rowData["Type Name"];
            if (string.IsNullOrEmpty(typeName))
            {
                Debug.LogWarning("Skipping row with empty Type Name");
                return;
            }
            
            // Create the ScriptableObject
            CaptainType captainType = ScriptableObject.CreateInstance<CaptainType>();
            
            // Map string fields
            captainType.typeName = typeName;
            
            // Map array fields
            if (rowData.ContainsKey("Factions") && !string.IsNullOrEmpty(rowData["Factions"]))
            {
                captainType.factions = rowData["Factions"].Split(',').Select(s => s.Trim()).ToArray();
            }
            
            if (rowData.ContainsKey("Possible Ranks") && !string.IsNullOrEmpty(rowData["Possible Ranks"]))
            {
                captainType.commonRanks = rowData["Possible Ranks"].Split(',').Select(s => s.Trim()).ToArray();
            }
            
            if (rowData.ContainsKey("First Name Options") && !string.IsNullOrEmpty(rowData["First Name Options"]))
            {
                captainType.possibleFirstNames = rowData["First Name Options"].Split(',').Select(s => s.Trim()).ToArray();
            }
            
            if (rowData.ContainsKey("Last Name Options") && !string.IsNullOrEmpty(rowData["Last Name Options"]))
            {
                captainType.possibleLastNames = rowData["Last Name Options"].Split(',').Select(s => s.Trim()).ToArray();
            }
            
            // Map float fields
            if (rowData.ContainsKey("Bribery Chance") && !string.IsNullOrEmpty(rowData["Bribery Chance"]))
            {
                captainType.briberyChance = float.Parse(rowData["Bribery Chance"]);
            }
            
            // Map int fields
            if (rowData.ContainsKey("Min Bribe") && !string.IsNullOrEmpty(rowData["Min Bribe"]))
            {
                captainType.minBribeAmount = int.Parse(rowData["Min Bribe"]);
            }
            
            if (rowData.ContainsKey("Max Bribe") && !string.IsNullOrEmpty(rowData["Max Bribe"]))
            {
                captainType.maxBribeAmount = int.Parse(rowData["Max Bribe"]);
            }
            
            // Map dialogue arrays
            if (rowData.ContainsKey("Typical Behaviors") && !string.IsNullOrEmpty(rowData["Typical Behaviors"]))
            {
                captainType.typicalBehaviors = rowData["Typical Behaviors"].Split(',').Select(s => s.Trim()).ToArray();
            }
            
            if (rowData.ContainsKey("Dialogue Patterns") && !string.IsNullOrEmpty(rowData["Dialogue Patterns"]))
            {
                captainType.dialoguePatterns = rowData["Dialogue Patterns"].Split(',').Select(s => s.Trim()).ToArray();
            }
            
            if (rowData.ContainsKey("Bribery Phrases") && !string.IsNullOrEmpty(rowData["Bribery Phrases"]))
            {
                captainType.briberyPhrases = rowData["Bribery Phrases"].Split(',').Select(s => s.Trim()).ToArray();
            }
            
            // Create a valid filename
            string safeFileName = Regex.Replace(typeName, @"[^\w\d]", "_");
            string assetPath = $"{outputFolder}/{safeFileName}.asset";
            
            // Create the asset
            AssetDatabase.CreateAsset(captainType, assetPath);
            Debug.Log($"Created CaptainType: {typeName} at {assetPath}");
        }
    }
    
    // Scenario Importer
    public class ScenarioImporter : IDataImporter
    {
        public void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder)
        {
            string scenarioName = "";
            
            // Try to get ID first, then Name
            if (rowData.ContainsKey("ID"))
                scenarioName = rowData["ID"];
            else if (rowData.ContainsKey("Name"))
                scenarioName = rowData["Name"];
                
            if (string.IsNullOrEmpty(scenarioName))
            {
                Debug.LogWarning("Skipping row with empty Scenario ID/Name");
                return;
            }
            
            // Create the ScriptableObject
            ShipScenario scenario = ScriptableObject.CreateInstance<ShipScenario>();
            
            // Map fields based on field mapping
            MapFields(scenario, rowData, fieldMapping);
            
            // Create a valid filename
            string safeFileName = Regex.Replace(scenarioName, @"[^\w\d]", "_");
            string assetPath = $"{outputFolder}/{safeFileName}.asset";
            
            // Create the asset
            AssetDatabase.CreateAsset(scenario, assetPath);
            Debug.Log($"Created ShipScenario: {scenarioName} at {assetPath}");
        }
        
        private void MapFields(ShipScenario scenario, Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping)
        {
            foreach (var mapping in fieldMapping)
            {
                if (!rowData.ContainsKey(mapping.Key) || string.IsNullOrEmpty(rowData[mapping.Key]))
                    continue;
                    
                string csvFieldName = mapping.Key;
                string objectFieldName = mapping.Value;
                string value = rowData[csvFieldName];
                
                var fieldInfo = typeof(ShipScenario).GetField(objectFieldName);
                if (fieldInfo == null)
                    continue;
                    
                if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(scenario, value);
                }
                else if (fieldInfo.FieldType == typeof(string[]))
                {
                    fieldInfo.SetValue(scenario, value.Split(',').Select(s => s.Trim()).ToArray());
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    int intValue;
                    if (int.TryParse(value, out intValue))
                        fieldInfo.SetValue(scenario, intValue);
                }
                else if (fieldInfo.FieldType == typeof(float))
                {
                    float floatValue;
                    if (float.TryParse(value, out floatValue))
                        fieldInfo.SetValue(scenario, floatValue);
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    fieldInfo.SetValue(scenario, value.ToUpper() == "TRUE" || value == "1" || value.ToUpper() == "YES");
                }
                else if (fieldInfo.FieldType.IsEnum)
                {
                    try
                    {
                        var enumValue = Enum.Parse(fieldInfo.FieldType, value, true);
                        fieldInfo.SetValue(scenario, enumValue);
                    }
                    catch
                    {
                        Debug.LogWarning($"Could not parse enum value '{value}' for field {objectFieldName}");
                    }
                }
            }
        }
    }
    
    // Location Importer
    public class LocationImporter : IDataImporter
    {
        public void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder)
        {
            string locationCode = "";
            
            if (rowData.ContainsKey("ID"))
                locationCode = rowData["ID"];
                
            if (string.IsNullOrEmpty(locationCode))
            {
                Debug.LogWarning("Skipping row with empty Location ID");
                return;
            }
            
            // Create the ScriptableObject
            LocationData location = ScriptableObject.CreateInstance<LocationData>();
            
            // Map the fields
            foreach (var mapping in fieldMapping)
            {
                if (!rowData.ContainsKey(mapping.Key) || string.IsNullOrEmpty(rowData[mapping.Key]))
                    continue;
                    
                string csvFieldName = mapping.Key;
                string objectFieldName = mapping.Value;
                string value = rowData[csvFieldName];
                
                var fieldInfo = typeof(LocationData).GetField(objectFieldName);
                if (fieldInfo == null)
                    continue;
                    
                if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(location, value);
                }
                else if (fieldInfo.FieldType == typeof(string[]))
                {
                    fieldInfo.SetValue(location, value.Split(',').Select(s => s.Trim()).ToArray());
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    fieldInfo.SetValue(location, value.ToUpper() == "TRUE" || value == "1" || value.ToUpper() == "YES");
                }
                else if (fieldInfo.FieldType.IsEnum)
                {
                    try
                    {
                        var enumValue = Enum.Parse(fieldInfo.FieldType, value, true);
                        fieldInfo.SetValue(location, enumValue);
                    }
                    catch
                    {
                        Debug.LogWarning($"Could not parse enum value '{value}' for field {objectFieldName}");
                    }
                }
            }
            
            // Create a valid filename
            string locationName = !string.IsNullOrEmpty(location.locationName) ? location.locationName : locationCode;
            string safeFileName = Regex.Replace(locationName, @"[^\w\d]", "_");
            string assetPath = $"{outputFolder}/{safeFileName}.asset";
            
            // Create the asset
            AssetDatabase.CreateAsset(location, assetPath);
            Debug.Log($"Created LocationData: {locationName} at {assetPath}");
        }
    }
    
    // Access Code Importer
    public class AccessCodeImporter : IDataImporter
    {
        public void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder)
        {
            string codeName = "";
            
            if (rowData.ContainsKey("ID"))
                codeName = rowData["ID"];
                
            if (string.IsNullOrEmpty(codeName))
            {
                Debug.LogWarning("Skipping row with empty Access Code ID");
                return;
            }
            
            // Create the ScriptableObject
            AccessCode accessCode = ScriptableObject.CreateInstance<AccessCode>();
            
            // Map the fields
            foreach (var mapping in fieldMapping)
            {
                if (!rowData.ContainsKey(mapping.Key) || string.IsNullOrEmpty(rowData[mapping.Key]))
                    continue;
                    
                string csvFieldName = mapping.Key;
                string objectFieldName = mapping.Value;
                string value = rowData[csvFieldName];
                
                var fieldInfo = typeof(AccessCode).GetField(objectFieldName);
                if (fieldInfo == null)
                    continue;
                    
                if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(accessCode, value);
                }
                else if (fieldInfo.FieldType == typeof(string[]))
                {
                    fieldInfo.SetValue(accessCode, value.Split(',').Select(s => s.Trim()).ToArray());
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    int intValue;
                    if (int.TryParse(value, out intValue))
                        fieldInfo.SetValue(accessCode, intValue);
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    fieldInfo.SetValue(accessCode, value.ToUpper() == "TRUE" || value == "1" || value.ToUpper() == "YES");
                }
                else if (fieldInfo.FieldType.IsEnum)
                {
                    try
                    {
                        var enumValue = Enum.Parse(fieldInfo.FieldType, value, true);
                        fieldInfo.SetValue(accessCode, enumValue);
                    }
                    catch
                    {
                        Debug.LogWarning($"Could not parse enum value '{value}' for field {objectFieldName}");
                    }
                }
            }
            
            // Create a valid filename
            string safeFileName = Regex.Replace(codeName, @"[^\w\d]", "_");
            string assetPath = $"{outputFolder}/{safeFileName}.asset";
            
            // Create the asset
            AssetDatabase.CreateAsset(accessCode, assetPath);
            Debug.Log($"Created AccessCode: {codeName} at {assetPath}");
        }
    }
    
    // Manifest Importer
    public class ManifestImporter : IDataImporter
    {
        public void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder)
        {
            string manifestCode = "";
            
            if (rowData.ContainsKey("ID"))
                manifestCode = rowData["ID"];
                
            if (string.IsNullOrEmpty(manifestCode))
            {
                Debug.LogWarning("Skipping row with empty Manifest ID");
                return;
            }
            
            // Create the ScriptableObject
            CargoManifest manifest = ScriptableObject.CreateInstance<CargoManifest>();
            
            // Map the fields
            foreach (var mapping in fieldMapping)
            {
                if (!rowData.ContainsKey(mapping.Key) || string.IsNullOrEmpty(rowData[mapping.Key]))
                    continue;
                    
                string csvFieldName = mapping.Key;
                string objectFieldName = mapping.Value;
                string value = rowData[csvFieldName];
                
                var fieldInfo = typeof(CargoManifest).GetField(objectFieldName);
                if (fieldInfo == null)
                    continue;
                    
                if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(manifest, value);
                }
                else if (fieldInfo.FieldType == typeof(string[]))
                {
                    fieldInfo.SetValue(manifest, value.Split(',').Select(s => s.Trim()).ToArray());
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    fieldInfo.SetValue(manifest, value.ToUpper() == "TRUE" || value == "1" || value.ToUpper() == "YES");
                }
            }
            
            // Create a valid filename
            string manifestName = !string.IsNullOrEmpty(manifest.manifestName) ? manifest.manifestName : manifestCode;
            string safeFileName = Regex.Replace(manifestName, @"[^\w\d]", "_");
            string assetPath = $"{outputFolder}/{safeFileName}.asset";
            
            // Create the asset
            AssetDatabase.CreateAsset(manifest, assetPath);
            Debug.Log($"Created CargoManifest: {manifestName} at {assetPath}");
        }
    }
    
    // Day Rule Importer
    public class DayRuleImporter : IDataImporter
    {
        public void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder)
        {
            string ruleName = "";
            
            if (rowData.ContainsKey("ID"))
                ruleName = rowData["ID"];
                
            if (string.IsNullOrEmpty(ruleName))
            {
                Debug.LogWarning("Skipping row with empty Day Rule ID");
                return;
            }
            
            // Create the ScriptableObject
            DailyRule rule = ScriptableObject.CreateInstance<DailyRule>();
            
            // Map the fields
            foreach (var mapping in fieldMapping)
            {
                if (!rowData.ContainsKey(mapping.Key) || string.IsNullOrEmpty(rowData[mapping.Key]))
                    continue;
                    
                string csvFieldName = mapping.Key;
                string objectFieldName = mapping.Value;
                string value = rowData[csvFieldName];
                
                var fieldInfo = typeof(DailyRule).GetField(objectFieldName);
                if (fieldInfo == null)
                    continue;
                    
                if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(rule, value);
                }
                else if (fieldInfo.FieldType == typeof(string[]))
                {
                    fieldInfo.SetValue(rule, value.Split(',').Select(s => s.Trim()).ToArray());
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    int intValue;
                    if (int.TryParse(value, out intValue))
                        fieldInfo.SetValue(rule, intValue);
                }
            }
            
            // Create a valid filename
            string safeFileName = Regex.Replace(ruleName, @"[^\w\d]", "_");
            string assetPath = $"{outputFolder}/{safeFileName}.asset";
            
            // Create the asset
            AssetDatabase.CreateAsset(rule, assetPath);
            Debug.Log($"Created DailyRule: {ruleName} at {assetPath}");
        }
    }
    
    // Consequence Importer
    public class ConsequenceImporter : IDataImporter
    {
        public void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder)
        {
            string consequenceName = "";
            
            if (rowData.ContainsKey("ID"))
                consequenceName = rowData["ID"];
                
            if (string.IsNullOrEmpty(consequenceName))
            {
                Debug.LogWarning("Skipping row with empty Consequence ID");
                return;
            }
            
            // Create the ScriptableObject
            Consequence consequence = ScriptableObject.CreateInstance<Consequence>();
            
            // Map the fields
            foreach (var mapping in fieldMapping)
            {
                if (!rowData.ContainsKey(mapping.Key) || string.IsNullOrEmpty(rowData[mapping.Key]))
                    continue;
                    
                string csvFieldName = mapping.Key;
                string objectFieldName = mapping.Value;
                string value = rowData[csvFieldName];
                
                var fieldInfo = typeof(Consequence).GetField(objectFieldName);
                if (fieldInfo == null)
                    continue;
                    
                if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(consequence, value);
                }
                else if (fieldInfo.FieldType == typeof(string[]))
                {
                    fieldInfo.SetValue(consequence, value.Split(',').Select(s => s.Trim()).ToArray());
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    int intValue;
                    if (int.TryParse(value, out intValue))
                        fieldInfo.SetValue(consequence, intValue);
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    fieldInfo.SetValue(consequence, value.ToUpper() == "TRUE" || value == "1" || value.ToUpper() == "YES");
                }
                else if (fieldInfo.FieldType.IsEnum)
                {
                    try
                    {
                        var enumValue = Enum.Parse(fieldInfo.FieldType, value, true);
                        fieldInfo.SetValue(consequence, enumValue);
                    }
                    catch
                    {
                        Debug.LogWarning($"Could not parse enum value '{value}' for field {objectFieldName}");
                    }
                }
            }
            
            // Create a valid filename
            string safeFileName = Regex.Replace(consequenceName, @"[^\w\d]", "_");
            string assetPath = $"{outputFolder}/{safeFileName}.asset";
            
            // Create the asset
            AssetDatabase.CreateAsset(consequence, assetPath);
            Debug.Log($"Created Consequence: {consequenceName} at {assetPath}");
        }
    }
    
    // Contraband Importer
    public class ContrabandImporter : IDataImporter
    {
        public void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder)
        {
            string itemName = "";
            
            if (rowData.ContainsKey("ID"))
                itemName = rowData["ID"];
            else if (rowData.ContainsKey("Name"))
                itemName = rowData["Name"];
                
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning("Skipping row with empty Contraband Item ID/Name");
                return;
            }
            
            // Create the ScriptableObject
            ContrabandItem item = ScriptableObject.CreateInstance<ContrabandItem>();
            
            // Map the fields
            foreach (var mapping in fieldMapping)
            {
                if (!rowData.ContainsKey(mapping.Key) || string.IsNullOrEmpty(rowData[mapping.Key]))
                    continue;
                    
                string csvFieldName = mapping.Key;
                string objectFieldName = mapping.Value;
                string value = rowData[csvFieldName];
                
                var fieldInfo = typeof(ContrabandItem).GetField(objectFieldName);
                if (fieldInfo == null)
                    continue;
                    
                if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(item, value);
                }
                else if (fieldInfo.FieldType == typeof(string[]))
                {
                    fieldInfo.SetValue(item, value.Split(',').Select(s => s.Trim()).ToArray());
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    fieldInfo.SetValue(item, value.ToUpper() == "TRUE" || value == "1" || value.ToUpper() == "YES");
                }
                else if (fieldInfo.FieldType.IsEnum)
                {
                    try
                    {
                        var enumValue = Enum.Parse(fieldInfo.FieldType, value, true);
                        fieldInfo.SetValue(item, enumValue);
                    }
                    catch
                    {
                        Debug.LogWarning($"Could not parse enum value '{value}' for field {objectFieldName}");
                    }
                }
            }
            
            // Create a valid filename
            string safeFileName = Regex.Replace(itemName, @"[^\w\d]", "_");
            string assetPath = $"{outputFolder}/{safeFileName}.asset";
            
            // Create the asset
            AssetDatabase.CreateAsset(item, assetPath);
            Debug.Log($"Created ContrabandItem: {itemName} at {assetPath}");
        }
    }
    
    // Media Asset Importer
    public class MediaAssetImporter : IDataImporter
    {
        public void ImportRow(Dictionary<string, string> rowData, Dictionary<string, string> fieldMapping, string outputFolder)
        {
            string assetName = "";
            
            if (rowData.ContainsKey("ID"))
                assetName = rowData["ID"];
                
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogWarning("Skipping row with empty Media Asset ID");
                return;
            }
            
            // Create the ScriptableObject
            MediaAsset mediaAsset = ScriptableObject.CreateInstance<MediaAsset>();
            
            // Map the fields
            foreach (var mapping in fieldMapping)
            {
                if (!rowData.ContainsKey(mapping.Key) || string.IsNullOrEmpty(rowData[mapping.Key]))
                    continue;
                    
                string csvFieldName = mapping.Key;
                string objectFieldName = mapping.Value;
                string value = rowData[csvFieldName];
                
                var fieldInfo = typeof(MediaAsset).GetField(objectFieldName);
                if (fieldInfo == null)
                    continue;
                    
                if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(mediaAsset, value);
                }
                else if (fieldInfo.FieldType.IsEnum)
                {
                    try
                    {
                        var enumValue = Enum.Parse(fieldInfo.FieldType, value, true);
                        fieldInfo.SetValue(mediaAsset, enumValue);
                    }
                    catch
                    {
                        Debug.LogWarning($"Could not parse enum value '{value}' for field {objectFieldName}");
                    }
                }
            }
            
            // Create a valid filename
            string safeFileName = Regex.Replace(assetName, @"[^\w\d]", "_");
            string assetPath = $"{outputFolder}/{safeFileName}.asset";
            
            // Create the asset
            AssetDatabase.CreateAsset(mediaAsset, assetPath);
            Debug.Log($"Created MediaAsset: {assetName} at {assetPath}");
        }
    }
}