using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Diagnostic tool to check the video system setup
/// </summary>
public class VideoSystemDiagnostics : MonoBehaviour
{
    public ShipVideoSystem shipVideoSystem;
    
    [Header("Test Inputs")]
    public string testFaction = "imperium";
    public string testShipType = "orion Shuttle";
    
    [Header("Debug Output")]
    public VideoClip resultCaptainVideo;
    public VideoClip resultShipVideo;
    
    void Start()
    {
        // Find the ShipVideoSystem if not assigned
        if (shipVideoSystem == null)
        {
            shipVideoSystem = FindFirstObjectByType<ShipVideoSystem>();
            if (shipVideoSystem == null)
            {
                Debug.LogError("Cannot find ShipVideoSystem!");
                return;
            }
        }
        
        // Run diagnostics
        RunDiagnostics();
    }
    
    public void RunDiagnostics()
    {
        Debug.Log("=== VIDEO SYSTEM DIAGNOSTICS ===");
        
        // Check for null arrays
        if (shipVideoSystem.shipVideos == null)
        {
            Debug.LogError("shipVideos array is null!");
        }
        else
        {
            Debug.Log($"Ship Videos count: {shipVideoSystem.shipVideos.Length}");
            
            // Check each ship video entry
            foreach (var shipData in shipVideoSystem.shipVideos)
            {
                if (shipData.shipVideo == null)
                {
                    Debug.LogWarning($"NULL ship video for type: {shipData.shipTypeName}");
                }
                else
                {
                    Debug.Log($"Ship type {shipData.shipTypeName} has video: {shipData.shipVideo.name}");
                }
            }
        }
        
        // Check captain videos
        if (shipVideoSystem.captainVideos == null)
        {
            Debug.LogError("captainVideos array is null!");
        }
        else
        {
            Debug.Log($"Captain Videos count: {shipVideoSystem.captainVideos.Length}");
            
            // Check each faction
            foreach (var captainData in shipVideoSystem.captainVideos)
            {
                if (captainData.captainVideos == null)
                {
                    Debug.LogWarning($"NULL captainVideos array for faction: {captainData.faction}");
                }
                else
                {
                    Debug.Log($"Faction {captainData.faction} has {captainData.captainVideos.Length} videos");
                    
                    // Check for null entries
                    int nullCount = 0;
                    for (int i = 0; i < captainData.captainVideos.Length; i++)
                    {
                        if (captainData.captainVideos[i] == null)
                        {
                            nullCount++;
                        }
                        else
                        {
                            Debug.Log($"  - Video {i}: {captainData.captainVideos[i].name}");
                        }
                    }
                    
                    if (nullCount > 0)
                    {
                        Debug.LogWarning($"Faction {captainData.faction} has {nullCount} NULL video entries");
                    }
                }
            }
        }
        
        // Check default captain videos
        if (shipVideoSystem.defaultCaptainVideos == null)
        {
            Debug.LogError("defaultCaptainVideos array is null!");
        }
        else
        {
            Debug.Log($"Default Captain Videos count: {shipVideoSystem.defaultCaptainVideos.Length}");
            
            // Check for null entries
            int nullCount = 0;
            for (int i = 0; i < shipVideoSystem.defaultCaptainVideos.Length; i++)
            {
                if (shipVideoSystem.defaultCaptainVideos[i] == null)
                {
                    nullCount++;
                }
                else
                {
                    Debug.Log($"  - Default Video {i}: {shipVideoSystem.defaultCaptainVideos[i].name}");
                }
            }
            
            if (nullCount > 0)
            {
                Debug.LogWarning($"Default captain videos has {nullCount} NULL entries");
            }
        }
        
        // Test GetShipVideoForType
        Debug.Log($"Testing GetShipVideoForType with '{testShipType}'...");
        resultShipVideo = shipVideoSystem.GetShipVideoForType(testShipType);
        if (resultShipVideo == null)
        {
            Debug.LogError($"No ship video found for type: {testShipType}");
        }
        else
        {
            Debug.Log($"Found ship video: {resultShipVideo.name}");
        }
        
        // Test GetCaptainVideo
        Debug.Log($"Testing GetCaptainVideo with faction '{testFaction}'...");
        for (int i = 0; i < 5; i++)
        {
            resultCaptainVideo = shipVideoSystem.GetCaptainVideo(testFaction);
            if (resultCaptainVideo == null)
            {
                Debug.LogError($"No captain video found for faction: {testFaction}");
            }
            else
            {
                Debug.Log($"Try {i+1}: Found captain video: {resultCaptainVideo.name}");
            }
        }
        
        Debug.Log("=== END OF DIAGNOSTICS ===");
    }
    
    [ContextMenu("Run Diagnostics")]
    public void RunDiagnosticFromMenu()
    {
        RunDiagnostics();
    }
}