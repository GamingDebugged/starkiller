# Starkiller Base Command - Fix Instructions

## Overview of the Fix

This package contains fixes for the blue screen issue and decision button functionality in Starkiller Base Command. The primary issues were related to object persistence between scene transitions and reference management between key systems.

## How to Apply the Fix

1. **Add the BootstrapperManager**:
   - Create an empty GameObject in your starting scene (such as your main menu scene)
   - Add the `BootstrapperManager` component to this GameObject
   - Ensure all options are checked (createShipGeneratorManager, installMissingManagers, attachToSceneLoads)

2. **Add DontDestroy Component to MasterShipGenerator**:
   - Find your MasterShipGenerator GameObject in the hierarchy
   - Add the `MasterShipGeneratorDontDestroy` component to it
   - This ensures the MasterShipGenerator is properly marked for persistence across scenes

3. **Update Existing Components**:
   - Ensure you're using the updated versions of:
     - ShipTimingController.cs
     - MasterShipGenerator.cs (with the DontDestroyOnLoad changes)

4. **Add New Manager Components**:
   - ShipGeneratorManager.cs - Maintains references between systems
   - ShipGeneratorManagerInstaller.cs - Creates the manager at runtime if needed
   - BootstrapperManager.cs - Ensures all key systems are created and maintained
   - MasterShipGeneratorDontDestroy.cs - Helper to ensure persistence

## Troubleshooting Steps

If you continue to experience issues:

1. **Check Console for Errors**:
   - Look for any errors related to missing components or references

2. **Force Reference Sync**:
   - Find the BootstrapperManager in the hierarchy
   - Click the "Force Reference Sync" button in the Inspector
   - This will re-establish all connections between systems

3. **Reset Timing Controller**:
   - If buttons still don't work, find the ShipTimingController
   - Call its ResetCooldown() method to clear any stuck flags

4. **Check Object Persistence**:
   - Enable "DontDestroyOnLoad" scene view in the Unity editor (if available)
   - Verify that MasterShipGenerator, GameStateController, and ShipTimingController are present
   - Make sure they have properly established references to each other

## Technical Details

### Root Causes Fixed

1. **Missing DontDestroyOnLoad**: The MasterShipGenerator wasn't properly marked as DontDestroyOnLoad, causing it to be destroyed during scene transitions.

2. **Reference Loss**: Systems were losing references to each other when scenes changed, particularly the CredentialChecker losing its reference to MasterShipGenerator.

3. **Button Processing Locks**: Decision processing flags were getting stuck in a locked state, preventing buttons from working.

4. **State Management**: Game state transitions weren't properly ensuring that all systems were ready for the new state.

### Implementation Notes

- Created persistent managers with proper singleton patterns and DontDestroyOnLoad handling
- Improved reference acquisition with fallbacks and delayed initialization
- Implemented reference tracking through a central ShipGeneratorManager
- Added bootstrap system to ensure critical managers exist at all times
- Applied more robust error handling and recovery mechanisms