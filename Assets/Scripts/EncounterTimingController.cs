using UnityEngine;
using System.Collections;

public class EncounterTimingController : MonoBehaviour
{
    [Header("Timing Settings")]
    [SerializeField] private float minimumEncounterDuration = 2.0f; // Reduced from 5.0f
    [SerializeField] private float cooldownAfterDecision = 0.5f; // Reduced from 2.0f
    [SerializeField] private bool enableTimingRestrictions = true; // Toggle for testing
    
    [Header("State")]
    [SerializeField] private bool isEncounterActive = false;
    [SerializeField] private float encounterStartTime = 0f;
    [SerializeField] private string currentEncounterId = "";
    
    private static EncounterTimingController _instance;
    public static EncounterTimingController Instance => _instance;
    
    private float minimumTimeBetweenEncounters = 2.0f; // Reduced from 5.0f
    private float lastEncounterDisplayTime = 0f;
    private Coroutine cooldownCoroutine;
    private bool isInCooldown = false;
    
    private MasterShipEncounter currentEncounter;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void OnEncounterDisplayed(MasterShipEncounter encounter)
    {
        if (encounter == null) return;
        
        // Generate a unique ID for the encounter
        string encounterId = GenerateEncounterId(encounter);
        
        // If we're displaying the same encounter again, don't reset the timer
        if (encounterId == currentEncounterId && isEncounterActive)
        {
            return;
        }
        
        // Clear any existing cooldown when a new encounter is displayed
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
            isInCooldown = false;
        }
        
        isEncounterActive = true;
        encounterStartTime = Time.time;
        currentEncounterId = encounterId;
        Debug.Log($"EncounterTimingController: Encounter started - {encounter.shipType} (ID: {encounterId})");
    }
    
    public void OnDecisionMade(bool approved)
    {
        if (!isEncounterActive) return;
        
        float duration = Time.time - encounterStartTime;
        Debug.Log($"EncounterTimingController: Decision after {duration:F1}s - {(approved ? "Approved" : "Denied")}");
        
        isEncounterActive = false;
        currentEncounterId = "";
        
        // Start cooldown
        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);
        cooldownCoroutine = StartCoroutine(DecisionCooldown());
    }
    
    public bool CanDisplayNewEncounter()
    {
        // Check if enough time has passed since last encounter
        float timeSinceLastEncounter = Time.time - lastEncounterDisplayTime;
        
        if (timeSinceLastEncounter < minimumTimeBetweenEncounters)
        {
            Debug.Log($"EncounterTimingController: Encounter too recent ({timeSinceLastEncounter:F1}s < {minimumTimeBetweenEncounters:F1}s)");
            return false;
        }
        
        return true;
    }
    
    public void ForceReset()
    {
        Debug.Log("EncounterTimingController: Force reset requested");
        
        isEncounterActive = false;
        encounterStartTime = 0f;
        currentEncounterId = "";
        isInCooldown = false;
        
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
    }
    
    /// <summary>
    /// Emergency method to bypass all timing restrictions
    /// </summary>
    public void DisableTimingRestrictions()
    {
        enableTimingRestrictions = false;
        ForceReset();
        Debug.LogWarning("EncounterTimingController: Timing restrictions DISABLED");
    }
    
    /// <summary>
    /// Re-enable timing restrictions
    /// </summary>
    public void EnableTimingRestrictions()
    {
        enableTimingRestrictions = true;
        Debug.Log("EncounterTimingController: Timing restrictions enabled");
    }
    
    private IEnumerator DecisionCooldown()
    {
        isInCooldown = true;
        yield return new WaitForSeconds(cooldownAfterDecision);
        isInCooldown = false;
        cooldownCoroutine = null;
        Debug.Log("EncounterTimingController: Cooldown complete, ready for next encounter");
    }
    
    /// <summary>
    /// Generate a unique ID for an encounter
    /// </summary>
    private string GenerateEncounterId(MasterShipEncounter encounter)
    {
        if (encounter == null) return "";
        return $"{encounter.shipType}_{encounter.captainName}_{encounter.GetHashCode()}";
    }
    
    // Debug method to check current state
    public void LogCurrentState()
    {
        Debug.Log($"EncounterTimingController State:");
        Debug.Log($"  - Timing Enabled: {enableTimingRestrictions}");
        Debug.Log($"  - Encounter Active: {isEncounterActive}");
        Debug.Log($"  - In Cooldown: {isInCooldown}");
        Debug.Log($"  - Current ID: {currentEncounterId}");
        Debug.Log($"  - Can Display New: {CanDisplayNewEncounter()}");
    }
}