# Refactored Scripts Directory

This directory contains the cleaned-up, properly organized scripts for Starkiller Base Command.

## Migration Strategy

We're using a parallel development approach:
1. Old scripts remain in their original locations (working)
2. New refactored scripts are created here
3. Systems are gradually migrated to use new scripts
4. Old scripts are deprecated only after new ones are fully tested

## Directory Structure

```
_RefactoredScripts/
├── Core/
│   ├── Managers/           # Core game managers (singletons)
│   ├── Systems/           # Game systems (encounter, decision, etc.)
│   └── Data/             # Data structures and ScriptableObjects
├── Gameplay/
│   ├── Encounters/       # Encounter generation and management
│   ├── Decisions/        # Decision processing and consequences
│   ├── Ships/           # Ship-related logic
│   └── Captains/        # Captain-related logic
├── UI/
│   ├── Panels/          # UI panel controllers
│   ├── Elements/        # Individual UI elements
│   └── Utilities/       # UI helper scripts
├── Services/            # Service layer (API, persistence, etc.)
└── Utilities/          # Helper classes and extensions
```

## Migration Status

- [ ] GameManager → Split into focused managers
- [ ] MasterShipGenerator → Cleaned and refactored
- [ ] CredentialChecker → Separated UI from logic
- [ ] Captain ID Card → Migrated and tested

## Guidelines

1. **Namespace everything**: `namespace Starkiller.Core.Managers`
2. **Use interfaces**: Define contracts for all systems
3. **Event-driven**: Use events instead of direct references
4. **Dependency injection**: Use ServiceLocator pattern
5. **Single responsibility**: Each class does ONE thing well