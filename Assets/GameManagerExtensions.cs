using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Extensions for the GameManager to handle daily reports
/// </summary>
public static class GameManagerExtensions
{
    /// <summary>
    /// Calculate salary based on performance
    /// </summary>
    public static int CalculateSalaryExtended(this GameManager gameManager, int shipsProcessed, int requiredShips, int strikes, int baseSalary, int bonusPerShip, int penaltyPerMistake)
    {
        int salary = 0;
        
        // Base salary if quota met
        if (shipsProcessed >= requiredShips)
        {
            salary += baseSalary;
            
            // Bonus for extra ships
            int extraShips = shipsProcessed - requiredShips;
            if (extraShips > 0)
            {
                salary += extraShips * bonusPerShip;
            }
        }
        else
        {
            // Partial salary if quota not met
            float percentComplete = (float)shipsProcessed / requiredShips;
            salary += Mathf.FloorToInt(baseSalary * percentComplete);
        }
        
        // Deduct penalties for mistakes
        salary -= strikes * penaltyPerMistake;
        
        // Ensure salary isn't negative
        salary = Mathf.Max(0, salary);
        
        return salary;
    }
    
    /// <summary>
    /// Gets default expenses for the family
    /// </summary>
    public static Dictionary<string, int> GetDefaultExpenses(this GameManager gameManager, int premiumQuartersCost, int childcareCost, int medicalCost = 0, int equipmentCost = 0, int protectionCost = 0)
    {
        Dictionary<string, int> expenses = new Dictionary<string, int>();
        
        // Add basic expenses
        expenses.Add("Premium Quarters", premiumQuartersCost);
        expenses.Add("Childcare", childcareCost);
        
        // Add optional expenses if needed
        if (medicalCost > 0)
            expenses.Add("Medical Care", medicalCost);
            
        if (equipmentCost > 0)
            expenses.Add("Equipment", equipmentCost);
            
        if (protectionCost > 0)
            expenses.Add("Protection", protectionCost);
            
        return expenses;
    }
}