using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Starkiller.Core.Managers;
using Starkiller.Core;

namespace Starkiller.UI
{
    /// <summary>
    /// UI component for displaying the upcoming ship queue
    /// Shows a preview of ships waiting to be processed
    /// </summary>
    public class ShipQueueUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform shipQueueContainer; // Parent for ship queue items
        [SerializeField] private GameObject shipQueueItemPrefab; // Prefab for each ship in queue
        [SerializeField] private int maxDisplayedShips = 5; // Maximum ships to show
        
        [Header("Visual Settings")]
        [SerializeField] private Color normalShipColor = Color.green;
        [SerializeField] private Color suspiciousShipColor = new Color(1f, 0.5f, 0f); // Orange
        [SerializeField] private Color specialShipColor = Color.cyan;
        [SerializeField] private float updateInterval = 0.5f; // How often to refresh the display
        
        [Header("Animation")]
        [SerializeField] private float itemSpacing = 80f; // Spacing between queue items
        [SerializeField] private float animationDuration = 0.3f; // Animation time for transitions
        
        // Component references
        private EncounterManager _encounterManager;
        private List<GameObject> _queueItemPool = new List<GameObject>();
        private float _lastUpdateTime;
        
        private void Awake()
        {
            // Get reference to EncounterManager
            _encounterManager = ServiceLocator.Get<EncounterManager>();
            
            if (_encounterManager == null)
            {
                Debug.LogError("[ShipQueueUI] EncounterManager not found!");
                enabled = false;
                return;
            }
            
            // Pre-create pool of UI items
            CreateItemPool();
        }
        
        private void OnEnable()
        {
            // Subscribe to events
            if (_encounterManager != null)
            {
                EncounterManager.OnQueueSizeChanged += OnQueueChanged;
                EncounterManager.OnEncounterStarted += OnEncounterStarted;
            }
            
            // Initial update
            UpdateQueueDisplay();
        }
        
        private void OnDisable()
        {
            // Unsubscribe from events
            if (_encounterManager != null)
            {
                EncounterManager.OnQueueSizeChanged -= OnQueueChanged;
                EncounterManager.OnEncounterStarted -= OnEncounterStarted;
            }
        }
        
        private void Update()
        {
            // Periodic refresh to keep animations smooth
            if (Time.time - _lastUpdateTime > updateInterval)
            {
                UpdateQueueDisplay();
                _lastUpdateTime = Time.time;
            }
        }
        
        /// <summary>
        /// Create a pool of reusable UI items
        /// </summary>
        private void CreateItemPool()
        {
            if (shipQueueItemPrefab == null || shipQueueContainer == null)
            {
                Debug.LogError("[ShipQueueUI] Missing prefab or container reference!");
                return;
            }
            
            for (int i = 0; i < maxDisplayedShips + 2; i++)
            {
                GameObject item = Instantiate(shipQueueItemPrefab, shipQueueContainer);
                item.SetActive(false);
                _queueItemPool.Add(item);
            }
        }
        
        /// <summary>
        /// Update the visual display of the queue
        /// </summary>
        private void UpdateQueueDisplay()
        {
            if (_encounterManager == null) return;
            
            // Get upcoming encounters
            var upcomingEncounters = _encounterManager.GetUpcomingEncounters(maxDisplayedShips);
            
            // Hide all items first
            foreach (var item in _queueItemPool)
            {
                item.SetActive(false);
            }
            
            // Display each encounter
            for (int i = 0; i < upcomingEncounters.Count && i < _queueItemPool.Count; i++)
            {
                var encounter = upcomingEncounters[i];
                var item = _queueItemPool[i];
                
                // Activate and configure the item
                item.SetActive(true);
                ConfigureQueueItem(item, encounter, i);
                
                // Animate position
                AnimateItemPosition(item, i);
            }
        }
        
        /// <summary>
        /// Configure a queue item with encounter data
        /// </summary>
        private void ConfigureQueueItem(GameObject item, EncounterData encounter, int index)
        {
            // Get components
            var shipNameText = item.GetComponentInChildren<TMP_Text>();
            var backgroundImage = item.GetComponent<Image>();
            var warningIcon = item.transform.Find("WarningIcon");
            var etaText = item.transform.Find("ETA")?.GetComponent<TMP_Text>();
            
            // Set ship name
            if (shipNameText != null)
            {
                shipNameText.text = encounter.ShipType;
            }
            
            // Set color based on type
            if (backgroundImage != null)
            {
                Color targetColor = normalShipColor;
                
                switch (encounter.Type)
                {
                    case EncounterType.Suspicious:
                        targetColor = suspiciousShipColor;
                        break;
                    case EncounterType.SpecialEvent:
                        targetColor = specialShipColor;
                        break;
                }
                
                backgroundImage.color = targetColor;
            }
            
            // Show warning icon for suspicious ships
            if (warningIcon != null)
            {
                warningIcon.gameObject.SetActive(encounter.Type == EncounterType.Suspicious);
            }
            
            // Calculate and show ETA
            if (etaText != null)
            {
                // Use the base encounter interval from settings
                float baseInterval = 15f; // Default fallback
                
                // If you add the public property to EncounterManager, uncomment this:
                // baseInterval = _encounterManager.BaseEncounterInterval;
                
                float eta = (index + 1) * baseInterval;
                etaText.text = FormatETA(eta);
            }
            
            // Add hover effect component if not present
            if (item.GetComponent<ShipQueueItemHover>() == null)
            {
                item.AddComponent<ShipQueueItemHover>();
            }
        }
        
        /// <summary>
        /// Animate item to its position in the queue
        /// </summary>
        private void AnimateItemPosition(GameObject item, int index)
        {
            RectTransform rect = item.GetComponent<RectTransform>();
            if (rect == null) return;
            
            // Target position
            Vector2 targetPos = new Vector2(0, -index * itemSpacing);
            
            // Smooth animation
            if (Application.isPlaying)
            {
                rect.anchoredPosition = Vector2.Lerp(
                    rect.anchoredPosition, 
                    targetPos, 
                    Time.deltaTime / animationDuration
                );
            }
            else
            {
                rect.anchoredPosition = targetPos;
            }
        }
        
        /// <summary>
        /// Format time as ETA string
        /// </summary>
        private string FormatETA(float seconds)
        {
            if (seconds < 60)
            {
                return $"ETA: {Mathf.RoundToInt(seconds)}s";
            }
            else
            {
                int minutes = Mathf.FloorToInt(seconds / 60);
                int secs = Mathf.RoundToInt(seconds % 60);
                return $"ETA: {minutes}:{secs:00}";
            }
        }
        
        // Event handlers
        private void OnQueueChanged(int newSize)
        {
            UpdateQueueDisplay();
        }
        
        private void OnEncounterStarted(IEncounter encounter)
        {
            // Refresh display when a new encounter starts
            UpdateQueueDisplay();
        }
    }
    
    /// <summary>
    /// Simple hover effect for queue items
    /// </summary>
    public class ShipQueueItemHover : MonoBehaviour
    {
        private RectTransform _rect;
        private Vector2 _originalScale;
        private bool _isHovered;
        
        private void Start()
        {
            _rect = GetComponent<RectTransform>();
            _originalScale = _rect.localScale;
        }
        
        private void OnEnable()
        {
            // Reset scale
            if (_rect != null)
            {
                _rect.localScale = _originalScale;
            }
        }
        
        public void OnPointerEnter()
        {
            _isHovered = true;
        }
        
        public void OnPointerExit()
        {
            _isHovered = false;
        }
        
        private void Update()
        {
            if (_rect == null) return;
            
            // Scale effect on hover
            Vector2 targetScale = _isHovered ? _originalScale * 1.05f : _originalScale;
            _rect.localScale = Vector2.Lerp(_rect.localScale, targetScale, Time.deltaTime * 10f);
        }
    }
}