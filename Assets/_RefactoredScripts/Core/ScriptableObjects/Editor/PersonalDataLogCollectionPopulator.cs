using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Starkiller.Core.ScriptableObjects;
using Starkiller.Core.Managers;

namespace Starkiller.Core.ScriptableObjects.Editor
{
    /// <summary>
    /// Editor utility to automatically populate PersonalDataLogCollectionSO with available entries
    /// </summary>
    public class PersonalDataLogCollectionPopulator : EditorWindow
    {
        private PersonalDataLogCollectionSO targetCollection;
        private bool autoDetectEntries = true;
        private string searchPath = "Assets/Resources/_ScriptableObjects/PersonalDataLog";
        
        [MenuItem("Starkiller/Personal Data Log/Populate Collection")]
        public static void ShowWindow()
        {
            GetWindow<PersonalDataLogCollectionPopulator>("Data Log Collection Populator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Personal Data Log Collection Populator", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            targetCollection = (PersonalDataLogCollectionSO)EditorGUILayout.ObjectField(
                "Target Collection", targetCollection, typeof(PersonalDataLogCollectionSO), false);
            
            GUILayout.Space(10);
            
            autoDetectEntries = EditorGUILayout.Toggle("Auto-detect Entries", autoDetectEntries);
            
            if (!autoDetectEntries)
            {
                searchPath = EditorGUILayout.TextField("Search Path", searchPath);
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Populate Collection"))
            {
                PopulateCollection();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Clear Collection"))
            {
                ClearCollection();
            }
            
            GUILayout.Space(20);
            
            // Show current collection contents
            if (targetCollection != null)
            {
                GUILayout.Label("Current Collection Contents:", EditorStyles.boldLabel);
                
                var imperiumCount = targetCollection.ImperiumNewsEntries?.Count ?? 0;
                var familyCount = targetCollection.FamilyChatEntries?.Count ?? 0;
                var frontierCount = targetCollection.FrontierEzineEntries?.Count ?? 0;
                var daySpecificCount = targetCollection.DaySpecificEntriesList?.Count ?? 0;
                var eventTriggeredCount = targetCollection.EventTriggeredEntriesList?.Count ?? 0;
                
                GUILayout.Label($"Imperium News: {imperiumCount} entries");
                GUILayout.Label($"Family Chat: {familyCount} entries");
                GUILayout.Label($"Frontier E-zine: {frontierCount} entries");
                GUILayout.Label($"Day Specific: {daySpecificCount} entries");
                GUILayout.Label($"Event Triggered: {eventTriggeredCount} entries");
            }
        }
        
        private void PopulateCollection()
        {
            if (targetCollection == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a target collection first.", "OK");
                return;
            }
            
            List<PersonalDataLogEntrySO> allEntries = new List<PersonalDataLogEntrySO>();
            
            if (autoDetectEntries)
            {
                // Find all PersonalDataLogEntrySO assets in the project
                string[] guids = AssetDatabase.FindAssets("t:PersonalDataLogEntrySO");
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var entry = AssetDatabase.LoadAssetAtPath<PersonalDataLogEntrySO>(assetPath);
                    if (entry != null)
                    {
                        allEntries.Add(entry);
                    }
                }
            }
            else
            {
                // Search in specified path
                string[] guids = AssetDatabase.FindAssets("t:PersonalDataLogEntrySO", new[] { searchPath });
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var entry = AssetDatabase.LoadAssetAtPath<PersonalDataLogEntrySO>(assetPath);
                    if (entry != null)
                    {
                        allEntries.Add(entry);
                    }
                }
            }
            
            if (allEntries.Count == 0)
            {
                EditorUtility.DisplayDialog("No Entries Found", "No PersonalDataLogEntrySO assets were found.", "OK");
                return;
            }
            
            // Group entries by feed type
            var imperiumEntries = allEntries.Where(e => e.feedType == FeedType.ImperiumNews).ToArray();
            var familyEntries = allEntries.Where(e => e.feedType == FeedType.FamilyChat).ToArray();
            var frontierEntries = allEntries.Where(e => e.feedType == FeedType.FrontierEzine).ToArray();
            
            // Assign to collection (just basic feed organization for now)
            Undo.RecordObject(targetCollection, "Populate Personal Data Log Collection");
            
            targetCollection.ImperiumNewsEntries = imperiumEntries.ToList();
            targetCollection.FamilyChatEntries = familyEntries.ToList();
            targetCollection.FrontierEzineEntries = frontierEntries.ToList();
            
            // For now, clear advanced collections - they can be manually organized later
            targetCollection.DaySpecificEntriesList = new List<PersonalDataLogCollectionSO.DaySpecificEntries>();
            targetCollection.EventTriggeredEntriesList = new List<PersonalDataLogCollectionSO.EventTriggeredEntries>();
            
            // Mark as dirty for saving
            EditorUtility.SetDirty(targetCollection);
            
            // Save assets
            AssetDatabase.SaveAssets();
            
            Debug.Log($"[PersonalDataLogCollectionPopulator] Populated collection with {allEntries.Count} total entries:");
            Debug.Log($"  - Imperium News: {imperiumEntries.Length}");
            Debug.Log($"  - Family Chat: {familyEntries.Length}");
            Debug.Log($"  - Frontier E-zine: {frontierEntries.Length}");
            
            EditorUtility.DisplayDialog("Success", 
                $"Collection populated with {allEntries.Count} entries!\n\n" +
                $"Imperium News: {imperiumEntries.Length}\n" +
                $"Family Chat: {familyEntries.Length}\n" +
                $"Frontier E-zine: {frontierEntries.Length}\n\n" +
                $"Advanced collections (Day Specific/Event Triggered) can be organized manually in the Inspector.", "OK");
        }
        
        private void ClearCollection()
        {
            if (targetCollection == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a target collection first.", "OK");
                return;
            }
            
            if (EditorUtility.DisplayDialog("Clear Collection", 
                "Are you sure you want to clear all entries from this collection?", "Yes", "No"))
            {
                Undo.RecordObject(targetCollection, "Clear Personal Data Log Collection");
                
                targetCollection.ImperiumNewsEntries = new List<PersonalDataLogEntrySO>();
                targetCollection.FamilyChatEntries = new List<PersonalDataLogEntrySO>();
                targetCollection.FrontierEzineEntries = new List<PersonalDataLogEntrySO>();
                targetCollection.DaySpecificEntriesList = new List<PersonalDataLogCollectionSO.DaySpecificEntries>();
                targetCollection.EventTriggeredEntriesList = new List<PersonalDataLogCollectionSO.EventTriggeredEntries>();
                
                EditorUtility.SetDirty(targetCollection);
                AssetDatabase.SaveAssets();
                
                Debug.Log("[PersonalDataLogCollectionPopulator] Collection cleared");
                EditorUtility.DisplayDialog("Success", "Collection cleared!", "OK");
            }
        }
    }
}