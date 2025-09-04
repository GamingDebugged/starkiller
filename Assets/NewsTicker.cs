using UnityEngine;
using TMPro;

/// <summary>
/// Controls a scrolling news ticker effect for UI text
/// Handles long text as a single-line horizontally scrolling element
/// </summary>
public class NewsTicker : MonoBehaviour
{
    [Header("Scrolling Settings")]
    [Tooltip("Speed at which the text scrolls in pixels per second")]
    public float scrollSpeed = 100f;
    
    [Tooltip("Starting X position (off screen to the right)")]
    public float startingXPosition = 1920f;
    
    [Tooltip("Ending X position (when to reset back to start)")]
    public float endingXPosition = -3000f;
    
    // The TMPro text component
    private TMP_Text textComponent;
    
    // RectTransform of the text
    private RectTransform rectTransform;
    
    void Start()
    {
        // Get components
        textComponent = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
        
        if (textComponent == null)
        {
            Debug.LogError("NewsTicker: No TMP_Text component found on GameObject " + gameObject.name);
            enabled = false;
            return;
        }
        
        // Configure the text component for a ticker
        ConfigureTextComponent();
        
        // Set initial position
        ResetPosition();
        
        Debug.Log("NewsTicker initialized with text: " + textComponent.text.Substring(0, Mathf.Min(20, textComponent.text.Length)) + "...");
    }
    
    void ConfigureTextComponent()
    {
        // Configure TextMeshPro settings for a ticker
        textComponent.textWrappingMode = TextWrappingModes.NoWrap; // Updated from enableWordWrapping = false
        textComponent.overflowMode = TextOverflowModes.Overflow;
        textComponent.horizontalMapping = TextureMappingOptions.Character;
        textComponent.verticalMapping = TextureMappingOptions.Character;
        textComponent.alignment = TextAlignmentOptions.Left;
        
        // Force update
        textComponent.ForceMeshUpdate();
    }
    
    void ResetPosition()
    {
        // Put the text off-screen to the right
        Vector3 position = rectTransform.localPosition;
        position.x = startingXPosition;
        rectTransform.localPosition = position;
    }
    
    void Update()
    {
        // Move the text to the left
        Vector3 position = rectTransform.localPosition;
        position.x -= scrollSpeed * Time.deltaTime;
        rectTransform.localPosition = position;
        
        // If the text has moved completely off-screen to the left, reset it
        if (position.x < endingXPosition)
        {
            ResetPosition();
            Debug.Log("News ticker reset to start position");
        }
    }
}