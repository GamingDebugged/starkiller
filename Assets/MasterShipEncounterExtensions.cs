using UnityEngine;
using System.Collections.Generic;
using StarkillerBaseCommand;

namespace StarkillerBaseCommand.Extensions
{
    /// <summary>
    /// Adapter class to help transition from old ship encounter classes to the MasterShipEncounter
    /// This will allow existing code to continue working during the transition
    /// </summary>
    public static class MasterShipEncounterExtensions
    {
        /// <summary>
        /// Convert from legacy ShipEncounter to MasterShipEncounter
        /// </summary>
        public static MasterShipEncounter ToMasterEncounter(this ShipEncounter source)
        {
            return MasterShipEncounter.FromLegacyEncounter(source);
        }
        
        /// <summary>
        /// Create a display method that accepts old ShipEncounter types and converts them
        /// </summary>
        public static void DisplayEncounter(this CredentialChecker checker, ShipEncounter oldEncounter)
        {
            // Convert to new format first
            MasterShipEncounter masterEncounter = oldEncounter.ToMasterEncounter();
            
            // Then display the master encounter
            checker.DisplayEncounter(masterEncounter);
        }
        
        /// <summary>
        /// Makes CredentialChecker backward compatible with VideoEnhancedShipEncounter
        /// </summary>
        public static void DisplayEncounter(this CredentialChecker checker, VideoEnhancedShipEncounter oldVideoEncounter)
        {
            // Convert to new format first
            MasterShipEncounter masterEncounter = oldVideoEncounter.ToMasterEncounter();
            
            // Then display the master encounter
            checker.DisplayEncounter(masterEncounter);
        }
        
        /// <summary>
        /// Makes CredentialChecker backward compatible with EnhancedShipEncounter
        /// </summary>
        public static void DisplayEncounter(this CredentialChecker checker, EnhancedShipEncounter oldEnhancedEncounter)
        {
            // Convert to new format first
            MasterShipEncounter masterEncounter = oldEnhancedEncounter.ToMasterEncounter();
            
            // Then display the master encounter
            checker.DisplayEncounter(masterEncounter);
        }
    }
}