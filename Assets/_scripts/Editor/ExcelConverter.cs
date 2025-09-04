using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;
using System.Text;

namespace StarkillerBaseCommand.EditorTools
{
    public class ExcelConverter : EditorWindow
    {
        private string excelFilePath = "";
        private string csvOutputFolder = "";
        private string pythonPath = "python3";
        private bool showAdvanced = false;
        private string conversionStatus = "";
        private bool isConverting = false;
        
        [MenuItem("Starkiller/Data Import/Convert Excel to CSV")]
        public static void ShowWindow()
        {
            GetWindow<ExcelConverter>("Excel to CSV");
        }
        
        private void OnEnable()
        {
            // Load saved paths
            excelFilePath = EditorPrefs.GetString("StarkkillerExcelPath", "");
            csvOutputFolder = EditorPrefs.GetString("StarkkillerCSVFolder", Application.dataPath + "/_Temp/CSV");
            pythonPath = EditorPrefs.GetString("StarkkillerPythonPath", "python3");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Excel to CSV Converter", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Excel file selection
            EditorGUILayout.BeginHorizontal();
            excelFilePath = EditorGUILayout.TextField("Excel File:", excelFilePath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
                if (!string.IsNullOrEmpty(path))
                {
                    excelFilePath = path;
                    EditorPrefs.SetString("StarkkillerExcelPath", excelFilePath);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // CSV output folder selection
            EditorGUILayout.BeginHorizontal();
            csvOutputFolder = EditorGUILayout.TextField("Output Folder:", csvOutputFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select CSV Output Folder", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    csvOutputFolder = path;
                    EditorPrefs.SetString("StarkkillerCSVFolder", csvOutputFolder);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // Advanced options
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Options", true);
            if (showAdvanced)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.BeginHorizontal();
                pythonPath = EditorGUILayout.TextField("Python Path:", pythonPath);
                if (GUILayout.Button("Test", GUILayout.Width(60)))
                {
                    TestPython();
                }
                EditorGUILayout.EndHorizontal();
                
                if (GUILayout.Button("Create Python Script"))
                {
                    CreatePythonScript();
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // Conversion button
            GUI.enabled = !isConverting && !string.IsNullOrEmpty(excelFilePath) && !string.IsNullOrEmpty(csvOutputFolder);
            if (GUILayout.Button("Convert Excel to CSV"))
            {
                ConvertExcelToCSV();
            }
            GUI.enabled = true;
            
            // Show status message
            if (!string.IsNullOrEmpty(conversionStatus))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(conversionStatus, MessageType.Info);
            }
        }
        
        private void TestPython()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = pythonPath;
                process.StartInfo.Arguments = "--version";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                
                if (string.IsNullOrEmpty(error))
                {
                    EditorUtility.DisplayDialog("Python Test", "Python is working correctly: " + output, "OK");
                    EditorPrefs.SetString("StarkkillerPythonPath", pythonPath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Python Error", "Error testing Python: " + error, "OK");
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Python Error", "Error testing Python: " + ex.Message, "OK");
            }
        }
        
        private void CreatePythonScript()
        {
            string scriptContent = @"#!/usr/bin/env python3
# Excel to CSV Converter for Starkiller Base Command
import pandas as pd
import os
import sys
import argparse
import re

def clean_filename(filename):
    # Clean a filename to be safe for filesystems
    # Replace spaces with underscores
    cleaned = filename.replace(' ', '_')
    # Remove any non-alphanumeric characters (except underscores)
    cleaned = re.sub(r'[^\w]', '', cleaned)
    return cleaned

def convert_excel_to_csv(excel_file, output_folder):
    # Convert Excel file to CSV, one file per sheet
    try:
        # Create output folder if it doesn't exist
        if not os.path.exists(output_folder):
            os.makedirs(output_folder)
            
        # Read Excel file
        excel = pd.ExcelFile(excel_file)
        sheet_names = excel.sheet_names
        
        # Convert each sheet to CSV
        for sheet in sheet_names:
            df = pd.read_excel(excel, sheet_name=sheet)
            
            # Clean up the data - handle NaN values
            df = df.fillna('')
            
            # Create a safe filename
            safe_sheet_name = clean_filename(sheet)
            csv_file = os.path.join(output_folder, f'{safe_sheet_name}.csv')
            
            # Save to CSV
            df.to_csv(csv_file, index=False, encoding='utf-8')
            print(f'Converted sheet {sheet} to {csv_file}')
            
        return True, f'Successfully converted {len(sheet_names)} sheets to CSV'
    except Exception as e:
        return False, f'Error converting Excel to CSV: {str(e)}'

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Convert Excel file to CSV, one file per sheet')
    parser.add_argument('excel_file', help='Path to Excel file')
    parser.add_argument('output_folder', help='Folder to output CSV files')
    
    args = parser.parse_args()
    success, message = convert_excel_to_csv(args.excel_file, args.output_folder)
    
    if success:
        print(message)
        sys.exit(0)
    else:
        print(message, file=sys.stderr)
        sys.exit(1)
";

            string scriptPath = Path.Combine(Application.dataPath, "_Temp");
            if (!Directory.Exists(scriptPath))
            {
                Directory.CreateDirectory(scriptPath);
            }
            
            string fullPath = Path.Combine(scriptPath, "excel_to_csv.py");
            File.WriteAllText(fullPath, scriptContent);
            
            conversionStatus = $"Python script created at:\n{fullPath}\n\nYou need to have pandas installed (pip install pandas)";
            
            EditorUtility.DisplayDialog("Script Created", 
                $"Python script created at:\n{fullPath}\n\nMake sure you have pandas installed (pip install pandas)", "OK");
        }
        
        private void ConvertExcelToCSV()
        {
            if (string.IsNullOrEmpty(excelFilePath) || !File.Exists(excelFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Please select a valid Excel file.", "OK");
                return;
            }
            
            if (string.IsNullOrEmpty(csvOutputFolder))
            {
                EditorUtility.DisplayDialog("Error", "Please select a CSV output folder.", "OK");
                return;
            }
            
            isConverting = true;
            conversionStatus = "Converting...";
            
            // Create the script if it doesn't exist
            string scriptPath = Path.Combine(Application.dataPath, "_Temp", "excel_to_csv.py");
            if (!File.Exists(scriptPath))
            {
                CreatePythonScript();
            }
            
            try
            {
                // Make output directory if it doesn't exist
                if (!Directory.Exists(csvOutputFolder))
                {
                    Directory.CreateDirectory(csvOutputFolder);
                }
                
                // Start the process
                Process process = new Process();
                process.StartInfo.FileName = pythonPath;
                process.StartInfo.Arguments = $"\"{scriptPath}\" \"{excelFilePath}\" \"{csvOutputFolder}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                
                StringBuilder outputBuilder = new StringBuilder();
                StringBuilder errorBuilder = new StringBuilder();
                
                process.OutputDataReceived += (sender, args) => {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        outputBuilder.AppendLine(args.Data);
                    }
                };
                
                process.ErrorDataReceived += (sender, args) => {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        errorBuilder.AppendLine(args.Data);
                    }
                };
                
                conversionStatus = "Starting Python conversion process...";
                Repaint();
                
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                
                string output = outputBuilder.ToString();
                string error = errorBuilder.ToString();
                
                if (process.ExitCode == 0)
                {
                    conversionStatus = "Conversion completed successfully!\n" + output;
                    AssetDatabase.Refresh();
                    EditorUtility.DisplayDialog("Success", "Excel file successfully converted to CSV!", "OK");
                }
                else
                {
                    conversionStatus = "Error during conversion:\n" + error;
                    EditorUtility.DisplayDialog("Error", "Failed to convert Excel to CSV:\n" + error, "OK");
                }
            }
            catch (Exception ex)
            {
                conversionStatus = "Exception occurred: " + ex.Message;
                EditorUtility.DisplayDialog("Error", "Exception occurred: " + ex.Message, "OK");
            }
            finally
            {
                isConverting = false;
                Repaint();
            }
        }
    }
}