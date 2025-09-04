# Starkiller Base Command Implementation Guide

## Fixing Current Errors

### 1. Add Missing Methods to GameManager.cs

Add the following methods to the GameManager.cs script to fix the MoralChoiceManager.cs errors:

```csharp
// Check if the game is currently active
public bool IsGameActive()
{
    return gameActive;
}

// Check if the game is currently paused
public bool IsPaused()
{
    return isPaused;
}

// Pause the game
public void PauseGame()
{
    isPaused = true;
}

// Resume the game
public void ResumeGame()
{
    isPaused = false;
}

// Add credits to the player's balance
public void AddCredits(int amount)
{
    credits += amount;
    UpdateUI();
}

// Update the player's loyalty and sympathy values
public void UpdateLoyalty(int imperial, int rebellion)
{
    imperialLoyalty = imperial;
    rebellionSympathy = rebellion;
    
    // Clamp values to prevent extreme swings
    imperialLoyalty = Mathf.Clamp(imperialLoyalty, -10, 10);
    rebellionSympathy = Mathf.Clamp(rebellionSympathy, -10, 10);
}

// Add a key decision to the decision history
public void AddKeyDecision(string decision)
{
    if (!string.IsNullOrEmpty(decision))
    {
        keyDecisions.Add(decision);
    }
}
```

### 2. Add FamilyStatusInfo Class

Create a new script called FamilyStatusInfo.cs with the implementation provided. This class stores and reports family status information and is used by both FamilyManager and DailyReportManager.

## Implementing Tabbed LogBook Interface

### 1. Create LogBookManager Script

Create a new script called LogBookManager.cs to manage the tabbed log book interface. This script handles:
- Switching between tabs (Rules, Ship Types, Destinations)
- Generating content for each tab
- Opening and closing the log book

### 2. Update CredentialChecker

Modify the OpenLogBook and CloseLogBook methods in CredentialChecker.cs to work with the new LogBookManager:

```csharp
void OpenLogBook()
{
    // Find LogBookManager or use cached reference
    LogBookManager logBookManager = FindFirstObjectByType<LogBookManager>();
    
    // If we have a LogBookManager, use it
    if (logBookManager != null)
    {
        logBookManager.OpenLogBook();
    }
    // Otherwise, fall back to the old behavior
    else if (logBookPanel)
    {
        logBookPanel.SetActive(true);
        UpdateLogBook(); // Update the content
    }
}

void CloseLogBook()
{
    // Find LogBookManager or use cached reference
    LogBookManager logBookManager = FindFirstObjectByType<LogBookManager>();
    
    // If we have a LogBookManager, use it
    if (logBookManager != null)
    {
        logBookManager.CloseLogBook();
    }
    // Otherwise, fall back to the old behavior
    else if (logBookPanel)
    {
        logBookPanel.SetActive(false);
    }
}
```

## Unity Scene Setup

### 1. LogBook UI Setup

1. Create a "LogBookPanel" GameObject in your Canvas
2. Add three tab buttons at the top: "Rules", "Ship Types", "Destinations"
3. Add visual indicators for active tabs (can be colored bars under buttons)
4. Add a content area (Text - TextMeshPro)
5. Add a close button
6. Attach the LogBookManager script to the panel
7. Assign references in the Inspector

### 2. Integration with Existing Components

1. Adjust the UI navigation buttons to find the LogBookManager
2. Ensure any other scripts that reference the log book work with the new system
3. Update the existing log book panel to match the visual style of the new tabbed interface

## Testing Steps

1. Test the GameManager methods by triggering moral choice events
2. Test the LogBook tab navigation by clicking the different tab buttons
3. Check that all content displays correctly in each tab
4. Verify that the log book opens and closes properly
5. Ensure there are no console errors during gameplay

## Visual Polish

1. Add color coding for rules and access codes (e.g., valid items in green)
2. Add First Order styling to the log book (dark colors, angular design)
3. Consider adding small icons next to ship types
4. Include a clear visual indicator of which tab is currently active

## Next Steps After Fixing These Issues

1. Complete any remaining UI polish
2. Implement sound effects for UI interactions
3. Add visual effects for approvals and denials
4. Balance the gameplay economy and difficulty progression 
5. Add more varied ship encounters and moral choices
