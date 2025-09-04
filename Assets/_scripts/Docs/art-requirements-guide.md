# Art Requirements and Reference Guide

## Visual Style Overview

The game should embrace a Star Wars Imperial aesthetic with a focus on the clean, functional, and somewhat austere design language of the Empire. The UI should feel like an actual Imperial terminal that an officer would use on the Starkiller Base.

### Color Palette Recommendation
- **Primary**: Dark grays and blacks (imperial console backgrounds)
- **Secondary**: Whites and light grays (text and highlights)
- **Accent**: Imperial reds (#BA0C2F) and blues (#0A3161)
- **Functional**: Green for approval (#32CD32), red for denial (#DC143C)

## Required Art Assets

### UI Elements

#### Main Menu
- **Background**: Starkiller Base exterior view or command center interior
- **Title**: "Starkiller Base Command" - Imperial font style
- **Buttons**:
  - Start Game
  - Tutorial
  - Credits
  - Quit
- **Imperial Logo**: Either First Order or Empire depending on era preference

#### Gameplay Screen
- **Terminal Frame**: Console-like border around the entire interface
- **Ship Display Area**: Simplified visual representation of the approaching ship
- **Information Panel**: Area for displaying ship credentials and story
- **Reference Section**: Log book area (can be toggled)
- **Decision Buttons**:
  - Approve (green imperial styling)
  - Deny (red imperial styling)

#### Log Book
- **Cover**: Imperial-styled folder or datapad
- **Pages**: Technical specifications with imperial styling
- **Tab System**: For different reference sections

#### Feedback Elements
- **Approval Stamp**: Visual indication of approval
- **Rejection Stamp**: Visual indication of rejection
- **Warning Indicators**: For strikes/mistakes
- **Success Indicator**: For correct decisions

### Ship Types (Simple Icons or Silhouettes)
- Lambda Shuttle
- Cargo Freighter
- TIE Fighter
- Imperial Transport
- Supply Ship
- Scout Ship
- Command Shuttle

### Background Elements
- **Main View**: Starkiller Base exterior or space dock
- **Secondary Layers**: Stars, distant planets, passing ships
- **Parallax Elements**: Moving clouds, atmospheric effects

### Additional Visual Elements
- **Imperial Insignia/First Order Symbol**: For branding
- **Officer Rank Indicators**: For player status
- **Security Clearance Visuals**: For access code representation
- **Cargo Icons**: Simple representations of different cargo types

## Animation Requirements (Simple)

- **Button Highlights**: Subtle glow or color change on hover
- **Approval/Denial**: Stamp animation or effect
- **Ship Arrival**: Simple entry animation for new ships
- **Feedback Flash**: Brief screen effect for correct/incorrect
- **Terminal Boot**: Startup sequence for game beginning

## Audio Assets Needed

- **UI Sounds**:
  - Button clicks
  - Terminal beeps
  - Alert sounds
- **Gameplay Sounds**:
  - Approval stamp
  - Denial buzz
  - Error alarm (for strikes)
  - Success confirmation
- **Ambient Audio**:
  - Imperial terminal background hum
  - Distant ship sounds
  - Base ambience
- **Music**:
  - Imperial-themed background music (subtle)
  - Tension music for difficult decisions
  - Game over theme

## Technical Art Notes

### Canvas Setup
- **Resolution**: Design for 16:9 aspect ratio (1920x1080)
- **Scaling**: Use Canvas Scaler set to "Scale With Screen Size"
- **Reference Resolution**: 1920x1080
- **UI Scale Mode**: Scale with screen size
- **Match Width or Height**: 0.5 (balanced)

### Asset Specifications
- **Texture Format**: PNG (for transparency) or JPG (for backgrounds)
- **Resolution**: Create at 2x target size for high DPI displays
- **Text Elements**: Use TextMeshPro for crisp text rendering
- **Button States**: Create normal, highlighted, pressed, and disabled states

### 2D Implementation Notes
- **Sorting Layers**: Create dedicated layers for:
  - Background (lowest)
  - Midground
  - UI Elements
  - Foreground Effects (highest)
- **Sprite Pivots**: Center for most UI elements
- **Slicing**: Use 9-slice where appropriate for scalable UI elements

## Visual Reference Points

### Imperial Console Styling
- Clean, geometric designs
- Angular elements
- Minimalist approach
- Monospaced fonts
- Status lights and indicators
- Sharp contrast between elements

### Star Wars Officer Terminals
- Dark backgrounds
- Glowing text and icons
- Functional rather than decorative
- Grid-based layouts
- Clear hierarchy of information
- Limited color palette

## Implementation Priority

1. **Core UI Framework** - Basic terminal and panels
2. **Ship Representation** - Simple icons/silhouettes
3. **Decision Interface** - Approve/deny buttons and feedback
4. **Log Book Design** - Reference materials
5. **Visual Polish** - Effects, transitions, and details

## Art Integration Process

1. Create assets at recommended sizes
2. Import into Unity project
3. Set appropriate import settings (compression, format)
4. Assign to UI elements through the Inspector
5. Adjust anchors and positions to fit layout
6. Test at different resolutions

## Placeholder Strategy

Until final art is ready, we recommend using:
- Simple colored blocks for layout
- Basic shapes for ships
- Plain text for information
- Standard Unity buttons

This will allow testing of gameplay while art assets are being developed.
