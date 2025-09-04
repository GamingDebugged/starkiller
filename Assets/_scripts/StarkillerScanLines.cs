using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates scan line effect for Imperial holographic displays using a RawImage
/// Specifically for Starkiller Base Command project
/// </summary>
[RequireComponent(typeof(RawImage))]
public class StarkillerScanLines : MonoBehaviour
{
    [Header("Scan Line Settings")]
    public float speed = 10f;
    public int lineCount = 50;
    [Range(0f, 1f)]
    public float intensity = 0.3f;
    public Color lineColor = new Color(0.5f, 0.8f, 1.0f, 0.3f);
    
    // Texture for scan lines
    private Texture2D scanLineTexture;
    private RawImage rawImage;
    private float offset = 0f;
    
    void Awake()
    {
        // Get references
        rawImage = GetComponent<RawImage>();
        
        // Create scan line texture
        CreateScanlineTexture();
        
        // Apply to Raw Image
        if (rawImage != null && scanLineTexture != null)
        {
            rawImage.texture = scanLineTexture;
            rawImage.color = lineColor;
        }
    }
    
    void Update()
    {
        if (rawImage == null || scanLineTexture == null)
            return;
            
        // Animate scan lines
        offset += Time.deltaTime * speed;
        if (offset > 1.0f)
            offset -= 1.0f;
            
        // Apply offset to UV rect
        Rect uvRect = rawImage.uvRect;
        uvRect.y = offset;
        rawImage.uvRect = uvRect;
    }
    
    // Create a texture with horizontal scan lines
    void CreateScanlineTexture()
    {
        // Create texture
        scanLineTexture = new Texture2D(1, lineCount, TextureFormat.RGBA32, false);
        scanLineTexture.wrapMode = TextureWrapMode.Repeat;
        
        // Create pixels with alternating transparency
        Color[] pixels = new Color[lineCount];
        
        for (int y = 0; y < lineCount; y++)
        {
            // Make alternating lines transparent
            if (y % 2 == 0)
            {
                pixels[y] = new Color(1, 1, 1, intensity);
            }
            else
            {
                pixels[y] = new Color(1, 1, 1, 0);
            }
        }
        
        // Apply pixels to texture
        scanLineTexture.SetPixels(pixels);
        scanLineTexture.Apply();
    }
    
    void OnDestroy()
    {
        // Clean up texture
        if (scanLineTexture != null)
        {
            Destroy(scanLineTexture);
        }
    }
}
