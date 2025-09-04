# Troubleshooting Integration Issues

When integrating the new `MasterShipEncounter` system with existing code, you may encounter some errors. This guide will help you resolve the most common issues.

## Common Error Types

### 1. Type Conversion Errors

```
Argument cannot convert from 'ShipEncounter' to 'MasterShipEncounter'
```

This occurs when code expects the new `MasterShipEncounter` type but is receiving one of the old types (`ShipEncounter`, `EnhancedShipEncounter`, or `VideoEnhancedShipEncounter`).

### 2. DayRule vs DailyRule Errors

```
'DailyRule' does not contain a definition for 'ruleType'
```

This happens due to a mismatch between a standalone `DailyRule` class and the nested `StarkkillerContentManager.DayRule` class.

## Solutions

### Using the Adapter Class

We've provided a `MasterShipEncounterAdapter.cs` class that adds extension methods to make the transition smoother:

```csharp
// Example: In a script that's still using the old types
public void SomeMethod(ShipEncounter oldEncounter)
{
    // This will automatically convert to MasterShipEncounter
    credentialChecker.DisplayEncounter(oldEncounter);
}
```

The adapter handles conversion between:
- `ShipEncounter` → `MasterShipEncounter`
- `EnhancedShipEncounter` → `MasterShipEncounter`
- `VideoEnhancedShipEncounter` → `MasterShipEncounter`

### Fixing Type References in Your Code

For scripts that you control, update their method parameters:

```csharp
// Old
public void ProcessEncounter(ShipEncounter encounter)

// New
public void ProcessEncounter(MasterShipEncounter encounter)
```

Then use the adapter or manual conversion at call sites:

```csharp
// Using adapter extension method
ProcessEncounter(oldEncounter.ToMasterEncounter());

// Or use direct conversion
ProcessEncounter(MasterShipEncounter.FromLegacyEncounter(oldEncounter));
```

### DayRule Type Issues

When working with day rules, always use the fully qualified name:

```csharp
// Instead of
List<DailyRule> rules;

// Use
List<StarkkillerContentManager.DayRule> rules;
```

Or add a using alias at the top of your script:

```csharp
using DayRule = StarkillerBaseCommand.StarkkillerContentManager.DayRule;
```

## Step-by-Step Integration Approach

1. Add the new classes (`MasterShipEncounter.cs`, `MasterShipGenerator.cs`, and adapter)
2. Set up the new system in parallel with the old one
3. Use the adapter methods where needed to bridge old and new code
4. Gradually update individual components to use the new system directly
5. Once everything is working with the new system, remove the old classes

## Testing Your Integration

After making changes to address errors, test:

1. Can you create encounters?
2. Do videos and images display properly?
3. Does the credential checking logic work?
4. Are approvals and denials processed correctly?

## Getting Help

If you encounter issues not covered here, check:
- Are all namespace references correct?
- Are you using the correct parameter types in method signatures?
- Have you properly initialized the `MasterShipGenerator`?

Remember that the goal is gradual transition, not an overnight rewrite. The adapter pattern allows both systems to coexist during the transition period.
