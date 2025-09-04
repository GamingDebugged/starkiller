using UnityEngine;

/// <summary>
/// Reference implementation for GameManager encounter flow patches
/// This is a utility class with static methods - not meant to be attached to GameObjects
/// </summary>
public static class GameManagerEncounterFlowPatch
{
    /// <summary>
    /// Get integration instructions for modifying GameManager.cs
    /// </summary>
    public static string GetIntegrationInstructions()
    {
        return @"
INTEGRATION INSTRUCTIONS FOR GameManager.cs:

1. Add RequestNextEncounter method:
   - Add the following public method to GameManager:
   
   public void RequestNextEncounter(string requestSource = ""Unknown"")
   {
       EncounterFlowManager flowManager = EncounterFlowManager.Instance;
       if (flowManager != null)
       {
           Debug.Log($""[GameManager] Delegating encounter request from '{requestSource}' to EncounterFlowManager"");
           flowManager.RequestNextEncounter(requestSource);
           return;
       }
       Debug.LogWarning(""[GameManager] EncounterFlowManager not found, using direct generation"");
       GenerateNewShipEncounter();
   }

2. Replace SpawnNextShip coroutine content:
   - Find the existing SpawnNextShip coroutine
   - Replace GenerateNewShipEncounter(); with RequestNextEncounter(""SpawnNextShip"");

3. Modify OnDecisionMade method:
   - At the end of OnDecisionMade, after 'UpdateUI();'
   - Add code to notify EncounterFlowManager:
   
   EncounterFlowManager flowManager = EncounterFlowManager.Instance;
   if (flowManager != null)
   {
       flowManager.OnDecisionMade();
   }

4. Modify HandlePostDecisionFlow in CredentialChecker:
   - Remove or comment out any calls to NextEncounter()
   - This prevents duplicate encounter requests

5. Add to Start() method:
   - Ensure EncounterFlowManager.Instance is created at startup

Example changes:

// In SpawnNextShip coroutine, replace:
GenerateNewShipEncounter();
UpdateUI();

// With:
RequestNextEncounter(""SpawnNextShip"");

// In OnDecisionMade, add after UpdateUI():
EncounterFlowManager flowManager = EncounterFlowManager.Instance;
if (flowManager != null)
{
    flowManager.OnDecisionMade();
}
";
    }

    /// <summary>
    /// Print integration instructions to console
    /// </summary>
    [UnityEditor.MenuItem("Starkkiller/Print GameManager Integration Instructions")]
    public static void PrintInstructions()
    {
        Debug.Log(GetIntegrationInstructions());
    }
}
