using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Applies a holographic effect to a UI image
/// </summary>
[RequireComponent(typeof(Image))]
public class HologramEffect : MonoBehaviour
{
    [Header("Hologram Settings")]
    [Range(0f, 1f)]
    public float scanLinesDensity = 0.5f;
    [Range(0f, 1f)]
    public float scanLinesIntensity = 0.2f;
    [Range(0f, 1f)]
    public float glitchAmount = 0.1f;
    [Range(0f, 5f)]
    public float flickerSpeed = 1.0f;
    [Range(0f, 1f)]
    public float flickerAmount = 0.1f;
    public Color hologramTint = new Color(0.5f, 0.8f, 1.0f, 0.8f);
    
    // The material with our hologram shader
    private Material hologramMaterial;
    // Reference to the image we're applying the effect to
    private Image targetImage;
    
    private float glitchTimer = 0f;
    private float flickerTimer = 0f;
    
    void Start()
    {
        // Get the target image
        targetImage = GetComponent<Image>();
        if (targetImage == null)
        {
            Debug.LogWarning("HologramEffect: No Image component found!");
            enabled = false;
            return;
        }
        
        // Create a new material with the default UI shader
        hologramMaterial = new Material(Shader.Find("UI/Default"));
        
        // Apply hologram effect properties
        UpdateMaterial();
        
        // Apply material to the image
        targetImage.material = hologramMaterial;
    }
    
    void Update()
    {
        if (targetImage == null || hologramMaterial == null)
            return;
            
        // Update timers for effects
        flickerTimer += Time.deltaTime * flickerSpeed;
        glitchTimer += Time.deltaTime;
        
        // Apply flicker effect
        float flickerValue = 1.0f - (Mathf.Sin(flickerTimer) * flickerAmount * 0.5f);
        Color currentColor = hologramTint * flickerValue;
        currentColor.a = hologramTint.a;
        targetImage.color = currentColor;
        
        // Random glitch effect
        if (Random.value < glitchAmount * 0.01f)
        {
            StartCoroutine(GlitchEffect());
        }
    }
    
    // Update the hologram material properties
    void UpdateMaterial()
    {
        if (hologramMaterial == null)
            return;
            
        // Since we can't create a custom shader here, we'll just adjust the color and opacity
        hologramMaterial.color = hologramTint;
        targetImage.color = hologramTint;
    }
    
    // Create a glitch effect
    IEnumerator GlitchEffect()
    {
        if (targetImage == null) yield break;
        
        RectTransform rect = targetImage.rectTransform;
        Vector3 originalPosition = rect.localPosition;
        
        // Create glitch by shifting position briefly
        for (int i = 0; i < 3; i++)
        {
            // Random offset based on glitch amount
            float offsetX = Random.Range(-5f, 5f) * glitchAmount;
            rect.localPosition = originalPosition + new Vector3(offsetX, 0, 0);
            
            // Brief wait
            yield return new WaitForSeconds(0.05f);
        }
        
        // Reset position
        rect.localPosition = originalPosition;
    }
    
    void OnDestroy()
    {
        // Clean up material
        if (hologramMaterial != null)
        {
            Destroy(hologramMaterial);
        }
    }
}
