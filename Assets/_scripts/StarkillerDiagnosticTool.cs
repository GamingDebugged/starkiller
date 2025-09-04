using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StarkillerBaseCommand;

/// <summary>
/// Diagnostic tool for Starkiller Base Command systems
/// Helps identify and fix common issues with the game systems
/// </summary>
public class StarkillerDiagnosticTool : MonoBehaviour
{
    [Header("System References")]
    [SerializeField] private ShipEncounterSystem shipSystem;
    [SerializeField] private StarkkillerContentManager contentManager;
    [SerializeField] private MasterShipGenerator masterShipGenerator;
    
    [Header("Diagnostic Settings")]
    [SerializeField] private bool runOnStart = true;
    [SerializeField] private bool verboseLogging = true;
    
    [Header("Diagnostic Results")]
    [SerializeField] private List<string> diagnosisResults = new List<string>();
    [SerializeField] private int foundIssues = 0;
    [SerializeField] private int fixedIssues = 0;
    
    // Start by running diagnostics if enabled
    private void Start()
    {
        // Find references if not set
        FindSystemReferences();
        
        if (runOnStart)
        {
            RunDiagnostics();
        }
    }
    
    /// <summary>
    /// Find all system references if not assigned
    /// </summary>
    private void FindSystemReferences()
    {
        if (shipSystem == null)
            shipSystem = FindFirstObjectByType<ShipEncounterSystem>();
            
        if (contentManager == null)
            contentManager = FindFirstObjectByType<StarkkillerContentManager>();
            
        if (masterShipGenerator == null)
            masterShipGenerator = FindFirstObjectByType<MasterShipGenerator>();
    }
    
    /// <summary>
    /// Run all diagnostic checks
    /// </summary>
    public void RunDiagnostics()
    {
        // Clear previous results
        diagnosisResults.Clear();
        foundIssues = 0;
        fixedIssues = 0;
        
        Log("Starting Starkiller diagnostic scan...");
        
        // Check various systems
        CheckShipSystem();
        CheckContentManager();
        CheckMasterShipGenerator();
        
        // Report results
        Log($"Diagnostic scan complete. Found {foundIssues} issues, fixed {fixedIssues}.");
    }
    
    /// <summary>
    /// Check the ShipEncounterSystem for issues
    /// </summary>
    private void CheckShipSystem()
    {
        if (shipSystem == null)
        {
            Log("ISSUE: ShipEncounterSystem not found in scene.");
            foundIssues++;
            return;
        }
        
        // Check if access codes are created - don't use Length or Count
        bool codesCreated = shipSystem.validAccessCodes != null;
        if (!codesCreated)
        {
            Log("ISSUE: ShipEncounterSystem has no valid access codes.");
            foundIssues++;
            
            // Try to fix
            shipSystem.RegenerateAccessCodes();
            Log("FIX: Regenerated access codes for ShipEncounterSystem.");
            fixedIssues++;
        }
        
        // Check if ship types are loaded - don't use Length or Count
        if (shipSystem.shipTypes == null)
        {
            Log("ISSUE: ShipEncounterSystem has no ship types.");
            foundIssues++;
        }
    }
    
    /// <summary>
    /// Check the ContentManager for issues
    /// </summary>
    private void CheckContentManager()
    {
        if (contentManager == null)
        {
            Log("ISSUE: StarkkillerContentManager not found in scene.");
            foundIssues++;
            return;
        }
        
        // Check if access codes are created
        if (contentManager.currentAccessCodes == null || contentManager.currentAccessCodes.Count == 0)
        {
            Log("ISSUE: ContentManager has no valid access codes.");
            foundIssues++;
            
            // Try to fix
            contentManager.RegenerateAccessCodes();
            Log("FIX: Regenerated access codes for ContentManager.");
            fixedIssues++;
        }
        
        // Check for missing content databases
        if (contentManager.shipTypes == null)
        {
            Log("ISSUE: ContentManager has no ship types.");
            foundIssues++;
        }
        
        if (contentManager.captainTypes == null)
        {
            Log("ISSUE: ContentManager has no captain types.");
            foundIssues++;
        }
        
        if (contentManager.scenarios == null)
        {
            Log("ISSUE: ContentManager has no scenarios.");
            foundIssues++;
        }
    }
    
    /// <summary>
    /// Check the MasterShipGenerator for issues
    /// </summary>
    private void CheckMasterShipGenerator()
    {
        if (masterShipGenerator == null)
        {
            Log("ISSUE: MasterShipGenerator not found in scene.");
            foundIssues++;
            return;
        }
        
        // Check for missing components via reflection
        var contentManagerField = masterShipGenerator.GetType().GetField("contentManager", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
        if (contentManagerField != null)
        {
            object contentManagerRef = contentManagerField.GetValue(masterShipGenerator);
            if (contentManagerRef == null)
            {
                Log("ISSUE: MasterShipGenerator missing ContentManager reference.");
                foundIssues++;
                
                // Try to fix
                if (contentManager != null)
                {
                    contentManagerField.SetValue(masterShipGenerator, contentManager);
                    Log("FIX: Connected ContentManager to MasterShipGenerator.");
                    fixedIssues++;
                }
            }
        }
    }
    
    /// <summary>
    /// Log a diagnostic message
    /// </summary>
    private void Log(string message)
    {
        // Add to results
        diagnosisResults.Add(message);
        
        // Log to console if verbose
        if (verboseLogging)
        {
            Debug.Log($"[StarkillerDiagnostic] {message}");
        }
    }
    
    /// <summary>
    /// Create a report of the diagnostic results
    /// </summary>
    public string GetDiagnosticReport()
    {
        string report = $"Starkiller Diagnostic Report\n";
        report += $"Found Issues: {foundIssues}, Fixed: {fixedIssues}\n\n";
        
        for (int i = 0; i < diagnosisResults.Count; i++)
        {
            report += $"{i+1}. {diagnosisResults[i]}\n";
        }
        
        return report;
    }
}