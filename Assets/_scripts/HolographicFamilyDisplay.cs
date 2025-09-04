using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the holographic displays for all family members
/// </summary>
public class HolographicFamilyDisplay : MonoBehaviour
{
    [Header("Hologram References")]
    public GameObject hologramPrefab;           // Prefab for individual holograms
    public Transform hologramContainer;         // Container for instantiated holograms
    public GameObject missingHologramPrefab;    // Prefab for empty/missing hologram slots
    
    [Header("Default Portraits")]
    public Sprite scientistPortrait;            // Portrait for Lira (Imperium Scientist)
    public Sprite striketrooperPortrait;         // Portrait for Jace (Striketrooper)
    public Sprite mechanicPortrait;             // Portrait for Kira (Fighter Mechanic)
    public Sprite babyPortrait;                 // Portrait for Nova (Baby)
    public Sprite droidPortrait;                // Portrait for R2-M4 (Family Droid)
    
    [Header("System References")]
    public ImperialFamilySystem familySystem;   // Reference to the family system
    
    // List of active hologram displays
    private List<FamilyMemberHologram> holograms = new List<FamilyMemberHologram>();
    
    void Start()
    {
        // Find family system if not assigned
        if (familySystem == null)
            familySystem = FindFirstObjectByType<ImperialFamilySystem>();
            
        if (familySystem == null)
        {
            Debug.LogError("HolographicFamilyDisplay: ImperialFamilySystem not found!");
            return;
        }
        
        // Initialize displays
        InitializeHolograms();
    }
    
    // Create hologram displays for each family member
    public void InitializeHolograms()
    {
        // Clear any existing holograms
        ClearHolograms();
        
        if (familySystem == null || hologramPrefab == null || hologramContainer == null)
            return;
            
        // Get family members from system
        if (familySystem.familyMembers == null || familySystem.familyMembers.Length == 0)
            return;
            
        // Create holograms for each family member
        foreach (var member in familySystem.familyMembers)
        {
            // Create hologram instance
            GameObject hologramObj = Instantiate(hologramPrefab, hologramContainer);
            FamilyMemberHologram hologram = hologramObj.GetComponent<FamilyMemberHologram>();
            
            if (hologram != null)
            {
                // Get appropriate portrait
                Sprite portrait = GetPortraitForMember(member.name, member.occupation);
                
                // Initialize hologram
                hologram.Initialize(
                    member.name, 
                    member.occupation, 
                    portrait != null ? portrait : member.portrait, 
                    member.needsMedicalCare, 
                    member.needsEquipment, 
                    member.needsTraining
                );
                
                // Add to list
                holograms.Add(hologram);
            }
        }
    }
    
    // Update hologram displays with current family status
    public void UpdateHolograms()
    {
        if (familySystem == null || familySystem.familyMembers == null)
            return;
            
        // Match hologram count to family member count
        EnsureHologramCount();
        
        // Update each hologram
        for (int i = 0; i < familySystem.familyMembers.Length && i < holograms.Count; i++)
        {
            var member = familySystem.familyMembers[i];
            var hologram = holograms[i];
            
            // Update status flags
            hologram.needsMedicalCare = member.needsMedicalCare;
            hologram.needsEquipment = member.needsEquipment;
            hologram.needsProtection = member.needsTraining;
            
            // Update status and visual state
            hologram.UpdateStatus();
            hologram.UpdateVisualState();
        }
    }
    
    // Make sure hologram count matches family member count
    void EnsureHologramCount()
    {
        if (familySystem == null || familySystem.familyMembers == null)
            return;
            
        int familyCount = familySystem.familyMembers.Length;
        
        // Add more holograms if needed
        while (holograms.Count < familyCount)
        {
            if (hologramPrefab == null || hologramContainer == null)
                break;
                
            GameObject hologramObj = Instantiate(hologramPrefab, hologramContainer);
            FamilyMemberHologram hologram = hologramObj.GetComponent<FamilyMemberHologram>();
            
            if (hologram != null)
            {
                var member = familySystem.familyMembers[holograms.Count];
                Sprite portrait = GetPortraitForMember(member.name, member.occupation);
                
                hologram.Initialize(
                    member.name, 
                    member.occupation, 
                    portrait != null ? portrait : member.portrait, 
                    member.needsMedicalCare, 
                    member.needsEquipment, 
                    member.needsTraining
                );
                
                holograms.Add(hologram);
            }
        }
        
        // Remove excess holograms
        while (holograms.Count > familyCount)
        {
            int lastIndex = holograms.Count - 1;
            if (holograms[lastIndex] != null)
            {
                Destroy(holograms[lastIndex].gameObject);
            }
            holograms.RemoveAt(lastIndex);
        }
    }
    
    // Clear all hologram instances
    void ClearHolograms()
    {
        foreach (var hologram in holograms)
        {
            if (hologram != null && hologram.gameObject != null)
            {
                Destroy(hologram.gameObject);
            }
        }
        
        holograms.Clear();
        
        // Also clear any direct children of the container
        if (hologramContainer != null)
        {
            foreach (Transform child in hologramContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    // Get appropriate portrait based on name and occupation
    Sprite GetPortraitForMember(string name, string occupation)
    {
        switch (occupation)
        {
            case "Imperium Scientist":
                return scientistPortrait;
                
            case "Striketrooper":
                return striketrooperPortrait;
                
            case "Fighter Mechanic":
                return mechanicPortrait;
                
            case "Baby":
                return babyPortrait;
                
            case "Family Droid":
                return droidPortrait;
                
            default:
                return null;
        }
    }
    
    // Set a family member as missing or deceased
    public void SetFamilyMemberStatus(string memberName, FamilyMemberHologram.MemberStatus status)
    {
        foreach (var hologram in holograms)
        {
            if (hologram.nameText != null && hologram.nameText.text.StartsWith(memberName))
            {
                hologram.currentStatus = status;
                hologram.UpdateVisualState();
                return;
            }
        }
    }
}
