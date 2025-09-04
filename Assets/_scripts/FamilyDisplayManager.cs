using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the display of family members and their status in the UI
/// Works with ImperialFamilySystem to show family information
/// </summary>
public class FamilyDisplayManager : MonoBehaviour
{
    [Header("Display References")]
    public FamilyMemberDisplay[] memberDisplays;
    public GameObject familyPanel;
    
    [Header("System Reference")]
    public ImperialFamilySystem familySystem;
    
    [Header("UI References")]
    public Button viewFamilyButton;
    public TMP_Text familyEventText;
    public TMP_Text familyStatusText;
    
    [Header("Default Portraits")]
    public Sprite officerPortrait;        // Portrait for Emma (Imperium Officer)
    public Sprite mechanicPortrait;       // Portrait for Kira (Fighter Mechanic)
    public Sprite cadetPortrait;          // Portrait for Jace (Trooper Cadet)
    public Sprite droidPortrait;          // Portrait for R2-D4 (Family Droid)
    
    // Start is called before the first frame update
    void Start()
    {
        // Find family system if not assigned
        if (familySystem == null)
            familySystem = FindFirstObjectByType<ImperialFamilySystem>();
            
        if (familySystem == null)
        {
            Debug.LogError("FamilyDisplayManager: No ImperialFamilySystem found!");
            return;
        }
        
        // Set up button for viewing family
        if (viewFamilyButton != null)
            viewFamilyButton.onClick.AddListener(ToggleFamilyPanel);
            
        // Initialize displays
        InitializeDisplays();
        
        // Hide panel initially
        if (familyPanel != null)
            familyPanel.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        // Update display status
        if (familySystem != null && familySystem.familyMembers != null)
        {
            UpdateDisplayStatus();
        }
        
        // Update family event text if it exists
        if (familyEventText != null && familySystem != null && 
            familySystem.familyEventText != null && 
            familySystem.familyEventText.text != familyEventText.text)
        {
            familyEventText.text = familySystem.familyEventText.text;
        }
    }
    
    /// <summary>
    /// Initialize all family member displays
    /// </summary>
    void InitializeDisplays()
    {
        if (familySystem == null || familySystem.familyMembers == null)
            return;
            
        // Create member displays if they don't exist
        if (memberDisplays == null || memberDisplays.Length < familySystem.familyMembers.Length)
        {
            // Look for existing displays in the hierarchy
            Transform displayContainer = transform.Find("MemberDisplays");
            
            if (displayContainer == null)
            {
                // Create a container if it doesn't exist
                GameObject containerObj = new GameObject("MemberDisplays");
                containerObj.transform.SetParent(transform, false);
                displayContainer = containerObj.transform;
            }
            
            // Find existing display components
            List<FamilyMemberDisplay> existingDisplays = new List<FamilyMemberDisplay>();
            foreach (Transform child in displayContainer)
            {
                FamilyMemberDisplay display = child.GetComponent<FamilyMemberDisplay>();
                if (display != null)
                    existingDisplays.Add(display);
            }
            
            // Create new display components as needed
            while (existingDisplays.Count < familySystem.familyMembers.Length)
            {
                GameObject displayObj = new GameObject("MemberDisplay_" + existingDisplays.Count);
                displayObj.transform.SetParent(displayContainer, false);
                
                FamilyMemberDisplay display = displayObj.AddComponent<FamilyMemberDisplay>();
                existingDisplays.Add(display);
                
                // Add required UI components if missing
                if (displayObj.GetComponentInChildren<Image>() == null)
                {
                    GameObject portraitObj = new GameObject("Portrait");
                    portraitObj.transform.SetParent(displayObj.transform, false);
                    portraitObj.AddComponent<Image>();
                }
                
                if (displayObj.GetComponentInChildren<TMP_Text>() == null)
                {
                    GameObject nameObj = new GameObject("Name");
                    nameObj.transform.SetParent(displayObj.transform, false);
                    nameObj.AddComponent<TMP_Text>();
                }
            }
            
            // Set the display array
            memberDisplays = existingDisplays.ToArray();
        }
        
        // Set up each display
        for (int i = 0; i < familySystem.familyMembers.Length; i++)
        {
            if (i < memberDisplays.Length && memberDisplays[i] != null)
            {
                var member = familySystem.familyMembers[i];
                
                // Get appropriate portrait if member doesn't have one
                Sprite portrait = member.portrait;
                if (portrait == null)
                {
                    portrait = GetDefaultPortrait(member.occupation);
                    
                    // Update member portrait for future reference
                    if (portrait != null)
                        familySystem.familyMembers[i].portrait = portrait;
                }
                
                // Set up display
                memberDisplays[i].Setup(
                    member.name,
                    member.occupation,
                    portrait
                );
            }
        }
        
        // Update status
        UpdateDisplayStatus();
    }
    
    /// <summary>
    /// Update the status of all displays
    /// </summary>
    public void UpdateDisplayStatus()
    {
        if (familySystem == null || familySystem.familyMembers == null)
            return;
            
        int count = Mathf.Min(memberDisplays.Length, familySystem.familyMembers.Length);
        for (int i = 0; i < count; i++)
        {
            if (memberDisplays[i] != null)
            {
                var member = familySystem.familyMembers[i];
                memberDisplays[i].UpdateStatus(
                    member.needsMedicalCare,
                    member.needsEquipment,
                    member.needsTraining
                );
            }
        }
        
        // Update overall family status text if available
        if (familyStatusText != null)
        {
            FamilyStatusInfo status = familySystem.GetFamilyStatusInfo();
            familyStatusText.text = status.GetStatusSummary();
        }
    }
    
    /// <summary>
    /// Set a family member as missing or present
    /// </summary>
    public void SetFamilyMemberMissing(string memberName, bool isMissing)
    {
        foreach (var display in memberDisplays)
        {
            if (display != null && display.nameText != null && 
                display.nameText.text.StartsWith(memberName))
            {
                display.SetMissing(isMissing);
                break;
            }
        }
    }
    
    /// <summary>
    /// Toggle the family panel visibility
    /// </summary>
    public void ToggleFamilyPanel()
    {
        if (familyPanel != null)
        {
            bool newState = !familyPanel.activeSelf;
            familyPanel.SetActive(newState);
            
            // Also call original system methods to keep everything in sync
            if (familySystem != null)
            {
                if (newState)
                    familySystem.ShowFamilyPanel();
                else
                    familySystem.HideFamilyPanel();
            }
        }
    }
    
    /// <summary>
    /// Show the family panel
    /// </summary>
    public void ShowFamilyPanel()
    {
        if (familyPanel != null)
        {
            familyPanel.SetActive(true);
            
            if (familySystem != null)
                familySystem.ShowFamilyPanel();
        }
    }
    
    /// <summary>
    /// Hide the family panel
    /// </summary>
    public void HideFamilyPanel()
    {
        if (familyPanel != null)
        {
            familyPanel.SetActive(false);
            
            if (familySystem != null)
                familySystem.HideFamilyPanel();
        }
    }
    
    /// <summary>
    /// Refresh all displays (call after major changes to family members)
    /// </summary>
    public void RefreshDisplays()
    {
        InitializeDisplays();
    }
    
    /// <summary>
    /// Get the default portrait based on occupation
    /// </summary>
    private Sprite GetDefaultPortrait(string occupation)
    {
        switch (occupation.ToLower())
        {
            case "imperial officer":
                return officerPortrait;
                
            case "fighter mechanic":
                return mechanicPortrait;
                
            case "trooper cadet":
                return cadetPortrait;
                
            case "family droid":
                return droidPortrait;
                
            default:
                return null;
        }
    }
}