using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the encounter flow to prevent rapid overwriting and ensure proper timing between encounters
/// </summary>
public class EncounterFlowManager : MonoBehaviour
{
    // Singleton instance
    private static EncounterFlowManager _instance;
    public static EncounterFlowManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EncounterFlowManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("EncounterFlowManager");
                    _instance = go.AddComponent<EncounterFlowManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Encounter Timing Settings")]
    [SerializeField] private float minEncounterInterval = 5f; // Minimum 5 seconds between encounters
    [SerializeField] private float encounterCooldown = 3f; // Cooldown after processing a decision
    
    // State tracking
    private bool isProcessingEncounter = false;
    private float lastEncounterTime = 0f;
    private float lastDecisionTime = 0f;
    private bool isInCooldown = false;
    
    // References
    private GameManager gameManager;
    private MasterShipGenerator shipGenerator;
    private CredentialChecker credentialChecker;
    
    void Awake()
    {
        // Ensure singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    void Start()
    {
        // Get references
        gameManager = FindFirstObjectByType<GameManager>();
        shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
        credentialChecker = FindFirstObjectByType<CredentialChecker>();
        
        if (gameManager == null)
            Debug.LogError("EncounterFlowManager: GameManager not found!");
        if (shipGenerator == null)
            Debug.LogWarning("EncounterFlowManager: MasterShipGenerator not found!");
        if (credentialChecker == null)
            Debug.LogError("EncounterFlowManager: CredentialChecker not found!");
    }

    /// <summary>
    /// Request the next encounter - centralized entry point
    /// </summary>
    public void RequestNextEncounter(string requestSource = "Unknown")
    {
        // Check if we're allowed to process a new encounter
        if (!CanProcessNewEncounter())
        {
            Debug.Log($"[EncounterFlow] Request from '{requestSource}' blocked - " +
                     $"Processing: {isProcessingEncounter}, " +
                     $"Cooldown: {isInCooldown}, " +
                     $"Time since last: {Time.time - lastEncounterTime:F1}s");
            return;
        }
        
        Debug.Log($"[EncounterFlow] Processing encounter request from '{requestSource}'");
        StartCoroutine(ProcessNextEncounter());
    }

    /// <summary>
    /// Check if we can process a new encounter
    /// </summary>
    private bool CanProcessNewEncounter()
    {
        // Don't process if already processing
        if (isProcessingEncounter) return false;
        
        // Don't process during cooldown
        if (isInCooldown) return false;
        
        // Check minimum time between encounters
        if (Time.time - lastEncounterTime < minEncounterInterval) return false;
        
        // Check if game is in valid state
        if (gameManager != null)
        {
            if (!gameManager.IsGameActive() || gameManager.IsPaused()) return false;
        }
        
        return true;
    }

    /// <summary>
    /// Process the next encounter with proper timing
    /// </summary>
    private IEnumerator ProcessNextEncounter()
    {
        isProcessingEncounter = true;
        lastEncounterTime = Time.time;
        
        // Small delay to ensure UI is ready
        yield return new WaitForSeconds(0.5f);
        
        // Get encounter from appropriate generator
        if (shipGenerator != null)
        {
            var encounter = shipGenerator.GetNextEncounter();
            if (encounter != null && credentialChecker != null)
            {
                Debug.Log($"[EncounterFlow] Displaying encounter: {encounter.shipType}");
                credentialChecker.DisplayEncounter(encounter);
            }
            else
            {
                Debug.LogWarning("[EncounterFlow] Failed to get or display encounter");
            }
        }
        else
        {
            // Fallback to GameManager's generation method
            Debug.Log("[EncounterFlow] Using GameManager fallback for encounter generation");
            gameManager.SendMessage("GenerateNewShipEncounter", SendMessageOptions.DontRequireReceiver);
        }
        
        // Wait minimum interval before allowing next request
        yield return new WaitForSeconds(minEncounterInterval);
        
        isProcessingEncounter = false;
    }

    /// <summary>
    /// Called when a decision is made - triggers cooldown
    /// </summary>
    public void OnDecisionMade()
    {
        lastDecisionTime = Time.time;
        StartCoroutine(DecisionCooldown());
    }

    /// <summary>
    /// Cooldown after making a decision
    /// </summary>
    private IEnumerator DecisionCooldown()
    {
        isInCooldown = true;
        Debug.Log($"[EncounterFlow] Starting {encounterCooldown}s cooldown after decision");
        
        yield return new WaitForSeconds(encounterCooldown);
        
        isInCooldown = false;
        Debug.Log("[EncounterFlow] Cooldown complete, ready for next encounter");
        
        // COMMENT OUT AUTO-REQUEST TO STOP AUTOMATIC ENCOUNTER ADVANCEMENT
        // RequestNextEncounter("Post-Decision Auto-Request");
    }

    /// <summary>
    /// Reset the flow manager state
    /// </summary>
    public void Reset()
    {
        isProcessingEncounter = false;
        isInCooldown = false;
        lastEncounterTime = 0f;
        lastDecisionTime = 0f;
    }

    /// <summary>
    /// Get current state for debugging
    /// </summary>
    public string GetDebugState()
    {
        return $"Processing: {isProcessingEncounter}, Cooldown: {isInCooldown}, " +
               $"Time since last encounter: {Time.time - lastEncounterTime:F1}s, " +
               $"Time since last decision: {Time.time - lastDecisionTime:F1}s";
    }
}
