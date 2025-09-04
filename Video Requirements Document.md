# Starkiller Base Command - Video Requirements Document

**Date:** August 2025  
**Purpose:** Complete list of video assets needed for gameplay and narrative sequences  
**Total Videos:** ~75-80 unique clips

---

## 🎬 CORE GAMEPLAY VIDEOS

### Welcome & Tutorial
- **Welcome Video** - First boot introduction to checkpoint system
- **Tutorial Inspection** - How to review documents
- **Tutorial Approval** - Demonstration of approval process
- **Tutorial Denial** - Demonstration of denial process
- **Tutorial Tractor Beam** - When and how to use scanning

### Basic Ship Responses (5)
- **Greeting** - Initial ship approach and document presentation
- **Approval Response** - Ship cleared to proceed animation
- **Denial Response** - Ship turned away animation
- **Holding Response** - Ship told to wait in queue
- **Tractor Beam Response** - Ship being pulled for inspection

### Inspection Process Videos (6)
- **Document Review** - Close-up of papers being examined
- **Scanner Activation** - Tractor beam engaging
- **Cargo Scan** - Interior ship scanning sequence
- **Contraband Discovery** - Finding illegal items
- **Clean Scan Result** - Nothing suspicious found
- **Passenger Verification** - Checking identity documents

### Bribery Sequence (4)
- **Bribe Offer** - Ship captain offering credits
- **Bribe Accepted** - Taking the money, ship proceeds
- **Bribe Rejected** - Refusing corruption
- **Bribe Reported** - Alerting security to attempt

### Return Interactions (5)
- **Holding Return Impatient** - Ship angry about wait time
- **Holding Return Desperate** - Pleading to be processed
- **Post-Scan Clean** - Ship cleared after inspection
- **Post-Scan Contraband** - Ship caught with illegal goods
- **Post-Scan Arrest** - Security taking ship crew

---

## 📰 NEWS & MEDIA VIDEOS

### Imperium News Broadcasts (10)
- **Daily Imperial Update** - Standard propaganda broadcast
- **Emperor's Address** - Major policy speech
- **Military Victory** - Battle footage and celebration
- **Terrorist Attack** - Rebel bombing aftermath
- **Economic Report** - Trade route closures
- **Security Crackdown** - New checkpoint procedures
- **Public Execution** - Consequences for traitors
- **Loyalty Rewards** - Citizens being honored
- **Station Announcement** - Local imperial orders
- **Emergency Broadcast** - Crisis alert

### Frontier E-zine Reports (8)
- **Underground News** - Rebel perspective broadcast
- **Trade Route Update** - Smuggler intelligence
- **Refugee Crisis** - Humanitarian disaster footage
- **Protest Footage** - Civil unrest documentation
- **Black Market Report** - Illegal trade information
- **Corruption Exposed** - Imperial officer scandal
- **Missing Ships Investigation** - Disappeared vessels
- **Frontier Resistance** - Rebel recruitment video

---

## 👨‍👩‍👧‍👦 FAMILY CRISIS VIDEOS

### Partner - Alex (5)
- **Lab Accident** - Alex injured in research facility
- **Arrest Warrant** - Security searching for Alex
- **Secret Message** - Alex contacting rebels
- **Interrogation** - Alex being questioned
- **Final Goodbye** - Alex's last message

### Son - Marcus (5)
- **Training Footage** - Marcus in trooper program
- **Pirate Meeting** - Security cam of Marcus with criminals
- **Gambling Debt** - Marcus owing dangerous people
- **Bar Fight** - Marcus in violent altercation
- **Missing Person** - Marcus disappeared

### Daughter - Sarah (5)
- **Workshop Fire** - Sarah's mechanic shop burning
- **Refugee Discovery** - Hidden families found
- **Sarah's Arrest** - Being taken by security
- **Escape Attempt** - Sarah fleeing station
- **Memorial** - If Sarah dies

### Baby - Hope (4)
- **Medical Emergency** - Hope having seizure
- **Hospital Overwhelmed** - No beds available
- **Medicine Shortage** - Cannot get treatment
- **Critical Condition** - Hope near death

### Droid - D-3X (3)
- **Strange Behavior** - D-3X acting unusually
- **Memory Wipe** - D-3X being reprogrammed
- **Hidden Message** - D-3X revealing secrets

---

## 🎭 SPECIAL NARRATIVE VIDEOS

### Point of No Return (Days 23-27) (5)
- **Imperial Loyalty Test** - High official testing you
- **Rebel Recruitment** - Direct approach from resistance
- **Family Ultimatum** - Choose job or family
- **The Final Ship** - Encounter determining ending
- **Moment of Truth** - Your crucial decision

### High-Stakes Encounters (8)
- **VIP Imperial Escort** - Dignitaries demanding priority
- **Mass Casualty Ship** - Medical emergency vessel
- **Prisoner Transport** - Rebels in custody
- **Weapons Smuggler** - Major contraband bust
- **Spy Vessel** - Intelligence operative
- **Refugee Ship** - Desperate families fleeing
- **Pirate Intimidation** - Threats from criminals
- **Mysterious Cargo** - Unknown dangerous goods

### Environmental/Atmosphere (6)
- **Station Exterior Dawn** - Early morning establishing shot
- **Station Exterior Dusk** - Evening degradation visible
- **Queue Riot** - Checkpoint crowd violence
- **Imperial Parade** - Propaganda march
- **Drift Sickness Ward** - Hospital crisis
- **Black Market Deal** - Background corruption

---

## 🎬 ENDING VIDEOS

### Victory Endings (3)
- **Imperial Ceremony** - Family at promotion event
- **Rebel Victory** - Station liberated
- **Escape Success** - Family fled to safety

### Tragic Endings (4)
- **Alone at Console** - Lost everyone
- **Family Memorial** - Funeral for loved ones
- **Public Execution** - Your execution
- **Station Destruction** - Everything burns

### Ambiguous Endings (2)
- **Compromise** - Partial family survival
- **Unknown Future** - Uncertain fate

---

## 📋 TECHNICAL SPECIFICATIONS

### Video Requirements
- **Format:** MP4 H.264 or Unity VideoClip
- **Resolution:** 1920x1080 (UI) / 1280x720 (encounters)
- **Frame Rate:** 30fps standard, 60fps for action
- **Duration:** 3-10 seconds (responses), 15-30 seconds (narratives), 30-60 seconds (news)
- **Audio:** Embedded stereo track for dialogue/effects
- **Compression:** Optimized for real-time playback

### Categorization for Code
```
VideoType Enum:
- Greeting
- Response
- Inspection  
- News
- Family
- Narrative
- Environmental
- Ending
```

### Triggering Logic
- **Day-based:** Specific videos on certain days
- **State-based:** Family relationship thresholds
- **Event-based:** Consequence tokens trigger
- **Random:** Pool selection for variety
- **Sequential:** Story arc progression

---

## 🎯 PRIORITY LEVELS

### Essential (Must Have) - 20 videos
- All Core Gameplay Videos
- Basic news broadcasts (2-3)
- Key family crisis moments (1 per member)
- At least 3 ending videos

### Important (Should Have) - 25 videos
- Extended inspection sequences
- Bribery interactions
- More news variety
- Environmental atmosphere
- Point of No Return sequences

### Desired (Nice to Have) - 30+ videos
- Full family story arcs
- All special encounters
- Complete news cycles
- Multiple ending variations
- Rich environmental details

---

## 📝 PRODUCTION NOTES

### Style Guidelines
- **Aesthetic:** Worn, industrial, dystopian
- **Color Palette:** Muted grays, harsh fluorescents, warning reds
- **Camera:** Security footage feel, static angles
- **Effects:** Scan lines, interference, degradation
- **Audio:** Echoing space, mechanical sounds, distant alarms

### Reusability
- Create modular segments that can be combined
- Use overlay effects for variety (anger, fear, urgency)
- Audio variations more important than visual
- Text overlays can change context of same video

### Performance Considerations
- Stream larger videos from disk
- Preload frequently used clips
- Use video pools for common responses
- Consider lower resolution for background videos

---

## 📅 IMPLEMENTATION PHASES

### Phase 1: Core Loop
Implement basic inspection videos and responses to make game playable

### Phase 2: Narrative Layer
Add family videos and news broadcasts for story depth

### Phase 3: Polish
Environmental videos, special encounters, multiple endings

### Phase 4: Optimization
Compress, pool, and stream optimization for performance

---

*This document represents the complete video asset requirements for Starkiller Base Command. Videos should be produced in order of priority, with essential videos enabling basic gameplay and additional videos enriching the narrative experience.*