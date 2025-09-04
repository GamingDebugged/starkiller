using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using StarkillerBaseCommand;
using Starkiller.Core.ScriptableObjects;
using Starkiller.Core.Managers;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages family member states, relationships, and crisis events
    /// Tracks health, happiness, safety, and generates appropriate responses
    /// </summary>
    public class FamilyPressureManager : MonoBehaviour
    {
        [Header("Family Members")]
        [SerializeField] private List<FamilyMember> familyMembers = new List<FamilyMember>();
        
        [Header("Relationship Settings")]
        [SerializeField] private int baseRelationshipDecay = 2; // Daily decay if no interaction
        [SerializeField] private int ignorePenalty = 10; // Penalty for ignoring messages
        [SerializeField] private int crisisDaysThreshold = 3; // Days before relationship crisis
        
        [Header("Death Mechanics")]
        [SerializeField] private bool enableRandomDeath = true;
        [SerializeField] private AnimationCurve deathProbabilityCurve; // Maps health/safety to death chance
        
        [Header("Popup Settings")]
        [SerializeField] private float basePopupFrequency = 2.5f; // Days between popups per family member
        [SerializeField] private int maxPopupsPerDay = 3;
        
        [Header("Crisis Management")]
        [SerializeField] private List<RelationshipCrisis> activeCrises = new List<RelationshipCrisis>();
        [SerializeField] private List<FamilyPopupData> queuedPopups = new List<FamilyPopupData>();
        
        [Header("UI References")]
        [SerializeField] private GameObject familyPopupPrefab;
        [SerializeField] private Transform popupContainer;
        
        // Tracking dictionaries
        private Dictionary<string, IgnoredMessageTracker> ignoredMessages = new Dictionary<string, IgnoredMessageTracker>();
        private Dictionary<FamilyRole, float> lastPopupTime = new Dictionary<FamilyRole, float>();
        private Dictionary<FamilyRole, List<string>> messageHistory = new Dictionary<FamilyRole, List<string>>();
        
        // Dependencies
        private PersonalDataLogManager dataLogManager;
        private ConsequenceManager consequenceManager;
        private GameManager gameManager;
        private CreditsManager creditsManager;
        private NarrativeStateManager narrativeState;
        
        // Events
        public static event Action<FamilyMember> OnFamilyMemberDeath;
        public static event Action<FamilyMember, int> OnRelationshipChange;
        public static event Action<RelationshipCrisis> OnRelationshipCrisis;
        public static event Action<FamilyPopupData> OnFamilyPopup;
        public static event Action<int> OnStrikeRemoved;
        
        private static FamilyPressureManager instance;
        public static FamilyPressureManager Instance => instance;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            
            ServiceLocator.Register<FamilyPressureManager>(this);
            InitializeFamilyMembers();
            InitializeDeathCurve();
        }
        
        private void Start()
        {
            // Get dependencies
            dataLogManager = ServiceLocator.Get<PersonalDataLogManager>();
            consequenceManager = ConsequenceManager.Instance ?? ServiceLocator.Get<ConsequenceManager>();
            gameManager = FindFirstObjectByType<GameManager>();
            creditsManager = ServiceLocator.Get<CreditsManager>();
            narrativeState = ServiceLocator.Get<NarrativeStateManager>();
            
            // Subscribe to events
            if (dataLogManager != null)
            {
                PersonalDataLogManager.OnDataLogClosed += OnDataLogClosed;
            }
            
            Debug.Log("[FamilyPressureManager] Initialized with " + familyMembers.Count + " family members");
        }
        
        /// <summary>
        /// Initialize family members with starting values
        /// </summary>
        private void InitializeFamilyMembers()
        {
            if (familyMembers.Count == 0)
            {
                // Create default family
                familyMembers = new List<FamilyMember>
                {
                    new FamilyMember
                    {
                        name = "Alex",
                        role = FamilyRole.Partner,
                        relationship = 75,
                        happiness = 70,
                        safety = 90,
                        health = 85,
                        isAlive = true,
                        currentPhase = StoryArcPhase.Setup,
                        personalityTraits = new[] { "Intelligent", "Curious", "Protective" }
                    },
                    new FamilyMember
                    {
                        name = "Marcus",
                        role = FamilyRole.Son,
                        relationship = 65,
                        happiness = 60,
                        safety = 75,
                        health = 90,
                        isAlive = true,
                        currentPhase = StoryArcPhase.Setup,
                        personalityTraits = new[] { "Rebellious", "Ambitious", "Impulsive" }
                    },
                    new FamilyMember
                    {
                        name = "Sarah",
                        role = FamilyRole.Daughter,
                        relationship = 80,
                        happiness = 75,
                        safety = 85,
                        health = 88,
                        isAlive = true,
                        currentPhase = StoryArcPhase.Setup,
                        personalityTraits = new[] { "Compassionate", "Resourceful", "Idealistic" }
                    },
                    new FamilyMember
                    {
                        name = "Hope",
                        role = FamilyRole.Baby,
                        relationship = 100,
                        happiness = 50,
                        safety = 95,
                        health = 60,
                        isAlive = true,
                        currentPhase = StoryArcPhase.Setup,
                        personalityTraits = new[] { "Innocent", "Vulnerable", "Loved" }
                    },
                    new FamilyMember
                    {
                        name = "D-3X",
                        role = FamilyRole.Droid,
                        relationship = 50,
                        happiness = 50,
                        safety = 100,
                        health = 100,
                        isAlive = true,
                        currentPhase = StoryArcPhase.Setup,
                        personalityTraits = new[] { "Mysterious", "Loyal?", "Analytical" }
                    }
                };
            }
            
            // Initialize tracking dictionaries
            foreach (var member in familyMembers)
            {
                lastPopupTime[member.role] = 0;
                messageHistory[member.role] = new List<string>();
            }
        }
        
        /// <summary>
        /// Initialize death probability curve
        /// </summary>
        private void InitializeDeathCurve()
        {
            if (deathProbabilityCurve == null || deathProbabilityCurve.keys.Length == 0)
            {
                deathProbabilityCurve = new AnimationCurve();
                deathProbabilityCurve.AddKey(100, 0f);    // 100 health/safety = 0% death chance
                deathProbabilityCurve.AddKey(80, 0f);     // 80 = 0%
                deathProbabilityCurve.AddKey(60, 0.01f);  // 60 = 1%
                deathProbabilityCurve.AddKey(40, 0.03f);  // 40 = 3%
                deathProbabilityCurve.AddKey(20, 0.05f);  // 20 = 5%
                deathProbabilityCurve.AddKey(0, 0.1f);    // 0 = 10%
            }
        }
        
        /// <summary>
        /// Daily update - called at start of each day
        /// </summary>
        public void DailyUpdate(int currentDay)
        {
            Debug.Log($"[FamilyPressureManager] Daily update for day {currentDay}");
            
            // Update each family member
            foreach (var member in familyMembers)
            {
                if (!member.isAlive) continue;
                
                // Natural relationship decay
                ApplyRelationshipDecay(member);
                
                // Check for random death
                if (enableRandomDeath)
                {
                    CheckRandomDeath(member, currentDay);
                }
                
                // Progress story arc
                UpdateStoryPhase(member, currentDay);
                
                // Check for crisis triggers
                CheckForCrisis(member, currentDay);
            }
            
            // Process ignored messages
            ProcessIgnoredMessages(currentDay);
            
            // Generate today's popups
            GenerateDailyPopups(currentDay);
            
            // Clean up resolved crises
            CleanupResolvedCrises();
        }
        
        /// <summary>
        /// Apply daily relationship decay
        /// </summary>
        private void ApplyRelationshipDecay(FamilyMember member)
        {
            if (member.role == FamilyRole.Baby || member.role == FamilyRole.Droid)
                return; // Baby and droid don't decay normally
            
            member.relationship = Mathf.Max(0, member.relationship - baseRelationshipDecay);
            
            if (member.relationship < 30 && member.relationship > 0)
            {
                Debug.LogWarning($"[FamilyPressureManager] {member.name}'s relationship critically low: {member.relationship}");
            }
        }
        
        /// <summary>
        /// Check if family member should randomly die
        /// </summary>
        private void CheckRandomDeath(FamilyMember member, int currentDay)
        {
            if (!member.isAlive) return;
            
            // Calculate death chance based on lowest of health/safety
            float lowestStat = Mathf.Min(member.health, member.safety);
            float deathChance = deathProbabilityCurve.Evaluate(lowestStat);
            
            if (deathChance > 0 && UnityEngine.Random.value < deathChance)
            {
                // Warning before death - give player one last chance
                if (!member.hasDeathWarning)
                {
                    member.hasDeathWarning = true;
                    CreateEmergencyPopup(member, currentDay);
                    Debug.LogWarning($"[FamilyPressureManager] {member.name} in critical danger! Death warning issued.");
                }
                else
                {
                    // They had their warning, now they die
                    ExecuteFamilyDeath(member, DetermineDeathType(member), currentDay);
                }
            }
        }
        
        /// <summary>
        /// Determine how the family member dies based on their story
        /// </summary>
        private DeathType DetermineDeathType(FamilyMember member)
        {
            switch (member.role)
            {
                case FamilyRole.Partner:
                    return member.activeTokens.Contains("RebelContact") ? DeathType.Disappeared : DeathType.Accident;
                case FamilyRole.Son:
                    return member.safety < member.health ? DeathType.Combat : DeathType.Accident;
                case FamilyRole.Daughter:
                    return member.activeTokens.Contains("HidingRefugees") ? DeathType.Execution : DeathType.Accident;
                case FamilyRole.Baby:
                    return DeathType.Medical;
                default:
                    return DeathType.Unknown;
            }
        }
        
        /// <summary>
        /// Execute family member death
        /// </summary>
        private void ExecuteFamilyDeath(FamilyMember member, DeathType deathType, int currentDay)
        {
            member.isAlive = false;
            member.deathDay = currentDay;
            member.deathType = deathType;
            
            Debug.Log($"[FamilyPressureManager] {member.name} has died on day {currentDay} from {deathType}");
            
            // Impact on other family members
            foreach (var otherMember in familyMembers)
            {
                if (otherMember.isAlive && otherMember != member)
                {
                    otherMember.happiness = Mathf.Max(0, otherMember.happiness - 30);
                    otherMember.relationship = Mathf.Max(0, otherMember.relationship - 20);
                }
            }
            
            // Create consequence token
            if (consequenceManager != null)
            {
                var consequenceData = new ConsequenceData
                {
                    newsHeadline = $"{member.name} Family Death",
                    scenarioToTrigger = $"FamilyDeath_{member.role}",
                    loyaltyImpact = -10,
                    suspicionIncrease = 5,
                    affectsFamily = true
                };
                consequenceManager.AddConsequenceToken($"Death_{member.role}_{currentDay}", 0, consequenceData);
            }
            
            // Trigger event
            OnFamilyMemberDeath?.Invoke(member);
            
            // Generate news about death
            GenerateDeathNews(member, deathType, currentDay);
        }
        
        /// <summary>
        /// Track when a family message is ignored
        /// </summary>
        public void RecordIgnoredMessage(string messageId, FamilyRole sender)
        {
            if (!ignoredMessages.ContainsKey(messageId))
            {
                ignoredMessages[messageId] = new IgnoredMessageTracker
                {
                    messageId = messageId,
                    sender = sender,
                    daysIgnored = 1,
                    hasEscalated = false
                };
            }
            else
            {
                ignoredMessages[messageId].daysIgnored++;
            }
            
            // Apply immediate relationship penalty
            var member = GetFamilyMember(sender);
            if (member != null)
            {
                ModifyRelationship(member, -ignorePenalty);
            }
        }
        
        /// <summary>
        /// Process ignored messages and trigger escalations
        /// </summary>
        private void ProcessIgnoredMessages(int currentDay)
        {
            var messagesToProcess = ignoredMessages.Values.ToList();
            
            foreach (var ignored in messagesToProcess)
            {
                if (ignored.daysIgnored >= crisisDaysThreshold && !ignored.hasEscalated)
                {
                    var member = GetFamilyMember(ignored.sender);
                    if (member != null && member.isAlive)
                    {
                        TriggerRelationshipCrisis(member, CrisisType.Ignored, currentDay);
                        ignored.hasEscalated = true;
                    }
                }
                
                // Family member takes independent action after being ignored
                if (ignored.daysIgnored >= 2 && ignored.independentAction == null)
                {
                    GenerateIndependentAction(ignored, currentDay);
                }
            }
        }
        
        /// <summary>
        /// Generate independent action when ignored
        /// </summary>
        private void GenerateIndependentAction(IgnoredMessageTracker ignored, int currentDay)
        {
            var member = GetFamilyMember(ignored.sender);
            if (member == null || !member.isAlive) return;
            
            switch (member.role)
            {
                case FamilyRole.Partner:
                    ignored.independentAction = () => {
                        member.activeTokens.Add("ContactedRebels");
                        Debug.Log($"[FamilyPressureManager] {member.name} contacted rebels without your knowledge");
                    };
                    break;
                case FamilyRole.Son:
                    ignored.independentAction = () => {
                        member.activeTokens.Add("MadePirateDeal");
                        member.safety -= 10;
                        Debug.Log($"[FamilyPressureManager] {member.name} made deal with pirates alone");
                    };
                    break;
                case FamilyRole.Daughter:
                    ignored.independentAction = () => {
                        member.activeTokens.Add("HidingRefugees");
                        member.safety -= 15;
                        Debug.Log($"[FamilyPressureManager] {member.name} hid refugees without telling you");
                    };
                    break;
            }
            
            ignored.independentAction?.Invoke();
        }
        
        /// <summary>
        /// Trigger a relationship crisis
        /// </summary>
        private void TriggerRelationshipCrisis(FamilyMember member, CrisisType type, int currentDay)
        {
            var crisis = new RelationshipCrisis
            {
                member = member,
                crisisType = type,
                triggerDay = currentDay,
                requiresImmediateResponse = true,
                resolved = false
            };
            
            activeCrises.Add(crisis);
            
            Debug.LogWarning($"[FamilyPressureManager] CRISIS: {member.name} - {type}");
            
            // Create crisis popup
            CreateCrisisPopup(crisis, currentDay);
            
            // Trigger event
            OnRelationshipCrisis?.Invoke(crisis);
        }
        
        /// <summary>
        /// Generate daily popups based on family state
        /// </summary>
        private void GenerateDailyPopups(int currentDay)
        {
            List<FamilyMember> eligibleMembers = new List<FamilyMember>();
            
            foreach (var member in familyMembers)
            {
                if (!member.isAlive) continue;
                
                float daysSinceLastPopup = currentDay - lastPopupTime.GetValueOrDefault(member.role, 0);
                if (daysSinceLastPopup >= basePopupFrequency)
                {
                    eligibleMembers.Add(member);
                }
            }
            
            // Randomly select up to maxPopupsPerDay
            int popupsToGenerate = Mathf.Min(eligibleMembers.Count, maxPopupsPerDay);
            for (int i = 0; i < popupsToGenerate && eligibleMembers.Count > 0; i++)
            {
                int index = UnityEngine.Random.Range(0, eligibleMembers.Count);
                var member = eligibleMembers[index];
                eligibleMembers.RemoveAt(index);
                
                GeneratePopupForMember(member, currentDay);
                lastPopupTime[member.role] = currentDay;
            }
        }
        
        /// <summary>
        /// Generate appropriate popup for family member
        /// </summary>
        private void GeneratePopupForMember(FamilyMember member, int currentDay)
        {
            PopupType type = DeterminePopupType(member, currentDay);
            FamilyPopupData popup = CreatePopupData(member, type, currentDay);
            
            queuedPopups.Add(popup);
            
            // Show immediately if urgent
            if (type == PopupType.Emergency)
            {
                ShowPopup(popup);
            }
        }
        
        /// <summary>
        /// Determine what type of popup to generate
        /// </summary>
        private PopupType DeterminePopupType(FamilyMember member, int currentDay)
        {
            // Emergency if health/safety critical
            if (member.health < 20 || member.safety < 20)
                return PopupType.Emergency;
            
            // Day-based emotional progression
            if (currentDay <= 10)
            {
                // Early days - mostly positive
                float roll = UnityEngine.Random.value;
                if (roll < 0.6f) return PopupType.Positive;
                if (roll < 0.9f) return PopupType.Neutral;
                return PopupType.Negative;
            }
            else if (currentDay <= 20)
            {
                // Middle days - balanced
                float roll = UnityEngine.Random.value;
                if (roll < 0.4f) return PopupType.Positive;
                if (roll < 0.7f) return PopupType.Neutral;
                return PopupType.Negative;
            }
            else
            {
                // Late days - mostly negative
                float roll = UnityEngine.Random.value;
                if (roll < 0.2f) return PopupType.Positive;
                if (roll < 0.4f) return PopupType.Neutral;
                if (roll < 0.8f) return PopupType.Negative;
                return PopupType.Emergency;
            }
        }
        
        /// <summary>
        /// Handle positive moment rewards
        /// </summary>
        public void ProcessPositiveMoment(FamilyMember member, PositiveMomentType momentType)
        {
            switch (momentType)
            {
                case PositiveMomentType.BabyDrawing:
                    // Remove a strike from record
                    if (gameManager != null && gameManager.GetStrikes() > 0)
                    {
                        gameManager.RemoveStrike();
                        OnStrikeRemoved?.Invoke(1);
                        Debug.Log("[FamilyPressureManager] Baby's drawing removed a strike!");
                    }
                    // Happiness boost for all
                    foreach (var fm in familyMembers)
                    {
                        if (fm.isAlive) fm.happiness = Mathf.Min(100, fm.happiness + 5);
                    }
                    break;
                    
                case PositiveMomentType.SonAchievement:
                    // Credit bonus
                    if (creditsManager != null)
                    {
                        creditsManager.AddCredits(200, "Son's squad leader bonus");
                    }
                    member.happiness = Mathf.Min(100, member.happiness + 15);
                    break;
                    
                case PositiveMomentType.DaughterInnovation:
                    // Save credits
                    if (creditsManager != null)
                    {
                        creditsManager.AddCredits(50, "Daughter fixed heating unit");
                    }
                    member.happiness = Mathf.Min(100, member.happiness + 10);
                    break;
                    
                case PositiveMomentType.PartnerSupport:
                    // Performance boost (would need to integrate with shift system)
                    if (gameManager != null)
                    {
                        gameManager.SetPerformanceModifier(1.15f);
                        Debug.Log("[FamilyPressureManager] Partner support boosted performance to 115%");
                    }
                    member.relationship = Mathf.Min(100, member.relationship + 10);
                    break;
                    
                case PositiveMomentType.DroidMystery:
                    // Random benefit
                    int randomBenefit = UnityEngine.Random.Range(50, 100);
                    if (creditsManager != null)
                    {
                        creditsManager.AddCredits(randomBenefit, "D-3X found accounting error");
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Get family member by role
        /// </summary>
        public FamilyMember GetFamilyMember(FamilyRole role)
        {
            return familyMembers.FirstOrDefault(m => m.role == role);
        }
        
        /// <summary>
        /// Get all living family members
        /// </summary>
        public List<FamilyMember> GetLivingFamily()
        {
            return familyMembers.Where(m => m.isAlive).ToList();
        }
        
        /// <summary>
        /// Modify relationship with family member
        /// </summary>
        public void ModifyRelationship(FamilyMember member, int amount)
        {
            if (member == null || !member.isAlive) return;
            
            int oldValue = member.relationship;
            member.relationship = Mathf.Clamp(member.relationship + amount, 0, 100);
            
            if (oldValue != member.relationship)
            {
                OnRelationshipChange?.Invoke(member, member.relationship - oldValue);
            }
        }
        
        /// <summary>
        /// Create popup data for family member
        /// </summary>
        private FamilyPopupData CreatePopupData(FamilyMember member, PopupType type, int currentDay)
        {
            var popup = new FamilyPopupData
            {
                source = member,
                type = type,
                dayTriggered = currentDay,
                isIgnorable = (type != PopupType.Emergency)
            };
            
            // Generate content based on member and type
            GeneratePopupContent(popup, member, type, currentDay);
            
            return popup;
        }
        
        /// <summary>
        /// Generate appropriate popup content
        /// </summary>
        private void GeneratePopupContent(FamilyPopupData popup, FamilyMember member, PopupType type, int currentDay)
        {
            switch (member.role)
            {
                case FamilyRole.Baby:
                    GenerateBabyPopup(popup, type, currentDay);
                    break;
                case FamilyRole.Son:
                    GenerateSonPopup(popup, type, currentDay);
                    break;
                case FamilyRole.Daughter:
                    GenerateDaughterPopup(popup, type, currentDay);
                    break;
                case FamilyRole.Partner:
                    GeneratePartnerPopup(popup, type, currentDay);
                    break;
                case FamilyRole.Droid:
                    GenerateDroidPopup(popup, type, currentDay);
                    break;
            }
        }
        
        /// <summary>
        /// Generate baby-specific popup content
        /// </summary>
        private void GenerateBabyPopup(FamilyPopupData popup, PopupType type, int currentDay)
        {
            switch (type)
            {
                case PopupType.Positive:
                    popup.title = "Look what I made!";
                    popup.content = "A colorful drawing of the family standing together";
                    popup.responses = new List<PopupResponse>
                    {
                        new PopupResponse { text = "Beautiful!", consequence = () => ProcessPositiveMoment(popup.source, PositiveMomentType.BabyDrawing) },
                        new PopupResponse { text = "Put it on the fridge!", consequence = () => { popup.source.happiness += 10; foreach(var m in familyMembers) m.happiness += 3; } },
                        new PopupResponse { text = "Not now", consequence = () => popup.source.happiness -= 5 }
                    };
                    break;
                case PopupType.Negative:
                    popup.title = "Baby is crying";
                    popup.content = "The baby won't stop coughing and feels warm";
                    popup.responses = new List<PopupResponse>
                    {
                        new PopupResponse { text = "Call doctor (50 credits)", consequence = () => { creditsManager?.DeductCredits(50, "Doctor visit"); popup.source.health += 10; } },
                        new PopupResponse { text = "Wait and see", consequence = () => popup.source.health -= 5 }
                    };
                    break;
                case PopupType.Emergency:
                    popup.title = "MEDICAL EMERGENCY";
                    popup.content = "Baby can't breathe properly - needs immediate care!";
                    popup.isIgnorable = false;
                    popup.responses = new List<PopupResponse>
                    {
                        new PopupResponse { text = "Rush to hospital (500 credits)", consequence = () => { creditsManager?.DeductCredits(500, "Emergency care"); popup.source.health = 40; popup.source.hasDeathWarning = false; } },
                        new PopupResponse { text = "Can't afford it", consequence = () => popup.source.health = 5 }
                    };
                    break;
            }
        }
        
        /// <summary>
        /// Get family member stats for debugging
        /// </summary>
        [ContextMenu("Log Family Status")]
        public void LogFamilyStatus()
        {
            foreach (var member in familyMembers)
            {
                Debug.Log($"[FamilyPressureManager] {member.name} ({member.role}): " +
                         $"Alive={member.isAlive}, Rel={member.relationship}, " +
                         $"Happy={member.happiness}, Safe={member.safety}, Health={member.health}");
            }
        }
        
        /// <summary>
        /// Get current family status for ending determination
        /// </summary>
        public FamilyEndingStatus GetFamilyEndingStatus()
        {
            return new FamilyEndingStatus
            {
                survivingMembers = familyMembers.Count(m => m.isAlive),
                totalDeaths = familyMembers.Count(m => !m.isAlive),
                averageRelationship = familyMembers.Where(m => m.isAlive).Select(m => m.relationship).DefaultIfEmpty(0).Average(),
                hasPartner = GetFamilyMember(FamilyRole.Partner)?.isAlive ?? false,
                hasSon = GetFamilyMember(FamilyRole.Son)?.isAlive ?? false,
                hasDaughter = GetFamilyMember(FamilyRole.Daughter)?.isAlive ?? false,
                hasBaby = GetFamilyMember(FamilyRole.Baby)?.isAlive ?? false
            };
        }
        
        /// <summary>
        /// Get average family safety for legacy compatibility
        /// </summary>
        public int GetFamilySafety()
        {
            var livingMembers = familyMembers.Where(m => m.isAlive).ToList();
            if (livingMembers.Count == 0) return 0;
            
            return (int)livingMembers.Average(m => m.safety);
        }
        
        /// <summary>
        /// Get average family happiness for legacy compatibility
        /// </summary>
        public int GetFamilyHappiness()
        {
            var livingMembers = familyMembers.Where(m => m.isAlive).ToList();
            if (livingMembers.Count == 0) return 0;
            
            return (int)livingMembers.Average(m => m.happiness);
        }
        
        /// <summary>
        /// Trigger family expense for legacy compatibility
        /// </summary>
        public void TriggerFamilyExpense(int amount, string reason)
        {
            if (creditsManager != null)
            {
                creditsManager.DeductCredits(amount, reason);
                
                // Find most relevant family member for this expense
                var targetMember = familyMembers.FirstOrDefault(m => m.isAlive && m.health < 50);
                if (targetMember != null)
                {
                    targetMember.happiness -= 5; // Stress from financial pressure
                }
            }
        }
        
        /// <summary>
        /// Trigger family expense with 3 parameters for legacy compatibility
        /// </summary>
        public void TriggerFamilyExpense(string reason, int amount, int severity)
        {
            if (creditsManager != null)
            {
                creditsManager.DeductCredits(amount, reason);
                
                // Apply stress based on severity
                foreach (var member in familyMembers)
                {
                    if (member.isAlive)
                    {
                        member.happiness -= severity * 2; // Higher severity = more stress
                        member.happiness = Mathf.Max(0, member.happiness);
                    }
                }
            }
        }
        
        // TODO: Add remaining popup generation methods for Son, Daughter, Partner, Droid
        // TODO: Add UI integration for popup display
        // TODO: Add integration with checkpoint scenarios
        // TODO: Add save/load support for family state
        
        private void OnDestroy()
        {
            if (dataLogManager != null)
            {
                PersonalDataLogManager.OnDataLogClosed -= OnDataLogClosed;
            }
        }
        
        #region Helper Methods
        
        private void UpdateStoryPhase(FamilyMember member, int currentDay)
        {
            if (currentDay <= 10)
                member.currentPhase = StoryArcPhase.Setup;
            else if (currentDay <= 20)
                member.currentPhase = StoryArcPhase.Escalation;
            else if (currentDay <= 27)
                member.currentPhase = StoryArcPhase.Crisis;
            else
                member.currentPhase = StoryArcPhase.Resolution;
        }
        
        private void CheckForCrisis(FamilyMember member, int currentDay)
        {
            if (member.relationship < 20 && !HasActiveCrisis(member, CrisisType.Relationship))
            {
                TriggerRelationshipCrisis(member, CrisisType.Relationship, currentDay);
            }
            
            if (member.health < 30 && !HasActiveCrisis(member, CrisisType.Health))
            {
                TriggerRelationshipCrisis(member, CrisisType.Health, currentDay);
            }
            
            if (member.safety < 30 && !HasActiveCrisis(member, CrisisType.Safety))
            {
                TriggerRelationshipCrisis(member, CrisisType.Safety, currentDay);
            }
        }
        
        private bool HasActiveCrisis(FamilyMember member, CrisisType type)
        {
            return activeCrises.Any(c => c.member == member && c.crisisType == type && !c.resolved);
        }
        
        private void CreateEmergencyPopup(FamilyMember member, int currentDay)
        {
            var popup = new FamilyPopupData
            {
                source = member,
                type = PopupType.Emergency,
                title = $"EMERGENCY: {member.name} in Critical Danger!",
                content = GetEmergencyMessage(member),
                dayTriggered = currentDay,
                isIgnorable = false,
                ignoreRelationshipPenalty = 50
            };
            
            popup.responses = new List<PopupResponse>
            {
                new PopupResponse 
                { 
                    text = $"Do everything possible (1000 credits)",
                    consequence = () => {
                        if (creditsManager != null && creditsManager.CurrentCredits >= 1000)
                        {
                            creditsManager.DeductCredits(1000, $"Emergency intervention for {member.name}");
                            member.health = Mathf.Max(40, member.health);
                            member.safety = Mathf.Max(40, member.safety);
                            member.hasDeathWarning = false;
                        }
                    }
                },
                new PopupResponse
                {
                    text = "I can't help",
                    consequence = () => {
                        member.relationship -= 30;
                    }
                }
            };
            
            ShowPopup(popup);
        }
        
        private string GetEmergencyMessage(FamilyMember member)
        {
            switch (member.role)
            {
                case FamilyRole.Partner:
                    return "Imperial Intelligence is closing in. They suspect rebel connections!";
                case FamilyRole.Son:
                    return "Pirates threatening his life over unpaid debts!";
                case FamilyRole.Daughter:
                    return "Security sweep of her shop imminent - refugees will be found!";
                case FamilyRole.Baby:
                    return "Drift Sickness critical - organs failing without treatment!";
                default:
                    return "Immediate intervention required!";
            }
        }
        
        private void ShowPopup(FamilyPopupData popup)
        {
            OnFamilyPopup?.Invoke(popup);
            
            if (familyPopupPrefab != null && popupContainer != null)
            {
                var popupObj = Instantiate(familyPopupPrefab, popupContainer);
                // TODO: Create FamilyPopupUI component
                Debug.Log($"[FamilyPressureManager] Would show popup: {popup.title}");
                // var popupComponent = popupObj.GetComponent<FamilyPopupUI>();
                // if (popupComponent != null)
                // {
                //     popupComponent.Initialize(popup);
                // }
            }
        }
        
        private void CreateCrisisPopup(RelationshipCrisis crisis, int currentDay)
        {
            var popup = new FamilyPopupData
            {
                source = crisis.member,
                type = PopupType.Emergency,
                title = "RELATIONSHIP CRISIS",
                content = GetCrisisMessage(crisis),
                dayTriggered = currentDay,
                isIgnorable = false,
                ignoreRelationshipPenalty = 30
            };
            
            popup.responses = new List<PopupResponse>
            {
                new PopupResponse
                {
                    text = "I'm sorry, let's talk",
                    consequence = () => {
                        crisis.resolved = true;
                        crisis.member.relationship += 10;
                    }
                },
                new PopupResponse
                {
                    text = "I'm doing my best!",
                    consequence = () => {
                        crisis.resolved = true;
                        crisis.member.relationship -= 5;
                    }
                },
                new PopupResponse
                {
                    text = "Leave me alone",
                    consequence = () => {
                        crisis.resolved = true;
                        crisis.member.relationship -= 20;
                        crisis.member.activeTokens.Add("Estranged");
                    }
                }
            };
            
            ShowPopup(popup);
        }
        
        private string GetCrisisMessage(RelationshipCrisis crisis)
        {
            if (crisis.crisisType == CrisisType.Ignored)
            {
                return $"{crisis.member.name}: \"You haven't responded to me in days. Do I even matter to you anymore?\"";
            }
            else if (crisis.crisisType == CrisisType.Relationship)
            {
                return $"{crisis.member.name}: \"I don't know who you are anymore. The Empire has changed you.\"";
            }
            else if (crisis.crisisType == CrisisType.Health)
            {
                return $"{crisis.member.name} is seriously ill and needs immediate help!";
            }
            else if (crisis.crisisType == CrisisType.Safety)
            {
                return $"{crisis.member.name} is in serious danger!";
            }
            
            return "Your family needs you.";
        }
        
        private void GenerateSonPopup(FamilyPopupData popup, PopupType type, int currentDay)
        {
            // TODO: Implement son-specific popup content
        }
        
        private void GenerateDaughterPopup(FamilyPopupData popup, PopupType type, int currentDay)
        {
            // TODO: Implement daughter-specific popup content
        }
        
        private void GeneratePartnerPopup(FamilyPopupData popup, PopupType type, int currentDay)
        {
            // TODO: Implement partner-specific popup content
        }
        
        private void GenerateDroidPopup(FamilyPopupData popup, PopupType type, int currentDay)
        {
            // TODO: Implement droid-specific popup content
        }
        
        private void CleanupResolvedCrises()
        {
            activeCrises.RemoveAll(c => c.resolved);
        }
        
        private void GenerateDeathNews(FamilyMember member, DeathType deathType, int currentDay)
        {
            if (dataLogManager == null) return;
            
            string headline = "";
            string content = "";
            
            switch (deathType)
            {
                case DeathType.Medical:
                    headline = "Medical Emergency at Base Housing";
                    content = "Despite best efforts of medical staff, young child succumbs to Drift Sickness.";
                    break;
                case DeathType.Combat:
                    headline = "Training Accident Claims Recruit";
                    content = "Promising young trooper killed in live-fire exercise gone wrong.";
                    break;
                case DeathType.Execution:
                    headline = "Traitor Executed for Harboring Fugitives";
                    content = "Dock worker found guilty of hiding refugees, sentence carried out immediately.";
                    break;
                case DeathType.Disappeared:
                    headline = "Science Division Researcher Missing";
                    content = "Authorities investigating disappearance, foul play suspected.";
                    break;
            }
            
            var entry = new DataLogEntry
            {
                feedType = FeedType.ImperiumNews,
                headline = headline,
                content = content,
                severity = 3,
                timestamp = DateTime.Now,
                requiresAction = false
            };
            
            dataLogManager.AddLogEntry(entry);
        }
        
        private void OnDataLogClosed()
        {
            // TODO: Check which family messages weren't interacted with
        }
        
        #endregion
    }
    
    // Supporting classes and enums
    [System.Serializable]
    public class FamilyMember
    {
        public string name;
        public FamilyRole role;
        public int relationship = 50; // 0-100
        public int happiness = 50;    // 0-100
        public int safety = 50;       // 0-100
        public int health = 50;       // 0-100
        public bool isAlive = true;
        public List<string> activeTokens = new List<string>();
        public StoryArcPhase currentPhase = StoryArcPhase.Setup;
        public string[] personalityTraits;
        public bool hasDeathWarning = false;
        public int deathDay = -1;
        public DeathType deathType = DeathType.None;
    }
    
    public enum FamilyRole
    {
        Partner,
        Son,
        Daughter,
        Baby,
        Droid
    }
    
    public enum StoryArcPhase
    {
        Setup,      // Days 1-10
        Escalation, // Days 11-20
        Crisis,     // Days 21-27
        Resolution  // Days 28-30
    }
    
    public enum DeathType
    {
        None,
        Medical,
        Combat,
        Execution,
        Disappeared,
        Accident,
        Unknown
    }
    
    public enum CrisisType
    {
        Ignored,
        Relationship,
        Health,
        Safety,
        Financial,
        Betrayal
    }
    
    public enum PopupType
    {
        Positive,
        Negative,
        Neutral,
        Mystery,
        Emergency
    }
    
    public enum PositiveMomentType
    {
        BabyDrawing,
        SonAchievement,
        DaughterInnovation,
        PartnerSupport,
        DroidMystery
    }
    
    [System.Serializable]
    public class RelationshipCrisis
    {
        public FamilyMember member;
        public CrisisType crisisType;
        public int triggerDay;
        public bool requiresImmediateResponse;
        public bool resolved;
    }
    
    [System.Serializable]
    public class IgnoredMessageTracker
    {
        public string messageId;
        public FamilyRole sender;
        public int daysIgnored;
        public bool hasEscalated;
        public string escalationMessageId;
        public System.Action independentAction;
    }
    
    [System.Serializable]
    public class FamilyPopupData
    {
        public FamilyMember source;
        public PopupType type;
        public string title;
        public string content;
        public Sprite visualElement; // Drawing, photo, flyer
        public List<PopupResponse> responses;
        public ConsequenceToken[] consequences;
        public bool isIgnorable = true;
        public int ignoreRelationshipPenalty = 5;
        public int dayTriggered;
    }
    
    [System.Serializable]
    public class PopupResponse
    {
        public string text;
        public System.Action consequence;
    }
    
    [System.Serializable]
    public class FamilyEndingStatus
    {
        public int survivingMembers;
        public int totalDeaths;
        public double averageRelationship;
        public bool hasPartner;
        public bool hasSon;
        public bool hasDaughter;
        public bool hasBaby;
    }
}