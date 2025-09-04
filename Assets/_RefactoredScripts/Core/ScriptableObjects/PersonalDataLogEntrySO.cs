using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using StarkillerBaseCommand;
using Starkiller.Core.Managers;

namespace Starkiller.Core.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject for PersonalDataLog entries
    /// Allows designers to create narrative content without code
    /// </summary>
    [CreateAssetMenu(fileName = "New DataLog Entry", menuName = "Starkiller/Personal Data Log/Entry", order = 1)]
    public class PersonalDataLogEntrySO : ScriptableObject
    {
        [Header("Entry Identification")]
        [SerializeField] private string entryId = System.Guid.NewGuid().ToString();
        [SerializeField] private string entryName;
        
        [Header("Entry Type")]
        public FeedType feedType = FeedType.ImperiumNews;
        
        [Header("Basic Content")]
        public string headline;
        [TextArea(3, 10)]
        public string content;
        public VideoClip videoClip;
        [Tooltip("Custom thumbnail for video (optional - will use first frame if not set)")]
        public Sprite videoThumbnail;
        
        [Header("Appearance Conditions")]
        [Tooltip("Event ID that triggers this entry (leave empty for time-based)")]
        public string triggerEventId;
        [Tooltip("Minimum day this entry can appear")]
        public int minDay = 1;
        [Tooltip("Maximum day this entry can appear")]
        public int maxDay = 999;
        [Tooltip("Chance this entry will appear when conditions are met (0-1)")]
        [Range(0f, 1f)]
        public float appearanceChance = 1f;
        
        [Header("Imperial News Specific")]
        public Consequence.SeverityLevel severity = Consequence.SeverityLevel.Minor;
        [Tooltip("Links this entry to a specific incident type")]
        public bool linkedToIncident;
        public string linkedIncidentType;
        
        [Header("Family Chat Specific")]
        public bool requiresAction;
        public string actionText = "Help Family";
        public int creditCost = 0;
        public string actionId;
        [Tooltip("Days this action persists (0 = single day)")]
        public int persistDuration = 0;
        
        [Header("Content Variations")]
        [Tooltip("Alternative headlines for variety")]
        public List<string> alternateHeadlines = new List<string>();
        [Tooltip("Alternative content for variety")]
        public List<string> alternateContent = new List<string>();
        
        [Header("Prerequisites")]
        [Tooltip("Other entry IDs that must have appeared first")]
        public List<string> prerequisiteEntryIds = new List<string>();
        [Tooltip("Entry IDs that prevent this from appearing")]
        public List<string> exclusiveWithEntryIds = new List<string>();
        
        [Header("Consequences")]
        [Tooltip("Entry IDs to add when this appears")]
        public List<string> triggersEntryIds = new List<string>();
        [Tooltip("Family pressure to add when this appears")]
        public int familyPressureChange = 0;
        [Tooltip("Imperial loyalty impact")]
        public int imperialLoyaltyChange = 0;
        
        public string EntryId => entryId;
        public string EntryName => entryName;
        
        /// <summary>
        /// Get randomized headline
        /// </summary>
        public string GetRandomHeadline()
        {
            if (alternateHeadlines.Count == 0) return headline;
            
            List<string> allHeadlines = new List<string> { headline };
            allHeadlines.AddRange(alternateHeadlines);
            return allHeadlines[Random.Range(0, allHeadlines.Count)];
        }
        
        /// <summary>
        /// Get randomized content
        /// </summary>
        public string GetRandomContent()
        {
            if (alternateContent.Count == 0) return content;
            
            List<string> allContent = new List<string> { content };
            allContent.AddRange(alternateContent);
            return allContent[Random.Range(0, allContent.Count)];
        }
        
        /// <summary>
        /// Check if this entry can appear on the given day
        /// </summary>
        public bool IsValidForDay(int currentDay)
        {
            return currentDay >= minDay && currentDay <= maxDay;
        }
        
        /// <summary>
        /// Check if appearance chance is met
        /// </summary>
        public bool ShouldAppear()
        {
            return Random.value <= appearanceChance;
        }
        
        /// <summary>
        /// Convert to runtime DataLogEntry
        /// </summary>
        public DataLogEntry ToDataLogEntry()
        {
            var entry = new DataLogEntry
            {
                feedType = feedType,
                headline = GetRandomHeadline(),
                content = GetRandomContent(),
                videoClip = videoClip,
                videoThumbnail = videoThumbnail,
                requiresAction = requiresAction,
                severity = (int)severity,
                timestamp = System.DateTime.Now
            };
            
            if (requiresAction)
            {
                entry.familyActionData = new FamilyActionData
                {
                    actionText = actionText,
                    creditCost = creditCost,
                    actionId = string.IsNullOrEmpty(actionId) ? entryId : actionId
                };
            }
            
            return entry;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(entryId))
            {
                entryId = System.Guid.NewGuid().ToString();
            }
            
            if (string.IsNullOrEmpty(entryName) && !string.IsNullOrEmpty(headline))
            {
                entryName = headline;
            }
        }
#endif
    }
}