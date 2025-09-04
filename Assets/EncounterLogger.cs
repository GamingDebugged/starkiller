using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.Video;

namespace StarkillerBaseCommand.Encounters
{
    /// <summary>
    /// Centralized logging system for encounter-related events and performance tracking
    /// </summary>
    [CreateAssetMenu(fileName = "EncounterLogger", menuName = "Starkiller/Encounters/Encounter Logger", order = 3)]
    public class EncounterLogger : ScriptableObject
    {
        [Serializable]
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Critical
        }

        [Serializable]
        public class EncounterLogEntry
        {
            public string encounterID;
            public DateTime timestamp;
            public LogLevel logLevel;
            public string message;
            public string shipType;
            public string accessCode;
            public float processingTime;
            public string additionalData;
        }

        [Header("Logger Configuration")]
        public bool enableFileLogging = true;
        public string logFilePath = "Logs/EncounterLogs/";
        public LogLevel minimumLogLevel = LogLevel.Info;

        [Header("Performance Tracking")]
        public bool trackPerformance = true;
        public int maxPerformanceEntries = 100;

        // In-memory log storage
        [SerializeField] private List<EncounterLogEntry> logEntries = new List<EncounterLogEntry>();
        [SerializeField] private List<EncounterLogEntry> performanceEntries = new List<EncounterLogEntry>();

        // Delegates for external logging
        public delegate void LogEventHandler(EncounterLogEntry logEntry);
        public event LogEventHandler OnLogEvent;

        private void OnEnable()
        {
            // Ensure log directory exists
            if (enableFileLogging)
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, logFilePath));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create log directory: {ex.Message}");
                    enableFileLogging = false;
                }
            }
        }

        /// <summary>
        /// Log an encounter-related event
        /// </summary>
        public void LogEncounter(EncounterData encounterData, LogLevel logLevel, string message, float processingTime = 0f)
        {
            // Create log entry
            var logEntry = new EncounterLogEntry
            {
                encounterID = encounterData.encounterID,
                timestamp = DateTime.Now,
                logLevel = logLevel,
                message = message,
                shipType = encounterData.shipType,
                accessCode = encounterData.accessCode,
                processingTime = processingTime,
                additionalData = JsonUtility.ToJson(encounterData)
            };

            // Add to in-memory log
            if (logLevel >= minimumLogLevel)
            {
                logEntries.Add(logEntry);

                // Manage log entry count
                if (logEntries.Count > maxPerformanceEntries * 2)
                {
                    logEntries.RemoveRange(0, logEntries.Count - maxPerformanceEntries);
                }
            }

            // Performance tracking
            if (trackPerformance && processingTime > 0)
            {
                performanceEntries.Add(logEntry);

                // Manage performance entry count
                if (performanceEntries.Count > maxPerformanceEntries)
                {
                    performanceEntries.RemoveAt(0);
                }
            }

            // File logging
            if (enableFileLogging)
            {
                WriteLogToFile(logEntry);
            }

            // Trigger external logging
            OnLogEvent?.Invoke(logEntry);

            // Debug logging
            LogToUnityConsole(logEntry);
        }

        /// <summary>
        /// Write log entry to file
        /// </summary>
        private void WriteLogToFile(EncounterLogEntry logEntry)
        {
            try
            {
                string filename = Path.Combine(
                    Application.persistentDataPath, 
                    logFilePath, 
                    $"encounter_log_{DateTime.Now:yyyyMMdd}.txt"
                );

                string logMessage = FormatLogEntryForFile(logEntry);

                File.AppendAllText(filename, logMessage);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write log to file: {ex.Message}");
            }
        }

        /// <summary>
        /// Format log entry for file output
        /// </summary>
        private string FormatLogEntryForFile(EncounterLogEntry logEntry)
        {
            return $"[{logEntry.timestamp:yyyy-MM-dd HH:mm:ss}] " +
                   $"{logEntry.logLevel} - " +
                   $"Encounter {logEntry.encounterID} " +
                   $"({logEntry.shipType}) - " +
                   $"{logEntry.message}\n" +
                   $"Processing Time: {logEntry.processingTime}s\n" +
                   $"Additional Data: {logEntry.additionalData}\n\n";
        }

        /// <summary>
        /// Log to Unity Console
        /// </summary>
        private void LogToUnityConsole(EncounterLogEntry logEntry)
        {
            switch (logEntry.logLevel)
            {
                case LogLevel.Debug:
                    Debug.Log(logEntry.message);
                    break;
                case LogLevel.Info:
                    Debug.Log(logEntry.message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(logEntry.message);
                    break;
                case LogLevel.Error:
                    Debug.LogError(logEntry.message);
                    break;
                case LogLevel.Critical:
                    Debug.LogError($"CRITICAL: {logEntry.message}");
                    break;
            }
        }

        /// <summary>
        /// Get performance summary
        /// </summary>
        public string GetPerformanceSummary()
        {
            if (performanceEntries.Count == 0)
                return "No performance data available.";

            float averageProcessingTime = 0f;
            float maxProcessingTime = float.MinValue;
            float minProcessingTime = float.MaxValue;

            foreach (var entry in performanceEntries)
            {
                averageProcessingTime += entry.processingTime;
                maxProcessingTime = Mathf.Max(maxProcessingTime, entry.processingTime);
                minProcessingTime = Mathf.Min(minProcessingTime, entry.processingTime);
            }

            averageProcessingTime /= performanceEntries.Count;

            return $"Performance Summary:\n" +
                   $"Total Encounters: {performanceEntries.Count}\n" +
                   $"Average Processing Time: {averageProcessingTime:F4}s\n" +
                   $"Max Processing Time: {maxProcessingTime:F4}s\n" +
                   $"Min Processing Time: {minProcessingTime:F4}s";
        }

        /// <summary>
        /// Clear all logs
        /// </summary>
        public void ClearLogs()
        {
            logEntries.Clear();
            performanceEntries.Clear();
        }
    }
}