# Singleton Pattern and Reference Handling Issues

## Issues Found

1. **Inconsistent Singleton Implementation**:
   - MasterShipGenerator implements a singleton pattern with a static `Instance` property but lacks proper initialization checks 
   - The instance is set in Awake() but there's no OnDestroy() cleanup

2. **Direct FindFirstObjectByType Usage**:
   - In CredentialChecker.cs, there are multiple places where FindFirstObjectByType<MasterShipGenerator>() is used directly
   - This bypasses the singleton pattern, leading to potential inconsistencies

3. **ProcessDecisionWithEncounter Implementation**:
   - This method in MasterShipGenerator doesn't properly handle null references
   - At line 951-986, it attempts to recover from a null encounter, but the function can still fail
   
4. **Event Subscription/Unsubscription Issues**:
   - CredentialChecker subscribes to OnEncounterReady in Start() and unsubscribes in OnDestroy()
   - However, multiple subscription attempts occur as the CredentialChecker tries to recover from null references

5. **Excessive Fallback Logic**:
   - Both classes have extensive fallback logic that might hide serious issues rather than revealing them

## Comprehensive Fix Approach

1. **Proper Singleton Implementation for MasterShipGenerator**:
   - Add null check in Instance property getter
   - Implement proper OnDestroy cleanup
   - Use `DontDestroyOnLoad` correctly
   
2. **Reliable Reference Management**:
   - Always use the singleton pattern's Instance property instead of FindFirstObjectByType
   - Add clear error handling and diagnostics for null references
   
3. **Fix ProcessDecisionWithEncounter**:
   - Improve null reference handling
   - Add clearer error logging
   - Ensure proper cleanup of current encounter reference
   
4. **Event Subscription Management**:
   - Implement a cleaner way to handle event subscriptions
   - Add safety checks before firing events
   
5. **Unified Recovery Strategy**:
   - Consolidate the fallback/recovery logic in one place
   - Improve error reporting for tracking issues in production