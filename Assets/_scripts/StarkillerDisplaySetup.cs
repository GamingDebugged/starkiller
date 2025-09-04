using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Helper for setting up family member display UI in Starkiller Base Command
/// </summary>
public class StarkillerDisplaySetup : MonoBehaviour
{
    [Header("Imperial Family UI")]
    public GameObject familyPanel;
    public Transform familyMemberContainer;
    public GameObject familyMemberPrefab;
    public TMPro.TMP_Text familyEventText;
    
    [Header("Default Portraits")]
    public Sprite scientistPortrait;
    public Sprite mechanicPortrait;
    public Sprite stormtrooperPortrait;
    public Sprite babyPortrait;
    public Sprite droidPortrait;
    
    private FamilyDisplayManager displayManager;
    private ImperialFamilySystem familySystem;
    
    void OnEnable()
    {
        // Find references
        displayManager = GetComponent<FamilyDisplayManager>();
        familySystem = FindFirstObjectByType<ImperialFamilySystem>();
        
        if (displayManager == null)
        {
            displayManager = gameObject.AddComponent<FamilyDisplayManager>();
            Debug.Log("Added FamilyDisplayManager component");
        }
        
        if (familySystem == null)
        {
            Debug.LogError("StarkillerDisplaySetup: ImperialFamilySystem not found!");
            return;
        }
        
        // Set up the display manager
        ConfigureDisplayManager();
    }
    
    /// <summary>
    /// Set up the display manager with references
    /// </summary>
    void ConfigureDisplayManager()
    {
        if (displayManager == null) return;
        
        // Set references
        displayManager.familyPanel = familyPanel;
        displayManager.familySystem = familySystem;
        displayManager.familyEventText = familyEventText;
        
        // Create displays if needed
        if (familyMemberContainer != null && familyMemberPrefab != null &&
            (familyMemberContainer.childCount == 0 || displayManager.memberDisplays == null || displayManager.memberDisplays.Length == 0))
        {
            GenerateFamilyMemberDisplays();
        }
        else if (familyMemberContainer != null && familyMemberContainer.childCount > 0)
        {
            // Use existing display objects
            ConfigureExistingDisplays();
        }
    }
    
    /// <summary>
    /// Create display objects for all family members
    /// </summary>
    void GenerateFamilyMemberDisplays()
    {
        if (familySystem == null || familySystem.familyMembers == null || 
            familyMemberContainer == null || familyMemberPrefab == null)
            return;
            
        // Clear any existing children
        foreach (Transform child in familyMemberContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create a display for each family member
        List<FamilyMemberDisplay> displays = new List<FamilyMemberDisplay>();
        foreach (var member in familySystem.familyMembers)
        {
            // Create display object
            GameObject obj = Instantiate(familyMemberPrefab, familyMemberContainer);
            obj.name = "Display_" + member.name;
            
            // Get or add display component
            FamilyMemberDisplay display = obj.GetComponent<FamilyMemberDisplay>();
            if (display == null)
                display = obj.AddComponent<FamilyMemberDisplay>();
                
            // Set up portrait based on occupation
            Sprite portrait = GetPortraitByOccupation(member.occupation);
            if (portrait != null && member.portrait == null)
            {
                member.portrait = portrait; // Assign to family member
            }
            
            // Add to list
            displays.Add(display);
        }
        
        // Assign the array of displays to the manager
        displayManager.memberDisplays = displays.ToArray();
    }
    
    /// <summary>
    /// Set up existing display objects with FamilyMemberDisplay components
    /// </summary>
    void ConfigureExistingDisplays()
    {
        if (familyMemberContainer == null)
            return;
            
        List<FamilyMemberDisplay> displays = new List<FamilyMemberDisplay>();
        
        foreach (Transform child in familyMemberContainer)
        {
            // Get or add display component
            FamilyMemberDisplay display = child.GetComponent<FamilyMemberDisplay>();
            if (display == null)
                display = child.gameObject.AddComponent<FamilyMemberDisplay>();
                
            // Find UI elements
            display.portraitImage = FindChildComponent<Image>(child.gameObject, "Portrait");
            display.nameText = FindChildComponent<TMP_Text>(child.gameObject, "Name");
            display.statusBackground = FindChildComponent<Image>(child.gameObject, "Background");
            
            // Find status icons
            display.medicalIcon = FindChildWithName(child.gameObject, "Medical");
            display.equipmentIcon = FindChildWithName(child.gameObject, "Equipment");
            display.protectionIcon = FindChildWithName(child.gameObject, "Protection");
            
            // Add to list
            displays.Add(display);
        }
        
        // Assign the array of displays to the manager
        displayManager.memberDisplays = displays.ToArray();
    }
    
    /// <summary>
    /// Get an appropriate portrait sprite based on occupation
    /// </summary>
    Sprite GetPortraitByOccupation(string occupation)
    {
        switch (occupation.ToLower())
        {
            case "imperial scientist":
                return scientistPortrait;
            case "fighter mechanic":
                return mechanicPortrait;
            case "stormtrooper":
                return stormtrooperPortrait;
            case "baby":
                return babyPortrait;
            case "family droid":
                return droidPortrait;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// Find a component in children with name containing the search term
    /// </summary>
    T FindChildComponent<T>(GameObject parent, string nameContains) where T : Component
    {
        // First try direct children
        foreach (T component in parent.GetComponentsInChildren<T>())
        {
            if (component.gameObject.name.Contains(nameContains))
                return component;
        }
        
        // If not found, return the first one if it exists
        T[] components = parent.GetComponentsInChildren<T>();
        if (components.Length > 0)
            return components[0];
            
        return null;
    }
    
    /// <summary>
    /// Find a child GameObject with name containing the search term
    /// </summary>
    GameObject FindChildWithName(GameObject parent, string nameContains)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.gameObject != parent && child.name.Contains(nameContains))
                return child.gameObject;
        }
        
        return null;
    }
}
