# Starkiller Base Command - Manager Hierarchy Setup Guide

This guide explains how to fix the `DontDestroyOnLoad` issues and properly structure your GameObject hierarchy.

## Step 1: Create the Managers GameObject

1. Open your main scene (most likely `MainGame.unity` in Assets/_scripts/_Scenes/)
2. Create a new **empty GameObject** at the root level named `Managers`
3. Add the `ManagerInitializer` component to this GameObject 
   - This new script will automatically handle DontDestroyOnLoad for all child managers

## Step 2: Organize Your Manager Scripts

1. Move the following objects as children of the `Managers` GameObject:
   - `DebugMonitor`
   - `MasterShipGenerator`
   - `EncounterSystemMigrationManager`
   - `EncounterSystemManager` (if present in your scene)

2. **Important**: Make sure these objects are **direct children** of the `Managers` GameObject:
   ```
   - Managers (with ManagerInitializer script)
     |- DebugMonitor
     |- MasterShipGenerator
     |- EncounterSystemMigrationManager
     |- EncounterSystemManager (if present)
   ```

## Step 3: Configure the ManagerInitializer

1. In the Inspector for the `Managers` GameObject, set references in the ManagerInitializer:
   - `Debug Monitor`: Reference to your DebugMonitor
   - `Master Ship Generator`: Reference to your MasterShipGenerator
   - `Migration Manager`: Reference to your EncounterSystemMigrationManager
   - Check the `Init On Awake` option
   - Check the `Verbose` option (for debugging)

## Step 4: Fix CredentialChecker References

1. Find the `CredentialChecker` GameObject in your scene 
   - It's likely inside a UI Canvas
2. In the Inspector, check the `Master Ship Generator` field:
   - Make sure it references the MasterShipGenerator you just organized
   - If it's empty, drag your MasterShipGenerator object to this field

## Step 5: Verify Structure At Runtime

1. Enter Play mode in the Unity Editor
2. Open the Console window to check for DontDestroyOnLoad warnings
3. Look for log messages from the ManagerInitializer confirming that your managers are properly organized
4. No "Not a root GameObject, cannot use DontDestroyOnLoad" messages should appear

## Troubleshooting

### Still seeing DontDestroyOnLoad warnings?

- Check the parent-child relationships in your hierarchy
- Make sure all manager scripts are direct children of the `Managers` GameObject
- Verify that the ManagerInitializer component is on the root `Managers` GameObject

### Missing References?

- Make sure you've properly assigned all references in the ManagerInitializer
- Check that CredentialChecker correctly references MasterShipGenerator
- Use the `Find And Organize Managers` button on the ManagerInitializer if you see missing references

### EncounterSystem errors?

- Make sure the EncounterSystemMigrationManager is properly referenced and can find the MasterShipGenerator
- Check that bridge classes are being created as expected

## Understanding The Fix

The new hierarchy design fixes DontDestroyOnLoad issues by:

1. Creating a single root object (`Managers`) that calls DontDestroyOnLoad
2. Making all manager scripts children of this object
3. Removing direct DontDestroyOnLoad calls from individual scripts
4. Properly handling references between systems

This approach follows Unity best practices where only root GameObjects should use DontDestroyOnLoad, not their children.