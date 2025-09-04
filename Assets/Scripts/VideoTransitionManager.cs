using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Manages smooth transitions between videos to prevent jarring cuts
    /// </summary>
    public class VideoTransitionManager : MonoBehaviour
    {
        [Header("Video Players")]
        [SerializeField] private VideoPlayer shipVideoPlayer;
        [SerializeField] private VideoPlayer captainVideoPlayer;
        
        [Header("Display References")]
        [SerializeField] private RawImage shipVideoImage;
        [SerializeField] private RawImage captainVideoImage;
        [SerializeField] private GameObject shipVideoContainer;
        [SerializeField] private GameObject captainVideoContainer;
        
        [Header("Transition Settings")]
        [SerializeField] private float fadeOutDuration = 0.5f;
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float delayBetweenVideos = 0.2f;
        
        [Header("Debug")]
        [SerializeField] private bool verboseLogging = true;
        
        // Events
        public event Action OnTransitionComplete;
        
        // Track transition state to prevent multiple transitions at once
        private bool isTransitioning = false;
        
        /// <summary>
        /// Transition to a new ship video with smooth fading
        /// </summary>
        public void TransitionShipVideo(VideoClip newVideo)
        {
            if (isTransitioning)
            {
                if (verboseLogging)
                    Debug.Log("VideoTransitionManager: Already in transition, ignoring request.");
                return;
            }
            
            if (shipVideoPlayer == null || shipVideoImage == null)
            {
                Debug.LogError("VideoTransitionManager: Ship video components not assigned!");
                return;
            }
            
            StartCoroutine(PerformVideoTransition(shipVideoPlayer, shipVideoImage, newVideo, shipVideoContainer));
        }
        
        /// <summary>
        /// Transition to a new captain video with smooth fading
        /// </summary>
        public void TransitionCaptainVideo(VideoClip newVideo)
        {
            if (isTransitioning)
            {
                if (verboseLogging)
                    Debug.Log("VideoTransitionManager: Already in transition, ignoring request.");
                return;
            }
            
            if (captainVideoPlayer == null || captainVideoImage == null)
            {
                Debug.LogError("VideoTransitionManager: Captain video components not assigned!");
                return;
            }
            
            StartCoroutine(PerformVideoTransition(captainVideoPlayer, captainVideoImage, newVideo, captainVideoContainer));
        }
        
        /// <summary>
        /// Perform the actual video transition with fading
        /// </summary>
        private IEnumerator PerformVideoTransition(VideoPlayer player, RawImage display, VideoClip newVideo, GameObject container)
        {
            isTransitioning = true;
            
            if (verboseLogging)
                Debug.Log($"VideoTransitionManager: Starting transition to video: {newVideo?.name ?? "null"}");
            
            // Get the initial color with full alpha
            Color startColor = display.color;
            
            // 1. Fade out current video if playing
            if (player.isPlaying)
            {
                float elapsedTime = 0;
                while (elapsedTime < fadeOutDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / fadeOutDuration);
                    
                    // Update alpha
                    Color newColor = display.color;
                    newColor.a = Mathf.Lerp(startColor.a, 0f, t);
                    display.color = newColor;
                    
                    yield return null;
                }
                
                // Ensure alpha is 0
                Color finalFadeOutColor = display.color;
                finalFadeOutColor.a = 0f;
                display.color = finalFadeOutColor;
                
                // Stop the current video
                player.Stop();
            }
            
            // 2. Change to new video
            if (newVideo != null)
            {
                player.clip = newVideo;
                player.Prepare();
                
                // Wait until prepared
                while (!player.isPrepared)
                {
                    yield return null;
                }
                
                // Short delay before starting new video
                yield return new WaitForSeconds(delayBetweenVideos);
                
                // Make container visible
                if (container != null)
                    container.SetActive(true);
                
                // Start playing the new video
                player.Play();
                
                // 3. Fade in new video
                float elapsedTime = 0;
                while (elapsedTime < fadeInDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / fadeInDuration);
                    
                    // Update alpha
                    Color newColor = display.color;
                    newColor.a = Mathf.Lerp(0f, startColor.a, t);
                    display.color = newColor;
                    
                    yield return null;
                }
                
                // Ensure alpha is back to original
                display.color = startColor;
            }
            else
            {
                // If no new video, hide the container
                if (container != null)
                    container.SetActive(false);
            }
            
            // Reset state and notify listeners
            isTransitioning = false;
            OnTransitionComplete?.Invoke();
            
            if (verboseLogging)
                Debug.Log("VideoTransitionManager: Transition complete");
        }
    }
}