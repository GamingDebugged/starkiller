using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using StarkillerBaseCommand.Encounters;

namespace StarkillerBaseCommand.Encounters.Editor
{
    public class EncounterMediaManagerEditor
    {
        [MenuItem("Assets/Create/Starkiller/Encounters/Encounter Media Manager")]
        public static void CreateEncounterMediaManager()
        {
            // Create the ScriptableObject
            EncounterMediaManager asset = ScriptableObject.CreateInstance<EncounterMediaManager>();
            
            // Determine the save path
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (System.IO.Path.GetExtension(path) != "")
            {
                path = path.Replace(System.IO.Path.GetFileName(path), "");
            }
            
            // Generate a unique asset name
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/NewEncounterMediaManager.asset");
            
            // Create the asset
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            
            // Select the created asset in the Project view
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}