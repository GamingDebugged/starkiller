using UnityEngine;

namespace StarkillerBaseCommand.Narrative
{
    /// <summary>
    /// Categories for different types of decisions in the narrative system
    /// </summary>
    public enum DecisionCategory
    {
        Moral,      // Decisions about right/wrong (e.g., accepting bribes, helping rebels)
        Tactical,   // Strategic gameplay decisions (e.g., using tractor beam, holding pattern)
        Financial,  // Money-related decisions (e.g., accepting bribes, choosing expenses)
        Political   // Faction alignment decisions (e.g., supporting Imperium vs Insurgents)
    }

    /// <summary>
    /// The pressure/difficulty level of a decision
    /// </summary>
    public enum DecisionPressure
    {
        Low,        // Routine decision with minimal consequences
        Medium,     // Standard decision with moderate impact
        High,       // Important decision with significant consequences
        Critical    // Game-changing decision that heavily impacts the narrative
    }

    /// <summary>
    /// Environmental context when a decision was made
    /// </summary>
    [System.Serializable]
    public class DecisionContext
    {
        [Header("Time Context")]
        public float remainingShiftTime;  // How much time was left in the shift
        public int currentDay;            // What day the decision was made
        public bool wasUnderTimePressure; // Was the timer running low?
        
        [Header("Resource Context")]
        public int creditsAtDecision;     // Player's credits when deciding
        public int strikesAtDecision;     // Current strikes when deciding
        public int shipsProcessedToday;   // Ships processed so far today
        
        [Header("Family Context")]
        public bool familyInCrisis;       // Was family having issues?
        public int familyHealthStatus;    // 0-100 health percentage
        public int familyMoraleStatus;    // 0-100 morale percentage
        
        /// <summary>
        /// Create a context snapshot from current game state
        /// </summary>
        public static DecisionContext CreateFromCurrentState()
        {
            var context = new DecisionContext();
            
            // Try to get GameManager for current state
            GameManager gm = GameObject.FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                context.currentDay = gm.currentDay;
                context.creditsAtDecision = gm.GetCredits();
                context.strikesAtDecision = gm.GetStrikes();
                context.shipsProcessedToday = gm.GetShipsProcessed();
                
                // Check if under time pressure (less than 60 seconds remaining)
                // Note: You'll need to make remainingTime accessible in GameManager
                // context.remainingShiftTime = gm.remainingTime;
                // context.wasUnderTimePressure = gm.remainingTime < 60f;
            }
            
            // Try to get family status
            ImperialFamilySystem familySystem = GameObject.FindFirstObjectByType<ImperialFamilySystem>();
            if (familySystem != null)
            {
                // Note: You'll need to add these methods to ImperialFamilySystem
                // context.familyHealthStatus = familySystem.GetAverageHealth();
                // context.familyMoraleStatus = familySystem.GetAverageMorale();
                // context.familyInCrisis = familySystem.IsInCrisis();
            }
            
            return context;
        }
    }
}