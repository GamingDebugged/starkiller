# Starkiller Base Command - Implementation Guide

This guide will help you set up the enhanced game in Unity using the updated scripts.

## Project Setup

1. **Create a new Unity project** (2D or 3D, whichever you prefer)
2. **Import required assets:**
   - UI elements (buttons, panels, etc.)
   - Audio files (button clicks, approvals, denials)
   - Any Star Wars-themed visuals you want to use

## Scene Structure

1. **Create your Game Scene with these GameObjects:**

```
- Main Camera
- Canvas (UI)
  - MainMenuPanel
    - Title
    - StartGameButton
    - TutorialButton
    - CreditsButton
    - QuitButton
  - GameplayPanel
    - GameStatusText
    - ShiftTimerText
    - ShipInfoPanel
    - CredentialsPanel
    - ShipImage
    - ScannerEffect
    - LogBookButton
    - LogBookPanel
      - TabRegulations
      - TabShips
      - TabCodes
      - TabContent
    - ApproveButton
    - DenyButton
    - BribeButton
    - FeedbackPanel
    - ApprovedStamp
    - DeniedStamp
  - FamilyPanel
    - FamilyMemberContainer
    - FamilyStatusText
    - CloseButton
  - DailyReportPanel
    - SalaryText
    - ExpensesText
    - CreditsText
    - ContinueButton
  - MoralChoicePanel
    - ChoiceText
    - Option1Button
    - Option2Button
  - BriefingPanel
    - DailyBriefingText
    - ContinueButton
  - GameOverPanel
    - GameOverText
    - FinalScoreText
    - RestartButton
  - TutorialPanel
    - TutorialText
    - BackButton
  - CreditsPanel
    - CreditsText
    - BackButton
- GameManager
- UIManager
- ShipEncounterSystem
- FamilySystem
```

## Script Assignment

1. **Add the updated scripts to your project**
2. **Assign scripts to GameObjects:**
   - Add `GameManager.cs` to the GameManager GameObject
   - Add `UIManager.cs` to the UIManager GameObject
   - Add `ShipEncounterSystem.cs` to the ShipEncounterSystem GameObject
   - Add `CredentialChecker.cs` to the GameplayPanel GameObject
   - Add `FamilySystem.cs` to the FamilySystem GameObject

## Component Configuration

### GameManager

1. **Set up UI References:**
   - Drag GameStatusText into the "Game Status Text" field
   - Drag ShiftTimerText into the "Game Timer Text" field
   - Drag DailyBriefingText into the "Daily Briefing Text" field
   - Drag GameOverPanel into the "Game Over Panel" field
   - Drag FinalScoreText into the "Final Score Text" field
   - Drag RestartButton into the "Restart Button" field
   - Drag DailyReportPanel into the "Daily Report Panel" field
   - Drag SalaryText into the "Salary Text" field
   - Drag ExpensesText into the "Expenses Text" field
   - Drag ContinueButton into the "Continue Button" field
   - Drag MoralChoicePanel into the "Moral Choice Panel" field

2. **Set up Component References:**
   - Drag the CredentialChecker component into the "Credential Checker" field
   - Drag the FamilySystem component into the "Family System" field

3. **Configure Game Settings:**
   - Set Max Mistakes (default 3)
   - Set Time Between Ships (default 2 seconds)
   - Set Shift Time Limit (default 360 seconds - 6 minutes)
   - Set Required Ships Per Day (default 8)
   - Set Base Salary (default 30)
   - Set Bonus Per Extra Ship (default 5)
   - Set Penalty Per Mistake (default 5)

4. **Configure Family Settings:**
   - Set Rent Cost (default 20)
   - Set Food Cost (default 15)
   - Set Heat Cost (default 10)
   - Set Medicine Cost (default 30)

### ShipEncounterSystem

This system needs data to generate ship encounters. You'll need to set up these arrays in the Inspector:

1. **Ship Types:**
   - Add several ship types with names and possible stories
   - Example: "Lambda Shuttle" with stories like "Delivering officers from Command"

2. **Destinations:**
   - Add destinations like "Starkiller Base" and "Shield Generator Planet"
   - For each, specify valid origins

3. **Origins:**
   - Add origins like "Imperial Center", "Kuat Shipyards", etc.

4. **Cargo Items:**
   - Add items like "Power Converters", "Blaster Parts", "Food Supplies"

5. **Illegal Cargo Items:**
   - Add items like "Unmarked Containers", "Unregistered Weapons", "Communication Jammers"

6. **Valid Access Codes:**
   - Add codes like "SK-1138", "SK-2187", etc.

7. **Story Ships:**
   - Add special story ships with specific narratives
   - Example:
     - Name: "Millennium Falcon"
     - Captain: "Unknown"
     - Story: "Ship appears to be heavily modified. Captain seems nervous."
     - Tag: "rebel"
     - Appear on Day: 3

### CredentialChecker

1. **UI References:**
   - Drag ShipInfoPanel into the "Ship Info Panel" field
   - Drag CredentialsPanel into the "Credentials Panel" field
   - Drag FeedbackText into the "Feedback Text" field
   - Drag FeedbackPanel into the "Feedback Panel" field
   - Drag ShipImage into the "Ship Image" field

2. **Reference Books:**
   - Drag LogBookPanel into the "Log Book Panel" field
   - Drag LogBookTabContent into the "Log Book Tab Content" field
   - Drag LogBookTabRegulations into the "Log Book Tab Regulations" field
   - Drag LogBookTabShips into the "Log Book Tab Ships" field
   - Drag LogBookTabCodes into the "Log Book Tab Codes" field

3. **Decision Interface:**
   - Drag ApproveButton into the "Approve Button" field
   - Drag DenyButton into the "Deny Button" field
   - Drag OpenLogBookButton into the "Open Log Book Button" field
   - Drag CloseLogBookButton into the "Close Log Book Button" field
   - Drag BribeButton into the "Bribe Button" field
   - Drag BribeText into the "Bribe Text" field

4. **Audio:**
   - Drag audio sources into respective fields
   - Add sound files for stamp, page turning, etc.

5. **Visual Effects:**
   - Drag ApprovedStamp into the "Approved Stamp" field
   - Drag DeniedStamp into the "Denied Stamp" field
   - Drag ShipScanner into the "Ship Scanner" field

### FamilySystem

1. **Family Members:**
   - Set up family members in the inspector or use the default implementation
   - Add names, relations, and portraits if available

2. **UI References:**
   - Drag FamilyStatusPanel into the "Family Status Panel" field
   - Drag FamilyMemberContainer into the "Family Member Container" field
   - Drag FamilyMemberPrefab into the "Family Member Prefab" field

3. **Game Settings:**
   - Set Sick Chance When No Heat (default 0.3)
   - Set Sick Chance When No Food (default 0.2)
   - Set Recovery Chance (default 0.3)

### UIManager

1. **Menu Panels:**
   - Drag each panel (MainMenuPanel, GameplayPanel, etc.) to its respective field

2. **Menu Buttons:**
   - Drag all UI buttons to their respective fields

## Family Member Prefab Setup

Create a prefab for family members with:

1. **Basic Components:**
   - RectTransform
   - Image (for background)
   - Text component for the name

2. **Child Objects:**
   - Portrait (Image component)
   - SickIcon (Image with medical symbol)
   - HungryIcon (Image with food symbol)
   - ColdIcon (Image with cold/temperature symbol)

## Creating Game Content

To make the game more engaging:

1. **Varied Ship Stories** - Add diverse narratives to the ShipType entries
2. **Interesting Destinations and Origins** - Create a rich universe with many locations
3. **Creative Cargo Items** - Add Star Wars themed cargo
4. **Story Ships** - Create special encounters that tell a larger story
5. **Family Events** - Add more varied family events in the FamilySystem
6. **Moral Dilemmas** - Write compelling moral choice scenarios

## Testing

1. Start in Play mode and check if the UI navigation works
2. Test the game flow from main menu → gameplay → daily report → next day
3. Verify that ship encounters generate properly
4. Test both correct and incorrect decisions
5. Check if strikes are counted properly
6. Verify the family system updates correctly
7. Test moral choice scenarios
8. Ensure the economic system works (salary, expenses, etc.)

## Next Steps After Basic Implementation

1. **Add visual styling** to match the Star Wars theme
2. **Implement ship scanning** feature as an expansion
3. **Add more complex credentials** to check
4. **Include narrative elements** like special events
5. **Add progression** with increasing difficulty
6. **Expand the family system** with more events and consequences
7. **Add faction reputation** tracking

## Troubleshooting Tips

- If components aren't working, check the console for error messages
- Verify all references are properly assigned in the Inspector
- Test each component individually by adding debug logs
- If UI elements aren't appearing, check Canvas settings and anchoring
- For family system issues, check if the family member prefab is set up correctly
- For moral choice issues, verify the button listeners are assigned properly
