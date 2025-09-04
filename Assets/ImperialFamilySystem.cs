using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the player's Imperium family status and related game mechanics
/// Core system for family interaction in the game
/// </summary>
public class ImperialFamilySystem : MonoBehaviour
{
    [System.Serializable]
    public class FamilyMember
    {
        public string name;               // Member name
        public string occupation;         // Imperium occupation
        public Sprite portrait;           // Visual representation
        public bool needsMedicalCare;     // Whether needs medical attention
        public bool needsEquipment;       // Whether needs specialized equipment
        public bool needsTraining;        // Whether needs training/protection
        [TextArea(2, 4)]
        public string[] possibleEvents;   // Possible events for this family member
    }
    
    [Header("Family Members")]
    public FamilyMember[] familyMembers;
    
    [Header("UI References")]
    public GameObject familyStatusPanel;
    public Transform familyMemberContainer;
    public GameObject familyMemberPrefab;
    public TMP_Text familyEventText;
    public Button viewFamilyButton;
    
    [Header("Game Settings")]
    public float medicalChance = 0.3f;          // Chance of needing medical care
    public float equipmentChance = 0.2f;        // Chance of needing equipment
    public float trainingChance = 0.25f;        // Chance of needing training
    public float recoveryChance = 0.4f;         // Chance of recovering from issues
    
    [Header("Cost Settings")]
    public int medicalCareCost = 30;            // Cost for specialized medical care
    public int equipmentCost = 25;              // Cost for specialized equipment
    public int trainingCost = 20;               // Cost for training/bribes
    public int premiumQuartersCost = 15;        // Cost for better living quarters
    public int childcareCost = 10;              // Cost for childcare services
    public int robotMaintenanceCost = 15;       // Cost for maintaining family droid
    
    // Status tracking
    private bool hasMedicalCare = true;
    private bool hasEquipment = true;
    private bool hasTraining = true;
    private bool hasPremiumQuarters = true;     // Default to premium quarters
    private bool hasChildcare = true;
    private bool hasDroidMaintenance = true;
    private List<string> familyEvents = new List<string>();
    
    // References to other systems
    private GameManager gameManager;
    private FamilyDisplayManager displayManager;
    
    // Start is called before the first frame update
    void Start()
    {
        // Find the GameManager
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("ImperialFamilySystem: GameManager not found!");
        }
        
        // Find or setup display manager
        SetupDisplayManager();
        
        // Initialize family members if needed
        InitializeFamily();
        
        // Set up UI
        if (viewFamilyButton != null)
            viewFamilyButton.onClick.AddListener(ToggleFamilyPanel);
        
        // Hide family panel initially
        if (familyStatusPanel)
            familyStatusPanel.SetActive(false);
        
        // Generate initial family event
        AddFamilyEvent(GenerateFamilyEvent());
    }
    
    // Update is called once per frame
    void Update()
    {
        // Handle any time-based family events if needed
    }
    
    /// <summary>
    /// Set up the display manager for UI
    /// </summary>
    private void SetupDisplayManager()
    {
        // Try to find display manager in scene
        displayManager = FindFirstObjectByType<FamilyDisplayManager>();
        
        // If found, set up the reference to this system
        if (displayManager != null)
        {
            displayManager.familySystem = this;
            
            // Initialize displays if needed
            if (displayManager.memberDisplays == null || displayManager.memberDisplays.Length == 0)
            {
                displayManager.RefreshDisplays();
            }
        }
    }
    
    /// <summary>
    /// Initialize the family members if not already set up
    /// </summary>
    private void InitializeFamily()
    {
        if (familyMembers == null || familyMembers.Length == 0)
        {
            // Create default Imperium family with the structure that was agreed on
            familyMembers = new FamilyMember[]
            {
                new FamilyMember { 
                    name = "Emma", 
                    occupation = "Imperium Officer", 
                    needsMedicalCare = false, 
                    needsEquipment = false, 
                    needsTraining = false,
                    possibleEvents = new string[] {
                        "Emma has been commended for her loyalty to the Empire.",
                        "Emma is being considered for a promotion to senior officer.",
                        "Emma is concerned about recent security protocols."
                    }
                },
                new FamilyMember { 
                    name = "Kira", 
                    occupation = "Fighter Mechanic", 
                    needsMedicalCare = false, 
                    needsEquipment = false, 
                    needsTraining = false,
                    possibleEvents = new string[] {
                        "Kira needs specialized tools for her new assignment.",
                        "Kira fixed a high-ranking officer's ship and received recognition.",
                        "Kira is working overtime on TIE fighter repairs."
                    }
                },
                new FamilyMember { 
                    name = "Jace", 
                    occupation = "Trooper Cadet", 
                    needsMedicalCare = false, 
                    needsEquipment = false, 
                    needsTraining = true,
                    possibleEvents = new string[] {
                        "Jace's training squad is being assigned to a new instructor.",
                        "Jace was injured during combat training. He needs medical attention.",
                        "Jace was commended for his marksmanship during evaluations."
                    }
                },
                new FamilyMember { 
                    name = "R2-D4", 
                    occupation = "Family Droid", 
                    needsMedicalCare = false, 
                    needsEquipment = true, 
                    needsTraining = false,
                    possibleEvents = new string[] {
                        "R2-D4 needs a memory upgrade to meet new Imperium protocols.",
                        "R2-D4 requires servicing for its aging actuators.",
                        "R2-D4 detected unusual communications in your sector."
                    }
                }
            };
        }
        
        // Create UI elements for family members if prefab exists
        if (familyMemberContainer && familyMemberPrefab)
        {
            // Clear any existing children
            foreach (Transform child in familyMemberContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create new ones
            foreach (FamilyMember member in familyMembers)
            {
                GameObject memberObj = Instantiate(familyMemberPrefab, familyMemberContainer);
                
                // Try to set name using TMP_Text
                TMP_Text nameText = memberObj.GetComponentInChildren<TMP_Text>();
                if (nameText) nameText.text = member.name + " (" + member.occupation + ")";
                
                // Try to set portrait if available
                Image portrait = memberObj.transform.Find("Portrait")?.GetComponent<Image>();
                if (portrait && member.portrait) portrait.sprite = member.portrait;
                
                // Set status indicators
                UpdateMemberStatusIcons(memberObj, member);
            }
        }
    }
    
    /// <summary>
    /// Update a family member's status icons in the UI
    /// </summary>
    private void UpdateMemberStatusIcons(GameObject memberObj, FamilyMember member)
    {
        if (memberObj == null) return;
        
        // Update medical icon
        GameObject medicalIcon = memberObj.transform.Find("MedicalIcon")?.gameObject;
        if (medicalIcon) medicalIcon.SetActive(member.needsMedicalCare);
        
        // Update equipment icon
        GameObject equipmentIcon = memberObj.transform.Find("EquipmentIcon")?.gameObject;
        if (equipmentIcon) equipmentIcon.SetActive(member.needsEquipment);
        
        // Update training icon
        GameObject trainingIcon = memberObj.transform.Find("TrainingIcon")?.gameObject;
        if (trainingIcon) trainingIcon.SetActive(member.needsTraining);
    }
    
    /// <summary>
    /// Process daily family events and status changes
    /// Called by GameManager at the start of each new day
    /// </summary>
    public void ProcessDay(bool insufficientCredits)
    {
        // Update care status based on credits
        if (insufficientCredits)
        {
            hasMedicalCare = false;
            hasEquipment = false;
            hasTraining = false;
            hasPremiumQuarters = false;
            hasChildcare = false;
            hasDroidMaintenance = false;
        }
        
        // Process each family member
        foreach (FamilyMember member in familyMembers)
        {
            // Process recovery if care is provided
            if (member.needsMedicalCare && hasMedicalCare && Random.value < recoveryChance)
            {
                member.needsMedicalCare = false;
                AddFamilyEvent($"{member.name} has recovered thanks to Imperium medical care.");
            }
            
            if (member.needsEquipment && hasEquipment && Random.value < recoveryChance)
            {
                // Special handling for droid
                if (member.occupation == "Family Droid")
                {
                    if (hasDroidMaintenance)
                    {
                        member.needsEquipment = false;
                        AddFamilyEvent($"{member.name} has been properly serviced and is functioning optimally.");
                    }
                }
                else
                {
                    member.needsEquipment = false;
                    AddFamilyEvent($"{member.name} received the necessary equipment.");
                }
            }
            
            if (member.needsTraining && hasTraining && Random.value < recoveryChance)
            {
                member.needsTraining = false;
                AddFamilyEvent($"{member.name} has completed their training successfully.");
            }
            
            // Check for new issues based on occupation
            if (!member.needsMedicalCare && Random.value < medicalChance)
            {
                // Different chances based on occupation
                float adjustedChance = medicalChance;
                
                if (member.occupation == "Trooper Cadet" && !hasTraining)
                {
                    // Higher chance of injury for cadet without training
                    adjustedChance *= 2f;
                }
                
                if (Random.value < adjustedChance)
                {
                    member.needsMedicalCare = true;
                    if (member.occupation == "Trooper Cadet")
                        AddFamilyEvent($"{member.name} was injured during training exercises.");
                    else if (member.occupation != "Family Droid") // Droids don't need medical care
                        AddFamilyEvent($"{member.name} requires medical attention.");
                }
            }
            
            if (!member.needsEquipment && Random.value < equipmentChance)
            {
                if (member.occupation == "Fighter Mechanic")
                {
                    member.needsEquipment = true;
                    AddFamilyEvent($"{member.name} needs specialized tools for an important assignment.");
                }
                else if (member.occupation == "Family Droid")
                {
                    member.needsEquipment = true;
                    AddFamilyEvent($"{member.name} requires maintenance and new parts.");
                }
            }
            
            if (!member.needsTraining && Random.value < trainingChance)
            {
                if (member.occupation == "Trooper Cadet")
                {
                    member.needsTraining = true;
                    AddFamilyEvent($"{member.name}'s training requires additional resources.");
                }
            }
        }
        
        // Generate random family event
        if (Random.value < 0.4f)
        {
            AddFamilyEvent(GenerateFamilyEvent());
        }
        
        // Update UI
        UpdateFamilyUI();
        
        // Update display manager if available
        if (displayManager != null)
            displayManager.UpdateDisplayStatus();
    }
    
    /// <summary>
    /// Add a family event to the history
    /// </summary>
    public void AddFamilyEvent(string eventText)
    {
        if (string.IsNullOrEmpty(eventText))
            return;
            
        familyEvents.Add(eventText);
        
        // If there are too many events, remove the oldest
        if (familyEvents.Count > 10)
        {
            familyEvents.RemoveAt(0);
        }
        
        // Update the UI if available
        if (familyEventText)
        {
            familyEventText.text = eventText;
        }
    }
    
    /// <summary>
    /// Set whether medical care is provided
    /// </summary>
    public void SetMedicalCareStatus(bool provideMedicalCare)
    {
        hasMedicalCare = provideMedicalCare;
    }
    
    /// <summary>
    /// Set whether equipment is provided
    /// </summary>
    public void SetEquipmentStatus(bool provideEquipment)
    {
        hasEquipment = provideEquipment;
    }
    
    /// <summary>
    /// Set whether training is provided
    /// </summary>
    public void SetTrainingStatus(bool provideTraining)
    {
        hasTraining = provideTraining;
    }
    
    /// <summary>
    /// Set whether premium quarters are provided
    /// </summary>
    public void SetQuartersStatus(bool providePremiumQuarters)
    {
        hasPremiumQuarters = providePremiumQuarters;
    }
    
    /// <summary>
    /// Set whether childcare is provided
    /// </summary>
    public void SetChildcareStatus(bool provideChildcare)
    {
        hasChildcare = provideChildcare;
    }
    
    /// <summary>
    /// Set whether droid maintenance is provided
    /// </summary>
    public void SetDroidMaintenanceStatus(bool provideMaintenance)
    {
        hasDroidMaintenance = provideMaintenance;
    }
    
    /// <summary>
    /// Check if any family member needs medical care
    /// </summary>
    public bool NeedsMedicalCare()
    {
        foreach (FamilyMember member in familyMembers)
        {
            if (member.needsMedicalCare) return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if any family member needs equipment
    /// </summary>
    public bool NeedsEquipment()
    {
        foreach (FamilyMember member in familyMembers)
        {
            if (member.needsEquipment) return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if any family member needs training
    /// </summary>
    public bool NeedsTraining()
    {
        foreach (FamilyMember member in familyMembers)
        {
            if (member.needsTraining) return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Toggle the family panel visibility
    /// </summary>
    public void ToggleFamilyPanel()
    {
        if (familyStatusPanel)
        {
            bool newState = !familyStatusPanel.activeSelf;
            familyStatusPanel.SetActive(newState);
            
            // Update UI when showing panel
            if (newState)
                UpdateFamilyUI();
        }
    }
    
    /// <summary>
    /// Show the family status panel
    /// </summary>
    public void ShowFamilyPanel()
    {
        if (familyStatusPanel)
        {
            familyStatusPanel.SetActive(true);
            UpdateFamilyUI();
        }
    }
    
    /// <summary>
    /// Hide the family status panel
    /// </summary>
    public void HideFamilyPanel()
    {
        if (familyStatusPanel)
            familyStatusPanel.SetActive(false);
    }
    
    /// <summary>
    /// Update the family UI
    /// </summary>
    public void UpdateFamilyUI()
    {
        // Skip if no container
        if (familyMemberContainer == null) return;
        
        // Update each family member's UI
        for (int i = 0; i < familyMembers.Length; i++)
        {
            if (i < familyMemberContainer.childCount)
            {
                GameObject memberObj = familyMemberContainer.GetChild(i).gameObject;
                UpdateMemberStatusIcons(memberObj, familyMembers[i]);
            }
        }
        
        // Update display manager if available
        if (displayManager != null)
            displayManager.UpdateDisplayStatus();
    }
    
    /// <summary>
    /// Get the family status info object for external systems like the Daily Briefing
    /// </summary>
    public FamilyStatusInfo GetFamilyStatusInfo()
    {
        FamilyStatusInfo info = new FamilyStatusInfo();
        
        // Create arrays for status info
        info.Names = new string[familyMembers.Length];
        info.Occupations = new string[familyMembers.Length];
        info.Portraits = new Sprite[familyMembers.Length];
        info.NeedsMedical = new bool[familyMembers.Length];
        info.NeedsEquipment = new bool[familyMembers.Length];
        info.NeedsTraining = new bool[familyMembers.Length];
        info.HealthStatus = new int[familyMembers.Length];
        info.LoyaltyStatus = new int[familyMembers.Length];
        
        // Fill in data for each family member
        for (int i = 0; i < familyMembers.Length; i++)
        {
            info.Names[i] = familyMembers[i].name;
            info.Occupations[i] = familyMembers[i].occupation;
            info.Portraits[i] = familyMembers[i].portrait;
            info.NeedsMedical[i] = familyMembers[i].needsMedicalCare;
            info.NeedsEquipment[i] = familyMembers[i].needsEquipment;
            info.NeedsTraining[i] = familyMembers[i].needsTraining;
            
            // Calculate health status (0-100) - lower if needs medical care
            info.HealthStatus[i] = familyMembers[i].needsMedicalCare ? 60 : 90;
            
            // Loyalty is generally high for Imperium families
            info.LoyaltyStatus[i] = 90;
        }
        
        // Set family environment properties
        info.HasPremiumQuarters = hasPremiumQuarters;
        info.NeedsChildcare = !hasChildcare;
        info.NeedsDroidMaintenance = !hasDroidMaintenance;
        
        return info;
    }
    
    /// <summary>
    /// Generate a family event based on current family status
    /// </summary>
    public string GenerateFamilyEvent()
    {
        // 70% chance to pick a random family member's event
        if (Random.value < 0.7f && familyMembers.Length > 0)
        {
            // Pick a random family member
            FamilyMember member = familyMembers[Random.Range(0, familyMembers.Length)];
            
            // If they have possible events, pick one
            if (member.possibleEvents != null && member.possibleEvents.Length > 0)
            {
                return member.possibleEvents[Random.Range(0, member.possibleEvents.Length)];
            }
        }
        
        // General family events
        string[] events = new string[]
        {
            "Your family has been invited to an Imperium officer's gathering.",
            "A surprise inspection of family quarters is scheduled for next week.",
            "Your family received a commendation for Imperium loyalty.",
            "Your superior officer commented on your family's exemplary service to the Empire.",
            "Your family quarters need maintenance. The repair droids have been dispatched.",
            "A new Imperium directive requires all family members to attend loyalty classes.",
            "Your family receives a care package from relatives on Coruscant.",
            "A neighbor reports suspicious conversations near your quarters.",
            "Your family receives notice about upcoming officer family relocation exercises."
        };
        
        return events[Random.Range(0, events.Length)];
    }
    
    /// <summary>
    /// Calculate daily expenses for the family based on needs
    /// </summary>
    public Dictionary<string, int> CalculateExpenses()
    {
        Dictionary<string, int> expenses = new Dictionary<string, int>();
        
        // Check if any family member needs medical care
        bool needsMedical = false;
        bool needsEquipment = false;
        bool needsTraining = false;
        bool needsDroidMaintenance = false;
        
        foreach (var member in familyMembers)
        {
            if (member.needsMedicalCare)
                needsMedical = true;
                
            if (member.needsEquipment)
            {
                needsEquipment = true;
                
                // Check specifically for droid
                if (member.occupation == "Family Droid")
                    needsDroidMaintenance = true;
            }
                
            if (member.needsTraining)
                needsTraining = true;
        }
        
        // Add expenses based on needs
        if (needsMedical)
            expenses.Add("Medical Care", medicalCareCost);
            
        if (needsEquipment)
            expenses.Add("Equipment", equipmentCost);
            
        if (needsTraining)
            expenses.Add("Training", trainingCost);
            
        // Always add premium quarters cost - this is a quality of life choice
        expenses.Add("Premium Quarters", premiumQuartersCost);
        
        // Add childcare for the children
        expenses.Add("Childcare", childcareCost);
        
        // Add droid maintenance cost if needed
        if (needsDroidMaintenance)
            expenses.Add("Droid Maintenance", robotMaintenanceCost);
        
        return expenses;
    }
    
    /// <summary>
    /// Get total expenses for the day
    /// </summary>
    public int GetTotalExpenses()
    {
        int total = 0;
        
        // Sum up all expenses
        foreach (var expense in CalculateExpenses())
        {
            total += expense.Value;
        }
        return total;
    }                       
}
