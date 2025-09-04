using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Video;

namespace StarkillerBaseCommand.Encounters
{
    [CreateAssetMenu(fileName = "ShipNarrativeData", menuName = "Starkiller/Encounters/Ship Narrative Data", order = 10)]
    public class ShipNarrativeData : ScriptableObject
    {
        [System.Serializable]
        public class NarrativeEntry
        {
            [Header("Ship Identification")]
            public string shipType;
            public string shipSubtype; // Optional for more specific categorization

            [Header("Narrative Content")]
            [TextArea(3, 10)]
            public string[] possibleStories;
            
            [TextArea(3, 10)]
            public string[] videoDescriptions;

            [Header("Media Assets")]
            public VideoClip[] associatedVideos;
            public Sprite[] associatedSprites;

            [Header("Encounter Variations")]
            public bool isStoryShip = false;
            public string storyTag;
        }

        [Header("Narrative Entries")]
        public List<NarrativeEntry> narrativeEntries = new List<NarrativeEntry>();

        /// <summary>
        /// Get a narrative entry for a specific ship type
        /// </summary>
        public NarrativeEntry GetNarrativeForShipType(string shipType, string subtype = null)
        {
            // First try to match both ship type and subtype
            if (!string.IsNullOrEmpty(subtype))
            {
                var specificMatch = narrativeEntries.Find(entry => 
                    entry.shipType.Equals(shipType, System.StringComparison.OrdinalIgnoreCase) && 
                    entry.shipSubtype.Equals(subtype, System.StringComparison.OrdinalIgnoreCase));
                
                if (specificMatch != null)
                    return specificMatch;
            }
            
            // Fall back to matching just ship type
            return narrativeEntries.Find(entry => 
                entry.shipType.Equals(shipType, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get a random narrative entry
        /// </summary>
        public NarrativeEntry GetRandomNarrativeEntry()
        {
            if (narrativeEntries.Count == 0)
                return null;
            
            return narrativeEntries[Random.Range(0, narrativeEntries.Count)];
        }
    }
}