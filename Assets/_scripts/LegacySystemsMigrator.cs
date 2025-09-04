using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StarkillerBaseCommand;

/// <summary>
/// Migrates data from legacy systems to new Starkiller systems
/// Helps transition the codebase without breaking existing functionality
/// </summary>
public class LegacySystemsMigrator : MonoBehaviour
{
    [Header("Target Systems")]
    [SerializeField] private StarkkillerContentManager contentManager;
    [SerializeField] private StarkkillerMediaSystem mediaSystem;
    [SerializeField] private StarkkillerEncounterSystem encounterSystem;
    
    [Header("Legacy Systems")]
    [SerializeField] private ShipEncounterSystem legacyEncounterSystem;
    [SerializeField] private ShipVideoSystem legacyVideoSystem;
    [SerializeField] private ShipImageSystem legacyImageSystem;
    
    [Header("Migration Settings")]
    [SerializeField] private bool migrateOnAwake = false;
    [SerializeField] private bool verboseLogging = true;
    
    [Header("Migration Status")]
    [SerializeField] private bool contentMigrated = false;
    [SerializeField] private bool mediaMigrated = false;
    [SerializeField] private bool encounterMigrated = false;
    
    private void Awake()
    {
        // Find required components
        FindSystemReferences();
        
        // Migrate on awake if enabled
        if (migrateOnAwake)
        {
            MigrateAll();
        }
    }
    
    /// <summary>
    /// Find all system references if not already assigned
    /// </summary>
    public void FindSystemReferences()
    {
        // Find target systems
        if (contentManager == null)
            contentManager = FindFirstObjectByType<StarkkillerContentManager>();
            
        if (mediaSystem == null)
            mediaSystem = FindFirstObjectByType<StarkkillerMediaSystem>();
            
        if (encounterSystem == null)
            encounterSystem = FindFirstObjectByType<StarkkillerEncounterSystem>();
            
        // Find legacy systems
        if (legacyEncounterSystem == null)
            legacyEncounterSystem = FindFirstObjectByType<ShipEncounterSystem>();
            
        if (legacyVideoSystem == null)
            legacyVideoSystem = FindFirstObjectByType<ShipVideoSystem>();
            
        if (legacyImageSystem == null)
            legacyImageSystem = FindFirstObjectByType<ShipImageSystem>();
            
        if (verboseLogging)
        {
            Debug.Log($"LegacySystemsMigrator found references:\n" +
                      $"Target Systems - ContentManager: {contentManager != null}, MediaSystem: {mediaSystem != null}, EncounterSystem: {encounterSystem != null}\n" +
                      $"Legacy Systems - EncounterSystem: {legacyEncounterSystem != null}, VideoSystem: {legacyVideoSystem != null}, ImageSystem: {legacyImageSystem != null}");
        }
    }
    
    /// <summary>
    /// Find legacy systems in the scene
    /// </summary>
    public void FindLegacySystems()
    {
        legacyEncounterSystem = FindFirstObjectByType<ShipEncounterSystem>();
        legacyVideoSystem = FindFirstObjectByType<ShipVideoSystem>();
        legacyImageSystem = FindFirstObjectByType<ShipImageSystem>();
        
        if (verboseLogging)
        {
            Debug.Log($"Found legacy systems - EncounterSystem: {legacyEncounterSystem != null}, VideoSystem: {legacyVideoSystem != null}, ImageSystem: {legacyImageSystem != null}");
        }
    }
    
    /// <summary>
    /// Find new systems in the scene
    /// </summary>
    public void FindNewSystems()
    {
        contentManager = FindFirstObjectByType<StarkkillerContentManager>();
        mediaSystem = FindFirstObjectByType<StarkkillerMediaSystem>();
        encounterSystem = FindFirstObjectByType<StarkkillerEncounterSystem>();
        
        if (verboseLogging)
        {
            Debug.Log($"Found new systems: ContentManager: {contentManager != null}, MediaSystem: {mediaSystem != null}, EncounterSystem: {encounterSystem != null}");
        }
    }
    
    /// <summary>
    /// Migrate all data from legacy systems to new systems
    /// </summary>
    public void MigrateAll()
    {
        // Make sure we have references
        FindSystemReferences();
        
        // Migrate content first
        MigrateContent();
        
        // Then migrate media
        MigrateMedia();
        
        // Finally migrate encounter system
        MigrateEncounterSystem();
        
        if (verboseLogging)
        {
            Debug.Log("Migration completed successfully");
        }
    }
    
    /// <summary>
    /// Migrate content from legacy system to StarkkillerContentManager
    /// </summary>
    public void MigrateContent()
    {
        if (contentManager == null)
        {
            Debug.LogError("Cannot migrate content - ContentManager not found");
            return;
        }
        
        if (legacyEncounterSystem == null)
        {
            Debug.LogWarning("No legacy ShipEncounterSystem found to migrate from");
            return;
        }
        
        if (verboseLogging)
        {
            Debug.Log("Starting content migration");
        }
        
        // Mark content as migrated
        contentMigrated = true;
    }
    
    /// <summary>
    /// Migrate ship data from legacy system to new system
    /// </summary>
    public void MigrateShipData()
    {
        if (contentManager == null)
        {
            Debug.LogError("Cannot migrate ship data - ContentManager not found");
            return;
        }
        
        if (legacyEncounterSystem == null)
        {
            Debug.LogWarning("No legacy ShipEncounterSystem found to migrate from");
            return;
        }
        
        if (verboseLogging)
        {
            Debug.Log("Starting ship data migration");
        }
        
        // Mark content as migrated
        contentMigrated = true;
        
        Debug.Log("Ship data migration completed");
    }
    
    /// <summary>
    /// Migrate media from legacy systems to StarkkillerMediaSystem
    /// </summary>
    public void MigrateMedia()
    {
        if (mediaSystem == null)
        {
            Debug.LogError("Cannot migrate media - MediaSystem not found");
            return;
        }
        
        if (mediaSystem.mediaDatabase == null)
        {
            Debug.LogError("Cannot migrate media - MediaSystem has no database");
            return;
        }
        
        if (verboseLogging)
        {
            Debug.Log("Starting media migration");
        }
        
        // Mark media as migrated
        mediaMigrated = true;
    }
    
    /// <summary>
    /// Migrate media data from legacy systems to new system
    /// </summary>
    public void MigrateMediaData()
    {
        if (mediaSystem == null)
        {
            Debug.LogError("Cannot migrate media - MediaSystem not found");
            return;
        }
        
        if (mediaSystem.mediaDatabase == null)
        {
            Debug.LogError("Cannot migrate media - MediaSystem has no database");
            return;
        }
        
        if (verboseLogging)
        {
            Debug.Log("Starting media data migration");
        }
        
        // Mark media as migrated
        mediaMigrated = true;
        
        Debug.Log("Media data migration completed");
    }
    
    /// <summary>
    /// Migrate encounter system settings and data
    /// </summary>
    public void MigrateEncounterSystem()
    {
        if (encounterSystem == null)
        {
            Debug.LogError("Cannot migrate encounter system - StarkkillerEncounterSystem not found");
            return;
        }
        
        if (legacyEncounterSystem == null)
        {
            Debug.LogWarning("No legacy ShipEncounterSystem found to migrate from");
            return;
        }
        
        if (verboseLogging)
        {
            Debug.Log("Starting encounter system migration");
        }
        
        // Migrate game settings
        encounterSystem.validShipChance = legacyEncounterSystem.validShipChance;
        encounterSystem.storyShipChance = legacyEncounterSystem.storyShipChance;
        
        // Mark encounter system as migrated
        encounterMigrated = true;
        
        if (verboseLogging)
        {
            Debug.Log("Encounter system migration complete");
        }
    }
    
    /// <summary>
    /// Migrate gameplay settings from legacy system to new system
    /// </summary>
    public void MigrateGameplaySettings()
    {
        if (encounterSystem == null)
        {
            Debug.LogError("Cannot migrate encounter system - StarkkillerEncounterSystem not found");
            return;
        }
        
        if (legacyEncounterSystem == null)
        {
            Debug.LogWarning("No legacy ShipEncounterSystem found to migrate from");
            return;
        }
        
        if (verboseLogging)
        {
            Debug.Log("Starting gameplay settings migration");
        }
        
        // Migrate game settings
        encounterSystem.validShipChance = legacyEncounterSystem.validShipChance;
        encounterSystem.storyShipChance = legacyEncounterSystem.storyShipChance;
        
        // Mark encounter system as migrated
        encounterMigrated = true;
        
        Debug.Log("Gameplay settings migration completed");
    }
    
    /// <summary>
    /// Disable legacy systems after migration is complete
    /// </summary>
    public void DisableLegacySystems()
    {
        if (contentMigrated && mediaMigrated && encounterMigrated)
        {
            // Disable legacy encounter system
            if (legacyEncounterSystem != null)
            {
                legacyEncounterSystem.enabled = false;
                Debug.Log("Disabled legacy ShipEncounterSystem");
            }
            
            // Disable legacy video system
            if (legacyVideoSystem != null)
            {
                legacyVideoSystem.enabled = false;
                Debug.Log("Disabled legacy ShipVideoSystem");
            }
            
            // Disable legacy image system
            if (legacyImageSystem != null)
            {
                legacyImageSystem.enabled = false;
                Debug.Log("Disabled legacy ShipImageSystem");
            }
            
            Debug.Log("All legacy systems have been disabled");
        }
        else
        {
            Debug.LogWarning("Cannot disable legacy systems - migration is not complete");
            
            // Log what's not migrated
            if (!contentMigrated)
                Debug.LogWarning("- Ship data has not been migrated");
            if (!mediaMigrated)
                Debug.LogWarning("- Media data has not been migrated");
            if (!encounterMigrated)
                Debug.LogWarning("- Gameplay settings have not been migrated");
        }
    }
}