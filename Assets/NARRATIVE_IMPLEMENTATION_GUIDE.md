# Narrative System Implementation Guide
**Date:** August 1st, 2025  
**Purpose:** Step-by-step implementation roadmap for the narrative system designed in GDD_Chapter_Narrative_Flow.md

---

## 🎯 IMPLEMENTATION PRIORITY ORDER

### Phase 1: Core Infrastructure (Week 1-2)
1. **Enhance CaptainType Dialog System**
2. **Create Family Pressure Manager**
3. **Extend Consequence Token System**
4. **Implement Narrative State Tracking**

### Phase 2: Content Integration (Week 3-4)
5. **Create Story Scenarios with Major Alignment Shifts**
6. **Implement Recurring Captain Memory System**
7. **Expand PersonalDataLog News Feeds**
8. **Create Point of No Return System**

### Phase 3: Ending System (Week 5-6)
9. **Implement Ending Determination Logic**
10. **Create Ending Sequence Triggers**
11. **Balance Testing and Tuning**

---

## 📋 DETAILED IMPLEMENTATION STEPS

### 1. Enhance CaptainType Dialog System

**File to Modify:** `Assets/Resources/_ScriptableObjects/CaptainTypes/*.asset`

**New Fields to Add:**
```csharp
[Header("Relationship-Based Dialog")]
[TextArea(2,4)] public string returningAfterApproval = "";
[TextArea(2,4)] public string returningAfterDenial = "";
[TextArea(2,4)] public string returningAfterHolding = "";
[TextArea(2,4)] public string returningAfterTractorBeam = "";
[TextArea(2,4)] public string returningAfterBribery = "";
```

**Implementation Notes:**
- Update CaptainType.cs script to include these fields
- Modify EncounterMediaTransitionManager to check captain relationship state
- Create enum: `CaptainRelationship { FirstMeeting, Friendly, Neutral, Hostile }`
- Store relationship data in save system

**Code Location:** `Assets/_scripts/CaptainType.cs` and related encounter systems

---

### 2. Create Family Pressure Manager

**New Script:** `Assets/_RefactoredScripts/Core/Managers/FamilyPressureManager.cs`

**Core Responsibilities:**
- Track family financial status and happiness
- Generate pressure events based on day and player actions
- Interface with PersonalDataLog for family chat updates
- Trigger InspectionPanel events for family crises

**Key Methods to Implement:**
```csharp
public class FamilyPressureManager : MonoBehaviour
{
    [Header("Family Status")]
    public int familyHappiness = 50;
    public int familySafety = 100;
    public int outstandingExpenses = 0;
    
    [Header("Pressure Events")]
    public List<FamilyPressureEvent> scheduledEvents;
    public List<FamilyPressureEvent> triggerableEvents;
    
    public void TriggerFamilyExpense(string reason, int amount, int urgencyDays);
    public void UpdateFamilyStatus(int happinessChange, int safetyChange);
    public bool CanPlayerAffordBasicNeeds();
    public FamilyPressureEvent GetNextPressureEvent();
}

[System.Serializable]
public class FamilyPressureEvent
{
    public string eventName;
    public string description;
    public int creditCost;
    public int dayTriggered;
    public int urgencyDays; // How many days until crisis
    public bool isResolved;
}
```

**Integration Points:**
- Register with ServiceLocator
- Subscribe to day progression events
- Coordinate with PersonalDataLogManager for family chat
- Trigger InspectionManager for family-related officials

---

### 3. Extend Consequence Token System

**File to Enhance:** `Assets/_scripts/ConsequenceManager.cs`

**New Token System:**
```csharp
[System.Serializable]
public class ConsequenceToken
{
    public string tokenId;
    public string sourceDecision;
    public int dayCreated;
    public int triggerDay;
    public bool hasTriggered;
    public ConsequenceData consequenceData;
}

[System.Serializable]
public class ConsequenceData
{
    public string scenarioToTrigger;
    public string newsHeadline;
    public int loyaltyImpact;
    public int suspicionIncrease;
    public bool affectsFamily;
}
```

**Methods to Add:**
```csharp
public void AddConsequenceToken(string decisionId, int delayDays, ConsequenceData consequence);
public void ProcessDailyTokens(int currentDay);
public List<ConsequenceToken> GetActiveTokens();
public bool HasTokenOfType(string tokenType);
```

**Integration:**
- Called from decision processing in CredentialChecker
- Triggers PersonalDataLog news updates
- Can trigger family pressure events
- Feeds into suspicion/investigation system

---

### 4. Implement Narrative State Tracking

**New Script:** `Assets/_RefactoredScripts/Core/Managers/NarrativeStateManager.cs`

**Core Data Structure:**
```csharp
public class NarrativeStateManager : MonoBehaviour
{
    [Header("Alignment Tracking")]
    public int imperialLoyalty = 0;    // -100 to +100
    public int rebellionSympathy = 0;  // -100 to +100
    public int corruptionLevel = 0;    // 0 to 100
    public int suspicionLevel = 0;     // 0 to 100
    
    [Header("Major Story Beats")]
    public List<string> completedStoryBeats;
    public bool pointOfNoReturnReached = false;
    public EndingPath lockedEndingPath = EndingPath.None;
    
    [Header("Captain Relationships")]
    public Dictionary<string, CaptainRelationshipData> captainRelationships;
    
    public EndingType DetermineEnding();
    public void RecordStoryBeat(string beatId);
    public void UpdateCaptainRelationship(string captainId, PlayerDecision decision);
    public bool IsEligibleForEnding(EndingType ending);
}

public enum EndingPath { None, Rebel, Imperial, Neutral, Corrupt }
public enum EndingType 
{ 
    FreedomFighter, Martyr, Refugee, Underground, 
    GrayMan, Compromised, GoodSoldier, TrueBeliever, 
    BridgeCommander, ImperialHero 
}
```

---

### 5. Create Story Scenarios with Major Alignment Shifts

**Directory:** `Assets/Resources/_ScriptableObjects/Scenarios/MajorDecisions/`

**New ScriptableObjects to Create:**
- `TheDefector.asset` - Imperial officer seeking rebel help (+5 Rebel)
- `ThePurgeOrder.asset` - Order to detain all non-humans (+5 Imperial)  
- `TheChildTransport.asset` - Families fleeing persecution (+3 Rebel)
- `TheWeaponsInspector.asset` - Loyalty test scenario (+3 Imperial)
- `TheSpyExtraction.asset` - Point of No Return rebel scenario
- `TheFamilyBetrayal.asset` - Point of No Return imperial scenario

**Enhanced Scenario Fields:**
```csharp
[Header("Narrative Impact")]
public int majorLoyaltyShift = 0;
public int majorSympathyShift = 0;
public List<string> storyBeatsToTrigger;
public bool isPointOfNoReturn = false;
public EndingPath forcesEndingPath = EndingPath.None;
```

---

### 6. Implement Recurring Captain Memory System

**Enhancement to:** `Assets/MasterShipEncounter.cs`

**New Fields:**
```csharp
[Header("Recurring Captain Data")]
public bool isRecurringCaptain = false;
public string captainUniqueId = "";
public int appearanceCount = 0;
public CaptainRelationship currentRelationship = CaptainRelationship.FirstMeeting;
public List<PlayerDecision> previousDecisions = new List<PlayerDecision>();
```

**New Manager:** `Assets/_RefactoredScripts/Core/Managers/RecurringCaptainManager.cs`
```csharp
public class RecurringCaptainManager : MonoBehaviour
{
    public Dictionary<string, RecurringCaptainData> captainMemory;
    
    public void RecordCaptainEncounter(string captainId, PlayerDecision decision);
    public CaptainRelationship GetCaptainRelationship(string captainId);
    public bool ShouldCaptainReturn(string captainId, int currentDay);
    public string GetAppropriateDialog(string captainId, DialogType dialogType);
}
```

---

### 7. Expand PersonalDataLog News Feeds

**File to Enhance:** `Assets/_RefactoredScripts/Core/Managers/PersonalDataLogManager.cs`

**New News Generation System:**
```csharp
[System.Serializable]
public class NewsTemplate
{
    public string templateId;
    public FeedType feedType;
    public string headline;
    public List<string> bodyVariations;
    public List<string> requiredTokens; // Consequence tokens that trigger this news
    public int dayDelay; // Days after token creation
}

public class NewsGenerator
{
    public List<NewsTemplate> newsTemplates;
    
    public List<DataLogEntry> GenerateNewsFromTokens(List<ConsequenceToken> activeTokens);
    public DataLogEntry CreateFamilyPressureNews(FamilyPressureEvent pressureEvent);
    public DataLogEntry CreateFactionNews(int loyaltyLevel, int sympathyLevel);
}
```

**News Categories to Implement:**
- **Imperial News:** Based on your loyalty decisions
- **Family Updates:** Generated from FamilyPressureManager
- **Consequence Reports:** From your previous decisions
- **Investigation Warnings:** Based on suspicion level

---

### 8. Create Point of No Return System

**New Script:** `Assets/_RefactoredScripts/Core/Managers/PointOfNoReturnManager.cs`

**Core Logic:**
```csharp
public class PointOfNoReturnManager : MonoBehaviour
{
    [Header("Point of No Return Settings")]
    public int pointOfNoReturnStartDay = 23;
    public int pointOfNoReturnEndDay = 27;
    
    public void CheckForPointOfNoReturn(int currentDay);
    public ScenarioData GetPointOfNoReturnScenario(NarrativeState currentState);
    public void LockEndingPath(EndingPath path);
    public bool IsPointOfNoReturnDay(int day);
}
```

**Scenario Triggers:**
- Days 23-27: Check player's alignment
- Trigger appropriate dramatic scenario based on current path
- Lock ending path based on choice made
- Prevent certain scenarios from appearing afterwards

---

### 9. Implement Ending Determination Logic

**Integration Point:** `Assets/_RefactoredScripts/Core/Managers/NarrativeStateManager.cs`

**Ending Algorithm:**
```csharp
public EndingType DetermineEnding()
{
    // Primary factor: Point of No Return choice
    if (lockedEndingPath != EndingPath.None)
    {
        return GetEndingFromLockedPath();
    }
    
    // Secondary factors: Loyalty, corruption, family status
    float alignmentScore = (imperialLoyalty - rebellionSympathy) / 200f; // -1 to 1
    float corruptionScore = corruptionLevel / 100f; // 0 to 1
    float familyScore = familyPressureManager.GetFamilyStatus() / 100f; // 0 to 1
    
    // Complex decision tree based on all factors
    return CalculateEndingFromScores(alignmentScore, corruptionScore, familyScore);
}
```

---

### 10. Integration Checklist

**Systems That Need Updates:**
- [ ] CredentialChecker - Record decisions for consequence tokens
- [ ] GameManager - Interface with narrative state tracking
- [ ] DayProgressionManager - Trigger narrative events on day change
- [ ] ServiceLocator - Register all new managers
- [ ] Save System - Persist narrative state and captain relationships

**Testing Requirements:**
- [ ] Full 30-day playthrough for each ending path
- [ ] Consequence token timing verification
- [ ] Captain relationship persistence
- [ ] Family pressure escalation balance
- [ ] Point of No Return scenario triggering

---

## 🚀 POST-COMPACT PROMPT

**Use this prompt after compacting:**

"I need to implement the narrative system we designed for Starkiller Base Command. We created a comprehensive design with 10 endings, family pressure system, recurring captains, and consequence tokens. Please review:

1. `/Assets/GDD_Chapter_Narrative_Flow.md` - Our complete narrative design
2. `/Assets/NARRATIVE_IMPLEMENTATION_GUIDE.md` - This implementation roadmap  
3. `/Assets/SYSTEM_FLOW_DOCUMENTATION.md` - System architecture understanding

Start with Phase 1: Core Infrastructure. Focus on creating the FamilyPressureManager first, then enhance the CaptainType dialog system with relationship-based responses. Remember: this is a 30-day campaign with pre-rendered videos, so we need authored scenarios that lead to specific endings.

The key innovation is using family pressure to drive moral compromise - players must choose between Imperial loyalty and family survival, creating powerful Papers Please-style moral dilemmas."

---

*This implementation guide provides the complete roadmap for building the narrative system. Follow the phases in order, and refer to the GDD chapter for design rationale behind each decision.*