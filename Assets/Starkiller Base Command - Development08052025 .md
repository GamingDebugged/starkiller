# Starkiller Base Command - Development Plan

## Project Overview
Starkiller Base Command is a management game where players make decisions about approving or denying ships while balancing professional duty with personal needs. Players face increasingly complex rules and scenarios while dealing with the consequences of their decisions, managing their family's needs, and navigating a story that can lead to either promotion within the Imperium or escape with the insurgents.

## Project Knowledge Management

### Core Files for Cross-Reference
Due to limitations in the Project Knowledge capacity, files will need to be swapped in and out depending on which system we're working on. This section catalogs key files that may need to be referenced across different implementation tasks.

#### Core System Files
- **GameManager.cs** - Core game logic and state management (1000+ lines)
- **CredentialChecker.cs** - Ship decision processing (1000+ lines)
- **ShipEncounterGenerator.cs** - Ship encounter creation (1000+ lines)
- **MasterShipGenerator.cs** - Enhanced ship system (900+ lines)
- **StarkkillerContentManager.cs** - Content and data management (800+ lines)
- **StarkkillerLogBookManager.cs** - Rule book UI (1000+ lines)

#### Family System Files
- **ImperialFamilySystem.cs** - Family management (900+ lines)
- **FamilyStatusInfo.cs** - Family data container (400 lines)
- **FamilyDisplayManager.cs** - Family UI management (350 lines)
- **FamilyMemberDisplay.cs** - Individual family member UI (200 lines) 
- **FamilyDisplaySetup.cs** - UI setup helper (200 lines)
- **FamilyMemberHologram.cs** - Holographic display (200 lines)
- **HolographicFamilyDisplay.cs** - Hologram system (250 lines)

#### Consequences System Files
- **Consequence.cs** - ScriptableObject definition (150 lines)
- **StarkillerBaseCommand/Consequence.cs** - Namespaced version (150 lines)
- **SecurityConsequenceManager.cs** - Security incidents (400 lines)
- **ConsequenceCompatibilityManager.cs** - Bridge between implementations (150 lines)

#### Game Flow Files
- **DailyBriefingManager.cs** - Day start briefings (550 lines)
- **DailyReportManager.cs** - End of day reports (400 lines)
- **TimeManager.cs** - Game time control (250 lines)
- **GameStateController.cs** - Game state transitions (300 lines)

#### UI Files
- **StarkkillerLogBookManager.cs** - Rule book management (1000+ lines)
- **FamilyDisplayManager.cs** - Family UI handling (350 lines)
- **DailyBriefingManager.cs** - Daily briefing UI (550 lines)
- **DailyReportManager.cs** - Daily report UI (400 lines)

### File Request Strategy
When implementing a feature, we should request:

1. **Primary System Files** - Direct files for the system being worked on
2. **Integration Point Files** - Files that will be modified to connect with the system
3. **Reference Files** - Files that contain patterns or code examples needed

For example, when working on the Consequences System, we should request:
- All Consequences System Files
- GameManager.cs
- DailyReportManager.cs
- ImperialFamilySystem.cs

## File Structure & Organization

### Core Game Systems
- **GameManager.cs** - Central manager for game state and progression
- **CredentialChecker.cs** - Handles ship approval/denial decisions
- **StarkkillerLogBookManager.cs** - Manages the rule book and references
- **ShipEncounterGenerator.cs** - Generates ship encounters
- **MasterShipGenerator.cs** - Updated ship encounter generation system
- **DailyBriefingManager.cs** - Handles daily briefing screens
- **DailyReportManager.cs** - Manages end-of-day reports
- **TimeManager.cs** - Controls game time flow
- **GameStateController.cs** - Manages transitions between game states

### Family System Files
- **ImperialFamilySystem.cs** - Core class for family management
- **FamilyStatusInfo.cs** - Data container for family status
- **FamilyDisplayManager.cs** - Manages family UI display
- **FamilyMemberDisplay.cs** - UI for individual family members
- **FamilyDisplaySetup.cs** - Helper for setting up family displays
- **FamilyMemberHologram.cs** - Holographic family member display
- **HolographicFamilyDisplay.cs** - Advanced holographic family display

### Consequences System Files
- **Consequence.cs** - ScriptableObject for consequence definitions
- **SecurityConsequenceManager.cs** - Manages security incidents and their effects
- **ConsequenceCompatibilityManager.cs** - Bridges different consequence implementations

### Systems To Implement/Enhance
- **ConsequenceManager.cs** - Enhanced system to manage all types of consequences
- **StoryManager.cs** - Handles story progression and key events
- **RuleProgressionManager.cs** - Controls rule changes across days
- **GameEndingManager.cs** - Manages different game endings
- **CutsceneManager.cs** - Handles story cutscenes and videos
- **ImperialInspectionSystem.cs** - Manages inspection events

## System Implementation Plans

### 1. Consequences System Enhancement
**Current Status:** Partially implemented with multiple versions and compatibility concerns. We need to unify and enhance this system.

**Goals:**
- Consolidate the two Consequence implementations into a single, unified system
- Enhance the SecurityConsequenceManager to handle all consequence types
- Create a comprehensive UI for consequences
- Develop long-term consequence tracking and compound effects
- Connect consequences more deeply to family impacts and story progression

**Implementation Tasks:**
1. **Consequence System Consolidation**
   - Review both Consequence implementations and select the most feature-complete
   - Migrate any unique features from the secondary implementation
   - Update all references to use the consolidated class
   - Eliminate the need for the compatibility manager
   
2. **ConsequenceManager Implementation**
   - Enhance existing SecurityConsequenceManager into a full ConsequenceManager
   - Add support for different consequence types beyond security
   - Implement categorization for consequences (security, financial, family, etc.)
   - Create a persistent consequence history system
   
3. **Consequence UI Enhancement**
   - Design and implement improved report panels for different severity levels
   - Add visual indicators for active consequence effects
   - Create a consequences log accessible via the daily report
   - Add animations and sound effects for consequence notifications
   
4. **Long-term Consequence Effects**
   - Implement multi-day effects from consequences
   - Create compound effects when similar consequences occur repeatedly
   - Design escalation mechanics for repeated mistakes
   - Add special events triggered by consequence combinations

**Integration Points:**
- `GameManager.OnDecisionMade()` - Hook for triggering consequences
- `DailyReportManager` - Enhanced display of consequence reports
- `ImperialFamilySystem` - Deeper integration of family effects
- `DailyBriefingManager` - Add consequence updates to briefings

**Project Knowledge Files Needed:**
- Primary: All Consequences System Files
- Integration: GameManager.cs, DailyReportManager.cs 
- Reference: SecurityConsequenceManager.cs, ImperialFamilySystem.cs

### 2. Family System Enhancement
**Current Status:** Well-implemented with core mechanics, UI, and family member management. Needs expansion of events, storylines, and deeper game integration.

**Goals:**
- Create more dynamic family events tied to game decisions
- Implement individual family member story arcs
- Add family loyalty and risk tracking
- Create more meaningful family interactions and decisions
- Enhance the connection between family status and gameplay consequences

**Implementation Tasks:**
1. **Family Event System Expansion**
   - Create a database of family events with varying severity levels
   - Implement event chains that can progress over multiple days
   - Add decision points within family events
   - Create varying outcomes based on credits spent/choices made
   
2. **Family Member Storylines**
   - Implement individual arcs for each family member:
     - Emma (Officer): Career progression vs. growing doubts about Imperium
     - Kira (Mechanic): Technical innovation vs. resistance sympathies
     - Jace (Cadet): Training excellence vs. ethical questions
     - R2-D4 (Droid): Loyalty programming vs. data access
   - Create key decision points in each storyline
   - Implement consequences for each path
   
3. **Family Loyalty System**
   - Add individual loyalty tracking for each family member
   - Implement events that can pull family members toward rebellion
   - Create mechanics for Imperium monitoring of family loyalty
   - Add risk of family member detention/reassignment
   
4. **Enhanced Family UI**
   - Improve the family panel with more detailed status information
   - Add conversation options with family members
   - Create interactive hologram communication events
   - Implement family message log

**Integration Points:**
- `ImperialFamilySystem.ProcessDay()` - Enhance with storyline progression
- `ConsequenceManager` - Connect consequences to family impacts
- `GameManager` - Add family decision tracking
- `StoryManager` - Link family storylines to main story progression

**Project Knowledge Files Needed:**
- Primary: All Family System Files
- Integration: GameManager.cs, ConsequenceManager.cs (new)
- Reference: DailyReportManager.cs, DailyBriefingManager.cs

### 3. Dynamic Rules System
**Purpose:** Increase complexity and challenge as game progresses.

**Implementation:**
- Create `RuleProgressionManager.cs` to manage rule changes
- Implement rule sets for different phases of the game:
  - **Phase 1 (Days 1-3)**: Basic access code validation
  - **Phase 2 (Days 4-7)**: Add origin validation
  - **Phase 3 (Days 8-12)**: Add manifest checking
  - **Phase 4 (Days 13+)**: Add contraband inspection
- Design "inspection day" events with stricter enforcement
- Create readable rule explanations for daily briefing
- Implement rule-based quiz questions during inspections

**Integration Points:**
- `DailyBriefingManager` - Display changing rules clearly
- `ShipEncounterGenerator` - Generate ships that test specific rules
- `StarkkillerContentManager` - Store rule definitions
- `CredentialChecker` - Apply current rule set for validation

**Project Knowledge Files Needed:**
- Primary: New RuleProgressionManager.cs, DailyRulesGenerator.cs
- Integration: DailyBriefingManager.cs, StarkkillerContentManager.cs 
- Reference: CredentialChecker.cs, ShipEncounterGenerator.cs

### 4. Story Progression
**Purpose:** Create narrative cohesion and meaningful progression.

**Implementation:**
- Create `StoryManager.cs` to track overall story arc
- Implement dual story paths:
  - **Imperium Loyalty Path** - Rising through the ranks
  - **Insurgent Sympathy Path** - Secret rebellion support
- Create special encounters that advance the storyline
- Design key decision points that significantly impact story
- Add character interactions with superiors and insurgent contacts
- Implement story phases (orientation, escalation, climax)

**Integration Points:**
- `ShipEncounterGenerator` - Create and flag story encounters
- `GameManager` - Track key decisions and loyalty metrics
- `DailyBriefingManager` - Add story updates
- `ImperialFamilySystem` - Connect family events to story progression
- `ConsequenceManager` - Link consequences to story impact

**Project Knowledge Files Needed:**
- Primary: New StoryManager.cs and story-related files
- Integration: GameManager.cs, ShipEncounterGenerator.cs, ImperialFamilySystem.cs
- Reference: DailyBriefingManager.cs, ConsequenceManager.cs

### 5. Dual Endings
**Purpose:** Provide meaningful conclusion based on player choices.

**Implementation:**
- Create `GameEndingManager.cs` to handle game conclusions
- Implement two main endings:
  - **Imperial Promotion Ending** - Rise through Imperium ranks
  - **Insurgent Escape Ending** - Escape with family to join rebellion
- Design requirements for each ending (loyalty metrics, key decisions)
- Create ending cutscenes and sequences
- Implement epilogue system showing impact of decisions

**Integration Points:**
- `GameManager` - Track conditions for ending triggers
- `StoryManager` - Direct to appropriate ending based on choices
- `ImperialFamilySystem` - Family status impacts available endings
- `ConsequenceManager` - Consequences affect ending availability

**Project Knowledge Files Needed:**
- Primary: New GameEndingManager.cs and ending-related files
- Integration: GameManager.cs, StoryManager.cs
- Reference: ImperialFamilySystem.cs, TimeManager.cs, GameStateController.cs

### 6. Polish Elements
**Purpose:** Enhance player experience and immersion.

**Implementation:**
- Create `CutsceneManager.cs` for key story moments
- Implement `ImperialInspectionSystem.cs` for surprise inspections
- Add sound effects for all interactions
- Create options menu for settings
- Implement credits sequence and acknowledgments
- Add tutorial system for new players

**Integration Points:**
- `GameStateController` - Handle game state during cutscenes
- `TimeManager` - Pause during important events
- `GameManager` - Trigger inspections and special events

**Project Knowledge Files Needed:**
- Primary: New CutsceneManager.cs, ImperialInspectionSystem.cs
- Integration: GameStateController.cs, GameManager.cs
- Reference: TimeManager.cs, UI reference files

## Development Priorities & Timeline

### Sprint 1: Core Systems Enhancement (2 weeks)
1. **Consequences System Enhancement & Consolidation** (1 week)
   - Consolidate Consequence implementations
   - Enhance ConsequenceManager functionality
   - Improve consequence UI and reporting

2. **Family System Storylines** (1 week)
   - Implement family member story arcs
   - Add family event chains
   - Enhance family UI

### Sprint 2: Narrative & Rules (2 weeks)
3. **Dynamic Rules System** (1 week)
   - Create RuleProgressionManager
   - Implement rule sets for game phases
   - Update DailyBriefingManager to explain new rules

4. **Story Progression** (1 week)
   - Implement StoryManager
   - Create special story encounters
   - Design key decision points

### Sprint 3: Endings & Polish (2 weeks)
5. **Dual Endings** (1 week)
   - Create GameEndingManager
   - Implement ending requirements
   - Design ending sequences

6. **Polish Elements** (1 week)
   - Add CutsceneManager
   - Implement inspections
   - Add sound effects and options
   - Create tutorial

## Implementation Notes

### Consequence System Implementation
The Consequence system should be consolidated using the most feature-complete implementation. Based on the files provided, the StarkillerBaseCommand.Consequence version appears to be more structured, with clearer severity levels, types, and effects. The consolidation should:

1. Choose the most complete Consequence class (likely the namespace version)
2. Migrate any unique features from the other version
3. Update all asset references to use the consolidated class
4. Eliminate the need for the compatibility manager

The ConsequenceManager should expand on the existing SecurityConsequenceManager to handle all types of consequences beyond just security incidents. It should track:

- Security breaches
- Family impacts
- Loyalty shifts
- Financial penalties
- Special events triggered by consequences

The UI should be enhanced to show:
- Immediate consequences of decisions
- Daily summary of consequences
- Long-term effects tracking
- Visual severity indicators

### Family System Expansions
The existing family system is well-implemented but needs more narrative depth. Key enhancements include:

1. Individual storylines for each family member that progress over multiple days
2. Family loyalty tracking separate from the player's loyalty
3. Risk mechanics where family members could be reassigned or detained
4. More visual representations of family status changes
5. Enhanced holographic communication options

### Technical Considerations
- Use ScriptableObjects for data-driven design
- Implement event-based communication between systems
- Ensure clean separation of concerns
- Document all new public methods and classes
- Create editor tools for content creation

## Data Files & Resources

### ScriptableObject Files
The game uses ScriptableObjects extensively. Key assets include:

- **Consequence Assets**: 
  - UnauthorizedShipAccess.asset
  - SecurityProtocolBreach.asset
  - InsurgentInfiltration.asset
  - WifeInjured.asset

- **Ship Type Assets**:
  - Various ship types in the Resources/_ScriptableObjects/ShipTypes folder

- **Captain Type Assets**:
  - Various captain types in the Resources/_ScriptableObjects/CaptainTypes folder

- **Scenario Assets**:
  - Various scenarios in the Resources/_ScriptableObjects/Scenarios folder

### CSV Data Files
Data is also stored in CSV format for easy editing:

- **FAMILY_SYSTEM.csv** - Family member data
- **CONSEQUENCES.csv** - Consequence definitions
- Other CSV files for game content

### Unity Prefabs & UI Elements
The game uses several prefab types:

- **UI Prefabs**: 
  - FamilyMemberPrefab
  - ShipInfoPrefab
  - ConsequenceReportPrefab

- **ScriptableObject Templates**:
  - ConsequenceTemplate
  - ShipTypeTemplate
  - CaptainTypeTemplate

## Version Control & Collaboration Strategy

### Branch Structure
- **main**: Stable, playable version
- **develop**: Integration branch for features
- **feature/[feature-name]**: Individual feature branches

### Merge Strategy
- Complete features in dedicated branches
- Merge to develop when feature is complete
- Test thoroughly on develop
- Merge to main for stable releases

### Code Review Checklist
- Documentation complete
- Error handling implemented
- Performance considerations addressed
- UI/UX elements consistent
- Integration points properly connected

## Key Questions for Implementation

1. Consequences System:
   - Should we merge the two Consequence implementations or rebuild?
   - How detailed should consequence cascades be?
   - Should consequences directly affect gameplay mechanics?

2. Family System:
   - How complex should family member storylines be?
   - Should family members have independent loyalty metrics?
   - What level of risk should family members face?

3. Story Progression:
   - How linear vs. branching should the storyline be?
   - How many key decisions should impact the ending?
   - Should there be "point of no return" decisions?

4. Game Balance:
   - How quickly should game difficulty increase?
   - What balance of rewards vs. penalties works best?
   - How forgiving should the game be with mistakes?