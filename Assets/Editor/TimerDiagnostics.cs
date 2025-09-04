using UnityEngine;
using UnityEditor;
using TMPro;

/// <summary>
/// Diagnostic tool to debug timer and difficulty profile issues
/// </summary>
public class TimerDiagnostics : EditorWindow
{
    [MenuItem("Starkiller/Debug Timer Issues")]
    public static void ShowWindow()
    {
        GetWindow<TimerDiagnostics>("Timer Diagnostics");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Timer Diagnostics", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Check Timer State"))
        {
            CheckTimerState();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Check Difficulty Profile"))
        {
            CheckDifficultyProfile();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Force Timer Restart"))
        {
            ForceTimerRestart();
        }
    }
    
    private void CheckTimerState()
    {
        Debug.Log("=== TIMER STATE DIAGNOSTICS ===");
        
        // Check ShiftTimerManager
        var shiftTimerManager = FindFirstObjectByType<Starkiller.Core.Managers.ShiftTimerManager>();
        if (shiftTimerManager != null)
        {
            var serializedObject = new SerializedObject(shiftTimerManager);
            
            var gameTimerTextProp = serializedObject.FindProperty("gameTimerText");
            var shiftTimeLimitProp = serializedObject.FindProperty("shiftTimeLimit");
            var isTimerActiveProp = serializedObject.FindProperty("_isTimerActive");
            var useDifficultyProfileProp = serializedObject.FindProperty("useDifficultyProfile");
            var difficultyProfileProp = serializedObject.FindProperty("difficultyProfile");
            
            Debug.Log($"🕒 ShiftTimerManager Found:");
            Debug.Log($"  - gameTimerText: {(gameTimerTextProp?.objectReferenceValue != null ? gameTimerTextProp.objectReferenceValue.name : "NULL")}");
            Debug.Log($"  - shiftTimeLimit: {shiftTimeLimitProp?.floatValue ?? 0}s");
            Debug.Log($"  - isTimerActive: {isTimerActiveProp?.boolValue ?? false}");
            Debug.Log($"  - useDifficultyProfile: {useDifficultyProfileProp?.boolValue ?? false}");
            Debug.Log($"  - difficultyProfile: {(difficultyProfileProp?.objectReferenceValue != null ? difficultyProfileProp.objectReferenceValue.name : "NULL")}");
            
            // Check current day settings
            var currentDaySettings = shiftTimerManager.GetCurrentDaySettings();
            if (currentDaySettings != null)
            {
                Debug.Log($"📅 Current Day Settings:");
                Debug.Log($"  - Day: {currentDaySettings.dayNumber}");
                Debug.Log($"  - Time Limit: {currentDaySettings.shiftTimeLimit}s");
                Debug.Log($"  - Ship Quota: {currentDaySettings.shipQuota}");
            }
            else
            {
                Debug.LogError("❌ Current day settings are NULL!");
            }
        }
        else
        {
            Debug.LogError("❌ ShiftTimerManager not found!");
        }
        
        // Check GameManager
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            var serializedObject = new SerializedObject(gameManager);
            var gameTimerTextProp = serializedObject.FindProperty("gameTimerText");
            var currentDayProp = serializedObject.FindProperty("currentDay");
            
            Debug.Log($"🎮 GameManager Found:");
            Debug.Log($"  - gameTimerText: {(gameTimerTextProp?.objectReferenceValue != null ? gameTimerTextProp.objectReferenceValue.name : "NULL")}");
            Debug.Log($"  - currentDay: {currentDayProp?.intValue ?? 0}");
        }
        else
        {
            Debug.LogError("❌ GameManager not found!");
        }
        
        Debug.Log("=== END TIMER DIAGNOSTICS ===");
    }
    
    private void CheckDifficultyProfile()
    {
        Debug.Log("=== DIFFICULTY PROFILE DIAGNOSTICS ===");
        
        var shiftTimerManager = FindFirstObjectByType<Starkiller.Core.Managers.ShiftTimerManager>();
        if (shiftTimerManager == null)
        {
            Debug.LogError("❌ ShiftTimerManager not found!");
            return;
        }
        
        var difficultyProfile = shiftTimerManager.GetDifficultyProfile();
        if (difficultyProfile == null)
        {
            Debug.LogError("❌ No difficulty profile assigned!");
            return;
        }
        
        Debug.Log($"📊 Difficulty Profile: {difficultyProfile.name}");
        
        // Check settings for days 1-5
        for (int day = 1; day <= 5; day++)
        {
            var daySettings = difficultyProfile.GetDaySettings(day);
            if (daySettings != null)
            {
                Debug.Log($"📅 Day {day} Settings:");
                Debug.Log($"  - Time Limit: {daySettings.shiftTimeLimit}s");
                Debug.Log($"  - Ship Quota: {daySettings.shipQuota}");
                Debug.Log($"  - Warnings Enabled: {daySettings.enableWarnings}");
            }
            else
            {
                Debug.LogWarning($"⚠️ No settings found for Day {day}!");
            }
        }
        
        Debug.Log("=== END DIFFICULTY PROFILE DIAGNOSTICS ===");
    }
    
    private void ForceTimerRestart()
    {
        Debug.Log("🔄 Force restarting timer...");
        
        var shiftTimerManager = FindFirstObjectByType<Starkiller.Core.Managers.ShiftTimerManager>();
        if (shiftTimerManager != null)
        {
            // Stop current timer
            shiftTimerManager.StopTimer();
            
            // Wait a frame then restart
            EditorApplication.delayCall += () => {
                if (shiftTimerManager != null)
                {
                    shiftTimerManager.StartTimer();
                    Debug.Log("✅ Timer restarted!");
                }
            };
        }
        else
        {
            Debug.LogError("❌ ShiftTimerManager not found!");
        }
    }
}