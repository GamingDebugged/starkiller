# Personal Data Log UI Prefab Structure

## Main Panel Hierarchy

```
PersonalDataLogPanel (GameObject)
├── Background (Image) - Dark holographic background
├── Header (GameObject)
│   ├── Title (TextMeshPro) - "PERSONAL DATA LOG - DAY X"
│   └── DateTime (TextMeshPro) - Current date/time
├── ContentArea (GameObject)
│   ├── ImperiumNewsSection (GameObject)
│   │   ├── SectionHeader (GameObject)
│   │   │   ├── Icon (Image) - Imperial symbol
│   │   │   └── Title (TextMeshPro) - "IMPERIUM NEWS"
│   │   ├── ScrollView (ScrollRect)
│   │   │   └── Content (GameObject) - Vertical Layout Group
│   │   └── Divider (Image) - Visual separator
│   ├── FamilyChatSection (GameObject)
│   │   ├── SectionHeader (GameObject)
│   │   │   ├── Icon (Image) - Family/chat symbol
│   │   │   └── Title (TextMeshPro) - "FAMILY GROUP CHAT"
│   │   ├── ScrollView (ScrollRect)
│   │   │   └── Content (GameObject) - Vertical Layout Group
│   │   └── Divider (Image) - Visual separator
│   └── FrontierEzineSection (GameObject)
│       ├── SectionHeader (GameObject)
│       │   ├── Icon (Image) - Frontier/news symbol
│       │   └── Title (TextMeshPro) - "FRONTIER E-ZINE"
│       └── ScrollView (ScrollRect)
│           └── Content (GameObject) - Vertical Layout Group
├── Footer (GameObject)
│   └── ContinueButton (Button)
│       └── ButtonText (TextMeshPro) - "CONTINUE TO BRIEFING"
└── TimeModifierBehavior (Component) - Pauses time when active
```

## Entry Templates

### NewsEntryTemplate (Prefab)
```
NewsEntryTemplate (GameObject)
├── Background (Image) - Entry background
├── Content (GameObject) - Horizontal Layout Group
│   ├── Headline (TextMeshPro) - Bold title text
│   ├── Timestamp (TextMeshPro) - Small timestamp
│   └── ContentText (TextMeshPro) - Main article text
└── Spacer (LayoutElement) - Spacing between entries
```

### FamilyActionTemplate (Prefab)
```
FamilyActionTemplate (GameObject)
├── Background (Image) - Slightly different color for actions
├── Content (GameObject) - Vertical Layout Group
│   ├── Header (GameObject) - Horizontal Layout Group
│   │   ├── Headline (TextMeshPro) - Bold title
│   │   └── Timestamp (TextMeshPro) - Small timestamp
│   ├── ContentText (TextMeshPro) - Main message
│   └── ActionArea (GameObject) - Horizontal Layout Group
│       ├── ActionButton (Button)
│       │   └── ButtonText (TextMeshPro) - "Pay 100 Credits"
│       └── StatusText (TextMeshPro) - "Insufficient funds" if needed
└── Spacer (LayoutElement)
```

### VideoEntryTemplate (Prefab) - For future use
```
VideoEntryTemplate (GameObject)
├── Background (Image)
├── Content (GameObject) - Vertical Layout Group
│   ├── Header (GameObject)
│   │   ├── Headline (TextMeshPro)
│   │   └── Timestamp (TextMeshPro)
│   ├── VideoArea (GameObject)
│   │   ├── VideoPlayer (VideoPlayer)
│   │   └── VideoDisplay (RawImage)
│   └── ContentText (TextMeshPro)
└── Spacer (LayoutElement)
```

## Component Settings

### PersonalDataLogPanel
- **Canvas Group**: Alpha = 1, Interactable = true, Blocks Raycasts = true
- **RectTransform**: Anchored to full screen
- **Background Image**: Dark semi-transparent color (0, 0, 0, 0.9)

### Content Area Layout
- **Horizontal Layout Group**: 
  - Child Alignment = Upper Left
  - Child Force Expand Width = true
  - Child Force Expand Height = true
  - Spacing = 20

### Section ScrollViews
- **ScrollRect**: 
  - Horizontal = false, Vertical = true
  - Movement Type = Clamped
  - Scrollbar Visibility = Auto Hide
- **Content Layout Group**: 
  - Vertical Layout Group
  - Child Alignment = Upper Left
  - Child Force Expand Width = true
  - Child Force Expand Height = false
  - Spacing = 10

### Entry Templates
- **Content Size Fitter**: 
  - Horizontal Fit = Unconstrained
  - Vertical Fit = Preferred Size
- **Layout Element**: 
  - Preferred Height = -1 (flexible)
  - Min Height = 60

## Material/Styling Suggestions

### Colors (Holographic Theme)
- **Background**: Dark blue-black (0.05, 0.1, 0.2, 0.95)
- **Imperium News**: Imperial red accent (0.8, 0.1, 0.1, 1)
- **Family Chat**: Warm orange accent (1, 0.6, 0.2, 1)
- **Frontier E-zine**: Neutral cyan accent (0.2, 0.8, 1, 1)
- **Text**: Light cyan (0.8, 0.9, 1, 1)
- **Action Buttons**: Bright yellow (1, 1, 0.3, 1)

### Fonts
- **Headers**: Bold, larger size (18-20pt)
- **Content**: Regular weight (12-14pt)
- **Timestamps**: Small, italic (10pt)

## Inspector References for PersonalDataLogManager

```csharp
[Header("UI References")]
public GameObject dataLogPanel;           // → PersonalDataLogPanel
public TMP_Text headerText;              // → Header/Title
public Button continueButton;            // → Footer/ContinueButton

[Header("Feed Sections")]  
public Transform imperiumNewsSection;    // → ImperiumNewsSection/ScrollView/Content
public Transform familyChatSection;     // → FamilyChatSection/ScrollView/Content
public Transform frontierEzineSection;  // → FrontierEzineSection/ScrollView/Content

[Header("Feed Templates")]
public GameObject newsEntryTemplate;     // → NewsEntryTemplate prefab
public GameObject familyActionTemplate; // → FamilyActionTemplate prefab
public GameObject videoEntryTemplate;   // → VideoEntryTemplate prefab (future)
```

## Setup Instructions

1. **Create Main Panel**: Canvas → UI → Panel, rename to "PersonalDataLogPanel"
2. **Add Components**: Add Canvas Group and TimeModifierBehavior
3. **Create Sections**: Build the three-section layout with scroll views
4. **Create Templates**: Build entry templates as separate prefabs
5. **Assign References**: Drag components to PersonalDataLogManager script
6. **Style**: Apply holographic color scheme and fonts
7. **Test**: Use PersonalDataLogManager's "Add Sample Data" context menu

## Animation Suggestions (Optional)

- **Panel Entry**: Fade in with scale animation (0.8 → 1.0)
- **Feed Loading**: Stagger entry appearances with slight delays
- **Button Hover**: Glow effect on action buttons
- **Section Headers**: Subtle pulsing glow for sci-fi feel