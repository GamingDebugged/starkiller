using UnityEngine;

/// <summary>
/// Time modifier component that can be attached to any GameObject
/// Will pause or slow down time while active
/// </summary>
public class TimeModifierBehavior : MonoBehaviour
{
    [Header("Time Modifier Settings")]
    [Tooltip("Whether this object pauses time or just slows it down")]
    public bool pauseTime = true;
    
    [Tooltip("Automatically modify time when this GameObject is enabled")]
    public bool modifyOnEnable = true;
    
    [Tooltip("Automatically resume time when this GameObject is disabled")]
    public bool resumeOnDisable = true;
    
    [Tooltip("Custom ID for this time modifier (leave blank for auto-generated)")]
    [SerializeField] private string modifierId = "";
    
    // Track if we're currently modifying time
    private bool isModifyingTime = false;
    
    private void OnEnable()
    {
        if (modifyOnEnable)
        {
            ApplyTimeModification();
        }
    }
    
    private void OnDisable()
    {
        if (resumeOnDisable && isModifyingTime)
        {
            ResumeTime();
        }
    }
    
    private void OnDestroy()
    {
        // Always resume time when destroyed to prevent issues
        if (isModifyingTime)
        {
            TimeManager.Instance.ResumeTimeForSource(gameObject);
            isModifyingTime = false;
        }
    }
    
    /// <summary>
    /// Apply the appropriate time modification
    /// </summary>
    public void ApplyTimeModification()
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogWarning("TimeModifierBehavior: TimeManager instance not found!");
            return;
        }
        
        if (pauseTime)
        {
            TimeManager.Instance.PauseTime(gameObject, modifierId);
        }
        else
        {
            TimeManager.Instance.SlowTime(gameObject, modifierId);
        }
        
        isModifyingTime = true;
    }
    
    /// <summary>
    /// Resume normal time
    /// </summary>
    public void ResumeTime()
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogWarning("TimeModifierBehavior: TimeManager instance not found!");
            return;
        }
        
        if (string.IsNullOrEmpty(modifierId))
        {
            TimeManager.Instance.ResumeTimeForSource(gameObject);
        }
        else
        {
            TimeManager.Instance.ResumeTime(modifierId);
        }
        
        isModifyingTime = false;
    }
    
    /// <summary>
    /// Toggle the time modification on/off
    /// </summary>
    public void ToggleTimeModification()
    {
        if (isModifyingTime)
        {
            ResumeTime();
        }
        else
        {
            ApplyTimeModification();
        }
    }
}