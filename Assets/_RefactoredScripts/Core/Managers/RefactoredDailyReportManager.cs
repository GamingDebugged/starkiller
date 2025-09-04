using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages daily reports, end-of-day summaries, and day transitions
    /// Extracted from GameManager for focused daily reporting responsibility
    /// This is the refactored version that integrates with the new manager architecture
    /// </summary>
    public class RefactoredDailyReportManager : MonoBehaviour
    {
        [Header("Report Settings")]
        [SerializeField] private bool enableDailyReports = true;
        [SerializeField] private bool enableAutoAdvance = false;
        [SerializeField] private float autoAdvanceDelay = 5f;
        [SerializeField] private bool showDetailedBreakdown = true;
        [SerializeField] private bool enableReportLogging = true;
        
        [Header("UI References")]
        [SerializeField] private GameObject dailyReportPanel;
        [SerializeField] private TMP_Text salaryText;
        [SerializeField] private TMP_Text expensesText;
        [SerializeField] private TMP_Text summaryText;
        [SerializeField] private TMP_Text performanceText;
        [SerializeField] private Button continueButton;
        [SerializeField] private GameObject reportBackground;
        
        [Header("Report Components")]
        [SerializeField] private GameObject salaryBreakdownPanel;
        [SerializeField] private GameObject expenseBreakdownPanel;
        [SerializeField] private GameObject performanceMetricsPanel;
        [SerializeField] private GameObject loyaltyStatusPanel;
        [SerializeField] private TMP_Text loyaltyStatusText;
        
        [Header("Visual Elements")]
        [SerializeField] private Image dailyGradeImage;
        [SerializeField] private Sprite[] gradeSprites; // F, D, C, B, A grades
        [SerializeField] private Color[] gradeColors;
        [SerializeField] private GameObject[] performanceStars;
        
        [Header("Audio")]
        [SerializeField] private bool enableReportSounds = true;
        [SerializeField] private string reportGeneratedSound = "report_generated";
        [SerializeField] private string dayCompleteSound = "day_complete";
        [SerializeField] private string newDaySound = "new_day";
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableReportDebugging = true;
        
        // Daily report state
        private bool _isReportActive = false;
        private bool _isTransitioningDay = false;
        private DailyReport _currentReport;
        private List<DailyReport> _reportHistory = new List<DailyReport>();
        private int _currentDay = 1;
        
        // Performance tracking
        private int _shipsProcessed = 0;
        private int _correctDecisions = 0;
        private int _wrongDecisions = 0;
        private int _currentStrikes = 0;
        private int _totalCredits = 0;
        
        // Dependencies
        private SalaryManager _salaryManager;
        private PerformanceManager _performanceManager;
        private CreditsManager _creditsManager;
        private LoyaltyManager _loyaltyManager;
        private DayProgressionManager _dayManager;
        private AudioManager _audioManager;
        private UICoordinator _uiCoordinator;
        private NotificationManager _notificationManager;
        
        // Events
        public static event Action<DailyReport> OnDailyReportGenerated;
        public static event Action<DailyReport> OnDailyReportDisplayed;
        public static event Action<int> OnNextDayRequested;
        public static event Action<bool> OnReportStateChanged;
        public static event Action<DailyReport> OnReportCompleted;
        
        // Public properties
        public bool IsReportActive => _isReportActive;
        public bool IsTransitioningDay => _isTransitioningDay;
        public DailyReport CurrentReport => _currentReport;
        public List<DailyReport> ReportHistory => new List<DailyReport>(_reportHistory);
        public int CurrentDay => _currentDay;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<RefactoredDailyReportManager>(this);
            
            if (enableDebugLogs)
                Debug.Log("[RefactoredDailyReportManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _salaryManager = ServiceLocator.Get<SalaryManager>();
            _performanceManager = ServiceLocator.Get<PerformanceManager>();
            _creditsManager = ServiceLocator.Get<CreditsManager>();
            _loyaltyManager = ServiceLocator.Get<LoyaltyManager>();
            _dayManager = ServiceLocator.Get<DayProgressionManager>();
            _audioManager = ServiceLocator.Get<AudioManager>();
            _uiCoordinator = ServiceLocator.Get<UICoordinator>();
            _notificationManager = ServiceLocator.Get<NotificationManager>();
            
            // Subscribe to events
            if (_dayManager != null)
            {
                DayProgressionManager.OnShiftEnded += OnShiftEnded;
                DayProgressionManager.OnDayStarted += OnDayStarted;
            }
            
            // Setup UI
            SetupUI();
            
            // Initially hide report panel
            if (dailyReportPanel != null)
                dailyReportPanel.SetActive(false);
            
            if (enableDebugLogs)
                Debug.Log("[RefactoredDailyReportManager] Daily report system ready");
        }
        
        /// <summary>
        /// Setup UI components and event listeners
        /// </summary>
        private void SetupUI()
        {
            // Only setup continue button if we have our own UI panel assigned
            if (continueButton != null && dailyReportPanel != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(RequestNextDay);
                
                if (enableDebugLogs)
                    Debug.Log("[RefactoredDailyReportManager] Setup RefactoredDailyReportManager continue button");
            }
            else if (enableDebugLogs)
            {
                Debug.Log("[RefactoredDailyReportManager] Skipping button setup - using original DailyReportManager UI");
            }
        }
        
        /// <summary>
        /// Generate and display daily report
        /// </summary>
        public void GenerateDailyReport()
        {
            GenerateDailyReport(true); // Default: show UI
        }
        
        /// <summary>
        /// Generate daily report with option to skip UI display
        /// </summary>
        public void GenerateDailyReport(bool showUI)
        {
            if (!enableDailyReports)
            {
                if (enableDebugLogs)
                    Debug.Log("[RefactoredDailyReportManager] Daily reports disabled");
                return;
            }
            
            if (_isReportActive)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[RefactoredDailyReportManager] Daily report already active");
                return;
            }
            
            // Collect data for report
            var reportData = CollectReportData();
            
            // Create daily report
            _currentReport = CreateDailyReport(reportData);
            
            // Display the report only if showUI is true
            if (showUI)
            {
                DisplayDailyReport(_currentReport);
            }
            else
            {
                // Just set the state without showing UI
                _isReportActive = true;
                OnReportStateChanged?.Invoke(true);
                
                if (enableDebugLogs)
                    Debug.Log("[RefactoredDailyReportManager] Daily report generated (no UI) for logic/events only");
            }
            
            // Record in history
            _reportHistory.Add(_currentReport);
            
            // Trigger events
            OnDailyReportGenerated?.Invoke(_currentReport);
            if (showUI)
            {
                OnDailyReportDisplayed?.Invoke(_currentReport);
            }
            
            if (enableReportLogging)
                Debug.Log($"[RefactoredDailyReportManager] Daily report generated for day {_currentDay} (UI: {showUI})");
        }
        
        /// <summary>
        /// Collect data needed for daily report
        /// </summary>
        private DailyReportData CollectReportData()
        {
            var data = new DailyReportData();
            
            // Performance data
            if (_performanceManager != null)
            {
                data.ShipsProcessed = _performanceManager.ShipsProcessed;
                data.CorrectDecisions = _performanceManager.CorrectDecisions;
                data.WrongDecisions = _performanceManager.WrongDecisions;
                data.CurrentStrikes = _performanceManager.CurrentStrikes;
                data.AccuracyRate = _performanceManager.AccuracyRate;
                data.AverageDecisionTime = _performanceManager.AverageDecisionTime;
            }
            
            // Salary data
            if (_salaryManager != null)
            {
                data.DailySalary = _salaryManager.CurrentSalary;
                data.SalaryBreakdown = _salaryManager.CurrentBreakdown;
            }
            
            // Credits data
            if (_creditsManager != null)
            {
                data.CurrentCredits = _creditsManager.CurrentCredits;
                data.CreditsEarned = _creditsManager.CreditsEarnedToday;
                data.CreditsSpent = _creditsManager.CreditsSpentToday;
            }
            
            // Loyalty data
            if (_loyaltyManager != null)
            {
                data.ImperialLoyalty = _loyaltyManager.ImperialLoyalty;
                data.RebellionSympathy = _loyaltyManager.RebellionSympathy;
                data.LoyaltyChanges = new List<string>(); // TODO: Implement loyalty change tracking
            }
            
            // Day information
            if (_dayManager != null)
            {
                data.Day = _dayManager.CurrentDay;
                data.ShiftDuration = _dayManager.RemainingTime;
                data.RequiredShips = 10; // TODO: Get from day manager configuration
            }
            
            return data;
        }
        
        /// <summary>
        /// Create daily report from collected data
        /// </summary>
        private DailyReport CreateDailyReport(DailyReportData data)
        {
            var report = new DailyReport
            {
                Day = data.Day,
                Date = DateTime.Now,
                ShipsProcessed = data.ShipsProcessed,
                RequiredShips = data.RequiredShips,
                CorrectDecisions = data.CorrectDecisions,
                WrongDecisions = data.WrongDecisions,
                CurrentStrikes = data.CurrentStrikes,
                AccuracyRate = data.AccuracyRate,
                DailySalary = data.DailySalary,
                SalaryBreakdown = data.SalaryBreakdown,
                CurrentCredits = data.CurrentCredits,
                CreditsEarned = data.CreditsEarned,
                CreditsSpent = data.CreditsSpent,
                ImperialLoyalty = data.ImperialLoyalty,
                RebellionSympathy = data.RebellionSympathy,
                LoyaltyChanges = data.LoyaltyChanges,
                PerformanceGrade = CalculatePerformanceGrade(data),
                DayComplete = true,
                Expenses = CalculateExpenses()
            };
            
            return report;
        }
        
        /// <summary>
        /// Calculate performance grade based on daily performance
        /// </summary>
        private PerformanceGrade CalculatePerformanceGrade(DailyReportData data)
        {
            float score = 0f;
            
            // Accuracy score (40% weight)
            score += data.AccuracyRate * 0.4f;
            
            // Efficiency score (30% weight)
            float efficiency = data.RequiredShips > 0 ? (float)data.ShipsProcessed / data.RequiredShips : 0f;
            score += Mathf.Min(efficiency, 1f) * 0.3f;
            
            // Strike penalty (30% weight)
            float strikeScore = data.CurrentStrikes == 0 ? 1f : Mathf.Max(0f, 1f - (data.CurrentStrikes * 0.25f));
            score += strikeScore * 0.3f;
            
            // Convert to grade
            if (score >= 0.9f) return PerformanceGrade.A;
            if (score >= 0.8f) return PerformanceGrade.B;
            if (score >= 0.7f) return PerformanceGrade.C;
            if (score >= 0.6f) return PerformanceGrade.D;
            return PerformanceGrade.F;
        }
        
        /// <summary>
        /// Calculate daily expenses
        /// </summary>
        private Dictionary<string, int> CalculateExpenses()
        {
            var expenses = new Dictionary<string, int>();
            
            // Base living expenses
            expenses.Add("Premium Quarters", 15);
            expenses.Add("Childcare", 10);
            
            // Periodic expenses based on day
            if (_currentDay % 3 == 0)
            {
                expenses.Add("Medical Care", 30);
            }
            
            if (_currentDay % 4 == 1)
            {
                expenses.Add("Equipment", 25);
            }
            
            if (_currentDay % 5 == 2)
            {
                expenses.Add("Training", 20);
            }
            
            return expenses;
        }
        
        /// <summary>
        /// Display daily report UI
        /// </summary>
        private void DisplayDailyReport(DailyReport report)
        {
            if (dailyReportPanel == null)
            {
                Debug.LogError("[RefactoredDailyReportManager] Daily report panel not assigned!");
                return;
            }
            
            _isReportActive = true;
            OnReportStateChanged?.Invoke(true);
            
            // Show report panel
            dailyReportPanel.SetActive(true);
            
            // Update summary text
            if (summaryText != null)
            {
                summaryText.text = GenerateSummaryText(report);
            }
            
            // Update salary text
            if (salaryText != null)
            {
                salaryText.text = GenerateSalaryText(report);
            }
            
            // Update expenses text
            if (expensesText != null)
            {
                expensesText.text = GenerateExpensesText(report);
            }
            
            // Update performance text
            if (performanceText != null)
            {
                performanceText.text = GeneratePerformanceText(report);
            }
            
            // Update loyalty status
            if (loyaltyStatusText != null)
            {
                loyaltyStatusText.text = GenerateLoyaltyText(report);
            }
            
            // Update visual elements
            UpdateVisualElements(report);
            
            // Play report sound
            if (_audioManager != null && enableReportSounds)
            {
                _audioManager.PlaySound(reportGeneratedSound);
            }
            
            // Auto-advance if enabled
            if (enableAutoAdvance)
            {
                StartCoroutine(AutoAdvanceToNextDay());
            }
        }
        
        /// <summary>
        /// Generate summary text for the report
        /// </summary>
        private string GenerateSummaryText(DailyReport report)
        {
            var text = new System.Text.StringBuilder();
            
            text.AppendLine($"<size=24><b>DAY {report.Day} REPORT</b></size>");
            text.AppendLine($"<size=18>{report.Date:MMM dd, yyyy}</size>");
            text.AppendLine();
            
            text.AppendLine($"<b>Performance Grade:</b> <color={(GetGradeColor(report.PerformanceGrade))}>{report.PerformanceGrade}</color>");
            text.AppendLine($"<b>Ships Processed:</b> {report.ShipsProcessed}/{report.RequiredShips}");
            text.AppendLine($"<b>Accuracy:</b> {report.AccuracyRate:P1}");
            text.AppendLine($"<b>Current Strikes:</b> {report.CurrentStrikes}/3");
            text.AppendLine();
            
            text.AppendLine($"<b>Net Credits:</b> {report.CurrentCredits}");
            text.AppendLine($"<b>Earned Today:</b> +{report.CreditsEarned}");
            text.AppendLine($"<b>Spent Today:</b> -{report.CreditsSpent}");
            
            return text.ToString();
        }
        
        /// <summary>
        /// Generate salary breakdown text
        /// </summary>
        private string GenerateSalaryText(DailyReport report)
        {
            var text = new System.Text.StringBuilder();
            
            text.AppendLine("<b>SALARY BREAKDOWN</b>");
            text.AppendLine();
            
            if (report.SalaryBreakdown != null)
            {
                text.AppendLine($"Base Salary: {report.SalaryBreakdown.BaseSalary} credits");
                
                if (report.SalaryBreakdown.ExtraShipBonus > 0)
                {
                    text.AppendLine($"Extra Ships ({report.SalaryBreakdown.ExtraShipsProcessed}): +{report.SalaryBreakdown.ExtraShipBonus} credits");
                }
                
                if (report.SalaryBreakdown.PerformanceBonus > 0)
                {
                    text.AppendLine($"Performance Bonus: +{report.SalaryBreakdown.PerformanceBonus} credits");
                }
                
                if (report.SalaryBreakdown.PerfectDayBonus > 0)
                {
                    text.AppendLine($"Perfect Day Bonus: +{report.SalaryBreakdown.PerfectDayBonus} credits");
                }
                
                if (report.SalaryBreakdown.StrikePenalty > 0)
                {
                    text.AppendLine($"Strike Penalty ({report.SalaryBreakdown.StrikesIncurred}): -{report.SalaryBreakdown.StrikePenalty} credits");
                }
                
                text.AppendLine();
                text.AppendLine($"<b>Total Salary: {report.SalaryBreakdown.TotalSalary} credits</b>");
            }
            else
            {
                text.AppendLine($"Daily Salary: {report.DailySalary} credits");
            }
            
            return text.ToString();
        }
        
        /// <summary>
        /// Generate expenses breakdown text
        /// </summary>
        private string GenerateExpensesText(DailyReport report)
        {
            var text = new System.Text.StringBuilder();
            
            text.AppendLine("<b>DAILY EXPENSES</b>");
            text.AppendLine();
            
            int totalExpenses = 0;
            foreach (var expense in report.Expenses)
            {
                text.AppendLine($"{expense.Key}: {expense.Value} credits");
                totalExpenses += expense.Value;
            }
            
            text.AppendLine();
            text.AppendLine($"<b>Total Expenses: {totalExpenses} credits</b>");
            
            return text.ToString();
        }
        
        /// <summary>
        /// Generate performance metrics text
        /// </summary>
        private string GeneratePerformanceText(DailyReport report)
        {
            var text = new System.Text.StringBuilder();
            
            text.AppendLine("<b>PERFORMANCE METRICS</b>");
            text.AppendLine();
            
            text.AppendLine($"Correct Decisions: {report.CorrectDecisions}");
            text.AppendLine($"Wrong Decisions: {report.WrongDecisions}");
            text.AppendLine($"Accuracy Rate: {report.AccuracyRate:P1}");
            text.AppendLine($"Ships Processed: {report.ShipsProcessed}");
            text.AppendLine($"Required Ships: {report.RequiredShips}");
            
            float efficiency = report.RequiredShips > 0 ? (float)report.ShipsProcessed / report.RequiredShips : 0f;
            text.AppendLine($"Efficiency: {efficiency:P1}");
            
            return text.ToString();
        }
        
        /// <summary>
        /// Generate loyalty status text
        /// </summary>
        private string GenerateLoyaltyText(DailyReport report)
        {
            var text = new System.Text.StringBuilder();
            
            text.AppendLine("<b>LOYALTY STATUS</b>");
            text.AppendLine();
            
            text.AppendLine($"Imperial Loyalty: {report.ImperialLoyalty}/10");
            text.AppendLine($"Rebellion Sympathy: {report.RebellionSympathy}/10");
            
            if (report.LoyaltyChanges != null && report.LoyaltyChanges.Count > 0)
            {
                text.AppendLine();
                text.AppendLine("Today's Changes:");
                foreach (var change in report.LoyaltyChanges)
                {
                    text.AppendLine($"• {change}");
                }
            }
            
            return text.ToString();
        }
        
        /// <summary>
        /// Update visual elements based on report
        /// </summary>
        private void UpdateVisualElements(DailyReport report)
        {
            // Update grade display
            if (dailyGradeImage != null && gradeSprites != null)
            {
                int gradeIndex = (int)report.PerformanceGrade;
                if (gradeIndex >= 0 && gradeIndex < gradeSprites.Length)
                {
                    dailyGradeImage.sprite = gradeSprites[gradeIndex];
                }
            }
            
            // Update performance stars
            if (performanceStars != null)
            {
                int starCount = GetStarCount(report.PerformanceGrade);
                for (int i = 0; i < performanceStars.Length; i++)
                {
                    if (performanceStars[i] != null)
                    {
                        performanceStars[i].SetActive(i < starCount);
                    }
                }
            }
        }
        
        /// <summary>
        /// Get color for performance grade
        /// </summary>
        private string GetGradeColor(PerformanceGrade grade)
        {
            return grade switch
            {
                PerformanceGrade.A => "#00FF00",
                PerformanceGrade.B => "#FFFF00",
                PerformanceGrade.C => "#FFA500",
                PerformanceGrade.D => "#FF4500",
                PerformanceGrade.F => "#FF0000",
                _ => "#FFFFFF"
            };
        }
        
        /// <summary>
        /// Get star count for performance grade
        /// </summary>
        private int GetStarCount(PerformanceGrade grade)
        {
            return grade switch
            {
                PerformanceGrade.A => 5,
                PerformanceGrade.B => 4,
                PerformanceGrade.C => 3,
                PerformanceGrade.D => 2,
                PerformanceGrade.F => 1,
                _ => 0
            };
        }
        
        
        /// <summary>
        /// Request next day transition
        /// </summary>
        public void RequestNextDay()
        {
            if (_isTransitioningDay)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[RefactoredDailyReportManager] Already transitioning to next day");
                return;
            }
            
            _isTransitioningDay = true;
            
            // Play day complete sound
            if (_audioManager != null && enableReportSounds)
            {
                _audioManager.PlaySound(dayCompleteSound);
            }
            
            // Hide report panel
            if (dailyReportPanel != null)
                dailyReportPanel.SetActive(false);
            
            _isReportActive = false;
            OnReportStateChanged?.Invoke(false);
            
            // Complete the current report
            if (_currentReport != null)
            {
                OnReportCompleted?.Invoke(_currentReport);
            }
            
            // Request next day through DayProgressionManager (preserves existing flow)
            if (_dayManager != null)
            {
                Debug.Log($"[RefactoredDailyReportManager] Current day before increment: {_dayManager.CurrentDay}");
                _dayManager.StartNewDay();
                Debug.Log($"[RefactoredDailyReportManager] Day after increment: {_dayManager.CurrentDay}");
                _currentDay = _dayManager.CurrentDay; // Sync our internal counter
            }
            else
            {
                // Fallback: increment our own counter and trigger event
                _currentDay++;
                OnNextDayRequested?.Invoke(_currentDay);
                Debug.LogWarning("[RefactoredDailyReportManager] DayProgressionManager not found, using fallback day increment");
            }
            
            // Show Personal Data Log for the NEW day (same as original DailyReportManager)
            var personalDataLogManager = ServiceLocator.Get<PersonalDataLogManager>();
            if (personalDataLogManager != null)
            {
                Debug.Log("[RefactoredDailyReportManager] Showing Personal Data Log");
                personalDataLogManager.ShowDataLog();
            }
            else
            {
                Debug.LogWarning("[RefactoredDailyReportManager] PersonalDataLogManager not found, skipping Personal Data Log");
            }
            
            // Play new day sound
            if (_audioManager != null && enableReportSounds)
            {
                _audioManager.PlaySound(newDaySound);
            }
            
            if (enableReportLogging)
                Debug.Log($"[RefactoredDailyReportManager] Transitioning to day {_currentDay}");
            
            // Reset transition flag after delay
            StartCoroutine(ResetTransitionFlag());
        }
        
        /// <summary>
        /// Auto-advance to next day
        /// </summary>
        private System.Collections.IEnumerator AutoAdvanceToNextDay()
        {
            yield return new WaitForSeconds(autoAdvanceDelay);
            
            if (_isReportActive)
            {
                RequestNextDay();
            }
        }
        
        /// <summary>
        /// Reset transition flag
        /// </summary>
        private System.Collections.IEnumerator ResetTransitionFlag()
        {
            yield return new WaitForSeconds(0.5f);
            _isTransitioningDay = false;
        }
        
        /// <summary>
        /// Get daily report statistics
        /// </summary>
        public DailyReportStatistics GetStatistics()
        {
            return new DailyReportStatistics
            {
                TotalReports = _reportHistory.Count,
                AverageGrade = _reportHistory.Count > 0 ? _reportHistory.Average(r => (float)r.PerformanceGrade) : 0f,
                BestGrade = _reportHistory.Count > 0 ? _reportHistory.Min(r => r.PerformanceGrade) : PerformanceGrade.F,
                TotalShipsProcessed = _reportHistory.Sum(r => r.ShipsProcessed),
                TotalCreditsEarned = _reportHistory.Sum(r => r.CreditsEarned),
                AverageAccuracy = _reportHistory.Count > 0 ? _reportHistory.Average(r => r.AccuracyRate) : 0f,
                ReportHistory = new List<DailyReport>(_reportHistory)
            };
        }
        
        /// <summary>
        /// Reset daily report tracking
        /// </summary>
        public void ResetReportTracking()
        {
            _reportHistory.Clear();
            _currentReport = null;
            _currentDay = 1;
            _isReportActive = false;
            _isTransitioningDay = false;
            
            if (dailyReportPanel != null)
                dailyReportPanel.SetActive(false);
            
            if (enableDebugLogs)
                Debug.Log("[RefactoredDailyReportManager] Daily report tracking reset");
        }
        
        // Event handlers
        private void OnShiftEnded()
        {
            GenerateDailyReport();
        }
        
        private void OnDayStarted(int day)
        {
            _currentDay = day;
            
            if (enableDebugLogs)
                Debug.Log($"[RefactoredDailyReportManager] Day {day} started");
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_dayManager != null)
            {
                DayProgressionManager.OnShiftEnded -= OnShiftEnded;
                DayProgressionManager.OnDayStarted -= OnDayStarted;
            }
            
            // Clear event subscriptions
            OnDailyReportGenerated = null;
            OnDailyReportDisplayed = null;
            OnNextDayRequested = null;
            OnReportStateChanged = null;
            OnReportCompleted = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Generate Daily Report")]
        private void TestGenerateDailyReport()
        {
            GenerateDailyReport();
        }
        
        [ContextMenu("Test: Request Next Day")]
        private void TestRequestNextDay()
        {
            RequestNextDay();
        }
        
        [ContextMenu("Show Report Statistics")]
        private void ShowReportStatistics()
        {
            var stats = GetStatistics();
            Debug.Log("=== DAILY REPORT STATISTICS ===");
            Debug.Log($"Total Reports: {stats.TotalReports}");
            Debug.Log($"Average Grade: {stats.AverageGrade:F1}");
            Debug.Log($"Best Grade: {stats.BestGrade}");
            Debug.Log($"Total Ships Processed: {stats.TotalShipsProcessed}");
            Debug.Log($"Total Credits Earned: {stats.TotalCreditsEarned}");
            Debug.Log($"Average Accuracy: {stats.AverageAccuracy:P1}");
            Debug.Log("=== END STATISTICS ===");
        }
    }
    
    // Supporting data structures
    [System.Serializable]
    public class DailyReportData
    {
        public int Day;
        public int ShipsProcessed;
        public int RequiredShips;
        public int CorrectDecisions;
        public int WrongDecisions;
        public int CurrentStrikes;
        public float AccuracyRate;
        public float AverageDecisionTime;
        public int DailySalary;
        public SalaryBreakdown SalaryBreakdown;
        public int CurrentCredits;
        public int CreditsEarned;
        public int CreditsSpent;
        public int ImperialLoyalty;
        public int RebellionSympathy;
        public List<string> LoyaltyChanges;
        public float ShiftDuration;
    }
    
    [System.Serializable]
    public class DailyReport
    {
        public int Day;
        public DateTime Date;
        public int ShipsProcessed;
        public int RequiredShips;
        public int CorrectDecisions;
        public int WrongDecisions;
        public int CurrentStrikes;
        public float AccuracyRate;
        public int DailySalary;
        public SalaryBreakdown SalaryBreakdown;
        public int CurrentCredits;
        public int CreditsEarned;
        public int CreditsSpent;
        public int ImperialLoyalty;
        public int RebellionSympathy;
        public List<string> LoyaltyChanges;
        public PerformanceGrade PerformanceGrade;
        public bool DayComplete;
        public Dictionary<string, int> Expenses;
    }
    
    [System.Serializable]
    public struct DailyReportStatistics
    {
        public int TotalReports;
        public float AverageGrade;
        public PerformanceGrade BestGrade;
        public int TotalShipsProcessed;
        public int TotalCreditsEarned;
        public float AverageAccuracy;
        public List<DailyReport> ReportHistory;
    }
    
    public enum PerformanceGrade
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        F = 4
    }
    
    // Extension methods for LINQ support
    public static class DailyReportExtensions
    {
        public static float Average<T>(this IEnumerable<T> source, Func<T, float> selector)
        {
            float sum = 0f;
            int count = 0;
            
            foreach (var item in source)
            {
                sum += selector(item);
                count++;
            }
            
            return count > 0 ? sum / count : 0f;
        }
        
        public static int Sum<T>(this IEnumerable<T> source, Func<T, int> selector)
        {
            int sum = 0;
            foreach (var item in source)
            {
                sum += selector(item);
            }
            return sum;
        }
        
        public static T Min<T>(this IEnumerable<T> source, Func<T, T> selector) where T : IComparable<T>
        {
            T min = default(T);
            bool hasValue = false;
            
            foreach (var item in source)
            {
                T value = selector(item);
                if (!hasValue || value.CompareTo(min) < 0)
                {
                    min = value;
                    hasValue = true;
                }
            }
            
            return hasValue ? min : default(T);
        }
    }
}