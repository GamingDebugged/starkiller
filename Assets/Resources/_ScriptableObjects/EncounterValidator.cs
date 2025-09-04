using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarkillerBaseCommand.Encounters
{
    /// <summary>
    /// Provides comprehensive validation for encounter data
    /// Supports custom rules, detailed reporting, and debugging
    /// </summary>
    [CreateAssetMenu(fileName = "New Encounter Data", menuName = "Starkiller/Encounters/Encounter Data", order = 1)]
    public class EncounterDataSO : ScriptableObject
    {
        [Serializable]
        public class ValidationRule
        {
            public string ruleName;
            public Func<EncounterData, bool> validationCheck;
            public string errorMessage;
        }

        [Serializable]
        public class ValidationReport
        {
            public bool isValid;
            public List<string> errors = new List<string>();
            public List<string> warnings = new List<string>();
        }

        [Header("Preset Validation Rules")]
        public List<ValidationRule> standardRules = new List<ValidationRule>();

        [Header("Custom Validation Rules")]
        public List<ValidationRule> customRules = new List<ValidationRule>();

        // Default constructor to set up standard rules
        private void OnEnable()
        {
            if (standardRules.Count == 0)
            {
                InitializeStandardRules();
            }
        }

        private void InitializeStandardRules()
        {
            standardRules = new List<ValidationRule>
            {
                new ValidationRule
                {
                    ruleName = "Valid Access Code",
                    validationCheck = (encounter) => 
                        !string.IsNullOrEmpty(encounter.accessCode) && 
                        encounter.accessCode.Length >= 6 && 
                        (encounter.accessCode.StartsWith("SK-") || 
                         encounter.accessCode.StartsWith("IM-") || 
                         encounter.accessCode.StartsWith("FO-")),
                    errorMessage = "Invalid access code format"
                },
                new ValidationRule
                {
                    ruleName = "Ship Type Defined",
                    validationCheck = (encounter) => 
                        !string.IsNullOrEmpty(encounter.shipType),
                    errorMessage = "Ship type must be specified"
                },
                new ValidationRule
                {
                    ruleName = "Captain Information Complete",
                    validationCheck = (encounter) => 
                        !string.IsNullOrEmpty(encounter.captainName) && 
                        !string.IsNullOrEmpty(encounter.captainRank),
                    errorMessage = "Captain information is incomplete"
                },
                new ValidationRule
                {
                    ruleName = "Crew Size Valid",
                    validationCheck = (encounter) => 
                        encounter.crewSize > 0 && encounter.crewSize < 1000,
                    errorMessage = "Crew size is out of acceptable range"
                },
                new ValidationRule
                {
                    ruleName = "Origin and Destination Defined",
                    validationCheck = (encounter) => 
                        !string.IsNullOrEmpty(encounter.origin) && 
                        !string.IsNullOrEmpty(encounter.destination),
                    errorMessage = "Origin and destination must be specified"
                }
            };
        }

        /// <summary>
        /// Validate an encounter with all rules
        /// </summary>
        public ValidationReport ValidateEncounter(EncounterData encounterData)
        {
            ValidationReport report = new ValidationReport();

            // Validate standard rules
            foreach (var rule in standardRules)
            {
                ValidateRule(encounterData, rule, report);
            }

            // Validate custom rules
            foreach (var rule in customRules)
            {
                ValidateRule(encounterData, rule, report);
            }

            // Final validation status
            report.isValid = report.errors.Count == 0;

            return report;
        }

        private void ValidateRule(EncounterData encounterData, ValidationRule rule, ValidationReport report)
        {
            try
            {
                if (!rule.validationCheck(encounterData))
                {
                    report.errors.Add($"{rule.ruleName}: {rule.errorMessage}");
                }
            }
            catch (Exception ex)
            {
                report.errors.Add($"{rule.ruleName}: Validation error - {ex.Message}");
            }
        }

        /// <summary>
        /// Add a custom validation rule
        /// </summary>
        public void AddCustomRule(string ruleName, Func<EncounterData, bool> validationCheck, string errorMessage)
        {
            customRules.Add(new ValidationRule
            {
                ruleName = ruleName,
                validationCheck = validationCheck,
                errorMessage = errorMessage
            });
        }

        /// <summary>
        /// Remove a custom validation rule
        /// </summary>
        public void RemoveCustomRule(string ruleName)
        {
            customRules.RemoveAll(rule => rule.ruleName == ruleName);
        }

        /// <summary>
        /// Utility method to log validation report
        /// </summary>
        public void LogValidationReport(ValidationReport report)
        {
            if (report.isValid)
            {
                Debug.Log("Encounter Validation: PASSED");
            }
            else
            {
                Debug.LogWarning("Encounter Validation: FAILED");
                foreach (var error in report.errors)
                {
                    Debug.LogError(error);
                }
            }
        }
    }
}