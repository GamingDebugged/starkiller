# Start Shift Button Setup Guide

## Quick Fix for "Shift Already Active" Issue

The shift is starting automatically because the `DayProgressionManager` auto-starts shifts when entering Gameplay state. Here's how to fix it:

### Option 1: Disable Auto-Start in Inspector (Recommended)
1. Find the `DayProgressionManager` GameObject in your scene
2. In the Inspector, expand the "Shift Control" section
3. **Uncheck** `Auto Start Shift On Gameplay`
4. **Check** `Require Manual Shift Start`
5. Save the scene

### Option 2: Modify the Component via Code
Add this to your initialization code:
```csharp
var dayManager = ServiceLocator.Get<DayProgressionManager>();
if (dayManager != null)
{
    // Disable auto-start
    dayManager.SetFieldValue("autoStartShiftOnGameplay", false);
    dayManager.SetFieldValue("requireManualShiftStart", true);
}
```

### Option 3: Scene Setup Script
Create this helper script and run it once:
```csharp
[ContextMenu("Setup Manual Shift Start")]
private void SetupManualShiftStart()
{
    var dayManager = FindObjectOfType<DayProgressionManager>();
    if (dayManager != null)
    {
        var serializedObject = new UnityEditor.SerializedObject(dayManager);
        serializedObject.FindProperty("autoStartShiftOnGameplay").boolValue = false;
        serializedObject.FindProperty("requireManualShiftStart").boolValue = true;
        serializedObject.ApplyModifiedProperties();
        Debug.Log("✅ DayProgressionManager configured for manual shift start");
    }
}
```

## Complete Integration Checklist

- [ ] Disable auto-start in `DayProgressionManager`
- [ ] Add `ShiftStartController` to scene
- [ ] Assign your StartShift button to the controller
- [ ] Configure button visibility settings
- [ ] Test that button appears at day start
- [ ] Verify shift starts only when button clicked
- [ ] Check timer begins counting after button click

## How It Should Work

1. **Day Starts**: Game enters Gameplay state
2. **No Auto-Start**: DayProgressionManager waits for manual start
3. **Button Shows**: ShiftStartController displays the button/panel
4. **Player Clicks**: User reviews briefing and clicks start
5. **Shift Begins**: Timer starts, encounters become available

## Troubleshooting

### "Shift already active" Error
- Check `DayProgressionManager` settings
- Ensure `requireManualShiftStart` is true
- Verify no other scripts are calling `StartShift()`

### Button Not Showing
- Check game state is `Gameplay`
- Verify `showButtonAtDayStart` is enabled
- Ensure button GameObject is assigned

### Timer Not Starting
- Check `ShiftTimerManager` is registered
- Verify timer gets `StartTimer()` call
- Check debug logs for timer initialization

## Event Flow Diagram

```
Day Start
    ↓
Game State → Gameplay
    ↓
DayProgressionManager (does NOT auto-start shift)
    ↓
ShiftStartController shows button
    ↓
Player clicks "Start Shift"
    ↓
ShiftStartController.OnStartShiftClicked()
    ├→ DayProgressionManager.StartShift()
    ├→ ShiftTimerManager.StartTimer()
    └→ UI updates, button hides
```

This setup gives players explicit control over when their shift begins!