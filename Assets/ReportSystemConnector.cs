using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Connects the GameManager with the DailyReportManager
/// Ensures proper communication between these components
/// </summary>
public class ReportSystemConnector : MonoBehaviour
{
    [Header("Component References")]
    public GameManager gameManager;
    public DailyReportManager reportManager;
    public ImperialFamilySystem imperialFamilySystem;
    
    void Start()
    {
        // Find components if not assigned
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
            
        if (reportManager == null)
            reportManager = FindFirstObjectByType<DailyReportManager>();
            
        if (imperialFamilySystem == null)
            imperialFamilySystem = FindFirstObjectByType<ImperialFamilySystem>();
            
        // Log component status
        Debug.Log($"ReportSystemConnector - GameManager: {gameManager != null}, " +
                 $"ReportManager: {reportManager != null}, " +
                 $"ImperialFamilySystem: {imperialFamilySystem != null}");
    }
    
    /// <summary>
    /// Call this method from GameManager's ShowDailyReport if needed
    /// </summary>
    public void ShowDailyReportWithFamilyStatus(int day, int credits, int salary, int expenses,
                                              int shipsProcessed, int requiredShips,
                                              int strikes, int penaltyPerMistake, int baseSalary)
    {
        if (reportManager == null)
        {
            Debug.LogError("ReportSystemConnector: Cannot show report - DailyReportManager not found!");
            return;
        }
        
        // Create expense dictionary
        Dictionary<string, int> expenseDict = new Dictionary<string, int>();
        
        // Get expenses from family system if available
        if (imperialFamilySystem != null)
        {
            expenseDict = imperialFamilySystem.CalculateExpenses();
        }
        else
        {
            // Create default expenses
            expenseDict.Add("Premium Quarters", 15);
            expenseDict.Add("Childcare", 10);
        }
        
        // Get family status info
        FamilyStatusInfo familyStatus = null;
        if (imperialFamilySystem != null)
        {
            familyStatus = imperialFamilySystem.GetFamilyStatusInfo();
        }
        else
        {
            // Create default family status
            familyStatus = FamilyStatusInfo.CreateDefault();
        }
        
        // Show the report using the DailyReportManager
        reportManager.ShowDailyReport(
            day, credits, salary, expenses,
            shipsProcessed, requiredShips,
            strikes, penaltyPerMistake, baseSalary,
            familyStatus, expenseDict
        );
    }
}