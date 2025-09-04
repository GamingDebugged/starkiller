using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace StarkillerBaseCommand.EditorTools
{
    public class DataModelGenerator : EditorWindow
    {
        private string dataModelName = "";
        private List<FieldDefinition> fields = new List<FieldDefinition>();
        private Vector2 scrollPosition;
        private string[] fieldTypes = new string[] 
        {
            "string", "int", "float", "bool", 
            "string[]", "int[]", "float[]", 
            "Sprite", "VideoClip",
            "ShipType", "CaptainType", "ShipScenario"
        };
        
        private string outputNamespace = "StarkillerBaseCommand";
        private string outputFolder = "Assets/_scripts";
        
        [MenuItem("Starkiller/Data Import/Generate Data Model")]
        public static void ShowWindow()
        {
            GetWindow<DataModelGenerator>("Data Model Generator");
        }
        
        private void OnEnable()
        {
            // Add a default field if list is empty
            if (fields.Count == 0)
            {
                fields.Add(new FieldDefinition { Name = "id", Type = "string", Description = "Unique identifier" });
                fields.Add(new FieldDefinition { Name = "name", Type = "string", Description = "Display name" });
            }
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Data Model Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Model name input
            dataModelName = EditorGUILayout.TextField("Model Name:", dataModelName);
            
            // Output settings
            outputNamespace = EditorGUILayout.TextField("Namespace:", outputNamespace);
            
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
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Fields section
            GUILayout.Label("Fields", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // Header row
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Name", EditorStyles.boldLabel, GUILayout.Width(150));
            GUILayout.Label("Type", EditorStyles.boldLabel, GUILayout.Width(150));
            GUILayout.Label("Description", EditorStyles.boldLabel, GUILayout.Width(200));
            GUILayout.Label("Actions", EditorStyles.boldLabel, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            
            // Field rows
            for (int i = 0; i < fields.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                fields[i].Name = EditorGUILayout.TextField(fields[i].Name, GUILayout.Width(150));
                
                int typeIndex = System.Array.IndexOf(fieldTypes, fields[i].Type);
                if (typeIndex < 0) typeIndex = 0;
                typeIndex = EditorGUILayout.Popup(typeIndex, fieldTypes, GUILayout.Width(150));
                fields[i].Type = fieldTypes[typeIndex];
                
                fields[i].Description = EditorGUILayout.TextField(fields[i].Description, GUILayout.Width(200));
                
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    fields.RemoveAt(i);
                    GUIUtility.ExitGUI();
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Add Field"))
            {
                fields.Add(new FieldDefinition { Name = "newField", Type = "string", Description = "" });
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
            
            // Generate button
            GUI.enabled = !string.IsNullOrEmpty(dataModelName) && fields.Count > 0;
            if (GUILayout.Button("Generate Model"))
            {
                GenerateModel();
            }
            GUI.enabled = true;
        }
        
        private void GenerateModel()
        {
            if (string.IsNullOrEmpty(dataModelName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a model name.", "OK");
                return;
            }
            
            // Make sure the first letter is capitalized
            string className = dataModelName;
            if (!string.IsNullOrEmpty(className) && char.IsLower(className[0]))
            {
                className = char.ToUpper(className[0]) + className.Substring(1);
            }
            
            StringBuilder sb = new StringBuilder();
            
            // Add header
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            
            // Add namespace
            sb.AppendLine($"namespace {outputNamespace}");
            sb.AppendLine("{");
            
            // Add class definition
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {className} data model for Starkiller Base Command");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    [CreateAssetMenu(fileName = \"{className}\", menuName = \"Starkiller/{className}\")]");
            sb.AppendLine($"    public class {className} : ScriptableObject");
            sb.AppendLine("    {");
            
            // Add fields
            foreach (var field in fields)
            {
                // Add comment if there's a description
                if (!string.IsNullOrEmpty(field.Description))
                {
                    sb.AppendLine($"        /// <summary>");
                    sb.AppendLine($"        /// {field.Description}");
                    sb.AppendLine($"        /// </summary>");
                }
                
                // Add Header attribute for better organization in inspector
                sb.AppendLine($"        [Header(\"{FormatHeader(field.Name)}\")]");
                
                // Add field
                string fieldType = MapFieldType(field.Type);
                sb.AppendLine($"        public {fieldType} {field.Name};");
                sb.AppendLine();
            }
            
            // Close class and namespace
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            // Ensure directory exists
            string directory = outputFolder;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Write the file
            string filePath = Path.Combine(directory, $"{className}.cs");
            File.WriteAllText(filePath, sb.ToString());
            
            // Refresh asset database
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", $"Model generated successfully at:\n{filePath}", "OK");
        }
        
        private string FormatHeader(string name)
        {
            // Capitalize first letter and add spaces before capital letters
            string result = "";
            for (int i = 0; i < name.Length; i++)
            {
                if (i == 0)
                {
                    result += char.ToUpper(name[i]);
                }
                else if (char.IsUpper(name[i]))
                {
                    result += " " + name[i];
                }
                else
                {
                    result += name[i];
                }
            }
            return result;
        }
        
        private string MapFieldType(string type)
        {
            switch (type)
            {
                case "string[]":
                    return "string[]";
                case "int[]":
                    return "int[]";
                case "float[]":
                    return "float[]";
                case "Sprite":
                    return "Sprite";
                case "VideoClip":
                    return "UnityEngine.Video.VideoClip";
                case "ShipType":
                case "CaptainType":
                case "ShipScenario":
                    return type;
                default:
                    return type;
            }
        }
    }
    
    public class FieldDefinition
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}