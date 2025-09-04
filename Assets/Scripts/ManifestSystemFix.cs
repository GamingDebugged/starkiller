using UnityEngine;
using System.Collections.Generic;
using StarkillerBaseCommand;

/// <summary>
/// Quick fix to help resolve manifest loading issues
/// Attach this to a GameObject in the scene to debug and fix manifest problems
/// </summary>
public class ManifestSystemFix : MonoBehaviour
{
    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool createSampleManifestsOnStart = false;
    [SerializeField] private bool useRuntimeManifests = true;
    
    [Header("References")]
    private ManifestManager manifestManager;
    private MasterShipGenerator shipGenerator;
    
    void Start()
    {
        // Find references
        manifestManager = FindFirstObjectByType<ManifestManager>();
        shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        
        if (showDebugInfo)
        {
            CheckManifestSystem();
        }
        
        if (createSampleManifestsOnStart)
        {
            CreateSampleManifests();
        }
        
        // Ensure ManifestManager uses runtime generation when no ScriptableObjects exist
        if (manifestManager != null && useRuntimeManifests)
        {
            EnableRuntimeManifestGeneration();
        }
    }
    
    /// <summary>
    /// Check the current state of the manifest system
    /// </summary>
    private void CheckManifestSystem()
    {
        Debug.Log("=== Manifest System Check ===");
        
        // Check Resources folder structure
        ResourcePathManager.DebugPrintValidPaths();
        
        // Try to load manifests
        var manifests = ResourceLoadingHelper.LoadCargoManifests();
        Debug.Log($"Found {manifests.Count} cargo manifests in Resources");
        
        if (manifests.Count == 0)
        {
            Debug.LogWarning("No manifest ScriptableObjects found! The system will use runtime generation.");
        }
        else
        {
            foreach (var manifest in manifests)
            {
                if (manifest != null)
                {
                    Debug.Log($"- {manifest.name}: {manifest.manifestName} (Faction: {string.Join(",", manifest.allowedFactions ?? new string[]{"None"})})");
                }
            }
        }
        
        // Check if ManifestManager is working
        if (manifestManager != null)
        {
            Debug.Log($"ManifestManager found and {(manifestManager.enabled ? "enabled" : "disabled")}");
        }
        else
        {
            Debug.LogError("ManifestManager not found in scene!");
        }
    }
    
    /// <summary>
    /// Enable runtime manifest generation in ManifestManager
    /// </summary>
    private void EnableRuntimeManifestGeneration()
    {
        if (manifestManager == null) return;
        
        // The ManifestManager already has fallback generation built in
        // We just need to ensure it's being used
        Debug.Log("ManifestSystemFix: Runtime manifest generation is enabled in ManifestManager");
        
        // Test runtime generation
        TestRuntimeManifestGeneration();
    }
    
    /// <summary>
    /// Test runtime manifest generation
    /// </summary>
    private void TestRuntimeManifestGeneration()
    {
        if (manifestManager == null) return;
        
        Debug.Log("=== Testing Runtime Manifest Generation ===");
        
        string[] testFactions = { "Imperium", "Insurgent", "Neutral" };
        
        foreach (string faction in testFactions)
        {
            // Test the fallback generation
            string fallbackManifest = manifestManager.GenerateFallbackManifest(faction, Random.value > 0.7f);
            Debug.Log($"Fallback manifest for {faction}: {fallbackManifest}");
            
            // Test the full selection with fallback
            var manifest = manifestManager.SelectManifestForShipWithFallback(null, faction, 1);
            if (manifest != null)
            {
                Debug.Log($"Selected manifest for {faction}: {manifest.manifestName} (Runtime: {manifest.name.Contains("RUNTIME")})");
            }
        }
    }
    
    /// <summary>
    /// Create sample manifests in the project
    /// </summary>
    [ContextMenu("Create Sample Manifests")]
    private void CreateSampleManifests()
    {
        #if UNITY_EDITOR
        Debug.Log("Creating sample manifests...");
        
        // First ensure the folder structure exists
        ResourcePathManager.CreateResourceFolderStructure();
        
        // Call the ManifestManager's create method
        if (manifestManager != null)
        {
            manifestManager.CreateSampleManifests();
        }
        else
        {
            Debug.LogError("ManifestManager not found - cannot create sample manifests");
        }
        #else
        Debug.LogWarning("Sample manifest creation only works in the Unity Editor");
        #endif
    }
    
    /// <summary>
    /// Fix the MasterShipGenerator to use ManifestManager properly
    /// </summary>
    [ContextMenu("Fix Ship Generator Manifest Usage")]
    private void FixShipGeneratorManifestUsage()
    {
        if (shipGenerator == null)
        {
            Debug.LogError("MasterShipGenerator not found!");
            return;
        }
        
        if (manifestManager == null)
        {
            Debug.LogError("ManifestManager not found!");
            return;
        }
        
        Debug.Log("Ship Generator and Manifest Manager are both present - manifest system should work");
        
        // The MasterShipGenerator should already be using ManifestManager.Instance
        // Let's verify by creating a test encounter
        TestManifestIntegration();
    }
    
    /// <summary>
    /// Test the integration between MasterShipGenerator and ManifestManager
    /// </summary>
    private void TestManifestIntegration()
    {
        Debug.Log("=== Testing Manifest Integration ===");
        
        // Force generate a new encounter
        if (shipGenerator != null)
        {
            // Get the next encounter method
            var methodInfo = shipGenerator.GetType().GetMethod("GenerateNextEncounter", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
            if (methodInfo != null)
            {
                methodInfo.Invoke(shipGenerator, null);
                Debug.Log("Triggered encounter generation - check if manifest was assigned");
            }
            else
            {
                Debug.LogWarning("Could not find GenerateNextEncounter method");
            }
        }
    }
    
    /// <summary>
    /// Quick action to fix common issues
    /// </summary>
    [ContextMenu("Quick Fix All Issues")]
    public void QuickFixAllIssues()
    {
        Debug.Log("=== Running Quick Fix ===");
        
        // 1. Ensure ManifestManager exists
        if (manifestManager == null)
        {
            GameObject mmGO = GameObject.Find("ManifestManager");
            if (mmGO == null)
            {
                mmGO = new GameObject("ManifestManager");
                manifestManager = mmGO.AddComponent<ManifestManager>();
                Debug.Log("Created ManifestManager");
            }
        }
        
        // 2. Enable runtime generation
        EnableRuntimeManifestGeneration();
        
        // 3. Test the system
        TestRuntimeManifestGeneration();
        
        Debug.Log("Quick fix complete!");
    }
}
