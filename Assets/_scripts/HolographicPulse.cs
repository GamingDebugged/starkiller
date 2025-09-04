using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adds a subtle pulsing effect to UI elements for holographic displays
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class HolographicPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    [Range(0.1f, 5f)]
    public float pulseSpeed = 1.0f;        // Speed of the pulse effect
    [Range(0f, 0.5f)]
    public float pulseAmount = 0.1f;       // Amount of pulsing (0-1)
    [Range(0f, 0.2f)]
    public float glitchChance = 0.01f;     // Chance of glitch per frame
    
    // The canvas group for opacity changes
    private CanvasGroup canvasGroup;
    
    // Original position for glitch effect
    private Vector3 originalPosition;
    
    // Pulse timing
    private float pulseTimer = 0f;
    
    // Whether a glitch is active
    private bool glitchActive = false;
    
    void Start()
    {
        // Get canvas group component
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Store original position
        originalPosition = transform.localPosition;
    }
    
    void Update()
    {
        if (canvasGroup == null)
            return;
            
        // Update pulse timer
        pulseTimer += Time.deltaTime * pulseSpeed;
        
        // Apply pulse effect to alpha
        float pulse = Mathf.Sin(pulseTimer) * pulseAmount;
        canvasGroup.alpha = 1f - pulseAmount + pulse;
        
        // Random glitch chance
        if (!glitchActive && Random.value < glitchChance)
        {
            StartGlitch();
        }
    }
    
    /// <summary>
    /// Start a brief glitch effect
    /// </summary>
    void StartGlitch()
    {
        if (glitchActive)
            return;
            
        glitchActive = true;
        
        // Calculate a random offset
        float offsetX = Random.Range(-5f, 5f);
        
        // Apply offset
        transform.localPosition = originalPosition + new Vector3(offsetX, 0, 0);
        
        // Reset after a short delay
        Invoke("EndGlitch", 0.05f);
    }
    
    /// <summary>
    /// End the glitch effect
    /// </summary>
    void EndGlitch()
    {
        // Reset position
        transform.localPosition = originalPosition;
        glitchActive = false;
    }
}
