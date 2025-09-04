using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using System.Linq;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages game over conditions, detection, and UI presentation
    /// Extracted from GameManager for focused game over responsibility
    /// </summary>
    public class GameOverManager : MonoBehaviour
    {
        [Header("Game Over Settings")]
        [SerializeField] private int maxMistakes = 3;
        [SerializeField] private int minCreditsThreshold = 0;
        [SerializeField] private bool enableAutoGameOver = true;
        [SerializeField] private float gameOverDelay = 1f;
        
        [Header("Game Over Conditions")]
        [SerializeField] private bool enableMistakeGameOver = true;
        [SerializeField] private bool enableCreditsGameOver = true;
        [SerializeField] private bool enableTimeGameOver = false;
        [SerializeField] private bool enableLoyaltyGameOver = false;
        [SerializeField] private int criticalLoyaltyThreshold = -10;
        
        [Header("UI References")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private GameObject gameOverBackground;
        
        [Header("Game Over Messages")]
        [SerializeField] private string mistakeGameOverMessage = "You have been terminated due to excessive errors.";
        [SerializeField] private string creditsGameOverMessage = "Your family has been reassigned to a remote outpost due to financial mismanagement.";
        [SerializeField] private string loyaltyGameOverMessage = "Your loyalty to the Imperium has been questioned. You have been reassigned.";
        [SerializeField] private string timeGameOverMessage = "Your shift has ended. Performance review indicates termination.";
        
        [Header("Audio")]
        [SerializeField] private bool enableGameOverSound = true;
        [SerializeField] private string gameOverSoundName = "game_over";
        [SerializeField] private string gameOverMusicName = "game_over_music";
        [SerializeField] private bool stopBackgroundMusic = true;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableGameOverLogging = true;
        
        // Game over state
        private bool _isGameOver = false;
        private bool _isProcessingGameOver = false;
        private GameOverReason _currentGameOverReason = GameOverReason.None;
        private string _gameOverMessage = "";
        private DateTime _gameOverTime = DateTime.MinValue;
        private List<GameOverRecord> _gameOverHistory = new List<GameOverRecord>();
        
        // Game statistics for final display
        private int _finalScore = 0;
        private int _daysSurvived = 0;
        private int _totalShipsProcessed = 0;
        private int _correctDecisions = 0;
        private int _wrongDecisions = 0;
        private int _currentStrikes = 0;
        private int _finalCredits = 0;
        
        // Dependencies
        private PerformanceManager _performanceManager;
        private CreditsManager _creditsManager;
        private LoyaltyManager _loyaltyManager;
        private DayProgressionManager _dayManager;
        private AudioManager _audioManager;
        private UICoordinator _uiCoordinator;
        private NotificationManager _notificationManager;
        
        // Events
        public static event Action<GameOverReason, string> OnGameOverTriggered;
        public static event Action<GameOverRecord> OnGameOverProcessed;
        public static event Action OnRestartRequested;
        public static event Action OnQuitRequested;
        public static event Action<bool> OnGameOverStateChanged;
        
        // Public properties
        public bool IsGameOver => _isGameOver;
        public GameOverReason CurrentGameOverReason => _currentGameOverReason;
        public string GameOverMessage => _gameOverMessage;
        public int MaxMistakes => maxMistakes;
        public int MinCreditsThreshold => minCreditsThreshold;
        public List<GameOverRecord> GameOverHistory => new List<GameOverRecord>(_gameOverHistory);
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<GameOverManager>(this);
            
            if (enableDebugLogs)
                Debug.Log("[GameOverManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _performanceManager = ServiceLocator.Get<PerformanceManager>();
            _creditsManager = ServiceLocator.Get<CreditsManager>();
            _loyaltyManager = ServiceLocator.Get<LoyaltyManager>();
            _dayManager = ServiceLocator.Get<DayProgressionManager>();
            _audioManager = ServiceLocator.Get<AudioManager>();
            _uiCoordinator = ServiceLocator.Get<UICoordinator>();
            _notificationManager = ServiceLocator.Get<NotificationManager>();
            
            // Subscribe to events
            if (_performanceManager != null)
            {
                PerformanceManager.OnStrikesChanged += (current, max) => OnStrikesChanged(current);
            }
            
            if (_creditsManager != null)
            {
                CreditsManager.OnCreditsChanged += (newAmount, changeAmount) => OnCreditsChanged(newAmount, "Credits changed");
            }
            
            if (_loyaltyManager != null)
            {
                LoyaltyManager.OnLoyaltyChanged += OnLoyaltyChanged;
            }
            
            // Subscribe to GameEvents for game over triggers
            GameEvents.OnGameEnded += OnGameEndedEvent;
            
            // Subscribe to DecisionTracker events if available
            DecisionTracker.OnGameOverTriggered += OnDecisionTrackerGameOver;
            
            // Setup UI
            SetupUI();
            
            // Initially hide game over panel
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
            
            if (enableDebugLogs)
                Debug.Log("[GameOverManager] Game over system ready");
        }
        
        /// <summary>
        /// Setup UI components and event listeners
        /// </summary>
        private void SetupUI()
        {
            // Setup restart button
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(RequestRestart);
            }
            
            // Setup quit button
            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(RequestQuit);
            }
        }
        
        /// <summary>
        /// Check for game over conditions
        /// </summary>
        public void CheckGameOverConditions()
        {
            if (_isGameOver || _isProcessingGameOver || !enableAutoGameOver)
                return;
            
            // Check mistake-based game over
            if (enableMistakeGameOver && _performanceManager != null)
            {
                int currentStrikes = _performanceManager.CurrentStrikes;
                if (currentStrikes >= maxMistakes)
                {
                    TriggerGameOver(GameOverReason.TooManyMistakes, mistakeGameOverMessage);
                    return;
                }
            }
            
            // Check credits-based game over
            if (enableCreditsGameOver && _creditsManager != null)
            {
                int currentCredits = _creditsManager.CurrentCredits;
                if (currentCredits < minCreditsThreshold)
                {
                    TriggerGameOver(GameOverReason.InsufficientCredits, creditsGameOverMessage);
                    return;
                }
            }
            
            // Check loyalty-based game over
            if (enableLoyaltyGameOver && _loyaltyManager != null)
            {
                int imperialLoyalty = _loyaltyManager.ImperialLoyalty;
                if (imperialLoyalty <= criticalLoyaltyThreshold)
                {
                    TriggerGameOver(GameOverReason.LoyaltyFailure, loyaltyGameOverMessage);
                    return;
                }
            }
        }
        
        /// <summary>
        /// Trigger game over with specific reason
        /// </summary>
        public void TriggerGameOver(GameOverReason reason, string message)
        {
            if (_isGameOver || _isProcessingGameOver)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[GameOverManager] Game over already triggered or processing");
                return;
            }
            
            _isProcessingGameOver = true;
            _currentGameOverReason = reason;
            _gameOverMessage = message;
            _gameOverTime = DateTime.Now;
            
            // Trigger event
            OnGameOverTriggered?.Invoke(reason, message);
            
            // Collect final statistics
            CollectFinalStatistics();
            
            // Start game over sequence
            StartCoroutine(ProcessGameOver());
            
            if (enableGameOverLogging)
                Debug.Log($"[GameOverManager] Game over triggered: {reason} - {message}");
        }
        
        /// <summary>
        /// Process game over sequence
        /// </summary>
        private System.Collections.IEnumerator ProcessGameOver()
        {
            // Wait for game over delay
            yield return new WaitForSeconds(gameOverDelay);
            
            // Set game over state
            _isGameOver = true;
            OnGameOverStateChanged?.Invoke(true);
            
            // Stop background music if enabled
            if (stopBackgroundMusic && _audioManager != null)
            {
                _audioManager.StopAllSounds();
            }
            
            // Play game over sound
            if (enableGameOverSound && _audioManager != null)
            {
                _audioManager.PlaySound(gameOverSoundName);
                
                // Play game over music if available
                if (!string.IsNullOrEmpty(gameOverMusicName))
                {
                    _audioManager.PlayMusic(gameOverMusicName);
                }
            }
            
            // Pause the game and stop day progression
            var gameStateManager = ServiceLocator.Get<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.PauseGame();
            }
            
            // Stop day progression completely
            var dayProgressionManager = ServiceLocator.Get<DayProgressionManager>();
            if (dayProgressionManager != null)
            {
                dayProgressionManager.StopDayProgression();
            }
            
            // Stop shift timer
            var shiftTimerManager = ServiceLocator.Get<ShiftTimerManager>();
            if (shiftTimerManager != null)
            {
                shiftTimerManager.StopTimer();
            }
            
            // Change game state to GameOver immediately
            GameEvents.TriggerGameStateChanged(Starkiller.Core.GameState.GameOver);
            
            // Display game over UI
            DisplayGameOverUI();
            
            // Record game over
            RecordGameOver();
            
            _isProcessingGameOver = false;
            
            if (enableGameOverLogging)
                Debug.Log("[GameOverManager] Game over sequence completed");
        }
        
        /// <summary>
        /// Display game over UI
        /// </summary>
        private void DisplayGameOverUI()
        {
            if (gameOverPanel == null)
            {
                Debug.LogError("[GameOverManager] Game over panel not assigned!");
                return;
            }
            
            // Show game over panel
            gameOverPanel.SetActive(true);
            
            // Update final score text
            if (finalScoreText != null)
            {
                string scoreText = GenerateFinalScoreText();
                finalScoreText.text = scoreText;
            }
            
            // Show background if available
            if (gameOverBackground != null)
            {
                gameOverBackground.SetActive(true);
            }
        }
        
        /// <summary>
        /// Generate final score display text
        /// </summary>
        private string GenerateFinalScoreText()
        {
            var text = new System.Text.StringBuilder();
            
            // Main game over message
            text.AppendLine("GAME OVER");
            text.AppendLine();
            text.AppendLine(_gameOverMessage);
            text.AppendLine();
            
            // Statistics
            text.AppendLine($"Final Score: {_finalScore}");
            text.AppendLine($"Days Survived: {_daysSurvived}");
            text.AppendLine($"Total Ships Processed: {_totalShipsProcessed}");
            text.AppendLine($"Correct Decisions: {_correctDecisions}");
            text.AppendLine($"Wrong Decisions: {_wrongDecisions}");
            text.AppendLine($"Final Credits: {_finalCredits}");
            
            // Accuracy calculation
            if (_correctDecisions + _wrongDecisions > 0)
            {
                float accuracy = (float)_correctDecisions / (_correctDecisions + _wrongDecisions) * 100f;
                text.AppendLine($"Accuracy: {accuracy:F1}%");
            }
            
            text.AppendLine();
            
            // Loyalty-based ending
            if (_loyaltyManager != null)
            {
                int imperialLoyalty = _loyaltyManager.ImperialLoyalty;
                int rebellionSympathy = _loyaltyManager.RebellionSympathy;
                
                if (imperialLoyalty > 5 && rebellionSympathy < 0)
                {
                    text.AppendLine("You remained loyal to the Imperium until the end.");
                }
                else if (rebellionSympathy > 5 && imperialLoyalty < 0)
                {
                    text.AppendLine("Your sympathy for the Insurgency has been noted in your permanent record.");
                }
                else if (imperialLoyalty <= criticalLoyaltyThreshold)
                {
                    text.AppendLine("Your loyalty to the Imperium has been questioned.");
                }
                else
                {
                    text.AppendLine("Your service record shows mixed loyalties.");
                }
            }
            
            return text.ToString();
        }
        
        /// <summary>
        /// Collect final statistics for display
        /// </summary>
        private void CollectFinalStatistics()
        {
            // Try to get data from refactored managers first
            if (_performanceManager != null)
            {
                _finalScore = _performanceManager.CurrentScore;
                _correctDecisions = _performanceManager.CorrectDecisions;
                _wrongDecisions = _performanceManager.WrongDecisions;
                _currentStrikes = _performanceManager.CurrentStrikes;
            }
            
            // If refactored managers don't have data, fall back to GameManager legacy data
            if (_correctDecisions == 0 && _wrongDecisions == 0)
            {
                var gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    _correctDecisions = gameManager.GetCorrectDecisions();
                    _wrongDecisions = gameManager.GetWrongDecisions();
                    _currentStrikes = gameManager.GetCurrentStrikes();
                    
                    if (enableDebugLogs)
                        Debug.Log($"[GameOverManager] Using legacy GameManager data: {_correctDecisions} correct, {_wrongDecisions} wrong, {_currentStrikes} strikes");
                }
            }
            
            if (_creditsManager != null)
            {
                _finalCredits = _creditsManager.CurrentCredits;
            }
            else
            {
                // Fall back to GameManager credits if CreditsManager not available
                var gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    _finalCredits = gameManager.GetCredits();
                }
            }
            
            if (_dayManager != null)
            {
                _daysSurvived = _dayManager.CurrentDay;
                // Use total ships processed across all days for final score
                _totalShipsProcessed = _dayManager.TotalShipsProcessed;
            }
            else
            {
                // Fall back to GameManager data
                var gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    _daysSurvived = gameManager.GetCurrentDay();
                    _totalShipsProcessed = gameManager.GetTotalShipsProcessed(); // Total across all days
                }
            }
        }
        
        /// <summary>
        /// Record game over in history
        /// </summary>
        private void RecordGameOver()
        {
            var record = new GameOverRecord
            {
                Reason = _currentGameOverReason,
                Message = _gameOverMessage,
                Date = _gameOverTime,
                FinalScore = _finalScore,
                DaysSurvived = _daysSurvived,
                TotalShipsProcessed = _totalShipsProcessed,
                CorrectDecisions = _correctDecisions,
                WrongDecisions = _wrongDecisions,
                FinalCredits = _finalCredits,
                CurrentStrikes = _currentStrikes,
                ImperialLoyalty = _loyaltyManager != null ? _loyaltyManager.ImperialLoyalty : 0,
                RebellionSympathy = _loyaltyManager != null ? _loyaltyManager.RebellionSympathy : 0
            };
            
            _gameOverHistory.Add(record);
            OnGameOverProcessed?.Invoke(record);
            
            if (enableGameOverLogging)
                Debug.Log($"[GameOverManager] Game over recorded: {_currentGameOverReason}");
        }
        
        /// <summary>
        /// Request game restart
        /// </summary>
        public void RequestRestart()
        {
            if (enableDebugLogs)
                Debug.Log("[GameOverManager] Restart requested");
            
            // Hide game over UI
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
            
            if (gameOverBackground != null)
                gameOverBackground.SetActive(false);
            
            // Reset game over state
            ResetGameOverState();
            
            // Trigger restart event
            OnRestartRequested?.Invoke();
        }
        
        /// <summary>
        /// Request quit to main menu
        /// </summary>
        public void RequestQuit()
        {
            if (enableDebugLogs)
                Debug.Log("[GameOverManager] Quit requested");
            
            OnQuitRequested?.Invoke();
        }
        
        /// <summary>
        /// Reset game over state
        /// </summary>
        private void ResetGameOverState()
        {
            _isGameOver = false;
            _isProcessingGameOver = false;
            _currentGameOverReason = GameOverReason.None;
            _gameOverMessage = "";
            _gameOverTime = DateTime.MinValue;
            
            // Reset statistics
            _finalScore = 0;
            _daysSurvived = 0;
            _totalShipsProcessed = 0;
            _correctDecisions = 0;
            _wrongDecisions = 0;
            _currentStrikes = 0;
            _finalCredits = 0;
            
            OnGameOverStateChanged?.Invoke(false);
            
            if (enableDebugLogs)
                Debug.Log("[GameOverManager] Game over state reset");
        }
        
        /// <summary>
        /// Get game over statistics
        /// </summary>
        public GameOverStatistics GetStatistics()
        {
            return new GameOverStatistics
            {
                TotalGameOvers = _gameOverHistory.Count,
                GameOversByReason = _gameOverHistory.GroupBy(r => r.Reason).ToDictionary(g => g.Key, g => g.Count()),
                BestScore = _gameOverHistory.Count > 0 ? _gameOverHistory.Max(r => r.FinalScore) : 0,
                BestDaysSurvived = _gameOverHistory.Count > 0 ? _gameOverHistory.Max(r => r.DaysSurvived) : 0,
                AverageScore = _gameOverHistory.Count > 0 ? _gameOverHistory.Average(r => r.FinalScore) : 0,
                AverageDaysSurvived = _gameOverHistory.Count > 0 ? _gameOverHistory.Average(r => r.DaysSurvived) : 0,
                GameOverHistory = new List<GameOverRecord>(_gameOverHistory)
            };
        }
        
        /// <summary>
        /// Reset game over tracking (for new session)
        /// </summary>
        public void ResetGameOverTracking()
        {
            _gameOverHistory.Clear();
            ResetGameOverState();
            
            if (enableDebugLogs)
                Debug.Log("[GameOverManager] Game over tracking reset");
        }
        
        // Event handlers
        private void OnStrikesChanged(int newStrikes)
        {
            _currentStrikes = newStrikes;
            CheckGameOverConditions();
        }
        
        private void OnCreditsChanged(int newCredits, string reason)
        {
            CheckGameOverConditions();
        }
        
        private void OnLoyaltyChanged(int imperialLoyalty, int rebellionSympathy)
        {
            CheckGameOverConditions();
        }
        
        /// <summary>
        /// Handle GameEvents.OnGameEnded event
        /// </summary>
        private void OnGameEndedEvent()
        {
            if (enableDebugLogs)
                Debug.Log("[GameOverManager] Received OnGameEnded event - triggering game over");
            
            // Check what caused the game over
            var decisionTracker = ServiceLocator.Get<DecisionTracker>();
            if (decisionTracker != null && decisionTracker.CurrentStrikes >= maxMistakes)
            {
                TriggerGameOver(GameOverReason.TooManyMistakes, mistakeGameOverMessage);
            }
            else
            {
                // Generic game over if we can't determine the specific reason
                TriggerGameOver(GameOverReason.SystemFailure, "The game has ended due to system failure.");
            }
        }
        
        /// <summary>
        /// Handle DecisionTracker.OnGameOverTriggered event
        /// </summary>
        private void OnDecisionTrackerGameOver()
        {
            if (enableDebugLogs)
                Debug.Log("[GameOverManager] Received OnGameOverTriggered event from DecisionTracker - triggering game over");
            
            TriggerGameOver(GameOverReason.TooManyMistakes, mistakeGameOverMessage);
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_performanceManager != null)
            {
                PerformanceManager.OnStrikesChanged -= (current, max) => OnStrikesChanged(current);
            }
            
            if (_creditsManager != null)
            {
                CreditsManager.OnCreditsChanged -= (newAmount, changeAmount) => OnCreditsChanged(newAmount, "Credits changed");
            }
            
            if (_loyaltyManager != null)
            {
                LoyaltyManager.OnLoyaltyChanged -= OnLoyaltyChanged;
            }
            
            // Unsubscribe from GameEvents
            GameEvents.OnGameEnded -= OnGameEndedEvent;
            
            // Unsubscribe from DecisionTracker events
            DecisionTracker.OnGameOverTriggered -= OnDecisionTrackerGameOver;
            
            // Clear event subscriptions
            OnGameOverTriggered = null;
            OnGameOverProcessed = null;
            OnRestartRequested = null;
            OnQuitRequested = null;
            OnGameOverStateChanged = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Trigger Mistake Game Over")]
        private void TestTriggerMistakeGameOver()
        {
            TriggerGameOver(GameOverReason.TooManyMistakes, mistakeGameOverMessage);
        }
        
        [ContextMenu("Test: Trigger Credits Game Over")]
        private void TestTriggerCreditsGameOver()
        {
            TriggerGameOver(GameOverReason.InsufficientCredits, creditsGameOverMessage);
        }
        
        [ContextMenu("Test: Check Game Over Conditions")]
        private void TestCheckGameOverConditions()
        {
            CheckGameOverConditions();
        }
        
        [ContextMenu("Show Game Over Statistics")]
        private void ShowGameOverStatistics()
        {
            var stats = GetStatistics();
            Debug.Log("=== GAME OVER STATISTICS ===");
            Debug.Log($"Total Game Overs: {stats.TotalGameOvers}");
            Debug.Log($"Best Score: {stats.BestScore}");
            Debug.Log($"Best Days Survived: {stats.BestDaysSurvived}");
            Debug.Log($"Average Score: {stats.AverageScore:F1}");
            Debug.Log($"Average Days Survived: {stats.AverageDaysSurvived:F1}");
            foreach (var kvp in stats.GameOversByReason)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value} times");
            }
            Debug.Log("=== END STATISTICS ===");
        }
    }
    
    // Supporting data structures
    public enum GameOverReason
    {
        None,
        TooManyMistakes,
        InsufficientCredits,
        LoyaltyFailure,
        TimeLimit,
        FamilyIssues,
        SystemFailure,
        ManualTrigger
    }
    
    [System.Serializable]
    public class GameOverRecord
    {
        public GameOverReason Reason;
        public string Message;
        public DateTime Date;
        public int FinalScore;
        public int DaysSurvived;
        public int TotalShipsProcessed;
        public int CorrectDecisions;
        public int WrongDecisions;
        public int FinalCredits;
        public int CurrentStrikes;
        public int ImperialLoyalty;
        public int RebellionSympathy;
    }
    
    [System.Serializable]
    public struct GameOverStatistics
    {
        public int TotalGameOvers;
        public Dictionary<GameOverReason, int> GameOversByReason;
        public int BestScore;
        public int BestDaysSurvived;
        public double AverageScore;
        public double AverageDaysSurvived;
        public List<GameOverRecord> GameOverHistory;
    }
    
    // Extension methods for LINQ support
    public static class GameOverExtensions
    {
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
            this IEnumerable<TSource> source, 
            Func<TSource, TKey> keySelector)
        {
            var groups = new Dictionary<TKey, List<TSource>>();
            
            foreach (var item in source)
            {
                var key = keySelector(item);
                if (!groups.ContainsKey(key))
                {
                    groups[key] = new List<TSource>();
                }
                groups[key].Add(item);
            }
            
            return groups.Select(kvp => new Grouping<TKey, TSource>(kvp.Key, kvp.Value));
        }
        
        public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            
            foreach (var item in source)
            {
                var key = keySelector(item);
                var value = valueSelector(item);
                dictionary[key] = value;
            }
            
            return dictionary;
        }
        
        public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            int max = int.MinValue;
            foreach (var item in source)
            {
                int value = selector(item);
                if (value > max)
                    max = value;
            }
            return max;
        }
        
        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            int sum = 0;
            int count = 0;
            
            foreach (var item in source)
            {
                sum += selector(item);
                count++;
            }
            
            return count > 0 ? (double)sum / count : 0;
        }
        
        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            int count = 0;
            foreach (var item in source)
                count++;
            return count;
        }
    }
    
    // Helper class for grouping
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public TKey Key { get; private set; }
        private List<TElement> _elements;
        
        public Grouping(TKey key, List<TElement> elements)
        {
            Key = key;
            _elements = elements;
        }
        
        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}