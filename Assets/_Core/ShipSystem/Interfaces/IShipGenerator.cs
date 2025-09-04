namespace StarkillerBaseCommand
{
    /// <summary>
    /// Interface for ship generators in the Starkiller Base Command game
    /// </summary>
    public interface IShipGenerator
    {
        /// <summary>
        /// Get the next ship encounter
        /// </summary>
        MasterShipEncounter GetNextEncounter();
        
        /// <summary>
        /// Generate encounters for a specific day
        /// </summary>
        void GenerateEncountersForDay(int day);
        
        /// <summary>
        /// Generate a random ship encounter
        /// </summary>
        MasterShipEncounter GenerateRandomEncounter(bool forceValid = false);
        
        /// <summary>
        /// Process a decision on an encounter
        /// </summary>
        void ProcessDecisionWithEncounter(bool approved, MasterShipEncounter encounter);
        
        /// <summary>
        /// Start a new day
        /// </summary>
        void StartNewDay(int day);
    }
}
