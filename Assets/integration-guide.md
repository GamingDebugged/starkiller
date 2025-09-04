# Starkiller Base Command - New Ship Encounter System

This document explains how to integrate the new unified ship encounter system in your project.

## Overview

The new ship encounter system consolidates multiple classes into a single, comprehensive structure:

- `MasterShipEncounter.cs` - A unified class containing all ship encounter data and media
- `MasterShipGenerator.cs` - A generator that creates ship encounters using ScriptableObjects
- Updated `CredentialChecker.cs` - UI controller modified to work with the new system

## Why This New System?

The old system used inheritance to add features to encounters:
- `ShipEncounter` (base class)
- `EnhancedShipEncounter` (added images)
- `VideoEnhancedShipEncounter` (added videos)

This led to complex type conversions and scattered functionality. The new system:

- Consolidates all data and media in a single class
- Treats videos as core features, not optional extras
- Provides a cleaner API for working with encounters
- Reduces dependencies and improves maintainability

## Integration Steps

### 1. Add the New Scripts

Add these scripts to your project:
- `MasterShipEncounter.cs`
- `MasterShipGenerator.cs`
- Updated `CredentialChecker.cs`

### 2. Create a GameObject with the Generator

Create a new GameObject in your scene named "ShipEncounterSystem" and add:
- `MasterShipGenerator` component

### 3. Configure the Generator

In the Inspector, assign:
- Reference to `StarkkillerContentManager`
- Reference to `StarkkillerMediaSystem`
- (Optional) Lists of ShipTypes, CaptainTypes, and ShipScenarios if not using ContentManager

### 4. Update UI Components

- Replace your existing `CredentialChecker` component with the updated version
- Assign all necessary UI references in the Inspector

### 5. Remove Old Scripts (After Testing)

Once the new system is working correctly, you can remove:
- `ShipEncounter.cs`
- `EnhancedShipEncounter.cs`
- `VideoEnhancedShipEncounter.cs`
- `ShipEncounterGenerator.cs` (if used)
- `ShipEncounterSystem.cs` (if used)

## How It Works

1. **Encounter Creation**:
   - `MasterShipGenerator` creates `MasterShipEncounter` objects
   - Encounters are pre-populated with data from ScriptableObjects
   - Videos and images are added directly to the encounter object

2. **UI Display**:
   - `CredentialChecker` displays encounter details in the UI
   - Shows ship/captain videos when available
   - Falls back to images when videos aren't available

3. **Decision Processing**:
   - When player approves/denies a ship, decision is sent to the generator
   - Generator applies consequences if needed

## Additional Features

The new system includes:
- Proper event handling for encounter ready notifications
- Built-in validation methods for checking credentials
- StringBuilder optimization for string formatting
- Direct access to all media types in a single object
- Comprehensive documentation in code

## Customization

You can extend `MasterShipEncounter` with additional properties or methods as needed. The class is designed to be comprehensive but also extensible for future features.

## Troubleshooting

- **Videos not playing**: Ensure StarkkillerMediaDatabase is properly assigned to StarkkillerMediaSystem
- **UI not updating**: Check that CredentialChecker has all required references 
- **No encounters generated**: Verify ContentManager has shipTypes, captainTypes, scenarios populated
- **Validation errors**: Check that ContentManager has proper rule definitions
