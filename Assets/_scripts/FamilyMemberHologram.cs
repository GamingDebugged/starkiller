using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the holographic display for a single family member
/// </summary>
public class FamilyMemberHologram : MonoBehaviour
{
    [Header("UI References")]
    public Image portraitImage;                    // The main portrait image
    public Image hologramBaseImage;                // The projector base
    public Image statusOverlayImage;               // Colored overlay for status effects
    public GameObject medicalIcon;                 // Icon for medical needs
    public GameObject equipmentIcon;               // Icon for equipment needs  
    public GameObject protectionIcon;              // Icon for protection needs
    public TMP_Text nameText;                      // Text for name and occupation
    
    [Header("Hologram Settings")]
    public float pulseSpeed = 1.0f;                // Speed of hologram pulse effect
    public float pulseAmount = 0.1f;               // Amount of pulse effect (0-1)
    public Color contentColor = new Color(0.5f, 0.8f, 1.0f, 0.8f);  // Default hologram color
    public Color strugglingColor = new Color(1.0f, 0.8f, 0.5f, 0.8f); // Color when struggling
    public Color injuredColor = new Color(1.0f, 0.4f, 0.4f, 0.8f);  // Color when injured/ill
    public Color missingColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);  // Color when missing/deceased
    
    [Header("Status")]
    public MemberStatus currentStatus = MemberStatus.Content;
    public bool needsMedicalCare = false;
    public bool needsEquipment = false;
    public bool needsProtection = false;
    
    // Internal variables
    private float pulseTimer = 0f;
    private CanvasGroup canvasGroup;
    private Material hologramMaterial;
    
    // Enums for family member status
    public enum MemberStatus
    {
        Happy,
        Content,
        Struggling,
        Injured,
        Missing
    }
    
    void Awake()
    {
        // Get the canvas group or add one if it doesn't exist
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        // Set up initial state
        UpdateVisualState();
    }
    
    void Update()
    {
        // Apply subtle pulse effect
        if (currentStatus != MemberStatus.Missing)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float pulseValue = Mathf.Sin(pulseTimer) * pulseAmount + (1f - pulseAmount);
            canvasGroup.alpha = pulseValue;
            
            // Add occasional glitch effect for struggling or injured
            if ((currentStatus == MemberStatus.Struggling || currentStatus == MemberStatus.Injured) 
                && Random.value < 0.01f)
            {
                StartCoroutine(GlitchEffect());
            }
        }
    }
    
    // Initialize with family member data
    public void Initialize(string name, string occupation, Sprite portrait, 
                          bool medical, bool equipment, bool protection)
    {
        // Set name and occupation
        if (nameText != null)
            nameText.text = name + "\n(" + occupation + ")";
            
        // Set portrait
        if (portraitImage != null && portrait != null)
            portraitImage.sprite = portrait;
            
        // Set status flags
        needsMedicalCare = medical;
        needsEquipment = equipment;
        needsProtection = protection;
        
        // Determine overall status
        UpdateStatus();
        
        // Update visual elements
        UpdateVisualState();
    }
    
    // Update the visual state based on current status
    public void UpdateVisualState()
    {
        // Show/hide status icons
        if (medicalIcon != null)
            medicalIcon.SetActive(needsMedicalCare);
            
        if (equipmentIcon != null)
            equipmentIcon.SetActive(needsEquipment);
            
        if (protectionIcon != null)
            protectionIcon.SetActive(needsProtection);
            
        // Apply color based on status
        Color statusColor = contentColor;
        
        switch (currentStatus)
        {
            case MemberStatus.Happy:
                statusColor = contentColor;
                break;
                
            case MemberStatus.Content:
                statusColor = contentColor;
                break;
                
            case MemberStatus.Struggling:
                statusColor = strugglingColor;
                break;
                
            case MemberStatus.Injured:
                statusColor = injuredColor;
                break;
                
            case MemberStatus.Missing:
                statusColor = missingColor;
                // Stop pulsing for missing
                canvasGroup.alpha = 0.5f;
                break;
        }
        
        // Apply color to overlay if available
        if (statusOverlayImage != null)
            statusOverlayImage.color = statusColor;
            
        // Adjust portrait color
        if (portraitImage != null)
            portraitImage.color = statusColor;
    }
    
    // Update status based on needs
    public void UpdateStatus()
    {
        if (needsMedicalCare)
        {
            currentStatus = MemberStatus.Injured;
        }
        else if (needsProtection)
        {
            currentStatus = MemberStatus.Struggling;
        }
        else if (needsEquipment)
        {
            currentStatus = MemberStatus.Struggling;
        }
        else
        {
            currentStatus = MemberStatus.Content;
        }
    }
    
    // Create a glitch effect
    IEnumerator GlitchEffect()
    {
        if (portraitImage == null) yield break;
        
        // Store original position
        Vector3 originalPosition = portraitImage.rectTransform.localPosition;
        
        // Apply random offsets a few times
        for (int i = 0; i < 3; i++)
        {
            // Random offset
            float offsetX = Random.Range(-5f, 5f);
            portraitImage.rectTransform.localPosition = originalPosition + new Vector3(offsetX, 0, 0);
            
            // Brief wait
            yield return new WaitForSeconds(0.05f);
        }
        
        // Reset position
        portraitImage.rectTransform.localPosition = originalPosition;
    }
}
