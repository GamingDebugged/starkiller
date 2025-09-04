# Game Design Document - Chapter: Narrative Flow

**Game:** Starkiller Base Command  
**Document Version:** 1.0  
**Date:** August 1st, 2025  
**Author:** Claude & Development Team

---

## 1. Narrative Overview

### Core Concept
Players control a docking bay officer at Starkiller Base over a 30-day campaign. Through daily decisions about which ships to approve, deny, or investigate, players navigate between Imperial loyalty and personal survival while supporting their family. The narrative uses pre-rendered videos with scripted scenarios leading to 10 distinct endings.

### Narrative Pillars
1. **Moral Compromise** - Every decision has consequences
2. **Family Pressure** - Financial and safety needs drive difficult choices  
3. **Cascading Consequences** - Today's mercy becomes tomorrow's crisis
4. **Political Spectrum** - Choices pull player between Rebellion and Empire

---

## 2. Ending Structure (10 Endings)

The game features 10 authored endings along a political spectrum from Rebel sympathizer to Imperial zealot:

### Rebel Endings (Far Left)
1. **"Freedom Fighter"** - Successfully escape with family to rebel base after actively aiding the Rebellion
2. **"Martyr"** - Family escapes to safety while you stay behind to ensure their freedom

### Survivor Endings (Left-Center)  
3. **"Refugee"** - Flee to the frontier with family, starting over with nothing but freedom
4. **"Underground"** - Remain at post while secretly helping resistance, family hidden

### Neutral Endings (Center)
5. **"Gray Man"** - Keep head down, avoid all sides, family barely survives
6. **"Compromised"** - Under investigation but not yet caught, future uncertain

### Imperial Endings (Right-Center)
7. **"Good Soldier"** - Promoted for loyalty, family struggles financially but remains safe  
8. **"True Believer"** - Honored by Empire, but family relationships strained by your choices

### Zealot Endings (Far Right)
9. **"Bridge Commander"** - Ultimate promotion achieved by sacrificing everything, including family
10. **"Imperial Hero"** - Public commendation for exposing rebels, family "relocated for protection"

---

## 3. 30-Day Campaign Structure

### Days 1-10: Foundation Phase
- **Narrative Goals:** Establish routine, introduce game mechanics, plant consequence seeds
- **Family Pressure:** Minor (child needs school supplies, partner mentions bills)
- **Key Scenarios:** Tutorial encounters, first recurring captain, minor smuggler
- **Consequences:** Hidden tokens placed for later activation

### Days 11-20: Escalation Phase  
- **Narrative Goals:** Consequences emerge, loyalties tested, stakes rise
- **Family Pressure:** Moderate (medical bills, housing costs, "protection" fees)
- **Key Scenarios:** First inspection, returning captains with memory, moral dilemmas
- **Consequences:** Early decisions create visible problems

### Days 21-30: Resolution Phase
- **Narrative Goals:** Major choices, point of no return, ending determination
- **Family Pressure:** Critical (family member detained, eviction threatened, medical emergency)
- **Key Scenarios:** Final loyalty tests, investigation climax, ending branch scenarios
- **Consequences:** Full cascade of previous choices

---

## 4. Recurring Captain System

### Captain Return Schedule
Captains can appear 3-4 times maximum across the 30-day campaign, with at least 5 days between appearances (travel time).

### Relationship States
Each recurring captain tracks their relationship with the player:
- **Friendly** - Previously helped (approved/bribed)
- **Neutral** - No strong previous interaction  
- **Hostile** - Previously hindered (denied/detained)

### Dialog Adaptation
CaptainType ScriptableObjects include relationship-based dialog:
```
- returningAfterApproval: "Thanks to you, my business thrives..."
- returningAfterDenial: "I hope you're more reasonable today..."  
- returningAfterHolding: "Those delays cost me thousands..."
- returningAfterTractorBeam: "My ship still bears the damage..."
- returningAfterBribery: "I knew we understood each other..." *wink*
```

### Memory Impact
Returning captains offer different opportunities based on relationship:
- **Friendly:** Bigger bribes, useful information, assistance in crisis
- **Hostile:** Threats, reports to superiors, revenge scenarios
- **Neutral:** Standard interactions

---

## 5. Family Pressure System

### "Behind the Curtain" Design
Family members never appear directly. Their story is told through:
- **Medical Officials:** "Your partner's treatment requires immediate payment"
- **School Administrators:** "Your child's tuition is overdue by 30 days"
- **Housing Authority:** "Quarters maintenance fee has increased"
- **Imperial Bureaucrats:** "Family residency permits require renewal"

### Pressure Escalation
- **Week 1:** Gentle reminders, small expenses (10-50 credits)
- **Week 2:** Urgent needs, moderate costs (100-200 credits)
- **Week 3:** Crisis situations, major expenses (300-500 credits)
- **Week 4:** Desperate circumstances, impossible costs (1000+ credits)

### Consequence Integration
Family pressure appears through:
- **InspectionPanel:** Officials arrive with bad news
- **PersonalDataLog:** Family chat shows growing desperation
- **Credit Costs:** Increasing mandatory expenses

---

## 6. Consequence System

### Consequence Tokens
Each significant decision creates hidden tokens that trigger future events:
```
Decision: Approve smuggler → Token: "SMUGGLER_APPROVED_DAY5"
Later Trigger: Day 8 → Scenario: "Contraband discovered in shipment"
```

### Consequence Chains
Multi-day story arcs based on player actions:
1. **Day 3:** Approve medical supply ship with discrepancies
2. **Day 7:** News report of black market medical supplies
3. **Day 10:** Investigation into medical supply approvals
4. **Day 13:** Inspection of your station

### Visibility Through PersonalDataLog
Three feeds provide foreshadowing and consequence visibility:

**Imperium News:**
- Security breaches from your approvals
- Rebel activities using your helped resources
- New protocols responding to your corruption

**Family Chat:**
- Increasing desperation about finances
- Mentions of neighbors disappearing  
- Children asking difficult questions

**Frontier E-Zine:**
- Black market price changes
- Trade route disruptions
- Rumors of purges

---

## 7. Point of No Return System

### Concept
Around Day 23-27, players face scenarios that lock in their ending path. These are dramatic moral choices that clearly align the player with a faction.

### Example Point of No Return Scenarios:

**Rebel Path Lock:**
- Allow known spy to escape with Death Star Starkiller plans
- Help transport ship full of Force-sensitive children

**Imperial Path Lock:**
- Report your own family as rebel sympathizers
- Execute direct order to destroy refugee ship

**Neutral Path Lock:**
- Refuse both Imperial and Rebel pressure
- Focus solely on family survival

### Mechanical Impact
Once triggered, these scenarios:
- Lock out certain endings
- Unlock specific final week scenarios
- Determine which faction contacts you
- Set up the specific ending sequence

---

## 8. Story Element Integration

### Scenario Weighting
Beyond individual decisions, certain scenarios provide major alignment shifts:
- **"The Defector"** - Imperial officer seeking rebel protection (+5 Rebel)
- **"The Purge Order"** - Command to detain all non-humans (+5 Imperial)
- **"The Child Transport"** - Families fleeing persecution (+3 Rebel)
- **"The Weapons Inspector"** - Routine becomes loyalty test (+3 Imperial)

### Decision Templates
Pre-authored decision outcomes ensuring consistent narrative:
```yaml
Decision: "Approve Refugee Ship"
- Imperial Loyalty: -3
- Rebel Sympathy: +5  
- Credits: -50 (bribe refused)
- Consequence Token: "REFUGEES_SAVED"
- Family Impact: "Hope" status increase
```

### Narrative Manager Tracking
Continuously monitors:
- Loyalty balance (Imperial vs Rebel)
- Corruption level (bribes taken)
- Family status (safe/threatened/desperate)
- Suspicion level (how closely watched)

---

## 9. Implementation Notes

### Technical Constraints
- 10-20 total endings (video production limit)
- 30-day campaign (scope limit)
- 3-4 appearances per recurring captain (believable travel time)
- Pre-rendered videos require pre-planned scenarios

### Content Requirements
- 15-20 unique ship scenarios
- 5-7 recurring captains with relationship branches
- 30-40 family pressure events
- 50+ PersonalDataLog news items
- 10 ending sequences with unique videos

### Balancing Considerations
- Bribe amounts vs family expenses
- Frequency of moral dilemmas
- Pacing of consequence reveals
- Difficulty of achieving each ending

---

## 10. Narrative Flow Example

**Day 1-5:** Player learns mechanics, meets Captain Vega (smuggler)  
**Day 6:** Small family expense - child needs medicine (50 credits)  
**Day 8:** Vega returns, offers 100 credit bribe, player accepts  
**Day 10:** Imperium News reports increased smuggling  
**Day 13:** Family expense - rent increase (200 credits)  
**Day 15:** Investigation scenario triggered by smuggling  
**Day 17:** Vega returns again, desperate, offers 300 credits  
**Day 20:** Major family crisis - partner needs surgery (500 credits)  
**Day 23:** Point of No Return - Help Vega escape with rebel plans  
**Day 25:** Marked as rebel sympathizer, escape plan begins  
**Day 28:** Final preparation, family packing  
**Day 30:** Ending sequence - "Freedom Fighter" escape

---

*This narrative design creates meaningful player agency within authored constraints, using family pressure as the emotional engine driving moral compromise. The recurring captain system and consequence chains ensure that every playthrough feels personal and consequential.*