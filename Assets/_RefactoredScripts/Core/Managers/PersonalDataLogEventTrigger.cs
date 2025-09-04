using UnityEngine;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Helper class to demonstrate how other systems can trigger PersonalDataLog entries
    /// This shows integration points with ConsequenceManager and other game systems
    /// </summary>
    public static class PersonalDataLogEventTrigger
    {
        /// <summary>
        /// Call this when a player approves a smuggler ship
        /// </summary>
        public static void TriggerSmugglerApproved()
        {
            PersonalDataLogManager.TriggerEvent("smuggler_approved");
            Debug.Log("[PersonalDataLogEventTrigger] Triggered smuggler approved event");
        }
        
        /// <summary>
        /// Call this when a player catches a smuggler
        /// </summary>
        public static void TriggerSmugglerCaught()
        {
            PersonalDataLogManager.TriggerEvent("smuggler_caught");
            Debug.Log("[PersonalDataLogEventTrigger] Triggered smuggler caught event");
        }
        
        /// <summary>
        /// Call this when a player stops a terrorist
        /// </summary>
        public static void TriggerTerroristStopped()
        {
            PersonalDataLogManager.TriggerEvent("terrorist_stopped");
            Debug.Log("[PersonalDataLogEventTrigger] Triggered terrorist stopped event");
        }
        
        /// <summary>
        /// Call this when a player takes a bribe
        /// </summary>
        public static void TriggerBribeTaken(int amount)
        {
            PersonalDataLogManager.TriggerEvent("bribe_taken");
            
            // Also add family entry about mysterious extra money
            var logManager = ServiceLocator.Get<PersonalDataLogManager>();
            if (logManager != null)
            {
                logManager.AddPersistentFamilyAction(
                    "Unexpected Windfall",
                    $"Emma: 'Honey, there's an extra {amount} credits in our account. Did you get a bonus?'",
                    "Deflect Questions",
                    0, // No cost to deflect
                    "deflect_bribe_questions",
                    3 // Lasts 3 days
                );
            }
            
            Debug.Log($"[PersonalDataLogEventTrigger] Triggered bribe taken event for {amount} credits");
        }
        
        /// <summary>
        /// Call this when player helps rebel ships
        /// </summary>
        public static void TriggerHelpdRebels()
        {
            PersonalDataLogManager.TriggerEvent("helped_rebels");
            Debug.Log("[PersonalDataLogEventTrigger] Triggered helped rebels event");
        }
        
        /// <summary>
        /// Call this when security is increased due to incidents
        /// </summary>
        public static void TriggerIncreasedSecurity()
        {
            PersonalDataLogManager.TriggerEvent("increased_security");
            Debug.Log("[PersonalDataLogEventTrigger] Triggered increased security event");
        }
        
        /// <summary>
        /// Call this for family pressure events
        /// </summary>
        public static void TriggerFamilyPressure(string pressureType, int creditCost, string message)
        {
            var logManager = ServiceLocator.Get<PersonalDataLogManager>();
            if (logManager != null)
            {
                logManager.AddPersistentFamilyAction(
                    "Family Emergency",
                    message,
                    "Send Money",
                    creditCost,
                    $"family_pressure_{pressureType}",
                    5 // Lasts 5 days
                );
            }
            
            Debug.Log($"[PersonalDataLogEventTrigger] Triggered family pressure: {pressureType}");
        }
        
        /// <summary>
        /// Call this for story-specific events
        /// </summary>
        public static void TriggerStoryEvent(string storyEventId)
        {
            PersonalDataLogManager.TriggerEvent(storyEventId);
            Debug.Log($"[PersonalDataLogEventTrigger] Triggered story event: {storyEventId}");
        }
    }
}