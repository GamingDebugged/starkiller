using UnityEngine;
using UnityEditor;
using StarkillerBaseCommand;

public class SimpleManifestFix : EditorWindow
{
    [MenuItem("Tools/Simple Manifest Fix")]
    public static void ShowWindow()
    {
        GetWindow<SimpleManifestFix>("Simple Fix");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Simple Manifest Fixer", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Fix Agricultural Livestock Transport"))
        {
            FixSingleManifest("Agricultural_Livestock_Transport");
        }
        
        if (GUILayout.Button("List All Manifests"))
        {
            ListManifests();
        }
    }

    static void FixSingleManifest(string fileName)
    {
        string path = "Assets/Resources/_ScriptableObjects/Manifests/" + fileName + ".asset";
        CargoManifest manifest = AssetDatabase.LoadAssetAtPath<CargoManifest>(path);
        
        if (manifest != null)
        {
            // Fix Agricultural Livestock Transport as test
            manifest.manifestName = "Agricultural Livestock Transport";
            manifest.manifestCode = "MANIFEST-1627";
            manifest.manifestDescription = "Standard livestock transport for base food production facilities";
            manifest.declaredItems = new string[] { "Bantha (6 head)", "Nerf (12 head)", "Feed Supplies (2 tons)", "Veterinary Equipment" };
            manifest.actualItems = new string[] { "Bantha (6 head)", "Nerf (12 head)", "Feed Supplies (2 tons)", "Veterinary Equipment" };
            manifest.totalCargoUnits = 20;
            manifest.cargoWeight = 15;
            manifest.factionRestriction = CargoManifest.FactionRestriction.Universal;
            manifest.allowedFactions = new string[] { };
            manifest.requiredShipCategories = new string[] { "Transport", "Merchant", "Civilian" };
            manifest.hasContraband = false;
            manifest.hasFalseEntries = false;
            manifest.contrabandType = CargoManifest.ContrabandType.None;
            manifest.contrabandItems = new string[] { };
            manifest.isEasilyDetectable = true;
            manifest.clearanceCode = "AGR-LIVE-3344";
            manifest.authorizedBy = "Agricultural Authority";
            manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
            manifest.firstAppearanceDay = 1;
            manifest.lastAppearanceDay = -1;
            manifest.maxDailyAppearances = 3;
            manifest.requiresSpecialValidation = false;
            manifest.validationKeywords = new string[] { "livestock", "agricultural", "food" };
            manifest.suspiciousKeywords = new string[] { };
            manifest.reputationImpactIfWrong = -10;
            manifest.creditRewardIfCorrect = 5;
            manifest.priority = CargoManifest.ManifestPriority.Normal;
            
            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
            
            Debug.Log("Fixed: " + fileName);
        }
        else
        {
            Debug.LogError("Could not find manifest: " + path);
        }
    }
    
    static void ListManifests()
    {
        string[] guids = AssetDatabase.FindAssets("t:CargoManifest", new[] { "Assets/Resources/_ScriptableObjects/Manifests" });
        Debug.Log($"Found {guids.Length} CargoManifest assets:");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CargoManifest manifest = AssetDatabase.LoadAssetAtPath<CargoManifest>(path);
            Debug.Log($"- {path} (Name: {manifest?.manifestName ?? "null"})");
        }
    }
}