using UnityEngine;
using System.Collections.Generic;
using Starkiller.Core.Managers;

namespace Starkiller.Core.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject containing all data for a specific ending
    /// Includes narrative text, achievements, and visual/audio references
    /// </summary>
    [CreateAssetMenu(fileName = "EndingData", menuName = "Starkiller/Narrative/Ending Data")]
    public class EndingDataSO : ScriptableObject
    {
        [Header("Ending Identification")]
        [SerializeField] private EndingType endingType;
        [SerializeField] private string endingTitle;
        [SerializeField] private string endingSubtitle;
        
        [Header("Narrative Content")]
        [TextArea(5, 10)]
        [SerializeField] private string mainNarrativeText;
        [TextArea(3, 5)]
        [SerializeField] private string summaryText;
        
        [Header("Achievement Lists")]
        [SerializeField] private List<string> positiveAchievements = new List<string>();
        [SerializeField] private List<string> negativeAchievements = new List<string>();
        [SerializeField] private List<string> neutralAchievements = new List<string>();
        
        [Header("Visual Elements")]
        [SerializeField] private Sprite endingImage;
        [SerializeField] private Color endingThemeColor = Color.white;
        [SerializeField] private AnimationClip endingAnimation;
        
        [Header("Audio Elements")]
        [SerializeField] private AudioClip endingMusic;
        [SerializeField] private AudioClip endingNarration;
        [SerializeField] private float musicVolume = 0.7f;
        
        [Header("UI Configuration")]
        [SerializeField] private float displayDuration = 30f;
        [SerializeField] private bool allowSkip = true;
        [SerializeField] private float fadeInDuration = 2f;
        [SerializeField] private float fadeOutDuration = 2f;
        
        [Header("Ending Statistics")]
        [SerializeField] private bool showStatistics = true;
        [SerializeField] private List<string> statisticsToDisplay = new List<string>()
        {
            "Days Survived",
            "Ships Processed",
            "Family Status",
            "Imperial Loyalty",
            "Rebellion Sympathy",
            "Credits Earned",
            "Bribes Taken"
        };
        
        // Properties
        public EndingType EndingType => endingType;
        public string Title => endingTitle;
        public string Subtitle => endingSubtitle;
        public string MainNarrative => mainNarrativeText;
        public string Summary => summaryText;
        public List<string> PositiveAchievements => positiveAchievements;
        public List<string> NegativeAchievements => negativeAchievements;
        public List<string> NeutralAchievements => neutralAchievements;
        public Sprite EndingImage => endingImage;
        public Color ThemeColor => endingThemeColor;
        public AnimationClip EndingAnimation => endingAnimation;
        public AudioClip EndingMusic => endingMusic;
        public AudioClip EndingNarration => endingNarration;
        public float MusicVolume => musicVolume;
        public float DisplayDuration => displayDuration;
        public bool AllowSkip => allowSkip;
        public float FadeInDuration => fadeInDuration;
        public float FadeOutDuration => fadeOutDuration;
        public bool ShowStatistics => showStatistics;
        public List<string> StatisticsToDisplay => statisticsToDisplay;
        
        /// <summary>
        /// Get all achievements formatted for display
        /// </summary>
        public List<string> GetAllAchievements()
        {
            var allAchievements = new List<string>();
            
            foreach (var achievement in positiveAchievements)
            {
                allAchievements.Add($"+ {achievement}");
            }
            
            foreach (var achievement in negativeAchievements)
            {
                allAchievements.Add($"- {achievement}");
            }
            
            foreach (var achievement in neutralAchievements)
            {
                allAchievements.Add($"• {achievement}");
            }
            
            return allAchievements;
        }
        
        /// <summary>
        /// Validate the ending data
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(endingTitle))
            {
                Debug.LogWarning($"EndingDataSO: Missing title for {endingType}");
                return false;
            }
            
            if (string.IsNullOrEmpty(mainNarrativeText))
            {
                Debug.LogWarning($"EndingDataSO: Missing narrative text for {endingType}");
                return false;
            }
            
            return true;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor validation
        /// </summary>
        private void OnValidate()
        {
            // Ensure we have a title
            if (string.IsNullOrEmpty(endingTitle))
            {
                endingTitle = endingType.ToString();
            }
            
            // Set default theme colors based on ending type
            if (endingThemeColor == Color.white)
            {
                endingThemeColor = GetDefaultThemeColor();
            }
        }
        
        private Color GetDefaultThemeColor()
        {
            switch (endingType)
            {
                case EndingType.FreedomFighter:
                case EndingType.Martyr:
                case EndingType.Refugee:
                case EndingType.Underground:
                    return new Color(0.8f, 0.2f, 0.2f); // Rebel red
                    
                case EndingType.GoodSoldier:
                case EndingType.TrueBeliever:
                case EndingType.BridgeCommander:
                case EndingType.ImperialHero:
                    return new Color(0.2f, 0.4f, 0.8f); // Imperial blue
                    
                case EndingType.GrayMan:
                case EndingType.Compromised:
                    return new Color(0.5f, 0.5f, 0.5f); // Neutral gray
                    
                default:
                    return Color.white;
            }
        }
        #endif
    }
}