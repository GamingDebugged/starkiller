using UnityEngine;

namespace Starkiller.Core.Helpers
{
    /// <summary>
    /// Example configurations for different scenario types
    /// Copy these settings to your ScenarioMediaHelper component
    /// </summary>
    public static class ScenarioUIConfigExamples
    {
        /// <summary>
        /// Get example configurations for all scenario types
        /// </summary>
        public static ScenarioUIConfig[] GetExampleConfigs()
        {
            return new ScenarioUIConfig[]
            {
                // Inspection Scenario
                new ScenarioUIConfig
                {
                    scenarioType = ShipScenario.ScenarioType.Inspection,
                    titleText = "IMPERIAL INSPECTION",
                    defaultMessage = "⚠️ ATTENTION OFFICER: A surprise inspection of Imperium Command is underway.\n\nAll operations temporarily suspended.",
                    statusText = "IN PROGRESS",
                    footerText = "Press CONTINUE when ready to proceed...",
                    backgroundColor = new Color(0.15f, 0.15f, 0.3f, 0.95f), // Dark blue
                    showVideo = true,
                    playAudio = true,
                    displayDuration = 8f,
                    waitForUserInput = true // Wait for user to click continue
                },
                
                // Problem Scenario
                new ScenarioUIConfig
                {
                    scenarioType = ShipScenario.ScenarioType.Problem,
                    titleText = "SECURITY ALERT",
                    defaultMessage = "🚨 ATTENTION: Security protocols have been activated.\n\nThreat assessment in progress.",
                    statusText = "EVALUATING",
                    footerText = "Await further instructions...",
                    backgroundColor = new Color(0.3f, 0.15f, 0.15f, 0.95f), // Dark red
                    showVideo = true,
                    playAudio = true,
                    displayDuration = 6f
                },
                
                // Invalid Scenario
                new ScenarioUIConfig
                {
                    scenarioType = ShipScenario.ScenarioType.Invalid,
                    titleText = "SECURITY VIOLATION",
                    defaultMessage = "⛔ INVALID CREDENTIALS DETECTED\n\nSecurity breach protocols initiated.",
                    statusText = "DENIED",
                    footerText = "Incident being logged...",
                    backgroundColor = new Color(0.25f, 0.1f, 0.1f, 0.95f), // Deep red
                    showVideo = false,
                    playAudio = true,
                    displayDuration = 4f
                },
                
                // Story Event
                new ScenarioUIConfig
                {
                    scenarioType = ShipScenario.ScenarioType.StoryEvent,
                    titleText = "PRIORITY TRANSMISSION",
                    defaultMessage = "📡 INCOMING PRIORITY MESSAGE\n\nImperial Command requires your attention.",
                    statusText = "RECEIVING",
                    footerText = "Press CONTINUE when ready to proceed...",
                    backgroundColor = new Color(0.1f, 0.2f, 0.1f, 0.95f), // Dark green
                    showVideo = true,
                    playAudio = false,
                    displayDuration = 10f,
                    waitForUserInput = true // Wait for user input for story events
                },
                
                // Standard Scenario
                new ScenarioUIConfig
                {
                    scenarioType = ShipScenario.ScenarioType.Standard,
                    titleText = "ROUTINE OPERATION",
                    defaultMessage = "ℹ️ Standard docking procedure initiated.\n\nProcessing clearance request.",
                    statusText = "PROCESSING",
                    footerText = "Please wait...",
                    backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f), // Neutral grey
                    showVideo = false,
                    playAudio = false,
                    displayDuration = 3f
                }
            };
        }
        
        /// <summary>
        /// Get configuration for a specific scenario type
        /// </summary>
        public static ScenarioUIConfig GetConfigForType(ShipScenario.ScenarioType type)
        {
            var configs = GetExampleConfigs();
            foreach (var config in configs)
            {
                if (config.scenarioType == type)
                    return config;
            }
            
            // Return a default config if not found
            return new ScenarioUIConfig
            {
                scenarioType = type,
                titleText = type.ToString().ToUpper(),
                defaultMessage = "Processing scenario...",
                statusText = "IN PROGRESS",
                footerText = "Please wait...",
                backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f),
                showVideo = true,
                playAudio = true,
                displayDuration = 6f
            };
        }
    }
}