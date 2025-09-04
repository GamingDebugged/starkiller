/*
GameManager.cs Modifications Required
=====================================

This is a text reference file containing the exact code changes needed for GameManager.cs
Since GameManager.cs is over 2000 lines, these changes must be made manually.

STEP 1: Add RequestNextEncounter Method
----------------------------------------
Add this new public method anywhere in the GameManager class (recommended: after Start()):

public void RequestNextEncounter(string requestSource = "Unknown")
{
    // Use the EncounterFlowManager if available
    EncounterFlowManager flowManager = EncounterFlowManager.Instance;
    if (flowManager != null)
    {
        Debug.Log($"[GameManager] Delegating encounter request from '{requestSource}' to EncounterFlowManager");
        flowManager.RequestNextEncounter(requestSource);
        return;
    }
    
    // Fallback to original behavior if no flow manager
    Debug.LogWarning("[GameManager] EncounterFlowManager not found, using direct generation");
    GenerateNewShipEncounter();
}


STEP 2: Modify SpawnNextShip Coroutine
---------------------------------------
Find the existing SpawnNextShip() coroutine and replace the content with:

IEnumerator SpawnNextShip()
{
    yield return new WaitForSeconds(timeBetweenShips);
    
    if (gameActive && !isPaused && currentGameState == GameState.Gameplay)
    {
        // Use centralized request instead of direct generation
        RequestNextEncounter("SpawnNextShip Coroutine");
    }
}


STEP 3: Modify OnDecisionMade Method
------------------------------------
In OnDecisionMade, find this section near the end:

// Update UI before spawning next ship
UpdateUI();

// Notify GameStateController
ShipTimingController timingController = ShipTimingController.Instance;
...

ADD THIS CODE BEFORE the timing controller section:

// Notify the flow manager about the decision
EncounterFlowManager flowManager = EncounterFlowManager.Instance;
if (flowManager != null)
{
    flowManager.OnDecisionMade();
    return; // Let flow manager handle the next encounter
}


STEP 4: Modify OnDailyBriefingCompleted
---------------------------------------
Find this line:
GenerateNewShipEncounter();

Replace with:
RequestNextEncounter("Daily Briefing Completed");


STEP 5: Optional - Add to Start Method
--------------------------------------
Add this near the end of Start() to ensure the manager exists:

// Ensure EncounterFlowManager exists
if (EncounterFlowManager.Instance == null)
{
    GameObject flowManagerObj = new GameObject("EncounterFlowManager");
    flowManagerObj.AddComponent<EncounterFlowManager>();
    Debug.Log("Created EncounterFlowManager instance");
}


TESTING
-------
After making these changes:
1. The console should show [EncounterFlow] messages
2. Encounters should have at least 5 seconds between them
3. After decisions, there should be a 3-second cooldown
4. No more rapid encounter overwriting!
*/

// Empty class to prevent compilation errors
public class GameManager_Modifications_README { }
