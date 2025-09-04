using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// TextAnimator provides various animation effects for TextMeshPro components
/// </summary>
public class TextAnimator : MonoBehaviour
{
    [Header("Component Reference")]
    public TMP_Text textComponent;
    
    [Header("Animation Settings")]
    public AnimationType animationType = AnimationType.TypeWriter;
    public float animationDuration = 1.0f;
    public float delayBetweenCharacters = 0.05f;
    
    [Header("Direction Settings (for Reveal animations)")]
    public RevealDirection revealDirection = RevealDirection.LeftToRight;
    
    [Header("Scale Settings (for Scale animations)")]
    public float startScale = 3.0f;
    public float endScale = 1.0f;
    
    [Header("Number Counter Settings")]
    public bool useEasing = true;
    public EasingType easingType = EasingType.EaseOutExpo;
    
    // Animation states
    private Coroutine activeAnimation;
    
    // Cached values
    private string originalText;
    private float originalFontSize;
    
    // Animation types
    public enum AnimationType
    {
        TypeWriter,
        RevealDirection,
        ScaleEffect,
        NumberCounter
    }
    
    // Direction for reveal animations
    public enum RevealDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }
    
    // Easing types for number counter
    public enum EasingType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo
    }

    void Start()
    {
        // Get the TextMeshPro component if not assigned
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
        
        if (textComponent == null)
        {
            Debug.LogError("TextAnimator: No TextMeshPro component found!");
            return;
        }
        
        // Cache original values
        originalText = textComponent.text;
        originalFontSize = textComponent.fontSize;
    }
    
    /// <summary>
    /// Start the animation with the current text
    /// </summary>
    public void Animate()
    {
        if (textComponent == null) return;
        
        // Cache the current text
        originalText = textComponent.text;
        
        // Stop any running animation
        StopAnimation();
        
        // Start the appropriate animation
        switch (animationType)
        {
            case AnimationType.TypeWriter:
                activeAnimation = StartCoroutine(TypeWriterAnimation());
                break;
                
            case AnimationType.RevealDirection:
                activeAnimation = StartCoroutine(RevealAnimation());
                break;
                
            case AnimationType.ScaleEffect:
                activeAnimation = StartCoroutine(ScaleAnimation());
                break;
                
            case AnimationType.NumberCounter:
                // Try to parse the text as a number
                if (float.TryParse(originalText, out float targetNumber))
                {
                    activeAnimation = StartCoroutine(NumberCounterAnimation(targetNumber));
                }
                else
                {
                    Debug.LogWarning("TextAnimator: Cannot use NumberCounter on non-numeric text!");
                    textComponent.text = originalText;
                }
                break;
        }
    }
    
    /// <summary>
    /// Start the animation with a specific text
    /// </summary>
    public void Animate(string text)
    {
        if (textComponent == null) return;
        
        // Set and cache the new text
        textComponent.text = text;
        originalText = text;
        
        // Start animation
        Animate();
    }
    
    /// <summary>
    /// Start a number counter animation to a target value
    /// </summary>
    public void AnimateNumber(float targetNumber, string format = "0")
    {
        if (textComponent == null) return;
        
        // Stop any running animation
        StopAnimation();
        
        // Start the number counter animation
        activeAnimation = StartCoroutine(NumberCounterAnimation(targetNumber, format));
    }
    
    /// <summary>
    /// Stop the current animation
    /// </summary>
    public void StopAnimation()
    {
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
            activeAnimation = null;
        }
        
        // Reset to original text
        if (textComponent != null)
            textComponent.text = originalText;
    }
    
    /// <summary>
    /// Complete the current animation immediately
    /// </summary>
    public void CompleteAnimation()
    {
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
            activeAnimation = null;
        }
        
        if (textComponent != null)
            textComponent.text = originalText;
    }
    
    #region Animation Coroutines
    
    /// <summary>
    /// Type writer animation - reveals text one character at a time
    /// </summary>
    private IEnumerator TypeWriterAnimation()
    {
        // Hide all characters initially
        textComponent.maxVisibleCharacters = 0;
        
        // Slowly reveal characters
        int totalCharacters = originalText.Length;
        
        for (int i = 0; i <= totalCharacters; i++)
        {
            textComponent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delayBetweenCharacters);
        }
        
        // Ensure all text is visible at the end
        textComponent.maxVisibleCharacters = totalCharacters;
    }
    
    /// <summary>
    /// Reveal animation - reveals text from a specific direction
    /// </summary>
    private IEnumerator RevealAnimation()
    {
        // Ensure text is visible but create a mask for characters
        textComponent.text = originalText;
        
        // Force text to update
        textComponent.ForceMeshUpdate();
        
        // Get text info
        TMP_TextInfo textInfo = textComponent.textInfo;
        
        // Create a copy of mesh colors
        Color32[][] originalColors = new Color32[textInfo.meshInfo.Length][];
        
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            // Get color from mesh info
            Color32[] newColors = textInfo.meshInfo[i].colors32;
            originalColors[i] = new Color32[newColors.Length];
            System.Array.Copy(newColors, originalColors[i], newColors.Length);
            
            // Set all colors to transparent
            for (int j = 0; j < newColors.Length; j++)
            {
                newColors[j].a = 0;
            }
        }
        
        // Animation loop
        float startTime = Time.time;
        float endTime = startTime + animationDuration;
        
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / animationDuration;
            
            // Get character positions based on direction
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                // Skip if character is whitespace
                if (!textInfo.characterInfo[i].isVisible) continue;
                
                int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                
                // Get character position based on direction
                float characterProgress = 0;
                
                switch (revealDirection)
                {
                    case RevealDirection.LeftToRight:
                        characterProgress = (float)i / textInfo.characterCount;
                        break;
                    
                    case RevealDirection.RightToLeft:
                        characterProgress = 1 - ((float)i / textInfo.characterCount);
                        break;
                    
                    case RevealDirection.TopToBottom:
                        // Y position normalized from top to bottom
                        float charTopY = textInfo.characterInfo[i].topLeft.y;
                        float textTopY = textInfo.meshInfo[meshIndex].vertices[0].y;
                        float textBottomY = textInfo.meshInfo[meshIndex].vertices[textInfo.meshInfo[meshIndex].vertices.Length - 1].y;
                        characterProgress = 1 - ((charTopY - textBottomY) / (textTopY - textBottomY));
                        break;
                    
                    case RevealDirection.BottomToTop:
                        // Y position normalized from bottom to top
                        float charBottomY = textInfo.characterInfo[i].bottomLeft.y;
                        float textTopY2 = textInfo.meshInfo[meshIndex].vertices[0].y;
                        float textBottomY2 = textInfo.meshInfo[meshIndex].vertices[textInfo.meshInfo[meshIndex].vertices.Length - 1].y;
                        characterProgress = (charBottomY - textBottomY2) / (textTopY2 - textBottomY2);
                        break;
                }
                
                // Calculate alpha based on progress and animation time
                float alpha = Mathf.Clamp01((t - characterProgress) * 5.0f);
                
                // Apply alpha to vertices
                for (int j = 0; j < 4; j++)
                {
                    int colorIndex = vertexIndex + j;
                    
                    // Create new color with original RGB but animated alpha
                    textInfo.meshInfo[meshIndex].colors32[colorIndex].a = (byte)(originalColors[meshIndex][colorIndex].a * alpha);
                }
            }
            
            // Update the mesh
            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            
            yield return null;
        }
        
        // Ensure all text is fully visible at the end
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            Color32[] colors = textInfo.meshInfo[i].colors32;
            
            for (int j = 0; j < colors.Length; j++)
            {
                colors[j].a = originalColors[i][j].a;
            }
        }
        
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
    
    /// <summary>
    /// Scale animation - text scales from startScale to endScale
    /// </summary>
    private IEnumerator ScaleAnimation()
    {
        // Store original font size
        float originalSize = textComponent.fontSize;
        
        // Set to start scale
        textComponent.text = originalText;
        textComponent.fontSize = originalSize * startScale;
        
        // Animation loop
        float startTime = Time.time;
        float endTime = startTime + animationDuration;
        
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / animationDuration;
            
            // Apply easing to make it more natural
            float easedT = EaseOutBack(t);
            
            // Linear interpolation between start and end scale
            float currentScale = Mathf.Lerp(startScale, endScale, easedT);
            
            // Apply scale to font size
            textComponent.fontSize = originalSize * currentScale;
            
            yield return null;
        }
        
        // Ensure it ends at exactly the right scale
        textComponent.fontSize = originalSize * endScale;
    }
    
    /// <summary>
    /// Number counter animation - counts from 0 to target number
    /// </summary>
    private IEnumerator NumberCounterAnimation(float targetNumber, string format = "0")
    {
        // Start at zero
        float startNumber = 0f;
        float currentNumber = startNumber;
        
        // Animation loop
        float startTime = Time.time;
        float endTime = startTime + animationDuration;
        
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / animationDuration;
            
            // Apply easing if enabled
            if (useEasing)
                t = ApplyEasing(t);
            
            // Interpolate between start and target number
            currentNumber = Mathf.Lerp(startNumber, targetNumber, t);
            
            // Update text with current number
            textComponent.text = currentNumber.ToString(format);
            
            yield return null;
        }
        
        // Ensure it ends at exactly the target number
        textComponent.text = targetNumber.ToString(format);
    }
    
    #endregion
    
    #region Easing Functions
    
    // Easing function for scale animation
    private float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;
        
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }
    
    // Apply selected easing function
    private float ApplyEasing(float t)
    {
        switch (easingType)
        {
            case EasingType.Linear:
                return t;
            case EasingType.EaseInQuad:
                return t * t;
            case EasingType.EaseOutQuad:
                return t * (2 - t);
            case EasingType.EaseInOutQuad:
                return t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t;
            case EasingType.EaseInCubic:
                return t * t * t;
            case EasingType.EaseOutCubic:
                return (--t) * t * t + 1;
            case EasingType.EaseInOutCubic:
                return t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
            case EasingType.EaseInExpo:
                return t == 0 ? 0 : Mathf.Pow(2, 10 * (t - 1));
            case EasingType.EaseOutExpo:
                return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
            case EasingType.EaseInOutExpo:
                if (t == 0) return 0;
                if (t == 1) return 1;
                return t < 0.5 ?
                    Mathf.Pow(2, 20 * t - 10) / 2 :
                    (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
            default:
                return t;
        }
    }
    
    #endregion
}