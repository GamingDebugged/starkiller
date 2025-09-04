using UnityEngine;
using UnityEditor;
using StarkillerBaseCommand;

public class FixManifestAssets : EditorWindow
{
    [MenuItem("Tools/Fix Manifest Assets")]
    public static void ShowWindow()
    {
        GetWindow<FixManifestAssets>("Fix Manifests");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Fix Existing Manifest Assets", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("This will populate all existing manifest assets with proper data from the YAML specifications.", MessageType.Info);
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Fix All Manifest Assets", GUILayout.Height(30)))
        {
            FixAllManifests();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("List All Manifest Files"))
        {
            ListManifestFiles();
        }
    }

    static void FixAllManifests()
    {
        string basePath = "Assets/Resources/_ScriptableObjects/Manifests/";
        
        // Fix each manifest by loading and updating it
        FixManifest(basePath + "Agricultural_Livestock_Transport.asset", CreateManifest1Data);
        FixManifest(basePath + "Agricultural_Workers_Transport.asset", CreateManifest2Data);
        FixManifest(basePath + "Civilian_Passenger_Transport.asset", CreateManifest3Data);
        FixManifest(basePath + "Droid_Shipment.asset", CreateManifest4Data);
        FixManifest(basePath + "Energy_Storage_Units.asset", CreateManifest5Data);
        FixManifest(basePath + "Exotic_Creature_Transport.asset", CreateManifest6Data);
        FixManifest(basePath + "Food_Supply_Shipment.asset", CreateManifest7Data);
        FixManifest(basePath + "Industrial_Metals_Shipment.asset", CreateManifest8Data);
        FixManifest(basePath + "Laboratory_Equipment.asset", CreateManifest9Data);
        FixManifest(basePath + "Luxury_Beverage_Shipment.asset", CreateManifest10Data);
        FixManifest(basePath + "Medical_Blood_Supply.asset", CreateManifest11Data);
        FixManifest(basePath + "Medical_Research_Specimens.asset", CreateManifest12Data);
        FixManifest(basePath + "Medical_Research_Supplies.asset", CreateManifest13Data);
        FixManifest(basePath + "Medical_Transplant_Organs.asset", CreateManifest14Data);
        FixManifest(basePath + "Military_Arms_Shipment.asset", CreateManifest15Data);
        FixManifest(basePath + "Mining_Equipment.asset", CreateManifest16Data);
        FixManifest(basePath + "Mining_Personnel_Transfer.asset", CreateManifest17Data);
        FixManifest(basePath + "Prisoner_Transport_Living_Cargo.asset", CreateManifest18Data);
        FixManifest(basePath + "Prisoner_Transport_Deceased.asset", CreateManifest19Data);
        FixManifest(basePath + "Secure_Data_Transfer.asset", CreateManifest20Data);
        FixManifest(basePath + "Salvaged_Electronics.asset", CreateManifest21Data);
        FixManifest(basePath + "Pet_Transport.asset", CreateManifest22Data);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Fixed 22 manifest assets successfully!");
    }
    
    static void FixManifest(string assetPath, System.Action<CargoManifest> populateAction)
    {
        CargoManifest manifest = AssetDatabase.LoadAssetAtPath<CargoManifest>(assetPath);
        if (manifest != null)
        {
            populateAction(manifest);
            EditorUtility.SetDirty(manifest);
            Debug.Log($"Fixed: {assetPath}");
        }
        else
        {
            Debug.LogWarning($"Could not load manifest at: {assetPath}");
        }
    }
    
    static void ListManifestFiles()
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

    // 1. AGRICULTURAL LIVESTOCK TRANSPORT
    static void CreateManifest1Data(CargoManifest manifest)
    {
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
    }

    // 2. AGRICULTURAL WORKERS TRANSPORT - PEOPLE TRAFFICKING
    static void CreateManifest2Data(CargoManifest manifest)
    {
        manifest.manifestName = "Agricultural Workers Transport";
        manifest.manifestCode = "MANIFEST-1776";
        manifest.manifestDescription = "Transport of seasonal agricultural workers and equipment";
        manifest.declaredItems = new string[] { "Agricultural Equipment", "Farming Supplies", "Worker Provisions (20 persons)", "Tools and Machinery" };
        manifest.actualItems = new string[] { "Trafficked Individuals (20)", "Restraint Equipment", "Sedatives", "Forged Documents" };
        manifest.totalCargoUnits = 25;
        manifest.cargoWeight = 50;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "pirates", "insurgent" };
        manifest.requiredShipCategories = new string[] { "Transport", "Civilian" };
        manifest.hasContraband = true;
        manifest.hasFalseEntries = true;
        manifest.contrabandType = CargoManifest.ContrabandType.Personnel;
        manifest.contrabandItems = new string[] { "Human Trafficking", "Illegal Restraints", "Controlled Substances" };
        manifest.isEasilyDetectable = false;
        manifest.clearanceCode = "";
        manifest.authorizedBy = "Unknown Signature";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 3;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "agricultural", "workers", "seasonal" };
        manifest.suspiciousKeywords = new string[] { "restraint", "sedative", "trafficking" };
        manifest.reputationImpactIfWrong = -50;
        manifest.creditRewardIfCorrect = 25;
        manifest.priority = CargoManifest.ManifestPriority.Critical;
    }

    // 3. CIVILIAN PASSENGER TRANSPORT
    static void CreateManifest3Data(CargoManifest manifest)
    {
        manifest.manifestName = "Civilian Passenger Transport";
        manifest.manifestCode = "MANIFEST-7156";
        manifest.manifestDescription = "Standard civilian passenger transport with verified identities";
        manifest.declaredItems = new string[] { "Registered Passengers (12)", "Personal Belongings", "Travel Documents", "Food Supplies" };
        manifest.actualItems = new string[] { "Registered Passengers (12)", "Personal Belongings", "Travel Documents", "Food Supplies" };
        manifest.totalCargoUnits = 15;
        manifest.cargoWeight = 5;
        manifest.factionRestriction = CargoManifest.FactionRestriction.Universal;
        manifest.allowedFactions = new string[] { };
        manifest.requiredShipCategories = new string[] { "Civilian", "Transport" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "CIV-PASS-3321";
        manifest.authorizedBy = "Port Authority";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 5;
        manifest.requiresSpecialValidation = false;
        manifest.validationKeywords = new string[] { "passenger", "civilian", "transport" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -10;
        manifest.creditRewardIfCorrect = 5;
        manifest.priority = CargoManifest.ManifestPriority.Normal;
    }

    // 4. DROID SHIPMENT
    static void CreateManifest4Data(CargoManifest manifest)
    {
        manifest.manifestName = "Droid Shipment";
        manifest.manifestCode = "MANIFEST-3826";
        manifest.manifestDescription = "Standard droid shipment for base operations and maintenance";
        manifest.declaredItems = new string[] { "Protocol Droids (5 units)", "Maintenance Droids (10 units)", "Astromech Droids (3 units)", "Restraining Bolts" };
        manifest.actualItems = new string[] { "Protocol Droids (5 units)", "Maintenance Droids (10 units)", "Astromech Droids (3 units)", "Restraining Bolts" };
        manifest.totalCargoUnits = 18;
        manifest.cargoWeight = 20;
        manifest.factionRestriction = CargoManifest.FactionRestriction.Universal;
        manifest.allowedFactions = new string[] { };
        manifest.requiredShipCategories = new string[] { "Merchant", "Imperial", "Transport" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "DRD-IMP-3344";
        manifest.authorizedBy = "Droid Registration Bureau";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 3;
        manifest.requiresSpecialValidation = false;
        manifest.validationKeywords = new string[] { "droid", "maintenance", "protocol" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -10;
        manifest.creditRewardIfCorrect = 5;
        manifest.priority = CargoManifest.ManifestPriority.Normal;
    }

    // 5. ENERGY STORAGE UNITS
    static void CreateManifest5Data(CargoManifest manifest)
    {
        manifest.manifestName = "Energy Storage Units";
        manifest.manifestCode = "MANIFEST-2422";
        manifest.manifestDescription = "Standard power cell shipment for base energy requirements";
        manifest.declaredItems = new string[] { "Power Cells Type A (1000 units)", "Power Cells Type B (500 units)", "Charging Stations (20 units)", "Safety Equipment" };
        manifest.actualItems = new string[] { "Power Cells Type A (1000 units)", "Power Cells Type B (500 units)", "Charging Stations (20 units)", "Safety Equipment" };
        manifest.totalCargoUnits = 1520;
        manifest.cargoWeight = 300;
        manifest.factionRestriction = CargoManifest.FactionRestriction.Universal;
        manifest.allowedFactions = new string[] { };
        manifest.requiredShipCategories = new string[] { "Merchant", "Transport", "Imperial" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "PWR-SUP-5567";
        manifest.authorizedBy = "Energy Management Division";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 4;
        manifest.requiresSpecialValidation = false;
        manifest.validationKeywords = new string[] { "power", "energy", "cells" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -10;
        manifest.creditRewardIfCorrect = 5;
        manifest.priority = CargoManifest.ManifestPriority.Normal;
    }

    // 6. EXOTIC CREATURE TRANSPORT
    static void CreateManifest6Data(CargoManifest manifest)
    {
        manifest.manifestName = "Exotic Creature Transport";
        manifest.manifestCode = "MANIFEST-8851";
        manifest.manifestDescription = "Licensed transport of exotic fauna for research purposes";
        manifest.declaredItems = new string[] { "Nexu (2 specimens)", "Ysalamir (6 specimens)", "Voorpak (4 specimens)", "Care Instructions" };
        manifest.actualItems = new string[] { "Nexu (2 specimens)", "Ysalamir (6 specimens)", "Voorpak (4 specimens)", "Care Instructions" };
        manifest.totalCargoUnits = 12;
        manifest.cargoWeight = 8;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "merchant", "theorder", "civilian" };
        manifest.requiredShipCategories = new string[] { "Transport", "Merchant" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "ZOO-EXO-4421";
        manifest.authorizedBy = "Xenobiology Department";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Restricted;
        manifest.firstAppearanceDay = 5;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "exotic", "creature", "research" };
        manifest.suspiciousKeywords = new string[] { "nexu", "ysalamir" };
        manifest.reputationImpactIfWrong = -15;
        manifest.creditRewardIfCorrect = 10;
        manifest.priority = CargoManifest.ManifestPriority.High;
    }

    // 7. FOOD SUPPLY SHIPMENT
    static void CreateManifest7Data(CargoManifest manifest)
    {
        manifest.manifestName = "Food Supply Shipment";
        manifest.manifestCode = "MANIFEST-9221";
        manifest.manifestDescription = "Standard food supply delivery for base personnel";
        manifest.declaredItems = new string[] { "Protein Rations (5000 units)", "Hydration Packs (3000 units)", "Fresh Produce (200 kg)", "Cooking Supplies" };
        manifest.actualItems = new string[] { "Protein Rations (5000 units)", "Hydration Packs (3000 units)", "Fresh Produce (200 kg)", "Cooking Supplies" };
        manifest.totalCargoUnits = 8200;
        manifest.cargoWeight = 4100;
        manifest.factionRestriction = CargoManifest.FactionRestriction.Universal;
        manifest.allowedFactions = new string[] { };
        manifest.requiredShipCategories = new string[] { "Merchant", "Transport", "Civilian" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "";
        manifest.authorizedBy = "Supply Corps";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 5;
        manifest.requiresSpecialValidation = false;
        manifest.validationKeywords = new string[] { };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -5;
        manifest.creditRewardIfCorrect = 5;
        manifest.priority = CargoManifest.ManifestPriority.Normal;
    }

    // 8. INDUSTRIAL METALS SHIPMENT
    static void CreateManifest8Data(CargoManifest manifest)
    {
        manifest.manifestName = "Industrial Metals Shipment";
        manifest.manifestCode = "MANIFEST-8139";
        manifest.manifestDescription = "Industrial metal shipment for base construction projects";
        manifest.declaredItems = new string[] { "Durasteel Ingots (500 tons)", "Titanium Alloy (200 tons)", "Copper Wiring (50 tons)", "Welding Supplies" };
        manifest.actualItems = new string[] { "Durasteel Ingots (500 tons)", "Titanium Alloy (200 tons)", "Copper Wiring (50 tons)", "Welding Supplies" };
        manifest.totalCargoUnits = 750;
        manifest.cargoWeight = 750;
        manifest.factionRestriction = CargoManifest.FactionRestriction.Universal;
        manifest.allowedFactions = new string[] { };
        manifest.requiredShipCategories = new string[] { "Merchant", "Transport", "Imperial" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "IND-MET-7788";
        manifest.authorizedBy = "Mining Guild";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 3;
        manifest.requiresSpecialValidation = false;
        manifest.validationKeywords = new string[] { "metal", "industrial", "construction" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -10;
        manifest.creditRewardIfCorrect = 5;
        manifest.priority = CargoManifest.ManifestPriority.Normal;
    }

    // 9. LABORATORY EQUIPMENT
    static void CreateManifest9Data(CargoManifest manifest)
    {
        manifest.manifestName = "Laboratory Equipment";
        manifest.manifestCode = "MANIFEST-4260";
        manifest.manifestDescription = "Advanced laboratory equipment for research facility";
        manifest.declaredItems = new string[] { "Quantum Analyzers (3 units)", "Microscopes (10 units)", "Computer Terminals (15 units)", "Calibration Tools" };
        manifest.actualItems = new string[] { "Quantum Analyzers (3 units)", "Microscopes (10 units)", "Computer Terminals (15 units)", "Calibration Tools" };
        manifest.totalCargoUnits = 30;
        manifest.cargoWeight = 25;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "imperial", "scientific", "theorder" };
        manifest.requiredShipCategories = new string[] { "Scientific", "Imperial", "Transport" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "SCI-EQP-2234";
        manifest.authorizedBy = "Science Division";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Restricted;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 2;
        manifest.requiresSpecialValidation = false;
        manifest.validationKeywords = new string[] { "laboratory", "scientific", "research" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -15;
        manifest.creditRewardIfCorrect = 10;
        manifest.priority = CargoManifest.ManifestPriority.High;
    }

    // 10. LUXURY BEVERAGE SHIPMENT
    static void CreateManifest10Data(CargoManifest manifest)
    {
        manifest.manifestName = "Luxury Beverage Shipment";
        manifest.manifestCode = "LB001";
        manifest.manifestDescription = "Licensed import of luxury beverages for officer's club";
        manifest.declaredItems = new string[] { "Corellian Brandy (50 cases)", "Alderaanian Wine (100 bottles)", "Bespin Port (25 cases)", "Import Licenses" };
        manifest.actualItems = new string[] { "Corellian Brandy (50 cases)", "Alderaanian Wine (100 bottles)", "Bespin Port (25 cases)", "Import Licenses" };
        manifest.totalCargoUnits = 175;
        manifest.cargoWeight = 350;
        manifest.factionRestriction = CargoManifest.FactionRestriction.Universal;
        manifest.allowedFactions = new string[] { };
        manifest.requiredShipCategories = new string[] { "Merchant", "Transport" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "TRD-LUX-6672";
        manifest.authorizedBy = "Trade Federation";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Restricted;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 3;
        manifest.requiresSpecialValidation = false;
        manifest.validationKeywords = new string[] { "luxury", "beverage", "import" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -10;
        manifest.creditRewardIfCorrect = 10;
        manifest.priority = CargoManifest.ManifestPriority.Normal;
    }

    // 11. MEDICAL BLOOD SUPPLY
    static void CreateManifest11Data(CargoManifest manifest)
    {
        manifest.manifestName = "Medical Blood Supply";
        manifest.manifestCode = "MANIFEST-8726";
        manifest.manifestDescription = "Emergency blood supply for base medical facility";
        manifest.declaredItems = new string[] { "Blood Units Type O (500 units)", "Blood Units Type A (300 units)", "Plasma (200 units)", "Storage Equipment" };
        manifest.actualItems = new string[] { "Blood Units Type O (500 units)", "Blood Units Type A (300 units)", "Plasma (200 units)", "Storage Equipment" };
        manifest.totalCargoUnits = 1000;
        manifest.cargoWeight = 100;
        manifest.factionRestriction = CargoManifest.FactionRestriction.Universal;
        manifest.allowedFactions = new string[] { };
        manifest.requiredShipCategories = new string[] { "Medical", "Imperial", "Transport" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "MED-BLD-7766";
        manifest.authorizedBy = "Medical Corps";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Restricted;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 2;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "medical", "blood", "emergency" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -20;
        manifest.creditRewardIfCorrect = 10;
        manifest.priority = CargoManifest.ManifestPriority.High;
    }

    // 12. MEDICAL RESEARCH SPECIMENS
    static void CreateManifest12Data(CargoManifest manifest)
    {
        manifest.manifestName = "Medical Research Specimens";
        manifest.manifestCode = "MANIFEST-5062";
        manifest.manifestDescription = "Biological specimens for advanced medical research";
        manifest.declaredItems = new string[] { "Biological Samples", "Research Materials", "Preservation Equipment", "Documentation" };
        manifest.actualItems = new string[] { "Black Market Organs", "Unknown Origin Body Parts", "No Documentation", "Illegal Preservation" };
        manifest.totalCargoUnits = 50;
        manifest.cargoWeight = 40;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "pirates", "insurgent" };
        manifest.requiredShipCategories = new string[] { "Transport", "Civilian" };
        manifest.hasContraband = true;
        manifest.hasFalseEntries = true;
        manifest.contrabandType = CargoManifest.ContrabandType.Technology;
        manifest.contrabandItems = new string[] { "Illegal Organs", "Body Parts", "No Documentation" };
        manifest.isEasilyDetectable = false;
        manifest.clearanceCode = "";
        manifest.authorizedBy = "Dr. Anonymous";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 5;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "medical", "research", "specimen" };
        manifest.suspiciousKeywords = new string[] { "organ", "body part", "anonymous" };
        manifest.reputationImpactIfWrong = -60;
        manifest.creditRewardIfCorrect = 30;
        manifest.priority = CargoManifest.ManifestPriority.Critical;
    }

    // 13. MEDICAL RESEARCH SUPPLIES - BIOWEAPON
    static void CreateManifest13Data(CargoManifest manifest)
    {
        manifest.manifestName = "Medical Research Supplies";
        manifest.manifestCode = "MANIFEST-7971";
        manifest.manifestDescription = "Routine medical research materials for disease prevention";
        manifest.declaredItems = new string[] { "Medical Equipment", "Vaccine Samples", "Research Notes", "Lab Supplies" };
        manifest.actualItems = new string[] { "Weaponized Pathogens", "Bioweapon Deployment Systems", "Contamination Gear", "False Documentation" };
        manifest.totalCargoUnits = 100;
        manifest.cargoWeight = 80;
        manifest.factionRestriction = CargoManifest.FactionRestriction.Restricted;
        manifest.allowedFactions = new string[] { "insurgent", "specialinterest" };
        manifest.requiredShipCategories = new string[] { "Scientific", "Transport" };
        manifest.hasContraband = true;
        manifest.hasFalseEntries = true;
        manifest.contrabandType = CargoManifest.ContrabandType.Mixed;
        manifest.contrabandItems = new string[] { "Bioweapons", "Weaponized Pathogens", "WMD Components" };
        manifest.isEasilyDetectable = false;
        manifest.clearanceCode = "";
        manifest.authorizedBy = "Dr. Kess (Forged)";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 10;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "medical", "vaccine", "research" };
        manifest.suspiciousKeywords = new string[] { "pathogen", "weapon", "contamination" };
        manifest.reputationImpactIfWrong = -100;
        manifest.creditRewardIfCorrect = 50;
        manifest.priority = CargoManifest.ManifestPriority.Critical;
    }

    // 14. MEDICAL TRANSPLANT ORGANS
    static void CreateManifest14Data(CargoManifest manifest)
    {
        manifest.manifestName = "Medical Transplant Organs";
        manifest.manifestCode = "MANIFEST-4910";
        manifest.manifestDescription = "Authorized organ transport for emergency medical procedures";
        manifest.declaredItems = new string[] { "Cryo-preserved Organs", "Medical Documentation", "Transplant Equipment", "Preservation Fluids" };
        manifest.actualItems = new string[] { "Cryo-preserved Organs", "Medical Documentation", "Transplant Equipment", "Preservation Fluids" };
        manifest.totalCargoUnits = 20;
        manifest.cargoWeight = 15;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "imperial", "medical", "civilian" };
        manifest.requiredShipCategories = new string[] { "Medical", "Imperial" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "MED-TRN-9988";
        manifest.authorizedBy = "Imperial Medical Authority";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Classified;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "transplant", "organ", "medical" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -30;
        manifest.creditRewardIfCorrect = 15;
        manifest.priority = CargoManifest.ManifestPriority.High;
    }

    // 15. MILITARY ARMS SHIPMENT
    static void CreateManifest15Data(CargoManifest manifest)
    {
        manifest.manifestName = "Military Arms Shipment";
        manifest.manifestCode = "MANIFEST-5089";
        manifest.manifestDescription = "Authorized military weapons transfer to Starkiller Base armory";
        manifest.declaredItems = new string[] { "Blaster Rifles (200 units)", "Power Packs (1000 units)", "Thermal Detonators (50 units)", "Armor Plating (100 sets)" };
        manifest.actualItems = new string[] { "Blaster Rifles (200 units)", "Power Packs (1000 units)", "Thermal Detonators (50 units)", "Armor Plating (100 sets)" };
        manifest.totalCargoUnits = 1350;
        manifest.cargoWeight = 2700;
        manifest.factionRestriction = CargoManifest.FactionRestriction.Restricted;
        manifest.allowedFactions = new string[] { "imperial", "military" };
        manifest.requiredShipCategories = new string[] { "Military", "Imperial" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "MIL-ARMS-2341";
        manifest.authorizedBy = "Imperial Quartermaster";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Classified;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 2;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "military", "weapons", "authorized" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -25;
        manifest.creditRewardIfCorrect = 15;
        manifest.priority = CargoManifest.ManifestPriority.High;
    }

    // 16. MINING EQUIPMENT - HIDDEN WEAPONS
    static void CreateManifest16Data(CargoManifest manifest)
    {
        manifest.manifestName = "Mining Equipment";
        manifest.manifestCode = "MANIFEST-9158";
        manifest.manifestDescription = "Standard mining equipment for asteroid operations";
        manifest.declaredItems = new string[] { "Mining Explosives", "Drilling Equipment", "Safety Gear", "Ore Processing Tools" };
        manifest.actualItems = new string[] { "Disruptor Weapons", "Ion Torpedoes", "Illegal Weapon Mods", "Forged Mining Permits" };
        manifest.totalCargoUnits = 500;
        manifest.cargoWeight = 1500;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "pirates", "insurgent" };
        manifest.requiredShipCategories = new string[] { "Merchant", "Civilian" };
        manifest.hasContraband = true;
        manifest.hasFalseEntries = true;
        manifest.contrabandType = CargoManifest.ContrabandType.Weapons;
        manifest.contrabandItems = new string[] { "Illegal Weapons", "Ion Weaponry", "Weapon Modifications" };
        manifest.isEasilyDetectable = false;
        manifest.clearanceCode = "";
        manifest.authorizedBy = "Forged Signature";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 3;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = false;
        manifest.validationKeywords = new string[] { };
        manifest.suspiciousKeywords = new string[] { "disruptor", "ion", "weapon" };
        manifest.reputationImpactIfWrong = -40;
        manifest.creditRewardIfCorrect = 20;
        manifest.priority = CargoManifest.ManifestPriority.High;
    }

    // 17. MINING PERSONNEL TRANSFER - SLAVES
    static void CreateManifest17Data(CargoManifest manifest)
    {
        manifest.manifestName = "Mining Personnel Transfer";
        manifest.manifestCode = "MANIFEST-4256";
        manifest.manifestDescription = "Voluntary contract workers for mining operations";
        manifest.declaredItems = new string[] { "Contract Workers (20)", "Mining Equipment", "Worker Quarters Supplies", "Safety Gear" };
        manifest.actualItems = new string[] { "Enslaved Individuals (20)", "Shock Collars", "Restraint Systems", "No Safety Equipment" };
        manifest.totalCargoUnits = 25;
        manifest.cargoWeight = 50;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "pirates", "specialinterest" };
        manifest.requiredShipCategories = new string[] { "Transport", "Civilian" };
        manifest.hasContraband = true;
        manifest.hasFalseEntries = true;
        manifest.contrabandType = CargoManifest.ContrabandType.Personnel;
        manifest.contrabandItems = new string[] { "Slave Labor", "Shock Devices", "Human Rights Violations" };
        manifest.isEasilyDetectable = false;
        manifest.clearanceCode = "";
        manifest.authorizedBy = "Outer Rim Mining Corp (Fake)";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 5;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "mining", "personnel", "contract" };
        manifest.suspiciousKeywords = new string[] { "slave", "shock", "restraint" };
        manifest.reputationImpactIfWrong = -70;
        manifest.creditRewardIfCorrect = 35;
        manifest.priority = CargoManifest.ManifestPriority.Critical;
    }

    // 18. PRISONER TRANSPORT - BOUNTY ALIVE
    static void CreateManifest18Data(CargoManifest manifest)
    {
        manifest.manifestName = "Prisoner Transport - Living Cargo";
        manifest.manifestCode = "BH001";
        manifest.manifestDescription = "Authorized transport of living bounty target for Imperial justice";
        manifest.declaredItems = new string[] { "Prisoner Transport Authorization", "Life Support Equipment", "Security Restraints", "Medical Supplies" };
        manifest.actualItems = new string[] { "Prisoner Transport Authorization", "Life Support Equipment", "Security Restraints", "Medical Supplies" };
        manifest.totalCargoUnits = 10;
        manifest.cargoWeight = 5;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "bountyhunters", "imperial", "military" };
        manifest.requiredShipCategories = new string[] { "BountyHunter", "Imperial" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "BNT-LIVE-7734";
        manifest.authorizedBy = "Bounty Hunter Guild";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Restricted;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 2;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "bounty", "prisoner", "authorized" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -20;
        manifest.creditRewardIfCorrect = 15;
        manifest.priority = CargoManifest.ManifestPriority.High;
    }

    // 19. PRISONER TRANSPORT - BOUNTY DEAD
    static void CreateManifest19Data(CargoManifest manifest)
    {
        manifest.manifestName = "Prisoner Transport - Deceased";
        manifest.manifestCode = "BH002";
        manifest.manifestDescription = "Transport of deceased bounty target for reward collection";
        manifest.declaredItems = new string[] { "Bounty Completion Certificate", "Cryo-preservation Unit", "Death Certificate", "Guild Authorization" };
        manifest.actualItems = new string[] { "Bounty Completion Certificate", "Cryo-preservation Unit", "Death Certificate", "Guild Authorization" };
        manifest.totalCargoUnits = 5;
        manifest.cargoWeight = 3;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "bountyhunters", "imperial" };
        manifest.requiredShipCategories = new string[] { "BountyHunter" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "BNT-DEAD-9921";
        manifest.authorizedBy = "Bounty Hunter Guild";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Restricted;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "bounty", "deceased", "certificate" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -20;
        manifest.creditRewardIfCorrect = 15;
        manifest.priority = CargoManifest.ManifestPriority.High;
    }

    // 20. CLASSIFIED DATA TRANSFER
    static void CreateManifest20Data(CargoManifest manifest)
    {
        manifest.manifestName = "Secure Data Transfer";
        manifest.manifestCode = "SD001";
        manifest.manifestDescription = "Classified data transfer - Level 5 clearance required";
        manifest.declaredItems = new string[] { "Encrypted Data Cores", "Secure Communication Equipment", "Authorization Codes", "Biometric Locks" };
        manifest.actualItems = new string[] { "Encrypted Data Cores", "Secure Communication Equipment", "Authorization Codes", "Biometric Locks" };
        manifest.totalCargoUnits = 5;
        manifest.cargoWeight = 2;
        manifest.factionRestriction = CargoManifest.FactionRestriction.Restricted;
        manifest.allowedFactions = new string[] { "imperial", "military" };
        manifest.requiredShipCategories = new string[] { "Imperial", "Military" };
        manifest.hasContraband = false;
        manifest.hasFalseEntries = false;
        manifest.contrabandType = CargoManifest.ContrabandType.None;
        manifest.contrabandItems = new string[] { };
        manifest.isEasilyDetectable = true;
        manifest.clearanceCode = "IMP-SEC-8834";
        manifest.authorizedBy = "Imperial Intelligence";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Classified;
        manifest.firstAppearanceDay = 1;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "classified", "secure", "encrypted" };
        manifest.suspiciousKeywords = new string[] { };
        manifest.reputationImpactIfWrong = -50;
        manifest.creditRewardIfCorrect = 20;
        manifest.priority = CargoManifest.ManifestPriority.High;
    }

    // 21. STOLEN GOODS
    static void CreateManifest21Data(CargoManifest manifest)
    {
        manifest.manifestName = "Salvaged Electronics";
        manifest.manifestCode = "SE443";
        manifest.manifestDescription = "Recycled electronic components from decommissioned vessels";
        manifest.declaredItems = new string[] { "Used Computer Parts", "Scrap Electronics", "Recycled Components", "Salvage Documentation" };
        manifest.actualItems = new string[] { "Imperial Decoder Units", "Military Comm Equipment", "Classified Data Cores", "Fake Documentation" };
        manifest.totalCargoUnits = 100;
        manifest.cargoWeight = 50;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "pirates", "insurgent" };
        manifest.requiredShipCategories = new string[] { "Merchant", "Civilian" };
        manifest.hasContraband = true;
        manifest.hasFalseEntries = true;
        manifest.contrabandType = CargoManifest.ContrabandType.Technology;
        manifest.contrabandItems = new string[] { "Stolen Military Hardware", "Classified Equipment", "Imperial Property" };
        manifest.isEasilyDetectable = false;
        manifest.clearanceCode = "";
        manifest.authorizedBy = "Salvage Inc. (Shell Company)";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 2;
        manifest.lastAppearanceDay = -1;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = false;
        manifest.validationKeywords = new string[] { };
        manifest.suspiciousKeywords = new string[] { "decoder", "military", "classified" };
        manifest.reputationImpactIfWrong = -35;
        manifest.creditRewardIfCorrect = 20;
        manifest.priority = CargoManifest.ManifestPriority.High;
    }

    // 22. TRIBBLES - BIOHAZARD
    static void CreateManifest22Data(CargoManifest manifest)
    {
        manifest.manifestName = "Pet Transport";
        manifest.manifestCode = "PT667";
        manifest.manifestDescription = "Cute and cuddly pets for crew morale improvement";
        manifest.declaredItems = new string[] { "Small Mammals (12 units)", "Pet Food", "Habitat Supplies", "Care Manual" };
        manifest.actualItems = new string[] { "Tribbles (12+++ rapidly multiplying)", "Tribble Food", "Containment Failure Imminent", "Emergency Protocols Missing" };
        manifest.totalCargoUnits = 12;
        manifest.cargoWeight = 24;
        manifest.factionRestriction = CargoManifest.FactionRestriction.FactionSpecific;
        manifest.allowedFactions = new string[] { "pirates", "civilian" };
        manifest.requiredShipCategories = new string[] { "Civilian", "Transport" };
        manifest.hasContraband = true;
        manifest.hasFalseEntries = true;
        manifest.contrabandType = CargoManifest.ContrabandType.Mixed;
        manifest.contrabandItems = new string[] { "Invasive Species", "Biohazard", "Ecological Threat" };
        manifest.isEasilyDetectable = false;
        manifest.clearanceCode = "";
        manifest.authorizedBy = "Exotic Pets Ltd.";
        manifest.requiredClearanceLevel = CargoManifest.ClearanceLevel.Standard;
        manifest.firstAppearanceDay = 7;
        manifest.lastAppearanceDay = 15;
        manifest.maxDailyAppearances = 1;
        manifest.requiresSpecialValidation = true;
        manifest.validationKeywords = new string[] { "pet", "mammal" };
        manifest.suspiciousKeywords = new string[] { "tribble", "multiply", "containment" };
        manifest.reputationImpactIfWrong = -100;
        manifest.creditRewardIfCorrect = 50;
        manifest.priority = CargoManifest.ManifestPriority.Critical;
    }
}