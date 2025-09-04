# Starkiller Base Command - Project Brief

## Game Concept
A Star Wars themed document-checking game inspired by "Papers, Please" where the player is the Access Manager for the Starkiller Base. The player must review incoming ship credentials and either approve or deny them access to the base or nearby planet.

### Core Gameplay Loop
1. Ship arrives with a story and credentials
2. Player checks credentials against requirements in a log book
3. Player decides to approve or deny access
4. Consequences occur based on decision correctness
5. Repeat with new ships

## Game Elements

### Player Role
Access Manager who controls which ships can access Starkiller Base or the nearby shield generator planet.

### Credentials to Check
- Ship type (compared against daily approved list)
- Access code (changes daily)
- Destination (Base or Planet)
- Origin (approved sources for each destination)
- Manifest (cargo being transported)

### Potential Ship Situations
- Ships may be on time, late, or early
- Ships may report being attacked by pirates, rebels, empire, or trade officials
- Ships may have suffered damage from storms, meteors, or mechanical failures
- Ships may have been boarded by hostiles
- Ships may be rebels in disguise attempting to infiltrate
- Ships may be smuggling contraband or people

### Win/Loss Conditions
- Small errors result in a "strike" (lose a pip)
- Three strikes results in termination (game over)
- Major errors (letting rebels aboard) results in immediate game over
- Success increases score

## Technical Implementation

### Unity Project Type
- 2D Universal Project
- UI-based gameplay
- Potential for parallax effects in background

### Core Scripts

#### 1. GameManager.cs
Handles overall game state, scoring, and game flow.
```
- Tracks game state (active, game over)
- Manages strikes/chances system
- Controls game flow between encounters
- Processes success/failure outcomes
```

#### 2. ShipEncounterSystem.cs
Generates random ship encounters with varied scenarios.
```
- Creates ship data with varied parameters
- Generates valid and invalid credentials
- Maintains daily logs for reference
- Controls difficulty progression
```

#### 3. CredentialChecker.cs
Handles the credential verification UI and logic.
```
- Displays ship information and credentials
- Provides log book reference material
- Processes player decisions
- Shows feedback on decisions
```

#### 4. UIManager.cs
Manages UI navigation and transitions.
```
- Controls panel switching
- Handles button clicks
- Manages transitions between game states
- Controls audio for UI elements
```

### UI Elements
- Main Menu Panel
- Gameplay Panel
- Ship Info Display
- Credentials Panel
- Log Book Panel
- Decision Buttons (Approve/Deny)
- Feedback Display
- Game Over Panel

## Development Timeline (12 Hours)

### Day 1 (6 Hours)
1. **Setup (1 hour)**
   - Create Unity project
   - Import scripts
   - Set up basic UI framework

2. **Core Gameplay (3 hours)**
   - Implement GameManager
   - Configure ShipEncounterSystem
   - Create basic ship data
   - Set up credential checking

3. **Basic UI (2 hours)**
   - Create interactive UI elements
   - Implement log book system
   - Set up feedback system

### Day 2 (6 Hours)
1. **Visuals and Audio (2 hours)**
   - Add Star Wars themed art assets
   - Implement sound effects
   - Create visual feedback

2. **Content Creation (2 hours)**
   - Create varied ship encounters
   - Expand credential database
   - Add story elements

3. **Testing and Polish (2 hours)**
   - Test gameplay flow
   - Fix bugs
   - Balance difficulty
   - Final polish

## Collaboration Approach
- Developer focusing on code implementation and debugging
- Artist focusing on visual elements, UI design, and asset creation
- Regular check-ins to align on progress and adjust scope if needed

## Development Notes

### Design Priorities
1. Focus on core mechanics first (credential checking)
2. Add depth gradually through varied scenarios
3. Maintain a Star Wars aesthetic throughout
4. Simple UI that clearly shows required information
5. Progressively increasing complexity

### Expansion Ideas (Post-MVP)
- Ship scanning mechanic
- Reputation system with different factions
- Character progression/customization
- Time pressure elements
- Economy system (getting paid per correct decision)

## Reference Images/Inspiration
- Papers, Please (gameplay mechanics)
- Star Wars Imperial aesthetic (visual style)
- Scene from Return of the Jedi with Imperial officer allowing rebels to land on Endor

## Technical Requirements
- Unity 2D
- UI navigation system
- Random scenario generation
- Decision-based gameplay
- Simple sound system

## Art Requirements
- Imperial-styled UI elements
- Ship icons/images
- Background visuals (Starkiller Base)
- Button designs (approve/deny)
- Log book visual design
- Feedback visuals

## Implementation Notes
- Start with placeholder art to test functionality
- Use modular code for easy expansion
- Focus on core loop before adding complexity
- Create an easily expandable scenario system
- Prioritize clear feedback to player decisions
