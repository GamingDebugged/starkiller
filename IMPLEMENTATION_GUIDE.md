# Starkiller Base Command - Implementation Guide

This guide outlines the changes made to fix DontDestroyOnLoad and reference issues in the game.

## Files Modified

1. **MasterShipGenerator.cs**
   - Fixed DontDestroyOnLoad to check for a root GameObject or ManagerInitializer parent
   - Added proper detection of parent-child hierarchy

2. **DebugMonitor.cs**
   - Added proper DontDestroyOnLoad handling with parent GameObject checking
   - Will persist itself only if it's a root GameObject or is managed by ManagerInitializer

3. **EncounterSystemMigrationManager.cs**
   - Added missing methods: FindSystemReferences() and LogDetectedSystems()
   - Added proper DontDestroyOnLoad handling with parent GameObject checking
   - Fixed missing reference errors

## New Files Created

1. **ManagerInitializer.cs**
   - New script that handles all manager persistence
   - Organizes managers into a proper hierarchy
   - Ensures managers are properly connected to each other

2. **STARKILLER_HIERARCHY_SETUP.md**
   - Detailed guide for setting up the GameObject hierarchy
   - Explains how to organize managers under a single parent

3. **GameManagers.prefab.txt**
   - Template prefab (saved as text) to import into Unity
   - Pre-configured with the correct hierarchy and components

## Implementation Instructions

### 1. First, import the new script:

1. Open the ManagerInitializer.cs script in your project
2. Compile the project to ensure the new script is recognized

### 2. Set up the hierarchy:

1. Create a new empty GameObject named "Managers" at the root level of your scene
2. Add the ManagerInitializer component to it
3. Move these GameObjects to be children of Managers:
   - DebugMonitor
   - MasterShipGenerator
   - EncounterSystemMigrationManager
   - Any other relevant manager scripts

### 3. Configure references:

1. In the ManagerInitializer component on the Managers GameObject:
   - Assign the DebugMonitor reference
   - Assign the MasterShipGenerator reference
   - Assign the EncounterSystemMigrationManager reference
   - Check the "Init On Awake" option
   - Check the "Verbose" option

### 4. Update the CredentialChecker:

1. Find the CredentialChecker in your scene
2. In its Inspector, ensure the MasterShipGenerator reference is set to your reorganized manager

### 5. Test the implementation:

1. Play the scene and check the console for any DontDestroyOnLoad warnings
2. Verify that the ManagerInitializer is logging messages about finding and organizing managers
3. Check that CredentialChecker can find the MasterShipGenerator

## Alternative: Use the Prefab

If you prefer, you can use the provided GameManagers.prefab.txt:
1. Rename it to GameManagers.prefab
2. Import it into your Unity project
3. Drag it into your scene
4. Configure references as needed

## Troubleshooting

If you still encounter issues:

1. **Missing references:** Use the FindAndOrganizeManagers function in ManagerInitializer
2. **DontDestroyOnLoad warnings:** Ensure all managers are direct children of the Managers GameObject
3. **EncounterSystemManager errors:** Ensure the systemManager field is assigned in EncounterSystemMigrationManager

### Specific Error Solutions

#### Missing 'ManagerInitializer' Type:

If you get errors like:
```
Assets/DebugMonitor.cs(136,13): error CS0246: The type or namespace name 'ManagerInitializer' could not be found
```

Make sure you've:
1. Added `using StarkillerBaseCommand;` to the top of any script that uses ManagerInitializer
2. The ManagerInitializer.cs script is properly within the StarkillerBaseCommand namespace
3. The script has been compiled at least once in Unity

#### CS0414: Field is assigned but never used:

For warnings like:
```
Warning CS0414: The field 'EncounterSystemCoordinator.isInitialized' is assigned but its value is never used
```

Add a method that uses the field:
```csharp
public bool IsInitialized()
{
    return isInitialized;
}
```
This makes the field useful and silences the warning.

By following this implementation guide, your Starkiller Base Command project should now have properly organized managers with correct persistence between scenes.