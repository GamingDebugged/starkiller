# PersonalDataLog System Documentation

## Overview
The PersonalDataLog is a narrative UI system in the Starkiller Unity game that presents daily news, family communications, and frontier information to the player in an immersive, terminal-like interface. It serves as the primary storytelling vehicle and creates emotional investment through family connections and world-building.

## Vision & Purpose
- **Narrative Immersion**: Creates a believable in-universe information system
- **Emotional Investment**: Family messages create personal stakes in the player's decisions
- **World Building**: News feeds establish the broader galactic context
- **Player Agency**: Family actions allow spending credits to help loved ones
- **Pacing Control**: Appears between game days to provide narrative beats

## System Architecture

### Core Components

#### 1. PersonalDataLogManager.cs
**Location**: `Assets/_RefactoredScripts/Core/Managers/PersonalDataLogManager.cs`

**Primary Responsibilities**:
- Orchestrates the entire PersonalDataLog system
- Collects daily data from multiple sources
- Manages template instantiation and content population
- Handles user interactions (Read, Family Actions, Video Playback)
- Integrates with game progression systems

**Key Dependencies**:
- ServiceLocator pattern for dependency injection
- DayProgressionManager for current day tracking
- ConsequenceManager for incident data
- GameManager for game state integration

#### 2. PersonalDataLogEntrySO.cs
**Location**: `Assets/_RefactoredScripts/Core/ScriptableObjects/PersonalDataLogEntrySO.cs`

**Purpose**: ScriptableObject class that allows designers to create content without code

**Key Features**:
- Day-based appearance conditions (minDay, maxDay)
- Content variations (alternate headlines/content)
- Family action requirements (credit costs, persistence)
- Video clip and thumbnail support
- Consequence tracking (family pressure, imperial loyalty)

#### 3. Data Structures

**DataLogEntry** (Runtime):
```csharp
public class DataLogEntry {
    public FeedType feedType;
    public string headline;
    public string content;
    public VideoClip videoClip;
    public Sprite videoThumbnail;  // Custom thumbnail support
    public bool requiresAction;
    public FamilyActionData familyActionData;
    // ... other fields
}
```

**FeedType Enum**:
- `ImperiumNews`: Official imperial propaganda and security reports
- `FamilyChat`: Personal messages from family members
- `FrontierEzine`: Underground/independent news source

### Template System

#### UI Templates (Prefabs)
- **NewsEntryTemplate**: For Imperium News and Frontier E-zine entries
- **FamilyActionTemplate**: For family messages with action buttons
- **VideoEntryTemplate**: Specialized template for video-heavy content (used by Inspection System)

#### Template Selection Logic
```csharp
private GameObject SelectTemplateForEntry(DataLogEntry entry)
{
    // Video content gets VideoEntryTemplate
    if (entry.videoClip != null && videoEntryTemplate != null)
        return videoEntryTemplate;
    
    // Family messages get FamilyActionTemplate  
    if (entry.feedType == FeedType.FamilyChat && familyActionTemplate != null)
        return familyActionTemplate;
    
    // Everything else gets NewsEntryTemplate
    return newsEntryTemplate;
}
```

## Video System Architecture

### Video Playback Chain
```
VideoClip → VideoPlayer → RenderTexture → RawImage → Screen
```

### Key Features
1. **Unique RenderTextures**: Each video entry gets its own RenderTexture to prevent conflicts
2. **Custom Thumbnails**: Optional Sprite thumbnails, falls back to first frame
3. **User-Controlled Playback**: Play buttons prevent autoplay and hierarchy issues
4. **Safe Preparation**: Coroutine-based video preparation with timeout protection

### Video Setup Process
```csharp
// 1. Create unique RenderTexture per video
string textureName = $"VideoRT_{entry.headline}_{Time.time}";
RenderTexture uniqueTexture = new RenderTexture(640, 360, 0);
videoPlayer.targetTexture = uniqueTexture;

// 2. Show thumbnail initially (custom or first frame)
if (entry.videoThumbnail != null)
    rawImage.texture = entry.videoThumbnail.texture;

// 3. Play button switches to video texture and starts playback
```

## Content Creation Workflow

### For Designers
1. **Create ScriptableObject**: Right-click → Create → Starkiller → Personal Data Log → Entry
2. **Configure Entry**:
   - Set feedType (ImperiumNews, FamilyChat, FrontierEzine)
   - Write headline and content
   - Set appearance conditions (minDay, maxDay, appearanceChance)
   - Add video clip and thumbnail if needed
   - Configure family actions if applicable
3. **Place in Collection**: Add to PersonalDataLogCollectionSO for system discovery

### Content Categories

#### Imperium News
- **Security incidents** linked to player performance
- **Propaganda messages** reinforcing imperial themes
- **Policy announcements** affecting game mechanics
- **Victory celebrations** based on player success

#### Family Chat
- **Daily check-ins** providing emotional connection
- **Financial pressures** requiring credit expenditure
- **Medical emergencies** creating urgent decisions
- **Milestone celebrations** rewarding player progress

#### Frontier E-zine
- **Underground resistance** news providing alternative perspective
- **Economic impacts** of imperial policies
- **Trade route information** relevant to player operations
- **Independent journalism** challenging official narratives

## Game Integration Points

### Day Progression Flow
```
StartNextDay() → GameManager checks ShouldShowPersonalLog() → 
PersonalDataLogManager.ShowDataLog() → CollectDailyData() → 
PopulateUI() → Player Interaction → OnContinueClicked() → StartDay()
```

### Data Collection Sources
1. **ScriptableObjects** (CollectFromScriptableObjects): Designer-created content
2. **Consequence System** (CollectConsequenceData): Dynamic incident reporting
3. **Persistent Actions** (GetPersistentFamilyActions): Multi-day family situations

### Integration Systems
- **ConsequenceManager**: Provides security incidents for news
- **DecisionTracker**: Influences family pressure and content
- **CreditSystem**: Enables family action purchases
- **DayProgressionManager**: Controls when PersonalDataLog appears

## User Interaction Patterns

### Reading Behavior
- **News Entries**: Mark as read with ✓, remain visible for reference
- **Family Entries**: Disappear after reading (notification-style)
- **Video Entries**: Persistent for repeated viewing

### Family Actions
- **Credit Cost**: Displayed prominently (e.g., "Pay Bills (30 Credits)")
- **Immediate Effect**: Credits deducted, family pressure reduced
- **Narrative Consequence**: Affects future family message tone

### Video Controls
- **Thumbnail Display**: Shows custom image or first frame
- **Play Button**: User-initiated playback
- **Texture Switching**: Seamlessly transitions from thumbnail to video

## Technical Considerations

### Performance
- **Unique RenderTextures**: Memory overhead scales with concurrent videos
- **Template Pooling**: Consider object pooling for high-frequency usage
- **Video Preparation**: Async preparation prevents frame drops

### Error Handling
- **Missing References**: Graceful degradation when components not found
- **Video Preparation Timeout**: 5-second timeout prevents infinite waits
- **Null Safety**: Extensive null checks throughout system

### Debug Features
- **Comprehensive Logging**: Detailed debug output for troubleshooting
- **Component Path Tracking**: GetGameObjectPath() for hierarchy debugging
- **State Validation**: Regular checks of VideoPlayer and UI states

## Future Enhancement Opportunities

### Content System
- **Branching Narratives**: Prerequisite/consequence chains between entries
- **Dynamic Content**: Procedural headline/content generation
- **Localization Support**: Multi-language content system

### UI/UX
- **Animation System**: Entry appearance/dismissal animations
- **Sound Integration**: Audio cues for different message types
- **Visual Theming**: Per-feed visual styling

### Technical
- **Memory Optimization**: RenderTexture pooling system
- **Video Streaming**: Support for larger video files
- **Save/Load Integration**: Persistent read states and family actions

## Troubleshooting Guide

### Common Issues
1. **Grey Video Boxes**: Check RenderTexture assignment and VideoPlayer hierarchy
2. **Buttons Not Working**: Verify template component references
3. **Missing Entries**: Check day conditions and ScriptableObject collections
4. **Performance Issues**: Monitor RenderTexture count and video preparation

### Debug Tools
- **Console Logging**: Enable `enableDebugLogs` in PersonalDataLogManager
- **Component Inspector**: Use GetGameObjectPath() output to locate components
- **Template Validation**: Run PersonalDataLogTemplateSetup context menu functions

## File Locations

### Core Scripts
- `Assets/_RefactoredScripts/Core/Managers/PersonalDataLogManager.cs`
- `Assets/_RefactoredScripts/Core/ScriptableObjects/PersonalDataLogEntrySO.cs`
- `Assets/_scripts/PersonalDataLogTemplateSetup.cs`

### Content Assets
- `Assets/Resources/_ScriptableObjects/PersonalDataLog/` (all content)
- `Assets/_Prefabs/NewsEntryTemplate.prefab`
- `Assets/_Prefabs/FamilyActionTemplate.prefab` 
- `Assets/_Prefabs/VideoEntryTemplate.prefab`

### Media Assets
- `Assets/_videos/` (video content)
- `Assets/` (thumbnail images as needed)

---

*This system represents a complete narrative content delivery platform that seamlessly integrates with the broader Starkiller game systems while providing rich, immersive storytelling opportunities.*