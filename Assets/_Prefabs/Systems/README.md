# Starkiller Base Command - Encounter System Manager

## Overview

This system manages the various ship encounter generators to avoid conflicts between multiple systems running simultaneously. It ensures that only one system is active at a time, properly connecting the active system to the CredentialChecker.

## Usage

1. Add the `EncounterSystemManager` prefab to your scene
2. Select which encounter system to use as the primary system:
   - `MasterShipGenerator` (recommended) - The unified encounter system
   - `LegacyEncounterSystem` - The original encounter system
   - `StarkkillerEncounterSystem` - The new Starkiller-specific system
   - `ShipGeneratorCoordinator` - The coordination system for multiple generators

## Troubleshooting

If you're experiencing issues with ship encounters:

1. Check Debug logs for EncounterSystemManager messages
2. Verify that only one encounter system is active
3. Ensure the CredentialChecker is connected to the active system
4. Try increasing the `initializationDelay` if there are timing issues

## Technical Details

- The manager uses delayed initialization to avoid race conditions during startup
- It disables all non-active systems to prevent conflicts
- It handles setting up proper event connections between systems
- It ensures only one system provides encounters to the credential checker

## Migration Notes

This system replaces the legacy `LegacySystemsMigrator` approach to disabling old systems. Instead of permanently disabling legacy systems, this manager coordinates which system is active, making it easier to switch between systems for testing.

If you encounter any issues, please set `verboseLogging` to true and check the console for detailed logs.