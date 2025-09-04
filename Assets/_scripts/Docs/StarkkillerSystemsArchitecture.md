# Starkiller Base Command - New Architecture

This document outlines the new consolidated architecture for the Starkiller Base Command game.

## Overview

The new architecture consolidates multiple overlapping systems into a clean, modular set of components:

1. **StarkkillerContentManager**: Central repository for all game content, including ships, captains, scenarios, and game rules.
2. **StarkkillerMediaDatabase**: ScriptableObject that stores all media assets organized by type.
3. **StarkkillerMediaSystem**: Runtime component that provides a unified interface for accessing all visual assets.
4. **StarkkillerEncounterSystem**: Core gameplay system that generates and processes ship encounters.
5. **LegacySystemsMigrator**: Utility to help transition from the old architecture.

## Core Principles

This architecture is designed around several key principles:

- **Separation of Data and Logic**: Data is stored in ScriptableObjects, while behavior is in MonoBehaviours.
- **Single Responsibility**: Each system has a clear, focused role and minimal dependencies.
- **Dependency Injection**: Systems reference each other through explicit connections rather than using globals.
- **Clean APIs**: Each system provides clear, well-documented methods for external use.
- **Migration Support**: The architecture supports a smooth transition from legacy systems.

## Component Relationships

```
┌────────────────────┐     ┌────────────────────┐     ┌────────────────────┐
│                    │     │                    │     │                    │
│ ContentManager     ├────►│ EncounterSystem    │◄────┤ MediaSystem        │
│ (Game content)     │     │ (Core gameplay)    │     │ (Visual resources) │
│                    │     │                    │     │                    │
└────────────────────┘     └────────────────────┘     └─────────┬──────────┘
                                                               │
                                                               │
                                                     ┌─────────▼──────────┐
                                                     │                    │
                                                     │ MediaDatabase      │
                                                     │ (Asset storage)    │
                                                     │                    │
                                                     └────────────────────┘
```

## Setup Instructions

### 1. Using the Editor Menu

The simplest way to set up the new architecture is to use the included editor tools:

1. Go to `Starkiller > Create Systems` in the Unity menu.
2. This will create all necessary components and connect them.
3. If needed, create a media database with `Starkiller > Create Media Database`.
4. For testing, create a test scene with `Starkiller > Create Test Scene`.

### 2. Manual Setup

If you prefer to set up the systems manually:

1. Create an empty GameObject named "StarkkillerSystems".
2. Add child GameObjects for ContentManager, MediaSystem, and EncounterSystem.
3. Add the respective component scripts to each GameObject.
4. Create a StarkkillerMediaDatabase ScriptableObject in your Resources folder.
5. Assign references between the systems in the Inspector.

## Migration from Legacy Systems

The LegacySystemsMigrator component helps transition from the old architecture:

1. Ensure all systems are set up (using the editor menu or manually).
2. Use the migrator's `FindLegacySystems()` method to locate old systems.
3. Call `MigrateAll()` to transfer all data and settings to the new architecture.
4. Optionally enable `disableLegacySystemsAfterMigration` to automatically disable old systems.

## Key Components in Detail

### StarkkillerContentManager

This is the central manager for all game content:

- **Content:** Ships, captains, scenarios, consequences
- **Game State:** Access codes, current day, imperial loyalty
- **Game Rules:** Daily rules, validity checks

```csharp
// Example: Get a random ship type
ShipType randomShip = StarkkillerContentManager.Instance.GetRandomShipType();

// Example: Verify an access code
bool isValid = StarkkillerContentManager.Instance.IsAccessCodeValid("SK-1234");
```

### StarkkillerMediaDatabase

This ScriptableObject stores all visual assets:

- **Ship Media:** Images and videos organized by ship type
- **Captain Media:** Portraits and videos organized by faction
- **Scenario Media:** Images and videos for special events
- **UI Elements:** Standard UI icons and images

### StarkkillerMediaSystem

This provides runtime access to all visual assets:

- **Ship Visuals:** Static images and videos of ships
- **Captain Visuals:** Portraits and videos of captains
- **Enhancement:** Adds visual assets to encounters

```csharp
// Example: Get a ship image
Sprite shipImage = StarkkillerMediaSystem.Instance.GetShipImage("Imperial Shuttle");

// Example: Enhance an encounter with media
EnhancedShipEncounter enhanced = 
    StarkkillerMediaSystem.Instance.EnhanceEncounterWithMedia(baseEncounter);
```

### StarkkillerEncounterSystem

This manages the core gameplay loop:

- **Generation:** Creates random and story-based encounters
- **Processing:** Handles player decisions and consequences
- **State:** Tracks game progress and statistics

```csharp
// Example: Get the next encounter
ShipEncounter encounter = StarkkillerEncounterSystem.Instance.GetNextEncounter();

// Example: Process a player decision
StarkkillerEncounterSystem.Instance.ProcessDecision(true); // Approve ship
```

## Best Practices

1. **Adding New Content:**
   - Create ScriptableObjects for new ship types, captain types, etc.
   - Add them to the ContentManager's respective lists.

2. **Adding New Media:**
   - Add sprites and videos to the MediaDatabase.
   - Organize them by type (ship, captain, scenario).

3. **Extending Functionality:**
   - Add new methods to the appropriate system.
   - Maintain clear separation of concerns between systems.

4. **UI Integration:**
   - Use EnhancedShipEncounter and VideoEnhancedShipEncounter for UI display.
   - Let the MediaSystem handle all content enhancement.

## Testing and Validation

The included StarkkillerTestScene component provides a simple way to test the new architecture:

1. Create a test scene using the editor menu.
2. Add UI elements in the Inspector.
3. Run the test to see encounters being generated and processed through the new systems.

## Troubleshooting

Common issues and solutions:

- **Missing Media:** Check that the MediaDatabase is properly populated and assigned to the MediaSystem.
- **No Encounters Generated:** Verify ContentManager is initialized with ship types and scenarios.
- **Systems Not Connected:** Ensure all system references are properly assigned in the Inspector.
- **Migration Errors:** Check console for specific error messages during migration.

## Next Steps

After implementing this architecture:

1. Complete the migration of all existing data.
2. Test thoroughly with various encounter types.
3. Replace direct references to legacy systems in UI scripts.
4. Consider adding events/callbacks for better cross-system communication.