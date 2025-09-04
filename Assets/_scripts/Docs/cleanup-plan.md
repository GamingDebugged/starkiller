# File Cleanup and Error Resolution Plan

## Current Issues

1. **Duplicate FamilyStatusInfo classes**
   - `family-status-info.cs` and `updated-family-status-info.cs` both define the same class
   - Results in duplicate definition errors

2. **TabType accessibility error in LogBookManager**
   - The enum `TabType` is less accessible than the method using it

3. **Duplicate attributes**
   - Multiple `[System.Serializable]` attributes on the same class

## Resolution Steps

### 1. Replace Existing Files

#### FamilyStatusInfo

1. Delete both:
   - `family-status-info.cs`
   - `updated-family-status-info.cs`

2. Add consolidated version:
   - `FamilyStatusInfo.cs` (with proper casing)
   - This consolidated version handles both the original and Imperial family formats

#### LogBookManager

1. Delete:
   - `log-book-manager.cs`

2. Add fixed version:
   - `LogBookManager.cs` (with proper casing)
   - Fixed version makes the `TabType` enum public

### 2. Update References

Ensure the GameManager and FamilySystem/ImperialFamilySystem scripts are updated to work with the consolidated FamilyStatusInfo class.

### 3. Naming Conventions

Follow proper C# naming conventions for files:
- Use PascalCase for class files (e.g., `FamilyStatusInfo.cs` instead of `family-status-info.cs`)
- Make sure namespaces and class names match file names

### 4. Version Control

If using version control:
1. Make sure to commit the clean, consolidated files
2. Document changes in commit message
3. Test thoroughly after cleanup

## After Cleanup

Once the cleanup is complete, the project should have:

1. A single, unified `FamilyStatusInfo` class that works with both the original and Imperial family systems
2. A properly functioning `LogBookManager` with no accessibility errors
3. Clean file structure with proper naming conventions
