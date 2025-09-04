# Starkiller Base Command: Encounter Media Manager Population Guide

## Overview
This guide provides comprehensive instructions for populating the EncounterMediaManager in the Starkiller Base Command project.

## Recommended Folder Structure
```
Assets/
└── _Starkiller/
    ├── Media/
    │   ├── Sprites/
    │   │   ├── Ships/
    │   │   │   ├── ImperialShuttle/
    │   │   │   ├── CargoFreighter/
    │   │   │   └── ScoutVessel/
    │   │   └── Captains/
    │   │       ├── Imperium/
    │   │       ├── Merchant/
    │   │       └── Rebel/
    │   └── Videos/
    │       ├── Ships/
    │       └── Captains/
    └── ScriptableObjects/
        └── EncounterMedia/
```

## Sprite Preparation

### Import Settings
Recommended sprite import settings:
- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- Read/Write Enabled: Checked
- Compression: High Quality

### Naming Conventions

#### Ship Sprites
Examples:
- `ImperialShuttle_Standard`
- `ImperialShuttle_Damaged`
- `CargoFreighter_Standard`
- `ScoutVessel_Approach`

#### Captain Portraits
Examples:
- `Imperium_Captain_Tarkin`
- `Imperium_Commander_Piett`
- `Merchant_Trader_Solo`

## Video Preparation

### Import Considerations
- Ensure videos are compatible with Unity's VideoClip
- Transcode to match target platforms
- Optimize for streaming if possible

### Naming Conventions

#### Ship Videos
Examples:
- `ImperialShuttle_Approach`
- `CargoFreighter_Idle`
- `ScoutVessel_Scanning`

#### Captain Videos
Examples:
- `Imperium_Greeting`
- `Merchant_Negotiation`
- `Rebel_Defiance`

## Media Categories Configuration

### Purpose
Media Categories allow sophisticated media selection through tags and grouping.

### Example Categories

#### Imperial Ships Category
- Name: "Imperial Ships"
- Sprites: 
  - ImperialShuttle_Standard
  - ImperialShuttle_Damaged
- Videos:
  - ImperialShuttle_Approach
- Tags: 
  - "Military"
  - "Imperium"
  - "Standard"

#### Merchant Ships Category
- Name: "Merchant Ships"
- Sprites:
  - CargoFreeger_Standard
  - CargoFreighter_Damaged
- Videos:
  - CargoFreighter_Idle
- Tags:
  - "Civilian"
  - "Trade"

## Fallback Assets
Set default assets for scenarios with no matching media:
- Default Ship Sprite
- Default Captain Portrait
- Default Ship Video
- Default Captain Video

## Configuration Settings

### Recommended Defaults
- Max Cached Media: 50
- Enable Dynamic Media Selection: ON
- Enforce Tag Consistency: Depends on media variety

## Best Practices

1. Use consistent, descriptive naming
2. Create multiple media variants for interesting variety
3. Organize media in a clear, logical structure
4. Consider performance - don't overload with high-res assets

## Moving Sprite Folders

### Preserving References
- Unity typically maintains references when moving asset folders
- Verify references after moving

### Safe Move Process
1. Use Unity's Project window to move the folder
2. Right-click on source folder
3. Select "Move"
4. Navigate to desired location

### Verification Steps
- Open ScriptableObjects using sprites
- Verify sprite references remain linked
- Manually re-link if necessary

## Troubleshooting

### Broken References
- If references break, manually re-link in the Inspector
- Check for hardcoded paths in custom scripts
- Verify third-party asset compatibility

### Performance Optimization
- Use appropriately sized sprites/videos
- Compress media where possible
- Limit total number of variants

## Support
For additional support, contact the project lead or refer to the project documentation.
```
