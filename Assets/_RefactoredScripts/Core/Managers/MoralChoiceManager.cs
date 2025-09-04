using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages moral choice events and their consequences
    /// Extracted from GameManager for focused moral decision responsibility
    /// </summary>
    public class MoralChoiceManager : MonoBehaviour
    {
        [Header("Moral Choice Settings")]
        [SerializeField] private float dailyChoiceChance = 0.3f; // 30% chance per day
        [SerializeField] private int maxChoicesPerDay = 2;
        [SerializeField] private float minTimeBetweenChoices = 120f; // 2 minutes
        [SerializeField] private float choiceTimeoutDuration = 60f; // 1 minute to decide
        [SerializeField] private bool enableRandomTiming = true;
        
        [Header("Choice Consequences")]
        [SerializeField] private int followRulesLoyaltyBonus = 2;
        [SerializeField] private int followRulesRebelPenalty = -1;
        [SerializeField] private int bendRulesLoyaltyPenalty = -1;
        [SerializeField] private int bendRulesRebelBonus = 2;
        [SerializeField] private int bribeAmount = 20;
        [SerializeField] private int luxuryBribeAmount = 30;
        [SerializeField] private bool enableLoyaltyFeedback = true;
        
        [Header("UI References")]
        [SerializeField] private GameObject moralChoicePanel;
        [SerializeField] private TMP_Text choiceText;
        [SerializeField] private Button option1Button;
        [SerializeField] private Button option2Button;
        [SerializeField] private TMP_Text option1Text;
        [SerializeField] private TMP_Text option2Text;
        [SerializeField] private GameObject timeoutWarning;
        [SerializeField] private TMP_Text timeoutText;
        
        [Header("Audio")]
        [SerializeField] private bool enableChoiceSounds = true;
        [SerializeField] private string choicePresentedSound = "moral_choice_presented";
        [SerializeField] private string choiceConfirmedSound = "moral_choice_confirmed";
        [SerializeField] private string timeoutWarningSound = "timeout_warning";
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableChoiceLogging = true;
        [SerializeField] private bool enableTestChoices = false;
        
        // Choice state
        private bool _isChoiceActive = false;
        private bool _isChoiceTimedOut = false;
        private MoralChoiceData _currentChoice;
        private Coroutine _choiceTimeoutCoroutine;
        private int _choicesGeneratedToday = 0;
        private float _lastChoiceTime = 0f;
        
        // Choice scenarios
        private List<MoralChoiceScenario> _scenarios = new List<MoralChoiceScenario>();
        private List<MoralChoiceData> _choiceHistory = new List<MoralChoiceData>();
        private int _totalChoicesPresented = 0;
        private int _totalChoicesCompleted = 0;
        private int _totalChoicesTimedOut = 0;
        
        // Dependencies
        private GameStateManager _gameStateManager;
        private DayProgressionManager _dayManager;
        private LoyaltyManager _loyaltyManager;
        private CreditsManager _creditsManager;
        private NotificationManager _notificationManager;
        private AudioManager _audioManager;
        
        // Events
        public static event Action<MoralChoiceData> OnChoicePresented;
        public static event Action<MoralChoiceData, int> OnChoiceSelected;
        public static event Action<MoralChoiceData> OnChoiceTimedOut;
        public static event Action<MoralChoiceData> OnChoiceCompleted;
        public static event Action<bool> OnGamePausedForChoice;
        
        // Public properties
        public bool IsChoiceActive => _isChoiceActive;
        public MoralChoiceData CurrentChoice => _currentChoice;
        public int ChoicesGeneratedToday => _choicesGeneratedToday;
        public int TotalChoicesPresented => _totalChoicesPresented;
        public int TotalChoicesCompleted => _totalChoicesCompleted;
        public List<MoralChoiceData> ChoiceHistory => new List<MoralChoiceData>(_choiceHistory);
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<MoralChoiceManager>(this);
            
            // Initialize scenarios
            InitializeScenarios();
            
            if (enableDebugLogs)
                Debug.Log("[MoralChoiceManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager dependencies
            _gameStateManager = ServiceLocator.Get<GameStateManager>();
            _dayManager = ServiceLocator.Get<DayProgressionManager>();
            _loyaltyManager = ServiceLocator.Get<LoyaltyManager>();
            _creditsManager = ServiceLocator.Get<CreditsManager>();
            _notificationManager = ServiceLocator.Get<NotificationManager>();
            _audioManager = ServiceLocator.Get<AudioManager>();
            
            // Subscribe to events
            if (_dayManager != null)
            {
                DayProgressionManager.OnDayStarted += OnDayStarted;
                DayProgressionManager.OnShiftStarted += OnShiftStarted;
                DayProgressionManager.OnShiftEnded += OnShiftEnded;
            }
            
            if (_gameStateManager != null)
            {
                GameEvents.OnGameStateChanged += OnGameStateChanged;
            }
            
            // Validate UI
            ValidateUIReferences();
            
            // Setup button listeners
            SetupButtonListeners();
            
            if (enableDebugLogs)
                Debug.Log("[MoralChoiceManager] Moral choice system ready");
        }
        
        /// <summary>
        /// Initialize moral choice scenarios
        /// </summary>
        private void InitializeScenarios()
        {
            _scenarios.Add(new MoralChoiceScenario
            {
                Id = "bribery_medical",
                Title = "Medical Supply Bribery",
                Description = "A ship captain discreetly offers you 20 credits to overlook an expired access code. He claims it's for medical supplies urgently needed on the base.",
                Option1Text = "Follow Protocol",
                Option1Description = "Deny the request and follow standard procedures",
                Option2Text = "Accept Bribe",
                Option2Description = "Accept the credits and approve the ship",
                HasBribe = true,
                Category = ChoiceCategory.Corruption,
                Difficulty = ChoiceDifficulty.Medium
            });
            
            _scenarios.Add(new MoralChoiceScenario
            {
                Id = "personal_connection",
                Title = "Childhood Friend",
                Description = "You recognise a passenger on board as a childhood friend. Their papers seem in order but the system marks them as a potential rebel sympathiser.",
                Option1Text = "Follow System",
                Option1Description = "Trust the system and deny clearance",
                Option2Text = "Trust Friend",
                Option2Description = "Override the system and approve clearance",
                HasBribe = false,
                Category = ChoiceCategory.Personal,
                Difficulty = ChoiceDifficulty.High
            });
            
            _scenarios.Add(new MoralChoiceScenario
            {
                Id = "authority_pressure",
                Title = "Imperial Officer",
                Description = "An Imperium officer requests you expedite their clearance despite questionable manifest details. Denying them could affect your standing with superiors.",
                Option1Text = "Follow Procedure",
                Option1Description = "Insist on proper documentation despite rank",
                Option2Text = "Expedite Clearance",
                Option2Description = "Approve the officer's request immediately",
                HasBribe = false,
                Category = ChoiceCategory.Authority,
                Difficulty = ChoiceDifficulty.High
            });
            
            _scenarios.Add(new MoralChoiceScenario
            {
                Id = "family_emergency",
                Title = "Family Emergency",
                Description = "A civilian pleads that their family member needs immediate medical attention. Their transit papers are technically expired, but only by a few hours.",
                Option1Text = "Deny Transit",
                Option1Description = "Expired papers mean denied access, no exceptions",
                Option2Text = "Allow Transit",
                Option2Description = "Show compassion and allow emergency transit",
                HasBribe = false,
                Category = ChoiceCategory.Compassion,
                Difficulty = ChoiceDifficulty.Low
            });
            
            _scenarios.Add(new MoralChoiceScenario
            {
                Id = "information_trade",
                Title = "Information Broker",
                Description = "A smuggler offers valuable intelligence about rebel movements in exchange for overlooking their cargo manifest discrepancies.",
                Option1Text = "Refuse Deal",
                Option1Description = "Maintain integrity and process normally",
                Option2Text = "Accept Intel",
                Option2Description = "Take the information and clear the ship",
                HasBribe = false,
                Category = ChoiceCategory.Intelligence,
                Difficulty = ChoiceDifficulty.Medium
            });
            
            _scenarios.Add(new MoralChoiceScenario
            {
                Id = "family_with_children",
                Title = "Family Transfer",
                Description = "A nervous family claims to be transferring to Starkiller Base, but their clearance codes are outdated. They have small children with them who look hungry and tired.",
                Option1Text = "Allow Transit",
                Option1Description = "Show compassion to a family in need",
                Option2Text = "Deny Entry",
                Option2Description = "Maintain security despite humanitarian concerns",
                HasBribe = false,
                Category = ChoiceCategory.Compassion,
                Difficulty = ChoiceDifficulty.Medium
            });
            
            _scenarios.Add(new MoralChoiceScenario
            {
                Id = "officer_contraband",
                Title = "Contraband Luxury Goods",
                Description = "A First Order officer is trying to bring in contraband luxury goods. He offers you 30 credits to 'forget' about the extra items on his manifest.",
                Option1Text = "Accept Bribe",
                Option1Description = "Accept the credits and ignore the contraband",
                Option2Text = "Report Contraband",
                Option2Description = "Report the officer for attempting to smuggle contraband",
                HasBribe = true,
                Category = ChoiceCategory.Corruption,
                Difficulty = ChoiceDifficulty.Low
            });
        }
        
        /// <summary>
        /// Schedule a moral choice event
        /// </summary>
        public void ScheduleChoiceEvent(float delay = 0f)
        {
            if (_choicesGeneratedToday >= maxChoicesPerDay)
            {
                if (enableDebugLogs)
                    Debug.Log("[MoralChoiceManager] Maximum choices for today reached");
                return;
            }
            
            if (Time.time - _lastChoiceTime < minTimeBetweenChoices)
            {
                if (enableDebugLogs)
                    Debug.Log("[MoralChoiceManager] Too soon for next choice");
                return;
            }
            
            StartCoroutine(TriggerChoiceEventCoroutine(delay));
        }
        
        /// <summary>
        /// Try to trigger a moral choice event (compatibility method)
        /// </summary>
        public void TryTriggerMoralChoice(float maxDelayTime)
        {
            if (UnityEngine.Random.value < dailyChoiceChance)
            {
                float randomTime = UnityEngine.Random.Range(60f, maxDelayTime);
                ScheduleChoiceEvent(randomTime);
            }
        }
        
        /// <summary>
        /// Trigger choice event coroutine
        /// </summary>
        private IEnumerator TriggerChoiceEventCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (_gameStateManager != null && _gameStateManager.CurrentState == GameState.Gameplay)
            {
                PresentMoralChoice();
            }
        }
        
        /// <summary>
        /// Present a moral choice to the player
        /// </summary>
        public void PresentMoralChoice(string specificScenarioId = null)
        {
            if (_isChoiceActive)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[MoralChoiceManager] Choice already active");
                return;
            }
            
            // Select scenario
            MoralChoiceScenario scenario = SelectScenario(specificScenarioId);
            if (scenario == null)
            {
                Debug.LogError("[MoralChoiceManager] No scenario available");
                return;
            }
            
            // Create choice data
            _currentChoice = new MoralChoiceData
            {
                Id = System.Guid.NewGuid().ToString(),
                Scenario = scenario,
                PresentedTime = DateTime.Now,
                TimeRemaining = choiceTimeoutDuration
            };
            
            // Update state
            _isChoiceActive = true;
            _isChoiceTimedOut = false;
            _choicesGeneratedToday++;
            _totalChoicesPresented++;
            _lastChoiceTime = Time.time;
            
            // Setup UI
            SetupChoiceUI();
            
            // Show panel
            if (moralChoicePanel != null)
            {
                moralChoicePanel.SetActive(true);
            }
            
            // Pause game
            if (_gameStateManager != null)
            {
                _gameStateManager.PauseGame();
            }
            
            // Play sound
            if (_audioManager != null && enableChoiceSounds)
            {
                _audioManager.PlaySound(choicePresentedSound);
            }
            
            // Start timeout timer
            _choiceTimeoutCoroutine = StartCoroutine(ChoiceTimeoutCoroutine());
            
            // Trigger events
            OnChoicePresented?.Invoke(_currentChoice);
            OnGamePausedForChoice?.Invoke(true);
            
            if (enableChoiceLogging)
            {
                Debug.Log($"[MoralChoiceManager] Moral choice presented: {scenario.Title}");
            }
        }
        
        /// <summary>
        /// Select a scenario for presentation
        /// </summary>
        private MoralChoiceScenario SelectScenario(string specificId = null)
        {
            if (!string.IsNullOrEmpty(specificId))
            {
                return _scenarios.FirstOrDefault(s => s.Id == specificId);
            }
            
            // Filter scenarios based on history to avoid repetition
            var availableScenarios = _scenarios.Where(s => 
                !_choiceHistory.Any(h => h.Scenario.Id == s.Id && 
                (DateTime.Now - h.PresentedTime).TotalDays < 1)).ToList();
            
            if (availableScenarios.Count == 0)
            {
                availableScenarios = _scenarios; // Use all if none available
            }
            
            // Adaptive scenario selection based on player loyalty
            MoralChoiceScenario selectedScenario = null;
            
            if (_loyaltyManager != null)
            {
                int imperialLoyalty = _loyaltyManager.ImperialLoyalty;
                int rebellionSympathy = _loyaltyManager.RebellionSympathy;
                
                // 70% chance of adaptive selection based on loyalty
                if (UnityEngine.Random.value < 0.7f)
                {
                    if (imperialLoyalty > 3)
                    {
                        // Loyal player - test their loyalty with compassion/humanitarian scenarios
                        selectedScenario = availableScenarios.FirstOrDefault(s => 
                            s.Category == ChoiceCategory.Compassion || 
                            s.Category == ChoiceCategory.Personal);
                    }
                    else if (rebellionSympathy > 3)
                    {
                        // Sympathetic player - test them with authority/corruption scenarios
                        selectedScenario = availableScenarios.FirstOrDefault(s => 
                            s.Category == ChoiceCategory.Authority || 
                            s.Category == ChoiceCategory.Corruption);
                    }
                    else if (imperialLoyalty < -3)
                    {
                        // Anti-imperial player - present imperial authority scenarios
                        selectedScenario = availableScenarios.FirstOrDefault(s => 
                            s.Category == ChoiceCategory.Authority);
                    }
                    else if (rebellionSympathy < -3)
                    {
                        // Anti-rebellion player - present compassion scenarios
                        selectedScenario = availableScenarios.FirstOrDefault(s => 
                            s.Category == ChoiceCategory.Compassion);
                    }
                }
            }
            
            // If no adaptive scenario found, pick random
            if (selectedScenario == null)
            {
                selectedScenario = availableScenarios[UnityEngine.Random.Range(0, availableScenarios.Count)];
            }
            
            return selectedScenario;
        }
        
        /// <summary>
        /// Setup choice UI elements
        /// </summary>
        private void SetupChoiceUI()
        {
            if (_currentChoice == null) return;
            
            var scenario = _currentChoice.Scenario;
            
            // Set main text
            if (choiceText != null)
            {
                choiceText.text = scenario.Description;
            }
            
            // Set option texts
            if (option1Text != null)
            {
                option1Text.text = scenario.Option1Text;
            }
            
            if (option2Text != null)
            {
                option2Text.text = scenario.Option2Text;
            }
            
            // Update timeout display
            if (timeoutText != null)
            {
                timeoutText.text = $"Time: {choiceTimeoutDuration:F0}s";
            }
        }
        
        /// <summary>
        /// Setup button listeners
        /// </summary>
        private void SetupButtonListeners()
        {
            if (option1Button != null)
            {
                option1Button.onClick.AddListener(() => SelectChoice(1));
            }
            
            if (option2Button != null)
            {
                option2Button.onClick.AddListener(() => SelectChoice(2));
            }
        }
        
        /// <summary>
        /// Player selects a choice option
        /// </summary>
        public void SelectChoice(int option)
        {
            if (!_isChoiceActive || _currentChoice == null || _isChoiceTimedOut)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[MoralChoiceManager] Invalid choice selection");
                return;
            }
            
            // Stop timeout coroutine
            if (_choiceTimeoutCoroutine != null)
            {
                StopCoroutine(_choiceTimeoutCoroutine);
                _choiceTimeoutCoroutine = null;
            }
            
            // Record choice
            _currentChoice.SelectedOption = option;
            _currentChoice.CompletedTime = DateTime.Now;
            _currentChoice.WasCompleted = true;
            
            // Process consequences
            ProcessChoiceConsequences(option);
            
            // Play sound
            if (_audioManager != null && enableChoiceSounds)
            {
                _audioManager.PlaySound(choiceConfirmedSound);
            }
            
            // Trigger events
            OnChoiceSelected?.Invoke(_currentChoice, option);
            
            // Complete choice
            CompleteChoice();
            
            if (enableChoiceLogging)
            {
                Debug.Log($"[MoralChoiceManager] Choice selected: Option {option} - {_currentChoice.Scenario.Title}");
            }
        }
        
        /// <summary>
        /// Process the consequences of a choice
        /// </summary>
        private void ProcessChoiceConsequences(int option)
        {
            if (_currentChoice == null) return;
            
            var scenario = _currentChoice.Scenario;
            int loyaltyChange = 0;
            int rebelChange = 0;
            int creditChange = 0;
            string consequence = "";
            
            switch (option)
            {
                case 1: // Follow rules option
                    loyaltyChange = followRulesLoyaltyBonus;
                    rebelChange = followRulesRebelPenalty;
                    consequence = "Followed imperium protocols";
                    break;
                    
                case 2: // Bend rules option
                    loyaltyChange = bendRulesLoyaltyPenalty;
                    rebelChange = bendRulesRebelBonus;
                    consequence = "Bent the rules for personal reasons";
                    
                    // Check for bribe
                    if (scenario.HasBribe)
                    {
                        creditChange = scenario.Id == "officer_contraband" ? luxuryBribeAmount : bribeAmount;
                        consequence += $" (accepted bribe: {creditChange} credits)";
                    }
                    break;
            }
            
            // Apply consequences
            if (_loyaltyManager != null)
            {
                _loyaltyManager.UpdateLoyalty(loyaltyChange, rebelChange, consequence);
            }
            
            if (_creditsManager != null && creditChange != 0)
            {
                _creditsManager.AddCredits(creditChange, $"Moral choice: {scenario.Title}");
            }
            
            // Record consequence
            _currentChoice.LoyaltyChange = loyaltyChange;
            _currentChoice.RebelChange = rebelChange;
            _currentChoice.CreditChange = creditChange;
            _currentChoice.Consequence = consequence;
            
            // Show feedback
            ShowChoiceFeedback(option, loyaltyChange, rebelChange, creditChange);
        }
        
        /// <summary>
        /// Show feedback for choice consequences
        /// </summary>
        private void ShowChoiceFeedback(int option, int loyaltyChange, int rebelChange, int creditChange)
        {
            if (!enableLoyaltyFeedback || _notificationManager == null) return;
            
            string message = "";
            NotificationType type = NotificationType.Info;
            
            // Loyalty feedback
            if (loyaltyChange >= 2)
            {
                message = "Your superiors are pleased with your decision.";
                type = NotificationType.Success;
            }
            else if (loyaltyChange <= -2)
            {
                message = "The Imperium is displeased with your actions.";
                type = NotificationType.Warning;
            }
            
            if (rebelChange >= 2)
            {
                message += " The resistance will remember this.";
                type = NotificationType.Info;
            }
            
            // Credit feedback
            if (creditChange > 0)
            {
                message += $" (+{creditChange} credits)";
                type = NotificationType.Success;
            }
            
            // Show notification
            if (!string.IsNullOrEmpty(message))
            {
                _notificationManager.ShowNotification(message, type);
            }
        }
        
        /// <summary>
        /// Handle choice timeout
        /// </summary>
        private IEnumerator ChoiceTimeoutCoroutine()
        {
            float elapsed = 0f;
            
            while (elapsed < choiceTimeoutDuration)
            {
                elapsed += Time.unscaledDeltaTime; // Use unscaled time since game is paused
                _currentChoice.TimeRemaining = choiceTimeoutDuration - elapsed;
                
                // Update timeout display
                if (timeoutText != null)
                {
                    timeoutText.text = $"Time: {_currentChoice.TimeRemaining:F0}s";
                }
                
                // Show warning at 10 seconds
                if (_currentChoice.TimeRemaining <= 10f && timeoutWarning != null && !timeoutWarning.activeInHierarchy)
                {
                    timeoutWarning.SetActive(true);
                    
                    if (_audioManager != null && enableChoiceSounds)
                    {
                        _audioManager.PlaySound(timeoutWarningSound);
                    }
                }
                
                yield return null;
            }
            
            // Timeout reached
            TimeoutChoice();
        }
        
        /// <summary>
        /// Handle choice timeout
        /// </summary>
        private void TimeoutChoice()
        {
            if (!_isChoiceActive || _currentChoice == null) return;
            
            _isChoiceTimedOut = true;
            _totalChoicesTimedOut++;
            
            // Record timeout
            _currentChoice.WasTimedOut = true;
            _currentChoice.CompletedTime = DateTime.Now;
            
            // Default to option 1 (follow rules) on timeout
            _currentChoice.SelectedOption = 1;
            _currentChoice.Consequence = "Timed out - default action taken";
            
            // Show timeout notification
            if (_notificationManager != null)
            {
                _notificationManager.ShowNotification("⏰ Choice timed out - default action taken", NotificationType.Warning);
            }
            
            // Trigger events
            OnChoiceTimedOut?.Invoke(_currentChoice);
            
            // Complete choice
            CompleteChoice();
            
            if (enableChoiceLogging)
            {
                Debug.Log($"[MoralChoiceManager] Choice timed out: {_currentChoice.Scenario.Title}");
            }
        }
        
        /// <summary>
        /// Complete the current choice
        /// </summary>
        private void CompleteChoice()
        {
            if (_currentChoice == null) return;
            
            // Add to history
            _choiceHistory.Add(_currentChoice);
            _totalChoicesCompleted++;
            
            // Hide UI
            if (moralChoicePanel != null)
            {
                moralChoicePanel.SetActive(false);
            }
            
            if (timeoutWarning != null)
            {
                timeoutWarning.SetActive(false);
            }
            
            // Resume game
            if (_gameStateManager != null)
            {
                _gameStateManager.ResumeGame();
            }
            
            // Trigger events
            OnChoiceCompleted?.Invoke(_currentChoice);
            OnGamePausedForChoice?.Invoke(false);
            
            // Reset state
            _isChoiceActive = false;
            _isChoiceTimedOut = false;
            var completedChoice = _currentChoice;
            _currentChoice = null;
            
            if (enableChoiceLogging)
            {
                Debug.Log($"[MoralChoiceManager] Choice completed: {completedChoice.Scenario.Title}");
            }
        }
        
        /// <summary>
        /// Get choice statistics
        /// </summary>
        public MoralChoiceStatistics GetStatistics()
        {
            var stats = new MoralChoiceStatistics
            {
                TotalChoicesPresented = _totalChoicesPresented,
                TotalChoicesCompleted = _totalChoicesCompleted,
                TotalChoicesTimedOut = _totalChoicesTimedOut,
                ChoicesGeneratedToday = _choicesGeneratedToday,
                CompletionRate = _totalChoicesPresented > 0 ? (float)_totalChoicesCompleted / _totalChoicesPresented : 0f,
                TimeoutRate = _totalChoicesPresented > 0 ? (float)_totalChoicesTimedOut / _totalChoicesPresented : 0f
            };
            
            // Calculate category statistics
            var categoryStats = new Dictionary<ChoiceCategory, int>();
            foreach (var choice in _choiceHistory)
            {
                if (!categoryStats.ContainsKey(choice.Scenario.Category))
                    categoryStats[choice.Scenario.Category] = 0;
                categoryStats[choice.Scenario.Category]++;
            }
            stats.CategoryStatistics = categoryStats;
            
            return stats;
        }
        
        /// <summary>
        /// Validate UI references
        /// </summary>
        private void ValidateUIReferences()
        {
            if (moralChoicePanel == null)
            {
                Debug.LogError("[MoralChoiceManager] moralChoicePanel is not assigned!");
            }
            
            if (choiceText == null)
            {
                Debug.LogWarning("[MoralChoiceManager] choiceText is not assigned");
            }
            
            if (option1Button == null || option2Button == null)
            {
                Debug.LogWarning("[MoralChoiceManager] Choice buttons are not assigned");
            }
        }
        
        // Event handlers
        private void OnDayStarted(int day)
        {
            _choicesGeneratedToday = 0;
            
            // Schedule random choice if enabled
            if (enableRandomTiming && UnityEngine.Random.value < dailyChoiceChance)
            {
                if (_dayManager != null)
                {
                    float randomTime = UnityEngine.Random.Range(60f, _dayManager.RemainingTime - 60f);
                    ScheduleChoiceEvent(randomTime);
                }
            }
            
            if (enableDebugLogs)
                Debug.Log($"[MoralChoiceManager] Day {day} started - choice counter reset");
        }
        
        private void OnShiftStarted()
        {
            if (enableDebugLogs)
                Debug.Log("[MoralChoiceManager] Shift started - moral choices enabled");
        }
        
        private void OnShiftEnded()
        {
            // Force complete any active choice
            if (_isChoiceActive)
            {
                TimeoutChoice();
            }
            
            if (enableDebugLogs)
                Debug.Log("[MoralChoiceManager] Shift ended - moral choices disabled");
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            // Handle game state changes
            if (newState != GameState.Gameplay && _isChoiceActive)
            {
                // Force complete choice if game state changes
                TimeoutChoice();
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_dayManager != null)
            {
                DayProgressionManager.OnDayStarted -= OnDayStarted;
                DayProgressionManager.OnShiftStarted -= OnShiftStarted;
                DayProgressionManager.OnShiftEnded -= OnShiftEnded;
            }
            
            GameEvents.OnGameStateChanged -= OnGameStateChanged;
            
            // Clear event subscriptions
            OnChoicePresented = null;
            OnChoiceSelected = null;
            OnChoiceTimedOut = null;
            OnChoiceCompleted = null;
            OnGamePausedForChoice = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Present Random Choice")]
        private void TestPresentRandomChoice()
        {
            if (enableTestChoices)
                PresentMoralChoice();
        }
        
        [ContextMenu("Test: Present Bribery Choice")]
        private void TestPresentBriberyChoice()
        {
            if (enableTestChoices)
                PresentMoralChoice("bribery_medical");
        }
        
        [ContextMenu("Test: Present Personal Choice")]
        private void TestPresentPersonalChoice()
        {
            if (enableTestChoices)
                PresentMoralChoice("personal_connection");
        }
        
        [ContextMenu("Show Choice Statistics")]
        private void ShowChoiceStatistics()
        {
            var stats = GetStatistics();
            Debug.Log("=== MORAL CHOICE STATISTICS ===");
            Debug.Log($"Total Presented: {stats.TotalChoicesPresented}");
            Debug.Log($"Total Completed: {stats.TotalChoicesCompleted}");
            Debug.Log($"Total Timed Out: {stats.TotalChoicesTimedOut}");
            Debug.Log($"Today: {stats.ChoicesGeneratedToday}");
            Debug.Log($"Completion Rate: {stats.CompletionRate:P1}");
            Debug.Log($"Timeout Rate: {stats.TimeoutRate:P1}");
            
            foreach (var kvp in stats.CategoryStatistics)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value} choices");
            }
            Debug.Log("=== END STATISTICS ===");
        }
    }
    
    // Supporting data structures
    [System.Serializable]
    public class MoralChoiceScenario
    {
        public string Id;
        public string Title;
        public string Description;
        public string Option1Text;
        public string Option1Description;
        public string Option2Text;
        public string Option2Description;
        public bool HasBribe;
        public ChoiceCategory Category;
        public ChoiceDifficulty Difficulty;
    }
    
    [System.Serializable]
    public class MoralChoiceData
    {
        public string Id;
        public MoralChoiceScenario Scenario;
        public DateTime PresentedTime;
        public DateTime CompletedTime;
        public int SelectedOption;
        public bool WasCompleted;
        public bool WasTimedOut;
        public float TimeRemaining;
        public int LoyaltyChange;
        public int RebelChange;
        public int CreditChange;
        public string Consequence;
    }
    
    [System.Serializable]
    public struct MoralChoiceStatistics
    {
        public int TotalChoicesPresented;
        public int TotalChoicesCompleted;
        public int TotalChoicesTimedOut;
        public int ChoicesGeneratedToday;
        public float CompletionRate;
        public float TimeoutRate;
        public Dictionary<ChoiceCategory, int> CategoryStatistics;
    }
    
    public enum ChoiceCategory
    {
        Corruption,
        Personal,
        Authority,
        Compassion,
        Intelligence,
        Duty,
        Survival
    }
    
    public enum ChoiceDifficulty
    {
        Low,
        Medium,
        High
    }
}