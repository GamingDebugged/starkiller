# Starkiller Base Command - Scenario System Guide

## Overview

The ShipScenario system in Starkiller Base Command provides a powerful framework for creating both routine daily encounters and dramatic story moments. This guide covers the enhanced video system that enables cinematic storytelling while maintaining gameplay flow.

## Table of Contents
- [System Architecture](#system-architecture)
- [ShipScenario Configuration](#shipscenario-configuration)
- [Video System Integration](#video-system-integration)
- [Creating Scenarios](#creating-scenarios)
- [Integration with Game Systems](#integration-with-game-systems)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

---

## System Architecture

### Core Components

1. **ShipScenario** (`Assets/ShipScenario.cs`)
   - ScriptableObject defining encounter situations
   - Contains narrative content, videos, and gameplay rules
   - Supports both story missions and daily encounters

2. **MasterShipEncounter** (`Assets/MasterShipEncounter.cs`)
   - Runtime representation of ship encounters
   - Integrates scenario data with ship/captain information
   - Manages video selection priority

3. **MasterShipGenerator** (`Assets/_Core/ShipSystem/MasterShipGenerator.cs`)
   - Generates daily mix of encounters
   - Balances story scenarios with routine encounters
   - Manages scenario timing and availability

4. **ScenarioMediaHelper** (`Assets/_RefactoredScripts/Core/Helpers/ScenarioMediaHelper.cs`)
   - Handles UI presentation for scenarios
   - Manages video playback and audio
   - Controls panel display and transitions

### Data Flow

```
ShipScenario (Definition) 
    ↓
MasterShipGenerator (Selection & Generation)
    ↓
MasterShipEncounter (Runtime Instance)
    ↓
Game Systems (UI, Video Players, Consequence Manager)
```

---

## ShipScenario Configuration

### Field Reference

#### Basic Information
| Field | Type | Usage | Required |
|-------|------|-------|----------|
| `scenarioName` | string | Internal identifier for debugging | Yes |
| `type` | ScenarioType | Controls UI behavior (Standard, Problem, Invalid, StoryEvent, Inspection) | Yes |

#### Applicability
| Field | Type | Usage | Still Active |
|-------|------|-------|-------------|
| `applicableShipTypes[]` | ShipType[] | Filters which ships can use this scenario | ✅ Yes |
| `applicableCaptainTypes[]` | CaptainType[] | Filters which captains can use this scenario | ✅ Yes |
| `dayFirstAppears` | int | Controls when scenario becomes available | ✅ Yes |
| `maxAppearances` | int | Limits how often scenario appears (-1 = unlimited) | ✅ Yes |

#### Scenario Content
| Field | Type | Usage | Notes |
|-------|------|-------|-------|
| `possibleStories[]` | string[] | Text descriptions shown in ship info panel | Still used for narrative text |
| `possibleManifests[]` | string[] | Cargo/passenger descriptions | Fallback if ManifestManager unavailable |

#### Special Rules
| Field | Type | Usage | Critical |
|-------|------|-------|----------|
| `shouldBeApproved` | bool | Defines correct decision | ⚠️ CRITICAL |
| `invalidReason` | string | Explanation for denial | Used when shouldBeApproved = false |
| `offersBribe` | bool | Whether bribe is offered | Gameplay mechanic |
| `bribeChanceMultiplier` | float | Modifies bribe probability | 0-1 range |

#### Consequences
| Field | Type | Usage |
|-------|------|-------|
| `severityLevel` | SeverityLevel | Minor/Moderate/Severe |
| `possibleConsequences[]` | string[] | Consequence descriptions |
| `minCasualties` | int | Minimum casualties if wrong |
| `maxCasualties` | int | Maximum casualties if wrong |

#### Story Elements
| Field | Type | Usage | Important |
|-------|------|-------|-----------|
| `isStoryMission` | bool | Enables scenario video priority | 🔑 KEY FLAG |
| `storyTag` | string | Groups related scenarios | e.g., "insurgent", "refugee" |

---

## Video System Integration

### Video Field Structure

#### Presentation Videos
```csharp
public VideoClip greetingVideo;        // Captain's initial presentation
public VideoClip storyVideo;           // Shows the narrative situation
public VideoClip[] alternativeGreetingVideos;  // Variety for replays
```

#### Player Decision Response Videos
```csharp
public VideoClip approveVideo;         // Played when approved
public VideoClip denyVideo;            // Played when denied
public VideoClip holdingPatternVideo;  // Ship in holding
public VideoClip tractorBeamVideo;     // Ship captured
```

#### Consequence Videos
```csharp
public VideoClip[] correctDecisionConsequenceVideos;  // Right choice outcomes
public VideoClip[] wrongDecisionConsequenceVideos;    // Wrong choice outcomes
```

#### Audio Elements
```csharp
public AudioClip introAudioClip;       // Scenario introduction
public AudioClip dramaticAudioClip;    // Dramatic moments
public AudioClip consequenceAudioClip; // Consequence moments
```

### Video Priority System

```
Story Scenarios (isStoryMission = true):
  1. Check HasCompleteVideoContent()
  2. If TRUE → Use scenario videos
  3. If FALSE → Fall back to media database

Regular Scenarios (isStoryMission = false):
  1. Always use media database videos
  2. Scenario videos ignored even if present
```

### Helper Methods

```csharp
// Check if scenario has complete video content
bool hasVideos = scenario.HasCompleteVideoContent();

// Get specific videos
VideoClip greeting = scenario.GetGreetingVideo();
VideoClip response = scenario.GetDecisionResponseVideo(decision);
VideoClip consequence = scenario.GetConsequenceVideo(wasCorrect);

// Get audio for different moments
AudioClip audio = scenario.GetAudioClip(ShipScenario.AudioMoment.Introduction);
```

---

## Creating Scenarios

### Example 1: Refugee Children (Story Scenario)

```yaml
Basic Information:
  scenarioName: "Refugee Children Escape"
  type: StoryEvent

Story Elements:
  isStoryMission: true ✅
  storyTag: "refugee_children"
  dayFirstAppears: 3

Applicability:
  applicableShipTypes: [Transport, Civilian]
  applicableCaptainTypes: [Civilian, Resistance]

Content:
  possibleStories: 
    - "Transport carrying refugee families fleeing Imperial oppression"
    - "Desperate families seeking sanctuary from war-torn system"
  possibleManifests:
    - "50 civilian refugees, including 20 children"

Rules:
  shouldBeApproved: true  # Moral choice - help the refugees
  invalidReason: ""       # Not needed since should approve

Videos:
  greetingVideo: Captain_Refugee_Plea.mp4
  storyVideo: Children_In_Cargo_Hold.mp4
  approveVideo: Families_Embrace_Freedom.mp4
  denyVideo: Children_Turned_Away.mp4
  correctDecisionConsequenceVideos: 
    - Happy_Families_Settlement.mp4
  wrongDecisionConsequenceVideos:
    - News_Report_Refugee_Tragedy.mp4
```

### Example 2: Imperial Smuggler (Invalid Scenario)

```yaml
Basic Information:
  scenarioName: "Imperial Officer Smuggling"
  type: Invalid

Story Elements:
  isStoryMission: true ✅
  storyTag: "imperial_corruption"
  dayFirstAppears: 5

Content:
  possibleStories:
    - "High-ranking officer with suspicious cargo manifest"
    - "Imperial vessel with undeclared modifications"

Rules:
  shouldBeApproved: false  # Should catch the corruption
  invalidReason: "Hidden contraband detected in cargo hold"
  offersBribe: true
  bribeChanceMultiplier: 1.5

Videos:
  greetingVideo: Officer_Acting_Nervous.mp4
  storyVideo: Hidden_Contraband_Scan.mp4
  approveVideo: Officer_Smirking_Escape.mp4
  denyVideo: Officer_Arrested.mp4
  correctDecisionConsequenceVideos:
    - Officer_Court_Martial.mp4
  wrongDecisionConsequenceVideos:
    - Corruption_Spreads_News.mp4

Consequences:
  severityLevel: Severe
  possibleConsequences:
    - "Imperial corruption investigation launched - {CASUALTIES} officers implicated"
  minCasualties: 5
  maxCasualties: 15
```

### Example 3: Standard Supply Run (Daily Grind)

```yaml
Basic Information:
  scenarioName: "Standard Supply Delivery"
  type: Standard

Story Elements:
  isStoryMission: false ❌  # Regular encounter
  storyTag: ""

Applicability:
  applicableShipTypes: [Transport, Cargo, Merchant]
  applicableCaptainTypes: []  # Any captain type

Content:
  possibleStories:
    - "Routine supply delivery for Imperial facilities"
    - "Standard cargo transfer from Core Worlds"
  possibleManifests:
    - "Medical supplies, food rations, spare parts"
    - "Construction materials, equipment, provisions"

Rules:
  shouldBeApproved: true
  
Videos: # All empty - uses media database
  greetingVideo: null
  approveVideo: null
  denyVideo: null
```

---

## Integration with Game Systems

### 1. Manifest System (Dual Approach)

```csharp
// MasterShipEncounter.cs:584-594
// First: Get simple manifest from scenario
encounter.manifest = scenario.GetRandomManifest();

// Then: Override with ManifestManager if available
if (ManifestManager.Instance != null)
{
    encounter.manifestData = ManifestManager.Instance.SelectManifestForShip(
        shipType, 
        encounter.faction, 
        currentDay
    );
}
```

**Result**: Scenarios provide fallback manifests, but ManifestManager takes priority when available.

### 2. Video Selection Priority

```csharp
// MasterShipEncounter.cs:846-880
if (isStoryShip && HasScenarioVideos())
{
    // Use scenario videos for story encounters
    Debug.Log("Using scenario videos for story encounter");
}
else
{
    // Regular encounters use media database
    captainVideo = mediaDatabase.GetCaptainVideo(...);
}
```

### 3. Decision Response Integration

```csharp
// In your UI/Video controller:
void OnPlayerDecision(MasterShipEncounter.DecisionState decision)
{
    // Get appropriate response video
    VideoClip responseVideo = encounter.GetDecisionResponseVideo(decision);
    
    if (responseVideo != null)
    {
        videoPlayer.clip = responseVideo;
        videoPlayer.Play();
    }
}
```

### 4. PersonalDataLog Integration

```csharp
// After decision is evaluated:
bool wasCorrect = (playerApproved == encounter.shouldApprove);
VideoClip consequenceVideo = encounter.GetConsequenceVideo(wasCorrect);

// Add to PersonalDataLog with video
personalDataLog.AddEntry(encounter, decision, consequenceVideo);
```

---

## Best Practices

### Do's ✅

1. **Story Scenarios**
   - Always set `isStoryMission = true`
   - Provide at least greeting + one decision video
   - Use meaningful `storyTag` for grouping
   - Test with `HasCompleteVideoContent()`

2. **Regular Scenarios**
   - Keep `isStoryMission = false`
   - Focus on `possibleStories[]` variety
   - Leave video fields empty
   - Use for daily grind content

3. **Performance**
   - Preload critical scenario videos
   - Use alternativeGreetingVideos for variety
   - Keep consequence videos short (5-10 seconds)

4. **Narrative Design**
   - Match `possibleStories[]` text to video content
   - Ensure `shouldBeApproved` aligns with moral choice
   - Use `dayFirstAppears` for story progression

### Don'ts ❌

1. **Common Mistakes**
   - Don't mix story and regular scenarios randomly
   - Don't leave `shouldBeApproved` undefined
   - Don't create scenarios without applicability filters
   - Don't use huge video files (keep under 50MB)

2. **Video Issues**
   - Don't assign partial video sets to story scenarios
   - Don't use scenario videos for regular encounters
   - Don't forget consequence videos for story missions

---

## Troubleshooting

### Scenario Not Appearing

```csharp
// Check in MasterShipGenerator
Debug.Log($"Day: {currentDay}, Scenario appears: {scenario.dayFirstAppears}");
Debug.Log($"Applicable ships: {scenario.applicableShipTypes.Length}");
```

### Videos Not Playing

```csharp
// Verify scenario setup
Debug.Log($"Has videos: {scenario.HasCompleteVideoContent()}");
Debug.Log($"Is story: {scenario.isStoryMission}");
Debug.Log($"Greeting video: {scenario.greetingVideo != null}");
```

### Wrong Decision Consequences

```csharp
// Check decision logic
Debug.Log($"Should approve: {scenario.shouldBeApproved}");
Debug.Log($"Player approved: {playerDecision == DecisionState.Approved}");
Debug.Log($"Was correct: {wasCorrectDecision}");
```

---

## Related Scripts Reference

### Core Scripts
- `Assets/ShipScenario.cs` - ScriptableObject definition
- `Assets/MasterShipEncounter.cs` - Runtime encounter instance
- `Assets/_Core/ShipSystem/MasterShipGenerator.cs` - Encounter generation

### Support Scripts
- `Assets/_RefactoredScripts/Core/Helpers/ScenarioMediaHelper.cs` - UI/Media handling
- `Assets/ManifestManager.cs` - Advanced manifest system
- `Assets/ConsequenceManager.cs` - Decision outcome handling
- `Assets/_RefactoredScripts/Core/Managers/PersonalDataLogManager.cs` - Decision history

### Integration Points
- `CredentialChecker.cs:466` - Displays ship info text
- `VideoPlayerSetup.cs` - Handles video playback
- `DailyBriefingManager.cs` - Day progression triggers

---

## Version History

- **v2.0** (Current) - Enhanced video system with full scenario support
- **v1.0** - Basic scenario system with text-only content

---

## Summary

The ShipScenario system provides a flexible framework for both routine encounters and cinematic story moments. By properly configuring the `isStoryMission` flag and providing appropriate videos, you can create memorable narrative experiences while maintaining the daily grind of border control simulation.

Remember: Story scenarios are your narrative highlights - make them count with complete video content. Regular scenarios maintain the gameplay flow - keep them simple and use the media database.

For questions or issues, check the Debug Monitor (`Assets/DebugMonitor.cs`) for detailed logging of scenario selection and video playback.