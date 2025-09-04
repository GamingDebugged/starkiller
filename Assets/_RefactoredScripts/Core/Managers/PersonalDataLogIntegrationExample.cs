using UnityEngine;
using StarkillerBaseCommand;
using static Starkiller.Core.Managers.PersonalDataLogManager;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Example integration showing how to connect PersonalDataLog to other game systems
    /// This demonstrates where to place trigger calls in your existing code
    /// </summary>
    public class PersonalDataLogIntegrationExample : MonoBehaviour
    {
        /// <summary>
        /// Example: How to integrate with ConsequenceManager
        /// Add these calls to your ConsequenceManager methods
        /// </summary>
        public void ExampleConsequenceManagerIntegration()
        {
            // When recording an incident in ConsequenceManager, also trigger PersonalDataLog events
            
            // Example from when a smuggler is caught:
            /*
            public void RecordSmugglerCaughtIncident(string shipName, int creditsSeized)
            {
                // Existing ConsequenceManager code...
                var incident = new IncidentRecord
                {
                    title = "Smuggling Operation Disrupted",
                    description = $"Imperial forces intercepted {shipName} carrying contraband.",
                    // ... other fields
                };
                incidents.Add(incident);
                
                // NEW: Trigger PersonalDataLog entry
                PersonalDataLogEventTrigger.TriggerSmugglerCaught();
            }
            */
            
            // Example from when a terrorist is stopped:
            /*
            public void RecordTerroristStoppedIncident(string shipName, int liveSaved)
            {
                // Existing ConsequenceManager code...
                var incident = new IncidentRecord
                {
                    title = "Terrorist Plot Foiled",
                    description = $"Alert checkpoint officer prevents catastrophic attack.",
                    // ... other fields
                };
                incidents.Add(incident);
                
                // NEW: Trigger PersonalDataLog entry
                PersonalDataLogEventTrigger.TriggerTerroristStopped();
            }
            */
        }
        
        /// <summary>
        /// Example: How to integrate with ship approval system
        /// Add these calls to your ship processing logic
        /// </summary>
        public void ExampleShipProcessingIntegration()
        {
            // When player makes decision on a ship encounter:
            
            /*
            public void ProcessShipDecision(MasterShipEncounter encounter, bool approved, bool bribeTaken = false)
            {
                if (bribeTaken)
                {
                    // Player took a bribe
                    PersonalDataLogEventTrigger.TriggerBribeTaken(encounter.bribeAmount);
                }
                
                if (approved)
                {
                    if (encounter.shipType.Contains("Smuggler"))
                    {
                        // Player approved a smuggler ship
                        PersonalDataLogEventTrigger.TriggerSmugglerApproved();
                    }
                    else if (encounter.storyTag == "rebel")
                    {
                        // Player helped rebels
                        PersonalDataLogEventTrigger.TriggerHelpdRebels();
                    }
                }
                else
                {
                    // Player denied the ship
                    if (encounter.shipType.Contains("Terrorist"))
                    {
                        PersonalDataLogEventTrigger.TriggerTerroristStopped();
                    }
                }
            }
            */
        }
        
        /// <summary>
        /// Example: How to integrate with family pressure system
        /// Add these calls to your FamilyPressureManager
        /// </summary>
        public void ExampleFamilyPressureIntegration()
        {
            // When a family pressure event occurs:
            
            /*
            public void TriggerMedicalBillsPressure()
            {
                // Existing family pressure logic...
                
                // NEW: Add to PersonalDataLog
                PersonalDataLogEventTrigger.TriggerFamilyPressure(
                    "medical_bills",
                    2000,
                    "The hospital called again about Tommy's treatment costs. They're threatening to stop his medication."
                );
            }
            
            public void TriggerRentPressure()
            {
                PersonalDataLogEventTrigger.TriggerFamilyPressure(
                    "rent",
                    3000,
                    "Landlord came by today. He says if we don't pay by Friday, we're out."
                );
            }
            */
        }
        
        /// <summary>
        /// Example: How to integrate with story system
        /// Add these calls when story events occur
        /// </summary>
        public void ExampleStoryIntegration()
        {
            // When major story events happen:
            
            /*
            public void TriggerChapter2Start()
            {
                PersonalDataLogEventTrigger.TriggerStoryEvent("chapter_2_start");
            }
            
            public void TriggerInspectorArrival()
            {
                PersonalDataLogEventTrigger.TriggerStoryEvent("inspector_arrival");
            }
            
            public void TriggerRebelContactEvent()
            {
                PersonalDataLogEventTrigger.TriggerStoryEvent("rebel_contact");
            }
            */
        }
        
        /// <summary>
        /// Example: How to manually add entries from code
        /// Use this for dynamic content generation
        /// </summary>
        public void ExampleManualEntryCreation()
        {
            var logManager = ServiceLocator.Get<PersonalDataLogManager>();
            if (logManager != null)
            {
                // Add a news entry about recent player actions
                logManager.AddLogEntry(new DataLogEntry
                {
                    feedType = FeedType.ImperiumNews,
                    headline = "Checkpoint Performance Under Review",
                    content = "Following recent security incidents, Imperial Command announces performance review of all checkpoint personnel.",
                    severity = 2,
                    timestamp = System.DateTime.Now,
                    requiresAction = false
                });
                
                // Add a family chat entry
                logManager.AddLogEntry(new DataLogEntry
                {
                    feedType = FeedType.FamilyChat,
                    headline = "Sarah's School Play",
                    content = "Don't forget Sarah's school play is next week! She's been practicing her lines every day. She really hopes you can make it.",
                    timestamp = System.DateTime.Now,
                    requiresAction = false
                });
            }
        }
    }
}