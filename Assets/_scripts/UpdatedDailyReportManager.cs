using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the daily report UI and related functionality for Imperial officers
/// </summary>
public class ImperialDailyReportManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dailyReportPanel;           // The main panel for the daily report
    public TMP_Text reportHeaderText;             // Header text for the report
    public TMP_Text salaryText;                   // Text showing daily earnings
    public TMP_Text expensesText;                 // Text showing daily expenses
    public TMP_Text previousBalanceText;          // Text showing previous balance
    public TMP_Text incomeText;                   // Text showing income amount
    public TMP_Text expensesSummaryText;          // Text showing expenses summary
    public TMP_Text creditsTotalText;             // Text showing final credits
    public TMP_Text familyStatusText;             // Text showing family status
    public Button continueButton;                 // Button to continue to next day
    
    [Header("Color Settings")]
    public Color incomeColor = new Color(0.29f, 0.87f, 0.5f);    // Green color for income
    public Color expensesColor = new Color(0.94f, 0.27f, 0.27f); // Red color for expenses
    public Color balanceColor = new Color(0.98f, 0.8f, 0.08f);   // Yellow color for balance
    
    [Header("Visual Elements")]
    public GameObject imperialLogo;               // First Order/Imperial logo
    public GameObject redSeparator;               // Red line separator
    
    // Reference to the GameManager
    private GameManager gameManager;
    
    // History of daily reports for easy reference
    private List<ImperialReportData> reportHistory = new List<ImperialReportData>();
    
    void Start()
    {
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
        
        // Hide the panel initially
        if (dailyReportPanel)
        {
            dailyReportPanel.SetActive(false);
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
        if (dailyReportPanel == null) return;
        
        // Store previous credits for display
        int previousCredits = currentCredits;
        
        // Calculate new balance
        int newCredits = previousCredits + salary - expenses;
        
        // Show the report panel
        dailyReportPanel.SetActive(true);
        
        // Update header text
        if (reportHeaderText)
        {
            reportHeaderText.text = $"DAILY PERFORMANCE REPORT - DAY {day}";
        }
        
        // Update salary text
        if (salaryText)
        {
            salaryText.text = $"<b>IMPERIAL SALARY DETAILS</b>\n\n" +
                            $"Base Duty Compensation: {baseSalary} credits\n" +
                            $"Ships Processed: {shipsProcessed}/{requiredShips}\n";
                            
            // Add bonus information if quota exceeded
            if (shipsProcessed > requiredShips)
            {
                int bonusShips = shipsProcessed - requiredShips;
                int bonusCredits = 5 * bonusShips; // Assuming 5 credits per extra ship
                salaryText.text += $"Efficiency Bonus: +{bonusCredits} credits\n";
            }
            
            // Add penalty information
            if (currentStrikes > 0)
            {
                salaryText.text += $"Protocol Violations: -{currentStrikes * penaltyPerMistake} credits\n";
            }
            
            // Add total
            salaryText.text += $"\n<b>Total Compensation:</b> {salary} credits";
        }
        
        // Update expenses text
        if (expensesText)
        {
            expensesText.text = $"<b>PERSONAL EXPENDITURES</b>\n\n";
            
            // Add each expense item with imperial theming
            foreach (var expense in expenseBreakdown)
            {
                string expenseName = expense.Key;
                
                // Add more descriptive text based on expense type
                switch (expense.Key)
                {
                    case "Medical Care":
                        expenseName = "Specialized Medical Treatment";
                        break;
                    case "Equipment":
                        expenseName = "Technical Equipment Requisition";
                        break;
                    case "Protection":
                        expenseName = "Personnel Security Allocation";
                        break;
                    case "Premium Quarters":
                        expenseName = "Officer-Class Accommodation";
                        break;
                    case "Childcare":
                        expenseName = "Imperial Youth Services";
                        break;
                }
                
                expensesText.text += $"{expenseName}: {expense.Value} credits\n";
            }
            
            // Add total expenses
            expensesText.text += $"\n<b>Total Expenditures:</b> {expenses} credits";
        }
        
        // Update financial summary
        string colorHexIncome = ColorUtility.ToHtmlStringRGB(incomeColor);
        string colorHexExpenses = ColorUtility.ToHtmlStringRGB(expensesColor);
        string colorHexBalance = ColorUtility.ToHtmlStringRGB(balanceColor);
        
        if (previousBalanceText)
            previousBalanceText.text = $"Previous Balance: {previousCredits} credits";
            
        if (incomeText)
            incomeText.text = $"Income: <color=#{colorHexIncome}>+{salary} credits</color>";
            
        if (expensesSummaryText)
            expensesSummaryText.text = $"Expenses: <color=#{colorHexExpenses}>-{expenses} credits</color>";
            
        if (creditsTotalText)
        {
            creditsTotalText.text = $"Remaining Credits: <color=#{colorHexBalance}>{newCredits} credits</color>";
            
            // Add a warning message if credits are low
            if (newCredits < 20)
            {
                creditsTotalText.text += "\n\n<color=#FF4500>NOTICE: Low credit balance may impact family amenities.</color>";
            }
        }
        
        // Update family status
        if (familyStatusText && familyStatus != null)
        {
            familyStatusText.text = "<b>FAMILY STATUS REPORT</b>\n\n" + familyStatus.GetStatusSummary();
        }
        
        // Add to report history
        SaveReportData(day, previousCredits, salary, expenses, newCredits, familyStatus);
    }
    
    /// <summary>
    /// Called when the continue button is clicked
    /// </summary>
    void OnContinueClicked()
    {
        // Hide the panel
        dailyReportPanel.SetActive(false);
        
        // Tell the game manager to start the next day
        if (gameManager)
        {
            gameManager.StartNextDay();
        }
    }
    
    /// <summary>
    /// Saves the report data for history
    /// </summary>
    private void SaveReportData(int day, int previousCredits, int salary, 
                              int expenses, int newCredits, FamilyStatusInfo familyStatus)
    {
        ImperialReportData report = new ImperialReportData
        {
            Day = day,
            PreviousCredits = previousCredits,
            Salary = salary,
            Expenses = expenses,
            EndingCredits = newCredits,
            FamilyStatus = familyStatus
        };
        
        reportHistory.Add(report);
        
        // Here you could also save using Easy Save if desired
        // ES3.Save("dayReports/" + day, report);
    }
}

/// <summary>
/// Data structure to store imperial daily report information
/// </summary>
[System.Serializable]
public class ImperialReportData
{
    public int Day;
    public int PreviousCredits;
    public int Salary;
    public int Expenses;
    public int EndingCredits;
    public FamilyStatusInfo FamilyStatus;
    
    // Could add additional fields like:
    public int ImperialLoyaltyScore;
    public string[] KeyDecisions;
}
