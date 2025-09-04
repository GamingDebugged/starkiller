using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Manages the Captain ID Card display system
    /// Shows detailed captain information when clicking on captain video/portrait
    /// </summary>
    public class CaptainIDCard : MonoBehaviour
    {
        [Header("ID Card Panel")]
        [Tooltip("Main ID card panel GameObject")]
        public GameObject idCardPanel;
        
        [Tooltip("Background image that will be tinted with faction color")]
        public Image cardBackground;
        
        [Tooltip("Secondary elements that should use faction color")]
        public Image[] accentElements;
        
        [Header("Captain Information")]
        [Tooltip("Captain portrait/photo on the ID")]
        public Image captainPhoto;
        
        [Tooltip("Text showing 'CAPTAIN IMAGE' label")]
        public TMP_Text captainImageLabel;
        
        [Tooltip("Captain's full name")]
        public TMP_Text captainNameText;
        
        [Tooltip("Captain's rank")]
        public TMP_Text rankText;
        
        [Tooltip("Authority level number")]
        public TMP_Text authorityLevelNumber;
        
        [Header("Faction Information")]
        [Tooltip("Faction name text")]
        public TMP_Text factionNameText;
        
        [Tooltip("Faction icon/badge")]
        public Image factionIcon;
        
        [Tooltip("Faction banner background")]
        public Image factionBanner;
        
        [Tooltip("Text showing 'FACTION ICON' label")]
        public TMP_Text factionIconLabel;
        
        [Header("Ship Information")]
        [Tooltip("Ship types this captain is authorized for")]
        public TMP_Text authorizedShipTypes;
        
        [Header("Special Interest Section")]
        [Tooltip("Special interest header text")]
        public TMP_Text specialInterestHeader;
        
        [Tooltip("Captain's special traits or interests")]
        public TMP_Text specialInterestText;
        
        [Header("Animation")]
        [Tooltip("Animation duration for show/hide")]
        public float animationDuration = 0.3f;
        
        [Tooltip("Canvas group for fade animations")]
        public CanvasGroup canvasGroup;
        
        [Header("Audio")]
        [Tooltip("Sound when ID card opens")]
        public AudioSource openSound;
        
        [Tooltip("Sound when ID card closes")]
        public AudioSource closeSound;
        
        [Header("Auto-Encounter Generation")]
        [Tooltip("Automatically generate new encounter when panels are empty")]
        [SerializeField] private bool autoGenerateEncounterWhenEmpty = true;
        [Tooltip("Delay before auto-generating new encounter")]
        [SerializeField] private float delayBeforeAutoGenerate = 0.5f;
        
        // Current encounter reference
        private MasterShipEncounter currentEncounter;
        
        // Flag to prevent multiple animations
        private bool isAnimating = false;
        
        // Reference to FactionManager
        private FactionManager factionManager;
        
        // Store the original scale from Unity Inspector
        private Vector3 originalScale;
        
        void Start()
        {
            // Find FactionManager
            factionManager = FindFirstObjectByType<FactionManager>();
            
            // Store the original scale from Unity Inspector
            if (idCardPanel != null)
            {
                originalScale = idCardPanel.transform.localScale;
                Debug.Log($"[CaptainIDCard] Stored original scale: {originalScale}");
            }
            
            // Only hide panel if it's not already set up in Unity Inspector
            if (idCardPanel != null && !idCardPanel.activeSelf)
            {
                // Panel is already inactive in Inspector, respect that setting
                Debug.Log("[CaptainIDCard] Panel was inactive in Inspector, keeping it hidden");
            }
            else if (idCardPanel != null)
            {
                // Panel is active in Inspector, but ensure it starts with proper visual state
                Debug.Log("[CaptainIDCard] Panel was active in Inspector, setting up initial visual state");
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;  // Start invisible but keep GameObject active
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
            
            // Set up canvas group if not assigned
            if (canvasGroup == null && idCardPanel != null)
            {
                canvasGroup = idCardPanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = idCardPanel.AddComponent<CanvasGroup>();
                }
            }
        }
        
        /// <summary>
        /// Show the ID card with captain information
        /// </summary>
        public void ShowIDCard(MasterShipEncounter encounter)
        {
            // ADD DEBUGGING FOR SHOW CALLS TOO
            Debug.Log($"[CaptainIDCard] ShowIDCard() called! Stack trace:");
            Debug.Log($"{UnityEngine.StackTraceUtility.ExtractStackTrace()}");
            Debug.Log($"[CaptainIDCard] encounter: {(encounter != null ? encounter.captainName : "null")}, isAnimating: {isAnimating}");
            
            if (encounter == null || isAnimating) 
            {
                Debug.Log("[CaptainIDCard] ShowIDCard returning early - encounter null or animating");
                return;
            }
            
            currentEncounter = encounter;
            PopulateIDCard();
            
            // Check if GameObject is active before starting coroutine
            if (gameObject.activeInHierarchy)
            {
                Debug.Log("[CaptainIDCard] Starting AnimateShow coroutine");
                // Show with animation
                StartCoroutine(AnimateShow());
            }
            else
            {
                Debug.Log("[CaptainIDCard] GameObject inactive, showing immediately");
                // If GameObject is inactive, show immediately without animation
                ShowImmediately();
            }
        }
        
        /// <summary>
        /// Hide the ID card
        /// </summary>
        public void HideIDCard()
        {
            // ADD COMPREHENSIVE DEBUGGING
            Debug.Log($"[CaptainIDCard] HideIDCard() called! Stack trace:");
            Debug.Log($"{UnityEngine.StackTraceUtility.ExtractStackTrace()}");
            Debug.Log($"[CaptainIDCard] isAnimating: {isAnimating}, gameObject.activeInHierarchy: {gameObject.activeInHierarchy}");
            Debug.Log($"[CaptainIDCard] idCardPanel.activeSelf: {(idCardPanel != null ? idCardPanel.activeSelf.ToString() : "null")}");
            
            if (isAnimating) 
            {
                Debug.Log("[CaptainIDCard] Already animating, ignoring HideIDCard call");
                return;
            }
            
            // Check if GameObject is active before starting coroutine
            if (gameObject.activeInHierarchy)
            {
                Debug.Log("[CaptainIDCard] Starting AnimateHide coroutine");
                StartCoroutine(AnimateHide());
            }
            else
            {
                Debug.Log("[CaptainIDCard] GameObject inactive, hiding immediately");
                // If GameObject is inactive, hide immediately without animation
                HideImmediately();
            }
        }
        
        /// <summary>
        /// Toggle ID card visibility
        /// </summary>
        public void ToggleIDCard(MasterShipEncounter encounter)
        {
            // Check visual visibility instead of GameObject active state
            if (canvasGroup != null && canvasGroup.alpha > 0.5f)
            {
                HideIDCard();
            }
            else
            {
                ShowIDCard(encounter);
            }
        }
        
        /// <summary>
        /// Toggle ID card visibility - for button clicks
        /// </summary>
        public void ToggleIDCardFromButton()
        {
            Debug.Log($"[CaptainIDCard] ToggleIDCardFromButton() called!");
            Debug.Log($"[CaptainIDCard] Toggle stack trace:");
            Debug.Log($"{UnityEngine.StackTraceUtility.ExtractStackTrace()}");
            Debug.Log($"[CaptainIDCard] idCardPanel.activeSelf: {(idCardPanel != null ? idCardPanel.activeSelf.ToString() : "null")}");
            Debug.Log($"[CaptainIDCard] canvasGroup.alpha: {(canvasGroup != null ? canvasGroup.alpha.ToString() : "null")}");
            
            // Check if card is visually showing (alpha > 0) instead of just active
            if (idCardPanel != null && canvasGroup != null && canvasGroup.alpha > 0.5f)
            {
                Debug.Log("[CaptainIDCard] Panel is visually showing (alpha > 0.5), hiding it");
                HideIDCard();
                return;
            }
            
            // Otherwise, try to show it
            Debug.Log("[CaptainIDCard] Panel is not active, trying to show it");
            CredentialChecker credChecker = FindFirstObjectByType<CredentialChecker>();
            
            if (credChecker != null)
            {
                MasterShipEncounter encounter = credChecker.GetCurrentEncounter();
                if (encounter != null)
                {
                    Debug.Log($"[CaptainIDCard] Found encounter: {encounter.captainName}, showing ID card");
                    ShowIDCard(encounter);
                }
                else
                {
                    Debug.Log("[CaptainIDCard] No current encounter found");
                    
                    // Check if game is in active gameplay state before trying to generate encounters
                    GameStateController gameState = GameStateController.Instance;
                    if (gameState != null && !gameState.IsGameplayActive())
                    {
                        Debug.Log("[CaptainIDCard] Game not in active gameplay state, skipping encounter generation");
                        return;
                    }
                    
                    // Try to generate a new encounter if enabled
                    if (autoGenerateEncounterWhenEmpty)
                    {
                        TryGenerateNewEncounter();
                    }
                }
            }
            else
            {
                Debug.Log("[CaptainIDCard] No CredentialChecker found");
            }
        }
        
        /// <summary>
        /// Populate the ID card with captain data
        /// </summary>
        private void PopulateIDCard()
        {
            if (currentEncounter == null) return;
            
            // Set captain photo
            if (captainPhoto != null && currentEncounter.captainPortrait != null)
            {
                captainPhoto.sprite = currentEncounter.captainPortrait;
            }
            
            // Set captain name
            if (captainNameText != null)
            {
                string firstName = "";
                string lastName = "";
                
                // Parse the full name to separate first and last
                if (!string.IsNullOrEmpty(currentEncounter.captainName))
                {
                    string[] nameParts = currentEncounter.captainName.Split(' ');
                    if (nameParts.Length >= 2)
                    {
                        // Skip the rank if it's included
                        int startIndex = 0;
                        if (nameParts[0] == currentEncounter.captainRank)
                        {
                            startIndex = 1;
                        }
                        
                        if (startIndex < nameParts.Length)
                        {
                            firstName = nameParts[startIndex];
                            if (startIndex + 1 < nameParts.Length)
                            {
                                lastName = nameParts[startIndex + 1];
                            }
                        }
                    }
                    else
                    {
                        firstName = currentEncounter.captainName;
                    }
                }
                
                captainNameText.text = $"{firstName} {lastName}".Trim();
            }
            
            // Set rank
            if (rankText != null)
            {
                rankText.text = $"RANK: {currentEncounter.captainRank ?? "UNKNOWN"}";
            }
            
            // Set authority level from captain type data
            if (authorityLevelNumber != null && currentEncounter.captainTypeData != null)
            {
                // Find the specific captain to get their authority level
                var captain = currentEncounter.captainTypeData.captains.Find(c => 
                    c.GetFullName() == currentEncounter.captainName);
                
                if (captain != null)
                {
                    authorityLevelNumber.text = captain.authorityLevel.ToString();
                }
                else
                {
                    authorityLevelNumber.text = "?";
                }
            }
            
            // Handle faction data
            bool factionColorApplied = false;
            
            if (factionManager != null && !string.IsNullOrEmpty(currentEncounter.captainFaction))
            {
                Debug.Log($"Looking for faction: '{currentEncounter.captainFaction}'");
                Faction factionData = factionManager.GetFaction(currentEncounter.captainFaction);
                
                if (factionData != null)
                {
                    Debug.Log($"Found faction: {factionData.displayName}");
                    
                    // Set faction name
                    if (factionNameText != null)
                    {
                        factionNameText.text = factionData.displayName.ToUpper();
                    }
                    
                    // Set faction icon
                    if (factionIcon != null && factionData.factionIcon != null)
                    {
                        factionIcon.sprite = factionData.factionIcon;
                        factionIcon.gameObject.SetActive(true);
                    }
                    else if (factionIcon != null)
                    {
                        factionIcon.gameObject.SetActive(false);
                    }
                    
                    // Apply faction color to card elements
                    ApplyFactionStyling(factionData);
                    factionColorApplied = true;
                }
                else
                {
                    Debug.LogWarning($"Faction not found: '{currentEncounter.captainFaction}'");
                    
                    // Fallback if faction not found
                    if (factionNameText != null)
                    {
                        factionNameText.text = currentEncounter.captainFaction.ToUpper();
                    }
                }
            }
            else
            {
                Debug.LogWarning("FactionManager is null or captain faction is empty");
            }
            
            // If no faction color was applied, use defaults
            if (!factionColorApplied)
            {
                Debug.Log("No faction color applied, using defaults");
                ApplyDefaultFactionColor();
            }
            
            // Set authorized ship types from encounter data
            if (authorizedShipTypes != null)
            {
                // Use the ship type from the encounter
                authorizedShipTypes.text = $"SHIP TYPES:\n{currentEncounter.shipType.ToUpper()}";
            }
            
            // Set special interest section
            if (specialInterestHeader != null && specialInterestText != null)
            {
                // Check if this is a special interest captain
                if (currentEncounter.captainFaction == "special_interest" || 
                    currentEncounter.captainFaction == "Special Interest")
                {
                    specialInterestHeader.gameObject.SetActive(true);
                    specialInterestText.gameObject.SetActive(true);
                    
                    // Try to get special traits from captain type data
                    if (currentEncounter.captainTypeData != null && 
                        currentEncounter.captainTypeData.typicalBehaviors != null &&
                        currentEncounter.captainTypeData.typicalBehaviors.Length > 0)
                    {
                        specialInterestText.text = currentEncounter.captainTypeData.typicalBehaviors[0];
                    }
                    else
                    {
                        specialInterestText.text = "CLASSIFIED";
                    }
                }
                else
                {
                    specialInterestHeader.gameObject.SetActive(false);
                    specialInterestText.gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Apply faction-specific styling to the ID card
        /// </summary>
        private void ApplyFactionStyling(Faction faction)
        {
            if (faction == null) return;
            
            Debug.Log($"ApplyFactionStyling called for faction: {faction.displayName}");
            Debug.Log($"Faction color: {faction.factionColor} (R:{faction.factionColor.r} G:{faction.factionColor.g} B:{faction.factionColor.b})");
            
            // Check if the faction color is white (which means it wasn't set properly)
            if (faction.factionColor == Color.white || faction.factionColor == new Color(1, 1, 1, 1))
            {
                Debug.LogWarning($"Faction {faction.displayName} has white color! Applying default colors.");
                ApplyDefaultFactionColor();
                return;
            }
            
            // Apply faction color with some transparency to background
            if (cardBackground != null)
            {
                Color bgColor = faction.factionColor;
                bgColor.a = 0.9f; // Slight transparency
                cardBackground.color = bgColor;
                Debug.Log($"Applied background color: {bgColor}");
            }
            
            // Apply faction color to accent elements
            if (accentElements != null)
            {
                foreach (var element in accentElements)
                {
                    if (element != null)
                    {
                        element.color = faction.factionColor;
                    }
                }
            }
            
            // Apply color to faction banner with darker shade
            if (factionBanner != null)
            {
                Color bannerColor = faction.factionColor * 0.7f; // Darker shade
                bannerColor.a = 1f;
                factionBanner.color = bannerColor;
            }
        }
        
        /// <summary>
        /// Apply default colors when faction data is missing or white
        /// </summary>
        private void ApplyDefaultFactionColor()
        {
            if (currentEncounter == null) return;
            
            Color defaultColor = Color.gray;
            string factionLower = currentEncounter.captainFaction?.ToLower() ?? "";
            
            Debug.Log($"Applying default color for faction: {currentEncounter.captainFaction}");
            
            // Set default colors based on faction name
            if (factionLower.Contains("imperium") || factionLower.Contains("imperial"))
                defaultColor = new Color(0.8f, 0.1f, 0.1f); // Red
            else if (factionLower.Contains("bounty"))
                defaultColor = new Color(0.9f, 0.7f, 0.1f); // Gold
            else if (factionLower.Contains("civilian"))
                defaultColor = new Color(0.2f, 0.4f, 0.8f); // Blue
            else if (factionLower.Contains("special"))
                defaultColor = new Color(0.7f, 0.1f, 0.7f); // Purple
            else if (factionLower.Contains("pirate"))
                defaultColor = new Color(0.8f, 0.3f, 0.1f); // Orange
            else if (factionLower.Contains("merchant"))
                defaultColor = new Color(0.1f, 0.6f, 0.1f); // Green
            else if (factionLower.Contains("military"))
                defaultColor = new Color(0.3f, 0.4f, 0.3f); // Military green
            else if (factionLower.Contains("order"))
                defaultColor = new Color(0.5f, 0.1f, 0.8f); // Deep purple
            else if (factionLower.Contains("automated"))
                defaultColor = new Color(0.0f, 0.8f, 1.0f); // Cyan
            
            Debug.Log($"Using default color: {defaultColor}");
            
            // Apply the default color
            if (cardBackground != null)
            {
                Color bgColor = defaultColor;
                bgColor.a = 0.9f;
                cardBackground.color = bgColor;
            }
            
            if (accentElements != null)
            {
                foreach (var element in accentElements)
                {
                    if (element != null)
                    {
                        element.color = defaultColor;
                    }
                }
            }
            
            if (factionBanner != null)
            {
                Color bannerColor = defaultColor * 0.7f;
                bannerColor.a = 1f;
                factionBanner.color = bannerColor;
            }
        }
        
        /// <summary>
        /// Animate showing the ID card
        /// </summary>
        private IEnumerator AnimateShow()
        {
            isAnimating = true;
            
            if (openSound != null) openSound.Play();
            
            // Make sure the panel is visually active and interactable
            idCardPanel.SetActive(true);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            // Start from scaled down and transparent
            Vector3 startScale = originalScale * 0.8f;
            idCardPanel.transform.localScale = startScale;
            canvasGroup.alpha = 0f;
            
            float elapsed = 0f;
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                
                // Ease out cubic
                t = 1f - Mathf.Pow(1f - t, 3f);
                
                idCardPanel.transform.localScale = Vector3.Lerp(startScale, originalScale, t);
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                
                yield return null;
            }
            
            idCardPanel.transform.localScale = originalScale;
            canvasGroup.alpha = 1f;
            
            isAnimating = false;
        }
        
        /// <summary>
        /// Animate hiding the ID card
        /// </summary>
        private IEnumerator AnimateHide()
        {
            isAnimating = true;
            
            if (closeSound != null) closeSound.Play();
            
            float elapsed = 0f;
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                
                // Ease in cubic
                t = Mathf.Pow(t, 3f);
                
                Vector3 endScale = originalScale * 0.8f;
                idCardPanel.transform.localScale = Vector3.Lerp(originalScale, endScale, t);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                
                yield return null;
            }
            
            // Don't deactivate the main panel - just hide it visually
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            // Reset scale to original for next show
            idCardPanel.transform.localScale = originalScale;
            
            isAnimating = false;
            currentEncounter = null;
        }
        
        /// <summary>
        /// Show the ID card immediately without animation (for when GameObject is inactive)
        /// </summary>
        private void ShowImmediately()
        {
            if (idCardPanel != null)
            {
                idCardPanel.SetActive(true);
                idCardPanel.transform.localScale = originalScale;
            }
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            
            isAnimating = false;
        }
        
        /// <summary>
        /// Hide the ID card immediately without animation (for when GameObject is inactive)
        /// </summary>
        private void HideImmediately()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            
            if (idCardPanel != null)
            {
                idCardPanel.transform.localScale = originalScale;
            }
            
            isAnimating = false;
            currentEncounter = null;
        }
        
        /// <summary>
        /// Called when clicking outside the ID card (to close it)
        /// </summary>
        public void OnBackgroundClick()
        {
            Debug.Log("[CaptainIDCard] Background clicked - hiding ID card!");
            Debug.Log($"[CaptainIDCard] Background click stack trace:");
            Debug.Log($"{UnityEngine.StackTraceUtility.ExtractStackTrace()}");
            HideIDCard();
        }
        
        /// <summary>
        /// Debug method to track when this GameObject gets deactivated
        /// </summary>
        private void OnDisable()
        {
            Debug.LogWarning($"[CaptainIDCard] GameObject '{gameObject.name}' was deactivated! Stack trace: {UnityEngine.StackTraceUtility.ExtractStackTrace()}");
        }
        
        /// <summary>
        /// Try to generate a new encounter when panels are empty
        /// </summary>
        private void TryGenerateNewEncounter()
        {
            Debug.Log("[CaptainIDCard] Attempting to generate new encounter due to empty panels");
            
            // Try various methods to generate a new encounter
            
            // Method 1: Try using GameManager
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                Debug.Log("[CaptainIDCard] Found GameManager, requesting new encounter");
                StartCoroutine(GenerateWithDelay(() => gameManager.RequestNextEncounter("CaptainIDCard")));
                return;
            }
            
            // Method 2: Try using EncounterFlowManager
            EncounterFlowManager flowManager = FindFirstObjectByType<EncounterFlowManager>();
            if (flowManager != null)
            {
                Debug.Log("[CaptainIDCard] Found EncounterFlowManager, requesting new encounter");
                StartCoroutine(GenerateWithDelay(() => flowManager.RequestNextEncounter("CaptainIDCard")));
                return;
            }
            
            // Method 3: Try using MasterShipGenerator directly
            MasterShipGenerator masterGenerator = FindFirstObjectByType<MasterShipGenerator>();
            CredentialChecker credChecker = FindFirstObjectByType<CredentialChecker>();
            if (masterGenerator != null && credChecker != null)
            {
                Debug.Log("[CaptainIDCard] Found MasterShipGenerator, generating encounter directly");
                StartCoroutine(GenerateWithDelay(() => 
                {
                    var newEncounter = masterGenerator.GetNextEncounter();
                    if (newEncounter != null)
                    {
                        credChecker.DisplayEncounter(newEncounter);
                    }
                }));
                return;
            }
            
            // Method 4: Try using EncounterSystemQuickFix
            EncounterSystemQuickFix quickFix = FindFirstObjectByType<EncounterSystemQuickFix>();
            if (quickFix != null)
            {
                Debug.Log("[CaptainIDCard] Found EncounterSystemQuickFix, forcing new encounter");
                StartCoroutine(GenerateWithDelay(() => quickFix.ForceNewEncounter()));
                return;
            }
            
            Debug.LogWarning("[CaptainIDCard] Could not find any system to generate new encounter");
        }
        
        /// <summary>
        /// Coroutine to add a small delay before generating encounter
        /// </summary>
        private IEnumerator GenerateWithDelay(System.Action generateAction)
        {
            yield return new WaitForSeconds(delayBeforeAutoGenerate);
            generateAction?.Invoke();
        }
    }
}