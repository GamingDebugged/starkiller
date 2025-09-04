using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Starkiller.Core.Managers;

namespace Starkiller.Core.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject collection for PersonalDataLog entries
    /// Manages pools of entries for different feeds and days
    /// </summary>
    [CreateAssetMenu(fileName = "DataLog Entry Collection", menuName = "Starkiller/Personal Data Log/Entry Collection", order = 2)]
    public class PersonalDataLogCollectionSO : ScriptableObject
    {
        [Header("General Entry Pools")]
        [SerializeField] private List<PersonalDataLogEntrySO> imperiumNewsEntries = new List<PersonalDataLogEntrySO>();
        [SerializeField] private List<PersonalDataLogEntrySO> familyChatEntries = new List<PersonalDataLogEntrySO>();
        [SerializeField] private List<PersonalDataLogEntrySO> frontierEzineEntries = new List<PersonalDataLogEntrySO>();
        
        [Header("Day-Specific Collections")]
        [SerializeField] private List<DaySpecificEntries> daySpecificEntries = new List<DaySpecificEntries>();
        
        [Header("Event-Triggered Collections")]
        [SerializeField] private List<EventTriggeredEntries> eventTriggeredEntries = new List<EventTriggeredEntries>();
        
        // Public properties for editor access
        public List<PersonalDataLogEntrySO> ImperiumNewsEntries 
        { 
            get => imperiumNewsEntries; 
            set => imperiumNewsEntries = value; 
        }
        public List<PersonalDataLogEntrySO> FamilyChatEntries 
        { 
            get => familyChatEntries; 
            set => familyChatEntries = value; 
        }
        public List<PersonalDataLogEntrySO> FrontierEzineEntries 
        { 
            get => frontierEzineEntries; 
            set => frontierEzineEntries = value; 
        }
        public List<DaySpecificEntries> DaySpecificEntriesList 
        { 
            get => daySpecificEntries; 
            set => daySpecificEntries = value; 
        }
        public List<EventTriggeredEntries> EventTriggeredEntriesList 
        { 
            get => eventTriggeredEntries; 
            set => eventTriggeredEntries = value; 
        }
        
        [Header("Random Flavor Entry Pools")]
        [SerializeField] private List<PersonalDataLogEntrySO> randomImperiumNews = new List<PersonalDataLogEntrySO>();
        [SerializeField] private List<PersonalDataLogEntrySO> randomFamilyChat = new List<PersonalDataLogEntrySO>();
        [SerializeField] private List<PersonalDataLogEntrySO> randomFrontierNews = new List<PersonalDataLogEntrySO>();
        
        [System.Serializable]
        public class DaySpecificEntries
        {
            public string groupName;
            public int day;
            public List<PersonalDataLogEntrySO> entries = new List<PersonalDataLogEntrySO>();
            public bool guaranteedToAppear = true;
        }
        
        [System.Serializable]
        public class EventTriggeredEntries
        {
            public string eventId;
            public string eventDescription;
            public List<PersonalDataLogEntrySO> entries = new List<PersonalDataLogEntrySO>();
        }
        
        private HashSet<string> usedEntryIds = new HashSet<string>();
        
        /// <summary>
        /// Get all entries valid for a specific day
        /// </summary>
        public List<PersonalDataLogEntrySO> GetEntriesForDay(int currentDay, HashSet<string> previouslyShownIds = null)
        {
            List<PersonalDataLogEntrySO> validEntries = new List<PersonalDataLogEntrySO>();
            
            // Initialize used entries if needed
            if (usedEntryIds == null)
            {
                usedEntryIds = new HashSet<string>();
            }
            
            if (previouslyShownIds != null)
            {
                usedEntryIds = previouslyShownIds;
            }
            
            // Add day-specific entries (with null safety)
            if (daySpecificEntries != null)
            {
                var daySpecific = daySpecificEntries.FirstOrDefault(d => d != null && d.day == currentDay);
                if (daySpecific != null && daySpecific.entries != null)
                {
                    foreach (var entry in daySpecific.entries)
                    {
                        if (entry != null && IsEntryValid(entry, currentDay))
                        {
                            validEntries.Add(entry);
                        }
                    }
                }
            }
            
            // Add general entries valid for this day
            AddValidEntries(imperiumNewsEntries, validEntries, currentDay);
            AddValidEntries(familyChatEntries, validEntries, currentDay);
            AddValidEntries(frontierEzineEntries, validEntries, currentDay);
            
            return validEntries;
        }
        
        /// <summary>
        /// Get entries triggered by a specific event
        /// </summary>
        public List<PersonalDataLogEntrySO> GetEntriesForEvent(string eventId, int currentDay)
        {
            List<PersonalDataLogEntrySO> triggeredEntries = new List<PersonalDataLogEntrySO>();
            
            var eventGroup = eventTriggeredEntries.FirstOrDefault(e => e.eventId == eventId);
            if (eventGroup != null)
            {
                foreach (var entry in eventGroup.entries)
                {
                    if (IsEntryValid(entry, currentDay))
                    {
                        triggeredEntries.Add(entry);
                    }
                }
            }
            
            // Also check individual entries for trigger events
            CheckTriggeredEntries(imperiumNewsEntries, eventId, currentDay, triggeredEntries);
            CheckTriggeredEntries(familyChatEntries, eventId, currentDay, triggeredEntries);
            CheckTriggeredEntries(frontierEzineEntries, eventId, currentDay, triggeredEntries);
            
            return triggeredEntries;
        }
        
        /// <summary>
        /// Get random flavor entries for variety
        /// </summary>
        public List<PersonalDataLogEntrySO> GetRandomEntries(int count, FeedType? specificFeed = null)
        {
            List<PersonalDataLogEntrySO> randomEntries = new List<PersonalDataLogEntrySO>();
            List<PersonalDataLogEntrySO> pool = new List<PersonalDataLogEntrySO>();
            
            // Build the pool based on feed type
            if (specificFeed == null)
            {
                pool.AddRange(randomImperiumNews);
                pool.AddRange(randomFamilyChat);
                pool.AddRange(randomFrontierNews);
            }
            else
            {
                switch (specificFeed)
                {
                    case FeedType.ImperiumNews:
                        pool.AddRange(randomImperiumNews);
                        break;
                    case FeedType.FamilyChat:
                        pool.AddRange(randomFamilyChat);
                        break;
                    case FeedType.FrontierEzine:
                        pool.AddRange(randomFrontierNews);
                        break;
                }
            }
            
            // Filter out already used entries
            pool = pool.Where(e => !usedEntryIds.Contains(e.EntryId)).ToList();
            
            // Randomly select entries
            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                var entry = pool[Random.Range(0, pool.Count)];
                if (entry.ShouldAppear())
                {
                    randomEntries.Add(entry);
                    pool.Remove(entry);
                    usedEntryIds.Add(entry.EntryId);
                }
            }
            
            return randomEntries;
        }
        
        /// <summary>
        /// Mark entries as used to prevent repetition
        /// </summary>
        public void MarkEntriesAsUsed(List<PersonalDataLogEntrySO> entries)
        {
            foreach (var entry in entries)
            {
                usedEntryIds.Add(entry.EntryId);
            }
        }
        
        /// <summary>
        /// Reset used entries tracking
        /// </summary>
        public void ResetUsedEntries()
        {
            usedEntryIds.Clear();
        }
        
        private bool IsEntryValid(PersonalDataLogEntrySO entry, int currentDay)
        {
            // Null safety check
            if (entry == null)
            {
                Debug.LogWarning("[PersonalDataLogCollectionSO] Null entry encountered in IsEntryValid");
                return false;
            }
            
            // Initialize used entries if needed
            if (usedEntryIds == null)
            {
                usedEntryIds = new HashSet<string>();
            }
            
            // Check if already used
            if (usedEntryIds.Contains(entry.EntryId))
                return false;
                
            // Check day validity
            if (!entry.IsValidForDay(currentDay))
                return false;
                
            // Check prerequisites (with null safety)
            if (entry.prerequisiteEntryIds != null)
            {
                foreach (var prereq in entry.prerequisiteEntryIds)
                {
                    if (!usedEntryIds.Contains(prereq))
                        return false;
                }
            }
            
            // Check exclusions (with null safety)
            if (entry.exclusiveWithEntryIds != null)
            {
                foreach (var exclusion in entry.exclusiveWithEntryIds)
                {
                    if (usedEntryIds.Contains(exclusion))
                        return false;
                }
            }
            
            return true;
        }
        
        private void AddValidEntries(List<PersonalDataLogEntrySO> source, List<PersonalDataLogEntrySO> destination, int currentDay)
        {
            if (source == null || destination == null) return;
            
            foreach (var entry in source)
            {
                if (entry == null)
                {
                    Debug.LogWarning("[PersonalDataLogCollectionSO] Null entry found in collection - skipping");
                    continue;
                }
                
                if (IsEntryValid(entry, currentDay) && string.IsNullOrEmpty(entry.triggerEventId))
                {
                    destination.Add(entry);
                    Debug.Log($"[PersonalDataLogCollectionSO] Added valid entry: {entry.name} for day {currentDay}");
                }
            }
        }
        
        private void CheckTriggeredEntries(List<PersonalDataLogEntrySO> source, string eventId, int currentDay, List<PersonalDataLogEntrySO> destination)
        {
            if (source == null || destination == null) return;
            
            foreach (var entry in source)
            {
                if (entry != null && entry.triggerEventId == eventId && IsEntryValid(entry, currentDay))
                {
                    destination.Add(entry);
                }
            }
        }
        
#if UNITY_EDITOR
        [ContextMenu("Validate All Entries")]
        private void ValidateAllEntries()
        {
            int invalidCount = 0;
            List<PersonalDataLogEntrySO> allEntries = new List<PersonalDataLogEntrySO>();
            
            allEntries.AddRange(imperiumNewsEntries);
            allEntries.AddRange(familyChatEntries);
            allEntries.AddRange(frontierEzineEntries);
            allEntries.AddRange(randomImperiumNews);
            allEntries.AddRange(randomFamilyChat);
            allEntries.AddRange(randomFrontierNews);
            
            foreach (var dayGroup in daySpecificEntries)
            {
                allEntries.AddRange(dayGroup.entries);
            }
            
            foreach (var eventGroup in eventTriggeredEntries)
            {
                allEntries.AddRange(eventGroup.entries);
            }
            
            // Remove duplicates
            allEntries = allEntries.Distinct().ToList();
            
            foreach (var entry in allEntries)
            {
                if (entry == null)
                {
                    Debug.LogError($"Null entry found in {name}");
                    invalidCount++;
                    continue;
                }
                
                if (string.IsNullOrEmpty(entry.headline))
                {
                    Debug.LogWarning($"Entry {entry.name} has no headline");
                    invalidCount++;
                }
                
                if (string.IsNullOrEmpty(entry.content))
                {
                    Debug.LogWarning($"Entry {entry.name} has no content");
                    invalidCount++;
                }
            }
            
            Debug.Log($"Validation complete. Found {allEntries.Count} entries, {invalidCount} issues.");
        }
#endif
    }
}