using UnityEngine;
using System.Collections.Generic;

namespace StarkillerBaseCommand
{
    public class AdvancedNarrativeTracker : MonoBehaviour
    {
        private static AdvancedNarrativeTracker instance;
        public static AdvancedNarrativeTracker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<AdvancedNarrativeTracker>();
                    if (instance == null)
                    {
                        GameObject go = GameObject.Find("NarrativeTracker");
                        if (go == null)
                        {
                            go = new GameObject("NarrativeTracker");
                        }
                        instance = go.AddComponent<AdvancedNarrativeTracker>();
                    }
                }
                return instance;
            }
        }

        [System.Serializable]
        public class StoryEvent
        {
            public string eventId;
            public int dayOccurred;
            public string description;
            public bool wasCompleted;
        }

        private List<StoryEvent> storyEvents = new List<StoryEvent>();
        private Dictionary<string, int> narrativeFlags = new Dictionary<string, int>();
        private HashSet<string> unlockedStoryTags = new HashSet<string>();

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        public void RecordStoryEvent(string eventId, string description)
        {
            StoryEvent newEvent = new StoryEvent
            {
                eventId = eventId,
                dayOccurred = GetCurrentDay(),
                description = description,
                wasCompleted = true
            };
            storyEvents.Add(newEvent);
            Debug.Log($"Story Event Recorded: {eventId} - {description}");
        }

        public bool HasEventOccurred(string eventId)
        {
            return storyEvents.Exists(e => e.eventId == eventId);
        }

        public void SetNarrativeFlag(string flagName, int value)
        {
            narrativeFlags[flagName] = value;
        }

        public int GetNarrativeFlag(string flagName)
        {
            return narrativeFlags.ContainsKey(flagName) ? narrativeFlags[flagName] : 0;
        }

        // Method expected by MasterShipGenerator
        public bool IsStoryTagUnlocked(string storyTag)
        {
            return unlockedStoryTags.Contains(storyTag);
        }

        // Method expected by MasterShipGenerator
        public void UnlockStoryTag(string storyTag)
        {
            if (!string.IsNullOrEmpty(storyTag))
            {
                unlockedStoryTags.Add(storyTag);
                Debug.Log($"Story tag unlocked: {storyTag}");
            }
        }

        private int GetCurrentDay()
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            return gm != null ? gm.currentDay : 1;
        }

        public bool ShouldTriggerStoryEvent(string eventId, int currentDay)
        {
            // Add your story trigger logic here
            // For now, just return false to prevent story events
            return false;
        }
    }
}