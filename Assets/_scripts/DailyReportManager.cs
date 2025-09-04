using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Starkiller.Core;

/// <summary>
/// Manages the daily report UI and related functionality
/// </summary>
public class DailyReportManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dailyReportPanel;           // The main panel for the daily report
    public TMP_Text reportHeaderText;             // Header text for the report

    [Header("Salary Panel References")]
    public TMP_Text baseSalaryValueText;          // Base salary value
    public TMP_Text shipsProcessedValueText;      // Ships processed value
    public TMP_Text efficiencyBonusValueText;     // Efficiency bonus value
    public TMP_Text mistakesValueText;            // Mistakes value
    public TMP_Text totalSalaryValueText;         // Total salary value

    [Header("Expenses Panel References")]
    public TMP_Text premiumQuartersValueText;     // Premium quarters expense
    public TMP_Text childcareValueText;           // Childcare expense
    public TMP_Text equipmentValueText;           // Equipment expense
    public TMP_Text medicalValueText;             // Medical expense
    public TMP_Text trainingValueText;            // Training expense
    public TMP_Text totalExpensesValueText;       // Total expenses

    [Header("Financial Summary References")]
    public TMP_Text previousBalanceValueText;     // Previous balance value
    public TMP_Text incomeValueText;              // Income value
    public TMP_Text expensesValueText;            // Expenses value
    public TMP_Text remainingCreditsValueText;    // Remaining credits value
    
    [Header("Status Reports")]
    public TMP_Text familyStatusText;             // Text showing family status
    public TMP_Text securityReportText;           // Text showing security incidents
    
    [Header("Navigation")]
    public Button continueButton;                 // Button to continue to next day
    
    [Header("Color Settings")]
    public Color incomeColor = new Color(0.29f, 0.87f, 0.5f);    // Green color for income
    public Color expensesColor = new Color(0.94f, 0.27f, 0.27f); // Red color for expenses
    public Color balanceColor = new Color(0.98f, 0.8f, 0.08f);   // Yellow color for balance
    
    // Reference to the GameManager
    private GameManager gameManager;
    
    // History of daily reports for easy reference
    private List<DailyReportData> reportHistory = new List<DailyReportData>();
    
    void Start()
    {
        Debug.Log($"DailyReportManager.Start() called - Panel active: {(dailyReportPanel != null ? dailyReportPanel.activeSelf.ToString() : "null")}");
        
        // Find the GameManager
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }
        
        // Set up the continue button
        if (continueButton)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        
        // Hide the panel initially (but don't hide it if it's currently showing a report)
        if (dailyReportPanel)
        {
            bool isPanelActivelyShowingReport = dailyReportPanel.activeSelf;
            if (!isPanelActivelyShowingReport)
            {
                dailyReportPanel.SetActive(false);
            }
            else
            {
                Debug.Log("DailyReportManager.Start(): Panel is already active showing a report, not hiding it");
            }
            
            // Add TimeModifierBehavior if not already present
            if (dailyReportPanel.GetComponent<TimeModifierBehavior>() == null)
            {
                TimeModifierBehavior timeModifier = dailyReportPanel.AddComponent<TimeModifierBehavior>();
                timeModifier.pauseTime = true;
                timeModifier.modifyOnEnable = true;
                timeModifier.resumeOnDisable = true;
            }
        }
    }
    
    /// <summary>
    /// Shows the daily report with all relevant information
    /// </summary>
    public void ShowDailyReport(int day, int currentCredits, int salary, int expenses, 
                           int shipsProcessed, int requiredShips, 
                           int currentStrikes, int penaltyPerMistake, int baseSalary,
                           FamilyStatusInfo familyStatus, 
                           Dictionary<string, int> expenseBreakdown)
    {
        Debug.Log("DailyReportManager.ShowDailyReport called");
        
        if (dailyReportPanel == null) 
        {
            Debug.LogError("DailyReportManager.dailyReportPanel is null! Cannot display report.");
            return;
        }
        
        // Store previous credits for display
        int previousCredits = currentCredits;
        
        // Calculate new balance
        int newCredits = previousCredits + salary - expenses;
        
        // Calculate other values
        int efficiency = shipsProcessed > requiredShips ? (shipsProcessed - requiredShips) * 5 : 0;
        int mistakes = currentStrikes * penaltyPerMistake;
        
        // Show the report panel
        dailyReportPanel.SetActive(true);
        Debug.Log("DailyReportPanel activated");
        
        try
        {
            // Update header text
            if (reportHeaderText)
                reportHeaderText.text = $"DAILY PERFORMANCE REPORT: DAY {day}";
                
            // Update salary panel values
            if (baseSalaryValueText)
                baseSalaryValueText.text = baseSalary.ToString();
                
            if (shipsProcessedValueText)
                shipsProcessedValueText.text = $"{shipsProcessed}/{requiredShips}";
                
            if (efficiencyBonusValueText)
                efficiencyBonusValueText.text = efficiency.ToString();
                
            if (mistakesValueText)
                mistakesValueText.text = $"{currentStrikes} (-{mistakes})";
                
            if (totalSalaryValueText)
                totalSalaryValueText.text = salary.ToString();
            
            // Update expense panel values (set defaults to empty or "0")
            if (premiumQuartersValueText) premiumQuartersValueText.text = "0";
            if (childcareValueText) childcareValueText.text = "0";
            if (equipmentValueText) equipmentValueText.text = "0";
            if (medicalValueText) medicalValueText.text = "0";
            if (trainingValueText) trainingValueText.text = "0";
            
            // Now update with actual expense values
            foreach (var expense in expenseBreakdown)
            {
                switch (expense.Key)
                {
                    case "Premium Quarters":
                        if (premiumQuartersValueText)
                            premiumQuartersValueText.text = expense.Value.ToString();
                        break;
                    case "Childcare":
                        if (childcareValueText)
                            childcareValueText.text = expense.Value.ToString();
                        break;
                    case "Equipment":
                        if (equipmentValueText)
                            equipmentValueText.text = expense.Value.ToString();
                        break;
                    case "Medical Care":
                    case "Medical":
                        if (medicalValueText)
                            medicalValueText.text = expense.Value.ToString();
                        break;
                    case "Training":
                        if (trainingValueText)
                            trainingValueText.text = expense.Value.ToString();
                        break;
                }
            }
            
            if (totalExpensesValueText)
                totalExpensesValueText.text = expenses.ToString();
            
            // Update financial summary values
            if (previousBalanceValueText)
                previousBalanceValueText.text = previousCredits.ToString();
                
            if (incomeValueText)
                incomeValueText.text = salary.ToString();
                
            if (expensesValueText)
                expensesValueText.text = expenses.ToString();
                
            if (remainingCreditsValueText)
                remainingCreditsValueText.text = newCredits.ToString();
            
            // Update family status
            if (familyStatusText && familyStatus != null)
            {
                familyStatusText.text = familyStatus.GetStatusSummary();
            }
            
            // Update security report
            if (securityReportText)
            {
                // Get the security incident report from ConsequenceManager
                ConsequenceManager consequenceManager = FindFirstObjectByType<ConsequenceManager>();
                if (consequenceManager != null)
                {
                    // Add the consequence report to the security section
                    securityReportText.text = consequenceManager.GenerateDailyReport();
                    Debug.Log("Added consequence report to daily summary");
                }
                else
                {
                    // Fallback if no ConsequenceManager found
                    securityReportText.text = "No security incidents to report.";
                    Debug.LogWarning("ConsequenceManager not found when generating security report");
                }
            }
            
            // Ensure that the continue button is properly assigned and visible
            if (continueButton)
            {
                Debug.Log("Continue button is assigned and should be active");
                continueButton.gameObject.SetActive(true);
                
                // Re-add the click listener just to be safe
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(OnContinueClicked);
                
                // Make sure the button is interactable
                continueButton.interactable = true;
            }
            else
            {
                Debug.LogError("Continue button is not assigned in DailyReportManager!");
            }
            
            Debug.Log("DailyReportManager.ShowDailyReport UI update completed successfully");
            
            // Add report to history
            SaveReportData(day, previousCredits, salary, expenses, newCredits, familyStatus);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in DailyReportManager.ShowDailyReport: {e.Message}\n{e.StackTrace}");
        }
        
        // Enforce that the panel is active as a final check
        if (!dailyReportPanel.activeSelf)
        {
            Debug.LogError("CRITICAL: dailyReportPanel still not active after all UI updates! Forcing it active now.");
            dailyReportPanel.SetActive(true);
        }
    }
    
    /// <summary>
    /// Called when the continue button is clicked
    /// </summary>
    public void OnContinueClicked()
    {
        Debug.Log("DailyReportManager: Continue button clicked, starting day progression sequence");
        
        // Hide the daily report UI
        if (dailyReportPanel != null)
        {
            dailyReportPanel.SetActive(false);
        }
        
        // Use RefactoredDailyReportManager for day progression if available (preserves all dependencies)
        var refactoredDailyReportManager = ServiceLocator.Get<Starkiller.Core.Managers.RefactoredDailyReportManager>();
        if (refactoredDailyReportManager != null)
        {
            Debug.Log("DailyReportManager: Using RefactoredDailyReportManager for day progression");
            refactoredDailyReportManager.RequestNextDay();
        }
        else
        {
            Debug.LogWarning("DailyReportManager: RefactoredDailyReportManager not found, using fallback day progression");
            
            // Fallback to direct DayProgressionManager
            var dayProgressionManager = ServiceLocator.Get<Starkiller.Core.Managers.DayProgressionManager>();
            if (dayProgressionManager != null)
            {
                Debug.Log($"DailyReportManager: Current day before increment: {dayProgressionManager.CurrentDay}");
                dayProgressionManager.StartNewDay();
                Debug.Log($"DailyReportManager: Day after increment: {dayProgressionManager.CurrentDay}");
            }
            else
            {
                Debug.LogError("DailyReportManager: DayProgressionManager not found! Cannot increment day.");
            }
            
            // Clean up for the new day WITHOUT calling StartNextDay (which might increment again)
            if (gameManager != null)
            {
                Debug.Log("DailyReportManager: Performing daily cleanup via GameManager");
                // Call a cleanup method instead of StartNextDay to avoid double increment
                if (gameManager.GetType().GetMethod("PerformDailyCleanup") != null)
                {
                    gameManager.GetType().GetMethod("PerformDailyCleanup").Invoke(gameManager, null);
                }
                else
                {
                    // Fallback: reset daily tracking without incrementing day
                    Debug.Log("DailyReportManager: Using StartNextDay but day already incremented by DayProgressionManager");
                    gameManager.StartNextDay();
                }
            }
            
            // Show Personal Data Log for the NEW day
            var personalDataLogManager = ServiceLocator.Get<Starkiller.Core.Managers.PersonalDataLogManager>();
            if (personalDataLogManager != null)
            {
                personalDataLogManager.ShowDataLog();
            }
            else
            {
                Debug.LogWarning("DailyReportManager: PersonalDataLogManager not found, proceeding directly to daily briefing");
                
                // Fallback to direct day start if PersonalDataLogManager not available
                if (gameManager != null)
                {
                    Debug.Log("DailyReportManager: Starting day via GameManager.StartDay() - day already incremented");
                    gameManager.StartDay();
                }
                else
                {
                    Debug.LogError("DailyReportManager: GameManager reference is null!");
                }
            }
        }
    }
    
    /// <summary>
    /// Saves the report data for history
    /// </summary>
    private void SaveReportData(int day, int previousCredits, int salary, 
                              int expenses, int newCredits, FamilyStatusInfo familyStatus)
    {
        DailyReportData report = new DailyReportData
        {
            Day = day,
            PreviousCredits = previousCredits,
            Salary = salary,
            Expenses = expenses,
            EndingCredits = newCredits,
            FamilyStatus = familyStatus
        };
        
        reportHistory.Add(report);
    }
}

/// <summary>
/// Data structure to store daily report information
/// </summary>
[System.Serializable]
public class DailyReportData
{
    public int Day;
    public int PreviousCredits;
    public int Salary;
    public int Expenses;
    public int EndingCredits;
    public FamilyStatusInfo FamilyStatus;
}