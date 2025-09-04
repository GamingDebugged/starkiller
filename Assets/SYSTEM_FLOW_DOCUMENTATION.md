# Starkiller Unity Game - System Flow Documentation
**Date:** August 1st, 2025  
**Purpose:** Memory aid and understanding guide for system interconnections

## 🔴 CRITICAL REMINDERS

**⚠️ REMEMBER: Do not simply replace scripts without understanding the other dependencies on that script.**

**⚠️ REMEMBER: Always check what other systems depend on a manager before modifying it.**

**⚠️ REMEMBER: The codebase has DUAL SYSTEMS running in parallel - original GameManager system and refactored manager system.**

---

## 📋 System Overview

The Starkiller Unity game operates with a dual architecture:
1. **Original Legacy System** - GameManager-centric, handles actual game data
2. **Refactored Manager System** - ServiceLocator-based, handles day progression logic and events

### Key Principle: **COORDINATION NOT REPLACEMENT**
Both systems work together harmoniously rather than one replacing the other.

---

## 🎮 Day Progression Flow

### Complete Day Transition Sequence:
```
Shift Timer Expires 
    ↓
GameManager.EndShift() 
    ↓
GameManager.ShowDailyReport() calls BOTH:
    ├── DailyReportManager.ShowDailyReport() [UI DISPLAY with real data]
    └── RefactoredDailyReportManager.GenerateDailyReport(false) [LOGIC ONLY]
    ↓
User clicks Continue in Daily Performance Report
    ↓
DailyReportManager.OnContinueClicked() 
    ↓
RefactoredDailyReportManager.RequestNextDay()
    ↓
DayProgressionManager.StartNewDay() [increments day]
    ↓
PersonalDataLogManager.ShowDataLog() [shows personal data]
    ↓
User clicks Continue in Personal Data Log
    ↓
DailyBriefingManager.ShowDailyBriefing() [shows briefing]
    ↓
User clicks Start Shift in Daily Briefing
    ↓
GameStateManager transitions: MainMenu → DailyBriefing → Gameplay
    ↓
New shift begins with encounters
```

---

## 🏗️ System Architecture

### Core Managers and Their Roles:

#### **GameManager** (`/Assets/_scripts/GameManager.cs`)
- **Role**: Original legacy system, holds actual game data
- **Responsibilities**: Credits, ships processed, strikes, salary calculation
- **Key Method**: `ShowDailyReport()` - calls BOTH UI and logic systems
- **Dependencies**: Nearly everything depends on this
- **⚠️ WARNING**: Modifying this breaks the entire game

#### **DailyReportManager** (`/Assets/_scripts/DailyReportManager.cs`)  
- **Role**: UI display for daily performance report
- **Responsibilities**: Populates and shows daily report with real GameManager data
- **Key Method**: `ShowDailyReport()` - displays populated UI
- **Continue Button**: Calls `RefactoredDailyReportManager.RequestNextDay()`

#### **RefactoredDailyReportManager** (`/Assets/_RefactoredScripts/Core/Managers/RefactoredDailyReportManager.cs`)
- **Role**: Day progression logic and events
- **Responsibilities**: Handles day transitions, events, dependencies
- **Key Method**: `RequestNextDay()` - orchestrates full day transition
- **⚠️ CRITICAL**: DayProgressionManager depends on its events

#### **DayProgressionManager** (`/Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs`)
- **Role**: Authoritative day counter and day start/end events
- **Dependencies**: Listens to `RefactoredDailyReportManager.OnNextDayRequested`
- **Key Method**: `StartNewDay()` - increments day, triggers events

#### **PersonalDataLogManager** (`/Assets/_RefactoredScripts/Core/Managers/PersonalDataLogManager.cs`)
- **Role**: Shows personal data log between daily report and briefing
- **Key Method**: `ShowDataLog()` - ensures daily report panel is hidden
- **Continue Button**: Shows DailyBriefingManager

#### **GameStateManager** (`/Assets/_RefactoredScripts/Core/Managers/GameStateManager.cs`)
- **Role**: Game state transitions with validation
- **Key Validation**: Only allows MainMenu → DailyBriefing → Gameplay
- **⚠️ CRITICAL**: EncounterManager requires Gameplay state to generate encounters

---

## 🎪 UI Text and Display Systems

### Ship Info Display:
- **Location**: ShipInfoText (TMP) in main game UI
- **Populated by**: `CredentialChecker.UpdateEncounterText()` calls `encounter.GetShipInfo()`
- **Demo Content**: `MasterShipEncounter.CreateTestEncounter()` provides fallback data
- **Fixed**: Now shows "Click Unlock to Connect To Incoming Ships" when no encounters

### Credentials Panel:
- **Location**: CredentialsPanel in main game UI  
- **Populated by**: `CredentialChecker.UpdateEncounterText()` calls `encounter.GetCredentialsInfo()`
- **Demo Content**: Fixed to show empty when no ships (no bribe offers)

### Dialog Text:
- **Location**: DialogText (TMP) - separate from captain dialog
- **Issue**: Shows hardcoded Unity scene text "Imperium shuttle requesting immediate clearance..."
- **Status**: NOT FIXED - different from captainDialogText in EncounterMediaTransitionManager

---

## 🔧 Key Integration Points

### 1. Daily Report System Integration:
```csharp
// GameManager.ShowDailyReport() coordinates both systems:
DailyReportManager originalManager = FindFirstObjectByType<DailyReportManager>();
originalManager.ShowDailyReport(/* real game data */);

RefactoredDailyReportManager refactoredManager = ServiceLocator.Get<RefactoredDailyReportManager>();
refactoredManager.GenerateDailyReport(false); // Logic only, no UI
```

### 2. Day Progression Coordination:
```csharp
// DailyReportManager.OnContinueClicked() delegates to refactored system:
RefactoredDailyReportManager refactored = ServiceLocator.Get<RefactoredDailyReportManager>();
refactored.RequestNextDay(); // Handles full sequence
```

### 3. State Management:
```csharp
// GameManager must transition through DailyBriefing state:
if (gameStateManager.CurrentState == GameState.MainMenu) {
    gameStateManager.ChangeState(GameState.DailyBriefing);
}
gameStateManager.ChangeState(GameState.Gameplay);
```

---

## 🛡️ Critical Dependencies Map

### RefactoredDailyReportManager Dependencies:
- **DayProgressionManager**: Listens to `OnNextDayRequested` event
- **DailyReportDebugger**: References for testing
- **ShiftEndDebugger**: References for debugging  
- **TestPhase5Integration**: Used in integration tests
- **GameManagerIntegrationHelper**: References in helper system

### DayProgressionManager Dependencies:
- **EncounterManager**: Depends on day start/end events
- **ShiftTimerManager**: Coordinates with day progression
- **Multiple debugging systems**: Reference for testing

### GameStateManager Dependencies:
- **EncounterManager**: Requires Gameplay state to function
- **UI systems**: Various UI elements check game state
- **Media systems**: Video playback depends on state

---

## 🚨 Common Pitfalls and Solutions

### ❌ Problem: "Daily Report shows no data"
**Root Cause**: Using RefactoredDailyReportManager UI instead of original DailyReportManager  
**Solution**: Use original DailyReportManager for UI, refactored for logic

### ❌ Problem: "Day doesn't progress after daily report" 
**Root Cause**: State transition conflicts between systems  
**Solution**: Let RefactoredDailyReportManager handle full progression sequence

### ❌ Problem: "Encounters don't generate on Day 2+"
**Root Cause**: GameStateManager not in Gameplay state  
**Solution**: Ensure proper state transition: MainMenu → DailyBriefing → Gameplay

### ❌ Problem: "Daily report still visible after transitions"
**Root Cause**: Multiple systems trying to control same UI panel  
**Solution**: PersonalDataLogManager explicitly hides daily report panel

---

## 🔄 Testing and Verification

### Key Test Points:
1. **Day 1 → Day 2 transition**: All panels should show and hide correctly
2. **Daily report data**: Should show real salary, ships processed, expenses
3. **Encounter generation**: Should work on Day 2+ (check Gameplay state)
4. **UI text**: Should show appropriate messages, no demo content visible

### Debug Commands:
- Use context menus in managers for testing individual components
- Check console logs for state transitions and day increments
- Verify ServiceLocator registrations at game start

---

## 📝 Future Maintenance Notes

### When Adding New Features:
1. **Determine which system owns the feature** (legacy vs refactored)
2. **Check all dependencies** before modifying existing managers
3. **Use coordination pattern** - don't replace, integrate
4. **Test full day progression flow** after any changes

### When Debugging Issues:
1. **Check both systems** - problem might be in coordination
2. **Verify ServiceLocator registrations** for refactored managers
3. **Check game state transitions** for UI/encounter issues
4. **Look at console logs** for state change debugging

---

## 📚 File Reference Quick Guide

### Core Scripts:
- **GameManager**: `/Assets/_scripts/GameManager.cs`
- **DailyReportManager**: `/Assets/_scripts/DailyReportManager.cs`  
- **CredentialChecker**: `/Assets/_scripts/CredentialChecker.cs`
- **MasterShipEncounter**: `/Assets/MasterShipEncounter.cs`

### Refactored Scripts:
- **RefactoredDailyReportManager**: `/Assets/_RefactoredScripts/Core/Managers/RefactoredDailyReportManager.cs`
- **DayProgressionManager**: `/Assets/_RefactoredScripts/Core/Managers/DayProgressionManager.cs`
- **PersonalDataLogManager**: `/Assets/_RefactoredScripts/Core/Managers/PersonalDataLogManager.cs`
- **GameStateManager**: `/Assets/_RefactoredScripts/Core/Managers/GameStateManager.cs`

---

*This documentation serves as a reference for understanding the complex interconnections in the Starkiller Unity game. Always refer to this before making significant system changes.*