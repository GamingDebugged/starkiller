# Difficulty Profile System

## Overview
The Difficulty Profile system allows you to create flexible, day-based difficulty configurations for your game. Each day can have different time limits, quotas, multipliers, and special events.

## Components

### 1. DifficultyProfile ScriptableObject
- **Location**: `Assets/_RefactoredScripts/Core/Configuration/DifficultyProfile.cs`
- **Purpose**: Defines difficulty settings for each day
- **Creation**: Right-click in Project → Create → Starkiller → Difficulty Profile

### 2. ShiftTimerManager Integration
- **Location**: `Assets/_RefactoredScripts/Core/Managers/ShiftTimerManager.cs`
- **Purpose**: Applies difficulty settings automatically based on the current day
- **Configuration**: Assign a DifficultyProfile in the inspector

## Creating a Difficulty Profile

### In Unity Editor:
1. Right-click in Project window
2. Select "Create → Starkiller → Difficulty Profile"
3. Name your profile (e.g., "EasyDifficulty", "HardDifficulty")
4. Configure the settings in the inspector

### Day Settings Configuration:
Each day can have the following settings:

#### Timer Settings:
- **Shift Time Limit**: Time in seconds for each shift
- **Bonus Time Multiplier**: Multiplier for bonus time earned
- **Max Bonus Time**: Maximum bonus time allowed
- **Enable Warnings**: Whether to show time warnings
- **Warning Times**: Array of times when warnings appear

#### Gameplay Settings:
- **Ship Quota**: Number of ships to process
- **Credit Multiplier**: Multiplier for credits earned
- **Strike Penalty Multiplier**: Multiplier for strike penalties

#### Special Features:
- **Special Event**: Name of special event for this day
- **Day Description**: Description of what makes this day unique

## Usage

### 1. Assign to ShiftTimerManager:
```csharp
// In Unity Inspector
ShiftTimerManager.difficultyProfile = yourDifficultyProfile;
ShiftTimerManager.useDifficultyProfile = true;
```

### 2. Programmatic Access:
```csharp
// Get current day settings
var daySettings = shiftTimerManager.GetCurrentDaySettings();

// Get time limit for specific day
float timeLimit = shiftTimerManager.GetTimeLimitForDay(5);

// Change difficulty profile at runtime
shiftTimerManager.SetDifficultyProfile(newProfile);
```

## Pre-made Profiles

### Standard Difficulty Profile
- **File**: `StandardDifficultyProfile.asset`
- **Description**: Progressive difficulty from 30s to 8s over 10 days
- **Features**: 
  - Time limits: 30s → 8s
  - Ship quotas: 10 → 28
  - Credit multipliers: 1.0x → 1.9x
  - Special events on days 5 and 10

### Easy Difficulty Profile
- **File**: `EasyDifficultyProfile.asset`
- **Description**: Relaxed difficulty with generous time limits
- **Features**:
  - Time limits: 45s → 35s
  - Ship quotas: 8 → 12
  - Higher bonus multipliers
  - Lower strike penalties

## Advanced Features

### Fallback System:
- If a day isn't explicitly configured, the system uses the last configured day
- If no configurations exist, it uses the default settings

### Context Menu Testing:
Right-click on ShiftTimerManager component for debug options:
- "Show Current Day Settings"
- "Test: Apply Day 1 Settings"
- "Test: Apply Day 5 Settings"
- "Test: Apply Day 10 Settings"

### Validation:
The system automatically validates configurations and logs warnings for:
- Invalid time limits
- Invalid ship quotas
- Duplicate day numbers
- Missing configurations

## Integration with Other Systems

### DayProgressionManager:
- Automatically applies settings when a new day starts
- Integrates with day progression events

### Credit/Strike Systems:
- Other managers can read multipliers from current day settings
- Bonus time calculations automatically use day-specific multipliers

### UI Systems:
- Ship quotas can be displayed in UI
- Special events can trigger UI notifications
- Time warnings adapt to day-specific settings

## Creating Custom Profiles

### For Different Game Modes:
1. **Arcade Mode**: Very short time limits, high multipliers
2. **Relaxed Mode**: Long time limits, forgiving penalties
3. **Progressive Mode**: Gradual difficulty increase
4. **Challenge Mode**: Extreme difficulty with unique mechanics

### Profile Naming Convention:
- Use descriptive names: "Standard", "Easy", "Hard", "Arcade"
- Include version numbers for iterations: "Standard_v2"
- Use prefixes for categories: "Campaign_Standard", "Endless_Hard"

## Best Practices

1. **Start Simple**: Begin with 5-10 day configurations
2. **Test Thoroughly**: Use context menu options to test different days
3. **Player Feedback**: Adjust based on player performance data
4. **Backup Profiles**: Keep copies of working configurations
5. **Document Changes**: Use day descriptions to note what makes each day unique

## Troubleshooting

### Common Issues:
- **No settings applied**: Check that `useDifficultyProfile` is enabled
- **Wrong time limits**: Verify day numbers in configuration
- **Missing profile**: Assign a profile in the inspector
- **Validation errors**: Check console for specific issues

### Debug Logging:
Enable `logDifficultyChanges` in ShiftTimerManager to see detailed logs of when settings are applied.