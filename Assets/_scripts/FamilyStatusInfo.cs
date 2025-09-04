using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Unified structure to hold Imperium family status information for reporting and storage
/// This consolidates all previous versions of family status info classes
/// </summary>
[System.Serializable]
public class FamilyStatusInfo
{
    // Family member information
    public string[] Names;          // Names of family members
    public string[] Occupations;    // Occupations of family members (Imperium Scientist, Stormtrooper, etc.)
    public Sprite[] Portraits;      // Portraits for each family member
    
    // Status indicators
    public bool[] NeedsMedical;     // Array of medical care needs for each family member
    public bool[] NeedsEquipment;   // Array of equipment needs for each family member
    public bool[] NeedsTraining;    // Array of training needs for each family member
    
    // Family environment
    public bool HasPremiumQuarters; // Whether the family has premium quarters (vs. standard)
    public bool NeedsChildcare;     // Whether the baby/child needs proper childcare
    public bool NeedsDroidMaintenance; // Whether the family droid needs maintenance
    
    // Additional status indicators (for Daily Briefing)
    public int[] HealthStatus;      // Health percentage (0-100) for each member
    public int[] LoyaltyStatus;     // Loyalty percentage (0-100) for each member
    
    // Field to support legacy format for backwards compatibility
    public bool HasChild { get { return NeedsChildcare; } }
    public bool HasSpouse { get { return Names != null && Names.Length > 1; } }
    public int ChildHealthStatus { get { return HealthStatus != null && HealthStatus.Length > 2 ? HealthStatus[2] : 85; } }
    public int SpouseHealthStatus { get { return HealthStatus != null && HealthStatus.Length > 1 ? HealthStatus[1] : 90; } }
    public string SpouseAssignment { get { return Occupations != null && Occupations.Length > 1 ? Occupations[1] : "Imperium"; } }
    public bool HasImperialEducation { get { return true; } }
    public bool HasMedicalCare { get { return !NeedsAnyCare(CareType.Medical); } }
    
    /// <summary>
    /// Get a summary of the family's status in Imperium format
    /// </summary>
    public string GetStatusSummary()
    {
        string summary = "";
        
        if (Names == null || Names.Length == 0)
        {
            return "No family status available.";
        }
        
        for (int i = 0; i < Names.Length; i++)
        {
            // Imperial format
            summary += $"• {Names[i]}";
            
            // Add occupation if available
            if (Occupations != null && i < Occupations.Length && !string.IsNullOrEmpty(Occupations[i]))
            {
                summary += $" ({Occupations[i]})";
            }
            
            summary += " ";
            
            // Collect status conditions
            List<string> conditions = new List<string>();
            
            if (NeedsMedical != null && i < NeedsMedical.Length && NeedsMedical[i])
                conditions.Add("needs medical care");
                
            if (NeedsEquipment != null && i < NeedsEquipment.Length && NeedsEquipment[i])
                conditions.Add("needs specialized equipment");
                
            if (NeedsTraining != null && i < NeedsTraining.Length && NeedsTraining[i])
                conditions.Add("needs training");
                
            if (conditions.Count == 0)
                summary += "is in good standing.";
            else
                summary += string.Join(" and ", conditions) + ".";
            
            if (i < Names.Length - 1)
                summary += "\n";
        }
        
        // Add housing and other family status info
        summary += "\n\n";
        
        // Add quarters status
        summary += HasPremiumQuarters ?
            "Your family is enjoying premium Imperium quarters." :
            "Your family is in standard Imperium quarters.";
            
        // Add childcare status if needed
        if (NeedsChildcare)
            summary += "\nYour child requires better childcare services.";
            
        // Add droid maintenance status if needed
        if (NeedsDroidMaintenance)
            summary += "\nYour family droid requires maintenance.";
        
        return summary;
    }
    
    /// <summary>
    /// Get simple status for a specific family member
    /// </summary>
    public string GetMemberStatus(int memberIndex)
    {
        if (Names == null || memberIndex >= Names.Length)
            return "Unknown";
            
        List<string> conditions = new List<string>();
        
        if (NeedsMedical != null && memberIndex < NeedsMedical.Length && NeedsMedical[memberIndex])
            conditions.Add("Medical");
            
        if (NeedsEquipment != null && memberIndex < NeedsEquipment.Length && NeedsEquipment[memberIndex])
            conditions.Add("Equipment");
            
        if (NeedsTraining != null && memberIndex < NeedsTraining.Length && NeedsTraining[memberIndex])
            conditions.Add("Training");
            
        if (conditions.Count == 0)
            return "Good Standing";
        else
            return string.Join(", ", conditions);
    }
    
    /// <summary>
    /// Create a default family status with your agreed family structure
    /// </summary>
    public static FamilyStatusInfo CreateDefault()
    {
        return new FamilyStatusInfo
        {
            // Family members as specified
            Names = new string[] { "Emma", "Kira", "Jace", "R2-D4" },
            Occupations = new string[] { "Imperium Officer", "Fighter Mechanic", "Trooper Cadet", "Family Droid" },
            
            // Status indicators - initially all good except for droid maintenance
            NeedsMedical = new bool[] { false, false, false, false },
            NeedsEquipment = new bool[] { false, false, false, true },
            NeedsTraining = new bool[] { false, false, true, false },
            
            // Family environment
            HasPremiumQuarters = true,
            NeedsChildcare = false,
            NeedsDroidMaintenance = true,
            
            // Additional status
            HealthStatus = new int[] { 95, 90, 85, 70 },
            LoyaltyStatus = new int[] { 90, 85, 95, 100 }
        };
    }
    
    /// <summary>
    /// Get whether any family member needs a specific type of care
    /// </summary>
    public bool NeedsAnyCare(CareType type)
    {
        switch (type)
        {
            case CareType.Medical:
                if (NeedsMedical == null)
                    return false;
                    
                for (int i = 0; i < NeedsMedical.Length; i++)
                {
                    if (NeedsMedical[i])
                        return true;
                }
                return false;
                
            case CareType.Equipment:
                if (NeedsEquipment == null)
                    return false;
                    
                for (int i = 0; i < NeedsEquipment.Length; i++)
                {
                    if (NeedsEquipment[i])
                        return true;
                }
                return NeedsDroidMaintenance; // Also include droid maintenance
                
            case CareType.Training:
                if (NeedsTraining == null)
                    return false;
                    
                for (int i = 0; i < NeedsTraining.Length; i++)
                {
                    if (NeedsTraining[i])
                        return true;
                }
                return false;
                
            default:
                return false;
        }
    }
}

/// <summary>
/// Types of care that family members may need
/// </summary>
public enum CareType
{
    Medical,
    Equipment,
    Training
}