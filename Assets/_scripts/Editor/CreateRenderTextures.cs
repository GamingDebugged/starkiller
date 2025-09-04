using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateRenderTextures : MonoBehaviour
{
    [MenuItem("Tools/Create Ship Video Render Textures")]
    static void CreateVideoRenderTextures()
    {
        // Create the RenderTextures directory if it doesn't exist
        if (!Directory.Exists("Assets/RenderTextures"))
        {
            Directory.CreateDirectory("Assets/RenderTextures");
            AssetDatabase.Refresh();
        }
        
        // Create a render texture for the ship video
        RenderTexture shipRenderTexture = new RenderTexture(512, 288, 24);
        shipRenderTexture.name = "ShipVideoRT";
        
        // Create a render texture for the captain video
        RenderTexture captainRenderTexture = new RenderTexture(256, 256, 24);
        captainRenderTexture.name = "CaptainVideoRT";
        
        // Save the render textures as assets
        AssetDatabase.CreateAsset(shipRenderTexture, "Assets/RenderTextures/ShipVideoRT.renderTexture");
        AssetDatabase.CreateAsset(captainRenderTexture, "Assets/RenderTextures/CaptainVideoRT.renderTexture");
        
        AssetDatabase.SaveAssets();
        
        Debug.Log("Created render textures for ship and captain videos");
    }
}
