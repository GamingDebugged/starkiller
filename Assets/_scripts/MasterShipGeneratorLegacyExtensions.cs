using UnityEngine;
using StarkillerBaseCommand;

/// <summary>
/// Additional extension methods for MasterShipGenerator to support legacy ProcessDecision calls
/// </summary>
public static class MasterShipGeneratorLegacyExtensions
{
    // Store the last encounter provided by GetNextEncounter for ProcessDecision compatibility
    private static MasterShipEncounter lastProvidedEncounter;
    
    /// <summary>
    /// Legacy ProcessDecision method for backward compatibility
    /// This method attempts to use the last encounter that was generated
    /// </summary>
    public static void ProcessDecision(this MasterShipGenerator generator, bool approved)
    {
        if (generator == null)
        {
            Debug.LogError("MasterShipGeneratorLegacyExtensions: Cannot process decision - generator is null!");
            return;
        }
        
        // Try to use the stored encounter if available
        if (lastProvidedEncounter != null)
        {
            Debug.Log($"MasterShipGeneratorLegacyExtensions: Processing decision ({approved}) with stored encounter");
            generator.ProcessDecisionWithEncounter(approved, lastProvidedEncounter);
            
            // Clear the stored encounter after use
            lastProvidedEncounter = null;
        }
        else
        {
            Debug.LogError("MasterShipGeneratorLegacyExtensions: No encounter available to process decision! " +
                          "This can happen if ProcessDecision is called without a prior GetNextEncounter call.");
            
            // As a fallback, try to get the next encounter and process it immediately
            MasterShipEncounter fallbackEncounter = generator.GetNextEncounter();
            if (fallbackEncounter != null)
            {
                Debug.LogWarning("MasterShipGeneratorLegacyExtensions: Using fallback encounter for decision processing");
                generator.ProcessDecisionWithEncounter(approved, fallbackEncounter);
            }
        }
    }
    
    /// <summary>
    /// Store the encounter when it's retrieved (should be called by bridge classes)
    /// </summary>
    public static void StoreLastEncounter(this MasterShipGenerator generator, MasterShipEncounter encounter)
    {
        lastProvidedEncounter = encounter;
        Debug.Log($"MasterShipGeneratorLegacyExtensions: Stored encounter for future ProcessDecision call");
    }
    
    /// <summary>
    /// Extension for GetNextEncounter that automatically stores the encounter
    /// </summary>
    public static MasterShipEncounter GetNextEncounterWithStorage(this MasterShipGenerator generator)
    {
        MasterShipEncounter encounter = generator.GetNextEncounter();
        if (encounter != null)
        {
            lastProvidedEncounter = encounter;
        }
        return encounter;
    }
}
