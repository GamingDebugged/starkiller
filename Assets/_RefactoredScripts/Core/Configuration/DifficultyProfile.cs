using UnityEngine;
using System.Collections.Generic;
using System;

namespace Starkiller.Core.Configuration
{
    /// <summary>
    /// ScriptableObject that defines difficulty settings for each day of the game
    /// Allows designers to create different difficulty profiles (Easy, Normal, Hard, etc.)
    /// </summary>
    [CreateAssetMenu(fileName = "DifficultyProfile", menuName = "Starkiller/Difficulty Profile")]
    public class DifficultyProfile : ScriptableObject
    {
        [System.Serializable]
        public class DaySettings
        {
            [Header("Day Information")]
            [Tooltip("Day number this configuration applies to")]
            public int dayNumber = 1;
            
            [Header("Timer Settings")]
            [Tooltip("Time limit for shifts on this day (in seconds)")]
            public float shiftTimeLimit = 30f;
            
            [Tooltip("Multiplier for bonus time earned on this day")]
            [Range(0.1f, 3f)]
            public float bonusTimeMultiplier = 1f;
            
            [Tooltip("Maximum bonus time allowed on this day")]
            public float maxBonusTime = 60f;
            
            [Header("Warning System")]
            [Tooltip("Enable time warnings for this day")]
            public bool enableWarnings = true;
            
            [Tooltip("Times (in seconds) when warnings should be shown")]
            public float[] warningTimes = { 10f, 5f };
            
            [Header("Gameplay Settings")]
            [Tooltip("Ship quota for this day")]
            public int shipQuota = 10;
            
            [Tooltip("Credit multiplier for this day")]
            [Range(0.5f, 2f)]
            public float creditMultiplier = 1f;
            
            [Tooltip("Strike penalty multiplier for this day")]
            [Range(0.5f, 2f)]
            public float strikePenaltyMultiplier = 1f;
            
            [Header("Special Events")]
            [Tooltip("Special event or modifier for this day")]
            public string specialEvent = "";
            
            [Tooltip("Additional description for this day")]
            [TextArea(2, 4)]
            public string dayDescription = "";
        }
        
        [Header("Profile Information")]
        [Tooltip("Name of this difficulty profile")]
        public string profileName = "Standard";
        
        [Tooltip("Description of this difficulty profile")]
        [TextArea(2, 4)]
        public string profileDescription = "Standard difficulty progression for the game";
        
        [Header("Day Configurations")]
        [Tooltip("List of day-specific settings")]
        public List<DaySettings> dayConfigurations = new List<DaySettings>();
        
        [Header("Fallback Settings")]
        [Tooltip("Use last configuration for days beyond the list")]
        public bool useLastConfigForExtraDays = true;
        
        [Tooltip("Default settings for days not explicitly configured")]
        public DaySettings defaultSettings = new DaySettings
        {
            dayNumber = 1,
            shiftTimeLimit = 30f,
            bonusTimeMultiplier = 1f,
            maxBonusTime = 60f,
            enableWarnings = true,
            warningTimes = new float[] { 10f, 5f },
            shipQuota = 10,
            creditMultiplier = 1f,
            strikePenaltyMultiplier = 1f,
            specialEvent = "",
            dayDescription = "Default day settings"
        };
        
        /// <summary>
        /// Get settings for a specific day
        /// </summary>
        /// <param name="day">Day number (1-based)</param>
        /// <returns>DaySettings for the specified day</returns>
        public DaySettings GetDaySettings(int day)
        {
            // Look for exact day match
            var settings = dayConfigurations.Find(d => d.dayNumber == day);
            if (settings != null) 
            {
                return settings;
            }
            
            // If using last config for extra days and we have configurations
            if (useLastConfigForExtraDays && dayConfigurations.Count > 0)
            {
                // Find the highest day number that's less than or equal to requested day
                DaySettings lastValidConfig = null;
                int highestDay = 0;
                
                foreach (var config in dayConfigurations)
                {
                    if (config.dayNumber <= day && config.dayNumber > highestDay)
                    {
                        highestDay = config.dayNumber;
                        lastValidConfig = config;
                    }
                }
                
                if (lastValidConfig != null)
                {
                    // Create a copy with the requested day number
                    var copiedSettings = CreateDaySettingsCopy(lastValidConfig);
                    copiedSettings.dayNumber = day;
                    return copiedSettings;
                }
            }
            
            // Fallback to default settings
            var defaultCopy = CreateDaySettingsCopy(defaultSettings);
            defaultCopy.dayNumber = day;
            return defaultCopy;
        }
        
        /// <summary>
        /// Create a copy of DaySettings (since it's a class, not a struct)
        /// </summary>
        private DaySettings CreateDaySettingsCopy(DaySettings original)
        {
            return new DaySettings
            {
                dayNumber = original.dayNumber,
                shiftTimeLimit = original.shiftTimeLimit,
                bonusTimeMultiplier = original.bonusTimeMultiplier,
                maxBonusTime = original.maxBonusTime,
                enableWarnings = original.enableWarnings,
                warningTimes = (float[])original.warningTimes.Clone(),
                shipQuota = original.shipQuota,
                creditMultiplier = original.creditMultiplier,
                strikePenaltyMultiplier = original.strikePenaltyMultiplier,
                specialEvent = original.specialEvent,
                dayDescription = original.dayDescription
            };
        }
        
        /// <summary>
        /// Get the maximum day number configured in this profile
        /// </summary>
        public int GetMaxConfiguredDay()
        {
            if (dayConfigurations.Count == 0) return 0;
            
            int maxDay = 0;
            foreach (var config in dayConfigurations)
            {
                if (config.dayNumber > maxDay)
                    maxDay = config.dayNumber;
            }
            return maxDay;
        }
        
        /// <summary>
        /// Check if a specific day has explicit configuration
        /// </summary>
        public bool HasExplicitDayConfiguration(int day)
        {
            return dayConfigurations.Find(d => d.dayNumber == day) != null;
        }
        
        /// <summary>
        /// Get all configured day numbers
        /// </summary>
        public List<int> GetConfiguredDays()
        {
            var days = new List<int>();
            foreach (var config in dayConfigurations)
            {
                days.Add(config.dayNumber);
            }
            days.Sort();
            return days;
        }
        
        /// <summary>
        /// Validate the profile configuration
        /// </summary>
        public void ValidateProfile()
        {
            if (dayConfigurations.Count == 0)
            {
                Debug.LogWarning($"[DifficultyProfile] '{profileName}' has no day configurations!");
                return;
            }
            
            // Check for duplicate day numbers
            var dayNumbers = new HashSet<int>();
            foreach (var config in dayConfigurations)
            {
                if (dayNumbers.Contains(config.dayNumber))
                {
                    Debug.LogWarning($"[DifficultyProfile] '{profileName}' has duplicate configuration for day {config.dayNumber}");
                }
                dayNumbers.Add(config.dayNumber);
                
                // Validate individual settings
                if (config.shiftTimeLimit <= 0)
                {
                    Debug.LogWarning($"[DifficultyProfile] Day {config.dayNumber} has invalid shift time limit: {config.shiftTimeLimit}");
                }
                
                if (config.shipQuota <= 0)
                {
                    Debug.LogWarning($"[DifficultyProfile] Day {config.dayNumber} has invalid ship quota: {config.shipQuota}");
                }
            }
        }
        
        /// <summary>
        /// Create a default progression profile
        /// </summary>
        [ContextMenu("Create Default Progression")]
        public void CreateDefaultProgression()
        {
            dayConfigurations.Clear();
            
            // Create 10 days of progressive difficulty
            for (int i = 1; i <= 10; i++)
            {
                var settings = new DaySettings
                {
                    dayNumber = i,
                    shiftTimeLimit = Mathf.Max(30f - (i - 1) * 2f, 10f), // 30s to 10s
                    bonusTimeMultiplier = 1f,
                    maxBonusTime = 60f,
                    enableWarnings = true,
                    warningTimes = new float[] { 10f, 5f },
                    shipQuota = 10 + (i - 1) * 2, // 10 to 28 ships
                    creditMultiplier = 1f + (i - 1) * 0.1f, // 1.0x to 1.9x
                    strikePenaltyMultiplier = 1f + (i - 1) * 0.1f, // Increasing penalty
                    specialEvent = i == 5 ? "Midpoint Challenge" : (i == 10 ? "Final Challenge" : ""),
                    dayDescription = $"Day {i} - Progressive difficulty"
                };
                
                dayConfigurations.Add(settings);
            }
            
            Debug.Log($"[DifficultyProfile] Created default progression with {dayConfigurations.Count} days");
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor validation
        /// </summary>
        private void OnValidate()
        {
            ValidateProfile();
        }
        #endif
    }
}