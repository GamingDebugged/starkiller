using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Test scene to demonstrate the new Starkiller systems architecture
    /// </summary>
    public class StarkkillerTestScene : MonoBehaviour
    {
        [Header("System References")]
        [Tooltip("Reference to the content manager")]
        public StarkkillerContentManager contentManager;
        
        [Tooltip("Reference to the media system")]
        public StarkkillerMediaSystem mediaSystem;
        
        [Tooltip("Reference to the encounter system")]
        public StarkkillerEncounterSystem encounterSystem;
        
        [Header("UI Elements")]
        [Tooltip("Text to display ship information")]
        public Text shipInfoText;
        
        [Tooltip("Text to display captain information")]
        public Text captainInfoText;
        
        [Tooltip("Image component to display ship")]
        public Image shipImageDisplay;
        
        [Tooltip("Image component to display captain")]
        public Image captainImageDisplay;
        
        [Tooltip("Video player for ship")]
        public VideoPlayer shipVideoPlayer;
        
        [Tooltip("Video player for captain")]
        public VideoPlayer captainVideoPlayer;
        
        [Tooltip("Approve button")]
        public Button approveButton;
        
        [Tooltip("Deny button")]
        public Button denyButton;
        
        [Tooltip("Next encounter button")]
        public Button nextButton;
        
        [Tooltip("Migration button")]
        public Button migrateButton;
        
        // Current encounter
        private ShipEncounter currentEncounter;
        private EnhancedShipEncounter currentEnhancedEncounter;
        private VideoEnhancedShipEncounter currentVideoEncounter;
        
        // Migrator for legacy systems
        private LegacySystemsMigrator migrator;
        
        private void Start()
        {
            // Find systems if not assigned
            FindSystems();
            
            // Setup buttons
            if (approveButton != null)
                approveButton.onClick.AddListener(ApproveShip);
                
            if (denyButton != null)
                denyButton.onClick.AddListener(DenyShip);
                
            if (nextButton != null)
                nextButton.onClick.AddListener(NextEncounter);
                
            if (migrateButton != null)
                migrateButton.onClick.AddListener(MigrateSystems);
                
            // Create a test encounter to start
            StartCoroutine(SetupTestEncounter());
        }
        
        /// <summary>
        /// Find all systems if not assigned
        /// </summary>
        private void FindSystems()
        {
            if (contentManager == null)
                contentManager = FindFirstObjectByType<StarkkillerContentManager>();
                
            if (mediaSystem == null)
                mediaSystem = FindFirstObjectByType<StarkkillerMediaSystem>();
                
            if (encounterSystem == null)
                encounterSystem = FindFirstObjectByType<StarkkillerEncounterSystem>();
                
            if (migrator == null)
                migrator = FindFirstObjectByType<LegacySystemsMigrator>();
                
            if (migrator == null)
            {
                // Create migrator if needed
                GameObject migratorObj = new GameObject("LegacySystemsMigrator");
                migrator = migratorObj.AddComponent<LegacySystemsMigrator>();
            }
        }
        
        /// <summary>
        /// Setup a test encounter
        /// </summary>
        private IEnumerator SetupTestEncounter()
        {
            Debug.Log("StarkkillerTestScene: Setting up test encounter");
            
            // Wait for systems to be properly initialized
            yield return new WaitForSeconds(0.5f);
            
            if (encounterSystem != null)
            {
                Debug.Log("StarkkillerTestScene: Encounter system found");
                
                // Get an encounter from the encounter system
                currentEncounter = encounterSystem.GetNextEncounter();
                
                if (currentEncounter == null)
                {
                    Debug.Log("StarkkillerTestScene: No encounter available, creating test ship");
                    // Create a test encounter
                    currentEncounter = encounterSystem.CreateTestShip();
                }
                
                Debug.Log($"StarkkillerTestScene: Generated encounter with ship type: '{currentEncounter.shipType}'");
                
                // Get enhanced versions
                if (mediaSystem != null)
                {
                    Debug.Log("StarkkillerTestScene: Media system found, enhancing encounter with media");
                    
                    // Verify media database is loaded
                    if (mediaSystem.mediaDatabase == null)
                    {
                        Debug.LogError("StarkkillerTestScene: Media database is not loaded in media system!");
                    }
                    else
                    {
                        Debug.Log($"StarkkillerTestScene: Media database has {mediaSystem.mediaDatabase.shipMedia.Count} ship entries");
                    }
                    
                    // Enhance with media (images)
                    currentEnhancedEncounter = mediaSystem.EnhanceEncounterWithMedia(currentEncounter);
                    
                    if (currentEnhancedEncounter != null)
                    {
                        Debug.Log($"StarkkillerTestScene: Enhanced encounter created");
                        Debug.Log($"StarkkillerTestScene: Enhanced encounter has ship image: {currentEnhancedEncounter.shipImage != null}");
                        Debug.Log($"StarkkillerTestScene: Enhanced encounter has captain portrait: {currentEnhancedEncounter.captainPortrait != null}");
                        
                        // Enhance with videos
                        currentVideoEncounter = mediaSystem.EnhanceEncounterWithVideo(currentEnhancedEncounter);
                        
                        if (currentVideoEncounter != null)
                        {
                            Debug.Log($"StarkkillerTestScene: Video-enhanced encounter created");
                            Debug.Log($"StarkkillerTestScene: Video encounter has ship video: {currentVideoEncounter.shipVideo != null}");
                            Debug.Log($"StarkkillerTestScene: Video encounter has captain video: {currentVideoEncounter.captainVideo != null}");
                        }
                        else
                        {
                            Debug.LogError("StarkkillerTestScene: Failed to create video-enhanced encounter");
                        }
                    }
                    else
                    {
                        Debug.LogError("StarkkillerTestScene: Failed to create enhanced encounter");
                    }
                    
                    // Update UI
                    UpdateUI();
                }
                else
                {
                    Debug.LogError("StarkkillerTestScene: Media system not found - cannot enhance encounter");
                }
            }
            else
            {
                Debug.LogError("StarkkillerTestScene: Encounter system not found - cannot create test encounter");
            }
        }
        
        /// <summary>
        /// Update the UI with the current encounter information
        /// </summary>
        private void UpdateUI()
        {
            if (currentEncounter == null)
                return;
                
            // Update ship info
            if (shipInfoText != null)
            {
                shipInfoText.text = $"Ship: {currentEncounter.shipType}\n" +
                                   $"Origin: {currentEncounter.origin}\n" +
                                   $"Destination: {currentEncounter.destination}\n" +
                                   $"Access Code: {currentEncounter.accessCode}\n" +
                                   $"Manifest: {currentEncounter.manifest}";
            }
            
            // Update captain info
            if (captainInfoText != null)
            {
                captainInfoText.text = $"Captain: {currentEncounter.captainName}\n" +
                                      $"Rank: {currentEncounter.captainRank}\n" +
                                      $"Story: {currentEncounter.story}";
                                      
                // Add validity info if in test mode
                captainInfoText.text += $"\n\nShould Approve: {currentEncounter.shouldApprove}";
                
                if (!string.IsNullOrEmpty(currentEncounter.invalidReason))
                {
                    captainInfoText.text += $"\nInvalid Reason: {currentEncounter.invalidReason}";
                }
            }
            
            // Update ship image
            if (shipImageDisplay != null && currentEnhancedEncounter != null)
            {
                Debug.Log($"UpdateUI: Updating ship image display. Ship image available: {currentEnhancedEncounter.shipImage != null}");
                if (currentEnhancedEncounter.shipImage != null)
                {
                    shipImageDisplay.sprite = currentEnhancedEncounter.shipImage;
                    shipImageDisplay.enabled = true;
                    Debug.Log($"UpdateUI: Set ship image sprite name: {currentEnhancedEncounter.shipImage.name}");
                }
                else
                {
                    Debug.LogWarning($"UpdateUI: No ship image available for {currentEnhancedEncounter.shipType}");
                    shipImageDisplay.enabled = false;
                }
            }
            else if (shipImageDisplay != null)
            {
                Debug.LogWarning("UpdateUI: currentEnhancedEncounter is null, can't update ship image");
                shipImageDisplay.enabled = false;
            }
            
            // Update captain image
            if (captainImageDisplay != null && currentEnhancedEncounter != null)
            {
                Debug.Log($"UpdateUI: Updating captain image display. Captain portrait available: {currentEnhancedEncounter.captainPortrait != null}");
                if (currentEnhancedEncounter.captainPortrait != null)
                {
                    captainImageDisplay.sprite = currentEnhancedEncounter.captainPortrait;
                    captainImageDisplay.enabled = true;
                    Debug.Log($"UpdateUI: Set captain portrait sprite name: {currentEnhancedEncounter.captainPortrait.name}");
                }
                else
                {
                    Debug.LogWarning($"UpdateUI: No captain portrait available for {currentEnhancedEncounter.captainName}");
                    captainImageDisplay.enabled = false;
                }
            }
            else if (captainImageDisplay != null)
            {
                Debug.LogWarning("UpdateUI: currentEnhancedEncounter is null, can't update captain image");
                captainImageDisplay.enabled = false;
            }
            
            // Update videos if available
            if (shipVideoPlayer != null && currentVideoEncounter != null)
            {
                Debug.Log($"UpdateUI: Updating ship video player. Ship video available: {currentVideoEncounter.shipVideo != null}");
                if (currentVideoEncounter.shipVideo != null)
                {
                    shipVideoPlayer.clip = currentVideoEncounter.shipVideo;
                    shipVideoPlayer.Play();
                    Debug.Log($"UpdateUI: Playing ship video: {currentVideoEncounter.shipVideo.name}");
                }
                else
                {
                    Debug.LogWarning($"UpdateUI: No ship video available for {currentVideoEncounter.shipType}");
                    shipVideoPlayer.Stop();
                }
            }
            else if (shipVideoPlayer != null)
            {
                Debug.LogWarning("UpdateUI: currentVideoEncounter is null, can't update ship video");
                shipVideoPlayer.Stop();
            }
            
            if (captainVideoPlayer != null && currentVideoEncounter != null)
            {
                Debug.Log($"UpdateUI: Updating captain video player. Captain video available: {currentVideoEncounter.captainVideo != null}");
                if (currentVideoEncounter.captainVideo != null)
                {
                    captainVideoPlayer.clip = currentVideoEncounter.captainVideo;
                    captainVideoPlayer.Play();
                    Debug.Log($"UpdateUI: Playing captain video: {currentVideoEncounter.captainVideo.name}");
                }
                else
                {
                    Debug.LogWarning($"UpdateUI: No captain video available for {currentVideoEncounter.captainName}");
                    captainVideoPlayer.Stop();
                }
            }
            else if (captainVideoPlayer != null)
            {
                Debug.LogWarning("UpdateUI: currentVideoEncounter is null, can't update captain video");
                captainVideoPlayer.Stop();
            }
            
            // Update buttons
            if (approveButton != null)
                approveButton.interactable = true;
                
            if (denyButton != null)
                denyButton.interactable = true;
                
            if (nextButton != null)
                nextButton.interactable = false;
        }
        
        /// <summary>
        /// Approve the current ship
        /// </summary>
        public void ApproveShip()
        {
            if (encounterSystem != null && currentEncounter != null)
            {
                encounterSystem.ProcessDecision(true);
                
                // Show result
                if (shipInfoText != null)
                {
                    shipInfoText.text = "SHIP APPROVED\n\n" + shipInfoText.text;
                }
                
                // Update button state
                if (approveButton != null)
                    approveButton.interactable = false;
                    
                if (denyButton != null)
                    denyButton.interactable = false;
                    
                if (nextButton != null)
                    nextButton.interactable = true;
                    
                // Clear current encounter
                currentEncounter = null;
            }
        }
        
        /// <summary>
        /// Deny the current ship
        /// </summary>
        public void DenyShip()
        {
            if (encounterSystem != null && currentEncounter != null)
            {
                encounterSystem.ProcessDecision(false);
                
                // Show result
                if (shipInfoText != null)
                {
                    shipInfoText.text = "SHIP DENIED\n\n" + shipInfoText.text;
                }
                
                // Update button state
                if (approveButton != null)
                    approveButton.interactable = false;
                    
                if (denyButton != null)
                    denyButton.interactable = false;
                    
                if (nextButton != null)
                    nextButton.interactable = true;
                    
                // Clear current encounter
                currentEncounter = null;
            }
        }
        
        /// <summary>
        /// Move to the next encounter
        /// </summary>
        public void NextEncounter()
        {
            StartCoroutine(SetupTestEncounter());
        }
        
        /// <summary>
        /// Migrate legacy systems to new architecture
        /// </summary>
        public void MigrateSystems()
        {
            if (migrator != null)
            {
                migrator.MigrateAll();
                
                // Update UI after migration
                if (shipInfoText != null)
                {
                    shipInfoText.text = "MIGRATION COMPLETE\n\nNew systems are ready to use.";
                }
                
                // Refresh encounter after migration
                StartCoroutine(SetupTestEncounter());
            }
            else
            {
                Debug.LogError("Migrator not found - cannot migrate systems");
            }
        }
    }
}