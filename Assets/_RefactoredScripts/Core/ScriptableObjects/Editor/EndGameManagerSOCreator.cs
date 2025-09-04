using UnityEngine;
using UnityEditor;
using Starkiller.Core.ScriptableObjects;
using Starkiller.Core.Managers;

namespace Starkiller.Core.ScriptableObjects.Editor
{
    /// <summary>
    /// Editor utility to create and populate EndGameManager ScriptableObjects
    /// </summary>
    public class EndGameManagerSOCreator : EditorWindow
    {
        [MenuItem("Starkiller/Create EndGame ScriptableObjects")]
        public static void ShowWindow()
        {
            GetWindow<EndGameManagerSOCreator>("EndGame SO Creator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("EndGame ScriptableObject Creator", EditorStyles.boldLabel);
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Create All 10 Ending Data Objects"))
            {
                CreateAllEndingDataSOs();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("Create Sample Scenario Consequences"))
            {
                CreateSampleScenarioConsequences();
            }
            
            GUILayout.Space(10);
            GUILayout.Label("Individual Ending Creation:", EditorStyles.boldLabel);
            
            foreach (EndingType ending in System.Enum.GetValues(typeof(EndingType)))
            {
                if (GUILayout.Button($"Create {ending} Data"))
                {
                    CreateEndingDataSO(ending);
                }
            }
        }
        
        private void CreateAllEndingDataSOs()
        {
            string folderPath = "Assets/_RefactoredScripts/Core/ScriptableObjects/EndingData/";
            
            // Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets/_RefactoredScripts/Core/ScriptableObjects", "EndingData");
            }
            
            foreach (EndingType ending in System.Enum.GetValues(typeof(EndingType)))
            {
                CreateEndingDataSO(ending, folderPath);
            }
            
            AssetDatabase.SaveAssets();
            Debug.Log("Created all 10 ending data ScriptableObjects!");
        }
        
        private void CreateEndingDataSO(EndingType endingType, string folderPath = "Assets/_RefactoredScripts/Core/ScriptableObjects/")
        {
            EndingDataSO endingData = ScriptableObject.CreateInstance<EndingDataSO>();
            
            // Use reflection to set the private endingType field
            var endingTypeField = typeof(EndingDataSO).GetField("endingType", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (endingTypeField != null)
            {
                endingTypeField.SetValue(endingData, endingType);
            }
            
            // Set default values based on ending type
            SetEndingDefaults(endingData, endingType);
            
            string fileName = $"Ending_{endingType}.asset";
            string assetPath = folderPath + fileName;
            
            AssetDatabase.CreateAsset(endingData, assetPath);
            EditorUtility.SetDirty(endingData);
            
            Debug.Log($"Created ending data: {fileName}");
        }
        
        private void SetEndingDefaults(EndingDataSO endingData, EndingType endingType)
        {
            // Use reflection to set private fields with default content
            var titleField = typeof(EndingDataSO).GetField("endingTitle", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var subtitleField = typeof(EndingDataSO).GetField("endingSubtitle", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var narrativeField = typeof(EndingDataSO).GetField("mainNarrativeText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positiveField = typeof(EndingDataSO).GetField("positiveAchievements", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var (title, subtitle, narrative, achievements) = GetEndingContent(endingType);
            
            titleField?.SetValue(endingData, title);
            subtitleField?.SetValue(endingData, subtitle);
            narrativeField?.SetValue(endingData, narrative);
            positiveField?.SetValue(endingData, achievements);
        }
        
        private (string title, string subtitle, string narrative, System.Collections.Generic.List<string> achievements) GetEndingContent(EndingType endingType)
        {
            switch (endingType)
            {
                case EndingType.FreedomFighter:
                    return ("Freedom Fighter", "You escaped with your family to join the rebellion", 
                           "After 30 days of careful decisions and growing sympathy for the rebel cause, you've successfully coordinated your family's escape to join the rebellion. Your knowledge of Imperial security protocols proves invaluable to the resistance.",
                           new System.Collections.Generic.List<string> { "Successfully coordinated family escape", "Provided critical intelligence to resistance" });
                
                case EndingType.Martyr:
                    return ("Martyr", "You sacrificed yourself so your family could escape", 
                           "Your family escaped to freedom while you remained behind to face Imperial justice. Your sacrifice ensured their safety and inspired others to join the resistance.",
                           new System.Collections.Generic.List<string> { "Ensured family escape to safety", "Inspired others to join resistance" });
                
                case EndingType.Refugee:
                    return ("Refugee", "You fled to the frontier with your family", 
                           "Unable to fully commit to either side, you chose survival above all. Your family escaped to the outer rim territories where Imperial control is weak.",
                           new System.Collections.Generic.List<string> { "Successfully fled with family", "Reached outer rim territories safely" });
                
                case EndingType.Underground:
                    return ("Underground", "You remained at your post while secretly helping the resistance", 
                           "Maintaining your cover as a loyal officer, you became a valuable asset to the underground network, providing intelligence while keeping your family safe.",
                           new System.Collections.Generic.List<string> { "Became valuable intelligence asset", "Maintained operational security" });
                
                case EndingType.GrayMan:
                    return ("Gray Man", "You kept your head down and survived", 
                           "By avoiding political entanglements and focusing solely on duty, you and your family survived the dangerous period. Neither side considers you a threat.",
                           new System.Collections.Generic.List<string> { "Maintained neutrality successfully", "Kept family safe from all factions" });
                
                case EndingType.Compromised:
                    return ("Compromised", "Your activities attracted unwanted attention", 
                           "Your attempts to play both sides have been noticed. Under investigation but not yet arrested, you live in constant fear of discovery.",
                           new System.Collections.Generic.List<string> { "Survived initial scrutiny", "Maintained partial operational security" });
                
                case EndingType.GoodSoldier:
                    return ("Good Soldier", "Your loyalty was rewarded with promotion", 
                           "Your unwavering service to the Empire earned recognition and advancement. However, the cost of your loyalty weighs heavily on your family relationships.",
                           new System.Collections.Generic.List<string> { "Earned Imperial commendation", "Received promotion and benefits" });
                
                case EndingType.TrueBeliever:
                    return ("True Believer", "You became a zealous supporter of the Empire", 
                           "Your dedication to Imperial ideals earned you recognition, but at the cost of your family's happiness. You believe the sacrifice was necessary for order.",
                           new System.Collections.Generic.List<string> { "Achieved ideological purity", "Earned recognition from Imperial Command" });
                
                case EndingType.BridgeCommander:
                    return ("Bridge Commander", "Your excellence earned ultimate promotion", 
                           "Through superior performance and unwavering loyalty, you earned assignment to a Star Destroyer bridge crew. Your family is proud but distant.",
                           new System.Collections.Generic.List<string> { "Achieved Star Destroyer assignment", "Demonstrated exceptional performance" });
                
                case EndingType.ImperialHero:
                    return ("Imperial Hero", "You received the highest honors", 
                           "Your service earned you the Empire's highest commendation. You are held up as an example to others, though your family has been 'relocated for protection.'",
                           new System.Collections.Generic.List<string> { "Received highest Imperial honors", "Became example for other officers" });
                
                default:
                    return ("Unknown Ending", "An unexpected outcome", "This ending was not properly configured.", new System.Collections.Generic.List<string>());
            }
        }
        
        private void CreateSampleScenarioConsequences()
        {
            string folderPath = "Assets/_RefactoredScripts/Core/ScriptableObjects/ScenarioConsequences/";
            
            // Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets/_RefactoredScripts/Core/ScriptableObjects", "ScenarioConsequences");
            }
            
            // Create sample scenario consequences
            CreateScenarioConsequence("BountyHunterChase", "The Imperial Traitor", folderPath);
            CreateScenarioConsequence("TheDefector", "The Defection Attempt", folderPath);
            CreateScenarioConsequence("TheChildTransport", "Refugee Family Transport", folderPath);
            CreateScenarioConsequence("ThePurgeOrder", "Non-Human Detention Orders", folderPath);
            
            AssetDatabase.SaveAssets();
            Debug.Log("Created sample scenario consequence ScriptableObjects!");
        }
        
        private void CreateScenarioConsequence(string scenarioId, string scenarioName, string folderPath)
        {
            ScenarioConsequenceSO scenario = ScriptableObject.CreateInstance<ScenarioConsequenceSO>();
            
            // Use reflection to set private fields
            var idField = typeof(ScenarioConsequenceSO).GetField("scenarioId", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var nameField = typeof(ScenarioConsequenceSO).GetField("scenarioName", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var descField = typeof(ScenarioConsequenceSO).GetField("scenarioDescription", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var mappingsField = typeof(ScenarioConsequenceSO).GetField("consequenceMappings", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var keyField = typeof(ScenarioConsequenceSO).GetField("isKeyScenario", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            idField?.SetValue(scenario, scenarioId);
            nameField?.SetValue(scenario, scenarioName);
            descField?.SetValue(scenario, $"Scenario involving {scenarioName.ToLower()}");
            keyField?.SetValue(scenario, true);
            
            // Create sample consequence mappings
            var mappings = GetSampleConsequenceMappings(scenarioId);
            mappingsField?.SetValue(scenario, mappings);
            
            string fileName = $"Scenario_{scenarioId}.asset";
            string assetPath = folderPath + fileName;
            
            AssetDatabase.CreateAsset(scenario, assetPath);
            EditorUtility.SetDirty(scenario);
            
            Debug.Log($"Created scenario consequence: {fileName}");
        }
        
        private System.Collections.Generic.List<ConsequenceMapping> GetSampleConsequenceMappings(string scenarioId)
        {
            var mappings = new System.Collections.Generic.List<ConsequenceMapping>();
            
            switch (scenarioId)
            {
                case "BountyHunterChase":
                    mappings.Add(new ConsequenceMapping 
                    { 
                        decisionId = "help_rebel", 
                        decisionLabel = "Help Rebel Escape",
                        achievementText = "The Imperial Traitor escaped the Ravager",
                        category = AchievementCategory.Rebellion,
                        loyaltyImpact = -2,
                        rebellionImpact = 3
                    });
                    mappings.Add(new ConsequenceMapping 
                    { 
                        decisionId = "detain_rebel", 
                        decisionLabel = "Detain Fugitive",
                        achievementText = "Captured dangerous fugitive for Imperial Justice",
                        category = AchievementCategory.Imperial,
                        loyaltyImpact = 3,
                        rebellionImpact = -2
                    });
                    mappings.Add(new ConsequenceMapping 
                    { 
                        decisionId = "neutral_protocol", 
                        decisionLabel = "Follow Standard Protocol",
                        achievementText = "Maintained security protocols during pursuit incident",
                        category = AchievementCategory.Neutral,
                        loyaltyImpact = 0,
                        rebellionImpact = 0
                    });
                    break;
                    
                case "TheDefector":
                    mappings.Add(new ConsequenceMapping 
                    { 
                        decisionId = "help_defect", 
                        decisionLabel = "Aid Defection",
                        achievementText = "Aided Imperial defector in reaching rebel forces",
                        category = AchievementCategory.Rebellion,
                        loyaltyImpact = -3,
                        rebellionImpact = 4
                    });
                    mappings.Add(new ConsequenceMapping 
                    { 
                        decisionId = "prevent_defection", 
                        decisionLabel = "Prevent Treason",
                        achievementText = "Prevented treasonous defection attempt",
                        category = AchievementCategory.Imperial,
                        loyaltyImpact = 4,
                        rebellionImpact = -3
                    });
                    break;
            }
            
            return mappings;
        }
    }
}