using UnityEngine;
using System.Collections.Generic;
using StarkillerBaseCommand;

/// <summary>
/// Central manager for controlling game time flow
/// Handles pausing, slowing, and resuming time based on active time modifiers
/// </summary>
public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Default time scale when no modifiers are active")]
    [SerializeField] private float defaultTimeScale = 1.0f;
    
    [Tooltip("Time scale when paused")]
    [SerializeField] private float pausedTimeScale = 0.0f;
    
    [Tooltip("Time scale when slowed")]
    [Range(0.1f, 0.9f)]
    [SerializeField] private float slowedTimeScale = 0.5f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    // Keep track of active time modifiers
    private Dictionary<string, TimeModifier> activeModifiers = new Dictionary<string, TimeModifier>();
    
    // Current time state
    private enum TimeState { Normal, Slowed, Paused }
    private TimeState currentState = TimeState.Normal;
    
    // Class to track time modifiers
    private class TimeModifier
    {
        public string id;
        public GameObject source;
        public bool isPause;
        
        public TimeModifier(string id, GameObject source, bool isPause)
        {
            this.id = id;
            this.source = source;
            this.isPause = isPause;
        }
    }
    
    // Singleton pattern
    private static TimeManager _instance;
    public static TimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TimeManager>();
                
                if (_instance == null)
                {
                    GameObject timeManagerObj = new GameObject("TimeManager");
                    _instance = timeManagerObj.AddComponent<TimeManager>();
                }
            }
            
            return _instance;
        }
    }
    
    private void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize time to default
        Time.timeScale = defaultTimeScale;
        LogStatus($"TimeManager initialized with default scale {defaultTimeScale}");
    }
    
    /// <summary>
    /// Pause time and register the source
    /// </summary>
    /// <param name="source">GameObject requesting the pause</param>
    /// <param name="id">Unique identifier for this pause request</param>
    public void PauseTime(GameObject source, string id = null)
    {
        // Generate ID if not provided
        if (string.IsNullOrEmpty(id))
        {
            id = source.name + "_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        }
        
        // Add or update modifier
        if (activeModifiers.ContainsKey(id))
        {
            activeModifiers[id] = new TimeModifier(id, source, true);
            LogStatus($"Updated time pause: {id} from {source.name}");
        }
        else
        {
            activeModifiers.Add(id, new TimeModifier(id, source, true));
            LogStatus($"Added time pause: {id} from {source.name}");
        }
        
        // Update time state
        UpdateTimeState();
    }
    
    /// <summary>
    /// Slow down time and register the source
    /// </summary>
    /// <param name="source">GameObject requesting the slow down</param>
    /// <param name="id">Unique identifier for this slow request</param>
    public void SlowTime(GameObject source, string id = null)
    {
        // Generate ID if not provided
        if (string.IsNullOrEmpty(id))
        {
            id = source.name + "_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        }
        
        // Add or update modifier
        if (activeModifiers.ContainsKey(id))
        {
            activeModifiers[id] = new TimeModifier(id, source, false);
            LogStatus($"Updated time slow: {id} from {source.name}");
        }
        else
        {
            activeModifiers.Add(id, new TimeModifier(id, source, false));
            LogStatus($"Added time slow: {id} from {source.name}");
        }
        
        // Update time state
        UpdateTimeState();
    }
    
    /// <summary>
    /// Resume time for a specific source
    /// </summary>
    /// <param name="id">ID of the modifier to remove</param>
    public void ResumeTime(string id)
    {
        if (activeModifiers.ContainsKey(id))
        {
            LogStatus($"Removed time modifier: {id} from {activeModifiers[id].source.name}");
            activeModifiers.Remove(id);
            
            // Update time state
            UpdateTimeState();
        }
    }
    
    /// <summary>
    /// Resume time for all modifiers from a specific source
    /// </summary>
    /// <param name="source">GameObject that was modifying time</param>
    public void ResumeTimeForSource(GameObject source)
    {
        List<string> keysToRemove = new List<string>();
        
        foreach (var pair in activeModifiers)
        {
            if (pair.Value.source == source)
            {
                keysToRemove.Add(pair.Key);
            }
        }
        
        foreach (var key in keysToRemove)
        {
            LogStatus($"Removed time modifier: {key} from {source.name}");
            activeModifiers.Remove(key);
        }
        
        // Update time state if any modifiers were removed
        if (keysToRemove.Count > 0)
        {
            UpdateTimeState();
        }
    }
    
    /// <summary>
    /// Update the time scale based on active modifiers
    /// </summary>
    private void UpdateTimeState()
    {
        // Check if any modifier is a pause
        bool hasPause = false;
        bool hasSlow = false;
        
        foreach (var modifier in activeModifiers.Values)
        {
            if (modifier.isPause)
            {
                hasPause = true;
                break;
            }
            else
            {
                hasSlow = true;
            }
        }
        
        // Apply appropriate time scale
        if (hasPause)
        {
            // Pause takes priority
            Time.timeScale = pausedTimeScale;
            currentState = TimeState.Paused;
            LogStatus($"Time PAUSED: {activeModifiers.Count} active modifiers");
        }
        else if (hasSlow)
        {
            // Apply slow down
            Time.timeScale = slowedTimeScale;
            currentState = TimeState.Slowed;
            LogStatus($"Time SLOWED: {activeModifiers.Count} active modifiers");
        }
        else
        {
            // Resume normal time
            Time.timeScale = defaultTimeScale;
            currentState = TimeState.Normal;
            LogStatus("Time RESUMED: Normal speed");
        }
    }
    
    /// <summary>
    /// Check if time is currently paused
    /// </summary>
    public bool IsTimePaused()
    {
        return currentState == TimeState.Paused;
    }
    
    /// <summary>
    /// Check if time is currently slowed
    /// </summary>
    public bool IsTimeSlowed()
    {
        return currentState == TimeState.Slowed;
    }
    
    /// <summary>
    /// Get the current time scale
    /// </summary>
    public float GetCurrentTimeScale()
    {
        return Time.timeScale;
    }
    
    /// <summary>
    /// Clear all time modifiers - useful for debugging
    /// </summary>
    [ContextMenu("Clear All Time Modifiers")]
    public void ClearAllTimeModifiers()
    {
        activeModifiers.Clear();
        Time.timeScale = defaultTimeScale;
        currentState = TimeState.Normal;
        LogStatus("Cleared all time modifiers - time resumed");
    }
    
    /// <summary>
    /// Show all active time modifiers
    /// </summary>
    [ContextMenu("Show Active Time Modifiers")]
    public void ShowActiveTimeModifiers()
    {
        Debug.Log($"[TimeManager] Active modifiers: {activeModifiers.Count}");
        foreach (var modifier in activeModifiers)
        {
            Debug.Log($"  - {modifier.Key}: {modifier.Value.source.name} (Pause: {modifier.Value.isPause})");
        }
    }

    /// <summary>
    /// Log status messages for debugging
    /// </summary>
    private void LogStatus(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[TimeManager] {message}");
        }
    }
}