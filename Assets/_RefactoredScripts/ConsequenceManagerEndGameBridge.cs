using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using StarkillerBaseCommand;
using Starkiller.Core;
using Starkiller.Core.ScriptableObjects;

/// <summary>
/// Bridge extension for ConsequenceManager to provide EndGameManager integration
/// This provides achievement data based on consequence history without modifying existing ScriptableObjects
/// </summary>
public static class ConsequenceManagerEndGameBridge
{
    /// <summary>
    /// Convert ConsequenceManager incidents into achievement data for EndGameManager
    /// This bridges the existing Consequence system with the new EndGameManager
    /// </summary>
    public static List<AchievementData> GetEndGameAchievements(this ConsequenceManager consequenceManager)
    {
        var achievements = new List<AchievementData>();
        
        if (consequenceManager == null) return achievements;
        
        // Get all incidents from the consequence manager
        var allIncidents = consequenceManager.GetType()
            .GetField("incidents", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
            .GetValue(consequenceManager) as List<ConsequenceManager.IncidentRecord>;
            
        if (allIncidents == null) return achievements;
        
        // Convert major incidents to achievements
        foreach (var incident in allIncidents)
        {
            if (incident.severity >= Consequence.SeverityLevel.Moderate)
            {
                var achievement = ConvertIncidentToAchievement(incident);
                if (achievement != null)
                {
                    achievements.Add(achievement);
                }
            }
        }
        
        // Add performance-based achievements
        AddPerformanceAchievements(consequenceManager, achievements);
        
        // Add behavioral achievements
        AddBehavioralAchievements(consequenceManager, achievements);
        
        return achievements;
    }
    
    private static AchievementData ConvertIncidentToAchievement(ConsequenceManager.IncidentRecord incident)
    {
        string achievementText = GetEndGameMessage(incident);
        var category = GetAchievementCategory(incident);
        int priority = GetAchievementPriority(incident);
        
        return new AchievementData
        {
            Text = achievementText,
            Category = category,
            Priority = priority,
            IsKeyAchievement = incident.severity >= Consequence.SeverityLevel.Severe,
            ScenarioName = $"Incident_{incident.type}"
        };
    }
    
    private static string GetEndGameMessage(ConsequenceManager.IncidentRecord incident)
    {
        // First, try to find the original Consequence ScriptableObject to get the custom endGameMessage
        var consequenceManager = ServiceLocator.Get<ConsequenceManager>();
        if (consequenceManager != null)
        {
            // Try to find the consequence in the manager's lists
            Consequence sourceConsequence = FindConsequenceByTitle(consequenceManager, incident.title);
            
            if (sourceConsequence != null && !string.IsNullOrEmpty(sourceConsequence.endGameMessage))
            {
                // Use the authored endGameMessage if available
                return sourceConsequence.endGameMessage;
            }
        }
        
        // Fallback to auto-generated text if no custom message
        return GenerateAchievementText(incident);
    }
    
    private static Consequence FindConsequenceByTitle(ConsequenceManager manager, string title)
    {
        // Use reflection to access the private consequence lists
        var minorList = manager.GetType().GetField("minorConsequences", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
            .GetValue(manager) as List<Consequence>;
        var moderateList = manager.GetType().GetField("moderateConsequences", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
            .GetValue(manager) as List<Consequence>;
        var severeList = manager.GetType().GetField("severeConsequences", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
            .GetValue(manager) as List<Consequence>;
        var criticalList = manager.GetType().GetField("criticalConsequences", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
            .GetValue(manager) as List<Consequence>;
        
        // Search all lists for matching title
        if (minorList != null)
        {
            var match = minorList.FirstOrDefault(c => c != null && c.title == title);
            if (match != null) return match;
        }
        if (moderateList != null)
        {
            var match = moderateList.FirstOrDefault(c => c != null && c.title == title);
            if (match != null) return match;
        }
        if (severeList != null)
        {
            var match = severeList.FirstOrDefault(c => c != null && c.title == title);
            if (match != null) return match;
        }
        if (criticalList != null)
        {
            var match = criticalList.FirstOrDefault(c => c != null && c.title == title);
            if (match != null) return match;
        }
        
        return null;
    }
    
    private static string GenerateAchievementText(ConsequenceManager.IncidentRecord incident)
    {
        // Auto-generated fallback text
        switch (incident.type)
        {
            case Consequence.ConsequenceType.Security:
                switch (incident.severity)
                {
                    case Consequence.SeverityLevel.Critical:
                        return $"Caused critical security breach: {incident.title}";
                    case Consequence.SeverityLevel.Severe:
                        return $"Triggered major security incident: {incident.title}";
                    case Consequence.SeverityLevel.Moderate:
                        return $"Security protocol violation recorded";
                    default:
                        return $"Minor security issue documented";
                }
                
            case Consequence.ConsequenceType.Financial:
                return incident.creditPenalty > 50 ? 
                    $"Major financial losses incurred ({incident.creditPenalty} credits)" :
                    $"Financial penalties applied ({incident.creditPenalty} credits)";
                    
            case Consequence.ConsequenceType.Family:
                return $"Decision affected family status: {incident.title}";
                
            case Consequence.ConsequenceType.Reputation:
                return incident.severity >= Consequence.SeverityLevel.Severe ?
                    $"Reputation severely damaged by actions" :
                    $"Professional standing affected by decisions";
                    
            case Consequence.ConsequenceType.Story:
                return $"Pivotal story decision made: {incident.title}";
                
            default:
                return $"Significant incident: {incident.title}";
        }
    }
    
    private static AchievementCategory GetAchievementCategory(ConsequenceManager.IncidentRecord incident)
    {
        switch (incident.type)
        {
            case Consequence.ConsequenceType.Security:
                return incident.severity >= Consequence.SeverityLevel.Severe ? 
                    AchievementCategory.Negative : AchievementCategory.Duty;
                    
            case Consequence.ConsequenceType.Financial:
                return AchievementCategory.Corruption;
                
            case Consequence.ConsequenceType.Family:
                return AchievementCategory.Family;
                
            case Consequence.ConsequenceType.Reputation:
                return incident.severity >= Consequence.SeverityLevel.Severe ?
                    AchievementCategory.Negative : AchievementCategory.Neutral;
                    
            case Consequence.ConsequenceType.Story:
                return AchievementCategory.Neutral; // Story decisions are context-dependent
                
            default:
                return AchievementCategory.Neutral;
        }
    }
    
    private static int GetAchievementPriority(ConsequenceManager.IncidentRecord incident)
    {
        // Higher severity = higher priority for ending display
        switch (incident.severity)
        {
            case Consequence.SeverityLevel.Critical: return 100;
            case Consequence.SeverityLevel.Severe: return 80;
            case Consequence.SeverityLevel.Moderate: return 60;
            default: return 40;
        }
    }
    
    private static void AddPerformanceAchievements(ConsequenceManager consequenceManager, List<AchievementData> achievements)
    {
        // Count incidents by severity
        int criticalIncidents = consequenceManager.GetIncidentCount(Consequence.SeverityLevel.Critical);
        int severeIncidents = consequenceManager.GetIncidentCount(Consequence.SeverityLevel.Severe);
        int totalCasualties = consequenceManager.GetTotalCasualties();
        int totalCreditLoss = consequenceManager.GetTotalCreditLoss();
        
        // Performance-based achievements
        if (criticalIncidents == 0 && severeIncidents == 0)
        {
            achievements.Add(new AchievementData
            {
                Text = "Maintained exemplary security record with no major incidents",
                Category = AchievementCategory.Positive,
                Priority = 90,
                IsKeyAchievement = true,
                ScenarioName = "Perfect_Performance"
            });
        }
        else if (criticalIncidents >= 3)
        {
            achievements.Add(new AchievementData
            {
                Text = $"Caused multiple critical security failures ({criticalIncidents} incidents)",
                Category = AchievementCategory.Negative,
                Priority = 95,
                IsKeyAchievement = true,
                ScenarioName = "Critical_Failures"
            });
        }
        
        if (totalCasualties > 100)
        {
            achievements.Add(new AchievementData
            {
                Text = $"Decisions resulted in {totalCasualties} Imperial casualties",
                Category = AchievementCategory.Negative,
                Priority = 85,
                IsKeyAchievement = true,
                ScenarioName = "High_Casualties"
            });
        }
        else if (totalCasualties == 0)
        {
            achievements.Add(new AchievementData
            {
                Text = "Prevented all Imperial casualties through careful decision-making",
                Category = AchievementCategory.Positive,
                Priority = 85,
                IsKeyAchievement = true,
                ScenarioName = "Zero_Casualties"
            });
        }
        
        if (totalCreditLoss > 500)
        {
            achievements.Add(new AchievementData
            {
                Text = $"Financial penalties exceeded {totalCreditLoss} credits",
                Category = AchievementCategory.Corruption,
                Priority = 70,
                IsKeyAchievement = false,
                ScenarioName = "High_Financial_Impact"
            });
        }
    }
    
    private static void AddBehavioralAchievements(ConsequenceManager consequenceManager, List<AchievementData> achievements)
    {
        // Behavioral pattern achievements
        int securityIncidents = consequenceManager.GetIncidentCountByType(Consequence.ConsequenceType.Security);
        int familyIncidents = consequenceManager.GetIncidentCountByType(Consequence.ConsequenceType.Family);
        
        if (familyIncidents > 3)
        {
            achievements.Add(new AchievementData
            {
                Text = "Repeatedly prioritized duty over family welfare",
                Category = AchievementCategory.Imperial,
                Priority = 75,
                IsKeyAchievement = true,
                ScenarioName = "Duty_Over_Family"
            });
        }
        else if (familyIncidents == 0 && securityIncidents > 0)
        {
            achievements.Add(new AchievementData
            {
                Text = "Protected family while managing security challenges",
                Category = AchievementCategory.Family,
                Priority = 80,
                IsKeyAchievement = true,
                ScenarioName = "Family_Protection"
            });
        }
        
        // Check for inspection triggers
        bool hasActiveLongTermEffects = consequenceManager.HasActiveLongTermEffects();
        if (hasActiveLongTermEffects)
        {
            achievements.Add(new AchievementData
            {
                Text = "Actions triggered ongoing security monitoring",
                Category = AchievementCategory.Negative,
                Priority = 70,
                IsKeyAchievement = false,
                ScenarioName = "Ongoing_Scrutiny"
            });
        }
    }
}