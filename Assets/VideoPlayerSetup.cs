using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using StarkillerBaseCommand;

/// <summary>
/// Handles the setup and configuration of video players for ship and captain displays
/// Ensures video players have the correct render textures and settings
/// </summary>
public class VideoPlayerSetup : MonoBehaviour
{
    [Header("Video Player References")]
    [SerializeField] private VideoPlayer shipVideoPlayer;
    [SerializeField] private VideoPlayer captainVideoPlayer;
    
    [Header("Render Texture References")]
    [SerializeField] private RenderTexture shipRenderTexture;
    [SerializeField] private RenderTexture captainRenderTexture;
    
    [Header("RawImage UI References")]
    [SerializeField] private RawImage shipVideoImage;
    [SerializeField] private RawImage captainVideoImage;
    
    [Header("Settings")]
    [SerializeField] private bool loopByDefault = true;
    [SerializeField] private bool playOnAwake = false;
    [SerializeField] private VideoRenderMode renderMode = VideoRenderMode.RenderTexture;
    
    void Awake()
    {
        // Ensure we have the components
        FindReferences();
        
        // Configure the video players
        ConfigureVideoPlayers();
    }
    
    /// <summary>
    /// Find references if not assigned in inspector
    /// </summary>
    private void FindReferences()
    {
        // Ship video player
        if (shipVideoPlayer == null)
        {
            // Try to find by name first
            Transform shipVidObj = transform.Find("ShipVideoPlayer");
            if (shipVidObj != null)
            {
                shipVideoPlayer = shipVidObj.GetComponent<VideoPlayer>();
            }
            
            // If still null, try to find any video player tagged appropriately
            if (shipVideoPlayer == null)
            {
                GameObject shipVidGameObj = GameObject.FindGameObjectWithTag("ShipVideoPlayer");
                if (shipVidGameObj != null)
                {
                    shipVideoPlayer = shipVidGameObj.GetComponent<VideoPlayer>();
                }
            }
        }
        
        // Captain video player
        if (captainVideoPlayer == null)
        {
            // Try to find by name first
            Transform captainVidObj = transform.Find("CaptainVideoPlayer");
            if (captainVidObj != null)
            {
                captainVideoPlayer = captainVidObj.GetComponent<VideoPlayer>();
            }
            
            // If still null, try to find any video player tagged appropriately
            if (captainVideoPlayer == null)
            {
                GameObject captainVidGameObj = GameObject.FindGameObjectWithTag("CaptainVideoPlayer");
                if (captainVidGameObj != null)
                {
                    captainVideoPlayer = captainVidGameObj.GetComponent<VideoPlayer>();
                }
            }
        }
        
        // Find RawImages if not assigned
        if (shipVideoImage == null && shipVideoPlayer != null)
        {
            // Look for a RawImage in the same parent
            shipVideoImage = shipVideoPlayer.GetComponentInChildren<RawImage>();
        }
        
        if (captainVideoImage == null && captainVideoPlayer != null)
        {
            // Look for a RawImage in the same parent
            captainVideoImage = captainVideoPlayer.GetComponentInChildren<RawImage>();
        }
    }
    
    /// <summary>
    /// Configure video players with the correct settings
    /// </summary>
    private void ConfigureVideoPlayers()
    {
        // Configure ship video player
        if (shipVideoPlayer != null)
        {
            shipVideoPlayer.playOnAwake = playOnAwake;
            shipVideoPlayer.isLooping = loopByDefault;
            shipVideoPlayer.renderMode = renderMode;
            
            // Assign render texture if available
            if (shipRenderTexture != null)
            {
                shipVideoPlayer.targetTexture = shipRenderTexture;
                
                // Also assign to RawImage if available
                if (shipVideoImage != null)
                {
                    shipVideoImage.texture = shipRenderTexture;
                }
            }
            
            Debug.Log("Ship video player configured successfully");
        }
        
        // Configure captain video player
        if (captainVideoPlayer != null)
        {
            captainVideoPlayer.playOnAwake = playOnAwake;
            captainVideoPlayer.isLooping = loopByDefault;
            captainVideoPlayer.renderMode = renderMode;
            
            // Assign render texture if available
            if (captainRenderTexture != null)
            {
                captainVideoPlayer.targetTexture = captainRenderTexture;
                
                // Also assign to RawImage if available
                if (captainVideoImage != null)
                {
                    captainVideoImage.texture = captainRenderTexture;
                }
            }
            
            Debug.Log("Captain video player configured successfully");
        }
    }
    
    /// <summary>
    /// Public API to play a video on the ship player
    /// </summary>
    public void PlayShipVideo(VideoClip clip)
    {
        if (shipVideoPlayer == null || clip == null)
            return;
            
        shipVideoPlayer.clip = clip;
        shipVideoPlayer.Play();
    }
    
    /// <summary>
    /// Public API to play a video on the captain player
    /// </summary>
    public void PlayCaptainVideo(VideoClip clip)
    {
        if (captainVideoPlayer == null || clip == null)
            return;
            
        captainVideoPlayer.clip = clip;
        captainVideoPlayer.Play();
    }
    
    /// <summary>
    /// Stop all video players
    /// </summary>
    public void StopAllVideos()
    {
        if (shipVideoPlayer != null && shipVideoPlayer.isPlaying)
        {
            shipVideoPlayer.Stop();
        }
        
        if (captainVideoPlayer != null && captainVideoPlayer.isPlaying)
        {
            captainVideoPlayer.Stop();
        }
    }
}