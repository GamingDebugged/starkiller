# Consequence EndGame Message Examples

## How to Use the New endGameMessage Field

### 1. **Open the Editor Tool**
   - Menu: `Starkiller > Consequence EndGame Messages`
   - This will show all 18 consequences with text fields

### 2. **Write Contextual Messages**
   Each message should be written from the perspective of reflecting on the player's journey.

## Example Messages for "Family Reassignment"

### Option A: Neutral/Factual
```
Family relocated to monitoring quarters following security protocol violations
```

### Option B: Imperial Perspective
```
Accepted family reassignment as consequence of duty failures without protest
```

### Option C: Emotional/Personal
```
Security concerns forced family separation, restricted to monitored communications
```

### Option D: Rebel Sympathetic
```
Empire's paranoia resulted in family being taken to surveillance quarters
```

## Writing Guidelines

### ✅ DO:
- **Keep it concise** (1-2 lines max)
- **Use past tense** (reflecting on journey)
- **Be specific** about the consequence
- **Match the tone** to severity level
- **Include numbers** where impactful (casualties, credits)

### ❌ DON'T:
- Use first person ("I caused...")
- Be overly dramatic for minor consequences
- Repeat the title verbatim
- Use present tense
- Make it generic

## Examples by Consequence Type

### Security Consequences
```yaml
Minor: "Security protocols briefly overlooked during routine inspection"
Moderate: "Failed to detect contraband, compromising checkpoint integrity"  
Severe: "Security breach resulted in unauthorized Imperial facility access"
Critical: "Catastrophic security failure led to 15 Imperial casualties"
```

### Family Consequences
```yaml
Minor: "Family received warning about associate's questionable loyalties"
Moderate: "Family placed under increased surveillance due to decisions"
Severe: "Family relocated to monitoring quarters after security concerns"
Critical: "Family detained indefinitely pending loyalty investigation"
```

### Financial Consequences
```yaml
Minor: "Received 10 credit penalty for procedural violation"
Moderate: "Lost 50 credits due to inspection failures"
Severe: "Major financial penalties exceeded monthly salary"
Critical: "Financial misconduct triggered full audit and asset freeze"
```

### Reputation Consequences
```yaml
Minor: "Service record noted minor compliance issues"
Moderate: "Professional standing diminished among Imperial officers"
Severe: "Reputation severely damaged, promotion prospects eliminated"
Critical: "Complete loss of Imperial trust and standing"
```

### Story Consequences
```yaml
"Aided rebel sympathizer escape despite Imperial orders"
"Detained innocent refugees to maintain cover"
"Accepted bribe to overlook forged documents"
"Revealed insurgent plot to Imperial command"
```

## Tone Variations by Ending Type

The same consequence can be framed differently based on context:

### "Family Reassignment" in Different Endings:

**Imperial Hero Ending:**
```
"Prioritized Imperial duty when family was reassigned for security"
```

**Freedom Fighter Ending:**
```
"Empire punished family for your growing rebel sympathies"
```

**Gray Man Ending:**
```
"Navigated family reassignment while maintaining neutral stance"
```

**Compromised Ending:**
```
"Failed to protect family from Imperial security apparatus"
```

## Quick Reference Format

For each of your 18 consequences, use this format:
```
[Consequence Name]
Type: [Security/Family/Financial/etc]
Severity: [Minor/Moderate/Severe/Critical]
EndGame Message: "[Your 1-2 line message]"
```

## Using the Editor Tool

1. **Open:** `Starkiller > Consequence EndGame Messages`
2. **Review:** Each consequence with suggested messages
3. **Edit:** Type custom messages or use suggestions
4. **Apply:** Click "Apply All Suggested Messages" for defaults
5. **Save:** Changes auto-save to ScriptableObjects

## Testing Your Messages

After adding endGameMessages:
1. Trigger the consequence in gameplay
2. Complete day 30
3. Check ending screen achievements
4. Your custom message should appear instead of generic text

## Priority Guidelines

Remember that consequences appear in endings based on:
- **Critical:** Priority 100 (always shown)
- **Severe:** Priority 80 (usually shown)
- **Moderate:** Priority 60 (shown if space)
- **Minor:** Priority 40 (rarely shown)

Write your best messages for Critical and Severe consequences as they're most likely to appear in endings!