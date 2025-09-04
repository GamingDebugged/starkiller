using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles the display and visual effects for an individual family member
/// </summary>
public class FamilyMemberDisplay : MonoBehaviour
{
    [Header("UI Components")]
    public Image portraitImage;
    public TMP_Text nameText;
    public Image statusBackground;
    
    [Header("Status Icons")]
    public GameObject medicalIcon;
    public GameObject equipmentIcon;
    public GameObject protectionIcon;
    
    [Header("Visual Effects")]
    public bool useHologramEffect = true;
    public Color hologramColor = new Color(0.5f, 0.8f, 1.0f, 0.9f);
    
    private bool isMissing = false;
    
    void Start()
    {
        // Initialize components if not assigned
        if (portraitImage == null)
            portraitImage = transform.Find("Portrait")?.GetComponent<Image>();
            
        if (nameText == null)
            nameText = transform.Find("Name")?.GetComponent<TMP_Text>();
            
        if (statusBackground == null)
            statusBackground = GetComponent<Image>();
            
        // Initialize status
        if (medicalIcon) medicalIcon.SetActive(false);
        if (equipmentIcon) equipmentIcon.SetActive(false);
        if (protectionIcon) protectionIcon.SetActive(false);
    }
    
    /// <summary>
    /// Set up the family member display with basic information
    /// </summary>
    public void Setup(string memberName, string occupation, Sprite portrait)
    {
        // Set name with occupation if provided
        if (nameText != null)
        {
            nameText.text = !string.IsNullOrEmpty(occupation) ? 
                $"{memberName} ({occupation})" : memberName;
        }
        
        // Set portrait if available
        if (portraitImage != null && portrait != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.color = Color.white;
        }
    }
    
    /// <summary>
    /// Update the status indicators for this family member
    /// </summary>
    public void UpdateStatus(bool needsMedical, bool needsEquipment, bool needsProtection)
    {
        if (medicalIcon) medicalIcon.SetActive(needsMedical);
        if (equipmentIcon) equipmentIcon.SetActive(needsEquipment);
        if (protectionIcon) protectionIcon.SetActive(needsProtection);
    }
    
    /// <summary>
    /// Add scanline holographic effect to this display
    /// </summary>
    public void AddScanlineEffect()
    {
        // Check if we already have the effect
        ImperialScanLines existingScanLines = GetComponentInChildren<ImperialScanLines>();
        if (existingScanLines != null)
            return;
            
        // Create a new GameObject for the scan lines
        GameObject scanlineObj = new GameObject("ScanLines");
        scanlineObj.transform.SetParent(transform, false);
        
        // Add the scanline component
        ImperialScanLines scanLines = scanlineObj.AddComponent<ImperialScanLines>();
        
        // Setup scanline effect properties
        if (portraitImage != null)
        {
            // Copy the rect transform values
            RectTransform scanRT = scanlineObj.AddComponent<RectTransform>();
            RectTransform portraitRT = portraitImage.GetComponent<RectTransform>();
            
            scanRT.anchorMin = portraitRT.anchorMin;
            scanRT.anchorMax = portraitRT.anchorMax;
            scanRT.anchoredPosition = portraitRT.anchoredPosition;
            scanRT.sizeDelta = portraitRT.sizeDelta;
            
            // Add an image component for the scan lines
            Image scanImage = scanlineObj.AddComponent<Image>();
            scanImage.color = new Color(0.5f, 0.8f, 1.0f, 0.2f);
            
            // Apply holographic color to portrait if found
            portraitImage.color = new Color(0.5f, 0.8f, 1.0f, 0.9f);
        }
    }

    /// <summary>
    /// Set the family member as missing or present
    /// </summary>
    public void SetMissing(bool missing)
    {
        isMissing = missing;
        
        // Apply visual effect for missing state
        if (portraitImage != null)
        {
            portraitImage.color = missing ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : Color.white;
        }
        
        if (nameText != null)
        {
            nameText.color = missing ? new Color(0.7f, 0.7f, 0.7f) : Color.white;
            
            // Append "MISSING" text if relevant
            if (missing && !nameText.text.Contains("MISSING"))
            {
                nameText.text += " <color=#FF6060>[MISSING]</color>";
            }
            else if (!missing && nameText.text.Contains("MISSING"))
            {
                nameText.text = nameText.text.Split('[')[0].Trim();
            }
        }
    }
}