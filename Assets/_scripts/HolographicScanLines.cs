using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates a simple scan line effect for holographic displays
/// Attach to any UI element to add scan lines that move
/// </summary>
[RequireComponent(typeof(RawImage))]
public class HolographicScanLines : MonoBehaviour
{
    [Header("Scan Line Settings")]
    [Range(0.1f, 10f)]
    public float scanSpeed = 2f;           // Speed of scan line movement
    [Range(0f, 1f)]
    public float scanIntensity = 0.2f;     // Intensity of scan lines
    public Color scanColor = new Color(0.5f, 0.8f, 1f, 0.3f); // Color of scan lines
    
    // The raw image component
    private RawImage rawImage;
    
    // Texture for scan lines
    private Texture2D scanTexture;
    
    // Animation offset
    private float offset = 0f;
    
    void Start()
    {
        // Get the RawImage component
        rawImage = GetComponent<RawImage>();
        
        // Create scan line texture
        CreateScanLineTexture();
        
        // Apply to raw image
        if (rawImage != null && scanTexture != null)
        {
            rawImage.texture = scanTexture;
            rawImage.color = scanColor;
        }
    }
    
    void Update()
    {
        if (rawImage == null || scanTexture == null)
            return;
            
        // Animate scan lines
        offset += Time.deltaTime * scanSpeed;
        if (offset > 1.0f)
            offset -= 1.0f;
            
        // Apply offset to UV rect
        Rect uvRect = rawImage.uvRect;
        uvRect.y = offset;
        rawImage.uvRect = uvRect;
    }
    
    /// <summary>
    /// Create a texture with scan lines
    /// </summary>
    void CreateScanLineTexture()
    {
        int textureHeight = 16; // Number of scan lines
        
        // Create texture
        scanTexture = new Texture2D(1, textureHeight, TextureFormat.RGBA32, false);
        scanTexture.wrapMode = TextureWrapMode.Repeat;
        
        // Create scan line pattern
        Color[] colors = new Color[textureHeight];
        for (int i = 0; i < textureHeight; i++)
        {
            // Every 4th line is visible
            if (i % 4 == 0)
            {
                colors[i] = new Color(1, 1, 1, scanIntensity);
            }
            else
            {
                colors[i] = new Color(1, 1, 1, 0);
            }
        }
        
        // Apply to texture
        scanTexture.SetPixels(colors);
        scanTexture.Apply();
    }
    
    void OnDestroy()
    {
        // Clean up texture
        if (scanTexture != null)
        {
            Destroy(scanTexture);
        }
    }
}
