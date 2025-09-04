using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;

/// <summary>
/// Manages all draggable panels in the game, allowing reset, minimize, and chaos features
/// </summary>
public class DraggablePanelManager : MonoBehaviour
{
    [System.Serializable]
    public class PanelInfo
    {
        public string panelName;
        public GameObject panelObject;
        public Vector2 defaultPosition;
        public Vector2 defaultSize;
        public bool startActive = true;
        public bool canClose = true;
        public bool canMinimize = true;
        public float annoyanceFactor = 1.0f; // How much this panel contributes to screen clutter
        
        [HideInInspector] public RectTransform rectTransform;
        [HideInInspector] public bool isMinimized = false;
        [HideInInspector] public Vector2 lastPosition;
        [HideInInspector] public CanvasGroup canvasGroup;
    }
    
    [Header("Panel Management")]
    [SerializeField] private List<PanelInfo> managedPanels = new List<PanelInfo>();
    [SerializeField] private Button resetAllButton;
    [SerializeField] private Button minimizeAllButton;
    [SerializeField] private Button cascadeWindowsButton;
    
    [Header("Minimized Panel Display")]
    [SerializeField] private GameObject minimizedPanelContainer;
    [SerializeField] private GameObject minimizedPanelPrefab; // Small button prefab
    [SerializeField] private float minimizedPanelSpacing = 110f;
    
    [Header("Chaos Settings")]
    [SerializeField] private bool enableRandomPopups = true;
    [SerializeField] private float minTimeBetweenPopups = 30f;
    [SerializeField] private float maxTimeBetweenPopups = 120f;
    [SerializeField] private List<GameObject> annoyingPopupPrefabs; // Ads, notifications, etc.
    
    [Header("Stress Mechanics")]
    [SerializeField] private float maxClutterBeforePenalty = 5f;
    [SerializeField] private TMP_Text clutterWarningText;
    [SerializeField] private Image stressIndicator;
    [SerializeField] private Gradient stressGradient; // Green to Red
    
    [Header("Effects")]
    [SerializeField] private float panelFadeSpeed = 5f;
    [SerializeField] private float cascadeOffset = 30f;
    [SerializeField] private AudioClip popupSound;
    [SerializeField] private AudioClip minimizeSound;
    [SerializeField] private AudioClip errorSound;
    
    // Runtime variables
    private Dictionary<GameObject, PanelInfo> panelLookup = new Dictionary<GameObject, PanelInfo>();
    private List<GameObject> activeAnnoyances = new List<GameObject>();
    private float nextPopupTime;
    private float currentClutterLevel = 0f;
    private AudioSource audioSource;
    
    // Singleton
    private static DraggablePanelManager _instance;
    public static DraggablePanelManager Instance => _instance;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        
        // Ensure this GameObject has proper transform type for its usage
        // If this is on a UI element, it needs RectTransform
        if (transform.parent != null && transform.parent.GetComponent<Canvas>() != null)
        {
            if (GetComponent<RectTransform>() == null)
            {
                Debug.LogWarning("[DraggablePanelManager] Manager is on a UI element but missing RectTransform. This should be on a regular GameObject, not a UI element.");
            }
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    void Start()
    {
        InitializePanels();
        SetupButtons();
        
        if (enableRandomPopups)
        {
            ScheduleNextPopup();
        }
    }
    
    void Update()
    {
        // Handle random popups
        if (enableRandomPopups && Time.time >= nextPopupTime)
        {
            SpawnAnnoyingPopup();
            ScheduleNextPopup();
        }
        
        // Update clutter level
        UpdateClutterLevel();
    }
    
    /// <summary>
    /// Initialize all managed panels
    /// </summary>
    private void InitializePanels()
    {
        foreach (var panel in managedPanels)
        {
            if (panel.panelObject == null) continue;
            
            // Cache components
            panel.rectTransform = panel.panelObject.GetComponent<RectTransform>();
            panel.canvasGroup = panel.panelObject.GetComponent<CanvasGroup>();
            if (panel.canvasGroup == null)
            {
                panel.canvasGroup = panel.panelObject.AddComponent<CanvasGroup>();
            }
            
            // Store default position if not set
            if (panel.defaultPosition == Vector2.zero)
            {
                panel.defaultPosition = panel.rectTransform.anchoredPosition;
            }
            
            if (panel.defaultSize == Vector2.zero)
            {
                panel.defaultSize = panel.rectTransform.sizeDelta;
            }
            
            // Add drag component if missing
            if (!panel.panelObject.GetComponent<UltraSimpleDrag>())
            {
                panel.panelObject.AddComponent<UltraSimpleDrag>();
            }
            
            // Setup close button if exists
            SetupPanelButtons(panel);
            
            // Set initial state
            panel.panelObject.SetActive(panel.startActive);
            
            // Add to lookup
            panelLookup[panel.panelObject] = panel;
        }
    }
    
    /// <summary>
    /// Setup close and minimize buttons on panels
    /// </summary>
    private void SetupPanelButtons(PanelInfo panel)
    {
        // Look for close button
        Transform closeBtn = panel.panelObject.transform.Find("TitleBar/CloseButton");
        if (closeBtn == null) closeBtn = panel.panelObject.transform.Find("CloseButton");
        
        if (closeBtn != null && panel.canClose)
        {
            Button btn = closeBtn.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => ClosePanel(panel.panelObject));
            }
        }
        
        // Look for minimize button
        Transform minBtn = panel.panelObject.transform.Find("TitleBar/MinimizeButton");
        if (minBtn == null) minBtn = panel.panelObject.transform.Find("MinimizeButton");
        
        if (minBtn != null && panel.canMinimize)
        {
            Button btn = minBtn.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => MinimizePanel(panel.panelObject));
            }
        }
    }
    
    /// <summary>
    /// Setup manager buttons
    /// </summary>
    private void SetupButtons()
    {
        if (resetAllButton != null)
        {
            resetAllButton.onClick.AddListener(ResetAllPanels);
        }
        
        if (minimizeAllButton != null)
        {
            minimizeAllButton.onClick.AddListener(MinimizeAllPanels);
        }
        
        if (cascadeWindowsButton != null)
        {
            cascadeWindowsButton.onClick.AddListener(CascadeWindows);
        }
    }
    
    /// <summary>
    /// Reset all panels to default positions
    /// </summary>
    public void ResetAllPanels()
    {
        foreach (var panel in managedPanels)
        {
            if (panel.panelObject == null) continue;
            
            // Restore from minimized if needed
            if (panel.isMinimized)
            {
                RestorePanel(panel.panelObject);
            }
            
            // Reset position and size
            panel.rectTransform.anchoredPosition = panel.defaultPosition;
            panel.rectTransform.sizeDelta = panel.defaultSize;
            
            // Make visible
            panel.panelObject.SetActive(true);
            panel.canvasGroup.alpha = 1f;
        }
        
        // Clear annoyances
        ClearAllAnnoyances();
        
        PlaySound(minimizeSound);
    }
    
    /// <summary>
    /// Minimize all panels
    /// </summary>
    public void MinimizeAllPanels()
    {
        foreach (var panel in managedPanels)
        {
            if (panel.panelObject != null && panel.panelObject.activeSelf && panel.canMinimize)
            {
                MinimizePanel(panel.panelObject);
            }
        }
    }
    
    /// <summary>
    /// Arrange windows in cascade pattern
    /// </summary>
    public void CascadeWindows()
    {
        float currentX = 100f;
        float currentY = 100f;
        int count = 0;
        
        foreach (var panel in managedPanels.Where(p => p.panelObject != null && p.panelObject.activeSelf))
        {
            if (panel.isMinimized) continue;
            
            panel.rectTransform.anchoredPosition = new Vector2(currentX, -currentY);
            panel.panelObject.transform.SetAsLastSibling();
            
            currentX += cascadeOffset;
            currentY += cascadeOffset;
            count++;
            
            // Wrap around if getting too far
            if (count % 5 == 0)
            {
                currentX = 100f;
                currentY = 100f;
            }
        }
        
        PlaySound(minimizeSound);
    }
    
    /// <summary>
    /// Close a specific panel
    /// </summary>
    public void ClosePanel(GameObject panel)
    {
        if (panelLookup.TryGetValue(panel, out PanelInfo info))
        {
            if (!info.canClose)
            {
                // Shake the panel to indicate it can't be closed
                StartCoroutine(ShakePanel(panel));
                PlaySound(errorSound);
                return;
            }
            
            panel.SetActive(false);
            PlaySound(minimizeSound);
        }
    }
    
    /// <summary>
    /// Minimize a specific panel
    /// </summary>
    public void MinimizePanel(GameObject panel)
    {
        if (panelLookup.TryGetValue(panel, out PanelInfo info))
        {
            if (!info.canMinimize)
            {
                StartCoroutine(ShakePanel(panel));
                PlaySound(errorSound);
                return;
            }
            
            info.isMinimized = true;
            info.lastPosition = info.rectTransform.anchoredPosition;
            panel.SetActive(false);
            
            // Create minimized icon
            CreateMinimizedIcon(info);
            
            PlaySound(minimizeSound);
        }
    }
    
    /// <summary>
    /// Restore a minimized panel
    /// </summary>
    public void RestorePanel(GameObject panel)
    {
        if (panelLookup.TryGetValue(panel, out PanelInfo info))
        {
            info.isMinimized = false;
            panel.SetActive(true);
            info.rectTransform.anchoredPosition = info.lastPosition;
            panel.transform.SetAsLastSibling();
            
            // Remove minimized icon
            RemoveMinimizedIcon(info);
            
            PlaySound(popupSound);
        }
    }
    
    /// <summary>
    /// Create a minimized icon in the taskbar
    /// </summary>
    private void CreateMinimizedIcon(PanelInfo info)
    {
        if (minimizedPanelContainer == null || minimizedPanelPrefab == null) return;
        
        GameObject icon = Instantiate(minimizedPanelPrefab, minimizedPanelContainer.transform);
        icon.name = info.panelName + "_Minimized";
        
        // Set text
        TMP_Text text = icon.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = info.panelName;
        }
        
        // Set click handler
        Button btn = icon.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => RestorePanel(info.panelObject));
        }
        
        // Position icon
        RectTransform rt = icon.GetComponent<RectTransform>();
        int index = minimizedPanelContainer.transform.childCount - 1;
        rt.anchoredPosition = new Vector2(index * minimizedPanelSpacing, 0);
    }
    
    /// <summary>
    /// Remove minimized icon
    /// </summary>
    private void RemoveMinimizedIcon(PanelInfo info)
    {
        if (minimizedPanelContainer == null) return;
        
        Transform icon = minimizedPanelContainer.transform.Find(info.panelName + "_Minimized");
        if (icon != null)
        {
            Destroy(icon.gameObject);
            
            // Reposition remaining icons
            for (int i = 0; i < minimizedPanelContainer.transform.childCount; i++)
            {
                RectTransform rt = minimizedPanelContainer.transform.GetChild(i).GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(i * minimizedPanelSpacing, 0);
            }
        }
    }
    
    /// <summary>
    /// Spawn an annoying popup to increase workplace stress
    /// </summary>
    private void SpawnAnnoyingPopup()
    {
        if (annoyingPopupPrefabs == null || annoyingPopupPrefabs.Count == 0) return;
        
        // Choose random popup
        GameObject prefab = annoyingPopupPrefabs[Random.Range(0, annoyingPopupPrefabs.Count)];
        
        // Instantiate popup - ensure it's created as a UI element
        GameObject popup = Instantiate(prefab, transform);
        
        // Ensure it has RectTransform (UI elements should have this automatically)
        RectTransform rt = popup.GetComponent<RectTransform>();
        if (rt == null)
        {
            rt = popup.AddComponent<RectTransform>();
        }
        
        // Random position
        rt.anchoredPosition = new Vector2(
            Random.Range(-300f, 300f),
            Random.Range(-200f, 200f)
        );
        
        // Ensure proper UI setup
        rt.localScale = Vector3.one;
        
        // Add drag component
        if (!popup.GetComponent<UltraSimpleDrag>())
        {
            popup.AddComponent<UltraSimpleDrag>();
        }
        
        // Track it
        activeAnnoyances.Add(popup);
        
        // Auto-close after random time
        float lifetime = Random.Range(30f, 120f);
        StartCoroutine(RemovePopupAfterDelay(popup, lifetime));
        
        PlaySound(popupSound);
        
        // Add close button functionality if it exists
        Button closeBtn = popup.GetComponentInChildren<Button>();
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(() => {
                activeAnnoyances.Remove(popup);
                Destroy(popup);
            });
        }
    }
    
    /// <summary>
    /// Remove popup after delay
    /// </summary>
    private IEnumerator RemovePopupAfterDelay(GameObject popup, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (popup != null)
        {
            activeAnnoyances.Remove(popup);
            Destroy(popup);
        }
    }
    
    /// <summary>
    /// Update the current clutter level based on active panels
    /// </summary>
    private void UpdateClutterLevel()
    {
        currentClutterLevel = 0f;
        
        // Count active panels
        foreach (var panel in managedPanels)
        {
            if (panel.panelObject != null && panel.panelObject.activeSelf && !panel.isMinimized)
            {
                currentClutterLevel += panel.annoyanceFactor;
            }
        }
        
        // Add annoyances
        currentClutterLevel += activeAnnoyances.Count * 1.5f;
        
        // Update UI
        if (stressIndicator != null)
        {
            float stressPercent = Mathf.Clamp01(currentClutterLevel / maxClutterBeforePenalty);
            stressIndicator.color = stressGradient.Evaluate(stressPercent);
            stressIndicator.fillAmount = stressPercent;
        }
        
        if (clutterWarningText != null)
        {
            if (currentClutterLevel > maxClutterBeforePenalty)
            {
                clutterWarningText.text = "WORKSPACE TOO CLUTTERED!";
                clutterWarningText.color = Color.red;
            }
            else if (currentClutterLevel > maxClutterBeforePenalty * 0.7f)
            {
                clutterWarningText.text = "Workspace getting cluttered...";
                clutterWarningText.color = Color.yellow;
            }
            else
            {
                clutterWarningText.text = "";
            }
        }
    }
    
    /// <summary>
    /// Get current stress/clutter level for gameplay impact
    /// </summary>
    public float GetStressLevel()
    {
        return Mathf.Clamp01(currentClutterLevel / maxClutterBeforePenalty);
    }
    
    /// <summary>
    /// Check if workspace is too cluttered
    /// </summary>
    public bool IsWorkspaceCluttered()
    {
        return currentClutterLevel > maxClutterBeforePenalty;
    }
    
    /// <summary>
    /// Clear all annoying popups
    /// </summary>
    public void ClearAllAnnoyances()
    {
        foreach (var annoyance in activeAnnoyances)
        {
            if (annoyance != null)
                Destroy(annoyance);
        }
        activeAnnoyances.Clear();
    }
    
    /// <summary>
    /// Schedule next random popup
    /// </summary>
    private void ScheduleNextPopup()
    {
        nextPopupTime = Time.time + Random.Range(minTimeBetweenPopups, maxTimeBetweenPopups);
    }
    
    /// <summary>
    /// Shake panel effect
    /// </summary>
    private System.Collections.IEnumerator ShakePanel(GameObject panel)
    {
        RectTransform rt = panel.GetComponent<RectTransform>();
        Vector3 originalPos = rt.localPosition;
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-5f, 5f);
            float y = Random.Range(-5f, 5f);
            rt.localPosition = originalPos + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        rt.localPosition = originalPos;
    }
    
    /// <summary>
    /// Play sound effect
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    /// <summary>
    /// Open a specific panel by name
    /// </summary>
    public void OpenPanel(string panelName)
    {
        var panel = managedPanels.FirstOrDefault(p => p.panelName == panelName);
        if (panel != null && panel.panelObject != null)
        {
            if (panel.isMinimized)
            {
                RestorePanel(panel.panelObject);
            }
            else
            {
                panel.panelObject.SetActive(true);
                panel.panelObject.transform.SetAsLastSibling();
                
                // Ensure all children are also active
                SetChildrenActive(panel.panelObject.transform, true);
            }
        }
    }
    
    /// <summary>
    /// Recursively set active state for all children
    /// </summary>
    private void SetChildrenActive(Transform parent, bool active)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(active);
            // Recursively activate children of children
            SetChildrenActive(child, active);
        }
    }
    
    /// <summary>
    /// Register a new panel at runtime
    /// </summary>
    public void RegisterPanel(GameObject panel, string name, Vector2 defaultPos, float annoyance = 1f)
    {
        PanelInfo info = new PanelInfo
        {
            panelName = name,
            panelObject = panel,
            defaultPosition = defaultPos,
            annoyanceFactor = annoyance,
            rectTransform = panel.GetComponent<RectTransform>(),
            canvasGroup = panel.GetComponent<CanvasGroup>()
        };
        
        if (info.canvasGroup == null)
        {
            info.canvasGroup = panel.AddComponent<CanvasGroup>();
        }
        
        managedPanels.Add(info);
        panelLookup[panel] = info;
        
        // Add drag component
        if (!panel.GetComponent<UltraSimpleDrag>())
        {
            panel.AddComponent<UltraSimpleDrag>();
        }
    }
}