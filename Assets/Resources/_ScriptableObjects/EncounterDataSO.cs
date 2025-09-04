using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Video;
using StarkillerBaseCommand.Encounters;

namespace StarkillerBaseCommand.Encounters
{
    /// <summary>
    /// Standardized encounter data structure for Starkiller Base Command
    /// Supports multiple encounter types with flexible configuration
    /// </summary>
    [CreateAssetMenu(fileName = "New Encounter Data", menuName = "Starkiller/Encounters/Encounter Data", order = 1)]
    public class EncounterData : ScriptableObject
    {
        [System.Serializable]
        public enum EncounterType
        {
            Standard,
            Story,
            Special,
            Tutorial,
            Emergency
        }

        [System.Serializable]
        public enum EncounterOutcome
        {
            Approved,
            Denied,
            Bribed,
            Captured,
            Released
        }

        [Header("Encounter Identification")]
        public string encounterID = System.Guid.NewGuid().ToString();
        public string encounterName;
        public EncounterType encounterType = EncounterType.Standard;

        [Header("Ship Details")]
        public string shipType;
        public string shipName;
        public string origin;
        public string destination;
        public int crewSize;

        [Header("Captain Information")]
        public string captainName;
        public string captainRank;
        public string captainFaction;

        [Header("Encounter Credentials")]
        public string accessCode;
        public bool requiresSpecialClearance;

        [Header("Narrative Impact")]
        public int imperialLoyaltyImpact = 0;
        public int insurgentSympathyImpact = 0;

        [Header("Encounter Rules")]
        public bool shouldBeApproved = false;
        public string invalidReason;

        [Header("Consequence Parameters")]
        public int creditPenalty = 0;
        public int potentialCasualties = 0;

        [Header("Bribe Mechanics")]
        public bool offersBribe = false;
        public int bribeAmount = 0;

        [Header("Media Assets")]
        public Sprite shipImage;
        public Sprite captainPortrait;
        public VideoClip shipVideo;
        public VideoClip captainVideo;

        [Header("Variations and Randomization")]
        public List<string> possibleStories = new List<string>();
        public List<string> possibleManifests = new List<string>();

        [Header("Special Flags")]
        public bool isStoryShip = false;
        public string storyTag;
        public bool canBeCaptured = false;

        [Header("Generation Constraints")]
        public int minDayAppearance = 1;
        public int maxDayAppearance = int.MaxValue;

        // Randomization Method
        public string GetRandomStory()
        {
            return possibleStories.Count > 0 
                ? possibleStories[UnityEngine.Random.Range(0, possibleStories.Count)] 
                : "Standard mission request";
        }

        public string GetRandomManifest()
        {
            return possibleManifests.Count > 0 
                ? possibleManifests[UnityEngine.Random.Range(0, possibleManifests.Count)] 
                : "Standard cargo and supplies";
        }

        // Validation Method
        public bool IsValidForDay(int currentDay)
        {
            return currentDay >= minDayAppearance && currentDay <= maxDayAppearance;
        }

        // Deep copy method for encounter generation
        public EncounterData CreateVariation()
        {
            EncounterData variation = CreateInstance<EncounterData>();
            
            // Copy all serializable fields
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(this), variation);
            
            // Regenerate unique ID
            variation.encounterID = System.Guid.NewGuid().ToString();
            
            // Randomize some fields
            variation.accessCode = GenerateAccessCode();
            variation.captainName = GenerateCaptainName();
            
            return variation;
        }

        private string GenerateAccessCode()
        {
            string[] prefixes = { "SK-", "IM-", "FO-" };
            string prefix = prefixes[UnityEngine.Random.Range(0, prefixes.Length)];
            return $"{prefix}{UnityEngine.Random.Range(1000, 9999)}";
        }

        private string GenerateCaptainName()
        {
            string[] firstNames = { "Aria", "Dax", "Kai", "Lena", "Zara" };
            string[] lastNames = { "Starr", "Voss", "Kane", "Ryder", "Nash" };
            return $"{firstNames[UnityEngine.Random.Range(0, firstNames.Length)]} {lastNames[UnityEngine.Random.Range(0, lastNames.Length)]}";
        }

        // Utility method to convert to MasterShipEncounter if needed
        public MasterShipEncounter ToMasterShipEncounter()
        {
            MasterShipEncounter encounter = new MasterShipEncounter
            {
                // Basic ship information
                shipType = this.shipType,
                shipName = this.shipName,
                destination = this.destination,
                origin = this.origin,
                accessCode = this.accessCode,
                story = GetRandomStory(),
                manifest = GetRandomManifest(),
                crewSize = this.crewSize,

                // Validation data
                shouldApprove = this.shouldBeApproved,
                invalidReason = this.invalidReason,

                // Captain information
                captainName = this.captainName,
                captainRank = this.captainRank,
                captainFaction = this.captainFaction,

                // Special elements
                isStoryShip = this.isStoryShip,
                storyTag = this.storyTag,

                // Bribe information
                offersBribe = this.offersBribe,
                bribeAmount = this.bribeAmount,

                // Consequence information
                consequenceDescription = $"Encounter from {encounterName}",
                casualtiesIfWrong = this.potentialCasualties,
                creditPenalty = this.creditPenalty,

                // Media content
                shipImage = this.shipImage,
                captainPortrait = this.captainPortrait,
                shipVideo = this.shipVideo,
                captainVideo = this.captainVideo,

                // Additional flags
                canBeCaptured = this.canBeCaptured
            };

            return encounter;
        }
    }
}