using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Coordinates between different ship generation systems to ensure compatibility
/// </summary>
public class ShipGeneratorCoordinator : MonoBehaviour
{
    [Header("System References")]
    public ShipEncounterSystem legacySystem;
    public EnhancedShipGenerator enhancedGenerator;
    public ShipEncounterGenerator newGenerator;
    public ShipVideoSystem videoSystem;  // Added video system reference
    public CredentialChecker credentialChecker;
    
    [Header("Generation Settings")]
    [Tooltip("Which generation system to use")]
    public GenerationMode activeGenerationMode = GenerationMode.Enhanced;
    
    [Header("Enhancement Settings")]
    [Tooltip("Whether to enhance ships with videos when available")]
    public bool enhanceWithVideo = true;
    
    public enum GenerationMode
    {
        Legacy,     // Use original ShipEncounterSystem
        Enhanced,   // Use EnhancedShipGenerator
        New         // Use new ShipEncounterGenerator
    }
    
    // Singleton pattern for easy access
    private static ShipGeneratorCoordinator _instance;
    public static ShipGeneratorCoordinator Instance
    {
        get { return _instance; }
    }
    
    void Awake()
    {
        // Setup singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Find references if not assigned
        if (legacySystem == null)
            legacySystem = FindFirstObjectByType<ShipEncounterSystem>();
            
        if (enhancedGenerator == null)
            enhancedGenerator = FindFirstObjectByType<EnhancedShipGenerator>();
            
        if (newGenerator == null)
            newGenerator = FindFirstObjectByType<ShipEncounterGenerator>();
            
        if (videoSystem == null)
            videoSystem = FindFirstObjectByType<ShipVideoSystem>();
            
        if (credentialChecker == null)
            credentialChecker = FindFirstObjectByType<CredentialChecker>();
            
        // Log which systems are available
        LogAvailableSystems();
    }
    
    void LogAvailableSystems()
    {
        string message = "ShipGeneratorCoordinator - Available systems: ";
        message += legacySystem != null ? "Legacy System ✓ " : "Legacy System ✗ ";
        message += enhancedGenerator != null ? "Enhanced Generator ✓ " : "Enhanced Generator ✗ ";
        message += newGenerator != null ? "New Generator ✓ " : "New Generator ✗ ";
        message += videoSystem != null ? "Video System ✓ " : "Video System ✗ ";
        
        Debug.Log(message);
        
        // Warn if selected mode isn't available
        if ((activeGenerationMode == GenerationMode.Legacy && legacySystem == null) ||
            (activeGenerationMode == GenerationMode.Enhanced && enhancedGenerator == null) ||
            (activeGenerationMode == GenerationMode.New && newGenerator == null))
        {
            Debug.LogWarning("Selected generation mode isn't available! Falling back to available system.");
        }
    }
    
    /// <summary>
    /// Generate a ship encounter using the active generation system
    /// </summary>
    public ShipEncounter GenerateShipEncounter(int currentDay = 1, int imperialLoyalty = 0, int rebellionSympathy = 0)
    {
        ShipEncounter encounter = null;
        
        switch (activeGenerationMode)
        {
            case GenerationMode.Legacy:
                if (legacySystem != null)
                    encounter = legacySystem.GenerateEncounter(currentDay, imperialLoyalty, rebellionSympathy);
                break;
                
            case GenerationMode.Enhanced:
                if (enhancedGenerator != null)
                    encounter = enhancedGenerator.GenerateEnhancedEncounter(currentDay, imperialLoyalty, rebellionSympathy);
                break;
                
            case GenerationMode.New:
                if (newGenerator != null)
                    encounter = newGenerator.GenerateRandomEncounter();
                break;
        }
        
        // Fallback if the selected mode failed
        if (encounter == null)
        {
            encounter = FallbackGeneration(currentDay, imperialLoyalty, rebellionSympathy);
        }
        
        // Enhance with video if enabled and available
        if (enhanceWithVideo && videoSystem != null)
        {
            encounter = videoSystem.EnhanceEncounterWithVideo(encounter);
        }
        
        return encounter;
    }
    
    /// <summary>
    /// Generate a valid test ship for debugging
    /// </summary>
    public ShipEncounter CreateTestShip()
    {
        ShipEncounter encounter = null;
        
        switch (activeGenerationMode)
        {
            case GenerationMode.Legacy:
                if (legacySystem != null)
                    encounter = legacySystem.CreateTestShip();
                break;
                
            case GenerationMode.Enhanced:
                if (enhancedGenerator != null)
                    encounter = enhancedGenerator.CreateTestEnhancedShip(true);
                break;
                
            case GenerationMode.New:
                if (newGenerator != null)
                    encounter = newGenerator.GenerateRandomEncounter(true);
                break;
        }
        
        // Fallback if the selected mode failed
        if (encounter == null)
        {
            encounter = FallbackTestShip();
        }
        
        // Enhance with video if enabled and available
        if (enhanceWithVideo && videoSystem != null)
        {
            encounter = videoSystem.EnhanceEncounterWithVideo(encounter);
        }
        
        return encounter;
    }
    
    /// <summary>
    /// Generate an invalid test ship for debugging
    /// </summary>
    public ShipEncounter CreateInvalidTestShip()
    {
        ShipEncounter ship = CreateTestShip();
        
        // Make the ship invalid
        ship.accessCode = "XX-1234";
        ship.shouldApprove = false;
        ship.invalidReason = "Invalid access code";
        
        return ship;
    }
    
    /// <summary>
    /// Display a test ship in the credential checker
    /// </summary>
    public void DisplayTestShip(bool valid = true)
    {
        if (credentialChecker == null)
        {
            Debug.LogError("CredentialChecker reference not set!");
            return;
        }
        
        ShipEncounter ship = valid ? CreateTestShip() : CreateInvalidTestShip();
        credentialChecker.DisplayEncounter(ship);
    }
    
    /// <summary>
    /// Fallback ship generation if the selected mode fails
    /// </summary>
    private ShipEncounter FallbackGeneration(int currentDay, int imperialLoyalty, int rebellionSympathy)
    {
        // Try each system in order of preference
        if (enhancedGenerator != null)
            return enhancedGenerator.GenerateEnhancedEncounter(currentDay, imperialLoyalty, rebellionSympathy);
            
        if (legacySystem != null)
            return legacySystem.GenerateEncounter(currentDay, imperialLoyalty, rebellionSympathy);
            
        if (newGenerator != null)
            return newGenerator.GenerateRandomEncounter();
            
        // If all else fails, create a basic ship manually
        return CreateBasicShip();
    }
    
    /// <summary>
    /// Fallback test ship creation if the selected mode fails
    /// </summary>
    private ShipEncounter FallbackTestShip()
    {
        // Try each system in order of preference
        if (enhancedGenerator != null)
            return enhancedGenerator.CreateTestEnhancedShip(true);
            
        if (legacySystem != null)
            return legacySystem.CreateTestShip();
            
        if (newGenerator != null)
            return newGenerator.GenerateRandomEncounter(true);
            
        // If all else fails, create a basic ship manually
        return CreateBasicShip();
    }
    
    /// <summary>
    /// Create a basic ship manually as a last resort
    /// </summary>
    private ShipEncounter CreateBasicShip()
    {
        ShipEncounter ship = new ShipEncounter();
        ship.shipType = "Orion Shuttle";
        ship.destination = "Imperium Base";
        ship.origin = "Stellar Destroyer Finalizer";
        ship.accessCode = "SK-7429";
        ship.story = "Ship requests routine maintenance.";
        ship.manifest = "Standard supplies and personnel.";
        ship.captainName = "Captain Fiett";
        ship.shouldApprove = true;
        return ship;
    }
}