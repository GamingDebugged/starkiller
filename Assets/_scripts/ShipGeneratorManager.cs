using UnityEngine;
using System.Collections;
using StarkillerBaseCommand;

/// <summary>
/// Persistent manager that ensures MasterShipGenerator references are maintained across scene transitions.
/// Serves as a central reference point to prevent systems from losing track of each other.
/// </summary>
public class ShipGeneratorManager : MonoBehaviour
{
    // Singleton pattern
    private static ShipGeneratorManager _instance;
    public static ShipGeneratorManager Instance => _instance;

    // Reference to the MasterShipGenerator
    private MasterShipGenerator _shipGenerator;
    public MasterShipGenerator ShipGenerator => _shipGenerator;

    // Reference to key systems
    private CredentialChecker _credentialChecker;
    private GameStateController _gameStateController;
    private ShipTimingController _shipTimingController;

    // Debug options
    [SerializeField] private bool verboseLogging = true;

    private void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Make this object persistent
        if (transform.root == transform)
        {
            DontDestroyOnLoad(gameObject);
            LogMessage("Ship Generator Manager initialized and set as persistent");
        }
        else
        {
            LogMessage("Warning: Ship Generator Manager is not a root GameObject. DontDestroyOnLoad may not work correctly.");
        }
    }

    private void Start()
    {
        // Find all critical references and connect them
        StartCoroutine(InitializeReferences());
    }

    /// <summary>
    /// Initialize references with a delay to ensure all systems have time to initialize
    /// </summary>
    private IEnumerator InitializeReferences()
    {
        // Wait for at least one frame to allow other systems to initialize
        yield return null;
        
        // Find MasterShipGenerator
        if (_shipGenerator == null)
        {
            _shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            if (_shipGenerator == null)
            {
                LogMessage("MasterShipGenerator not found! Some features will not work.");
                yield break;
            }
            LogMessage($"Connected to MasterShipGenerator: {_shipGenerator.name}");
        }
        
        // Find CredentialChecker
        _credentialChecker = FindFirstObjectByType<CredentialChecker>();
        if (_credentialChecker != null)
        {
            LogMessage($"Connected to CredentialChecker: {_credentialChecker.name}");
        }
        
        // Find GameStateController
        _gameStateController = GameStateController.Instance;
        if (_gameStateController != null)
        {
            LogMessage($"Connected to GameStateController");
            
            // Subscribe to game state changes
            _gameStateController.OnGameStateChanged += OnGameStateChanged;
        }
        
        // Find ShipTimingController
        _shipTimingController = ShipTimingController.Instance;
        if (_shipTimingController != null)
        {
            LogMessage($"Connected to ShipTimingController");
        }
        
        // Perform a connection validation
        yield return StartCoroutine(ValidateConnections());
        
        // If we're already in active gameplay, immediately force synchronize all references
        if (_gameStateController != null && _gameStateController.IsGameplayActive())
        {
            ForceSyncReferences();
        }
    }

    /// <summary>
    /// Force synchronize all references between systems
    /// </summary>
    public void ForceSyncReferences()
    {
        // Skip if we don't have the essential component
        if (_shipGenerator == null)
        {
            LogMessage("Cannot sync references - MasterShipGenerator not found!");
            return;
        }

        LogMessage("Forcing synchronization of references between systems");
        
        // Make sure CredentialChecker has the ship generator reference
        if (_credentialChecker != null)
        {
            // Using reflection to access private fields if needed
            var shipGeneratorField = _credentialChecker.GetType().GetField("shipGenerator", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
                
            if (shipGeneratorField != null)
            {
                shipGeneratorField.SetValue(_credentialChecker, _shipGenerator);
                LogMessage("Updated CredentialChecker's shipGenerator reference");
            }
            
            // Resubscribe to the encounter ready event
            try
            {
                // First try to unsubscribe to avoid duplicate registrations
                _shipGenerator.OnEncounterReady -= _credentialChecker.DisplayEncounter;
                
                // Then subscribe again
                _shipGenerator.OnEncounterReady += _credentialChecker.DisplayEncounter;
                LogMessage("Re-established event subscription between MasterShipGenerator and CredentialChecker");
            }
            catch (System.Exception e)
            {
                LogMessage($"Error re-establishing event subscription: {e.Message}");
            }
            
            // Force UI visibility update
            _credentialChecker.ForceUIVisibility();
        }
        
        // Reset any timing controller locks
        if (_shipTimingController != null)
        {
            _shipTimingController.ResetCooldown();
            LogMessage("Reset ShipTimingController cooldowns");
        }
        
        // If no ship is currently being processed, get the next encounter
        if (_credentialChecker != null && !_credentialChecker.HasActiveEncounter())
        {
            LogMessage("No active encounter detected, requesting next encounter");
            _shipGenerator.GetNextEncounter();
        }
    }

    /// <summary>
    /// Handle game state changes
    /// </summary>
    private void OnGameStateChanged(GameStateController.GameActivationState newState)
    {
        LogMessage($"Game state changed to: {newState}");
        
        // When transitioning to ActiveGameplay, ensure references are synchronized
        if (newState == GameStateController.GameActivationState.ActiveGameplay)
        {
            LogMessage("Game entering active gameplay state - ensuring all references are valid");
            StartCoroutine(DelayedSyncReferences(0.5f));
        }
    }

    /// <summary>
    /// Sync references after a delay to ensure all systems have time to initialize
    /// </summary>
    private IEnumerator DelayedSyncReferences(float delay)
    {
        yield return new WaitForSeconds(delay);
        ForceSyncReferences();
    }

    /// <summary>
    /// Validate that all essential connections are working
    /// </summary>
    private IEnumerator ValidateConnections()
    {
        LogMessage("Validating system connections...");
        
        // Validate MasterShipGenerator
        if (_shipGenerator == null)
        {
            LogMessage("CRITICAL: MasterShipGenerator not found!");
            yield break;
        }
        
        // Validate CredentialChecker
        if (_credentialChecker == null)
        {
            LogMessage("WARNING: CredentialChecker not found!");
        }
        else
        {
            // Validate event subscription
            bool isSubscribed = false;
            var field = typeof(MasterShipGenerator).GetField("OnEncounterReady", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                
            if (field != null)
            {
                var eventDelegate = field.GetValue(_shipGenerator) as MasterShipGenerator.EncounterReadyHandler;
                isSubscribed = (eventDelegate != null);
            }
            
            if (!isSubscribed)
            {
                LogMessage("WARNING: CredentialChecker not subscribed to OnEncounterReady event!");
                // Re-establish subscription
                try
                {
                    _shipGenerator.OnEncounterReady += _credentialChecker.DisplayEncounter;
                    LogMessage("Re-established event subscription");
                }
                catch (System.Exception e)
                {
                    LogMessage($"Error establishing event subscription: {e.Message}");
                }
            }
            else
            {
                LogMessage("CredentialChecker is properly subscribed to OnEncounterReady");
            }
        }
        
        // Validate ShipTimingController
        if (_shipTimingController == null)
        {
            LogMessage("WARNING: ShipTimingController not found!");
        }
        
        LogMessage("Connection validation complete");
    }

    /// <summary>
    /// Called by external systems to check if the MasterShipGenerator is currently available
    /// </summary>
    public bool HasValidShipGenerator()
    {
        return _shipGenerator != null;
    }

    /// <summary>
    /// Called by external systems to get the current MasterShipGenerator reference
    /// </summary>
    public MasterShipGenerator GetShipGenerator()
    {
        if (_shipGenerator == null)
        {
            // Try to find it again
            _shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
            if (_shipGenerator == null)
            {
                LogMessage("GetShipGenerator called but no generator found!");
            }
        }
        
        return _shipGenerator;
    }

    /// <summary>
    /// Log a message with optional prefix
    /// </summary>
    private void LogMessage(string message)
    {
        if (verboseLogging)
        {
            Debug.Log($"[ShipGeneratorManager] {message}");
        }
    }
}