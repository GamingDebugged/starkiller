using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using StarkillerBaseCommand;
using Starkiller.Core.Managers;

namespace Starkiller.Core.ScriptableObjects.Editor
{
    /// <summary>
    /// Editor utility for creating PersonalDataLog entries
    /// </summary>
    public class PersonalDataLogEntryCreator : EditorWindow
    {
        private FeedType selectedFeedType = FeedType.ImperiumNews;
        private int startDay = 1;
        private int endDay = 7;
        
        [MenuItem("Starkiller/Personal Data Log/Create Sample Entries")]
        public static void ShowWindow()
        {
            GetWindow<PersonalDataLogEntryCreator>("DataLog Entry Creator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Personal Data Log Entry Creator", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            selectedFeedType = (FeedType)EditorGUILayout.EnumPopup("Feed Type", selectedFeedType);
            startDay = EditorGUILayout.IntField("Start Day", startDay);
            endDay = EditorGUILayout.IntField("End Day", endDay);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Sample Entries for Days " + startDay + "-" + endDay))
            {
                CreateSampleEntries();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Full Sample Library"))
            {
                CreateFullSampleLibrary();
            }
        }
        
        private void CreateSampleEntries()
        {
            string folderPath = "Assets/Resources/_ScriptableObjects/PersonalDataLog/";
            
            // Ensure directory exists
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }
            
            for (int day = startDay; day <= endDay; day++)
            {
                CreateEntriesForDay(day, folderPath);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private void CreateEntriesForDay(int day, string folderPath)
        {
            switch (selectedFeedType)
            {
                case FeedType.ImperiumNews:
                    CreateImperiumNewsEntries(day, folderPath);
                    break;
                case FeedType.FamilyChat:
                    CreateFamilyChatEntries(day, folderPath);
                    break;
                case FeedType.FrontierEzine:
                    CreateFrontierEzineEntries(day, folderPath);
                    break;
            }
        }
        
        private void CreateImperiumNewsEntries(int day, string folderPath)
        {
            // Security incident reports
            var entry1 = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + $"ImperiumNews_Day{day}_SecurityBreach.asset",
                "Security Breach at Docking Bay",
                "Imperial Security reports unauthorized access attempt at Docking Bay 7. Suspect detained for questioning. Citizens reminded to report suspicious activity.",
                FeedType.ImperiumNews,
                day,
                day
            );
            entry1.severity = Consequence.SeverityLevel.Moderate;
            entry1.linkedToIncident = true;
            entry1.linkedIncidentType = "security_breach";
            
            // Imperial propaganda
            var entry2 = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + $"ImperiumNews_Day{day}_Propaganda.asset",
                "Imperial Fleet Strengthens Border Security",
                "Grand Admiral announces deployment of additional patrol vessels to frontier sectors. 'The Empire's reach extends to every corner of the galaxy,' states official communique.",
                FeedType.ImperiumNews,
                day,
                day + 3
            );
            entry2.severity = Consequence.SeverityLevel.Minor;
            
            EditorUtility.SetDirty(entry1);
            EditorUtility.SetDirty(entry2);
        }
        
        private void CreateFamilyChatEntries(int day, string folderPath)
        {
            // Family message
            var entry1 = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + $"FamilyChat_Day{day}_DailyMessage.asset",
                "Message from Sarah",
                "Hi Dad! Hope you're staying safe at work. Mom says dinner was quiet without you again. Love you!",
                FeedType.FamilyChat,
                day,
                day
            );
            
            // Family pressure
            var entry2 = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + $"FamilyChat_Day{day}_MedicalBills.asset",
                "Medical Bills Due",
                "The hospital sent another notice about Tommy's treatment. They're threatening to stop his medication if we don't pay soon.",
                FeedType.FamilyChat,
                day,
                day + 7
            );
            entry2.requiresAction = true;
            entry2.actionText = "Pay Medical Bills";
            entry2.creditCost = 2000;
            entry2.actionId = "medical_bills_01";
            entry2.persistDuration = 5;
            
            EditorUtility.SetDirty(entry1);
            EditorUtility.SetDirty(entry2);
        }
        
        private void CreateFrontierEzineEntries(int day, string folderPath)
        {
            // Trade news
            var entry1 = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + $"FrontierEzine_Day{day}_TradeNews.asset",
                "Hyperspace Fuel Prices Surge",
                "Independent traders report 30% increase in fuel costs following new Imperial taxation. Merchant Guild files formal protest.",
                FeedType.FrontierEzine,
                day,
                day + 2
            );
            
            // Underground news
            var entry2 = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + $"FrontierEzine_Day{day}_Underground.asset",
                "Mysterious Delays at Imperial Checkpoints",
                "Multiple reports of unusual screening procedures causing hours-long delays. Some ships turned away without explanation.",
                FeedType.FrontierEzine,
                day,
                day + 1
            );
            entry2.alternateHeadlines.Add("Checkpoint Delays Continue");
            entry2.alternateHeadlines.Add("Traders Face Extended Screenings");
            
            EditorUtility.SetDirty(entry1);
            EditorUtility.SetDirty(entry2);
        }
        
        private T CreateEntry<T>(string path, string headline, string content, FeedType feedType, int minDay, int maxDay) where T : PersonalDataLogEntrySO
        {
            T entry = ScriptableObject.CreateInstance<T>();
            entry.headline = headline;
            entry.content = content;
            entry.feedType = feedType;
            entry.minDay = minDay;
            entry.maxDay = maxDay;
            
            AssetDatabase.CreateAsset(entry, path);
            return entry;
        }
        
        private void CreateFullSampleLibrary()
        {
            string basePath = "Assets/Resources/_ScriptableObjects/PersonalDataLog/";
            
            // Create folder structure
            CreateFolderIfNeeded(basePath);
            CreateFolderIfNeeded(basePath + "ImperiumNews/");
            CreateFolderIfNeeded(basePath + "FamilyChat/");
            CreateFolderIfNeeded(basePath + "FrontierEzine/");
            CreateFolderIfNeeded(basePath + "Collections/");
            
            // Create entries for each type
            CreateImperiumNewsSampleLibrary(basePath + "ImperiumNews/");
            CreateFamilyChatSampleLibrary(basePath + "FamilyChat/");
            CreateFrontierEzineSampleLibrary(basePath + "FrontierEzine/");
            
            // Create master collection
            CreateMasterCollection(basePath + "Collections/");
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Sample library created successfully!");
        }
        
        private void CreateFolderIfNeeded(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
        
        private void CreateImperiumNewsSampleLibrary(string folderPath)
        {
            // Day 1-2 entries
            var securityAlert = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + "SecurityAlert_Smugglers.asset",
                "Smuggling Ring Exposed",
                "Imperial Intelligence uncovers major smuggling operation. 12 arrests made. Citizens who provided tips will receive commendations.",
                FeedType.ImperiumNews, 1, 3
            );
            securityAlert.severity = Consequence.SeverityLevel.Severe;
            securityAlert.triggerEventId = "smuggler_caught";
            
            // Propaganda pieces
            var propaganda1 = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + "Propaganda_Unity.asset",
                "Unity Through Strength",
                "Emperor's birthday celebrations unite billions across the galaxy. Local governors report record attendance at loyalty ceremonies.",
                FeedType.ImperiumNews, 1, 30
            );
            propaganda1.appearanceChance = 0.3f;
            
            // Incident reports
            var incident1 = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + "Incident_TerroristStopped.asset",
                "Terrorist Plot Foiled at Checkpoint",
                "Alert security officer prevents catastrophic attack. Explosive materials discovered during routine inspection. Officer receives Medal of Vigilance.",
                FeedType.ImperiumNews, 2, 7
            );
            incident1.severity = Consequence.SeverityLevel.Critical;
            incident1.triggerEventId = "terrorist_stopped";
            incident1.imperialLoyaltyChange = 5;
        }
        
        private void CreateFamilyChatSampleLibrary(string folderPath)
        {
            // Daily family messages
            var dailyMessage1 = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + "Daily_MorningMessage.asset",
                "Good morning!",
                "Hope you slept well at the barracks. Kids are getting ready for school. Tommy's cough is a bit better today. Stay safe! -Emma",
                FeedType.FamilyChat, 1, 30
            );
            dailyMessage1.appearanceChance = 0.5f;
            
            // Financial pressures
            var rentDue = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + "Pressure_RentDue.asset",
                "Rent Payment Overdue",
                "Landlord came by again. He's threatening eviction if we don't pay by the end of the week. I don't know what to do.",
                FeedType.FamilyChat, 3, 10
            );
            rentDue.requiresAction = true;
            rentDue.actionText = "Send Rent Money";
            rentDue.creditCost = 3000;
            rentDue.persistDuration = 7;
            rentDue.familyPressureChange = -10;
            
            // Emotional content
            var missYou = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + "Emotional_MissYou.asset",
                "We miss you",
                "Sarah drew a picture of you at work today. She says you're 'keeping everyone safe.' The kids don't understand why you can't come home.",
                FeedType.FamilyChat, 2, 30
            );
            missYou.appearanceChance = 0.4f;
        }
        
        private void CreateFrontierEzineSampleLibrary(string folderPath)
        {
            // Trade news
            var tradeRoute = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + "Trade_NewRoute.asset",
                "New Trade Route Opens",
                "Independent merchants celebrate opening of Kessel bypass. Journey times reduced by 40%. Imperial tariffs still apply.",
                FeedType.FrontierEzine, 1, 5
            );
            
            // Economic impact
            var economicImpact = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + "Economic_SecurityImpact.asset",
                "Security Crackdown Hurts Small Business",
                "Local merchants report 50% drop in revenue due to checkpoint delays. 'We're being strangled by bureaucracy,' says Traders Union.",
                FeedType.FrontierEzine, 2, 10
            );
            economicImpact.triggerEventId = "increased_security";
            
            // Underground resistance
            var resistance = CreateEntry<PersonalDataLogEntrySO>(
                folderPath + "Underground_Hope.asset",
                "Hope Lives in the Outer Rim",
                "Despite Imperial propaganda, free systems continue to thrive beyond the Empire's reach. 'Freedom finds a way,' reports anonymous source.",
                FeedType.FrontierEzine, 5, 30
            );
            resistance.appearanceChance = 0.2f;
            resistance.prerequisiteEntryIds.Add("player_helped_rebels");
        }
        
        private void CreateMasterCollection(string folderPath)
        {
            var collection = ScriptableObject.CreateInstance<PersonalDataLogCollectionSO>();
            
            // Load all created entries
            string[] guids = AssetDatabase.FindAssets("t:PersonalDataLogEntrySO", new[] { "Assets/Resources/_ScriptableObjects/PersonalDataLog" });
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                PersonalDataLogEntrySO entry = AssetDatabase.LoadAssetAtPath<PersonalDataLogEntrySO>(path);
                
                if (entry != null)
                {
                    Debug.Log($"Loading entry: {entry.headline} - Type: {entry.feedType}");
                }
            }
            
            AssetDatabase.CreateAsset(collection, folderPath + "MasterDataLogCollection.asset");
            EditorUtility.SetDirty(collection);
            
            Debug.Log("Master collection created. Please manually assign entries to appropriate lists.");
        }
    }
}