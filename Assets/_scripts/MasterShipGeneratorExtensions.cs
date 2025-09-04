using UnityEngine;

/// <summary>
/// Extension methods for MasterShipGenerator to prevent rapid encounter overwrites
/// </summary>
public static class MasterShipGeneratorExtensions
{
    private static float lastEncounterTime = 0f;
    private static float minimumEncounterInterval = 2.0f; // Minimum 2 seconds between encounters
    
    /// <summary>
    /// Checks if enough time has passed since the last encounter
    /// </summary>
    public static bool CanDisplayNewEncounter(this MasterShipGenerator generator)
    {
        float timeSinceLastEncounter = Time.time - lastEncounterTime;
        return timeSinceLastEncounter >= minimumEncounterInterval;
    }
    
    /// <summary>
    /// Records when an encounter was displayed
    /// </summary>
    public static void RecordEncounterDisplayed(this MasterShipGenerator generator)
    {
        lastEncounterTime = Time.time;
    }
    
    /// <summary>
    /// Gets the time remaining until a new encounter can be displayed
    /// </summary>
    public static float GetEncounterCooldownRemaining(this MasterShipGenerator generator)
    {
        float timeSinceLastEncounter = Time.time - lastEncounterTime;
        float remaining = minimumEncounterInterval - timeSinceLastEncounter;
        return Mathf.Max(0f, remaining);
    }
    
    /// <summary>
    /// Resets the encounter timing (use when starting a new day)
    /// </summary>
    public static void ResetEncounterTiming(this MasterShipGenerator generator)
    {
        lastEncounterTime = 0f;
        Debug.Log("MasterShipGeneratorExtensions: Encounter timing reset");
    }
}