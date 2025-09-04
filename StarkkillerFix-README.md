# Starkiller Base Command - System Fix Guide

This guide will help you resolve the issues with missing scripts and non-working buttons in the Starkiller Base Command game.

## Issue Summary

The game is experiencing the following issues:
1. Missing scripts on the EncounterSystemManager and StarkkillerEncounterSystem prefabs
2. Buttons not working due to missing connections between systems
3. Ship countdown is stuck in a loop without progressing

## Quick Fix for Button Functionality

For an immediate temporary fix to get the buttons working:

1. Create a new empty GameObject in your scene
2. Add the `ButtonFixHelper` component to it
3. If auto-find is enabled, it will attempt to connect to all required components
4. Otherwise, manually assign these components in the inspector:
   - CredentialChecker reference
   - MasterShipGenerator reference
   - Button references (Approve, Deny, Holding Pattern, Tractor Beam, Accept Bribe)
5. Press Play to test if the buttons now work

## Complete System Fix (Editor)

For a proper fix that resolves the underlying issues:

1. Open Unity Editor and the project
2. In the top menu, you will see "Starkiller" menu has been added
3. Select "Starkiller > Setup > Fix Missing Script References"
   - This will automatically fix the missing script references on the prefabs
4. Alternatively, you can recreate each prefab individually:
   - Select "Starkiller > Setup > Create All Encounter Systems"
   - Or create them one by one:
     - "Starkiller > Setup > Create EncounterSystemManager"
     - "Starkiller > Setup > Create StarkkillerEncounterSystem"
     - "Starkiller > Setup > Create ShipEncounterSystem"

## Scene Setup

After fixing the prefabs, you need to set up your scene correctly:

1. Delete any existing EncounterSystemManager, StarkkillerEncounterSystem, or ShipEncounterSystem objects that have missing scripts
2. Drag the new prefabs from _Core/Prefabs folder into your scene:
   - EncounterSystemManager prefab
   - MasterShipGenerator prefab (if not already present)
3. Ensure the GameManagers object contains all the necessary manager components
4. Check the CredentialChecker component's references to ensure they are not missing

## About the Fixes

The fixes include:

1. **Script Recreations**:
   - Recreated the missing StarkkillerEncounterSystem and ShipEncounterSystem scripts
   - Fixed their implementation to properly connect with MasterShipGenerator

2. **Editor Tools**:
   - Added editor tools to automatically create and fix prefabs
   - These tools verify the integrity of the prefabs and recreate them if needed

3. **Runtime Fix Helper**:
   - Created ButtonFixHelper to allow for runtime recovery
   - This component can reconnect buttons to their handlers even if the system references are broken

## Identifying the Root Cause

The root cause of the issues was:

1. Missing script definition files for bridge classes that connect legacy systems to the new MasterShipGenerator
2. Improper handling of the reference to MasterShipGenerator in StarkkillerEncounterSystem (trying to directly access a private field)
3. Multiple systems active simultaneously causing conflicts and reference issues

## Verification

To verify the fix is working:

1. Ship buttons should respond to clicks
2. Ships should appear and be processed correctly
3. The game countdown should progress normally, not get stuck
4. No missing script errors should appear in the console

## Need Additional Help?

If you encounter any issues or need further assistance, please refer to the Implementation Guide for more detailed instructions on setting up the ship system architecture.